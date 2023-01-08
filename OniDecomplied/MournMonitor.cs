// Decompiled with JetBrains decompiler
// Type: MournMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;

public class MournMonitor : GameStateMachine<MournMonitor, MournMonitor.Instance>
{
  private GameStateMachine<MournMonitor, MournMonitor.Instance, IStateMachineTarget, object>.State idle;
  private GameStateMachine<MournMonitor, MournMonitor.Instance, IStateMachineTarget, object>.State needsToMourn;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.idle;
    this.idle.EventHandler(GameHashes.EffectAdded, new GameStateMachine<MournMonitor, MournMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback(this.OnEffectAdded)).Enter((StateMachine<MournMonitor, MournMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      if (!this.ShouldMourn(smi))
        return;
      smi.GoTo((StateMachine.BaseState) this.needsToMourn);
    }));
    this.needsToMourn.ToggleChore((Func<MournMonitor.Instance, Chore>) (smi => (Chore) new MournChore(smi.master)), this.idle);
  }

  private bool ShouldMourn(MournMonitor.Instance smi)
  {
    Effect effect = Db.Get().effects.Get("Mourning");
    return smi.master.GetComponent<Effects>().HasEffect(effect);
  }

  private void OnEffectAdded(MournMonitor.Instance smi, object data)
  {
    if (!this.ShouldMourn(smi))
      return;
    smi.GoTo((StateMachine.BaseState) this.needsToMourn);
  }

  public new class Instance : 
    GameStateMachine<MournMonitor, MournMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }
  }
}
