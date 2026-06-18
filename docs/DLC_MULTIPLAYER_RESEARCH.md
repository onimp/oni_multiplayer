# DLC Multiplayer Research Notes

Research snapshot date: 2026-06-18.

This document gathers the external and local facts needed before implementing DLC-compatible multiplayer support.

## Executive conclusions

1. The old `supportedContent: VANILLA_ID,EXPANSION1_ID` path is not the right primary target for current ONI. Klei marks `supportedContent` deprecated as of U55 / March 2025. Current mod packaging should use `minimumSupportedBuild`, `version`, `APIVersion`, and optional `requiredDlcIds` / `forbiddenDlcIds`.
2. DLC support is now a DLC-combination problem, not just "Spaced Out on/off". Current public ONI includes Spaced Out, Frosty Planet Pack, Bionic Booster Pack, Prehistoric Planet Pack, Aquatic Planet Pack, plus cosmetics.
3. The fork should first add a runtime compatibility fingerprint: game build, active DLC IDs in save, loaded/installed DLC IDs, active mod footprint, multiplayer mod version, and protocol version.
4. The first hard blocker is likely not manifest loading, but runtime sync: large DLC save transfer, rockets/starmap, multi-world focus, side screens, object references, and BinaryFormatter serialization of DLC-specific runtime objects.
5. Upstream already has useful evidence in issues and branches, but no complete DLC implementation. The `main-ai` branch contains 2026 API compatibility work; `feature/game-loader` contains headless WorldGen/SimDLL research that may become useful later.

## Official mod metadata facts

Klei's current `mod_info.yaml` guidance:

- `minimumSupportedBuild` is required and controls whether the mod/archive is loaded for a game build.
- `APIVersion: 2` is required for DLL mods.
- `version` is display-only.
- `requiredDlcIds` and `forbiddenDlcIds` can restrict load based on active DLCs.
- Valid examples named by Klei:
  - `EXPANSION1_ID` = Spaced Out!
  - `DLC2_ID` = The Frosty Planet Pack
  - `DLC3_ID` = The Bionic Booster Pack
  - future DLCs should keep the `DLC#_ID` format.
- Klei says `VANILLA_ID` and `ALL` are not valid options for the new `requiredDlcIds` / `forbiddenDlcIds` lists.
- `supportedContent` is deprecated as of U55 / March 2025. It is still supported only for older archived mod versions.

Sources:

- Current Klei mod_info guide: https://forums.kleientertainment.com/forums/topic/158363-setting-up-mod_infoyaml-and-archived_versions/
- Older Spaced Out-era guide, useful only for archived versions: https://forums.kleientertainment.com/forums/topic/126022-setting-up-mod_infoyaml-and-archived_versions/

Implication for this fork:

- Do not implement a current-release "DLC support" switch by only changing `OniSupportedContent`.
- Replace/extend the MSBuild `GenerateModInfo` target to emit the current fields.
- Keep legacy `supportedContent` only if we package archived builds for older ONI versions.
- Use no DLC restriction by default once runtime compatibility is verified, or explicitly require tested DLC IDs for DLC-specific builds.

## Current ONI and DLC landscape

As of this research snapshot, Klei's game update index shows latest release build `737790`, released 2026-06-17.

Current Steam/Klei DLC set relevant to mod compatibility:

- `EXPANSION1_ID`: Oxygen Not Included - Spaced Out!, released 2021-12-16. Adds modular rockets, multi-base/multi-world gameplay, starmap, space travel, and related systems.
- `DLC2_ID`: The Frosty Planet Pack, released 2024-07-18.
- `DLC3_ID`: The Bionic Booster Pack, released 2024-12-12.
- `DLC4_ID`: inferred from Klei's `DLC#_ID` rule and public logs as The Prehistoric Planet Pack, released 2025-06-12. Verify with current `Player.log` before using this as a hardcoded mapping.
- `DLC5_ID`: likely The Aquatic Planet Pack by release order and Klei's `DLC#_ID` rule, released 2026-06-11. Verify with current `Player.log` before using this as a hardcoded mapping.
- `COSMETIC1_ID`: appears in public logs for Neutronium Cosmetics Pack; it should be included in diagnostics but likely not in multiplayer gameplay requirements.

Sources:

- Klei game updates index: https://forums.kleientertainment.com/game-updates/oni-alpha/
- Latest Klei update 737790: https://forums.kleientertainment.com/game-updates/oni-alpha/737790-r2757/
- Steam DLC page: https://store.steampowered.com/dlc/457140/Oxygen_Not_Included/
- Aquatic Planet Pack Steam page: https://store.steampowered.com/app/4310080/Oxygen_Not_Included_The_Aquatic_Planet_Pack/
- Aquatic Planet Pack Klei launch FAQ: https://forums.kleientertainment.com/forums/topic/172089-the-aquatic-planet-pack-is-now-available-update-klei-fest-2026/
- Public log showing `DLC4_ID` and `COSMETIC1_ID`: https://forums.kleientertainment.com/forums/topic/171550-were-meteor-showers-removed-in-the-dlc-or-did-the-mechanics-change/

