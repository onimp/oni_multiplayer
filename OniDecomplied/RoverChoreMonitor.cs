// Decompiled with JetBrains decompiler
// Type: RoverChoreMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;

public class RoverChoreMonitor : 
  GameStateMachine<RoverChoreMonitor, RoverChoreMonitor.Instance, IStateMachineTarget, RoverChoreMonitor.Def>
{
  public GameStateMachine<RoverChoreMonitor, RoverChoreMonitor.Instance, IStateMachineTarget, RoverChoreMonitor.Def>.State loop;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.loop;
    this.loop.ToggleBehaviour(GameTags.Creatures.Tunnel, (StateMachine<RoverChoreMonitor, RoverChoreMonitor.Instance, IStateMachineTarget, RoverChoreMonitor.Def>.Transition.ConditionCallback) (smi => true)).ToggleBehaviour(GameTags.Creatures.Builder, (StateMachine<RoverChoreMonitor, RoverChoreMonitor.Instance, IStateMachineTarget, RoverChoreMonitor.Def>.Transition.ConditionCallback) (smi => true));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<RoverChoreMonitor, RoverChoreMonitor.Instance, IStateMachineTarget, RoverChoreMonitor.Def>.GameInstance
  {
    [Serialize]
    public int lastDigCell = -1;
    private System.Action<object> OnDestinationReachedDelegate;

    public Instance(IStateMachineTarget master, RoverChoreMonitor.Def def)
      : base(master, def)
    {
    }

    protected override void OnCleanUp() => base.OnCleanUp();
  }
}
