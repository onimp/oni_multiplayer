// Decompiled with JetBrains decompiler
// Type: TaskAvailabilityMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class TaskAvailabilityMonitor : 
  GameStateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance>
{
  public GameStateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public GameStateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance, IStateMachineTarget, object>.State unavailable;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.EventTransition(GameHashes.NewDay, (Func<TaskAvailabilityMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.unavailable, (StateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => GameClock.Instance.GetCycle() > 0));
    this.unavailable.Enter("RefreshStatusItem", (StateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.RefreshStatusItem())).EventHandler(GameHashes.ScheduleChanged, (StateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.RefreshStatusItem()));
  }

  public new class Instance : 
    GameStateMachine<TaskAvailabilityMonitor, TaskAvailabilityMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public void RefreshStatusItem() => this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Idle);
  }
}
