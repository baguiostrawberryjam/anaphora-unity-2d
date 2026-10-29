using System.Collections.Generic;
using Anaphora.SanitySandbox;
using UnityEngine;

namespace Anaphora.HousePrototype
{
    [DefaultExecutionOrder(-80)]
    [RequireComponent(typeof(SanityController))]
    public sealed class HouseSanityIntegration : MonoBehaviour
    {
        public HouseSession Session;
        public SanityController Sanity;
        [Tooltip("Test substitute for enemy pursuit. Clear this field to disable the test area.")]
        public HouseRoom PressureTestRoom;
        private readonly List<HouseSurface> surfaces = new List<HouseSurface>();
        private Color originalBackground;
        private GUIStyle heading, body;

        private void Awake()
        {
            foreach (HouseSurface surface in FindObjectsByType<HouseSurface>())
                if (surface.gameObject.scene == gameObject.scene) surfaces.Add(surface);
            originalBackground = Session.View.backgroundColor;
        }

        private void OnEnable()
        {
            if (Sanity == null) Sanity = GetComponent<SanityController>();
            Sanity.WorldChanged += ApplyWorld;
        }

        private void Start()
        {
            Sanity.Paused = !Session.Player.Captured;
            ApplyWorld(Sanity.State.IsReality);
        }

        private void Update()
        {
            Sanity.Paused = !Session.Player.Captured;
            if (Sanity.Paused) return;
            Sanity.Threatened = PressureTestRoom != null &&
                PressureTestRoom.Contains(Session.Player.transform.position + Vector3.up);
            if (Input.GetKeyDown(KeyCode.Q)) Sanity.Shift();
            if (Sanity.SwitchingUnlocked && Input.GetKeyDown(KeyCode.H)) Sanity.PsychologicalHit(25);
        }

        private void ApplyWorld(bool reality)
        {
            foreach (HouseSurface surface in surfaces) if (surface != null) surface.SetWorld(reality);
            Session.View.backgroundColor = reality ? new Color(0.035f, 0.055f, 0.09f) : new Color(0.65f, 0.65f, 0.65f);
        }

        private void OnGUI()
        {
            if (Sanity == null || Sanity.State == null) return;
            if (heading == null)
            {
                heading = new GUIStyle(GUI.skin.label) { fontSize = 23, fontStyle = FontStyle.Bold };
                body = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
            }
            Matrix4x4 original = GUI.matrix;
            float scale = Mathf.Min(Screen.width / 1100f, Screen.height / 720f);
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
            float x = Screen.width / scale - 390;
            SanityState state = Sanity.State;
            Fill(new Rect(x, 20, 370, 325), new Color(0.03f, 0.04f, 0.055f, 0.96f));
            GUI.Label(new Rect(x + 18, 32, 335, 35), state.IsReality ? "REALITY" : "ILLUSION", heading);
            GUI.Label(new Rect(x + 18, 73, 335, 32), "SANITY  " + state.Value.ToString("0.0") + "%", heading);
            Fill(new Rect(x + 18, 112, 334, 16), new Color(0.15f, 0.18f, 0.22f));
            Fill(new Rect(x + 18, 112, 334 * state.Value / 100, 16),
                state.Value <= 25 ? new Color(0.95f, 0.35f, 0.3f) : new Color(0.35f, 0.8f, 0.78f));
            string status = Sanity.Paused ? "PAUSED" : Sanity.Threatened ? "PRESSURE TEST / STUDY" : "SAFE";
            GUI.Label(new Rect(x + 18, 141, 335, 30), status + "  |  " +
                (Sanity.Paused ? "0.0" : state.Rate(Sanity.Threatened).ToString("+0.0;-0.0;0.0")) + " / sec", body);
            GUI.Label(new Rect(x + 18, 177, 335, 53), (Sanity.SwitchingUnlocked ? "Q: shift   H: -25 sanity test hit" : "SHIFT LOCKED / Find the family picture") + "\nCooldown: " + state.CooldownRemaining.ToString("0.0") +
                "s   Returns: " + state.ForcedReturns, body);
            GUI.Label(new Rect(x + 18, 235, 335, 90), Sanity.LastMessage, body);
            GUI.matrix = original;
        }

        private static void Fill(Rect rect, Color color)
        {
            Color previous = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = previous;
        }

        private void OnDisable()
        {
            if (Sanity != null) Sanity.WorldChanged -= ApplyWorld;
            foreach (HouseSurface surface in surfaces) if (surface != null) surface.SetWorld(true);
            if (Session != null && Session.View != null) Session.View.backgroundColor = originalBackground;
        }
    }
}
