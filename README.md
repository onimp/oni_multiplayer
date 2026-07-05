# ONI Multiplayer Mod

**Cooperative multiplayer for _Oxygen Not Included_.** Host a colony, invite friends over Steam, and run
the same base together — everyone shares control of one asteroid cluster.

> ⚠️ **Prototype / work-in-progress.** This is an actively-developed proof of concept, not a finished mod.
> Expect rough edges and desync. It is playable co-op, not polished co-op.

> 🔱 **This is a fork.** Built on the original [**zuev93/oni_multiplayer**](https://github.com/zuev93/oni_multiplayer)
> by [zuev93](https://github.com/zuev93) and contributors. This fork ([macery12/oni_multiplayer](https://github.com/macery12/oni_multiplayer))
> continues that work — forward-porting it to a current game version and expanding what actually stays in
> sync during play. See [Credits](#credits).

> 🤖 **Developed with Claude Code.** Much of the sync work in this fork was implemented with
> [Claude Code](https://claude.com/claude-code) as a pair-programmer — mapping the game's internals,
> designing the host-authoritative sync layers, and writing the synchronizers. Details of what's synced and
> how live in [docs/sync.md](docs/sync.md).

---

## Game version compatibility

| | |
|---|---|
| **Supported game version** | `U57-707956` (Legacy Compatibility branch, released **Jan 16 2026**) |
| **How to get it** | Steam → _Oxygen Not Included_ → Properties → Betas → select **`legacy_compatibility_version`** |
| **DLC** | ❌ Not supported yet — **vanilla / base game only** (no Spaced Out!) |
| **Newer game versions** | ⏳ Support for the latest game version **and DLC** is planned — coming soon |

The mod is pinned to one exact build. It will **not** work on the default (latest) Steam branch or with DLC
enabled yet. Both players must be on the same `legacy_compatibility_version` branch.

---

## What's synced

The mod is **host-authoritative**: the host runs the "real" game and clients replay the host's actions. A
**hard-sync every in-game morning** (a full save transfer + reload) corrects any drift that slips through.

### ✅ Synced

| Feature | |
|---|---|
| Joining via Steam (invite / overlay) | ✅ |
| All player orders (dig, build, deconstruct, cancel, priority, mop, harvest, sweep, attack…) | ✅ |
| Bottom-toolbar tools & drag orders | ✅ |
| Building configuration (doors, valves, filters, automation, fabricator queues, thresholds, sliders…) | ✅ |
| Colony screens (Skills & Hats, Priorities, Schedules, Consumables, Research tree) | ✅ |
| Game speed & pause | ✅ |
| Player cursors & presence overlay | ✅ |
| Duplicant behavior — idle, move-to-safety, attack, death, pee | ✅ |
| Duplicant work — start **and** completion timing (jobs finish in lockstep) | ✅ |
| Digging / terrain removal | ✅ |
| Building completion & deconstruction | ✅ |
| Mopping & harvesting (produced items appear on both sides) | ✅ |
| Duplicant health, sickness, disease & effects (buffs/debuffs) | ✅ |
| World cell simulation — gas / liquid / temperature / mass | ✅ |
| Power — battery charge (stored energy) | ✅ |
| Research — tech unlocks | ✅ |
| Daily hard-sync safety net | ✅ |

### ❌ Not synced yet

Most of these are **intentionally deferred** — the daily hard-sync reconciles them once per cycle, so they
self-correct rather than break the game.

| Feature | |
|---|---|
| Materials economy — storage contents, fetch & deliver | ❌ |
| Sweep result (debris moved into storage) | ❌ |
| Toilet fill level & polluted-dirt output | ❌ |
| Duplicant vitals (calories, stress, stamina, bladder, breath) | ❌ |
| Eat / Sleep / Recreation / Mingle behavior | ❌ |
| Research **point** progress & skill XP (the *unlock* is synced, the accrual isn't) | ❌ |
| Rockets & space | ❌ |
| Critters | ❌ |

---

## Install

Both players must be on the **`legacy_compatibility_version`** Steam branch (see
[Game version compatibility](#game-version-compatibility)) with **DLC disabled**.

The mod installs to `%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local\MultiplayerMod`.

### Manual
1. Download the latest release from **https://github.com/macery12/oni_multiplayer/releases/latest**
2. Unzip anywhere
3. Copy `mod.yaml`, `mod_info.yaml` and `MultiplayerMod.dll` into
   `%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local\MultiplayerMod` (create the folders if they
   don't exist)

Then enable the mod in-game via the Mods menu.

---

## How to play

### Host
1. Start a game via **New multiplayer game** or **Load multiplayer game**
2. Wait for the game to load (the Steam overlay opens automatically)
3. Invite friends through the Steam overlay

### Joining
1. Launch the game first
2. Accept the invite or join via the Steam overlay
3. Wait for the game to load

---

## Project status & roadmap

- **Now:** playable co-op on the pinned game version, vanilla only. Ongoing work is expanding what stays in
  sync mid-cycle (so you rely less on the daily hard-sync).
- **Next:** support for the latest game version and DLC.
- Living sync status: **[docs/sync.md](docs/sync.md)**.

---

## Credits

- **Original mod:** [zuev93/oni_multiplayer](https://github.com/zuev93/oni_multiplayer) by
  [zuev93](https://github.com/zuev93) and its contributors — the foundation this fork is built on.
- **This fork:** [macery12](https://github.com/macery12) — forward-port + expanded sync, developed with

---

## Contributing & contact

- Developer guide: [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md)
- Issues & suggestions: open an issue on [this repo](https://github.com/macery12/oni_multiplayer/issues)
- Original project's community Discord: https://discord.gg/3TQ97w8Qwq
