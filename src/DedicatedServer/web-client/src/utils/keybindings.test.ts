import { describe, it, expect } from 'vitest';
import {
  resolveKeyAction,
  isInputTagName,
  KEYBINDINGS,
  type KeyAction,
} from './keybindings';

// ── resolveKeyAction — primary keys ──────────────────────────────────────────

describe('resolveKeyAction — zoom', () => {
  it('+ maps to zoom-in', () => {
    expect(resolveKeyAction('+')).toBe('zoom-in');
  });

  it('= maps to zoom-in (no-shift shortcut)', () => {
    expect(resolveKeyAction('=')).toBe('zoom-in');
  });

  it('- maps to zoom-out', () => {
    expect(resolveKeyAction('-')).toBe('zoom-out');
  });

  it('0 maps to zoom-reset', () => {
    expect(resolveKeyAction('0')).toBe('zoom-reset');
  });
});

describe('resolveKeyAction — pan (arrow keys)', () => {
  it('ArrowLeft maps to pan-left', () => {
    expect(resolveKeyAction('ArrowLeft')).toBe('pan-left');
  });

  it('ArrowRight maps to pan-right', () => {
    expect(resolveKeyAction('ArrowRight')).toBe('pan-right');
  });

  it('ArrowUp maps to pan-up', () => {
    expect(resolveKeyAction('ArrowUp')).toBe('pan-up');
  });

  it('ArrowDown maps to pan-down', () => {
    expect(resolveKeyAction('ArrowDown')).toBe('pan-down');
  });
});

describe('resolveKeyAction — pan (WASD)', () => {
  it('a maps to pan-left', () => {
    expect(resolveKeyAction('a')).toBe('pan-left');
  });

  it('d maps to pan-right', () => {
    expect(resolveKeyAction('d')).toBe('pan-right');
  });

  it('w maps to pan-up', () => {
    expect(resolveKeyAction('w')).toBe('pan-up');
  });

  it('s maps to pan-down', () => {
    expect(resolveKeyAction('s')).toBe('pan-down');
  });
});

describe('resolveKeyAction — view / help', () => {
  it('g maps to toggle-grid', () => {
    expect(resolveKeyAction('g')).toBe('toggle-grid');
  });

  it('h maps to show-help', () => {
    expect(resolveKeyAction('h')).toBe('show-help');
  });

  it('? maps to show-help', () => {
    expect(resolveKeyAction('?')).toBe('show-help');
  });

  it('Escape maps to close-help', () => {
    expect(resolveKeyAction('Escape')).toBe('close-help');
  });
});

// ── resolveKeyAction — unknown / edge cases ───────────────────────────────────

describe('resolveKeyAction — unbound keys return null', () => {
  it('returns null for an unbound letter', () => {
    expect(resolveKeyAction('q')).toBeNull();
  });

  it('returns null for an empty string', () => {
    expect(resolveKeyAction('')).toBeNull();
  });

  it('returns null for a number string outside bindings', () => {
    expect(resolveKeyAction('5')).toBeNull();
  });

  it('is case-sensitive: G (upper) is not bound', () => {
    expect(resolveKeyAction('G')).toBeNull();
  });

  it('is case-sensitive: A (upper) is not bound', () => {
    expect(resolveKeyAction('A')).toBeNull();
  });

  it('is case-sensitive: H (upper) is not bound', () => {
    expect(resolveKeyAction('H')).toBeNull();
  });

  it('returns null for Tab', () => {
    expect(resolveKeyAction('Tab')).toBeNull();
  });
});

// ── KEYBINDINGS table integrity ───────────────────────────────────────────────

describe('KEYBINDINGS table — structural validity', () => {
  it('every binding has at least one key', () => {
    for (const b of KEYBINDINGS) {
      expect(b.keys.length, `${b.action} must have at least one key`).toBeGreaterThan(0);
    }
  });

  it('every binding has a non-empty label and description', () => {
    for (const b of KEYBINDINGS) {
      expect(b.label.length,       `${b.action} label`).toBeGreaterThan(0);
      expect(b.description.length, `${b.action} description`).toBeGreaterThan(0);
    }
  });

  it('all actions present in the table are valid KeyAction values', () => {
    const valid: KeyAction[] = [
      'zoom-in', 'zoom-out', 'zoom-reset',
      'pan-left', 'pan-right', 'pan-up', 'pan-down',
      'toggle-grid', 'toggle-minimap', 'show-help', 'close-help',
    ];
    for (const b of KEYBINDINGS) {
      expect(valid).toContain(b.action);
    }
  });

  it('all 11 KeyAction values are covered by at least one binding', () => {
    const covered = new Set(KEYBINDINGS.map(b => b.action));
    const required: KeyAction[] = [
      'zoom-in', 'zoom-out', 'zoom-reset',
      'pan-left', 'pan-right', 'pan-up', 'pan-down',
      'toggle-grid', 'toggle-minimap', 'show-help', 'close-help',
    ];
    for (const action of required) {
      expect(covered.has(action), `${action} must be covered`).toBe(true);
    }
  });

  it('no key string appears in more than one binding', () => {
    const seen = new Map<string, KeyAction>();
    for (const b of KEYBINDINGS) {
      for (const k of b.keys) {
        expect(seen.has(k), `key "${k}" bound twice`).toBe(false);
        seen.set(k, b.action);
      }
    }
  });
});

// ── isInputTagName (pure, no DOM needed) ─────────────────────────────────────

describe('isInputTagName', () => {
  it('returns true for "input"', () => {
    expect(isInputTagName('input')).toBe(true);
  });

  it('returns true for "textarea"', () => {
    expect(isInputTagName('textarea')).toBe(true);
  });

  it('returns true for "select"', () => {
    expect(isInputTagName('select')).toBe(true);
  });

  it('is case-insensitive: INPUT (uppercase) also returns true', () => {
    expect(isInputTagName('INPUT')).toBe(true);
  });

  it('is case-insensitive: TEXTAREA returns true', () => {
    expect(isInputTagName('TEXTAREA')).toBe(true);
  });

  it('returns false for "div"', () => {
    expect(isInputTagName('div')).toBe(false);
  });

  it('returns false for "canvas"', () => {
    expect(isInputTagName('canvas')).toBe(false);
  });

  it('returns false for "button"', () => {
    expect(isInputTagName('button')).toBe(false);
  });

  it('returns false for an empty string', () => {
    expect(isInputTagName('')).toBe(false);
  });
});
