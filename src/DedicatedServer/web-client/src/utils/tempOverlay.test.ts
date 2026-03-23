import { describe, it, expect } from 'vitest';
import { tempOverlayColor, TEMP_MIN_K, TEMP_MAX_K } from './tempOverlay';
import { temperatureToColor } from './tempColor';

// ── helper ────────────────────────────────────────────────────────────────────

function parseRgb(s: string): [number, number, number] {
  const m = s.match(/^rgb\((\d+),(\d+),(\d+)\)$/);
  if (!m) throw new Error(`not an rgb string: "${s}"`);
  return [+m[1], +m[2], +m[3]];
}

// ── wiring: tempOverlayColor delegates to temperatureToColor ─────────────────
// These tests verify the adapter layer, not the color math itself
// (that is covered by tempColor.test.ts).

describe('tempOverlayColor — wiring to temperatureToColor', () => {
  it('returns the same value as temperatureToColor() with default bounds', () => {
    const temps = [173.15, 236, 298.15, 361, 423.15, 0, 500, 1000];
    for (const t of temps) {
      expect(tempOverlayColor(t)).toBe(temperatureToColor(t, TEMP_MIN_K, TEMP_MAX_K));
    }
  });

  it('returns an rgb() string for any temperature', () => {
    for (const t of [100, 200, 273.15, 298, 373, 500]) {
      expect(tempOverlayColor(t)).toMatch(/^rgb\(\d+,\d+,\d+\)$/);
    }
  });
});

// ── cold end: TEMP_MIN_K → cyan ───────────────────────────────────────────────

describe('tempOverlayColor — cold end (TEMP_MIN_K ≈ 173 K)', () => {
  it('at TEMP_MIN_K blue channel is dominant (cyan palette)', () => {
    const [r, , b] = parseRgb(tempOverlayColor(TEMP_MIN_K));
    // Cyan: r ≈ 0, g ≈ 255, b ≈ 255 — both g and b high; r low
    expect(b).toBeGreaterThan(200);
    expect(r).toBeLessThan(50);
  });

  it('below TEMP_MIN_K clamps to same color as TEMP_MIN_K', () => {
    expect(tempOverlayColor(TEMP_MIN_K - 50)).toBe(tempOverlayColor(TEMP_MIN_K));
  });

  it('absolute zero still returns a valid rgb() string', () => {
    expect(tempOverlayColor(0)).toMatch(/^rgb\(\d+,\d+,\d+\)$/);
  });
});

// ── mid range: room temperature → lime-green ─────────────────────────────────

describe('tempOverlayColor — room temperature (~298 K)', () => {
  it('green channel is dominant at room temp', () => {
    const [r, g, b] = parseRgb(tempOverlayColor(298.15));
    expect(g).toBeGreaterThan(r);
    expect(g).toBeGreaterThan(b);
    expect(g).toBeGreaterThan(200);
  });
});

// ── hot end: TEMP_MAX_K → orange-red ─────────────────────────────────────────

describe('tempOverlayColor — hot end (TEMP_MAX_K ≈ 423 K)', () => {
  it('at TEMP_MAX_K red channel is dominant (orange-red palette)', () => {
    const [r, g, b] = parseRgb(tempOverlayColor(TEMP_MAX_K));
    expect(r).toBeGreaterThan(g);
    expect(r).toBeGreaterThan(b);
    expect(r).toBeGreaterThan(200);
  });

  it('above TEMP_MAX_K clamps to same color as TEMP_MAX_K', () => {
    expect(tempOverlayColor(TEMP_MAX_K + 200)).toBe(tempOverlayColor(TEMP_MAX_K));
  });
});

// ── monotonic gradient ────────────────────────────────────────────────────────

describe('tempOverlayColor — gradient is monotonic cold→hot', () => {
  it('hotter temperatures produce more red (overall trend)', () => {
    // Sample across the full range; red should trend upward
    const cold = parseRgb(tempOverlayColor(TEMP_MIN_K));
    const hot  = parseRgb(tempOverlayColor(TEMP_MAX_K));
    expect(hot[0]).toBeGreaterThan(cold[0]);  // red: hot > cold
  });

  it('colder temperatures produce more blue (overall trend)', () => {
    const cold = parseRgb(tempOverlayColor(TEMP_MIN_K));
    const hot  = parseRgb(tempOverlayColor(TEMP_MAX_K));
    expect(cold[2]).toBeGreaterThan(hot[2]);  // blue: cold > hot
  });
});

// ── constants ─────────────────────────────────────────────────────────────────

describe('tempOverlay — constants', () => {
  it('TEMP_MIN_K is below freezing (~173 K = –100°C)', () => {
    expect(TEMP_MIN_K).toBeLessThan(273.15);
    expect(TEMP_MIN_K).toBeGreaterThan(0);
  });

  it('TEMP_MAX_K is above boiling (~423 K = +150°C)', () => {
    expect(TEMP_MAX_K).toBeGreaterThan(373.15);
  });

  it('TEMP_MAX_K > TEMP_MIN_K', () => {
    expect(TEMP_MAX_K).toBeGreaterThan(TEMP_MIN_K);
  });
});
