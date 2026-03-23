import { describe, it, expect } from 'vitest';
import {
  toPct,
  cleanSmState,
  formatRawAmount,
  formatDupeDetail,
} from './dupeDetail';
import type { EntityData } from '../api/types';

// ── helpers ───────────────────────────────────────────────────────────────────

function makeDupe(overrides: Partial<EntityData> = {}): EntityData {
  return {
    type:         'duplicant',
    name:         'Abe',
    x:            42,
    y:            17,
    w:            1,
    h:            2,
    currentChore: 'Eat',
    smState:      'root.alive.notasleep.working.loop',
    navIsMoving:  false,
    navCell:      500,
    stamina:      73,
    staminaMax:   100,
    calories:     2_753_821,
    caloriesMax:  4_000_000,
    ...overrides,
  };
}

// ── toPct ─────────────────────────────────────────────────────────────────────

describe('toPct', () => {
  it('returns 73 for 73/100', () => {
    expect(toPct(73, 100)).toBe(73);
  });

  it('returns 0 for 0/100', () => {
    expect(toPct(0, 100)).toBe(0);
  });

  it('returns 100 for 100/100', () => {
    expect(toPct(100, 100)).toBe(100);
  });

  it('clamps to 100 when value > max', () => {
    expect(toPct(110, 100)).toBe(100);
  });

  it('clamps to 0 when value is negative', () => {
    expect(toPct(-5, 100)).toBe(0);
  });

  it('returns 0 when max is 0 (division guard)', () => {
    expect(toPct(50, 0)).toBe(0);
  });

  it('returns 0 when max is negative', () => {
    expect(toPct(50, -1)).toBe(0);
  });

  it('rounds fractional percentages', () => {
    expect(toPct(1, 3)).toBe(33);
    expect(toPct(2, 3)).toBe(67);
  });

  it('handles large calorie values', () => {
    expect(toPct(2_000_000, 4_000_000)).toBe(50);
  });
});

// ── cleanSmState ──────────────────────────────────────────────────────────────

describe('cleanSmState', () => {
  it('strips "root.alive.notasleep." prefix', () => {
    expect(cleanSmState('root.alive.notasleep.idle')).toBe('idle');
  });

  it('strips "root.alive.notasleep." and replaces remaining dots', () => {
    expect(cleanSmState('root.alive.notasleep.working.loop')).toBe('working › loop');
  });

  it('strips "root.alive." prefix (without notasleep)', () => {
    expect(cleanSmState('root.alive.sleeping')).toBe('sleeping');
  });

  it('strips bare "root." prefix', () => {
    expect(cleanSmState('root.dead')).toBe('dead');
  });

  it('passes through "none"', () => {
    expect(cleanSmState('none')).toBe('none');
  });

  it('returns "none" for null', () => {
    expect(cleanSmState(null)).toBe('none');
  });

  it('returns "none" for undefined', () => {
    expect(cleanSmState(undefined)).toBe('none');
  });

  it('returns "none" for empty string', () => {
    expect(cleanSmState('')).toBe('none');
  });

  it('handles a state with no recognized prefix', () => {
    expect(cleanSmState('custom.state')).toBe('custom › state');
  });

  it('handles single-segment path after prefix strip', () => {
    expect(cleanSmState('root.alive.sleeping')).toBe('sleeping');
  });
});

// ── formatRawAmount ───────────────────────────────────────────────────────────

describe('formatRawAmount', () => {
  it('formats integer below 1000 as-is', () => {
    expect(formatRawAmount(73)).toBe('73');
  });

  it('formats 1000 with thousands separator', () => {
    // fr locale uses non-breaking space as thousands separator
    expect(formatRawAmount(1000)).toMatch(/1.000/);
  });

  it('formats large calorie value', () => {
    const result = formatRawAmount(4_000_000);
    expect(result).toMatch(/4/);
    expect(result).toMatch(/000/);
  });

  it('rounds fractional values', () => {
    expect(formatRawAmount(73.6)).toBe('74');
    expect(formatRawAmount(73.4)).toBe('73');
  });
});

// ── formatDupeDetail ─────────────────────────────────────────────────────────

