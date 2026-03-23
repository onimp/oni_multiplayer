import { describe, it, expect } from 'vitest';
import {
  createRenderGate,
  needsRender,
  recordRender,
  markDirty,
} from './renderGate';
import type { RenderSnapshot } from './renderGate';

// ── helpers ───────────────────────────────────────────────────────────────────

function makeSnap(overrides: Partial<RenderSnapshot> = {}): RenderSnapshot {
  return {
    entities:     {},
    world:        {},
    overlay:      'element',
    showEntities: true,
    showGrid:     false,
    showMinimap:  true,
    serverUps:    60,
    ...overrides,
  };
}

// ── createRenderGate ──────────────────────────────────────────────────────────

describe('createRenderGate', () => {
  it('starts with dirty=true so the very first frame always draws', () => {
    const gate = createRenderGate();
    expect(gate.dirty).toBe(true);
  });

  it('starts with lastSnapshot=null', () => {
    const gate = createRenderGate();
    expect(gate.lastSnapshot).toBeNull();
  });
});

// ── needsRender — initial state ───────────────────────────────────────────────

describe('needsRender — initial / dirty', () => {
  it('returns true on a fresh gate (dirty=true, no snapshot)', () => {
    const gate = createRenderGate();
    expect(needsRender(gate, makeSnap())).toBe(true);
  });

  it('returns true when gate has no snapshot even if dirty=false', () => {
    const gate = createRenderGate();
    gate.dirty = false; // force clear (but lastSnapshot is still null)
    expect(needsRender(gate, makeSnap())).toBe(true);
  });

  it('returns true after markDirty even with a recorded snapshot', () => {
    const gate = createRenderGate();
    const snap = makeSnap();
    recordRender(gate, snap);
    markDirty(gate);
    expect(needsRender(gate, snap)).toBe(true); // same snapshot, but dirty
  });
});

// ── needsRender — skip when nothing changed ───────────────────────────────────

describe('needsRender — skips redraw when data unchanged', () => {
  it('returns false on the frame immediately after recordRender with same snapshot', () => {
    const gate = createRenderGate();
    const snap = makeSnap();
    recordRender(gate, snap);
    // Same snapshot object AND same values — no change
    expect(needsRender(gate, snap)).toBe(false);
  });

  it('returns false for multiple consecutive frames with identical snapshot', () => {
    const gate = createRenderGate();
    const snap = makeSnap();
    recordRender(gate, snap);
    // Simulate 5 rAF frames with no data change
    for (let i = 0; i < 5; i++) {
      expect(needsRender(gate, snap)).toBe(false);
    }
  });

  it('returns false when a new snapshot object is created with identical values', () => {
    const gate = createRenderGate();
    const entities = { entities: [] };
    const world    = { width: 256, height: 128 };
    const snap1 = makeSnap({ entities, world });
    recordRender(gate, snap1);

    // New snapshot object but same references for entities/world
    const snap2 = makeSnap({ entities, world });
    expect(needsRender(gate, snap2)).toBe(false);
  });
});

// ── needsRender — triggers on each data change type ──────────────────────────

describe('needsRender — triggers on data changes', () => {
  it('returns true when entities reference changes', () => {
    const gate = createRenderGate();
    const snap = makeSnap({ entities: { entities: [] } });
    recordRender(gate, snap);

    const newSnap = { ...snap, entities: { entities: [] } }; // new object
    expect(needsRender(gate, newSnap)).toBe(true);
  });

  it('returns true when world reference changes', () => {
    const gate = createRenderGate();
    const snap = makeSnap({ world: { width: 256, height: 128, e: [], t: [], m: [] } });
    recordRender(gate, snap);

    const newSnap = { ...snap, world: { width: 256, height: 128, e: [], t: [], m: [] } };
    expect(needsRender(gate, newSnap)).toBe(true);
  });

  it('returns true when overlay changes', () => {
    const gate = createRenderGate();
    const snap = makeSnap({ overlay: 'element' });
    recordRender(gate, snap);
    expect(needsRender(gate, { ...snap, overlay: 'temperature' })).toBe(true);
  });

  it('returns true when showEntities toggles', () => {
    const gate = createRenderGate();
    const snap = makeSnap({ showEntities: true });
    recordRender(gate, snap);
    expect(needsRender(gate, { ...snap, showEntities: false })).toBe(true);
  });

  it('returns true when showGrid toggles', () => {
    const gate = createRenderGate();
    const snap = makeSnap({ showGrid: false });
    recordRender(gate, snap);
    expect(needsRender(gate, { ...snap, showGrid: true })).toBe(true);
  });

  it('returns true when showMinimap toggles', () => {
    const gate = createRenderGate();
    const snap = makeSnap({ showMinimap: true });
    recordRender(gate, snap);
    expect(needsRender(gate, { ...snap, showMinimap: false })).toBe(true);
  });

  it('returns true when serverUps changes', () => {
    const gate = createRenderGate();
    const snap = makeSnap({ serverUps: 60 });
    recordRender(gate, snap);
    expect(needsRender(gate, { ...snap, serverUps: 59 })).toBe(true);
  });

  it('returns true when serverUps goes from defined to undefined', () => {
    const gate = createRenderGate();
    const snap = makeSnap({ serverUps: 60 });
    recordRender(gate, snap);
    expect(needsRender(gate, { ...snap, serverUps: undefined })).toBe(true);
  });
});

