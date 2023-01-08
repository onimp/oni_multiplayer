// Decompiled with JetBrains decompiler
// Type: SoundUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class SoundUtil
{
  public static float GetLiquidDepth(int cell)
  {
    float num = (float) (0.0 + (double) Grid.Mass[cell] * (Grid.Element[cell].IsLiquid ? 1.0 : 0.0));
    int index = Grid.CellBelow(cell);
    if (Grid.IsValidCell(index))
      num += Grid.Mass[index] * (Grid.Element[index].IsLiquid ? 1f : 0.0f);
    return Mathf.Min(num / 1000f, 1f);
  }

  public static float GetLiquidVolume(float mass) => Mathf.Min(mass / 100f, 1f);
}