describe('formatDupeDetail — returns null for non-duplicants', () => {
  it('returns null for critter', () => {
    expect(formatDupeDetail({ type: 'critter', name: 'Hatch', x: 0, y: 0 } as EntityData)).toBeNull();
  });

  it('returns null for building', () => {
    expect(formatDupeDetail({ type: 'building', name: 'Headquarters', x: 0, y: 0 } as EntityData)).toBeNull();
  });
});

describe('formatDupeDetail — name', () => {
  it('copies name directly', () => {
    expect(formatDupeDetail(makeDupe({ name: 'Ada' }))?.name).toBe('Ada');
  });
});

describe('formatDupeDetail — chore', () => {
  it('formats "Eat" chore', () => {
    expect(formatDupeDetail(makeDupe({ currentChore: 'Eat' }))?.currentChore).toBe('Eat');
  });

  it('formats null chore as "Idle"', () => {
    expect(formatDupeDetail(makeDupe({ currentChore: undefined }))?.currentChore).toBe('Idle');
  });
});

describe('formatDupeDetail — SM state', () => {
  it('cleans SM state path', () => {
    expect(formatDupeDetail(makeDupe({ smState: 'root.alive.notasleep.idle' }))?.smState).toBe('idle');
  });

  it('handles null smState', () => {
    expect(formatDupeDetail(makeDupe({ smState: null }))?.smState).toBe('none');
  });

  it('handles undefined smState', () => {
    expect(formatDupeDetail(makeDupe({ smState: undefined }))?.smState).toBe('none');
  });
});

describe('formatDupeDetail — stamina', () => {
  it('computes staminaPct correctly', () => {
    expect(formatDupeDetail(makeDupe({ stamina: 73, staminaMax: 100 }))?.staminaPct).toBe(73);
  });

  it('clamps staminaPct to 100', () => {
    expect(formatDupeDetail(makeDupe({ stamina: 105, staminaMax: 100 }))?.staminaPct).toBe(100);
  });

  it('defaults stamina to 0 when absent', () => {
    expect(formatDupeDetail(makeDupe({ stamina: undefined }))?.staminaPct).toBe(0);
  });

  it('defaults staminaMax to 100 when absent', () => {
    const detail = formatDupeDetail(makeDupe({ stamina: 50, staminaMax: undefined }));
    expect(detail?.staminaPct).toBe(50);
  });

  it('includes staminaRaw string', () => {
    const detail = formatDupeDetail(makeDupe({ stamina: 73, staminaMax: 100 }));
    expect(detail?.staminaRaw).toContain('73');
    expect(detail?.staminaRaw).toContain('100');
  });
});

describe('formatDupeDetail — calories', () => {
  it('computes caloriesPct correctly', () => {
    const detail = formatDupeDetail(makeDupe({ calories: 2_000_000, caloriesMax: 4_000_000 }));
    expect(detail?.caloriesPct).toBe(50);
  });

  it('clamps caloriesPct to 0 when calories=0', () => {
    expect(formatDupeDetail(makeDupe({ calories: 0 }))?.caloriesPct).toBe(0);
  });

  it('defaults caloriesMax to 4M when absent', () => {
    const detail = formatDupeDetail(makeDupe({ calories: 4_000_000, caloriesMax: undefined }));
    expect(detail?.caloriesPct).toBe(100);
  });

  it('includes caloriesRaw string with both values', () => {
    const detail = formatDupeDetail(makeDupe({ calories: 2_000_000, caloriesMax: 4_000_000 }));
    expect(detail?.caloriesRaw).toBeTruthy();
    expect(detail?.caloriesRaw).toContain('/');
  });
});

describe('formatDupeDetail — position + nav', () => {
  it('copies x/y into position', () => {
    const detail = formatDupeDetail(makeDupe({ x: 42, y: 17 }));
    expect(detail?.position).toEqual({ x: 42, y: 17 });
  });

  it('moving=true when navIsMoving=true', () => {
    expect(formatDupeDetail(makeDupe({ navIsMoving: true }))?.moving).toBe(true);
  });

  it('moving=false when navIsMoving=false', () => {
    expect(formatDupeDetail(makeDupe({ navIsMoving: false }))?.moving).toBe(false);
  });

  it('moving defaults to false when navIsMoving absent', () => {
    expect(formatDupeDetail(makeDupe({ navIsMoving: undefined }))?.moving).toBe(false);
  });
});
