import type { CellData, EntityData, OverlayMode, WorldData, EntitiesResponse } from '../api/types';
import { getElementColor, getElementName, ENTITY_COLORS } from './constants';

export interface CellInfo {
  x: number;
  y: number;
  element: string;
  elementId: number;
  temperature: number;
  temperatureC: number;
  mass: number;
  entities?: string[];
}

export interface RenderOptions {
  overlay: OverlayMode;
  showEntities: boolean;
  showGrid: boolean;
}

export class WorldRenderer {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private cellSize = 10;
  private offsetX = 0;
  private offsetY = 0;
  private centered = false;

  constructor(canvas: HTMLCanvasElement) {
    this.canvas = canvas;
    const ctx = canvas.getContext('2d');
    if (!ctx) throw new Error('Failed to get 2D context');
    this.ctx = ctx;
  }

  get currentCellSize() { return this.cellSize; }
  get currentOffsetX() { return this.offsetX; }
  get currentOffsetY() { return this.offsetY; }

  zoom(delta: number, mouseX: number, mouseY: number) {
    const oldSize = this.cellSize;
    this.cellSize = Math.max(2, Math.min(40, this.cellSize + delta));
    if (oldSize !== this.cellSize) {
      const scale = this.cellSize / oldSize;
      this.offsetX = mouseX - (mouseX - this.offsetX) * scale;
      this.offsetY = mouseY - (mouseY - this.offsetY) * scale;
    }
  }

  pan(dx: number, dy: number) {
    this.offsetX += dx;
    this.offsetY += dy;
  }

  setOffset(x: number, y: number) {
    this.offsetX = x;
    this.offsetY = y;
  }

  getCellAt(mouseX: number, mouseY: number, world: WorldData, entities?: EntitiesResponse | null): CellInfo | null {
    const cellX = Math.floor((mouseX - this.offsetX) / this.cellSize);
    const cellY = world.height - 1 - Math.floor((mouseY - this.offsetY) / this.cellSize);

    if (cellX < 0 || cellX >= world.width || cellY < 0 || cellY >= world.height) return null;

    const idx = cellY * world.width + cellX;
    const cell = world.cells[idx];
    const cellEntities = entities?.entities
      ?.filter(e => {
        const ew = e.w ?? 1;
        const eh = e.h ?? 1;
        return cellX >= e.x && cellX < e.x + ew && cellY >= e.y && cellY < e.y + eh;
      })
      .map(e => `${e.name} (${e.type}${(e.w ?? 1) > 1 || (e.h ?? 1) > 1 ? ` ${e.w}x${e.h}` : ''})`);
    return {
      x: cellX,
      y: cellY,
      element: getElementName(cell.element),
      elementId: cell.element,
      temperature: cell.temperature,
      temperatureC: parseFloat((cell.temperature - 273.15).toFixed(1)),
      mass: cell.mass,
      entities: cellEntities?.length ? cellEntities : undefined,
    };
  }

