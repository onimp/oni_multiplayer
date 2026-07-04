# Multiplayer Sync Status

Living status of what is synchronized between host and connected clients. The mod is
**host-authoritative**: the host runs the real game, clients suppress their own AI/decisions and
replay the host's actions as commands. A **daily hard-sync** (full `.sav` transfer + reload, see
`WorldManager.Sync()`) papers over any residual drift once per cycle.

Legend: ✅ done · 🟡 partial / in progress · 🔴 not synced (known gap) · ⚪ out of scope

_Last updated: 2026-07-04._

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

> **Priority lanes:** traffic is split across three Steam priority lanes (`NetworkLane`) so bulk data
> can no longer head-of-line-block gameplay:
> - **Gameplay** (lane 0, highest, reliable) — movement, pause, chores, tool orders, config. Default
>   for every command.
> - **Sim** (lane 1, low, **unreliable**) — `SyncSimCells`; idempotent + re-sent, so dropped packets
>   self-heal. Chunks are kept to a single message (must not fragment).
> - **Bulk** (lane 2, lowest, reliable) — save transfer (`LoadWorld`) + `SyncWorldDebugSnapshot`.
>
> Lanes are set via `ConfigureConnectionLanes`; sends go through `SteamNetworkingMessageSender`
> (`AllocateMessage` + flat `SendMessages`). If lanes can't be configured — **or if any individual
> native lane send reports failure** — the transport falls back to `SendMessageToConnection` but still
> applies each lane's reliability flag (so unreliable-sim keeps working; only cross-lane prioritisation
> is lost). The sender checks the `SendMessages` result and only reports success when Steam actually
> queued the message, so a broken native path degrades gracefully instead of silently dropping gameplay
> commands (which would show up as teleporting duplicants). Look for a one-time
> `Priority-lane send failed …` / `ConfigureConnectionLanes failed …` warning in the log to tell whether
> lanes are actually active.

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

## Duplicant internal state (host-authoritative streaming)

Axis B — the numbers/flags each machine simulates independently on the dupe. Streamed host→client and
stomped each ~1 s tick by `DuplicantStateSynchronizer` + `SyncDuplicantState` (reliable Gameplay lane,
all live dupes per tick, resolved by `MinionIdentity` `ComponentReference`).

| State | Status | Notes |
|---|---|---|
| Sicknesses / diseases (food poisoning, slimelung…) + cure % | 🟡 | First cut (2026-07-04, builds, not live-tested). `Infect`/`Cure`/`SetPercentCured` diff, guarded by `Database.Sicknesses.IsValidID`. |
| Effects (buffs/debuffs) + remaining time + immunities | 🟡 | First cut. `Add`/`Remove` + `timeRemaining` diff, `AddImmunity`/`RemoveImmunity`, guarded by `Db.Get().effects.Exists`. |
| Health / HitPoints + incapacitation | 🟡 | First cut. Raw-set `hitPoints` then `OnHealthChanged(0f)` to recompute state/wounds/bar. |
| Vitals (Calories / Stress / Stamina / Bladder / Breath) | 🔴 | Deferred — cosmetic on the client (its AI is suppressed); self-heal at hard-sync. |

## Work results (host-authoritative replication)

Rather than reproduce the whole labor+materials economy deterministically, the host replays the
**result** of completed work to clients — either by result-replicating it (dig, building) or, where
re-running it on the client is harmful (double-produces / fights a stream / NREs), by **suppressing the
client's side effect** and letting the host stay authoritative (mop, harvest, toilet — Task #2 Phase A).

