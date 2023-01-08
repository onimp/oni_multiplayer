// Decompiled with JetBrains decompiler
// Type: BuildingInventory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BuildingInventory : KMonoBehaviour
{
  public static BuildingInventory Instance;
  private Dictionary<Tag, HashSet<BuildingComplete>> Buildings = new Dictionary<Tag, HashSet<BuildingComplete>>();

  public static void DestroyInstance() => BuildingInventory.Instance = (BuildingInventory) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    BuildingInventory.Instance = this;
  }

  public HashSet<BuildingComplete> GetBuildings(Tag tag) => this.Buildings[tag];

  public int BuildingCount(Tag tag) => !this.Buildings.ContainsKey(tag) ? 0 : this.Buildings[tag].Count;

  public int BuildingCountForWorld_BAD_PERF(Tag tag, int worldId)
  {
    if (!this.Buildings.ContainsKey(tag))
      return 0;
    int num = 0;
    foreach (KMonoBehaviour component in this.Buildings[tag])
    {
      if (component.GetMyWorldId() == worldId)
        ++num;
    }
    return num;
  }

  public void RegisterBuilding(BuildingComplete building)
  {
    Tag prefabTag = building.prefabid.PrefabTag;
    HashSet<BuildingComplete> buildingCompleteSet;
    if (!this.Buildings.TryGetValue(prefabTag, out buildingCompleteSet))
    {
      buildingCompleteSet = new HashSet<BuildingComplete>();
      this.Buildings[prefabTag] = buildingCompleteSet;
    }
    buildingCompleteSet.Add(building);
  }

  public void UnregisterBuilding(BuildingComplete building)
  {
    Tag prefabTag = building.prefabid.PrefabTag;
    HashSet<BuildingComplete> buildingCompleteSet;
    if (!this.Buildings.TryGetValue(prefabTag, out buildingCompleteSet))
      DebugUtil.DevLogError(string.Format("Unregistering building {0} before it was registered.", (object) prefabTag));
    else
      DebugUtil.DevAssert(buildingCompleteSet.Remove(building), string.Format("Building {0} was not found to be removed", (object) prefabTag), (Object) null);
  }
}
