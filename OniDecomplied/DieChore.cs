// Decompiled with JetBrains decompiler
// Type: DieChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class DieChore : Chore<DieChore.StatesInstance>
{
  public DieChore(IStateMachineTarget master, Death death)
    : base(Db.Get().ChoreTypes.Die, master, master.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.showAvailabilityInHoverText = false;
    this.smi = new DieChore.StatesInstance(this, death);
  }

  public class StatesInstance : 
    GameStateMachine<DieChore.States, DieChore.StatesInstance, DieChore, object>.GameInstance
  {
    public StatesInstance(DieChore master, Death death)
      : base(master)
    {
      this.sm.death.Set(death, this.smi);
    }

    public void PlayPreAnim()
    {
      string preAnim = this.sm.death.Get(this.smi).preAnim;
      this.GetComponent<KAnimControllerBase>().Play(HashedString.op_Implicit(preAnim));
    }
  }

  public class States : GameStateMachine<DieChore.States, DieChore.StatesInstance, DieChore>
  {
    public GameStateMachine<DieChore.States, DieChore.StatesInstance, DieChore, object>.State dying;
    public GameStateMachine<DieChore.States, DieChore.StatesInstance, DieChore, object>.State dead;
    public StateMachine<DieChore.States, DieChore.StatesInstance, DieChore, object>.ResourceParameter<Death> death;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.dying;
      this.dying.OnAnimQueueComplete(this.dead).Enter("PlayAnim", (StateMachine<DieChore.States, DieChore.StatesInstance, DieChore, object>.State.Callback) (smi => smi.PlayPreAnim()));
      this.dead.ReturnSuccess();
    }
  }
}
