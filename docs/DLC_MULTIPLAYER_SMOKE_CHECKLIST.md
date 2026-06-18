# DLC Multiplayer Smoke Checklist

Use this checklist before describing the branch as playable with DLC.

## Setup

- Build `MultiplayerMod` in `Release` or `Publish`.
- Install the generated `mod.yaml`, `mod_info.yaml`, and `MultiplayerMod.dll` into a local ONI mod folder on both machines.
- Confirm generated `mod_info.yaml` includes `minimumSupportedBuild`, `version`, and `APIVersion`, and does not include `supportedContent` unless explicitly building an archived legacy package.
- Use the same ONI build, same DLC selection, and same enabled mod list on host and client.

## Compatibility Gate

- Host with matching DLC/mods: client reaches world sync.
- Remove one DLC or active mod on the client: join is rejected before world load with a compatibility message.
- Change the multiplayer mod build on one side: join is rejected before world load.

## Save Transfer

- Start with a new vanilla colony and verify hard sync loads on the client.
- Start with a Spaced Out colony with multiple worlds and verify hard sync loads on the client.
- Test a large colony save over 10 MiB and verify transfer completes without hanging.
- Confirm the client overlay shows save receive progress and then a verification/loading phase.
- Interrupt a client during transfer and confirm the host remains usable.

## DLC Risk Areas

- Open starmap and rocket screens during multiplayer and confirm the mod does not crash.
- Trigger rocket launch-related UI and confirm unsafe calls show a `DLC multiplayer preview` notification and are blocked rather than serialized.
- Toggle red alert while focused on a secondary world and confirm the target world is preserved or safely falls back.
- Move between asteroids and confirm remote player cursor labels indicate when another player is on a different world.
- Build, dig, mop, copy settings, stamp, and move-to-location on a secondary asteroid; confirm commands either apply to the intended world or are skipped with a clear log/notification.
- Change a RailGun launch mass on a secondary asteroid and confirm a null or cross-world target does not crash.
- Watch `Player.log` for Harmony patch failures, serialization exceptions, or checksum errors.
