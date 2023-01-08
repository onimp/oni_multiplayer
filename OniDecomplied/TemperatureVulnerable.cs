// Decompiled with JetBrains decompiler
// Type: TemperatureVulnerable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class TemperatureVulnerable : 
  StateMachineComponent<TemperatureVulnerable.StatesInstance>,
  IGameObjectEffectDescriptor,
  IWiltCause,
  ISlicedSim1000ms
{
  private OccupyArea _occupyArea;
  [SerializeField]
  private float internalTemperatureLethal_Low;
  [SerializeField]
  private float internalTemperatureWarning_Low;
  [SerializeField]
  private float internalTemperatureWarning_High;
  [SerializeField]
  private float internalTemperatureLethal_High;
  private AttributeInstance wiltTempRangeModAttribute;
  private float temperatureRangeModScalar;
  private const float minimumMassForReading = 0.1f;
  [MyCmpReq]
  private PrimaryElement primaryElement;
  [MyCmpReq]
  private SimTemperatureTransfer temperatureTransfer;
  private AmountInstance displayTemperatureAmount;
  private TemperatureVulnerable.TemperatureState internalTemperatureState = TemperatureVulnerable.TemperatureState.Normal;
  private float averageTemp;
  private int cellCount;
  private static readonly Func<int, object, bool> GetAverageTemperatureCbDelegate = (Func<int, object, bool>) ((cell, data) => TemperatureVulnerable.GetAverageTemperatureCb(cell, data));

  private OccupyArea occupyArea
  {
    get
    {
      if (Object.op_Equality((Object) this._occupyArea, (Object) null))
        this._occupyArea = ((Component) this).GetComponent<OccupyArea>();
      return this._occupyArea;
    }
  }

  public float TemperatureLethalLow => this.internalTemperatureLethal_Low;

  public float TemperatureLethalHigh => this.internalTemperatureLethal_High;

  public float TemperatureWarningLow => this.wiltTempRangeModAttribute != null ? this.internalTemperatureWarning_Low + (1f - this.wiltTempRangeModAttribute.GetTotalValue()) * this.temperatureRangeModScalar : this.internalTemperatureWarning_Low;

  public float TemperatureWarningHigh => this.wiltTempRangeModAttribute != null ? this.internalTemperatureWarning_High - (1f - this.wiltTempRangeModAttribute.GetTotalValue()) * this.temperatureRangeModScalar : this.internalTemperatureWarning_High;

  public event Action<float, float> OnTemperature;

  public float InternalTemperature => this.primaryElement.Temperature;

  public TemperatureVulnerable.TemperatureState GetInternalTemperatureState => this.internalTemperatureState;

  public bool IsLethal => this.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.LethalHot || this.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.LethalCold;

  public bool IsNormal => this.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.Normal;

  WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[1];

  public string WiltStateString
  {
    get
    {
      if (this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.warningCold))
        return Db.Get().CreatureStatusItems.Cold_Crop.resolveStringCallback((string) CREATURES.STATUSITEMS.COLD_CROP.NAME, (object) this);
      return this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.warningHot) ? Db.Get().CreatureStatusItems.Hot_Crop.resolveStringCallback((string) CREATURES.STATUSITEMS.HOT_CROP.NAME, (object) this) : "";
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.displayTemperatureAmount = ((Component) this).gameObject.GetAmounts().Add(new AmountInstance(Db.Get().Amounts.Temperature, ((Component) this).gameObject));
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.wiltTempRangeModAttribute = this.GetAttributes().Get(Db.Get().PlantAttributes.WiltTempRangeMod);
    this.temperatureRangeModScalar = (float) (((double) this.internalTemperatureWarning_High - (double) this.internalTemperatureWarning_Low) / 2.0);
    SlicedUpdaterSim1000ms<TemperatureVulnerable>.instance.RegisterUpdate1000ms(this);
    double num = (double) this.smi.sm.internalTemp.Set(this.primaryElement.Temperature, this.smi);
    this.smi.StartSM();
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    SlicedUpdaterSim1000ms<TemperatureVulnerable>.instance.UnregisterUpdate1000ms(this);
  }

  public void Configure(
    float tempWarningLow,
    float tempLethalLow,
    float tempWarningHigh,
    float tempLethalHigh)
  {
    this.internalTemperatureWarning_Low = tempWarningLow;
    this.internalTemperatureLethal_Low = tempLethalLow;
    this.internalTemperatureLethal_High = tempLethalHigh;
    this.internalTemperatureWarning_High = tempWarningHigh;
  }

  public bool IsCellSafe(int cell)
  {
    float averageTemperature = this.GetAverageTemperature(cell);
    return (double) averageTemperature > -1.0 && (double) averageTemperature > (double) this.TemperatureLethalLow && (double) averageTemperature < (double) this.internalTemperatureLethal_High;
  }

  public void SlicedSim1000ms(float dt)
  {
    if (!Grid.IsValidCell(Grid.PosToCell(((Component) this).gameObject)))
      return;
    double num = (double) this.smi.sm.internalTemp.Set(this.InternalTemperature, this.smi);
    this.displayTemperatureAmount.value = this.InternalTemperature;
    if (this.OnTemperature == null)
      return;
    this.OnTemperature(dt, this.InternalTemperature);
  }

  private static bool GetAverageTemperatureCb(int cell, object data)
  {
    TemperatureVulnerable temperatureVulnerable = data as TemperatureVulnerable;
    if ((double) Grid.Mass[cell] > 0.10000000149011612)
    {
      temperatureVulnerable.averageTemp += Grid.Temperature[cell];
      ++temperatureVulnerable.cellCount;
    }
    return true;
  }

  private float GetAverageTemperature(int cell)
  {
    this.averageTemp = 0.0f;
    this.cellCount = 0;
    this.occupyArea.TestArea(cell, (object) this, TemperatureVulnerable.GetAverageTemperatureCbDelegate);
    return this.cellCount > 0 ? this.averageTemp / (float) this.cellCount : -1f;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    float num = (float) (((double) this.internalTemperatureWarning_High - (double) this.internalTemperatureWarning_Low) / 2.0);
    float temp1 = this.wiltTempRangeModAttribute != null ? this.TemperatureWarningLow : this.internalTemperatureWarning_Low + (1f - ((Component) this).GetComponent<Modifiers>().GetPreModifiedAttributeValue(Db.Get().PlantAttributes.WiltTempRangeMod)) * num;
    float temp2 = this.wiltTempRangeModAttribute != null ? this.TemperatureWarningHigh : this.internalTemperatureWarning_High - (1f - ((Component) this).GetComponent<Modifiers>().GetPreModifiedAttributeValue(Db.Get().PlantAttributes.WiltTempRangeMod)) * num;
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.REQUIRES_TEMPERATURE, (object) GameUtil.GetFormattedTemperature(temp1, displayUnits: false), (object) GameUtil.GetFormattedTemperature(temp2)), string.Format((string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_TEMPERATURE, (object) GameUtil.GetFormattedTemperature(temp1, displayUnits: false), (object) GameUtil.GetFormattedTemperature(temp2)), (Descriptor.DescriptorType) 0, false));
    return descriptors;
  }

  public class StatesInstance : 
    GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.GameInstance
  {
    public bool hasMaturity;

    public StatesInstance(TemperatureVulnerable master)
      : base(master)
    {
      if (Db.Get().Amounts.Maturity.Lookup(this.gameObject) == null)
        return;
      this.hasMaturity = true;
    }
  }

  public class States : 
    GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable>
  {
    public StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.FloatParameter internalTemp;
    public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State lethalCold;
    public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State lethalHot;
    public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State warningCold;
    public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State warningHot;
    public GameStateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State normal;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.normal;
      this.lethalCold.Enter((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State.Callback) (smi => smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.LethalCold)).TriggerOnEnter(GameHashes.TooColdFatal).ParamTransition<float>((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>) this.internalTemp, this.warningCold, (StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p > (double) smi.master.TemperatureLethalLow)).Enter(new StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State.Callback(TemperatureVulnerable.States.Kill));
      this.lethalHot.Enter((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State.Callback) (smi => smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.LethalHot)).TriggerOnEnter(GameHashes.TooHotFatal).ParamTransition<float>((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>) this.internalTemp, this.warningHot, (StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p < (double) smi.master.TemperatureLethalHigh)).Enter(new StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State.Callback(TemperatureVulnerable.States.Kill));
      this.warningCold.Enter((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State.Callback) (smi => smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.WarningCold)).TriggerOnEnter(GameHashes.TooColdWarning).ParamTransition<float>((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>) this.internalTemp, this.lethalCold, (StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p < (double) smi.master.TemperatureLethalLow)).ParamTransition<float>((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>) this.internalTemp, this.normal, (StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p > (double) smi.master.TemperatureWarningLow));
      this.warningHot.Enter((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State.Callback) (smi => smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.WarningHot)).TriggerOnEnter(GameHashes.TooHotWarning).ParamTransition<float>((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>) this.internalTemp, this.lethalHot, (StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p > (double) smi.master.TemperatureLethalHigh)).ParamTransition<float>((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>) this.internalTemp, this.normal, (StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p < (double) smi.master.TemperatureWarningHigh));
      this.normal.Enter((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.State.Callback) (smi => smi.master.internalTemperatureState = TemperatureVulnerable.TemperatureState.Normal)).TriggerOnEnter(GameHashes.OptimalTemperatureAchieved).ParamTransition<float>((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>) this.internalTemp, this.warningHot, (StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p > (double) smi.master.TemperatureWarningHigh)).ParamTransition<float>((StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>) this.internalTemp, this.warningCold, (StateMachine<TemperatureVulnerable.States, TemperatureVulnerable.StatesInstance, TemperatureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p < (double) smi.master.TemperatureWarningLow));
    }

    private static void Kill(StateMachine.Instance smi) => smi.GetSMI<DeathMonitor.Instance>()?.Kill(Db.Get().Deaths.Generic);
  }

  public enum TemperatureState
  {
    LethalCold,
    WarningCold,
    Normal,
    WarningHot,
    LethalHot,
  }
}
