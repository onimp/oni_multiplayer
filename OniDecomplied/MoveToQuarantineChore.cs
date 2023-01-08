// Decompiled with JetBrains decompiler
// Type: MoveToQuarantineChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class MoveToQuarantineChore : Chore<MoveToQuarantineChore.StatesInstance>
{
  public MoveToQuarantineChore(IStateMachineTarget target, KMonoBehaviour quarantine_area)
    : base(Db.Get().ChoreTypes.MoveToQuarantine, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new MoveToQuarantineChore.StatesInstance(this, target.gameObject);
    this.smi.sm.locator.Set(((Component) quarantine_area).gameObject, this.smi, false);
  }

  public class StatesInstance : 
    GameStateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore, object>.GameInstance
  {
    public StatesInstance(MoveToQuarantineChore master, GameObject quarantined)
      : base(master)
    {
      this.sm.quarantined.Set(quarantined, this.smi, false);
    }
  }

  public class States : 
    GameStateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore>
  {
    public StateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore, object>.TargetParameter locator;
    public StateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore, object>.TargetParameter quarantined;
    public GameStateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore, object>.ApproachSubState<IApproachable> approach;
    public GameStateMachine<MoveToQuarantineChore.States, MoveToQuarantineChore.StatesInstance, MoveToQuarantineChore, object>.State success;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.approach;
      this.approach.InitializeStates(this.quarantined, this.locator, this.success);
      this.success.ReturnSuccess();
    }
  }
}
