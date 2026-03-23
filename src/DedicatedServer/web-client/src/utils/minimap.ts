/**
 * minimap.ts — pure coordinate utilities for the canvas minimap overlay.
 *
 * No canvas API references here — all functions take plain numbers and return
 * plain numbers/objects so they can be unit-tested in Node/vitest without DOM.
 *
 * The minimap is a fixed-size rectangle drawn in the bottom-right corner of
 * the main canvas.  It shows a bird's-eye view of the world at reduced scale
 * with a viewport rectangle indicating the currently visible region.
 */

export const MINIMAP_W      = 160;  // minimap width  in canvas pixels
export const MINIMAP_H      = 120;  // minimap height in canvas pixels
export const MINIMAP_MARGIN =   8;  // gap from canvas edge

/** Returns the canvas-space position of the minimap top-left corner. */
export function minimapOrigin(
  canvasW: number,
  canvasH: number,
): { x: number; y: number } {
  return {
    x: canvasW - MINIMAP_W - MINIMAP_MARGIN,
    y: canvasH - MINIMAP_H - MINIMAP_MARGIN,
  };
}

/**
 * Maps a world-cell X coordinate to a minimap-local X (0…mmW).
 * No Y-flip needed for X.
 */
export function worldToMinimapX(worldX: number, worldW: number, mmW: number): number {
  return (worldX / worldW) * mmW;
}

/**
 * Maps a world-cell Y coordinate to a minimap-local Y (0…mmH).
 *
 * ONI world Y=0 is the **bottom** row; minimap Y=0 is the **top** edge.
 * So we flip: mmY = (worldH - 1 - worldY) / worldH * mmH.
 */
export function worldToMinimapY(worldY: number, worldH: number, mmH: number): number {
  return ((worldH - 1 - worldY) / worldH) * mmH;
}

/**
 * Returns the viewport indicator rectangle in minimap-local coordinates.
 *
 * The main canvas renders at:
 *   screenX = offsetX + cellX * cellSize
 *   screenY = offsetY + (worldH - 1 - cellY) * cellSize
 *
 * Solving for the world-origin corner (cellX=0, cellY=worldH-1 bottom-left)
 * in screen space gives the top-left of the rendered world:
 *   screenX0 = offsetX  →  viewport left edge = -offsetX / (worldW * cellSize) * mmW
 *   screenY0 = offsetY  →  viewport top  edge = -offsetY / (worldH * cellSize) * mmH
 *
 * Width/height scale the visible canvas area into minimap pixels.
 */
export function viewportRectInMinimap(
  offsetX: number,
  offsetY: number,
  cellSize: number,
  canvasW: number,
  canvasH: number,
  worldW: number,
  worldH: number,
  mmW: number,
  mmH: number,
): { x: number; y: number; w: number; h: number } {
  const worldPixW = worldW * cellSize;
  const worldPixH = worldH * cellSize;
  return {
    x: (-offsetX / worldPixW) * mmW,
    y: (-offsetY / worldPixH) * mmH,
    w: (canvasW  / worldPixW) * mmW,
    h: (canvasH  / worldPixH) * mmH,
  };
}

/**
 * Converts a click position inside the minimap (relative to minimap origin)
 * to the world-cell coordinates at the centre of that minimap pixel.
 */
export function minimapClickToWorld(
  mmClickX: number,
  mmClickY: number,
  worldW: number,
  worldH: number,
  mmW: number,
  mmH: number,
): { worldX: number; worldY: number } {
  // Reverse worldToMinimapX/Y:
  const worldX = (mmClickX / mmW) * worldW;
  const worldY = worldH - 1 - (mmClickY / mmH) * worldH;
  return { worldX, worldY };
}

/**
 * Computes the main-canvas offsetX/offsetY needed to centre the view on a
 * given world-cell position (worldCX, worldCY).
 */
export function worldCenterToOffset(
  worldCX: number,
  worldCY: number,
  canvasW: number,
  canvasH: number,
  worldH: number,
  cellSize: number,
): { offsetX: number; offsetY: number } {
  // screenX(worldCX) should equal canvasW/2
  // offsetX + worldCX * cellSize = canvasW/2  →  offsetX = canvasW/2 - worldCX*cellSize
  // screenY for worldCY (top of cell): offsetY + (worldH - worldCY - 1)*cellSize = canvasH/2
  return {
    offsetX: canvasW / 2 - worldCX * cellSize,
    offsetY: canvasH / 2 - (worldH - worldCY - 1) * cellSize,
  };
}

/**
 * Returns true when a canvas-space point (px, py) lies inside the minimap.
 * Used by WorldCanvas mouse handlers to detect minimap clicks.
 */
export function isInsideMinimap(
  px: number,
  py: number,
  canvasW: number,
  canvasH: number,
): boolean {
  const o = minimapOrigin(canvasW, canvasH);
  return px >= o.x && px < o.x + MINIMAP_W && py >= o.y && py < o.y + MINIMAP_H;
}
