// Decompiled with JetBrains decompiler
// Type: LiquidTileEdging
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class LiquidTileEdging
{
  private void Update()
  {
    int min_x;
    int min_y;
    int max_x;
    int max_y;
    Grid.GetVisibleExtents(out min_x, out min_y, out max_x, out max_y);
    int num1 = Math.Max(0, min_x);
    int num2 = Math.Max(0, min_y);
    int num3 = Mathf.Min(num1, Grid.WidthInCells - 1);
    int num4 = Mathf.Min(num2, Grid.HeightInCells - 1);
    int num5 = Mathf.CeilToInt((float) max_x);
    int num6 = Mathf.CeilToInt((float) max_y);
    int num7 = Mathf.Max(num5, 0);
    int num8 = Mathf.Max(num6, 0);
    int num9 = Mathf.Min(num7, Grid.WidthInCells - 1);
    int num10 = Mathf.Min(num8, Grid.HeightInCells - 1);
    int num11 = 0;
    int num12 = 0;
    int num13 = 0;
    for (int index1 = num4; index1 < num10; ++index1)
    {
      for (int index2 = num3; index2 < num9; ++index2)
      {
        int index3 = index1 * Grid.WidthInCells + index2;
        Element element = Grid.Element[index3];
        if (element.IsSolid)
          ++num11;
        else if (element.IsLiquid)
          ++num12;
        else
          ++num13;
      }
    }
  }
}
