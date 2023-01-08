// Decompiled with JetBrains decompiler
// Type: ClusterUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public static class ClusterUtil
{
  public static WorldContainer GetMyWorld(this StateMachine.Instance smi) => smi.GetComponent<StateMachineController>().GetMyWorld();

  public static WorldContainer GetMyWorld(this KMonoBehaviour component) => ((Component) component).gameObject.GetMyWorld();

  public static WorldContainer GetMyWorld(this GameObject gameObject)
  {
    int cell = Grid.PosToCell(gameObject);
    return Grid.IsValidCell(cell) && (int) Grid.WorldIdx[cell] != (int) ClusterManager.INVALID_WORLD_IDX ? ClusterManager.Instance.GetWorld((int) Grid.WorldIdx[cell]) : (WorldContainer) null;
  }

  public static int GetMyWorldId(this StateMachine.Instance smi) => smi.GetComponent<StateMachineController>().GetMyWorldId();

  public static int GetMyWorldId(this KMonoBehaviour component) => ((Component) component).gameObject.GetMyWorldId();

  public static int GetMyWorldId(this GameObject gameObject)
  {
    int cell = Grid.PosToCell(gameObject);
    return Grid.IsValidCell(cell) && (int) Grid.WorldIdx[cell] != (int) ClusterManager.INVALID_WORLD_IDX ? (int) Grid.WorldIdx[cell] : -1;
  }

  public static int GetMyParentWorldId(this StateMachine.Instance smi) => smi.GetComponent<StateMachineController>().GetMyParentWorldId();

  public static int GetMyParentWorldId(this KMonoBehaviour component) => ((Component) component).gameObject.GetMyParentWorldId();

  public static int GetMyParentWorldId(this GameObject gameObject)
  {
    WorldContainer myWorld = gameObject.GetMyWorld();
    return Object.op_Equality((Object) myWorld, (Object) null) ? gameObject.GetMyWorldId() : myWorld.ParentWorldId;
  }

  public static AxialI GetMyWorldLocation(this StateMachine.Instance smi) => smi.GetComponent<StateMachineController>().GetMyWorldLocation();

  public static AxialI GetMyWorldLocation(this KMonoBehaviour component) => ((Component) component).gameObject.GetMyWorldLocation();

  public static AxialI GetMyWorldLocation(this GameObject gameObject)
  {
    ClusterGridEntity component = gameObject.GetComponent<ClusterGridEntity>();
    if (Object.op_Inequality((Object) component, (Object) null))
      return component.Location;
    WorldContainer myWorld = gameObject.GetMyWorld();
    DebugUtil.DevAssertArgs((Object.op_Inequality((Object) myWorld, (Object) null) ? 1 : 0) != 0, new object[2]
    {
      (object) "GetMyWorldLocation called on object with no world",
      (object) gameObject
    });
    return ((Component) myWorld).GetComponent<ClusterGridEntity>().Location;
  }

  public static bool IsMyWorld(this GameObject go, GameObject otherGo)
  {
    int cell1 = Grid.PosToCell(go);
    int cell2 = Grid.PosToCell(otherGo);
    return Grid.IsValidCell(cell1) && Grid.IsValidCell(cell2) && (int) Grid.WorldIdx[cell1] == (int) Grid.WorldIdx[cell2];
  }

  public static bool IsMyParentWorld(this GameObject go, GameObject otherGo)
  {
    int cell1 = Grid.PosToCell(go);
    int cell2 = Grid.PosToCell(otherGo);
    if (Grid.IsValidCell(cell1) && Grid.IsValidCell(cell2))
    {
      if ((int) Grid.WorldIdx[cell1] == (int) Grid.WorldIdx[cell2])
        return true;
      WorldContainer world1 = ClusterManager.Instance.GetWorld((int) Grid.WorldIdx[cell1]);
      WorldContainer world2 = ClusterManager.Instance.GetWorld((int) Grid.WorldIdx[cell2]);
      if (Object.op_Equality((Object) world1, (Object) null))
        DebugUtil.DevLogError(string.Format("{0} at {1} has a valid cell but no world", (object) go, (object) cell1));
      if (Object.op_Equality((Object) world2, (Object) null))
        DebugUtil.DevLogError(string.Format("{0} at {1} has a valid cell but no world", (object) otherGo, (object) cell2));
      if (Object.op_Inequality((Object) world1, (Object) null) && Object.op_Inequality((Object) world2, (Object) null) && world1.ParentWorldId == world2.ParentWorldId)
        return true;
    }
    return false;
  }

  public static int GetAsteroidWorldIdAtLocation(AxialI location)
  {
    foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.cellContents[location])
    {
      if (clusterGridEntity.Layer == EntityLayer.Asteroid)
      {
        WorldContainer component = ((Component) clusterGridEntity).GetComponent<WorldContainer>();
        if (Object.op_Inequality((Object) component, (Object) null))
          return component.id;
      }
    }
    return -1;
  }

  public static bool ActiveWorldIsRocketInterior() => ClusterManager.Instance.activeWorld.IsModuleInterior;

  public static bool ActiveWorldHasPrinter() => ClusterManager.Instance.activeWorld.IsModuleInterior || Components.Telepads.GetWorldItems(ClusterManager.Instance.activeWorldId).Count > 0;

  public static float GetAmountFromRelatedWorlds(WorldInventory worldInventory, Tag element)
  {
    WorldContainer worldContainer1 = worldInventory.WorldContainer;
    float fromRelatedWorlds = 0.0f;
    int parentWorldId = worldContainer1.ParentWorldId;
    foreach (WorldContainer worldContainer2 in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
    {
      if (worldContainer2.ParentWorldId == parentWorldId)
        fromRelatedWorlds += worldContainer2.worldInventory.GetAmount(element, false);
    }
    return fromRelatedWorlds;
  }

  public static List<Pickupable> GetPickupablesFromRelatedWorlds(
    WorldInventory worldInventory,
    Tag tag)
  {
    List<Pickupable> fromRelatedWorlds = new List<Pickupable>();
    int parentWorldId = ((Component) worldInventory).GetComponent<WorldContainer>().ParentWorldId;
    foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
    {
      if (worldContainer.ParentWorldId == parentWorldId)
      {
        ICollection<Pickupable> pickupables = worldContainer.worldInventory.GetPickupables(tag);
        if (pickupables != null)
          fromRelatedWorlds.AddRange((IEnumerable<Pickupable>) pickupables);
      }
    }
    return fromRelatedWorlds;
  }

  public static string DebugGetMyWorldName(this GameObject gameObject)
  {
    WorldContainer myWorld = gameObject.GetMyWorld();
    return Object.op_Inequality((Object) myWorld, (Object) null) ? myWorld.worldName : string.Format("InvalidWorld(pos={0})", (object) TransformExtensions.GetPosition(gameObject.transform));
  }

  public static ClusterGridEntity ClosestVisibleAsteroidToLocation(AxialI location)
  {
    foreach (AxialI cell in AxialUtil.SpiralOut(location, ClusterGrid.Instance.numRings))
    {
      if (ClusterGrid.Instance.IsValidCell(cell) && ClusterGrid.Instance.IsCellVisible(cell))
      {
        ClusterGridEntity asteroidAtCell = ClusterGrid.Instance.GetAsteroidAtCell(cell);
        if (Object.op_Inequality((Object) asteroidAtCell, (Object) null))
          return asteroidAtCell;
      }
    }
    return (ClusterGridEntity) null;
  }
}
