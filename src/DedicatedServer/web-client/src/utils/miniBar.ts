/**
 * Renders a labelled mini progress bar as an HTML string.
 *
 * Bug fixed: fill <span> was missing `display:block`, so `width` was ignored
 * (inline elements don't respect width/height). Also switched from fixed-pixel
 * width (`pct * 80px`) to percentage (`pct * 100%`) so the bar always fills
 * proportionally regardless of the container's rendered size.
 *
 * @param label  - Emoji/text label shown before the bar
 * @param value  - Current value
 * @param max    - Maximum value (bar = value/max * 100%)
 * @param unit   - Optional unit; 'kcal' triggers /1000 display
 */
export function miniBar(label: string, value: number, max: number, unit = ''): string {
  const pct   = max > 0 ? Math.max(0, Math.min(1, value / max)) : 0;
  const color = pct > 0.7 ? '#4cff91' : pct > 0.3 ? '#ffe033' : '#e94560';
  // FIX: added `display:block` so width is respected; use % so it scales with container.
  const fill  = `display:block;width:${(pct * 100).toFixed(1)}%;height:100%;background:${color};border-radius:3px`;
  const bar   = `<span style="display:inline-block;width:80px;height:7px;background:rgba(255,255,255,0.12);border-radius:3px;vertical-align:middle;overflow:hidden"><span style="${fill}"></span></span>`;
  const text  = unit === 'kcal'
    ? `${(value / 1000).toFixed(0)} / ${(max / 1000).toFixed(0)} kcal`
    : `${Math.round(pct * 100)}%`;
  return `${label} ${bar} <span style="color:#aaa;font-size:10px">${text}</span>`;
}
