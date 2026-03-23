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
  | 'export-screenshot'
  | 'copy-state-json'
  | 'paste-state-json'
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
  /**
   * When true, this binding only fires when Ctrl (or ⌘ on Mac) is held.
   * When false/absent, it fires only when Ctrl is NOT held — preventing
   * plain-letter bindings from intercepting Ctrl+letter browser shortcuts.
   */
  ctrl?: boolean;
}

export const KEYBINDINGS: readonly KeyBinding[] = [
  // ── Zoom ─────────────────────────────────────────────────────────────────
  { keys: ['+', '='],              action: 'zoom-in',           label: '+ / =',    description: 'Zoom in' },
  { keys: ['-'],                   action: 'zoom-out',          label: '–',        description: 'Zoom out' },
  { keys: ['0'],                   action: 'zoom-reset',        label: '0',        description: 'Reset zoom & re-center' },
  // ── Pan ──────────────────────────────────────────────────────────────────
  { keys: ['ArrowLeft',  'a'],     action: 'pan-left',          label: '← / A',    description: 'Pan left' },
  { keys: ['ArrowRight', 'd'],     action: 'pan-right',         label: '→ / D',    description: 'Pan right' },
  { keys: ['ArrowUp',    'w'],     action: 'pan-up',            label: '↑ / W',    description: 'Pan up' },
  { keys: ['ArrowDown',  's'],     action: 'pan-down',          label: '↓ / S',    description: 'Pan down' },
  // ── View ─────────────────────────────────────────────────────────────────
  { keys: ['g'],                   action: 'toggle-grid',       label: 'G',        description: 'Toggle grid lines' },
  { keys: ['m'],                   action: 'toggle-minimap',    label: 'M',        description: 'Toggle minimap' },
  // ── Export / Clipboard ───────────────────────────────────────────────────
  { keys: ['s'], ctrl: true,       action: 'export-screenshot', label: 'Ctrl+S',       description: 'Export canvas as PNG' },
  // Ctrl+Shift+C/V: e.key is uppercase when Shift is held with Ctrl.
  { keys: ['C'], ctrl: true,       action: 'copy-state-json',   label: 'Ctrl+Shift+C', description: 'Copy game state as JSON' },
  { keys: ['V'], ctrl: true,       action: 'paste-state-json',  label: 'Ctrl+Shift+V', description: 'Paste game state from JSON' },
  // ── Help ─────────────────────────────────────────────────────────────────
  { keys: ['h', '?'],              action: 'show-help',         label: 'H / ?',    description: 'Show keyboard shortcuts' },
  { keys: ['Escape'],              action: 'close-help',        label: 'Esc',      description: 'Close help / Unpin tooltip' },
];

/**
 * Returns the action for a given key + modifier combination, or null.
 *
 * @param key   - e.key value from the keyboard event (case-sensitive)
 * @param ctrl  - true when Ctrl or ⌘ (Meta) was held during the event
 *
 * Matching rules:
 *  - A binding with ctrl:true  only fires when ctrl=true
 *  - A binding with ctrl:false/absent only fires when ctrl=false
 *  This prevents plain WASD pan from firing when Ctrl+W closes a browser tab.
 */
export function resolveKeyAction(key: string, ctrl = false): KeyAction | null {
  for (const binding of KEYBINDINGS) {
    if ((binding.ctrl ?? false) !== ctrl) continue;
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
