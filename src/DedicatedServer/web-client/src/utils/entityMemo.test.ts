import { describe, it, expect, vi } from 'vitest';
import { createEntityMemoCache, getOrCompute } from './entityMemo';

// ── helpers ───────────────────────────────────────────────────────────────────

/** Identity compute — returns entities cast to mutable for test simplicity. */
const identity = (entities: readonly number[]) => [...entities];

// ── createEntityMemoCache ─────────────────────────────────────────────────────

describe('createEntityMemoCache', () => {
  it('starts with all nulls and empty result', () => {
    const c = createEntityMemoCache<number>();
    expect(c.lastEntities).toBeNull();
    expect(c.lastFilter).toBeNull();
    expect(c.lastSort).toBeNull();
    expect(c.lastResult).toEqual([]);
  });
});

// ── getOrCompute — cold cache ─────────────────────────────────────────────────

describe('getOrCompute — cold cache', () => {
  it('calls compute on first invocation', () => {
    const compute = vi.fn((e: readonly number[]) => [...e]);
    const cache = createEntityMemoCache<number>();
    const entities = [1, 2, 3];
    getOrCompute(cache, entities, 'all', 'name', compute);
    expect(compute).toHaveBeenCalledTimes(1);
  });

  it('returns the result of compute on first call', () => {
    const cache = createEntityMemoCache<number>();
    const entities = [3, 1, 2];
    const result = getOrCompute(cache, entities, 'all', 'name', identity);
    expect(result).toEqual([3, 1, 2]);
  });

  it('stores inputs and result in cache after first call', () => {
    const cache = createEntityMemoCache<number>();
    const entities = [1, 2];
    getOrCompute(cache, entities, 'duplicant', 'type', identity);
    expect(cache.lastEntities).toBe(entities);
    expect(cache.lastFilter).toBe('duplicant');
    expect(cache.lastSort).toBe('type');
  });
});

// ── getOrCompute — cache hit (stable reference) ───────────────────────────────

describe('getOrCompute — cache hit', () => {
  it('does NOT call compute on second call with same inputs', () => {
    const compute = vi.fn(identity);
    const cache = createEntityMemoCache<number>();
    const entities = [1, 2, 3];
    getOrCompute(cache, entities, 'all', 'name', compute);
    getOrCompute(cache, entities, 'all', 'name', compute);
    expect(compute).toHaveBeenCalledTimes(1);
  });

  it('returns the SAME array reference on a cache hit', () => {
    const cache = createEntityMemoCache<number>();
    const entities = [1, 2, 3];
    const first  = getOrCompute(cache, entities, 'all', 'name', identity);
    const second = getOrCompute(cache, entities, 'all', 'name', identity);
    expect(second).toBe(first); // same reference, not just equal
  });

  it('returns same reference across many consecutive calls with same inputs', () => {
    const cache = createEntityMemoCache<number>();
    const entities = [7, 8];
    const first = getOrCompute(cache, entities, 'critter', 'chore', identity);
    for (let i = 0; i < 10; i++) {
      expect(getOrCompute(cache, entities, 'critter', 'chore', identity)).toBe(first);
    }
  });
});

// ── getOrCompute — cache miss (recompute) ─────────────────────────────────────

describe('getOrCompute — cache miss: entities reference changes', () => {
  it('recomputes when entity array reference changes', () => {
    const compute = vi.fn(identity);
    const cache = createEntityMemoCache<number>();
    const e1 = [1, 2];
    const e2 = [1, 2]; // same values, different reference
    getOrCompute(cache, e1, 'all', 'name', compute);
    getOrCompute(cache, e2, 'all', 'name', compute);
    expect(compute).toHaveBeenCalledTimes(2);
  });

  it('returns a NEW reference when entities reference changes', () => {
    const cache = createEntityMemoCache<number>();
    const e1 = [1, 2];
    const e2 = [1, 2];
    const r1 = getOrCompute(cache, e1, 'all', 'name', identity);
    const r2 = getOrCompute(cache, e2, 'all', 'name', identity);
    expect(r2).not.toBe(r1);
  });
});

