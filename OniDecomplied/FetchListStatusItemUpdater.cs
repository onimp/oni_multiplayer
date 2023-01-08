// Decompiled with JetBrains decompiler
// Type: FetchListStatusItemUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FetchListStatusItemUpdater")]
public class FetchListStatusItemUpdater : KMonoBehaviour, IRender200ms
{
  public static FetchListStatusItemUpdater instance;
  private List<FetchList2> fetchLists = new List<FetchList2>();
  private int[] currentIterationIndex = new int[(int) ClusterManager.INVALID_WORLD_IDX];
  private int maxIteratingCount = 100;

  public static void DestroyInstance() => FetchListStatusItemUpdater.instance = (FetchListStatusItemUpdater) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    FetchListStatusItemUpdater.instance = this;
  }

  public void AddFetchList(FetchList2 fetch_list) => this.fetchLists.Add(fetch_list);

  public void RemoveFetchList(FetchList2 fetch_list) => this.fetchLists.Remove(fetch_list);

  public void Render200ms(float dt)
  {
    foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
    {
      int id = worldContainer.id;
      DictionaryPool<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary1 = DictionaryPool<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList, FetchListStatusItemUpdater>.Allocate();
      int num1 = Math.Min(this.maxIteratingCount, this.fetchLists.Count - this.currentIterationIndex[id]);
      for (int index = 0; index < num1; ++index)
      {
        FetchList2 fetchList = this.fetchLists[index + this.currentIterationIndex[id]];
        if (!Object.op_Equality((Object) fetchList.Destination, (Object) null) && ((Component) fetchList.Destination).gameObject.GetMyWorldId() == id)
        {
          ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList pooledList = (ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList) null;
          int instanceId = ((Object) fetchList.Destination).GetInstanceID();
          if (!((Dictionary<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList>) pooledDictionary1).TryGetValue(instanceId, out pooledList))
          {
            pooledList = ListPool<FetchList2, FetchListStatusItemUpdater>.Allocate();
            ((Dictionary<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList>) pooledDictionary1)[instanceId] = pooledList;
          }
          ((List<FetchList2>) pooledList).Add(fetchList);
        }
      }
      this.currentIterationIndex[id] += num1;
      if (this.currentIterationIndex[id] >= this.fetchLists.Count)
        this.currentIterationIndex[id] = 0;
      DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary2 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
      DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary3 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
      foreach (KeyValuePair<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList> keyValuePair1 in (Dictionary<int, ListPool<FetchList2, FetchListStatusItemUpdater>.PooledList>) pooledDictionary1)
      {
        if (!Object.op_Equality((Object) ((List<FetchList2>) keyValuePair1.Value)[0].Destination.GetMyWorld(), (Object) null))
        {
          ListPool<Tag, FetchListStatusItemUpdater>.PooledList pooledList1 = ListPool<Tag, FetchListStatusItemUpdater>.Allocate();
          Storage destination = ((List<FetchList2>) keyValuePair1.Value)[0].Destination;
          foreach (FetchList2 fetchList2 in (List<FetchList2>) keyValuePair1.Value)
          {
            fetchList2.UpdateRemaining();
            foreach (KeyValuePair<Tag, float> keyValuePair2 in fetchList2.GetRemaining())
            {
              if (!((List<Tag>) pooledList1).Contains(keyValuePair2.Key))
                ((List<Tag>) pooledList1).Add(keyValuePair2.Key);
            }
          }
          ListPool<Pickupable, FetchListStatusItemUpdater>.PooledList pooledList2 = ListPool<Pickupable, FetchListStatusItemUpdater>.Allocate();
          foreach (GameObject gameObject in destination.items)
          {
            if (!Object.op_Equality((Object) gameObject, (Object) null))
            {
              Pickupable component = gameObject.GetComponent<Pickupable>();
              if (!Object.op_Equality((Object) component, (Object) null))
                ((List<Pickupable>) pooledList2).Add(component);
            }
          }
          DictionaryPool<Tag, float, FetchListStatusItemUpdater>.PooledDictionary pooledDictionary4 = DictionaryPool<Tag, float, FetchListStatusItemUpdater>.Allocate();
          foreach (Tag key in (List<Tag>) pooledList1)
          {
            float num2 = 0.0f;
            foreach (Pickupable pickupable in (List<Pickupable>) pooledList2)
            {
              if (pickupable.KPrefabID.HasTag(key))
                num2 += pickupable.TotalAmount;
            }
            ((Dictionary<Tag, float>) pooledDictionary4)[key] = num2;
          }
          foreach (Tag tag in (List<Tag>) pooledList1)
          {
            if (!((Dictionary<Tag, float>) pooledDictionary2).ContainsKey(tag))
              ((Dictionary<Tag, float>) pooledDictionary2)[tag] = destination.GetMyWorld().worldInventory.GetTotalAmount(tag, true);
            if (!((Dictionary<Tag, float>) pooledDictionary3).ContainsKey(tag))
              ((Dictionary<Tag, float>) pooledDictionary3)[tag] = destination.GetMyWorld().worldInventory.GetAmount(tag, true);
          }
          foreach (FetchList2 fetchList2 in (List<FetchList2>) keyValuePair1.Value)
          {
            bool should_add1 = false;
            bool should_add2 = true;
            bool should_add3 = false;
            foreach (KeyValuePair<Tag, float> keyValuePair3 in fetchList2.GetRemaining())
            {
              Tag key = keyValuePair3.Key;
              float num3 = keyValuePair3.Value;
              double num4 = (double) ((Dictionary<Tag, float>) pooledDictionary4)[key];
              float num5 = ((Dictionary<Tag, float>) pooledDictionary2)[key];
              float num6 = ((Dictionary<Tag, float>) pooledDictionary3)[key] + Mathf.Min(num3, num5);
              float minimumAmount = fetchList2.GetMinimumAmount(key);
              if (num4 + (double) num6 < (double) minimumAmount)
                should_add1 = true;
              if ((double) num6 < (double) num3)
                should_add2 = false;
              if (num4 + (double) num6 > (double) num3 && (double) num3 > (double) num6)
                should_add3 = true;
            }
            fetchList2.UpdateStatusItem(Db.Get().BuildingStatusItems.WaitingForMaterials, ref fetchList2.waitingForMaterialsHandle, should_add2);
            fetchList2.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, ref fetchList2.materialsUnavailableHandle, should_add1);
            fetchList2.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailableForRefill, ref fetchList2.materialsUnavailableForRefillHandle, should_add3);
          }
          pooledDictionary4.Recycle();
          pooledList2.Recycle();
          pooledList1.Recycle();
          keyValuePair1.Value.Recycle();
        }
      }
      pooledDictionary3.Recycle();
      pooledDictionary2.Recycle();
      pooledDictionary1.Recycle();
    }
  }
}
