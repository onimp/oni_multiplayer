// Decompiled with JetBrains decompiler
// Type: LogicSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections;
using UnityEngine;

[SerializationConfig]
public class LogicSwitch : Switch, IPlayerControlledToggle, ISim33ms
{
  public static readonly HashedString PORT_ID = HashedString.op_Implicit(nameof (LogicSwitch));
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<LogicSwitch> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicSwitch>((Action<LogicSwitch, object>) ((component, data) => component.OnCopySettings(data)));
  private bool wasOn;
  private System.Action firstFrameCallback;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<LogicSwitch>(-905833192, LogicSwitch.OnCopySettingsDelegate);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.wasOn = this.switchedOn;
    this.UpdateLogicCircuit();
    ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit(this.switchedOn ? "on" : "off"));
  }

  protected virtual void OnCleanUp() => base.OnCleanUp();

  private void OnCopySettings(object data)
  {
    LogicSwitch component = ((GameObject) data).GetComponent<LogicSwitch>();
    if (!Object.op_Inequality((Object) component, (Object) null) || this.switchedOn == component.switchedOn)
      return;
    this.switchedOn = component.switchedOn;
    this.UpdateVisualization();
    this.UpdateLogicCircuit();
  }

  protected override void Toggle()
  {
    base.Toggle();
    this.UpdateVisualization();
    this.UpdateLogicCircuit();
  }

  private void UpdateVisualization()
  {
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    if (this.wasOn != this.switchedOn)
    {
      component.Play(HashedString.op_Implicit(this.switchedOn ? "on_pre" : "on_pst"));
      component.Queue(HashedString.op_Implicit(this.switchedOn ? "on" : "off"));
    }
    this.wasOn = this.switchedOn;
  }

  private void UpdateLogicCircuit() => ((Component) this).GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);

  protected override void UpdateSwitchStatus()
  {
    StatusItem status_item = this.switchedOn ? Db.Get().BuildingStatusItems.LogicSwitchStatusActive : Db.Get().BuildingStatusItems.LogicSwitchStatusInactive;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
  }

  public void Sim33ms(float dt)
  {
    if (!this.ToggleRequested)
      return;
    this.Toggle();
    this.ToggleRequested = false;
    this.GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, (StatusItem) null);
  }

  public void SetFirstFrameCallback(System.Action ffCb)
  {
    this.firstFrameCallback = ffCb;
    ((MonoBehaviour) this).StartCoroutine(this.RunCallback());
  }

  private IEnumerator RunCallback()
  {
    yield return (object) null;
    if (this.firstFrameCallback != null)
    {
      this.firstFrameCallback();
      this.firstFrameCallback = (System.Action) null;
    }
    yield return (object) null;
  }

  public void ToggledByPlayer() => this.Toggle();

  public bool ToggledOn() => this.switchedOn;

  public KSelectable GetSelectable() => ((Component) this).GetComponent<KSelectable>();

  public string SideScreenTitleKey => "STRINGS.BUILDINGS.PREFABS.LOGICSWITCH.SIDESCREEN_TITLE";

  public bool ToggleRequested { get; set; }
}
