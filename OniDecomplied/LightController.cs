// Decompiled with JetBrains decompiler
// Type: LightController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class LightController : GameStateMachine<LightController, LightController.Instance>
{
  public GameStateMachine<LightController, LightController.Instance, IStateMachineTarget, object>.State off;
  public GameStateMachine<LightController, LightController.Instance, IStateMachineTarget, object>.State on;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.on, (StateMachine<LightController, LightController.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
    this.on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.off, (StateMachine<LightController, LightController.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingLight).Enter("SetActive", (StateMachine<LightController, LightController.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(true)));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<LightController, LightController.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master, LightController.Def def)
      : base(master, (object) def)
    {
    }
  }
}
