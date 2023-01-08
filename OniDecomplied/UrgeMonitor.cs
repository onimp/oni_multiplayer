// Decompiled with JetBrains decompiler
// Type: UrgeMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;

public class UrgeMonitor : GameStateMachine<UrgeMonitor, UrgeMonitor.Instance>
{
  public GameStateMachine<UrgeMonitor, UrgeMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public GameStateMachine<UrgeMonitor, UrgeMonitor.Instance, IStateMachineTarget, object>.State hasurge;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.Transition(this.hasurge, (StateMachine<UrgeMonitor, UrgeMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.HasUrge()));
    this.hasurge.Transition(this.satisfied, (StateMachine<UrgeMonitor, UrgeMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.HasUrge())).ToggleUrge((Func<UrgeMonitor.Instance, Urge>) (smi => smi.GetUrge()));
  }

  public new class Instance : 
    GameStateMachine<UrgeMonitor, UrgeMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    private AmountInstance amountInstance;
    private Urge urge;
    private ScheduleBlockType scheduleBlock;
    private Schedulable schedulable;
    private float inScheduleThreshold;
    private float outOfScheduleThreshold;
    private bool isThresholdMinimum;

    public Instance(
      IStateMachineTarget master,
      Urge urge,
      Amount amount,
      ScheduleBlockType schedule_block,
      float in_schedule_threshold,
      float out_of_schedule_threshold,
      bool is_threshold_minimum)
      : base(master)
    {
      this.urge = urge;
      this.scheduleBlock = schedule_block;
      this.schedulable = this.GetComponent<Schedulable>();
      this.amountInstance = this.gameObject.GetAmounts().Get(amount);
      this.isThresholdMinimum = is_threshold_minimum;
      this.inScheduleThreshold = in_schedule_threshold;
      this.outOfScheduleThreshold = out_of_schedule_threshold;
    }

    private float GetThreshold() => this.schedulable.IsAllowed(this.scheduleBlock) ? this.inScheduleThreshold : this.outOfScheduleThreshold;

    public Urge GetUrge() => this.urge;

    public bool HasUrge() => this.isThresholdMinimum ? (double) this.amountInstance.value >= (double) this.GetThreshold() : (double) this.amountInstance.value <= (double) this.GetThreshold();
  }
}
