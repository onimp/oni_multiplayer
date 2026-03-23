import { describe, it, expect } from 'vitest';
import { screenshotFilename, parseTickFromFilename } from './screenshot';

// ── screenshotFilename ────────────────────────────────────────────────────────

describe('screenshotFilename', () => {
  it('includes the tick number in the filename', () => {
    expect(screenshotFilename(1234)).toBe('oni-world-1234.png');
  });

  it('uses tick=0 as fallback when tick is undefined', () => {
    expect(screenshotFilename(undefined)).toBe('oni-world-0.png');
  });

  it('handles tick=0 explicitly', () => {
    expect(screenshotFilename(0)).toBe('oni-world-0.png');
  });

  it('handles large tick numbers (long game)', () => {
    expect(screenshotFilename(999999)).toBe('oni-world-999999.png');
  });

  it('always starts with "oni-world-"', () => {
    for (const tick of [1, 100, 50000, 0, undefined]) {
      expect(screenshotFilename(tick)).toMatch(/^oni-world-/);
    }
  });

  it('always ends with ".png"', () => {
    for (const tick of [1, 100, 50000, 0, undefined]) {
      expect(screenshotFilename(tick)).toMatch(/\.png$/);
    }
  });

  it('contains only the tick as the numeric part (no extra characters)', () => {
    const name = screenshotFilename(42);
    // Format must be exactly oni-world-{digits}.png
    expect(name).toMatch(/^oni-world-\d+\.png$/);
  });

  it('produces different filenames for different ticks', () => {
    const names = [1, 2, 100, 999].map(screenshotFilename);
    const unique = new Set(names);
    expect(unique.size).toBe(4);
  });
});

// ── parseTickFromFilename ─────────────────────────────────────────────────────

describe('parseTickFromFilename', () => {
  it('parses tick from a valid filename', () => {
    expect(parseTickFromFilename('oni-world-1234.png')).toBe(1234);
  });

  it('parses tick=0', () => {
    expect(parseTickFromFilename('oni-world-0.png')).toBe(0);
  });

  it('parses large tick numbers', () => {
    expect(parseTickFromFilename('oni-world-999999.png')).toBe(999999);
  });

  it('returns null for a filename with wrong prefix', () => {
    expect(parseTickFromFilename('screenshot-1234.png')).toBeNull();
  });

  it('returns null for a filename with wrong extension', () => {
    expect(parseTickFromFilename('oni-world-1234.jpg')).toBeNull();
  });

  it('returns null for an empty string', () => {
    expect(parseTickFromFilename('')).toBeNull();
  });

  it('returns null for a filename with non-numeric tick', () => {
    expect(parseTickFromFilename('oni-world-abc.png')).toBeNull();
  });

  it('round-trips: parseTickFromFilename(screenshotFilename(tick)) === tick', () => {
    for (const tick of [0, 1, 42, 1000, 999999]) {
      expect(parseTickFromFilename(screenshotFilename(tick))).toBe(tick);
    }
  });

  it('round-trip with undefined tick uses 0', () => {
    expect(parseTickFromFilename(screenshotFilename(undefined))).toBe(0);
  });
});
