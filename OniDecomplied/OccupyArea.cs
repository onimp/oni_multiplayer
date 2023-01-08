// Decompiled with JetBrains decompiler
// Type: OccupyArea
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/OccupyArea")]
public class OccupyArea : KMonoBehaviour
{
  public CellOffset[] OccupiedCellsOffsets;
  private CellOffset[] AboveOccupiedCellOffsets;
  private CellOffset[] BelowOccupiedCellOffsets;
  private int[] occupiedGridCells;
  public ObjectLayer[] objectLayers = new ObjectLayer[0];
  [SerializeField]
  private bool applyToCells = true;

  public bool ApplyToCells
  {
    get => this.applyToCells;
    set
    {
      if (value == this.applyToCells)
        return;
      if (value)
        this.UpdateOccupiedArea();
      else
        this.ClearOccupiedArea();
      this.applyToCells = value;
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!this.applyToCells)
      return;
    this.UpdateOccupiedArea();
  }

  private void ValidatePosition()
  {
    if (Grid.IsValidCell(Grid.PosToCell((KMonoBehaviour) this)))
      return;
    Debug.LogWarning((object) (((Object) this).name + " is outside the grid! DELETING!"));
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  [System.Runtime.Serialization.OnSerializing]
  private void OnSerializing() => this.ValidatePosition();

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized() => this.ValidatePosition();

  public void SetCellOffsets(CellOffset[] cells) => this.OccupiedCellsOffsets = cells;

  public bool CheckIsOccupying(int checkCell)
  {
    int cell = Grid.PosToCell(((Component) this).gameObject);
    if (checkCell == cell)
      return true;
    foreach (CellOffset occupiedCellsOffset in this.OccupiedCellsOffsets)
    {
      if (Grid.OffsetCell(cell, occupiedCellsOffset) == checkCell)
        return true;
    }
    return false;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    this.ClearOccupiedArea();
  }

  private void ClearOccupiedArea()
  {
    if (this.occupiedGridCells == null)
      return;
    foreach (ObjectLayer objectLayer in this.objectLayers)
    {
      if (objectLayer != ObjectLayer.NumLayers)
      {
        foreach (int occupiedGridCell in this.occupiedGridCells)
        {
          if (Object.op_Equality((Object) Grid.Objects[occupiedGridCell, (int) objectLayer], (Object) ((Component) this).gameObject))
            Grid.Objects[occupiedGridCell, (int) objectLayer] = (GameObject) null;
        }
      }
    }
  }

  public void UpdateOccupiedArea()
  {
    if (this.objectLayers.Length == 0)
      return;
    if (this.occupiedGridCells == null)
      this.occupiedGridCells = new int[this.OccupiedCellsOffsets.Length];
    this.ClearOccupiedArea();
    int cell1 = Grid.PosToCell(((Component) this).gameObject);
    foreach (ObjectLayer objectLayer in this.objectLayers)
    {
      if (objectLayer != ObjectLayer.NumLayers)
      {
        for (int index = 0; index < this.OccupiedCellsOffsets.Length; ++index)
        {
          CellOffset occupiedCellsOffset = this.OccupiedCellsOffsets[index];
          int cell2 = Grid.OffsetCell(cell1, occupiedCellsOffset);
          Grid.Objects[cell2, (int) objectLayer] = ((Component) this).gameObject;
          this.occupiedGridCells[index] = cell2;
        }
      }
    }
  }

  public int GetWidthInCells()
  {
    int val1_1 = int.MaxValue;
    int val1_2 = int.MinValue;
    foreach (CellOffset occupiedCellsOffset in this.OccupiedCellsOffsets)
    {
      val1_1 = Math.Min(val1_1, occupiedCellsOffset.x);
      val1_2 = Math.Max(val1_2, occupiedCellsOffset.x);
    }
    return val1_2 - val1_1 + 1;
  }

  public int GetHeightInCells()
  {
    int val1_1 = int.MaxValue;
    int val1_2 = int.MinValue;
    foreach (CellOffset occupiedCellsOffset in this.OccupiedCellsOffsets)
    {
      val1_1 = Math.Min(val1_1, occupiedCellsOffset.y);
      val1_2 = Math.Max(val1_2, occupiedCellsOffset.y);
    }
    return val1_2 - val1_1 + 1;
  }

  public Extents GetExtents() => new Extents(Grid.PosToCell(((Component) this).gameObject), this.OccupiedCellsOffsets);

  public Extents GetExtents(Orientation orientation) => new Extents(Grid.PosToCell(((Component) this).gameObject), this.OccupiedCellsOffsets, orientation);

  private void OnDrawGizmosSelected()
  {
    int cell = Grid.PosToCell(((Component) this).gameObject);
    if (this.OccupiedCellsOffsets != null)
    {
      foreach (CellOffset occupiedCellsOffset in this.OccupiedCellsOffsets)
      {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.op_Addition(Vector3.op_Addition(Grid.CellToPos(Grid.OffsetCell(cell, occupiedCellsOffset)), Vector3.op_Division(Vector3.right, 2f)), Vector3.op_Division(Vector3.up, 2f)), Vector3.one);
      }
    }
    if (this.AboveOccupiedCellOffsets != null)
    {
      foreach (CellOffset occupiedCellOffset in this.AboveOccupiedCellOffsets)
      {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.op_Addition(Vector3.op_Addition(Grid.CellToPos(Grid.OffsetCell(cell, occupiedCellOffset)), Vector3.op_Division(Vector3.right, 2f)), Vector3.op_Division(Vector3.up, 2f)), Vector3.op_Multiply(Vector3.one, 0.9f));
      }
    }
    if (this.BelowOccupiedCellOffsets == null)
      return;
    foreach (CellOffset occupiedCellOffset in this.BelowOccupiedCellOffsets)
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(Vector3.op_Addition(Vector3.op_Addition(Grid.CellToPos(Grid.OffsetCell(cell, occupiedCellOffset)), Vector3.op_Division(Vector3.right, 2f)), Vector3.op_Division(Vector3.up, 2f)), Vector3.op_Multiply(Vector3.one, 0.9f));
    }
  }

  public bool CanOccupyArea(int rootCell, ObjectLayer layer)
  {
    for (int index = 0; index < this.OccupiedCellsOffsets.Length; ++index)
    {
      CellOffset occupiedCellsOffset = this.OccupiedCellsOffsets[index];
      int cell = Grid.OffsetCell(rootCell, occupiedCellsOffset);
      if (Object.op_Inequality((Object) Grid.Objects[cell, (int) layer], (Object) null))
        return false;
    }
    return true;
  }

  public bool TestArea(int rootCell, object data, Func<int, object, bool> testDelegate)
  {
    for (int index = 0; index < this.OccupiedCellsOffsets.Length; ++index)
    {
      CellOffset occupiedCellsOffset = this.OccupiedCellsOffsets[index];
      int num = Grid.OffsetCell(rootCell, occupiedCellsOffset);
      if (!testDelegate(num, data))
        return false;
    }
    return true;
  }

  public bool TestAreaAbove(int rootCell, object data, Func<int, object, bool> testDelegate)
  {
    if (this.AboveOccupiedCellOffsets == null)
    {
      List<CellOffset> cellOffsetList = new List<CellOffset>();
      for (int index = 0; index < this.OccupiedCellsOffsets.Length; ++index)
      {
        CellOffset cellOffset;
        // ISSUE: explicit constructor call
        ((CellOffset) ref cellOffset).\u002Ector(this.OccupiedCellsOffsets[index].x, this.OccupiedCellsOffsets[index].y + 1);
        if (Array.IndexOf<CellOffset>(this.OccupiedCellsOffsets, cellOffset) == -1)
          cellOffsetList.Add(cellOffset);
      }
      this.AboveOccupiedCellOffsets = cellOffsetList.ToArray();
    }
    for (int index = 0; index < this.AboveOccupiedCellOffsets.Length; ++index)
    {
      int num = Grid.OffsetCell(rootCell, this.AboveOccupiedCellOffsets[index]);
      if (!testDelegate(num, data))
        return false;
    }
    return true;
  }

  public bool TestAreaBelow(int rootCell, object data, Func<int, object, bool> testDelegate)
  {
    if (this.BelowOccupiedCellOffsets == null)
    {
      List<CellOffset> cellOffsetList = new List<CellOffset>();
      for (int index = 0; index < this.OccupiedCellsOffsets.Length; ++index)
      {
        CellOffset cellOffset;
        // ISSUE: explicit constructor call
        ((CellOffset) ref cellOffset).\u002Ector(this.OccupiedCellsOffsets[index].x, this.OccupiedCellsOffsets[index].y - 1);
        if (Array.IndexOf<CellOffset>(this.OccupiedCellsOffsets, cellOffset) == -1)
          cellOffsetList.Add(cellOffset);
      }
      this.BelowOccupiedCellOffsets = cellOffsetList.ToArray();
    }
    for (int index = 0; index < this.BelowOccupiedCellOffsets.Length; ++index)
    {
      int num = Grid.OffsetCell(rootCell, this.BelowOccupiedCellOffsets[index]);
      if (!testDelegate(num, data))
        return false;
    }
    return true;
  }
}
