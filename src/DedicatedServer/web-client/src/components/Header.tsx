import type { GameState } from '../api/types';

interface Props {
  connected: boolean;
  retryIn: number | null;
  gameState: GameState | null;
}

export function Header({ connected, retryIn, gameState }: Props) {
  const cycleProgress = gameState?.cycleTime != null ? gameState.cycleTime / 600 : null;

  return (
    <header>
      <h1>ONI World Visualizer</h1>
      <div className="status">
        <span className={connected ? 'status-connected' : 'status-disconnected'}>
          {connected
            ? 'Connected'
            : retryIn != null ? `Disconnected — retry in ${retryIn}s` : 'Disconnected'}
        </span>
        {gameState && (
          <span className="game-info">
            Cycle {gameState.cycle}
            {cycleProgress !== null && (
              <span className="cycle-indicator" title={`${gameState.cycleTime?.toFixed(0)}s / 600s`}>
                <span className="cycle-icon">{gameState.isNight ? '🌙' : '☀️'}</span>
                <span className="cycle-bar">
                  <span
                    className={`cycle-bar-fill ${gameState.isNight ? 'night' : 'day'}`}
                    style={{ width: `${(cycleProgress * 100).toFixed(1)}%` }}
                  />
                </span>
                <span className="cycle-pct">{Math.round(cycleProgress * 100)}%</span>
              </span>
            )}
            {' '}| Tick {gameState.tick} |{' '}
            {gameState.duplicantCount} dupes | {gameState.buildingCount} buildings | {gameState.entityCount} entities
          </span>
        )}
      </div>
    </header>
  );
}
