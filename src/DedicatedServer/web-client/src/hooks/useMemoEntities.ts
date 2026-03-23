/**
 * useMemoEntities.ts — React hook that returns a stable sorted/filtered
 * entity array reference when the inputs haven't changed.
 *
 * Prevents the O(n log n) sort in EntityListPanel from running on every
 * 16ms poll frame when the data and options are unchanged.
 *
 * Uses the pure getOrCompute() gate from entityMemo.ts — the same
 * dirty-flag pattern used by renderGate.ts for the canvas loop.
 */

import { useRef } from 'react';
import type { EntityData } from '../api/types';
import { filterAndSort } from '../utils/entityList';
import type { EntityFilter, EntitySort } from '../utils/entityList';
import { createEntityMemoCache, getOrCompute } from '../utils/entityMemo';
import type { EntityMemoCache } from '../utils/entityMemo';

/**
 * Returns a stable sorted/filtered entity array.
 *
 * The returned reference is the same object across consecutive renders
 * as long as `entities` (by reference), `filter`, and `sort` are all
 * unchanged.  When any input changes the list is recomputed and the
 * new reference is returned.
 */
export function useMemoEntities(
  entities: EntityData[],
  filter: EntityFilter,
  sort: EntitySort,
): EntityData[] {
  const cache = useRef<EntityMemoCache<EntityData>>(createEntityMemoCache());
  return getOrCompute(
    cache.current,
    entities,
    filter,
    sort,
    (e, f, s) => filterAndSort(e as EntityData[], f as EntityFilter, s as EntitySort),
  );
}
