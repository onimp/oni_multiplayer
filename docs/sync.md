# Multiplayer Sync Status

What is kept in sync between the host and connected clients. The mod is **host-authoritative**: the host runs
the real game, clients replay the host's actions. A **daily hard-sync** (full save transfer + reload) reloads
the whole world once per cycle, so anything not synced live is corrected then.

**Game version:** `U57-707956`, vanilla — no DLC. **Last updated:** 2026-07-06.

> Plain player-facing summary: [README](../README.md#whats-synced).

**The four buckets below:**
- **Fully synced** — kept in lockstep live, as it happens.
- **Partially synced** — the main behaviour syncs live; a noted detail doesn't (that detail falls to hard-sync).
- **Hard-sync only** — not synced live; reconciled once per cycle at the world save/reload.
- **Not synced** — no dedicated handling; only the hard-sync reload touches it.

🧪 = code is in but not yet verified in a live 2-PC test. Everything unmarked is considered working.

---

## Fully synced

- **Player orders & tools** — dig, build, deconstruct, cancel, priority, sweep, disinfect, mop, harvest,
  attack, wrangle, empty-pipe, disconnect, copy-settings, stamp, move-to, wire/pipe build.
- **Management UI** — research select/cancel, schedules, priorities, consumables, skills/hats, assignments,
  building config (doors, valves, fabricator queues, filters, thresholds, sliders, automation, receptacles),
  side-screen sensors, red alert, immigration, speed/pause.
- **Duplicant behaviour** — idle/wander, move-to-safety, attack, death, bathroom, eat, sleep, mingle,
  recreation buildings (arcade, hot tub, espresso, etc.), and social recreation 🧪 (water-cooler
  socializing, room parties, balloon artist) — host creates + assigns the chore, client's own copy is
  cancelled; the party locator/work-time and each cooler's chit-chat spot are rebuilt on the client.
- **Critters** — position, facing, health, current animation.
- **Dig** — terrain removal.
- **Research** — order select/cancel.
- **Core** — network object identity (`MultiplayerId`), command pipeline, remote cursors/presence, dev-tool
  sync inspectors + manual force-save, and the daily hard-sync itself.

## Partially synced

- **Building completion** 🧪 — finished building appears on clients; client-built buildings now share the
  host's id (config on them resolves).
- **Deconstruction** 🧪 — building removal replicated.
- **Generic work** 🧪 (dig / build / cook / research / operate / …) — assignment + completion timing are
  host-gated so both sides finish together.
- **Mop** 🧪 — liquid removed and the bottle spawned on clients.
- **Harvest** 🧪 — crop picked and spawned on clients; *mutation genetics* → hard-sync.
- **Toilet** 🧪 — full flush synced host→client: fill level (meter + full/clean cycle), polluted-dirt output,
  and the germs added to the duplicant all reproduced (the dupe germs ride the duplicant germ-load stream).
- **Duplicant state** 🧪 — sickness/disease, effects/buffs, health, **vitals** (calories / stress / stamina /
  bladder / breath), **skill & attribute leveling XP**, and **carried germ load** all streamed host→client
  each ~1 s; *other amounts (body temperature, decor, immune level, toxicity, radiation)* → hard-sync.
- **Critter lifecycle** 🧪 — lay egg / hatch / grow up / death mirrored host→client under a shared id;
  *byproduct drops (meat/shell)* → hard-sync.
- **Critter emission** 🧪 — poop that lands in cells (gas/liquid/solid-tile) rides the world-sim cell stream;
  loose solid drops (Hatch coal, etc.) suppressed on the client and replicated host→client at the same cell;
  *internal-stored poop* and *tameness/calorie bookkeeping* → hard-sync.
- **Storage intake** 🧪 (sweep / fetch / deliver) — an item entering a `Storage` on the host is replicated so
  the client's loose item leaves the ground and lands in the same bin (host-authoritative, captured at
  `Storage.Store`, coalesced per destination storage to bound bursts); *stack-merge edge cases, items the
  client can't locate, and items **leaving** storage* → hard-sync.
- **World sim (cells)** — element / temperature / mass / disease streamed host→client (fluid cells; terrain is
  owned by dig/build).
- **Power** 🧪 — battery charge streamed; *transient generator/wire/transformer state* → hard-sync.
- **Research** 🧪 — tech unlock replicated on completion; *research-point accrual* → hard-sync.

## Hard-sync only

- **Materials economy (outbound)** — items *leaving* storage: drop, storage-to-storage transfer, building
  consumption. Items *entering* storage now sync live — see **Storage intake** under Partially synced.
- **Other duplicant amounts** — body temperature, decor, immune level, toxicity, radiation balance.
- **Research points** accrual (the tech *unlock* is synced live; the point accrual isn't).
- **Critter wildness / tameness** (ranching progress) and **byproduct drops** (meat / egg shell / raw egg).
- **Balloon items** handed out by the balloon artist (the artist's animation syncs; the balloons are materials).
- **Loose pickupable identity** — mop bottle, harvested crop, etc. carry no shared id.

## Not synced

- **Rockets / space** — only scattered launch/board hooks, no dedicated sync.
- **Critter AI / behaviour** — species chore tables & monitors still run per-machine (each side picks its own
  chores/targets); the host's position, facing, health, current *animation*, and element emission are streamed
  over the top, so the visible result tracks the host between hard-syncs even though the underlying decisions
  aren't replicated.
