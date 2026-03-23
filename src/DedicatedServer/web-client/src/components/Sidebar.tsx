import type { EntitiesResponse, EntityData, GameState, OverlayMode } from '../api/types';
import type { CellInfo } from '../renderer/WorldRenderer';
import { EntityListPanel } from './EntityListPanel';

interface Props {
  gameState: GameState | null;
  entities: EntitiesResponse | null;
  cellInfo: CellInfo | null;
  overlay: OverlayMode;
  showEntities: boolean;
  showGrid: boolean;
  autoRefresh: boolean;
  refreshInterval: number;
  pinnedEntityName: string | null;
  onOverlayChange: (mode: OverlayMode) => void;
  onShowEntitiesChange: (show: boolean) => void;
  onShowGridChange: (show: boolean) => void;
  onAutoRefreshChange: (auto: boolean) => void;
  onRefreshIntervalChange: (ms: number) => void;
  onRefreshNow: () => void;
  onPinEntity: (entity: EntityData | null) => void;
}

export function Sidebar({
  gameState, entities, cellInfo,
  overlay, showEntities, showGrid, autoRefresh, refreshInterval,
  pinnedEntityName,
  onOverlayChange, onShowEntitiesChange, onShowGridChange,
  onAutoRefreshChange, onRefreshIntervalChange, onRefreshNow,
  onPinEntity,
}: Props) {
  return (
    <aside className="sidebar">
      <section>
        <h3>Display</h3>
        {(['element', 'temperature', 'mass'] as OverlayMode[]).map(mode => (
          <label key={mode}>
            <input
              type="radio"
              name="overlay"
              value={mode}
              checked={overlay === mode}
              onChange={() => onOverlayChange(mode)}
            />
            {mode.charAt(0).toUpperCase() + mode.slice(1)}
          </label>
        ))}
        <label>
          <input type="checkbox" checked={showEntities} onChange={e => onShowEntitiesChange(e.target.checked)} />
          Show Entities
        </label>
        <label>
          <input type="checkbox" checked={showGrid} onChange={e => onShowGridChange(e.target.checked)} />
          Show Grid
        </label>
      </section>

      <section>
        <h3>Refresh</h3>
        <label>
          <input type="checkbox" checked={autoRefresh} onChange={e => onAutoRefreshChange(e.target.checked)} />
          Auto-refresh
        </label>
        <label>
          Interval (ms):
          <input
            type="number"
            value={refreshInterval}
            min={16}
            max={10000}
            step={1}
            onChange={e => onRefreshIntervalChange(Math.max(16, parseInt(e.target.value) || 16))}
          />
        </label>
        <button onClick={onRefreshNow}>Refresh Now</button>
      </section>

      <section>
        <h3>Cell Info</h3>
        {cellInfo ? (
          <div className="cell-details">
            <div><span className="label">Position:</span> ({cellInfo.x}, {cellInfo.y})</div>
            <div><span className="label">Element:</span> {cellInfo.element}</div>
            <div><span className="label">Temp:</span> {cellInfo.temperatureC}&deg;C ({cellInfo.temperature}K)</div>
            <div><span className="label">Mass:</span> {cellInfo.mass} kg</div>
            {cellInfo.entities && (
              <div><span className="label">Entities:</span> {cellInfo.entities.join(', ')}</div>
            )}
          </div>
        ) : (
          <div className="cell-details muted">Hover over a cell</div>
        )}
      </section>

      <EntityListPanel
        entities={entities?.entities ?? []}
        pinnedEntityName={pinnedEntityName}
        onPinEntity={onPinEntity}
      />

      {gameState && (
        <section>
          <h3>Game State</h3>
          <div className="cell-details">
            <div><span className="label">Cycle:</span> {gameState.cycle}</div>
            <div><span className="label">Tick:</span> {gameState.tick}</div>
            <div><span className="label">Speed:</span> {gameState.speed}x</div>
            <div><span className="label">Dupes:</span> {gameState.duplicantCount}</div>
            <div><span className="label">Buildings:</span> {gameState.buildingCount}</div>
          </div>
        </section>
      )}
    </aside>
  );
}
