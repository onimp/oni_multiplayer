/**
 * keybindings.ts — declarative key → action map for the ONI visualizer.
 *
 * Pure module: no DOM access, no side effects, no imports.
 * resolveKeyAction() is the single entry-point used by the global keydown
 * handler in App.tsx.
 */

export type KeyAction =
  | 'zoom-in'
  | 'zoom-out'
  | 'zoom-reset'
  | 'pan-left'
  | 'pan-right'
  | 'pan-up'
  | 'pan-down'
  | 'toggle-grid'
  | 'toggle-minimap'
  | 'show-help'
  | 'close-help';

export interface KeyBinding {
  /** All key values (e.target.key) that trigger this action. */
  keys: readonly string[];
  action: KeyAction;
  /** Display string for the help overlay (e.g. "+ / ="). */
  label: string;
  /** One-sentence description for the help table. */
  description: string;
}

export const KEYBINDINGS: readonly KeyBinding[] = [
  // ── Zoom ─────────────────────────────────────────────────────────────────
  { keys: ['+', '='],              action: 'zoom-in',     label: '+ / =',  description: 'Zoom in' },
  { keys: ['-'],                   action: 'zoom-out',    label: '–',      description: 'Zoom out' },
  { keys: ['0'],                   action: 'zoom-reset',  label: '0',      description: 'Reset zoom & re-center' },
  // ── Pan ──────────────────────────────────────────────────────────────────
  { keys: ['ArrowLeft',  'a'],     action: 'pan-left',    label: '← / A',  description: 'Pan left' },
  { keys: ['ArrowRight', 'd'],     action: 'pan-right',   label: '→ / D',  description: 'Pan right' },
  { keys: ['ArrowUp',    'w'],     action: 'pan-up',      label: '↑ / W',  description: 'Pan up' },
  { keys: ['ArrowDown',  's'],     action: 'pan-down',    label: '↓ / S',  description: 'Pan down' },
  // ── View ─────────────────────────────────────────────────────────────────
  { keys: ['g'],                   action: 'toggle-grid',    label: 'G',      description: 'Toggle grid lines' },
  { keys: ['m'],                   action: 'toggle-minimap', label: 'M',      description: 'Toggle minimap' },
  // ── Help ─────────────────────────────────────────────────────────────────
  { keys: ['h', '?'],              action: 'show-help',   label: 'H / ?',  description: 'Show keyboard shortcuts' },
  { keys: ['Escape'],              action: 'close-help',  label: 'Esc',    description: 'Close help / Unpin tooltip' },
];

/**
 * Returns the action for a given key value, or null if the key is unbound.
 *
 * Matching is case-sensitive ('g' and 'G' are distinct) — callers that want
 * case-insensitive matching should normalise before calling.
 */
export function resolveKeyAction(key: string): KeyAction | null {
  for (const binding of KEYBINDINGS) {
    if ((binding.keys as readonly string[]).includes(key)) return binding.action;
  }
  return null;
}

/**
 * Returns true for tag names that should suppress keyboard shortcuts
 * (user is typing in a form field).  Pure string check — testable in Node.
 */
export function isInputTagName(tagName: string): boolean {
  const t = tagName.toLowerCase();
  return t === 'input' || t === 'textarea' || t === 'select';
}

/**
 * Returns true when the event originates from a text-input element.
 * Used by the global handler to avoid hijacking typing in sidebar inputs.
 * Delegates to isInputTagName so the logic is unit-testable without DOM.
 */
export function isInputTarget(target: EventTarget | null): boolean {
  if (!target || typeof (target as { tagName?: unknown }).tagName !== 'string') return false;
  return isInputTagName((target as Element).tagName);
}
