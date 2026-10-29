# Sanity sandbox

Open this project in Unity **6000.3.6f1**, then open
`Assets/Anaphora3D/SanitySandbox/Scenes/SanitySandbox.unity` in Single mode (double-click it).
Wait for import/compilation, press Play, select the Game tab, and click START / RESUME.
The room, player, and camera are created when Play starts; an empty room in edit mode is expected.

## Controls

| Input | Action |
| --- | --- |
| WASD | Move |
| Mouse | Look |
| Q | Switch world |
| H | Simulate a 25-point psychological hit in either world |
| R | Reset sanity, exposure, return counter, and player position |
| Esc | Pause/resume and release/capture cursor |

Losing application focus also pauses. Click START / RESUME to continue.

## Test sequence

1. Start in Illusion at 100%. Press Q: the monochrome room becomes colored and sanity falls.
2. Remain in Reality: the rate increases with continuous exposure. At zero, return to Illusion automatically.
3. Try Q immediately after depletion: re-entry is blocked until both the cooldown and 20% minimum are satisfied.
4. Recover in Illusion: sanity rises and stops at 100%. After one second in Reality, Q permits a manual return.
5. Walk onto the striped pad ahead-right. The HUD says THREATENED. Reality drains faster and Illusion recovers more slowly. Outside this pad the player is safe; the pad ahead-left is a safe landmark.
6. Press H repeatedly in Reality: psychological damage can force a return. In Illusion it damages sanity without changing worlds.
7. Press R to repeat. Press Esc and confirm that sanity pauses with the cursor released.

## Balance and integration

Select **Sanity Sandbox** in the Hierarchy and edit the **Sanity Controller** fields.
Changes made before Play are used on start. Changes during Play apply when R is pressed
and Unity normally discards those Inspector edits after Play stops.

Defaults: 5 points/sec Reality drain; 10 points/sec safe recovery; 2 points/sec threatened
recovery; 6 points/sec extra threatened drain; 20-point entry threshold; 1-second shift cooldown.
Continuous Reality exposure adds 0.15 points/sec to the drain rate each second, capped at 5 extra points/sec.
Exposure pressure resets on return to Illusion. These are provisional balance values.

`SanityState` holds the independent rules. `SanityController` exposes switching, psychological
damage, the current state, and a world-change event for later house/enemy integration.
The danger pad is a test substitute for enemy pursuit. Shifting is already unlocked in this scene.
There is no health, combat, puzzle progression, final art, or picture-frame interaction yet.

The sandbox applies its own 3D pipeline while running and restores the previous pipeline on exit.
Open it alone, rather than additively alongside another game scene.
It has not been added to the original game's build scene list.

## Verification

From the repository root, with .NET 10 installed:

```powershell
dotnet run --project Tools/SanitySandboxChecks/SanitySandboxChecks.csproj
```

16 checks cover state transitions, depletion, threshold/cooldown, recovery limits,
threat penalties, exposure integration, and reset. The four scripts also compiled against
the available Unity 6000.5.4f1 engine libraries. This does not verify rendering or input in
Play Mode. The installed 6000.3.6f1 editor folder was incomplete during setup, so the scene
still needs the Play Mode test above in the project's intended editor version.
