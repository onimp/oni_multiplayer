import { describe, it, expect } from 'vitest';
import {
  liquidOverlayColor,
  LIQUID_BACKGROUND,
  LIQUID_MAX_MASS,
} from './liquidOverlay';

// ── helpers ───────────────────────────────────────────────────────────────────

function parseRgb(s: string): [number, number, number] {
  const m = s.match(/^rgb\((\d+),(\d+),(\d+)\)$/);
  if (!m) throw new Error(`not an rgb string: "${s}"`);
  return [+m[1], +m[2], +m[3]];
}

// ── zero / no-liquid cases ────────────────────────────────────────────────────

describe('liquidOverlayColor — zero mass', () => {
  it('returns LIQUID_BACKGROUND for Water at mass 0', () => {
    expect(liquidOverlayColor('Water', 0)).toBe(LIQUID_BACKGROUND);
  });

  it('returns LIQUID_BACKGROUND for Petroleum at mass 0', () => {
    expect(liquidOverlayColor('Petroleum', 0)).toBe(LIQUID_BACKGROUND);
  });

  it('returns LIQUID_BACKGROUND for negative mass', () => {
    expect(liquidOverlayColor('Water', -5)).toBe(LIQUID_BACKGROUND);
  });
});

// ── Task-specified liquids ────────────────────────────────────────────────────

describe('liquidOverlayColor — Water (blue)', () => {
  it('blue channel dominates at full mass', () => {
    const [r, g, b] = parseRgb(liquidOverlayColor('Water', LIQUID_MAX_MASS));
    expect(b).toBeGreaterThan(r);
    expect(b).toBeGreaterThan(g);
  });

  it('full mass is not LIQUID_BACKGROUND', () => {
    expect(liquidOverlayColor('Water', LIQUID_MAX_MASS)).not.toBe(LIQUID_BACKGROUND);
  });
});

describe('liquidOverlayColor — PollutedWater (green-grey)', () => {
  it('green channel dominates at full mass', () => {
    const [r, g, b] = parseRgb(liquidOverlayColor('PollutedWater', LIQUID_MAX_MASS));
    expect(g).toBeGreaterThan(r);
    expect(g).toBeGreaterThan(b);
  });

  it('DirtyWater alias returns same color as PollutedWater', () => {
    expect(liquidOverlayColor('DirtyWater', 500))
      .toBe(liquidOverlayColor('PollutedWater', 500));
  });
});

describe('liquidOverlayColor — Brine (teal)', () => {
  it('both green and blue channels are dominant at full mass', () => {
    const [r, g, b] = parseRgb(liquidOverlayColor('Brine', LIQUID_MAX_MASS));
    expect(g).toBeGreaterThan(r);
    expect(b).toBeGreaterThan(r);
  });
});

describe('liquidOverlayColor — CrudeOil (dark brown)', () => {
  it('red channel is highest at full mass', () => {
    const [r, g, b] = parseRgb(liquidOverlayColor('CrudeOil', LIQUID_MAX_MASS));
    expect(r).toBeGreaterThan(g);
    expect(r).toBeGreaterThan(b);
  });

  it('all channels are relatively low (dark brown)', () => {
    const [r, g, b] = parseRgb(liquidOverlayColor('CrudeOil', LIQUID_MAX_MASS));
    expect(Math.max(r, g, b)).toBeLessThan(100);
  });
});

describe('liquidOverlayColor — Magma (red-orange)', () => {
  it('red channel is dominant', () => {
    const [r, , b] = parseRgb(liquidOverlayColor('Magma', LIQUID_MAX_MASS));
    expect(r).toBeGreaterThan(b);
    expect(r).toBeGreaterThan(150);
  });
});

describe('liquidOverlayColor — Mercury (silver)', () => {
  it('all channels are close to each other (grey/silver)', () => {
    const [r, g, b] = parseRgb(liquidOverlayColor('Mercury', LIQUID_MAX_MASS));
    expect(Math.abs(r - g)).toBeLessThan(30);
    expect(Math.abs(g - b)).toBeLessThan(30);
  });
});

// ── All task-specified liquids are distinct ───────────────────────────────────

