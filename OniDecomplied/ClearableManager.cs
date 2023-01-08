// Decompiled with JetBrains decompiler
// Type: ClearableManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

internal class ClearableManager
{
  private KCompactedVector<ClearableManager.MarkedClearable> markedClearables = new KCompactedVector<ClearableManager.MarkedClearable>(0);
  private List<ClearableManager.SortedClearable> sortedClearables = new List<ClearableManager.SortedClearable>();

  public HandleVector<int>.Handle RegisterClearable(Clearable clearable) => this.markedClearables.Allocate(new ClearableManager.MarkedClearable()
  {
    clearable = clearable,
    pickupable = ((Component) clearable).GetComponent<Pickupable>(),
    prioritizable = ((Component) clearable).GetComponent<Prioritizable>()
  });

  public void UnregisterClearable(HandleVector<int>.Handle handle) => this.markedClearables.Free(handle);

  public void CollectAndSortClearables(Navigator navigator)
  {
    this.sortedClearables.Clear();
    foreach (ClearableManager.MarkedClearable data in this.markedClearables.GetDataList())
    {
      int navigationCost = data.pickupable.GetNavigationCost(navigator, data.pickupable.cachedCell);
      if (navigationCost != -1)
        this.sortedClearables.Add(new ClearableManager.SortedClearable()
        {
          pickupable = data.pickupable,
          masterPriority = data.prioritizable.GetMasterPriority(),
          cost = navigationCost
        });
    }
    this.sortedClearables.Sort((IComparer<ClearableManager.SortedClearable>) ClearableManager.SortedClearable.comparer);
  }

  public void CollectChores(
    List<GlobalChoreProvider.Fetch> fetches,
    ChoreConsumerState consumer_state,
    List<Chore.Precondition.Context> succeeded,
    List<Chore.Precondition.Context> failed_contexts)
  {
    ChoreType transport = Db.Get().ChoreTypes.Transport;
    int personalPriority = consumer_state.consumer.GetPersonalPriority(transport);
    int num = Game.Instance.advancedPersonalPriorities ? transport.explicitPriority : transport.priority;
    bool flag = false;
    for (int index1 = 0; index1 < this.sortedClearables.Count; ++index1)
    {
      ClearableManager.SortedClearable sortedClearable = this.sortedClearables[index1];
      Pickupable pickupable = sortedClearable.pickupable;
      PrioritySetting masterPriority = sortedClearable.masterPriority;
      Chore.Precondition.Context context = new Chore.Precondition.Context();
      context.personalPriority = personalPriority;
      KPrefabID kprefabId = pickupable.KPrefabID;
      for (int index2 = 0; fetches != null && index2 < fetches.Count; ++index2)
      {
        GlobalChoreProvider.Fetch fetch = fetches[index2];
        if ((fetch.chore.criteria != FetchChore.MatchCriteria.MatchID || !fetch.chore.tags.Contains(kprefabId.PrefabTag) ? (fetch.chore.criteria != FetchChore.MatchCriteria.MatchTags ? 0 : (kprefabId.HasTag(fetch.chore.tagsFirst) ? 1 : 0)) : 1) != 0)
        {
          context.Set((Chore) fetch.chore, consumer_state, false, (object) pickupable);
          context.choreTypeForPermission = transport;
          context.RunPreconditions();
          if (context.IsSuccess())
          {
            context.masterPriority = masterPriority;
            context.priority = num;
            context.interruptPriority = transport.interruptPriority;
            succeeded.Add(context);
            flag = true;
            break;
          }
        }
      }
      if (flag)
        break;
    }
  }

  private struct MarkedClearable
  {
    public Clearable clearable;
    public Pickupable pickupable;
    public Prioritizable prioritizable;
  }

  private struct SortedClearable
  {
    public Pickupable pickupable;
    public PrioritySetting masterPriority;
    public int cost;
    public static ClearableManager.SortedClearable.Comparer comparer = new ClearableManager.SortedClearable.Comparer();

    public class Comparer : IComparer<ClearableManager.SortedClearable>
    {
      public int Compare(ClearableManager.SortedClearable a, ClearableManager.SortedClearable b)
      {
        int num = b.masterPriority.priority_value - a.masterPriority.priority_value;
        return num == 0 ? a.cost - b.cost : num;
      }
    }
  }
}
