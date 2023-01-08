// Decompiled with JetBrains decompiler
// Type: WarmBlooded
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Klei.AI;
using STRINGS;
using System;
using UnityEngine;

public class WarmBlooded : StateMachineComponent<WarmBlooded.StatesInstance>
{
  [MyCmpAdd]
  private Notifier notifier;
  private AmountInstance externalTemperature;
  public AmountInstance temperature;
  private PrimaryElement primaryElement;
  public const float TRANSITION_DELAY_HOT = 3f;
  public const float TRANSITION_DELAY_COLD = 3f;

  protected virtual void OnPrefabInit()
  {
    this.externalTemperature = Db.Get().Amounts.ExternalTemperature.Lookup(((Component) this).gameObject);
    this.externalTemperature.value = Grid.Temperature[Grid.PosToCell((KMonoBehaviour) this)];
    this.temperature = Db.Get().Amounts.Temperature.Lookup(((Component) this).gameObject);
    this.primaryElement = ((Component) this).GetComponent<PrimaryElement>();
  }

  protected virtual void OnSpawn() => this.smi.StartSM();

  protected override void OnCleanUp() => base.OnCleanUp();

  public bool IsAtReasonableTemperature() => !this.smi.IsHot() && !this.smi.IsCold();

  public void SetTemperatureImmediate(float t) => this.temperature.value = t;

  public class StatesInstance : 
    GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.GameInstance
  {
    public AttributeModifier baseTemperatureModification;
    public AttributeModifier bodyRegulator;
    public AttributeModifier averageBodyRegulation;
    public AttributeModifier burningCalories;
    public float averageInternalTemperature;

    public StatesInstance(WarmBlooded smi)
      : base(smi)
    {
      this.baseTemperatureModification = new AttributeModifier(nameof (TemperatureDelta), 0.0f, (string) DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME, uiOnly: true, is_readonly: false);
      this.bodyRegulator = new AttributeModifier(nameof (TemperatureDelta), 0.0f, (string) DUPLICANTS.MODIFIERS.HOMEOSTASIS.NAME, uiOnly: true, is_readonly: false);
      this.burningCalories = new AttributeModifier("CaloriesDelta", 0.0f, (string) DUPLICANTS.MODIFIERS.BURNINGCALORIES.NAME, is_readonly: false);
      this.master.GetAttributes().Add(this.bodyRegulator);
      this.master.GetAttributes().Add(this.burningCalories);
      this.master.GetAttributes().Add(this.baseTemperatureModification);
      this.master.SetTemperatureImmediate(310.15f);
    }

    public float TemperatureDelta => this.bodyRegulator.Value;

    public float BodyTemperature => this.master.primaryElement.Temperature;

    public bool IsHot() => (double) this.BodyTemperature > 310.14999389648438;

    public bool IsCold() => (double) this.BodyTemperature < 310.14999389648438;
  }

  public class States : GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded>
  {
    public WarmBlooded.States.AliveState alive;
    public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State dead;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.alive.normal;
      this.root.TagTransition(GameTags.Dead, this.dead).Enter((StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State.Callback) (smi =>
      {
        PrimaryElement component1 = ((Component) smi.master).GetComponent<PrimaryElement>();
        float temperatureDelta = SimUtil.EnergyFlowToTemperatureDelta(0.08368001f, component1.Element.specificHeatCapacity, component1.Mass);
        smi.baseTemperatureModification.SetValue(temperatureDelta);
        CreatureSimTemperatureTransfer component2 = ((Component) smi.master).GetComponent<CreatureSimTemperatureTransfer>();
        component2.NonSimTemperatureModifiers.Add(smi.baseTemperatureModification);
        component2.NonSimTemperatureModifiers.Add(smi.bodyRegulator);
      }));
      this.alive.normal.Transition(this.alive.cold.transition, (StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback) (smi => smi.IsCold())).Transition(this.alive.hot.transition, (StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback) (smi => smi.IsHot()));
      this.alive.cold.transition.ScheduleGoTo(3f, (StateMachine.BaseState) this.alive.cold.regulating).Transition(this.alive.normal, (StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback) (smi => !smi.IsCold()));
      this.alive.cold.regulating.Transition(this.alive.normal, (StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback) (smi => !smi.IsCold())).Update("ColdRegulating", (Action<WarmBlooded.StatesInstance, float>) ((smi, dt) =>
      {
        PrimaryElement component = ((Component) smi.master).GetComponent<PrimaryElement>();
        float temperatureDelta1 = SimUtil.EnergyFlowToTemperatureDelta(0.08368001f, component.Element.specificHeatCapacity, component.Mass);
        float temperatureDelta2 = SimUtil.EnergyFlowToTemperatureDelta(0.5578667f, component.Element.specificHeatCapacity, component.Mass);
        float num1 = 310.15f - smi.BodyTemperature;
        float num2 = 1f;
        if ((double) temperatureDelta2 + (double) temperatureDelta1 > (double) num1)
          num2 = Mathf.Max(0.0f, num1 - temperatureDelta1) / temperatureDelta2;
        smi.bodyRegulator.SetValue(temperatureDelta2 * num2);
        smi.burningCalories.SetValue((float) (-0.55786669254302979 * (double) num2 * 1000.0 / 4184.0));
      })).Exit((StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State.Callback) (smi =>
      {
        smi.bodyRegulator.SetValue(0.0f);
        smi.burningCalories.SetValue(0.0f);
      }));
      this.alive.hot.transition.ScheduleGoTo(3f, (StateMachine.BaseState) this.alive.hot.regulating).Transition(this.alive.normal, (StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback) (smi => !smi.IsHot()));
      this.alive.hot.regulating.Transition(this.alive.normal, (StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.Transition.ConditionCallback) (smi => !smi.IsHot())).Update("WarmRegulating", (Action<WarmBlooded.StatesInstance, float>) ((smi, dt) =>
      {
        PrimaryElement component = ((Component) smi.master).GetComponent<PrimaryElement>();
        float temperatureDelta = SimUtil.EnergyFlowToTemperatureDelta(0.5578667f, component.Element.specificHeatCapacity, component.Mass);
        float num3 = 310.15f - smi.BodyTemperature;
        float num4 = 1f;
        if (((double) temperatureDelta - (double) smi.baseTemperatureModification.Value) * (double) dt < (double) num3)
          num4 = Mathf.Clamp(num3 / ((temperatureDelta - smi.baseTemperatureModification.Value) * dt), 0.0f, 1f);
        smi.bodyRegulator.SetValue(-temperatureDelta * num4);
        smi.burningCalories.SetValue((float) (-0.55786669254302979 * (double) num4 / 4184.0));
      })).Exit((StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State.Callback) (smi => smi.bodyRegulator.SetValue(0.0f)));
      this.dead.Enter((StateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State.Callback) (smi => ((Behaviour) smi.master).enabled = false));
    }

    public class RegulatingState : 
      GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State
    {
      public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State transition;
      public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State regulating;
    }

    public class AliveState : 
      GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State
    {
      public GameStateMachine<WarmBlooded.States, WarmBlooded.StatesInstance, WarmBlooded, object>.State normal;
      public WarmBlooded.States.RegulatingState cold;
      public WarmBlooded.States.RegulatingState hot;
    }
  }
}
