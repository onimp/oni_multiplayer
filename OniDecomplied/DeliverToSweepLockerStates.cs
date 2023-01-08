// Decompiled with JetBrains decompiler
// Type: DeliverToSweepLockerStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class DeliverToSweepLockerStates : 
  GameStateMachine<DeliverToSweepLockerStates, DeliverToSweepLockerStates.Instance, IStateMachineTarget, DeliverToSweepLockerStates.Def>
{
  public GameStateMachine<DeliverToSweepLockerStates, DeliverToSweepLockerStates.Instance, IStateMachineTarget, DeliverToSweepLockerStates.Def>.State idle;
  public GameStateMachine<DeliverToSweepLockerStates, DeliverToSweepLockerStates.Instance, IStateMachineTarget, DeliverToSweepLockerStates.Def>.State movingToStorage;
  public GameStateMachine<DeliverToSweepLockerStates, DeliverToSweepLockerStates.Instance, IStateMachineTarget, DeliverToSweepLockerStates.Def>.State unloading;
  public GameStateMachine<DeliverToSweepLockerStates, DeliverToSweepLockerStates.Instance, IStateMachineTarget, DeliverToSweepLockerStates.Def>.State lockerFull;
  public GameStateMachine<DeliverToSweepLockerStates, DeliverToSweepLockerStates.Instance, IStateMachineTarget, DeliverToSweepLockerStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.movingToStorage;
    this.idle.ScheduleGoTo(1f, (StateMachine.BaseState) this.movingToStorage);
    this.movingToStorage.MoveTo((Func<DeliverToSweepLockerStates.Instance, int>) (smi => !Object.op_Equality((Object) this.GetSweepLocker(smi), (Object) null) ? Grid.PosToCell((KMonoBehaviour) this.GetSweepLocker(smi)) : Grid.InvalidCell), this.unloading, this.idle);
    this.unloading.Enter((StateMachine<DeliverToSweepLockerStates, DeliverToSweepLockerStates.Instance, IStateMachineTarget, DeliverToSweepLockerStates.Def>.State.Callback) (smi =>
    {
      Storage sweepLocker = this.GetSweepLocker(smi);
      if (Object.op_Equality((Object) sweepLocker, (Object) null))
      {
        smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
      }
      else
      {
        Storage component = smi.master.gameObject.GetComponents<Storage>()[1];
        float num1 = Mathf.Max(0.0f, Mathf.Min(component.MassStored(), sweepLocker.RemainingCapacity()));
        for (int index = component.items.Count - 1; index >= 0; --index)
        {
          GameObject gameObject = component.items[index];
          if (!Object.op_Equality((Object) gameObject, (Object) null))
          {
            float amount = Mathf.Min(gameObject.GetComponent<PrimaryElement>().Mass, num1);
            if ((double) amount != 0.0)
            {
              double num2 = (double) component.Transfer(sweepLocker, gameObject.GetComponent<KPrefabID>().PrefabTag, amount);
            }
            num1 -= amount;
            if ((double) num1 <= 0.0)
              break;
          }
        }
        smi.master.GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("dropoff"));
        smi.master.GetComponent<KBatchedAnimController>().FlipX = false;
        ((Component) sweepLocker).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("dropoff"));
        if ((double) component.MassStored() > 0.0)
          smi.ScheduleGoTo(2f, (StateMachine.BaseState) this.lockerFull);
        else
          smi.ScheduleGoTo(2f, (StateMachine.BaseState) this.behaviourcomplete);
      }
    }));
    this.lockerFull.PlayAnim("react_bored", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.movingToStorage);
    this.behaviourcomplete.BehaviourComplete(GameTags.Robots.Behaviours.UnloadBehaviour);
  }

  public Storage GetSweepLocker(DeliverToSweepLockerStates.Instance smi)
  {
    StorageUnloadMonitor.Instance smi1 = smi.master.gameObject.GetSMI<StorageUnloadMonitor.Instance>();
    return smi1?.sm.sweepLocker.Get(smi1);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<DeliverToSweepLockerStates, DeliverToSweepLockerStates.Instance, IStateMachineTarget, DeliverToSweepLockerStates.Def>.GameInstance
  {
    public Instance(
      Chore<DeliverToSweepLockerStates.Instance> chore,
      DeliverToSweepLockerStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Robots.Behaviours.UnloadBehaviour);
    }

    public override void StartSM()
    {
      base.StartSM();
      this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().RobotStatusItems.UnloadingStorage, (object) this.gameObject);
    }

    protected override void OnCleanUp()
    {
      base.OnCleanUp();
      this.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().RobotStatusItems.UnloadingStorage);
    }
  }
}
