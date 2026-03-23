/**
 * DupeDetailPanel — shows detailed stats for a selected duplicant.
 *
 * Rendered as an overlay inside the canvas container (same approach as
 * CellInspectorPanel).  Displays: name, chore, stamina bar, calories bar,
 * SM state, position, and nav status.
 *
 * Skills, traits, and stress are NOT in the server API and therefore
 * cannot be shown (noted below).
 */

import type { EntityData } from '../api/types';
import { formatDupeDetail } from '../utils/dupeDetail';

interface Props {
  entity: EntityData;
  onClose: () => void;
}

/** Renders a compact bar: filled portion + percentage label. */
function StatBar({ pct, color, label, title }: {
  pct: number;
  color: string;
  label: string;
  title: string;
}) {
  return (
    <div className="dupe-stat-row" title={title}>
      <span className="dupe-stat-label">{label}</span>
      <div className="dupe-stat-bar-track">
        <div
          className="dupe-stat-bar-fill"
          style={{ width: `${pct}%`, background: color }}
        />
      </div>
      <span className="dupe-stat-pct">{pct}%</span>
    </div>
  );
}

export function DupeDetailPanel({ entity, onClose }: Props) {
  const detail = formatDupeDetail(entity);
  if (!detail) return null;

  return (
    <div className="dupe-detail-panel">
      {/* Header */}
      <div className="dupe-detail-header">
        <span className="dupe-detail-name">👤 {detail.name}</span>
        <button className="dupe-detail-close" onClick={onClose} aria-label="Close">✕</button>
      </div>

      {/* Chore */}
      <div className="dupe-detail-row">
        <span className="dupe-detail-key">Chore</span>
        <span className="dupe-detail-val">{detail.currentChore}</span>
      </div>

      {/* SM State */}
      <div className="dupe-detail-row">
        <span className="dupe-detail-key">State</span>
        <span className="dupe-detail-val dupe-detail-sm">{detail.smState}</span>
      </div>

      {/* Stamina bar */}
      <StatBar
        pct={detail.staminaPct}
        color={detail.staminaPct < 25 ? '#e74c3c' : detail.staminaPct < 60 ? '#f39c12' : '#2ecc71'}
        label="💤 Stamina"
        title={detail.staminaRaw}
      />

      {/* Calories bar */}
      <StatBar
        pct={detail.caloriesPct}
        color={detail.caloriesPct < 20 ? '#e74c3c' : detail.caloriesPct < 50 ? '#f39c12' : '#3498db'}
        label="🍖 Calories"
        title={detail.caloriesRaw}
      />

      {/* Position + nav */}
      <div className="dupe-detail-row">
        <span className="dupe-detail-key">Pos</span>
        <span className="dupe-detail-val">
          ({detail.position.x}, {detail.position.y})
          {detail.moving && <span className="dupe-detail-moving"> ↔ moving</span>}
        </span>
      </div>

      {/* API gap notice */}
      <div className="dupe-detail-muted">
        Skills &amp; traits not in API
      </div>
    </div>
  );
}
