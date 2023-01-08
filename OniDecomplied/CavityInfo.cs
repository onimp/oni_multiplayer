// Decompiled with JetBrains decompiler
// Type: CavityInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class CavityInfo
{
  public HandleVector<int>.Handle handle;
  public bool dirty;
  public int numCells;
  public int maxX;
  public int maxY;
  public int minX;
  public int minY;
  public Room room;
  public List<KPrefabID> buildings = new List<KPrefabID>();
  public List<KPrefabID> plants = new List<KPrefabID>();
  public List<KPrefabID> creatures = new List<KPrefabID>();
  public List<KPrefabID> eggs = new List<KPrefabID>();

  public CavityInfo()
  {
    this.handle = HandleVector<int>.InvalidHandle;
    this.dirty = true;
  }

  public void AddBuilding(KPrefabID bc)
  {
    this.buildings.Add(bc);
    this.dirty = true;
  }

  public void AddPlants(KPrefabID plant)
  {
    this.plants.Add(plant);
    this.dirty = true;
  }

  public void RemoveFromCavity(KPrefabID id, List<KPrefabID> listToRemove)
  {
    int index1 = -1;
    for (int index2 = 0; index2 < listToRemove.Count; ++index2)
    {
      if (id.InstanceID == listToRemove[index2].InstanceID)
      {
        index1 = index2;
        break;
      }
    }
    if (index1 < 0)
      return;
    listToRemove.RemoveAt(index1);
  }

  public void OnEnter(object data)
  {
    foreach (KPrefabID building in this.buildings)
    {
      if (Object.op_Inequality((Object) building, (Object) null))
        ((KMonoBehaviour) building).Trigger(-832141045, data);
    }
  }

  public Vector3 GetCenter() => new Vector3((float) (this.minX + (this.maxX - this.minX) / 2), (float) (this.minY + (this.maxY - this.minY) / 2));
}
