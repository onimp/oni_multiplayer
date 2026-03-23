import { useCallback, useEffect, useRef } from 'react';
import type { WorldData, EntitiesResponse, OverlayMode } from '../api/types';
import type { CellInfo } from '../renderer/WorldRenderer';
import { WorldRenderer } from '../renderer/WorldRenderer';

interface Props {
  world: WorldData | null;
  entities: EntitiesResponse | null;
  overlay: OverlayMode;
  showEntities: boolean;
  showGrid: boolean;
  serverUps: number | undefined;
  onCellHover: (info: CellInfo | null) => void;
}

export function WorldCanvas({ world, entities, overlay, showEntities, showGrid, serverUps, onCellHover }: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const rendererRef = useRef<WorldRenderer | null>(null);
  const isDragging = useRef(false);
  const lastMouse = useRef({ x: 0, y: 0 });

  // Refs holding latest data so the rAF loop always reads fresh values
  // without needing to restart the loop when props change.
  const worldRef = useRef<WorldData | null>(null);
  const entitiesRef = useRef<EntitiesResponse | null>(null);
  const overlayRef = useRef<OverlayMode>(overlay);
  const showEntitiesRef = useRef(showEntities);
  const showGridRef = useRef(showGrid);
  const serverUpsRef = useRef<number | undefined>(serverUps);

  // Keep refs in sync with latest props every render.
  worldRef.current = world;
  entitiesRef.current = entities;
  overlayRef.current = overlay;
  showEntitiesRef.current = showEntities;
  showGridRef.current = showGrid;
  serverUpsRef.current = serverUps;

  // Client UPS tracking: count rAF renders per second.
  const renderCountRef = useRef(0);
  const clientUpsRef = useRef(0);
  useEffect(() => {
    const id = window.setInterval(() => {
      clientUpsRef.current = renderCountRef.current;
      renderCountRef.current = 0;
    }, 1000);
    return () => clearInterval(id);
  }, []);

  // Initialize renderer + start rAF loop.
  useEffect(() => {
    if (!canvasRef.current) return;
    rendererRef.current = new WorldRenderer(canvasRef.current);

    let rafId: number;
    function loop() {
      const r = rendererRef.current;
      const w = worldRef.current;
      if (r && w) {
        r.render(w, entitiesRef.current, {
          overlay: overlayRef.current,
          showEntities: showEntitiesRef.current,
          showGrid: showGridRef.current,
        });
        renderCountRef.current++;
        r.renderUpsOverlay(serverUpsRef.current, clientUpsRef.current);
      }
      rafId = requestAnimationFrame(loop);
    }
    rafId = requestAnimationFrame(loop);
    return () => cancelAnimationFrame(rafId);
  }, []);

  // Resize canvas on parent resize.
  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const resizeObserver = new ResizeObserver(() => {
      const parent = canvas.parentElement;
      if (!parent) return;
      canvas.width = parent.clientWidth;
      canvas.height = parent.clientHeight;
    });
    resizeObserver.observe(canvas.parentElement!);
    return () => resizeObserver.disconnect();
  }, []);

  const handleWheel = useCallback((e: React.WheelEvent) => {
    e.preventDefault();
    if (!rendererRef.current || !worldRef.current) return;
    const rect = canvasRef.current!.getBoundingClientRect();
    rendererRef.current.zoom(e.deltaY > 0 ? -1 : 1, e.clientX - rect.left, e.clientY - rect.top);
    // rAF loop will redraw on next frame — no manual render() call needed.
  }, []);

  const handleMouseDown = useCallback((e: React.MouseEvent) => {
    if (e.button === 0) {
      isDragging.current = true;
      lastMouse.current = { x: e.clientX, y: e.clientY };
    }
  }, []);

  const handleMouseMove = useCallback((e: React.MouseEvent) => {
    if (!rendererRef.current || !worldRef.current) return;

    if (isDragging.current) {
      const dx = e.clientX - lastMouse.current.x;
      const dy = e.clientY - lastMouse.current.y;
      lastMouse.current = { x: e.clientX, y: e.clientY };
      rendererRef.current.pan(dx, dy);
      // rAF loop redraws on next frame.
    } else {
      const rect = canvasRef.current!.getBoundingClientRect();
      const info = rendererRef.current.getCellAt(
        e.clientX - rect.left, e.clientY - rect.top,
        worldRef.current, entitiesRef.current
      );
      onCellHover(info);
    }
  }, [onCellHover]);

  const handleMouseUp = useCallback(() => {
    isDragging.current = false;
  }, []);

  const handleMouseLeave = useCallback(() => {
    isDragging.current = false;
    onCellHover(null);
  }, [onCellHover]);

  return (
    <canvas
      ref={canvasRef}
      onWheel={handleWheel}
      onMouseDown={handleMouseDown}
      onMouseMove={handleMouseMove}
      onMouseUp={handleMouseUp}
      onMouseLeave={handleMouseLeave}
      style={{ display: 'block', cursor: 'crosshair' }}
    />
  );
}
