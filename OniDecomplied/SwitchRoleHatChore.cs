// Decompiled with JetBrains decompiler
// Type: SwitchRoleHatChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SwitchRoleHatChore : Chore<SwitchRoleHatChore.StatesInstance>
{
  public SwitchRoleHatChore(IStateMachineTarget target, ChoreType chore_type)
    : base(chore_type, target, target.GetComponent<ChoreProvider>(), false)
  {
    this.smi = new SwitchRoleHatChore.StatesInstance(this, target.gameObject);
  }

  public class StatesInstance : 
    GameStateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.GameInstance
  {
    public StatesInstance(SwitchRoleHatChore master, GameObject duplicant)
      : base(master)
    {
      this.sm.duplicant.Set(duplicant, this.smi, false);
    }
  }

  public class States : 
    GameStateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore>
  {
    public StateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.TargetParameter duplicant;
    public GameStateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.State remove_hat;
    public GameStateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.State start;
    public GameStateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.State delay;
    public GameStateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.State delay_pst;
    public GameStateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.State applyHat_pre;
    public GameStateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.State applyHat;
    public GameStateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.State complete;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.start;
      this.Target(this.duplicant);
      this.start.Enter((StateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.State.Callback) (smi =>
      {
        if (this.duplicant.Get(smi).GetComponent<MinionResume>().CurrentHat == null)
          smi.GoTo((StateMachine.BaseState) this.delay);
        else
          smi.GoTo((StateMachine.BaseState) this.remove_hat);
      }));
      this.remove_hat.ToggleAnims("anim_hat_kanim").PlayAnim("hat_off").OnAnimQueueComplete(this.delay);
      this.delay.ToggleThought(Db.Get().Thoughts.NewRole).ToggleExpression(Db.Get().Expressions.Happy).ToggleAnims("anim_selfish_kanim").QueueAnim("working_pre").QueueAnim("working_loop").QueueAnim("working_pst").OnAnimQueueComplete(this.applyHat_pre);
      this.applyHat_pre.ToggleAnims("anim_hat_kanim").Enter((StateMachine<SwitchRoleHatChore.States, SwitchRoleHatChore.StatesInstance, SwitchRoleHatChore, object>.State.Callback) (smi => this.duplicant.Get(smi).GetComponent<MinionResume>().ApplyTargetHat())).PlayAnim("hat_first").OnAnimQueueComplete(this.applyHat);
      this.applyHat.ToggleAnims("anim_hat_kanim").PlayAnim("working_pst").OnAnimQueueComplete(this.complete);
      this.complete.ReturnSuccess();
    }
  }
}
