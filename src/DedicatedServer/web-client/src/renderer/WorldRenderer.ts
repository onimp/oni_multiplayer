import type { EntityData, OverlayMode, WorldData, EntitiesResponse } from '../api/types';
import { getElementColor, getElementName, getElementState, ENTITY_COLORS } from './constants';
import { tempOverlayColor } from '../utils/tempOverlay';
import { gasOverlayColor, GAS_BACKGROUND } from '../utils/gasOverlay';
import { liquidOverlayColor, LIQUID_BACKGROUND } from '../utils/liquidOverlay';
import { buildingToRect, BUILDING_LABEL_MIN_CELL_SIZE } from '../utils/buildingRenderer';
import type { BuildingRect } from '../utils/buildingRenderer';

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
    const elementId = world.e[idx] ?? 0;
    const temperature = world.t[idx] ?? 0;
    const mass = world.m[idx] ?? 0;
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
      element: getElementName(elementId),
      elementId,
      temperature,
      temperatureC: parseFloat((temperature - 273.15).toFixed(1)),
      mass,
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
        const screenX = this.offsetX + x * cellSize;
        const screenY = this.offsetY + (world.height - 1 - y) * cellSize;

        if (screenX + cellSize < 0 || screenX > w || screenY + cellSize < 0 || screenY > h) continue;

        ctx.fillStyle = this.getCellColor(world.e[idx] ?? 0, world.t[idx] ?? 0, world.m[idx] ?? 0, options.overlay);
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

  private getCellColor(elementId: number, temperature: number, mass: number, overlay: OverlayMode): string {
    switch (overlay) {
      case 'element':
        return getElementColor(elementId);

      case 'temperature':
        // Delegate to ONI-accurate HSV formula via tempOverlay adapter.
        return tempOverlayColor(temperature);

      case 'mass': {
        if (mass <= 0) return '#0a0a0a';
        const logMass = Math.log10(mass + 1);
        const ratio = Math.min(1, logMass / 3.5);
        const b = Math.round(40 + ratio * 200);
        return `rgb(${b},${Math.round(b * 0.7)},${Math.round(b * 0.4)})`;
      }

      case 'gas': {
        // Only Gas-state cells get a color; solids, liquids, vacuum → dark background.
        if (getElementState(elementId) !== 'Gas') return GAS_BACKGROUND;
        return gasOverlayColor(getElementName(elementId), mass);
      }

      case 'liquid': {
        // Only Liquid-state cells get a color; solids, gases, vacuum → dark background.
        if (getElementState(elementId) !== 'Liquid') return LIQUID_BACKGROUND;
        return liquidOverlayColor(getElementName(elementId), mass);
      }

      default:
        return '#ff00ff';
    }
  }

  private renderEntities(world: WorldData, entities: EntityData[]) {
    const { ctx, cellSize } = this;
    const cw = this.canvas.width;
    const ch = this.canvas.height;

    // Collect visible entities by render category.
    // PillInfo covers both duplicants and critters: rx=ry gives a circle, rx≠ry gives a pill.
    type PillInfo = { cx: number; cy: number; rx: number; ry: number };
    // StateLabel: small text drawn below the marker (smState/currentChore for dupes, name for critters).
    type StateLabel = { cx: number; topY: number; text: string };

    const dupes:    PillInfo[] = [];
    const critters: PillInfo[] = [];
    const buildings: BuildingRect[] = [];                                // per-name colored rects
    const rects: Array<[number, number, number, number, string]> = [];  // [sx, sy, pw, ph, color]
    const stateLabels: StateLabel[] = [];


    for (const entity of entities) {
      // w/h from server reflect the real cell footprint (duplicant=1×2, Drecko=1×2, etc.)
      // Skip entities with missing or zero size — server must always send valid dimensions.
      const ew = entity.w;
      const eh = entity.h;
      if (!ew || !eh) {
        console.error('Entity missing size:', entity.name, entity);
        continue;
      }

      // Canvas top-left of bounding box (Y-axis inverted: ONI y=0 is world bottom)
      const sx = this.offsetX + entity.x * cellSize;
      const sy = this.offsetY + (world.height - entity.y - eh) * cellSize;

      // Frustum cull
      if (sx + ew * cellSize < 0 || sx > cw || sy + eh * cellSize < 0 || sy > ch) continue;

      // Canvas center of footprint
      const cx = this.offsetX + (entity.x + ew / 2) * cellSize;
      const cy = this.offsetY + (world.height - entity.y - eh / 2) * cellSize;

      if (entity.type === 'duplicant') {
        // Pill using actual w/h from server — rx/ry leave ~12% padding inside each cell
        const rx = ew * cellSize * 0.44;
        const ry = eh * cellSize * 0.44;
        dupes.push({ cx, cy, rx, ry });
        // State label: smState takes priority, fall back to currentChore
        const stateText = entity.smState ?? entity.currentChore;
        if (stateText && cellSize >= 6) stateLabels.push({ cx, topY: cy + ry + 2, text: stateText });
      } else if (entity.type === 'critter') {
        // Same pill logic: 1×1 → circle (rx=ry), 1×2 Drecko → tall pill (ry > rx)
        const rx = ew * cellSize * 0.44;
        const ry = eh * cellSize * 0.44;
        critters.push({ cx, cy, rx, ry });
        // Show critter name when zoomed in enough (too noisy at small zoom)
        if (cellSize >= 10) stateLabels.push({ cx, topY: cy + ry + 2, text: entity.name });
      } else if (entity.type === 'building') {
        // Buildings: per-name color via buildingRenderer (distinct per building type).
        // Rendered before dupes/critters so they form a background layer.
        buildings.push(buildingToRect({ name: entity.name, x: entity.x, y: entity.y, w: ew, h: eh }, world.height, this.offsetX, this.offsetY, cellSize));
      } else {
        // Ores, pickupables, generic entities → flat type color rect
        const color = ENTITY_COLORS[entity.type] ?? '#ffffff';
        rects.push([sx, sy, ew * cellSize, eh * cellSize, color]);
      }
    }

    // --- Buildings (layer behind dupes/critters) — per-name color + border + label ---
    for (const b of buildings) {
      ctx.globalAlpha = 0.45;
      ctx.fillStyle = b.color;
      ctx.fillRect(b.sx, b.sy, b.pw, b.ph);
      ctx.globalAlpha = 1;
      ctx.strokeStyle = b.color;
      ctx.lineWidth = 1;
      ctx.strokeRect(b.sx + 0.5, b.sy + 0.5, b.pw - 1, b.ph - 1);
    }
    // Building name labels (only when cellSize >= BUILDING_LABEL_MIN_CELL_SIZE)
    if (cellSize >= BUILDING_LABEL_MIN_CELL_SIZE && buildings.length > 0) {
      const fontSize = Math.max(7, Math.round(cellSize * 0.45));
      ctx.font = `${fontSize}px monospace`;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      for (const b of buildings) {
        if (!b.label) continue;
        const cx = b.sx + b.pw / 2;
        const cy = b.sy + b.ph / 2;
        // Truncate name to fit within the building's pixel width
        let text = b.label;
        while (text.length > 1 && ctx.measureText(text).width > b.pw - 4) {
          text = text.slice(0, text.length - 4) + '…';
        }
        ctx.fillStyle = 'rgba(0,0,0,0.75)';
        ctx.fillText(text, cx + 1, cy + 1);
        ctx.fillStyle = '#fff';
        ctx.fillText(text, cx, cy);
      }
    }

    // --- Rects (ores, pickupables, generic entities) ---
    for (const [x, y, w, h, color] of rects) {
      ctx.fillStyle = color;
      ctx.globalAlpha = 0.4;
      ctx.fillRect(x, y, w, h);
      ctx.globalAlpha = 1;
      ctx.strokeStyle = color;
      ctx.lineWidth = 1;
      ctx.strokeRect(x, y, w, h);
    }

    // Helper: batch-draw pills for one type (shadow + fill in two passes)
    const drawPills = (pills: PillInfo[], color: string) => {
      if (pills.length === 0) return;
      const shadow = 1.5;
      ctx.fillStyle = 'rgba(0,0,0,0.45)';
      ctx.beginPath();
      for (const p of pills)
        ctx.roundRect(p.cx - p.rx - shadow, p.cy - p.ry - shadow,
                      (p.rx + shadow) * 2, (p.ry + shadow) * 2, p.rx + shadow);
      ctx.fill();

      ctx.fillStyle = color;
      ctx.beginPath();
      for (const p of pills)
        ctx.roundRect(p.cx - p.rx, p.cy - p.ry, p.rx * 2, p.ry * 2, p.rx);
      ctx.fill();
    };

    drawPills(dupes,    ENTITY_COLORS['duplicant'] ?? '#ffe033');
    drawPills(critters, ENTITY_COLORS['critter']   ?? '#4cff91');

    // --- D/C labels inside markers (only when zoomed in) ---
    if (cellSize >= 8 && (dupes.length > 0 || critters.length > 0)) {
      const fontSize = Math.max(6, Math.round(cellSize * 0.65));
      ctx.font = `bold ${fontSize}px monospace`;
      ctx.fillStyle = '#000';
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      for (const d of dupes)    ctx.fillText('D', d.cx, d.cy);
      for (const c of critters) ctx.fillText('C', c.cx, c.cy);
    }

    // --- State / name labels below markers ---
    if (stateLabels.length > 0) {
      const fontSize = Math.max(7, Math.round(cellSize * 0.5));
      ctx.font = `${fontSize}px monospace`;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'top';
      // Shadow pass (1px offset, dark) for readability on any background
      ctx.fillStyle = 'rgba(0,0,0,0.85)';
      for (const { cx, topY, text } of stateLabels) ctx.fillText(text, cx + 1, topY + 1);
      // Text pass (white)
      ctx.fillStyle = '#ffffff';
      for (const { cx, topY, text } of stateLabels) ctx.fillText(text, cx, topY);
    }

  }

  /** Draws a small UPS counter overlay in the top-right corner of the canvas. */
  renderUpsOverlay(serverUps: number | undefined, clientUps: number) {
    const { ctx, canvas } = this;
    const text = `Server: ${serverUps ?? '--'} UPS | Client: ${clientUps} UPS`;
    const hPad = 8;
    const vPad = 5;
    ctx.font = 'bold 12px monospace';
    const textW = ctx.measureText(text).width;
    const boxW = textW + hPad * 2;
    const boxH = 12 + vPad * 2;
    const bx = canvas.width - boxW - 6;
    const by = 6;
    ctx.fillStyle = 'rgba(0,0,0,0.6)';
    ctx.fillRect(bx, by, boxW, boxH);
    ctx.fillStyle = '#e0e0e0';
    ctx.textAlign = 'left';
    ctx.textBaseline = 'top';
    ctx.fillText(text, bx + hPad, by + vPad);
  }
}
