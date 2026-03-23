import { useCallback, useEffect, useRef, useState } from 'react';
import type { WorldData, EntitiesResponse, GameState, OverlayMode } from './api/types';
import { fetchAll, fetchElements, fetchEntities, fetchGameState, fetchWorld } from './api/client';
import { loadElements, areElementsLoaded } from './renderer/constants';
import type { CellInfo } from './renderer/WorldRenderer';
import { Header } from './components/Header';
import { WorldCanvas } from './components/WorldCanvas';
import { Sidebar } from './components/Sidebar';
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

  return (
    <div className="app">
      <Header connected={connected} retryIn={retryIn} gameState={gameState} />
      <div className="main">
        <div className="canvas-container">
          <WorldCanvas
            world={world}
            entities={entities}
            overlay={overlay}
            showEntities={showEntities}
            showGrid={showGrid}
            serverUps={gameState?.serverUps}
            onCellHover={setCellInfo}
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
          onOverlayChange={setOverlay}
          onShowEntitiesChange={setShowEntities}
          onShowGridChange={setShowGrid}
          onAutoRefreshChange={setAutoRefresh}
          onRefreshIntervalChange={setRefreshInterval}
          onRefreshNow={refresh}
        />
      </div>
    </div>
  );
}
