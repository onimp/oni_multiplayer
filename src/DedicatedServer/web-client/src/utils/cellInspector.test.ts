import { describe, it, expect } from 'vitest';
import {
  temperatureLabel,
  temperatureLabelColor,
  formatMass,
  formatElementState,
  inspectCell,
} from './cellInspector';
import type { CellInspectorInput } from './cellInspector';

// ── temperatureLabel ──────────────────────────────────────────────────────────

describe('temperatureLabel', () => {
  it('labels absolute cold below 173.15 K', () => {
    expect(temperatureLabel(0)).toBe('Absolute Cold');
    expect(temperatureLabel(172)).toBe('Absolute Cold');
    expect(temperatureLabel(173.14)).toBe('Absolute Cold');
  });

  it('labels frozen 173–253 K', () => {
    expect(temperatureLabel(173.15)).toBe('Frozen');
    expect(temperatureLabel(200)).toBe('Frozen');
    expect(temperatureLabel(253.14)).toBe('Frozen');
  });

  it('labels cold 253–283 K', () => {
    expect(temperatureLabel(253.15)).toBe('Cold');
    expect(temperatureLabel(270)).toBe('Cold');
    expect(temperatureLabel(282.99)).toBe('Cold');
  });

  it('labels room temp 283–303 K', () => {
    expect(temperatureLabel(283.15)).toBe('Room Temp');
    expect(temperatureLabel(295)).toBe('Room Temp');
    expect(temperatureLabel(302.99)).toBe('Room Temp');
  });

  it('labels warm 303–333 K', () => {
    expect(temperatureLabel(303.15)).toBe('Warm');
    expect(temperatureLabel(320)).toBe('Warm');
    expect(temperatureLabel(332.99)).toBe('Warm');
  });

  it('labels hot 333–423 K', () => {
    expect(temperatureLabel(333.15)).toBe('Hot');
    expect(temperatureLabel(380)).toBe('Hot');
    expect(temperatureLabel(422.99)).toBe('Hot');
  });

  it('labels scalding at 423 K and above', () => {
    expect(temperatureLabel(423.15)).toBe('Scalding');
    expect(temperatureLabel(1000)).toBe('Scalding');
    expect(temperatureLabel(5000)).toBe('Scalding');
  });

  it('boundary: exactly 303.15 K is Room Temp', () => {
    // 303.15 K = 30°C
    expect(temperatureLabel(303.15)).toBe('Warm');   // >= 303.15
    expect(temperatureLabel(303.14)).toBe('Room Temp');
  });
});

// ── temperatureLabelColor ─────────────────────────────────────────────────────

