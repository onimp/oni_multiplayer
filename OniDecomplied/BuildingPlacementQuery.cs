// Decompiled with JetBrains decompiler
// Type: BuildingPlacementQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementQuery : PathFinderQuery
{
  public List<int> result_cells = new List<int>();
  private int max_results;
  private GameObject toPlace;
  private CellOffset[] cellOffsets;

  public BuildingPlacementQuery Reset(int max_results, GameObject toPlace)
  {
    this.max_results = max_results;
    this.toPlace = toPlace;
    this.cellOffsets = toPlace.GetComponent<OccupyArea>().OccupiedCellsOffsets;
    this.result_cells.Clear();
    return this;
  }

  public override bool IsMatch(int cell, int parent_cell, int cost)
  {
    if (!this.result_cells.Contains(cell) && this.CheckValidPlaceCell(cell))
      this.result_cells.Add(cell);
    return this.result_cells.Count >= this.max_results;
  }

  private bool CheckValidPlaceCell(int testCell)
  {
    if (!Grid.IsValidCell(testCell) || Grid.IsSolidCell(testCell) || Grid.ObjectLayers[1].ContainsKey(testCell))
      return false;
    bool flag = true;
    int widthInCells = this.toPlace.GetComponent<OccupyArea>().GetWidthInCells();
    int cell = testCell;
    for (int index = 0; index < widthInCells; ++index)
    {
      int cellInDirection = Grid.GetCellInDirection(cell, Direction.Down);
      if (!Grid.IsValidCell(cellInDirection) || !Grid.IsSolidCell(cellInDirection))
      {
        flag = false;
        break;
      }
      cell = Grid.GetCellInDirection(cell, Direction.Right);
    }
    if (flag)
    {
      for (int index = 0; index < this.cellOffsets.Length; ++index)
      {
        CellOffset cellOffset = this.cellOffsets[index];
        int num = Grid.OffsetCell(testCell, cellOffset);
        if (!Grid.IsValidCell(num) || Grid.IsSolidCell(num) || !Grid.IsValidBuildingCell(num) || Grid.ObjectLayers[1].ContainsKey(num))
        {
          flag = false;
          break;
        }
      }
    }
    return flag;
  }
}
