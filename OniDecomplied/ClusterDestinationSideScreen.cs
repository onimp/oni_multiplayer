// Decompiled with JetBrains decompiler
// Type: ClusterDestinationSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClusterDestinationSideScreen : SideScreenContent
{
  public Image destinationImage;
  public LocText destinationLabel;
  public KButton changeDestinationButton;
  public KButton clearDestinationButton;
  public DropDown launchPadDropDown;
  public KButton repeatButton;
  public ColorStyleSetting repeatOff;
  public ColorStyleSetting repeatOn;
  public ColorStyleSetting defaultButton;
  public ColorStyleSetting highlightButton;
  private int m_refreshHandle = -1;

  private ClusterDestinationSelector targetSelector { get; set; }

  private RocketClusterDestinationSelector targetRocketSelector { get; set; }

  protected virtual void OnSpawn()
  {
    this.changeDestinationButton.onClick += new System.Action(this.OnClickChangeDestination);
    this.clearDestinationButton.onClick += new System.Action(this.OnClickClearDestination);
    this.launchPadDropDown.targetDropDownContainer = GameScreenManager.Instance.ssOverlayCanvas;
    this.launchPadDropDown.CustomizeEmptyRow((string) STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE, (Sprite) null);
    this.repeatButton.onClick += new System.Action(this.OnRepeatClicked);
  }

  public override int GetSideScreenSortOrder() => 300;

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (show)
    {
      this.Refresh();
      this.m_refreshHandle = this.targetSelector.Subscribe(543433792, (Action<object>) (data => this.Refresh()));
    }
    else
    {
      if (this.m_refreshHandle == -1)
        return;
      this.targetSelector.Unsubscribe(this.m_refreshHandle);
      this.m_refreshHandle = -1;
      this.launchPadDropDown.Close();
    }
  }

  public override bool IsValidForTarget(GameObject target)
  {
    ClusterDestinationSelector component = target.GetComponent<ClusterDestinationSelector>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.assignable || Object.op_Inequality((Object) target.GetComponent<RocketModule>(), (Object) null) && target.HasTag(GameTags.LaunchButtonRocketModule))
      return true;
    return Object.op_Inequality((Object) target.GetComponent<RocketControlStation>(), (Object) null) && ((Component) target.GetComponent<RocketControlStation>().GetMyWorld()).GetComponent<Clustercraft>().Status != Clustercraft.CraftStatus.Launching;
  }

  public override void SetTarget(GameObject target)
  {
    this.targetSelector = target.GetComponent<ClusterDestinationSelector>();
    if (Object.op_Equality((Object) this.targetSelector, (Object) null))
    {
      if (Object.op_Inequality((Object) target.GetComponent<RocketModuleCluster>(), (Object) null))
        this.targetSelector = (ClusterDestinationSelector) target.GetComponent<RocketModuleCluster>().CraftInterface.GetClusterDestinationSelector();
      else if (Object.op_Inequality((Object) target.GetComponent<RocketControlStation>(), (Object) null))
        this.targetSelector = (ClusterDestinationSelector) ((Component) target.GetMyWorld()).GetComponent<Clustercraft>().ModuleInterface.GetClusterDestinationSelector();
    }
    this.targetRocketSelector = this.targetSelector as RocketClusterDestinationSelector;
  }

  private void Refresh(object data = null)
  {
    if (!this.targetSelector.IsAtDestination())
    {
      Sprite sprite;
      string label;
      ClusterGrid.Instance.GetLocationDescription(this.targetSelector.GetDestination(), out sprite, out label, out string _);
      this.destinationImage.sprite = sprite;
      ((TMP_Text) this.destinationLabel).text = (string) STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.TITLE + ": " + label;
      this.clearDestinationButton.isInteractable = true;
    }
    else
    {
      this.destinationImage.sprite = Assets.GetSprite(HashedString.op_Implicit("hex_unknown"));
      ((TMP_Text) this.destinationLabel).text = (string) STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.TITLE + ": " + (string) STRINGS.UI.SPACEDESTINATIONS.NONE.NAME;
      this.clearDestinationButton.isInteractable = false;
    }
    if (Object.op_Inequality((Object) this.targetRocketSelector, (Object) null))
    {
      List<LaunchPad> padsForDestination = LaunchPad.GetLaunchPadsForDestination(this.targetRocketSelector.GetDestination());
      ((Component) this.launchPadDropDown).gameObject.SetActive(true);
      ((Component) this.repeatButton).gameObject.SetActive(true);
      this.launchPadDropDown.Initialize((IEnumerable<IListableOption>) padsForDestination, new Action<IListableOption, object>(this.OnLaunchPadEntryClick), new Func<IListableOption, IListableOption, object, int>(this.PadDropDownSort), new Action<DropDownEntry, object>(this.PadDropDownEntryRefreshAction), targetData: ((object) this.targetRocketSelector));
      if (!this.targetRocketSelector.IsAtDestination() && padsForDestination.Count > 0)
      {
        this.launchPadDropDown.openButton.isInteractable = true;
        LaunchPad destinationPad = this.targetRocketSelector.GetDestinationPad();
        if (Object.op_Inequality((Object) destinationPad, (Object) null))
          ((TMP_Text) this.launchPadDropDown.selectedLabel).text = destinationPad.GetProperName();
        else
          ((TMP_Text) this.launchPadDropDown.selectedLabel).text = (string) STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE;
      }
      else
      {
        ((TMP_Text) this.launchPadDropDown.selectedLabel).text = (string) STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE;
        this.launchPadDropDown.openButton.isInteractable = false;
      }
      this.StyleRepeatButton();
    }
    else
    {
      ((Component) this.launchPadDropDown).gameObject.SetActive(false);
      ((Component) this.repeatButton).gameObject.SetActive(false);
    }
    this.StyleChangeDestinationButton();
  }

  private void OnClickChangeDestination()
  {
    if (this.targetSelector.assignable)
      ClusterMapScreen.Instance.ShowInSelectDestinationMode(this.targetSelector);
    this.StyleChangeDestinationButton();
  }

  private void StyleChangeDestinationButton()
  {
  }

  private void OnClickClearDestination() => this.targetSelector.SetDestination(this.targetSelector.GetMyWorldLocation());

  private void OnLaunchPadEntryClick(IListableOption option, object data) => this.targetRocketSelector.SetDestinationPad((LaunchPad) option);

  private void PadDropDownEntryRefreshAction(DropDownEntry entry, object targetData)
  {
    LaunchPad entryData = (LaunchPad) entry.entryData;
    Clustercraft component = ((Component) this.targetRocketSelector).GetComponent<Clustercraft>();
    if (Object.op_Inequality((Object) entryData, (Object) null))
    {
      string failReason;
      if (component.CanLandAtPad(entryData, out failReason) == Clustercraft.PadLandingStatus.CanNeverLand)
      {
        entry.button.isInteractable = false;
        ((Image) entry.image).sprite = Assets.GetSprite(HashedString.op_Implicit("iconWarning"));
        entry.tooltip.SetSimpleTooltip(failReason);
      }
      else
      {
        entry.button.isInteractable = true;
        ((Image) entry.image).sprite = ((Component) entryData).GetComponent<Building>().Def.GetUISprite();
        entry.tooltip.SetSimpleTooltip(string.Format((string) STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_VALID_SITE, (object) entryData.GetProperName()));
      }
    }
    else
    {
      entry.button.isInteractable = true;
      ((Image) entry.image).sprite = Assets.GetBuildingDef("LaunchPad").GetUISprite();
      entry.tooltip.SetSimpleTooltip((string) STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_FIRST_AVAILABLE);
    }
  }

  private int PadDropDownSort(IListableOption a, IListableOption b, object targetData) => 0;

  private void OnRepeatClicked()
  {
    this.targetRocketSelector.Repeat = !this.targetRocketSelector.Repeat;
    this.StyleRepeatButton();
  }

  private void StyleRepeatButton()
  {
    this.repeatButton.bgImage.colorStyleSetting = this.targetRocketSelector.Repeat ? this.repeatOn : this.repeatOff;
    this.repeatButton.bgImage.ApplyColorStyleSetting();
  }
}
