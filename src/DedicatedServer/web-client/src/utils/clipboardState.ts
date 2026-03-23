/**
 * clipboardState.ts — pure serialization and validation helpers for
 * copying/pasting GameState as JSON.
 *
 * No DOM, no clipboard API — those live in App.tsx.
 * Fully testable in Node/vitest.
 */

import type { GameState } from '../api/types';
export type { GameState };

// ── Serialization ─────────────────────────────────────────────────────────────

/**
 * Serializes a GameState to a pretty-printed JSON string.
 * Stable field order matches the type declaration for readability.
 */
export function gameStateToJson(state: GameState): string {
  return JSON.stringify(state, null, 2);
}

// ── Parse result ──────────────────────────────────────────────────────────────

export interface ParseError {
  kind: 'parse-error' | 'validation-error';
  message: string;
}

export type ParseResult = GameState | ParseError;

/** Type guard: narrows a ParseResult to ParseError. */
export function isParseError(r: ParseResult): r is ParseError {
  return typeof r === 'object' && r !== null && 'kind' in r;
}

// ── Validation ────────────────────────────────────────────────────────────────

/**
 * Required numeric fields.  All must be present and of type number.
 * Matches the non-optional numerics in the GameState interface.
 */
const REQUIRED_NUMERIC: ReadonlyArray<keyof GameState> = [
  'tick',
  'cycle',
  'speed',
  'worldWidth',
  'worldHeight',
  'duplicantCount',
  'buildingCount',
  'entityCount',
];

/**
 * Parses a JSON string and validates it as a GameState.
 *
 * Returns the parsed GameState on success, or a ParseError describing
 * the first problem found (invalid JSON, wrong root type, missing field,
 * wrong field type).
 *
 * Optional fields (serverUps, cycleTime, isNight, bootErrorCount) are
 * preserved if present but not required.
 */
export function jsonToGameState(json: string): ParseResult {
  // ── Parse ──────────────────────────────────────────────────────────────────
  let parsed: unknown;
  try {
    parsed = JSON.parse(json);
  } catch (e) {
    return {
      kind:    'parse-error',
      message: `Invalid JSON: ${(e as Error).message}`,
    };
  }

  // ── Root type ──────────────────────────────────────────────────────────────
  if (parsed === null || typeof parsed !== 'object' || Array.isArray(parsed)) {
    return {
      kind:    'validation-error',
      message: 'Expected a JSON object at the root level',
    };
  }

  const obj = parsed as Record<string, unknown>;

  // ── Required numeric fields ────────────────────────────────────────────────
  for (const field of REQUIRED_NUMERIC) {
    if (typeof obj[field] !== 'number') {
      return {
        kind:    'validation-error',
        message: `Missing or non-numeric field: "${field}"`,
      };
    }
  }

  // ── Required boolean ──────────────────────────────────────────────────────
  if (typeof obj['paused'] !== 'boolean') {
    return {
      kind:    'validation-error',
      message: 'Missing or non-boolean field: "paused"',
    };
  }

  return parsed as GameState;
}