describe('getOrCompute — cache miss: filter changes', () => {
  it('recomputes when filter changes', () => {
    const compute = vi.fn(identity);
    const cache = createEntityMemoCache<number>();
    const entities = [1, 2];
    getOrCompute(cache, entities, 'all',       'name', compute);
    getOrCompute(cache, entities, 'duplicant', 'name', compute);
    expect(compute).toHaveBeenCalledTimes(2);
  });

  it('returns a new reference when filter changes', () => {
    const cache = createEntityMemoCache<number>();
    const entities = [1];
    const r1 = getOrCompute(cache, entities, 'all',     'name', identity);
    const r2 = getOrCompute(cache, entities, 'critter', 'name', identity);
    expect(r2).not.toBe(r1);
  });
});

describe('getOrCompute — cache miss: sort changes', () => {
  it('recomputes when sort changes', () => {
    const compute = vi.fn(identity);
    const cache = createEntityMemoCache<number>();
    const entities = [1, 2];
    getOrCompute(cache, entities, 'all', 'name', compute);
    getOrCompute(cache, entities, 'all', 'type', compute);
    expect(compute).toHaveBeenCalledTimes(2);
  });

  it('returns a new reference when sort changes', () => {
    const cache = createEntityMemoCache<number>();
    const entities = [1];
    const r1 = getOrCompute(cache, entities, 'all', 'name',  identity);
    const r2 = getOrCompute(cache, entities, 'all', 'chore', identity);
    expect(r2).not.toBe(r1);
  });
});

// ── getOrCompute — cache updates correctly ────────────────────────────────────

describe('getOrCompute — cache state after miss', () => {
  it('updates cached inputs after a miss', () => {
    const cache = createEntityMemoCache<number>();
    const e1 = [1];
    const e2 = [2];
    getOrCompute(cache, e1, 'all', 'name', identity);
    getOrCompute(cache, e2, 'all', 'type', identity);
    expect(cache.lastEntities).toBe(e2);
    expect(cache.lastSort).toBe('type');
  });

  it('subsequent hit after miss uses updated cache', () => {
    const compute = vi.fn(identity);
    const cache = createEntityMemoCache<number>();
    const e1 = [1];
    const e2 = [2];
    getOrCompute(cache, e1, 'all', 'name', compute);
    getOrCompute(cache, e2, 'all', 'name', compute); // miss → recompute
    const r3 = getOrCompute(cache, e2, 'all', 'name', compute); // hit
    expect(compute).toHaveBeenCalledTimes(2); // no third call
    expect(r3).toBe(cache.lastResult);
  });
});

// ── integration: simulate EntityListPanel render loop ────────────────────────

describe('integration — simulated render loop', () => {
  it('computes only once when entities and options are stable across many renders', () => {
    const compute = vi.fn(identity);
    const cache = createEntityMemoCache<number>();
    const entities = [1, 2, 3];

    for (let render = 0; render < 20; render++) {
      getOrCompute(cache, entities, 'all', 'name', compute);
    }
    expect(compute).toHaveBeenCalledTimes(1);
  });

  it('recomputes on each render when entities reference changes every frame', () => {
    const compute = vi.fn(identity);
    const cache = createEntityMemoCache<number>();

    for (let render = 0; render < 5; render++) {
      const entities = [render]; // new reference each time (live poll)
      getOrCompute(cache, entities, 'all', 'name', compute);
    }
    expect(compute).toHaveBeenCalledTimes(5);
  });

  it('recomputes when user changes filter then returns to original', () => {
    const compute = vi.fn(identity);
    const cache = createEntityMemoCache<number>();
    const entities = [1, 2];
    getOrCompute(cache, entities, 'all',       'name', compute); // render 1
    getOrCompute(cache, entities, 'duplicant', 'name', compute); // filter changed
    getOrCompute(cache, entities, 'all',       'name', compute); // back to all
    expect(compute).toHaveBeenCalledTimes(3);
  });

  it('returns stable reference when entities stable but filter flips then flips back', () => {
    const cache = createEntityMemoCache<number>();
    const entities = [1, 2];
    const r1 = getOrCompute(cache, entities, 'all', 'name', identity);
    getOrCompute(cache, entities, 'critter', 'name', identity); // different filter
    // same filter as r1 → cache miss (prev was 'critter'), so new ref
    const r3 = getOrCompute(cache, entities, 'all', 'name', identity);
    expect(r3).not.toBe(r1); // cache was overwritten by 'critter' call
  });
});
