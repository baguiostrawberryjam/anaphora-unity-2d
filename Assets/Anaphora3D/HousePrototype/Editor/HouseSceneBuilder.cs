using System;
using System.Collections.Generic;
using System.IO;
using Anaphora.SanitySandbox;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Anaphora.HousePrototype.Editor
{
    public static class HouseSceneBuilder
    {
        public const string ScenePath = "Assets/Anaphora3D/HousePrototype/Scenes/HouseExploration.unity";
        private static Material surface;
        private static Transform architecture, furniture;
        private static readonly Color Wall = new Color(0.68f, 0.7f, 0.72f);
        private static readonly Color Wood = new Color(0.4f, 0.35f, 0.3f);
        private static readonly List<HouseDoor> doors = new List<HouseDoor>();
        private static readonly List<HouseRoom> rooms = new List<HouseRoom>();

        [MenuItem("Anaphora/Sprint 1/Open House")]
        public static void OpenHouse()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(ScenePath)) { CreateHouse(); return; }
            EditorSceneManager.OpenScene(ScenePath);
        }

        [MenuItem("Anaphora/Sprint 1/Create House (new scene)")]
        public static void CreateHouse()
        {
            if (File.Exists(ScenePath))
            {
                Debug.Log("House scene already exists. Open it using Anaphora > Sprint 1 > Open House. Existing edits were preserved.");
                return;
            }
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Build();
        }

        public static void BuildAndValidate()
        {
            try
            {
                if (!File.Exists(ScenePath)) Build();
                else EditorSceneManager.OpenScene(ScenePath);
                ValidateHouse();
                Debug.Log("SPRINT1_SUCCESS: house scene created and geometry validation passed.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (Application.isBatchMode) EditorApplication.Exit(1);
                else throw;
            }
        }

        private static void Build()
        {
            surface = AssetDatabase.LoadAssetAtPath<Material>("Assets/Anaphora3D/SanitySandbox/Rendering/SandboxSurface.mat");
            RenderPipelineAsset pipeline = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>("Assets/Anaphora3D/SanitySandbox/Rendering/SandboxPipeline.asset");
            if (surface == null || pipeline == null) throw new InvalidOperationException("The tested sandbox rendering assets are missing.");
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            doors.Clear(); rooms.Clear();
            architecture = new GameObject("House - walls and floors").transform;
            furniture = new GameObject("Furniture - basic shape placeholders").transform;
            Transform roof = new GameObject("Ceilings - hide in Scene view to see floor plan").transform;
            Box(architecture, "House foundation", new Vector3(0, -0.2f, 0), new Vector3(14.2f, 0.4f, 18.2f), new Color(0.24f, 0.26f, 0.28f));
            Room("Entrance Hall", new Vector3(0, 1.5f, 0), new Vector3(2.8f, 3, 17.8f), false);
            Room("Living Room", new Vector3(-4.3f, 1.5f, -4.5f), new Vector3(5.1f, 3, 8.7f));
            Room("Study", new Vector3(-4.3f, 1.5f, 4.5f), new Vector3(5.1f, 3, 8.7f));
            Room("Kitchen", new Vector3(4.3f, 1.5f, -5), new Vector3(5.1f, 3, 7.7f));
            Room("Bathroom", new Vector3(4.3f, 1.5f, 0.5f), new Vector3(5.1f, 3, 2.7f));
            Room("Bedroom", new Vector3(4.3f, 1.5f, 5.5f), new Vector3(5.1f, 3, 6.7f));
            Box(architecture, "Living floor", new Vector3(-4.3f, 0.015f, -4.5f), new Vector3(5.2f, 0.02f, 8.8f), new Color(0.38f, 0.36f, 0.33f));
            Box(architecture, "Study floor", new Vector3(-4.3f, 0.015f, 4.5f), new Vector3(5.2f, 0.02f, 8.8f), new Color(0.35f, 0.38f, 0.37f));
            Box(architecture, "Kitchen floor", new Vector3(4.3f, 0.015f, -5), new Vector3(5.2f, 0.02f, 7.8f), new Color(0.43f, 0.43f, 0.42f));
            Box(architecture, "Bathroom floor", new Vector3(4.3f, 0.015f, 0.5f), new Vector3(5.2f, 0.02f, 2.8f), new Color(0.36f, 0.42f, 0.44f));
            Box(architecture, "Bedroom floor", new Vector3(4.3f, 0.015f, 5.5f), new Vector3(5.2f, 0.02f, 6.8f), new Color(0.4f, 0.36f, 0.39f));
            Box(architecture, "West exterior", new Vector3(-7, 1.5f, 0), new Vector3(0.2f, 3, 18.2f), Wall);
            Box(architecture, "East exterior", new Vector3(7, 1.5f, 0), new Vector3(0.2f, 3, 18.2f), Wall);
            Box(architecture, "North exterior", new Vector3(0, 1.5f, 9), new Vector3(14, 3, 0.2f), Wall);
            Box(architecture, "South exterior west", new Vector3(-3.9f, 1.5f, -9), new Vector3(6.2f, 3, 0.2f), Wall);
            Box(architecture, "South exterior east", new Vector3(3.9f, 1.5f, -9), new Vector3(6.2f, 3, 0.2f), Wall);
            Box(architecture, "Entrance lintel", new Vector3(0, 2.65f, -9), new Vector3(1.6f, 0.7f, 0.2f), Wall);
            Door("Front Door", new Vector3(-0.76f, 0, -9), 0, 95, 1.52f);
            SideWall(-1.5f, -9, 0, -4.5f, "Living Room", -95);
            SideWall(-1.5f, 0, 9, 4.5f, "Study", -95);
            SideWall(1.5f, -9, -1, -5, "Kitchen", 95);
            SideWall(1.5f, -1, 2, 0.5f, "Bathroom", 95);
            SideWall(1.5f, 2, 9, 5.5f, "Bedroom", 95);
            Box(architecture, "Living-study partition", new Vector3(-4.25f, 1.5f, 0), new Vector3(5.5f, 3, 0.2f), Wall);
            Box(architecture, "Kitchen-bath partition", new Vector3(4.25f, 1.5f, -1), new Vector3(5.5f, 3, 0.2f), Wall);
            Box(architecture, "Bath-bedroom partition", new Vector3(4.25f, 1.5f, 2), new Vector3(5.5f, 3, 0.2f), Wall);
            Box(roof, "House ceiling", new Vector3(0, 3.1f, 0), new Vector3(14.2f, 0.2f, 18.2f), new Color(0.48f, 0.5f, 0.51f));
            Box(architecture, "Porch", new Vector3(0, -0.1f, -10.5f), new Vector3(4, 0.2f, 3), new Color(0.33f, 0.35f, 0.37f));
            Box(architecture, "Porch west rail", new Vector3(-2, 0.55f, -10.5f), new Vector3(0.15f, 1.1f, 3), Wall);
            Box(architecture, "Porch east rail", new Vector3(2, 0.55f, -10.5f), new Vector3(0.15f, 1.1f, 3), Wall);
            Box(architecture, "Porch south rail", new Vector3(0, 0.55f, -12), new Vector3(4, 1.1f, 0.15f), Wall);
            Room("Front Porch", new Vector3(0, 1.5f, -10.6f), new Vector3(3.8f, 3, 2.6f), false);
            Furnish();

            GameObject actor = new GameObject("Player");
            var controller = actor.AddComponent<CharacterController>();
            controller.height = 1.8f; controller.radius = 0.3f; controller.center = new Vector3(0, 0.9f, 0);
            controller.stepOffset = 0.2f;
            GameObject eye = new GameObject("First Person Camera", typeof(Camera), typeof(AudioListener));
            eye.tag = "MainCamera";
            eye.transform.SetParent(actor.transform, false);
            eye.transform.localPosition = new Vector3(0, 1.65f, 0);
            Camera view = eye.GetComponent<Camera>();
            view.clearFlags = CameraClearFlags.SolidColor; view.backgroundColor = new Color(0.1f, 0.12f, 0.15f);
            view.fieldOfView = 70; view.nearClipPlane = 0.05f; view.farClipPlane = 80;
            var player = actor.AddComponent<SandboxFirstPerson>(); player.View = eye.transform; player.Speed = 3;
            Transform spawn = new GameObject("Player Spawn").transform;
            spawn.position = new Vector3(0, 0.1f, -7.3f);
            actor.transform.position = spawn.position;
            HouseSession session = new GameObject("House Session").AddComponent<HouseSession>();
            session.Player = player; session.View = view; session.Spawn = spawn; session.HousePipeline = pipeline;
            session.Doors = doors.ToArray(); session.Rooms = rooms.ToArray();
            Directory.CreateDirectory("Assets/Anaphora3D/HousePrototype/Scenes");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), ScenePath);
            AssetDatabase.SaveAssets();
        }

        private static void Room(string label, Vector3 center, Vector3 size, bool count = true)
        {
            GameObject obj = new GameObject(label + " - exploration area");
            obj.transform.position = center;
            BoxCollider area = obj.AddComponent<BoxCollider>(); area.isTrigger = true; area.size = size;
            HouseRoom room = obj.AddComponent<HouseRoom>(); room.Label = label; room.Area = area; room.CountForExploration = count;
            rooms.Add(room);
        }

        private static GameObject Box(Transform parent, string label, Vector3 position, Vector3 size, Color tint)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = label; obj.transform.SetParent(parent, false); obj.transform.localPosition = position; obj.transform.localScale = size;
            obj.GetComponent<Renderer>().sharedMaterial = surface;
            HouseSurface color = obj.AddComponent<HouseSurface>(); color.Tint = tint; color.Apply();
            return obj;
        }

        private static void SideWall(float x, float start, float end, float opening, string room, float angle)
        {
            const float half = 0.75f;
            float lower = opening - half, upper = opening + half;
            Box(architecture, room + " wall A", new Vector3(x, 1.5f, (start + lower) / 2), new Vector3(0.2f, 3, lower - start), Wall);
            Box(architecture, room + " wall B", new Vector3(x, 1.5f, (upper + end) / 2), new Vector3(0.2f, 3, end - upper), Wall);
            Box(architecture, room + " lintel", new Vector3(x, 2.65f, opening), new Vector3(0.2f, 0.7f, 1.5f), Wall);
            Door(room + " Door", new Vector3(x, 0, lower + 0.04f), -90, angle, 1.42f);
        }

        private static void Door(string label, Vector3 hinge, float yaw, float angle, float width)
        {
            Transform pivot = new GameObject(label + " - hinge").transform;
            pivot.position = hinge; pivot.rotation = Quaternion.Euler(0, yaw, 0);
            GameObject leaf = Box(pivot, label + " - panel", new Vector3(width / 2, 1.12f, 0), new Vector3(width, 2.24f, 0.1f), Wood);
            HouseDoor door = pivot.gameObject.AddComponent<HouseDoor>();
            door.Label = label; door.Leaf = leaf.transform; door.LeafCollider = leaf.GetComponent<BoxCollider>(); door.OpenAngle = angle;
            doors.Add(door);
        }

        private static void Furnish()
        {
            Box(furniture, "Living - sofa seat", new Vector3(-5.6f, 0.4f, -4.5f), new Vector3(1, 0.8f, 2.8f), new Color(0.35f, 0.4f, 0.43f));
            Box(furniture, "Living - sofa back", new Vector3(-6.2f, 0.9f, -4.5f), new Vector3(0.25f, 1, 2.8f), new Color(0.35f, 0.4f, 0.43f));
            Box(furniture, "Living - coffee table", new Vector3(-4, 0.3f, -4.5f), new Vector3(0.9f, 0.6f, 1.6f), Wood);
            Box(furniture, "Living - picture placeholder", new Vector3(-6.85f, 1.7f, -7), new Vector3(0.1f, 0.65f, 0.9f), Wood);
            Box(furniture, "Study - desk", new Vector3(-5.7f, 0.5f, 6.6f), new Vector3(1.4f, 1, 2.4f), Wood);
            Box(furniture, "Study - cabinet", new Vector3(-4.6f, 1.1f, 8.3f), new Vector3(2.4f, 2.2f, 0.5f), Wood);
            Box(furniture, "Kitchen - north counter", new Vector3(5, 0.5f, -8.3f), new Vector3(3.2f, 1, 0.9f), new Color(0.52f, 0.53f, 0.5f));
            Box(furniture, "Kitchen - east counter", new Vector3(6.3f, 0.5f, -5.8f), new Vector3(0.9f, 1, 4), new Color(0.52f, 0.53f, 0.5f));
            Box(furniture, "Kitchen - refrigerator", new Vector3(5.9f, 1, -1.8f), new Vector3(1, 2, 0.9f), new Color(0.65f, 0.67f, 0.68f));
            Box(furniture, "Bathroom - tub", new Vector3(5.4f, 0.35f, 0.5f), new Vector3(2.3f, 0.7f, 1.1f), new Color(0.65f, 0.69f, 0.7f));
            Box(furniture, "Bathroom - sink", new Vector3(3.7f, 0.45f, 1.5f), new Vector3(0.8f, 0.9f, 0.55f), new Color(0.65f, 0.69f, 0.7f));
            Box(furniture, "Bedroom - bed base", new Vector3(5, 0.25f, 6), new Vector3(2, 0.5f, 2.7f), Wood);
            Box(furniture, "Bedroom - mattress", new Vector3(5, 0.6f, 6), new Vector3(1.95f, 0.2f, 2.65f), new Color(0.64f, 0.6f, 0.62f));
            Box(furniture, "Bedroom - wardrobe", new Vector3(5.3f, 1.1f, 8.4f), new Vector3(2.5f, 2.2f, 0.6f), Wood);
        }

        private static void ValidateHouse()
        {
            HouseSession session = UnityEngine.Object.FindAnyObjectByType<HouseSession>();
            if (session == null || session.Doors.Length != 6 || session.Rooms.Length != 7)
                throw new Exception("Expected six doors and seven exploration areas (five rooms, hall, porch).");
            CharacterController actor = session.Player.GetComponent<CharacterController>();
            actor.enabled = false;
            foreach (HouseDoor door in session.Doors)
            {
                Vector3 center = door.Leaf.position;
                Vector3 normal = door.transform.forward;
                Physics.SyncTransforms();
                if (!Physics.Raycast(center - normal, normal, out RaycastHit closed, 2, ~0, QueryTriggerInteraction.Ignore)
                    || closed.collider != door.LeafCollider) throw new Exception(door.Label + " does not block passage when closed.");
                door.SetOpenImmediately(true);
                Physics.SyncTransforms();
                // A full player capsule must fit through the opening, including the lintel and adjacent walls.
                Vector3 pathCenter = center - normal * 0.8f;
                for (int step = 0; step <= 16; step++)
                {
                    Vector3 foot = pathCenter + normal * (step * 0.1f); foot.y = 0.08f;
                    if (Physics.CheckCapsule(foot + Vector3.up * 0.3f, foot + Vector3.up * 1.5f,
                        0.3f, ~0, QueryTriggerInteraction.Ignore)) throw new Exception(door.Label + " blocks the player when open.");
                }
                door.SetOpenImmediately(false);
            }
            actor.enabled = true;
            Physics.SyncTransforms();
            if (Physics.CheckCapsule(session.Spawn.position + Vector3.up * 0.3f,
                session.Spawn.position + Vector3.up * 1.5f, 0.3f, ~(1 << 2), QueryTriggerInteraction.Ignore))
            {
                // Exclude the player itself when checking its spawn.
                actor.enabled = false;
                Physics.SyncTransforms();
                bool blocked = Physics.CheckCapsule(session.Spawn.position + Vector3.up * 0.3f,
                    session.Spawn.position + Vector3.up * 1.5f, 0.3f, ~0, QueryTriggerInteraction.Ignore);
                actor.enabled = true;
                if (blocked) throw new Exception("Player spawn is obstructed.");
            }
            Debug.Log("SPRINT1_CHECKS: 6 closed-door raycasts, 6 open-door capsule paths, scene references and spawn passed.");
        }
    }
}
