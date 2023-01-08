// Decompiled with JetBrains decompiler
// Type: ExternalTemperatureMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using UnityEngine;

public class ExternalTemperatureMonitor : 
  GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance>
{
  public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State comfortable;
  public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State transitionToTooWarm;
  public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State tooWarm;
  public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State transitionToTooCool;
  public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State tooCool;
  public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State transitionToScalding;
  public GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State scalding;
  private const float SCALDING_DAMAGE_AMOUNT = 10f;
  private const float BODY_TEMPERATURE_AFFECT_EXTERNAL_FEEL_THRESHOLD = 0.5f;
  public const float BASE_STRESS_TOLERANCE_COLD = 0.278933346f;
  public const float BASE_STRESS_TOLERANCE_WARM = 0.278933346f;
  private const float START_GAME_AVERAGING_DELAY = 6f;
  private const float TRANSITION_TO_DELAY = 1f;
  private const float TRANSITION_OUT_DELAY = 6f;
  private const float TEMPERATURE_AVERAGING_RANGE = 6f;

  public static float GetExternalColdThreshold(Attributes affected_attributes) => affected_attributes == null ? -0.36261335f : (float) -(0.36261335015296936 - (double) affected_attributes.GetValue(Db.Get().Attributes.RoomTemperaturePreference.Id));

  public static float GetExternalWarmThreshold(Attributes affected_attributes) => affected_attributes == null ? 0.195253342f : (float) -(-0.19525334239006042 - (double) affected_attributes.GetValue(Db.Get().Attributes.RoomTemperaturePreference.Id));

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.comfortable;
    this.root.Enter((StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.AverageExternalTemperature = smi.GetCurrentExternalTemperature)).Update((System.Action<ExternalTemperatureMonitor.Instance, float>) ((smi, dt) =>
    {
      smi.AverageExternalTemperature *= Mathf.Max(0.0f, (float) (1.0 - (double) dt / 6.0));
      smi.AverageExternalTemperature += smi.GetCurrentExternalTemperature * (dt / 6f);
    }));
    this.comfortable.Transition(this.transitionToTooWarm, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsTooHot() && (double) smi.timeinstate > 6.0)).Transition(this.transitionToTooCool, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsTooCold() && (double) smi.timeinstate > 6.0));
    this.transitionToTooWarm.Transition(this.comfortable, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsTooHot())).Transition(this.tooWarm, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsTooHot() && (double) smi.timeinstate > 1.0));
    this.transitionToTooCool.Transition(this.comfortable, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsTooCold())).Transition(this.tooCool, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsTooCold() && (double) smi.timeinstate > 1.0));
    this.transitionToScalding.Transition(this.tooWarm, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsScalding())).Transition(this.scalding, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsScalding() && (double) smi.timeinstate > 1.0));
    this.tooWarm.Transition(this.comfortable, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsTooHot() && (double) smi.timeinstate > 6.0)).Transition(this.transitionToScalding, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsScalding())).ToggleExpression(Db.Get().Expressions.Hot).ToggleThought(Db.Get().Thoughts.Hot).ToggleStatusItem(Db.Get().DuplicantStatusItems.Hot, (Func<ExternalTemperatureMonitor.Instance, object>) (smi => (object) smi)).ToggleEffect("WarmAir").Enter((StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort)));
    this.scalding.Transition(this.tooWarm, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsScalding() && (double) smi.timeinstate > 6.0)).ToggleExpression(Db.Get().Expressions.Hot).ToggleThought(Db.Get().Thoughts.Hot).ToggleStatusItem(Db.Get().CreatureStatusItems.Scalding, (Func<ExternalTemperatureMonitor.Instance, object>) (smi => (object) smi)).Update("ScaldDamage", (System.Action<ExternalTemperatureMonitor.Instance, float>) ((smi, dt) => smi.ScaldDamage(dt)), (UpdateRate) 6);
    this.tooCool.Transition(this.comfortable, (StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsTooCold() && (double) smi.timeinstate > 6.0)).ToggleExpression(Db.Get().Expressions.Cold).ToggleThought(Db.Get().Thoughts.Cold).ToggleStatusItem(Db.Get().DuplicantStatusItems.Cold, (Func<ExternalTemperatureMonitor.Instance, object>) (smi => (object) smi)).ToggleEffect("ColdAir").Enter((StateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort)));
  }

  public new class Instance : 
    GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public float AverageExternalTemperature;
    public float ColdThreshold = 283.15f;
    public float HotThreshold = 306.15f;
    private AttributeModifier baseScalindingThreshold = new AttributeModifier("ScaldingThreshold", 345f, (string) DUPLICANTS.STATS.SKIN_DURABILITY.NAME);
    public Attributes attributes;
    public OccupyArea occupyArea;
    public AmountInstance internalTemperature;
    private TemperatureMonitor.Instance internalTemperatureMonitor;
    public CreatureSimTemperatureTransfer temperatureTransferer;
    public Health health;
    public PrimaryElement primaryElement;
    private const float MIN_SCALD_INTERVAL = 5f;
    private float lastScaldTime;

    public float GetCurrentExternalTemperature
    {
      get
      {
        int cell = Grid.PosToCell(this.gameObject);
        if (!Object.op_Inequality((Object) this.occupyArea, (Object) null))
          return Grid.Temperature[cell];
        float num1 = 0.0f;
        int num2 = 0;
        for (int index = 0; index < this.occupyArea.OccupiedCellsOffsets.Length; ++index)
        {
          int num3 = Grid.OffsetCell(cell, this.occupyArea.OccupiedCellsOffsets[index]);
          if (Grid.IsValidCell(num3))
          {
            ++num2;
            num1 += Grid.Temperature[num3];
          }
        }
        return num1 / (float) Mathf.Max(1, num2);
      }
    }

    public override void StartSM()
    {
      base.StartSM();
      this.smi.attributes.Get(Db.Get().Attributes.ScaldingThreshold).Add(this.baseScalindingThreshold);
    }

    public float GetCurrentColdThreshold => (double) this.internalTemperatureMonitor.IdealTemperatureDelta() > 0.5 ? 0.0f : CreatureSimTemperatureTransfer.PotentialEnergyFlowToCreature(Grid.PosToCell(this.gameObject), this.primaryElement, (SimTemperatureTransfer) this.temperatureTransferer);

    public float GetScaldingThreshold() => this.smi.attributes.GetValue("ScaldingThreshold");

    public float GetCurrentHotThreshold => this.HotThreshold;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.health = this.GetComponent<Health>();
      this.occupyArea = this.GetComponent<OccupyArea>();
      this.internalTemperatureMonitor = this.gameObject.GetSMI<TemperatureMonitor.Instance>();
      this.internalTemperature = Db.Get().Amounts.Temperature.Lookup(this.gameObject);
      this.temperatureTransferer = this.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
      this.primaryElement = this.gameObject.GetComponent<PrimaryElement>();
      this.attributes = this.gameObject.GetAttributes();
    }

    public bool IsTooHot() => (double) this.internalTemperatureMonitor.IdealTemperatureDelta() >= -0.5 && (double) this.smi.temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage > (double) ExternalTemperatureMonitor.GetExternalWarmThreshold(this.smi.attributes);

    public bool IsTooCold() => (double) this.internalTemperatureMonitor.IdealTemperatureDelta() <= 0.5 && (double) this.smi.temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage < (double) ExternalTemperatureMonitor.GetExternalColdThreshold(this.smi.attributes);

    public bool IsScalding() => (double) this.AverageExternalTemperature > (double) this.smi.attributes.GetValue("ScaldingThreshold");

    public void ScaldDamage(float dt)
    {
      if (!Object.op_Inequality((Object) this.health, (Object) null) || (double) Time.time - (double) this.lastScaldTime <= 5.0)
        return;
      this.lastScaldTime = Time.time;
      this.health.Damage(dt * 10f);
    }

    public float CurrentWorldTransferWattage() => this.temperatureTransferer.currentExchangeWattage;
  }
}
