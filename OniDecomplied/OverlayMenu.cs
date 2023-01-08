// Decompiled with JetBrains decompiler
// Type: OverlayMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OverlayMenu : KIconToggleMenu
{
  public static OverlayMenu Instance;
  private List<KIconToggleMenu.ToggleInfo> overlayToggleInfos;
  private UnityAction inputChangeReceiver;

  public static void DestroyInstance() => OverlayMenu.Instance = (OverlayMenu) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    OverlayMenu.Instance = this;
    this.InitializeToggles();
    this.Setup((IList<KIconToggleMenu.ToggleInfo>) this.overlayToggleInfos);
    Game.Instance.Subscribe(1798162660, new Action<object>(this.OnOverlayChanged));
    Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
    // ISSUE: method pointer
    KInputManager.InputChange.AddListener(new UnityAction((object) this, __methodptr(Refresh)));
    this.onSelect += new KIconToggleMenu.OnSelect(this.OnToggleSelect);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.RefreshButtons();
  }

  public void Refresh() => this.RefreshButtons();

  protected override void RefreshButtons()
  {
    base.RefreshButtons();
    if (Object.op_Equality((Object) Research.Instance, (Object) null))
      return;
    foreach (KIconToggleMenu.ToggleInfo overlayToggleInfo1 in this.overlayToggleInfos)
    {
      OverlayMenu.OverlayToggleInfo overlayToggleInfo2 = (OverlayMenu.OverlayToggleInfo) overlayToggleInfo1;
      ((Component) overlayToggleInfo1.toggle).gameObject.SetActive(overlayToggleInfo2.IsUnlocked());
      overlayToggleInfo1.tooltip = GameUtil.ReplaceHotkeyString(overlayToggleInfo2.originalToolTipText, overlayToggleInfo1.hotKey);
    }
  }

  private void OnResearchComplete(object data) => this.RefreshButtons();

  protected virtual void OnForcedCleanUp()
  {
    // ISSUE: method pointer
    KInputManager.InputChange.RemoveListener(new UnityAction((object) this, __methodptr(Refresh)));
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Game.Instance.Unsubscribe(1798162660, new Action<object>(this.OnOverlayChanged));
  }

  private void InitializeToggleGroups()
  {
  }

  private void InitializeToggles()
  {
    this.overlayToggleInfos = new List<KIconToggleMenu.ToggleInfo>()
    {
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.OXYGEN.BUTTON, "overlay_oxygen", OverlayModes.Oxygen.ID, hotKey: ((Action) 119), tooltip: ((string) UI.TOOLTIPS.OXYGENOVERLAYSTRING), tooltip_header: ((string) UI.OVERLAYS.OXYGEN.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.ELECTRICAL.BUTTON, "overlay_power", OverlayModes.Power.ID, hotKey: ((Action) 120), tooltip: ((string) UI.TOOLTIPS.POWEROVERLAYSTRING), tooltip_header: ((string) UI.OVERLAYS.ELECTRICAL.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.TEMPERATURE.BUTTON, "overlay_temperature", OverlayModes.Temperature.ID, hotKey: ((Action) 121), tooltip: ((string) UI.TOOLTIPS.TEMPERATUREOVERLAYSTRING), tooltip_header: ((string) UI.OVERLAYS.TEMPERATURE.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.TILEMODE.BUTTON, "overlay_materials", OverlayModes.TileMode.ID, hotKey: ((Action) 122), tooltip: ((string) UI.TOOLTIPS.TILEMODE_OVERLAY_STRING), tooltip_header: ((string) UI.OVERLAYS.TILEMODE.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.LIGHTING.BUTTON, "overlay_lights", OverlayModes.Light.ID, hotKey: ((Action) 123), tooltip: ((string) UI.TOOLTIPS.LIGHTSOVERLAYSTRING), tooltip_header: ((string) UI.OVERLAYS.LIGHTING.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.LIQUIDPLUMBING.BUTTON, "overlay_liquidvent", OverlayModes.LiquidConduits.ID, hotKey: ((Action) 124), tooltip: ((string) UI.TOOLTIPS.LIQUIDVENTOVERLAYSTRING), tooltip_header: ((string) UI.OVERLAYS.LIQUIDPLUMBING.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.GASPLUMBING.BUTTON, "overlay_gasvent", OverlayModes.GasConduits.ID, hotKey: ((Action) 125), tooltip: ((string) UI.TOOLTIPS.GASVENTOVERLAYSTRING), tooltip_header: ((string) UI.OVERLAYS.GASPLUMBING.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.DECOR.BUTTON, "overlay_decor", OverlayModes.Decor.ID, hotKey: ((Action) 126), tooltip: ((string) UI.TOOLTIPS.DECOROVERLAYSTRING), tooltip_header: ((string) UI.OVERLAYS.DECOR.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.DISEASE.BUTTON, "overlay_disease", OverlayModes.Disease.ID, hotKey: ((Action) (int) sbyte.MaxValue), tooltip: ((string) UI.TOOLTIPS.DISEASEOVERLAYSTRING), tooltip_header: ((string) UI.OVERLAYS.DISEASE.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.CROPS.BUTTON, "overlay_farming", OverlayModes.Crop.ID, hotKey: ((Action) 128), tooltip: ((string) UI.TOOLTIPS.CROPS_OVERLAY_STRING), tooltip_header: ((string) UI.OVERLAYS.CROPS.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.ROOMS.BUTTON, "overlay_rooms", OverlayModes.Rooms.ID, hotKey: ((Action) 129), tooltip: ((string) UI.TOOLTIPS.ROOMSOVERLAYSTRING), tooltip_header: ((string) UI.OVERLAYS.ROOMS.BUTTON)),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.SUIT.BUTTON, "overlay_suit", OverlayModes.Suit.ID, "SuitsOverlay", (Action) 130, (string) UI.TOOLTIPS.SUITOVERLAYSTRING, (string) UI.OVERLAYS.SUIT.BUTTON),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.LOGIC.BUTTON, "overlay_logic", OverlayModes.Logic.ID, "AutomationOverlay", (Action) 131, (string) UI.TOOLTIPS.LOGICOVERLAYSTRING, (string) UI.OVERLAYS.LOGIC.BUTTON),
      (KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.CONVEYOR.BUTTON, "overlay_conveyor", OverlayModes.SolidConveyor.ID, "ConveyorOverlay", (Action) 132, (string) UI.TOOLTIPS.CONVEYOR_OVERLAY_STRING, (string) UI.OVERLAYS.CONVEYOR.BUTTON)
    };
    if (!Sim.IsRadiationEnabled())
      return;
    this.overlayToggleInfos.Add((KIconToggleMenu.ToggleInfo) new OverlayMenu.OverlayToggleInfo((string) UI.OVERLAYS.RADIATION.BUTTON, "overlay_radiation", OverlayModes.Radiation.ID, hotKey: ((Action) 133), tooltip: ((string) UI.TOOLTIPS.RADIATIONOVERLAYSTRING), tooltip_header: ((string) UI.OVERLAYS.RADIATION.BUTTON)));
  }

  private void OnToggleSelect(KIconToggleMenu.ToggleInfo toggle_info)
  {
    if (HashedString.op_Equality(SimDebugView.Instance.GetMode(), ((OverlayMenu.OverlayToggleInfo) toggle_info).simView))
    {
      OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
    }
    else
    {
      if (!((OverlayMenu.OverlayToggleInfo) toggle_info).IsUnlocked())
        return;
      OverlayScreen.Instance.ToggleOverlay(((OverlayMenu.OverlayToggleInfo) toggle_info).simView);
    }
  }

  private void OnOverlayChanged(object overlay_data)
  {
    HashedString hashedString = (HashedString) overlay_data;
    for (int index = 0; index < this.overlayToggleInfos.Count; ++index)
      this.overlayToggleInfos[index].toggle.isOn = HashedString.op_Equality(((OverlayMenu.OverlayToggleInfo) this.overlayToggleInfos[index]).simView, hashedString);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed)
      return;
    if (HashedString.op_Inequality(OverlayScreen.Instance.GetMode(), OverlayModes.None.ID) && e.TryConsume((Action) 1))
      OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyDown(e);
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed)
      return;
    if (HashedString.op_Inequality(OverlayScreen.Instance.GetMode(), OverlayModes.None.ID) && PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
      OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyUp(e);
  }

  private class OverlayToggleGroup : KIconToggleMenu.ToggleInfo
  {
    public List<OverlayMenu.OverlayToggleInfo> toggleInfoGroup;
    public string requiredTechItem;
    [SerializeField]
    private int activeToggleInfo;

    public OverlayToggleGroup(
      string text,
      string icon_name,
      List<OverlayMenu.OverlayToggleInfo> toggle_group,
      string required_tech_item = "",
      Action hot_key = 275,
      string tooltip = "",
      string tooltip_header = "")
      : base(text, icon_name, hotkey: hot_key, tooltip: tooltip, tooltip_header: tooltip_header)
    {
      this.toggleInfoGroup = toggle_group;
    }

    public bool IsUnlocked() => DebugHandler.InstantBuildMode || string.IsNullOrEmpty(this.requiredTechItem) || Db.Get().Techs.IsTechItemComplete(this.requiredTechItem);

    public OverlayMenu.OverlayToggleInfo GetActiveToggleInfo() => this.toggleInfoGroup[this.activeToggleInfo];
  }

  private class OverlayToggleInfo : KIconToggleMenu.ToggleInfo
  {
    public HashedString simView;
    public string requiredTechItem;
    public string originalToolTipText;

    public OverlayToggleInfo(
      string text,
      string icon_name,
      HashedString sim_view,
      string required_tech_item = "",
      Action hotKey = 275,
      string tooltip = "",
      string tooltip_header = "")
      : base(text, icon_name, hotkey: hotKey, tooltip: tooltip, tooltip_header: tooltip_header)
    {
      this.originalToolTipText = tooltip;
      tooltip = GameUtil.ReplaceHotkeyString(tooltip, hotKey);
      this.simView = sim_view;
      this.requiredTechItem = required_tech_item;
    }

    public bool IsUnlocked() => DebugHandler.InstantBuildMode || string.IsNullOrEmpty(this.requiredTechItem) || Db.Get().Techs.IsTechItemComplete(this.requiredTechItem);
  }
}
