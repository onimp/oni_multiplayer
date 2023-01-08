// Decompiled with JetBrains decompiler
// Type: LogicCounter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class LogicCounter : Switch, ISaveLoadable
{
  [Serialize]
  public int maxCount;
  [Serialize]
  public int currentCount;
  [Serialize]
  public bool resetCountAtMax;
  [Serialize]
  public bool advancedMode;
  private bool wasOn;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<LogicCounter> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicCounter>((Action<LogicCounter, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly EventSystem.IntraObjectHandler<LogicCounter> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicCounter>((Action<LogicCounter, object>) ((component, data) => component.OnLogicValueChanged(data)));
  public static readonly HashedString INPUT_PORT_ID = new HashedString("LogicCounterInput");
  public static readonly HashedString RESET_PORT_ID = new HashedString("LogicCounterReset");
  public static readonly HashedString OUTPUT_PORT_ID = new HashedString("LogicCounterOutput");
  private bool resetRequested;
  [Serialize]
  private bool wasResetting;
  [Serialize]
  private bool wasIncrementing;
  [Serialize]
  public bool receivedFirstSignal;
  private bool pulsingActive;
  private const int pulseLength = 1;
  private int pulseTicksRemaining;
  private MeterController meter;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<LogicCounter>(-905833192, LogicCounter.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    LogicCounter component = ((GameObject) data).GetComponent<LogicCounter>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.maxCount = component.maxCount;
    this.resetCountAtMax = component.resetCountAtMax;
    this.advancedMode = component.advancedMode;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.OnToggle += new Action<bool>(this.OnSwitchToggled);
    Game.Instance.logicCircuitManager.onLogicTick += new System.Action(this.LogicTick);
    if (this.maxCount == 0)
      this.maxCount = 10;
    this.Subscribe<LogicCounter>(-801688580, LogicCounter.OnLogicValueChangedDelegate);
    this.UpdateLogicCircuit();
    this.UpdateVisualState(true);
    this.wasOn = this.switchedOn;
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    this.meter = new MeterController((KAnimControllerBase) component, "meter_target", component.FlipY ? "meter_dn" : "meter_up", Meter.Offset.UserSpecified, Grid.SceneLayer.LogicGatesFront, Vector3.zero, (string[]) null);
    this.UpdateMeter();
  }

  protected virtual void OnCleanUp() => Game.Instance.logicCircuitManager.onLogicTick -= new System.Action(this.LogicTick);

  private void OnSwitchToggled(bool toggled_on)
  {
    this.UpdateLogicCircuit();
    this.UpdateVisualState();
  }

  public void UpdateLogicCircuit()
  {
    if (!this.receivedFirstSignal)
      return;
    ((Component) this).GetComponent<LogicPorts>().SendSignal(LogicCounter.OUTPUT_PORT_ID, this.switchedOn ? 1 : 0);
  }

  public void UpdateMeter()
  {
    float num = this.advancedMode ? (float) (this.currentCount % this.maxCount) : (float) this.currentCount;
    if ((double) num == 10.0)
      num = 0.0f;
    this.meter.SetPositionPercent(num / 10f);
  }

  public void UpdateVisualState(bool force = false)
  {
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    if (!this.receivedFirstSignal)
    {
      component.Play(HashedString.op_Implicit("off"));
    }
    else
    {
      if (!(this.wasOn != this.switchedOn | force))
        return;
      int num = (this.switchedOn ? 4 : 0) + (this.wasResetting ? 2 : 0) + (this.wasIncrementing ? 1 : 0);
      this.wasOn = this.switchedOn;
      component.Play(HashedString.op_Implicit("on_" + num.ToString()));
    }
  }

  public void OnLogicValueChanged(object data)
  {
    LogicValueChanged logicValueChanged = (LogicValueChanged) data;
    if (HashedString.op_Equality(logicValueChanged.portID, LogicCounter.INPUT_PORT_ID))
    {
      int newValue = logicValueChanged.newValue;
      this.receivedFirstSignal = true;
      if (LogicCircuitNetwork.IsBitActive(0, newValue))
      {
        if (!this.wasIncrementing)
        {
          this.wasIncrementing = true;
          if (!this.wasResetting)
          {
            if (this.currentCount == this.maxCount || this.currentCount >= 10)
              this.currentCount = 0;
            ++this.currentCount;
            this.UpdateMeter();
            this.SetCounterState();
            if (this.currentCount == this.maxCount && this.resetCountAtMax)
              this.resetRequested = true;
          }
        }
      }
      else
        this.wasIncrementing = false;
    }
    else
    {
      if (!HashedString.op_Equality(logicValueChanged.portID, LogicCounter.RESET_PORT_ID))
        return;
      int newValue = logicValueChanged.newValue;
      this.receivedFirstSignal = true;
      if (LogicCircuitNetwork.IsBitActive(0, newValue))
      {
        if (!this.wasResetting)
        {
          this.wasResetting = true;
          this.ResetCounter();
        }
      }
      else
        this.wasResetting = false;
    }
    this.UpdateVisualState(true);
    this.UpdateLogicCircuit();
  }

  protected override void UpdateSwitchStatus()
  {
    StatusItem status_item = this.switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
  }

  public void ResetCounter()
  {
    this.resetRequested = false;
    this.currentCount = 0;
    this.SetCounterState();
    this.UpdateVisualState(true);
    this.UpdateMeter();
    this.UpdateLogicCircuit();
  }

  public void LogicTick()
  {
    if (this.resetRequested)
      this.ResetCounter();
    if (!this.pulsingActive)
      return;
    --this.pulseTicksRemaining;
    if (this.pulseTicksRemaining > 0)
      return;
    this.pulsingActive = false;
    this.SetState(false);
    this.UpdateVisualState();
    this.UpdateMeter();
    this.UpdateLogicCircuit();
  }

  public void SetCounterState()
  {
    this.SetState(this.advancedMode ? this.currentCount % this.maxCount == 0 : this.currentCount == this.maxCount);
    if (!this.advancedMode || this.currentCount % this.maxCount != 0)
      return;
    this.pulsingActive = true;
    this.pulseTicksRemaining = 2;
  }
}
