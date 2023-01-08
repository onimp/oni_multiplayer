// Decompiled with JetBrains decompiler
// Type: SighChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SighChore : Chore<SighChore.StatesInstance>
{
  public SighChore(IStateMachineTarget target)
    : base(Db.Get().ChoreTypes.Sigh, target, target.GetComponent<ChoreProvider>(), false)
  {
    this.smi = new SighChore.StatesInstance(this, target.gameObject);
  }

  public class StatesInstance : 
    GameStateMachine<SighChore.States, SighChore.StatesInstance, SighChore, object>.GameInstance
  {
    public StatesInstance(SighChore master, GameObject sigher)
      : base(master)
    {
      this.sm.sigher.Set(sigher, this.smi, false);
    }
  }

  public class States : GameStateMachine<SighChore.States, SighChore.StatesInstance, SighChore>
  {
    public StateMachine<SighChore.States, SighChore.StatesInstance, SighChore, object>.TargetParameter sigher;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.root;
      this.Target(this.sigher);
      this.root.PlayAnim("emote_depressed").OnAnimQueueComplete((GameStateMachine<SighChore.States, SighChore.StatesInstance, SighChore, object>.State) null);
    }
  }
}
