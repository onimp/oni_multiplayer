// Decompiled with JetBrains decompiler
// Type: VerticalModuleTiler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class VerticalModuleTiler : KMonoBehaviour
{
  private HandleVector<int>.Handle partitionerEntry;
  public ObjectLayer objectLayer = ObjectLayer.Building;
  private Extents extents;
  private VerticalModuleTiler.AnimCapType topCapSetting;
  private VerticalModuleTiler.AnimCapType bottomCapSetting;
  private bool manageTopCap = true;
  private bool manageBottomCap = true;
  private KAnimSynchronizedController topCapWide;
  private KAnimSynchronizedController bottomCapWide;
  private static readonly string topCapStr = "#cap_top_5";
  private static readonly string bottomCapStr = "#cap_bottom_5";
  private bool dirty;
  [MyCmpGet]
  private KAnimControllerBase animController;
  private Vector3 m_previousAnimControllerOffset;

  protected virtual void OnSpawn()
  {
    OccupyArea component1 = ((Component) this).GetComponent<OccupyArea>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      this.extents = component1.GetExtents();
    KBatchedAnimController component2 = ((Component) this).GetComponent<KBatchedAnimController>();
    if (this.manageTopCap)
      this.topCapWide = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) component2.GetLayer(), VerticalModuleTiler.topCapStr);
    if (this.manageBottomCap)
      this.bottomCapWide = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) component2.GetLayer(), VerticalModuleTiler.bottomCapStr);
    this.PostReorderMove();
  }

  protected virtual void OnCleanUp()
  {
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    base.OnCleanUp();
  }

  public void PostReorderMove() => this.dirty = true;

  private void OnNeighbourCellsUpdated(object data)
  {
    if (Object.op_Equality((Object) this, (Object) null) || Object.op_Equality((Object) ((Component) this).gameObject, (Object) null) || !this.partitionerEntry.IsValid())
      return;
    this.UpdateEndCaps();
  }

  private void UpdateEndCaps()
  {
    Grid.CellToXY(Grid.PosToCell((KMonoBehaviour) this), out int _, out int _);
    int cellTop = this.GetCellTop();
    int cellBottom = this.GetCellBottom();
    if (Grid.IsValidCell(cellTop))
      this.topCapSetting = !this.HasWideNeighbor(cellTop) ? VerticalModuleTiler.AnimCapType.ThreeWide : VerticalModuleTiler.AnimCapType.FiveWide;
    if (Grid.IsValidCell(cellBottom))
      this.bottomCapSetting = !this.HasWideNeighbor(cellBottom) ? VerticalModuleTiler.AnimCapType.ThreeWide : VerticalModuleTiler.AnimCapType.FiveWide;
    if (this.manageTopCap)
      this.topCapWide.Enable(this.topCapSetting == VerticalModuleTiler.AnimCapType.FiveWide);
    if (!this.manageBottomCap)
      return;
    this.bottomCapWide.Enable(this.bottomCapSetting == VerticalModuleTiler.AnimCapType.FiveWide);
  }

  private int GetCellTop()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    int y;
    Grid.CellToXY(cell, out int _, out y);
    CellOffset offset;
    // ISSUE: explicit constructor call
    ((CellOffset) ref offset).\u002Ector(0, this.extents.y - y + this.extents.height);
    return Grid.OffsetCell(cell, offset);
  }

  private int GetCellBottom()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    int y;
    Grid.CellToXY(cell, out int _, out y);
    CellOffset offset;
    // ISSUE: explicit constructor call
    ((CellOffset) ref offset).\u002Ector(0, this.extents.y - y - 1);
    return Grid.OffsetCell(cell, offset);
  }

  private bool HasWideNeighbor(int neighbour_cell)
  {
    bool flag = false;
    GameObject gameObject = Grid.Objects[neighbour_cell, (int) this.objectLayer];
    if (Object.op_Inequality((Object) gameObject, (Object) null))
    {
      KPrefabID component = gameObject.GetComponent<KPrefabID>();
      if (Object.op_Inequality((Object) component, (Object) null) && Object.op_Inequality((Object) ((Component) component).GetComponent<ReorderableBuilding>(), (Object) null) && ((Component) component).GetComponent<Building>().Def.WidthInCells >= 5)
        flag = true;
    }
    return flag;
  }

  private void LateUpdate()
  {
    if (Vector3.op_Inequality(this.animController.Offset, this.m_previousAnimControllerOffset))
    {
      this.m_previousAnimControllerOffset = this.animController.Offset;
      this.bottomCapWide.Dirty();
      this.topCapWide.Dirty();
    }
    if (!this.dirty)
      return;
    if (HandleVector<int>.Handle.op_Inequality(this.partitionerEntry, HandleVector<int>.InvalidHandle))
      GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    OccupyArea component = ((Component) this).GetComponent<OccupyArea>();
    if (Object.op_Inequality((Object) component, (Object) null))
      this.extents = component.GetExtents();
    this.partitionerEntry = GameScenePartitioner.Instance.Add("VerticalModuleTiler.OnSpawn", (object) ((Component) this).gameObject, new Extents(this.extents.x, this.extents.y - 1, this.extents.width, this.extents.height + 2), GameScenePartitioner.Instance.objectLayers[(int) this.objectLayer], new Action<object>(this.OnNeighbourCellsUpdated));
    this.UpdateEndCaps();
    this.dirty = false;
  }

  private enum AnimCapType
  {
    ThreeWide,
    FiveWide,
  }
}
