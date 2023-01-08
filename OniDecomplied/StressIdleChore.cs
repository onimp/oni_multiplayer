// Decompiled with JetBrains decompiler
// Type: StressIdleChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class StressIdleChore : Chore<StressIdleChore.StatesInstance>
{
  public StressIdleChore(IStateMachineTarget target)
    : base(Db.Get().ChoreTypes.StressIdle, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.personalNeeds)
  {
    this.smi = new StressIdleChore.StatesInstance(this, target.gameObject);
  }

  public class StatesInstance : 
    GameStateMachine<StressIdleChore.States, StressIdleChore.StatesInstance, StressIdleChore, object>.GameInstance
  {
    public StatesInstance(StressIdleChore master, GameObject idler)
      : base(master)
    {
      this.sm.idler.Set(idler, this.smi, false);
    }
  }

  public class States : 
    GameStateMachine<StressIdleChore.States, StressIdleChore.StatesInstance, StressIdleChore>
  {
    public StateMachine<StressIdleChore.States, StressIdleChore.StatesInstance, StressIdleChore, object>.TargetParameter idler;
    public GameStateMachine<StressIdleChore.States, StressIdleChore.StatesInstance, StressIdleChore, object>.State idle;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.Target(this.idler);
      this.idle.PlayAnim("idle_default", (KAnim.PlayMode) 0);
    }
  }
}
