// Decompiled with JetBrains decompiler
// Type: RequestCrewSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class RequestCrewSideScreen : SideScreenContent
{
  private PassengerRocketModule rocketModule;
  public KToggle crewReleaseButton;
  public KToggle crewRequestButton;
  private Dictionary<KToggle, PassengerRocketModule.RequestCrewState> toggleMap = new Dictionary<KToggle, PassengerRocketModule.RequestCrewState>();
  public KButton changeCrewButton;
  public KScreen changeCrewSideScreenPrefab;
  private AssignmentGroupControllerSideScreen activeChangeCrewSideScreen;

  protected virtual void OnSpawn()
  {
    this.changeCrewButton.onClick += new System.Action(this.OnChangeCrewButtonPressed);
    this.crewReleaseButton.onClick += new System.Action(this.CrewRelease);
    this.crewRequestButton.onClick += new System.Action(this.CrewRequest);
    this.toggleMap.Add(this.crewReleaseButton, PassengerRocketModule.RequestCrewState.Release);
    this.toggleMap.Add(this.crewRequestButton, PassengerRocketModule.RequestCrewState.Request);
    this.Refresh();
  }

  public override int GetSideScreenSortOrder() => 100;

  public override bool IsValidForTarget(GameObject target)
  {
    PassengerRocketModule component1 = target.GetComponent<PassengerRocketModule>();
    RocketControlStation component2 = target.GetComponent<RocketControlStation>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      return Object.op_Inequality((Object) component1.GetMyWorld(), (Object) null);
    if (!Object.op_Inequality((Object) component2, (Object) null))
      return false;
    RocketControlStation.StatesInstance smi = ((Component) component2).GetSMI<RocketControlStation.StatesInstance>();
    return !smi.sm.IsInFlight(smi) && !smi.sm.IsLaunching(smi);
  }

  public override void SetTarget(GameObject target)
  {
    this.rocketModule = !Object.op_Inequality((Object) target.GetComponent<RocketControlStation>(), (Object) null) ? target.GetComponent<PassengerRocketModule>() : ((Component) target.GetMyWorld()).GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule();
    this.Refresh();
  }

  private void Refresh() => this.RefreshRequestButtons();

  private void CrewRelease()
  {
    this.rocketModule.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Release);
    this.RefreshRequestButtons();
  }

  private void CrewRequest()
  {
    this.rocketModule.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Request);
    this.RefreshRequestButtons();
  }

  private void RefreshRequestButtons()
  {
    foreach (KeyValuePair<KToggle, PassengerRocketModule.RequestCrewState> toggle in this.toggleMap)
      this.RefreshRequestButton(toggle.Key);
  }

  private void RefreshRequestButton(KToggle button)
  {
    if (this.toggleMap[button] == this.rocketModule.PassengersRequested)
    {
      button.isOn = true;
      foreach (ImageToggleState componentsInChild in ((Component) button).GetComponentsInChildren<ImageToggleState>())
        componentsInChild.SetActive();
      ((Behaviour) ((Component) button).GetComponent<ImageToggleStateThrobber>()).enabled = false;
    }
    else
    {
      button.isOn = false;
      foreach (ImageToggleState componentsInChild in ((Component) button).GetComponentsInChildren<ImageToggleState>())
        componentsInChild.SetInactive();
      ((Behaviour) ((Component) button).GetComponent<ImageToggleStateThrobber>()).enabled = false;
    }
  }

  private void OnChangeCrewButtonPressed()
  {
    if (Object.op_Equality((Object) this.activeChangeCrewSideScreen, (Object) null))
    {
      this.activeChangeCrewSideScreen = (AssignmentGroupControllerSideScreen) DetailsScreen.Instance.SetSecondarySideScreen(this.changeCrewSideScreenPrefab, (string) UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TITLE);
      this.activeChangeCrewSideScreen.SetTarget(((Component) this.rocketModule).gameObject);
    }
    else
    {
      DetailsScreen.Instance.ClearSecondarySideScreen();
      this.activeChangeCrewSideScreen = (AssignmentGroupControllerSideScreen) null;
    }
  }

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (show)
      return;
    DetailsScreen.Instance.ClearSecondarySideScreen();
    this.activeChangeCrewSideScreen = (AssignmentGroupControllerSideScreen) null;
  }
}
