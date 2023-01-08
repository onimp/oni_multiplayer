// Decompiled with JetBrains decompiler
// Type: AnimTileableController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/AnimTileableController")]
public class AnimTileableController : KMonoBehaviour
{
  private HandleVector<int>.Handle partitionerEntry;
  public ObjectLayer objectLayer = ObjectLayer.Building;
  public Tag[] tags;
  private Extents extents;
  public string leftName = "#cap_left";
  public string rightName = "#cap_right";
  public string topName = "#cap_top";
  public string bottomName = "#cap_bottom";
  private KAnimSynchronizedController left;
  private KAnimSynchronizedController right;
  private KAnimSynchronizedController top;
  private KAnimSynchronizedController bottom;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (this.tags != null && this.tags.Length != 0)
      return;
    this.tags = new Tag[1]
    {
      ((Component) this).GetComponent<KPrefabID>().PrefabTag
    };
  }

  protected virtual void OnSpawn()
  {
    OccupyArea component1 = ((Component) this).GetComponent<OccupyArea>();
    this.extents = !Object.op_Inequality((Object) component1, (Object) null) ? ((Component) this).GetComponent<Building>().GetExtents() : component1.GetExtents();
    this.partitionerEntry = GameScenePartitioner.Instance.Add("AnimTileable.OnSpawn", (object) ((Component) this).gameObject, new Extents(this.extents.x - 1, this.extents.y - 1, this.extents.width + 2, this.extents.height + 2), GameScenePartitioner.Instance.objectLayers[(int) this.objectLayer], new Action<object>(this.OnNeighbourCellsUpdated));
    KBatchedAnimController component2 = ((Component) this).GetComponent<KBatchedAnimController>();
    this.left = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) component2.GetLayer(), this.leftName);
    this.right = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) component2.GetLayer(), this.rightName);
    this.top = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) component2.GetLayer(), this.topName);
    this.bottom = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) component2.GetLayer(), this.bottomName);
    this.UpdateEndCaps();
  }

  protected virtual void OnCleanUp()
  {
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    base.OnCleanUp();
  }

  private void UpdateEndCaps()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    bool enable1 = true;
    bool enable2 = true;
    bool enable3 = true;
    bool enable4 = true;
    int x;
    int y;
    Grid.CellToXY(cell, out x, out y);
    CellOffset rotatedCellOffset1;
    // ISSUE: explicit constructor call
    ((CellOffset) ref rotatedCellOffset1).\u002Ector(this.extents.x - x - 1, 0);
    CellOffset rotatedCellOffset2;
    // ISSUE: explicit constructor call
    ((CellOffset) ref rotatedCellOffset2).\u002Ector(this.extents.x - x + this.extents.width, 0);
    CellOffset rotatedCellOffset3;
    // ISSUE: explicit constructor call
    ((CellOffset) ref rotatedCellOffset3).\u002Ector(0, this.extents.y - y + this.extents.height);
    CellOffset rotatedCellOffset4;
    // ISSUE: explicit constructor call
    ((CellOffset) ref rotatedCellOffset4).\u002Ector(0, this.extents.y - y - 1);
    Rotatable component = ((Component) this).GetComponent<Rotatable>();
    if (Object.op_Implicit((Object) component))
    {
      rotatedCellOffset1 = component.GetRotatedCellOffset(rotatedCellOffset1);
      rotatedCellOffset2 = component.GetRotatedCellOffset(rotatedCellOffset2);
      rotatedCellOffset3 = component.GetRotatedCellOffset(rotatedCellOffset3);
      rotatedCellOffset4 = component.GetRotatedCellOffset(rotatedCellOffset4);
    }
    int num1 = Grid.OffsetCell(cell, rotatedCellOffset1);
    int num2 = Grid.OffsetCell(cell, rotatedCellOffset2);
    int num3 = Grid.OffsetCell(cell, rotatedCellOffset3);
    int num4 = Grid.OffsetCell(cell, rotatedCellOffset4);
    if (Grid.IsValidCell(num1))
      enable1 = !this.HasTileableNeighbour(num1);
    if (Grid.IsValidCell(num2))
      enable2 = !this.HasTileableNeighbour(num2);
    if (Grid.IsValidCell(num3))
      enable3 = !this.HasTileableNeighbour(num3);
    if (Grid.IsValidCell(num4))
      enable4 = !this.HasTileableNeighbour(num4);
    this.left.Enable(enable1);
    this.right.Enable(enable2);
    this.top.Enable(enable3);
    this.bottom.Enable(enable4);
  }

  private bool HasTileableNeighbour(int neighbour_cell)
  {
    bool flag = false;
    GameObject gameObject = Grid.Objects[neighbour_cell, (int) this.objectLayer];
    if (Object.op_Inequality((Object) gameObject, (Object) null))
    {
      KPrefabID component = gameObject.GetComponent<KPrefabID>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.HasAnyTags(this.tags))
        flag = true;
    }
    return flag;
  }

  private void OnNeighbourCellsUpdated(object data)
  {
    if (Object.op_Equality((Object) this, (Object) null) || Object.op_Equality((Object) ((Component) this).gameObject, (Object) null) || !this.partitionerEntry.IsValid())
      return;
    this.UpdateEndCaps();
  }
}
