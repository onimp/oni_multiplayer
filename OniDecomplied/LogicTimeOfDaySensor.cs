// Decompiled with JetBrains decompiler
// Type: LogicTimeOfDaySensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class LogicTimeOfDaySensor : Switch, ISaveLoadable, ISim200ms
{
  [SerializeField]
  [Serialize]
  public float startTime;
  [SerializeField]
  [Serialize]
  public float duration = 1f;
  private bool wasOn;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<LogicTimeOfDaySensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicTimeOfDaySensor>((Action<LogicTimeOfDaySensor, object>) ((component, data) => component.OnCopySettings(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<LogicTimeOfDaySensor>(-905833192, LogicTimeOfDaySensor.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    LogicTimeOfDaySensor component = ((GameObject) data).GetComponent<LogicTimeOfDaySensor>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.startTime = component.startTime;
    this.duration = component.duration;
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
    float cycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
    bool on = false;
    if ((double) cycleAsPercentage >= (double) this.startTime && (double) cycleAsPercentage < (double) this.startTime + (double) this.duration)
      on = true;
    if ((double) cycleAsPercentage < (double) this.startTime + (double) this.duration - 1.0)
      on = true;
    this.SetState(on);
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
}
