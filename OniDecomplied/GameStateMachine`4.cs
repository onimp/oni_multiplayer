// Decompiled with JetBrains decompiler
// Type: GameStateMachine`4
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType> : 
  StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>
  where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>
  where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameInstance
  where MasterType : IStateMachineTarget
{
  public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State root = new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State();
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<bool>.Callback IsFalse = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<bool>.Callback) ((smi, p) => !p);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<bool>.Callback IsTrue = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<bool>.Callback) ((smi, p) => p);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback IsZero = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback) ((smi, p) => (double) p == 0.0);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback IsLTZero = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback) ((smi, p) => (double) p < 0.0);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback IsLTEZero = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback) ((smi, p) => (double) p <= 0.0);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback IsGTZero = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback) ((smi, p) => (double) p > 0.0);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback IsGTEZero = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback) ((smi, p) => (double) p >= 0.0);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback IsOne = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback) ((smi, p) => (double) p == 1.0);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback IsLTOne = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback) ((smi, p) => (double) p < 1.0);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback IsLTEOne = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback) ((smi, p) => (double) p <= 1.0);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback IsGTOne = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback) ((smi, p) => (double) p > 1.0);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback IsGTEOne = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<float>.Callback) ((smi, p) => (double) p >= 1.0);
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<GameObject>.Callback IsNotNull = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Inequality((Object) p, (Object) null));
  protected static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<GameObject>.Callback IsNull = (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Equality((Object) p, (Object) null));

  public override void InitializeStates(out StateMachine.BaseState default_state) => base.InitializeStates(out default_state);

  public static StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback Not(
    StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback transition_cb)
  {
    return (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback) (smi => !transition_cb(smi));
  }

  public override void BindStates()
  {
    this.BindState((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) null, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) this.root, "root");
    this.BindStates((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) this.root, (object) this);
  }

  public class PreLoopPostState : 
    GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
  {
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State pre;
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State loop;
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State pst;
  }

  public class WorkingState : 
    GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
  {
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State waiting;
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State working_pre;
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State working_loop;
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State working_pst;
  }

  public class GameInstance : 
    StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GenericInstance
  {
    public void Queue(string anim, KAnim.PlayMode mode = 1) => this.smi.GetComponent<KBatchedAnimController>().Queue(HashedString.op_Implicit(anim), mode);

    public void Play(string anim, KAnim.PlayMode mode = 1) => this.smi.GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit(anim), mode);

    public GameInstance(MasterType master, DefType def)
      : base(master)
    {
      this.def = def;
    }

    public GameInstance(MasterType master)
      : base(master)
    {
    }
  }

  public class TagTransitionData : 
    StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition
  {
    private Tag[] tags;
    private bool onRemove;
    private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target;
    private bool is_executing;
    private Func<StateMachineInstanceType, Tag[]> tags_callback;

    public TagTransitionData(
      string name,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State source_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state,
      int idx,
      Tag[] tags,
      bool on_remove,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target,
      Func<StateMachineInstanceType, Tag[]> tags_callback = null)
      : base(name, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) source_state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) target_state, idx, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback) null)
    {
      this.tags = tags;
      this.onRemove = on_remove;
      this.target = target;
      this.tags_callback = tags_callback;
    }

    public override void Evaluate(StateMachine.Instance smi)
    {
      StateMachineInstanceType smi1 = smi as StateMachineInstanceType;
      Debug.Assert((object) smi1 != null);
      if (!this.onRemove)
      {
        if (!this.HasAllTags(smi1))
          return;
      }
      else if (this.HasAnyTags(smi1))
        return;
      this.ExecuteTransition(smi1);
    }

    private bool HasAllTags(StateMachineInstanceType smi) => this.target.Get(smi).GetComponent<KPrefabID>().HasAllTags(this.tags_callback != null ? this.tags_callback(smi) : this.tags);

    private bool HasAnyTags(StateMachineInstanceType smi) => this.target.Get(smi).GetComponent<KPrefabID>().HasAnyTags(this.tags_callback != null ? this.tags_callback(smi) : this.tags);

    private void ExecuteTransition(StateMachineInstanceType smi)
    {
      if (this.is_executing)
        return;
      this.is_executing = true;
      smi.GoTo(this.targetState);
      this.is_executing = false;
    }

    private void OnCallback(StateMachineInstanceType smi)
    {
      if (Object.op_Equality((Object) this.target.Get(smi), (Object) null))
        return;
      if (!this.onRemove)
      {
        if (!this.HasAllTags(smi))
          return;
      }
      else if (this.HasAnyTags(smi))
        return;
      this.ExecuteTransition(smi);
    }

    public override StateMachine.BaseTransition.Context Register(StateMachine.Instance smi)
    {
      StateMachineInstanceType smi_internal = smi as StateMachineInstanceType;
      Debug.Assert((object) smi_internal != null);
      return base.Register((StateMachine.Instance) smi_internal) with
      {
        handlerId = KMonoBehaviourExtensions.Subscribe(this.target.Get(smi_internal), -1582839653, (System.Action<object>) (data => this.OnCallback(smi_internal)))
      };
    }

    public override void Unregister(
      StateMachine.Instance smi,
      StateMachine.BaseTransition.Context context)
    {
      StateMachineInstanceType smi1 = smi as StateMachineInstanceType;
      Debug.Assert((object) smi1 != null);
      base.Unregister((StateMachine.Instance) smi1, context);
      if (!Object.op_Inequality((Object) this.target.Get(smi1), (Object) null))
        return;
      KMonoBehaviourExtensions.Unsubscribe(this.target.Get(smi1), context.handlerId);
    }
  }

  public class EventTransitionData : 
    StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition
  {
    private GameHashes evtId;
    private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target;
    private Func<StateMachineInstanceType, KMonoBehaviour> globalEventSystemCallback;

    public EventTransitionData(
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State source_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state,
      int idx,
      GameHashes evt,
      Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target)
      : base(evt.ToString(), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) source_state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) target_state, idx, condition)
    {
      this.evtId = evt;
      this.target = target;
      this.globalEventSystemCallback = global_event_system_callback;
    }

    public override void Evaluate(StateMachine.Instance smi)
    {
      StateMachineInstanceType smi1 = smi as StateMachineInstanceType;
      Debug.Assert((object) smi1 != null);
      if (this.condition == null || !this.condition(smi1))
        return;
      this.ExecuteTransition(smi1);
    }

    private void ExecuteTransition(StateMachineInstanceType smi) => smi.GoTo(this.targetState);

    private void OnCallback(StateMachineInstanceType smi)
    {
      if (this.condition != null && !this.condition(smi))
        return;
      this.ExecuteTransition(smi);
    }

    public override StateMachine.BaseTransition.Context Register(StateMachine.Instance smi)
    {
      StateMachineInstanceType smi_internal = smi as StateMachineInstanceType;
      Debug.Assert((object) smi_internal != null);
      StateMachine.BaseTransition.Context context = base.Register((StateMachine.Instance) smi_internal);
      System.Action<object> action = (System.Action<object>) (d => this.OnCallback(smi_internal));
      GameObject gameObject;
      if (this.globalEventSystemCallback != null)
      {
        gameObject = ((Component) this.globalEventSystemCallback(smi_internal)).gameObject;
      }
      else
      {
        gameObject = this.target.Get(smi_internal);
        if (Object.op_Equality((Object) gameObject, (Object) null))
          throw new InvalidOperationException("TargetParameter: " + this.target.name + " is null");
      }
      context.handlerId = KMonoBehaviourExtensions.Subscribe(gameObject, (int) this.evtId, action);
      return context;
    }

    public override void Unregister(
      StateMachine.Instance smi,
      StateMachine.BaseTransition.Context context)
    {
      StateMachineInstanceType smi1 = smi as StateMachineInstanceType;
      Debug.Assert((object) smi1 != null);
      base.Unregister((StateMachine.Instance) smi1, context);
      GameObject gameObject = (GameObject) null;
      if (this.globalEventSystemCallback != null)
      {
        KMonoBehaviour kmonoBehaviour = this.globalEventSystemCallback(smi1);
        if (Object.op_Inequality((Object) kmonoBehaviour, (Object) null))
          gameObject = ((Component) kmonoBehaviour).gameObject;
      }
      else
        gameObject = this.target.Get(smi1);
      if (!Object.op_Inequality((Object) gameObject, (Object) null))
        return;
      KMonoBehaviourExtensions.Unsubscribe(gameObject, context.handlerId);
    }
  }

  public new class State : 
    StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
  {
    [StateMachine.DoNotAutoCreate]
    private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter stateTarget;

    private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter GetStateTarget()
    {
      if (this.stateTarget != null)
        return this.stateTarget;
      return this.parent != null ? ((GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) this.parent).GetStateTarget() : this.sm.stateTarget ?? this.sm.masterTarget;
    }

    public int CreateDataTableEntry() => ((__Boxed<StateMachineType>) (object) this.sm).dataTableSize++;

    public int CreateUpdateTableEntry() => ((__Boxed<StateMachineType>) (object) this.sm).updateTableSize++;

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State root => this;

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoNothing() => this;

    private static List<StateMachine.Action> AddAction(
      string name,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback,
      List<StateMachine.Action> actions,
      bool add_to_end)
    {
      if (actions == null)
        actions = new List<StateMachine.Action>();
      StateMachine.Action action = new StateMachine.Action(name, (object) callback);
      if (add_to_end)
        actions.Add(action);
      else
        actions.Insert(0, action);
      return actions;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State master
    {
      get
      {
        this.stateTarget = this.sm.masterTarget;
        return this;
      }
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Target(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target)
    {
      this.stateTarget = target;
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Update(
      System.Action<StateMachineInstanceType, float> callback,
      UpdateRate update_rate = 5,
      bool load_balance = false)
    {
      return this.Update(this.sm.name + "." + this.name, callback, update_rate, load_balance);
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State BatchUpdate(
      UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update,
      UpdateRate update_rate = 5)
    {
      return this.BatchUpdate(this.sm.name + "." + this.name, batch_update, update_rate);
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Enter(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
    {
      return this.Enter(nameof (Enter), callback);
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Exit(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
    {
      return this.Exit(nameof (Exit), callback);
    }

    private GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InternalUpdate(
      string name,
      UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater bucket_updater,
      UpdateRate update_rate,
      bool load_balance,
      UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update = null)
    {
      int updateTableEntry = this.CreateUpdateTableEntry();
      if (this.updateActions == null)
        this.updateActions = new List<StateMachine.UpdateAction>();
      StateMachine.UpdateAction updateAction = new StateMachine.UpdateAction();
      updateAction.updateTableIdx = updateTableEntry;
      updateAction.updateRate = update_rate;
      updateAction.updater = (object) bucket_updater;
      int length = 1;
      if (load_balance)
        length = Singleton<StateMachineUpdater>.Instance.GetFrameCount(update_rate);
      updateAction.buckets = new StateMachineUpdater.BaseUpdateBucket[length];
      for (int index = 0; index < length; ++index)
      {
        UpdateBucketWithUpdater<StateMachineInstanceType> bucketWithUpdater = new UpdateBucketWithUpdater<StateMachineInstanceType>(name);
        bucketWithUpdater.batch_update_delegate = batch_update;
        Singleton<StateMachineUpdater>.Instance.AddBucket(update_rate, (StateMachineUpdater.BaseUpdateBucket) bucketWithUpdater);
        updateAction.buckets[index] = (StateMachineUpdater.BaseUpdateBucket) bucketWithUpdater;
      }
      this.updateActions.Add(updateAction);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State UpdateTransition(
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State destination_state,
      Func<StateMachineInstanceType, float, bool> callback,
      UpdateRate update_rate = 5,
      bool load_balance = false)
    {
      System.Action<StateMachineInstanceType, float> checkCallback = (System.Action<StateMachineInstanceType, float>) ((smi, dt) =>
      {
        if (!callback(smi, dt))
          return;
        smi.GoTo((StateMachine.BaseState) destination_state);
      });
      this.Enter((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => checkCallback(smi, 0.0f)));
      this.Update(checkCallback, update_rate, load_balance);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Update(
      string name,
      System.Action<StateMachineInstanceType, float> callback,
      UpdateRate update_rate = 5,
      bool load_balance = false)
    {
      return this.InternalUpdate(name, (UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater) new BucketUpdater<StateMachineInstanceType>(callback), update_rate, load_balance);
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State BatchUpdate(
      string name,
      UpdateBucketWithUpdater<StateMachineInstanceType>.BatchUpdateDelegate batch_update,
      UpdateRate update_rate = 5)
    {
      return this.InternalUpdate(name, (UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater) null, update_rate, false, batch_update);
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State FastUpdate(
      string name,
      UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater updater,
      UpdateRate update_rate = 5,
      bool load_balance = false)
    {
      return this.InternalUpdate(name, updater, update_rate, load_balance);
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Enter(
      string name,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
    {
      this.enterActions = GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.AddAction(name, callback, this.enterActions, true);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Exit(
      string name,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
    {
      this.exitActions = GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.AddAction(name, callback, this.exitActions, false);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Toggle(
      string name,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback enter_callback,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback exit_callback)
    {
      int data_idx = this.CreateDataTableEntry();
      this.Enter("ToggleEnter(" + name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        smi.dataTable[data_idx] = GameStateMachineHelper.HasToggleEnteredFlag;
        enter_callback(smi);
      }));
      this.Exit("ToggleExit(" + name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (smi.dataTable[data_idx] == null)
          return;
        smi.dataTable[data_idx] = (object) null;
        exit_callback(smi);
      }));
      return this;
    }

    private void Break(StateMachineInstanceType smi)
    {
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State BreakOnEnter() => this.Enter((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.Break(smi)));

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State BreakOnExit() => this.Exit((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.Break(smi)));

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State AddEffect(
      string effect_name)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddEffect(" + effect_name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Effects>(smi).Add(effect_name, true)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleAnims(
      Func<StateMachineInstanceType, KAnimFile> chooser_callback)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("EnableAnims()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KAnimFile kanim_file = chooser_callback(smi);
        if (Object.op_Equality((Object) kanim_file, (Object) null))
          return;
        state_target.Get<KAnimControllerBase>(smi).AddAnimOverrides(kanim_file);
      }));
      this.Exit("Disableanims()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KAnimFile kanim_file = chooser_callback(smi);
        if (Object.op_Equality((Object) kanim_file, (Object) null))
          return;
        state_target.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(kanim_file);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleAnims(
      Func<StateMachineInstanceType, HashedString> chooser_callback)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("EnableAnims()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        HashedString name = chooser_callback(smi);
        if (HashedString.op_Equality(name, HashedString.op_Implicit((string) null)) || !((HashedString) ref name).IsValid)
          return;
        KAnimFile anim = Assets.GetAnim(name);
        if (Object.op_Equality((Object) anim, (Object) null))
          Debug.LogWarning((object) ("Missing anims: " + name.ToString()));
        else
          state_target.Get<KAnimControllerBase>(smi).AddAnimOverrides(anim);
      }));
      this.Exit("Disableanims()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        HashedString name = chooser_callback(smi);
        if (HashedString.op_Equality(name, HashedString.op_Implicit((string) null)) || !((HashedString) ref name).IsValid)
          return;
        KAnimFile anim = Assets.GetAnim(name);
        if (!Object.op_Inequality((Object) anim, (Object) null))
          return;
        state_target.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(anim);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleAnims(
      string anim_file,
      float priority = 0.0f,
      string RequiredDlc = "")
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Toggle("ToggleAnims(" + anim_file + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (!DlcManager.IsContentActive(RequiredDlc))
          return;
        KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
        if (Object.op_Equality((Object) anim, (Object) null))
          Debug.LogError((object) ("Trying to add missing override anims:" + anim_file));
        state_target.Get<KAnimControllerBase>(smi).AddAnimOverrides(anim, priority);
      }), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (!DlcManager.IsContentActive(RequiredDlc))
          return;
        KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
        state_target.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(anim);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleAttributeModifier(
      string modifier_name,
      Func<StateMachineInstanceType, AttributeModifier> callback,
      Func<StateMachineInstanceType, bool> condition = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      int data_idx = this.CreateDataTableEntry();
      this.Enter("AddAttributeModifier( " + modifier_name + " )", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (condition != null && !condition(smi))
          return;
        AttributeModifier modifier = callback(smi);
        DebugUtil.Assert(smi.dataTable[data_idx] == null);
        smi.dataTable[data_idx] = (object) modifier;
        state_target.Get(smi).GetAttributes().Add(modifier);
      }));
      this.Exit("RemoveAttributeModifier( " + modifier_name + " )", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (smi.dataTable[data_idx] == null)
          return;
        AttributeModifier modifier = (AttributeModifier) smi.dataTable[data_idx];
        smi.dataTable[data_idx] = (object) null;
        GameObject go = state_target.Get(smi);
        if (!Object.op_Inequality((Object) go, (Object) null))
          return;
        go.GetAttributes().Remove(modifier);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleLoopingSound(
      string event_name,
      Func<StateMachineInstanceType, bool> condition = null,
      bool pause_on_game_pause = true,
      bool enable_culling = true,
      bool enable_camera_scaled_position = true)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("StartLoopingSound( " + event_name + " )", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (condition != null && !condition(smi))
          return;
        state_target.Get(smi).GetComponent<LoopingSounds>().StartSound(event_name, pause_on_game_pause, enable_culling, enable_camera_scaled_position);
      }));
      this.Exit("StopLoopingSound( " + event_name + " )", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get(smi).GetComponent<LoopingSounds>().StopSound(event_name)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleLoopingSound(
      string state_label,
      Func<StateMachineInstanceType, string> event_name_callback,
      Func<StateMachineInstanceType, bool> condition = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      int data_idx = this.CreateDataTableEntry();
      this.Enter("StartLoopingSound( " + state_label + " )", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (condition != null && !condition(smi))
          return;
        string asset = event_name_callback(smi);
        smi.dataTable[data_idx] = (object) asset;
        state_target.Get(smi).GetComponent<LoopingSounds>().StartSound(asset);
      }));
      this.Exit("StopLoopingSound( " + state_label + " )", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (smi.dataTable[data_idx] == null)
          return;
        state_target.Get(smi).GetComponent<LoopingSounds>().StopSound((string) smi.dataTable[data_idx]);
        smi.dataTable[data_idx] = (object) null;
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State RefreshUserMenuOnEnter()
    {
      this.Enter("RefreshUserMenuOnEnter()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => Game.Instance.userMenu.Refresh(smi.master.gameObject)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State WorkableStartTransition(
      Func<StateMachineInstanceType, Workable> get_workable_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state)
    {
      int data_idx = this.CreateDataTableEntry();
      this.Enter("Enter WorkableStartTransition(" + target_state.longName + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Workable workable1 = get_workable_callback(smi);
        if (!Object.op_Inequality((Object) workable1, (Object) null))
          return;
        System.Action<Workable, Workable.WorkableEvent> action = (System.Action<Workable, Workable.WorkableEvent>) ((workable, evt) =>
        {
          if (evt != Workable.WorkableEvent.WorkStarted)
            return;
          smi.GoTo((StateMachine.BaseState) target_state);
        });
        smi.dataTable[data_idx] = (object) action;
        workable1.OnWorkableEventCB += action;
      }));
      this.Exit("Exit WorkableStartTransition(" + target_state.longName + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Workable workable = get_workable_callback(smi);
        if (!Object.op_Inequality((Object) workable, (Object) null))
          return;
        System.Action<Workable, Workable.WorkableEvent> action = (System.Action<Workable, Workable.WorkableEvent>) smi.dataTable[data_idx];
        smi.dataTable[data_idx] = (object) null;
        workable.OnWorkableEventCB -= action;
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State WorkableStopTransition(
      Func<StateMachineInstanceType, Workable> get_workable_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state)
    {
      int data_idx = this.CreateDataTableEntry();
      this.Enter("Enter WorkableStopTransition(" + target_state.longName + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Workable workable1 = get_workable_callback(smi);
        if (!Object.op_Inequality((Object) workable1, (Object) null))
          return;
        System.Action<Workable, Workable.WorkableEvent> action = (System.Action<Workable, Workable.WorkableEvent>) ((workable, evt) =>
        {
          if (evt != Workable.WorkableEvent.WorkStopped)
            return;
          smi.GoTo((StateMachine.BaseState) target_state);
        });
        smi.dataTable[data_idx] = (object) action;
        workable1.OnWorkableEventCB += action;
      }));
      this.Exit("Exit WorkableStopTransition(" + target_state.longName + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Workable workable = get_workable_callback(smi);
        if (!Object.op_Inequality((Object) workable, (Object) null))
          return;
        System.Action<Workable, Workable.WorkableEvent> action = (System.Action<Workable, Workable.WorkableEvent>) smi.dataTable[data_idx];
        smi.dataTable[data_idx] = (object) null;
        workable.OnWorkableEventCB -= action;
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State WorkableCompleteTransition(
      Func<StateMachineInstanceType, Workable> get_workable_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state)
    {
      int data_idx = this.CreateDataTableEntry();
      this.Enter("Enter WorkableCompleteTransition(" + target_state.longName + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Workable workable1 = get_workable_callback(smi);
        if (!Object.op_Inequality((Object) workable1, (Object) null))
          return;
        System.Action<Workable, Workable.WorkableEvent> action = (System.Action<Workable, Workable.WorkableEvent>) ((workable, evt) =>
        {
          if (evt != Workable.WorkableEvent.WorkCompleted)
            return;
          smi.GoTo((StateMachine.BaseState) target_state);
        });
        smi.dataTable[data_idx] = (object) action;
        workable1.OnWorkableEventCB += action;
      }));
      this.Exit("Exit WorkableCompleteTransition(" + target_state.longName + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Workable workable = get_workable_callback(smi);
        if (!Object.op_Inequality((Object) workable, (Object) null))
          return;
        System.Action<Workable, Workable.WorkableEvent> action = (System.Action<Workable, Workable.WorkableEvent>) smi.dataTable[data_idx];
        smi.dataTable[data_idx] = (object) null;
        workable.OnWorkableEventCB -= action;
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleGravity()
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      int data_idx = this.CreateDataTableEntry();
      this.Enter("AddComponent<Gravity>()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        GameObject go = state_target.Get(smi);
        smi.dataTable[data_idx] = (object) go;
        GameComps.Gravities.Add(go, Vector2.zero);
      }));
      this.Exit("RemoveComponent<Gravity>()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        GameObject gameObject = (GameObject) smi.dataTable[data_idx];
        smi.dataTable[data_idx] = (object) null;
        GameComps.Gravities.Remove(gameObject);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleGravity(
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State landed_state)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.EventTransition(GameHashes.Landed, landed_state);
      this.Toggle("GravityComponent", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => GameComps.Gravities.Add(state_target.Get(smi), Vector2.zero)), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => GameComps.Gravities.Remove(state_target.Get(smi))));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleThought(
      Func<StateMachineInstanceType, Thought> chooser_callback)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("EnableThought()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Thought thought = chooser_callback(smi);
        state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().AddThought(thought);
      }));
      this.Exit("DisableThought()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Thought thought = chooser_callback(smi);
        state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().RemoveThought(thought);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleThought(
      Thought thought,
      Func<StateMachineInstanceType, bool> condition_callback = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddThought(" + thought.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (condition_callback != null && !condition_callback(smi))
          return;
        state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().AddThought(thought);
      }));
      if (condition_callback != null)
        this.Update("ValidateThought(" + thought.Id + ")", (System.Action<StateMachineInstanceType, float>) ((smi, dt) =>
        {
          if (condition_callback(smi))
            state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().AddThought(thought);
          else
            state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().RemoveThought(thought);
        }));
      this.Exit("RemoveThought(" + thought.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get(smi).GetSMI<ThoughtGraph.Instance>().RemoveThought(thought)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleExpression(
      Func<StateMachineInstanceType, Expression> chooser_callback)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddExpression", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<FaceGraph>(smi).AddExpression(chooser_callback(smi))));
      this.Exit("RemoveExpression", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<FaceGraph>(smi).RemoveExpression(chooser_callback(smi))));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleExpression(
      Expression expression,
      Func<StateMachineInstanceType, bool> condition = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddExpression(" + expression.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (condition != null && !condition(smi))
          return;
        state_target.Get<FaceGraph>(smi).AddExpression(expression);
      }));
      if (condition != null)
        this.Update("ValidateExpression(" + expression.Id + ")", (System.Action<StateMachineInstanceType, float>) ((smi, dt) =>
        {
          if (condition(smi))
            state_target.Get<FaceGraph>(smi).AddExpression(expression);
          else
            state_target.Get<FaceGraph>(smi).RemoveExpression(expression);
        }));
      this.Exit("RemoveExpression(" + expression.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        FaceGraph faceGraph = state_target.Get<FaceGraph>(smi);
        if (!Object.op_Inequality((Object) faceGraph, (Object) null))
          return;
        faceGraph.RemoveExpression(expression);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleMainStatusItem(
      StatusItem status_item,
      Func<StateMachineInstanceType, object> callback = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddMainStatusItem(" + status_item.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        object data = callback != null ? callback(smi) : (object) smi;
        state_target.Get<KSelectable>(smi).SetStatusItem(Db.Get().StatusItemCategories.Main, status_item, data);
      }));
      this.Exit("RemoveMainStatusItem(" + status_item.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KSelectable kselectable = state_target.Get<KSelectable>(smi);
        if (!Object.op_Inequality((Object) kselectable, (Object) null))
          return;
        kselectable.SetStatusItem(Db.Get().StatusItemCategories.Main, (StatusItem) null);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleMainStatusItem(
      Func<StateMachineInstanceType, StatusItem> status_item_cb,
      Func<StateMachineInstanceType, object> callback = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddMainStatusItem(DynamicGeneration)", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        object data = callback != null ? callback(smi) : (object) smi;
        state_target.Get<KSelectable>(smi).SetStatusItem(Db.Get().StatusItemCategories.Main, status_item_cb(smi), data);
      }));
      this.Exit("RemoveMainStatusItem(DynamicGeneration)", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KSelectable kselectable = state_target.Get<KSelectable>(smi);
        if (!Object.op_Inequality((Object) kselectable, (Object) null))
          return;
        kselectable.SetStatusItem(Db.Get().StatusItemCategories.Main, (StatusItem) null);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleCategoryStatusItem(
      StatusItemCategory category,
      StatusItem status_item,
      object data = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddCategoryStatusItem(" + category.Id + ", " + status_item.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<KSelectable>(smi).SetStatusItem(category, status_item, data != null ? data : (object) smi)));
      this.Exit("RemoveCategoryStatusItem(" + category.Id + ", " + status_item.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KSelectable kselectable = state_target.Get<KSelectable>(smi);
        if (!Object.op_Inequality((Object) kselectable, (Object) null))
          return;
        kselectable.SetStatusItem(category, (StatusItem) null);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStatusItem(
      StatusItem status_item,
      object data = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      int data_idx = this.CreateDataTableEntry();
      this.Enter("AddStatusItem(" + status_item.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        object data1 = data ?? (object) smi;
        Guid guid = state_target.Get<KSelectable>(smi).AddStatusItem(status_item, data1);
        smi.dataTable[data_idx] = (object) guid;
      }));
      this.Exit("RemoveStatusItem(" + status_item.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KSelectable kselectable = state_target.Get<KSelectable>(smi);
        if (Object.op_Inequality((Object) kselectable, (Object) null) && smi.dataTable[data_idx] != null)
        {
          Guid guid = (Guid) smi.dataTable[data_idx];
          kselectable.RemoveStatusItem(guid);
        }
        smi.dataTable[data_idx] = (object) null;
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleSnapOn(
      string snap_on)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("SnapOn(" + snap_on + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<SnapOn>(smi).AttachSnapOnByName(snap_on)));
      this.Exit("SnapOff(" + snap_on + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        SnapOn snapOn = state_target.Get<SnapOn>(smi);
        if (!Object.op_Inequality((Object) snapOn, (Object) null))
          return;
        snapOn.DetachSnapOnByName(snap_on);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleTag(
      Tag tag)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddTag(" + ((Tag) ref tag).Name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<KPrefabID>(smi).AddTag(tag, false)));
      this.Exit("RemoveTag(" + ((Tag) ref tag).Name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<KPrefabID>(smi).RemoveTag(tag)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleTag(
      Func<StateMachineInstanceType, Tag> behaviour_tag_cb)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddTag(DynamicallyConstructed)", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<KPrefabID>(smi).AddTag(behaviour_tag_cb(smi), false)));
      this.Exit("RemoveTag(DynamicallyConstructed)", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<KPrefabID>(smi).RemoveTag(behaviour_tag_cb(smi))));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStatusItem(
      StatusItem status_item,
      Func<StateMachineInstanceType, object> callback)
    {
      return this.ToggleStatusItem(status_item, callback, (StatusItemCategory) null);
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStatusItem(
      StatusItem status_item,
      Func<StateMachineInstanceType, object> callback,
      StatusItemCategory category)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      int data_idx = this.CreateDataTableEntry();
      this.Enter("AddStatusItem(" + status_item.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (category == null)
        {
          object data = callback != null ? callback(smi) : (object) null;
          Guid guid = state_target.Get<KSelectable>(smi).AddStatusItem(status_item, data);
          smi.dataTable[data_idx] = (object) guid;
        }
        else
        {
          object data = callback != null ? callback(smi) : (object) null;
          Guid guid = state_target.Get<KSelectable>(smi).SetStatusItem(category, status_item, data);
          smi.dataTable[data_idx] = (object) guid;
        }
      }));
      this.Exit("RemoveStatusItem(" + status_item.Id + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KSelectable kselectable = state_target.Get<KSelectable>(smi);
        if (Object.op_Inequality((Object) kselectable, (Object) null) && smi.dataTable[data_idx] != null)
        {
          if (category == null)
          {
            Guid guid = (Guid) smi.dataTable[data_idx];
            kselectable.RemoveStatusItem(guid);
          }
          else
            kselectable.SetStatusItem(category, (StatusItem) null);
        }
        smi.dataTable[data_idx] = (object) null;
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStatusItem(
      Func<StateMachineInstanceType, StatusItem> status_item_cb,
      Func<StateMachineInstanceType, object> data_callback = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      int data_idx = this.CreateDataTableEntry();
      this.Enter("AddStatusItem(DynamicallyConstructed)", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        StatusItem status_item = status_item_cb(smi);
        if (status_item == null)
          return;
        object data = data_callback != null ? data_callback(smi) : (object) null;
        Guid guid = state_target.Get<KSelectable>(smi).AddStatusItem(status_item, data);
        smi.dataTable[data_idx] = (object) guid;
      }));
      this.Exit("RemoveStatusItem(DynamicallyConstructed)", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KSelectable kselectable = state_target.Get<KSelectable>(smi);
        if (Object.op_Inequality((Object) kselectable, (Object) null) && smi.dataTable[data_idx] != null)
        {
          Guid guid = (Guid) smi.dataTable[data_idx];
          kselectable.RemoveStatusItem(guid);
        }
        smi.dataTable[data_idx] = (object) null;
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleFX(
      Func<StateMachineInstanceType, StateMachine.Instance> callback)
    {
      int data_idx = this.CreateDataTableEntry();
      this.Enter("EnableFX()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        StateMachine.Instance instance = callback(smi);
        if (instance == null)
          return;
        instance.StartSM();
        smi.dataTable[data_idx] = (object) instance;
      }));
      this.Exit("DisableFX()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        StateMachine.Instance instance = (StateMachine.Instance) smi.dataTable[data_idx];
        smi.dataTable[data_idx] = (object) null;
        instance?.StopSM("ToggleFX.Exit");
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State BehaviourComplete(
      Func<StateMachineInstanceType, Tag> tag_cb,
      bool on_exit = false)
    {
      if (on_exit)
        this.Exit("BehaviourComplete()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
        {
          smi.Trigger(-739654666, (object) tag_cb(smi));
          smi.GoTo((StateMachine.BaseState) null);
        }));
      else
        this.Enter("BehaviourComplete()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
        {
          smi.Trigger(-739654666, (object) tag_cb(smi));
          smi.GoTo((StateMachine.BaseState) null);
        }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State BehaviourComplete(
      Tag tag,
      bool on_exit = false)
    {
      if (on_exit)
        this.Exit("BehaviourComplete(" + tag.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
        {
          smi.Trigger(-739654666, (object) tag);
          smi.GoTo((StateMachine.BaseState) null);
        }));
      else
        this.Enter("BehaviourComplete(" + tag.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
        {
          smi.Trigger(-739654666, (object) tag);
          smi.GoTo((StateMachine.BaseState) null);
        }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleBehaviour(
      Tag behaviour_tag,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback precondition,
      System.Action<StateMachineInstanceType> on_complete = null)
    {
      Func<object, bool> precondition_cb = (Func<object, bool>) (obj => precondition(obj as StateMachineInstanceType));
      this.Enter("AddPrecondition", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) smi.GetComponent<ChoreConsumer>(), (Object) null))
          return;
        smi.GetComponent<ChoreConsumer>().AddBehaviourPrecondition(behaviour_tag, precondition_cb, (object) smi);
      }));
      this.Exit("RemovePrecondition", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) smi.GetComponent<ChoreConsumer>(), (Object) null))
          return;
        smi.GetComponent<ChoreConsumer>().RemoveBehaviourPrecondition(behaviour_tag, precondition_cb, (object) smi);
      }));
      this.ToggleTag(behaviour_tag);
      if (on_complete != null)
        this.EventHandler(GameHashes.BehaviourTagComplete, (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback) ((smi, data) =>
        {
          if (!Tag.op_Equality((Tag) data, behaviour_tag))
            return;
          on_complete(smi);
        }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleBehaviour(
      Func<StateMachineInstanceType, Tag> behaviour_tag_cb,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback precondition,
      System.Action<StateMachineInstanceType> on_complete = null)
    {
      Func<object, bool> precondition_cb = (Func<object, bool>) (obj => precondition(obj as StateMachineInstanceType));
      this.Enter("AddPrecondition", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) smi.GetComponent<ChoreConsumer>(), (Object) null))
          return;
        smi.GetComponent<ChoreConsumer>().AddBehaviourPrecondition(behaviour_tag_cb(smi), precondition_cb, (object) smi);
      }));
      this.Exit("RemovePrecondition", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) smi.GetComponent<ChoreConsumer>(), (Object) null))
          return;
        smi.GetComponent<ChoreConsumer>().RemoveBehaviourPrecondition(behaviour_tag_cb(smi), precondition_cb, (object) smi);
      }));
      this.ToggleTag(behaviour_tag_cb);
      if (on_complete != null)
        this.EventHandler(GameHashes.BehaviourTagComplete, (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback) ((smi, data) =>
        {
          if (!Tag.op_Equality((Tag) data, behaviour_tag_cb(smi)))
            return;
          on_complete(smi);
        }));
      return this;
    }

    public void ClearFetch(StateMachineInstanceType smi, int fetch_data_idx, int callback_data_idx)
    {
      FetchList2 fetchList2 = (FetchList2) smi.dataTable[fetch_data_idx];
      if (fetchList2 == null)
        return;
      smi.dataTable[fetch_data_idx] = (object) null;
      smi.dataTable[callback_data_idx] = (object) null;
      fetchList2.Cancel("ClearFetchListFromSM");
    }

    public void SetupFetch(
      Func<StateMachineInstanceType, FetchList2> create_fetchlist_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state,
      StateMachineInstanceType smi,
      int fetch_data_idx,
      int callback_data_idx)
    {
      FetchList2 fetchList2 = create_fetchlist_callback(smi);
      System.Action on_complete = (System.Action) (() =>
      {
        this.ClearFetch(smi, fetch_data_idx, callback_data_idx);
        smi.GoTo((StateMachine.BaseState) target_state);
      });
      fetchList2.Submit(on_complete, true);
      smi.dataTable[fetch_data_idx] = (object) fetchList2;
      smi.dataTable[callback_data_idx] = (object) on_complete;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleFetch(
      Func<StateMachineInstanceType, FetchList2> create_fetchlist_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state)
    {
      int data_idx = this.CreateDataTableEntry();
      int callback_data_idx = this.CreateDataTableEntry();
      this.Enter("ToggleFetchEnter()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.SetupFetch(create_fetchlist_callback, target_state, smi, data_idx, callback_data_idx)));
      this.Exit("ToggleFetchExit()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.ClearFetch(smi, data_idx, callback_data_idx)));
      return this;
    }

    private void ClearChore(
      StateMachineInstanceType smi,
      int chore_data_idx,
      int callback_data_idx)
    {
      Chore chore = (Chore) smi.dataTable[chore_data_idx];
      if (chore == null)
        return;
      System.Action<Chore> action = (System.Action<Chore>) smi.dataTable[callback_data_idx];
      smi.dataTable[chore_data_idx] = (object) null;
      smi.dataTable[callback_data_idx] = (object) null;
      chore.onExit -= action;
      chore.Cancel("ClearGlobalChore");
    }

    private Chore SetupChore(
      Func<StateMachineInstanceType, Chore> create_chore_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state,
      StateMachineInstanceType smi,
      int chore_data_idx,
      int callback_data_idx,
      bool is_success_state_reentrant,
      bool is_failure_state_reentrant)
    {
      Chore chore = create_chore_callback(smi);
      DebugUtil.DevAssert(!chore.IsPreemptable, "ToggleChore can't be used with preemptable chores! :( (but it should...)", (Object) null);
      chore.runUntilComplete = false;
      System.Action<Chore> action = (System.Action<Chore>) (chore_param =>
      {
        bool isComplete = chore.isComplete;
        if ((isComplete & is_success_state_reentrant ? 1 : (!is_failure_state_reentrant ? 0 : (!isComplete ? 1 : 0))) != 0)
        {
          this.SetupChore(create_chore_callback, success_state, failure_state, smi, chore_data_idx, callback_data_idx, is_success_state_reentrant, is_failure_state_reentrant);
        }
        else
        {
          GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state = success_state;
          if (!isComplete)
            state = failure_state;
          this.ClearChore(smi, chore_data_idx, callback_data_idx);
          smi.GoTo((StateMachine.BaseState) state);
        }
      });
      chore.onExit += action;
      smi.dataTable[chore_data_idx] = (object) chore;
      smi.dataTable[callback_data_idx] = (object) action;
      return chore;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleRecurringChore(
      Func<StateMachineInstanceType, Chore> callback,
      Func<StateMachineInstanceType, bool> condition = null)
    {
      int data_idx = this.CreateDataTableEntry();
      int callback_data_idx = this.CreateDataTableEntry();
      this.Enter("ToggleRecurringChoreEnter()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (condition != null && !condition(smi))
          return;
        this.SetupChore(callback, this, this, smi, data_idx, callback_data_idx, true, true);
      }));
      this.Exit("ToggleRecurringChoreExit()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.ClearChore(smi, data_idx, callback_data_idx)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleChore(
      Func<StateMachineInstanceType, Chore> callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state)
    {
      int data_idx = this.CreateDataTableEntry();
      int callback_data_idx = this.CreateDataTableEntry();
      this.Enter("ToggleChoreEnter()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.SetupChore(callback, target_state, target_state, smi, data_idx, callback_data_idx, false, false)));
      this.Exit("ToggleChoreExit()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.ClearChore(smi, data_idx, callback_data_idx)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleChore(
      Func<StateMachineInstanceType, Chore> callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
    {
      int data_idx = this.CreateDataTableEntry();
      int callback_data_idx = this.CreateDataTableEntry();
      bool is_success_state_reentrant = success_state == this;
      bool is_failure_state_reentrant = failure_state == this;
      this.Enter("ToggleChoreEnter()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.SetupChore(callback, success_state, failure_state, smi, data_idx, callback_data_idx, is_success_state_reentrant, is_failure_state_reentrant)));
      this.Exit("ToggleChoreExit()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.ClearChore(smi, data_idx, callback_data_idx)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleReactable(
      Func<StateMachineInstanceType, Reactable> callback)
    {
      int data_idx = this.CreateDataTableEntry();
      this.Enter((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => smi.dataTable[data_idx] = (object) callback(smi)));
      this.Exit((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Reactable reactable = (Reactable) smi.dataTable[data_idx];
        smi.dataTable[data_idx] = (object) null;
        reactable?.Cleanup();
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State RemoveEffect(
      string effect_name)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("RemoveEffect(" + effect_name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Effects>(smi).Remove(effect_name)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleEffect(
      string effect_name)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddEffect(" + effect_name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Effects>(smi).Add(effect_name, false)));
      this.Exit("RemoveEffect(" + effect_name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Effects>(smi).Remove(effect_name)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleEffect(
      Func<StateMachineInstanceType, Effect> callback)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddEffect()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Effects>(smi).Add(callback(smi), false)));
      this.Exit("RemoveEffect()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Effects>(smi).Remove(callback(smi))));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleEffect(
      Func<StateMachineInstanceType, string> callback)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddEffect()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Effects>(smi).Add(callback(smi), false)));
      this.Exit("RemoveEffect()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Effects>(smi).Remove(callback(smi))));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State LogOnExit(
      Func<StateMachineInstanceType, string> callback)
    {
      this.Enter("Log()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => { }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State LogOnEnter(
      Func<StateMachineInstanceType, string> callback)
    {
      this.Exit("Log()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => { }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleUrge(
      Urge urge)
    {
      return this.ToggleUrge((Func<StateMachineInstanceType, Urge>) (smi => urge));
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleUrge(
      Func<StateMachineInstanceType, Urge> urge_callback)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("AddUrge()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Urge urge = urge_callback(smi);
        state_target.Get<ChoreConsumer>(smi).AddUrge(urge);
      }));
      this.Exit("RemoveUrge()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Urge urge = urge_callback(smi);
        ChoreConsumer choreConsumer = state_target.Get<ChoreConsumer>(smi);
        if (!Object.op_Inequality((Object) choreConsumer, (Object) null))
          return;
        choreConsumer.RemoveUrge(urge);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State OnTargetLost(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter parameter,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State target_state)
    {
      this.ParamTransition<GameObject>((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<GameObject>) parameter, target_state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Equality((Object) p, (Object) null)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleBrain(
      string reason)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("StopBrain(" + reason + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Brain>(smi).Stop(reason)));
      this.Exit("ResetBrain(" + reason + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Brain>(smi).Reset(reason)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State TriggerOnEnter(
      GameHashes evt,
      Func<StateMachineInstanceType, object> callback = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("Trigger(" + evt.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        GameObject gameObject = state_target.Get(smi);
        object obj1 = callback != null ? callback(smi) : (object) null;
        int num = (int) evt;
        object obj2 = obj1;
        EventExtensions.Trigger(gameObject, num, obj2);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State TriggerOnExit(
      GameHashes evt,
      Func<StateMachineInstanceType, object> callback = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Exit("Trigger(" + evt.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        GameObject gameObject = state_target.Get(smi);
        if (!Object.op_Inequality((Object) gameObject, (Object) null))
          return;
        object obj = callback != null ? callback(smi) : (object) null;
        EventExtensions.Trigger(gameObject, (int) evt, obj);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStateMachine(
      Func<StateMachineInstanceType, StateMachine.Instance> callback)
    {
      int data_idx = this.CreateDataTableEntry();
      this.Enter("EnableStateMachine()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        StateMachine.Instance instance = callback(smi);
        smi.dataTable[data_idx] = (object) instance;
        instance.StartSM();
      }));
      this.Exit("DisableStateMachine()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        StateMachine.Instance instance = (StateMachine.Instance) smi.dataTable[data_idx];
        smi.dataTable[data_idx] = (object) null;
        instance?.StopSM("ToggleStateMachine.Exit");
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleComponent<ComponentType>(
      bool disable = false)
      where ComponentType : MonoBehaviour
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("EnableComponent(" + typeof (ComponentType).Name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => ((Behaviour) (object) state_target.Get<ComponentType>(smi)).enabled = !disable));
      this.Exit("DisableComponent(" + typeof (ComponentType).Name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => ((Behaviour) (object) state_target.Get<ComponentType>(smi)).enabled = disable));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeOperationalFlag(
      Operational.Flag flag,
      bool init_val = false)
    {
      this.Enter("InitOperationalFlag (" + flag.Name + ", " + init_val.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => smi.GetComponent<Operational>().SetFlag(flag, init_val)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleOperationalFlag(
      Operational.Flag flag)
    {
      this.Enter("ToggleOperationalFlag True (" + flag.Name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => smi.GetComponent<Operational>().SetFlag(flag, true)));
      this.Exit("ToggleOperationalFlag False (" + flag.Name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => smi.GetComponent<Operational>().SetFlag(flag, false)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleReserve(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter reserver,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter pickup_target,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter requested_amount,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter actual_amount)
    {
      int data_idx = this.CreateDataTableEntry();
      this.Enter("Reserve(" + pickup_target.name + ", " + requested_amount.name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Pickupable pickupable = pickup_target.Get<Pickupable>(smi);
        GameObject gameObject = reserver.Get(smi);
        float val1 = requested_amount.Get(smi);
        float val2 = Mathf.Max(1f, Db.Get().Attributes.CarryAmount.Lookup(gameObject).GetTotalValue());
        float amount = Math.Min(Math.Min(val1, val2), pickupable.UnreservedAmount);
        if ((double) amount <= 0.0)
        {
          pickupable.PrintReservations();
          Debug.LogError((object) (val2.ToString() + ", " + val1.ToString() + ", " + pickupable.UnreservedAmount.ToString() + ", " + amount.ToString()));
        }
        double num1 = (double) actual_amount.Set(amount, smi);
        int num2 = pickupable.Reserve(nameof (ToggleReserve), gameObject, amount);
        smi.dataTable[data_idx] = (object) num2;
      }));
      this.Exit("Unreserve(" + pickup_target.name + ", " + requested_amount.name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        int ticket = (int) smi.dataTable[data_idx];
        smi.dataTable[data_idx] = (object) null;
        Pickupable pickupable = pickup_target.Get<Pickupable>(smi);
        if (!Object.op_Inequality((Object) pickupable, (Object) null))
          return;
        pickupable.Unreserve(nameof (ToggleReserve), ticket);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleWork(
      string work_type,
      System.Action<StateMachineInstanceType> callback,
      Func<StateMachineInstanceType, bool> validate_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("StartWork(" + work_type + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (validate_callback(smi))
          callback(smi);
        else
          smi.GoTo((StateMachine.BaseState) failure_state);
      }));
      this.Update("Work(" + work_type + ")", (System.Action<StateMachineInstanceType, float>) ((smi, dt) =>
      {
        if (validate_callback(smi))
        {
          switch (state_target.Get<Worker>(smi).Work(dt))
          {
            case Worker.WorkResult.Success:
              smi.GoTo((StateMachine.BaseState) success_state);
              break;
            case Worker.WorkResult.Failed:
              smi.GoTo((StateMachine.BaseState) failure_state);
              break;
          }
        }
        else
          smi.GoTo((StateMachine.BaseState) failure_state);
      }), (UpdateRate) 4);
      this.Exit("StopWork()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Worker>(smi).StopWork()));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleWork<WorkableType>(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter source_target,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state,
      Func<StateMachineInstanceType, bool> is_valid_cb)
      where WorkableType : Workable
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.ToggleWork(typeof (WorkableType).Name, (System.Action<StateMachineInstanceType>) (smi =>
      {
        Workable workable = (Workable) source_target.Get<WorkableType>(smi);
        state_target.Get<Worker>(smi).StartWork(new Worker.StartWorkInfo(workable));
      }), (Func<StateMachineInstanceType, bool>) (smi =>
      {
        if (!Object.op_Inequality((Object) (object) source_target.Get<WorkableType>(smi), (Object) null))
          return false;
        return is_valid_cb == null || is_valid_cb(smi);
      }), success_state, failure_state);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoEat(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter source_target,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter amount,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.ToggleWork("Eat", (System.Action<StateMachineInstanceType>) (smi =>
      {
        Edible edible = source_target.Get<Edible>(smi);
        Worker worker = state_target.Get<Worker>(smi);
        float amount1 = amount.Get(smi);
        Edible.EdibleStartWorkInfo start_work_info = new Edible.EdibleStartWorkInfo((Workable) edible, amount1);
        worker.StartWork((Worker.StartWorkInfo) start_work_info);
      }), (Func<StateMachineInstanceType, bool>) (smi => Object.op_Inequality((Object) source_target.Get<Edible>(smi), (Object) null)), success_state, failure_state);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoSleep(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter sleeper,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter bed,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.ToggleWork("Sleep", (System.Action<StateMachineInstanceType>) (smi => state_target.Get<Worker>(smi).StartWork(new Worker.StartWorkInfo((Workable) bed.Get<Sleepable>(smi)))), (Func<StateMachineInstanceType, bool>) (smi => Object.op_Inequality((Object) bed.Get<Sleepable>(smi), (Object) null)), success_state, failure_state);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoDelivery(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter worker_param,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter storage_param,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
    {
      this.ToggleWork("Pickup", (System.Action<StateMachineInstanceType>) (smi => worker_param.Get<Worker>(smi).StartWork(new Worker.StartWorkInfo((Workable) storage_param.Get<Storage>(smi)))), (Func<StateMachineInstanceType, bool>) (smi => Object.op_Inequality((Object) storage_param.Get<Storage>(smi), (Object) null)), success_state, failure_state);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoPickup(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter source_target,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter result_target,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter amount,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.ToggleWork("Pickup", (System.Action<StateMachineInstanceType>) (smi =>
      {
        Pickupable pickupable = source_target.Get<Pickupable>(smi);
        Worker worker = state_target.Get<Worker>(smi);
        float amount1 = amount.Get(smi);
        Pickupable.PickupableStartWorkInfo start_work_info = new Pickupable.PickupableStartWorkInfo(pickupable, amount1, (System.Action<GameObject>) (result => result_target.Set(result, smi, false)));
        worker.StartWork((Worker.StartWorkInfo) start_work_info);
      }), (Func<StateMachineInstanceType, bool>) (smi => Object.op_Inequality((Object) source_target.Get<Pickupable>(smi), (Object) null) || Object.op_Inequality((Object) result_target.Get<Pickupable>(smi), (Object) null)), success_state, failure_state);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleNotification(
      Func<StateMachineInstanceType, Notification> callback)
    {
      int data_idx = this.CreateDataTableEntry();
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("EnableNotification()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Notification notification = callback(smi);
        smi.dataTable[data_idx] = (object) notification;
        state_target.AddOrGet<Notifier>(smi).Add(notification);
      }));
      this.Exit("DisableNotification()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Notification notification = (Notification) smi.dataTable[data_idx];
        if (notification == null)
          return;
        if (state_target != null)
        {
          Notifier notifier = state_target.Get<Notifier>(smi);
          if (Object.op_Inequality((Object) notifier, (Object) null))
            notifier.Remove(notification);
        }
        smi.dataTable[data_idx] = (object) null;
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoReport(
      ReportManager.ReportType reportType,
      Func<StateMachineInstanceType, float> callback,
      Func<StateMachineInstanceType, string> context_callback = null)
    {
      this.Enter("DoReport()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => ReportManager.Instance.ReportValue(reportType, callback(smi), context_callback != null ? context_callback(smi) : (string) null)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoNotification(
      Func<StateMachineInstanceType, Notification> callback)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("DoNotification()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Notification notification = callback(smi);
        state_target.AddOrGet<Notifier>(smi).Add(notification);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DoTutorial(
      Tutorial.TutorialMessages msg)
    {
      this.Enter("DoTutorial()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => Tutorial.Instance.TutorialMessage(msg)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleScheduleCallback(
      string name,
      Func<StateMachineInstanceType, float> time_cb,
      System.Action<StateMachineInstanceType> callback)
    {
      int data_idx = this.CreateDataTableEntry();
      this.Enter("AddScheduledCallback(" + name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        SchedulerHandle schedulerHandle = GameScheduler.Instance.Schedule(name, time_cb(smi), (System.Action<object>) (smi_data => callback((StateMachineInstanceType) smi_data)), (object) smi, (SchedulerGroup) null);
        DebugUtil.Assert(smi.dataTable[data_idx] == null);
        smi.dataTable[data_idx] = (object) schedulerHandle;
      }));
      this.Exit("RemoveScheduledCallback(" + name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (smi.dataTable[data_idx] == null)
          return;
        SchedulerHandle schedulerHandle = (SchedulerHandle) smi.dataTable[data_idx];
        smi.dataTable[data_idx] = (object) null;
        schedulerHandle.ClearScheduler();
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ScheduleGoTo(
      Func<StateMachineInstanceType, float> time_cb,
      StateMachine.BaseState state)
    {
      this.Enter("ScheduleGoTo(" + state.name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => smi.ScheduleGoTo(time_cb(smi), state)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ScheduleGoTo(
      float time,
      StateMachine.BaseState state)
    {
      this.Enter("ScheduleGoTo(" + time.ToString() + ", " + state.name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => smi.ScheduleGoTo(time, state)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ScheduleAction(
      string name,
      Func<StateMachineInstanceType, float> time_cb,
      System.Action<StateMachineInstanceType> action)
    {
      this.Enter("ScheduleAction(" + name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => smi.Schedule(time_cb(smi), (System.Action<object>) (obj => action(smi)), (object) null)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ScheduleAction(
      string name,
      float time,
      System.Action<StateMachineInstanceType> action)
    {
      this.Enter("ScheduleAction(" + time.ToString() + ", " + name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => smi.Schedule(time, (System.Action<object>) (obj => action(smi)), (object) null)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ScheduleActionNextFrame(
      string name,
      System.Action<StateMachineInstanceType> action)
    {
      this.Enter("ScheduleActionNextFrame(" + name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => smi.ScheduleNextFrame((System.Action<object>) (obj => action(smi)), (object) null)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventHandler(
      GameHashes evt,
      Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
    {
      return this.EventHandler(evt, global_event_system_callback, (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback) ((smi, d) => callback(smi)));
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventHandler(
      GameHashes evt,
      Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback callback)
    {
      if (this.events == null)
        this.events = new List<StateEvent>();
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter stateTarget = this.GetStateTarget();
      this.events.Add((StateEvent) new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent(evt, callback, stateTarget, global_event_system_callback));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventHandler(
      GameHashes evt,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback callback)
    {
      return this.EventHandler(evt, (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback) ((smi, d) => callback(smi)));
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventHandler(
      GameHashes evt,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback callback)
    {
      this.EventHandler(evt, (Func<StateMachineInstanceType, KMonoBehaviour>) null, callback);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventHandlerTransition(
      GameHashes evt,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      Func<StateMachineInstanceType, object, bool> callback)
    {
      return this.EventHandler(evt, (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback) ((smi, d) =>
      {
        if (!callback(smi, d))
          return;
        smi.GoTo((StateMachine.BaseState) state);
      }));
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventHandlerTransition(
      GameHashes evt,
      Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      Func<StateMachineInstanceType, object, bool> callback)
    {
      return this.EventHandler(evt, global_event_system_callback, (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback) ((smi, d) =>
      {
        if (!callback(smi, d))
          return;
        smi.GoTo((StateMachine.BaseState) state);
      }));
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ParamTransition<ParameterType>(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType> parameter,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Callback callback)
    {
      DebugUtil.DevAssert(state != this, "Can't transition to self!", (Object) null);
      if (this.transitions == null)
        this.transitions = new List<StateMachine.BaseTransition>();
      this.transitions.Add((StateMachine.BaseTransition) new StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<ParameterType>.Transition(this.transitions.Count, parameter, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) state, callback));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State OnSignal(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Signal signal,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      Func<StateMachineInstanceType, bool> callback)
    {
      this.ParamTransition<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter>((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter>) signal, state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter>.Callback) ((smi, p) => callback(smi)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State OnSignal(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Signal signal,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state)
    {
      this.ParamTransition<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter>((StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter>) signal, state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Parameter<StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.SignalParameter>.Callback) null);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EnterTransition(
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition)
    {
      string str = "(Stop)";
      if (state != null)
        str = state.name;
      this.Enter("Transition(" + str + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (!condition(smi))
          return;
        smi.GoTo((StateMachine.BaseState) state);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Transition(
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition,
      UpdateRate update_rate = 5)
    {
      string str = "(Stop)";
      if (state != null)
        str = state.name;
      this.Enter("Transition(" + str + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (!condition(smi))
          return;
        smi.GoTo((StateMachine.BaseState) state);
      }));
      this.FastUpdate("Transition(" + str + ")", (UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater) new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.TransitionUpdater(condition, state), update_rate);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State DefaultState(
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State default_state)
    {
      this.defaultState = (StateMachine.BaseState) default_state;
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State GoTo(
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state)
    {
      DebugUtil.DevAssert(state != this, "Can't transition to self", (Object) null);
      string str = "(null)";
      if (state != null)
        str = state.name;
      this.Update("GoTo(" + str + ")", (System.Action<StateMachineInstanceType, float>) ((smi, dt) => smi.GoTo((StateMachine.BaseState) state)));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State StopMoving()
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target = this.GetStateTarget();
      this.Enter("StopMoving()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => target.Get<Navigator>(smi).Stop()));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStationaryIdling()
    {
      this.GetStateTarget();
      this.ToggleTag(GameTags.StationaryIdling);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State OnBehaviourComplete(
      Tag behaviour,
      System.Action<StateMachineInstanceType> cb)
    {
      this.EventHandler(GameHashes.BehaviourTagComplete, (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback) ((smi, d) =>
      {
        if (!Tag.op_Equality((Tag) d, behaviour))
          return;
        cb(smi);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State MoveTo(
      Func<StateMachineInstanceType, int> cell_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state = null,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State fail_state = null,
      bool update_cell = false)
    {
      return this.MoveTo(cell_callback, (Func<StateMachineInstanceType, CellOffset[]>) null, success_state, fail_state, update_cell);
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State MoveTo(
      Func<StateMachineInstanceType, int> cell_callback,
      Func<StateMachineInstanceType, CellOffset[]> cell_offsets_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state = null,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State fail_state = null,
      bool update_cell = false)
    {
      this.EventTransition(GameHashes.DestinationReached, success_state);
      this.EventTransition(GameHashes.NavigationFailed, fail_state);
      CellOffset[] default_offset = new CellOffset[1];
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("MoveTo()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        int num = cell_callback(smi);
        Navigator navigator = state_target.Get<Navigator>(smi);
        CellOffset[] cellOffsetArray = default_offset;
        if (cell_offsets_callback != null)
          cellOffsetArray = cell_offsets_callback(smi);
        int cell = num;
        CellOffset[] offsets = cellOffsetArray;
        navigator.GoTo(cell, offsets);
      }));
      if (update_cell)
        this.Update("MoveTo()", (System.Action<StateMachineInstanceType, float>) ((smi, dt) =>
        {
          int cell = cell_callback(smi);
          state_target.Get<Navigator>(smi).UpdateTarget(cell);
        }));
      this.Exit("StopMoving()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get(smi).GetComponent<Navigator>().Stop()));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State MoveTo<ApproachableType>(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter move_parameter,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State fail_state = null,
      CellOffset[] override_offsets = null,
      NavTactic tactic = null)
      where ApproachableType : IApproachable
    {
      this.EventTransition(GameHashes.DestinationReached, success_state);
      this.EventTransition(GameHashes.NavigationFailed, fail_state);
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      CellOffset[] offsets;
      this.Enter("MoveTo(" + move_parameter.name + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        offsets = override_offsets;
        IApproachable approachable = (IApproachable) move_parameter.Get<ApproachableType>(smi);
        KMonoBehaviour kmonoBehaviour = move_parameter.Get<KMonoBehaviour>(smi);
        if (Object.op_Equality((Object) kmonoBehaviour, (Object) null))
        {
          smi.GoTo((StateMachine.BaseState) fail_state);
        }
        else
        {
          Navigator component = state_target.Get(smi).GetComponent<Navigator>();
          if (offsets == null)
            offsets = approachable.GetOffsets();
          KMonoBehaviour target = kmonoBehaviour;
          CellOffset[] offsets1 = offsets;
          NavTactic tactic1 = tactic;
          component.GoTo(target, offsets1, tactic1);
        }
      }));
      this.Exit("StopMoving()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => state_target.Get<Navigator>(smi).Stop()));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State Face(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter face_target,
      float x_offset = 0.0f)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter(nameof (Face), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (face_target == null)
          return;
        IApproachable approachable = face_target.Get<IApproachable>(smi);
        if (approachable == null)
          return;
        float target_x = TransformExtensions.GetPosition(approachable.transform).x + x_offset;
        state_target.Get<Facing>(smi).Face(target_x);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State TagTransition(
      Tag[] tags,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      bool on_remove = false)
    {
      DebugUtil.DevAssert(state != this, "Can't transition to self!", (Object) null);
      if (this.transitions == null)
        this.transitions = new List<StateMachine.BaseTransition>();
      this.transitions.Add((StateMachine.BaseTransition) new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TagTransitionData(tags.ToString(), this, state, this.transitions.Count, tags, on_remove, this.GetStateTarget()));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State TagTransition(
      Func<StateMachineInstanceType, Tag[]> tags_cb,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      bool on_remove = false)
    {
      DebugUtil.DevAssert(state != this, "Can't transition to self!", (Object) null);
      if (this.transitions == null)
        this.transitions = new List<StateMachine.BaseTransition>();
      this.transitions.Add((StateMachine.BaseTransition) new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TagTransitionData("DynamicTransition", this, state, this.transitions.Count, (Tag[]) null, on_remove, this.GetStateTarget(), tags_cb));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State TagTransition(
      Tag tag,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      bool on_remove = false)
    {
      return this.TagTransition(new Tag[1]{ tag }, state, on_remove);
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventTransition(
      GameHashes evt,
      Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition = null)
    {
      DebugUtil.DevAssert(state != this, "Can't transition to self!", (Object) null);
      if (this.transitions == null)
        this.transitions = new List<StateMachine.BaseTransition>();
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter stateTarget = this.GetStateTarget();
      this.transitions.Add((StateMachine.BaseTransition) new GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.EventTransitionData(this, state, this.transitions.Count, evt, global_event_system_callback, condition, stateTarget));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State EventTransition(
      GameHashes evt,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition = null)
    {
      return this.EventTransition(evt, (Func<StateMachineInstanceType, KMonoBehaviour>) null, state, condition);
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ReturnSuccess()
    {
      this.Enter("ReturnSuccess()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        smi.SetStatus(StateMachine.Status.Success);
        smi.StopSM("GameStateMachine.ReturnSuccess()");
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ReturnFailure()
    {
      this.Enter("ReturnFailure()", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        smi.SetStatus(StateMachine.Status.Failed);
        smi.StopSM("GameStateMachine.ReturnFailure()");
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State ToggleStatusItem(
      string name,
      string tooltip,
      string icon = "",
      StatusItem.IconType icon_type = StatusItem.IconType.Info,
      NotificationType notification_type = NotificationType.Neutral,
      bool allow_multiples = false,
      HashedString render_overlay = default (HashedString),
      int status_overlays = 129022,
      Func<string, StateMachineInstanceType, string> resolve_string_callback = null,
      Func<string, StateMachineInstanceType, string> resolve_tooltip_callback = null,
      StatusItemCategory category = null)
    {
      StatusItem status_item = new StatusItem(this.longName, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays);
      if (resolve_string_callback != null)
        status_item.resolveStringCallback = (Func<string, object, string>) ((str, obj) => resolve_string_callback(str, (StateMachineInstanceType) obj));
      if (resolve_tooltip_callback != null)
        status_item.resolveTooltipCallback = (Func<string, object, string>) ((str, obj) => resolve_tooltip_callback(str, (StateMachineInstanceType) obj));
      this.ToggleStatusItem(status_item, (Func<StateMachineInstanceType, object>) (smi => (object) smi), category);
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlayAnim(
      string anim)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      KAnim.PlayMode mode = (KAnim.PlayMode) 1;
      this.Enter("PlayAnim(" + anim + ", " + mode.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
        if (!Object.op_Inequality((Object) kanimControllerBase, (Object) null))
          return;
        kanimControllerBase.Play(HashedString.op_Implicit(anim), mode);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlayAnim(
      Func<StateMachineInstanceType, string> anim_cb,
      KAnim.PlayMode mode = 1)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("PlayAnim(" + mode.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
        if (!Object.op_Inequality((Object) kanimControllerBase, (Object) null))
          return;
        kanimControllerBase.Play(HashedString.op_Implicit(anim_cb(smi)), mode);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlayAnim(
      string anim,
      KAnim.PlayMode mode)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("PlayAnim(" + anim + ", " + mode.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
        if (!Object.op_Inequality((Object) kanimControllerBase, (Object) null))
          return;
        kanimControllerBase.Play(HashedString.op_Implicit(anim), mode);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlayAnim(
      string anim,
      KAnim.PlayMode mode,
      Func<StateMachineInstanceType, string> suffix_callback)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("PlayAnim(" + anim + ", " + mode.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        string str = "";
        if (suffix_callback != null)
          str = suffix_callback(smi);
        KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
        if (!Object.op_Inequality((Object) kanimControllerBase, (Object) null))
          return;
        kanimControllerBase.Play(HashedString.op_Implicit(anim + str), mode);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State QueueAnim(
      Func<StateMachineInstanceType, string> anim_cb,
      bool loop = false,
      Func<StateMachineInstanceType, string> suffix_callback = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      KAnim.PlayMode mode = (KAnim.PlayMode) 1;
      if (loop)
        mode = (KAnim.PlayMode) 0;
      this.Enter("QueueAnim(" + mode.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        string str = "";
        if (suffix_callback != null)
          str = suffix_callback(smi);
        KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
        if (!Object.op_Inequality((Object) kanimControllerBase, (Object) null))
          return;
        kanimControllerBase.Queue(HashedString.op_Implicit(anim_cb(smi) + str), mode);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State QueueAnim(
      string anim,
      bool loop = false,
      Func<StateMachineInstanceType, string> suffix_callback = null)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      KAnim.PlayMode mode = (KAnim.PlayMode) 1;
      if (loop)
        mode = (KAnim.PlayMode) 0;
      this.Enter("QueueAnim(" + anim + ", " + mode.ToString() + ")", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        string str = "";
        if (suffix_callback != null)
          str = suffix_callback(smi);
        KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
        if (!Object.op_Inequality((Object) kanimControllerBase, (Object) null))
          return;
        kanimControllerBase.Queue(HashedString.op_Implicit(anim + str), mode);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlayAnims(
      Func<StateMachineInstanceType, HashedString[]> anims_callback,
      KAnim.PlayMode mode = 1)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter(nameof (PlayAnims), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
        if (!Object.op_Inequality((Object) kanimControllerBase, (Object) null))
          return;
        HashedString[] anim_names = anims_callback(smi);
        kanimControllerBase.Play(anim_names, mode);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State PlayAnims(
      Func<StateMachineInstanceType, HashedString[]> anims_callback,
      Func<StateMachineInstanceType, KAnim.PlayMode> mode_cb)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter(nameof (PlayAnims), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        KAnimControllerBase kanimControllerBase = state_target.Get<KAnimControllerBase>(smi);
        if (!Object.op_Inequality((Object) kanimControllerBase, (Object) null))
          return;
        HashedString[] anim_names = anims_callback(smi);
        KAnim.PlayMode mode = mode_cb(smi);
        kanimControllerBase.Play(anim_names, mode);
      }));
      return this;
    }

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State OnAnimQueueComplete(
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state)
    {
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter state_target = this.GetStateTarget();
      this.Enter("CheckIfAnimQueueIsEmpty", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        if (!state_target.Get<KBatchedAnimController>(smi).IsStopped())
          return;
        smi.GoTo((StateMachine.BaseState) state);
      }));
      return this.EventTransition(GameHashes.AnimQueueComplete, state);
    }

    internal void EventHandler() => throw new NotImplementedException();

    private class TransitionUpdater : UpdateBucketWithUpdater<StateMachineInstanceType>.IUpdater
    {
      private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition;
      private GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state;

      public TransitionUpdater(
        StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback condition,
        GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State state)
      {
        this.condition = condition;
        this.state = state;
      }

      public void Update(StateMachineInstanceType smi, float dt)
      {
        if (!this.condition(smi))
          return;
        smi.GoTo((StateMachine.BaseState) this.state);
      }
    }
  }

  public class GameEvent : StateEvent
  {
    private GameHashes id;
    private StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target;
    private GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback callback;
    private Func<StateMachineInstanceType, KMonoBehaviour> globalEventSystemCallback;

    public GameEvent(
      GameHashes id,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameEvent.Callback callback,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target,
      Func<StateMachineInstanceType, KMonoBehaviour> global_event_system_callback)
      : base(id.ToString())
    {
      this.id = id;
      this.target = target;
      this.callback = callback;
      this.globalEventSystemCallback = global_event_system_callback;
    }

    public override StateEvent.Context Subscribe(StateMachine.Instance smi)
    {
      StateEvent.Context context = base.Subscribe(smi);
      StateMachineInstanceType cast_smi = (StateMachineInstanceType) smi;
      System.Action<object> action = (System.Action<object>) (d =>
      {
        if (StateMachine.Instance.error)
          return;
        this.callback(cast_smi, d);
      });
      if (this.globalEventSystemCallback != null)
      {
        KMonoBehaviour kmonoBehaviour = this.globalEventSystemCallback(cast_smi);
        context.data = kmonoBehaviour.Subscribe((int) this.id, action);
      }
      else
        context.data = KMonoBehaviourExtensions.Subscribe(this.target.Get(cast_smi), (int) this.id, action);
      return context;
    }

    public override void Unsubscribe(StateMachine.Instance smi, StateEvent.Context context)
    {
      StateMachineInstanceType smi1 = (StateMachineInstanceType) smi;
      if (this.globalEventSystemCallback != null)
      {
        KMonoBehaviour kmonoBehaviour = this.globalEventSystemCallback(smi1);
        if (!Object.op_Inequality((Object) kmonoBehaviour, (Object) null))
          return;
        kmonoBehaviour.Unsubscribe(context.data);
      }
      else
      {
        GameObject gameObject = this.target.Get(smi1);
        if (!Object.op_Inequality((Object) gameObject, (Object) null))
          return;
        KMonoBehaviourExtensions.Unsubscribe(gameObject, context.data);
      }
    }

    public delegate void Callback(StateMachineInstanceType smi, object callback_data)
      where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>
      where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.GameInstance
      where MasterType : IStateMachineTarget;
  }

  public class ApproachSubState<ApproachableType> : 
    GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
    where ApproachableType : IApproachable
  {
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter mover,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter move_target,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state = null,
      CellOffset[] override_offsets = null,
      NavTactic tactic = null)
    {
      this.root.Target(mover).OnTargetLost(move_target, failure_state).MoveTo<ApproachableType>(move_target, success_state, failure_state, override_offsets, tactic == null ? NavigationTactics.ReduceTravelDistance : tactic);
      return (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) this;
    }
  }

  public class DebugGoToSubState : 
    GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
  {
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State exit_state)
    {
      this.root.Enter("GoToCursor", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.GoToCursor(smi))).EventHandler(GameHashes.DebugGoTo, (Func<StateMachineInstanceType, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi => this.GoToCursor(smi))).EventTransition(GameHashes.DestinationReached, exit_state).EventTransition(GameHashes.NavigationFailed, exit_state);
      return (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) this;
    }

    public void GoToCursor(StateMachineInstanceType smi) => smi.GetComponent<Navigator>().GoTo(Grid.PosToCell(DebugHandler.GetMousePos()), new CellOffset[1]);
  }

  public class DropSubState : 
    GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
  {
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter carrier,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter item,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter drop_target,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state = null)
    {
      this.root.Target(carrier).Enter("Drop", (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State.Callback) (smi =>
      {
        Storage storage = carrier.Get<Storage>(smi);
        GameObject gameObject = item.Get(smi);
        GameObject go = gameObject;
        storage.Drop(go, true);
        int cell = Grid.CellAbove(Grid.PosToCell(TransformExtensions.GetPosition(drop_target.Get<Transform>(smi))));
        TransformExtensions.SetPosition(gameObject.transform, Grid.CellToPosCCC(cell, Grid.SceneLayer.Move));
        smi.GoTo((StateMachine.BaseState) success_state);
      }));
      return (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) this;
    }
  }

  public class FetchSubState : 
    GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
  {
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.ApproachSubState<Pickupable> approach;
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State pickup;
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success;

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter fetcher,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter pickup_source,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter pickup_chunk,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter requested_amount,
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.FloatParameter actual_amount,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State success_state,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State failure_state = null)
    {
      this.Target(fetcher);
      this.root.DefaultState((GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) this.approach).ToggleReserve(fetcher, pickup_source, requested_amount, actual_amount);
      this.approach.InitializeStates(fetcher, pickup_source, this.pickup, tactic: NavigationTactics.ReduceTravelDistance).OnTargetLost(pickup_source, failure_state);
      this.pickup.DoPickup(pickup_source, pickup_chunk, actual_amount, success_state, failure_state);
      return (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) this;
    }
  }

  public class HungrySubState : 
    GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
  {
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State satisfied;
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State hungry;

    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter target,
      StatusItem status_item)
    {
      this.Target(target);
      this.root.DefaultState(this.satisfied);
      this.satisfied.EventTransition(GameHashes.AddUrge, this.hungry, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback) (smi => GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.HungrySubState.IsHungry(smi)));
      this.hungry.EventTransition(GameHashes.RemoveUrge, this.satisfied, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback) (smi => !GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.HungrySubState.IsHungry(smi))).ToggleStatusItem(status_item);
      return (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) this;
    }

    private static bool IsHungry(StateMachineInstanceType smi) => smi.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Eat);
  }

  public class PlantAliveSubState : 
    GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State
  {
    public GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State InitializeStates(
      StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.TargetParameter plant,
      GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State death_state = null)
    {
      this.root.Target(plant).TagTransition(GameTags.Uprooted, death_state).EventTransition(GameHashes.TooColdFatal, death_state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback) (smi => GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.PlantAliveSubState.isLethalTemperature(plant.Get(smi)))).EventTransition(GameHashes.TooHotFatal, death_state, (StateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.Transition.ConditionCallback) (smi => GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.PlantAliveSubState.isLethalTemperature(plant.Get(smi)))).EventTransition(GameHashes.Drowned, death_state);
      return (GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, DefType>.State) this;
    }

    public bool ForceUpdateStatus(GameObject plant)
    {
      TemperatureVulnerable component1 = plant.GetComponent<TemperatureVulnerable>();
      EntombVulnerable component2 = plant.GetComponent<EntombVulnerable>();
      PressureVulnerable component3 = plant.GetComponent<PressureVulnerable>();
      if (!Object.op_Equality((Object) component1, (Object) null) && component1.IsLethal || !Object.op_Equality((Object) component2, (Object) null) && component2.GetEntombed)
        return false;
      return Object.op_Equality((Object) component3, (Object) null) || !component3.IsLethal;
    }

    private static bool isLethalTemperature(GameObject plant)
    {
      TemperatureVulnerable component = plant.GetComponent<TemperatureVulnerable>();
      return !Object.op_Equality((Object) component, (Object) null) && (component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.LethalCold || component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.LethalHot);
    }
  }
}
