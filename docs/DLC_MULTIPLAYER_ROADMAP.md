# DLC Multiplayer PR Roadmap

This branch tracks work needed to make Oni Multiplayer reviewable upstream while adding DLC-compatible multiplayer support and improving the player experience.

## PR baseline

- Upstream: <https://github.com/onimp/oni_multiplayer>
- Working branch: `dlc-multiplayer-maintenance`
- Base branch: upstream `main-ai`
- Personal fork policy: temporary PR carrier only, not a long-lived distribution line.

## Current evidence

- The project README now labels DLC support as preview and requires runtime smoke testing before calling it stable.
- `src/MultiplayerMod/MultiplayerMod.csproj` now targets current Klei metadata without a default root-level `supportedContent` restriction.
- Klei's current `mod_info.yaml` guidance uses optional `requiredDlcIds` and `forbiddenDlcIds` for DLC restrictions; `supportedContent` is deprecated as of U55 / March 2025 and should only be used for archived builds targeting older game versions.
- The current hard-sync model sends a full save to clients through `WorldManager.Sync()`, then reloads it client-side.
- The current debug drift snapshot hashes global grid arrays and chore/state-machine state, so DLC multi-world and cluster systems need special attention.
- Detailed research notes are tracked in [DLC Multiplayer Research Notes](DLC_MULTIPLAYER_RESEARCH.md).

## Maintenance principles

1. Keep Vanilla working while DLC support is added.
2. Prefer runtime capability checks over unconditional DLC assumptions.
3. Keep the host authoritative for commands, save sync, and resync decisions.
4. Treat DLC support as a compatibility matrix, not a manifest-only change.
5. Improve UX around lobby readiness, version mismatches, resync progress, and recoverable desyncs before calling the fork playable.

## Workstreams

### 1. Build and packaging

- Keep current `mod_info.yaml` build properties for `minimumSupportedBuild`, `version`, `APIVersion`, and optional DLC restrictions.
- Keep legacy `supportedContent` available only for archived builds targeting older ONI versions.
- Do not publish DLC compatibility metadata until the DLC smoke matrix passes.
- Update release packaging to make the generated `mod_info.yaml` visible and easy to audit.
- Document local `Directory.Build.props.user` overrides for custom Steam library paths.

### 2. DLC startup compatibility

- Launch with Spaced Out! enabled and collect Harmony patch failures from `Player.log`.
- Audit patches that touch world loading, colony diagnostics, research, schedules, priorities, side screens, and tools.
- Wrap optional or moved DLC targets with existing compatibility helpers where possible.
- Add a small runtime compatibility report to the multiplayer diagnostics overlay.

### 3. Multi-world and cluster synchronization

- Audit every command that assumes `ClusterManager.Instance.activeWorld` or a single active asteroid.
- Include world identity in commands that operate on grid cells, buildings, chores, diagnostics, and overlays.
- Extend debug snapshots toward per-world/cluster drift reporting instead of one global error count.
- Verify hard sync preserves rockets, asteroids, space POIs, teleporter state, and world focus.

### 4. Save/load and resync reliability

- Validate cloud and local save paths with DLC saves.
- Measure full-save payload size and fragmentation behavior for larger DLC colonies.
- Transfer full saves as bounded chunks with size and checksum validation.
- Add clearer status messages for pause, save capture, transfer, load, and resume phases.
- Add a manual "request resync" path that is safe for non-host players to trigger through the host.

### 5. DLC gameplay command coverage

- Re-test existing synced UI and tool commands under DLC.
- Add or repair commands for DLC-specific screens and interactions, especially rockets, starmap, teleporters, radbolts, and multi-asteroid colony controls.
- Keep command serialization deterministic and version-tolerant.
- Add targeted tests for any new command arguments or surrogate serializers.

### 6. Multiplayer experience upgrades

- Show host/client game build, DLC mode, mod version, and ready state in the lobby/wait UI.
- Detect incompatible mod or game versions before loading a save.
- Improve cursor labels for players on different screens or worlds.
- Add better disconnect, reconnect, and resync messaging.
- Track latency/packet loss enough to explain whether lag is network, save transfer, or simulation drift.

## First implementation candidates

1. Add a non-invasive compatibility fingerprint that reads game build, DLC/content mode, active save DLC IDs, mod list, and generated mod version.
2. Generate current Klei `mod_info.yaml` metadata without default DLC restrictions.
3. Transfer full saves in chunks so DLC colonies are not limited by one large command payload.
4. Add DLC smoke-test notes and log parsing scripts for Harmony failures.
5. Extend debug snapshots with world IDs before modifying gameplay sync behavior.
