// Decompiled with JetBrains decompiler
// Type: LaunchButtonSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaunchButtonSideScreen : SideScreenContent
{
  public KButton launchButton;
  public LocText statusText;
  private RocketModuleCluster rocketModule;
  private LaunchPad selectedPad;
  private bool acknowledgeWarnings;
  private float lastRefreshTime;
  private const float UPDATE_FREQUENCY = 1f;
  private static readonly EventSystem.IntraObjectHandler<LaunchButtonSideScreen> RefreshDelegate = new EventSystem.IntraObjectHandler<LaunchButtonSideScreen>((Action<LaunchButtonSideScreen, object>) ((cmp, data) => cmp.Refresh()));

  protected virtual void OnSpawn()
  {
    this.Refresh();
    this.launchButton.onClick += new System.Action(this.TriggerLaunch);
  }

  public override int GetSideScreenSortOrder() => -100;

  public override bool IsValidForTarget(GameObject target)
  {
    if (Object.op_Inequality((Object) target.GetComponent<RocketModule>(), (Object) null) && target.HasTag(GameTags.LaunchButtonRocketModule))
      return true;
    return Object.op_Implicit((Object) target.GetComponent<LaunchPad>()) && target.GetComponent<LaunchPad>().HasRocketWithCommandModule();
  }

  public override void SetTarget(GameObject target)
  {
    bool flag = Object.op_Equality((Object) this.rocketModule, (Object) null) || Object.op_Inequality((Object) ((Component) this.rocketModule).gameObject, (Object) target);
    this.selectedPad = (LaunchPad) null;
    this.rocketModule = target.GetComponent<RocketModuleCluster>();
    if (Object.op_Equality((Object) this.rocketModule, (Object) null))
    {
      this.selectedPad = target.GetComponent<LaunchPad>();
      if (Object.op_Inequality((Object) this.selectedPad, (Object) null))
      {
        foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.selectedPad.LandedRocket.CraftInterface.ClusterModules)
        {
          if (Object.op_Implicit((Object) ((Component) clusterModule.Get()).GetComponent<LaunchableRocketCluster>()))
          {
            this.rocketModule = ((Component) clusterModule.Get()).GetComponent<RocketModuleCluster>();
            break;
          }
        }
      }
    }
    if (Object.op_Equality((Object) this.selectedPad, (Object) null))
      this.selectedPad = this.rocketModule.CraftInterface.CurrentPad;
    if (flag)
      this.acknowledgeWarnings = false;
    this.rocketModule.CraftInterface.Subscribe<LaunchButtonSideScreen>(543433792, LaunchButtonSideScreen.RefreshDelegate);
    this.rocketModule.CraftInterface.Subscribe<LaunchButtonSideScreen>(1655598572, LaunchButtonSideScreen.RefreshDelegate);
    this.Refresh();
  }

  public override void ClearTarget()
  {
    if (!Object.op_Inequality((Object) this.rocketModule, (Object) null))
      return;
    this.rocketModule.CraftInterface.Unsubscribe<LaunchButtonSideScreen>(543433792, LaunchButtonSideScreen.RefreshDelegate, false);
    this.rocketModule.CraftInterface.Unsubscribe<LaunchButtonSideScreen>(1655598572, LaunchButtonSideScreen.RefreshDelegate, false);
    this.rocketModule = (RocketModuleCluster) null;
  }

  private void TriggerLaunch()
  {
    int num = this.acknowledgeWarnings ? 0 : (this.rocketModule.CraftInterface.HasLaunchWarnings() ? 1 : 0);
    bool flag = this.rocketModule.CraftInterface.IsLaunchRequested();
    if (num != 0)
      this.acknowledgeWarnings = true;
    else if (flag)
    {
      this.rocketModule.CraftInterface.CancelLaunch();
      this.acknowledgeWarnings = false;
    }
    else
      this.rocketModule.CraftInterface.TriggerLaunch();
    this.Refresh();
  }

  public void Update()
  {
    if ((double) Time.unscaledTime <= (double) this.lastRefreshTime + 1.0)
      return;
    this.lastRefreshTime = Time.unscaledTime;
    this.Refresh();
  }

  private void Refresh()
  {
    if (Object.op_Equality((Object) this.rocketModule, (Object) null) || Object.op_Equality((Object) this.selectedPad, (Object) null))
      return;
    bool flag1 = !this.acknowledgeWarnings && this.rocketModule.CraftInterface.HasLaunchWarnings();
    bool flag2 = this.rocketModule.CraftInterface.IsLaunchRequested();
    int num = this.selectedPad.IsLogicInputConnected() ? 1 : 0;
    bool flag3 = num != 0 ? this.rocketModule.CraftInterface.CheckReadyForAutomatedLaunchCommand() : this.rocketModule.CraftInterface.CheckPreppedForLaunch();
    if (num != 0)
    {
      this.launchButton.isInteractable = false;
      ((TMP_Text) ((Component) this.launchButton).GetComponentInChildren<LocText>()).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_AUTOMATION_CONTROLLED;
      ((Component) this.launchButton).GetComponentInChildren<ToolTip>().toolTip = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_AUTOMATION_CONTROLLED_TOOLTIP;
    }
    else if (DebugHandler.InstantBuildMode | flag3)
    {
      this.launchButton.isInteractable = true;
      if (flag2)
      {
        ((TMP_Text) ((Component) this.launchButton).GetComponentInChildren<LocText>()).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_REQUESTED_BUTTON;
        ((Component) this.launchButton).GetComponentInChildren<ToolTip>().toolTip = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_REQUESTED_BUTTON_TOOLTIP;
      }
      else if (flag1)
      {
        ((TMP_Text) ((Component) this.launchButton).GetComponentInChildren<LocText>()).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_WARNINGS_BUTTON;
        ((Component) this.launchButton).GetComponentInChildren<ToolTip>().toolTip = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_WARNINGS_BUTTON_TOOLTIP;
      }
      else
      {
        LocString locString = DebugHandler.InstantBuildMode ? UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON_DEBUG : UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON;
        ((TMP_Text) ((Component) this.launchButton).GetComponentInChildren<LocText>()).text = (string) locString;
        ((Component) this.launchButton).GetComponentInChildren<ToolTip>().toolTip = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON_TOOLTIP;
      }
    }
    else
    {
      this.launchButton.isInteractable = false;
      ((TMP_Text) ((Component) this.launchButton).GetComponentInChildren<LocText>()).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON;
      ((Component) this.launchButton).GetComponentInChildren<ToolTip>().toolTip = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAUNCH_BUTTON_NOT_READY_TOOLTIP;
    }
    if (Object.op_Equality((Object) this.rocketModule.CraftInterface.GetInteriorWorld(), (Object) null))
    {
      ((TMP_Text) this.statusText).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.STILL_PREPPING;
    }
    else
    {
      PassengerRocketModule component = ((Component) this.rocketModule).GetComponent<PassengerRocketModule>();
      List<RocketControlStation> worldItems = Components.RocketControlStations.GetWorldItems(this.rocketModule.CraftInterface.GetInteriorWorld().id);
      RocketControlStationLaunchWorkable stationLaunchWorkable = (RocketControlStationLaunchWorkable) null;
      if (worldItems != null && worldItems.Count > 0)
        stationLaunchWorkable = ((Component) worldItems[0]).GetComponent<RocketControlStationLaunchWorkable>();
      if (Object.op_Equality((Object) component, (Object) null) || Object.op_Equality((Object) stationLaunchWorkable, (Object) null))
      {
        ((TMP_Text) this.statusText).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.STILL_PREPPING;
      }
      else
      {
        bool flag4 = component.CheckPassengersBoarded();
        bool flag5 = !component.CheckExtraPassengers();
        bool flag6 = Object.op_Inequality((Object) stationLaunchWorkable.worker, (Object) null);
        bool flag7 = ((Component) this.rocketModule.CraftInterface).HasTag(GameTags.RocketNotOnGround);
        if (!flag3)
          ((TMP_Text) this.statusText).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.STILL_PREPPING;
        else if (!flag2)
          ((TMP_Text) this.statusText).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.READY_FOR_LAUNCH;
        else if (!flag4)
          ((TMP_Text) this.statusText).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.LOADING_CREW;
        else if (!flag5)
          ((TMP_Text) this.statusText).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.UNLOADING_PASSENGERS;
        else if (!flag6)
          ((TMP_Text) this.statusText).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.WAITING_FOR_PILOT;
        else if (!flag7)
          ((TMP_Text) this.statusText).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.COUNTING_DOWN;
        else
          ((TMP_Text) this.statusText).text = (string) UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.STATUS.TAKING_OFF;
      }
    }
  }
}
