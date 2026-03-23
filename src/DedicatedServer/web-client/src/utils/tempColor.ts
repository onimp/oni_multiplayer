/**
 * ONI temperature-to-color mapping — matches SimDebugView.TemperatureToColor().
 *
 * Bug fixed: the previous renderer used a hand-rolled RGB gradient
 * (blue→cyan→green→yellow→red) which does NOT match the game.  The actual
 * game formula is HSV-based:
 *
 *   ratio = clamp01((temp - min) / (max - min))
 *   hue   = (10 + (1 - ratio) * 171) / 360   // ~0.503 cold → ~0.028 hot
 *   color = HSVToRGB(hue, 1, 1)
 *
 * Resulting palette:
 *   cold (~173 K)  → cyan       rgb(0,   251, 255)
 *   cool (~236 K)  → green      rgb(0,   255,  78)
 *   room (~298 K)  → lime       rgb(104, 255,   0)
 *   warm (~361 K)  → yellow     rgb(255, 224,   0)
 *   hot  (~423 K)  → orange-red rgb(255,  43,   0)
 *
 * Source: Assembly-CSharp SimDebugView.TemperatureToColor (decompiled).
 */

/** Standard HSV→RGB conversion (H,S,V all in [0,1]). */
function hsvToRgb(h: number, s: number, v: number): [number, number, number] {
  const i = Math.floor(h * 6);
  const f = h * 6 - i;
  const p = v * (1 - s);
  const q = v * (1 - f * s);
  const t = v * (1 - (1 - f) * s);
  switch (i % 6) {
    case 0: return [v, t, p];
    case 1: return [q, v, p];
    case 2: return [p, v, t];
    case 3: return [p, q, v];
    case 4: return [t, p, v];
    case 5: return [v, p, q];
    default: return [0, 0, 0];
  }
}

/**
 * Converts a temperature in Kelvin to a CSS rgb() string matching the ONI
 * temperature overlay palette.
 *
 * @param temperature - Cell temperature in Kelvin
 * @param minTemp     - Lower bound of display range (default 173.15 K = -100°C)
 * @param maxTemp     - Upper bound of display range (default 423.15 K = +150°C)
 */
export function temperatureToColor(
  temperature: number,
  minTemp = 173.15,
  maxTemp = 423.15,
): string {
  const ratio = Math.max(0, Math.min(1, (temperature - minTemp) / (maxTemp - minTemp)));
  const hue   = (10 + (1 - ratio) * 171) / 360;
  const [r, g, b] = hsvToRgb(hue, 1, 1);
  return `rgb(${Math.round(r * 255)},${Math.round(g * 255)},${Math.round(b * 255)})`;
}
