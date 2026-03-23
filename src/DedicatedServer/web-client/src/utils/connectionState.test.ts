import { describe, it, expect } from 'vitest';
import { deriveConnectionStatus } from './connectionState';
import type { ConnectionStatus } from './connectionState';

/**
 * Unit tests for deriveConnectionStatus().
 *
 * Covers the three overlay phases the task requires:
 *   • 'connecting'    — initial state, never received data yet
 *   • 'disconnected'  — had data, lost connection (error state)
 *   • 'connected'     — at least one fetch succeeded, overlay hidden
 *
 * All tests are pure — no React, no DOM, no timers.
 */
describe('deriveConnectionStatus', () => {

  // ── connecting phase ──────────────────────────────────────────────────────

  it('connecting: initial state — app just mounted, no fetch has succeeded', () => {
    const s = deriveConnectionStatus({ connected: false, hasData: false, retryIn: null });
    expect(s.phase).toBe('connecting');
    expect(s.retryIn).toBeNull();
  });

  it('connecting: still waiting during first-attempt backoff (retryIn forwarded)', () => {
    const s = deriveConnectionStatus({ connected: false, hasData: false, retryIn: 3 });
    expect(s.phase).toBe('connecting');
    expect(s.retryIn).toBe(3);
  });

  it('connecting: retryIn=0 still shows connecting phase (countdown finished, fetch pending)', () => {
    const s = deriveConnectionStatus({ connected: false, hasData: false, retryIn: 0 });
    expect(s.phase).toBe('connecting');
  });

  // ── connected phase ───────────────────────────────────────────────────────

  it('connected: successful first connection', () => {
    const s = deriveConnectionStatus({ connected: true, hasData: true, retryIn: null });
    expect(s.phase).toBe('connected');
    expect(s.retryIn).toBeNull();
  });

  it('connected: retryIn is always null when phase is connected (stale value ignored)', () => {
    // connected=true always wins, even if caller forgot to clear retryIn
    const s = deriveConnectionStatus({ connected: true, hasData: true, retryIn: 5 });
    expect(s.phase).toBe('connected');
    expect(s.retryIn).toBeNull();
  });

  it('connected: hasData=false is fine if connected=true (edge — elements loaded, world pending)', () => {
    const s = deriveConnectionStatus({ connected: true, hasData: false, retryIn: null });
    expect(s.phase).toBe('connected');
  });

  // ── disconnected phase ────────────────────────────────────────────────────

  it('disconnected: had data, fetch now failing, no retry scheduled', () => {
    const s = deriveConnectionStatus({ connected: false, hasData: true, retryIn: null });
    expect(s.phase).toBe('disconnected');
    expect(s.retryIn).toBeNull();
  });

  it('disconnected: had data, fetch failing, retry countdown active', () => {
    const s = deriveConnectionStatus({ connected: false, hasData: true, retryIn: 8 });
    expect(s.phase).toBe('disconnected');
    expect(s.retryIn).toBe(8);
  });

  it('disconnected: retryIn counts down correctly across multiple ticks', () => {
    const ticks: ConnectionStatus[] = [5, 4, 3, 2, 1].map(t =>
      deriveConnectionStatus({ connected: false, hasData: true, retryIn: t }),
    );
    ticks.forEach(s => expect(s.phase).toBe('disconnected'));
    expect(ticks.map(s => s.retryIn)).toEqual([5, 4, 3, 2, 1]);
  });

  // ── state transitions (the "reconnected" requirement) ────────────────────

  it('reconnected: transitions from connecting → connected (first successful fetch)', () => {
    const before = deriveConnectionStatus({ connected: false, hasData: false, retryIn: null });
    const after  = deriveConnectionStatus({ connected: true,  hasData: true,  retryIn: null });
    expect(before.phase).toBe('connecting');
    expect(after.phase).toBe('connected');   // overlay auto-dismissed
  });

  it('reconnected: transitions from disconnected → connected (server came back)', () => {
    const before = deriveConnectionStatus({ connected: false, hasData: true,  retryIn: 2 });
    const after  = deriveConnectionStatus({ connected: true,  hasData: true,  retryIn: null });
    expect(before.phase).toBe('disconnected');
    expect(after.phase).toBe('connected');   // overlay auto-dismissed
    expect(after.retryIn).toBeNull();        // countdown cleared
  });

  it('reconnected: a second disconnect after reconnect goes back to disconnected', () => {
    const connected   = deriveConnectionStatus({ connected: true,  hasData: true, retryIn: null });
    const lostAgain   = deriveConnectionStatus({ connected: false, hasData: true, retryIn: 4  });
    expect(connected.phase).toBe('connected');
    expect(lostAgain.phase).toBe('disconnected');
    expect(lostAgain.retryIn).toBe(4);
  });
});
