/**
 * tempOverlay.ts — thin adapter layer following the gas/liquid overlay pattern.
 *
 * Delegates to the ONI-accurate HSV formula in tempColor.ts so the color
 * math stays in one place (already tested in tempColor.test.ts).
 *
 * Data source: /api/world t[] array — temperature in Kelvin per cell.
 * Already present in the WorldData response; no new endpoint needed.
 *
 * Palette (matches SimDebugView.TemperatureToColor):
 *   TEMP_MIN_K  (~173 K / -100°C) → cyan
 *   ~236 K / -37°C               → green
 *   ~298 K /  25°C (room temp)   → lime
 *   ~361 K /  88°C               → yellow
 *   TEMP_MAX_K  (~423 K / +150°C) → orange-red
 */

import { temperatureToColor } from './tempColor';

/** Lower display bound — below this everything clamps to cyan. */
export const TEMP_MIN_K = 173.15;   // –100°C

/** Upper display bound — above this everything clamps to orange-red. */
export const TEMP_MAX_K = 423.15;   // +150°C

/**
 * Returns the CSS rgb() color for a cell's temperature in the temperature
 * overlay mode.  Uses the same ONI HSV formula as the in-game temperature HUD.
 *
 * @param temperatureK - Cell temperature from /api/world t[] (Kelvin)
 */
export function tempOverlayColor(temperatureK: number): string {
  return temperatureToColor(temperatureK, TEMP_MIN_K, TEMP_MAX_K);
}
