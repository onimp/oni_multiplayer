/**
 * entityMemo.ts — pure memoization cache for filtered+sorted entity lists.
 *
 * Avoids the O(n log n) sort on every 16ms rAF poll when entities, filter,
 * and sort haven't changed.  Uses object-reference comparison for the entity
 * array (same pattern as renderGate.ts) — a new ref means new poll data.
 *
 * Pure module — no React, no DOM.  Fully testable in Node/vitest.
 * The React hook (useMemoEntities.ts) wraps this with a useRef.
 */

export interface EntityMemoCache<T> {
  /** Last entity array reference.  null = cache is cold. */
  lastEntities: readonly T[] | null;
  /** Last filter string value. */
  lastFilter: string | null;
  /** Last sort string value. */
  lastSort: string | null;
  /** The cached result from the last compute call. */
  lastResult: T[];
}

/** Creates a cold (empty) cache. */
export function createEntityMemoCache<T>(): EntityMemoCache<T> {
  return { lastEntities: null, lastFilter: null, lastSort: null, lastResult: [] };
}

/**
 * Returns the cached result when all three inputs are identical to last call,
 * otherwise calls `compute(entities, filter, sort)`, stores the result, and
 * returns it.
 *
 * @param cache   - mutable cache object (stored in a useRef)
 * @param entities - entity array — compared by reference (O(1))
 * @param filter   - filter string — compared by value
 * @param sort     - sort string — compared by value
 * @param compute  - pure function that produces the derived array
 */
export function getOrCompute<T>(
  cache: EntityMemoCache<T>,
  entities: readonly T[],
  filter: string,
  sort: string,
  compute: (entities: readonly T[], filter: string, sort: string) => T[],
): T[] {
  if (
    cache.lastEntities === entities &&
    cache.lastFilter   === filter   &&
    cache.lastSort     === sort
  ) {
    return cache.lastResult;
  }
  cache.lastEntities = entities;
  cache.lastFilter   = filter;
  cache.lastSort     = sort;
  cache.lastResult   = compute(entities, filter, sort);
  return cache.lastResult;
}
