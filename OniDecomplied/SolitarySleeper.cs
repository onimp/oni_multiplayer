// Decompiled with JetBrains decompiler
// Type: SolitarySleeper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class SolitarySleeper : StateMachineComponent<SolitarySleeper.StatesInstance>
{
  protected virtual void OnSpawn() => this.smi.StartSM();

  protected bool IsUncomfortable()
  {
    if (!((Component) this).gameObject.GetSMI<StaminaMonitor.Instance>().IsSleeping())
      return false;
    int num = 5;
    bool flag1 = true;
    bool flag2 = true;
    int cell = Grid.PosToCell(((Component) this).gameObject);
    for (int x = 1; x < num; ++x)
    {
      int i1 = Grid.OffsetCell(cell, x, 0);
      int i2 = Grid.OffsetCell(cell, -x, 0);
      if (Grid.Solid[i2])
        flag1 = false;
      if (Grid.Solid[i1])
        flag2 = false;
      foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
      {
        if (flag1 && Grid.PosToCell(((Component) minionIdentity).gameObject) == i2 || flag2 && Grid.PosToCell(((Component) minionIdentity).gameObject) == i1)
          return true;
      }
    }
    return false;
  }

  public class StatesInstance : 
    GameStateMachine<SolitarySleeper.States, SolitarySleeper.StatesInstance, SolitarySleeper, object>.GameInstance
  {
    public StatesInstance(SolitarySleeper master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<SolitarySleeper.States, SolitarySleeper.StatesInstance, SolitarySleeper>
  {
    public GameStateMachine<SolitarySleeper.States, SolitarySleeper.StatesInstance, SolitarySleeper, object>.State satisfied;
    public GameStateMachine<SolitarySleeper.States, SolitarySleeper.StatesInstance, SolitarySleeper, object>.State suffering;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.satisfied;
      this.root.TagTransition(GameTags.Dead, (GameStateMachine<SolitarySleeper.States, SolitarySleeper.StatesInstance, SolitarySleeper, object>.State) null).EventTransition(GameHashes.NewDay, this.satisfied).Update("SolitarySleeperCheck", (Action<SolitarySleeper.StatesInstance, float>) ((smi, dt) =>
      {
        if (smi.master.IsUncomfortable())
        {
          if (smi.GetCurrentState() == this.suffering)
            return;
          smi.GoTo((StateMachine.BaseState) this.suffering);
        }
        else
        {
          if (smi.GetCurrentState() == this.satisfied)
            return;
          smi.GoTo((StateMachine.BaseState) this.satisfied);
        }
      }), (UpdateRate) 7);
      this.suffering.AddEffect("PeopleTooCloseWhileSleeping").ToggleExpression(Db.Get().Expressions.Uncomfortable).Update("PeopleTooCloseSleepFail", (Action<SolitarySleeper.StatesInstance, float>) ((smi, dt) => EventExtensions.Trigger(((Component) smi.master).gameObject, 1338475637, (object) this)), (UpdateRate) 6);
      this.satisfied.DoNothing();
    }
  }
}
