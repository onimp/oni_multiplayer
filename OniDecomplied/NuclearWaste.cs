// Decompiled with JetBrains decompiler
// Type: NuclearWaste
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

public class NuclearWaste : 
  GameStateMachine<NuclearWaste, NuclearWaste.Instance, IStateMachineTarget, NuclearWaste.Def>
{
  private const float lifetime = 600f;
  public GameStateMachine<NuclearWaste, NuclearWaste.Instance, IStateMachineTarget, NuclearWaste.Def>.State idle;
  public GameStateMachine<NuclearWaste, NuclearWaste.Instance, IStateMachineTarget, NuclearWaste.Def>.State decayed;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.idle;
    this.idle.PlayAnim((Func<NuclearWaste.Instance, string>) (smi => smi.GetAnimToPlay())).Update((System.Action<NuclearWaste.Instance, float>) ((smi, dt) =>
    {
      smi.timeAlive += dt;
      string animToPlay = smi.GetAnimToPlay();
      if (smi.GetComponent<KBatchedAnimController>().GetCurrentAnim().name != animToPlay)
        smi.Play(animToPlay);
      if ((double) smi.timeAlive < 600.0)
        return;
      smi.GoTo((StateMachine.BaseState) this.decayed);
    }), (UpdateRate) 7).EventHandler(GameHashes.Absorb, (GameStateMachine<NuclearWaste, NuclearWaste.Instance, IStateMachineTarget, NuclearWaste.Def>.GameEvent.Callback) ((smi, otherObject) =>
    {
      Pickupable cmp = (Pickupable) otherObject;
      float timeAlive = ((Component) cmp).GetSMI<NuclearWaste.Instance>().timeAlive;
      float mass1 = ((Component) cmp).GetComponent<PrimaryElement>().Mass;
      float mass2 = smi.master.GetComponent<PrimaryElement>().Mass;
      float num = (float) (((double) mass2 - (double) mass1) * (double) smi.timeAlive + (double) mass1 * (double) timeAlive) / mass2;
      smi.timeAlive = num;
      string animToPlay = smi.GetAnimToPlay();
      if (smi.GetComponent<KBatchedAnimController>().GetCurrentAnim().name != animToPlay)
        smi.Play(animToPlay);
      if ((double) smi.timeAlive < 600.0)
        return;
      smi.GoTo((StateMachine.BaseState) this.decayed);
    }));
    this.decayed.Enter((StateMachine<NuclearWaste, NuclearWaste.Instance, IStateMachineTarget, NuclearWaste.Def>.State.Callback) (smi =>
    {
      smi.GetComponent<Dumpable>().Dump();
      Util.KDestroyGameObject(smi.master.gameObject);
    }));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<NuclearWaste, NuclearWaste.Instance, IStateMachineTarget, NuclearWaste.Def>.GameInstance
  {
    [Serialize]
    public float timeAlive;
    private float percentageRemaining;

    public Instance(IStateMachineTarget master, NuclearWaste.Def def)
      : base(master, def)
    {
    }

    public string GetAnimToPlay()
    {
      this.percentageRemaining = (float) (1.0 - (double) this.smi.timeAlive / 600.0);
      if ((double) this.percentageRemaining <= 0.33000001311302185)
        return "idle1";
      return (double) this.percentageRemaining <= 0.6600000262260437 ? "idle2" : "idle3";
    }
  }
}
