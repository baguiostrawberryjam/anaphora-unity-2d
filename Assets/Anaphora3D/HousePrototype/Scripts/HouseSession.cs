using Anaphora.SanitySandbox;
using UnityEngine;
using UnityEngine.Rendering;

namespace Anaphora.HousePrototype
{
    [DefaultExecutionOrder(-100)]
    public sealed class HouseSession : MonoBehaviour
    {
        public SandboxFirstPerson Player;
        public Camera View;
        public Transform Spawn;
        public RenderPipelineAsset HousePipeline;
        public HouseDoor[] Doors;
        public HouseRoom[] Rooms;
        public SanityController Sanity;
        public HousePicture FamilyPicture;
        public float InteractionDistance = 2.5f;
        private RenderPipelineAsset previousDefault, previousQuality;
        private IHouseInteractable target;
        private string currentRoom = "Entrance Hall";
        private GUIStyle heading, body;
        private bool initialized;

        private void Start()
        {
            previousDefault = GraphicsSettings.defaultRenderPipeline;
            previousQuality = QualitySettings.renderPipeline;
            if (HousePipeline != null)
            {
                GraphicsSettings.defaultRenderPipeline = HousePipeline;
                QualitySettings.renderPipeline = HousePipeline;
            }
            initialized = true;
            ResetWalkthrough();
            Capture(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Capture(!Player.Captured);
            target = null;
            if (!Player.Captured) return;
            if (Input.GetKeyDown(KeyCode.R)) { ResetWalkthrough(); return; }
            if (Physics.Raycast(View.transform.position, View.transform.forward, out RaycastHit hit,
                InteractionDistance, ~0, QueryTriggerInteraction.Ignore))
                target = HouseInteraction.Find(hit.collider);
            if (target != null && Input.GetKeyDown(KeyCode.E)) target.Interact();
            Vector3 feet = Player.transform.position + Vector3.up;
            foreach (HouseRoom room in Rooms)
                if (room.Contains(feet)) { currentRoom = room.Label; room.Visited = true; break; }
        }

        private void Capture(bool value)
        {
            Player.Capture(value);
            if (Sanity != null) Sanity.Paused = !value;
            foreach (HouseDoor door in Doors) door.Paused = !value;
        }

        private void ResetWalkthrough()
        {
            CharacterController controller = Player.GetComponent<CharacterController>();
            Player.ReturnToStart();
            controller.enabled = false;
            Player.transform.SetPositionAndRotation(Spawn.position, Spawn.rotation);
            controller.enabled = true;
            foreach (HouseDoor door in Doors) door.ResetDoor();
            foreach (HouseRoom room in Rooms) room.Visited = false;
            if (Sanity != null) Sanity.ResetSimulation();
            if (FamilyPicture != null) FamilyPicture.ResetInspection();
            currentRoom = "Entrance Hall";
            Physics.SyncTransforms();
        }

        private void OnApplicationFocus(bool focused) { if (!focused && initialized) Capture(false); }

        private void OnGUI()
        {
            if (!initialized) return;
            if (heading == null)
            {
                heading = new GUIStyle(GUI.skin.label) { fontSize = 23, fontStyle = FontStyle.Bold, wordWrap = true };
                body = new GUIStyle(GUI.skin.label) { fontSize = 17, wordWrap = true };
            }
            Matrix4x4 original = GUI.matrix;
            float scale = Mathf.Min(Screen.width / 1100f, Screen.height / 720f);
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
            float width = Screen.width / scale, height = Screen.height / scale;
            Panel(new Rect(20, 20, 370, 280));
            GUI.Label(new Rect(38, 32, 340, 35), "ANAPHORA / HOUSE", heading);
            GUI.Label(new Rect(38, 73, 335, 30), currentRoom, body);
            int visited = 0, total = 0;
            string checklist = "";
            foreach (HouseRoom room in Rooms)
            {
                if (!room.CountForExploration) continue;
                total++;
                if (room.Visited) visited++;
                checklist += (room.Visited ? "[x] " : "[ ] ") + room.Label + "\n";
            }
            GUI.Label(new Rect(38, 110, 335, 30), "Rooms explored: " + visited + " / " + total, body);
            GUI.Label(new Rect(38, 148, 335, 145), checklist, body);
            Panel(new Rect(20, height - 112, width - 40, 92));
            string objective = FamilyPicture != null && !FamilyPicture.HasBeenExamined
                ? "Objective: examine the broken family picture in the Living Room."
                : "Reality shifting unlocked. Use Q to switch worlds and continue exploring.";
            GUI.Label(new Rect(36, height - 103, width - 72, 32), objective, body);
            GUI.Label(new Rect(36, height - 65, width - 72, 40),
                visited == total ? "House walkthrough complete. Press R to explore again." : "Explore the house. WASD + mouse: move / look     E: door     Esc: pause     R: restart", body);
            if (Player.Captured)
            {
                GUI.Label(new Rect(width / 2 - 6, height / 2 - 14, 24, 30), "+", body);
                if (target != null)
                {
                    Panel(new Rect(width / 2 - 220, height / 2 + 38, 440, 70));
                    GUI.Label(new Rect(width / 2 - 205, height / 2 + 48, 410, 55),
                        target.InteractionPrompt, body);
                }
            }
            else
            {
                Panel(new Rect(width / 2 - 245, height / 2 - 115, 490, 230));
                GUI.Label(new Rect(width / 2 - 220, height / 2 - 95, 440, 40), "HOUSE / REALITY SHIFTING", heading);
                GUI.Label(new Rect(width / 2 - 220, height / 2 - 43, 440, 80),
                    "E: doors / examine family picture\nFind the picture in the Living Room.\nQ: shift after unlock   Esc: pause   R: restart", body);
                if (GUI.Button(new Rect(width / 2 - 150, height / 2 + 55, 300, 40), "START / RESUME")) Capture(true);
            }
            GUI.matrix = original;
        }

        private static void Panel(Rect rect)
        {
            Color previous = GUI.color;
            GUI.color = new Color(0.03f, 0.04f, 0.055f, 0.96f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = previous;
        }

        private void OnDestroy()
        {
            if (!initialized) return;
            GraphicsSettings.defaultRenderPipeline = previousDefault;
            QualitySettings.renderPipeline = previousQuality;
            if (Player != null) Player.Capture(false);
        }
    }
}
