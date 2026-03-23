/**
 * dupeDetail.ts — pure formatting helpers for the duplicant detail panel.
 *
 * API fields available (from BuildLiveMinionDto):
 *   name, x, y, w, h, currentChore, smState, navIsMoving, navCell,
 *   stamina, staminaMax, calories, caloriesMax
 *
 * NOT in API (no server support yet): skills, traits, stress.
 *
 * No DOM, no React — fully testable in Node/vitest.
 */

import type { EntityData } from '../api/types';

// ── Types ─────────────────────────────────────────────────────────────────────

export interface DupeDetail {
  /** Duplicant's personal name ("Abe", "Ada", …) */
  name: string;
  /** Formatted chore label: chore type name, or "Idle" when null/absent */
  currentChore: string;
  /** Cleaned SM state path (leading "root.alive." and "root." stripped; dots → " › ") */
  smState: string;
  /** Stamina as 0–100 integer, clamped */
  staminaPct: number;
  /** Calories as 0–100 integer, clamped */
  caloriesPct: number;
  /** Raw stamina for tooltip (e.g. "73 / 100") */
  staminaRaw: string;
  /** Raw calories for tooltip (e.g. "2 753 821 / 4 000 000") */
  caloriesRaw: string;
  /** Whether the nav system reports active movement */
  moving: boolean;
  /** World-cell position */
  position: { x: number; y: number };
}

// ── Helpers ───────────────────────────────────────────────────────────────────

/**
 * Clamps a value to [0, 100] and rounds to the nearest integer.
 * Guards against divide-by-zero when max is 0.
 */
export function toPct(value: number, max: number): number {
  if (max <= 0) return 0;
  return Math.round(Math.min(100, Math.max(0, (value / max) * 100)));
}

/**
 * Strips leading "root.alive." or "root." from an SM state path, then
 * replaces remaining dots with " › " for readability.
 *
 * Examples:
 *   "root.alive.notasleep.idle"  → "idle"
 *   "root.alive.sleeping"        → "sleeping"
 *   "root.dead"                  → "dead"
 *   "none"                       → "none"
 */
export function cleanSmState(raw: string | null | undefined): string {
  if (!raw || raw === 'none') return 'none';
  let s = raw;
  if (s.startsWith('root.alive.notasleep.')) s = s.slice('root.alive.notasleep.'.length);
  else if (s.startsWith('root.alive.'))       s = s.slice('root.alive.'.length);
  else if (s.startsWith('root.'))             s = s.slice('root.'.length);
  return s.replace(/\./g, ' › ');
}

/**
 * Formats a raw numeric value for display with locale thousands separators,
 * rounded to an integer.  E.g. 2753821 → "2 753 821".
 */
export function formatRawAmount(value: number): string {
  return Math.round(value).toLocaleString('fr'); // space thousands separator
}

// ── Main export ───────────────────────────────────────────────────────────────

/**
 * Formats an EntityData into DupeDetail for display in the detail panel.
 * Returns null when the entity is not a duplicant.
 */
export function formatDupeDetail(entity: EntityData): DupeDetail | null {
  if (entity.type !== 'duplicant') return null;

  const stamina    = entity.stamina    ?? 0;
  const staminaMax = entity.staminaMax ?? 100;
  const calories    = entity.calories    ?? 0;
  const caloriesMax = entity.caloriesMax ?? 4_000_000;

  return {
    name:         entity.name,
    currentChore: entity.currentChore ?? 'Idle',
    smState:      cleanSmState(entity.smState),
    staminaPct:   toPct(stamina, staminaMax),
    caloriesPct:  toPct(calories, caloriesMax),
    staminaRaw:   `${formatRawAmount(stamina)} / ${formatRawAmount(staminaMax)}`,
    caloriesRaw:  `${formatRawAmount(calories)} / ${formatRawAmount(caloriesMax)}`,
    moving:       entity.navIsMoving ?? false,
    position:     { x: entity.x, y: entity.y },
  };
}
