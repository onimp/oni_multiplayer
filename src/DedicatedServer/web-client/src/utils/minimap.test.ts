import { describe, it, expect } from 'vitest';
import {
  MINIMAP_W, MINIMAP_H, MINIMAP_MARGIN,
  minimapOrigin,
  worldToMinimapX, worldToMinimapY,
  viewportRectInMinimap,
  minimapClickToWorld,
  worldCenterToOffset,
  isInsideMinimap,
} from './minimap';

// ── minimapOrigin ──────────────────────────────────────────────────────────────

describe('minimapOrigin', () => {
  it('places minimap bottom-right with configured margin', () => {
    const o = minimapOrigin(800, 600);
    expect(o.x).toBe(800 - MINIMAP_W - MINIMAP_MARGIN);
    expect(o.y).toBe(600 - MINIMAP_H - MINIMAP_MARGIN);
  });
});

// ── worldToMinimapX ────────────────────────────────────────────────────────────

describe('worldToMinimapX', () => {
  it('maps world x=0 to minimap x=0', () => {
    expect(worldToMinimapX(0, 256, 160)).toBe(0);
  });

  it('maps world x=worldW to minimap x=mmW (right edge)', () => {
    expect(worldToMinimapX(256, 256, 160)).toBe(160);
  });

  it('maps midpoint correctly', () => {
    expect(worldToMinimapX(128, 256, 160)).toBe(80);
  });

  it('scales proportionally for non-square minimap', () => {
    // worldW=100, mmW=50 → scale=0.5
    expect(worldToMinimapX(40, 100, 50)).toBeCloseTo(20);
  });
});

// ── worldToMinimapY ────────────────────────────────────────────────────────────

describe('worldToMinimapY', () => {
  it('maps the top world row (worldY = worldH-1) to minimap y ≈ 0', () => {
    // worldY = worldH - 1 → mmY = (worldH - 1 - (worldH-1)) / worldH * mmH = 0
    expect(worldToMinimapY(127, 128, 120)).toBe(0);
  });

  it('maps world y=0 (bottom row) to minimap bottom', () => {
    // worldY=0 → mmY = (worldH-1)/worldH * mmH ≈ mmH (just under)
    const v = worldToMinimapY(0, 128, 120);
    expect(v).toBeCloseTo(120 * (127 / 128));
  });

  it('maps middle row to roughly half', () => {
    const worldH = 128;
    const mmH    = 120;
    const mid    = Math.floor(worldH / 2);           // 64
    const v      = worldToMinimapY(mid, worldH, mmH);
    // (worldH - 1 - 64) / 128 * 120 = 63/128*120 ≈ 59.06
    expect(v).toBeCloseTo((worldH - 1 - mid) / worldH * mmH);
  });
});

// ── viewportRectInMinimap ─────────────────────────────────────────────────────

describe('viewportRectInMinimap', () => {
  // World is 256×128 cells, rendered at cellSize=10 → world pixel size 2560×1280.
  // Canvas is 800×600.

  const worldW = 256, worldH = 128, cellSize = 10;
  const canvasW = 800, canvasH = 600;
  const mmW = 160, mmH = 120;

  it('viewport x=0,y=0 when world origin is at canvas origin', () => {
    const r = viewportRectInMinimap(0, 0, cellSize, canvasW, canvasH, worldW, worldH, mmW, mmH);
    expect(r.x).toBeCloseTo(0);
    expect(r.y).toBeCloseTo(0);
  });

  it('viewport width/height proportional to canvas vs world size', () => {
    const r = viewportRectInMinimap(0, 0, cellSize, canvasW, canvasH, worldW, worldH, mmW, mmH);
    // canvasW / (worldW * cellSize) * mmW = 800/2560*160 = 50
    expect(r.w).toBeCloseTo(50);
    // canvasH / (worldH * cellSize) * mmH = 600/1280*120 = 56.25
    expect(r.h).toBeCloseTo(56.25);
  });

  it('negative offset shifts viewport rect right (world has moved right)', () => {
    // offset = -(worldPixW/2) means we're looking at the right half
    const r = viewportRectInMinimap(-1280, 0, cellSize, canvasW, canvasH, worldW, worldH, mmW, mmH);
    // x = -(-1280)/2560 * 160 = 80
    expect(r.x).toBeCloseTo(80);
  });

  it('viewport position is zero when world is centred on canvas', () => {
    // When centred: offsetX = (canvasW - worldPixW)/2 = (800 - 2560)/2 = -880
    const offsetX = (canvasW - worldW * cellSize) / 2;
    const offsetY = (canvasH - worldH * cellSize) / 2;
    const r = viewportRectInMinimap(offsetX, offsetY, cellSize, canvasW, canvasH, worldW, worldH, mmW, mmH);
    // x = -offsetX / worldPixW * mmW = 880/2560*160 = 55
    expect(r.x).toBeCloseTo((-offsetX / (worldW * cellSize)) * mmW);
    expect(r.y).toBeCloseTo((-offsetY / (worldH * cellSize)) * mmH);
  });
});

