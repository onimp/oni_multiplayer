import { describe, it, expect } from 'vitest';
import { gameStateToJson, jsonToGameState, isParseError } from './clipboardState';
import type { GameState, ParseResult } from './clipboardState';

// ── helpers ───────────────────────────────────────────────────────────────────

function makeState(overrides: Partial<GameState> = {}): GameState {
  return {
    tick:            12345,
    cycle:           10,
    speed:           1,
    paused:          false,
    worldWidth:      256,
    worldHeight:     128,
    duplicantCount:  5,
    buildingCount:   80,
    entityCount:     120,
    ...overrides,
  };
}

// ── gameStateToJson ───────────────────────────────────────────────────────────

describe('gameStateToJson', () => {
  it('produces valid JSON that round-trips through JSON.parse', () => {
    const state = makeState();
    const json = gameStateToJson(state);
    expect(() => JSON.parse(json)).not.toThrow();
  });

  it('round-trips: jsonToGameState(gameStateToJson(state)) returns the same state', () => {
    const state = makeState();
    const result = jsonToGameState(gameStateToJson(state));
    expect(isParseError(result)).toBe(false);
    expect(result).toMatchObject(state);
  });

  it('uses 2-space indentation (pretty-printed)', () => {
    const json = gameStateToJson(makeState());
    expect(json).toContain('  "tick"');
  });

  it('includes all required numeric fields', () => {
    const json = gameStateToJson(makeState());
    const required = ['tick', 'cycle', 'speed', 'worldWidth', 'worldHeight',
      'duplicantCount', 'buildingCount', 'entityCount'];
    for (const field of required) {
      expect(json).toContain(`"${field}"`);
    }
  });

  it('includes the paused boolean field', () => {
    expect(gameStateToJson(makeState({ paused: true }))).toContain('"paused": true');
    expect(gameStateToJson(makeState({ paused: false }))).toContain('"paused": false');
  });

  it('preserves optional fields when present', () => {
    const state = makeState({ serverUps: 60, cycleTime: 300, isNight: true, bootErrorCount: 0 });
    const json = gameStateToJson(state);
    expect(json).toContain('"serverUps"');
    expect(json).toContain('"cycleTime"');
    expect(json).toContain('"isNight"');
    expect(json).toContain('"bootErrorCount"');
  });

  it('omits optional fields when absent', () => {
    const json = gameStateToJson(makeState());
    expect(json).not.toContain('"serverUps"');
    expect(json).not.toContain('"cycleTime"');
  });
});

// ── isParseError ──────────────────────────────────────────────────────────────

describe('isParseError', () => {
  it('returns true for a parse-error result', () => {
    const r: ParseResult = { kind: 'parse-error', message: 'bad' };
    expect(isParseError(r)).toBe(true);
  });

  it('returns true for a validation-error result', () => {
    const r: ParseResult = { kind: 'validation-error', message: 'missing field' };
    expect(isParseError(r)).toBe(true);
  });

  it('returns false for a valid GameState', () => {
    expect(isParseError(makeState())).toBe(false);
  });
});

// ── jsonToGameState — valid input ─────────────────────────────────────────────

describe('jsonToGameState — valid input', () => {
  it('returns a GameState object for valid minimal input', () => {
    const result = jsonToGameState(gameStateToJson(makeState()));
    expect(isParseError(result)).toBe(false);
  });

  it('numeric fields have correct values', () => {
    const state = makeState({ tick: 99999, cycle: 7, duplicantCount: 3 });
    const result = jsonToGameState(gameStateToJson(state)) as GameState;
    expect(result.tick).toBe(99999);
    expect(result.cycle).toBe(7);
    expect(result.duplicantCount).toBe(3);
  });

  it('boolean paused=true is preserved', () => {
    const result = jsonToGameState(gameStateToJson(makeState({ paused: true }))) as GameState;
    expect(result.paused).toBe(true);
  });

  it('boolean paused=false is preserved', () => {
    const result = jsonToGameState(gameStateToJson(makeState({ paused: false }))) as GameState;
    expect(result.paused).toBe(false);
  });

  it('preserves optional serverUps when present', () => {
    const result = jsonToGameState(gameStateToJson(makeState({ serverUps: 58 }))) as GameState;
    expect(result.serverUps).toBe(58);
  });

  it('accepts extra unknown fields without error', () => {
    const json = JSON.stringify({ ...makeState(), extraField: 'hello' });
    expect(isParseError(jsonToGameState(json))).toBe(false);
  });

  it('accepts zero values for numeric fields', () => {
    const state = makeState({ tick: 0, cycle: 0, duplicantCount: 0, buildingCount: 0, entityCount: 0 });
    expect(isParseError(jsonToGameState(gameStateToJson(state)))).toBe(false);
  });

  it('accepts negative speed (edge case — unusual but valid JSON)', () => {
    const state = makeState({ speed: -1 });
    expect(isParseError(jsonToGameState(gameStateToJson(state)))).toBe(false);
  });
});

