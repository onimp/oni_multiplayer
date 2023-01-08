// Decompiled with JetBrains decompiler
// Type: OperationalController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class OperationalController : 
  GameStateMachine<OperationalController, OperationalController.Instance>
{
  public GameStateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget, object>.State off;
  public GameStateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget, object>.State working_pre;
  public GameStateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget, object>.State working_loop;
  public GameStateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget, object>.State working_pst;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.root.EventTransition(GameHashes.OperationalChanged, this.off, (StateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational));
    this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.working_pre, (StateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
    this.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(this.working_loop);
    this.working_loop.PlayAnim("working_loop", (KAnim.PlayMode) 0).EventTransition(GameHashes.OperationalChanged, this.working_pst, (StateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational));
    this.working_pst.PlayAnim("working_pst").OnAnimQueueComplete(this.off);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<OperationalController, OperationalController.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master, OperationalController.Def def)
      : base(master, (object) def)
    {
    }
  }
}
