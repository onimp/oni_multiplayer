// Decompiled with JetBrains decompiler
// Type: LiquidFetchMask
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class LiquidFetchMask
{
  private bool[] isLiquidAvailable;
  private CellOffset maxOffset;

  public LiquidFetchMask(CellOffset[][] offset_table)
  {
    for (int index1 = 0; index1 < offset_table.Length; ++index1)
    {
      for (int index2 = 0; index2 < offset_table[index1].Length; ++index2)
      {
        this.maxOffset.x = Math.Max(this.maxOffset.x, Math.Abs(offset_table[index1][index2].x));
        this.maxOffset.y = Math.Max(this.maxOffset.y, Math.Abs(offset_table[index1][index2].y));
      }
    }
    this.isLiquidAvailable = new bool[Grid.CellCount];
    for (int cell = 0; cell < Grid.CellCount; ++cell)
      this.RefreshCell(cell);
  }

  private void RefreshCell(int cell)
  {
    CellOffset offset = Grid.GetOffset(cell);
    for (int y = Math.Max(0, offset.y - this.maxOffset.y); y < Grid.HeightInCells && y < offset.y + this.maxOffset.y; ++y)
    {
      for (int x = Math.Max(0, offset.x - this.maxOffset.x); x < Grid.WidthInCells && x < offset.x + this.maxOffset.x; ++x)
      {
        if (Grid.Element[Grid.XYToCell(x, y)].IsLiquid)
        {
          this.isLiquidAvailable[cell] = true;
          return;
        }
      }
    }
    this.isLiquidAvailable[cell] = false;
  }

  public void MarkDirty(int cell) => this.RefreshCell(cell);

  public bool IsLiquidAvailable(int cell) => this.isLiquidAvailable[cell];

  public void Destroy() => this.isLiquidAvailable = (bool[]) null;
}
