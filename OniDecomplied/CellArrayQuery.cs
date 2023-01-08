// Decompiled with JetBrains decompiler
// Type: CellArrayQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class CellArrayQuery : PathFinderQuery
{
  private int[] targetCells;

  public CellArrayQuery Reset(int[] target_cells)
  {
    this.targetCells = target_cells;
    return this;
  }

  public override bool IsMatch(int cell, int parent_cell, int cost)
  {
    for (int index = 0; index < this.targetCells.Length; ++index)
    {
      if (this.targetCells[index] == cell)
        return true;
    }
    return false;
  }
}
