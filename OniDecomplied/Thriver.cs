// Decompiled with JetBrains decompiler
// Type: Thriver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[SkipSaveFileSerialization]
public class Thriver : StateMachineComponent<Thriver.StatesInstance>
{
  protected virtual void OnSpawn() => this.smi.StartSM();

  public class StatesInstance : 
    GameStateMachine<Thriver.States, Thriver.StatesInstance, Thriver, object>.GameInstance
  {
    public StatesInstance(Thriver master)
      : base(master)
    {
    }

    public bool IsStressed()
    {
      StressMonitor.Instance smi = ((Component) this.master).GetSMI<StressMonitor.Instance>();
      return smi != null && smi.IsStressed();
    }
  }

  public class States : GameStateMachine<Thriver.States, Thriver.StatesInstance, Thriver>
  {
    public GameStateMachine<Thriver.States, Thriver.StatesInstance, Thriver, object>.State idle;
    public GameStateMachine<Thriver.States, Thriver.StatesInstance, Thriver, object>.State stressed;
    public GameStateMachine<Thriver.States, Thriver.StatesInstance, Thriver, object>.State toostressed;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.root.EventTransition(GameHashes.NotStressed, this.idle).EventTransition(GameHashes.Stressed, this.stressed).EventTransition(GameHashes.StressedHadEnough, this.stressed).Enter((StateMachine<Thriver.States, Thriver.StatesInstance, Thriver, object>.State.Callback) (smi =>
      {
        StressMonitor.Instance smi1 = ((Component) smi.master).GetSMI<StressMonitor.Instance>();
        if (smi1 == null || !smi1.IsStressed())
          return;
        smi.GoTo((StateMachine.BaseState) this.stressed);
      }));
      this.idle.DoNothing();
      this.stressed.ToggleEffect(nameof (Thriver));
      this.toostressed.DoNothing();
    }
  }
}
