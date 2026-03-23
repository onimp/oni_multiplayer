/**
 * tickBuffer.ts — fixed-capacity circular buffer storing the last N poll
 * snapshots for the timeline scrubber.
 *
 * Each entry captures entities + gameState at one game tick.  When the buffer
 * is full the oldest entry is silently overwritten (ring buffer / FIFO eviction).
 *
 * Pure module — no DOM, no React.  Fully testable in Node/vitest.
 *
 * Index convention for getTickAt():
 *   index 0  = oldest stored entry
 *   index size-1 = newest stored entry (most recent push)
 */

import type { EntitiesResponse, GameState } from '../api/types';

/** One captured moment in time. */
export interface TickSnapshot {
  /** Game tick number at the time of capture. */
  tick:       number;
  entities:   EntitiesResponse;
  gameState:  GameState;
}

/** Internal circular-buffer state.  Treat as opaque outside this module. */
export interface TickBuffer {
  readonly capacity: number;
  /** Raw storage — slots may be undefined before first wrap. */
  readonly _items:   (TickSnapshot | undefined)[];
  /** Next write position (0..capacity-1). */
  _head:  number;
  /** Number of items currently stored (0..capacity). */
  size:   number;
}

// ── Factory ───────────────────────────────────────────────────────────────────

/** Creates an empty buffer that will hold at most `capacity` snapshots. */
export function createTickBuffer(capacity: number): TickBuffer {
  if (capacity < 1) throw new RangeError(`capacity must be >= 1, got ${capacity}`);
  return {
    capacity,
    _items: new Array<TickSnapshot | undefined>(capacity).fill(undefined),
    _head:  0,
    size:   0,
  };
}

// ── Write ─────────────────────────────────────────────────────────────────────

/**
 * Appends a snapshot to the buffer.
 * When full, the oldest entry is silently evicted (ring overwrite).
 */
export function pushTick(buf: TickBuffer, snap: TickSnapshot): void {
  buf._items[buf._head] = snap;
  buf._head = (buf._head + 1) % buf.capacity;
  if (buf.size < buf.capacity) buf.size++;
}

// ── Read ──────────────────────────────────────────────────────────────────────

/**
 * Returns the snapshot at logical `index` (0 = oldest, size-1 = newest),
 * or undefined when the index is out of range.
 */
export function getTickAt(buf: TickBuffer, index: number): TickSnapshot | undefined {
  if (index < 0 || index >= buf.size) return undefined;
  // Oldest entry is at slot: (head - size + capacity*2) % capacity
  // Adding capacity*2 avoids negative modulo.
  const slot = (buf._head - buf.size + index + buf.capacity * 2) % buf.capacity;
  return buf._items[slot];
}

/** Returns the most-recently pushed snapshot, or undefined if empty. */
export function getLatestTick(buf: TickBuffer): TickSnapshot | undefined {
  return getTickAt(buf, buf.size - 1);
}

/**
 * Returns all stored snapshots as a new array, ordered oldest → newest.
 * O(n) allocation; call only when the consumer needs the full list
 * (e.g. re-rendering the scrubber after a new tick was buffered).
 */
export function getAllTicks(buf: TickBuffer): TickSnapshot[] {
  const out: TickSnapshot[] = [];
  for (let i = 0; i < buf.size; i++) {
    const s = getTickAt(buf, i);
    if (s !== undefined) out.push(s);
  }
  return out;
}
