// Decompiled with JetBrains decompiler
// Type: QuarantineFeedableMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class QuarantineFeedableMonitor : 
  GameStateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance>
{
  public GameStateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public GameStateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance, IStateMachineTarget, object>.State hungry;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.satisfied.EventTransition(GameHashes.AddUrge, this.hungry, (StateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsHungry()));
    this.hungry.EventTransition(GameHashes.RemoveUrge, this.satisfied, (StateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsHungry()));
  }

  public new class Instance : 
    GameStateMachine<QuarantineFeedableMonitor, QuarantineFeedableMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public bool IsHungry() => this.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.Eat);
  }
}
