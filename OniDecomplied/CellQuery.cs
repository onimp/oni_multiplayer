// Decompiled with JetBrains decompiler
// Type: CellQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class CellQuery : PathFinderQuery
{
  private int targetCell;

  public CellQuery Reset(int target_cell)
  {
    this.targetCell = target_cell;
    return this;
  }

  public override bool IsMatch(int cell, int parent_cell, int cost) => cell == this.targetCell;
}
