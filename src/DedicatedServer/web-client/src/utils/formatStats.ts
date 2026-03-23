import type { GameState } from '../api/types';

/**
 * Pure formatting logic for the server stats bar — no React, no DOM.
 *
 * Rule: show '—' when a field is absent (undefined/null), never show '0'
 * in place of missing data.  A genuine value of 0 is shown as "0".
 */

/** Formatted values for each stats-bar field. */
export interface StatsFields {
  tick:       string;   // game tick counter
  entities:   string;   // total entity count
  bootErrors: string;   // errors captured at server boot (may be '—' for older backends)
}

/** Format a number for display, or return '—' when absent. */
export function fmtStat(n: number | undefined | null): string {
  return n != null ? String(n) : '—';
}

/**
 * Derives display-ready StatsFields from a GameState (or null when not yet loaded).
 * All fields fall back to '—' when the corresponding value is unavailable.
 */
export function formatStats(gameState: GameState | null): StatsFields {
  return {
    tick:       fmtStat(gameState?.tick),
    entities:   fmtStat(gameState?.entityCount),
    bootErrors: fmtStat(gameState?.bootErrorCount),
  };
}

/**
 * Renders the stats as a single pipe-separated line, matching the canonical
 * format: "Tick: 412 | Entities: 3348 | Boot errors: 2225"
 * Used in tests to verify the full formatted output in one assertion.
 */
export function formatStatsLine(gameState: GameState | null): string {
  const { tick, entities, bootErrors } = formatStats(gameState);
  return `Tick: ${tick} | Entities: ${entities} | Boot errors: ${bootErrors}`;
}
