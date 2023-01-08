// Decompiled with JetBrains decompiler
// Type: FixedCapturableMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class FixedCapturableMonitor : 
  GameStateMachine<FixedCapturableMonitor, FixedCapturableMonitor.Instance, IStateMachineTarget, FixedCapturableMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleBehaviour(GameTags.Creatures.WantsToGetCaptured, (StateMachine<FixedCapturableMonitor, FixedCapturableMonitor.Instance, IStateMachineTarget, FixedCapturableMonitor.Def>.Transition.ConditionCallback) (smi => smi.ShouldGoGetCaptured())).Enter((StateMachine<FixedCapturableMonitor, FixedCapturableMonitor.Instance, IStateMachineTarget, FixedCapturableMonitor.Def>.State.Callback) (smi => Components.FixedCapturableMonitors.Add(smi))).Exit((StateMachine<FixedCapturableMonitor, FixedCapturableMonitor.Instance, IStateMachineTarget, FixedCapturableMonitor.Def>.State.Callback) (smi => Components.FixedCapturableMonitors.Remove(smi)));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<FixedCapturableMonitor, FixedCapturableMonitor.Instance, IStateMachineTarget, FixedCapturableMonitor.Def>.GameInstance
  {
    public FixedCapturePoint.Instance targetCapturePoint;

    public Instance(IStateMachineTarget master, FixedCapturableMonitor.Def def)
      : base(master, def)
    {
    }

    public bool ShouldGoGetCaptured() => this.targetCapturePoint != null && this.targetCapturePoint.IsRunning() && this.targetCapturePoint.shouldCreatureGoGetCaptured;
  }
}
