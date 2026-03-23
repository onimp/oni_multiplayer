/**
 * renderGate.ts — pure dirty-flag logic that decides whether the canvas
 * needs to be redrawn this frame.
 *
 * A redraw is needed when:
 *  (a) dirty flag is set — viewport changed: pan, zoom, resize, options toggle
 *  (b) any data reference changed since the last render — new poll result arrived
 *
 * Object reference comparison is used for entities/world (O(1)) because
 * App.tsx creates a new object on every successful fetch, so a changed
 * reference means new data.  Primitive props use strict equality.
 *
 * Pure module — no DOM, no canvas API.  Fully testable in Node/vitest.
 */

/** Snapshot of all rendering inputs at the time of the last draw. */
export interface RenderSnapshot {
  entities:     unknown;  // EntitiesResponse — compared by reference
  world:        unknown;  // WorldData        — compared by reference
  overlay:      string;
  showEntities: boolean;
  showGrid:     boolean;
  showMinimap:  boolean;
  serverUps:    number | undefined;
}

export interface RenderGate {
  /** True when viewport changed (pan/zoom/resize); always triggers a redraw. */
  dirty:         boolean;
  /** Snapshot captured at the last successful draw. null = never drawn. */
  lastSnapshot:  RenderSnapshot | null;
}

/** Creates a fresh gate.  dirty=true so the first frame always draws. */
export function createRenderGate(): RenderGate {
  return { dirty: true, lastSnapshot: null };
}

/**
 * Returns true when the canvas should be redrawn this frame.
 *
 * Call at the start of the rAF loop body; if it returns false, skip all
 * canvas drawing work entirely (tooltip DOM updates can still run).
 */
export function needsRender(gate: RenderGate, snap: RenderSnapshot): boolean {
  if (gate.dirty || gate.lastSnapshot === null) return true;
  const s = gate.lastSnapshot;
  return (
    snap.entities      !== s.entities      ||
    snap.world         !== s.world         ||
    snap.overlay       !== s.overlay       ||
    snap.showEntities  !== s.showEntities  ||
    snap.showGrid      !== s.showGrid      ||
    snap.showMinimap   !== s.showMinimap   ||
    snap.serverUps     !== s.serverUps
  );
}

/**
 * Call immediately after a successful canvas draw to reset the gate.
 * Stores a shallow copy of the snapshot so future calls to needsRender
 * can detect changes.
 */
export function recordRender(gate: RenderGate, snap: RenderSnapshot): void {
  gate.dirty        = false;
  gate.lastSnapshot = { ...snap };
}

/**
 * Marks the gate dirty — forces a redraw on the next frame regardless of
 * whether data has changed.  Call from pan, zoom, resize, and options-toggle
 * handlers so viewport changes are reflected immediately.
 */
export function markDirty(gate: RenderGate): void {
  gate.dirty = true;
}
