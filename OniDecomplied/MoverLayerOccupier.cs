// Decompiled with JetBrains decompiler
// Type: MoverLayerOccupier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AntiCluster")]
public class MoverLayerOccupier : KMonoBehaviour, ISim200ms
{
  private int previousCell = Grid.InvalidCell;
  public ObjectLayer[] objectLayers;
  public CellOffset[] cellOffsets;

  private void RefreshCellOccupy()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    foreach (CellOffset cellOffset in this.cellOffsets)
    {
      int current_cell = Grid.OffsetCell(cell, cellOffset);
      if (this.previousCell != Grid.InvalidCell)
        this.UpdateCell(Grid.OffsetCell(this.previousCell, cellOffset), current_cell);
      else
        this.UpdateCell(this.previousCell, current_cell);
    }
    this.previousCell = cell;
  }

  public void Sim200ms(float dt) => this.RefreshCellOccupy();

  private void UpdateCell(int previous_cell, int current_cell)
  {
    foreach (ObjectLayer objectLayer in this.objectLayers)
    {
      if (previous_cell != Grid.InvalidCell && previous_cell != current_cell && Object.op_Equality((Object) Grid.Objects[previous_cell, (int) objectLayer], (Object) ((Component) this).gameObject))
        Grid.Objects[previous_cell, (int) objectLayer] = (GameObject) null;
      GameObject gameObject = Grid.Objects[current_cell, (int) objectLayer];
      if (Object.op_Equality((Object) gameObject, (Object) null))
        Grid.Objects[current_cell, (int) objectLayer] = ((Component) this).gameObject;
      else if (((Component) this).GetComponent<KPrefabID>().InstanceID > gameObject.GetComponent<KPrefabID>().InstanceID)
        Grid.Objects[current_cell, (int) objectLayer] = ((Component) this).gameObject;
    }
  }

  private void CleanUpOccupiedCells()
  {
    int cell1 = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    foreach (CellOffset cellOffset in this.cellOffsets)
    {
      int cell2 = Grid.OffsetCell(cell1, cellOffset);
      foreach (ObjectLayer objectLayer in this.objectLayers)
      {
        if (Object.op_Equality((Object) Grid.Objects[cell2, (int) objectLayer], (Object) ((Component) this).gameObject))
          Grid.Objects[cell2, (int) objectLayer] = (GameObject) null;
      }
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.RefreshCellOccupy();
  }

  protected virtual void OnCleanUp()
  {
    this.CleanUpOccupiedCells();
    base.OnCleanUp();
  }
}
