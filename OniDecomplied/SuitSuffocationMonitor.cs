// Decompiled with JetBrains decompiler
// Type: SuitSuffocationMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;

public class SuitSuffocationMonitor : 
  GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance>
{
  public SuitSuffocationMonitor.SatisfiedState satisfied;
  public SuitSuffocationMonitor.NoOxygenState nooxygen;
  public GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State death;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.DefaultState(this.satisfied.normal).ToggleAttributeModifier("Breathing", (Func<SuitSuffocationMonitor.Instance, AttributeModifier>) (smi => smi.breathing)).Transition((GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State) this.nooxygen, (StateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsTankEmpty()));
    this.satisfied.normal.Transition(this.satisfied.low, (StateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.suitTank.NeedsRecharging()));
    this.satisfied.low.DoNothing();
    this.nooxygen.ToggleExpression(Db.Get().Expressions.Suffocate).ToggleAttributeModifier("Holding Breath", (Func<SuitSuffocationMonitor.Instance, AttributeModifier>) (smi => smi.holdingbreath)).ToggleTag(GameTags.NoOxygen).DefaultState(this.nooxygen.holdingbreath);
    this.nooxygen.holdingbreath.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.HoldingBreath).Transition(this.nooxygen.suffocating, (StateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsSuffocating()));
    this.nooxygen.suffocating.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.Suffocating).Transition(this.death, (StateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.HasSuffocated()));
    this.death.Enter("SuffocationDeath", (StateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.Kill()));
  }

  public class NoOxygenState : 
    GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State holdingbreath;
    public GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State suffocating;
  }

  public class SatisfiedState : 
    GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State normal;
    public GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.State low;
  }

  public new class Instance : 
    GameStateMachine<SuitSuffocationMonitor, SuitSuffocationMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    private AmountInstance breath;
    public AttributeModifier breathing;
    public AttributeModifier holdingbreath;
    private OxygenBreather masterOxygenBreather;

    public SuitTank suitTank { get; private set; }

    public Instance(IStateMachineTarget master, SuitTank suit_tank)
      : base(master)
    {
      this.breath = Db.Get().Amounts.Breath.Lookup(master.gameObject);
      Klei.AI.Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
      float num = 0.909090936f;
      this.breathing = new AttributeModifier(deltaAttribute.Id, num, (string) DUPLICANTS.MODIFIERS.BREATHING.NAME);
      this.holdingbreath = new AttributeModifier(deltaAttribute.Id, -num, (string) DUPLICANTS.MODIFIERS.HOLDINGBREATH.NAME);
      this.suitTank = suit_tank;
    }

    public bool IsTankEmpty() => this.suitTank.IsEmpty();

    public bool HasSuffocated() => (double) this.breath.value <= 0.0;

    public bool IsSuffocating() => (double) this.breath.value <= 45.454547882080078;

    public void Kill() => this.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Suffocation);
  }
}
