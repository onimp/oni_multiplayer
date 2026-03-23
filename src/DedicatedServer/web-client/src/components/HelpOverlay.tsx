import { KEYBINDINGS } from '../utils/keybindings';

interface Props {
  onClose: () => void;
}

export function HelpOverlay({ onClose }: Props) {
  return (
    <div className="help-overlay" role="dialog" aria-modal="true" aria-label="Keyboard shortcuts" onClick={onClose}>
      <div className="help-panel" onClick={e => e.stopPropagation()}>
        <div className="help-header">
          <h2>Keyboard Shortcuts</h2>
          <button className="help-close" onClick={onClose} aria-label="Close">✕</button>
        </div>
        <table className="help-table">
          <tbody>
            {KEYBINDINGS.map((b, i) => (
              <tr key={i}>
                <td><kbd className="help-kbd">{b.label}</kbd></td>
                <td className="help-desc">{b.description}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <p className="help-footer">Click outside or press <kbd className="help-kbd">Esc</kbd> to close</p>
      </div>
    </div>
  );
}
