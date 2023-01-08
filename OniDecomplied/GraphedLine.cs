// Decompiled with JetBrains decompiler
// Type: GraphedLine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

[AddComponentMenu("KMonoBehaviour/scripts/GraphedLine")]
[Serializable]
public class GraphedLine : KMonoBehaviour
{
  public UILineRenderer line_renderer;
  public LineLayer layer;
  private Vector2[] points = new Vector2[0];
  [SerializeField]
  private GameObject highlightPoint;

  public int PointCount => this.points.Length;

  public void SetPoints(Vector2[] points)
  {
    this.points = points;
    this.UpdatePoints();
  }

  private void UpdatePoints()
  {
    Vector2[] vector2Array = new Vector2[this.points.Length];
    for (int index = 0; index < vector2Array.Length; ++index)
      vector2Array[index] = this.layer.graph.GetRelativePosition(this.points[index]);
    this.line_renderer.Points = vector2Array;
  }

  public Vector2 GetClosestDataToPointOnXAxis(Vector2 toPoint)
  {
    float num = this.layer.graph.axis_x.min_value + this.layer.graph.axis_x.range * (toPoint.x / Util.rectTransform((Component) this.layer.graph).sizeDelta.x);
    Vector2 dataToPointOnXaxis = Vector2.zero;
    foreach (Vector2 point in this.points)
    {
      if ((double) Mathf.Abs(point.x - num) < (double) Mathf.Abs(dataToPointOnXaxis.x - num))
        dataToPointOnXaxis = point;
    }
    return dataToPointOnXaxis;
  }

  public void HidePointHighlight()
  {
    if (!Object.op_Inequality((Object) this.highlightPoint, (Object) null))
      return;
    this.highlightPoint.SetActive(false);
  }

  public void SetPointHighlight(Vector2 point)
  {
    if (Object.op_Equality((Object) this.highlightPoint, (Object) null))
      return;
    this.highlightPoint.SetActive(true);
    Vector2 relativePosition = this.layer.graph.GetRelativePosition(point);
    TransformExtensions.SetLocalPosition((Transform) Util.rectTransform(this.highlightPoint), Vector2.op_Implicit(new Vector2((float) ((double) relativePosition.x * (double) Util.rectTransform((Component) this.layer.graph).sizeDelta.x - (double) Util.rectTransform((Component) this.layer.graph).sizeDelta.x / 2.0), (float) ((double) relativePosition.y * (double) Util.rectTransform((Component) this.layer.graph).sizeDelta.y - (double) Util.rectTransform((Component) this.layer.graph).sizeDelta.y / 2.0))));
    ToolTip component = ((Component) this.layer.graph).GetComponent<ToolTip>();
    component.ClearMultiStringTooltip();
    ToolTip toolTip = component;
    double x = (double) ((Transform) Util.rectTransform(this.highlightPoint)).localPosition.x;
    Rect rect = Util.rectTransform((Component) this.layer.graph).rect;
    double num = (double) ((Rect) ref rect).height / 2.0 - 12.0;
    Vector2 vector2 = new Vector2((float) x, (float) num);
    toolTip.tooltipPositionOffset = vector2;
    component.SetSimpleTooltip(this.layer.graph.axis_x.name + " " + point.x.ToString() + ", " + Mathf.RoundToInt(point.y).ToString() + " " + this.layer.graph.axis_y.name);
    ToolTipScreen.Instance.SetToolTip(component);
  }
}
