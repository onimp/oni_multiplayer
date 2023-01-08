// Decompiled with JetBrains decompiler
// Type: BuildingEnabledButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/BuildingEnabledButton")]
public class BuildingEnabledButton : KMonoBehaviour, ISaveLoadable, IToggleHandler
{
  [MyCmpAdd]
  private Toggleable Toggleable;
  [MyCmpReq]
  private Operational Operational;
  private int ToggleIdx;
  [Serialize]
  private bool buildingEnabled = true;
  [Serialize]
  private bool queuedToggle;
  public static readonly Operational.Flag EnabledFlag = new Operational.Flag("building_enabled", Operational.Flag.Type.Functional);
  private static readonly EventSystem.IntraObjectHandler<BuildingEnabledButton> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BuildingEnabledButton>((Action<BuildingEnabledButton, object>) ((component, data) => component.OnRefreshUserMenu(data)));

  public bool IsEnabled
  {
    get => Object.op_Inequality((Object) this.Operational, (Object) null) && this.Operational.GetFlag(BuildingEnabledButton.EnabledFlag);
    set
    {
      this.Operational.SetFlag(BuildingEnabledButton.EnabledFlag, value);
      Game.Instance.userMenu.Refresh(((Component) this).gameObject);
      this.buildingEnabled = value;
      ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.BuildingDisabled, !this.buildingEnabled);
      this.Trigger(1088293757, (object) this.buildingEnabled);
    }
  }

  public bool WaitingForDisable => this.IsEnabled && this.Toggleable.IsToggleQueued(this.ToggleIdx);

  protected virtual void OnPrefabInit()
  {
    this.ToggleIdx = this.Toggleable.SetTarget((IToggleHandler) this);
    this.Subscribe<BuildingEnabledButton>(493375141, BuildingEnabledButton.OnRefreshUserMenuDelegate);
  }

  protected virtual void OnSpawn()
  {
    this.IsEnabled = this.buildingEnabled;
    if (!this.queuedToggle)
      return;
    this.OnMenuToggle();
  }

  public void HandleToggle()
  {
    this.queuedToggle = false;
    Prioritizable.RemoveRef(((Component) this).gameObject);
    this.OnToggle();
  }

  public bool IsHandlerOn() => this.IsEnabled;

  private void OnToggle()
  {
    this.IsEnabled = !this.IsEnabled;
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
  }

  private void OnMenuToggle()
  {
    if (!this.Toggleable.IsToggleQueued(this.ToggleIdx))
    {
      if (this.IsEnabled)
        this.Trigger(2108245096, (object) "BuildingDisabled");
      this.queuedToggle = true;
      Prioritizable.AddRef(((Component) this).gameObject);
    }
    else
    {
      this.queuedToggle = false;
      Prioritizable.RemoveRef(((Component) this).gameObject);
    }
    this.Toggleable.Toggle(this.ToggleIdx);
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
  }

  private void OnRefreshUserMenu(object data)
  {
    bool isEnabled = this.IsEnabled;
    bool flag = this.Toggleable.IsToggleQueued(this.ToggleIdx);
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, isEnabled && !flag || !isEnabled & flag ? new KIconButtonMenu.ButtonInfo("action_building_disabled", (string) UI.USERMENUACTIONS.ENABLEBUILDING.NAME, new System.Action(this.OnMenuToggle), (Action) 166, tooltipText: ((string) UI.USERMENUACTIONS.ENABLEBUILDING.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_building_disabled", (string) UI.USERMENUACTIONS.ENABLEBUILDING.NAME_OFF, new System.Action(this.OnMenuToggle), (Action) 166, tooltipText: ((string) UI.USERMENUACTIONS.ENABLEBUILDING.TOOLTIP_OFF)));
  }
}
