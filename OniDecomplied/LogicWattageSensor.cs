// Decompiled with JetBrains decompiler
// Type: LogicWattageSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
public class LogicWattageSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
  [Serialize]
  public float thresholdWattage;
  [Serialize]
  public bool activateOnHigherThan;
  [Serialize]
  public bool dirty = true;
  private readonly float minWattage;
  private readonly float maxWattage = 1.5f * Wire.GetMaxWattageAsFloat(Wire.WattageRating.Max50000);
  private float currentWattage;
  private bool wasOn;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<LogicWattageSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicWattageSensor>((Action<LogicWattageSensor, object>) ((component, data) => component.OnCopySettings(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<LogicWattageSensor>(-905833192, LogicWattageSensor.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    LogicWattageSensor component = ((GameObject) data).GetComponent<LogicWattageSensor>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.Threshold = component.Threshold;
    this.ActivateAboveThreshold = component.ActivateAboveThreshold;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.OnToggle += new Action<bool>(this.OnSwitchToggled);
    this.UpdateVisualState(true);
    this.UpdateLogicCircuit();
    this.wasOn = this.switchedOn;
  }

  public void Sim200ms(float dt)
  {
    this.currentWattage = Game.Instance.circuitManager.GetWattsUsedByCircuit(Game.Instance.circuitManager.GetCircuitID(Grid.PosToCell((KMonoBehaviour) this)));
    this.currentWattage = Mathf.Max(0.0f, this.currentWattage);
    if (this.activateOnHigherThan)
    {
      if (((double) this.currentWattage <= (double) this.thresholdWattage || this.IsSwitchedOn) && ((double) this.currentWattage > (double) this.thresholdWattage || !this.IsSwitchedOn))
        return;
      this.Toggle();
    }
    else
    {
      if (((double) this.currentWattage < (double) this.thresholdWattage || !this.IsSwitchedOn) && ((double) this.currentWattage >= (double) this.thresholdWattage || this.IsSwitchedOn))
        return;
      this.Toggle();
    }
  }

  public float GetWattageUsed() => this.currentWattage;

  private void OnSwitchToggled(bool toggled_on)
  {
    this.UpdateVisualState();
    this.UpdateLogicCircuit();
  }

  private void UpdateLogicCircuit() => ((Component) this).GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);

  private void UpdateVisualState(bool force = false)
  {
    if (!(this.wasOn != this.switchedOn | force))
      return;
    this.wasOn = this.switchedOn;
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    component.Play(HashedString.op_Implicit(this.switchedOn ? "on_pre" : "on_pst"));
    component.Queue(HashedString.op_Implicit(this.switchedOn ? "on" : "off"));
  }

  protected override void UpdateSwitchStatus()
  {
    StatusItem status_item = this.switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
  }

  public float Threshold
  {
    get => this.thresholdWattage;
    set
    {
      this.thresholdWattage = value;
      this.dirty = true;
    }
  }

  public bool ActivateAboveThreshold
  {
    get => this.activateOnHigherThan;
    set
    {
      this.activateOnHigherThan = value;
      this.dirty = true;
    }
  }

  public float CurrentValue => this.GetWattageUsed();

  public float RangeMin => this.minWattage;

  public float RangeMax => this.maxWattage;

  public float GetRangeMinInputField() => this.minWattage;

  public float GetRangeMaxInputField() => this.maxWattage;

  public LocString Title => UI.UISIDESCREENS.WATTAGESWITCHSIDESCREEN.TITLE;

  public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.WATTAGE;

  public string AboveToolTip => (string) UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.WATTAGE_TOOLTIP_ABOVE;

  public string BelowToolTip => (string) UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.WATTAGE_TOOLTIP_BELOW;

  public string Format(float value, bool units) => GameUtil.GetFormattedWattage(value, GameUtil.WattageFormatterUnit.Watts, units);

  public float ProcessedSliderValue(float input) => Mathf.Round(input);

  public float ProcessedInputValue(float input) => input;

  public LocString ThresholdValueUnits() => UI.UNITSUFFIXES.ELECTRICAL.WATT;

  public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

  public int IncrementScale => 1;

  public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[4]
  {
    new NonLinearSlider.Range(5f, 5f),
    new NonLinearSlider.Range(35f, 1000f),
    new NonLinearSlider.Range(50f, 3000f),
    new NonLinearSlider.Range(10f, this.maxWattage)
  };
}
