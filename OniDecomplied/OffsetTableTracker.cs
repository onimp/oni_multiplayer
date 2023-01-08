// Decompiled with JetBrains decompiler
// Type: OffsetTableTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class OffsetTableTracker : OffsetTracker
{
  private readonly CellOffset[][] table;
  public HandleVector<int>.Handle solidPartitionerEntry;
  public HandleVector<int>.Handle validNavCellChangedPartitionerEntry;
  private static NavGrid navGridImpl;
  private KMonoBehaviour cmp;
  private int[] DEBUG_rowValidIdx;

  private static NavGrid navGrid
  {
    get
    {
      if (OffsetTableTracker.navGridImpl == null)
        OffsetTableTracker.navGridImpl = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
      return OffsetTableTracker.navGridImpl;
    }
  }

  public OffsetTableTracker(CellOffset[][] table, KMonoBehaviour cmp)
  {
    this.table = table;
    this.cmp = cmp;
  }

  protected override void UpdateCell(int previous_cell, int current_cell)
  {
    if (previous_cell == current_cell)
      return;
    base.UpdateCell(previous_cell, current_cell);
    Extents extents = new Extents(current_cell, this.table);
    extents.height += 2;
    --extents.y;
    if (!this.solidPartitionerEntry.IsValid())
    {
      this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("OffsetTableTracker.UpdateCell", (object) ((Component) this.cmp).gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnCellChanged));
      this.validNavCellChangedPartitionerEntry = GameScenePartitioner.Instance.Add("OffsetTableTracker.UpdateCell", (object) ((Component) this.cmp).gameObject, extents, GameScenePartitioner.Instance.validNavCellChangedLayer, new Action<object>(this.OnCellChanged));
    }
    else
    {
      GameScenePartitioner.Instance.UpdatePosition(this.solidPartitionerEntry, extents);
      GameScenePartitioner.Instance.UpdatePosition(this.validNavCellChangedPartitionerEntry, extents);
    }
    this.offsets = (CellOffset[]) null;
  }

  private static bool IsValidRow(int current_cell, CellOffset[] row, int rowIdx, int[] debugIdxs)
  {
    for (int index = 1; index < row.Length; ++index)
    {
      int num = Grid.OffsetCell(current_cell, row[index]);
      if (!Grid.IsValidCell(num) || Grid.Solid[num])
        return false;
    }
    return true;
  }

  private void UpdateOffsets(int cell, CellOffset[][] table)
  {
    HashSetPool<CellOffset, OffsetTableTracker>.PooledHashSet pooledHashSet = HashSetPool<CellOffset, OffsetTableTracker>.Allocate();
    if (Grid.IsValidCell(cell))
    {
      for (int rowIdx = 0; rowIdx < table.Length; ++rowIdx)
      {
        CellOffset[] row = table[rowIdx];
        if (!((HashSet<CellOffset>) pooledHashSet).Contains(row[0]))
        {
          int cell1 = Grid.OffsetCell(cell, row[0]);
          for (int index = 0; index < OffsetTableTracker.navGrid.ValidNavTypes.Length; ++index)
          {
            NavType validNavType = OffsetTableTracker.navGrid.ValidNavTypes[index];
            if (validNavType != NavType.Tube && OffsetTableTracker.navGrid.NavTable.IsValid(cell1, validNavType) && OffsetTableTracker.IsValidRow(cell, row, rowIdx, this.DEBUG_rowValidIdx))
            {
              ((HashSet<CellOffset>) pooledHashSet).Add(row[0]);
              break;
            }
          }
        }
      }
    }
    if (this.offsets == null || this.offsets.Length != ((HashSet<CellOffset>) pooledHashSet).Count)
      this.offsets = new CellOffset[((HashSet<CellOffset>) pooledHashSet).Count];
    ((HashSet<CellOffset>) pooledHashSet).CopyTo(this.offsets);
    pooledHashSet.Recycle();
  }

  protected override void UpdateOffsets(int current_cell)
  {
    base.UpdateOffsets(current_cell);
    this.UpdateOffsets(current_cell, this.table);
  }

  private void OnCellChanged(object data) => this.offsets = (CellOffset[]) null;

  public override void Clear()
  {
    GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
    GameScenePartitioner.Instance.Free(ref this.validNavCellChangedPartitionerEntry);
  }

  public static void OnPathfindingInvalidated() => OffsetTableTracker.navGridImpl = (NavGrid) null;
}
