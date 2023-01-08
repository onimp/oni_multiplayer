// Decompiled with JetBrains decompiler
// Type: SolidLogicValve
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;

[SerializationConfig]
public class SolidLogicValve : StateMachineComponent<SolidLogicValve.StatesInstance>
{
  [MyCmpReq]
  private Operational operational;
  [MyCmpReq]
  private SolidConduitBridge bridge;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  protected override void OnCleanUp() => base.OnCleanUp();

  public class States : 
    GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve>
  {
    public GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State off;
    public SolidLogicValve.States.ReadyStates on;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.off;
      this.root.DoNothing();
      this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, (GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State) this.on, (StateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational)).Enter((StateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(false)));
      this.on.DefaultState(this.on.idle).EventTransition(GameHashes.OperationalChanged, this.off, (StateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).Enter((StateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(true)));
      this.on.idle.PlayAnim("on").Transition(this.on.working, (StateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.Transition.ConditionCallback) (smi => smi.IsDispensing()));
      this.on.working.PlayAnim("on_flow", (KAnim.PlayMode) 0).Transition(this.on.idle, (StateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.Transition.ConditionCallback) (smi => !smi.IsDispensing()));
    }

    public class ReadyStates : 
      GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State
    {
      public GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State idle;
      public GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.State working;
    }
  }

  public class StatesInstance : 
    GameStateMachine<SolidLogicValve.States, SolidLogicValve.StatesInstance, SolidLogicValve, object>.GameInstance
  {
    public StatesInstance(SolidLogicValve master)
      : base(master)
    {
    }

    public bool IsDispensing() => this.master.bridge.IsDispensing;
  }
}
