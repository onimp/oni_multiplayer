// Decompiled with JetBrains decompiler
// Type: RescueSweepBotChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class RescueSweepBotChore : Chore<RescueSweepBotChore.StatesInstance>
{
  public Chore.Precondition CanReachBaseStation = new Chore.Precondition()
  {
    id = nameof (CanReachBaseStation),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      if (Object.op_Equality((Object) context.consumerState.consumer, (Object) null))
        return false;
      KMonoBehaviour cmp = (KMonoBehaviour) data;
      return !Object.op_Equality((Object) cmp, (Object) null) && context.consumerState.consumer.navigator.CanReach(Grid.PosToCell(cmp));
    })
  };
  public static Chore.Precondition CanReachIncapacitated = new Chore.Precondition()
  {
    id = nameof (CanReachIncapacitated),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      KMonoBehaviour kmonoBehaviour = (KMonoBehaviour) data;
      if (Object.op_Equality((Object) kmonoBehaviour, (Object) null))
        return false;
      int navigationCost = context.consumerState.navigator.GetNavigationCost(Grid.PosToCell(TransformExtensions.GetPosition(kmonoBehaviour.transform)));
      if (-1 == navigationCost)
        return false;
      context.cost += navigationCost;
      return true;
    })
  };

  public RescueSweepBotChore(
    IStateMachineTarget master,
    GameObject sweepBot,
    GameObject baseStation)
    : base(Db.Get().ChoreTypes.RescueIncapacitated, master, (ChoreProvider) null, false, master_priority_class: PriorityScreen.PriorityClass.personalNeeds)
  {
    this.smi = new RescueSweepBotChore.StatesInstance(this);
    this.runUntilComplete = true;
    this.AddPrecondition(RescueSweepBotChore.CanReachIncapacitated, (object) sweepBot.GetComponent<Storage>());
    this.AddPrecondition(this.CanReachBaseStation, (object) baseStation.GetComponent<Storage>());
  }

  public override void Begin(Chore.Precondition.Context context)
  {
    this.smi.sm.rescuer.Set(context.consumerState.gameObject, this.smi, false);
    this.smi.sm.rescueTarget.Set(this.gameObject, this.smi, false);
    this.smi.sm.deliverTarget.Set(((Component) this.gameObject.GetSMI<SweepBotTrappedStates.Instance>().sm.GetSweepLocker(this.gameObject.GetSMI<SweepBotTrappedStates.Instance>())).gameObject, this.smi, false);
    base.Begin(context);
  }

  protected override void End(string reason)
  {
    this.DropSweepBot();
    base.End(reason);
  }

  private void DropSweepBot()
  {
    if (!Object.op_Inequality((Object) this.smi.sm.rescuer.Get(this.smi), (Object) null) || !Object.op_Inequality((Object) this.smi.sm.rescueTarget.Get(this.smi), (Object) null))
      return;
    this.smi.sm.rescuer.Get(this.smi).GetComponent<Storage>().Drop(this.smi.sm.rescueTarget.Get(this.smi), true);
  }

  public class StatesInstance : 
    GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.GameInstance
  {
    public StatesInstance(RescueSweepBotChore master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore>
  {
    public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.ApproachSubState<Storage> approachSweepBot;
    public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State failure;
    public RescueSweepBotChore.States.HoldingSweepBot holding;
    public StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.TargetParameter rescueTarget;
    public StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.TargetParameter deliverTarget;
    public StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.TargetParameter rescuer;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.approachSweepBot;
      this.approachSweepBot.InitializeStates(this.rescuer, this.rescueTarget, this.holding.pickup, this.failure, Grid.DefaultOffset);
      this.holding.Target(this.rescuer).Enter((StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State.Callback) (smi =>
      {
        KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("anim_incapacitated_carrier_kanim"));
        this.rescuer.Get(smi).GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim);
        this.rescuer.Get(smi).GetComponent<KAnimControllerBase>().AddAnimOverrides(anim);
      })).Exit((StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State.Callback) (smi =>
      {
        KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("anim_incapacitated_carrier_kanim"));
        this.rescuer.Get(smi).GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim);
      }));
      this.holding.pickup.Target(this.rescuer).PlayAnim("pickup").Enter((StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State.Callback) (smi => { })).Exit((StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State.Callback) (smi =>
      {
        this.rescuer.Get(smi).GetComponent<Storage>().Store(this.rescueTarget.Get(smi));
        TransformExtensions.SetLocalPosition(this.rescueTarget.Get(smi).transform, Vector3.zero);
        KBatchedAnimTracker component = this.rescueTarget.Get(smi).GetComponent<KBatchedAnimTracker>();
        component.symbol = new HashedString("snapTo_pivot");
        component.offset = new Vector3(0.0f, 0.0f, 1f);
      })).EventTransition(GameHashes.AnimQueueComplete, (GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State) this.holding.delivering);
      this.holding.delivering.InitializeStates(this.rescuer, this.deliverTarget, this.holding.deposit, this.holding.ditch).Update((Action<RescueSweepBotChore.StatesInstance, float>) ((smi, dt) =>
      {
        if (!Object.op_Equality((Object) this.deliverTarget.Get(smi), (Object) null))
          return;
        smi.GoTo((StateMachine.BaseState) this.holding.ditch);
      }));
      this.holding.deposit.PlayAnim("place").EventHandler(GameHashes.AnimQueueComplete, (StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State.Callback) (smi =>
      {
        smi.master.DropSweepBot();
        smi.SetStatus(StateMachine.Status.Success);
        smi.StopSM("complete");
      }));
      this.holding.ditch.PlayAnim("place").ScheduleGoTo(0.5f, (StateMachine.BaseState) this.failure).Exit((StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State.Callback) (smi => smi.master.DropSweepBot()));
      this.failure.Enter((StateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State.Callback) (smi =>
      {
        smi.SetStatus(StateMachine.Status.Failed);
        smi.StopSM("failed");
      }));
    }

    public class HoldingSweepBot : 
      GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State
    {
      public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State pickup;
      public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.ApproachSubState<IApproachable> delivering;
      public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State deposit;
      public GameStateMachine<RescueSweepBotChore.States, RescueSweepBotChore.StatesInstance, RescueSweepBotChore, object>.State ditch;
    }
  }
}
