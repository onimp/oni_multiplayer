import { describe, it, expect } from 'vitest';
import { miniBar } from './miniBar';

describe('miniBar', () => {
  // -------------------------------------------------------------------------
  // Bug-regression: fill <span> was missing display:block so the browser
  // treated width as irrelevant (inline element).  Also width was expressed
  // as absolute pixels (pct*80px) rather than a percentage relative to the
  // container.  Both issues caused the bar to render at 0 width.
  // -------------------------------------------------------------------------

  it('fill span has display:block so width is not ignored', () => {
    const html = miniBar('💤', 50, 100);
    // The inner (fill) span must have display:block
    expect(html).toContain('display:block');
  });

  it('width is expressed as a percentage, not pixels', () => {
    const html = miniBar('💤', 50, 100);
    // Must contain a %-based width on the fill span (not "40px" etc.)
    expect(html).toMatch(/width:\d+(\.\d+)?%/);
    // Must NOT use fixed pixels for the fill width (the outer container can
    // still have px; we check the fill value specifically)
    const fillStyle = html.match(/display:block;([^"]*)/)?.[1] ?? '';
    expect(fillStyle).toMatch(/width:\d+(\.\d+)?%/);
    expect(fillStyle).not.toMatch(/width:\d+px/);
  });

  it('50% value produces ~50% width', () => {
    const html = miniBar('💤', 50, 100);
    expect(html).toContain('width:50.0%');
  });

  it('100% value produces 100% width', () => {
    const html = miniBar('💤', 100, 100);
    expect(html).toContain('width:100.0%');
  });

  it('0% value produces 0% width', () => {
    const html = miniBar('💤', 0, 100);
    expect(html).toContain('width:0.0%');
  });

  it('max=0 does not divide by zero — produces 0% width', () => {
    const html = miniBar('💤', 50, 0);
    expect(html).toContain('width:0.0%');
  });

  it('value above max is clamped to 100%', () => {
    const html = miniBar('💤', 200, 100);
    expect(html).toContain('width:100.0%');
  });

  it('kcal unit formats text as kcal (divides by 1000)', () => {
    const html = miniBar('🍖', 500000, 1000000, 'kcal');
    expect(html).toContain('kcal');
    expect(html).toContain('500');
    expect(html).toContain('1000');
  });

  it('colour is green when pct > 0.7', () => {
    const html = miniBar('💤', 80, 100);
    expect(html).toContain('#4cff91');
  });

  it('colour is yellow when 0.3 < pct <= 0.7', () => {
    const html = miniBar('💤', 50, 100);
    expect(html).toContain('#ffe033');
  });

  it('colour is red when pct <= 0.3', () => {
    const html = miniBar('💤', 20, 100);
    expect(html).toContain('#e94560');
  });
});
