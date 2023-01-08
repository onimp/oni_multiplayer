// Decompiled with JetBrains decompiler
// Type: CritterElementMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class CritterElementMonitor : 
  GameStateMachine<CritterElementMonitor, CritterElementMonitor.Instance, IStateMachineTarget>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.Update("UpdateInElement", (System.Action<CritterElementMonitor.Instance, float>) ((smi, dt) => smi.UpdateCurrentElement(dt)), (UpdateRate) 6);
  }

  public new class Instance : 
    GameStateMachine<CritterElementMonitor, CritterElementMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public event System.Action<float> OnUpdateEggChances;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public void UpdateCurrentElement(float dt) => this.OnUpdateEggChances(dt);
  }
}
