// Decompiled with JetBrains decompiler
// Type: Dreamer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class Dreamer : GameStateMachine<Dreamer, Dreamer.Instance>
{
  public StateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.Signal stopDreaming;
  public StateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.Signal startDreaming;
  public GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State notDreaming;
  public GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State dreaming;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.notDreaming;
    this.notDreaming.OnSignal(this.startDreaming, this.dreaming, (Func<Dreamer.Instance, bool>) (smi => smi.currentDream != null));
    this.dreaming.Enter(new StateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State.Callback(Dreamer.PrepareDream)).OnSignal(this.stopDreaming, this.notDreaming).Update(new System.Action<Dreamer.Instance, float>(this.UpdateDream), (UpdateRate) 3).Exit(new StateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State.Callback(this.RemoveDream));
  }

  private void RemoveDream(Dreamer.Instance smi)
  {
    smi.SetDream((Dream) null);
    NameDisplayScreen.Instance.StopDreaming(smi.gameObject);
  }

  private void UpdateDream(Dreamer.Instance smi, float dt) => NameDisplayScreen.Instance.DreamTick(smi.gameObject, dt);

  private static void PrepareDream(Dreamer.Instance smi) => NameDisplayScreen.Instance.SetDream(smi.gameObject, smi.currentDream);

  public class DreamingState : 
    GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State hidden;
    public GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.State visible;
  }

  public new class Instance : 
    GameStateMachine<Dreamer, Dreamer.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Dream currentDream;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      NameDisplayScreen.Instance.RegisterComponent(this.gameObject, (object) this);
    }

    public void SetDream(Dream dream) => this.currentDream = dream;

    public void StartDreaming() => this.sm.startDreaming.Trigger(this.smi);

    public void StopDreaming()
    {
      this.SetDream((Dream) null);
      this.sm.stopDreaming.Trigger(this.smi);
    }
  }
}
