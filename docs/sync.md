# Multiplayer Sync Status

Living status of what is synchronized between host and connected clients. The mod is
**host-authoritative**: the host runs the real game, clients suppress their own AI/decisions and
replay the host's actions as commands. A **daily hard-sync** (full `.sav` transfer + reload, see
`WorldManager.Sync()`) papers over any residual drift once per cycle.

Legend: ✅ done · 🟡 partial / in progress · 🔴 not synced (known gap) · ⚪ out of scope

_Last updated: 2026-07-03._

---

## Core infrastructure

| Area | Status | Notes |
|---|---|---|
| Object identity (`MultiplayerId`) | ✅ | Every registered instance gets a network id; references resolve by id on both sides. |
| Command pipeline | ✅ | `[Serializable]` `MultiplayerCommand`s auto-discovered; BinaryFormatter + Steam networking with 512 KiB fragmentation. |
| Reference resolution safety | ✅ | Missing objects throw `ObjectNotFoundException` (logged + skipped) instead of crashing the batch. |
| Daily hard-sync (full reload) | ✅ | Authoritative safety net; corrects everything but only once per cycle. |
| Cursor / player presence | ✅ | Remote cursors and player state overlay. |
| Per-duplicant sync inspector (`DevToolDuplicantSync`) | ✅ | DEBUG dev tool: diffs each dupe host-vs-local by id. |
| Manual force save + sync | ✅ | Dev tools → Multiplayer/Controls: host-only button runs the end-of-day save + hard-sync on demand (`DevToolMultiplayerControls`). |
| World divergence metric | ✅ | `WorldDebugSnapshotComparator` hashes grid buffers every 30 s → the "N errors" diagnostic. |

> **Latency caveat:** all commands share one reliable, in-order Steam stream, so bulk traffic (sim
> stream, save transfer) head-of-line-blocks latency-sensitive gameplay. The sim stream is kept
> deliberately light (~11 KB/s) to avoid this. Tight-sync-without-lag needs a lower-priority network
> lane for bulk traffic — not yet done.

## Duplicants (chores)

| Behavior | Status | Notes |
|---|---|---|
| Chore creation + assignment framework | ✅ | Host creates chore → `CreateChore` → client rebuilds w/ same id; client native chore selection blocked. |
| Idle / wander | ✅ | `IdleChoreSynchronizer` (+ `IdleStatesSynchronizer`). |
| MoveToSafety | ✅ | `MoveToSafetyChoreSynchronizer`. |
| Attack | ✅ | `AttackChoreSynchronizer`. |
| Death | ✅ | `DeathMonitorSynchronizer`. |
| Pee | ✅ | `PeeChoreSynchronizer`. |
| Generic work (`WorkChore<T>`: dig/build/sweep/deliver/mop/harvest/operate/research/cook…) | 🟡 | Recognized structurally (`ChoresPatcher.IsWorkChore`), assigned to same dupe, position-synced via `WorkChoreSynchronizer`. **Work timing not gated** — each side runs the labor independently. |
| Eat / Sleep / Recreation / Mingle | 🔴 | No synchronizer; drift hidden by hard-sync. |

## Work results (host-authoritative replication)

Rather than reproduce the whole labor+materials economy deterministically, the host replays the
**result** of completed work to clients.

| Result | Status | Notes |
|---|---|---|
| Dig (terrain removal) | ✅ | `DigSynchronizer` patches `WorldDamage.DestroyCell`; client replays via `SyncDugCell`. Idempotent. |
| Building completion | 🟡 | `ConstructionSynchronizer` on `Constructable.FinishConstruction`; client spawns finished building (`Def.Build`, null storage) + removes ghost via `SyncBuildingComplete`. First cut — see gaps. |
| Deconstruct | 🟡 | Host patches `Deconstructable.OnCompleteWork`; client removes building at cell via `SyncDeconstruct`. First cut. |

## Core simulation (cells)

| Buffer | Status | Notes |
|---|---|---|
| Element / Temperature / Mass / Disease | 🟡 | `SimStateSynchronizer` streams host `Grid` buffers in round-robin chunks; client applies `SimMessages.ModifyCell`. First cut, untested — the fix for gas/oxygen desync. |
| Liquid/gas flow | 🟡 | Follows from the above (element+mass per cell). |

## Player tools / building interaction

| Action | Status | Notes |
|---|---|---|
| Dig orders | ✅ | Drag-tool events synced. |
| Build orders (placement) | ✅ | `BuildEvents` → `Build` command places same ghost on client. |
| Utility build (wire/pipe) | ✅ | `BuildUtility` / `BuildWire`. |
| Deconstruct / cancel / priority orders | ✅ | Drag-tool + `ChangePriority`. |
| Building config (doors, valves, fabricator queues, filters, thresholds, sliders, automation, receptacles…) | ✅ | Broad coverage via `ObjectEvents` component-method patches. |

## Known gaps / not synced

- 🔴 **Materials economy** — fetch / deliver / storage contents are **not** synced. Root cause of the
  "3 of 5 ladders": on the client the Constructable's storage is empty, so `OnCompleteWork` bails
  ("uhhh this constructable is about to generate a nan"). The building-completion replication above
  works around this by spawning the finished building directly rather than relying on client storage.
- 🔴 **Identity of client-spawned completions** — buildings the client spawns on completion do not
  share the host's `MultiplayerId`, so later id-based syncs targeting those specific buildings may not
  resolve. Fine for passive buildings (ladders, tiles); a gap for configurable ones.
- 🔴 **Research, schedules, per-dupe assignments/priorities, rockets/space.**
- 🔴 **Chore retry-storm** — construction chores whose target never replicated loop forever
  (`CreateChore … not found` → `SetDriverChore … not found`). Separate open bug.
- ⚪ **Critters** — explicitly out of scope for now.

## Roadmap (rough order)

1. Validate sim-state replication (gas/oxygen) live; tune chunk size / cadence.
2. Validate building + deconstruct completion (ladders) live.
3. Fix the chore retry-storm (unresolved construction targets).
4. Fill-in duplicant chores by measured drift: Eat, Sleep, Fetch/Deliver, Mop/Harvest.
5. Shared identity for client-spawned buildings (enables config sync on them).
