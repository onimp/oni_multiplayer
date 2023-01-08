// Decompiled with JetBrains decompiler
// Type: BeeHappinessMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;

public class BeeHappinessMonitor : 
  GameStateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>
{
  private GameStateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>.State satisfied;
  private GameStateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>.State happy;
  private GameStateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>.State unhappy;
  private Effect happyEffect;
  private Effect unhappyEffect;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.Transition(this.happy, new StateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>.Transition.ConditionCallback(BeeHappinessMonitor.IsHappy), (UpdateRate) 6).Transition(this.unhappy, GameStateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>.Not(new StateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>.Transition.ConditionCallback(BeeHappinessMonitor.IsHappy)), (UpdateRate) 6);
    this.happy.ToggleEffect((Func<BeeHappinessMonitor.Instance, Effect>) (smi => this.happyEffect)).TriggerOnEnter(GameHashes.Happy).Transition(this.satisfied, GameStateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>.Not(new StateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>.Transition.ConditionCallback(BeeHappinessMonitor.IsHappy)), (UpdateRate) 6);
    this.unhappy.TriggerOnEnter(GameHashes.Unhappy).Transition(this.satisfied, new StateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>.Transition.ConditionCallback(BeeHappinessMonitor.IsHappy), (UpdateRate) 6).ToggleEffect((Func<BeeHappinessMonitor.Instance, Effect>) (smi => this.unhappyEffect));
    this.happyEffect = new Effect("Happy", (string) CREATURES.MODIFIERS.HAPPY.NAME, (string) CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0.0f, true, false, false);
    this.unhappyEffect = new Effect("Unhappy", (string) CREATURES.MODIFIERS.UNHAPPY.NAME, (string) CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0.0f, true, false, true);
  }

  private static bool IsHappy(BeeHappinessMonitor.Instance smi) => (double) smi.happiness.GetTotalValue() >= (double) smi.def.threshold;

  public class Def : StateMachine.BaseDef
  {
    public float threshold;
  }

  public new class Instance : 
    GameStateMachine<BeeHappinessMonitor, BeeHappinessMonitor.Instance, IStateMachineTarget, BeeHappinessMonitor.Def>.GameInstance
  {
    public AttributeInstance happiness;

    public Instance(IStateMachineTarget master, BeeHappinessMonitor.Def def)
      : base(master, def)
    {
      this.happiness = this.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Happiness);
    }
  }
}
