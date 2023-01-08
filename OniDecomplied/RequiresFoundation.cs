// Decompiled with JetBrains decompiler
// Type: RequiresFoundation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RequiresFoundation : 
  KGameObjectComponentManager<RequiresFoundation.Data>,
  IKComponentManager
{
  public static readonly Operational.Flag solidFoundation = new Operational.Flag("solid_foundation", Operational.Flag.Type.Functional);

  public HandleVector<int>.Handle Add(GameObject go)
  {
    BuildingDef def = go.GetComponent<Building>().Def;
    int cell1 = Grid.PosToCell(TransformExtensions.GetPosition(go.transform));
    RequiresFoundation.Data data1 = new RequiresFoundation.Data()
    {
      cell = cell1,
      width = def.WidthInCells,
      height = def.HeightInCells,
      buildRule = def.BuildLocationRule,
      solid = true,
      go = go
    };
    HandleVector<int>.Handle h = this.Add(go, data1);
    if (def.ContinuouslyCheckFoundation)
    {
      data1.changeCallback = (Action<object>) (d => this.OnSolidChanged(h));
      Rotatable component = data1.go.GetComponent<Rotatable>();
      Orientation orientation = Object.op_Inequality((Object) component, (Object) null) ? component.GetOrientation() : Orientation.Neutral;
      int num1 = -(def.WidthInCells - 1) / 2;
      int num2 = def.WidthInCells / 2;
      CellOffset offset1;
      // ISSUE: explicit constructor call
      ((CellOffset) ref offset1).\u002Ector(num1, -1);
      CellOffset offset2;
      // ISSUE: explicit constructor call
      ((CellOffset) ref offset2).\u002Ector(num2, -1);
      if (def.BuildLocationRule == BuildLocationRule.OnCeiling || def.BuildLocationRule == BuildLocationRule.InCorner)
      {
        offset1.y = def.HeightInCells;
        offset2.y = def.HeightInCells;
      }
      else if (def.BuildLocationRule == BuildLocationRule.OnWall)
      {
        // ISSUE: explicit constructor call
        ((CellOffset) ref offset1).\u002Ector(num1 - 1, 0);
        // ISSUE: explicit constructor call
        ((CellOffset) ref offset2).\u002Ector(num1 - 1, def.HeightInCells);
      }
      else if (def.BuildLocationRule == BuildLocationRule.WallFloor)
      {
        // ISSUE: explicit constructor call
        ((CellOffset) ref offset1).\u002Ector(num1 - 1, -1);
        // ISSUE: explicit constructor call
        ((CellOffset) ref offset2).\u002Ector(num2, def.HeightInCells - 1);
      }
      CellOffset rotatedCellOffset1 = Rotatable.GetRotatedCellOffset(offset1, orientation);
      CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(offset2, orientation);
      int cell2 = Grid.OffsetCell(cell1, rotatedCellOffset1);
      int cell3 = Grid.OffsetCell(cell1, rotatedCellOffset2);
      Vector2I xy1 = Grid.CellToXY(cell2);
      Vector2I xy2 = Grid.CellToXY(cell3);
      float num3 = (float) Mathf.Min(xy1.x, xy2.x);
      float num4 = (float) Mathf.Max(xy1.x, xy2.x);
      float num5 = (float) Mathf.Min(xy1.y, xy2.y);
      float num6 = (float) Mathf.Max(xy1.y, xy2.y);
      Rect rect = Rect.MinMaxRect(num3, num5, num4, num6);
      data1.solidPartitionerEntry = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", (object) go, (int) ((Rect) ref rect).x, (int) ((Rect) ref rect).y, (int) ((Rect) ref rect).width + 1, (int) ((Rect) ref rect).height + 1, GameScenePartitioner.Instance.solidChangedLayer, data1.changeCallback);
      data1.buildingPartitionerEntry = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", (object) go, (int) ((Rect) ref rect).x, (int) ((Rect) ref rect).y, (int) ((Rect) ref rect).width + 1, (int) ((Rect) ref rect).height + 1, GameScenePartitioner.Instance.objectLayers[1], data1.changeCallback);
      if (def.BuildLocationRule == BuildLocationRule.BuildingAttachPoint || def.BuildLocationRule == BuildLocationRule.OnFloorOrBuildingAttachPoint)
        data1.go.GetComponent<AttachableBuilding>().onAttachmentNetworkChanged += data1.changeCallback;
      ((KCompactedVector<RequiresFoundation.Data>) this).SetData(h, data1);
      this.OnSolidChanged(h);
      RequiresFoundation.Data data2 = ((KCompactedVector<RequiresFoundation.Data>) this).GetData(h);
      this.UpdateSolidState(data2.solid, ref data2, true);
    }
    return h;
  }

  protected virtual void OnCleanUp(HandleVector<int>.Handle h)
  {
    RequiresFoundation.Data data = ((KCompactedVector<RequiresFoundation.Data>) this).GetData(h);
    GameScenePartitioner.Instance.Free(ref data.solidPartitionerEntry);
    GameScenePartitioner.Instance.Free(ref data.buildingPartitionerEntry);
    AttachableBuilding component = data.go.GetComponent<AttachableBuilding>();
    if (!Util.IsNullOrDestroyed((object) component))
      component.onAttachmentNetworkChanged -= data.changeCallback;
    ((KCompactedVector<RequiresFoundation.Data>) this).SetData(h, data);
  }

  private void OnSolidChanged(HandleVector<int>.Handle h)
  {
    RequiresFoundation.Data data = ((KCompactedVector<RequiresFoundation.Data>) this).GetData(h);
    SimCellOccupier component1 = data.go.GetComponent<SimCellOccupier>();
    if (!Object.op_Equality((Object) component1, (Object) null) && !component1.IsReady())
      return;
    Rotatable component2 = data.go.GetComponent<Rotatable>();
    Orientation orientation = Object.op_Inequality((Object) component2, (Object) null) ? component2.GetOrientation() : Orientation.Neutral;
    bool is_solid = BuildingDef.CheckFoundation(data.cell, orientation, data.buildRule, data.width, data.height, new Tag());
    if (!is_solid && (data.buildRule == BuildLocationRule.BuildingAttachPoint || data.buildRule == BuildLocationRule.OnFloorOrBuildingAttachPoint))
    {
      List<GameObject> buildings = new List<GameObject>();
      AttachableBuilding.GetAttachedBelow(data.go.GetComponent<AttachableBuilding>(), ref buildings);
      if (buildings.Count > 0)
      {
        Operational component3 = ((IEnumerable<GameObject>) buildings).Last<GameObject>().GetComponent<Operational>();
        if (Object.op_Inequality((Object) component3, (Object) null) && component3.GetFlag(RequiresFoundation.solidFoundation))
          is_solid = true;
      }
    }
    this.UpdateSolidState(is_solid, ref data);
    ((KCompactedVector<RequiresFoundation.Data>) this).SetData(h, data);
  }

  private void UpdateSolidState(bool is_solid, ref RequiresFoundation.Data data, bool forceUpdate = false)
  {
    if (!(data.solid != is_solid | forceUpdate))
      return;
    data.solid = is_solid;
    Operational component1 = data.go.GetComponent<Operational>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      component1.SetFlag(RequiresFoundation.solidFoundation, is_solid);
    AttachableBuilding component2 = data.go.GetComponent<AttachableBuilding>();
    if (Object.op_Inequality((Object) component2, (Object) null))
    {
      List<GameObject> buildings = new List<GameObject>();
      AttachableBuilding.GetAttachedAbove(component2, ref buildings);
      AttachableBuilding.NotifyBuildingsNetworkChanged(buildings);
    }
    data.go.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.MissingFoundation, !is_solid, (object) this);
  }

  public struct Data
  {
    public int cell;
    public int width;
    public int height;
    public BuildLocationRule buildRule;
    public HandleVector<int>.Handle solidPartitionerEntry;
    public HandleVector<int>.Handle buildingPartitionerEntry;
    public bool solid;
    public GameObject go;
    public Action<object> changeCallback;
  }
}
