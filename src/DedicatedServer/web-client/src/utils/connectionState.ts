/**
 * Connection status derivation — pure, framework-free, fully testable.
 *
 * The visualizer has three distinct connection phases:
 *
 *   connecting   — no successful fetch has ever completed; we are waiting for
 *                  the first response (may also be here during initial backoff
 *                  retries before any data arrives).
 *
 *   connected    — at least one fetch succeeded and the latest fetch succeeded;
 *                  canvas overlay is hidden.
 *
 *   disconnected — we previously had data but the most recent fetch failed;
 *                  show an error overlay with optional retry countdown and a
 *                  manual reconnect button.
 *
 * Keeping the derivation here (rather than scattered across components) makes
 * it trivial to unit-test all state transitions without any React/DOM involved.
 */

export type ConnectionPhase = 'connecting' | 'connected' | 'disconnected';

export interface ConnectionStatus {
  /** Current phase of the connection lifecycle. */
  phase: ConnectionPhase;
  /**
   * Seconds until the next automatic retry attempt, or null when not
   * currently in a backoff countdown.
   */
  retryIn: number | null;
}

export interface DeriveConnectionStatusOpts {
  /** True when the most recent fetch returned successfully. */
  connected: boolean;
  /**
   * True once the server has returned world data at least once this session.
   * Used to distinguish "never connected" (connecting) from "lost connection"
   * (disconnected).
   */
  hasData: boolean;
  /** Seconds until next retry, forwarded from the backoff countdown. */
  retryIn: number | null;
}

/**
 * Derives the current ConnectionStatus from raw App state.
 *
 * Priority:
 *   1. connected=true  → always 'connected', retryIn is cleared.
 *   2. !hasData        → 'connecting' (waiting for first ever response).
 *   3. hasData         → 'disconnected' (lost connection after having it).
 */
export function deriveConnectionStatus(
  opts: DeriveConnectionStatusOpts,
): ConnectionStatus {
  if (opts.connected) {
    return { phase: 'connected', retryIn: null };
  }
  if (!opts.hasData) {
    return { phase: 'connecting', retryIn: opts.retryIn };
  }
  return { phase: 'disconnected', retryIn: opts.retryIn };
}
