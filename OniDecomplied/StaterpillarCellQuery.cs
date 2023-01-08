// Decompiled with JetBrains decompiler
// Type: StaterpillarCellQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class StaterpillarCellQuery : PathFinderQuery
{
  public List<int> result_cells = new List<int>();
  private int max_results;
  private GameObject tester;
  private ObjectLayer connectorLayer;

  public StaterpillarCellQuery Reset(int max_results, GameObject tester, ObjectLayer conduitLayer)
  {
    this.max_results = max_results;
    this.tester = tester;
    this.result_cells.Clear();
    ObjectLayer objectLayer;
    switch (conduitLayer)
    {
      case ObjectLayer.GasConduit:
        objectLayer = ObjectLayer.GasConduitConnection;
        break;
      case ObjectLayer.LiquidConduit:
        objectLayer = ObjectLayer.LiquidConduitConnection;
        break;
      case ObjectLayer.SolidConduit:
        objectLayer = ObjectLayer.SolidConduitConnection;
        break;
      case ObjectLayer.Wire:
        objectLayer = ObjectLayer.WireConnectors;
        break;
      default:
        objectLayer = conduitLayer;
        break;
    }
    this.connectorLayer = objectLayer;
    return this;
  }

  public override bool IsMatch(int cell, int parent_cell, int cost)
  {
    if (!this.result_cells.Contains(cell) && this.CheckValidRoofCell(cell))
      this.result_cells.Add(cell);
    return this.result_cells.Count >= this.max_results;
  }

  private bool CheckValidRoofCell(int testCell)
  {
    if (!this.tester.GetComponent<Navigator>().NavGrid.NavTable.IsValid(testCell, NavType.Ceiling))
      return false;
    int cellInDirection = Grid.GetCellInDirection(testCell, Direction.Down);
    return !Grid.ObjectLayers[1].ContainsKey(testCell) && !Grid.ObjectLayers[1].ContainsKey(cellInDirection) && !Object.op_Implicit((Object) Grid.Objects[cellInDirection, (int) this.connectorLayer]) && Grid.IsValidBuildingCell(testCell) && Grid.IsValidCell(cellInDirection) && Grid.IsValidBuildingCell(cellInDirection) && !Grid.IsSolidCell(cellInDirection);
  }
}
