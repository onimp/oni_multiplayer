// Decompiled with JetBrains decompiler
// Type: ManagementMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ManagementMenu : KIconToggleMenu
{
  private const float UI_WIDTH_COMPRESS_THRESHOLD = 1300f;
  [MyCmpReq]
  public ManagementMenuNotificationDisplayer notificationDisplayer;
  public static ManagementMenu Instance;
  [Header("Management Menu Specific")]
  [SerializeField]
  private KToggle smallPrefab;
  public KToggle PauseMenuButton;
  [Header("Top Right Screen References")]
  public JobsTableScreen jobsScreen;
  public VitalsTableScreen vitalsScreen;
  public ScheduleScreen scheduleScreen;
  public ReportScreen reportsScreen;
  public CodexScreen codexScreen;
  public ConsumablesTableScreen consumablesScreen;
  private StarmapScreen starmapScreen;
  private ClusterMapScreen clusterMapScreen;
  private SkillsScreen skillsScreen;
  private ResearchScreen researchScreen;
  [Header("Notification Styles")]
  public ColorStyleSetting noAlertColorStyle;
  public List<ColorStyleSetting> alertColorStyle;
  public List<TextStyleSetting> alertTextStyle;
  private ManagementMenu.ManagementMenuToggleInfo jobsInfo;
  private ManagementMenu.ManagementMenuToggleInfo consumablesInfo;
  private ManagementMenu.ManagementMenuToggleInfo scheduleInfo;
  private ManagementMenu.ManagementMenuToggleInfo vitalsInfo;
  private ManagementMenu.ManagementMenuToggleInfo reportsInfo;
  private ManagementMenu.ManagementMenuToggleInfo researchInfo;
  private ManagementMenu.ManagementMenuToggleInfo codexInfo;
  private ManagementMenu.ManagementMenuToggleInfo starmapInfo;
  private ManagementMenu.ManagementMenuToggleInfo clusterMapInfo;
  private ManagementMenu.ManagementMenuToggleInfo skillsInfo;
  private ManagementMenu.ManagementMenuToggleInfo[] fullscreenUIs;
  private Dictionary<ManagementMenu.ManagementMenuToggleInfo, ManagementMenu.ScreenData> ScreenInfoMatch = new Dictionary<ManagementMenu.ManagementMenuToggleInfo, ManagementMenu.ScreenData>();
  private ManagementMenu.ScreenData activeScreen;
  private KButton activeButton;
  private string skillsTooltip;
  private string skillsTooltipDisabled;
  private string researchTooltip;
  private string researchTooltipDisabled;
  private string starmapTooltip;
  private string starmapTooltipDisabled;
  private string clusterMapTooltip;
  private string clusterMapTooltipDisabled;
  private List<KScreen> mutuallyExclusiveScreens = new List<KScreen>();

  public static void DestroyInstance() => ManagementMenu.Instance = (ManagementMenu) null;

  public virtual float GetSortKey() => 21f;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ManagementMenu.Instance = this;
    this.notificationDisplayer.onNotificationsChanged += new System.Action(this.OnNotificationsChanged);
    CodexCache.CodexCacheInit();
    ScheduledUIInstantiation component = ((Component) GameScreenManager.Instance).GetComponent<ScheduledUIInstantiation>();
    this.starmapScreen = component.GetInstantiatedObject<StarmapScreen>();
    this.clusterMapScreen = component.GetInstantiatedObject<ClusterMapScreen>();
    this.skillsScreen = component.GetInstantiatedObject<SkillsScreen>();
    this.researchScreen = component.GetInstantiatedObject<ResearchScreen>();
    this.fullscreenUIs = new ManagementMenu.ManagementMenuToggleInfo[4]
    {
      this.researchInfo,
      this.skillsInfo,
      this.starmapInfo,
      this.clusterMapInfo
    };
    ((KMonoBehaviour) this).Subscribe(((Component) Game.Instance).gameObject, 288942073, new Action<object>(this.OnUIClear));
    this.consumablesInfo = new ManagementMenu.ManagementMenuToggleInfo((string) STRINGS.UI.CONSUMABLES, "OverviewUI_consumables_icon", hotkey: ((Action) 109), tooltip: ((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_CONSUMABLES));
    this.AddToggleTooltip(this.consumablesInfo);
    this.vitalsInfo = new ManagementMenu.ManagementMenuToggleInfo((string) STRINGS.UI.VITALS, "OverviewUI_vitals_icon", hotkey: ((Action) 110), tooltip: ((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_VITALS));
    this.AddToggleTooltip(this.vitalsInfo);
    this.researchInfo = new ManagementMenu.ManagementMenuToggleInfo((string) STRINGS.UI.RESEARCH, "OverviewUI_research_nav_icon", hotkey: ((Action) 112), tooltip: ((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH));
    this.AddToggleTooltip(this.researchInfo, (string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_RESEARCH);
    this.jobsInfo = new ManagementMenu.ManagementMenuToggleInfo((string) STRINGS.UI.JOBS, "OverviewUI_priority_icon", hotkey: ((Action) 108), tooltip: ((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_JOBS));
    this.AddToggleTooltip(this.jobsInfo);
    this.skillsInfo = new ManagementMenu.ManagementMenuToggleInfo((string) STRINGS.UI.SKILLS, "OverviewUI_jobs_icon", hotkey: ((Action) 116), tooltip: ((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_SKILLS));
    this.AddToggleTooltip(this.skillsInfo, (string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_SKILL_STATION);
    this.starmapInfo = new ManagementMenu.ManagementMenuToggleInfo((string) STRINGS.UI.STARMAP.MANAGEMENT_BUTTON, "OverviewUI_starmap_icon", hotkey: ((Action) 117), tooltip: ((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_STARMAP));
    this.AddToggleTooltip(this.starmapInfo, (string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_TELESCOPE);
    this.clusterMapInfo = new ManagementMenu.ManagementMenuToggleInfo((string) STRINGS.UI.STARMAP.MANAGEMENT_BUTTON, "OverviewUI_starmap_icon", hotkey: ((Action) 117), tooltip: ((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_STARMAP));
    this.AddToggleTooltip(this.clusterMapInfo);
    this.scheduleInfo = new ManagementMenu.ManagementMenuToggleInfo((string) STRINGS.UI.SCHEDULE, "OverviewUI_schedule2_icon", hotkey: ((Action) 113), tooltip: ((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_SCHEDULE));
    this.AddToggleTooltip(this.scheduleInfo);
    this.reportsInfo = new ManagementMenu.ManagementMenuToggleInfo((string) STRINGS.UI.REPORT, "OverviewUI_reports_icon", hotkey: ((Action) 114), tooltip: ((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_DAILYREPORT));
    this.AddToggleTooltip(this.reportsInfo);
    this.reportsInfo.prefabOverride = this.smallPrefab;
    this.codexInfo = new ManagementMenu.ManagementMenuToggleInfo((string) STRINGS.UI.CODEX.MANAGEMENT_BUTTON, "OverviewUI_database_icon", hotkey: ((Action) 115), tooltip: ((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_CODEX));
    this.AddToggleTooltip(this.codexInfo);
    this.codexInfo.prefabOverride = this.smallPrefab;
    this.ScreenInfoMatch.Add(this.consumablesInfo, new ManagementMenu.ScreenData()
    {
      screen = (KScreen) this.consumablesScreen,
      tabIdx = 3,
      toggleInfo = this.consumablesInfo,
      cancelHandler = (Func<bool>) null
    });
    this.ScreenInfoMatch.Add(this.vitalsInfo, new ManagementMenu.ScreenData()
    {
      screen = (KScreen) this.vitalsScreen,
      tabIdx = 2,
      toggleInfo = this.vitalsInfo,
      cancelHandler = (Func<bool>) null
    });
    this.ScreenInfoMatch.Add(this.reportsInfo, new ManagementMenu.ScreenData()
    {
      screen = (KScreen) this.reportsScreen,
      tabIdx = 4,
      toggleInfo = this.reportsInfo,
      cancelHandler = (Func<bool>) null
    });
    this.ScreenInfoMatch.Add(this.jobsInfo, new ManagementMenu.ScreenData()
    {
      screen = (KScreen) this.jobsScreen,
      tabIdx = 1,
      toggleInfo = this.jobsInfo,
      cancelHandler = (Func<bool>) null
    });
    this.ScreenInfoMatch.Add(this.skillsInfo, new ManagementMenu.ScreenData()
    {
      screen = (KScreen) this.skillsScreen,
      tabIdx = 0,
      toggleInfo = this.skillsInfo,
      cancelHandler = (Func<bool>) null
    });
    this.ScreenInfoMatch.Add(this.codexInfo, new ManagementMenu.ScreenData()
    {
      screen = (KScreen) this.codexScreen,
      tabIdx = 6,
      toggleInfo = this.codexInfo,
      cancelHandler = (Func<bool>) null
    });
    this.ScreenInfoMatch.Add(this.scheduleInfo, new ManagementMenu.ScreenData()
    {
      screen = (KScreen) this.scheduleScreen,
      tabIdx = 7,
      toggleInfo = this.scheduleInfo,
      cancelHandler = (Func<bool>) null
    });
    if (DlcManager.FeatureClusterSpaceEnabled())
      this.ScreenInfoMatch.Add(this.clusterMapInfo, new ManagementMenu.ScreenData()
      {
        screen = (KScreen) this.clusterMapScreen,
        tabIdx = 7,
        toggleInfo = this.clusterMapInfo,
        cancelHandler = new Func<bool>(this.clusterMapScreen.TryHandleCancel)
      });
    else
      this.ScreenInfoMatch.Add(this.starmapInfo, new ManagementMenu.ScreenData()
      {
        screen = (KScreen) this.starmapScreen,
        tabIdx = 7,
        toggleInfo = this.starmapInfo,
        cancelHandler = (Func<bool>) null
      });
    this.ScreenInfoMatch.Add(this.researchInfo, new ManagementMenu.ScreenData()
    {
      screen = (KScreen) this.researchScreen,
      tabIdx = 5,
      toggleInfo = this.researchInfo,
      cancelHandler = (Func<bool>) null
    });
    List<KIconToggleMenu.ToggleInfo> toggleInfo = new List<KIconToggleMenu.ToggleInfo>();
    toggleInfo.Add((KIconToggleMenu.ToggleInfo) this.vitalsInfo);
    toggleInfo.Add((KIconToggleMenu.ToggleInfo) this.consumablesInfo);
    toggleInfo.Add((KIconToggleMenu.ToggleInfo) this.scheduleInfo);
    toggleInfo.Add((KIconToggleMenu.ToggleInfo) this.jobsInfo);
    toggleInfo.Add((KIconToggleMenu.ToggleInfo) this.skillsInfo);
    toggleInfo.Add((KIconToggleMenu.ToggleInfo) this.researchInfo);
    if (DlcManager.FeatureClusterSpaceEnabled())
      toggleInfo.Add((KIconToggleMenu.ToggleInfo) this.clusterMapInfo);
    else
      toggleInfo.Add((KIconToggleMenu.ToggleInfo) this.starmapInfo);
    toggleInfo.Add((KIconToggleMenu.ToggleInfo) this.reportsInfo);
    toggleInfo.Add((KIconToggleMenu.ToggleInfo) this.codexInfo);
    this.Setup((IList<KIconToggleMenu.ToggleInfo>) toggleInfo);
    this.onSelect += new KIconToggleMenu.OnSelect(this.OnButtonClick);
    this.PauseMenuButton.onClick += new System.Action(this.OnPauseMenuClicked);
    ((Component) this.PauseMenuButton).transform.SetAsLastSibling();
    ((Component) this.PauseMenuButton).GetComponent<ToolTip>().toolTip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_PAUSEMENU, (Action) 1);
    // ISSUE: method pointer
    KInputManager.InputChange.AddListener(new UnityAction((object) this, __methodptr(OnInputChanged)));
    Components.ResearchCenters.OnAdd += new Action<IResearchCenter>(this.CheckResearch);
    Components.ResearchCenters.OnRemove += new Action<IResearchCenter>(this.CheckResearch);
    Components.RoleStations.OnAdd += new Action<RoleStation>(this.CheckSkills);
    Components.RoleStations.OnRemove += new Action<RoleStation>(this.CheckSkills);
    Game.Instance.Subscribe(-809948329, new Action<object>(this.CheckResearch));
    Game.Instance.Subscribe(-809948329, new Action<object>(this.CheckSkills));
    Game.Instance.Subscribe(445618876, new Action<object>(this.OnResolutionChanged));
    if (!DlcManager.FeatureClusterSpaceEnabled())
    {
      Components.Telescopes.OnAdd += new Action<Telescope>(this.CheckStarmap);
      Components.Telescopes.OnRemove += new Action<Telescope>(this.CheckStarmap);
    }
    this.CheckResearch((object) null);
    this.CheckSkills();
    if (!DlcManager.FeatureClusterSpaceEnabled())
      this.CheckStarmap();
    this.researchInfo.toggle.soundPlayer.AcceptClickCondition = (Func<bool>) (() => this.ResearchAvailable() || this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.researchInfo]);
    foreach (KToggle toggle in this.toggles)
    {
      toggle.soundPlayer.toggle_widget_sound_events[0].PlaySound = false;
      toggle.soundPlayer.toggle_widget_sound_events[1].PlaySound = false;
    }
    this.OnResolutionChanged();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.mutuallyExclusiveScreens.Add((KScreen) AllResourcesScreen.Instance);
    this.mutuallyExclusiveScreens.Add((KScreen) AllDiagnosticsScreen.Instance);
    this.OnNotificationsChanged();
  }

  protected virtual void OnForcedCleanUp()
  {
    // ISSUE: method pointer
    KInputManager.InputChange.RemoveListener(new UnityAction((object) this, __methodptr(OnInputChanged)));
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  private void OnInputChanged()
  {
    ((Component) this.PauseMenuButton).GetComponent<ToolTip>().toolTip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_PAUSEMENU, (Action) 1);
    this.consumablesInfo.tooltip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_CONSUMABLES, this.consumablesInfo.hotKey);
    this.vitalsInfo.tooltip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_VITALS, this.vitalsInfo.hotKey);
    this.researchInfo.tooltip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH, this.researchInfo.hotKey);
    this.jobsInfo.tooltip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_JOBS, this.jobsInfo.hotKey);
    this.skillsInfo.tooltip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_SKILLS, this.skillsInfo.hotKey);
    this.starmapInfo.tooltip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_STARMAP, this.starmapInfo.hotKey);
    this.clusterMapInfo.tooltip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_STARMAP, this.clusterMapInfo.hotKey);
    this.scheduleInfo.tooltip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_SCHEDULE, this.scheduleInfo.hotKey);
    this.reportsInfo.tooltip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_DAILYREPORT, this.reportsInfo.hotKey);
    this.codexInfo.tooltip = GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.MANAGEMENTMENU_CODEX, this.codexInfo.hotKey);
  }

  private void OnResolutionChanged(object data = null)
  {
    bool flag = (double) Screen.width < 1300.0;
    foreach (Component toggle in this.toggles)
    {
      HierarchyReferences component = toggle.GetComponent<HierarchyReferences>();
      if (!Object.op_Equality((Object) component, (Object) null))
      {
        RectTransform reference = component.GetReference<RectTransform>("TextContainer");
        if (!Object.op_Equality((Object) reference, (Object) null))
          ((Component) reference).gameObject.SetActive(!flag);
      }
    }
  }

  private void OnNotificationsChanged()
  {
    foreach (KeyValuePair<ManagementMenu.ManagementMenuToggleInfo, ManagementMenu.ScreenData> keyValuePair in this.ScreenInfoMatch)
      keyValuePair.Key.SetNotificationDisplay(false, false, (ColorStyleSetting) null, this.noAlertColorStyle);
  }

  private void AddToggleTooltip(
    ManagementMenu.ManagementMenuToggleInfo toggleInfo,
    string disabledTooltip = null)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    ManagementMenu.\u003C\u003Ec__DisplayClass51_0 cDisplayClass510 = new ManagementMenu.\u003C\u003Ec__DisplayClass51_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass510.disabledTooltip = disabledTooltip;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass510.toggleInfo = toggleInfo;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    cDisplayClass510.toggleInfo.getTooltipText = new ToolTip.ComplexTooltipDelegate((object) cDisplayClass510, __methodptr(\u003CAddToggleTooltip\u003Eb__0));
  }

  public bool IsFullscreenUIActive()
  {
    if (this.activeScreen == null)
      return false;
    foreach (ManagementMenu.ManagementMenuToggleInfo fullscreenUi in this.fullscreenUIs)
    {
      if (this.activeScreen.toggleInfo == fullscreenUi)
        return true;
    }
    return false;
  }

  private void OnPauseMenuClicked()
  {
    PauseScreen.Instance.Show(true);
    this.PauseMenuButton.isOn = false;
  }

  public void CheckResearch(object o)
  {
    if (Object.op_Equality((Object) this.researchInfo.toggle, (Object) null))
      return;
    bool disabled = Components.ResearchCenters.Count <= 0 && !DebugHandler.InstantBuildMode;
    bool active = !disabled && this.activeScreen != null && this.activeScreen.toggleInfo == this.researchInfo;
    this.ConfigureToggle(this.researchInfo.toggle, disabled, active);
  }

  public void CheckSkills(object o = null)
  {
    if (Object.op_Equality((Object) this.skillsInfo.toggle, (Object) null))
      return;
    this.ConfigureToggle(this.skillsInfo.toggle, Components.RoleStations.Count <= 0 && !DebugHandler.InstantBuildMode, this.activeScreen != null && this.activeScreen.toggleInfo == this.skillsInfo);
  }

  public void CheckStarmap(object o = null)
  {
    if (Object.op_Equality((Object) this.starmapInfo.toggle, (Object) null))
      return;
    this.ConfigureToggle(this.starmapInfo.toggle, Components.Telescopes.Count <= 0 && !DebugHandler.InstantBuildMode, this.activeScreen != null && this.activeScreen.toggleInfo == this.starmapInfo);
  }

  private void ConfigureToggle(KToggle toggle, bool disabled, bool active)
  {
    ((Selectable) toggle).interactable = !disabled;
    if (disabled)
      ((Component) toggle).GetComponentInChildren<ImageToggleState>().SetDisabled();
    else
      ((Component) toggle).GetComponentInChildren<ImageToggleState>().SetActiveState(active);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (this.activeScreen != null && e.TryConsume((Action) 1))
      this.ToggleIfCancelUnhandled(this.activeScreen);
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyDown(e);
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (this.activeScreen != null && PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
      this.ToggleIfCancelUnhandled(this.activeScreen);
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyUp(e);
  }

  private void ToggleIfCancelUnhandled(ManagementMenu.ScreenData screenData)
  {
    if (screenData.cancelHandler != null && screenData.cancelHandler())
      return;
    this.ToggleScreen(screenData);
  }

  private bool ResearchAvailable() => Components.ResearchCenters.Count > 0 || DebugHandler.InstantBuildMode;

  private bool SkillsAvailable() => Components.RoleStations.Count > 0 || DebugHandler.InstantBuildMode;

  public static bool StarmapAvailable() => Components.Telescopes.Count > 0 || DebugHandler.InstantBuildMode;

  public void CloseAll()
  {
    if (this.activeScreen == null)
      return;
    if (this.activeScreen.toggleInfo != null)
      this.ToggleScreen(this.activeScreen);
    this.CloseActive();
    this.ClearSelection();
  }

  private void OnUIClear(object data) => this.CloseAll();

  public void ToggleScreen(ManagementMenu.ScreenData screenData)
  {
    if (screenData == null)
      return;
    if (screenData.toggleInfo == this.researchInfo && !this.ResearchAvailable())
    {
      this.CheckResearch((object) null);
      this.CloseActive();
    }
    else if (screenData.toggleInfo == this.skillsInfo && !this.SkillsAvailable())
    {
      this.CheckSkills();
      this.CloseActive();
    }
    else if (screenData.toggleInfo == this.starmapInfo && !ManagementMenu.StarmapAvailable())
    {
      this.CheckStarmap();
      this.CloseActive();
    }
    else
    {
      if (((Component) screenData.toggleInfo.toggle).gameObject.GetComponentInChildren<ImageToggleState>().IsDisabled)
        return;
      if (this.activeScreen != null)
      {
        this.activeScreen.toggleInfo.toggle.isOn = false;
        ((Component) this.activeScreen.toggleInfo.toggle).gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
      }
      if (this.activeScreen != screenData)
      {
        OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
        if (this.activeScreen != null)
          this.activeScreen.toggleInfo.toggle.ActivateFlourish(false);
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open"));
        AudioMixer.instance.Start(AudioMixerSnapshots.Get().MenuOpenMigrated);
        screenData.toggleInfo.toggle.ActivateFlourish(true);
        ((Component) screenData.toggleInfo.toggle).gameObject.GetComponentInChildren<ImageToggleState>().SetActive();
        this.CloseActive();
        this.activeScreen = screenData;
        if (!this.activeScreen.screen.IsActive())
          this.activeScreen.screen.Activate();
        this.activeScreen.screen.Show(true);
        foreach (ManagementMenuNotification menuNotification in this.notificationDisplayer.GetNotificationsForAction(screenData.toggleInfo.hotKey))
        {
          if (menuNotification.customClickCallback != null)
          {
            menuNotification.customClickCallback(menuNotification.customClickData);
            break;
          }
        }
        foreach (KScreen mutuallyExclusiveScreen in this.mutuallyExclusiveScreens)
          mutuallyExclusiveScreen.Show(false);
      }
      else
      {
        this.activeScreen.screen.Show(false);
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
        AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MenuOpenMigrated);
        this.activeScreen.toggleInfo.toggle.ActivateFlourish(false);
        this.activeScreen = (ManagementMenu.ScreenData) null;
        ((Component) screenData.toggleInfo.toggle).gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
      }
    }
  }

  public void OnButtonClick(KIconToggleMenu.ToggleInfo toggle_info) => this.ToggleScreen(this.ScreenInfoMatch[(ManagementMenu.ManagementMenuToggleInfo) toggle_info]);

  private void CloseActive()
  {
    if (this.activeScreen == null)
      return;
    this.activeScreen.toggleInfo.toggle.isOn = false;
    this.activeScreen.screen.Show(false);
    this.activeScreen = (ManagementMenu.ScreenData) null;
  }

  public void ToggleResearch()
  {
    if (!this.ResearchAvailable() && this.activeScreen != this.ScreenInfoMatch[ManagementMenu.Instance.researchInfo] || this.researchInfo == null)
      return;
    this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.researchInfo]);
  }

  public void ToggleCodex() => this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.codexInfo]);

  public void OpenCodexToEntry(string id, ContentContainer targetContainer = null)
  {
    if (!((Component) this.codexScreen).gameObject.activeInHierarchy)
      this.ToggleCodex();
    this.codexScreen.ChangeArticle(id, targetPosition: new Vector3());
    this.codexScreen.FocusContainer(targetContainer);
  }

  public void ToggleSkills()
  {
    if (!this.SkillsAvailable() && this.activeScreen != this.ScreenInfoMatch[ManagementMenu.Instance.skillsInfo] || this.skillsInfo == null)
      return;
    this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.skillsInfo]);
  }

  public void ToggleStarmap()
  {
    if (this.starmapInfo == null)
      return;
    this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.starmapInfo]);
  }

  public void ToggleClusterMap() => this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.clusterMapInfo]);

  public void TogglePriorities() => this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.jobsInfo]);

  public void OpenReports(int day)
  {
    if (this.activeScreen != this.ScreenInfoMatch[ManagementMenu.Instance.reportsInfo])
      this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.reportsInfo]);
    ReportScreen.Instance.ShowReport(day);
  }

  public void OpenResearch()
  {
    if (this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.researchInfo])
      return;
    this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.researchInfo]);
  }

  public void OpenStarmap()
  {
    if (this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.starmapInfo])
      return;
    this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.starmapInfo]);
  }

  public void OpenClusterMap()
  {
    if (this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.clusterMapInfo])
      return;
    this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.clusterMapInfo]);
  }

  public void CloseClusterMap()
  {
    if (this.activeScreen != this.ScreenInfoMatch[ManagementMenu.Instance.clusterMapInfo])
      return;
    this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.clusterMapInfo]);
  }

  public void OpenSkills(MinionIdentity minionIdentity)
  {
    this.skillsScreen.CurrentlySelectedMinion = (IAssignableIdentity) minionIdentity;
    if (this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.skillsInfo])
      return;
    this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.skillsInfo]);
  }

  public bool IsScreenOpen(KScreen screen) => this.activeScreen != null && Object.op_Equality((Object) this.activeScreen.screen, (Object) screen);

  public class ScreenData
  {
    public KScreen screen;
    public ManagementMenu.ManagementMenuToggleInfo toggleInfo;
    public Func<bool> cancelHandler;
    public int tabIdx;
  }

  public class ManagementMenuToggleInfo : KIconToggleMenu.ToggleInfo
  {
    public ImageToggleState alertImage;
    public ImageToggleState glowImage;
    private ColorStyleSetting originalButtonSetting;

    public ManagementMenuToggleInfo(
      string text,
      string icon,
      object user_data = null,
      Action hotkey = 275,
      string tooltip = "",
      string tooltip_header = "")
      : base(text, icon, user_data, hotkey, tooltip, tooltip_header)
    {
      this.tooltip = GameUtil.ReplaceHotkeyString(this.tooltip, this.hotKey);
    }

    public void SetNotificationDisplay(
      bool showAlertImage,
      bool showGlow,
      ColorStyleSetting buttonColorStyle,
      ColorStyleSetting alertColorStyle)
    {
      ImageToggleState component = ((Component) this.toggle).GetComponent<ImageToggleState>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        if (Object.op_Inequality((Object) buttonColorStyle, (Object) null))
          component.SetColorStyle(buttonColorStyle);
        else
          component.SetColorStyle(this.originalButtonSetting);
      }
      if (Object.op_Inequality((Object) this.alertImage, (Object) null))
      {
        ((Component) this.alertImage).gameObject.SetActive(showAlertImage);
        this.alertImage.SetColorStyle(alertColorStyle);
      }
      if (!Object.op_Inequality((Object) this.glowImage, (Object) null))
        return;
      ((Component) this.glowImage).gameObject.SetActive(showGlow);
      if (!Object.op_Inequality((Object) buttonColorStyle, (Object) null))
        return;
      this.glowImage.SetColorStyle(buttonColorStyle);
    }

    public override void SetToggle(KToggle toggle)
    {
      base.SetToggle(toggle);
      ImageToggleState component1 = ((Component) toggle).GetComponent<ImageToggleState>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        this.originalButtonSetting = component1.colorStyleSetting;
      HierarchyReferences component2 = ((Component) toggle).GetComponent<HierarchyReferences>();
      if (!Object.op_Inequality((Object) component2, (Object) null))
        return;
      this.alertImage = component2.GetReference<ImageToggleState>("AlertImage");
      this.glowImage = component2.GetReference<ImageToggleState>("GlowImage");
    }
  }
}
