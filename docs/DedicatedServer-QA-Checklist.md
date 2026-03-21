# DedicatedServer — QA Regression Checklist

Visualizer + headless server regression checklist. Update status after each fix/verification.

**Statuses:** 🔴 OPEN · 🟡 IN PROGRESS · ✅ FIXED · ✅ VERIFIED

---

## Test Environment

### Prerequisites
- macOS arm64 (Apple Silicon), Rosetta 2 installed
- ONI installed via Steam
- x86_64 Mono at `~/.mono-x64/6.12.0/`
- .NET SDK 9 at `~/.dotnet/`

### Pipeline (run in order after every code change)

```bash
# 1. Build
cd /Users/zuev93/dev/oni_multiplayer
~/.dotnet/dotnet build src/DedicatedServer/DedicatedServer.csproj -c Debug

# 2. Patch CoreModule + Assembly-CSharp (REQUIRED — both args)
~/.dotnet/dotnet run --project src/PatchInternalCalls/PatchInternalCalls.csproj -- \
  src/DedicatedServer/bin/Debug/net48/UnityEngine.CoreModule.dll.orig \
  src/DedicatedServer/bin/Debug/net48/UnityEngine.CoreModule.dll \
  src/DedicatedServer/bin/Debug/net48/DedicatedServer.exe \
  src/DedicatedServer/bin/Debug/net48/Assembly-CSharp.dll

# 3. Run
ONI_STREAMING_ASSETS="$HOME/Library/Application Support/Steam/steamapps/common/OxygenNotIncluded/OxygenNotIncluded.app/Contents/Resources/Data/StreamingAssets" \
  /usr/bin/arch -x86_64 ~/.mono-x64/6.12.0/bin/mono \
  src/DedicatedServer/bin/Debug/net48/ServerLauncher.exe
```

> ⚠️ **Common mistake:** omitting the 4th arg (`Assembly-CSharp.dll`) to PatchInternalCalls causes entity registration to fail (0/431 entities instead of 235/431).

### Expected healthy boot output
```
[WorldBuilder] Registered 235/431 entities (20 failed), prefabs: 324
[WorldBuilder] Added 3 Minion prefabs at (128,187)
[WorldBuilder] Entity spawning: 989 spawned, 2318 skipped
[GameLoader] Boot complete.
Web server running at http://localhost:8080/
State machine tick loop started (60 Hz target)
```

---

## Visualizer Issues

---

### DS-001 · Entity markers off by 0.5 (on cell borders)

**Status:** ✅ FIXED

**Component:** Web visualizer — entity rendering

**Fix:** `RealWorldState.cs` `GetEntitySize()` — corrected coordinate origin; `WorldRenderer.ts` — explicit center formula (`x + w/2`, `y + h/2`) instead of relying on implicit offset. Also fixed wrong `h` for critters.

**Steps to reproduce:**
1. Run server with pipeline above
2. Open `http://localhost:8080/` in browser
3. Enable **Show Entities** overlay
4. Observe dupe/animal marker positions relative to grid cells

**Expected:** Each entity marker is centered inside its grid cell

**Actual before fix:** Markers were drawn on cell borders/edges (off by 0.5 cells)

**How to verify (regression):**
- Open visualizer → Show Entities
- Zoom in to a dupe
- Marker center must fall visually inside the cell, not on the boundary line
- Check 3 different dupes at different positions — all must be centered
- Check at least one critter (Hatch/Drecko) — marker must also be inside its cell

---

### DS-002 · Hardcoded entity sizes (all render as 1×1)

**Status:** ✅ FIXED

**Component:** `/entities` API + web visualizer renderer

**Fix:** `RealWorldState.cs` `GetCritterSize()` — returns real sizes per entity type (Drecko = 1×2, rest = 1×1). Frontend (`WorldRenderer.ts`) now reads `entity.w` / `entity.h` directly from API response instead of hardcoding 1×1.

**Steps to reproduce:**
1. Run server, open visualizer, enable Show Entities
2. Observe dupe shape — previously rendered as a square 1×1

**Expected sizes:**

| Entity      | Expected size | Source |
|-------------|---------------|--------|
| Minion/Dupe | 1×2 (pill)    | API    |
| Drecko      | 1×2           | `GetCritterSize()` |
| Hatch       | 1×1           | `GetCritterSize()` |
| Pacu        | 2×1           | `GetCritterSize()` |

**Actual before fix:** All entities rendered as 1×1 regardless of type

**How to verify (regression):**
- Open visualizer → Show Entities
- Dupe must render as a 1×2 pill shape (taller than wide)
- Confirm each entity type matches the size table above
- Inspect `/api/entities` JSON — each entry must include `"w"` and `"h"` fields with correct values

