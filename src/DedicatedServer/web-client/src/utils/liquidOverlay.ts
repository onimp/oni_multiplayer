/**
 * liquidOverlay.ts — pure functions: liquid element + mass → canvas color string.
 *
 * Same interface as gasOverlay.ts.  WorldRenderer calls liquidOverlayColor()
 * only for Liquid-state cells (it checks getElementState() first).
 *
 * Liquids fill cells up to ~1000 kg; the color scale uses LIQUID_MAX_MASS = 1000
 * with √-scaling so even a shallow puddle (10 kg) is visible.
 */

/** Background color for non-liquid cells in liquid overlay mode. */
export const LIQUID_BACKGROUND = '#0a0a0a';

/**
 * Mass (kg) at which a liquid cell reaches full color intensity.
 * ONI full cell is ~1000 kg for most liquids.
 */
export const LIQUID_MAX_MASS = 1000;

type RGB = readonly [number, number, number];

// ── Per-liquid RGB colors at full intensity ──────────────────────────────────
// Keys match element PrefabIDs from /api/elements.

const LIQUID_COLORS: Record<string, RGB> = {
  // Common liquids (task-specified)
  Water:                   [  40, 130, 230],   // blue
  DirtyWater:              [  80, 130,  80],   // green-grey (same substance as PollutedWater)
  PollutedWater:           [  80, 130,  80],   // green-grey
  Brine:                   [  40, 160, 160],   // teal
  SaltWater:               [  50, 180, 200],   // cyan
  CrudeOil:                [  60,  35,  15],   // dark brown
  Petroleum:               [ 180, 110,  30],   // amber
  Ethanol:                 [ 220, 180,  50],   // gold
  Magma:                   [ 255,  80,  20],   // red-orange
  Naphtha:                 [ 130,  80,  20],   // dark amber
  Mercury:                 [ 190, 190, 210],   // silver-grey

  // Cryogenic / exotic liquids
  LiquidOxygen:            [  80, 180, 255],   // pale sky blue
  LiquidHydrogen:          [ 180, 210, 255],   // icy blue-white
  LiquidCarbonDioxide:     [ 140, 140, 170],   // grey-blue
  LiquidSulfur:            [ 230, 220,  50],   // bright yellow
  LiquidPhosphorus:        [ 130, 230,  80],   // green
  LiquidMethane:           [ 170, 200, 230],   // pale blue
  SuperCoolant:            [   0, 240, 220],   // vivid teal

  // Molten metals
  MoltenIron:              [ 255, 120,  30],   // bright orange
  MoltenGold:              [ 255, 200,  40],   // golden yellow
  MoltenCopper:            [ 220, 110,  50],   // copper-orange
  MoltenGlass:             [ 255, 160,  80],   // light orange
  MoltenAluminum:          [ 200, 200, 220],   // pale silver
  MoltenTungsten:          [ 200, 160, 100],   // light bronze
  MoltenNiobium:           [ 140, 100, 230],   // purple-ish

  // Other
  ViscoGel:                [ 160,  80, 200],   // purple
  PhytoOil:                [ 160, 200,  80],   // yellow-green
};

/**
 * Returns the CSS rgb() color for a liquid-state cell.
 * Caller must only pass Liquid-state elements.
 *
 * Intensity uses √(mass / LIQUID_MAX_MASS) so shallow puddles remain visible
 * while full cells (1000 kg) show maximum saturation.
 *
 * @param elementName - PrefabID from /api/elements, e.g. "Water"
 * @param mass        - cell mass in kg (from /api/world m[] array)
 */
export function liquidOverlayColor(elementName: string, mass: number): string {
  if (mass <= 0) return LIQUID_BACKGROUND;

  const clamped   = Math.min(Math.max(mass, 0), LIQUID_MAX_MASS);
  const intensity = Math.sqrt(clamped / LIQUID_MAX_MASS);   // √ curve

  const rgb = LIQUID_COLORS[elementName];
  if (!rgb) {
    // Unknown liquid — neutral blue-grey, pressure-scaled
    const v = Math.round(30 + intensity * 110);
    return `rgb(${Math.round(v * 0.6)},${Math.round(v * 0.7)},${v})`;
  }

  const BG = 10;
  const r = Math.round(BG + (rgb[0] - BG) * intensity);
  const g = Math.round(BG + (rgb[1] - BG) * intensity);
  const b = Math.round(BG + (rgb[2] - BG) * intensity);
  return `rgb(${r},${g},${b})`;
}