Implication for testing:

- Treat "DLC support" as a matrix:
  - Base game only
  - Spaced Out only
  - Spaced Out + planet packs
  - Each planet pack without Spaced Out where Klei allows it
  - Multiple DLCs installed but inactive in save
  - Save upgraded from base game into a DLC-enabled save

## Runtime compatibility data to capture

The mod needs a small compatibility model before multiplayer join/load:

- ONI build/revision and branch string.
- Multiplayer mod assembly informational version.
- Multiplayer protocol version.
- Loaded/installed DLC IDs from `DlcManager`.
- Active DLC IDs in the save/game from the active game context.
- Active Klei mods and their static IDs, versions, and enabled state.
- Whether the host and client are running the same content fingerprint.

Klei's current mod guide says DLC-specific logic should use `DlcManager` for installed DLC checks and `Game` for DLC active in save checks when packaging fields are not expressive enough.

Existing upstream issue: https://github.com/onimp/oni_multiplayer/issues/7

Implementation target:

- Add a serializable `CompatibilityFingerprint`.
- Send it in the first host/client handshake, before world save transfer.
- Refuse or warn on mismatches with a precise message.
- Log it at runtime startup and expose it in the multiplayer status overlay.

## Upstream repository facts

Repository:

- `onimp/oni_multiplayer`
- License: MIT
- Default branch: `main`
- Latest upstream push observed: 2026-03-23
- Latest release: `v1.1.3-alpha`, published 2024-12-20.

Branches:

- `main`: current normal mod line.
- `1.x`: older release maintenance line.
- `main-ai`: includes 2026 API compatibility and dedicated server scaffolding.
- `feature/game-loader`: open PR for real WorldGen + SimDLL physics in a dedicated server.
- `feature/work-chores`, `feature/world-entities`, `research/dupes-sync-via-preconditions`: potential references for sync/domain work.

Important upstream issues:

- #7: Compare enabled DLCs and mods upon join.
- #84: Sync starmap screen.
- #169: Sync remaining side screens; body says remaining screens include DLC-related screens.
- #353: Client cannot load saves larger than 10 MiB; likely worse with DLC colonies.
- #368: Multiplayer with DLC request; points to the Spaced Out support milestone.
- #376: DLC2/Frosty startup crash; shows `RequireExecutionLevelAttribute.CheckExecutionLevel` null reference during preload.
- #379: Crash when firing a rocket; BinaryFormatter deserialization `TargetParameterCountException`, build U54-652372.
- #383/#385: Dedicated server / game loader research.

Useful upstream PRs:

- #382: `Fix build and test compatibility with current ONI version`, merged to `main-ai`, not `main`. It updates current-game API calls, test runner setup, and excludes dev-tool files not present in the installed ONI version.
- #385: `feat: real WorldGen + SimDLL physics for dedicated server`, open against `main-ai`. Useful for later headless/replay testing, not required for initial in-game DLC support.

## Local code audit

### Packaging

Files:

- `src/MultiplayerMod/MultiplayerMod.csproj`
- `src/MultiplayerMod/Directory.Build.targets`

Current state:

- `OniSupportedContent` is `VANILLA_ID`.
- `GenerateModInfo` emits deprecated `supportedContent`.
- `OniMinimumSupportedBuild` is still `577063` on the current fork line.

Needed:

- Add current mod_info fields.
- Decide whether to rebase/cherry-pick `main-ai` compatibility before bumping build metadata.
- Do not mark DLC-compatible until runtime smoke tests pass.

### Load and hard sync

Files:

- `src/MultiplayerMod/Multiplayer/World/WorldManager.cs`
- `src/MultiplayerMod/Game/World/SaveLoaderEvents.cs`

Current state:

- Host pauses, saves active save file, sends full `WorldSave`, client writes it to local/cloud save path and calls `LoadScreen.DoLoad`.
- This model may work as a coarse DLC fallback, but larger DLC saves and multi-world state will stress transfer, progress UI, and mismatch recovery.

Needed:

- Measure DLC save size.
- Stream or chunk save transfer robustly with progress and retry.
- Include save header metadata in the compatibility handshake.
- Validate active DLCs before loading the host save on clients.

### Network serialization and payload size

Files:

- `src/MultiplayerMod/Platform/Steam/Network/Configuration.cs`
- `src/MultiplayerMod/Platform/Steam/Network/Messaging/NetworkMessageFactory.cs`
- `src/MultiplayerMod/Platform/Steam/Network/Messaging/NetworkMessageProcessor.cs`
- `src/MultiplayerMod/Platform/Steam/Network/Messaging/NetworkSerializer.cs`
- `src/MultiplayerMod/Platform/Steam/Network/Messaging/Surrogates/*`

Current state:

- Steam send buffer defaults to 10 MiB.
- Messages are fragmented at 512 KiB.
- Reassembly assumes fragments arrive in order; `FragmentsBuffer.Append` places each received fragment at the next index rather than carrying a fragment index.
- Final reassembly deserializes a buffer sized to `count * MaxFragmentDataSize`, so trailing zero bytes are possible.
- Runtime uses `BinaryFormatter`, which is brittle and unsafe for untrusted inputs. For this Steam-friends-only prototype it is existing architecture, but DLC runtime types increase break risk.

