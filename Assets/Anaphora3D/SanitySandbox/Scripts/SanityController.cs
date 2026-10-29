using System;
using UnityEngine;

namespace Anaphora.SanitySandbox
{
    public sealed class SanityController : MonoBehaviour
    {
        [Header("Starting balance (sanity points per second)")]
        [Min(0)] public float RealityDrain = 5;
        [Min(0)] public float SafeRecovery = 10;
        [Min(0)] public float ThreatenedRecovery = 2;
        [Min(0)] public float ExtraThreatDrain = 6;
        [Header("Switching")]
        [Tooltip("Disabled in the house until the broken family picture is examined.")]
        public bool StartsUnlocked = true;
        public bool SwitchingUnlocked { get; private set; }
        [Range(1, 100)] public float MinimumEntrySanity = 20;
        [Min(0)] public float ShiftCooldown = 1;
        [Header("Prolonged Reality exposure")]
        [Min(0)] public float PressureIncreasePerSecond = 0.15f;
        [Min(0)] public float MaximumExtraDrain = 5;

        public SanityState State { get; private set; }
        public bool Threatened { get; set; }
        public bool Paused { get; set; }
        public string LastMessage { get; private set; } = "Press Q to enter Reality.";
        public event Action<bool> WorldChanged;

        private void Awake() { ResetSimulation(); }

        private void Update()
        {
            if (Paused) return;
            bool previousWorld = State.IsReality;
            State.Tick(Time.deltaTime, Threatened);
            PublishWorldChange(previousWorld);
        }

        public void Shift()
        {
            if (Paused) return;
            if (!SwitchingUnlocked)
            {
                LastMessage = "Reality shifting is locked. Examine the broken family picture in the Living Room.";
                return;
            }
            bool previousWorld = State.IsReality;
            State.TryShift(out string message);
            LastMessage = message;
            PublishWorldChange(previousWorld);
        }

        public void PsychologicalHit(float damage)
        {
            if (Paused) return;
            bool previousWorld = State.IsReality;
            State.ApplyDamage(damage);
            LastMessage = "Psychological hit: -" + damage.ToString("0") + " sanity.";
            PublishWorldChange(previousWorld);
        }

        public void UnlockSwitching()
        {
            SwitchingUnlocked = true;
            LastMessage = "Reality shifting unlocked. Q switches worlds.";
        }

        public void ResetSimulation()
        {
            State = new SanityState(RealityDrain, SafeRecovery, ThreatenedRecovery,
                ExtraThreatDrain, MinimumEntrySanity, ShiftCooldown,
                PressureIncreasePerSecond, MaximumExtraDrain);
            Threatened = false;
            SwitchingUnlocked = StartsUnlocked;
            LastMessage = SwitchingUnlocked ? "Reset to 100%. Press Q to enter Reality." :
                "Find and examine the broken family picture in the Living Room.";
            WorldChanged?.Invoke(false);
        }

        private void PublishWorldChange(bool previousWorld)
        {
            if (previousWorld == State.IsReality) return;
            if (!State.IsReality && State.Value == 0)
                LastMessage = "SANITY DEPLETED: forced back to Illusion. Recover before shifting.";
            WorldChanged?.Invoke(State.IsReality);
        }
    }
}
