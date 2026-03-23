import { describe, it, expect } from 'vitest';
import {
  createTickBuffer,
  pushTick,
  getTickAt,
  getLatestTick,
  getAllTicks,
} from './tickBuffer';
import type { TickBuffer, TickSnapshot } from './tickBuffer';

// ── helpers ───────────────────────────────────────────────────────────────────

function makeSnap(tick: number): TickSnapshot {
  return {
    tick,
    entities:  { tick, entities: [] },
    gameState: {
      tick, cycle: 1, speed: 1, paused: false,
      worldWidth: 256, worldHeight: 128,
      duplicantCount: 0, buildingCount: 0, entityCount: 0,
    },
  };
}

/** Push N snapshots with sequential ticks starting at `startTick`. */
function pushN(buf: TickBuffer, count: number, startTick = 1): void {
  for (let i = 0; i < count; i++) pushTick(buf, makeSnap(startTick + i));
}

// ── createTickBuffer ──────────────────────────────────────────────────────────

describe('createTickBuffer', () => {
  it('creates an empty buffer', () => {
    const buf = createTickBuffer(10);
    expect(buf.size).toBe(0);
    expect(buf.capacity).toBe(10);
  });

  it('throws on capacity < 1', () => {
    expect(() => createTickBuffer(0)).toThrow();
    expect(() => createTickBuffer(-5)).toThrow();
  });

  it('capacity=1 is valid', () => {
    const buf = createTickBuffer(1);
    expect(buf.capacity).toBe(1);
    expect(buf.size).toBe(0);
  });
});

// ── pushTick — basic ──────────────────────────────────────────────────────────

describe('pushTick — size tracking', () => {
  it('size increases on each push up to capacity', () => {
    const buf = createTickBuffer(3);
    expect(buf.size).toBe(0);
    pushTick(buf, makeSnap(1)); expect(buf.size).toBe(1);
    pushTick(buf, makeSnap(2)); expect(buf.size).toBe(2);
    pushTick(buf, makeSnap(3)); expect(buf.size).toBe(3);
  });

  it('size does not exceed capacity when overfilled', () => {
    const buf = createTickBuffer(3);
    pushN(buf, 10);
    expect(buf.size).toBe(3);
  });

  it('capacity=1 always holds exactly 1 item', () => {
    const buf = createTickBuffer(1);
    pushTick(buf, makeSnap(1));
    pushTick(buf, makeSnap(2));
    expect(buf.size).toBe(1);
    expect(getTickAt(buf, 0)?.tick).toBe(2); // newest replaces oldest
  });
});

// ── getTickAt ─────────────────────────────────────────────────────────────────

describe('getTickAt — within capacity', () => {
  it('returns undefined for empty buffer', () => {
    const buf = createTickBuffer(5);
    expect(getTickAt(buf, 0)).toBeUndefined();
  });

  it('returns undefined for negative index', () => {
    const buf = createTickBuffer(5);
    pushTick(buf, makeSnap(1));
    expect(getTickAt(buf, -1)).toBeUndefined();
  });

  it('returns undefined for index >= size', () => {
    const buf = createTickBuffer(5);
    pushN(buf, 3);
    expect(getTickAt(buf, 3)).toBeUndefined();
    expect(getTickAt(buf, 10)).toBeUndefined();
  });

  it('index 0 is oldest, index size-1 is newest', () => {
    const buf = createTickBuffer(5);
    pushN(buf, 4); // ticks 1,2,3,4
    expect(getTickAt(buf, 0)?.tick).toBe(1); // oldest
    expect(getTickAt(buf, 3)?.tick).toBe(4); // newest
  });

  it('intermediate indices return correct items', () => {
    const buf = createTickBuffer(5);
    pushN(buf, 5); // ticks 1..5
    for (let i = 0; i < 5; i++) {
      expect(getTickAt(buf, i)?.tick).toBe(i + 1);
    }
  });
});

