// Decompiled with JetBrains decompiler
// Type: TakeOffHatChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class TakeOffHatChore : Chore<TakeOffHatChore.StatesInstance>
{
  public TakeOffHatChore(IStateMachineTarget target, ChoreType chore_type)
    : base(chore_type, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new TakeOffHatChore.StatesInstance(this, target.gameObject);
  }

  public class StatesInstance : 
    GameStateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.GameInstance
  {
    public StatesInstance(TakeOffHatChore master, GameObject duplicant)
      : base(master)
    {
      this.sm.duplicant.Set(duplicant, this.smi, false);
    }
  }

  public class States : 
    GameStateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore>
  {
    public StateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.TargetParameter duplicant;
    public GameStateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.State remove_hat_pre;
    public GameStateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.State remove_hat;
    public GameStateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.State complete;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.remove_hat_pre;
      this.Target(this.duplicant);
      this.remove_hat_pre.Enter((StateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.State.Callback) (smi =>
      {
        if (this.duplicant.Get(smi).GetComponent<MinionResume>().CurrentHat != null)
          smi.GoTo((StateMachine.BaseState) this.remove_hat);
        else
          smi.GoTo((StateMachine.BaseState) this.complete);
      }));
      this.remove_hat.ToggleAnims("anim_hat_kanim").PlayAnim("hat_off").OnAnimQueueComplete(this.complete);
      this.complete.Enter((StateMachine<TakeOffHatChore.States, TakeOffHatChore.StatesInstance, TakeOffHatChore, object>.State.Callback) (smi => smi.master.GetComponent<MinionResume>().RemoveHat())).ReturnSuccess();
    }
  }
}
