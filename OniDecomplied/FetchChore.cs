// Decompiled with JetBrains decompiler
// Type: FetchChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FetchChore : Chore<FetchChore.StatesInstance>
{
  public HashSet<Tag> tags;
  public Tag tagsFirst;
  public FetchChore.MatchCriteria criteria;
  public int tagsHash;
  public Tag requiredTag;
  public Tag[] forbiddenTags;
  public int forbidHash;
  public Automatable automatable;
  public bool allowMultifetch = true;
  private HandleVector<int>.Handle partitionerEntry;
  public static readonly Chore.Precondition IsFetchTargetAvailable = new Chore.Precondition()
  {
    id = nameof (IsFetchTargetAvailable),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_FETCH_TARGET_AVAILABLE,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      FetchChore chore = (FetchChore) context.chore;
      Pickupable pickup = (Pickupable) context.data;
      bool flag;
      if (Object.op_Equality((Object) pickup, (Object) null))
      {
        pickup = chore.FindFetchTarget(context.consumerState);
        flag = Object.op_Inequality((Object) pickup, (Object) null);
      }
      else
        flag = FetchManager.IsFetchablePickup(pickup, chore, context.consumerState.storage);
      if (flag)
      {
        if (Object.op_Equality((Object) pickup, (Object) null))
        {
          Debug.Log((object) string.Format("Failed to find fetch target for {0}", (object) chore.destination));
          return false;
        }
        context.data = (object) pickup;
        int cost;
        if (context.consumerState.consumer.GetNavigationCost((IApproachable) pickup, out cost))
        {
          context.cost += cost;
          return true;
        }
      }
      return false;
    })
  };

  public float originalAmount => this.smi.sm.requestedamount.Get(this.smi);

  public float amount
  {
    get => this.smi.sm.actualamount.Get(this.smi);
    set
    {
      double num = (double) this.smi.sm.actualamount.Set(value, this.smi);
    }
  }

  public Pickupable fetchTarget
  {
    get => this.smi.sm.chunk.Get<Pickupable>(this.smi);
    set => this.smi.sm.chunk.Set((KMonoBehaviour) value, this.smi);
  }

  public GameObject fetcher
  {
    get => this.smi.sm.fetcher.Get(this.smi);
    set => this.smi.sm.fetcher.Set(value, this.smi, false);
  }

  public Storage destination { get; private set; }

  public void FetchAreaBegin(Chore.Precondition.Context context, float amount_to_be_fetched)
  {
    this.amount = amount_to_be_fetched;
    this.smi.sm.fetcher.Set(context.consumerState.gameObject, this.smi, false);
    ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, 1f, context.chore.choreType.Name, GameUtil.GetChoreName((Chore) this, context.data));
    base.Begin(context);
  }

  public void FetchAreaEnd(ChoreDriver driver, Pickupable pickupable, bool is_success)
  {
    if (is_success)
    {
      ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, -1f, this.choreType.Name, GameUtil.GetChoreName((Chore) this, (object) pickupable));
      this.fetchTarget = pickupable;
      this.driver = driver;
      this.fetcher = ((Component) driver).gameObject;
      this.Succeed(nameof (FetchAreaEnd));
      ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().LogFetchChore(this.fetcher, this.choreType);
    }
    else
    {
      this.SetOverrideTarget((ChoreConsumer) null);
      this.Fail("FetchAreaFail");
    }
  }

  public Pickupable FindFetchTarget(ChoreConsumerState consumer_state)
  {
    if (!Object.op_Inequality((Object) this.destination, (Object) null))
      return (Pickupable) null;
    return consumer_state.hasSolidTransferArm ? consumer_state.solidTransferArm.FindFetchTarget(this.destination, this) : Game.Instance.fetchManager.FindFetchTarget(this.destination, this);
  }

  public override void Begin(Chore.Precondition.Context context)
  {
    Pickupable pickupable = (Pickupable) context.data;
    if (Object.op_Equality((Object) pickupable, (Object) null))
      pickupable = this.FindFetchTarget(context.consumerState);
    this.smi.sm.source.Set(((Component) pickupable).gameObject, this.smi, false);
    pickupable.Subscribe(-1582839653, new Action<object>(this.OnTagsChanged));
    base.Begin(context);
  }

  protected override void End(string reason)
  {
    Pickupable pickupable = this.smi.sm.source.Get<Pickupable>(this.smi);
    if (Object.op_Inequality((Object) pickupable, (Object) null))
      pickupable.Unsubscribe(-1582839653, new Action<object>(this.OnTagsChanged));
    base.End(reason);
  }

  private void OnTagsChanged(object data)
  {
    if (!Object.op_Inequality((Object) this.smi.sm.chunk.Get(this.smi), (Object) null))
      return;
    this.Fail("Tags changed");
  }

  public override void PrepareChore(ref Chore.Precondition.Context context) => context.chore = (Chore) new FetchAreaChore(context);

  public float AmountWaitingToFetch() => Object.op_Equality((Object) this.fetcher, (Object) null) ? this.originalAmount : this.amount;

  public FetchChore(
    ChoreType choreType,
    Storage destination,
    float amount,
    HashSet<Tag> tags,
    FetchChore.MatchCriteria criteria,
    Tag required_tag,
    Tag[] forbidden_tags = null,
    ChoreProvider chore_provider = null,
    bool run_until_complete = true,
    Action<Chore> on_complete = null,
    Action<Chore> on_begin = null,
    Action<Chore> on_end = null,
    Operational.State operational_requirement = Operational.State.Operational,
    int priority_mod = 0)
    : base(choreType, (IStateMachineTarget) destination, chore_provider, run_until_complete, on_complete, on_begin, on_end, priority_mod: priority_mod)
  {
    if (choreType == null)
      Debug.LogError((object) "You must specify a chore type for fetching!");
    this.tagsFirst = tags.Count > 0 ? ((IEnumerable<Tag>) tags).First<Tag>() : Tag.Invalid;
    if ((double) amount <= (double) PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) string.Format("Chore {0} is requesting {1} {2} to {3}", (object) choreType.Id, (object) this.tagsFirst, (object) amount, Object.op_Inequality((Object) destination, (Object) null) ? (object) ((Object) destination).name : (object) "to nowhere")
      });
    this.SetPrioritizable(Object.op_Inequality((Object) destination.prioritizable, (Object) null) ? destination.prioritizable : ((Component) destination).GetComponent<Prioritizable>());
    this.smi = new FetchChore.StatesInstance(this);
    double num = (double) this.smi.sm.requestedamount.Set(amount, this.smi);
    this.destination = destination;
    DebugUtil.DevAssert(criteria != FetchChore.MatchCriteria.MatchTags || tags.Count <= 1, "For performance reasons fetch chores are limited to one tag when matching tags!", (Object) null);
    this.tags = tags;
    this.criteria = criteria;
    this.tagsHash = FetchChore.ComputeHashCodeForTags((IEnumerable<Tag>) tags);
    this.requiredTag = required_tag;
    this.forbiddenTags = forbidden_tags != null ? forbidden_tags : new Tag[0];
    this.forbidHash = FetchChore.ComputeHashCodeForTags((IEnumerable<Tag>) this.forbiddenTags);
    DebugUtil.DevAssert(!tags.Contains(GameTags.Preserved), "Fetch chore fetching invalid tags.", (Object) null);
    if (destination.GetOnlyFetchMarkedItems())
    {
      DebugUtil.DevAssert(!((Tag) ref this.requiredTag).IsValid, "Only one requiredTag is supported at a time, this will stomp!", (Object) null);
      this.requiredTag = GameTags.Garbage;
    }
    this.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, (object) Db.Get().ScheduleBlockTypes.Work);
    this.AddPrecondition(ChorePreconditions.instance.CanMoveTo, (object) destination);
    this.AddPrecondition(FetchChore.IsFetchTargetAvailable);
    Deconstructable component1 = this.target.GetComponent<Deconstructable>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      this.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, (object) component1);
    BuildingEnabledButton component2 = this.target.GetComponent<BuildingEnabledButton>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      this.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, (object) component2);
    if (operational_requirement != Operational.State.None)
    {
      Operational component3 = ((Component) destination).GetComponent<Operational>();
      if (Object.op_Inequality((Object) component3, (Object) null))
      {
        Chore.Precondition precondition = ChorePreconditions.instance.IsOperational;
        if (operational_requirement == Operational.State.Functional)
          precondition = ChorePreconditions.instance.IsFunctional;
        this.AddPrecondition(precondition, (object) component3);
      }
    }
    this.partitionerEntry = GameScenePartitioner.Instance.Add(((Object) destination).name, (object) this, Grid.PosToCell((KMonoBehaviour) destination), GameScenePartitioner.Instance.fetchChoreLayer, (Action<object>) null);
    destination.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
    this.automatable = ((Component) destination).GetComponent<Automatable>();
    if (!Object.op_Implicit((Object) this.automatable))
      return;
    this.AddPrecondition(ChorePreconditions.instance.IsAllowedByAutomation, (object) this.automatable);
  }

  private void OnOnlyFetchMarkedItemsSettingChanged(object data)
  {
    if (!Object.op_Inequality((Object) this.destination, (Object) null))
      return;
    if (this.destination.GetOnlyFetchMarkedItems())
    {
      DebugUtil.DevAssert(!((Tag) ref this.requiredTag).IsValid, "Only one requiredTag is supported at a time, this will stomp!", (Object) null);
      this.requiredTag = GameTags.Garbage;
    }
    else
      this.requiredTag = Tag.Invalid;
  }

  private void OnMasterPriorityChanged(
    PriorityScreen.PriorityClass priorityClass,
    int priority_value)
  {
    this.masterPriority.priority_class = priorityClass;
    this.masterPriority.priority_value = priority_value;
  }

  public override void CollectChores(
    ChoreConsumerState consumer_state,
    List<Chore.Precondition.Context> succeeded_contexts,
    List<Chore.Precondition.Context> failed_contexts,
    bool is_attempting_override)
  {
  }

  public void CollectChoresFromGlobalChoreProvider(
    ChoreConsumerState consumer_state,
    List<Chore.Precondition.Context> succeeded_contexts,
    List<Chore.Precondition.Context> failed_contexts,
    bool is_attempting_override)
  {
    base.CollectChores(consumer_state, succeeded_contexts, failed_contexts, is_attempting_override);
  }

  public override void Cleanup()
  {
    base.Cleanup();
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    if (!Object.op_Inequality((Object) this.destination, (Object) null))
      return;
    this.destination.Unsubscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
  }

  public static int ComputeHashCodeForTags(IEnumerable<Tag> tags)
  {
    int hashCodeForTags = 123137;
    foreach (Tag tag in new SortedSet<Tag>(tags))
      hashCodeForTags = (hashCodeForTags << 5) + hashCodeForTags ^ ((Tag) ref tag).GetHash();
    return hashCodeForTags;
  }

  public enum MatchCriteria
  {
    MatchID,
    MatchTags,
  }

  public class StatesInstance : 
    GameStateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.GameInstance
  {
    public StatesInstance(FetchChore master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore>
  {
    public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.TargetParameter fetcher;
    public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.TargetParameter source;
    public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.TargetParameter chunk;
    public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.FloatParameter requestedamount;
    public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.FloatParameter actualamount;

    public override void InitializeStates(out StateMachine.BaseState default_state) => default_state = (StateMachine.BaseState) this.root;
  }
}