---

## API / Performance Issues

---

### DS-003 · Endpoint latency — `/world` 500ms, `/entities` 100ms

**Status:** 🟡 IN PROGRESS _(Dan — TTL cache fix in progress)_

**Component:** `WebServer` — `/world` endpoint

**Steps to reproduce:**
1. Run server, open browser DevTools → Network tab
2. Navigate to `http://localhost:8080/`
3. Let auto-refresh run for 10+ cycles
4. Observe request timing in Network tab

**Expected:** All endpoint responses < 50ms (p99)

**Actual (measured, commit 10709fd):**

| Endpoint | Run 1 | Run 2 | Run 3 | Run 4 | Run 5 | Cache |
|----------|-------|-------|-------|-------|-------|-------|
| `/api/world` (gzip) | 518ms | 334ms | 321ms | 327ms | 324ms | MISS every time |
| `/api/entities` | 41ms | 1.6ms | 1.2ms | 0.9ms | 0.9ms | PASS after warmup |
| `/api/state` | 10ms | 1.7ms | 1.1ms | 0.8ms | 1.0ms | PASS after warmup |

**Root cause:** `/api/world` snapshot is cache-keyed by game tick. Tick advances every frame → cache invalidated on every request → always 230–265ms rebuild of a 1995KB JSON snapshot. `/api/entities` and `/api/state` are fast after first request.

**Log evidence:**
```
[WorldState] Cache MISS: rebuilt world snapshot 1995KB in 265ms (tick=899)
[WorldState] Cache MISS: rebuilt world snapshot 1995KB in 232ms (tick=951)
[WorldState] Cache HIT: state age=18ms   ← /api/state hits cache fine
```

**How to verify fix:**
- Run `for i in 1 2 3 4 5; do curl -w "%{time_total}\n" -H "Accept-Encoding: gzip" http://localhost:8080/api/world -o /dev/null -s; done`
- All 5 runs must be < 50ms
- Log must show `Cache HIT` for `/api/world` on runs 2–5
- Fix expected: TTL-based invalidation (~200ms window) instead of per-tick

---

### DS-004 · Auto-refresh DDoS (concurrent overlapping requests)

**Status:** ✅ FIXED _(sequential polling implemented in App.tsx)_

**Component:** Web client — `App.tsx` polling loop

**Steps to reproduce (regression check):**
1. Open visualizer in browser
2. Open DevTools → Network tab
3. Observe `/entities` request timing over 30 seconds

**Expected:** Each new request starts only after the previous response completes (sequential, no overlap)

**Actual before fix:** Interval timer fired regardless of previous request status → concurrent requests piling up under slow server

**How to verify (regression):**
- DevTools Network tab → filter `/entities`
- Confirm requests are sequential: request N+1 initiates only after request N receives a response
- Under artificial 500ms server delay, requests must still be sequential (not stacked)
- No more than 1 in-flight request at any time

---

## Game Simulation Issues

---

### DS-005 · Dupes not moving after spawn

**Status:** 🔴 OPEN _(Dan — investigating ChoreConsumer.consumerState initialization)_

**Component:** Game simulation — Minion AI / ChoreConsumer / Brain

**Steps to reproduce:**
1. Run server, wait for boot
2. `curl http://localhost:8080/api/entities | python3 -c "import sys,json; d=json.load(sys.stdin); [print(e['name'],e['x'],e['y']) for e in d['entities'] if e['type']=='duplicant']"`
3. Wait 30 seconds, repeat
4. Compare coordinates

**Expected:** Dupes move around the map, executing at minimum an Idle chore

**Actual (measured, commit 10709fd):**

| Time | Game tick | Dupe 1 | Dupe 2 | Dupe 3 |
|------|-----------|--------|--------|--------|
| T=0  | 1129 | (128, 187) | (129, 187) | (130, 187) |
| T=30s | 1129 | (128, 187) | (129, 187) | (130, 187) |

Sim IS advancing (game tick reaches 9131+), dupes stationary.

**Root cause (confirmed in log):**
- `ChoreProvider is not initialized` × 3 (one per Minion)
- `ChoreDriver is not initialized` × 3
- `ChoreConsumer.OnSpawn` throws: `ChoreTable.Instance` ctor → `RanchedStates` NPE → `consumerState` stays `null`
- `Brain.UpdateBrain()` IS called via `BrainScheduler.RenderEveryTick()` ✅
- But `Brain.FindBetterChore` → `ChoreConsumer.FindNextChore` → `StandardChoreBase.CollectChores` → NPE (`consumerState` null) ❌
- **17,098 `[StateMachineTick] Error`** in log (new error replacing MoverLayerOccupier spam)