  render(world: WorldData, entities: EntitiesResponse | null, options: RenderOptions) {
    const { ctx, canvas, cellSize } = this;
    const w = canvas.width;
    const h = canvas.height;

    // Auto-center on first render
    if (!this.centered) {
      this.offsetX = (w - world.width * cellSize) / 2;
      this.offsetY = (h - world.height * cellSize) / 2;
      this.centered = true;
    }

    // Clear
    ctx.fillStyle = '#0a0a0a';
    ctx.fillRect(0, 0, w, h);

    // Render cells
    for (let y = 0; y < world.height; y++) {
      for (let x = 0; x < world.width; x++) {
        const idx = y * world.width + x;
        const cell = world.cells[idx];
        const screenX = this.offsetX + x * cellSize;
        const screenY = this.offsetY + (world.height - 1 - y) * cellSize;

        if (screenX + cellSize < 0 || screenX > w || screenY + cellSize < 0 || screenY > h) continue;

        ctx.fillStyle = this.getCellColor(cell, options.overlay);
        ctx.fillRect(Math.round(screenX), Math.round(screenY), Math.ceil(cellSize), Math.ceil(cellSize));
      }
    }

    // World boundary outline (always visible)
    ctx.strokeStyle = 'rgba(255,255,255,0.25)';
    ctx.lineWidth = 1.5;
    ctx.strokeRect(
      this.offsetX,
      this.offsetY,
      world.width * cellSize,
      world.height * cellSize
    );

    // Grid
    if (options.showGrid && cellSize >= 6) {
      ctx.strokeStyle = 'rgba(255,255,255,0.2)';
      ctx.lineWidth = 0.5;
      for (let x = 0; x <= world.width; x++) {
        const sx = this.offsetX + x * cellSize;
        ctx.beginPath();
        ctx.moveTo(sx, this.offsetY);
        ctx.lineTo(sx, this.offsetY + world.height * cellSize);
        ctx.stroke();
      }
      for (let y = 0; y <= world.height; y++) {
        const sy = this.offsetY + y * cellSize;
        ctx.beginPath();
        ctx.moveTo(this.offsetX, sy);
        ctx.lineTo(this.offsetX + world.width * cellSize, sy);
        ctx.stroke();
      }
    }

    // Entities
    if (options.showEntities && entities) {
      this.renderEntities(world, entities.entities);
    }
  }

  private getCellColor(cell: CellData, overlay: OverlayMode): string {
    switch (overlay) {
      case 'element':
        return getElementColor(cell.element);

      case 'temperature': {
        const t = cell.temperature;
        const ratio = Math.max(0, Math.min(1, (t - 200) / 200));
        if (ratio < 0.25) {
          const f = ratio / 0.25;
          return `rgb(0,${Math.round(f * 180)},${Math.round(180 + f * 75)})`;
        } else if (ratio < 0.5) {
          const f = (ratio - 0.25) / 0.25;
          return `rgb(0,${Math.round(180 + f * 75)},${Math.round(255 - f * 255)})`;
        } else if (ratio < 0.75) {
          const f = (ratio - 0.5) / 0.25;
          return `rgb(${Math.round(f * 255)},255,0)`;
        } else {
          const f = (ratio - 0.75) / 0.25;
          return `rgb(255,${Math.round(255 - f * 255)},0)`;
        }
      }

      case 'mass': {
        if (cell.mass <= 0) return '#0a0a0a';
        const logMass = Math.log10(cell.mass + 1);
        const ratio = Math.min(1, logMass / 3.5);
        const b = Math.round(40 + ratio * 200);
        return `rgb(${b},${Math.round(b * 0.7)},${Math.round(b * 0.4)})`;
      }

      default:
        return '#ff00ff';
    }
  }

  private renderEntities(world: WorldData, entities: EntityData[]) {
    const { ctx, cellSize } = this;

    for (const entity of entities) {
      const ew = entity.w ?? 1;
      const eh = entity.h ?? 1;
      const screenX = this.offsetX + entity.x * cellSize;
      const screenY = this.offsetY + (world.height - entity.y - eh) * cellSize;
      const color = ENTITY_COLORS[entity.type] ?? '#ffffff';

      if (screenX + ew * cellSize < 0 || screenX > this.canvas.width ||
          screenY + eh * cellSize < 0 || screenY > this.canvas.height) continue;

      if (entity.type === 'building' && ew > 1 || eh > 1) {
        // Buildings with size: filled rectangle with border
        ctx.fillStyle = color;
        ctx.globalAlpha = 0.4;
        ctx.fillRect(screenX, screenY, ew * cellSize, eh * cellSize);
        ctx.globalAlpha = 1;
        ctx.strokeStyle = color;
        ctx.lineWidth = 1;
        ctx.strokeRect(screenX, screenY, ew * cellSize, eh * cellSize);
      } else {
        // Small entities: dot at center
        const cx = screenX + ew * cellSize / 2;
        const cy = screenY + eh * cellSize / 2;
        const size = Math.max(2, cellSize * 0.3);
        ctx.fillStyle = color;
        ctx.fillRect(cx - size / 2, cy - size / 2, size, size);
      }
    }
  }
}
