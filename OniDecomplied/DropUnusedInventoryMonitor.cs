// Decompiled with JetBrains decompiler
// Type: DropUnusedInventoryMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class DropUnusedInventoryMonitor : 
  GameStateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance>
{
  public GameStateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public GameStateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance, IStateMachineTarget, object>.State hasinventory;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.EventTransition(GameHashes.OnStorageChange, this.hasinventory, (StateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Storage>().Count > 0));
    this.hasinventory.EventTransition(GameHashes.OnStorageChange, this.hasinventory, (StateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Storage>().Count == 0)).ToggleChore((Func<DropUnusedInventoryMonitor.Instance, Chore>) (smi => (Chore) new DropUnusedInventoryChore(Db.Get().ChoreTypes.DropUnusedInventory, smi.master)), this.satisfied);
  }

  public new class Instance : 
    GameStateMachine<DropUnusedInventoryMonitor, DropUnusedInventoryMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }
  }
}
