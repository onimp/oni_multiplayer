# ONI Multiplayer — Sync Roadmap (pick-up plan)

Living plan for continuing the sync work. Networking (priority lanes) is done; this file tracks the
gameplay-sync work that comes next. Companion status doc: [docs/sync.md](docs/sync.md).

_Last updated: 2026-07-04._

---

## Mental model (read this first)

A duplicant diverges along **two independent axes**, and they need **different** fixes:

- **Axis A — Behavior (what the dupe is *doing*):** chores/movement. Handled by **command replay**.
  The host decides; the client is a *puppet* — its own chore-selection AI is suppressed
  ([`MultiplayerDriverChores`](src/MultiplayerMod/Multiplayer/Chores/Driver/MultiplayerDriverChores.cs)
  preconditions) and it replays the host's `CreateChore` / `SetDriverChore` / `MoveTo`. It runs each
  supported chore's state machine **locally** (so it walks via its own `Navigator`).
- **Axis B — Internal state (the *numbers* on the dupe):** needs, germs, disease, effects, health.
  **Not synced at all** — each machine simulates them independently; only the daily hard-sync corrects.

Key consequences:
- Because the client's decision AI is suppressed, most **needs** (calories, bladder, stress, stamina)
  are effectively **cosmetic** on the client and self-heal at hard-sync → **low priority** (food included).
- **Germs / disease / effects / health** locally simulate into **visible outcomes** (sickness, vomiting,
  debuffs, death) → drift here is real divergence → **worth syncing**.
- **Work side-effects that produce/consume resources** (dig output, building, toilet flush, mopping,
  sweeping, harvesting) must be **host-authoritative and result-replicated**, never run twice. Running
  them on the client double-produces and often NREs on client-spawned buildings.

Two reusable patterns already in the codebase:
- **State streaming** (host round-robins buffers → client stomps): `SimStateSynchronizer` +
  `SyncSimCells`. Idempotent, unreliable Sim lane.
- **Result replication** (host patches a completion seam → sends a command → client applies the result):
  `DigSynchronizer`+`SyncDugCell`, `ConstructionSynchronizer`+`SyncBuildingComplete`/`SyncDeconstruct`.
- **Client suppression** (stop the client from re-running a host-authoritative side effect): Harmony
  prefix returning `false` on the client, e.g.
  [`ToiletFlushSynchronizer`](src/MultiplayerMod/Multiplayer/World/ToiletFlushSynchronizer.cs) (below).

---

## Recently done (this session)

- **Netcode priority lanes** — 3 Steam lanes (Gameplay/Sim/Bulk); custom `SteamNetworkingMessageSender`.
  Hardened: it now checks the `SendMessages` result and **falls back to flat `SendMessageToConnection`**
  on any failure, so a broken native path degrades gracefully instead of silently dropping gameplay
  (which showed up as teleporting dupes). Watch the log for a one-time `Priority-lane send failed …` /
  `ConfigureConnectionLanes failed …` to know whether lanes are actually active.
- **Dig flicker fixed** — `SyncSimCells` now skips solid cells, so the fluid sim can never add/remove
  terrain (that races Dig/Construction sync and re-solidified just-dug cells).
- **`ComponentReference.Resolve()`** — throws `ObjectNotFoundException` (logged + skipped) instead of NRE.
- **Toilet flush crash fixed** — `ToiletFlushSynchronizer` suppresses `Toilet.FlushMultiple` on the
  client (host-authoritative production; was NRE-ing on client-spawned toilets + double-producing germs).

Open follow-up from the toilet fix: the toilet **fill level** and **polluted-dirt output** are no longer
produced on the client at all → client toilet won't visually fill / need emptying until hard-sync. Fold
proper toilet-fill + output replication into task #2. Duplicant germ gain from the toilet is covered by #1.

---

## Task #1 — Duplicant health / germs / effects sync ✅ FIRST CUT BUILT (2026-07-04)

