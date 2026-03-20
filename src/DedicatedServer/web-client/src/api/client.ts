import type { WorldData, EntitiesResponse, GameState, ElementsResponse } from './types';

const BASE_URL = '/api';

async function fetchJson<T>(path: string): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`);
  if (!res.ok) throw new Error(`API error: ${res.status} ${res.statusText}`);
  return res.json();
}

export async function fetchWorld(): Promise<WorldData> {
  return fetchJson<WorldData>('/world');
}

export async function fetchEntities(): Promise<EntitiesResponse> {
  return fetchJson<EntitiesResponse>('/entities');
}

export async function fetchGameState(): Promise<GameState> {
  return fetchJson<GameState>('/state');
}

export async function fetchElements(): Promise<ElementsResponse> {
  return fetchJson<ElementsResponse>('/elements');
}

export async function fetchAll(): Promise<{
  world: WorldData;
  entities: EntitiesResponse;
  state: GameState;
}> {
  const [world, entities, state] = await Promise.all([
    fetchWorld(),
    fetchEntities(),
    fetchGameState(),
  ]);
  return { world, entities, state };
}