| Result | Status | Notes |
|---|---|---|
| Dig (terrain removal) | ✅ | `DigSynchronizer` patches `WorldDamage.DestroyCell`; client replays via `SyncDugCell`. Idempotent. |
| Building completion | 🟡 | `ConstructionSynchronizer` on `Constructable.FinishConstruction`; client spawns finished building (`Def.Build`, null storage) + removes ghost via `SyncBuildingComplete`. First cut — see gaps. |
| Deconstruct | 🟡 | Host patches `Deconstructable.OnCompleteWork`; client removes building at cell via `SyncDeconstruct`. First cut. |
| Mop (liquid removal + bottle) | ✅ | **Suppressed** on client (`MopSynchronizer` prefixes `Moppable.MopCell` → `false`, Task #2 Phase A). Host consumes the mass + bottles the liquid; the client's liquid removal arrives via the fluid stream and the `Moppable` self-destructs once the streamed cell goes dry. The bottled `SubstanceChunk` is **spawn-replicated** to the client (Phase B: `OnCellMopped` postfix → `SyncSpawnPickupable.LiquidChunk`). |
| Harvest (crop pickup) | ✅ | **Suppressed** on client (`HarvestSynchronizer` prefixes `Crop.SpawnSomeFruit` → `false`, Task #2 Phase A). Stops double/mismatched food + mutation-roll drift; host spawns the crop and **spawn-replicates** it (Phase B: `SpawnSomeFruit` postfix → `SyncSpawnPickupable.Prefab`). Mutation genetics not replicated (hard-sync backstop). |
| Toilet flush | 🟡 | **Suppressed** on client (`ToiletFlushSynchronizer` prefixes `Toilet.FlushMultiple` → `false`). Fill level + polluted-dirt output not yet replicated (Phase C). |

## Core simulation (cells)

| Buffer | Status | Notes |
|---|---|---|
| Element / Temperature / Mass / Disease | 🟡 | `SimStateSynchronizer` streams host `Grid` buffers in round-robin chunks (full map ~16 s) on the unreliable Sim lane; client applies `SimMessages.ModifyCell`. **Fluid cells only** — `SyncSimCells` skips any cell where the incoming or current element is solid, so it can never add/remove terrain (that's owned by Dig/Construction sync). Prevents a stale sim chunk on the slow lane from re-solidifying a just-dug cell. The fix for gas/oxygen desync. |
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

- 🟡 **Produced pickupables** — the host now spawn-replicates *newly produced* loose pickupables (mop
  bottle, harvested crop) to clients via `SyncSpawnPickupable` (anonymous, no shared id). This does **not**
  extend to moving existing items (sweep) or storage — see below.
- 🔴 **Materials economy** — fetch / deliver / storage contents are **not** synced. Root cause of the
  "3 of 5 ladders": on the client the Constructable's storage is empty, so `OnCompleteWork` bails
  ("uhhh this constructable is about to generate a nan"). The building-completion replication above
  works around this by spawning the finished building directly rather than relying on client storage.
- 🔴 **Identity of client-spawned completions** — buildings the client spawns on completion do not
  share the host's `MultiplayerId`, so later id-based syncs targeting those specific buildings may not
  resolve. Fine for passive buildings (ladders, tiles); a gap for configurable ones.
- 🔴 **Research, schedules, per-dupe assignments/priorities, rockets/space.**
- 🟡 **Chore retry-storm** — construction chores whose target isn't resolvable on the client
  (`CreateChore … not found` → `SetDriverChore … not found`). **Storm defused (2026-07-04):**
  `ObjectNotFoundException` is the designed graceful-skip path, so `CommandExceptionHandler` now logs it at
  Debug and skips the full object-table dump (previously Warning + dump on *every* retry). Host chores are
  removed from the registry on cleanup (`ChoresPatcher.ChoreCleanup`), so there's no registration leak.
  Remaining root cause: the target ghost has no shared identity on the client (see below) — the client just
  can't mirror that specific chore, which is harmless now that work *results* are result-replicated.
- ⚪ **Critters** — explicitly out of scope for now.

## Roadmap (rough order)

1. Validate sim-state replication (gas/oxygen) live; tune chunk size / cadence.
2. Validate building + deconstruct completion (ladders) live.
3. Fix the chore retry-storm (unresolved construction targets).
4. Fill-in duplicant chores by measured drift: Eat, Sleep, Fetch/Deliver, Mop/Harvest.
5. Shared identity for client-spawned buildings (enables config sync on them).
