import { describe, it, expect } from 'vitest';
import { fmtStat, formatStats, formatStatsLine } from './formatStats';
import type { GameState } from '../api/types';

/** Minimal valid GameState for tests (only fields used by formatStats). */
function makeState(overrides: Partial<GameState> = {}): GameState {
  return {
    tick: 0,
    cycle: 1,
    speed: 1,
    paused: false,
    worldWidth: 256,
    worldHeight: 128,
    duplicantCount: 0,
    buildingCount: 0,
    entityCount: 0,
    ...overrides,
  };
}

// ── fmtStat ───────────────────────────────────────────────────────────────────

describe('fmtStat', () => {
  it('formats a positive integer as a string', () => {
    expect(fmtStat(412)).toBe('412');
  });

  it('formats zero as "0" — genuine zero is not a dash', () => {
    expect(fmtStat(0)).toBe('0');
  });

  it('returns "—" for undefined', () => {
    expect(fmtStat(undefined)).toBe('—');
  });

  it('returns "—" for null', () => {
    expect(fmtStat(null)).toBe('—');
  });

  it('formats large numbers as plain strings (no locale commas)', () => {
    expect(fmtStat(3348)).toBe('3348');
    expect(fmtStat(2225)).toBe('2225');
  });
});

// ── formatStats ───────────────────────────────────────────────────────────────

describe('formatStats', () => {
  it('returns dashes for all fields when gameState is null', () => {
    const s = formatStats(null);
    expect(s.tick).toBe('—');
    expect(s.entities).toBe('—');
    expect(s.bootErrors).toBe('—');
  });

  it('formats tick from gameState.tick', () => {
    expect(formatStats(makeState({ tick: 412 })).tick).toBe('412');
  });

  it('formats entities from gameState.entityCount', () => {
    expect(formatStats(makeState({ entityCount: 3348 })).entities).toBe('3348');
  });

  it('formats bootErrors from gameState.bootErrorCount', () => {
    expect(formatStats(makeState({ bootErrorCount: 2225 })).bootErrors).toBe('2225');
  });

  it('shows "—" for bootErrors when bootErrorCount is absent (older backend)', () => {
    // bootErrorCount not set → undefined
    const state = makeState();
    delete (state as Partial<GameState>).bootErrorCount;
    expect(formatStats(state).bootErrors).toBe('—');
  });

  it('shows "0" for bootErrors when explicitly zero — not a dash', () => {
    expect(formatStats(makeState({ bootErrorCount: 0 })).bootErrors).toBe('0');
  });

  it('shows "0" for tick=0 at server start — not a dash', () => {
    expect(formatStats(makeState({ tick: 0 })).tick).toBe('0');
  });

  it('shows "0" for entityCount=0 — not a dash', () => {
    expect(formatStats(makeState({ entityCount: 0 })).entities).toBe('0');
  });
});

// ── formatStatsLine ───────────────────────────────────────────────────────────

describe('formatStatsLine', () => {
  it('produces the canonical pipe-separated format when all fields present', () => {
    const state = makeState({ tick: 412, entityCount: 3348, bootErrorCount: 2225 });
    expect(formatStatsLine(state)).toBe('Tick: 412 | Entities: 3348 | Boot errors: 2225');
  });

  it('uses dashes for all fields when gameState is null', () => {
    expect(formatStatsLine(null)).toBe('Tick: — | Entities: — | Boot errors: —');
  });

  it('uses dashes only for absent fields, not for present ones', () => {
    const state = makeState({ tick: 100, entityCount: 500 });
    // bootErrorCount absent → dash; tick/entities present
    expect(formatStatsLine(state)).toBe('Tick: 100 | Entities: 500 | Boot errors: —');
  });

  it('handles zero values correctly — shows 0 not dash', () => {
    const state = makeState({ tick: 0, entityCount: 0, bootErrorCount: 0 });
    expect(formatStatsLine(state)).toBe('Tick: 0 | Entities: 0 | Boot errors: 0');
  });

  it('handles high tick and entity counts', () => {
    const state = makeState({ tick: 999999, entityCount: 10000, bootErrorCount: 0 });
    expect(formatStatsLine(state)).toBe('Tick: 999999 | Entities: 10000 | Boot errors: 0');
  });
});
