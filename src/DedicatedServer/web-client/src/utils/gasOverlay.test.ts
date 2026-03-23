import { describe, it, expect } from 'vitest';
import {
  gasOverlayColor,
  GAS_BACKGROUND,
  GAS_MAX_MASS,
} from './gasOverlay';

// ── helpers ───────────────────────────────────────────────────────────────────

/** Parse 'rgb(r,g,b)' → [r, g, b] */
function parseRgb(s: string): [number, number, number] {
  const m = s.match(/^rgb\((\d+),(\d+),(\d+)\)$/);
  if (!m) throw new Error(`not an rgb string: "${s}"`);
  return [+m[1], +m[2], +m[3]];
}

// ── zero / no-gas cases ───────────────────────────────────────────────────────

describe('gasOverlayColor — zero mass', () => {
  it('returns GAS_BACKGROUND for Oxygen at mass 0', () => {
    expect(gasOverlayColor('Oxygen', 0)).toBe(GAS_BACKGROUND);
  });

  it('returns GAS_BACKGROUND for CarbonDioxide at mass 0', () => {
    expect(gasOverlayColor('CarbonDioxide', 0)).toBe(GAS_BACKGROUND);
  });

  it('returns GAS_BACKGROUND for negative mass (treated as 0)', () => {
    expect(gasOverlayColor('Oxygen', -1)).toBe(GAS_BACKGROUND);
  });
});

// ── O2 — blue family ──────────────────────────────────────────────────────────

describe('gasOverlayColor — Oxygen (blue)', () => {
  it('at max mass blue channel dominates', () => {
    const [r, g, b] = parseRgb(gasOverlayColor('Oxygen', GAS_MAX_MASS));
    expect(b).toBeGreaterThan(r);
    expect(b).toBeGreaterThan(g);
  });

  it('at full mass (1 kg) blue channel is at its maximum for this gas', () => {
    const [, , bFull] = parseRgb(gasOverlayColor('Oxygen', GAS_MAX_MASS));
    const [, , bHalf] = parseRgb(gasOverlayColor('Oxygen', GAS_MAX_MASS * 0.25));
    expect(bFull).toBeGreaterThan(bHalf);
  });

  it('low pressure produces a dimmer color than high pressure', () => {
    const [r1, g1, b1] = parseRgb(gasOverlayColor('Oxygen', 0.1));
    const [r2, g2, b2] = parseRgb(gasOverlayColor('Oxygen', 1.0));
    // All channels should be ≤ their full-pressure counterparts
    expect(r1).toBeLessThanOrEqual(r2);
    expect(g1).toBeLessThanOrEqual(g2);
    expect(b1).toBeLessThanOrEqual(b2);
  });

  it('mass above GAS_MAX_MASS clamps — same color as GAS_MAX_MASS', () => {
    expect(gasOverlayColor('Oxygen', GAS_MAX_MASS * 2))
      .toBe(gasOverlayColor('Oxygen', GAS_MAX_MASS));
  });

  it('mass exactly at GAS_MAX_MASS is not GAS_BACKGROUND', () => {
    expect(gasOverlayColor('Oxygen', GAS_MAX_MASS)).not.toBe(GAS_BACKGROUND);
  });
});

// ── CO2 — yellow family ───────────────────────────────────────────────────────

describe('gasOverlayColor — CarbonDioxide (yellow)', () => {
  it('at max mass both red and green channels are dominant over blue', () => {
    const [r, g, b] = parseRgb(gasOverlayColor('CarbonDioxide', GAS_MAX_MASS));
    expect(r).toBeGreaterThan(b);
    expect(g).toBeGreaterThan(b);
  });

  it('at full mass red and green are each greater than 100', () => {
    const [r, g] = parseRgb(gasOverlayColor('CarbonDioxide', GAS_MAX_MASS));
    expect(r).toBeGreaterThan(100);
    expect(g).toBeGreaterThan(100);
  });

  it('lower pressure → lower channel values', () => {
    const lo = parseRgb(gasOverlayColor('CarbonDioxide', 0.2));
    const hi = parseRgb(gasOverlayColor('CarbonDioxide', 1.0));
    expect(lo[0]).toBeLessThan(hi[0]); // red
    expect(lo[1]).toBeLessThan(hi[1]); // green
  });
});

