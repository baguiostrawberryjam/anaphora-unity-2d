# ANAPHORA - 3D Prototype

A first-person prototype adapting ANAPHORA, a 2D story-based psychological horror
game, into 3D. The current playable section combines house exploration, reality
shifting, sanity management, and the first story interaction in one scene.

## Requirements

- Unity Editor **6000.5.4f1**.
- Git LFS for the original game's images, audio, fonts, and other large files.
- .NET 10 only if running the standalone sanity-rule checks.

After cloning, run `git lfs pull` if Git LFS assets were not downloaded. Open the
project in Unity Hub with the editor version above and allow packages to import.

## Play the current prototype

1. Choose **Anaphora > Sprint 1 > Open House**, or open
   `Assets/Anaphora3D/HousePrototype/Scenes/HouseExploration.unity`.
2. Press **Play**, select the Game tab, and click **START / RESUME**.
3. Enter the Living Room on the left near the starting position.
4. Look at the broken family picture on the far wall and press **E**.
5. The first Reality shift occurs and **Q** becomes available for manual switching.

| Input | Action |
| --- | --- |
| WASD / mouse | Move / look |
| E | Open or close a door; examine the family picture |
| Q | Switch worlds after examining the picture |
| H | Simulate a 25-point sanity hit after unlocking shifting |
| R | Reset the walkthrough, picture discovery, and sanity |
| Esc | Pause/resume and release/capture the cursor |

Reality drains sanity; Illusion restores it. At zero sanity, the player returns to
Illusion automatically. The Study is a **pressure test area**, simulating extra
drain and slower recovery without an enemy. The minimum re-entry threshold is
20 sanity and the shift cooldown is one second. These balance values are provisional.

The standalone sandbox remains available at
`Assets/Anaphora3D/SanitySandbox/Scenes/SanitySandbox.unity`. Shifting is already
unlocked there so the core mechanic can be tested independently.

## Project layout

| Location | Contents |
| --- | --- |
| `Assets/Anaphora2D` | Original 2D game scenes, scripts, art, branding, and recovery scene |
| `Assets/Anaphora3D/HousePrototype` | Editable house scene, doors, interactions, and story unlock |
| `Assets/Anaphora3D/SanitySandbox` | Sanity rules, controller, movement, standalone sandbox, and prototype rendering assets |
| `Assets/Editor` | One-time scene setup/organization scripts and Play Mode checks |
| `Assets/Settings`, other shared top-level folders | Shared project settings and third-party dependencies |
| `Tools/SanitySandboxChecks` | Standalone checks for the sanity rules |

The **Open House** menu retains its Sprint 1 label and now opens the combined
Sprint 1 + Sprint 2 scene. Open that scene alone rather than additively with the sandbox.
Scene geometry is editable directly in Unity. One-time migration/setup utilities
are guarded for the original development folder; teammates should open the saved scene.

## Verification and current limits

Run the engine-independent checks from the repository root:

```powershell
dotnet run --project Tools/SanitySandboxChecks/SanitySandboxChecks.csproj
```

The sanity rules, house collision/door clearances, and combined story/sanity
behavior have automated checks. Human playtesting is still needed for presentation,
controls, and pacing.

The current scene uses basic shapes. Combat, enemy AI, inventory, cross-world
puzzles, final art/audio, and save/load for the new progression are future work.
The existing build scene list still follows the original 2D game; this prototype
is currently tested by opening its scene in the editor.

See [CHANGELOG.md](CHANGELOG.md) for the completed development work and validation.
