import { useCallback, useEffect, useRef, useState } from 'react';
import type { WorldData, EntitiesResponse, GameState, OverlayMode } from './api/types';
import { fetchAll, fetchElements } from './api/client';
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
  const [cellInfo, setCellInfo] = useState<CellInfo | null>(null);

  const [overlay, setOverlay] = useState<OverlayMode>('element');
  const [showEntities, setShowEntities] = useState(true);
  const [showGrid, setShowGrid] = useState(false);
  const [autoRefresh, setAutoRefresh] = useState(false);
  const [refreshInterval, setRefreshInterval] = useState(1000);

  const timerRef    = useRef<number | null>(null);
  const fetchingRef = useRef(false);

  const refresh = useCallback(async () => {
    try {
      // Load element definitions on first fetch
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

  useEffect(() => {
    if (!autoRefresh) {
      if (timerRef.current) clearTimeout(timerRef.current);
      timerRef.current = null;
      return;
    }

    let active = true;

    // Sequential poll: next request only starts after the previous one completes.
    // Prevents request pile-up when server is slow.
    async function poll() {
      if (!active) return;
      if (!fetchingRef.current) {
        fetchingRef.current = true;
        try { await refresh(); }
        finally { fetchingRef.current = false; }
      }
      if (active) timerRef.current = window.setTimeout(poll, refreshInterval);
    }

    poll();

    return () => {
      active = false;
      if (timerRef.current) clearTimeout(timerRef.current);
    };
  }, [autoRefresh, refreshInterval, refresh]);

  return (
    <div className="app">
      <Header connected={connected} gameState={gameState} />
      <div className="main">
        <div className="canvas-container">
          <WorldCanvas
            world={world}
            entities={entities}
            overlay={overlay}
            showEntities={showEntities}
            showGrid={showGrid}
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
