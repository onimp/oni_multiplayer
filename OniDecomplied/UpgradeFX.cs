// Decompiled with JetBrains decompiler
// Type: UpgradeFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class UpgradeFX : GameStateMachine<UpgradeFX, UpgradeFX.Instance>
{
  public StateMachine<UpgradeFX, UpgradeFX.Instance, IStateMachineTarget, object>.TargetParameter fx;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.Target(this.fx);
    this.root.PlayAnim("upgrade").OnAnimQueueComplete((GameStateMachine<UpgradeFX, UpgradeFX.Instance, IStateMachineTarget, object>.State) null).ToggleReactable((Func<UpgradeFX.Instance, Reactable>) (smi => smi.CreateReactable())).Exit("DestroyFX", (StateMachine<UpgradeFX, UpgradeFX.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.DestroyFX()));
  }

  public new class Instance : 
    GameStateMachine<UpgradeFX, UpgradeFX.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master, Vector3 offset)
      : base(master)
    {
      this.sm.fx.Set(((Component) FXHelpers.CreateEffect("upgrade_fx_kanim", Vector3.op_Addition(TransformExtensions.GetPosition(master.gameObject.transform), offset), master.gameObject.transform, true)).gameObject, this.smi);
    }

    public void DestroyFX() => Util.KDestroyGameObject(this.sm.fx.Get(this.smi));

    public Reactable CreateReactable() => (Reactable) new EmoteReactable(this.master.gameObject, HashedString.op_Implicit(nameof (UpgradeFX)), Db.Get().ChoreTypes.Emote).SetEmote(Db.Get().Emotes.Minion.Cheer);
  }
}
