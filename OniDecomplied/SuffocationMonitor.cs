// Decompiled with JetBrains decompiler
// Type: SuffocationMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using UnityEngine;

public class SuffocationMonitor : GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance>
{
  public SuffocationMonitor.SatisfiedState satisfied;
  public SuffocationMonitor.NoOxygenState nooxygen;
  public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State death;
  public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State dead;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.root.Update("CheckOverPressure", (System.Action<SuffocationMonitor.Instance, float>) ((smi, dt) => smi.CheckOverPressure())).TagTransition(GameTags.Dead, this.dead);
    this.satisfied.DefaultState(this.satisfied.normal).ToggleAttributeModifier("Breathing", (Func<SuffocationMonitor.Instance, AttributeModifier>) (smi => smi.breathing)).EventTransition(GameHashes.ExitedBreathableArea, (GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State) this.nooxygen, (StateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsInBreathableArea()));
    this.satisfied.normal.Transition(this.satisfied.low, (StateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.oxygenBreather.IsLowOxygen()));
    this.satisfied.low.Transition(this.satisfied.normal, (StateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.oxygenBreather.IsLowOxygen())).Transition((GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State) this.nooxygen, (StateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsInBreathableArea())).ToggleEffect("LowOxygen");
    this.nooxygen.EventTransition(GameHashes.EnteredBreathableArea, (GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State) this.satisfied, (StateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsInBreathableArea())).TagTransition(GameTags.RecoveringBreath, (GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State) this.satisfied).ToggleExpression(Db.Get().Expressions.Suffocate).ToggleAttributeModifier("Holding Breath", (Func<SuffocationMonitor.Instance, AttributeModifier>) (smi => smi.holdingbreath)).ToggleTag(GameTags.NoOxygen).DefaultState(this.nooxygen.holdingbreath);
    this.nooxygen.holdingbreath.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.HoldingBreath).Transition(this.nooxygen.suffocating, (StateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsSuffocating()));
    this.nooxygen.suffocating.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Suffocation, Db.Get().DuplicantStatusItems.Suffocating).Transition(this.death, (StateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.HasSuffocated()));
    this.death.Enter("SuffocationDeath", (StateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.Kill()));
    this.dead.DoNothing();
  }

  public class NoOxygenState : 
    GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State holdingbreath;
    public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State suffocating;
  }

  public class SatisfiedState : 
    GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State normal;
    public GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.State low;
  }

  public new class Instance : 
    GameStateMachine<SuffocationMonitor, SuffocationMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    private AmountInstance breath;
    public AttributeModifier breathing;
    public AttributeModifier holdingbreath;
    private static CellOffset[] pressureTestOffsets = new CellOffset[2]
    {
      new CellOffset(0, 0),
      new CellOffset(0, 1)
    };
    private const float HIGH_PRESSURE_DELAY = 3f;
    private bool wasInHighPressure;
    private float highPressureTime;

    public OxygenBreather oxygenBreather { get; private set; }

    public Instance(OxygenBreather oxygen_breather)
      : base((IStateMachineTarget) oxygen_breather)
    {
      this.breath = Db.Get().Amounts.Breath.Lookup(this.master.gameObject);
      Klei.AI.Attribute deltaAttribute = Db.Get().Amounts.Breath.deltaAttribute;
      float num = 0.909090936f;
      this.breathing = new AttributeModifier(deltaAttribute.Id, num, (string) DUPLICANTS.MODIFIERS.BREATHING.NAME);
      this.holdingbreath = new AttributeModifier(deltaAttribute.Id, -num, (string) DUPLICANTS.MODIFIERS.HOLDINGBREATH.NAME);
      this.oxygenBreather = oxygen_breather;
    }

    public bool IsInBreathableArea() => this.master.GetComponent<KPrefabID>().HasTag(GameTags.RecoveringBreath) || this.master.GetComponent<Sensors>().GetSensor<BreathableAreaSensor>().IsBreathable() || ((Component) this.oxygenBreather).HasTag(GameTags.InTransitTube);

    public bool HasSuffocated() => (double) this.breath.value <= 0.0;

    public bool IsSuffocating() => (double) this.breath.deltaAttribute.GetTotalValue() <= 0.0 && (double) this.breath.value <= 45.454547882080078;

    public void Kill() => this.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Suffocation);

    public void CheckOverPressure()
    {
      if (this.IsInHighPressure())
      {
        if (!this.wasInHighPressure)
        {
          this.wasInHighPressure = true;
          this.highPressureTime = Time.time;
        }
        else
        {
          if ((double) Time.time - (double) this.highPressureTime <= 3.0)
            return;
          this.master.GetComponent<Effects>().Add("PoppedEarDrums", true);
        }
      }
      else
        this.wasInHighPressure = false;
    }

    private bool IsInHighPressure()
    {
      int cell = Grid.PosToCell(this.gameObject);
      for (int index1 = 0; index1 < SuffocationMonitor.Instance.pressureTestOffsets.Length; ++index1)
      {
        int index2 = Grid.OffsetCell(cell, SuffocationMonitor.Instance.pressureTestOffsets[index1]);
        if (Grid.IsValidCell(index2) && Grid.Element[index2].IsGas && (double) Grid.Mass[index2] > 4.0)
          return true;
      }
      return false;
    }
  }
}
