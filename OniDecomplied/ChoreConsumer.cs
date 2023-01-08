// Decompiled with JetBrains decompiler
// Type: ChoreConsumer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ChoreConsumer")]
public class ChoreConsumer : KMonoBehaviour, IPersonalPriorityManager
{
  public const int DEFAULT_PERSONAL_CHORE_PRIORITY = 3;
  public const int MIN_PERSONAL_PRIORITY = 0;
  public const int MAX_PERSONAL_PRIORITY = 5;
  public const int PRIORITY_DISABLED = 0;
  public const int PRIORITY_VERYLOW = 1;
  public const int PRIORITY_LOW = 2;
  public const int PRIORITY_FLAT = 3;
  public const int PRIORITY_HIGH = 4;
  public const int PRIORITY_VERYHIGH = 5;
  [MyCmpAdd]
  public ChoreProvider choreProvider;
  [MyCmpAdd]
  public ChoreDriver choreDriver;
  [MyCmpGet]
  public Navigator navigator;
  [MyCmpGet]
  public MinionResume resume;
  [MyCmpAdd]
  private User user;
  public System.Action choreRulesChanged;
  public bool debug;
  private List<ChoreProvider> providers = new List<ChoreProvider>();
  private List<Urge> urges = new List<Urge>();
  public ChoreTable choreTable;
  private ChoreTable.Instance choreTableInstance;
  public ChoreConsumerState consumerState;
  private Dictionary<Tag, ChoreConsumer.BehaviourPrecondition> behaviourPreconditions = new Dictionary<Tag, ChoreConsumer.BehaviourPrecondition>();
  private ChoreConsumer.PreconditionSnapshot preconditionSnapshot = new ChoreConsumer.PreconditionSnapshot();
  private ChoreConsumer.PreconditionSnapshot lastSuccessfulPreconditionSnapshot = new ChoreConsumer.PreconditionSnapshot();
  [Serialize]
  private Dictionary<HashedString, ChoreConsumer.PriorityInfo> choreGroupPriorities = new Dictionary<HashedString, ChoreConsumer.PriorityInfo>();
  private Dictionary<HashedString, int> choreTypePriorities = new Dictionary<HashedString, int>();
  private List<HashedString> traitDisabledChoreGroups = new List<HashedString>();
  private List<HashedString> userDisabledChoreGroups = new List<HashedString>();
  private int stationaryReach = -1;

  public List<ChoreProvider> GetProviders() => this.providers;

  public ChoreConsumer.PreconditionSnapshot GetLastPreconditionSnapshot() => this.preconditionSnapshot;

  public List<Chore.Precondition.Context> GetSuceededPreconditionContexts() => this.lastSuccessfulPreconditionSnapshot.succeededContexts;

  public List<Chore.Precondition.Context> GetFailedPreconditionContexts() => this.lastSuccessfulPreconditionSnapshot.failedContexts;

