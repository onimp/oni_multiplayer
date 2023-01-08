// Decompiled with JetBrains decompiler
// Type: LineLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineLayer : GraphLayer
{
  [Header("Lines")]
  public LineLayer.LineFormat[] line_formatting;
  public Image areaFill;
  public GameObject prefab_line;
  public GameObject line_container;
  private List<GraphedLine> lines = new List<GraphedLine>();
  protected float fillAlphaMin = 0.33f;
  protected float fillFadePixels = 15f;
  public bool fillAreaUnderLine;
  private Texture2D areaTexture;
  private int compressDataToPointCount = 256;
  private LineLayer.DataScalingType compressType = LineLayer.DataScalingType.DropValues;

  private void InitAreaTexture()
  {
    if (Object.op_Inequality((Object) this.areaTexture, (Object) null))
      return;
    this.areaTexture = new Texture2D(96, 32);
    this.areaFill.sprite = Sprite.Create(this.areaTexture, new Rect(0.0f, 0.0f, (float) ((Texture) this.areaTexture).width, (float) ((Texture) this.areaTexture).height), new Vector2(0.5f, 0.5f), 100f);
  }

  public virtual GraphedLine NewLine(Tuple<float, float>[] points, string ID = "")
  {
    Vector2[] points1 = new Vector2[points.Length];
    for (int index = 0; index < points.Length; ++index)
      points1[index] = new Vector2(points[index].first, points[index].second);
    if (this.fillAreaUnderLine)
    {
      this.InitAreaTexture();
      Vector2 min = this.CalculateMin(points);
      Vector2 vector2_1 = Vector2.op_Subtraction(this.CalculateMax(points), min);
      ((Texture) this.areaTexture).filterMode = (FilterMode) 0;
      for (int index1 = 0; index1 < ((Texture) this.areaTexture).width; ++index1)
      {
        float num1 = min.x + vector2_1.x * ((float) index1 / (float) ((Texture) this.areaTexture).width);
        if (points.Length > 1)
        {
          int index2 = 1;
          for (int index3 = 1; index3 < points.Length; ++index3)
          {
            if ((double) points[index3].first >= (double) num1)
            {
              index2 = index3;
              break;
            }
          }
          Vector2 vector2_2;
          // ISSUE: explicit constructor call
          ((Vector2) ref vector2_2).\u002Ector(points[index2].first - points[index2 - 1].first, points[index2].second - points[index2 - 1].second);
          float num2 = (num1 - points[index2 - 1].first) / vector2_2.x;
          bool flag = false;
          int num3 = -1;
          for (int index4 = ((Texture) this.areaTexture).height - 1; index4 >= 0; --index4)
          {
            if (!flag && (double) min.y + (double) vector2_1.y * ((double) index4 / (double) ((Texture) this.areaTexture).height) < (double) points[index2 - 1].second + (double) vector2_2.y * (double) num2)
            {
              flag = true;
              num3 = index4;
            }
            Color color = flag ? new Color(1f, 1f, 1f, Mathf.Lerp(1f, this.fillAlphaMin, Mathf.Clamp((float) (num3 - index4) / this.fillFadePixels, 0.0f, 1f))) : Color.clear;
            this.areaTexture.SetPixel(index1, index4, color);
          }
        }
      }
      this.areaTexture.Apply();
      ((Graphic) this.areaFill).color = this.line_formatting[0].color;
    }
    return this.NewLine(points1, ID);
  }

  private GraphedLine FindLine(string ID)
  {
    string str = string.Format("line_{0}", (object) ID);
    foreach (GraphedLine line in this.lines)
    {
      if (((Object) line).name == str)
        return ((Component) line).GetComponent<GraphedLine>();
    }
    GameObject gameObject = Util.KInstantiateUI(this.prefab_line, this.line_container, true);
    ((Object) gameObject).name = str;
    GraphedLine component = gameObject.GetComponent<GraphedLine>();
    this.lines.Add(component);
    return component;
  }

  public virtual void RefreshLine(Tuple<float, float>[] data, string ID)
  {
    this.FillArea(data);
    Vector2[] points;
    if (data.Length > this.compressDataToPointCount)
    {
      Vector2[] vector2Array = new Vector2[this.compressDataToPointCount];
      if (this.compressType == LineLayer.DataScalingType.DropValues)
      {
        float num1 = (float) (data.Length - this.compressDataToPointCount + 1);
        float num2 = (float) data.Length / num1;
        int index1 = 0;
        float num3 = 0.0f;
        for (int index2 = 0; index2 < data.Length; ++index2)
        {
          ++num3;
          if ((double) num3 >= (double) num2)
          {
            num3 -= num2;
          }
          else
          {
            vector2Array[index1] = new Vector2(data[index2].first, data[index2].second);
            ++index1;
          }
        }
        if (Vector2.op_Equality(vector2Array[this.compressDataToPointCount - 1], Vector2.zero))
          vector2Array[this.compressDataToPointCount - 1] = vector2Array[this.compressDataToPointCount - 2];
      }
      else
      {
        int num4 = data.Length / this.compressDataToPointCount;
        for (int index3 = 0; index3 < this.compressDataToPointCount; ++index3)
        {
          if (index3 > 0)
          {
            float num5 = 0.0f;
            switch (this.compressType)
            {
              case LineLayer.DataScalingType.Average:
                for (int index4 = 0; index4 < num4; ++index4)
                  num5 += data[index3 * num4 - index4].second;
                num5 /= (float) num4;
                break;
              case LineLayer.DataScalingType.Max:
                for (int index5 = 0; index5 < num4; ++index5)
                  num5 = Mathf.Max(num5, data[index3 * num4 - index5].second);
                break;
            }
            vector2Array[index3] = new Vector2(data[index3 * num4].first, num5);
          }
        }
      }
      points = vector2Array;
    }
    else
    {
      points = new Vector2[data.Length];
      for (int index = 0; index < data.Length; ++index)
        points[index] = new Vector2(data[index].first, data[index].second);
    }
    GraphedLine line = this.FindLine(ID);
    line.SetPoints(points);
    ((Graphic) line.line_renderer).color = this.line_formatting[this.lines.Count % this.line_formatting.Length].color;
    line.line_renderer.LineThickness = (float) this.line_formatting[this.lines.Count % this.line_formatting.Length].thickness;
  }

  private void FillArea(Tuple<float, float>[] points)
  {
    if (!this.fillAreaUnderLine)
      return;
    this.InitAreaTexture();
    Vector2 min;
    Vector2 max;
    this.CalculateMinMax(points, out min, out max);
    Vector2 vector2_1 = Vector2.op_Subtraction(max, min);
    ((Texture) this.areaTexture).filterMode = (FilterMode) 0;
    Vector2 vector2_2 = new Vector2();
    for (int index1 = 0; index1 < ((Texture) this.areaTexture).width; ++index1)
    {
      float num1 = min.x + vector2_1.x * ((float) index1 / (float) ((Texture) this.areaTexture).width);
      if (points.Length > 1)
      {
        int index2 = 1;
        for (int index3 = 1; index3 < points.Length; ++index3)
        {
          if ((double) points[index3].first >= (double) num1)
          {
            index2 = index3;
            break;
          }
        }
        vector2_2.x = points[index2].first - points[index2 - 1].first;
        vector2_2.y = points[index2].second - points[index2 - 1].second;
        float num2 = (num1 - points[index2 - 1].first) / vector2_2.x;
        bool flag = false;
        int num3 = -1;
        for (int index4 = ((Texture) this.areaTexture).height - 1; index4 >= 0; --index4)
        {
          if (!flag && (double) min.y + (double) vector2_1.y * ((double) index4 / (double) ((Texture) this.areaTexture).height) < (double) points[index2 - 1].second + (double) vector2_2.y * (double) num2)
          {
            flag = true;
            num3 = index4;
          }
          Color color = flag ? new Color(1f, 1f, 1f, Mathf.Lerp(1f, this.fillAlphaMin, Mathf.Clamp((float) (num3 - index4) / this.fillFadePixels, 0.0f, 1f))) : Color.clear;
          this.areaTexture.SetPixel(index1, index4, color);
        }
      }
    }
    this.areaTexture.Apply();
    ((Graphic) this.areaFill).color = this.line_formatting[0].color;
  }

  private void CalculateMinMax(Tuple<float, float>[] points, out Vector2 min, out Vector2 max)
  {
    max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
    min = new Vector2(float.PositiveInfinity, 0.0f);
    for (int index = 0; index < points.Length; ++index)
    {
      max = new Vector2(Mathf.Max(points[index].first, max.x), Mathf.Max(points[index].second, max.y));
      min = new Vector2(Mathf.Min(points[index].first, min.x), Mathf.Min(points[index].second, min.y));
    }
  }

  protected Vector2 CalculateMax(Tuple<float, float>[] points)
  {
    Vector2 max;
    // ISSUE: explicit constructor call
    ((Vector2) ref max).\u002Ector(float.NegativeInfinity, float.NegativeInfinity);
    for (int index = 0; index < points.Length; ++index)
    {
      // ISSUE: explicit constructor call
      ((Vector2) ref max).\u002Ector(Mathf.Max(points[index].first, max.x), Mathf.Max(points[index].second, max.y));
    }
    return max;
  }

  protected Vector2 CalculateMin(Tuple<float, float>[] points)
  {
    Vector2 min;
    // ISSUE: explicit constructor call
    ((Vector2) ref min).\u002Ector(float.PositiveInfinity, 0.0f);
    for (int index = 0; index < points.Length; ++index)
    {
      // ISSUE: explicit constructor call
      ((Vector2) ref min).\u002Ector(Mathf.Min(points[index].first, min.x), Mathf.Min(points[index].second, min.y));
    }
    return min;
  }

  public GraphedLine NewLine(Vector2[] points, string ID = "")
  {
    GameObject gameObject = Util.KInstantiateUI(this.prefab_line, this.line_container, true);
    if (ID == "")
      ID = this.lines.Count.ToString();
    ((Object) gameObject).name = string.Format("line_{0}", (object) ID);
    GraphedLine component = gameObject.GetComponent<GraphedLine>();
    if (points.Length > this.compressDataToPointCount)
    {
      Vector2[] vector2Array = new Vector2[this.compressDataToPointCount];
      if (this.compressType == LineLayer.DataScalingType.DropValues)
      {
        float num1 = (float) (points.Length - this.compressDataToPointCount + 1);
        float num2 = (float) points.Length / num1;
        int index1 = 0;
        float num3 = 0.0f;
        for (int index2 = 0; index2 < points.Length; ++index2)
        {
          ++num3;
          if ((double) num3 >= (double) num2)
          {
            num3 -= num2;
          }
          else
          {
            vector2Array[index1] = points[index2];
            ++index1;
          }
        }
        if (Vector2.op_Equality(vector2Array[this.compressDataToPointCount - 1], Vector2.zero))
          vector2Array[this.compressDataToPointCount - 1] = vector2Array[this.compressDataToPointCount - 2];
      }
      else
      {
        int num4 = points.Length / this.compressDataToPointCount;
        for (int index3 = 0; index3 < this.compressDataToPointCount; ++index3)
        {
          if (index3 > 0)
          {
            float num5 = 0.0f;
            switch (this.compressType)
            {
              case LineLayer.DataScalingType.Average:
                for (int index4 = 0; index4 < num4; ++index4)
                  num5 += points[index3 * num4 - index4].y;
                num5 /= (float) num4;
                break;
              case LineLayer.DataScalingType.Max:
                for (int index5 = 0; index5 < num4; ++index5)
                  num5 = Mathf.Max(num5, points[index3 * num4 - index5].y);
                break;
            }
            vector2Array[index3] = new Vector2(points[index3 * num4].x, num5);
          }
        }
      }
      points = vector2Array;
    }
    component.SetPoints(points);
    ((Graphic) component.line_renderer).color = this.line_formatting[this.lines.Count % this.line_formatting.Length].color;
    component.line_renderer.LineThickness = (float) this.line_formatting[this.lines.Count % this.line_formatting.Length].thickness;
    this.lines.Add(component);
    return component;
  }

  public void ClearLines()
  {
    foreach (GraphedLine line in this.lines)
    {
      if (Object.op_Inequality((Object) line, (Object) null) && Object.op_Inequality((Object) ((Component) line).gameObject, (Object) null))
        Object.DestroyImmediate((Object) ((Component) line).gameObject);
    }
    this.lines.Clear();
  }

  private void Update()
  {
    RectTransform component = ((Component) this).gameObject.GetComponent<RectTransform>();
    if (!RectTransformUtility.RectangleContainsScreenPoint(component, Vector2.op_Implicit(Input.mousePosition)))
    {
      for (int index = 0; index < this.lines.Count; ++index)
        this.lines[index].HidePointHighlight();
    }
    else
    {
      Vector2 toPoint = Vector2.zero;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(((Component) this).gameObject.GetComponent<RectTransform>(), Vector2.op_Implicit(Input.mousePosition), (Camera) null, ref toPoint);
      toPoint = Vector2.op_Addition(toPoint, Vector2.op_Division(component.sizeDelta, 2f));
      for (int index = 0; index < this.lines.Count; ++index)
      {
        if (this.lines[index].PointCount != 0)
        {
          Vector2 dataToPointOnXaxis = this.lines[index].GetClosestDataToPointOnXAxis(toPoint);
          if (!float.IsInfinity(dataToPointOnXaxis.x) && !float.IsNaN(dataToPointOnXaxis.x) && !float.IsInfinity(dataToPointOnXaxis.y) && !float.IsNaN(dataToPointOnXaxis.y))
            this.lines[index].SetPointHighlight(dataToPointOnXaxis);
          else
            this.lines[index].HidePointHighlight();
        }
      }
    }
  }

  [Serializable]
  public struct LineFormat
  {
    public Color color;
    public int thickness;
  }

  public enum DataScalingType
  {
    Average,
    Max,
    DropValues,
  }
}
