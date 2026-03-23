import { describe, it, expect } from 'vitest';
import { shouldRefreshHover } from './hoverRefresh';

/**
 * Tests for the hover-tooltip auto-refresh predicate.
 *
 * Bug being covered: before the fix, the hover tooltip was only updated inside
 * handleMouseMove, so it became stale when server data changed while the
 * cursor was stationary.  The fix: the rAF loop calls shouldRefreshHover()
 * every frame and re-runs the hit-test + innerHTML update using a stored
 * mouse position — no mouse event required.
 *
 * These tests prove the refresh logic fires (or doesn't fire) purely based on
 * hover state, with zero mouse events involved.
 */
describe('shouldRefreshHover', () => {
  it('returns true when hovering and tooltip is not pinned', () => {
    expect(shouldRefreshHover({ isHovering: true, isPinned: false })).toBe(true);
  });

  it('returns false when not hovering (cursor left canvas)', () => {
    expect(shouldRefreshHover({ isHovering: false, isPinned: false })).toBe(false);
  });

  it('returns false when tooltip is pinned — pinned has its own refresh path', () => {
    expect(shouldRefreshHover({ isHovering: true, isPinned: true })).toBe(false);
  });

  it('returns false when both pinned and not hovering', () => {
    expect(shouldRefreshHover({ isHovering: false, isPinned: true })).toBe(false);
  });

  it('refresh fires independently of any mouse event — calling it multiple times with same state is idempotent', () => {
    const state = { isHovering: true, isPinned: false };
    // Simulate multiple rAF ticks with no mouse movement
    expect(shouldRefreshHover(state)).toBe(true);
    expect(shouldRefreshHover(state)).toBe(true);
    expect(shouldRefreshHover(state)).toBe(true);
  });

  it('transitions correctly: hovering → pinned → hovering', () => {
    expect(shouldRefreshHover({ isHovering: true,  isPinned: false })).toBe(true);
    expect(shouldRefreshHover({ isHovering: true,  isPinned: true  })).toBe(false); // pinned
    expect(shouldRefreshHover({ isHovering: true,  isPinned: false })).toBe(true);  // unpinned
  });

  it('transitions correctly: hovering → mouse leaves → returns', () => {
    expect(shouldRefreshHover({ isHovering: true,  isPinned: false })).toBe(true);
    expect(shouldRefreshHover({ isHovering: false, isPinned: false })).toBe(false); // left canvas
    expect(shouldRefreshHover({ isHovering: true,  isPinned: false })).toBe(true);  // re-entered
  });
});