  public ChoreConsumer.PreconditionSnapshot GetLastSuccessfulPreconditionSnapshot() => this.lastSuccessfulPreconditionSnapshot;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (Object.op_Inequality((Object) ChoreGroupManager.instance, (Object) null))
    {
      foreach (KeyValuePair<Tag, int> keyValuePair in ChoreGroupManager.instance.DefaultChorePermission)
      {
        bool flag = false;
        Tag key;
        foreach (HashedString disabledChoreGroup in this.userDisabledChoreGroups)
        {
          int hashValue = ((HashedString) ref disabledChoreGroup).HashValue;
          key = keyValuePair.Key;
          int hashCode = key.GetHashCode();
          if (hashValue == hashCode)
          {
            flag = true;
            break;
          }
        }
        if (!flag && keyValuePair.Value == 0)
        {
          List<HashedString> disabledChoreGroups = this.userDisabledChoreGroups;
          key = keyValuePair.Key;
          HashedString hashedString = new HashedString(key.GetHashCode());
          disabledChoreGroups.Add(hashedString);
        }
      }
    }
    this.providers.Add(this.choreProvider);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    KPrefabID component = ((Component) this).GetComponent<KPrefabID>();
    if (this.choreTable != null)
      this.choreTableInstance = new ChoreTable.Instance(this.choreTable, component);
    foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
    {
      int personalPriority = this.GetPersonalPriority(resource);
      this.UpdateChoreTypePriorities(resource, personalPriority);
      this.SetPermittedByUser(resource, personalPriority != 0);
    }
    this.consumerState = new ChoreConsumerState(this);
  }

  protected virtual void OnForcedCleanUp()
  {
    if (this.consumerState != null)
      this.consumerState.navigator = (Navigator) null;
    this.navigator = (Navigator) null;
    base.OnForcedCleanUp();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (this.choreTableInstance == null)
      return;
    this.choreTableInstance.OnCleanUp(((Component) this).GetComponent<KPrefabID>());
    this.choreTableInstance = (ChoreTable.Instance) null;
  }

  public bool IsPermittedByUser(ChoreGroup chore_group) => chore_group == null || !this.userDisabledChoreGroups.Contains(chore_group.IdHash);

  public void SetPermittedByUser(ChoreGroup chore_group, bool is_allowed)
  {
    if (is_allowed)
    {
      if (!this.userDisabledChoreGroups.Remove(chore_group.IdHash))
        return;
      Util.Signal(this.choreRulesChanged);
    }
    else
    {
      if (this.userDisabledChoreGroups.Contains(chore_group.IdHash))
        return;
      this.userDisabledChoreGroups.Add(chore_group.IdHash);
      Util.Signal(this.choreRulesChanged);
    }
  }

  public bool IsPermittedByTraits(ChoreGroup chore_group) => chore_group == null || !this.traitDisabledChoreGroups.Contains(chore_group.IdHash);

  public void SetPermittedByTraits(ChoreGroup chore_group, bool is_enabled)
  {
    if (is_enabled)
    {
      if (!this.traitDisabledChoreGroups.Remove(chore_group.IdHash))
        return;
      Util.Signal(this.choreRulesChanged);
    }
    else
    {
      if (this.traitDisabledChoreGroups.Contains(chore_group.IdHash))
        return;
      this.traitDisabledChoreGroups.Add(chore_group.IdHash);
      Util.Signal(this.choreRulesChanged);
    }
  }

  private bool ChooseChore(
    ref Chore.Precondition.Context out_context,
    List<Chore.Precondition.Context> succeeded_contexts)
  {
    if (succeeded_contexts.Count == 0)
      return false;
    Chore currentChore = this.choreDriver.GetCurrentChore();
    if (currentChore == null)
    {
      for (int index = succeeded_contexts.Count - 1; index >= 0; --index)
      {
        Chore.Precondition.Context succeededContext = succeeded_contexts[index];
        if (succeededContext.IsSuccess())
        {
          out_context = succeededContext;
          return true;
        }
      }
    }
    else
    {
      int interruptPriority = Db.Get().ChoreTypes.TopPriority.interruptPriority;
      int num = currentChore.masterPriority.priority_class == PriorityScreen.PriorityClass.topPriority ? interruptPriority : currentChore.choreType.interruptPriority;
      for (int index = succeeded_contexts.Count - 1; index >= 0; --index)
      {
        Chore.Precondition.Context succeededContext = succeeded_contexts[index];
        if (succeededContext.IsSuccess() && (succeededContext.masterPriority.priority_class == PriorityScreen.PriorityClass.topPriority ? interruptPriority : succeededContext.interruptPriority) > num && !currentChore.choreType.interruptExclusion.Overlaps((IEnumerable<Tag>) succeededContext.chore.choreType.tags))
        {
          out_context = succeededContext;
          return true;
        }
      }
    }
    return false;
  }

  public bool FindNextChore(ref Chore.Precondition.Context out_context)
  {
    if (this.debug)
    {
      int num1 = 0 + 1;
    }
    this.preconditionSnapshot.Clear();
    this.consumerState.Refresh();
    if (this.consumerState.hasSolidTransferArm)
    {
      Debug.Assert(this.stationaryReach > 0);
      CellOffset offset = Grid.GetOffset(Grid.PosToCell((KMonoBehaviour) this));
      Extents extents = new Extents(offset.x, offset.y, this.stationaryReach);
      ListPool<ScenePartitionerEntry, ChoreConsumer>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ChoreConsumer>.Allocate();
      GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.fetchChoreLayer, (List<ScenePartitionerEntry>) gathered_entries);
      foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
      {
        if (partitionerEntry.obj == null)
          DebugUtil.Assert(false, "FindNextChore found an entry that was null");
        else if (!(partitionerEntry.obj is FetchChore fetchChore))
          DebugUtil.Assert(false, "FindNextChore found an entry that wasn't a FetchChore");
        else if (fetchChore.target == null)
          DebugUtil.Assert(false, "FindNextChore found an entry with a null target");
        else if (fetchChore.isNull)
          Debug.LogWarning((object) "FindNextChore found an entry that isNull");
        else if (this.consumerState.solidTransferArm.IsCellReachable(Grid.PosToCell(fetchChore.gameObject)))
          fetchChore.CollectChoresFromGlobalChoreProvider(this.consumerState, this.preconditionSnapshot.succeededContexts, this.preconditionSnapshot.failedContexts, false);
      }
      gathered_entries.Recycle();
    }
    else
    {
      for (int index = 0; index < this.providers.Count; ++index)
        this.providers[index].CollectChores(this.consumerState, this.preconditionSnapshot.succeededContexts, this.preconditionSnapshot.failedContexts);
    }
    this.preconditionSnapshot.succeededContexts.Sort();
    List<Chore.Precondition.Context> succeededContexts = this.preconditionSnapshot.succeededContexts;
    int num2 = this.ChooseChore(ref out_context, succeededContexts) ? 1 : 0;
    if (num2 == 0)
      return num2 != 0;
    this.preconditionSnapshot.CopyTo(this.lastSuccessfulPreconditionSnapshot);
    return num2 != 0;
  }

  public void AddProvider(ChoreProvider provider)
  {
    DebugUtil.Assert(Object.op_Inequality((Object) provider, (Object) null));
    this.providers.Add(provider);
  }

  public void RemoveProvider(ChoreProvider provider) => this.providers.Remove(provider);

  public void AddUrge(Urge urge)
  {
    DebugUtil.Assert(urge != null);
    this.urges.Add(urge);
    this.Trigger(-736698276, (object) urge);
  }

  public void RemoveUrge(Urge urge)
  {
    this.urges.Remove(urge);
    this.Trigger(231622047, (object) urge);
  }

  public bool HasUrge(Urge urge) => this.urges.Contains(urge);

  public List<Urge> GetUrges() => this.urges;

  [Conditional("ENABLE_LOGGER")]
  public void Log(string evt, string param)
  {
  }

  public bool IsPermittedOrEnabled(ChoreType chore_type, Chore chore)
  {
    if (chore_type.groups.Length == 0)
      return true;
    bool flag1 = false;
    bool flag2 = true;
    for (int index = 0; index < chore_type.groups.Length; ++index)
    {
      ChoreGroup group = chore_type.groups[index];
      if (!this.IsPermittedByTraits(group))
        flag2 = false;
      if (this.IsPermittedByUser(group))
        flag1 = true;
    }
    return flag1 & flag2;
  }

  public void SetReach(int reach) => this.stationaryReach = reach;

  public bool GetNavigationCost(IApproachable approachable, out int cost)
  {
    if (Object.op_Implicit((Object) this.navigator))
    {
      cost = this.navigator.GetNavigationCost(approachable);
      if (cost != -1)
        return true;
    }
    else if (this.consumerState.hasSolidTransferArm)
    {
      int cell = approachable.GetCell();
      if (this.consumerState.solidTransferArm.IsCellReachable(cell))
      {
        cost = Grid.GetCellRange(this.NaturalBuildingCell(), cell);
        return true;
      }
    }
    cost = 0;
    return false;
  }

  public bool GetNavigationCost(int cell, out int cost)
  {
    if (Object.op_Implicit((Object) this.navigator))
    {
      cost = this.navigator.GetNavigationCost(cell);
      if (cost != -1)
        return true;
    }
    else if (this.consumerState.hasSolidTransferArm && this.consumerState.solidTransferArm.IsCellReachable(cell))
    {
      cost = Grid.GetCellRange(this.NaturalBuildingCell(), cell);
      return true;
    }
    cost = 0;
    return false;
  }

  public bool CanReach(IApproachable approachable)
  {
    if (Object.op_Implicit((Object) this.navigator))
      return this.navigator.CanReach(approachable);
    return this.consumerState.hasSolidTransferArm && this.consumerState.solidTransferArm.IsCellReachable(approachable.GetCell());
  }

  public bool IsWithinReach(IApproachable approachable) => Object.op_Implicit((Object) this.navigator) ? !Object.op_Equality((Object) this, (Object) null) && !Object.op_Equality((Object) ((Component) this).gameObject, (Object) null) && Grid.IsCellOffsetOf(Grid.PosToCell((KMonoBehaviour) this), approachable.GetCell(), approachable.GetOffsets()) : this.consumerState.hasSolidTransferArm && this.consumerState.solidTransferArm.IsCellReachable(approachable.GetCell());

  public void ShowHoverTextOnHoveredItem(
    Chore.Precondition.Context context,
    KSelectable hover_obj,
    HoverTextDrawer drawer,
    SelectToolHoverTextCard hover_text_card)
  {
    if (context.chore.target.isNull || Object.op_Inequality((Object) context.chore.target.gameObject, (Object) ((Component) hover_obj).gameObject))
      return;
    drawer.NewLine();
    drawer.AddIndent();
    drawer.DrawText(context.chore.choreType.Name, hover_text_card.Styles_BodyText.Standard);
    if (context.IsSuccess())
      return;
    Chore.PreconditionInstance precondition = context.chore.GetPreconditions()[context.failedPreconditionId];
    string str1 = precondition.description;
    if (string.IsNullOrEmpty(str1))
      str1 = precondition.id;
    if (Object.op_Inequality((Object) context.chore.driver, (Object) null))
      str1 = str1.Replace("{Assignee}", ((Component) context.chore.driver).GetProperName());
    string str2 = str1.Replace("{Selected}", ((Component) this).GetProperName());
    drawer.DrawText(" (" + str2 + ")", hover_text_card.Styles_BodyText.Standard);
  }

  public void ShowHoverTextOnHoveredItem(
    KSelectable hover_obj,
    HoverTextDrawer drawer,
    SelectToolHoverTextCard hover_text_card)
  {
    bool flag = false;
    foreach (Chore.Precondition.Context succeededContext in this.preconditionSnapshot.succeededContexts)
    {
      if (succeededContext.chore.showAvailabilityInHoverText && !succeededContext.chore.target.isNull && !Object.op_Inequality((Object) succeededContext.chore.target.gameObject, (Object) ((Component) hover_obj).gameObject))
      {
        if (!flag)
        {
          drawer.NewLine();
          drawer.DrawText(DUPLICANTS.CHORES.PRECONDITIONS.HEADER.ToString().Replace("{Selected}", ((Component) this).GetProperName()), hover_text_card.Styles_BodyText.Standard);
          flag = true;
        }
        this.ShowHoverTextOnHoveredItem(succeededContext, hover_obj, drawer, hover_text_card);
      }
    }
    foreach (Chore.Precondition.Context failedContext in this.preconditionSnapshot.failedContexts)
    {
      if (failedContext.chore.showAvailabilityInHoverText && !failedContext.chore.target.isNull && !Object.op_Inequality((Object) failedContext.chore.target.gameObject, (Object) ((Component) hover_obj).gameObject))
      {
        if (!flag)
        {
          drawer.NewLine();
          drawer.DrawText(DUPLICANTS.CHORES.PRECONDITIONS.HEADER.ToString().Replace("{Selected}", ((Component) this).GetProperName()), hover_text_card.Styles_BodyText.Standard);
          flag = true;
        }
        this.ShowHoverTextOnHoveredItem(failedContext, hover_obj, drawer, hover_text_card);
      }
    }
  }

  public int GetPersonalPriority(ChoreType chore_type)
  {
    int num;
    if (!this.choreTypePriorities.TryGetValue(chore_type.IdHash, out num))
      num = 3;
    return Mathf.Clamp(num, 0, 5);
  }

  public int GetPersonalPriority(ChoreGroup group)
  {
    int num = 3;
    ChoreConsumer.PriorityInfo priorityInfo;
    if (this.choreGroupPriorities.TryGetValue(group.IdHash, out priorityInfo))
      num = priorityInfo.priority;
    return Mathf.Clamp(num, 0, 5);
  }

  public void SetPersonalPriority(ChoreGroup group, int value)
  {
    if (group.choreTypes == null)
      return;
    value = Mathf.Clamp(value, 0, 5);
    ChoreConsumer.PriorityInfo priorityInfo;
    if (!this.choreGroupPriorities.TryGetValue(group.IdHash, out priorityInfo))
      priorityInfo.priority = 3;
    this.choreGroupPriorities[group.IdHash] = new ChoreConsumer.PriorityInfo()
    {
      priority = value
    };
    this.UpdateChoreTypePriorities(group, value);
    this.SetPermittedByUser(group, value != 0);
  }

  public int GetAssociatedSkillLevel(ChoreGroup group) => (int) this.GetAttributes().GetValue(group.attribute.Id);

  private void UpdateChoreTypePriorities(ChoreGroup group, int value)
  {
    ChoreGroups choreGroups = Db.Get().ChoreGroups;
    foreach (ChoreType choreType1 in group.choreTypes)
    {
      int num = 0;
      foreach (ChoreGroup resource in choreGroups.resources)
      {
        if (resource.choreTypes != null)
        {
          foreach (Resource choreType2 in resource.choreTypes)
          {
            if (HashedString.op_Equality(choreType2.IdHash, choreType1.IdHash))
            {
              int personalPriority = this.GetPersonalPriority(resource);
              num = Mathf.Max(num, personalPriority);
            }
          }
        }
      }
      this.choreTypePriorities[choreType1.IdHash] = num;
    }
  }

  public void ResetPersonalPriorities()
  {
  }

  public bool RunBehaviourPrecondition(Tag tag)
  {
    ChoreConsumer.BehaviourPrecondition behaviourPrecondition = new ChoreConsumer.BehaviourPrecondition();
    return this.behaviourPreconditions.TryGetValue(tag, out behaviourPrecondition) && behaviourPrecondition.cb(behaviourPrecondition.arg);
  }

  public void AddBehaviourPrecondition(Tag tag, Func<object, bool> precondition, object arg)
  {
    DebugUtil.Assert(!this.behaviourPreconditions.ContainsKey(tag));
    this.behaviourPreconditions[tag] = new ChoreConsumer.BehaviourPrecondition()
    {
      cb = precondition,
      arg = arg
    };
  }

  public void RemoveBehaviourPrecondition(Tag tag, Func<object, bool> precondition, object arg) => this.behaviourPreconditions.Remove(tag);

  public bool IsChoreEqualOrAboveCurrentChorePriority<StateMachineType>()
  {
    Chore currentChore = this.choreDriver.GetCurrentChore();
    return currentChore == null || currentChore.choreType.priority <= this.choreTable.GetChorePriority<StateMachineType>(this);
  }

  public bool IsChoreGroupDisabled(ChoreGroup chore_group)
  {
    bool flag = false;
    Traits component = ((Component) this).gameObject.GetComponent<Traits>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.IsChoreGroupDisabled(chore_group))
      flag = true;
    return flag;
  }

  public Dictionary<HashedString, ChoreConsumer.PriorityInfo> GetChoreGroupPriorities() => this.choreGroupPriorities;

  public void SetChoreGroupPriorities(
    Dictionary<HashedString, ChoreConsumer.PriorityInfo> priorities)
  {
    this.choreGroupPriorities = priorities;
  }

  private struct BehaviourPrecondition
  {
    public Func<object, bool> cb;
    public object arg;
  }

  public class PreconditionSnapshot
  {
    public List<Chore.Precondition.Context> succeededContexts = new List<Chore.Precondition.Context>();
    public List<Chore.Precondition.Context> failedContexts = new List<Chore.Precondition.Context>();
    public bool doFailedContextsNeedSorting = true;

    public void CopyTo(ChoreConsumer.PreconditionSnapshot snapshot)
    {
      snapshot.Clear();
      snapshot.succeededContexts.AddRange((IEnumerable<Chore.Precondition.Context>) this.succeededContexts);
      snapshot.failedContexts.AddRange((IEnumerable<Chore.Precondition.Context>) this.failedContexts);
      snapshot.doFailedContextsNeedSorting = true;
    }

    public void Clear()
    {
      this.succeededContexts.Clear();
      this.failedContexts.Clear();
      this.doFailedContextsNeedSorting = true;
    }
  }

  public struct PriorityInfo
  {
    public int priority;
  }
}
