// Decompiled with JetBrains decompiler
// Type: HappinessMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;

public class HappinessMonitor : 
  GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>
{
  private GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State satisfied;
  private HappinessMonitor.HappyState happy;
  private HappinessMonitor.UnhappyState unhappy;
  private Effect happyWildEffect;
  private Effect happyTameEffect;
  private Effect unhappyWildEffect;
  private Effect unhappyTameEffect;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.Transition((GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State) this.happy, new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsHappy), (UpdateRate) 6).Transition((GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State) this.unhappy, GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Not(new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsHappy)), (UpdateRate) 6);
    this.happy.DefaultState(this.happy.wild).Transition(this.satisfied, GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Not(new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsHappy)), (UpdateRate) 6);
    this.happy.wild.ToggleEffect((Func<HappinessMonitor.Instance, Effect>) (smi => this.happyWildEffect)).TagTransition(GameTags.Creatures.Wild, this.happy.tame, true);
    this.happy.tame.ToggleEffect((Func<HappinessMonitor.Instance, Effect>) (smi => this.happyTameEffect)).TagTransition(GameTags.Creatures.Wild, this.happy.wild);
    this.unhappy.DefaultState(this.unhappy.wild).Transition(this.satisfied, new StateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.Transition.ConditionCallback(HappinessMonitor.IsHappy), (UpdateRate) 6).ToggleTag(GameTags.Creatures.Unhappy);
    this.unhappy.wild.ToggleEffect((Func<HappinessMonitor.Instance, Effect>) (smi => this.unhappyWildEffect)).TagTransition(GameTags.Creatures.Wild, this.unhappy.tame, true);
    this.unhappy.tame.ToggleEffect((Func<HappinessMonitor.Instance, Effect>) (smi => this.unhappyTameEffect)).TagTransition(GameTags.Creatures.Wild, this.unhappy.wild);
    this.happyWildEffect = new Effect("Happy", (string) CREATURES.MODIFIERS.HAPPY.NAME, (string) CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0.0f, true, false, false);
    this.happyTameEffect = new Effect("Happy", (string) CREATURES.MODIFIERS.HAPPY.NAME, (string) CREATURES.MODIFIERS.HAPPY.TOOLTIP, 0.0f, true, false, false);
    this.unhappyWildEffect = new Effect("Unhappy", (string) CREATURES.MODIFIERS.UNHAPPY.NAME, (string) CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0.0f, true, false, true);
    this.unhappyTameEffect = new Effect("Unhappy", (string) CREATURES.MODIFIERS.UNHAPPY.NAME, (string) CREATURES.MODIFIERS.UNHAPPY.TOOLTIP, 0.0f, true, false, true);
    this.happyTameEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, 9f, (string) CREATURES.MODIFIERS.HAPPY.NAME, true));
    this.unhappyWildEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -15f, (string) CREATURES.MODIFIERS.UNHAPPY.NAME));
    this.unhappyTameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -80f, (string) CREATURES.MODIFIERS.UNHAPPY.NAME));
  }

  private static bool IsHappy(HappinessMonitor.Instance smi) => (double) smi.happiness.GetTotalValue() >= (double) smi.def.threshold;

  public class Def : StateMachine.BaseDef
  {
    public float threshold;
  }

  public class UnhappyState : 
    GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State
  {
    public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State wild;
    public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State tame;
  }

  public class HappyState : 
    GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State
  {
    public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State wild;
    public GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.State tame;
  }

  public new class Instance : 
    GameStateMachine<HappinessMonitor, HappinessMonitor.Instance, IStateMachineTarget, HappinessMonitor.Def>.GameInstance
  {
    public AttributeInstance happiness;

    public Instance(IStateMachineTarget master, HappinessMonitor.Def def)
      : base(master, def)
    {
      this.happiness = this.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Happiness);
    }
  }
}
