// Decompiled with JetBrains decompiler
// Type: Placeable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Placeable")]
public class Placeable : KMonoBehaviour
{
  [MyCmpReq]
  private OccupyArea occupyArea;
  public string kAnimName;
  public string animName;
  public List<Placeable.PlacementRules> placementRules = new List<Placeable.PlacementRules>();
  [NonSerialized]
  public int restrictWorldId;

  public bool IsValidPlaceLocation(int cell, out string reason)
  {
    if (this.placementRules.Contains(Placeable.PlacementRules.RestrictToWorld) && (int) Grid.WorldIdx[cell] != this.restrictWorldId)
    {
      reason = (string) UI.TOOLS.PLACE.REASONS.RESTRICT_TO_WORLD;
      return false;
    }
    if (!this.occupyArea.CanOccupyArea(cell, this.occupyArea.objectLayers[0]))
    {
      reason = (string) UI.TOOLS.PLACE.REASONS.CAN_OCCUPY_AREA;
      return false;
    }
    if (this.placementRules.Contains(Placeable.PlacementRules.OnFoundation) && !this.occupyArea.TestAreaBelow(cell, (object) null, new Func<int, object, bool>(this.FoundationTest)))
    {
      reason = (string) UI.TOOLS.PLACE.REASONS.ON_FOUNDATION;
      return false;
    }
    if (this.placementRules.Contains(Placeable.PlacementRules.VisibleToSpace) && !this.occupyArea.TestArea(cell, (object) null, new Func<int, object, bool>(this.SunnySpaceTest)))
    {
      reason = (string) UI.TOOLS.PLACE.REASONS.VISIBLE_TO_SPACE;
      return false;
    }
    reason = "ok!";
    return true;
  }

  private bool SunnySpaceTest(int cell, object data)
  {
    if (!Grid.IsValidCell(cell))
      return false;
    int x;
    int y;
    Grid.CellToXY(cell, out x, out y);
    int id = (int) Grid.WorldIdx[cell];
    WorldContainer world = ClusterManager.Instance.GetWorld(id);
    int top = world.WorldOffset.y + world.WorldSize.y;
    if (Grid.Solid[cell] || Grid.Foundation[cell])
      return false;
    return Grid.ExposedToSunlight[cell] >= (byte) 253 || this.ClearPathToSky(x, y, top);
  }

  private bool ClearPathToSky(int x, int startY, int top)
  {
    for (int y = startY; y < top; ++y)
    {
      int cell = Grid.XYToCell(x, y);
      if (Grid.Solid[cell] || Grid.Foundation[cell])
        return false;
    }
    return true;
  }

  private bool FoundationTest(int cell, object data)
  {
    if (!Grid.IsValidBuildingCell(cell))
      return false;
    return Grid.Solid[cell] || Grid.Foundation[cell];
  }

  public enum PlacementRules
  {
    OnFoundation,
    VisibleToSpace,
    RestrictToWorld,
  }
}
