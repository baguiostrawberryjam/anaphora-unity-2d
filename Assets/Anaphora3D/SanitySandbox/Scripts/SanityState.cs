using System;

namespace Anaphora.SanitySandbox
{
    // Engine-independent rules so the same system can later drive the house and enemies.
    public sealed class SanityState
    {
        public const float Maximum = 100f;
        public float Value { get; private set; } = Maximum;
        public bool IsReality { get; private set; }
        public float ExposureSeconds { get; private set; }
        public float CooldownRemaining { get; private set; }
        public int ForcedReturns { get; private set; }
        public float Drain { get; }
        public float SafeRecovery { get; }
        public float ThreatRecovery { get; }
        public float ThreatDrain { get; }
        public float EntryThreshold { get; }
        public float ShiftCooldown { get; }
        public float PressurePerSecond { get; }
        public float MaximumExtraDrain { get; }

        public SanityState(float drain = 5, float safeRecovery = 10, float threatRecovery = 2,
            float threatDrain = 6, float entryThreshold = 20, float shiftCooldown = 1,
            float pressurePerSecond = 0.15f, float maximumExtraDrain = 5)
        {
            Drain = Math.Max(0, drain);
            SafeRecovery = Math.Max(0, safeRecovery);
            ThreatRecovery = Math.Max(0, threatRecovery);
            ThreatDrain = Math.Max(0, threatDrain);
            EntryThreshold = Math.Min(Maximum, Math.Max(1, entryThreshold));
            ShiftCooldown = Math.Max(0, shiftCooldown);
            PressurePerSecond = Math.Max(0, pressurePerSecond);
            MaximumExtraDrain = Math.Max(0, maximumExtraDrain);
        }

        public float Rate(bool threatened)
        {
            return IsReality
                ? -(Drain + Math.Min(MaximumExtraDrain, ExposureSeconds * PressurePerSecond)
                    + (threatened ? ThreatDrain : 0))
                : (threatened ? ThreatRecovery : SafeRecovery);
        }

        public bool TryShift(out string message)
        {
            if (CooldownRemaining > 0)
            {
                message = "Shift is cooling down.";
                return false;
            }
            if (!IsReality && Value < EntryThreshold)
            {
                message = "Recover to " + EntryThreshold.ToString("0") + "% before entering Reality.";
                return false;
            }
            IsReality = !IsReality;
            ExposureSeconds = 0;
            CooldownRemaining = ShiftCooldown;
            message = IsReality ? "Entered Reality. Sanity is draining." : "Returned to Illusion. Sanity is recovering.";
            return true;
        }

        public void Tick(float seconds, bool threatened)
        {
            if (seconds <= 0 || float.IsNaN(seconds) || float.IsInfinity(seconds)) return;
            CooldownRemaining = Math.Max(0, CooldownRemaining - seconds);
            if (!IsReality)
            {
                Value = Math.Min(Maximum, Value + Rate(threatened) * seconds);
                return;
            }

            // Integrate increasing pressure rather than tying drain to frame count.
            float pressure = PressureIntegral(ExposureSeconds + seconds) - PressureIntegral(ExposureSeconds);
            ExposureSeconds += seconds;
            ApplyDamage((Drain + (threatened ? ThreatDrain : 0)) * seconds + pressure);
        }

        private float PressureIntegral(float time)
        {
            if (PressurePerSecond <= 0) return 0;
            float ramp = Math.Min(time, MaximumExtraDrain / PressurePerSecond);
            return 0.5f * PressurePerSecond * ramp * ramp + MaximumExtraDrain * (time - ramp);
        }

        public void ApplyDamage(float amount)
        {
            if (amount <= 0 || float.IsNaN(amount) || float.IsInfinity(amount)) return;
            Value = Math.Max(0, Value - amount);
            if (Value > 0 || !IsReality) return;
            IsReality = false;
            ExposureSeconds = 0;
            CooldownRemaining = ShiftCooldown;
            ForcedReturns++;
        }

        public void Reset()
        {
            Value = Maximum;
            IsReality = false;
            ExposureSeconds = 0;
            CooldownRemaining = 0;
            ForcedReturns = 0;
        }
    }
}