describe('liquidOverlayColor — all task-specified liquids distinct', () => {
  const liquids = [
    'Water', 'PollutedWater', 'Brine', 'SaltWater',
    'CrudeOil', 'Petroleum', 'Ethanol', 'Magma', 'Naphtha', 'Mercury',
  ];

  it('every specified liquid returns a non-background rgb() at full mass', () => {
    for (const l of liquids) {
      const color = liquidOverlayColor(l, LIQUID_MAX_MASS);
      expect(color, `${l} should not be background`).not.toBe(LIQUID_BACKGROUND);
      expect(color, `${l} should be an rgb() string`).toMatch(/^rgb\(\d+,\d+,\d+\)$/);
    }
  });

  it('all 10 specified liquids produce distinct colors at full mass', () => {
    const colors = liquids.map(l => liquidOverlayColor(l, LIQUID_MAX_MASS));
    const unique = new Set(colors);
    expect(unique.size).toBe(liquids.length);
  });
});

// ── Mass scaling ──────────────────────────────────────────────────────────────

describe('liquidOverlayColor — mass scaling', () => {
  it('higher mass → higher channel values (Water)', () => {
    const lo = parseRgb(liquidOverlayColor('Water', 10));
    const hi = parseRgb(liquidOverlayColor('Water', 1000));
    expect(hi[2]).toBeGreaterThan(lo[2]); // blue
  });

  it('mass above LIQUID_MAX_MASS clamps to same color as LIQUID_MAX_MASS', () => {
    expect(liquidOverlayColor('Water', LIQUID_MAX_MASS * 2))
      .toBe(liquidOverlayColor('Water', LIQUID_MAX_MASS));
  });

  it('very small mass (1 kg out of 1000) is still visible, not background', () => {
    const color = liquidOverlayColor('Water', 1);
    expect(color).not.toBe(LIQUID_BACKGROUND);
    expect(color).toMatch(/^rgb\(\d+,\d+,\d+\)$/);
  });

  it('mid-mass (500 kg) is dimmer than full (1000 kg)', () => {
    const [,,bHalf] = parseRgb(liquidOverlayColor('Water', 500));
    const [,,bFull] = parseRgb(liquidOverlayColor('Water', 1000));
    expect(bHalf).toBeLessThan(bFull);
  });
});

// ── Unknown liquid fallback ───────────────────────────────────────────────────

describe('liquidOverlayColor — unknown liquid fallback', () => {
  it('returns an rgb() string for unknown liquid with mass > 0', () => {
    const color = liquidOverlayColor('FutureLiquid_XYZ', 500);
    expect(color).toMatch(/^rgb\(\d+,\d+,\d+\)$/);
    expect(color).not.toBe(LIQUID_BACKGROUND);
  });

  it('unknown liquid is brighter at higher pressure', () => {
    const [,,b1] = parseRgb(liquidOverlayColor('UnknownLiquid', 10));
    const [,,b2] = parseRgb(liquidOverlayColor('UnknownLiquid', 1000));
    expect(b2).toBeGreaterThan(b1);
  });

  it('unknown liquid has a blue-ish tint (b >= r)', () => {
    const [r,,b] = parseRgb(liquidOverlayColor('AnyUnknown', 500));
    expect(b).toBeGreaterThanOrEqual(r);
  });
});

// ── Constants ─────────────────────────────────────────────────────────────────

describe('liquidOverlay — constants', () => {
  it('LIQUID_MAX_MASS is a positive number', () => {
    expect(typeof LIQUID_MAX_MASS).toBe('number');
    expect(LIQUID_MAX_MASS).toBeGreaterThan(0);
  });

  it('LIQUID_MAX_MASS is substantially larger than GAS scale (liquids are denser)', () => {
    // Gas max is 1 kg; liquids fill to ~1000 kg — at least 100× difference
    expect(LIQUID_MAX_MASS).toBeGreaterThanOrEqual(100);
  });

  it('LIQUID_BACKGROUND is a non-empty string', () => {
    expect(typeof LIQUID_BACKGROUND).toBe('string');
    expect(LIQUID_BACKGROUND.length).toBeGreaterThan(0);
  });
});
