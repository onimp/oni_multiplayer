// Decompiled with JetBrains decompiler
// Type: PoweredActiveTransitionController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class PoweredActiveTransitionController : 
  GameStateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>
{
  public GameStateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.State off;
  public GameStateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.State on;
  public GameStateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.State on_pre;
  public GameStateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.State on_pst;
  public GameStateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.State working;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on_pre, (StateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
    this.on_pre.PlayAnim("working_pre").OnAnimQueueComplete(this.on);
    this.on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.on_pst, (StateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).EventTransition(GameHashes.ActiveChanged, this.working, (StateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsActive));
    this.on_pst.PlayAnim("working_pst").OnAnimQueueComplete(this.off);
    this.working.PlayAnim("working_loop", (KAnim.PlayMode) 0).EventTransition(GameHashes.OperationalChanged, this.on_pst, (StateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).EventTransition(GameHashes.ActiveChanged, this.on, (StateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsActive)).Enter((StateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.State.Callback) (smi =>
    {
      if (!smi.def.showWorkingStatus)
        return;
      smi.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.Working);
    })).Exit((StateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.State.Callback) (smi =>
    {
      if (!smi.def.showWorkingStatus)
        return;
      smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.Working);
    }));
  }

  public class Def : StateMachine.BaseDef
  {
    public bool showWorkingStatus;
  }

  public new class Instance : 
    GameStateMachine<PoweredActiveTransitionController, PoweredActiveTransitionController.Instance, IStateMachineTarget, PoweredActiveTransitionController.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, PoweredActiveTransitionController.Def def)
      : base(master, def)
    {
    }
  }
}
