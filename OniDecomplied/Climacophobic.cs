// Decompiled with JetBrains decompiler
// Type: Climacophobic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Climacophobic : StateMachineComponent<Climacophobic.StatesInstance>
{
  protected virtual void OnSpawn() => this.smi.StartSM();

  protected bool IsUncomfortable()
  {
    int num1 = 5;
    int cell1 = Grid.PosToCell(((Component) this).gameObject);
    if (!this.isCellLadder(cell1))
      return false;
    int num2 = 1;
    bool flag1 = true;
    bool flag2 = true;
    for (int y = 1; y < num1; ++y)
    {
      int cell2 = Grid.OffsetCell(cell1, 0, y);
      int cell3 = Grid.OffsetCell(cell1, 0, -y);
      if (flag1 && this.isCellLadder(cell2))
        ++num2;
      else
        flag1 = false;
      if (flag2 && this.isCellLadder(cell3))
        ++num2;
      else
        flag2 = false;
    }
    return num2 >= num1;
  }

  private bool isCellLadder(int cell)
  {
    if (!Grid.IsValidCell(cell))
      return false;
    GameObject gameObject = Grid.Objects[cell, 1];
    return !Object.op_Equality((Object) gameObject, (Object) null) && !Object.op_Equality((Object) gameObject.GetComponent<Ladder>(), (Object) null);
  }

  public class StatesInstance : 
    GameStateMachine<Climacophobic.States, Climacophobic.StatesInstance, Climacophobic, object>.GameInstance
  {
    public StatesInstance(Climacophobic master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<Climacophobic.States, Climacophobic.StatesInstance, Climacophobic>
  {
    public GameStateMachine<Climacophobic.States, Climacophobic.StatesInstance, Climacophobic, object>.State satisfied;
    public GameStateMachine<Climacophobic.States, Climacophobic.StatesInstance, Climacophobic, object>.State suffering;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.satisfied;
      this.root.Update("ClimacophobicCheck", (Action<Climacophobic.StatesInstance, float>) ((smi, dt) =>
      {
        if (smi.master.IsUncomfortable())
          smi.GoTo((StateMachine.BaseState) this.suffering);
        else
          smi.GoTo((StateMachine.BaseState) this.satisfied);
      }), (UpdateRate) 6);
      this.suffering.AddEffect("Vertigo").ToggleExpression(Db.Get().Expressions.Uncomfortable);
      this.satisfied.DoNothing();
    }
  }
}
