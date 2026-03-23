import { describe, it, expect } from 'vitest';
import {
  colorForBuilding,
  hashBuildingColor,
  buildingToRect,
  BUILDING_LABEL_MIN_CELL_SIZE,
} from './buildingRenderer';

// ── colorForBuilding ──────────────────────────────────────────────────────────

describe('colorForBuilding', () => {
  it('returns the specific color for Headquarters', () => {
    expect(colorForBuilding('Headquarters')).toBe('#e94560');
  });

  it('returns the specific color for ResearchCenter', () => {
    expect(colorForBuilding('ResearchCenter')).toBe('#4ca3dd');
  });

  it('returns the specific color for Outhouse', () => {
    expect(colorForBuilding('Outhouse')).toBe('#8B4513');
  });

  it('returns a non-empty string for an unknown building', () => {
    const color = colorForBuilding('SomeUnknownBuilding_XYZ');
    expect(typeof color).toBe('string');
    expect(color.length).toBeGreaterThan(0);
  });

  it('unknown building uses hashBuildingColor (consistent with direct call)', () => {
    const name = 'FutureTeleporter';
    expect(colorForBuilding(name)).toBe(hashBuildingColor(name));
  });
});

// ── hashBuildingColor ─────────────────────────────────────────────────────────

describe('hashBuildingColor', () => {
  it('returns an hsl() string', () => {
    expect(hashBuildingColor('SomeBuilding')).toMatch(/^hsl\(\d+,\d+%,\d+%\)$/);
  });

  it('is deterministic — same name always returns same color', () => {
    const name = 'GasFilter';
    expect(hashBuildingColor(name)).toBe(hashBuildingColor(name));
  });

  it('produces different colors for different names', () => {
    // Not guaranteed for all pairs, but extremely unlikely to collide
    expect(hashBuildingColor('BuildingA')).not.toBe(hashBuildingColor('BuildingZ'));
  });

  it('handles empty string without throwing', () => {
    expect(() => hashBuildingColor('')).not.toThrow();
  });

  it('handles a single character', () => {
    expect(hashBuildingColor('X')).toMatch(/^hsl\(\d+,\d+%,\d+%\)$/);
  });
});

// ── buildingToRect — coordinate mapping ──────────────────────────────────────

describe('buildingToRect — coordinate mapping', () => {
  const worldHeight = 384;
  const offsetX = 0;
  const offsetY = 0;
  const cellSize = 10;

  it('maps x=0, y=0 (world bottom-left) to canvas top of last row', () => {
    const entity = { name: 'Test', x: 0, y: 0, w: 1, h: 1 };
    const rect = buildingToRect(entity, worldHeight, offsetX, offsetY, cellSize);
    // Y-flip: cell y=0 → canvas row at (worldHeight - 0 - 1) = 383
    expect(rect.sx).toBe(0);
    expect(rect.sy).toBe((worldHeight - 1) * cellSize);
  });

  it('maps x=10, y=20, w=2, h=3 with cellSize=10, no offset', () => {
    const entity = { name: 'Test', x: 10, y: 20, w: 2, h: 3 };
    const rect = buildingToRect(entity, worldHeight, 0, 0, cellSize);
    expect(rect.sx).toBe(10 * cellSize);
    // sy = (worldHeight - y - h) * cs = (384 - 20 - 3) * 10 = 3610
    expect(rect.sy).toBe((worldHeight - 20 - 3) * cellSize);
  });

  it('applies offsetX and offsetY to the canvas position', () => {
    const entity = { name: 'Test', x: 5, y: 5, w: 1, h: 1 };
    const base = buildingToRect(entity, worldHeight, 0, 0, cellSize);
    const shifted = buildingToRect(entity, worldHeight, 100, 200, cellSize);
    expect(shifted.sx).toBe(base.sx + 100);
    expect(shifted.sy).toBe(base.sy + 200);
  });

  it('scales pixel size by cellSize', () => {
    const entity = { name: 'Test', x: 0, y: 0, w: 3, h: 2 };
    const rect = buildingToRect(entity, worldHeight, 0, 0, 5);
    expect(rect.pw).toBe(3 * 5); // 15
    expect(rect.ph).toBe(2 * 5); // 10
  });

  it('pw = entity.w * cellSize', () => {
    const entity = { name: 'Test', x: 0, y: 0, w: 4, h: 4 };
    const rect = buildingToRect(entity, worldHeight, 0, 0, 12);
    expect(rect.pw).toBe(4 * 12);
    expect(rect.ph).toBe(4 * 12);
  });

  it('a 1×1 building at world center maps to the correct canvas cell', () => {
    const cx = 128;
    const cy = 192;
    const entity = { name: 'Test', x: cx, y: cy, w: 1, h: 1 };
    const rect = buildingToRect(entity, worldHeight, 0, 0, cellSize);
    expect(rect.sx).toBe(cx * cellSize);
    expect(rect.sy).toBe((worldHeight - cy - 1) * cellSize);
  });

  it('Headquarters (1×1) placed at top of world has sy ≈ 0', () => {
    // Top of world in ONI coords: y = worldHeight - 1
    const entity = { name: 'Headquarters', x: 0, y: worldHeight - 1, w: 1, h: 1 };
    const rect = buildingToRect(entity, worldHeight, 0, 0, cellSize);
    expect(rect.sy).toBe(0);
  });
});

// ── buildingToRect — color assignment ────────────────────────────────────────

describe('buildingToRect — color assignment', () => {
  it('uses the known color for Headquarters', () => {
    const entity = { name: 'Headquarters', x: 0, y: 0, w: 1, h: 1 };
    const rect = buildingToRect(entity, 100, 0, 0, 10);
    expect(rect.color).toBe('#e94560');
  });

  it('uses a deterministic hash color for unknown buildings', () => {
    const name = 'SomeNewBuilding';
    const rect = buildingToRect({ name, x: 0, y: 0, w: 1, h: 1 }, 100, 0, 0, 10);
    expect(rect.color).toBe(hashBuildingColor(name));
  });
});

// ── buildingToRect — label threshold ─────────────────────────────────────────

describe('buildingToRect — label rendering threshold', () => {
  const entity = { name: 'ResearchCenter', x: 0, y: 0, w: 2, h: 2 };
  const worldH = 100;

  it('label is null when cellSize is below BUILDING_LABEL_MIN_CELL_SIZE', () => {
    const rect = buildingToRect(entity, worldH, 0, 0, BUILDING_LABEL_MIN_CELL_SIZE - 1);
    expect(rect.label).toBeNull();
  });

  it('label equals building name when cellSize equals BUILDING_LABEL_MIN_CELL_SIZE', () => {
    const rect = buildingToRect(entity, worldH, 0, 0, BUILDING_LABEL_MIN_CELL_SIZE);
    expect(rect.label).toBe('ResearchCenter');
  });

  it('label equals building name when cellSize exceeds threshold', () => {
    const rect = buildingToRect(entity, worldH, 0, 0, BUILDING_LABEL_MIN_CELL_SIZE + 10);
    expect(rect.label).toBe('ResearchCenter');
  });

  it('BUILDING_LABEL_MIN_CELL_SIZE is a positive integer', () => {
    expect(Number.isInteger(BUILDING_LABEL_MIN_CELL_SIZE)).toBe(true);
    expect(BUILDING_LABEL_MIN_CELL_SIZE).toBeGreaterThan(0);
  });
});
