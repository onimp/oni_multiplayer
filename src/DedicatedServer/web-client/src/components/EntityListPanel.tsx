import { useMemo, useState } from 'react';
import type { EntityData } from '../api/types';
import { getEntityChore, TYPE_LABELS } from '../utils/entityList';
import type { EntityFilter, EntitySort } from '../utils/entityList';
import { useMemoEntities } from '../hooks/useMemoEntities';

interface Props {
  entities: EntityData[];
  pinnedEntityName: string | null;
  onPinEntity: (entity: EntityData | null) => void;
}

export function EntityListPanel({ entities, pinnedEntityName, onPinEntity }: Props) {
  const [open,   setOpen]   = useState(false);
  const [filter, setFilter] = useState<EntityFilter>('all');
  const [sort,   setSort]   = useState<EntitySort>('name');

  // Stable reference — only recomputes when entities ref, filter, or sort changes.
  const list = useMemoEntities(entities, filter, sort);

  // Counts are derived only from the entities reference — useMemo skips
  // recompute when the entities array reference is unchanged.
  const { dupes, critters } = useMemo(() => ({
    dupes:    entities.filter(e => e.type === 'duplicant').length,
    critters: entities.filter(e => e.type === 'critter').length,
  }), [entities]);

  const summary = dupes > 0 || critters > 0
    ? `${dupes}D ${critters}C`
    : `${entities.length}`;

  return (
    <section className="entity-list-panel">
      <button
        className="entity-list-toggle"
        onClick={() => setOpen(o => !o)}
        aria-expanded={open}
      >
        <span className="entity-list-toggle-arrow">{open ? '▼' : '▶'}</span>
        Entities
        <span className="entity-list-toggle-count">{summary}</span>
      </button>

      {open && (
        <div className="entity-list-body">
          {/* Controls */}
          <div className="entity-list-controls">
            <select
              value={filter}
              onChange={e => setFilter(e.target.value as EntityFilter)}
              title="Filter by type"
            >
              <option value="all">All types</option>
              <option value="duplicant">Dupes only</option>
              <option value="critter">Critters only</option>
            </select>
            <select
              value={sort}
              onChange={e => setSort(e.target.value as EntitySort)}
              title="Sort order"
            >
              <option value="name">Name ↑</option>
              <option value="type">Type ↑</option>
              <option value="chore">Chore ↑</option>
            </select>
          </div>

          {/* Scrollable list */}
          <div className="entity-list-scroll" role="list">
            {list.length === 0 && (
              <div className="muted" style={{ padding: '6px 4px' }}>No entities</div>
            )}
            {list.map((e, i) => {
              const pinned = pinnedEntityName === e.name;
              const chore  = getEntityChore(e);
              return (
                <div
                  key={i}
                  role="listitem"
                  className={`entity-row${pinned ? ' entity-row--pinned' : ''}`}
                  onClick={() => onPinEntity(pinned ? null : e)}
                  title={pinned ? 'Click to unpin' : 'Click to pin tooltip'}
                >
                  <span className="entity-row-icon" aria-hidden="true">
                    {TYPE_LABELS[e.type] ?? '?'}
                  </span>
                  <span className="entity-row-name">{e.name}</span>
                  <span className="entity-row-chore">{chore}</span>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </section>
  );
}