describe('getTickAt — after circular wrap', () => {
  it('oldest entry is evicted after overfill', () => {
    const buf = createTickBuffer(3);
    pushN(buf, 5); // push 1,2,3,4,5 — capacity=3, so only 3,4,5 remain
    expect(getTickAt(buf, 0)?.tick).toBe(3); // oldest surviving
    expect(getTickAt(buf, 1)?.tick).toBe(4);
    expect(getTickAt(buf, 2)?.tick).toBe(5); // newest
  });

  it('size stays at capacity after many pushes', () => {
    const buf = createTickBuffer(4);
    pushN(buf, 100);
    expect(buf.size).toBe(4);
  });

  it('ordered oldest→newest after many overwrites', () => {
    const buf = createTickBuffer(4);
    pushN(buf, 10); // ticks 1..10, only 7,8,9,10 remain
    const all = getAllTicks(buf).map(s => s.tick);
    expect(all).toEqual([7, 8, 9, 10]);
  });

  it('single overwrite: tick 4 evicts tick 1 in capacity=3 buffer', () => {
    const buf = createTickBuffer(3);
    pushN(buf, 4); // push 1,2,3,4 — 1 evicted
    expect(getTickAt(buf, 0)?.tick).toBe(2);
    expect(getTickAt(buf, 1)?.tick).toBe(3);
    expect(getTickAt(buf, 2)?.tick).toBe(4);
  });

  it('each successive push evicts the correct oldest', () => {
    const buf = createTickBuffer(3);
    pushN(buf, 3); // [1,2,3]
    for (let extra = 4; extra <= 10; extra++) {
      pushTick(buf, makeSnap(extra));
      // Oldest should be (extra - 2)
      expect(getTickAt(buf, 0)?.tick).toBe(extra - 2);
      expect(getTickAt(buf, 2)?.tick).toBe(extra); // newest
    }
  });
});

// ── getLatestTick ─────────────────────────────────────────────────────────────

describe('getLatestTick', () => {
  it('returns undefined for empty buffer', () => {
    expect(getLatestTick(createTickBuffer(5))).toBeUndefined();
  });

  it('returns the most recently pushed snapshot', () => {
    const buf = createTickBuffer(10);
    pushN(buf, 5);
    expect(getLatestTick(buf)?.tick).toBe(5);
  });

  it('returns the newest after circular wrap', () => {
    const buf = createTickBuffer(3);
    pushN(buf, 7);
    expect(getLatestTick(buf)?.tick).toBe(7);
  });
});

// ── getAllTicks ───────────────────────────────────────────────────────────────

describe('getAllTicks', () => {
  it('returns empty array for empty buffer', () => {
    expect(getAllTicks(createTickBuffer(5))).toEqual([]);
  });

  it('returns items in oldest→newest order before wrap', () => {
    const buf = createTickBuffer(5);
    pushN(buf, 3);
    const ticks = getAllTicks(buf).map(s => s.tick);
    expect(ticks).toEqual([1, 2, 3]);
  });

  it('returns exactly capacity items when buffer is full', () => {
    const buf = createTickBuffer(4);
    pushN(buf, 4);
    expect(getAllTicks(buf)).toHaveLength(4);
  });

  it('returns items in oldest→newest order after wrap', () => {
    const buf = createTickBuffer(4);
    pushN(buf, 7); // only 4,5,6,7 remain
    const ticks = getAllTicks(buf).map(s => s.tick);
    expect(ticks).toEqual([4, 5, 6, 7]);
  });

  it('result does not share storage with the buffer (new array)', () => {
    const buf = createTickBuffer(3);
    pushN(buf, 3);
    const arr1 = getAllTicks(buf);
    pushTick(buf, makeSnap(99)); // mutate buffer
    const arr2 = getAllTicks(buf);
    expect(arr1.map(s => s.tick)).toEqual([1, 2, 3]); // arr1 unchanged
    expect(arr2.map(s => s.tick)).toEqual([2, 3, 99]); // arr2 reflects new state
  });
});

// ── snapshot identity (reference preservation) ───────────────────────────────

describe('snapshot identity', () => {
  it('stores the exact snapshot reference (no clone)', () => {
    const buf = createTickBuffer(5);
    const snap = makeSnap(42);
    pushTick(buf, snap);
    expect(getTickAt(buf, 0)).toBe(snap); // same reference
  });

  it('getLatestTick returns same reference as last push', () => {
    const buf = createTickBuffer(5);
    const snap = makeSnap(7);
    pushN(buf, 3);
    pushTick(buf, snap);
    expect(getLatestTick(buf)).toBe(snap);
  });
});

// ── integration: simulating a poll loop ──────────────────────────────────────

describe('integration — poll loop simulation', () => {
  it('buffer always contains the last capacity ticks after a long run', () => {
    const buf = createTickBuffer(100);
    const TOTAL = 500;
    pushN(buf, TOTAL);
    expect(buf.size).toBe(100);
    const ticks = getAllTicks(buf).map(s => s.tick);
    // Should contain 401..500
    expect(ticks[0]).toBe(TOTAL - 99);
    expect(ticks[99]).toBe(TOTAL);
  });

  it('getTickAt is consistent with getAllTicks for any buffer state', () => {
    const buf = createTickBuffer(7);
    pushN(buf, 20); // well past wrap
    const all = getAllTicks(buf);
    for (let i = 0; i < buf.size; i++) {
      expect(getTickAt(buf, i)).toBe(all[i]);
    }
  });
});
