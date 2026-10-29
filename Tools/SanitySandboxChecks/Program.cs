using System;
using Anaphora.SanitySandbox;

static class Program
{
    static int count;
    static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception("FAILED: " + label);
        count++;
        Console.WriteLine("PASS: " + label);
    }
    static void Main()
    {
        var state = new SanityState(pressurePerSecond: 0);
        Check(state.Value == 100 && !state.IsReality, "Starts at 100 in Illusion");
        Check(state.TryShift(out _), "Can enter Reality");
        Check(!state.TryShift(out _), "Repeated shift is blocked during cooldown");
        state.Tick(2, false);
        Check(state.Value == 90, "Reality drains at the configured rate");
        state.TryShift(out _);
        state.Tick(5, false);
        Check(state.Value == 100, "Recovery clamps at 100");
        state.TryShift(out _);
        state.ApplyDamage(200);
        Check(state.Value == 0 && !state.IsReality && state.ForcedReturns == 1, "Damage at zero forces one return");
        state.Tick(1, false);
        Check(!state.TryShift(out _), "Cannot re-enter below 20 sanity");
        state.Tick(1, false);
        Check(state.TryShift(out _), "Can re-enter at 20 sanity");
        state.Tick(10, true);
        Check(state.Value == 0 && !state.IsReality && state.ForcedReturns == 2, "Passive depletion also forces return");
        state.Tick(2, true);
        Check(state.Value == 4, "Threat slows Illusion recovery");
        state.Reset();
        Check(state.Value == 100 && state.ForcedReturns == 0 && state.CooldownRemaining == 0, "Reset clears state");
        state.TryShift(out _);
        state.Tick(1, true);
        Check(state.Value == 89, "Threat adds Reality drain");
        var oneStep = new SanityState();
        var manySteps = new SanityState();
        oneStep.TryShift(out _); manySteps.TryShift(out _);
        oneStep.Tick(10, false);
        for (int i = 0; i < 100; i++) manySteps.Tick(0.1f, false);
        Check(Math.Abs(oneStep.Value - manySteps.Value) < 0.001f, "Exposure pressure is independent of frame subdivision");
        Check(Math.Abs(oneStep.Value - 42.5f) < 0.001f, "Prolonged exposure increases drain");
        var capped = new SanityState(drain: 0, maximumExtraDrain: 1);
        capped.TryShift(out _);
        capped.Tick(10, false);
        Check(Math.Abs(capped.Value - 93.33333f) < 0.001f, "Exposure pressure stops increasing at its cap");
        float before = capped.Value;
        capped.Tick(-1, false); capped.Tick(float.NaN, false); capped.ApplyDamage(-20);
        Check(capped.Value == before, "Invalid time and negative damage do not change sanity");
        Console.WriteLine(count + " sanity checks passed.");
    }
}
