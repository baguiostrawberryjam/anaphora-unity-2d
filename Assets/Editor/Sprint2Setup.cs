using System;
using System.IO;
using System.Linq;
using Anaphora.HousePrototype;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Sprint2Setup
{
    private const string ScenePath = "Assets/Anaphora3D/HousePrototype/Scenes/HouseExploration.unity";

    public static void ApplyAndValidate()
    {
        try
        {
            if (!Path.GetFullPath(Application.dataPath).Equals(Path.GetFullPath("C:/Users/Libay/Anaphora/anaphora-unity-3d/Assets"), StringComparison.OrdinalIgnoreCase))
                throw new IOException("Sprint 2 setup is restricted to the requested 3D project.");
            Directory.CreateDirectory("Logs");
            File.Copy(ScenePath, "Logs/HouseBeforeSprint2-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + ".unity-backup");
            string sceneGuid = AssetDatabase.AssetPathToGUID(ScenePath);
            EditorSceneManager.OpenScene(ScenePath);
            HouseSession session = UnityEngine.Object.FindAnyObjectByType<HouseSession>();
            if (session == null || session.Sanity == null) throw new Exception("Combined house and sanity scene is required.");
            session.Sanity.StartsUnlocked = false;
            HousePicture picture = UnityEngine.Object.FindAnyObjectByType<HousePicture>();
            if (picture == null)
            {
                GameObject frame = GameObject.Find("Living - picture placeholder");
                if (frame == null) throw new Exception("The Living Room picture placeholder is missing; preserve the house and select its replacement before setup.");
                picture = frame.AddComponent<HousePicture>();
            }
            picture.Sanity = session.Sanity;
            session.FamilyPicture = picture;
            if (picture.transform.Find("Picture Details") == null) AddDetails(picture.transform);
            EditorSceneManager.MarkSceneDirty(session.gameObject.scene);
            EditorSceneManager.SaveScene(session.gameObject.scene);
            AssetDatabase.SaveAssets();
            if (AssetDatabase.AssetPathToGUID(ScenePath) != sceneGuid) throw new Exception("House scene GUID changed.");
            Anaphora.HousePrototype.Editor.HouseSceneBuilder.BuildAndValidate();
            Debug.Log("SPRINT2_SETUP_SUCCESS: picture interaction added to the existing house.");
            Sprint2Checks.Begin();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else throw;
        }
    }

    private static void AddDetails(Transform frame)
    {
        Material material = frame.GetComponent<Renderer>().sharedMaterial;
        Transform details = new GameObject("Picture Details").transform;
        details.SetParent(frame, false);
        Color dark = new Color(0.12f, 0.1f, 0.09f);
        Part(details, material, "Top frame", PrimitiveType.Cube, new Vector3(0.6f, 0.48f, 0), new Vector3(0.2f, 0.09f, 1.05f), dark);
        Part(details, material, "Bottom frame", PrimitiveType.Cube, new Vector3(0.6f, -0.48f, 0), new Vector3(0.2f, 0.09f, 1.05f), dark);
        Part(details, material, "Left frame", PrimitiveType.Cube, new Vector3(0.6f, 0, -0.48f), new Vector3(0.2f, 0.9f, 0.08f), dark);
        Part(details, material, "Right frame", PrimitiveType.Cube, new Vector3(0.6f, 0, 0.48f), new Vector3(0.2f, 0.9f, 0.08f), dark);
        var backing = frame.GetComponent<HouseSurface>();
        backing.Tint = new Color(0.65f, 0.62f, 0.53f); backing.Apply();
        for (int i = 0; i < 3; i++)
        {
            float z = (i - 1) * 0.26f;
            float small = i == 1 ? 0.75f : 1;
            Part(details, material, "Family silhouette head " + i, PrimitiveType.Sphere,
                new Vector3(0.66f, 0.15f * small, z), new Vector3(0.12f, 0.2f * small, 0.14f), dark);
            Part(details, material, "Family silhouette body " + i, PrimitiveType.Cube,
                new Vector3(0.65f, -0.15f, z), new Vector3(0.1f, 0.32f * small, 0.18f), dark);
        }
        GameObject crackA = Part(details, material, "Cracked glass upper", PrimitiveType.Cube,
            new Vector3(0.82f, 0.22f, 0.05f), new Vector3(0.08f, 0.5f, 0.02f), Color.black);
        crackA.transform.localRotation = Quaternion.Euler(35, 0, 0);
        GameObject crackB = Part(details, material, "Cracked glass lower", PrimitiveType.Cube,
            new Vector3(0.82f, -0.2f, 0.04f), new Vector3(0.08f, 0.5f, 0.02f), Color.black);
        crackB.transform.localRotation = Quaternion.Euler(-30, 0, 0);
    }

    private static GameObject Part(Transform parent, Material material, string label, PrimitiveType primitive, Vector3 position, Vector3 scale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = label; part.transform.SetParent(parent, false);
        part.transform.localPosition = position; part.transform.localScale = scale;
        part.GetComponent<Renderer>().sharedMaterial = material;
        HouseSurface surface = part.AddComponent<HouseSurface>(); surface.Tint = color; surface.Apply();
        return part;
    }
}

[InitializeOnLoad]
public static class Sprint2Checks
{
    private const string StageKey = "Anaphora.Sprint2Checks.Stage";
    private static int frames;
    static Sprint2Checks()
    {
        EditorApplication.playModeStateChanged += Changed;
        if (SessionState.GetInt(StageKey, 0) == 1 && !EditorApplication.isPlayingOrWillChangePlaymode)
            EditorApplication.delayCall += Enter;
    }

    public static void Begin() { SessionState.SetInt(StageKey, 1); EditorApplication.delayCall += Enter; }
    private static void Enter() { if (!EditorApplication.isPlayingOrWillChangePlaymode) EditorApplication.isPlaying = true; }
    private static void Changed(PlayModeStateChange change)
    {
        if (SessionState.GetInt(StageKey, 0) == 0) return;
        if (change == PlayModeStateChange.EnteredPlayMode) { frames = 0; EditorApplication.update += Run; }
        if (change == PlayModeStateChange.EnteredEditMode && SessionState.GetInt(StageKey, 0) == 2)
        {
            SessionState.SetInt(StageKey, 0);
            Debug.Log("SPRINT2_SUCCESS: Play Mode story unlock and regression checks passed.");
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }
    }

    private static void Check(bool value, string label)
    {
        if (!value) throw new Exception("Sprint 2 check failed: " + label);
        Debug.Log("SPRINT2_CHECK: " + label);
    }

    private static void Run()
    {
        if (++frames < 8) return;
        EditorApplication.update -= Run;
        try
        {
            HouseSession session = UnityEngine.Object.FindAnyObjectByType<HouseSession>();
            var sanity = session.Sanity;
            HousePicture picture = session.FamilyPicture;
            var integration = session.GetComponent<HouseSanityIntegration>();
            Check(!sanity.SwitchingUnlocked && !picture.HasBeenExamined && !sanity.State.IsReality, "Start in Illusion with shifting locked");
            session.Player.Capture(true);
            integration.SendMessage("Update");
            sanity.Shift();
            Check(!sanity.State.IsReality && !sanity.SwitchingUnlocked, "Manual shift cannot bypass picture discovery");
            var actor = session.Player.GetComponent<CharacterController>();
            actor.enabled = false;
            session.Player.transform.SetPositionAndRotation(new Vector3(-4.8f, 0.1f, -7), Quaternion.Euler(0, -90, 0));
            actor.enabled = true;
            Physics.SyncTransforms();
            bool found = Physics.Raycast(session.View.transform.position, session.View.transform.forward, out RaycastHit hit,
                session.InteractionDistance, ~0, QueryTriggerInteraction.Ignore);
            Check(found && ReferenceEquals(HouseInteraction.Find(hit.collider), picture), "Picture is reachable with the real interaction raycast");
            sanity.Paused = true;
            picture.Interact();
            Check(!picture.HasBeenExamined && !sanity.SwitchingUnlocked, "Cannot inspect picture while paused");
            sanity.Paused = false;
            Vector3 position = session.Player.transform.position;
            HouseInteraction.Find(hit.collider).Interact();
            Check(picture.HasBeenExamined && sanity.SwitchingUnlocked && sanity.State.IsReality, "E on picture unlocks and immediately enters Reality");
            Check(session.Player.transform.position == position, "First shift keeps the player in place");
            sanity.PsychologicalHit(30);
            float remaining = sanity.State.Value;
            picture.Interact();
            Check(sanity.State.IsReality && sanity.State.Value == remaining, "Repeat inspection does not reset sanity or trigger another shift");
            HouseDoor door = session.Doors[0];
            Check(ReferenceEquals(HouseInteraction.Find(door.LeafCollider), door), "Doors use the same interaction system");
            door.SetOpenImmediately(true);
            integration.PressureTestRoom.Visited = true;
            sanity.State.Tick(1, false);
            sanity.Shift();
            Check(!sanity.State.IsReality && sanity.SwitchingUnlocked, "Manual return works after discovery");
            sanity.State.Tick(1, false);
            sanity.Shift();
            Check(sanity.State.IsReality, "Manual re-entry works after discovery");
            sanity.PsychologicalHit(200);
            Check(!sanity.State.IsReality && sanity.SwitchingUnlocked && picture.HasBeenExamined && door.IsOpen &&
                integration.PressureTestRoom.Visited && session.Player.transform.position == position,
                "Forced return preserves unlock, picture discovery, doors, location and exploration");
            session.SendMessage("ResetWalkthrough");
            Check(!sanity.SwitchingUnlocked && !picture.HasBeenExamined && !sanity.State.IsReality && sanity.State.Value == 100,
                "R resets discovery, unlock and sanity together");
            picture.Interact();
            Check(picture.HasBeenExamined && sanity.State.IsReality, "Discovery can be replayed after reset");
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
