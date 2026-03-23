import { describe, it, expect } from 'vitest';
import { minimapNeedsRedraw } from './minimapGate';
import type { MinimapViewport } from './minimapGate';

// ── helpers ───────────────────────────────────────────────────────────────────

function makeVP(overrides: Partial<MinimapViewport> = {}): MinimapViewport {
  return {
    offsetX:  0,
    offsetY:  0,
    cellSize: 10,
    canvasW:  800,
    canvasH:  600,
    worldW:   256,
    worldH:   128,
    ...overrides,
  };
}

const ENTITIES_A = { entities: [{ name: 'Duplicant-1', type: 'duplicant', x: 10, y: 20, w: 1, h: 2 }] };
const ENTITIES_B = { entities: [{ name: 'Duplicant-1', type: 'duplicant', x: 11, y: 20, w: 1, h: 2 }] };

// ── first draw (prevViewport === null) ────────────────────────────────────────

describe('minimapNeedsRedraw — first draw', () => {
  it('returns true when prevViewport is null (never drawn)', () => {
    expect(minimapNeedsRedraw(null, ENTITIES_A, null, makeVP())).toBe(true);
  });

  it('returns true on first draw even when entities is null', () => {
    expect(minimapNeedsRedraw(null, null, null, makeVP())).toBe(true);
  });
});

// ── stable: no redraw needed ──────────────────────────────────────────────────

describe('minimapNeedsRedraw — stable: returns false', () => {
  it('returns false when entities and viewport are identical', () => {
    const vp = makeVP();
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, vp, { ...vp })).toBe(false);
  });

  it('returns false when the exact same viewport object is passed', () => {
    const vp = makeVP();
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, vp, vp)).toBe(false);
  });

  it('returns false when entities is null/null and viewport is stable', () => {
    const vp = makeVP();
    expect(minimapNeedsRedraw(null, null, vp, { ...vp })).toBe(false);
  });

  it('returns false across many consecutive identical frames', () => {
    const vp = makeVP();
    for (let i = 0; i < 10; i++) {
      expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, vp, { ...vp })).toBe(false);
    }
  });
});

// ── entity reference change → true ───────────────────────────────────────────

describe('minimapNeedsRedraw — entity ref change', () => {
  it('returns true when entities reference changes', () => {
    const vp = makeVP();
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_B, vp, { ...vp })).toBe(true);
  });

  it('returns true when entities changes from null to non-null', () => {
    const vp = makeVP();
    expect(minimapNeedsRedraw(null, ENTITIES_A, vp, { ...vp })).toBe(true);
  });

  it('returns true when entities changes from non-null to null', () => {
    const vp = makeVP();
    expect(minimapNeedsRedraw(ENTITIES_A, null, vp, { ...vp })).toBe(true);
  });

  it('returns true for same-value entities in a NEW object reference', () => {
    const vp = makeVP();
    // Simulate a new poll arriving with the same data but a fresh JS object
    const fresh = { ...ENTITIES_A };
    expect(minimapNeedsRedraw(ENTITIES_A, fresh, vp, { ...vp })).toBe(true);
  });
});

// ── viewport change → true ────────────────────────────────────────────────────

describe('minimapNeedsRedraw — viewport change: offsetX', () => {
  it('returns true when offsetX changes (pan right)', () => {
    const base = makeVP({ offsetX: 0 });
    const panned = makeVP({ offsetX: -50 });
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, base, panned)).toBe(true);
  });
});

describe('minimapNeedsRedraw — viewport change: offsetY', () => {
  it('returns true when offsetY changes (pan down)', () => {
    const base = makeVP({ offsetY: 0 });
    const panned = makeVP({ offsetY: -80 });
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, base, panned)).toBe(true);
  });
});

