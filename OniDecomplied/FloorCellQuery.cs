// Decompiled with JetBrains decompiler
// Type: FloorCellQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class FloorCellQuery : PathFinderQuery
{
  public List<int> result_cells = new List<int>();
  private int max_results;
  private int adjacent_cells_buffer;

  public FloorCellQuery Reset(int max_results, int adjacent_cells_buffer = 0)
  {
    this.max_results = max_results;
    this.adjacent_cells_buffer = adjacent_cells_buffer;
    this.result_cells.Clear();
    return this;
  }

  public override bool IsMatch(int cell, int parent_cell, int cost)
  {
    if (!this.result_cells.Contains(cell) && this.CheckValidFloorCell(cell))
      this.result_cells.Add(cell);
    return this.result_cells.Count >= this.max_results;
  }

  private bool CheckValidFloorCell(int testCell)
  {
    if (!Grid.IsValidCell(testCell) || Grid.IsSolidCell(testCell))
      return false;
    int cellInDirection1 = Grid.GetCellInDirection(testCell, Direction.Up);
    int cellInDirection2 = Grid.GetCellInDirection(testCell, Direction.Down);
    if (Grid.ObjectLayers[1].ContainsKey(testCell) || !Grid.IsValidCell(cellInDirection2) || !Grid.IsSolidCell(cellInDirection2) || !Grid.IsValidCell(cellInDirection1) || Grid.IsSolidCell(cellInDirection1))
      return false;
    int cell1 = testCell;
    int cell2 = testCell;
    for (int index = 0; index < this.adjacent_cells_buffer; ++index)
    {
      cell1 = Grid.CellLeft(cell1);
      cell2 = Grid.CellRight(cell2);
      if (!Grid.IsValidCell(cell1) || Grid.IsSolidCell(cell1) || !Grid.IsValidCell(cell2) || Grid.IsSolidCell(cell2))
        return false;
    }
    return true;
  }
}
