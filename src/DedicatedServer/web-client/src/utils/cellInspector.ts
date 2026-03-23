/**
 * cellInspector.ts — pure formatting utilities for the cell inspector panel.
 *
 * No canvas API, no DOM, no imports from game data modules.
 * All functions take plain values and return plain objects/strings so they
 * can be unit-tested in Node/vitest without a browser.
 */

// ── Input / Output types ──────────────────────────────────────────────────────

/** Raw cell data passed from WorldCanvas to the inspector formatter. */
export interface CellInspectorInput {
  x: number;
  y: number;
  /** Element name string (e.g. "Water", "Oxygen", "Granite"). */
  element: string;
  /** Physical state: "Gas" | "Liquid" | "Solid" | "Vacuum" — passed by caller. */
  elementState: string;
  /** Temperature in Kelvin. */
  temperature: number;
  /** Pre-computed Celsius (temperature - 273.15). */
  temperatureC: number;
  /** Mass in kilograms. */
  mass: number;
  /** Entity descriptions already formatted as strings, e.g. "Bob (duplicant)". */
  entities?: string[];
}

/** Formatted display object — all fields are ready-to-render strings. */
export interface CellInspectorData {
  coords: string;
  element: string;
  elementState: string;
  mass: string;
  temperatureK: string;
  temperatureC: string;
  temperatureLabel: string;
  /** Hex color hint for the temperature label badge. */
  temperatureColor: string;
  entities: string[];
}

// ── Pure helpers ──────────────────────────────────────────────────────────────

/**
 * Returns a human-readable temperature label for a Kelvin value.
 *
 * Thresholds (rough ONI / real-world feel):
 *  < 173 K  (-100°C)  →  "Absolute Cold"
 *  < 253 K  ( -20°C)  →  "Frozen"
 *  < 283 K  (  10°C)  →  "Cold"
 *  < 303 K  (  30°C)  →  "Room Temp"
 *  < 333 K  (  60°C)  →  "Warm"
 *  < 423 K  ( 150°C)  →  "Hot"
 *  ≥ 423 K             →  "Scalding"
 */
export function temperatureLabel(K: number): string {
  if (K < 173.15) return 'Absolute Cold';
  if (K < 253.15) return 'Frozen';
  if (K < 283.15) return 'Cold';
  if (K < 303.15) return 'Room Temp';
  if (K < 333.15) return 'Warm';
  if (K < 423.15) return 'Hot';
  return 'Scalding';
}

/**
 * Maps a temperature label to a display color.
 * Colors are chosen to be readable on a dark background.
 */
export function temperatureLabelColor(label: string): string {
  switch (label) {
    case 'Absolute Cold': return '#a0c4ff';
    case 'Frozen':        return '#74b9ff';
    case 'Cold':          return '#81ecec';
    case 'Room Temp':     return '#55efc4';
    case 'Warm':          return '#ffeaa7';
    case 'Hot':           return '#fdcb6e';
    case 'Scalding':      return '#e17055';
    default:              return '#aaaaaa';
  }
}

/**
 * Formats mass in kg with appropriate precision.
 * Very small masses shown in grams for readability.
 */
export function formatMass(mass: number): string {
  if (mass <= 0) return '0 kg';
  if (mass < 0.001) return `${(mass * 1000).toFixed(3)} g`;
  if (mass < 1)     return `${mass.toFixed(3)} kg`;
  if (mass < 100)   return `${mass.toFixed(2)} kg`;
  return `${mass.toFixed(1)} kg`;
}

/**
 * Returns a display-friendly element state string with optional emoji prefix.
 */
export function formatElementState(state: string): string {
  switch (state) {
    case 'Gas':    return '💨 Gas';
    case 'Liquid': return '💧 Liquid';
    case 'Solid':  return '🪨 Solid';
    case 'Vacuum': return '✨ Vacuum';
    default:       return state;
  }
}

// ── Main formatter ────────────────────────────────────────────────────────────

/**
 * Converts raw cell data into a fully-formatted CellInspectorData object.
 *
 * Pure function — no side effects, no I/O.
 */
export function inspectCell(cell: CellInspectorInput): CellInspectorData {
  const label = temperatureLabel(cell.temperature);
  return {
    coords:           `(${cell.x}, ${cell.y})`,
    element:          cell.element,
    elementState:     formatElementState(cell.elementState),
    mass:             formatMass(cell.mass),
    temperatureK:     `${cell.temperature.toFixed(2)} K`,
    temperatureC:     `${cell.temperatureC.toFixed(1)}°C`,
    temperatureLabel: label,
    temperatureColor: temperatureLabelColor(label),
    entities:         cell.entities ?? [],
  };
}
