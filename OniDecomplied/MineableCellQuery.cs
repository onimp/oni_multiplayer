// Decompiled with JetBrains decompiler
// Type: MineableCellQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class MineableCellQuery : PathFinderQuery
{
  public List<int> result_cells = new List<int>();
  private Tag element;
  private int max_results;
  public static List<Direction> DIRECTION_CHECKS = new List<Direction>()
  {
    Direction.Down,
    Direction.Right,
    Direction.Left,
    Direction.Up
  };

  public MineableCellQuery Reset(Tag element, int max_results)
  {
    this.element = element;
    this.max_results = max_results;
    this.result_cells.Clear();
    return this;
  }

  public override bool IsMatch(int cell, int parent_cell, int cost)
  {
    if (!this.result_cells.Contains(cell) && this.CheckValidMineCell(this.element, cell))
      this.result_cells.Add(cell);
    return this.result_cells.Count >= this.max_results;
  }

  private bool CheckValidMineCell(Tag element, int testCell)
  {
    if (!Grid.IsValidCell(testCell))
      return false;
    foreach (Direction d in MineableCellQuery.DIRECTION_CHECKS)
    {
      int cellInDirection = Grid.GetCellInDirection(testCell, d);
      if (Grid.IsValidCell(cellInDirection) && Grid.IsSolidCell(cellInDirection) && !Grid.Foundation[cellInDirection] && Tag.op_Equality(Grid.Element[cellInDirection].tag, element))
        return true;
    }
    return false;
  }
}
