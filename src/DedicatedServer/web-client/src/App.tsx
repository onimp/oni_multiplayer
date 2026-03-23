import { useCallback, useEffect, useRef, useState } from 'react';
import type { WorldData, EntitiesResponse, EntityData, GameState, OverlayMode } from './api/types';
import { fetchAll, fetchElements, fetchEntities, fetchGameState, fetchWorld } from './api/client';
import { loadElements, areElementsLoaded } from './renderer/constants';
import type { CellInfo } from './renderer/WorldRenderer';
import { Header } from './components/Header';
import { WorldCanvas } from './components/WorldCanvas';
import { Sidebar } from './components/Sidebar';
import { ConnectionOverlay } from './components/ConnectionOverlay';
import { StatsBar } from './components/StatsBar';
import { deriveConnectionStatus } from './utils/connectionState';
import './index.css';

export default function App() {
  const [world, setWorld] = useState<WorldData | null>(null);
  const [entities, setEntities] = useState<EntitiesResponse | null>(null);
  const [gameState, setGameState] = useState<GameState | null>(null);
  const [connected, setConnected] = useState(false);
  const [retryIn, setRetryIn] = useState<number | null>(null);
  const [cellInfo, setCellInfo] = useState<CellInfo | null>(null);

  // Backoff state: current delay ms (0 = normal poll rate). Reset to 0 on success.
  const backoffRef = useRef(0);
  const retryTimerRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const [overlay, setOverlay] = useState<OverlayMode>('element');
  const [showEntities, setShowEntities] = useState(true);
  const [showGrid, setShowGrid] = useState(false);
  const [autoRefresh, setAutoRefresh] = useState(true);
  const [refreshInterval, setRefreshInterval] = useState(16);

  // Sidebar entity pin: name of the entity selected in EntityListPanel.
  // Passed down to WorldCanvas so the tooltip tracks the entity on the canvas.
  const [pinnedEntityName, setPinnedEntityName] = useState<string | null>(null);
  // Keep the full EntityData so WorldCanvas can render the tooltip without a separate lookup.
  const [pinnedEntity, setPinnedEntity] = useState<EntityData | null>(null);

  const handlePinEntity = useCallback((entity: EntityData | null) => {
    setPinnedEntityName(entity?.name ?? null);
    setPinnedEntity(entity);
  }, []);

  const handleEntityUnpinned = useCallback(() => {
    setPinnedEntityName(null);
    setPinnedEntity(null);
  }, []);

  // Initial full load (world + entities + state + elements)
  const refresh = useCallback(async () => {
    try {
      if (!areElementsLoaded()) {
        const elemData = await fetchElements();
        loadElements(elemData.elements);
      }
      const data = await fetchAll();
      setWorld(data.world);
      setEntities(data.entities);
      setGameState(data.state);
      setConnected(true);
    } catch {
      setConnected(false);
      setGameState(null);  // clear stale stats on failed initial load
    }
  }, []);

  useEffect(() => { refresh(); }, [refresh]);

  // Fast poll: entities + state at refreshInterval (default 16ms → ~60 UPS).
  // On error: exponential backoff 1s→2s→4s→8s→10s (max). Resets on success.
  // Skips cycles if a fetch is still in-flight to avoid pile-up.
  useEffect(() => {
    if (!autoRefresh) return;
    let active = true;
    let fetching = false;

    function clearCountdown() {
      if (retryTimerRef.current) { clearInterval(retryTimerRef.current); retryTimerRef.current = null; }
      setRetryIn(null);
    }

    function startCountdown(delayMs: number) {
      clearCountdown();
      let remaining = Math.round(delayMs / 1000);
      setRetryIn(remaining);
      retryTimerRef.current = setInterval(() => {
        remaining -= 1;
        if (remaining <= 0) { clearCountdown(); } else { setRetryIn(remaining); }
      }, 1000);
    }

    function tick() {
      if (!active) return;
      if (!fetching) {
        fetching = true;
        Promise.all([fetchEntities(), fetchGameState()])
          .then(([ent, st]) => {
            setEntities(ent); setGameState(st); setConnected(true);
            backoffRef.current = 0;
            clearCountdown();
          })
          .catch(() => {
            setConnected(false);
            setGameState(null);  // clear stale stats so StatsBar shows — not old values
            const next = backoffRef.current === 0 ? 1000 : Math.min(backoffRef.current * 2, 10000);
            backoffRef.current = next;
            startCountdown(next);
          })
          .finally(() => { fetching = false; });
      }
      const delay = backoffRef.current > 0 ? backoffRef.current : refreshInterval;
      if (active) window.setTimeout(tick, delay);
    }
    tick();
    return () => { active = false; clearCountdown(); };
  }, [autoRefresh, refreshInterval]);

  // Slow poll: world grid at 1 s — large payload, changes infrequently.
  useEffect(() => {
    if (!autoRefresh) return;
    const id = window.setInterval(() => {
      fetchWorld().then(setWorld).catch(() => {});
    }, 1000);
    return () => clearInterval(id);
  }, [autoRefresh]);

  const connStatus = deriveConnectionStatus({
    connected,
    hasData: world !== null,
    retryIn,
  });

  return (
    <div className="app">
      <Header connected={connected} retryIn={retryIn} gameState={gameState} />
      <StatsBar gameState={gameState} />
      <div className="main">
        <div className="canvas-container">
          <ConnectionOverlay status={connStatus} onReconnect={refresh} />
          <WorldCanvas
            world={world}
            entities={entities}
            overlay={overlay}
            showEntities={showEntities}
            showGrid={showGrid}
            serverUps={gameState?.serverUps}
            onCellHover={setCellInfo}
            pinnedEntity={pinnedEntity}
            onEntityUnpinned={handleEntityUnpinned}
          />
        </div>
        <Sidebar
          gameState={gameState}
          entities={entities}
          cellInfo={cellInfo}
          overlay={overlay}
          showEntities={showEntities}
          showGrid={showGrid}
          autoRefresh={autoRefresh}
          refreshInterval={refreshInterval}
          pinnedEntityName={pinnedEntityName}
          onOverlayChange={setOverlay}
          onShowEntitiesChange={setShowEntities}
          onShowGridChange={setShowGrid}
          onAutoRefreshChange={setAutoRefresh}
          onRefreshIntervalChange={setRefreshInterval}
          onRefreshNow={refresh}
          onPinEntity={handlePinEntity}
        />
      </div>
    </div>
  );
}