describe('temperatureLabelColor', () => {
  it('returns a hex color for each known label', () => {
    const labels = ['Absolute Cold', 'Frozen', 'Cold', 'Room Temp', 'Warm', 'Hot', 'Scalding'];
    for (const label of labels) {
      const color = temperatureLabelColor(label);
      expect(color).toMatch(/^#[0-9a-f]{6}$/i);
    }
  });

  it('returns fallback color for unknown label', () => {
    expect(temperatureLabelColor('Unknown')).toBe('#aaaaaa');
  });

  it('cool labels are bluer, hot labels are redder', () => {
    const frozen   = temperatureLabelColor('Frozen');
    const scalding = temperatureLabelColor('Scalding');
    // Frozen should have higher blue component, scalding higher red
    const frozenR   = parseInt(frozen.slice(1, 3), 16);
    const scaldingR = parseInt(scalding.slice(1, 3), 16);
    expect(scaldingR).toBeGreaterThan(frozenR);
  });
});

// ── formatMass ────────────────────────────────────────────────────────────────

describe('formatMass', () => {
  it('formats zero mass', () => {
    expect(formatMass(0)).toBe('0 kg');
    expect(formatMass(-1)).toBe('0 kg');
  });

  it('formats sub-milligram mass in grams', () => {
    expect(formatMass(0.0001)).toBe('0.100 g');
    expect(formatMass(0.0005)).toBe('0.500 g');
  });

  it('formats small kg (< 1 kg) with 3 decimals', () => {
    expect(formatMass(0.5)).toBe('0.500 kg');
    expect(formatMass(0.001)).toBe('0.001 kg');
  });

  it('formats medium kg (1–100) with 2 decimals', () => {
    expect(formatMass(1)).toBe('1.00 kg');
    expect(formatMass(50.5)).toBe('50.50 kg');
    expect(formatMass(99.99)).toBe('99.99 kg');
  });

  it('formats large kg (≥ 100) with 1 decimal', () => {
    expect(formatMass(100)).toBe('100.0 kg');
    expect(formatMass(1000)).toBe('1000.0 kg');
    expect(formatMass(999.9)).toBe('999.9 kg');
  });
});

// ── formatElementState ────────────────────────────────────────────────────────

describe('formatElementState', () => {
  it('formats known states with emoji prefix', () => {
    expect(formatElementState('Gas')).toBe('💨 Gas');
    expect(formatElementState('Liquid')).toBe('💧 Liquid');
    expect(formatElementState('Solid')).toBe('🪨 Solid');
    expect(formatElementState('Vacuum')).toBe('✨ Vacuum');
  });

  it('returns unknown states as-is', () => {
    expect(formatElementState('Plasma')).toBe('Plasma');
    expect(formatElementState('')).toBe('');
  });
});

// ── inspectCell ───────────────────────────────────────────────────────────────

describe('inspectCell', () => {
  const base: CellInspectorInput = {
    x: 42,
    y: 17,
    element: 'Water',
    elementState: 'Liquid',
    temperature: 300.15,
    temperatureC: 27.0,
    mass: 800,
    entities: [],
  };

  it('formats coordinates as (x, y)', () => {
    expect(inspectCell(base).coords).toBe('(42, 17)');
  });

  it('formats element state with emoji', () => {
    expect(inspectCell(base).elementState).toBe('💧 Liquid');
  });

  it('formats mass correctly', () => {
    expect(inspectCell(base).mass).toBe('800.0 kg');
  });

  it('formats temperature in both K and C', () => {
    const r = inspectCell(base);
    expect(r.temperatureK).toBe('300.15 K');
    expect(r.temperatureC).toBe('27.0°C');
  });

  it('assigns correct temperature label and color', () => {
    const r = inspectCell(base);
    expect(r.temperatureLabel).toBe('Room Temp');
    expect(r.temperatureColor).toMatch(/^#[0-9a-f]{6}$/i);
  });

  it('returns empty entities array when none present', () => {
    expect(inspectCell({ ...base, entities: undefined }).entities).toEqual([]);
  });

  it('passes through entity strings', () => {
    const r = inspectCell({ ...base, entities: ['Bob (duplicant)', 'Squirrel (critter)'] });
    expect(r.entities).toEqual(['Bob (duplicant)', 'Squirrel (critter)']);
  });

  it('handles vacuum cell (zero mass, Vacuum state)', () => {
    const vac: CellInspectorInput = { ...base, element: 'Vacuum', elementState: 'Vacuum', mass: 0 };
    const r = inspectCell(vac);
    expect(r.mass).toBe('0 kg');
    expect(r.elementState).toBe('✨ Vacuum');
  });

  it('handles very hot cell (Scalding)', () => {
    const hot: CellInspectorInput = { ...base, temperature: 2000, temperatureC: 1726.85 };
    const r = inspectCell(hot);
    expect(r.temperatureLabel).toBe('Scalding');
  });

  it('handles very cold cell (Absolute Cold)', () => {
    const cold: CellInspectorInput = { ...base, temperature: 100, temperatureC: -173.15 };
    const r = inspectCell(cold);
    expect(r.temperatureLabel).toBe('Absolute Cold');
  });
});
