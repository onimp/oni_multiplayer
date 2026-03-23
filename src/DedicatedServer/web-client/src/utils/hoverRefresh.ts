/**
 * Hover-tooltip refresh state + predicate.
 *
 * The bug: the hover tooltip was ONLY updated inside handleMouseMove, so it
 * became stale the moment entity data refreshed from the server while the
 * cursor stayed still.
 *
 * Fix: the rAF loop calls shouldRefreshHover() every frame and, when it
 * returns true, re-runs the hit-test + innerHTML update using the stored
 * mouse position — no mouse event required.
 */
export interface HoverState {
  /** True when the tooltip is pinned (clicked). Pinned tooltip has its own rAF refresh path. */
  isPinned: boolean;
  /** True while the cursor is inside the canvas bounds. */
  isHovering: boolean;
}

/**
 * Returns true when the hover tooltip should be refreshed on the current
 * animation frame — i.e. cursor is inside the canvas AND the tooltip is not
 * in pinned mode (pinned has its own refresh path).
 *
 * Extracted as a pure function so it can be unit-tested independently of the
 * React component and without any mouse events being fired.
 */
export function shouldRefreshHover(state: HoverState): boolean {
  return state.isHovering && !state.isPinned;
}
