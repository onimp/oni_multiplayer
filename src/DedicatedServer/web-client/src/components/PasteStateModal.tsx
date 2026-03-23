/**
 * PasteStateModal — modal dialog for pasting a GameState JSON blob.
 *
 * Validate parses the input and shows a one-line preview or error message.
 * Apply commits the parsed state; Cancel / Escape / backdrop-click dismisses.
 */

import { useState } from 'react';
import type { GameState } from '../api/types';
import { jsonToGameState, isParseError } from '../utils/clipboardState';

interface Props {
  onApply: (state: GameState) => void;
  onClose: () => void;
}

export function PasteStateModal({ onApply, onClose }: Props) {
  const [text,    setText]    = useState('');
  const [error,   setError]   = useState<string | null>(null);
  const [preview, setPreview] = useState<GameState | null>(null);

  const handleValidate = () => {
    const result = jsonToGameState(text.trim());
    if (isParseError(result)) {
      setError(result.message);
      setPreview(null);
    } else {
      setError(null);
      setPreview(result);
    }
  };

  const handleApply = () => {
    if (preview) {
      onApply(preview);
      onClose();
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Escape') { e.stopPropagation(); onClose(); }
    // Ctrl+Enter = quick Apply when preview is ready
    if ((e.ctrlKey || e.metaKey) && e.key === 'Enter' && preview) {
      e.preventDefault();
      handleApply();
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose} role="dialog" aria-modal="true" aria-label="Paste game state JSON">
      <div className="modal-box" onClick={e => e.stopPropagation()} onKeyDown={handleKeyDown}>
        <div className="modal-header">
          <span className="modal-title">Paste Game State JSON</span>
          <button className="modal-close" onClick={onClose} aria-label="Close">✕</button>
        </div>

        <textarea
          className="modal-textarea"
          value={text}
          onChange={e => { setText(e.target.value); setError(null); setPreview(null); }}
          placeholder={'{\n  "tick": 12345,\n  "cycle": 10,\n  ...\n}'}
          autoFocus
          spellCheck={false}
          rows={10}
        />

        {error && (
          <div className="modal-error" role="alert">{error}</div>
        )}
        {preview && !error && (
          <div className="modal-preview">
            ✓ Valid — Tick {preview.tick.toLocaleString()} · Cycle {preview.cycle} · {preview.duplicantCount} dupe{preview.duplicantCount !== 1 ? 's' : ''}
          </div>
        )}

        <div className="modal-actions">
          <button
            className="modal-btn"
            onClick={handleValidate}
            disabled={!text.trim()}
          >
            Validate
          </button>
          <button
            className="modal-btn modal-btn-primary"
            onClick={handleApply}
            disabled={!preview}
            title="Apply (Ctrl+Enter)"
          >
            Apply
          </button>
          <button className="modal-btn" onClick={onClose}>Cancel</button>
        </div>
      </div>
    </div>
  );
}
