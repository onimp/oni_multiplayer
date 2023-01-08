// Decompiled with JetBrains decompiler
// Type: StorageUnloadMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class StorageUnloadMonitor : 
  GameStateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>
{
  public StateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.ObjectParameter<Storage> internalStorage = new StateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.ObjectParameter<Storage>();
  public StateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.ObjectParameter<Storage> sweepLocker;
  public GameStateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.State notFull;
  public GameStateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.State full;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.notFull;
    this.notFull.Transition(this.full, new StateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.Transition.ConditionCallback(StorageUnloadMonitor.WantsToUnload));
    this.full.ToggleStatusItem(Db.Get().RobotStatusItems.DustBinFull, (Func<StorageUnloadMonitor.Instance, object>) (smi => (object) smi.gameObject)).ToggleBehaviour(GameTags.Robots.Behaviours.UnloadBehaviour, (StateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.Transition.ConditionCallback) (data => true)).Transition(this.notFull, GameStateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.Not(new StateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.Transition.ConditionCallback(StorageUnloadMonitor.WantsToUnload)), (UpdateRate) 6).Enter((StateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.State.Callback) (smi =>
    {
      if ((double) smi.master.gameObject.GetComponents<Storage>()[1].RemainingCapacity() > 0.0)
        return;
      smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim(HashedString.op_Implicit("react_full"));
    }));
  }

  public static bool WantsToUnload(StorageUnloadMonitor.Instance smi)
  {
    Storage cmp = smi.sm.sweepLocker.Get(smi);
    return !Object.op_Equality((Object) cmp, (Object) null) && !Object.op_Equality((Object) smi.sm.internalStorage.Get(smi), (Object) null) && !smi.HasTag(GameTags.Robots.Behaviours.RechargeBehaviour) && (smi.sm.internalStorage.Get(smi).IsFull() || Object.op_Inequality((Object) cmp, (Object) null) && !smi.sm.internalStorage.Get(smi).IsEmpty() && Grid.PosToCell((KMonoBehaviour) cmp) == Grid.PosToCell((StateMachine.Instance) smi));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<StorageUnloadMonitor, StorageUnloadMonitor.Instance, IStateMachineTarget, StorageUnloadMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, StorageUnloadMonitor.Def def)
      : base(master, def)
    {
    }
  }
}
