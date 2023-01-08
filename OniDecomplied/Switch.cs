// Decompiled with JetBrains decompiler
// Type: Switch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Switch")]
public class Switch : KMonoBehaviour, ISaveLoadable, IToggleHandler
{
  [SerializeField]
  public bool manuallyControlled = true;
  [SerializeField]
  public bool defaultState = true;
  [Serialize]
  protected bool switchedOn = true;
  [MyCmpAdd]
  private Toggleable openSwitch;
  private int openToggleIndex;
  private static readonly EventSystem.IntraObjectHandler<Switch> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Switch>((Action<Switch, object>) ((component, data) => component.OnRefreshUserMenu(data)));

  public bool IsSwitchedOn => this.switchedOn;

  public event Action<bool> OnToggle;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.switchedOn = this.defaultState;
  }

  protected virtual void OnSpawn()
  {
    this.openToggleIndex = this.openSwitch.SetTarget((IToggleHandler) this);
    if (this.OnToggle != null)
      this.OnToggle(this.switchedOn);
    if (this.manuallyControlled)
      this.Subscribe<Switch>(493375141, Switch.OnRefreshUserMenuDelegate);
    this.UpdateSwitchStatus();
  }

  public void HandleToggle() => this.Toggle();

  public bool IsHandlerOn() => this.switchedOn;

  private void OnMinionToggle()
  {
    if (!DebugHandler.InstantBuildMode)
      this.openSwitch.Toggle(this.openToggleIndex);
    else
      this.Toggle();
  }

  protected virtual void Toggle() => this.SetState(!this.switchedOn);

  protected virtual void SetState(bool on)
  {
    if (this.switchedOn == on)
      return;
    this.switchedOn = on;
    this.UpdateSwitchStatus();
    if (this.OnToggle != null)
      this.OnToggle(this.switchedOn);
    if (!this.manuallyControlled)
      return;
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
  }

  protected virtual void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("action_power", (string) (this.switchedOn ? BUILDINGS.PREFABS.SWITCH.TURN_OFF : BUILDINGS.PREFABS.SWITCH.TURN_ON), new System.Action(this.OnMinionToggle), (Action) 166, tooltipText: ((string) (this.switchedOn ? BUILDINGS.PREFABS.SWITCH.TURN_OFF_TOOLTIP : BUILDINGS.PREFABS.SWITCH.TURN_ON_TOOLTIP))));

  protected virtual void UpdateSwitchStatus()
  {
    StatusItem status_item = this.switchedOn ? Db.Get().BuildingStatusItems.SwitchStatusActive : Db.Get().BuildingStatusItems.SwitchStatusInactive;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
  }
}
