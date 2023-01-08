// Decompiled with JetBrains decompiler
// Type: RancherChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using TUNING;
using UnityEngine;

public class RancherChore : Chore<RancherChore.RancherChoreStates.Instance>
{
  public Chore.Precondition IsOpenForRanching = new Chore.Precondition()
  {
    id = "IsCreatureAvailableForRanching",
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_CREATURE_AVAILABLE_FOR_RANCHING,
    sortOrder = -3,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      RanchStation.Instance instance = data as RanchStation.Instance;
      return !instance.HasRancher && instance.IsCritterAvailableForRanching;
    })
  };

  public RancherChore(KPrefabID rancher_station)
    : base(Db.Get().ChoreTypes.Ranch, (IStateMachineTarget) rancher_station, (ChoreProvider) null, false)
  {
    this.AddPrecondition(this.IsOpenForRanching, (object) ((Component) rancher_station).GetSMI<RanchStation.Instance>());
    this.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) Db.Get().SkillPerks.CanUseRanchStation.Id);
    this.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, (object) Db.Get().ScheduleBlockTypes.Work);
    this.AddPrecondition(ChorePreconditions.instance.CanMoveTo, (object) ((Component) rancher_station).GetComponent<Building>());
    this.AddPrecondition(ChorePreconditions.instance.IsOperational, (object) ((Component) rancher_station).GetComponent<Operational>());
    this.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, (object) ((Component) rancher_station).GetComponent<Deconstructable>());
    this.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, (object) ((Component) rancher_station).GetComponent<BuildingEnabledButton>());
    this.smi = new RancherChore.RancherChoreStates.Instance(rancher_station);
    this.SetPrioritizable(((Component) rancher_station).GetComponent<Prioritizable>());
  }

  public override void Begin(Chore.Precondition.Context context)
  {
    this.smi.sm.rancher.Set(context.consumerState.gameObject, this.smi);
    base.Begin(context);
  }

  protected override void End(string reason)
  {
    base.End(reason);
    this.smi.sm.rancher.Set((KMonoBehaviour) null, this.smi);
  }

  public class RancherChoreStates : 
    GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance>
  {
    public StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.TargetParameter rancher;
    private GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State moveToRanch;
    private RancherChore.RancherChoreStates.RanchState ranchCritter;
    private GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State waitForAvailableRanchable;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.moveToRanch;
      this.Target(this.rancher);
      this.root.Exit("TriggerRanchStationNoLongerAvailable", (StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.TriggerRanchStationNoLongerAvailable()));
      this.moveToRanch.MoveTo((Func<RancherChore.RancherChoreStates.Instance, int>) (smi => Grid.PosToCell(TransformExtensions.GetPosition(smi.transform))), this.waitForAvailableRanchable);
      this.waitForAvailableRanchable.Enter("FindRanchable", (StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.WaitForAvailableRanchable(0.0f))).Update("FindRanchable", (Action<RancherChore.RancherChoreStates.Instance, float>) ((smi, dt) => smi.WaitForAvailableRanchable(dt)));
      this.ranchCritter.ScheduleGoTo(0.5f, (StateMachine.BaseState) this.ranchCritter.callForCritter).EventTransition(GameHashes.CreatureAbandonedRanchStation, this.waitForAvailableRanchable);
      this.ranchCritter.callForCritter.ToggleAnims("anim_interacts_rancherstation_kanim").PlayAnim("calling_loop", (KAnim.PlayMode) 0).ScheduleActionNextFrame("TellCreatureRancherIsReady", (Action<RancherChore.RancherChoreStates.Instance>) (smi => smi.TellCreatureRancherIsReady())).Target(this.masterTarget).EventTransition(GameHashes.CreatureArrivedAtRanchStation, this.ranchCritter.working);
      this.ranchCritter.working.ToggleWork<RancherChore.RancherWorkable>(this.masterTarget, this.ranchCritter.pst, this.waitForAvailableRanchable, (Func<RancherChore.RancherChoreStates.Instance, bool>) null);
      this.ranchCritter.pst.ToggleAnims(new Func<RancherChore.RancherChoreStates.Instance, HashedString>(RancherChore.RancherChoreStates.GetRancherInteractAnim)).QueueAnim("wipe_brow").OnAnimQueueComplete(this.waitForAvailableRanchable);
    }

    private static HashedString GetRancherInteractAnim(RancherChore.RancherChoreStates.Instance smi) => smi.ranchStation.def.RancherInteractAnim;

    public static bool TryRanchCreature(RancherChore.RancherChoreStates.Instance smi)
    {
      Debug.Assert(smi.ranchStation != null, (object) "smi.ranchStation was null");
      RanchedStates.Instance activeRanchable = smi.ranchStation.ActiveRanchable;
      if (activeRanchable.IsNullOrStopped())
        return false;
      KPrefabID component = activeRanchable.GetComponent<KPrefabID>();
      EventExtensions.Trigger(smi.sm.rancher.Get(smi), 937885943, (object) ((Tag) ref component.PrefabTag).Name);
      smi.ranchStation.RanchCreature();
      return true;
    }

    private class RanchState : 
      GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State
    {
      public GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State callForCritter;
      public GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State working;
      public GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State pst;
    }

    public new class Instance : 
      GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.GameInstance
    {
      private const float WAIT_FOR_RANCHABLE_TIMEOUT = 2f;
      public RanchStation.Instance ranchStation;
      private float waitTime;

      public Instance(KPrefabID rancher_station)
        : base((IStateMachineTarget) rancher_station)
      {
        this.ranchStation = ((Component) rancher_station).GetSMI<RanchStation.Instance>();
      }

      public void WaitForAvailableRanchable(float dt)
      {
        this.waitTime += dt;
        GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State state = this.ranchStation.IsCritterAvailableForRanching ? (GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State) this.sm.ranchCritter : (GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State) null;
        if (state == null && (double) this.waitTime < 2.0)
          return;
        this.waitTime = 0.0f;
        this.GoTo((StateMachine.BaseState) state);
      }

      public void TriggerRanchStationNoLongerAvailable() => this.ranchStation.TriggerRanchStationNoLongerAvailable();

      public void TellCreatureRancherIsReady() => this.ranchStation.MessageRancherReady();
    }
  }

  public class RancherWorkable : Workable
  {
    private RanchStation.Instance ranch;
    private KBatchedAnimController critterAnimController;

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      this.ranch = ((Component) this).gameObject.GetSMI<RanchStation.Instance>();
      this.overrideAnims = new KAnimFile[1]
      {
        Assets.GetAnim(this.ranch.def.RancherInteractAnim)
      };
      this.SetWorkTime(this.ranch.def.WorkTime);
      this.SetWorkerStatusItem(this.ranch.def.RanchingStatusItem);
      this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
      this.skillExperienceSkillGroup = Db.Get().SkillGroups.Ranching.Id;
      this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
      this.lightEfficiencyBonus = false;
    }

    public override Klei.AI.Attribute GetWorkAttribute() => Db.Get().Attributes.Ranching;

    protected override void OnStartWork(Worker worker)
    {
      if (this.ranch == null)
        return;
      this.critterAnimController = this.ranch.ActiveRanchable.AnimController;
      this.critterAnimController.Play(this.ranch.def.RanchedPreAnim);
      this.critterAnimController.Queue(this.ranch.def.RanchedLoopAnim, (KAnim.PlayMode) 0);
    }

    public override void OnPendingCompleteWork(Worker work)
    {
      RancherChore.RancherChoreStates.Instance smi = ((Component) this).gameObject.GetSMI<RancherChore.RancherChoreStates.Instance>();
      if (this.ranch == null || smi == null || !RancherChore.RancherChoreStates.TryRanchCreature(smi))
        return;
      this.critterAnimController.Play(this.ranch.def.RanchedPstAnim);
    }

    protected override void OnAbortWork(Worker worker)
    {
      if (this.ranch == null || Object.op_Equality((Object) this.critterAnimController, (Object) null))
        return;
      this.critterAnimController.Play(this.ranch.def.RanchedAbortAnim);
    }
  }
}
