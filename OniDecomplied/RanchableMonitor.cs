// Decompiled with JetBrains decompiler
// Type: RanchableMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class RanchableMonitor : 
  GameStateMachine<RanchableMonitor, RanchableMonitor.Instance, IStateMachineTarget, RanchableMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleBehaviour(GameTags.Creatures.WantsToGetRanched, (StateMachine<RanchableMonitor, RanchableMonitor.Instance, IStateMachineTarget, RanchableMonitor.Def>.Transition.ConditionCallback) (smi => smi.ShouldGoGetRanched()));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<RanchableMonitor, RanchableMonitor.Instance, IStateMachineTarget, RanchableMonitor.Def>.GameInstance
  {
    public RanchStation.Instance TargetRanchStation;
    private Navigator navComponent;
    private RanchedStates.Instance states;

    public ChoreConsumer ChoreConsumer { get; private set; }

    public Navigator NavComponent => this.navComponent;

    public RanchedStates.Instance States
    {
      get
      {
        if (this.states == null)
          this.states = this.controller.GetSMI<RanchedStates.Instance>();
        return this.states;
      }
    }

    public Instance(IStateMachineTarget master, RanchableMonitor.Def def)
      : base(master, def)
    {
      this.ChoreConsumer = this.GetComponent<ChoreConsumer>();
      this.navComponent = this.GetComponent<Navigator>();
    }

    public bool ShouldGoGetRanched() => this.TargetRanchStation != null && this.TargetRanchStation.IsRunning() && this.TargetRanchStation.HasRancher;
  }
}
