import type { GameState } from '../api/types';
import { formatStats } from '../utils/formatStats';

interface Props {
  gameState: GameState | null;
}

/**
 * Compact single-line stats bar shown between the header and the canvas.
 * Data comes from the existing fast-poll GameState (updated every render
 * cycle via App's setInterval) — no new endpoint required.
 *
 * Fields:
 *   Tick        — current game tick
 *   Entities    — total entity count (dupes + critters + buildings + …)
 *   Boot errors — errors captured during server startup; highlighted red
 *                 when > 0; shows '—' when the backend does not send the field
 */
export function StatsBar({ gameState }: Props) {
  const { tick, entities, bootErrors } = formatStats(gameState);

  // Highlight boot errors in red when the count is a real non-zero number.
  const bootErrorAlert =
    gameState?.bootErrorCount != null && gameState.bootErrorCount > 0;

  return (
    <div className="stats-bar" role="status" aria-label="Server stats">
      <span className="stats-item">
        Tick: <strong>{tick}</strong>
      </span>
      <span className="stats-sep" aria-hidden="true">|</span>
      <span className="stats-item">
        Entities: <strong>{entities}</strong>
      </span>
      <span className="stats-sep" aria-hidden="true">|</span>
      <span className={`stats-item${bootErrorAlert ? ' stats-error' : ''}`}>
        Boot errors:{' '}
        <strong>{bootErrors}</strong>
      </span>
    </div>
  );
}
