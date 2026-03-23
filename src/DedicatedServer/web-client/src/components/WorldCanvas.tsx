import { useCallback, useEffect, useRef } from 'react';
import type { WorldData, EntitiesResponse, OverlayMode, EntityData } from '../api/types';
import type { CellInfo } from '../renderer/WorldRenderer';
import { WorldRenderer } from '../renderer/WorldRenderer';

/** Returns ALL entities whose cell footprint covers (mouseX, mouseY) in canvas pixels. */
function getEntitiesAt(
  mouseX: number, mouseY: number,
  world: WorldData, entities: EntitiesResponse | null,
  renderer: WorldRenderer,
): EntityData[] {
  if (!entities) return [];
  const cs = renderer.currentCellSize;
  const ox = renderer.currentOffsetX;
  const oy = renderer.currentOffsetY;
  const result: EntityData[] = [];
  for (const e of entities.entities) {
    const ew = e.w ?? 1;
    const eh = e.h ?? 1;
    const sx = ox + e.x * cs;
    const sy = oy + (world.height - e.y - eh) * cs;
    if (mouseX >= sx && mouseX < sx + ew * cs && mouseY >= sy && mouseY < sy + eh * cs) result.push(e);
  }
  return result;
}

/** Renders a labelled mini bar: label [████░░░░] pct%. Color green→yellow→red by fill ratio. */
function miniBar(label: string, value: number, max: number, unit = ''): string {
  const pct   = max > 0 ? Math.max(0, Math.min(1, value / max)) : 0;
  const color = pct > 0.7 ? '#4cff91' : pct > 0.3 ? '#ffe033' : '#e94560';
  const fill  = `width:${(pct * 80).toFixed(1)}px;height:100%;background:${color};border-radius:3px`;
  const bar   = `<span style="display:inline-block;width:80px;height:7px;background:rgba(255,255,255,0.12);border-radius:3px;vertical-align:middle;overflow:hidden"><span style="${fill}"></span></span>`;
  const text  = unit === 'kcal'
    ? `${(value / 1000).toFixed(0)} / ${(max / 1000).toFixed(0)} kcal`
    : `${Math.round(pct * 100)}%`;
  return `${label} ${bar} <span style="color:#aaa;font-size:10px">${text}</span>`;
}

/** Builds the inner HTML for the hover tooltip — one block per entity, separated by a divider. */
function tooltipHtml(hits: EntityData[]): string {
  return hits.map(e => {
    const rows: string[] = [`<b>${e.name}</b> <span style="color:#aaa">[${e.type}]</span>`];
    if (e.smState   !== undefined) rows.push(`SM: ${e.smState ?? 'null'}`);
    if (e.currentChore)            rows.push(`Chore: ${e.currentChore}`);
    if (e.stamina   !== undefined && e.staminaMax  !== undefined)
      rows.push(miniBar('💤', e.stamina, e.staminaMax));
    if (e.calories  !== undefined && e.caloriesMax !== undefined)
      rows.push(miniBar('🍖', e.calories, e.caloriesMax, 'kcal'));
    if (e.navIsMoving !== undefined) rows.push(`Moving: ${e.navIsMoving}`);
    if (e.navCell !== undefined)   rows.push(`NavCell: ${e.navCell}`);
    rows.push(`<span style="color:#888">(${e.x}, ${e.y})&nbsp;${e.w ?? 1}×${e.h ?? 1}</span>`);
    return rows.join('<br>');
  }).join('<hr style="border:none;border-top:1px solid rgba(255,255,255,0.2);margin:4px 0">');
}

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
  const tooltipRef = useRef<HTMLDivElement>(null);
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

    const TARGET_MS = 1000 / 60; // 16.666ms — cap render rate to 60fps
    let lastFrameTime = 0;
    let rafId: number;
    function loop(now: number) {
      rafId = requestAnimationFrame(loop);
      if (now - lastFrameTime < TARGET_MS) return; // skip frame if too soon
      lastFrameTime = now;
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
    } else {
      const rect = canvasRef.current!.getBoundingClientRect();
      const mouseX = e.clientX - rect.left;
      const mouseY = e.clientY - rect.top;

      onCellHover(rendererRef.current.getCellAt(mouseX, mouseY, worldRef.current, entitiesRef.current));

      // Entity tooltip — direct DOM update, no React re-render.
      const tt = tooltipRef.current;
      if (tt) {
        const hits = getEntitiesAt(mouseX, mouseY, worldRef.current, entitiesRef.current, rendererRef.current);
        if (hits.length > 0) {
          tt.innerHTML = tooltipHtml(hits);
          tt.style.left = `${mouseX + 14}px`;
          tt.style.top  = `${mouseY + 14}px`;
          tt.style.display = 'block';
        } else {
          tt.style.display = 'none';
        }
      }
    }
  }, [onCellHover]);

  const handleMouseUp = useCallback(() => {
    isDragging.current = false;
  }, []);

  const handleMouseLeave = useCallback(() => {
    isDragging.current = false;
    onCellHover(null);
    if (tooltipRef.current) tooltipRef.current.style.display = 'none';
  }, [onCellHover]);

  return (
    <div style={{ position: 'relative', width: '100%', height: '100%' }}>
      <canvas
        ref={canvasRef}
        onWheel={handleWheel}
        onMouseDown={handleMouseDown}
        onMouseMove={handleMouseMove}
        onMouseUp={handleMouseUp}
        onMouseLeave={handleMouseLeave}
        style={{ display: 'block', width: '100%', height: '100%', cursor: 'crosshair' }}
      />
      <div
        ref={tooltipRef}
        style={{
          display: 'none',
          position: 'absolute',
          pointerEvents: 'none',
          background: 'rgba(0,0,0,0.82)',
          color: '#e8e8e8',
          font: '11px/1.5 monospace',
          padding: '4px 8px',
          borderRadius: '4px',
          whiteSpace: 'nowrap',
          maxHeight: '220px',
          overflowY: 'auto',
          zIndex: 10,
        }}
      />
    </div>
  );
}
