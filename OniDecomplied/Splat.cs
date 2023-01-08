// Decompiled with JetBrains decompiler
// Type: Splat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class Splat : GameStateMachine<Splat, Splat.StatesInstance>
{
  public GameStateMachine<Splat, Splat.StatesInstance, IStateMachineTarget, object>.State complete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleChore((Func<Splat.StatesInstance, Chore>) (smi => (Chore) new WorkChore<SplatWorkable>(Db.Get().ChoreTypes.Mop, smi.master)), this.complete);
    this.complete.Enter((StateMachine<Splat, Splat.StatesInstance, IStateMachineTarget, object>.State.Callback) (smi => Util.KDestroyGameObject(smi.master.gameObject)));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class StatesInstance : 
    GameStateMachine<Splat, Splat.StatesInstance, IStateMachineTarget, object>.GameInstance
  {
    public StatesInstance(IStateMachineTarget master, Splat.Def def)
      : base(master, (object) def)
    {
    }
  }
}
