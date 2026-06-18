import type { GameState } from '../api/types';

interface Props {
  connected: boolean;
  gameState: GameState | null;
}

export function Header({ connected, gameState }: Props) {
  return (
    <header>
      <h1>ONI World Visualizer</h1>
      <div className="status">
        <span className={connected ? 'status-connected' : 'status-disconnected'}>
          {connected ? 'Connected' : 'Disconnected'}
        </span>
        {gameState && (
          <span className="game-info">
            Cycle {gameState.cycle} | Tick {gameState.tick} |{' '}
            {gameState.duplicantCount} dupes | {gameState.buildingCount} buildings
          </span>
        )}
      </div>
    </header>
  );
}