// ── recordRender ──────────────────────────────────────────────────────────────

describe('recordRender', () => {
  it('clears dirty flag', () => {
    const gate = createRenderGate();
    expect(gate.dirty).toBe(true);
    recordRender(gate, makeSnap());
    expect(gate.dirty).toBe(false);
  });

  it('stores a shallow copy of the snapshot (not same reference)', () => {
    const gate = createRenderGate();
    const snap = makeSnap();
    recordRender(gate, snap);
    expect(gate.lastSnapshot).not.toBe(snap); // copy, not same ref
    expect(gate.lastSnapshot?.overlay).toBe(snap.overlay);
  });

  it('subsequent recordRender updates the snapshot', () => {
    const gate = createRenderGate();
    const entities1 = { id: 1 };
    const entities2 = { id: 2 };
    recordRender(gate, makeSnap({ entities: entities1 }));
    recordRender(gate, makeSnap({ entities: entities2 }));
    expect(gate.lastSnapshot?.entities).toBe(entities2);
  });
});

// ── markDirty ─────────────────────────────────────────────────────────────────

describe('markDirty', () => {
  it('sets dirty=true', () => {
    const gate = createRenderGate();
    recordRender(gate, makeSnap()); // clears dirty
    expect(gate.dirty).toBe(false);
    markDirty(gate);
    expect(gate.dirty).toBe(true);
  });

  it('forces a render even when all snapshot values are the same', () => {
    const gate = createRenderGate();
    const snap = makeSnap();
    recordRender(gate, snap);
    markDirty(gate);
    // Same snap, but dirty flag forces render
    expect(needsRender(gate, snap)).toBe(true);
  });

  it('render after markDirty+recordRender skips on next same-data frame', () => {
    const gate = createRenderGate();
    const snap = makeSnap();
    recordRender(gate, snap);
    markDirty(gate);          // viewport changed (e.g. pan)
    recordRender(gate, snap); // frame was drawn
    // Next frame: same data, dirty cleared → skip
    expect(needsRender(gate, snap)).toBe(false);
  });
});

// ── integration: simulate rAF loop with poll ─────────────────────────────────

describe('integration — simulated rAF + data poll loop', () => {
  it('draws exactly once per unique data version across many frames', () => {
    const gate = createRenderGate();
    let drawCount = 0;

    // Simulate 10 rAF frames, data changes once (at frame 5)
    let entities = { v: 1 };
    const world  = { w: 256 };
    const snap   = () => makeSnap({ entities, world });

    for (let frame = 0; frame < 10; frame++) {
      if (frame === 5) entities = { v: 2 }; // new poll result

      if (needsRender(gate, snap())) {
        drawCount++;
        recordRender(gate, snap());
      }
    }

    // Drew on frame 0 (initial) and frame 5 (new entities)
    expect(drawCount).toBe(2);
  });

  it('draws on every frame when entities update each frame (live 60fps game)', () => {
    const gate = createRenderGate();
    let drawCount = 0;
    const world = {};

    for (let frame = 0; frame < 10; frame++) {
      const entities = { frame }; // new object each frame (like 60fps poll)
      const snap = makeSnap({ entities, world });
      if (needsRender(gate, snap)) {
        drawCount++;
        recordRender(gate, snap);
      }
    }
    // Every frame had a new entities ref → every frame drew
    expect(drawCount).toBe(10);
  });

  it('draws 0 extra frames between polls when world is static', () => {
    const gate = createRenderGate();
    let drawCount = 0;

    // Stable references — static world, no new data arrives
    const stableEntities = { v: 0 };
    const stableWorld    = { width: 256 };

    for (let frame = 0; frame < 20; frame++) {
      const snap = makeSnap({ entities: stableEntities, world: stableWorld });
      if (needsRender(gate, snap)) {
        drawCount++;
        recordRender(gate, snap);
      }
    }
    // Only frame 0 (initial dirty) triggered a draw; all subsequent frames skipped
    expect(drawCount).toBe(1);
  });
});
