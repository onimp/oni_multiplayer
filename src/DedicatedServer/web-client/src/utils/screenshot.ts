/**
 * screenshot.ts — pure filename generation + thin DOM download utility.
 *
 * The filename logic is pure (no DOM, no side effects) and unit-testable in
 * Node/vitest.  The download trigger is a minimal DOM wrapper kept separate
 * so the pure part stays clean.
 */

// ── Pure ──────────────────────────────────────────────────────────────────────

/**
 * Returns the filename for a canvas screenshot.
 *
 * Format: `oni-world-{tick}.png`
 * Falls back to tick=0 when the game state is not yet available.
 */
export function screenshotFilename(tick: number | undefined): string {
  return `oni-world-${tick ?? 0}.png`;
}

/**
 * Extracts the numeric tick from a filename produced by screenshotFilename().
 * Returns null if the filename does not match the expected pattern.
 *
 * Useful for tests and for parsing filenames back to tick values.
 */
export function parseTickFromFilename(filename: string): number | null {
  const match = filename.match(/^oni-world-(\d+)\.png$/);
  return match ? parseInt(match[1], 10) : null;
}

// ── DOM (not unit-tested) ─────────────────────────────────────────────────────

/**
 * Triggers a browser download of a data URL as a named PNG file.
 *
 * Creates a temporary <a> element, clicks it, and removes it immediately.
 * Not called in unit tests — DOM-dependent.
 */
export function triggerDownload(dataUrl: string, filename: string): void {
  const a = document.createElement('a');
  a.href = dataUrl;
  a.download = filename;
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
}
