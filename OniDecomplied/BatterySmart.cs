// Decompiled with JetBrains decompiler
// Type: BatterySmart
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Diagnostics;
using UnityEngine;

[SerializationConfig]
[DebuggerDisplay("{name}")]
public class BatterySmart : Battery, IActivationRangeTarget
{
  public static readonly HashedString PORT_ID = HashedString.op_Implicit("BatterySmartLogicPort");
  [Serialize]
  private int activateValue;
  [Serialize]
  private int deactivateValue = 100;
  [Serialize]
  private bool activated;
  [MyCmpGet]
  private LogicPorts logicPorts;
  private MeterController logicMeter;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<BatterySmart> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<BatterySmart>((Action<BatterySmart, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly EventSystem.IntraObjectHandler<BatterySmart> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<BatterySmart>((Action<BatterySmart, object>) ((component, data) => component.OnLogicValueChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<BatterySmart> UpdateLogicCircuitDelegate = new EventSystem.IntraObjectHandler<BatterySmart>((Action<BatterySmart, object>) ((component, data) => component.UpdateLogicCircuit(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<BatterySmart>(-905833192, BatterySmart.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    BatterySmart component = ((GameObject) data).GetComponent<BatterySmart>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.ActivateValue = component.ActivateValue;
    this.DeactivateValue = component.DeactivateValue;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.CreateLogicMeter();
    this.Subscribe<BatterySmart>(-801688580, BatterySmart.OnLogicValueChangedDelegate);
    this.Subscribe<BatterySmart>(-592767678, BatterySmart.UpdateLogicCircuitDelegate);
  }

  private void CreateLogicMeter() => this.logicMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "logicmeter_target", "logicmeter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());

  public override void EnergySim200ms(float dt)
  {
    base.EnergySim200ms(dt);
    this.UpdateLogicCircuit((object) null);
  }

  private void UpdateLogicCircuit(object data)
  {
    float num = (float) Mathf.RoundToInt(this.PercentFull * 100f);
    if (this.activated)
    {
      if ((double) num >= (double) this.deactivateValue)
        this.activated = false;
    }
    else if ((double) num <= (double) this.activateValue)
      this.activated = true;
    bool flag = this.activated & this.operational.IsOperational;
    this.logicPorts.SendSignal(BatterySmart.PORT_ID, flag ? 1 : 0);
  }

  private void OnLogicValueChanged(object data)
  {
    LogicValueChanged logicValueChanged = (LogicValueChanged) data;
    if (!HashedString.op_Equality(logicValueChanged.portID, BatterySmart.PORT_ID))
      return;
    this.SetLogicMeter(LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue));
  }

  public void SetLogicMeter(bool on)
  {
    if (this.logicMeter == null)
      return;
    this.logicMeter.SetPositionPercent(on ? 1f : 0.0f);
  }

  public float ActivateValue
  {
    get => (float) this.deactivateValue;
    set
    {
      this.deactivateValue = (int) value;
      this.UpdateLogicCircuit((object) null);
    }
  }

  public float DeactivateValue
  {
    get => (float) this.activateValue;
    set
    {
      this.activateValue = (int) value;
      this.UpdateLogicCircuit((object) null);
    }
  }

  public float MinValue => 0.0f;

  public float MaxValue => 100f;

  public bool UseWholeNumbers => true;

  public string ActivateTooltip => (string) BUILDINGS.PREFABS.BATTERYSMART.DEACTIVATE_TOOLTIP;

  public string DeactivateTooltip => (string) BUILDINGS.PREFABS.BATTERYSMART.ACTIVATE_TOOLTIP;

  public string ActivationRangeTitleText => (string) BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_TITLE;

  public string ActivateSliderLabelText => (string) BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_DEACTIVATE;

  public string DeactivateSliderLabelText => (string) BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_ACTIVATE;
}
