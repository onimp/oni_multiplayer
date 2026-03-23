import type { EntityData } from '../api/types';

export type EntityFilter = 'all' | 'duplicant' | 'critter';
export type EntitySort   = 'name' | 'type' | 'chore';

/** Icon/short label shown in the entity list for each type. */
export const TYPE_LABELS: Record<EntityData['type'], string> = {
  duplicant:  '👤',
  critter:    '🐾',
  building:   '🏗',
  entity:     '📦',
  pickupable: '⬆',
  ore:        '🪨',
};

/**
 * Returns the most-specific status string for an entity.
 * Priority: currentChore → smState → state → '—'
 */
export function getEntityChore(e: EntityData): string {
  if (e.currentChore) return e.currentChore;
  // smState: null or '' both treated as "no state" — fall through to next field
  if (e.smState) return e.smState;
  if (e.state)   return e.state;
  return '—';
}

/** Narrow the list to a specific entity type (or keep all). */
export function filterEntities(entities: EntityData[], filter: EntityFilter): EntityData[] {
  if (filter === 'all') return entities;
  return entities.filter(e => e.type === filter);
}

/** Return a sorted copy — does NOT mutate the input array. */
export function sortEntities(entities: EntityData[], sort: EntitySort): EntityData[] {
  const copy = [...entities];
  switch (sort) {
    case 'name':
      copy.sort((a, b) => a.name.localeCompare(b.name));
      break;
    case 'type':
      copy.sort((a, b) => a.type.localeCompare(b.type) || a.name.localeCompare(b.name));
      break;
    case 'chore':
      copy.sort((a, b) => getEntityChore(a).localeCompare(getEntityChore(b)));
      break;
  }
  return copy;
}

/** Convenience: filter then sort in one call. */
export function filterAndSort(
  entities: EntityData[],
  filter: EntityFilter,
  sort: EntitySort,
): EntityData[] {
  return sortEntities(filterEntities(entities, filter), sort);
}
