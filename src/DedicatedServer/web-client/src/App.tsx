import { useCallback, useEffect, useRef, useState } from 'react';
import type { WorldData, EntitiesResponse, EntityData, GameState, OverlayMode } from './api/types';
import { fetchAll, fetchElements, fetchEntities, fetchGameState, fetchWorld } from './api/client';
import { loadElements, areElementsLoaded } from './renderer/constants';
import type { CellInfo } from './renderer/WorldRenderer';
import type { CanvasActions } from './components/WorldCanvas';
import { Header } from './components/Header';
import { WorldCanvas } from './components/WorldCanvas';
import { Sidebar } from './components/Sidebar';
import { ConnectionOverlay } from './components/ConnectionOverlay';
import { StatsBar } from './components/StatsBar';
import { HelpOverlay } from './components/HelpOverlay';
import { TimelineScrubber } from './components/TimelineScrubber';
import { deriveConnectionStatus } from './utils/connectionState';
import { resolveKeyAction, isInputTarget } from './utils/keybindings';
import { screenshotFilename, triggerDownload } from './utils/screenshot';
import { getElementState } from './renderer/constants';
import { inspectCell } from './utils/cellInspector';
import { CellInspectorPanel } from './components/CellInspectorPanel';
import { createTickBuffer, pushTick, getLatestTick, getAllTicks } from './utils/tickBuffer';
import type { TickSnapshot } from './utils/tickBuffer';
import './index.css';

