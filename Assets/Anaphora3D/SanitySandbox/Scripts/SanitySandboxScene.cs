using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Anaphora.SanitySandbox
{
    [RequireComponent(typeof(SanityController))]
    public sealed class SanitySandboxScene : MonoBehaviour
    {
        [Tooltip("Dedicated 3D pipeline, applied only while this sandbox is running.")]
        public RenderPipelineAsset SandboxPipeline;
        public Material SurfaceMaterial;
        private RenderPipelineAsset previousDefault;
        private RenderPipelineAsset previousQuality;
        private SanityController sanity;
        private SandboxFirstPerson player;
        private Camera view;
        private readonly List<Renderer> surfaces = new List<Renderer>();
        private readonly List<Color> realityColors = new List<Color>();
        private readonly List<Material> ownedMaterials = new List<Material>();
        private bool pipelineApplied;
        private GUIStyle titleStyle, bodyStyle, smallStyle;

        private void Start()
        {
            sanity = GetComponent<SanityController>();
            if (SandboxPipeline == null || SurfaceMaterial == null)
            {
                Debug.LogError("Sandbox scene is missing its pipeline or surface material.", this);
                enabled = false;
                return;
            }
            previousDefault = GraphicsSettings.defaultRenderPipeline;
            previousQuality = QualitySettings.renderPipeline;
            GraphicsSettings.defaultRenderPipeline = SandboxPipeline;
            QualitySettings.renderPipeline = SandboxPipeline;
            pipelineApplied = true;

            BuildRoom();
            GameObject actor = new GameObject("Sandbox Player");
            actor.transform.SetParent(transform);
            CharacterController controller = actor.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.3f;
            controller.center = new Vector3(0, 0.9f, 0);
            controller.stepOffset = 0.25f;
            GameObject cameraObject = new GameObject("Sandbox Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.transform.SetParent(actor.transform, false);
            cameraObject.transform.localPosition = new Vector3(0, 1.65f, 0);
            view = cameraObject.GetComponent<Camera>();
            view.clearFlags = CameraClearFlags.SolidColor;
            view.nearClipPlane = 0.05f;
            view.farClipPlane = 60;
            view.fieldOfView = 70;
            player = actor.AddComponent<SandboxFirstPerson>();
            player.View = cameraObject.transform;
            player.ReturnToStart();
            sanity.WorldChanged += SetWorldAppearance;
            SetWorldAppearance(false);
            SetCaptured(false);
        }

        private void BuildRoom()
        {
            Color floor = new Color(0.21f, 0.26f, 0.29f);
            for (int x = -3; x <= 3; x++)
                for (int z = -4; z <= 4; z++)
                    Box("Floor tile", new Vector3(x * 2, -0.15f, z * 2), new Vector3(1.98f, 0.3f, 1.98f),
                        floor * (((x + z) % 2 == 0) ? 1 : 0.85f));
            Box("North wall", new Vector3(0, 1.5f, 9), new Vector3(14, 3, 0.3f), new Color(0.3f, 0.36f, 0.4f));
            Box("South wall", new Vector3(0, 1.5f, -9), new Vector3(14, 3, 0.3f), new Color(0.3f, 0.36f, 0.4f));
            Box("West wall", new Vector3(-7, 1.5f, 0), new Vector3(0.3f, 3, 18), new Color(0.25f, 0.31f, 0.35f));
            Box("East wall", new Vector3(7, 1.5f, 0), new Vector3(0.3f, 3, 18), new Color(0.25f, 0.31f, 0.35f));
            Box("Danger area", new Vector3(4, 0.015f, 4.5f), new Vector3(4, 0.02f, 5), new Color(0.55f, 0.14f, 0.15f));
            for (int i = 0; i < 5; i++)
                Box("Danger stripe", new Vector3(4, 0.03f, 2.5f + i), new Vector3(3.6f, 0.01f, 0.12f), new Color(0.95f, 0.65f, 0.4f));
            Box("Safe area", new Vector3(-4, 0.015f, 4.5f), new Vector3(4, 0.02f, 5), new Color(0.15f, 0.4f, 0.4f));
            Box("Center column", new Vector3(0, 1.2f, 4), new Vector3(0.8f, 2.4f, 0.8f), new Color(0.45f, 0.38f, 0.3f));
            Box("Safe landmark", new Vector3(-4, 0.6f, 6), new Vector3(0.9f, 1.2f, 0.9f), new Color(0.25f, 0.65f, 0.65f));
            Box("Danger landmark", new Vector3(4, 1, 6), new Vector3(0.7f, 2, 0.7f), new Color(0.8f, 0.23f, 0.2f));
        }

        private void Box(string label, Vector3 position, Vector3 scale, Color color)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = label;
            cube.transform.SetParent(transform, false);
            cube.transform.localPosition = position;
            cube.transform.localScale = scale;
            Material material = new Material(SurfaceMaterial);
            ownedMaterials.Add(material);
            Renderer surface = cube.GetComponent<Renderer>();
            surface.sharedMaterial = material;
            surfaces.Add(surface);
            realityColors.Add(color);
        }

        private void SetWorldAppearance(bool reality)
        {
            for (int i = 0; i < surfaces.Count; i++)
            {
                Color color = realityColors[i];
                float grey = Mathf.Lerp(0.25f, 0.85f, color.grayscale);
                surfaces[i].sharedMaterial.SetColor("_BaseColor", reality ? color : new Color(grey, grey, grey));
            }
            view.backgroundColor = reality ? new Color(0.035f, 0.055f, 0.09f) : new Color(0.75f, 0.75f, 0.75f);
        }

        private void Update()
        {
            if (player == null) return;
            if (Input.GetKeyDown(KeyCode.Escape)) SetCaptured(!player.Captured);
            if (!player.Captured) return;
            Vector3 position = player.transform.position;
            // A test-only stand-in for an enemy's pursuit state; independent of active world.
            sanity.Threatened = position.x >= 2 && position.x <= 6 && position.z >= 2 && position.z <= 7;
            if (Input.GetKeyDown(KeyCode.Q)) sanity.Shift();
            if (Input.GetKeyDown(KeyCode.H)) sanity.PsychologicalHit(25);
            if (Input.GetKeyDown(KeyCode.R))
            {
                sanity.ResetSimulation();
                player.ReturnToStart();
            }
        }

        private void SetCaptured(bool captured)
        {
            player.Capture(captured);
            sanity.Paused = !captured;
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused && player != null) SetCaptured(false);
        }

        private void OnGUI()
        {
            if (sanity == null || player == null) return;
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 24, fontStyle = FontStyle.Bold };
                bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 17, wordWrap = true };
                smallStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
            }
            Matrix4x4 previousMatrix = GUI.matrix;
            Color previousColor = GUI.color;
            float scale = Mathf.Min(Screen.width / 1100f, Screen.height / 720f);
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
            float width = Screen.width / scale;
            float height = Screen.height / scale;
            SanityState state = sanity.State;
            Panel(new Rect(20, 20, 455, 255));
            GUI.Label(new Rect(38, 32, 420, 35), "ANAPHORA / SANITY LAB", titleStyle);
            GUI.Label(new Rect(38, 72, 420, 28), state.IsReality ? "REALITY  /  exposure drains sanity" : "ILLUSION  /  sanity recovers", bodyStyle);
            GUI.Label(new Rect(38, 107, 420, 28), "SANITY  " + state.Value.ToString("0.0") + "%", titleStyle);
            Fill(new Rect(38, 144, 418, 18), new Color(0.15f, 0.18f, 0.22f));
            Fill(new Rect(38, 144, 418 * state.Value / SanityState.Maximum, 18),
                state.Value <= 25 ? new Color(0.95f, 0.35f, 0.3f) : new Color(0.35f, 0.8f, 0.78f));
            string rate = state.Rate(sanity.Threatened).ToString("+0.0;-0.0;0.0");
            GUI.Label(new Rect(38, 173, 420, 25), (sanity.Threatened ? "THREATENED" : "SAFE") + "  |  " + rate + " / sec", bodyStyle);
            GUI.Label(new Rect(38, 204, 420, 55), "Exposure: " + state.ExposureSeconds.ToString("0.0") + "s   |   Forced returns: " + state.ForcedReturns +
                "\nShift cooldown: " + state.CooldownRemaining.ToString("0.0") + "s", smallStyle);

            Panel(new Rect(width - 370, 20, 350, 175));
            GUI.Label(new Rect(width - 352, 34, 320, 150),
                "WASD  Move     Mouse  Look\nQ  Switch world\nH  Simulate -25 sanity attack\nR  Reset player and sanity\nEsc  Pause / release cursor", bodyStyle);
            Panel(new Rect(20, height - 116, width - 40, 96));
            GUI.Label(new Rect(38, height - 106, width - 76, 42), sanity.LastMessage, bodyStyle);
            GUI.Label(new Rect(38, height - 60, width - 76, 35),
                "Ahead-left: safe pad.  Ahead-right: striped danger pad (extra drain / slower recovery).  Health is not simulated.", smallStyle);
            GUI.Label(new Rect(width / 2 - 7, height / 2 - 14, 22, 28), "+", bodyStyle);

            if (state.Value <= 25 && player.Captured)
                GUI.Label(new Rect(width / 2 - 200, height / 2 + 45, 400, 50),
                    state.IsReality ? "LOW SANITY - RETURN WITH Q" : "RECOVERING - WAIT BEFORE SHIFTING", bodyStyle);
            if (!player.Captured)
            {
                Panel(new Rect(width / 2 - 230, height / 2 - 100, 460, 200));
                GUI.Label(new Rect(width / 2 - 210, height / 2 - 82, 420, 60), "SANITY SANDBOX / PAUSED", titleStyle);
                GUI.Label(new Rect(width / 2 - 210, height / 2 - 35, 420, 50), "Test shifting and recovery. The picture-frame unlock is bypassed in this test scene.", bodyStyle);
                if (GUI.Button(new Rect(width / 2 - 150, height / 2 + 35, 300, 42), "START / RESUME")) SetCaptured(true);
            }
            GUI.matrix = previousMatrix;
            GUI.color = previousColor;
        }

        private static void Fill(Rect rect, Color color)
        {
            Color previous = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = previous;
        }

        private static void Panel(Rect rect) { Fill(rect, new Color(0.035f, 0.045f, 0.065f, 0.96f)); }

        private void OnDestroy()
        {
            if (sanity != null) sanity.WorldChanged -= SetWorldAppearance;
            if (pipelineApplied)
            {
                GraphicsSettings.defaultRenderPipeline = previousDefault;
                QualitySettings.renderPipeline = previousQuality;
            }
            foreach (Material material in ownedMaterials) if (material != null) Destroy(material);
        }
    }
}