**Status:** first cut implemented + compiling; **not yet live-tested** (2-PC). Scope landed = Sicknesses +
Effects (+immunities) + Health/HitPoints; Vitals deferred (see below). Files:
[`DuplicantStateSynchronizer`](src/MultiplayerMod/Multiplayer/World/DuplicantStateSynchronizer.cs) (host
streamer, all live dupes per ~1 s tick) +
[`SyncDuplicantState`](src/MultiplayerMod/Multiplayer/Commands/Gameplay/SyncDuplicantState.cs) (client
stomp, reliable Gameplay lane), registered in `MultiplayerGameObjectsSpawner`. Remaining: run the live
verification below; fold **Vitals** in as a second pass if measured drift warrants it.

**Goal:** stop the most visible mid-game divergence — a dupe that's sick / poisoned / debuffed / dying on
the host but fine on the client (and vice-versa). This is the "pee sickness and other mid-game things"
bucket.

**Approach:** host-authoritative **state streaming**, same shape as `SimStateSynchronizer`. Host
periodically reads each duplicant's authoritative state and the client stomps it. Reuse the existing
per-dupe identity mapping (`MultiplayerId`) that `DevToolDuplicantSync` /
`WorldDebugSnapshot.CreateDuplicantSnapshots()` already uses to pair host↔client dupes.

**What to replicate (per duplicant, confirm exact APIs in `Assembly-Csharp/`):**
- **Sicknesses / diseases** — active `SicknessInstance`s (Food Poisoning, Slimelung, …). Likely via
  `MinionModifiers.sicknesses` (`Sicknesses` modifier) + germ-exposure counters. *This is the core of
  "pee sickness."*
- **Effects (buffs/debuffs)** — active `Effect`s on the `Effects` component (`ModifierSet` / effect
  instances with remaining time).
- **Vitals** — `HitPoints`/Health, and the threshold-critical amounts that trigger visible behavior:
  `Calories`, `Stress`, `Stamina`, `Bladder`, `Breath`. Read from the dupe's `Amounts`/`AmountInstance`s.
  (Vitals are lower value than sicknesses/effects — can be a second pass.)

**Files to create (mirror `SimStateSynchronizer` + `SyncSimCells`):**
- `Multiplayer/World/DuplicantStateSynchronizer.cs` — host `KMonoBehaviour`, round-robins dupes (or all
  each tick — there are few), gated `Host` + `LevelIsActive(Multiplayer)` + `Clients.Count > 0`, on a
  ~1–2 s cadence. Register in `MultiplayerGameObjectsSpawner` (same as `SimStateSynchronizer`).
- `Multiplayer/Commands/Gameplay/SyncDuplicantState.cs` — `[Serializable]`, carries `MultiplayerId` +
  the state payload; `Execute` resolves the dupe by id and applies. Mark `[MultiplayerCommand(Lane =
  NetworkLane.Sim)]` (idempotent, re-sent) — but only if it stays within one message; otherwise leave it
  on the default reliable Gameplay lane. Small payloads → reliable is fine.

**Application notes (client side):**
- Sicknesses/effects: add missing ones with the host's remaining duration, remove ones the host no longer
  has. Look for the game's add/remove APIs (`sicknesses.Infect(...)` / `Effects.Add`/`Remove`) rather than
  writing fields directly.
- Amounts: `amountInstance.SetValue(hostValue)` (idempotent stomp).
- Guard every resolve with the graceful `ObjectNotFoundException` pattern (dupe may not be replicated yet).

**Verify:** host-side food poisoning / disease appears on the client within a cadence tick; a debuff that
expires on the host expires on the client; no double-application; `DevToolDuplicantSync` on the client
shows the dupe rows in sync. No NREs.

---

## Task #2 — Work-result sync (mopping / sweeping / harvest / production)

