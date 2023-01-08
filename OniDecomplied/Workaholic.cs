// Decompiled with JetBrains decompiler
// Type: Workaholic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Workaholic : StateMachineComponent<Workaholic.StatesInstance>
{
  protected virtual void OnSpawn() => this.smi.StartSM();

  protected bool IsUncomfortable() => ((Component) this.smi.master).GetComponent<ChoreDriver>().GetCurrentChore() is IdleChore;

  public class StatesInstance : 
    GameStateMachine<Workaholic.States, Workaholic.StatesInstance, Workaholic, object>.GameInstance
  {
    public StatesInstance(Workaholic master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<Workaholic.States, Workaholic.StatesInstance, Workaholic>
  {
    public GameStateMachine<Workaholic.States, Workaholic.StatesInstance, Workaholic, object>.State satisfied;
    public GameStateMachine<Workaholic.States, Workaholic.StatesInstance, Workaholic, object>.State suffering;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.satisfied;
      this.root.Update("WorkaholicCheck", (Action<Workaholic.StatesInstance, float>) ((smi, dt) =>
      {
        if (smi.master.IsUncomfortable())
          smi.GoTo((StateMachine.BaseState) this.suffering);
        else
          smi.GoTo((StateMachine.BaseState) this.satisfied);
      }), (UpdateRate) 6);
      this.suffering.AddEffect("Restless").ToggleExpression(Db.Get().Expressions.Uncomfortable);
      this.satisfied.DoNothing();
    }
  }
}
