// Decompiled with JetBrains decompiler
// Type: ClusterMapPath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ClusterMapPath : MonoBehaviour
{
  private List<Vector2> m_nodes;
  private Color m_color;
  public UILineRenderer lineRenderer;
  public Image pathStart;
  public Image pathEnd;

  public void Init()
  {
    this.lineRenderer = ((Component) this).gameObject.GetComponentInChildren<UILineRenderer>();
    ((Component) this).gameObject.SetActive(true);
  }

  public void Init(List<Vector2> nodes, Color color)
  {
    this.m_nodes = nodes;
    this.m_color = color;
    this.lineRenderer = ((Component) this).gameObject.GetComponentInChildren<UILineRenderer>();
    this.UpdateColor();
    this.UpdateRenderer();
    ((Component) this).gameObject.SetActive(true);
  }

  public void SetColor(Color color)
  {
    this.m_color = color;
    this.UpdateColor();
  }

  private void UpdateColor()
  {
    ((Graphic) this.lineRenderer).color = this.m_color;
    ((Graphic) this.pathStart).color = this.m_color;
    ((Graphic) this.pathEnd).color = this.m_color;
  }

  public void SetPoints(List<Vector2> points)
  {
    this.m_nodes = points;
    this.UpdateRenderer();
  }

  private void UpdateRenderer()
  {
    this.lineRenderer.Points = ((IEnumerable<Vector2>) Util.GetPointsOnCatmullRomSpline(this.m_nodes, 10)).ToArray<Vector2>();
    if (this.lineRenderer.Points.Length > 1)
    {
      ((Component) this.pathStart).transform.localPosition = Vector2.op_Implicit(this.lineRenderer.Points[0]);
      ((Component) this.pathStart).gameObject.SetActive(true);
      Vector2 point1 = this.lineRenderer.Points[this.lineRenderer.Points.Length - 1];
      Vector2 point2 = this.lineRenderer.Points[this.lineRenderer.Points.Length - 2];
      ((Component) this.pathEnd).transform.localPosition = Vector2.op_Implicit(point1);
      ((Component) this.pathEnd).transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector2.op_Implicit(Vector2.op_Subtraction(point1, point2)));
      ((Component) this.pathEnd).gameObject.SetActive(true);
    }
    else
    {
      ((Component) this.pathStart).gameObject.SetActive(false);
      ((Component) this.pathEnd).gameObject.SetActive(false);
    }
  }

  public float GetRotationForNextSegment() => this.m_nodes.Count > 1 ? Vector2.SignedAngle(Vector2.up, Vector2.op_Subtraction(this.m_nodes[1], this.m_nodes[0])) : 0.0f;
}
