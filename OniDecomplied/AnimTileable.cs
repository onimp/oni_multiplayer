// Decompiled with JetBrains decompiler
// Type: AnimTileable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/AnimTileable")]
public class AnimTileable : KMonoBehaviour
{
  private HandleVector<int>.Handle partitionerEntry;
  public ObjectLayer objectLayer = ObjectLayer.Building;
  public Tag[] tags;
  private Extents extents;
  private static readonly KAnimHashedString[] leftSymbols = new KAnimHashedString[3]
  {
    new KAnimHashedString("cap_left"),
    new KAnimHashedString("cap_left_fg"),
    new KAnimHashedString("cap_left_place")
  };
  private static readonly KAnimHashedString[] rightSymbols = new KAnimHashedString[3]
  {
    new KAnimHashedString("cap_right"),
    new KAnimHashedString("cap_right_fg"),
    new KAnimHashedString("cap_right_place")
  };
  private static readonly KAnimHashedString[] topSymbols = new KAnimHashedString[3]
  {
    new KAnimHashedString("cap_top"),
    new KAnimHashedString("cap_top_fg"),
    new KAnimHashedString("cap_top_place")
  };
  private static readonly KAnimHashedString[] bottomSymbols = new KAnimHashedString[3]
  {
    new KAnimHashedString("cap_bottom"),
    new KAnimHashedString("cap_bottom_fg"),
    new KAnimHashedString("cap_bottom_place")
  };

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
    OccupyArea component = ((Component) this).GetComponent<OccupyArea>();
    this.extents = !Object.op_Inequality((Object) component, (Object) null) ? ((Component) this).GetComponent<Building>().GetExtents() : component.GetExtents();
    this.partitionerEntry = GameScenePartitioner.Instance.Add("AnimTileable.OnSpawn", (object) ((Component) this).gameObject, new Extents(this.extents.x - 1, this.extents.y - 1, this.extents.width + 2, this.extents.height + 2), GameScenePartitioner.Instance.objectLayers[(int) this.objectLayer], new Action<object>(this.OnNeighbourCellsUpdated));
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
    bool is_visible1 = true;
    bool is_visible2 = true;
    bool is_visible3 = true;
    bool is_visible4 = true;
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
      is_visible1 = !this.HasTileableNeighbour(num1);
    if (Grid.IsValidCell(num2))
      is_visible2 = !this.HasTileableNeighbour(num2);
    if (Grid.IsValidCell(num3))
      is_visible3 = !this.HasTileableNeighbour(num3);
    if (Grid.IsValidCell(num4))
      is_visible4 = !this.HasTileableNeighbour(num4);
    foreach (KBatchedAnimController componentsInChild in ((Component) this).GetComponentsInChildren<KBatchedAnimController>())
    {
      foreach (KAnimHashedString leftSymbol in AnimTileable.leftSymbols)
        componentsInChild.SetSymbolVisiblity(leftSymbol, is_visible1);
      foreach (KAnimHashedString rightSymbol in AnimTileable.rightSymbols)
        componentsInChild.SetSymbolVisiblity(rightSymbol, is_visible2);
      foreach (KAnimHashedString topSymbol in AnimTileable.topSymbols)
        componentsInChild.SetSymbolVisiblity(topSymbol, is_visible3);
      foreach (KAnimHashedString bottomSymbol in AnimTileable.bottomSymbols)
        componentsInChild.SetSymbolVisiblity(bottomSymbol, is_visible4);
    }
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
