export interface WorldData {
  width: number;
  height: number;
  tick: number;
  /** Flat arrays: index = y * width + x */
  e: number[];   // element ids
  t: number[];   // temperatures (Kelvin)
  m: number[];   // masses
}

export interface EntityData {
  type: 'duplicant' | 'critter' | 'building' | 'entity' | 'pickupable' | 'ore';
  name: string;
  x: number;
  y: number;
  w?: number;
  h?: number;
  state?: string;
  // Duplicant-specific fields (server adds these; may be absent for other types)
  currentChore?: string;
  smState?: string | null;
  navIsMoving?: boolean;
  navCell?: number;
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
  entityCount: number;
  serverUps?: number;
}

export type OverlayMode = 'element' | 'temperature' | 'mass';

export interface ElementInfo {
  id: number;
  name: string;
  state: string;
}

export interface ElementsResponse {
  elements: ElementInfo[];
}
