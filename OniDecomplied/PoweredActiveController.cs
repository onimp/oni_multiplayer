// Decompiled with JetBrains decompiler
// Type: PoweredActiveController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class PoweredActiveController : 
  GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>
{
  public GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State off;
  public GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State on;
  public PoweredActiveController.WorkingStates working;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (StateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
    this.on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.off, (StateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).EventTransition(GameHashes.ActiveChanged, this.working.pre, (StateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsActive));
    this.working.Enter((StateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State.Callback) (smi =>
    {
      if (!smi.def.showWorkingStatus)
        return;
      smi.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.Working);
    })).Exit((StateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State.Callback) (smi =>
    {
      if (!smi.def.showWorkingStatus)
        return;
      smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.Working);
    }));
    this.working.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.working.loop);
    this.working.loop.PlayAnim("working_loop", (KAnim.PlayMode) 0).EventTransition(GameHashes.OperationalChanged, this.working.pst, (StateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).EventTransition(GameHashes.ActiveChanged, this.working.pst, (StateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsActive));
    this.working.pst.PlayAnim("working_pst").OnAnimQueueComplete(this.on);
  }

  public class Def : StateMachine.BaseDef
  {
    public bool showWorkingStatus;
  }

  public class WorkingStates : 
    GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State
  {
    public GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State pre;
    public GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State loop;
    public GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.State pst;
  }

  public new class Instance : 
    GameStateMachine<PoweredActiveController, PoweredActiveController.Instance, IStateMachineTarget, PoweredActiveController.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, PoweredActiveController.Def def)
      : base(master, def)
    {
    }
  }
}
