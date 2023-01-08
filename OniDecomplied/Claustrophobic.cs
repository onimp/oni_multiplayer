// Decompiled with JetBrains decompiler
// Type: Claustrophobic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Claustrophobic : StateMachineComponent<Claustrophobic.StatesInstance>
{
  protected virtual void OnSpawn() => this.smi.StartSM();

  protected bool IsUncomfortable()
  {
    int num1 = 4;
    int cell = Grid.PosToCell(((Component) this).gameObject);
    for (int y = 0; y < num1 - 1; ++y)
    {
      int num2 = Grid.OffsetCell(cell, 0, y);
      if (Grid.IsValidCell(num2) && Grid.Solid[num2] || Grid.IsValidCell(Grid.CellRight(cell)) && Grid.IsValidCell(Grid.CellLeft(cell)) && Grid.Solid[Grid.CellRight(cell)] && Grid.Solid[Grid.CellLeft(cell)])
        return true;
    }
    return false;
  }

  public class StatesInstance : 
    GameStateMachine<Claustrophobic.States, Claustrophobic.StatesInstance, Claustrophobic, object>.GameInstance
  {
    public StatesInstance(Claustrophobic master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<Claustrophobic.States, Claustrophobic.StatesInstance, Claustrophobic>
  {
    public GameStateMachine<Claustrophobic.States, Claustrophobic.StatesInstance, Claustrophobic, object>.State satisfied;
    public GameStateMachine<Claustrophobic.States, Claustrophobic.StatesInstance, Claustrophobic, object>.State suffering;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.satisfied;
      this.root.Update("ClaustrophobicCheck", (Action<Claustrophobic.StatesInstance, float>) ((smi, dt) =>
      {
        if (smi.master.IsUncomfortable())
          smi.GoTo((StateMachine.BaseState) this.suffering);
        else
          smi.GoTo((StateMachine.BaseState) this.satisfied);
      }), (UpdateRate) 6);
      this.suffering.AddEffect(nameof (Claustrophobic)).ToggleExpression(Db.Get().Expressions.Uncomfortable);
      this.satisfied.DoNothing();
    }
  }
}
