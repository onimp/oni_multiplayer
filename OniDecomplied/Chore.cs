// Decompiled with JetBrains decompiler
// Type: Chore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Chore
{
  private static int nextId;
  public bool isExpanded;
  public bool showAvailabilityInHoverText = true;
  public PrioritySetting masterPriority;
  public Action<Chore> onExit;
  public Action<Chore> onComplete;
  private Action<Chore> onBegin;
  private Action<Chore> onEnd;
  public Action<Chore> onCleanup;
  public bool debug;
  private List<Chore.PreconditionInstance> preconditions = new List<Chore.PreconditionInstance>();
  private bool arePreconditionsDirty;
  public bool addToDailyReport;
  public ReportManager.ReportType reportType;
  private Prioritizable prioritizable;
  public const int MAX_PLAYER_BASIC_PRIORITY = 9;
  public const int MIN_PLAYER_BASIC_PRIORITY = 1;
  public const int MAX_PLAYER_HIGH_PRIORITY = 0;
  public const int MIN_PLAYER_HIGH_PRIORITY = 0;
  public const int MAX_PLAYER_EMERGENCY_PRIORITY = 1;
  public const int MIN_PLAYER_EMERGENCY_PRIORITY = 1;
  public const int DEFAULT_BASIC_PRIORITY = 5;
  public const int MAX_BASIC_PRIORITY = 10;
  public const int MIN_BASIC_PRIORITY = 0;
  public static bool ENABLE_PERSONAL_PRIORITIES = true;
  public static PrioritySetting DefaultPrioritySetting = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);

  public int id { get; private set; }

  public ChoreDriver driver { get; set; }

  public ChoreDriver lastDriver { get; set; }

  protected abstract StateMachine.Instance GetSMI();

  public ChoreType choreType { get; set; }

  public ChoreProvider provider { get; set; }

  public ChoreConsumer overrideTarget { get; private set; }

  public bool isComplete { get; protected set; }

  public IStateMachineTarget target { get; protected set; }

  public bool runUntilComplete { get; set; }

  public int priorityMod { get; set; }

  public bool InProgress() => Object.op_Inequality((Object) this.driver, (Object) null);

  public abstract GameObject gameObject { get; }

  public abstract bool isNull { get; }

  public bool IsValid() => Object.op_Inequality((Object) this.provider, (Object) null) && this.gameObject.GetMyWorldId() != -1;

  public bool IsPreemptable { get; protected set; }

  public Chore(
    ChoreType chore_type,
    IStateMachineTarget target,
    ChoreProvider chore_provider,
    bool run_until_complete,
    Action<Chore> on_complete,
    Action<Chore> on_begin,
    Action<Chore> on_end,
    PriorityScreen.PriorityClass priority_class,
    int priority_value,
    bool is_preemptable,
    bool allow_in_context_menu,
    int priority_mod,
    bool add_to_daily_report,
    ReportManager.ReportType report_type)
  {
    this.target = target;
    if (priority_value == int.MaxValue)
    {
      priority_class = PriorityScreen.PriorityClass.topPriority;
      priority_value = 2;
    }
    if (priority_value < 1 || priority_value > 9)
      Debug.LogErrorFormat("Priority Value Out Of Range: {0}", new object[1]
      {
        (object) priority_value
      });
    this.masterPriority = new PrioritySetting(priority_class, priority_value);
    this.priorityMod = priority_mod;
    this.id = ++Chore.nextId;
    if (Object.op_Equality((Object) chore_provider, (Object) null))
    {
      chore_provider = (ChoreProvider) GlobalChoreProvider.Instance;
      DebugUtil.Assert(Object.op_Inequality((Object) chore_provider, (Object) null));
    }
    this.choreType = chore_type;
    this.runUntilComplete = run_until_complete;
    this.onComplete = on_complete;
    this.onEnd = on_end;
    this.onBegin = on_begin;
    this.IsPreemptable = is_preemptable;
    this.AddPrecondition(ChorePreconditions.instance.IsValid);
    this.AddPrecondition(ChorePreconditions.instance.IsPermitted);
    this.AddPrecondition(ChorePreconditions.instance.IsPreemptable);
    this.AddPrecondition(ChorePreconditions.instance.HasUrge);
    this.AddPrecondition(ChorePreconditions.instance.IsMoreSatisfyingEarly);
    this.AddPrecondition(ChorePreconditions.instance.IsMoreSatisfyingLate);
    this.AddPrecondition(ChorePreconditions.instance.IsOverrideTargetNullOrMe);
    chore_provider.AddChore(this);
  }

  public virtual void Cleanup() => this.ClearPrioritizable();

  public void SetPriorityMod(int priorityMod) => this.priorityMod = priorityMod;

  public List<Chore.PreconditionInstance> GetPreconditions()
  {
    if (this.arePreconditionsDirty)
    {
      this.preconditions.Sort((Comparison<Chore.PreconditionInstance>) ((x, y) => x.sortOrder.CompareTo(y.sortOrder)));
      this.arePreconditionsDirty = false;
    }
    return this.preconditions;
  }

  protected void SetPrioritizable(Prioritizable prioritizable)
  {
    if (!Object.op_Inequality((Object) prioritizable, (Object) null) || !prioritizable.IsPrioritizable())
      return;
    this.prioritizable = prioritizable;
    this.masterPriority = prioritizable.GetMasterPriority();
    prioritizable.onPriorityChanged += new Action<PrioritySetting>(this.OnMasterPriorityChanged);
  }

  private void ClearPrioritizable()
  {
    if (!Object.op_Inequality((Object) this.prioritizable, (Object) null))
      return;
    this.prioritizable.onPriorityChanged -= new Action<PrioritySetting>(this.OnMasterPriorityChanged);
  }

  private void OnMasterPriorityChanged(PrioritySetting priority) => this.masterPriority = priority;

  public void SetOverrideTarget(ChoreConsumer chore_consumer)
  {
    if (Object.op_Inequality((Object) chore_consumer, (Object) null))
    {
      string name = ((Object) chore_consumer).name;
    }
    this.overrideTarget = chore_consumer;
    this.Fail("New override target");
  }

  public void AddPrecondition(Chore.Precondition precondition, object data = null)
  {
    this.arePreconditionsDirty = true;
    this.preconditions.Add(new Chore.PreconditionInstance()
    {
      id = precondition.id,
      description = precondition.description,
      sortOrder = precondition.sortOrder,
      fn = precondition.fn,
      data = data
    });
  }

  public virtual void CollectChores(
    ChoreConsumerState consumer_state,
    List<Chore.Precondition.Context> succeeded_contexts,
    List<Chore.Precondition.Context> failed_contexts,
    bool is_attempting_override)
  {
    Chore.Precondition.Context context = new Chore.Precondition.Context(this, consumer_state, is_attempting_override);
    context.RunPreconditions();
    if (context.IsSuccess())
      succeeded_contexts.Add(context);
    else
      failed_contexts.Add(context);
  }

  public bool SatisfiesUrge(Urge urge) => urge == this.choreType.urge;

  public ReportManager.ReportType GetReportType() => this.reportType;

  public virtual void PrepareChore(ref Chore.Precondition.Context context)
  {
  }

  public virtual string ResolveString(string str) => str;

  public virtual void Begin(Chore.Precondition.Context context)
  {
    if (Object.op_Inequality((Object) this.driver, (Object) null))
      Debug.LogErrorFormat("Chore.Begin driver already set {0} {1} {2}, provider {3}, driver {4} -> {5}", new object[6]
      {
        (object) this.id,
        (object) this.GetType(),
        (object) this.choreType.Id,
        (object) this.provider,
        (object) this.driver,
        (object) context.consumerState.choreDriver
      });
    if (Object.op_Equality((Object) this.provider, (Object) null))
      Debug.LogErrorFormat("Chore.Begin provider is null {0} {1} {2}, provider {3}, driver {4}", new object[5]
      {
        (object) this.id,
        (object) this.GetType(),
        (object) this.choreType.Id,
        (object) this.provider,
        (object) this.driver
      });
    this.driver = context.consumerState.choreDriver;
    StateMachine.Instance smi = this.GetSMI();
    smi.OnStop += new Action<string, StateMachine.Status>(this.OnStateMachineStop);
    KSelectable component = ((Component) this.driver).GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.SetStatusItem(Db.Get().StatusItemCategories.Main, this.GetStatusItem(), (object) this);
    smi.StartSM();
    if (this.onBegin == null)
      return;
    this.onBegin(this);
  }

  protected virtual void End(string reason)
  {
    if (Object.op_Inequality((Object) this.driver, (Object) null))
    {
      KSelectable component = ((Component) this.driver).GetComponent<KSelectable>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.SetStatusItem(Db.Get().StatusItemCategories.Main, (StatusItem) null);
    }
    StateMachine.Instance smi = this.GetSMI();
    smi.OnStop -= new Action<string, StateMachine.Status>(this.OnStateMachineStop);
    smi.StopSM(reason);
    if (Object.op_Equality((Object) this.driver, (Object) null))
      return;
    this.lastDriver = this.driver;
    this.driver = (ChoreDriver) null;
    if (this.onEnd != null)
      this.onEnd(this);
    if (this.onExit != null)
      this.onExit(this);
    this.driver = (ChoreDriver) null;
  }

  protected void Succeed(string reason)
  {
    if (!this.RemoveFromProvider())
      return;
    this.isComplete = true;
    if (this.onComplete != null)
      this.onComplete(this);
    if (this.addToDailyReport)
    {
      ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, -1f, this.choreType.Name, GameUtil.GetChoreName(this, (object) null));
      ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().LogSuitChore(Object.op_Inequality((Object) this.driver, (Object) null) ? this.driver : this.lastDriver);
    }
    this.End(reason);
    this.Cleanup();
  }

  protected virtual StatusItem GetStatusItem() => this.choreType.statusItem;

  public virtual void Fail(string reason)
  {
    if (Object.op_Equality((Object) this.provider, (Object) null) || Object.op_Equality((Object) this.driver, (Object) null))
      return;
    if (!this.runUntilComplete)
      this.Cancel(reason);
    else
      this.End(reason);
  }

  public void Cancel(string reason)
  {
    if (!this.RemoveFromProvider())
      return;
    if (this.addToDailyReport)
    {
      ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, -1f, this.choreType.Name, GameUtil.GetChoreName(this, (object) null));
      ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().LogSuitChore(Object.op_Inequality((Object) this.driver, (Object) null) ? this.driver : this.lastDriver);
    }
    this.End(reason);
    this.Cleanup();
  }

  protected virtual void OnStateMachineStop(string reason, StateMachine.Status status)
  {
    if (status == StateMachine.Status.Success)
      this.Succeed(reason);
    else
      this.Fail(reason);
  }

  private bool RemoveFromProvider()
  {
    if (!Object.op_Inequality((Object) this.provider, (Object) null))
      return false;
    this.provider.RemoveChore(this);
    return true;
  }

  public virtual bool CanPreempt(Chore.Precondition.Context context) => this.IsPreemptable;

  protected virtual void ShowCustomEditor(string filter, int width)
  {
  }

  public virtual string GetReportName(string context = null) => context == null || this.choreType.reportName == null ? this.choreType.Name : string.Format(this.choreType.reportName, (object) context);

  public delegate bool PreconditionFn(ref Chore.Precondition.Context context, object data);

  public struct PreconditionInstance
  {
    public string id;
    public string description;
    public int sortOrder;
    public Chore.PreconditionFn fn;
    public object data;
  }

  public struct Precondition
  {
    public string id;
    public string description;
    public int sortOrder;
    public Chore.PreconditionFn fn;

    [DebuggerDisplay("{chore.GetType()}, {chore.gameObject.name}")]
    public struct Context : 
      IComparable<Chore.Precondition.Context>,
      IEquatable<Chore.Precondition.Context>
    {
      public PrioritySetting masterPriority;
      public int personalPriority;
      public int priority;
      public int priorityMod;
      public int interruptPriority;
      public int cost;
      public int consumerPriority;
      public Chore chore;
      public ChoreConsumerState consumerState;
      public int failedPreconditionId;
      public object data;
      public bool isAttemptingOverride;
      public ChoreType choreTypeForPermission;
      public bool skipMoreSatisfyingEarlyPrecondition;

      public Context(
        Chore chore,
        ChoreConsumerState consumer_state,
        bool is_attempting_override,
        object data = null)
      {
        this.masterPriority = chore.masterPriority;
        this.personalPriority = consumer_state.consumer.GetPersonalPriority(chore.choreType);
        this.priority = 0;
        this.priorityMod = chore.priorityMod;
        this.consumerPriority = 0;
        this.interruptPriority = 0;
        this.cost = 0;
        this.chore = chore;
        this.consumerState = consumer_state;
        this.failedPreconditionId = -1;
        this.isAttemptingOverride = is_attempting_override;
        this.data = data;
        this.choreTypeForPermission = chore.choreType;
        this.skipMoreSatisfyingEarlyPrecondition = Object.op_Inequality((Object) RootMenu.Instance, (Object) null) && RootMenu.Instance.IsBuildingChorePanelActive();
        this.SetPriority(chore);
      }

      public void Set(
        Chore chore,
        ChoreConsumerState consumer_state,
        bool is_attempting_override,
        object data = null)
      {
        this.masterPriority = chore.masterPriority;
        this.priority = 0;
        this.priorityMod = chore.priorityMod;
        this.consumerPriority = 0;
        this.interruptPriority = 0;
        this.cost = 0;
        this.chore = chore;
        this.consumerState = consumer_state;
        this.failedPreconditionId = -1;
        this.isAttemptingOverride = is_attempting_override;
        this.data = data;
        this.choreTypeForPermission = chore.choreType;
        this.SetPriority(chore);
      }

      public void SetPriority(Chore chore)
      {
        this.priority = Game.Instance.advancedPersonalPriorities ? chore.choreType.explicitPriority : chore.choreType.priority;
        this.priorityMod = chore.priorityMod;
        this.interruptPriority = chore.choreType.interruptPriority;
      }

      public bool IsSuccess() => this.failedPreconditionId == -1;

      public bool IsPotentialSuccess()
      {
        if (this.IsSuccess() || Object.op_Equality((Object) this.chore.driver, (Object) this.consumerState.choreDriver))
          return true;
        if (this.failedPreconditionId != -1)
        {
          if (this.failedPreconditionId >= 0 && this.failedPreconditionId < this.chore.preconditions.Count)
            return this.chore.preconditions[this.failedPreconditionId].id == ChorePreconditions.instance.IsMoreSatisfyingLate.id;
          DebugUtil.DevLogErrorFormat("failedPreconditionId out of range {0}/{1}", new object[2]
          {
            (object) this.failedPreconditionId,
            (object) this.chore.preconditions.Count
          });
        }
        return false;
      }

      public void RunPreconditions()
      {
        if (this.chore.debug)
        {
          int num1 = 0 + 1;
          if (this.consumerState.consumer.debug)
          {
            int num2 = num1 + 1;
            Debugger.Break();
          }
        }
        if (this.chore.arePreconditionsDirty)
        {
          this.chore.preconditions.Sort((Comparison<Chore.PreconditionInstance>) ((x, y) => x.sortOrder.CompareTo(y.sortOrder)));
          this.chore.arePreconditionsDirty = false;
        }
        for (int index = 0; index < this.chore.preconditions.Count; ++index)
        {
          Chore.PreconditionInstance precondition = this.chore.preconditions[index];
          if (!precondition.fn(ref this, precondition.data))
          {
            this.failedPreconditionId = index;
            break;
          }
        }
      }

      public int CompareTo(Chore.Precondition.Context obj)
      {
        bool flag1 = this.failedPreconditionId != -1;
        bool flag2 = obj.failedPreconditionId != -1;
        if (flag1 == flag2)
        {
          int num1 = this.masterPriority.priority_class - obj.masterPriority.priority_class;
          if (num1 != 0)
            return num1;
          int num2 = this.personalPriority - obj.personalPriority;
          if (num2 != 0)
            return num2;
          int num3 = this.masterPriority.priority_value - obj.masterPriority.priority_value;
          if (num3 != 0)
            return num3;
          int num4 = this.priority - obj.priority;
          if (num4 != 0)
            return num4;
          int num5 = this.priorityMod - obj.priorityMod;
          if (num5 != 0)
            return num5;
          int num6 = this.consumerPriority - obj.consumerPriority;
          if (num6 != 0)
            return num6;
          int num7 = obj.cost - this.cost;
          if (num7 != 0)
            return num7;
          if (this.chore == null && obj.chore == null)
            return 0;
          if (this.chore == null)
            return -1;
          return obj.chore == null ? 1 : this.chore.id - obj.chore.id;
        }
        return !flag1 ? 1 : -1;
      }

      public override bool Equals(object obj) => this.CompareTo((Chore.Precondition.Context) obj) == 0;

      public bool Equals(Chore.Precondition.Context other) => this.CompareTo(other) == 0;

      public override int GetHashCode() => base.GetHashCode();

      public static bool operator ==(Chore.Precondition.Context x, Chore.Precondition.Context y) => x.CompareTo(y) == 0;

      public static bool operator !=(Chore.Precondition.Context x, Chore.Precondition.Context y) => x.CompareTo(y) != 0;

      public static bool ShouldFilter(string filter, string text)
      {
        if (string.IsNullOrEmpty(filter))
          return false;
        return string.IsNullOrEmpty(text) || text.ToLower().IndexOf(filter) < 0;
      }
    }
  }
}
