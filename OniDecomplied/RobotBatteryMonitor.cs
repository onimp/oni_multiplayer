// Decompiled with JetBrains decompiler
// Type: RobotBatteryMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;

public class RobotBatteryMonitor : 
  GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>
{
  public StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.ObjectParameter<Storage> internalStorage = new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.ObjectParameter<Storage>();
  public RobotBatteryMonitor.NeedsRechargeStates needsRechargeStates;
  public RobotBatteryMonitor.DrainingStates drainingStates;
  public GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State deadBattery;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.drainingStates;
    this.drainingStates.DefaultState(this.drainingStates.highBattery).Transition(this.deadBattery, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.BatteryDead)).Transition((GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State) this.needsRechargeStates, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.NeedsRecharge));
    this.drainingStates.highBattery.Transition(this.drainingStates.lowBattery, GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Not(new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.ChargeDecent)));
    this.drainingStates.lowBattery.Transition(this.drainingStates.highBattery, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.ChargeDecent)).ToggleStatusItem((Func<RobotBatteryMonitor.Instance, StatusItem>) (smi => !smi.def.canCharge ? Db.Get().RobotStatusItems.LowBatteryNoCharge : Db.Get().RobotStatusItems.LowBattery), (Func<RobotBatteryMonitor.Instance, object>) (smi => (object) smi.gameObject));
    this.needsRechargeStates.DefaultState(this.needsRechargeStates.lowBattery).Transition(this.deadBattery, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.BatteryDead)).Transition((GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State) this.drainingStates, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.ChargeComplete)).ToggleBehaviour(GameTags.Robots.Behaviours.RechargeBehaviour, (StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback) (smi => smi.def.canCharge));
    this.needsRechargeStates.lowBattery.ToggleStatusItem((Func<RobotBatteryMonitor.Instance, StatusItem>) (smi => !smi.def.canCharge ? Db.Get().RobotStatusItems.LowBatteryNoCharge : Db.Get().RobotStatusItems.LowBattery), (Func<RobotBatteryMonitor.Instance, object>) (smi => (object) smi.gameObject)).Transition(this.needsRechargeStates.mediumBattery, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.ChargeDecent));
    this.needsRechargeStates.mediumBattery.Transition(this.needsRechargeStates.lowBattery, GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Not(new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.ChargeDecent))).Transition(this.needsRechargeStates.trickleCharge, new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.ChargeFull));
    this.needsRechargeStates.trickleCharge.Transition(this.needsRechargeStates.mediumBattery, GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Not(new StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.Transition.ConditionCallback(RobotBatteryMonitor.ChargeFull)));
    this.deadBattery.ToggleStatusItem(Db.Get().RobotStatusItems.DeadBattery, (Func<RobotBatteryMonitor.Instance, object>) (smi => (object) smi.gameObject)).Enter((StateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State.Callback) (smi =>
    {
      if (smi.GetSMI<DeathMonitor.Instance>() == null)
        return;
      smi.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.DeadBattery);
    }));
  }

  public static bool NeedsRecharge(RobotBatteryMonitor.Instance smi) => (double) smi.amountInstance.value <= 0.0 || GameClock.Instance.IsNighttime();

  public static bool ChargeDecent(RobotBatteryMonitor.Instance smi) => (double) smi.amountInstance.value >= (double) smi.amountInstance.GetMax() * (double) smi.def.lowBatteryWarningPercent;

  public static bool ChargeFull(RobotBatteryMonitor.Instance smi) => (double) smi.amountInstance.value >= (double) smi.amountInstance.GetMax();

  public static bool ChargeComplete(RobotBatteryMonitor.Instance smi) => (double) smi.amountInstance.value >= (double) smi.amountInstance.GetMax() && !GameClock.Instance.IsNighttime();

  public static bool BatteryDead(RobotBatteryMonitor.Instance smi) => !smi.def.canCharge && (double) smi.amountInstance.value == 0.0;

  public class Def : StateMachine.BaseDef
  {
    public string batteryAmountId;
    public float lowBatteryWarningPercent;
    public bool canCharge;
  }

  public class DrainingStates : 
    GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State
  {
    public GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State highBattery;
    public GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State lowBattery;
  }

  public class NeedsRechargeStates : 
    GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State
  {
    public GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State lowBattery;
    public GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State mediumBattery;
    public GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.State trickleCharge;
  }

  public new class Instance : 
    GameStateMachine<RobotBatteryMonitor, RobotBatteryMonitor.Instance, IStateMachineTarget, RobotBatteryMonitor.Def>.GameInstance
  {
    public AmountInstance amountInstance;

    public Instance(IStateMachineTarget master, RobotBatteryMonitor.Def def)
      : base(master, def)
    {
      this.amountInstance = Db.Get().Amounts.Get(def.batteryAmountId).Lookup(this.gameObject);
      double num = (double) this.amountInstance.SetValue(this.amountInstance.GetMax());
    }
  }
}
