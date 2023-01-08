// Decompiled with JetBrains decompiler
// Type: DoctorMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class DoctorMonitor : GameStateMachine<DoctorMonitor, DoctorMonitor.Instance>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.root.ToggleUrge(Db.Get().Urges.Doctor);
  }

  public new class Instance : 
    GameStateMachine<DoctorMonitor, DoctorMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }
  }
}