Needed:

- Add fragment sequence numbers and actual total payload size.
- Make large save transfer explicit instead of piggybacking on generic command serialization.
- Add surrogates for DLC-specific objects only after failing commands are identified.
- Add a protocol version field before changing wire format.

### Multi-world and DLC gameplay assumptions

Files:

- `src/MultiplayerMod/Multiplayer/Objects/Reference/GridReference.cs`
- `src/MultiplayerMod/Multiplayer/Commands/Alerts/ChangeRedAlertState.cs`
- `src/MultiplayerMod/Game/UI/Screens/Events/MeterScreenEvents.cs`
- `src/MultiplayerMod/Multiplayer/World/Debug/WorldDebugSnapshot.cs`
- `src/MultiplayerMod/Game/Mechanics/Objects/ObjectEvents.cs`

Current state:

- `GridReference` stores only `Cell` and `Layer`; no world ID.
- Red alert paths use `ClusterManager.Instance.activeWorld`.
- Debug drift snapshot hashes global `Grid` arrays and reports one global error count.
- `ObjectEvents` patches rocket-related types (`PassengerRocketModule`, `RocketControlStation`, `CraftModuleInterface`, `LaunchConditionManager`) but the open rocket crash suggests serialization/execution is not robust.

Needed:

- Add world ID/focus context to grid and object commands.
- Audit all commands that resolve from `Grid.Objects[cell, layer]`.
- Extend debug snapshots to group drift by world ID, and include rocket/interior worlds.
- Retest rocket launch, starmap, crew boarding, rocket modules, POIs, teleporters, radbolts, and DLC buildings.

### UI and player experience

Current state:

- Existing status overlay can show text during startup/sync/load.
- Existing cursor labels show player screen names, but not player world/focus.
- Several upstream issues already request better wait/load/network diagnostics.

Needed:

- Show host/client build, DLCs, mod version, ready state.
- Add a clear mismatch dialog before save transfer.
- Show save transfer progress and throughput.
- Add manual host-mediated resync request.
- Add cursor world/screen labels for multi-world play.

## Build and local environment needs

Minimum local materials:

- Visual Studio 2022 or a .NET SDK/MSBuild setup that can build SDK-style `net48` projects.
- .NET Framework 4.8 targeting pack/reference assemblies.
- ONI installed locally.
- ONI managed assemblies:
  - `OxygenNotIncluded_Data/Managed/Assembly-CSharp.dll`
  - `Assembly-CSharp-firstpass.dll`
  - Unity and Steamworks DLLs referenced in the csproj.
- `Directory.Build.props.user` when ONI is not under `C:\Program Files (x86)\Steam`.
- Two Steam accounts / two machines or two isolated game installs for real multiplayer testing.

Current local machine gaps observed on 2026-06-18:

- `dotnet --info` reports no installed .NET SDK.
- `where msbuild` did not find MSBuild.
- Default Steam path does not contain `Assembly-CSharp.dll`.

Known upstream build command from issue #353:

```bash
dotnet restore
dotnet build --disable-build-servers src/MultiplayerMod/MultiplayerMod.csproj --configuration Release
```

On Windows, `Debug` configuration auto-installs to the local dev mods directory.

## Validation matrix

Static/build gates:

1. `dotnet restore`
2. `dotnet build --disable-build-servers src/MultiplayerMod/MultiplayerMod.csproj -c Release`
3. `dotnet test --disable-build-servers src/MultiplayerMod.Test/MultiplayerMod.Test.csproj -c Release` after test runner setup is available.
4. Inspect generated `mod_info.yaml`.

Runtime single-player smoke:

1. Base game only, mod loads.
2. Spaced Out enabled, mod loads.
3. Frosty only, if installed and save supports it.
4. Bionic only, if installed and save supports it.
5. Prehistoric only, if installed and save supports it.
6. Aquatic only, if installed and save supports it.
7. Mixed DLC save, mod loads.

Runtime multiplayer smoke:

1. Host and client identical base-game setup.
2. Host and client identical Spaced Out setup.
3. Host and client identical mixed-DLC setup.
4. Intentional DLC mismatch is detected before save transfer.
5. Intentional mod mismatch is detected before save transfer.
6. Large DLC save transfer over 10 MiB succeeds with progress.
7. Rocket launch and starmap actions do not crash.
8. Multi-world cell/object commands execute on the intended world.

## Recommended next implementation order

1. Decide base branch: either continue from `main` and cherry-pick #382 selectively, or reset maintenance onto `main-ai` if dedicated-server scaffolding is acceptable noise.
2. Add current `mod_info.yaml` generation fields while preserving old fields only for archived builds.
3. Add `CompatibilityFingerprint` and log/display it locally.
4. Add handshake exchange and mismatch blocking for DLC/mod/build/version.
5. Fix large save transfer protocol.
6. Add world-aware references and command context.
7. Triage rocket/starmap/side-screen DLC commands one by one.
8. Build the DLC smoke matrix into repeatable scripts/checklists.
