import type { CellInspectorData } from '../utils/cellInspector';

interface Props {
  data: CellInspectorData;
  onClose: () => void;
}

export function CellInspectorPanel({ data, onClose }: Props) {
  return (
    <div className="cell-inspector">
      <div className="cell-inspector-header">
        <span className="cell-inspector-title">Cell {data.coords}</span>
        <button className="cell-inspector-close" onClick={onClose} aria-label="Close inspector">✕</button>
      </div>

      <table className="cell-inspector-table">
        <tbody>
          <tr>
            <td className="ci-label">Element</td>
            <td className="ci-value">{data.element}</td>
          </tr>
          <tr>
            <td className="ci-label">State</td>
            <td className="ci-value">{data.elementState}</td>
          </tr>
          <tr>
            <td className="ci-label">Mass</td>
            <td className="ci-value">{data.mass}</td>
          </tr>
          <tr>
            <td className="ci-label">Temp</td>
            <td className="ci-value">
              {data.temperatureK}
              <span className="ci-temp-c"> / {data.temperatureC}</span>
            </td>
          </tr>
          <tr>
            <td className="ci-label"></td>
            <td>
              <span
                className="ci-temp-label"
                style={{ color: data.temperatureColor }}
              >
                {data.temperatureLabel}
              </span>
            </td>
          </tr>
        </tbody>
      </table>

      {data.entities.length > 0 && (
        <div className="ci-entities">
          <div className="ci-entities-heading">Entities</div>
          {data.entities.map((e, i) => (
            <div key={i} className="ci-entity-row">{e}</div>
          ))}
        </div>
      )}

      <div className="ci-footer">Right-click or Shift+click to inspect</div>
    </div>
  );
}