export default function App() {
  const [world, setWorld] = useState<WorldData | null>(null);
  const [entities, setEntities] = useState<EntitiesResponse | null>(null);
  const [gameState, setGameState] = useState<GameState | null>(null);
  const [connected, setConnected] = useState(false);
  const [retryIn, setRetryIn] = useState<number | null>(null);
  const [cellInfo, setCellInfo] = useState<CellInfo | null>(null);

  const [inspectedCell, setInspectedCell] = useState<CellInfo | null>(null);

  // Derived inspector data: formatted for display. Re-computed on inspectedCell change.
  const inspectorData = inspectedCell
    ? inspectCell({
        x: inspectedCell.x,
        y: inspectedCell.y,
        element: inspectedCell.element,
        elementState: getElementState(inspectedCell.elementId),
        temperature: inspectedCell.temperature,
        temperatureC: inspectedCell.temperatureC,
        mass: inspectedCell.mass,
        entities: inspectedCell.entities,
      })
    : null;

  // Backoff state: current delay ms (0 = normal poll rate). Reset to 0 on success.
  const backoffRef = useRef(0);
  const retryTimerRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const [overlay, setOverlay] = useState<OverlayMode>('element');
  const [showEntities, setShowEntities] = useState(true);
  const [showGrid, setShowGrid] = useState(false);
  const [showMinimap, setShowMinimap] = useState(true);
  const [autoRefresh, setAutoRefresh] = useState(true);
  const [refreshInterval, setRefreshInterval] = useState(16);

  // ── Tick history ──────────────────────────────────────────────────────────
  // Circular buffer of the last 200 unique-tick poll results.
  const tickBufferRef = useRef(createTickBuffer(200));
  // bufferTicks drives the scrubber UI — updated whenever a new unique tick is stored.
  const [bufferTicks, setBufferTicks] = useState<TickSnapshot[]>([]);
  // selectedTick: null = live mode, number = showing this historical tick.
  const [selectedTick, setSelectedTick] = useState<number | null>(null);

  // Imperative canvas API (zoom/pan) — populated by WorldCanvas on mount.
  const canvasActionsRef = useRef<CanvasActions | null>(null);

  // Always-current gameState ref — lets the keydown handler (registered once
  // with deps=[]) read the latest tick without capturing a stale closure.
  const gameStateRef = useRef(gameState);
  gameStateRef.current = gameState;

  // Help overlay visibility
  const [showHelp, setShowHelp] = useState(false);

  // Global keyboard shortcut handler. Skips when focus is on a form input so
  // typing in the refresh-interval field or overlay search doesn't trigger panning.
  useEffect(() => {
    const onKeyDown = (e: KeyboardEvent) => {
      if (isInputTarget(e.target)) return;
      const action = resolveKeyAction(e.key, e.ctrlKey || e.metaKey);
      if (!action) return;
      // Prevent browser defaults: arrow-key scroll, +/- zoom, Ctrl+S save-page
      e.preventDefault();
      switch (action) {
        case 'zoom-in':     canvasActionsRef.current?.zoomIn();    break;
        case 'zoom-out':    canvasActionsRef.current?.zoomOut();   break;
        case 'zoom-reset':  canvasActionsRef.current?.zoomReset(); break;
        case 'pan-left':    canvasActionsRef.current?.panLeft();   break;
        case 'pan-right':   canvasActionsRef.current?.panRight();  break;
        case 'pan-up':      canvasActionsRef.current?.panUp();     break;
        case 'pan-down':    canvasActionsRef.current?.panDown();   break;
        case 'toggle-grid':    setShowGrid(g => !g);               break;
        case 'toggle-minimap': setShowMinimap(m => !m);           break;
        case 'show-help':   setShowHelp(true);                            break;
        case 'close-help':  setShowHelp(false); setInspectedCell(null); setSelectedTick(null); break;
        case 'export-screenshot': {
          const dataUrl = canvasActionsRef.current?.screenshot();
          if (dataUrl) triggerDownload(dataUrl, screenshotFilename(gameStateRef.current?.tick));
          break;
        }
      }
    };
    window.addEventListener('keydown', onKeyDown);
    return () => window.removeEventListener('keydown', onKeyDown);
  }, []);

  // Sidebar entity pin: name of the entity selected in EntityListPanel.
  // Passed down to WorldCanvas so the tooltip tracks the entity on the canvas.
  const [pinnedEntityName, setPinnedEntityName] = useState<string | null>(null);
  // Keep the full EntityData so WorldCanvas can render the tooltip without a separate lookup.
  const [pinnedEntity, setPinnedEntity] = useState<EntityData | null>(null);

  const handlePinEntity = useCallback((entity: EntityData | null) => {
    setPinnedEntityName(entity?.name ?? null);
    setPinnedEntity(entity);
  }, []);

  const handleScreenshot = useCallback(() => {
    const dataUrl = canvasActionsRef.current?.screenshot();
    if (dataUrl) triggerDownload(dataUrl, screenshotFilename(gameState?.tick));
  }, [gameState?.tick]);

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
            // Buffer only unique game ticks (skip duplicate ticks from fast polls).
            if (ent.tick !== getLatestTick(tickBufferRef.current)?.tick) {
              pushTick(tickBufferRef.current, { tick: ent.tick, entities: ent, gameState: st });
              setBufferTicks(getAllTicks(tickBufferRef.current));
            }
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

  // When scrubbing: show the selected historical snapshot; fall back to live on miss.
  const scrubbedSnap = selectedTick !== null
    ? bufferTicks.find(s => s.tick === selectedTick)
    : undefined;
  const displayEntities  = scrubbedSnap?.entities  ?? entities;
  const displayGameState = scrubbedSnap?.gameState ?? gameState;

  const connStatus = deriveConnectionStatus({
    connected,
    hasData: world !== null,
    retryIn,
  });

  return (
    <div className="app">
      {showHelp && <HelpOverlay onClose={() => setShowHelp(false)} />}
      <Header connected={connected} retryIn={retryIn} gameState={displayGameState} />
      <StatsBar gameState={displayGameState} />
      <div className="main">
        <div className="canvas-container">
          <ConnectionOverlay status={connStatus} onReconnect={refresh} />
          {inspectorData && (
            <CellInspectorPanel
              data={inspectorData}
              onClose={() => setInspectedCell(null)}
            />
          )}
          <WorldCanvas
            world={world}
            entities={displayEntities}
            overlay={overlay}
            showEntities={showEntities}
            showGrid={showGrid}
            serverUps={displayGameState?.serverUps}
            onCellHover={setCellInfo}
            pinnedEntity={pinnedEntity}
            onEntityUnpinned={handleEntityUnpinned}
            actionsRef={canvasActionsRef}
            showMinimap={showMinimap}
            onCellInspect={setInspectedCell}
          />
          <TimelineScrubber
            ticks={bufferTicks}
            selectedTick={selectedTick}
            onSelect={setSelectedTick}
            onLive={() => setSelectedTick(null)}
          />
        </div>
        <Sidebar
          gameState={gameState}
          entities={displayEntities}
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
          onScreenshot={handleScreenshot}
          onPinEntity={handlePinEntity}
        />
      </div>
    </div>
  );
}
