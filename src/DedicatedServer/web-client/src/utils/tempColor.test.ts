import { describe, it, expect } from 'vitest';
import { temperatureToColor } from './tempColor';

/**
 * Tests for the ONI temperature-to-color mapping.
 *
 * Bug being covered: the old renderer used a hand-rolled RGB gradient
 * (blue→cyan→green→yellow→red) that does NOT match the game.  The real
 * formula from SimDebugView.TemperatureToColor is HSV-based:
 *
 *   ratio = clamp01((temp - 173.15) / 250)
 *   hue   = (10 + (1 - ratio) * 171) / 360
 *   color = HSVToRGB(hue, 1, 1)
 *
 * Expected palette (precomputed from the HSV formula):
 *   173.15 K (-100°C) → cyan        rgb(0,   251, 255)
 *   235.65 K ( -37°C) → green       rgb(0,   255,  78)
 *   298.15 K (  25°C) → lime        rgb(104, 255,   0)
 *   360.65 K (  87°C) → yellow      rgb(255, 224,   0)
 *   423.15 K ( 150°C) → orange-red  rgb(255,  43,   0)
 *
 * The old code would produce:
 *   173.15 K → rgb(0, 0, 180)    (blue — WRONG, should be cyan)
 *   423.15 K → rgb(255, 0, 0)    (pure red — WRONG, should be orange-red)
 */

/** Parse "rgb(r,g,b)" → [r,g,b] numbers. */
function parseRgb(css: string): [number, number, number] {
  const m = css.match(/rgb\((\d+),\s*(\d+),\s*(\d+)\)/);
  if (!m) throw new Error(`Not an rgb() string: ${css}`);
  return [parseInt(m[1]), parseInt(m[2]), parseInt(m[3])];
}

/** Allow ±2 rounding tolerance on each channel. */
function expectRgbClose(actual: string, expected: [number, number, number], label: string) {
  const [ar, ag, ab] = parseRgb(actual);
  const [er, eg, eb] = expected;
  expect(Math.abs(ar - er), `${label} R: got ${ar}, expected ~${er}`).toBeLessThanOrEqual(2);
  expect(Math.abs(ag - eg), `${label} G: got ${ag}, expected ~${eg}`).toBeLessThanOrEqual(2);
  expect(Math.abs(ab - eb), `${label} B: got ${ab}, expected ~${eb}`).toBeLessThanOrEqual(2);
}

describe('temperatureToColor — ONI HSV formula', () => {
  it('min temp 173.15 K → cyan (cold end of scale)', () => {
    expectRgbClose(temperatureToColor(173.15), [0, 251, 255], '173.15 K');
  });

  it('235.65 K (quarter point) → green', () => {
    expectRgbClose(temperatureToColor(235.65), [0, 255, 78], '235.65 K');
  });

  it('298.15 K (room temp, midpoint) → lime-green', () => {
    expectRgbClose(temperatureToColor(298.15), [104, 255, 0], '298.15 K');
  });

  it('360.65 K (three-quarter point) → yellow', () => {
    expectRgbClose(temperatureToColor(360.65), [255, 224, 0], '360.65 K');
  });

  it('max temp 423.15 K → orange-red (hot end of scale)', () => {
    expectRgbClose(temperatureToColor(423.15), [255, 43, 0], '423.15 K');
  });

  // --- regression: old code produced wrong colours at these points ---
  it('cold end is NOT blue (old code gave rgb(0,0,180))', () => {
    const [r, g, b] = parseRgb(temperatureToColor(173.15));
    // Must have significant blue AND green (cyan), not dominant blue alone
    expect(b).toBeGreaterThan(200);
    expect(g).toBeGreaterThan(200);
    expect(r).toBeLessThan(30);
  });

  it('hot end is NOT pure red (old code gave rgb(255,0,0)) — must have orange tint', () => {
    const [r, g, b] = parseRgb(temperatureToColor(423.15));
    expect(r).toBe(255);
    expect(g).toBeGreaterThan(20);   // orange tint (G > 0)
    expect(b).toBeLessThan(10);
  });

  // --- boundary / edge cases ---
  it('below min is clamped to cold colour', () => {
    expect(temperatureToColor(0)).toBe(temperatureToColor(173.15));
  });

  it('above max is clamped to hot colour', () => {
    expect(temperatureToColor(9999)).toBe(temperatureToColor(423.15));
  });

  it('returns a css rgb() string', () => {
    expect(temperatureToColor(300)).toMatch(/^rgb\(\d+,\d+,\d+\)$/);
  });
});
