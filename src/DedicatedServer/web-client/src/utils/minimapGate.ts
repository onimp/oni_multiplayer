/**
 * minimapGate.ts — pure dirty-check for the minimap overlay.
 *
 * The minimap needs to redraw when:
 *  (a) the entity array reference changes (new poll arrived)
 *  (b) the viewport parameters change (pan, zoom, resize)
 *
 * When neither changes, the cached offscreen minimap canvas can be blitted
 * to the main canvas without re-iterating entities or recomputing geometry.
 *
 * Pure module — no DOM, no canvas API.  Fully testable in Node/vitest.
 * WorldRenderer holds the mutable cache and calls minimapNeedsRedraw() at
 * the start of renderMinimap().
 */

/**
 * All parameters that affect the viewport rectangle drawn on the minimap.
 * Captured once per renderMinimap() call and compared by value.
 */
export interface MinimapViewport {
  offsetX:  number;
  offsetY:  number;
  cellSize: number;
  canvasW:  number;
  canvasH:  number;
  worldW:   number;
  worldH:   number;
}

/**
 * Returns true when the minimap offscreen canvas needs to be redrawn.
 *
 * @param prevEntities - entity reference from the last draw (null = never drawn)
 * @param nextEntities - entity reference for the current frame
 * @param prevViewport - viewport snapshot from the last draw (null = never drawn)
 * @param nextViewport - viewport snapshot for the current frame
 *
 * Matching rules:
 *  - Returns true on first draw (prevViewport === null)
 *  - Returns true when entity reference differs (new poll data)
 *  - Returns true when any viewport field differs (pan / zoom / resize)
 *  - Returns false when both are identical to last draw
 */
export function minimapNeedsRedraw(
  prevEntities: unknown,
  nextEntities: unknown,
  prevViewport: MinimapViewport | null,
  nextViewport: MinimapViewport,
): boolean {
  if (prevViewport === null) return true;       // first draw
  if (nextEntities !== prevEntities) return true; // new poll data
  return (
    nextViewport.offsetX  !== prevViewport.offsetX  ||
    nextViewport.offsetY  !== prevViewport.offsetY  ||
    nextViewport.cellSize !== prevViewport.cellSize ||
    nextViewport.canvasW  !== prevViewport.canvasW  ||
    nextViewport.canvasH  !== prevViewport.canvasH  ||
    nextViewport.worldW   !== prevViewport.worldW   ||
    nextViewport.worldH   !== prevViewport.worldH
  );
}
