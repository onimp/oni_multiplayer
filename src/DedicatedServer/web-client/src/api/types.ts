export interface CellData {
  element: number;
  temperature: number;
  mass: number;
}

export interface WorldData {
  width: number;
  height: number;
  tick: number;
  cells: CellData[];
}

export interface EntityData {
  type: 'duplicant' | 'building';
  name: string;
  x: number;
  y: number;
  state: string;
}

export interface EntitiesResponse {
  tick: number;
  entities: EntityData[];
}

export interface GameState {
  tick: number;
  cycle: number;
  speed: number;
  paused: boolean;
  worldWidth: number;
  worldHeight: number;
  duplicantCount: number;
  buildingCount: number;
}

export type OverlayMode = 'element' | 'temperature' | 'mass';