describe('minimapNeedsRedraw — viewport change: cellSize', () => {
  it('returns true when cellSize changes (zoom in)', () => {
    const base = makeVP({ cellSize: 10 });
    const zoomed = makeVP({ cellSize: 14 });
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, base, zoomed)).toBe(true);
  });

  it('returns true when cellSize decreases (zoom out)', () => {
    const base = makeVP({ cellSize: 10 });
    const zoomed = makeVP({ cellSize: 6 });
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, base, zoomed)).toBe(true);
  });
});

describe('minimapNeedsRedraw — viewport change: canvas size', () => {
  it('returns true when canvasW changes (window resize)', () => {
    const base    = makeVP({ canvasW: 800 });
    const resized = makeVP({ canvasW: 1024 });
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, base, resized)).toBe(true);
  });

  it('returns true when canvasH changes (window resize)', () => {
    const base    = makeVP({ canvasH: 600 });
    const resized = makeVP({ canvasH: 768 });
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, base, resized)).toBe(true);
  });
});

describe('minimapNeedsRedraw — viewport change: world dimensions', () => {
  it('returns true when worldW changes', () => {
    const base    = makeVP({ worldW: 256 });
    const changed = makeVP({ worldW: 384 });
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, base, changed)).toBe(true);
  });

  it('returns true when worldH changes', () => {
    const base    = makeVP({ worldH: 128 });
    const changed = makeVP({ worldH: 192 });
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_A, base, changed)).toBe(true);
  });
});

// ── both change ───────────────────────────────────────────────────────────────

describe('minimapNeedsRedraw — both entities and viewport change', () => {
  it('returns true when both change simultaneously', () => {
    const base    = makeVP({ offsetX: 0 });
    const changed = makeVP({ offsetX: -50 });
    expect(minimapNeedsRedraw(ENTITIES_A, ENTITIES_B, base, changed)).toBe(true);
  });
});

// ── integration: simulated render loop ───────────────────────────────────────

describe('minimapNeedsRedraw — integration: simulated render loop', () => {
  it('redraws only when something changes across a sequence of frames', () => {
    const vp0 = makeVP({ offsetX: 0 });
    let redrawCount = 0;

    // Frame 0: first draw (prevViewport null)
    let prev: MinimapViewport | null = null;
    let prevE: unknown = null;
    const frames: Array<{ e: unknown; vp: MinimapViewport }> = [
      { e: ENTITIES_A, vp: makeVP({ offsetX:   0 }) }, // frame 0 — initial
      { e: ENTITIES_A, vp: makeVP({ offsetX:   0 }) }, // frame 1 — nothing changed
      { e: ENTITIES_A, vp: makeVP({ offsetX:   0 }) }, // frame 2 — nothing changed
      { e: ENTITIES_A, vp: makeVP({ offsetX: -50 }) }, // frame 3 — user panned
      { e: ENTITIES_A, vp: makeVP({ offsetX: -50 }) }, // frame 4 — nothing changed
      { e: ENTITIES_B, vp: makeVP({ offsetX: -50 }) }, // frame 5 — new poll data
      { e: ENTITIES_B, vp: makeVP({ offsetX: -50 }) }, // frame 6 — nothing changed
    ];

    for (const { e, vp } of frames) {
      if (minimapNeedsRedraw(prevE, e, prev, vp)) {
        redrawCount++;
        prev  = { ...vp };
        prevE = e;
      }
    }

    // Expected: frame 0 (first), frame 3 (pan), frame 5 (new entities) = 3
    expect(redrawCount).toBe(3);
  });

  it('counts exactly one redraw for 20 identical frames after first draw', () => {
    const vp = makeVP();
    let redrawCount = 0;
    let prev: MinimapViewport | null = null;
    let prevE: unknown = null;

    for (let i = 0; i < 20; i++) {
      if (minimapNeedsRedraw(prevE, ENTITIES_A, prev, { ...vp })) {
        redrawCount++;
        prev  = { ...vp };
        prevE = ENTITIES_A;
      }
    }
    expect(redrawCount).toBe(1); // only first frame
  });
});
