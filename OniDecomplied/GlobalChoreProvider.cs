// Decompiled with JetBrains decompiler
// Type: GlobalChoreProvider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class GlobalChoreProvider : ChoreProvider, IRender200ms
{
  public static GlobalChoreProvider Instance;
  public Dictionary<int, List<FetchChore>> fetchMap = new Dictionary<int, List<FetchChore>>();
  public List<GlobalChoreProvider.Fetch> fetches = new List<GlobalChoreProvider.Fetch>();
  private static readonly GlobalChoreProvider.FetchComparer Comparer = new GlobalChoreProvider.FetchComparer();
  private ClearableManager clearableManager;
  private HashSet<Tag> storageFetchableTags = new HashSet<Tag>();

  public static void DestroyInstance() => GlobalChoreProvider.Instance = (GlobalChoreProvider) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    GlobalChoreProvider.Instance = this;
    this.clearableManager = new ClearableManager();
  }

  protected override void OnWorldRemoved(object data)
  {
    int num = (int) data;
    List<FetchChore> chores;
    if (this.fetchMap.TryGetValue(ClusterManager.Instance.GetWorld(num).ParentWorldId, out chores))
      this.ClearWorldChores<FetchChore>(chores, num);
    base.OnWorldRemoved(data);
  }

  protected override void OnWorldParentChanged(object data)
  {
    if (!(data is WorldParentChangedEventArgs changedEventArgs) || changedEventArgs.lastParentId == (int) ClusterManager.INVALID_WORLD_IDX)
      return;
    base.OnWorldParentChanged(data);
    List<FetchChore> oldChores;
    if (!this.fetchMap.TryGetValue(changedEventArgs.lastParentId, out oldChores))
      return;
    List<FetchChore> newChores;
    if (!this.fetchMap.TryGetValue(changedEventArgs.world.ParentWorldId, out newChores))
      this.fetchMap[changedEventArgs.world.ParentWorldId] = newChores = new List<FetchChore>();
    this.TransferChores<FetchChore>(oldChores, newChores, changedEventArgs.world.ParentWorldId);
  }

  public override void AddChore(Chore chore)
  {
    if (chore is FetchChore fetchChore)
    {
      int myParentWorldId = fetchChore.gameObject.GetMyParentWorldId();
      List<FetchChore> fetchChoreList;
      if (!this.fetchMap.TryGetValue(myParentWorldId, out fetchChoreList))
        this.fetchMap[myParentWorldId] = fetchChoreList = new List<FetchChore>();
      chore.provider = (ChoreProvider) this;
      fetchChoreList.Add(fetchChore);
    }
    else
      base.AddChore(chore);
  }

  public override void RemoveChore(Chore chore)
  {
    if (chore is FetchChore fetchChore)
    {
      List<FetchChore> fetchChoreList;
      if (this.fetchMap.TryGetValue(fetchChore.gameObject.GetMyParentWorldId(), out fetchChoreList))
        fetchChoreList.Remove(fetchChore);
      chore.provider = (ChoreProvider) null;
    }
    else
      base.RemoveChore(chore);
  }

  public void UpdateFetches(PathProber path_prober)
  {
    List<FetchChore> fetchChoreList = (List<FetchChore>) null;
    if (!this.fetchMap.TryGetValue(((Component) path_prober).gameObject.GetMyParentWorldId(), out fetchChoreList))
      return;
    this.fetches.Clear();
    Navigator component = ((Component) path_prober).GetComponent<Navigator>();
    GlobalChoreProvider.Fetch fetch1;
    for (int index = fetchChoreList.Count - 1; index >= 0; --index)
    {
      FetchChore fetchChore = fetchChoreList[index];
      if (!Object.op_Inequality((Object) fetchChore.driver, (Object) null) && (!Object.op_Inequality((Object) fetchChore.automatable, (Object) null) || !fetchChore.automatable.GetAutomationOnly()))
      {
        if (Object.op_Equality((Object) fetchChore.provider, (Object) null))
        {
          fetchChore.Cancel("no provider");
          fetchChoreList[index] = fetchChoreList[fetchChoreList.Count - 1];
          fetchChoreList.RemoveAt(fetchChoreList.Count - 1);
        }
        else
        {
          Storage destination = fetchChore.destination;
          if (!Object.op_Equality((Object) destination, (Object) null))
          {
            int navigationCost = component.GetNavigationCost((IApproachable) destination);
            if (navigationCost != -1)
            {
              List<GlobalChoreProvider.Fetch> fetches = this.fetches;
              fetch1 = new GlobalChoreProvider.Fetch();
              fetch1.chore = fetchChore;
              fetch1.idsHash = fetchChore.tagsHash;
              fetch1.cost = navigationCost;
              fetch1.priority = fetchChore.masterPriority;
              fetch1.category = destination.fetchCategory;
              GlobalChoreProvider.Fetch fetch2 = fetch1;
              fetches.Add(fetch2);
            }
          }
        }
      }
    }
    if (this.fetches.Count > 0)
    {
      this.fetches.Sort((IComparer<GlobalChoreProvider.Fetch>) GlobalChoreProvider.Comparer);
      int index1 = 1;
      int index2 = 0;
      for (; index1 < this.fetches.Count; ++index1)
      {
        fetch1 = this.fetches[index2];
        if (!fetch1.IsBetterThan(this.fetches[index1]))
        {
          ++index2;
          this.fetches[index2] = this.fetches[index1];
        }
      }
      this.fetches.RemoveRange(index2 + 1, this.fetches.Count - index2 - 1);
    }
    this.clearableManager.CollectAndSortClearables(component);
  }

  public override void CollectChores(
    ChoreConsumerState consumer_state,
    List<Chore.Precondition.Context> succeeded,
    List<Chore.Precondition.Context> failed_contexts)
  {
    base.CollectChores(consumer_state, succeeded, failed_contexts);
    this.clearableManager.CollectChores(this.fetches, consumer_state, succeeded, failed_contexts);
    for (int index = 0; index < this.fetches.Count; ++index)
      this.fetches[index].chore.CollectChoresFromGlobalChoreProvider(consumer_state, succeeded, failed_contexts, false);
  }

  public HandleVector<int>.Handle RegisterClearable(Clearable clearable) => this.clearableManager.RegisterClearable(clearable);

  public void UnregisterClearable(HandleVector<int>.Handle handle) => this.clearableManager.UnregisterClearable(handle);

  protected virtual void OnLoadLevel()
  {
    base.OnLoadLevel();
    GlobalChoreProvider.Instance = (GlobalChoreProvider) null;
  }

  public void Render200ms(float dt) => this.UpdateStorageFetchableBits();

  private void UpdateStorageFetchableBits()
  {
    ChoreType storageFetch = Db.Get().ChoreTypes.StorageFetch;
    ChoreType foodFetch = Db.Get().ChoreTypes.FoodFetch;
    this.storageFetchableTags.Clear();
    List<int> worldIdsSorted = ClusterManager.Instance.GetWorldIDsSorted();
    for (int index1 = 0; index1 < worldIdsSorted.Count; ++index1)
    {
      List<FetchChore> fetchChoreList;
      if (this.fetchMap.TryGetValue(worldIdsSorted[index1], out fetchChoreList))
      {
        for (int index2 = 0; index2 < fetchChoreList.Count; ++index2)
        {
          FetchChore fetchChore = fetchChoreList[index2];
          if ((fetchChore.choreType == storageFetch || fetchChore.choreType == foodFetch) && Object.op_Implicit((Object) fetchChore.destination))
          {
            int cell = Grid.PosToCell((KMonoBehaviour) fetchChore.destination);
            if (MinionGroupProber.Get().IsReachable(cell, fetchChore.destination.GetOffsets(cell)))
              this.storageFetchableTags.UnionWith((IEnumerable<Tag>) fetchChore.tags);
          }
        }
      }
    }
  }

  public bool ClearableHasDestination(Pickupable pickupable) => this.storageFetchableTags.Contains(pickupable.KPrefabID.PrefabTag);

  public struct Fetch
  {
    public FetchChore chore;
    public int idsHash;
    public int cost;
    public PrioritySetting priority;
    public Storage.FetchCategory category;

    public bool IsBetterThan(GlobalChoreProvider.Fetch fetch)
    {
      if (this.category != fetch.category || this.idsHash != fetch.idsHash || this.chore.choreType != fetch.chore.choreType)
        return false;
      if (this.priority.priority_class > fetch.priority.priority_class)
        return true;
      if (this.priority.priority_class == fetch.priority.priority_class)
      {
        if (this.priority.priority_value > fetch.priority.priority_value)
          return true;
        if (this.priority.priority_value == fetch.priority.priority_value)
          return this.cost <= fetch.cost;
      }
      return false;
    }
  }

  private class FetchComparer : IComparer<GlobalChoreProvider.Fetch>
  {
    public int Compare(GlobalChoreProvider.Fetch a, GlobalChoreProvider.Fetch b)
    {
      int num1 = b.priority.priority_class - a.priority.priority_class;
      if (num1 != 0)
        return num1;
      int num2 = b.priority.priority_value - a.priority.priority_value;
      return num2 != 0 ? num2 : a.cost - b.cost;
    }
  }

  private struct FindTopPriorityTask : IWorkItem<object>
  {
    private int start;
    private int end;
    private List<Prioritizable> worldCollection;
    public bool found;
    public static bool abort;

    public FindTopPriorityTask(int start, int end, List<Prioritizable> worldCollection)
    {
      this.start = start;
      this.end = end;
      this.worldCollection = worldCollection;
      this.found = false;
    }

    public void Run(object context)
    {
      if (GlobalChoreProvider.FindTopPriorityTask.abort)
        return;
      for (int start = this.start; start != this.end && this.worldCollection.Count > start; ++start)
      {
        if (!Object.op_Equality((Object) this.worldCollection[start], (Object) null) && this.worldCollection[start].IsTopPriority())
        {
          this.found = true;
          break;
        }
      }
      if (!this.found)
        return;
      GlobalChoreProvider.FindTopPriorityTask.abort = true;
    }
  }
}
