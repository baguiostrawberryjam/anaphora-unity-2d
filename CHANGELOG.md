# Changelog

Development changes for ANAPHORA's 3D prototype are recorded here. Entries describe
implemented behavior; proposed features are listed separately from completed work.

## [Unreleased] - 2026-10-29

### Added - Sanity prototype

- Standalone first-person sanity sandbox with Illusion and Reality modes.
- Configurable Reality drain, recovery in Illusion, psychological damage, and low-sanity warnings.
- Increasing drain during continuous Reality exposure, with a configurable cap.
- Automatic return to Illusion at zero sanity, a minimum re-entry threshold, and a switching cooldown.
- Safe versus threatened recovery and drain rates, with a test area simulating enemy pressure.
- Pause/resume, cursor capture, and a repeatable test reset.
- Engine-independent sanity rules with 16 automated checks.

### Added - Sprint 1: House exploration

- One editable 3D house scene built from basic shapes.
- Entrance hall, Living Room, Kitchen, Study, Bedroom, Bathroom, and front porch.
- First-person movement, collision, six hinged doors, interaction prompts, and exploration tracking.
- Door obstruction checks to stop the door swing when the player is in its path.
- Basic-shape furniture placeholders and a dedicated prototype surface shader/rendering setup.
- Editor commands under `Anaphora > Sprint 1` for opening or creating the house scene.

### Added - Combined house and sanity

- Sanity and world switching integrated into the house scene with one player and camera.
- Monochrome Illusion surfaces and colored Reality surfaces.
- Study designated as a temporary pressure test area; it is not an enemy encounter.
- World changes preserve player position, door states, and exploration progress.
- A shared reset for the house walkthrough and sanity state.

### Added - Sprint 2: First Reality shift

- A broken family picture in the Living Room, built from basic shapes.
- A common interaction interface for doors and the picture.
- Reality shifting starts locked in the house.
- Examining the picture with E unlocks shifting and immediately triggers the first Reality shift.
- Q permits manual switching after discovery, subject to sanity and cooldown rules.
- Re-examining the picture does not refill sanity or trigger another first shift.
- Discovery remains unlocked after forced returns; R resets discovery and the sanity system.
- Objective text, locked-state guidance, and updated controls in the house HUD.

### Changed - Project organization and Unity version

- Upgraded the 3D copy to Unity `6000.5.4f1`, including Unity's package/settings migration.
- Moved original game content into `Assets/Anaphora2D`.
- Moved the house and sanity prototype into `Assets/Anaphora3D`.
- Kept shared packages, rendering settings, and third-party dependencies at their existing top-level locations.
- Preserved 1,382 existing asset GUIDs during folder organization and updated build scene paths.

### Validation

- 16 sanity-rule checks passed.
- House checks passed for all six closed-door barriers, all six open-door capsule paths, and the player spawn.
- 10 combined-house Play Mode checks passed for appearance changes, forced returns, persistence, reset, and pausing.
- 13 Sprint 2 Play Mode checks passed for the locked start, interaction raycast, discovery, manual switching, persistence, and replay after reset.
- Play Mode checks ran in background mode; visual presentation and feel still require human playtesting.

### Remaining scope

- Cross-world investigation puzzle, inventory progression, enemy AI, combat, player health, complete story sequence, final art/audio, and a submission build.
- Current world appearances are prototype treatments; the proposed illustration/scribble deterioration and realistic art direction are not complete.
- Progress currently persists between world switches during the play session. Save/load for the new 3D progression has not been implemented.