// ── O2 ≠ CO2 colors ───────────────────────────────────────────────────────────

describe('gasOverlayColor — O2 vs CO2 are distinct', () => {
  it('O2 and CO2 at same mass produce different colors', () => {
    expect(gasOverlayColor('Oxygen', 0.5)).not.toBe(gasOverlayColor('CarbonDioxide', 0.5));
  });
});

// ── Other known gases ─────────────────────────────────────────────────────────

describe('gasOverlayColor — other known gases', () => {
  const gases = [
    'ContaminatedOxygen',
    'Hydrogen',
    'ChlorineGas',
    'Methane',
    'Steam',
    'NaphthGas',
  ];

  for (const gas of gases) {
    it(`${gas} at 1 kg returns a non-background rgb() string`, () => {
      const color = gasOverlayColor(gas, 1.0);
      expect(color).not.toBe(GAS_BACKGROUND);
      expect(color).toMatch(/^rgb\(\d+,\d+,\d+\)$/);
    });
  }

  it('all known gases at full pressure produce distinct colors from each other', () => {
    const colors = gases.map(g => gasOverlayColor(g, 1.0));
    const unique = new Set(colors);
    // At minimum most should be distinct — allow at most 1 duplicate (ChlorineGas/Chlorine aliases)
    expect(unique.size).toBeGreaterThanOrEqual(gases.length - 1);
  });
});

// ── Unknown gas fallback ──────────────────────────────────────────────────────

describe('gasOverlayColor — unknown gas fallback', () => {
  it('returns an rgb() string (not background, not empty) for unknown gas with mass > 0', () => {
    const color = gasOverlayColor('SomeFutureGas_XYZ', 0.8);
    expect(color).toMatch(/^rgb\(\d+,\d+,\d+\)$/);
    expect(color).not.toBe(GAS_BACKGROUND);
  });

  it('unknown gas at higher pressure is brighter than at lower pressure', () => {
    const [r1] = parseRgb(gasOverlayColor('UnknownGasType', 0.1));
    const [r2] = parseRgb(gasOverlayColor('UnknownGasType', 1.0));
    expect(r2).toBeGreaterThan(r1);
  });

  it('unknown gas returns equal r/g/b (neutral grey)', () => {
    const [r, g, b] = parseRgb(gasOverlayColor('AlienGas', 0.5));
    expect(r).toBe(g);
    expect(g).toBe(b);
  });
});

// ── Boundary values ───────────────────────────────────────────────────────────

describe('gasOverlayColor — boundary values', () => {
  it('very small mass (0.001 kg) still returns a visible non-background color', () => {
    const color = gasOverlayColor('Oxygen', 0.001);
    expect(color).not.toBe(GAS_BACKGROUND);
    expect(color).toMatch(/^rgb\(\d+,\d+,\d+\)$/);
  });

  it('mass exactly 0.5 × GAS_MAX_MASS is between background and full intensity', () => {
    const full = parseRgb(gasOverlayColor('Oxygen', GAS_MAX_MASS));
    const half = parseRgb(gasOverlayColor('Oxygen', GAS_MAX_MASS * 0.5));
    // Blue at 0.5 mass should be less than at full mass (√0.5 ≈ 0.707)
    expect(half[2]).toBeLessThan(full[2]);
    expect(half[2]).toBeGreaterThan(10); // above background (BG = 10)
  });

  it('GAS_MAX_MASS is a positive number', () => {
    expect(typeof GAS_MAX_MASS).toBe('number');
    expect(GAS_MAX_MASS).toBeGreaterThan(0);
  });

  it('GAS_BACKGROUND is a non-empty string', () => {
    expect(typeof GAS_BACKGROUND).toBe('string');
    expect(GAS_BACKGROUND.length).toBeGreaterThan(0);
  });
});
