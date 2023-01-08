// Decompiled with JetBrains decompiler
// Type: StarryEyed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class StarryEyed : StateMachineComponent<StarryEyed.StatesInstance>
{
  protected virtual void OnSpawn() => this.smi.StartSM();

  public class StatesInstance : 
    GameStateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.GameInstance
  {
    public StatesInstance(StarryEyed master)
      : base(master)
    {
    }

    public bool IsInSpace()
    {
      WorldContainer myWorld = this.GetMyWorld();
      if (!Object.op_Implicit((Object) myWorld))
        return false;
      int parentWorldId = myWorld.ParentWorldId;
      int id = myWorld.id;
      return Object.op_Implicit((Object) ((Component) myWorld).GetComponent<Clustercraft>()) && parentWorldId == id;
    }
  }

  public class States : GameStateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed>
  {
    public GameStateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.State idle;
    public GameStateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.State inSpace;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.root.Enter((StateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.State.Callback) (smi =>
      {
        if (!smi.IsInSpace())
          return;
        smi.GoTo((StateMachine.BaseState) this.inSpace);
      }));
      this.idle.EventTransition(GameHashes.MinionMigration, (Func<StarryEyed.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.inSpace, (StateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.Transition.ConditionCallback) (smi => smi.IsInSpace()));
      this.inSpace.EventTransition(GameHashes.MinionMigration, (Func<StarryEyed.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.idle, (StateMachine<StarryEyed.States, StarryEyed.StatesInstance, StarryEyed, object>.Transition.ConditionCallback) (smi => !smi.IsInSpace())).ToggleEffect(nameof (StarryEyed));
    }
  }
}
