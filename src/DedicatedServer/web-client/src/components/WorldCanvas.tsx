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
  onCellHover: (info: CellInfo | null) => void;
}

export function WorldCanvas({ world, entities, overlay, showEntities, showGrid, onCellHover }: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const rendererRef = useRef<WorldRenderer | null>(null);
  const isDragging = useRef(false);
  const lastMouse = useRef({ x: 0, y: 0 });

  // Initialize renderer
  useEffect(() => {
    if (!canvasRef.current) return;
    rendererRef.current = new WorldRenderer(canvasRef.current);
  }, []);

  // Resize canvas
  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const resizeObserver = new ResizeObserver(() => {
      const parent = canvas.parentElement;
      if (!parent) return;
      canvas.width = parent.clientWidth;
      canvas.height = parent.clientHeight;
      if (rendererRef.current && world) {
        rendererRef.current.render(world, entities, { overlay, showEntities, showGrid });
      }
    });

    resizeObserver.observe(canvas.parentElement!);
    return () => resizeObserver.disconnect();
  }, [world, entities, overlay, showEntities, showGrid]);

  // Render when data changes
  useEffect(() => {
    if (!rendererRef.current || !world) return;
    rendererRef.current.render(world, entities, { overlay, showEntities, showGrid });
  }, [world, entities, overlay, showEntities, showGrid]);

  const handleWheel = useCallback((e: React.WheelEvent) => {
    e.preventDefault();
    if (!rendererRef.current || !world) return;
    const rect = canvasRef.current!.getBoundingClientRect();
    const mouseX = e.clientX - rect.left;
    const mouseY = e.clientY - rect.top;
    rendererRef.current.zoom(e.deltaY > 0 ? -1 : 1, mouseX, mouseY);
    rendererRef.current.render(world, entities, { overlay, showEntities, showGrid });
  }, [world, entities, overlay, showEntities, showGrid]);

  const handleMouseDown = useCallback((e: React.MouseEvent) => {
    if (e.button === 0) {
      isDragging.current = true;
      lastMouse.current = { x: e.clientX, y: e.clientY };
    }
  }, []);

  const handleMouseMove = useCallback((e: React.MouseEvent) => {
    if (!rendererRef.current || !world) return;

    if (isDragging.current) {
      const dx = e.clientX - lastMouse.current.x;
      const dy = e.clientY - lastMouse.current.y;
      lastMouse.current = { x: e.clientX, y: e.clientY };
      rendererRef.current.pan(dx, dy);
      rendererRef.current.render(world, entities, { overlay, showEntities, showGrid });
    } else {
      const rect = canvasRef.current!.getBoundingClientRect();
      const mouseX = e.clientX - rect.left;
      const mouseY = e.clientY - rect.top;
      const info = rendererRef.current.getCellAt(mouseX, mouseY, world, entities);
      onCellHover(info);
    }
  }, [world, entities, overlay, showEntities, showGrid, onCellHover]);

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
