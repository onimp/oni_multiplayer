// Decompiled with JetBrains decompiler
// Type: Loner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Loner : StateMachineComponent<Loner.StatesInstance>
{
  protected virtual void OnSpawn() => this.smi.StartSM();

  public class StatesInstance : 
    GameStateMachine<Loner.States, Loner.StatesInstance, Loner, object>.GameInstance
  {
    public StatesInstance(Loner master)
      : base(master)
    {
    }

    public bool IsAlone()
    {
      WorldContainer myWorld = this.GetMyWorld();
      if (!Object.op_Implicit((Object) myWorld))
        return false;
      int parentWorldId = myWorld.ParentWorldId;
      int id = myWorld.id;
      MinionIdentity component = this.GetComponent<MinionIdentity>();
      foreach (MinionIdentity liveMinionIdentity in Components.LiveMinionIdentities)
      {
        if (Object.op_Inequality((Object) component, (Object) liveMinionIdentity))
        {
          int myWorldId = liveMinionIdentity.GetMyWorldId();
          if (id == myWorldId || parentWorldId == myWorldId)
            return false;
        }
      }
      return true;
    }
  }

  public class States : GameStateMachine<Loner.States, Loner.StatesInstance, Loner>
  {
    public GameStateMachine<Loner.States, Loner.StatesInstance, Loner, object>.State idle;
    public GameStateMachine<Loner.States, Loner.StatesInstance, Loner, object>.State alone;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.root.Enter((StateMachine<Loner.States, Loner.StatesInstance, Loner, object>.State.Callback) (smi =>
      {
        if (!smi.IsAlone())
          return;
        smi.GoTo((StateMachine.BaseState) this.alone);
      }));
      this.idle.EventTransition(GameHashes.MinionMigration, (Func<Loner.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.alone, (StateMachine<Loner.States, Loner.StatesInstance, Loner, object>.Transition.ConditionCallback) (smi => smi.IsAlone())).EventTransition(GameHashes.MinionDelta, (Func<Loner.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.alone, (StateMachine<Loner.States, Loner.StatesInstance, Loner, object>.Transition.ConditionCallback) (smi => smi.IsAlone()));
      this.alone.EventTransition(GameHashes.MinionMigration, (Func<Loner.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.idle, (StateMachine<Loner.States, Loner.StatesInstance, Loner, object>.Transition.ConditionCallback) (smi => !smi.IsAlone())).EventTransition(GameHashes.MinionDelta, (Func<Loner.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.idle, (StateMachine<Loner.States, Loner.StatesInstance, Loner, object>.Transition.ConditionCallback) (smi => !smi.IsAlone())).ToggleEffect(nameof (Loner));
    }
  }
}
