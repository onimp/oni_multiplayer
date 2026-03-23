/**
 * TimelineScrubber — horizontal bar overlaid at the bottom of the canvas,
 * showing buffered tick history.
 *
 * - Dots (oldest left → newest right) represent buffered poll results.
 * - Click a dot to freeze the visualizer at that historical tick.
 * - "● LIVE" button (or pressing Escape) returns to live mode.
 * - Track auto-scrolls to the newest entry when in live mode.
 */

import { memo, useEffect, useRef } from 'react';
import type { TickSnapshot } from '../utils/tickBuffer';

interface Props {
  /** All buffered snapshots, ordered oldest → newest. */
  ticks: TickSnapshot[];
  /** Currently selected tick (null = live mode). */
  selectedTick: number | null;
  onSelect: (tick: number) => void;
  onLive: () => void;
}

function TimelineScrubberInner({ ticks, selectedTick, onSelect, onLive }: Props) {
  const trackRef = useRef<HTMLDivElement>(null);

  // Auto-scroll track to the right (newest entry) when in live mode.
  useEffect(() => {
    if (selectedTick === null && trackRef.current) {
      trackRef.current.scrollLeft = trackRef.current.scrollWidth;
    }
  }, [ticks, selectedTick]);

  const isLive = selectedTick === null;

  return (
    <div className="timeline-scrubber" role="toolbar" aria-label="Tick history scrubber">
      {/* Live button */}
      <button
        className={`timeline-live-btn${isLive ? ' active' : ''}`}
        onClick={onLive}
        title="Return to live view"
      >
        ● LIVE
      </button>

      {/* Dot track */}
      <div className="timeline-track" ref={trackRef} role="group" aria-label="Tick history">
        {ticks.map((snap) => (
          <button
            key={snap.tick}
            className={`timeline-dot${snap.tick === selectedTick ? ' selected' : ''}`}
            onClick={() => onSelect(snap.tick)}
            title={`Tick ${snap.tick.toLocaleString()}`}
            aria-label={`Tick ${snap.tick}`}
            aria-pressed={snap.tick === selectedTick}
          />
        ))}
      </div>

      {/* Tick info */}
      <div className="timeline-tick-info">
        {isLive
          ? `${ticks.length} ticks`
          : `t=${selectedTick?.toLocaleString()}`
        }
      </div>

      {/* Scrubbing badge */}
      {!isLive && (
        <div className="timeline-scrub-badge" title="Viewing historical tick — click ● LIVE to resume">
          PAUSED
        </div>
      )}
    </div>
  );
}

/**
 * Memoized export: only re-renders when tick count, last tick value, or
 * selectedTick changes.  Avoids reconciling 200 dot elements on every
 * parent re-render when the timeline buffer hasn't actually changed.
 */
export const TimelineScrubber = memo(TimelineScrubberInner, (prev, next) => {
  if (prev.selectedTick !== next.selectedTick) return false;  // re-render
  if (prev.ticks.length !== next.ticks.length) return false;  // new dot added
  // Same length: check if newest tick changed (circular buffer shifted)
  const prevLast = prev.ticks[prev.ticks.length - 1]?.tick;
  const nextLast = next.ticks[next.ticks.length - 1]?.tick;
  return prevLast === nextLast;  // true = skip re-render, false = re-render
});
