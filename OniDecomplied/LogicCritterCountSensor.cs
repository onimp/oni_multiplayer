// Decompiled with JetBrains decompiler
// Type: LogicCritterCountSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
public class LogicCritterCountSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
  private bool wasOn;
  [Serialize]
  public bool countEggs = true;
  [Serialize]
  public bool countCritters = true;
  [Serialize]
  public int countThreshold;
  [Serialize]
  public bool activateOnGreaterThan = true;
  private int currentCount;
  private KSelectable selectable;
  private Guid roomStatusGUID;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<LogicCritterCountSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicCritterCountSensor>((Action<LogicCritterCountSensor, object>) ((component, data) => component.OnCopySettings(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.selectable = ((Component) this).GetComponent<KSelectable>();
    this.Subscribe<LogicCritterCountSensor>(-905833192, LogicCritterCountSensor.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    LogicCritterCountSensor component = ((GameObject) data).GetComponent<LogicCritterCountSensor>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.countThreshold = component.countThreshold;
    this.activateOnGreaterThan = component.activateOnGreaterThan;
    this.countCritters = component.countCritters;
    this.countEggs = component.countEggs;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.OnToggle += new Action<bool>(this.OnSwitchToggled);
    this.UpdateLogicCircuit();
    this.UpdateVisualState(true);
    this.wasOn = this.switchedOn;
  }

  public void Sim200ms(float dt)
  {
    Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(((Component) this).gameObject);
    if (roomOfGameObject != null)
    {
      this.currentCount = 0;
      if (this.countCritters)
        this.currentCount += roomOfGameObject.cavity.creatures.Count;
      if (this.countEggs)
        this.currentCount += roomOfGameObject.cavity.eggs.Count;
      this.SetState(this.activateOnGreaterThan ? this.currentCount > this.countThreshold : this.currentCount < this.countThreshold);
      if (!this.selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
        return;
      this.selectable.RemoveStatusItem(this.roomStatusGUID);
    }
    else
    {
      if (!this.selectable.HasStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom))
        this.roomStatusGUID = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.NotInAnyRoom);
      this.SetState(false);
    }
  }

  private void OnSwitchToggled(bool toggled_on)
  {
    this.UpdateLogicCircuit();
    this.UpdateVisualState();
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
    get => (float) this.countThreshold;
    set => this.countThreshold = (int) value;
  }

  public bool ActivateAboveThreshold
  {
    get => this.activateOnGreaterThan;
    set => this.activateOnGreaterThan = value;
  }

  public float CurrentValue => (float) this.currentCount;

  public float RangeMin => 0.0f;

  public float RangeMax => 64f;

  public float GetRangeMinInputField() => this.RangeMin;

  public float GetRangeMaxInputField() => this.RangeMax;

  public LocString Title => UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TITLE;

  public LocString ThresholdValueName => UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.VALUE_NAME;

  public string AboveToolTip => (string) UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_ABOVE;

  public string BelowToolTip => (string) UI.UISIDESCREENS.CRITTER_COUNT_SIDE_SCREEN.TOOLTIP_BELOW;

  public string Format(float value, bool units) => value.ToString();

  public float ProcessedSliderValue(float input) => Mathf.Round(input);

  public float ProcessedInputValue(float input) => Mathf.Round(input);

  public LocString ThresholdValueUnits() => (LocString) "";

  public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

  public int IncrementScale => 1;

  public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(this.RangeMax);
}
