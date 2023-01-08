// Decompiled with JetBrains decompiler
// Type: RocketModuleSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RocketModuleSideScreen : SideScreenContent
{
  public static RocketModuleSideScreen instance;
  private ReorderableBuilding reorderable;
  public KScreen changeModuleSideScreen;
  public Image moduleIcon;
  [Header("Buttons")]
  public KButton addNewModuleButton;
  public KButton removeModuleButton;
  public KButton changeModuleButton;
  public KButton moveModuleUpButton;
  public KButton moveModuleDownButton;
  public KButton viewInteriorButton;
  [Header("Labels")]
  public LocText moduleNameLabel;
  public LocText moduleDescriptionLabel;
  public TextStyleSetting nameSetting;
  public TextStyleSetting descriptionSetting;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    RocketModuleSideScreen.instance = this;
  }

  protected virtual void OnForcedCleanUp()
  {
    RocketModuleSideScreen.instance = (RocketModuleSideScreen) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  public override int GetSideScreenSortOrder() => 500;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.addNewModuleButton.onClick += (System.Action) (() =>
    {
      Vector2 vector2 = Vector2.zero;
      if (Object.op_Inequality((Object) SelectModuleSideScreen.Instance, (Object) null))
        vector2 = Util.rectTransform((Component) ((ScrollRect) SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>()).content).anchoredPosition;
      this.ClickAddNew(vector2.y);
    });
    this.removeModuleButton.onClick += new System.Action(this.ClickRemove);
    this.moveModuleUpButton.onClick += new System.Action(this.ClickSwapUp);
    this.moveModuleDownButton.onClick += new System.Action(this.ClickSwapDown);
    this.changeModuleButton.onClick += (System.Action) (() =>
    {
      Vector2 vector2 = Vector2.zero;
      if (Object.op_Inequality((Object) SelectModuleSideScreen.Instance, (Object) null))
        vector2 = Util.rectTransform((Component) ((ScrollRect) SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>()).content).anchoredPosition;
      this.ClickChangeModule(vector2.y);
    });
    this.viewInteriorButton.onClick += new System.Action(this.ClickViewInterior);
    this.moduleNameLabel.textStyleSetting = this.nameSetting;
    this.moduleDescriptionLabel.textStyleSetting = this.descriptionSetting;
    this.moduleNameLabel.ApplySettings();
    this.moduleDescriptionLabel.ApplySettings();
  }

  protected virtual void OnCmpDisable()
  {
    ((KMonoBehaviour) this).OnCmpDisable();
    DetailsScreen.Instance.ClearSecondarySideScreen();
  }

  protected virtual void OnCleanUp() => base.OnCleanUp();

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<ReorderableBuilding>(), (Object) null);

  public override void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.reorderable = new_target.GetComponent<ReorderableBuilding>();
      this.moduleIcon.sprite = Def.GetUISprite((object) ((Component) this.reorderable).gameObject).first;
      ((TMP_Text) this.moduleNameLabel).SetText(((Component) this.reorderable).GetProperName());
      ((TMP_Text) this.moduleDescriptionLabel).SetText(((Component) this.reorderable).GetComponent<Building>().Desc);
      this.UpdateButtonStates();
    }
  }

  public void UpdateButtonStates()
  {
    this.changeModuleButton.isInteractable = this.reorderable.CanChangeModule();
    ((Component) this.changeModuleButton).GetComponent<ToolTip>().SetSimpleTooltip(this.changeModuleButton.isInteractable ? STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONCHANGEMODULE.DESC.text : STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONCHANGEMODULE.INVALID.text);
    this.addNewModuleButton.isInteractable = true;
    ((Component) this.addNewModuleButton).GetComponent<ToolTip>().SetSimpleTooltip(STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.ADDMODULE.DESC.text);
    this.removeModuleButton.isInteractable = this.reorderable.CanRemoveModule();
    ((Component) this.removeModuleButton).GetComponent<ToolTip>().SetSimpleTooltip(this.removeModuleButton.isInteractable ? STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.DESC.text : STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.INVALID.text);
    this.moveModuleDownButton.isInteractable = this.reorderable.CanSwapDown();
    ((Component) this.moveModuleDownButton).GetComponent<ToolTip>().SetSimpleTooltip(this.moveModuleDownButton.isInteractable ? STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEDOWN.DESC.text : STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEDOWN.INVALID.text);
    this.moveModuleUpButton.isInteractable = this.reorderable.CanSwapUp();
    ((Component) this.moveModuleUpButton).GetComponent<ToolTip>().SetSimpleTooltip(this.moveModuleUpButton.isInteractable ? STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEUP.DESC.text : STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEUP.INVALID.text);
    ClustercraftExteriorDoor component = ((Component) this.reorderable).GetComponent<ClustercraftExteriorDoor>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.HasTargetWorld())
    {
      if (Object.op_Equality((Object) ClusterManager.Instance.activeWorld, (Object) component.GetTargetWorld()))
      {
        this.changeModuleButton.isInteractable = false;
        this.addNewModuleButton.isInteractable = false;
        this.removeModuleButton.isInteractable = false;
        this.moveModuleDownButton.isInteractable = false;
        this.moveModuleUpButton.isInteractable = false;
        this.viewInteriorButton.isInteractable = component.GetMyWorldId() != (int) ClusterManager.INVALID_WORLD_IDX;
        ((TMP_Text) ((Component) this.viewInteriorButton).GetComponentInChildren<LocText>()).SetText((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.LABEL);
        ((Component) this.viewInteriorButton).GetComponent<ToolTip>().SetSimpleTooltip(this.viewInteriorButton.isInteractable ? STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.DESC.text : STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.INVALID.text);
      }
      else
      {
        this.viewInteriorButton.isInteractable = Object.op_Inequality((Object) ((Component) this.reorderable).GetComponent<PassengerRocketModule>(), (Object) null);
        ((TMP_Text) ((Component) this.viewInteriorButton).GetComponentInChildren<LocText>()).SetText((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.LABEL);
        ((Component) this.viewInteriorButton).GetComponent<ToolTip>().SetSimpleTooltip(this.viewInteriorButton.isInteractable ? STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.DESC.text : STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.INVALID.text);
      }
    }
    else
    {
      this.viewInteriorButton.isInteractable = false;
      ((TMP_Text) ((Component) this.viewInteriorButton).GetComponentInChildren<LocText>()).SetText((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.LABEL);
      ((Component) this.viewInteriorButton).GetComponent<ToolTip>().SetSimpleTooltip(this.viewInteriorButton.isInteractable ? STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.DESC.text : STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWINTERIOR.INVALID.text);
    }
  }

  public void ClickAddNew(float scrollViewPosition, BuildingDef autoSelectDef = null)
  {
    SelectModuleSideScreen moduleSideScreen = (SelectModuleSideScreen) DetailsScreen.Instance.SetSecondarySideScreen(this.changeModuleSideScreen, (string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL);
    moduleSideScreen.addingNewModule = true;
    moduleSideScreen.SetTarget(((Component) this.reorderable).gameObject);
    if (Object.op_Inequality((Object) autoSelectDef, (Object) null))
      moduleSideScreen.SelectModule(autoSelectDef);
    this.ScrollToTargetPoint(scrollViewPosition);
  }

  private void ScrollToTargetPoint(float scrollViewPosition)
  {
    if (!Object.op_Inequality((Object) SelectModuleSideScreen.Instance, (Object) null))
      return;
    ((ScrollRect) SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>()).content.anchoredPosition = new Vector2(0.0f, scrollViewPosition);
    if (!((Component) this).gameObject.activeInHierarchy)
      return;
    ((MonoBehaviour) this).StartCoroutine(this.DelayedScrollToTargetPoint(scrollViewPosition));
  }

  private IEnumerator DelayedScrollToTargetPoint(float scrollViewPosition)
  {
    if (Object.op_Inequality((Object) SelectModuleSideScreen.Instance, (Object) null))
    {
      yield return (object) SequenceUtil.WaitForEndOfFrame;
      ((ScrollRect) SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>()).content.anchoredPosition = new Vector2(0.0f, scrollViewPosition);
    }
  }

  private void ClickRemove()
  {
    this.reorderable.Trigger(-790448070, (object) null);
    this.UpdateButtonStates();
  }

  private void ClickSwapUp()
  {
    this.reorderable.SwapWithAbove();
    this.UpdateButtonStates();
  }

  private void ClickSwapDown()
  {
    this.reorderable.SwapWithBelow();
    this.UpdateButtonStates();
  }

  private void ClickChangeModule(float scrollViewPosition)
  {
    SelectModuleSideScreen moduleSideScreen = (SelectModuleSideScreen) DetailsScreen.Instance.SetSecondarySideScreen(this.changeModuleSideScreen, (string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL);
    moduleSideScreen.addingNewModule = false;
    moduleSideScreen.SetTarget(((Component) this.reorderable).gameObject);
    this.ScrollToTargetPoint(scrollViewPosition);
  }

  private void ClickViewInterior()
  {
    ClustercraftExteriorDoor component1 = ((Component) this.reorderable).GetComponent<ClustercraftExteriorDoor>();
    PassengerRocketModule component2 = ((Component) this.reorderable).GetComponent<PassengerRocketModule>();
    WorldContainer targetWorld = component1.GetTargetWorld();
    WorldContainer myWorld = component1.GetMyWorld();
    if (Object.op_Equality((Object) ClusterManager.Instance.activeWorld, (Object) targetWorld))
    {
      if (myWorld.id != (int) ClusterManager.INVALID_WORLD_IDX)
      {
        AudioMixer.instance.Stop(component2.interiorReverbSnapshot);
        ClusterManager.Instance.SetActiveWorld(myWorld.id);
      }
    }
    else
    {
      AudioMixer.instance.Start(component2.interiorReverbSnapshot);
      ClusterManager.Instance.SetActiveWorld(targetWorld.id);
    }
    DetailsScreen.Instance.ClearSecondarySideScreen();
    this.UpdateButtonStates();
  }
}
