// Decompiled with JetBrains decompiler
// Type: ModularConduitPortTiler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ModularConduitPortTiler : KMonoBehaviour
{
  private HandleVector<int>.Handle partitionerEntry;
  public ObjectLayer objectLayer = ObjectLayer.Building;
  public Tag[] tags;
  public bool manageLeftCap = true;
  public bool manageRightCap = true;
  public int leftCapDefaultSceneLayerAdjust;
  public int rightCapDefaultSceneLayerAdjust;
  private Extents extents;
  private ModularConduitPortTiler.AnimCapType leftCapSetting;
  private ModularConduitPortTiler.AnimCapType rightCapSetting;
  private static readonly string leftCapDefaultStr = "#cap_left_default";
  private static readonly string leftCapLaunchpadStr = "#cap_left_launchpad";
  private static readonly string leftCapConduitStr = "#cap_left_conduit";
  private static readonly string rightCapDefaultStr = "#cap_right_default";
  private static readonly string rightCapLaunchpadStr = "#cap_right_launchpad";
  private static readonly string rightCapConduitStr = "#cap_right_conduit";
  private KAnimSynchronizedController leftCapDefault;
  private KAnimSynchronizedController leftCapLaunchpad;
  private KAnimSynchronizedController leftCapConduit;
  private KAnimSynchronizedController rightCapDefault;
  private KAnimSynchronizedController rightCapLaunchpad;
  private KAnimSynchronizedController rightCapConduit;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.ModularConduitPort, true);
    if (this.tags != null && this.tags.Length != 0)
      return;
    this.tags = new Tag[1]{ GameTags.ModularConduitPort };
  }

  protected virtual void OnSpawn()
  {
    OccupyArea component1 = ((Component) this).GetComponent<OccupyArea>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      this.extents = component1.GetExtents();
    KBatchedAnimController component2 = ((Component) this).GetComponent<KBatchedAnimController>();
    this.leftCapDefault = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) (component2.GetLayer() + this.leftCapDefaultSceneLayerAdjust), ModularConduitPortTiler.leftCapDefaultStr);
    if (this.manageLeftCap)
    {
      this.leftCapLaunchpad = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) component2.GetLayer(), ModularConduitPortTiler.leftCapLaunchpadStr);
      this.leftCapConduit = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) (component2.GetLayer() + 1), ModularConduitPortTiler.leftCapConduitStr);
    }
    this.rightCapDefault = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) (component2.GetLayer() + this.rightCapDefaultSceneLayerAdjust), ModularConduitPortTiler.rightCapDefaultStr);
    if (this.manageRightCap)
    {
      this.rightCapLaunchpad = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) component2.GetLayer(), ModularConduitPortTiler.rightCapLaunchpadStr);
      this.rightCapConduit = new KAnimSynchronizedController((KAnimControllerBase) component2, (Grid.SceneLayer) component2.GetLayer(), ModularConduitPortTiler.rightCapConduitStr);
    }
    this.partitionerEntry = GameScenePartitioner.Instance.Add("ModularConduitPort.OnSpawn", (object) ((Component) this).gameObject, new Extents(this.extents.x - 1, this.extents.y, this.extents.width + 2, this.extents.height), GameScenePartitioner.Instance.objectLayers[(int) this.objectLayer], new Action<object>(this.OnNeighbourCellsUpdated));
    this.UpdateEndCaps();
    this.CorrectAdjacentLaunchPads();
  }

  protected virtual void OnCleanUp()
  {
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    base.OnCleanUp();
  }

  private void UpdateEndCaps()
  {
    Grid.CellToXY(Grid.PosToCell((KMonoBehaviour) this), out int _, out int _);
    int cellLeft = this.GetCellLeft();
    int cellRight = this.GetCellRight();
    if (Grid.IsValidCell(cellLeft))
      this.leftCapSetting = !this.HasTileableNeighbour(cellLeft) ? (!this.HasLaunchpadNeighbour(cellLeft) ? ModularConduitPortTiler.AnimCapType.Default : ModularConduitPortTiler.AnimCapType.Launchpad) : ModularConduitPortTiler.AnimCapType.Conduit;
    if (Grid.IsValidCell(cellRight))
      this.rightCapSetting = !this.HasTileableNeighbour(cellRight) ? (!this.HasLaunchpadNeighbour(cellRight) ? ModularConduitPortTiler.AnimCapType.Default : ModularConduitPortTiler.AnimCapType.Launchpad) : ModularConduitPortTiler.AnimCapType.Conduit;
    if (this.manageLeftCap)
    {
      this.leftCapDefault.Enable(this.leftCapSetting == ModularConduitPortTiler.AnimCapType.Default);
      this.leftCapConduit.Enable(this.leftCapSetting == ModularConduitPortTiler.AnimCapType.Conduit);
      this.leftCapLaunchpad.Enable(this.leftCapSetting == ModularConduitPortTiler.AnimCapType.Launchpad);
    }
    if (!this.manageRightCap)
      return;
    this.rightCapDefault.Enable(this.rightCapSetting == ModularConduitPortTiler.AnimCapType.Default);
    this.rightCapConduit.Enable(this.rightCapSetting == ModularConduitPortTiler.AnimCapType.Conduit);
    this.rightCapLaunchpad.Enable(this.rightCapSetting == ModularConduitPortTiler.AnimCapType.Launchpad);
  }

  private int GetCellLeft()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    int x;
    Grid.CellToXY(cell, out x, out int _);
    CellOffset offset;
    // ISSUE: explicit constructor call
    ((CellOffset) ref offset).\u002Ector(this.extents.x - x - 1, 0);
    return Grid.OffsetCell(cell, offset);
  }

  private int GetCellRight()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    int x;
    Grid.CellToXY(cell, out x, out int _);
    CellOffset offset;
    // ISSUE: explicit constructor call
    ((CellOffset) ref offset).\u002Ector(this.extents.x - x + this.extents.width, 0);
    return Grid.OffsetCell(cell, offset);
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

  private bool HasLaunchpadNeighbour(int neighbour_cell)
  {
    GameObject gameObject = Grid.Objects[neighbour_cell, (int) this.objectLayer];
    return Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject.GetComponent<LaunchPad>(), (Object) null);
  }

  private void OnNeighbourCellsUpdated(object data)
  {
    if (Object.op_Equality((Object) this, (Object) null) || Object.op_Equality((Object) ((Component) this).gameObject, (Object) null) || !this.partitionerEntry.IsValid())
      return;
    this.UpdateEndCaps();
  }

  private void CorrectAdjacentLaunchPads()
  {
    int cellRight = this.GetCellRight();
    if (Grid.IsValidCell(cellRight) && this.HasLaunchpadNeighbour(cellRight))
      Grid.Objects[cellRight, 1].GetComponent<ModularConduitPortTiler>().UpdateEndCaps();
    int cellLeft = this.GetCellLeft();
    if (!Grid.IsValidCell(cellLeft) || !this.HasLaunchpadNeighbour(cellLeft))
      return;
    Grid.Objects[cellLeft, 1].GetComponent<ModularConduitPortTiler>().UpdateEndCaps();
  }

  private enum AnimCapType
  {
    Default,
    Conduit,
    Launchpad,
  }
}