// ── jsonToGameState — invalid JSON ────────────────────────────────────────────

describe('jsonToGameState — invalid JSON', () => {
  it('returns parse-error for an empty string', () => {
    const r = jsonToGameState('');
    expect(isParseError(r)).toBe(true);
    expect((r as import('./clipboardState').ParseError).kind).toBe('parse-error');
  });

  it('returns parse-error for malformed JSON', () => {
    const r = jsonToGameState('{tick: 1}'); // keys must be quoted
    expect(isParseError(r)).toBe(true);
    expect((r as import('./clipboardState').ParseError).kind).toBe('parse-error');
  });

  it('returns parse-error for truncated JSON', () => {
    const r = jsonToGameState('{"tick": 1,');
    expect(isParseError(r)).toBe(true);
  });

  it('parse-error message mentions "Invalid JSON"', () => {
    const r = jsonToGameState('not json') as import('./clipboardState').ParseError;
    expect(r.message).toMatch(/invalid json/i);
  });
});

// ── jsonToGameState — wrong root type ─────────────────────────────────────────

describe('jsonToGameState — wrong root type', () => {
  it('returns validation-error for a JSON array', () => {
    const r = jsonToGameState('[1, 2, 3]');
    expect(isParseError(r)).toBe(true);
    expect((r as import('./clipboardState').ParseError).kind).toBe('validation-error');
  });

  it('returns validation-error for a JSON string', () => {
    expect(isParseError(jsonToGameState('"hello"'))).toBe(true);
  });

  it('returns validation-error for a JSON number', () => {
    expect(isParseError(jsonToGameState('42'))).toBe(true);
  });

  it('returns validation-error for JSON null', () => {
    expect(isParseError(jsonToGameState('null'))).toBe(true);
  });

  it('returns validation-error for JSON boolean', () => {
    expect(isParseError(jsonToGameState('true'))).toBe(true);
  });
});

// ── jsonToGameState — missing required fields ─────────────────────────────────

describe('jsonToGameState — missing required numeric fields', () => {
  const requiredFields = [
    'tick', 'cycle', 'speed', 'worldWidth', 'worldHeight',
    'duplicantCount', 'buildingCount', 'entityCount',
  ] as const;

  for (const field of requiredFields) {
    it(`returns validation-error when "${field}" is missing`, () => {
      const state = makeState();
      const obj = { ...state } as Record<string, unknown>;
      delete obj[field];
      const r = jsonToGameState(JSON.stringify(obj));
      expect(isParseError(r)).toBe(true);
      expect((r as import('./clipboardState').ParseError).kind).toBe('validation-error');
    });

    it(`returns validation-error when "${field}" is a string instead of number`, () => {
      const state = makeState();
      const obj = { ...state, [field]: 'not-a-number' } as Record<string, unknown>;
      const r = jsonToGameState(JSON.stringify(obj));
      expect(isParseError(r)).toBe(true);
    });

    it(`returns validation-error when "${field}" is null`, () => {
      const obj = { ...makeState(), [field]: null } as Record<string, unknown>;
      const r = jsonToGameState(JSON.stringify(obj));
      expect(isParseError(r)).toBe(true);
    });
  }
});

describe('jsonToGameState — missing "paused" field', () => {
  it('returns validation-error when "paused" is missing', () => {
    const obj = { ...makeState() } as Record<string, unknown>;
    delete obj['paused'];
    const r = jsonToGameState(JSON.stringify(obj));
    expect(isParseError(r)).toBe(true);
    expect((r as import('./clipboardState').ParseError).kind).toBe('validation-error');
  });

  it('returns validation-error when "paused" is a number instead of boolean', () => {
    const r = jsonToGameState(JSON.stringify({ ...makeState(), paused: 0 }));
    expect(isParseError(r)).toBe(true);
  });

  it('returns validation-error when "paused" is a string', () => {
    const r = jsonToGameState(JSON.stringify({ ...makeState(), paused: 'false' }));
    expect(isParseError(r)).toBe(true);
  });

  it('validation-error message mentions "paused"', () => {
    const obj = { ...makeState() } as Record<string, unknown>;
    delete obj['paused'];
    const r = jsonToGameState(JSON.stringify(obj)) as import('./clipboardState').ParseError;
    expect(r.message).toMatch(/paused/);
  });
});

// ── error message quality ─────────────────────────────────────────────────────

describe('jsonToGameState — error messages', () => {
  it('validation-error names the missing field', () => {
    const obj = { ...makeState() } as Record<string, unknown>;
    delete obj['tick'];
    const r = jsonToGameState(JSON.stringify(obj)) as import('./clipboardState').ParseError;
    expect(r.message).toContain('tick');
  });

  it('validation-error for wrong type names the field', () => {
    const r = jsonToGameState(JSON.stringify({ ...makeState(), cycle: 'ten' })) as import('./clipboardState').ParseError;
    expect(r.message).toContain('cycle');
  });
});
