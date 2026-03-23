import { useCallback, useEffect, useRef } from 'react';
import type { WorldData, EntitiesResponse, OverlayMode, EntityData } from '../api/types';
import type { CellInfo } from '../renderer/WorldRenderer';
import { WorldRenderer } from '../renderer/WorldRenderer';
import { miniBar } from '../utils/miniBar';
import { shouldRefreshHover } from '../utils/hoverRefresh';


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
    const ew = e.w;
    const eh = e.h;
    if (!ew || !eh) continue; // skip entities with missing size
    const sx = ox + e.x * cs;
    const sy = oy + (world.height - e.y - eh) * cs;
    if (mouseX >= sx && mouseX < sx + ew * cs && mouseY >= sy && mouseY < sy + eh * cs) result.push(e);
  }
  return result;
}


/** Builds the inner HTML for the hover tooltip — one block per entity, separated by a divider. */
function tooltipHtml(hits: EntityData[], pinned = false): string {
  const pin = pinned ? '<div style="color:#aaa;font-size:9px;margin-bottom:3px">📌 pinned — click empty area or ESC to unpin</div>' : '';
  return pin + hits.map(e => {
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
  /** Entity pinned from the sidebar list — tooltip follows this entity on the canvas. */
  pinnedEntity?: EntityData | null;
  /** Called when a sidebar-pinned entity disappears from the entity list. */
  onEntityUnpinned?: () => void;
}

export function WorldCanvas({
  world, entities, overlay, showEntities, showGrid, serverUps,
  onCellHover, pinnedEntity, onEntityUnpinned,
}: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const tooltipRef = useRef<HTMLDivElement>(null);
  const rendererRef = useRef<WorldRenderer | null>(null);
  const isDragging = useRef(false);
  const hasDragged = useRef(false);
  const lastMouse = useRef({ x: 0, y: 0 });

  // Pinned tooltip state: isPinnedRef true = tooltip locked at pinnedPosRef coords.
  const isPinnedRef = useRef(false);
  const pinnedPosRef = useRef<{ mouseX: number; mouseY: number } | null>(null);

  // Hover tooltip state: tracks last known cursor position inside the canvas.
  // Updated on every mousemove; read by the rAF loop to auto-refresh tooltip content
  // without requiring the cursor to move again (fixes stale hover data).
  const hoverPosRef   = useRef<{ mouseX: number; mouseY: number } | null>(null);
  const isHoveringRef = useRef(false);

  // Sidebar-driven pin: entity selected from the EntityListPanel.
  // The rAF loop computes canvas coords from entity world coords each frame so
  // the tooltip follows the entity as it moves (or hides it if it disappears).
  const sidebarPinnedEntityRef = useRef<EntityData | null>(null);
  const onEntityUnpinnedRef    = useRef<(() => void) | undefined>(undefined);

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
  sidebarPinnedEntityRef.current = pinnedEntity ?? null;
  onEntityUnpinnedRef.current    = onEntityUnpinned;

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

  // Tooltip wheel: stop propagation so the canvas zoom handler doesn't fire,
  // but do NOT preventDefault — we want the browser to naturally scroll the tooltip div.
  useEffect(() => {
    const el = tooltipRef.current;
    if (!el) return;
    const handler = (e: WheelEvent) => { e.stopPropagation(); };
    el.addEventListener('wheel', handler, { passive: true });
    return () => el.removeEventListener('wheel', handler);
  }, []);

  // ESC key: unpin tooltip.
  useEffect(() => {
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && (isPinnedRef.current || sidebarPinnedEntityRef.current)) {
        isPinnedRef.current = false;
        pinnedPosRef.current = null;
        if (sidebarPinnedEntityRef.current) {
          sidebarPinnedEntityRef.current = null;
          onEntityUnpinnedRef.current?.();
        }
        const tt = tooltipRef.current;
        if (tt) { tt.style.display = 'none'; tt.style.outline = ''; }
      }
    };
    window.addEventListener('keydown', onKeyDown);
    return () => window.removeEventListener('keydown', onKeyDown);
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

        // Refresh pinned tooltip with latest entity data from entitiesRef (kept live by App fast poll).
        // Runs at 60fps — cost is O(n entities) hit-test, negligible for <1K entities.
        if (isPinnedRef.current && pinnedPosRef.current) {
          const tt = tooltipRef.current;
          if (tt) {
            const { mouseX, mouseY } = pinnedPosRef.current;
            const hits = getEntitiesAt(mouseX, mouseY, w, entitiesRef.current, r);
            if (hits.length > 0) {
              tt.innerHTML = tooltipHtml(hits, true);
            } else {
              // All entities left the pinned cell — auto-unpin.
              isPinnedRef.current = false;
              pinnedPosRef.current = null;
              tt.style.display = 'none';
              tt.style.outline = '';
            }
          }
        }

        // Sidebar-driven pin: find entity by name in live data, compute canvas position,
        // render tooltip there each frame so it tracks the entity as it moves.
        const sbEntity = sidebarPinnedEntityRef.current;
        if (sbEntity && !isPinnedRef.current) {
          const ent = entitiesRef.current?.entities.find(e => e.name === sbEntity.name) ?? null;
          const tt  = tooltipRef.current;
          if (ent && ent.w && ent.h) {
            const cs  = r.currentCellSize;
            const ttX = r.currentOffsetX + (ent.x + ent.w / 2) * cs;
            const ttY = r.currentOffsetY + (w.height - ent.y - ent.h) * cs - 8;
            if (tt) {
              tt.innerHTML = tooltipHtml([ent], true);
              tt.style.left    = `${ttX + 14}px`;
              tt.style.top     = `${Math.max(4, ttY)}px`;
              tt.style.display = 'block';
              tt.style.outline = '1px solid rgba(255,255,255,0.4)';
            }
          } else {
            // Entity disappeared from list — auto-unpin without re-triggering the prop.
            sidebarPinnedEntityRef.current = null;
            onEntityUnpinnedRef.current?.();
            if (tt) { tt.style.display = 'none'; tt.style.outline = ''; }
          }
        }

        // Refresh hover tooltip on every frame — fixes stale data when server pushes new
        // entity state (e.g. updated stamina/chore) while the cursor is stationary.
        // Only runs when hovering and not pinned (pinned has its own path above).
        if (shouldRefreshHover({ isPinned: isPinnedRef.current || !!sidebarPinnedEntityRef.current, isHovering: isHoveringRef.current })
            && hoverPosRef.current) {
          const tt = tooltipRef.current;
          if (tt) {
            const { mouseX, mouseY } = hoverPosRef.current;
            const hits = getEntitiesAt(mouseX, mouseY, w, entitiesRef.current, r);
            if (hits.length > 0) {
              tt.innerHTML = tooltipHtml(hits, false);
              tt.style.display = 'block';
            } else {
              tt.style.display = 'none';
            }
          }
        }
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

  // Canvas zoom: must be non-passive so preventDefault() actually suppresses page scroll.
  // Registered in the same useEffect as the resize observer to share the canvas ref check.
  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const handler = (e: WheelEvent) => {
      e.preventDefault();
      if (!rendererRef.current || !worldRef.current) return;
      const rect = canvas.getBoundingClientRect();
      rendererRef.current.zoom(e.deltaY > 0 ? -1 : 1, e.clientX - rect.left, e.clientY - rect.top);
    };
    canvas.addEventListener('wheel', handler, { passive: false });
    return () => canvas.removeEventListener('wheel', handler);
  }, []);

  const handleMouseDown = useCallback((e: React.MouseEvent) => {
    if (e.button === 0) {
      isDragging.current = true;
      hasDragged.current = false;
      lastMouse.current = { x: e.clientX, y: e.clientY };
    }
  }, []);

  const handleMouseMove = useCallback((e: React.MouseEvent) => {
    if (!rendererRef.current || !worldRef.current) return;

    if (isDragging.current) {
      const dx = e.clientX - lastMouse.current.x;
      const dy = e.clientY - lastMouse.current.y;
      lastMouse.current = { x: e.clientX, y: e.clientY };
      if (Math.abs(dx) > 1 || Math.abs(dy) > 1) {
        hasDragged.current = true;
        // Unpin on drag — viewport has shifted, pin position no longer meaningful.
        if (isPinnedRef.current) {
          isPinnedRef.current = false;
          pinnedPosRef.current = null;
          const tt = tooltipRef.current;
          if (tt) { tt.style.display = 'none'; tt.style.outline = ''; }
        }
      }
      rendererRef.current.pan(dx, dy);
    } else {
      const rect = canvasRef.current!.getBoundingClientRect();
      const mouseX = e.clientX - rect.left;
      const mouseY = e.clientY - rect.top;

      // Always track hover position — the rAF loop uses this to auto-refresh
      // tooltip content even when the cursor is stationary.
      isHoveringRef.current = true;
      hoverPosRef.current   = { mouseX, mouseY };

      // Cell hover for sidebar always updates.
      onCellHover(rendererRef.current.getCellAt(mouseX, mouseY, worldRef.current, entitiesRef.current));

      // Tooltip position update on move; content refresh happens in rAF loop.
      if (isPinnedRef.current) return;

      const tt = tooltipRef.current;
      if (tt) {
        tt.style.left = `${mouseX + 14}px`;
        tt.style.top  = `${mouseY + 14}px`;
      }
    }
  }, [onCellHover]);

  const handleMouseUp = useCallback((e: React.MouseEvent) => {
    if (e.button !== 0) return;
    const wasClick = !hasDragged.current;
    isDragging.current = false;
    hasDragged.current = false;

    if (wasClick && rendererRef.current && worldRef.current) {
      const rect = canvasRef.current!.getBoundingClientRect();
      const mouseX = e.clientX - rect.left;
      const mouseY = e.clientY - rect.top;
      const hits = getEntitiesAt(mouseX, mouseY, worldRef.current, entitiesRef.current, rendererRef.current);
      const tt = tooltipRef.current;
      if (hits.length > 0) {
        // Pin tooltip at click position; clear any sidebar pin (mutually exclusive).
        if (sidebarPinnedEntityRef.current) {
          sidebarPinnedEntityRef.current = null;
          onEntityUnpinnedRef.current?.();
        }
        isPinnedRef.current = true;
        pinnedPosRef.current = { mouseX, mouseY };
        if (tt) {
          tt.innerHTML = tooltipHtml(hits, true);
          tt.style.left = `${mouseX + 14}px`;
          tt.style.top  = `${mouseY + 14}px`;
          tt.style.display = 'block';
          tt.style.outline = '1px solid rgba(255,255,255,0.4)';
        }
      } else {
        // Click on empty area — unpin.
        isPinnedRef.current = false;
        pinnedPosRef.current = null;
        if (tt) { tt.style.display = 'none'; tt.style.outline = ''; }
      }
    }
  }, []);

  const handleMouseLeave = useCallback(() => {
    isDragging.current   = false;
    hasDragged.current   = false;
    isHoveringRef.current = false;
    hoverPosRef.current  = null;
    onCellHover(null);
    // Don't hide tooltip when pinned — it stays visible after cursor leaves canvas.
    if (!isPinnedRef.current && tooltipRef.current) {
      tooltipRef.current.style.display = 'none';
    }
  }, [onCellHover]);

  return (
    <div style={{ position: 'relative', width: '100%', height: '100%' }}>
      <canvas
        ref={canvasRef}
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
          pointerEvents: 'auto',
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
