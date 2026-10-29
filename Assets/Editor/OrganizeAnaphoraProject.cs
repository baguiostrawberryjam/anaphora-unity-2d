using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Anaphora.HousePrototype;
using Anaphora.SanitySandbox;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class OrganizeAnaphoraProject
{
    public const string HousePath = "Assets/Anaphora3D/HousePrototype/Scenes/HouseExploration.unity";
    private static readonly string[,] Moves = {
        { "Assets/Game", "Assets/Anaphora2D/Game" },
        { "Assets/Scenes", "Assets/Anaphora2D/Scenes" },
        { "Assets/Branding", "Assets/Anaphora2D/Branding" },
        { "Assets/_Recovery", "Assets/Anaphora2D/Recovery" },
        { "Assets/anaphora logo raw.png", "Assets/Anaphora2D/anaphora logo raw.png" },
        { "Assets/HousePrototype", "Assets/Anaphora3D/HousePrototype" },
        { "Assets/SanitySandbox", "Assets/Anaphora3D/SanitySandbox" }
    };

    public static void ApplyAndValidate()
    {
        try
        {
            string expectedAssets = Path.GetFullPath("C:/Users/Libay/Anaphora/anaphora-unity-3d/Assets");
            if (!Path.GetFullPath(Application.dataPath).Equals(expectedAssets, StringComparison.OrdinalIgnoreCase))
                throw new IOException("This migration is restricted to the requested 3D project.");
            foreach (string relative in Moves)
                if (!Path.GetFullPath(relative).StartsWith(expectedAssets + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                    throw new IOException("Move target is outside the 3D project's Assets folder: " + relative);
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var trackedGuids = new List<string>();
            var sceneGuids = EditorBuildSettings.scenes.Select(scene =>
                new KeyValuePair<string, bool>(AssetDatabase.AssetPathToGUID(scene.path), scene.enabled)).ToArray();
            string originalHouse = File.Exists(HousePath) ? HousePath : "Assets/HousePrototype/Scenes/HouseExploration.unity";
            Directory.CreateDirectory("Logs");
            string backup = "Logs/HouseBeforeSanity-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + ".unity-backup";
            File.Copy(originalHouse, backup, false);
            string houseGuid = AssetDatabase.AssetPathToGUID(originalHouse);
            foreach (string path in AssetDatabase.GetAllAssetPaths())
                if (path.StartsWith("Assets/", StringComparison.Ordinal)) trackedGuids.Add(AssetDatabase.AssetPathToGUID(path));
            foreach (string root in new[] { "Anaphora2D", "Anaphora3D" })
                if (!AssetDatabase.IsValidFolder("Assets/" + root)) AssetDatabase.CreateFolder("Assets", root);

            // Preflight all destinations before moving anything; Unity preserves each asset's GUID.
            for (int i = 0; i < Moves.GetLength(0); i++)
                if (AssetDatabase.AssetPathToGUID(Moves[i, 0]) != "" && AssetDatabase.AssetPathToGUID(Moves[i, 1]) != "")
                    throw new IOException("Both source and destination exist: " + Moves[i, 0]);
            AssetDatabase.StartAssetEditing();
            try
            {
                for (int i = 0; i < Moves.GetLength(0); i++)
                {
                    if (AssetDatabase.AssetPathToGUID(Moves[i, 0]) == "") continue;
                    string error = AssetDatabase.MoveAsset(Moves[i, 0], Moves[i, 1]);
                    if (!string.IsNullOrEmpty(error)) throw new IOException(error);
                    Debug.Log("ORGANIZED: " + Moves[i, 0] + " -> " + Moves[i, 1]);
                }
            }
            finally { AssetDatabase.StopAssetEditing(); }

            EditorBuildSettings.scenes = sceneGuids.Select(scene =>
                new EditorBuildSettingsScene(AssetDatabase.GUIDToAssetPath(scene.Key), scene.Value)).ToArray();
            UpdateText("Tools/SanitySandboxChecks/SanitySandboxChecks.csproj", "Assets/SanitySandbox/", "Assets/Anaphora3D/SanitySandbox/");
            UpdateText("Assets/Anaphora3D/SanitySandbox/README.md", "Assets/SanitySandbox/", "Assets/Anaphora3D/SanitySandbox/");
            EditorSceneManager.OpenScene(HousePath);
            HouseSession session = UnityEngine.Object.FindAnyObjectByType<HouseSession>();
            if (session == null) throw new Exception("House Session is missing.");
            SanityController sanity = session.GetComponent<SanityController>();
            if (sanity == null) sanity = session.gameObject.AddComponent<SanityController>();
            HouseSanityIntegration integration = session.GetComponent<HouseSanityIntegration>();
            if (integration == null) integration = session.gameObject.AddComponent<HouseSanityIntegration>();
            integration.Session = session;
            integration.Sanity = sanity;
            integration.PressureTestRoom = session.Rooms.First(room => room.Label == "Study");
            session.Sanity = sanity;
            EditorSceneManager.MarkSceneDirty(session.gameObject.scene);
            EditorSceneManager.SaveScene(session.gameObject.scene);
            AssetDatabase.SaveAssets();

            foreach (string guid in trackedGuids)
                if (!string.IsNullOrEmpty(guid) && AssetDatabase.GUIDToAssetPath(guid) == "")
                    throw new Exception("Asset GUID lost during organization: " + guid);
            if (AssetDatabase.AssetPathToGUID(HousePath) != houseGuid) throw new Exception("House scene GUID changed.");
            if (UnityEngine.Object.FindObjectsByType<Camera>().Length != 1 ||
                UnityEngine.Object.FindObjectsByType<SanityController>().Length != 1)
                throw new Exception("The combined house must contain exactly one camera and sanity controller.");
            if (session.Doors.Any(door => door == null) || session.Rooms.Any(room => room == null))
                throw new Exception("House references are missing.");
            Anaphora.HousePrototype.Editor.HouseSceneBuilder.BuildAndValidate();
            Debug.Log("ORGANIZATION_SUCCESS: " + trackedGuids.Count + " existing GUIDs preserved, build scene paths updated, combined house saved.");
            HouseIntegrationChecks.Begin();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else throw;
        }
    }

    private static void UpdateText(string path, string before, string after)
    {
        if (!File.Exists(path)) return;
        string source = File.ReadAllText(path);
        string updated = source.Replace(before, after);
        if (source != updated) File.WriteAllText(path, updated);
    }
}

[InitializeOnLoad]
public static class HouseIntegrationChecks
{
    private const string StageKey = "Anaphora.HouseIntegrationChecks.Stage";
    private static int frames;
    static HouseIntegrationChecks()
    {
        EditorApplication.playModeStateChanged += Changed;
        if (SessionState.GetInt(StageKey, 0) == 1 && !EditorApplication.isPlayingOrWillChangePlaymode)
            EditorApplication.delayCall += Enter;
    }

    public static void Begin()
    {
        SessionState.SetInt(StageKey, 1);
        EditorApplication.delayCall += Enter;
    }

    private static void Enter()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode) EditorApplication.isPlaying = true;
    }

    private static void Changed(PlayModeStateChange change)
    {
        if (SessionState.GetInt(StageKey, 0) == 0) return;
        if (change == PlayModeStateChange.EnteredPlayMode)
        {
            frames = 0;
            EditorApplication.update += RunAfterStart;
        }
        if (change == PlayModeStateChange.EnteredEditMode && SessionState.GetInt(StageKey, 0) == 2)
        {
            SessionState.SetInt(StageKey, 0);
            Debug.Log("HOUSE_INTEGRATION_SUCCESS: Play Mode checks passed; ready for a human playtest.");
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception("Integration check failed: " + message);
        Debug.Log("HOUSE_CHECK: " + message);
    }

    private static void RunAfterStart()
    {
        if (++frames < 8) return;
        EditorApplication.update -= RunAfterStart;
        try
        {
            HouseSession session = UnityEngine.Object.FindAnyObjectByType<HouseSession>();
            SanityController sanity = session.Sanity;
            var integration = session.GetComponent<HouseSanityIntegration>();
            Check(sanity != null && sanity.State != null && sanity.Paused && !sanity.State.IsReality, "House starts paused in Illusion");
            var surface = UnityEngine.Object.FindObjectsByType<HouseSurface>()
                .First(item => Mathf.Abs(item.Tint.r - item.Tint.b) > 0.01f);
            var properties = new MaterialPropertyBlock();
            surface.GetComponent<Renderer>().GetPropertyBlock(properties);
            Color illusion = properties.GetColor("_BaseColor");
            Check(Mathf.Abs(illusion.r - illusion.b) < 0.001f, "Illusion applies monochrome surfaces");
            session.Player.Capture(true);
            sanity.Paused = false;
            sanity.Shift();
            surface.GetComponent<Renderer>().GetPropertyBlock(properties);
            Check(sanity.State.IsReality && properties.GetColor("_BaseColor") == surface.Tint, "Q action changes sanity world and house colors together");
            HouseDoor door = session.Doors[0];
            door.SetOpenImmediately(true);
            integration.PressureTestRoom.Visited = true;
            var controller = session.Player.GetComponent<CharacterController>();
            controller.enabled = false;
            session.Player.transform.position = integration.PressureTestRoom.Area.bounds.center - Vector3.up * 1.4f;
            controller.enabled = true;
            integration.SendMessage("Update");
            Check(sanity.Threatened, "Study activates pressure test penalties");
            Vector3 position = session.Player.transform.position;
            Quaternion rotation = door.transform.localRotation;
            sanity.PsychologicalHit(200);
            Check(!sanity.State.IsReality && sanity.State.Value == 0 && sanity.State.ForcedReturns == 1, "Depletion forces return inside house");
            Check(session.Player.transform.position == position && door.transform.localRotation == rotation && integration.PressureTestRoom.Visited,
                "Forced return preserves position, doors and exploration progress");
            sanity.Shift();
            Check(!sanity.State.IsReality, "Immediate re-entry is blocked");
            sanity.State.Tick(3, false);
            sanity.Shift();
            Check(sanity.State.IsReality, "Recovery permits re-entry");
            session.SendMessage("ResetWalkthrough");
            Check(sanity.State.Value == 100 && !sanity.State.IsReality && !door.IsOpen &&
                !integration.PressureTestRoom.Visited && session.Player.transform.position == session.Spawn.position,
                "R action resets both house and sanity");
            session.Player.Capture(false);
            integration.SendMessage("Update");
            float value = sanity.State.Value;
            sanity.SendMessage("Update");
            Check(sanity.Paused && sanity.State.Value == value, "Releasing cursor pauses sanity");
            SessionState.SetInt(StageKey, 2);
            EditorApplication.isPlaying = false;
        }
        catch (Exception exception)
        {
            SessionState.SetInt(StageKey, 0);
            Debug.LogException(exception);
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else EditorApplication.isPlaying = false;
        }
    }
}
