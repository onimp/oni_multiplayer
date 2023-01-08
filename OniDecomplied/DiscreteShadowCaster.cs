// Decompiled with JetBrains decompiler
// Type: DiscreteShadowCaster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public static class DiscreteShadowCaster
{
  public static void GetVisibleCells(
    int cell,
    List<int> visiblePoints,
    int range,
    LightShape shape,
    bool canSeeThroughTransparent = true)
  {
    visiblePoints.Add(cell);
    Vector2I xy = Grid.CellToXY(cell);
    if (shape == LightShape.Circle)
    {
      DiscreteShadowCaster.ScanOctant(xy, range, 1, DiscreteShadowCaster.Octant.N_NW, 1.0, 0.0, visiblePoints, canSeeThroughTransparent);
      DiscreteShadowCaster.ScanOctant(xy, range, 1, DiscreteShadowCaster.Octant.N_NE, 1.0, 0.0, visiblePoints, canSeeThroughTransparent);
      DiscreteShadowCaster.ScanOctant(xy, range, 1, DiscreteShadowCaster.Octant.E_NE, 1.0, 0.0, visiblePoints, canSeeThroughTransparent);
      DiscreteShadowCaster.ScanOctant(xy, range, 1, DiscreteShadowCaster.Octant.E_SE, 1.0, 0.0, visiblePoints, canSeeThroughTransparent);
      DiscreteShadowCaster.ScanOctant(xy, range, 1, DiscreteShadowCaster.Octant.S_SE, 1.0, 0.0, visiblePoints, canSeeThroughTransparent);
      DiscreteShadowCaster.ScanOctant(xy, range, 1, DiscreteShadowCaster.Octant.S_SW, 1.0, 0.0, visiblePoints, canSeeThroughTransparent);
      DiscreteShadowCaster.ScanOctant(xy, range, 1, DiscreteShadowCaster.Octant.W_SW, 1.0, 0.0, visiblePoints, canSeeThroughTransparent);
      DiscreteShadowCaster.ScanOctant(xy, range, 1, DiscreteShadowCaster.Octant.W_NW, 1.0, 0.0, visiblePoints, canSeeThroughTransparent);
    }
    else
    {
      if (shape != LightShape.Cone)
        return;
      DiscreteShadowCaster.ScanOctant(xy, range, 1, DiscreteShadowCaster.Octant.S_SE, 1.0, 0.0, visiblePoints, canSeeThroughTransparent);
      DiscreteShadowCaster.ScanOctant(xy, range, 1, DiscreteShadowCaster.Octant.S_SW, 1.0, 0.0, visiblePoints, canSeeThroughTransparent);
    }
  }

  private static bool DoesOcclude(int x, int y, bool canSeeThroughTransparent = false)
  {
    int cell = Grid.XYToCell(x, y);
    return Grid.IsValidCell(cell) && (!canSeeThroughTransparent || !Grid.Transparent[cell]) && Grid.Solid[cell];
  }

  private static void ScanOctant(
    Vector2I cellPos,
    int range,
    int depth,
    DiscreteShadowCaster.Octant octant,
    double startSlope,
    double endSlope,
    List<int> visiblePoints,
    bool canSeeThroughTransparent = true)
  {
    int num1 = range * range;
    int num2 = 0;
    int num3 = 0;
    switch (octant)
    {
      case DiscreteShadowCaster.Octant.N_NW:
        num3 = cellPos.y + depth;
        if (num3 >= Grid.HeightInCells)
          return;
        int num4 = cellPos.x - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
        if (num4 < 0)
          num4 = 0;
        for (; DiscreteShadowCaster.GetSlope((double) num4, (double) num3, (double) cellPos.x, (double) cellPos.y, false) <= endSlope; ++num4)
        {
          if (DiscreteShadowCaster.GetVisDistance(num4, num3, cellPos.x, cellPos.y) <= num1)
          {
            if (DiscreteShadowCaster.DoesOcclude(num4, num3, canSeeThroughTransparent))
            {
              if (num4 - 1 >= 0 && !DiscreteShadowCaster.DoesOcclude(num4 - 1, num3, canSeeThroughTransparent) && !DiscreteShadowCaster.DoesOcclude(num4 - 1, num3 - 1, canSeeThroughTransparent))
                DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, DiscreteShadowCaster.GetSlope((double) num4 - 0.5, (double) num3 - 0.5, (double) cellPos.x, (double) cellPos.y, false), visiblePoints, canSeeThroughTransparent);
            }
            else
            {
              if (num4 - 1 >= 0 && DiscreteShadowCaster.DoesOcclude(num4 - 1, num3, canSeeThroughTransparent))
                startSlope = -DiscreteShadowCaster.GetSlope((double) num4 - 0.5, (double) num3 + 0.5, (double) cellPos.x, (double) cellPos.y, false);
              if (!DiscreteShadowCaster.DoesOcclude(num4, num3 - 1, canSeeThroughTransparent) && !visiblePoints.Contains(Grid.XYToCell(num4, num3)))
                visiblePoints.Add(Grid.XYToCell(num4, num3));
            }
          }
        }
        num2 = num4 - 1;
        break;
      case DiscreteShadowCaster.Octant.N_NE:
        num3 = cellPos.y + depth;
        if (num3 >= Grid.HeightInCells)
          return;
        int num5 = cellPos.x + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
        if (num5 >= Grid.WidthInCells)
          num5 = Grid.WidthInCells - 1;
        for (; DiscreteShadowCaster.GetSlope((double) num5, (double) num3, (double) cellPos.x, (double) cellPos.y, false) >= endSlope; --num5)
        {
          if (DiscreteShadowCaster.GetVisDistance(num5, num3, cellPos.x, cellPos.y) <= num1)
          {
            if (DiscreteShadowCaster.DoesOcclude(num5, num3, canSeeThroughTransparent))
            {
              if (num5 + 1 < Grid.HeightInCells && !DiscreteShadowCaster.DoesOcclude(num5 + 1, num3, canSeeThroughTransparent) && !DiscreteShadowCaster.DoesOcclude(num5 + 1, num3 - 1, canSeeThroughTransparent))
              {
                double slope = DiscreteShadowCaster.GetSlope((double) num5 + 0.5, (double) num3 - 0.5, (double) cellPos.x, (double) cellPos.y, false);
                DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, slope, visiblePoints, canSeeThroughTransparent);
              }
            }
            else
            {
              if (num5 + 1 < Grid.HeightInCells && DiscreteShadowCaster.DoesOcclude(num5 + 1, num3, canSeeThroughTransparent))
                startSlope = DiscreteShadowCaster.GetSlope((double) num5 + 0.5, (double) num3 + 0.5, (double) cellPos.x, (double) cellPos.y, false);
              if (!DiscreteShadowCaster.DoesOcclude(num5, num3 - 1, canSeeThroughTransparent) && !visiblePoints.Contains(Grid.XYToCell(num5, num3)))
                visiblePoints.Add(Grid.XYToCell(num5, num3));
            }
          }
        }
        num2 = num5 + 1;
        break;
      case DiscreteShadowCaster.Octant.E_NE:
        num2 = cellPos.x + depth;
        if (num2 >= Grid.WidthInCells)
          return;
        int num6 = cellPos.y + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
        if (num6 >= Grid.HeightInCells)
          num6 = Grid.HeightInCells - 1;
        for (; DiscreteShadowCaster.GetSlope((double) num2, (double) num6, (double) cellPos.x, (double) cellPos.y, true) >= endSlope; --num6)
        {
          if (DiscreteShadowCaster.GetVisDistance(num2, num6, cellPos.x, cellPos.y) <= num1)
          {
            if (DiscreteShadowCaster.DoesOcclude(num2, num6, canSeeThroughTransparent))
            {
              if (num6 + 1 < Grid.HeightInCells && !DiscreteShadowCaster.DoesOcclude(num2, num6 + 1, canSeeThroughTransparent) && !DiscreteShadowCaster.DoesOcclude(num2 - 1, num6 + 1, canSeeThroughTransparent))
                DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, DiscreteShadowCaster.GetSlope((double) num2 - 0.5, (double) num6 + 0.5, (double) cellPos.x, (double) cellPos.y, true), visiblePoints, canSeeThroughTransparent);
            }
            else
            {
              if (num6 + 1 < Grid.HeightInCells && DiscreteShadowCaster.DoesOcclude(num2, num6 + 1, canSeeThroughTransparent))
                startSlope = DiscreteShadowCaster.GetSlope((double) num2 + 0.5, (double) num6 + 0.5, (double) cellPos.x, (double) cellPos.y, true);
              if (!DiscreteShadowCaster.DoesOcclude(num2 - 1, num6, canSeeThroughTransparent) && !visiblePoints.Contains(Grid.XYToCell(num2, num6)))
                visiblePoints.Add(Grid.XYToCell(num2, num6));
            }
          }
        }
        num3 = num6 + 1;
        break;
      case DiscreteShadowCaster.Octant.E_SE:
        num2 = cellPos.x + depth;
        if (num2 >= Grid.WidthInCells)
          return;
        int num7 = cellPos.y - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
        if (num7 < 0)
          num7 = 0;
        for (; DiscreteShadowCaster.GetSlope((double) num2, (double) num7, (double) cellPos.x, (double) cellPos.y, true) <= endSlope; ++num7)
        {
          if (DiscreteShadowCaster.GetVisDistance(num2, num7, cellPos.x, cellPos.y) <= num1)
          {
            if (DiscreteShadowCaster.DoesOcclude(num2, num7, canSeeThroughTransparent))
            {
              if (num7 - 1 >= 0 && !DiscreteShadowCaster.DoesOcclude(num2, num7 - 1, canSeeThroughTransparent) && !DiscreteShadowCaster.DoesOcclude(num2 - 1, num7 - 1, canSeeThroughTransparent))
                DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, DiscreteShadowCaster.GetSlope((double) num2 - 0.5, (double) num7 - 0.5, (double) cellPos.x, (double) cellPos.y, true), visiblePoints, canSeeThroughTransparent);
            }
            else
            {
              if (num7 - 1 >= 0 && DiscreteShadowCaster.DoesOcclude(num2, num7 - 1, canSeeThroughTransparent))
                startSlope = -DiscreteShadowCaster.GetSlope((double) num2 + 0.5, (double) num7 - 0.5, (double) cellPos.x, (double) cellPos.y, true);
              if (!DiscreteShadowCaster.DoesOcclude(num2 - 1, num7, canSeeThroughTransparent) && !visiblePoints.Contains(Grid.XYToCell(num2, num7)))
                visiblePoints.Add(Grid.XYToCell(num2, num7));
            }
          }
        }
        num3 = num7 - 1;
        break;
      case DiscreteShadowCaster.Octant.S_SE:
        num3 = cellPos.y - depth;
        if (num3 < 0)
          return;
        int num8 = cellPos.x + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
        if (num8 >= Grid.WidthInCells)
          num8 = Grid.WidthInCells - 1;
        for (; DiscreteShadowCaster.GetSlope((double) num8, (double) num3, (double) cellPos.x, (double) cellPos.y, false) <= endSlope; --num8)
        {
          if (DiscreteShadowCaster.GetVisDistance(num8, num3, cellPos.x, cellPos.y) <= num1)
          {
            if (DiscreteShadowCaster.DoesOcclude(num8, num3, canSeeThroughTransparent))
            {
              if (num8 + 1 < Grid.WidthInCells && !DiscreteShadowCaster.DoesOcclude(num8 + 1, num3, canSeeThroughTransparent) && !DiscreteShadowCaster.DoesOcclude(num8 + 1, num3 + 1, canSeeThroughTransparent))
              {
                double slope = DiscreteShadowCaster.GetSlope((double) num8 + 0.5, (double) num3 + 0.5, (double) cellPos.x, (double) cellPos.y, false);
                DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, slope, visiblePoints, canSeeThroughTransparent);
              }
            }
            else
            {
              if (num8 + 1 < Grid.WidthInCells && DiscreteShadowCaster.DoesOcclude(num8 + 1, num3, canSeeThroughTransparent))
                startSlope = -DiscreteShadowCaster.GetSlope((double) num8 + 0.5, (double) num3 - 0.5, (double) cellPos.x, (double) cellPos.y, false);
              if (!DiscreteShadowCaster.DoesOcclude(num8, num3 + 1, canSeeThroughTransparent) && !visiblePoints.Contains(Grid.XYToCell(num8, num3)))
                visiblePoints.Add(Grid.XYToCell(num8, num3));
            }
          }
        }
        num2 = num8 + 1;
        break;
      case DiscreteShadowCaster.Octant.S_SW:
        num3 = cellPos.y - depth;
        if (num3 < 0)
          return;
        int num9 = cellPos.x - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
        if (num9 < 0)
          num9 = 0;
        for (; DiscreteShadowCaster.GetSlope((double) num9, (double) num3, (double) cellPos.x, (double) cellPos.y, false) >= endSlope; ++num9)
        {
          if (DiscreteShadowCaster.GetVisDistance(num9, num3, cellPos.x, cellPos.y) <= num1)
          {
            if (DiscreteShadowCaster.DoesOcclude(num9, num3, canSeeThroughTransparent))
            {
              if (num9 - 1 >= 0 && !DiscreteShadowCaster.DoesOcclude(num9 - 1, num3, canSeeThroughTransparent) && !DiscreteShadowCaster.DoesOcclude(num9 - 1, num3 + 1, canSeeThroughTransparent))
              {
                double slope = DiscreteShadowCaster.GetSlope((double) num9 - 0.5, (double) num3 + 0.5, (double) cellPos.x, (double) cellPos.y, false);
                DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, slope, visiblePoints, canSeeThroughTransparent);
              }
            }
            else
            {
              if (num9 - 1 >= 0 && DiscreteShadowCaster.DoesOcclude(num9 - 1, num3, canSeeThroughTransparent))
                startSlope = DiscreteShadowCaster.GetSlope((double) num9 - 0.5, (double) num3 - 0.5, (double) cellPos.x, (double) cellPos.y, false);
              if (!DiscreteShadowCaster.DoesOcclude(num9, num3 + 1, canSeeThroughTransparent) && !visiblePoints.Contains(Grid.XYToCell(num9, num3)))
                visiblePoints.Add(Grid.XYToCell(num9, num3));
            }
          }
        }
        num2 = num9 - 1;
        break;
      case DiscreteShadowCaster.Octant.W_SW:
        num2 = cellPos.x - depth;
        if (num2 < 0)
          return;
        int num10 = cellPos.y - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
        if (num10 < 0)
          num10 = 0;
        for (; DiscreteShadowCaster.GetSlope((double) num2, (double) num10, (double) cellPos.x, (double) cellPos.y, true) >= endSlope; ++num10)
        {
          if (DiscreteShadowCaster.GetVisDistance(num2, num10, cellPos.x, cellPos.y) <= num1)
          {
            if (DiscreteShadowCaster.DoesOcclude(num2, num10, canSeeThroughTransparent))
            {
              if (num10 - 1 >= 0 && !DiscreteShadowCaster.DoesOcclude(num2, num10 - 1, canSeeThroughTransparent) && !DiscreteShadowCaster.DoesOcclude(num2 + 1, num10 - 1, canSeeThroughTransparent))
                DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, DiscreteShadowCaster.GetSlope((double) num2 + 0.5, (double) num10 - 0.5, (double) cellPos.x, (double) cellPos.y, true), visiblePoints, canSeeThroughTransparent);
            }
            else
            {
              if (num10 - 1 >= 0 && DiscreteShadowCaster.DoesOcclude(num2, num10 - 1, canSeeThroughTransparent))
                startSlope = DiscreteShadowCaster.GetSlope((double) num2 - 0.5, (double) num10 - 0.5, (double) cellPos.x, (double) cellPos.y, true);
              if (!DiscreteShadowCaster.DoesOcclude(num2 + 1, num10, canSeeThroughTransparent) && !visiblePoints.Contains(Grid.XYToCell(num2, num10)))
                visiblePoints.Add(Grid.XYToCell(num2, num10));
            }
          }
        }
        num3 = num10 - 1;
        break;
      case DiscreteShadowCaster.Octant.W_NW:
        num2 = cellPos.x - depth;
        if (num2 < 0)
          return;
        int num11 = cellPos.y + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
        if (num11 >= Grid.HeightInCells)
          num11 = Grid.HeightInCells - 1;
        for (; DiscreteShadowCaster.GetSlope((double) num2, (double) num11, (double) cellPos.x, (double) cellPos.y, true) <= endSlope; --num11)
        {
          if (DiscreteShadowCaster.GetVisDistance(num2, num11, cellPos.x, cellPos.y) <= num1)
          {
            if (DiscreteShadowCaster.DoesOcclude(num2, num11, canSeeThroughTransparent))
            {
              if (num11 + 1 < Grid.HeightInCells && !DiscreteShadowCaster.DoesOcclude(num2, num11 + 1, canSeeThroughTransparent) && !DiscreteShadowCaster.DoesOcclude(num2 + 1, num11 + 1, canSeeThroughTransparent))
                DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, DiscreteShadowCaster.GetSlope((double) num2 + 0.5, (double) num11 + 0.5, (double) cellPos.x, (double) cellPos.y, true), visiblePoints, canSeeThroughTransparent);
            }
            else
            {
              if (num11 + 1 < Grid.HeightInCells && DiscreteShadowCaster.DoesOcclude(num2, num11 + 1, canSeeThroughTransparent))
                startSlope = -DiscreteShadowCaster.GetSlope((double) num2 - 0.5, (double) num11 + 0.5, (double) cellPos.x, (double) cellPos.y, true);
              if (!DiscreteShadowCaster.DoesOcclude(num2 + 1, num11, canSeeThroughTransparent) && !visiblePoints.Contains(Grid.XYToCell(num2, num11)))
                visiblePoints.Add(Grid.XYToCell(num2, num11));
            }
          }
        }
        num3 = num11 + 1;
        break;
    }
    if (num2 < 0)
      num2 = 0;
    else if (num2 >= Grid.WidthInCells)
      num2 = Grid.WidthInCells - 1;
    if (num3 < 0)
      num3 = 0;
    else if (num3 >= Grid.HeightInCells)
      num3 = Grid.HeightInCells - 1;
    if (!(depth < range & !DiscreteShadowCaster.DoesOcclude(num2, num3, canSeeThroughTransparent)))
      return;
    DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, endSlope, visiblePoints, canSeeThroughTransparent);
  }

  private static double GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert) => pInvert ? (pY1 - pY2) / (pX1 - pX2) : (pX1 - pX2) / (pY1 - pY2);

  private static int GetVisDistance(int pX1, int pY1, int pX2, int pY2) => (pX1 - pX2) * (pX1 - pX2) + (pY1 - pY2) * (pY1 - pY2);

  public enum Octant
  {
    N_NW,
    N_NE,
    E_NE,
    E_SE,
    S_SE,
    S_SW,
    W_SW,
    W_NW,
  }
}
