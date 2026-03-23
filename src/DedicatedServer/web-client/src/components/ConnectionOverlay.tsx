import type { ConnectionStatus } from '../utils/connectionState';

interface Props {
  status: ConnectionStatus;
  onReconnect: () => void;
}

/**
 * Full-canvas overlay shown while connecting or when disconnected.
 * Renders nothing when phase === 'connected' — auto-dismisses on reconnect.
 *
 * Must be placed inside a `position: relative` container (canvas-container).
 */
export function ConnectionOverlay({ status, onReconnect }: Props) {
  const { phase, retryIn } = status;

  if (phase === 'connected') return null;

  const isConnecting = phase === 'connecting';

  return (
    <div className={`conn-overlay ${phase}`} role="status" aria-live="polite">
      {isConnecting ? (
        <>
          <div className="conn-spinner" aria-hidden="true" />
          <div className="conn-title">Connecting to server…</div>
          {retryIn != null && (
            <div className="conn-subtitle">Retrying in {retryIn}s</div>
          )}
        </>
      ) : (
        <>
          <div className="conn-icon" aria-hidden="true">⚠</div>
          <div className="conn-title">Connection lost</div>
          <div className="conn-subtitle">
            {retryIn != null
              ? `Reconnecting in ${retryIn}s…`
              : 'Server unreachable'}
          </div>
          <button className="conn-btn" onClick={onReconnect}>
            Reconnect now
          </button>
        </>
      )}
    </div>
  );
}
