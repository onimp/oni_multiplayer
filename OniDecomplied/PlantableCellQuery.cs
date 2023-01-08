// Decompiled with JetBrains decompiler
// Type: PlantableCellQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PlantableCellQuery : PathFinderQuery
{
  public List<int> result_cells = new List<int>();
  private PlantableSeed seed;
  private int max_results;
  private int plantDetectionRadius = 6;
  private int maxPlantsInRadius = 2;

  public PlantableCellQuery Reset(PlantableSeed seed, int max_results)
  {
    this.seed = seed;
    this.max_results = max_results;
    this.result_cells.Clear();
    return this;
  }

  public override bool IsMatch(int cell, int parent_cell, int cost)
  {
    if (!this.result_cells.Contains(cell) && this.CheckValidPlotCell(this.seed, cell))
      this.result_cells.Add(cell);
    return this.result_cells.Count >= this.max_results;
  }

  private bool CheckValidPlotCell(PlantableSeed seed, int plant_cell)
  {
    if (!Grid.IsValidCell(plant_cell))
      return false;
    int num = seed.Direction != SingleEntityReceptacle.ReceptacleDirection.Bottom ? Grid.CellBelow(plant_cell) : Grid.CellAbove(plant_cell);
    if (!Grid.IsValidCell(num) || !Grid.Solid[num] || Object.op_Implicit((Object) Grid.Objects[plant_cell, 5]) || Object.op_Implicit((Object) Grid.Objects[plant_cell, 1]))
      return false;
    GameObject gameObject = Grid.Objects[num, 1];
    if (Object.op_Implicit((Object) gameObject))
    {
      PlantablePlot component = gameObject.GetComponent<PlantablePlot>();
      if (Object.op_Equality((Object) component, (Object) null) || component.Direction != seed.Direction || Object.op_Inequality((Object) component.Occupant, (Object) null))
        return false;
    }
    else if (!seed.TestSuitableGround(plant_cell) || PlantableCellQuery.CountNearbyPlants(num, this.plantDetectionRadius) > this.maxPlantsInRadius)
      return false;
    return true;
  }

  private static int CountNearbyPlants(int cell, int radius)
  {
    int x = 0;
    int y = 0;
    Grid.PosToXY(Grid.CellToPos(cell), out x, out y);
    int num1 = radius * 2;
    int x_bottomLeft = x - radius;
    int y_bottomLeft = y - radius;
    ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
    GameScenePartitioner.Instance.GatherEntries(x_bottomLeft, y_bottomLeft, num1, num1, GameScenePartitioner.Instance.plants, (List<ScenePartitionerEntry>) gathered_entries);
    int num2 = 0;
    foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
    {
      if (!Object.op_Implicit((Object) ((Component) partitionerEntry.obj).GetComponent<TreeBud>()))
        ++num2;
    }
    gathered_entries.Recycle();
    return num2;
  }
}
