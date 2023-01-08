// Decompiled with JetBrains decompiler
// Type: RocketSelfDestructMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class RocketSelfDestructMonitor : 
  GameStateMachine<RocketSelfDestructMonitor, RocketSelfDestructMonitor.Instance>
{
  public GameStateMachine<RocketSelfDestructMonitor, RocketSelfDestructMonitor.Instance, IStateMachineTarget, object>.State idle;
  public GameStateMachine<RocketSelfDestructMonitor, RocketSelfDestructMonitor.Instance, IStateMachineTarget, object>.State exploding;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.idle;
    this.idle.EventTransition(GameHashes.RocketSelfDestructRequested, this.exploding);
    this.exploding.Update((System.Action<RocketSelfDestructMonitor.Instance, float>) ((smi, dt) =>
    {
      if ((double) smi.timeinstate < 3.0)
        return;
      smi.master.Trigger(-1311384361, (object) null);
      smi.GoTo((StateMachine.BaseState) this.idle);
    }));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<RocketSelfDestructMonitor, RocketSelfDestructMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public KBatchedAnimController eyes;

    public Instance(IStateMachineTarget master, RocketSelfDestructMonitor.Def def)
      : base(master)
    {
    }
  }
}