> **Phases A + B BUILT (2026-07-04).** _Phase A_ — client double-production suppressed for mop + harvest
> (toilet already done), mirroring `ToiletFlushSynchronizer`:
> [`MopSynchronizer`](src/MultiplayerMod/Multiplayer/World/MopSynchronizer.cs) (`Moppable.MopCell` →
> `false` on client; cuts both the `ConsumeMass` that fights the fluid stream and the bottle spawn) +
> [`HarvestSynchronizer`](src/MultiplayerMod/Multiplayer/World/HarvestSynchronizer.cs)
> (`Crop.SpawnSomeFruit` → `false`; stops double/mismatched food + mutation drift). _Phase B_ — host
> spawn-replicates the produced pickupables via
> [`SyncSpawnPickupable`](src/MultiplayerMod/Multiplayer/Commands/Gameplay/SyncSpawnPickupable.cs)
> (reliable Gameplay lane, `LiquidChunk`/`Prefab` kinds, anonymous): the mop bottle (`OnCellMopped`
> postfix) and the harvested crop (`SpawnSomeFruit` postfix) now appear on the client. Builds; **not yet
> live-tested** (2-PC). Deferred: crop mutation genetics (hard-sync backstop) and **Phase C** (storage/
> economy — sweep, toilet fill/output — the high-frequency, gameplay-risky one; its own task). See
> task-2.md §5/§6.

> **Full design doc: [task-2.md](task-2.md)** (planned 2026-07-04). Read that first — it answers the
> fluid-sync question (fluids already reconcile via the cell stream; the real blocker is discrete
> **pickupables**, not liquids/gas), splits the bucket into "cell-based results" (doable now) vs
> "entity/pickupable results" (needs a spawn-replication primitive), and lists the open design decision.
> The summary below is retained for context.

**Goal:** the "everyday mid-game" actions — mopping, sweeping, harvesting, and building/deconstruct — land
their **result** on the client instead of each side running the labor independently (which drifts or NREs).

**Status of this bucket:**
- Dig ✅ (`DigSynchronizer` + `SyncDugCell`).
- Building/Deconstruct 🟡 (`ConstructionSynchronizer`; first cut — see gaps in docs/sync.md).
- Mop / Sweep / Harvest / other `WorkChore`s 🟡 — chore is recognized + position-synced
  (`WorkChoreSynchronizer`) but the **result** (dirt/liquid removed by mopping, debris swept into storage,
  crop harvested) isn't result-replicated the way dig is, because the materials economy isn't synced.
- Toilet flush — now **suppressed** on client (task from this session); needs proper fill/output
  replication here.

**Approach (per action):** find the host completion seam (usually `Workable.OnCompleteWork` or the
building's specific completion method), and either
1. **Result-replicate** — host sends a command; client applies the concrete result (like dig), **or**
2. **Suppress + replicate** — if the client re-running it is harmful, suppress on the client (prefix
   returning `false`, like `ToiletFlushSynchronizer`) and replicate the host's result.

**Likely order (by visibility):** Mop (liquid removal) → Sweep (debris → storage) → Harvest (crop pickup)
→ toilet fill/output → generic producer buildings. Each is a small, self-contained synchronizer.

**Watch out for:** the **materials economy is not synced** (fetch/deliver/storage contents). Anything that
depends on client storage having the right contents will bail ("about to generate a nan"); prefer
replicating the *result* over reproducing the economy. Client-spawned buildings don't share the host's
`MultiplayerId`, so id-based syncs on them won't resolve (fine for passive results).

---

## Cross-cutting / smaller follow-ups

- **Floor-pee target cell (Axis A gap):** dupes pee in different spots because the pee cell is chosen at
  runtime inside the chore SM, not carried in the command. Fix = host drives the chosen cell + trigger
  (small targeted synchronizer). Lower priority than #1/#2.
- **Eat / Sleep / Recreation / Mingle:** no synchronizer yet; drift hidden by hard-sync. Add by measured
  drift after #1/#2.
- **DevToolDuplicantSync is client-only** by design (compares local vs host snapshot; host has nothing to
  compare against). Use it on the **client** PC.

---

## Build & test

- Build (hard gate): `dotnet build src/MultiplayerMod/MultiplayerMod.csproj -c Debug -m:1 -nologo`
  → 0 errors (5 pre-existing warnings OK). Wipe `lib/exposed` + `dotnet build-server shutdown` only if
  publicizer rules change.
- Tests run via the IDE NUnit runner (CLI test-running not wired).
- Live validation is 2-PC: host + remote peer; peer pastes runtime errors. Decompiled game source under
  `Assembly-Csharp/` is copyright + gitignored — **do not commit it**. Mod builds on **ONI U57-707956-V** only.