// ── minimapClickToWorld ───────────────────────────────────────────────────────

describe('minimapClickToWorld', () => {
  const worldW = 256, worldH = 128, mmW = 160, mmH = 120;

  it('minimap top-left (0,0) maps to world top-left cell', () => {
    const { worldX, worldY } = minimapClickToWorld(0, 0, worldW, worldH, mmW, mmH);
    expect(worldX).toBeCloseTo(0);
    expect(worldY).toBeCloseTo(worldH - 1);  // top row in ONI coords
  });

  it('minimap bottom-right maps to world bottom-right cell', () => {
    const { worldX, worldY } = minimapClickToWorld(mmW, mmH, worldW, worldH, mmW, mmH);
    expect(worldX).toBeCloseTo(worldW);
    expect(worldY).toBeCloseTo(-1);           // just past bottom row
  });

  it('minimap centre maps to world centre', () => {
    const { worldX, worldY } = minimapClickToWorld(mmW / 2, mmH / 2, worldW, worldH, mmW, mmH);
    expect(worldX).toBeCloseTo(worldW / 2);
    expect(worldY).toBeCloseTo(worldH / 2 - 1);
  });
});

// ── worldCenterToOffset ───────────────────────────────────────────────────────

describe('worldCenterToOffset', () => {
  const canvasW = 800, canvasH = 600, worldH = 128, cellSize = 10;

  it('centring on (0, 0) bottom-left cell', () => {
    const { offsetX, offsetY } = worldCenterToOffset(0, 0, canvasW, canvasH, worldH, cellSize);
    // offsetX = 800/2 - 0*10 = 400
    expect(offsetX).toBe(400);
    // offsetY = 600/2 - (128 - 0 - 1)*10 = 300 - 1270 = -970
    expect(offsetY).toBe(300 - (worldH - 0 - 1) * cellSize);
  });

  it('centring on world centre cell', () => {
    const cx = 128, cy = 64;
    const { offsetX, offsetY } = worldCenterToOffset(cx, cy, canvasW, canvasH, worldH, cellSize);
    expect(offsetX).toBe(canvasW / 2 - cx * cellSize);
    expect(offsetY).toBe(canvasH / 2 - (worldH - cy - 1) * cellSize);
  });

  it('placing cell in canvas centre verifies: screenX = offsetX + cx*cellSize = canvasW/2', () => {
    const cx = 50, cy = 30;
    const { offsetX, offsetY } = worldCenterToOffset(cx, cy, canvasW, canvasH, worldH, cellSize);
    const screenX = offsetX + cx * cellSize;
    const screenY = offsetY + (worldH - cy - 1) * cellSize;
    expect(screenX).toBeCloseTo(canvasW / 2);
    expect(screenY).toBeCloseTo(canvasH / 2);
  });
});

// ── isInsideMinimap ───────────────────────────────────────────────────────────

describe('isInsideMinimap', () => {
  const canvasW = 800, canvasH = 600;
  // minimap origin: x = 800 - 160 - 8 = 632, y = 600 - 120 - 8 = 472

  it('point inside minimap returns true', () => {
    expect(isInsideMinimap(640, 480, canvasW, canvasH)).toBe(true);
  });

  it('point at minimap top-left corner returns true', () => {
    const { x, y } = minimapOrigin(canvasW, canvasH);
    expect(isInsideMinimap(x, y, canvasW, canvasH)).toBe(true);
  });

  it('point just outside left edge returns false', () => {
    const { x, y } = minimapOrigin(canvasW, canvasH);
    expect(isInsideMinimap(x - 1, y + 10, canvasW, canvasH)).toBe(false);
  });

  it('point just outside right edge returns false', () => {
    const { x, y } = minimapOrigin(canvasW, canvasH);
    expect(isInsideMinimap(x + MINIMAP_W, y + 10, canvasW, canvasH)).toBe(false);
  });

  it('point outside canvas area (top-left) returns false', () => {
    expect(isInsideMinimap(0, 0, canvasW, canvasH)).toBe(false);
  });
});
