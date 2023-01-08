// Decompiled with JetBrains decompiler
// Type: LogicGateBuffer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
public class LogicGateBuffer : LogicGate, ISingleSliderControl, ISliderControl
{
  [Serialize]
  private bool input_was_previously_positive;
  [Serialize]
  private float delayAmount = 5f;
  [Serialize]
  private int delayTicksRemaining;
  private MeterController meter;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<LogicGateBuffer> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicGateBuffer>((Action<LogicGateBuffer, object>) ((component, data) => component.OnCopySettings(data)));

  public float DelayAmount
  {
    get => this.delayAmount;
    set
    {
      this.delayAmount = value;
      int delayAmountTicks = this.DelayAmountTicks;
      if (this.delayTicksRemaining <= delayAmountTicks)
        return;
      this.delayTicksRemaining = delayAmountTicks;
    }
  }

  private int DelayAmountTicks => Mathf.RoundToInt(this.delayAmount / LogicCircuitManager.ClockTickInterval);

  public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.LOGIC_BUFFER_SIDE_SCREEN.TITLE";

  public string SliderUnits => (string) UI.UNITSUFFIXES.SECOND;

  public int SliderDecimalPlaces(int index) => 1;

  public float GetSliderMin(int index) => 0.1f;

  public float GetSliderMax(int index) => 200f;

  public float GetSliderValue(int index) => this.DelayAmount;

  public void SetSliderValue(float value, int index) => this.DelayAmount = value;

  public string GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.LOGIC_BUFFER_SIDE_SCREEN.TOOLTIP";

  string ISliderControl.GetSliderTooltip() => string.Format(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.UISIDESCREENS.LOGIC_BUFFER_SIDE_SCREEN.TOOLTIP")), (object) this.DelayAmount);

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<LogicGateBuffer>(-905833192, LogicGateBuffer.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    LogicGateBuffer component = ((GameObject) data).GetComponent<LogicGateBuffer>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.DelayAmount = component.DelayAmount;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.UserSpecified, Grid.SceneLayer.LogicGatesFront, Vector3.zero, (string[]) null);
    this.meter.SetPositionPercent(1f);
  }

  private void Update() => this.meter.SetPositionPercent(!this.input_was_previously_positive ? (this.delayTicksRemaining <= 0 ? 1f : (float) (this.DelayAmountTicks - this.delayTicksRemaining) / (float) this.DelayAmountTicks) : 0.0f);

  public override void LogicTick()
  {
    if (this.input_was_previously_positive || this.delayTicksRemaining <= 0)
      return;
    --this.delayTicksRemaining;
    if (this.delayTicksRemaining > 0)
      return;
    this.OnDelay();
  }

  protected override int GetCustomValue(int val1, int val2)
  {
    if (val1 != 0)
    {
      this.input_was_previously_positive = true;
      this.delayTicksRemaining = 0;
      this.meter.SetPositionPercent(0.0f);
    }
    else if (this.delayTicksRemaining <= 0)
    {
      if (this.input_was_previously_positive)
        this.delayTicksRemaining = this.DelayAmountTicks;
      this.input_was_previously_positive = false;
    }
    return val1 == 0 && this.delayTicksRemaining <= 0 ? 0 : 1;
  }

  private void OnDelay()
  {
    if (this.cleaningUp)
      return;
    this.delayTicksRemaining = 0;
    this.meter.SetPositionPercent(1f);
    if (this.outputValueOne == 0 || !(Game.Instance.logicCircuitSystem.GetNetworkForCell(this.OutputCellOne) is LogicCircuitNetwork))
      return;
    this.outputValueOne = 0;
    this.RefreshAnimation();
  }
}
