// Decompiled with JetBrains decompiler
// Type: SparkLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class SparkLayer : LineLayer
{
  public Image subZeroAreaFill;
  public SparkLayer.ColorRules colorRules;
  public bool debugMark;
  public bool scaleHeightToData = true;
  public bool scaleWidthToData = true;

  public void SetColor(ColonyDiagnostic.DiagnosticResult result)
  {
    switch (result.opinion)
    {
      case ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening:
      case ColonyDiagnostic.DiagnosticResult.Opinion.Bad:
        this.SetColor(Constants.NEGATIVE_COLOR);
        break;
      case ColonyDiagnostic.DiagnosticResult.Opinion.Warning:
      case ColonyDiagnostic.DiagnosticResult.Opinion.Concern:
        this.SetColor(Constants.WARNING_COLOR);
        break;
      case ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion:
      case ColonyDiagnostic.DiagnosticResult.Opinion.Tutorial:
      case ColonyDiagnostic.DiagnosticResult.Opinion.Normal:
        this.SetColor(Constants.NEUTRAL_COLOR);
        break;
      case ColonyDiagnostic.DiagnosticResult.Opinion.Good:
        this.SetColor(Constants.POSITIVE_COLOR);
        break;
      default:
        this.SetColor(Constants.NEUTRAL_COLOR);
        break;
    }
  }

  public void SetColor(Color color) => this.line_formatting[0].color = color;

  public override GraphedLine NewLine(Tuple<float, float>[] points, string ID = "")
  {
    Color positiveColor = Constants.POSITIVE_COLOR;
    Color neutralColor = Constants.NEUTRAL_COLOR;
    Color negativeColor = Constants.NEGATIVE_COLOR;
    if (this.colorRules.setOwnColor)
    {
      if (points.Length > 2)
      {
        if (this.colorRules.zeroIsBad && (double) points[points.Length - 1].second == 0.0)
          this.line_formatting[0].color = negativeColor;
        else if ((double) points[points.Length - 1].second > (double) points[points.Length - 2].second)
          this.line_formatting[0].color = this.colorRules.positiveIsGood ? positiveColor : negativeColor;
        else if ((double) points[points.Length - 1].second < (double) points[points.Length - 2].second)
          this.line_formatting[0].color = this.colorRules.positiveIsGood ? negativeColor : positiveColor;
        else
          this.line_formatting[0].color = neutralColor;
      }
      else
        this.line_formatting[0].color = neutralColor;
    }
    this.ScaleToData(points);
    if (Object.op_Inequality((Object) this.subZeroAreaFill, (Object) null))
      ((Graphic) this.subZeroAreaFill).color = new Color(this.line_formatting[0].color.r, this.line_formatting[0].color.g, this.line_formatting[0].color.b, this.fillAlphaMin);
    return base.NewLine(points, ID);
  }

  public override void RefreshLine(Tuple<float, float>[] points, string ID)
  {
    this.SetColor(points);
    this.ScaleToData(points);
    base.RefreshLine(points, ID);
  }

  private void SetColor(Tuple<float, float>[] points)
  {
    Color positiveColor = Constants.POSITIVE_COLOR;
    Color neutralColor = Constants.NEUTRAL_COLOR;
    Color negativeColor = Constants.NEGATIVE_COLOR;
    if (this.colorRules.setOwnColor)
    {
      if (points.Length > 2)
      {
        if (this.colorRules.zeroIsBad && (double) points[points.Length - 1].second == 0.0)
          this.line_formatting[0].color = negativeColor;
        else if ((double) points[points.Length - 1].second > (double) points[points.Length - 2].second)
          this.line_formatting[0].color = this.colorRules.positiveIsGood ? positiveColor : negativeColor;
        else if ((double) points[points.Length - 1].second < (double) points[points.Length - 2].second)
          this.line_formatting[0].color = this.colorRules.positiveIsGood ? negativeColor : positiveColor;
        else
          this.line_formatting[0].color = neutralColor;
      }
      else
        this.line_formatting[0].color = neutralColor;
    }
    if (!Object.op_Inequality((Object) this.subZeroAreaFill, (Object) null))
      return;
    ((Graphic) this.subZeroAreaFill).color = new Color(this.line_formatting[0].color.r, this.line_formatting[0].color.g, this.line_formatting[0].color.b, this.fillAlphaMin);
  }

  private void ScaleToData(Tuple<float, float>[] points)
  {
    if (!this.scaleWidthToData && !this.scaleHeightToData)
      return;
    Vector2 min = this.CalculateMin(points);
    Vector2 max = this.CalculateMax(points);
    if (this.scaleHeightToData)
    {
      this.graph.ClearHorizontalGuides();
      this.graph.axis_y.max_value = max.y;
      this.graph.axis_y.min_value = min.y;
      this.graph.RefreshHorizontalGuides();
    }
    if (!this.scaleWidthToData)
      return;
    this.graph.ClearVerticalGuides();
    this.graph.axis_x.max_value = max.x;
    this.graph.axis_x.min_value = min.x;
    this.graph.RefreshVerticalGuides();
  }

  [Serializable]
  public struct ColorRules
  {
    public bool setOwnColor;
    public bool positiveIsGood;
    public bool zeroIsBad;
  }
}
