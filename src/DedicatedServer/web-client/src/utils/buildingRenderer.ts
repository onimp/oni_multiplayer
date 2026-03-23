/**
 * buildingRenderer.ts — pure functions for building footprint rendering.
 *
 * No canvas API calls here — only math and color lookup.
 * WorldRenderer imports these and performs the actual draw calls.
 */

/** Minimum cellSize (px) at which a building name label is drawn. */
export const BUILDING_LABEL_MIN_CELL_SIZE = 8;

// ── Well-known building colors ────────────────────────────────────────────────
// Keyed on the BuildingDef.PrefabID that the server sends as entity.name.
// Unknown buildings fall through to hashBuildingColor().

const BUILDING_COLORS: Record<string, string> = {
  // Command
  Headquarters:                 '#e94560',

  // Research
  ResearchCenter:               '#4ca3dd',
  AdvancedResearchCenter:       '#2e86de',
  CosmicResearchCenter:         '#a29bfe',
  HighEnergyParticleSpawner:    '#6c5ce7',

  // Hygiene
  Outhouse:                     '#8B4513',
  FlushToilet:                  '#74b9ff',
  Shower:                       '#00cec9',
  WashBasin:                    '#81ecec',
  HandSanitizer:                '#55efc4',

  // Comfort
  Bed:                          '#fdcb6e',
  LuxuryBed:                    '#e17055',
  Cot:                          '#fdcb6e',
  MassageTable:                 '#fd79a8',

  // Food
  Farm:                         '#00b894',
  PlanterBox:                   '#2ecc71',
  Grill:                        '#e17055',
  CookingStation:               '#e07030',
  MicrobeMusher:                '#fdcb6e',
  EggCracker:                   '#f39c12',
  ClothingFabricator:           '#9b59b6',

  // Power
  ManualGenerator:              '#d63031',
  Generator:                    '#e17055',
  SolarPanel:                   '#74b9ff',
  Battery:                      '#ffeaa7',
  LargeBattery:                 '#f1c40f',
  Wire:                         '#dfe6e9',
  WireHighWattage:              '#fdcb6e',
  WireRefined:                  '#ecf0f1',

  // Gas systems
  GasPump:                      '#6c5ce7',
  GasFilter:                    '#55efc4',
  GasVent:                      '#74b9ff',
  GasVentHighPressure:          '#0984e3',
  GasReservoir:                 '#a29bfe',
  GasPipe:                      '#b2bec3',
  Electrolyzer:                 '#0984e3',
  AlgaeOxygenMachine:           '#27ae60',
  RustDeoxidizer:               '#e67e22',

  // Liquid systems
  LiquidPump:                   '#2d3436',
  LiquidFilter:                 '#00b894',
  LiquidVent:                   '#0984e3',
  LiquidReservoir:              '#2980b9',
  LiquidPipe:                   '#636e72',
  WaterPurifier:                '#3498db',
  IceCooledFan:                 '#aed6f1',
  SteamTurbine:                 '#a29bfe',

  // Doors & transit
  ManualPressureDoor:           '#b2bec3',
  Door:                         '#636e72',
  Airlock:                      '#7f8c8d',
  Ladder:                       '#ecf0f1',
  FirePole:                     '#e74c3c',
  TravelTube:                   '#3498db',
  TransitTube:                  '#2980b9',

  // Sensors & automation
  HeatDetector:                 '#e17055',
  ThermalSensor:                '#e74c3c',
  Thermometer:                  '#c0392b',
  LogicWire:                    '#1abc9c',
  LogicGateAnd:                 '#16a085',
  LogicGateOr:                  '#16a085',

  // Storage
  StorageLocker:                '#95a5a6',
  StorageLockerSmart:           '#7f8c8d',
  ObjectDispenser:              '#bdc3c7',
  ConveyorLoader:               '#2c3e50',
  ConveyorReceptacle:           '#34495e',

  // Medical
  DoctorStation:                '#e74c3c',
  HospitalBed:                  '#ecf0f1',
  Apothecary:                   '#c0392b',

  // Comfort / recreation
  Arcade:                       '#fd79a8',
  Espresso:                     '#6d4c41',
  Telescope:                    '#2c3e50',
  NatureReserve:                '#27ae60',

  // Misc
  Campfire:                     '#e74c3c',
  Desk:                         '#8e44ad',
};

/**
 * Deterministic hue-based color for unknown building types.
 * Same name always produces the same color; uses djb2-style hash.
 */
export function hashBuildingColor(name: string): string {
  let h = 5381;
  for (let i = 0; i < name.length; i++) {
    h = (((h << 5) + h) + name.charCodeAt(i)) >>> 0;
  }
  const hue = h % 360;
  const sat = 50 + (h % 20);
  const lit = 38 + (h % 16);
  return `hsl(${hue},${sat}%,${lit}%)`;
}

/** Returns a distinct color for the given building prefab name. */
export function colorForBuilding(name: string): string {
  return BUILDING_COLORS[name] ?? hashBuildingColor(name);
}

// ── Coordinate mapping ────────────────────────────────────────────────────────

export interface BuildingRect {
  /** Canvas X of the top-left corner (px). */
  sx: number;
  /** Canvas Y of the top-left corner (px). */
  sy: number;
  /** Pixel width (entity.w * cellSize). */
  pw: number;
  /** Pixel height (entity.h * cellSize). */
  ph: number;
  /** Fill/stroke color. */
  color: string;
  /**
   * Building name to draw as label, or null when cellSize is too small.
   * WorldRenderer handles truncation with measureText.
   */
  label: string | null;
}

/**
 * Maps a building entity to canvas rectangle coordinates.
 *
 * @param entity   - building data from /api/entities (x, y in world cells; w, h in cells)
 * @param worldHeight - world height in cells (used for Y-axis flip: ONI y=0 is world bottom)
 * @param offsetX  - renderer pan offset X (px)
 * @param offsetY  - renderer pan offset Y (px)
 * @param cellSize - current zoom level (px per cell)
 */
export function buildingToRect(
  entity: { name: string; x: number; y: number; w: number; h: number },
  worldHeight: number,
  offsetX: number,
  offsetY: number,
  cellSize: number,
): BuildingRect {
  // Y-axis flip: ONI y=0 is world bottom; canvas y=0 is top.
  // Building occupies rows [y .. y+h-1]; top-left canvas corner is at (worldHeight - y - h).
  const sx = offsetX + entity.x * cellSize;
  const sy = offsetY + (worldHeight - entity.y - entity.h) * cellSize;
  const pw = entity.w * cellSize;
  const ph = entity.h * cellSize;
  return {
    sx,
    sy,
    pw,
    ph,
    color: colorForBuilding(entity.name),
    label: cellSize >= BUILDING_LABEL_MIN_CELL_SIZE ? entity.name : null,
  };
}
