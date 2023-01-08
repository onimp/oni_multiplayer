// Decompiled with JetBrains decompiler
// Type: LaunchPadSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaunchPadSideScreen : SideScreenContent
{
  public GameObject content;
  private LaunchPad selectedPad;
  public LocText DescriptionText;
  public GameObject landableRocketRowPrefab;
  public GameObject newRocketPanel;
  public KButton startNewRocketbutton;
  public KButton devAutoRocketButton;
  public GameObject landableRowContainer;
  public GameObject nothingWaitingRow;
  public KScreen changeModuleSideScreen;
  private int refreshEventHandle = -1;
  public List<GameObject> waitingToLandRows = new List<GameObject>();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.startNewRocketbutton.onClick += new System.Action(this.ClickStartNewRocket);
    this.devAutoRocketButton.onClick += new System.Action(this.ClickAutoRocket);
  }

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (show)
      return;
    DetailsScreen.Instance.ClearSecondarySideScreen();
  }

  public override int GetSideScreenSortOrder() => 100;

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<LaunchPad>(), (Object) null);

  public override void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      if (this.refreshEventHandle != -1)
        this.selectedPad.Unsubscribe(this.refreshEventHandle);
      this.selectedPad = new_target.GetComponent<LaunchPad>();
      if (Object.op_Equality((Object) this.selectedPad, (Object) null))
      {
        Debug.LogError((object) "The gameObject received does not contain a LaunchPad component");
      }
      else
      {
        this.refreshEventHandle = this.selectedPad.Subscribe(-887025858, new Action<object>(this.RefreshWaitingToLandList));
        this.RefreshRocketButton();
        this.RefreshWaitingToLandList();
      }
    }
  }

  private void RefreshWaitingToLandList(object data = null)
  {
    for (int index = this.waitingToLandRows.Count - 1; index >= 0; --index)
      Util.KDestroyGameObject(this.waitingToLandRows[index]);
    this.waitingToLandRows.Clear();
    this.nothingWaitingRow.SetActive(true);
    AxialI myWorldLocation = this.selectedPad.GetMyWorldLocation();
    foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.GetEntitiesInRange(myWorldLocation))
    {
      Clustercraft craft = clusterGridEntity as Clustercraft;
      if (!Object.op_Equality((Object) craft, (Object) null) && craft.Status == Clustercraft.CraftStatus.InFlight && (!craft.IsFlightInProgress() || !AxialI.op_Inequality(craft.Destination, myWorldLocation)))
      {
        GameObject gameObject = Util.KInstantiateUI(this.landableRocketRowPrefab, this.landableRowContainer, true);
        ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).text = craft.Name;
        this.waitingToLandRows.Add(gameObject);
        KButton componentInChildren = gameObject.GetComponentInChildren<KButton>();
        ((TMP_Text) ((Component) componentInChildren).GetComponentInChildren<LocText>()).SetText((string) (Object.op_Equality((Object) craft.ModuleInterface.GetClusterDestinationSelector().GetDestinationPad(), (Object) this.selectedPad) ? UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.CANCEL_LAND_BUTTON : UI.UISIDESCREENS.LAUNCHPADSIDESCREEN.LAND_BUTTON));
        string failReason;
        componentInChildren.isInteractable = craft.CanLandAtPad(this.selectedPad, out failReason) != Clustercraft.PadLandingStatus.CanNeverLand;
        if (!componentInChildren.isInteractable)
          ((Component) componentInChildren).GetComponent<ToolTip>().SetSimpleTooltip(failReason);
        else
          ((Component) componentInChildren).GetComponent<ToolTip>().ClearMultiStringTooltip();
        componentInChildren.onClick += (System.Action) (() =>
        {
          if (Object.op_Equality((Object) craft.ModuleInterface.GetClusterDestinationSelector().GetDestinationPad(), (Object) this.selectedPad))
            ((Component) craft).GetComponent<ClusterDestinationSelector>().SetDestination(craft.Location);
          else
            craft.LandAtPad(this.selectedPad);
          this.RefreshWaitingToLandList();
        });
        this.nothingWaitingRow.SetActive(false);
      }
    }
  }

  private void ClickStartNewRocket() => ((SelectModuleSideScreen) DetailsScreen.Instance.SetSecondarySideScreen(this.changeModuleSideScreen, (string) UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL)).SetLaunchPad(this.selectedPad);

  private void RefreshRocketButton()
  {
    bool isOperational = ((Component) this.selectedPad).GetComponent<Operational>().IsOperational;
    this.startNewRocketbutton.isInteractable = Object.op_Equality((Object) this.selectedPad.LandedRocket, (Object) null) & isOperational;
    if (!isOperational)
      ((Component) this.startNewRocketbutton).GetComponent<ToolTip>().SetSimpleTooltip((string) UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_PAD_DISABLED);
    else
      ((Component) this.startNewRocketbutton).GetComponent<ToolTip>().ClearMultiStringTooltip();
    this.devAutoRocketButton.isInteractable = Object.op_Equality((Object) this.selectedPad.LandedRocket, (Object) null);
    ((Component) this.devAutoRocketButton).gameObject.SetActive(DebugHandler.InstantBuildMode);
  }

  private void ClickAutoRocket() => AutoRocketUtility.StartAutoRocket(this.selectedPad);
}
