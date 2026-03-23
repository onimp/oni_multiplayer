import { describe, it, expect } from 'vitest';
import type { EntityData } from '../api/types';
import {
  getEntityChore,
  filterEntities,
  sortEntities,
  filterAndSort,
  TYPE_LABELS,
} from './entityList';

// ── Helpers ──────────────────────────────────────────────────────────────────

function makeEntity(overrides: Partial<EntityData> & { name: string }): EntityData {
  return {
    type:  'entity',
    x: 0, y: 0,
    ...overrides,
  };
}

const dupe1 = makeEntity({ name: 'Alice', type: 'duplicant', currentChore: 'Dig',   smState: 'Work'  });
const dupe2 = makeEntity({ name: 'Bob',   type: 'duplicant', currentChore: 'Eat',   smState: 'Idle'  });
const crit1 = makeEntity({ name: 'Pip',   type: 'critter',   state: 'Wander'                         });
const crit2 = makeEntity({ name: 'Drecko',type: 'critter',   state: 'Graze'                          });
const bldg  = makeEntity({ name: 'Outhouse', type: 'building'                                        });

// ── getEntityChore ────────────────────────────────────────────────────────────

describe('getEntityChore', () => {
  it('returns currentChore when present (highest priority)', () => {
    expect(getEntityChore(dupe1)).toBe('Dig');
  });

  it('falls back to smState when currentChore absent', () => {
    const e = makeEntity({ name: 'X', smState: 'Relaxing' });
    expect(getEntityChore(e)).toBe('Relaxing');
  });

  it('falls back to state when both currentChore and smState absent', () => {
    expect(getEntityChore(crit1)).toBe('Wander');
  });

  it('returns — when all fields absent', () => {
    const e = makeEntity({ name: 'Empty' });
    expect(getEntityChore(e)).toBe('—');
  });

  it('returns — when smState is empty string (falsy)', () => {
    const e = makeEntity({ name: 'X', smState: '' });
    expect(getEntityChore(e)).toBe('—');
  });

  it('handles smState = null — falls through to state', () => {
    // smState: null is falsy, so state is checked next
    const e = makeEntity({ name: 'X', smState: null, state: 'Idle' });
    expect(getEntityChore(e)).toBe('Idle');
  });
});

// ── TYPE_LABELS ───────────────────────────────────────────────────────────────

describe('TYPE_LABELS', () => {
  it('has a label for every entity type', () => {
    const types: EntityData['type'][] = [
      'duplicant', 'critter', 'building', 'entity', 'pickupable', 'ore',
    ];
    for (const t of types) {
      expect(TYPE_LABELS[t]).toBeTruthy();
    }
  });
});

// ── filterEntities ────────────────────────────────────────────────────────────

describe('filterEntities', () => {
  const all = [dupe1, dupe2, crit1, crit2, bldg];

  it('filter=all returns entire list unchanged', () => {
    expect(filterEntities(all, 'all')).toHaveLength(5);
    expect(filterEntities(all, 'all')).toEqual(all);
  });

  it('filter=duplicant returns only duplicants', () => {
    const result = filterEntities(all, 'duplicant');
    expect(result).toHaveLength(2);
    expect(result.every(e => e.type === 'duplicant')).toBe(true);
  });

  it('filter=critter returns only critters', () => {
    const result = filterEntities(all, 'critter');
    expect(result).toHaveLength(2);
    expect(result.every(e => e.type === 'critter')).toBe(true);
  });

  it('returns empty array when no entities match filter', () => {
    expect(filterEntities([bldg], 'duplicant')).toHaveLength(0);
  });

  it('does not mutate the input array', () => {
    const input = [dupe1, crit1];
    filterEntities(input, 'duplicant');
    expect(input).toHaveLength(2);
  });
});

// ── sortEntities ──────────────────────────────────────────────────────────────

describe('sortEntities', () => {
  const mixed = [dupe2, crit1, dupe1, bldg, crit2]; // intentionally unsorted

  it('sort=name produces alphabetical order by name', () => {
    const result = sortEntities(mixed, 'name');
    const names = result.map(e => e.name);
    expect(names).toEqual([...names].sort());
  });

  it('sort=type groups by type, then name within each group', () => {
    const result = sortEntities(mixed, 'type');
    // building < critter < duplicant < entity (alphabetical type order)
    expect(result[0].type).toBe('building');
    expect(result[1].type).toBe('critter');
    expect(result[2].type).toBe('critter');
    expect(result[3].type).toBe('duplicant');
    expect(result[4].type).toBe('duplicant');
    // within critters: Drecko < Pip
    expect(result[1].name).toBe('Drecko');
    expect(result[2].name).toBe('Pip');
    // within duplicants: Alice < Bob
    expect(result[3].name).toBe('Alice');
    expect(result[4].name).toBe('Bob');
  });

  it('sort=chore sorts by resolved chore string (localeCompare order)', () => {
    // dupe1→Dig, dupe2→Eat, crit1→Wander, crit2→Graze, bldg→—
    const result = sortEntities(mixed, 'chore');
    const chores = result.map(e => getEntityChore(e));
    // Verify each pair is in non-decreasing localeCompare order (same comparator as impl uses)
    for (let i = 0; i < chores.length - 1; i++) {
      expect(chores[i].localeCompare(chores[i + 1])).toBeLessThanOrEqual(0);
    }
  });

  it('does not mutate the input array', () => {
    const input = [dupe2, dupe1];
    sortEntities(input, 'name');
    expect(input[0].name).toBe('Bob'); // unchanged
  });
});

// ── filterAndSort ─────────────────────────────────────────────────────────────

describe('filterAndSort', () => {
  const all = [dupe2, crit1, dupe1, crit2, bldg];

  it('filters then sorts: duplicants by name', () => {
    const result = filterAndSort(all, 'duplicant', 'name');
    expect(result).toHaveLength(2);
    expect(result[0].name).toBe('Alice');
    expect(result[1].name).toBe('Bob');
  });

  it('filters then sorts: critters by chore', () => {
    const result = filterAndSort(all, 'critter', 'chore');
    // Graze < Wander
    expect(result[0].name).toBe('Drecko');
    expect(result[1].name).toBe('Pip');
  });

  it('all + sort=name returns full sorted list', () => {
    const result = filterAndSort(all, 'all', 'name');
    expect(result).toHaveLength(5);
    const names = result.map(e => e.name);
    expect(names).toEqual([...names].sort());
  });

  it('returns empty array when filter matches nothing', () => {
    expect(filterAndSort([bldg], 'critter', 'name')).toHaveLength(0);
  });
});
