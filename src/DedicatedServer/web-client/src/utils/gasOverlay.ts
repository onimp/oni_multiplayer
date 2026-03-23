/**
 * gasOverlay.ts — pure functions: gas element + mass → canvas color string.
 *
 * ONI cells are single-element (no true per-cell gas mixing).  The "blend"
 * in the overlay spec refers to the color-space design: O2 maps to blue,
 * CO2 to yellow; other gases occupy their own hues; all scale by pressure.
 *
 * WorldRenderer calls gasOverlayColor() only for Gas-state cells
 * (it checks getElementState() first).  Non-gas cells use GAS_BACKGROUND.
 */

/** Background color for vacuum, solid, and liquid cells in gas overlay mode. */
export const GAS_BACKGROUND = '#0a0a0a';

/**
 * Mass (kg) at which a gas cell reaches full color intensity.
 * ONI standard atmosphere is ~1.8 kg/cell; 1.0 gives a useful mid-range scale.
 * Above this the color clamps — the cell is still shown at full brightness.
 */
export const GAS_MAX_MASS = 1.0;

// ── Per-gas RGB colors at full intensity ────────────────────────────────────
// Keys match element names sent by /api/elements (case-sensitive PrefabIDs).

type RGB = readonly [number, number, number];

const GAS_COLORS: Record<string, RGB> = {
  // Primary gases (O2 blue, CO2 yellow — matching the spec)
  Oxygen:              [  30, 120, 255],   // blue
  CarbonDioxide:       [ 255, 200,  50],   // yellow
  ContaminatedOxygen:  [  60, 200, 110],   // green
  Hydrogen:            [ 180, 160, 255],   // light purple
  ChlorineGas:         [  70, 210,  70],   // bright green
  Chlorine:            [  70, 210,  70],   // same (some ONI versions use this name)
  Methane:             [ 190, 130,  70],   // brownish-orange
  Steam:               [ 200, 200, 230],   // pale blue-white
  NaphthGas:           [  90,  65,  30],   // dark amber
  PhosphorusGas:       [ 180, 255,  70],   // yellow-green
  SourGas:             [ 190, 110,  30],   // dark amber
  // Rarer gases
  Helium:              [ 255, 180, 200],   // pink
  Neon:                [ 255,  80, 150],   // hot pink
  EthanolGas:          [ 230, 185, 120],   // light tan
};

/**
 * Returns the CSS rgb() color for a gas-state cell given its element name
 * and mass.  Caller is responsible for only passing Gas-state elements.
 *
 * Color intensity uses √(mass / GAS_MAX_MASS) — square-root scaling makes
 * low-pressure gas visible while not washing out the full-pressure end.
 *
 * @param elementName  - PrefabID string from /api/elements, e.g. "Oxygen"
 * @param mass         - cell mass in kg (from /api/world m[] array)
 * @returns            - CSS color string, or GAS_BACKGROUND when mass ≤ 0
 */
export function gasOverlayColor(elementName: string, mass: number): string {
  if (mass <= 0) return GAS_BACKGROUND;

  const clamped   = Math.min(Math.max(mass, 0), GAS_MAX_MASS);
  const intensity = Math.sqrt(clamped / GAS_MAX_MASS);   // 0→0, 1→1, sqrt curve

  const rgb = GAS_COLORS[elementName];
  if (!rgb) {
    // Unknown gas — neutral grey, still pressure-scaled for visibility
    const v = Math.round(30 + intensity * 100);
    return `rgb(${v},${v},${v})`;
  }

  // Linearly interpolate from the dark background (10,10,10) to full color.
  // This keeps zero-pressure cells dark while max-pressure shows full hue.
  const BG = 10;
  const r = Math.round(BG + (rgb[0] - BG) * intensity);
  const g = Math.round(BG + (rgb[1] - BG) * intensity);
  const b = Math.round(BG + (rgb[2] - BG) * intensity);
  return `rgb(${r},${g},${b})`;
}