**Full stack (new error):**
```
[StateMachineTick] Error: Object reference not set to an instance of an object
  at StandardChoreBase.CollectChores (ChoreConsumerState consumer_state, ...)
  at Chore.CollectChores (...)
  at ChoreProvider.CollectChores (...)
  at ChoreConsumer.FindNextChore (...)
  at Brain.FindBetterChore (...)
  at Brain.UpdateChores ()
  at Brain.UpdateBrain ()
  at BrainScheduler+BrainGroup.RenderEveryTick ()
```

**Fix direction:** Initialize `consumerState` in `WorldBuilder` before Minion spawn — without Harmony patch on `ChoreConsumer` (Harmony deadlocks on that method). Alternative: patch a different callsite or use a `KMonoBehaviour.OnSpawn` hook that runs after the schedule is assigned.

**How to verify fix:**
- T=0 and T=30s dupe coords must differ, OR log must show `IdleChore` assigned
- `grep "StateMachineTick.*Error" /tmp/ds-retest.log | wc -l` → must be near 0
- No `ChoreProvider is not initialized` errors in log

---

### DS-006 · MoverLayerOccupier NullRef spam (96K errors / 30s)

**Status:** ✅ FIXED _(commit 10709fd)_

**Component:** Game simulation — `MoverLayerOccupier.Sim200ms()`

**Fix:** `ChoreConsumerPatch.cs` removed + `consumerState` initialization moved to `WorldBuilder`. Side effect: `MoverLayerOccupier` errors dropped to 0 (creature occupancy layer now initializes correctly without the broken Harmony patch interfering).

**Steps to reproduce (regression check):**
1. Run server for 30 seconds (after boot)
2. Check log:
   ```bash
   grep -c "MoverLayerOccupier" /tmp/ds-retest.log
   ```

**Expected:** 0

**Actual before fix:** ~96,947 occurrences in 30 seconds

**Actual after fix (commit 10709fd):** 0 ✅

**How to verify (regression):**
1. Run server 30s after boot
2. `grep -c "MoverLayerOccupier" /tmp/ds-retest.log` → must return `0`

---

## Smoke Test Checklist (run after every deployment)

Run through these quickly before deeper regression testing:

| # | Check | How | Pass criteria |
|---|-------|-----|---------------|
| S1 | Build succeeds | `dotnet build` | 0 errors |
| S2 | Patch succeeds | PatchInternalCalls | `Patched 3474 methods`, `Assembly-CSharp patched` |
| S3 | Server boots | Check log | `[GameLoader] Boot complete.` present |
| S4 | Entities spawn | Check log | `989 spawned` (not `0 spawned`) |
| S5 | 3 Minions spawned | Check log | `[Entities] Spawned: Minion` × 3 |
| S6 | Tick loop starts | Check log | `State machine tick loop started (60 Hz target)` |
| S7 | No fatal crash | Check log | No `FATAL UNHANDLED EXCEPTION` |
| S8 | Web server up | `curl http://localhost:8080/` | HTTP 200 |
| S9 | Entities API | `curl http://localhost:8080/api/entities` | JSON array, non-empty |
| S10 | World API | `curl http://localhost:8080/api/world` | JSON with `width`, `height`, `cells` |

---

## Change Log

| Date | Commit | Change |
|------|--------|--------|
| 2026-03-21 | `9d60b59` | Headless Unity runtime via Cecil IL patching — previous `streamingAssetsPath` crash fixed |
| 2026-03-21 | `0268fff` | GameTickLoop — StateMachineUpdater + BrainScheduler now driven at 60 Hz |
| 2026-03-21 | `048592b` | Entity registration pipeline fix (0→235/431); WebServer port graceful catch; Assembly-CSharp render noops |
| 2026-03-21 | —         | DS-001 FIXED: entity marker off-by-0.5 — `RealWorldState.cs` GetEntitySize + `WorldRenderer.ts` explicit center formula; critter `h` corrected |
| 2026-03-21 | —         | DS-002 FIXED: real entity sizes in API — `RealWorldState.cs` GetCritterSize (Drecko=1×2, rest=1×1); frontend reads `entity.w`/`entity.h` |
| 2026-03-21 | `29de6f1` | Added ChoreConsumerPatch + gzip + perf diagnostics — introduced boot hang regression |
| 2026-03-21 | `fce8477` | Attempted fix for hang (removed HarmonyFinalizer) — did not resolve (root cause was patching ChoreConsumer.OnSpawn itself) |
| 2026-03-21 | `10709fd` | DS-006 FIXED: removed ChoreConsumerPatch entirely — MoverLayerOccupier 0 errors; boot hang fixed (33s boot); consumerState init moved to WorldBuilder |
