// Decompiled with JetBrains decompiler
// Type: SelectModuleSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectModuleSideScreen : KScreen
{
  public RocketModule module;
  private LaunchPad launchPad;
  public GameObject mainContents;
  [Header("Category")]
  public GameObject categoryPrefab;
  public GameObject moduleButtonPrefab;
  public GameObject categoryContent;
  private BuildingDef selectedModuleDef;
  public List<GameObject> categories = new List<GameObject>();
  public Dictionary<BuildingDef, GameObject> buttons = new Dictionary<BuildingDef, GameObject>();
  private Dictionary<BuildingDef, bool> moduleBuildableState = new Dictionary<BuildingDef, bool>();
  public static SelectModuleSideScreen Instance;
  public bool addingNewModule;
  public GameObject materialSelectionPanelPrefab;
  private MaterialSelectionPanel materialSelectionPanel;
  public KButton buildSelectedModuleButton;
  public ColorStyleSetting colorStyleButton;
  public ColorStyleSetting colorStyleButtonSelected;
  public ColorStyleSetting colorStyleButtonInactive;
  public ColorStyleSetting colorStyleButtonInactiveSelected;
  private List<int> gameSubscriptionHandles = new List<int>();
  public static List<string> moduleButtonSortOrder = new List<string>()
  {
    "CO2Engine",
    "SugarEngine",
    "SteamEngineCluster",
    "KeroseneEngineClusterSmall",
    "KeroseneEngineCluster",
    "HEPEngine",
    "HydrogenEngineCluster",
    "HabitatModuleSmall",
    "HabitatModuleMedium",
    "NoseconeBasic",
    "NoseconeHarvest",
    "OrbitalCargoModule",
    "ScoutModule",
    "PioneerModule",
    "LiquidFuelTankCluster",
    "SmallOxidizerTank",
    "OxidizerTankCluster",
    "OxidizerTankLiquidCluster",
    "SolidCargoBaySmall",
    "LiquidCargoBaySmall",
    "GasCargoBaySmall",
    "CargoBayCluster",
    "LiquidCargoBayCluster",
    "GasCargoBayCluster",
    "BatteryModule",
    "SolarPanelModule",
    "ArtifactCargoBay",
    "ScannerModule"
  };

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (show)
      return;
    DetailsScreen.Instance.ClearSecondarySideScreen();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    SelectModuleSideScreen.Instance = this;
    this.SpawnButtons();
    this.buildSelectedModuleButton.onClick += new System.Action(this.OnClickBuildSelectedModule);
  }

  protected virtual void OnForcedCleanUp()
  {
    SelectModuleSideScreen.Instance = (SelectModuleSideScreen) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  protected virtual void OnCmpDisable()
  {
    this.ClearSubscriptionHandles();
    this.module = (RocketModule) null;
    ((KMonoBehaviour) this).OnCmpDisable();
  }

  private void ClearSubscriptionHandles()
  {
    foreach (int subscriptionHandle in this.gameSubscriptionHandles)
      Game.Instance.Unsubscribe(subscriptionHandle);
    this.gameSubscriptionHandles.Clear();
  }

  protected virtual void OnCmpEnable()
  {
    ((KMonoBehaviour) this).OnCmpEnable();
    this.ClearSubscriptionHandles();
    this.gameSubscriptionHandles.Add(Game.Instance.Subscribe(-107300940, new Action<object>(this.UpdateBuildableStates)));
    this.gameSubscriptionHandles.Add(Game.Instance.Subscribe(-1948169901, new Action<object>(this.UpdateBuildableStates)));
  }

  protected virtual void OnCleanUp()
  {
    foreach (int subscriptionHandle in this.gameSubscriptionHandles)
      Game.Instance.Unsubscribe(subscriptionHandle);
    this.gameSubscriptionHandles.Clear();
    base.OnCleanUp();
  }

  public void SetLaunchPad(LaunchPad pad)
  {
    this.launchPad = pad;
    this.module = (RocketModule) null;
    this.UpdateBuildableStates();
    foreach (KeyValuePair<BuildingDef, GameObject> button in this.buttons)
      this.SetupBuildingTooltip(button.Value.GetComponent<ToolTip>(), button.Key);
  }

  public void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.module = (RocketModule) new_target.GetComponent<RocketModuleCluster>();
      if (Object.op_Equality((Object) this.module, (Object) null))
      {
        Debug.LogError((object) "The gameObject received does not contain a RocketModuleCluster component");
      }
      else
      {
        this.launchPad = (LaunchPad) null;
        foreach (KeyValuePair<BuildingDef, GameObject> button in this.buttons)
          this.SetupBuildingTooltip(button.Value.GetComponent<ToolTip>(), button.Key);
        this.UpdateBuildableStates();
        this.buildSelectedModuleButton.isInteractable = false;
        if (!Object.op_Inequality((Object) this.selectedModuleDef, (Object) null))
          return;
        this.SelectModule(this.selectedModuleDef);
      }
    }
  }

  private void UpdateBuildableStates(object data = null)
  {
    foreach (KeyValuePair<BuildingDef, GameObject> button in this.buttons)
    {
      if (!this.moduleBuildableState.ContainsKey(button.Key))
        this.moduleBuildableState.Add(button.Key, false);
      TechItem techItem = Db.Get().TechItems.TryGet(button.Key.PrefabID);
      if (techItem != null)
      {
        bool flag = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || techItem.IsComplete();
        button.Value.SetActive(flag);
      }
      else
        button.Value.SetActive(true);
      this.moduleBuildableState[button.Key] = this.TestBuildable(button.Key);
    }
    if (Object.op_Inequality((Object) this.selectedModuleDef, (Object) null))
      this.ConfigureMaterialSelector();
    this.SetButtonColors();
  }

  private void OnClickBuildSelectedModule()
  {
    if (!Object.op_Inequality((Object) this.selectedModuleDef, (Object) null))
      return;
    this.OrderBuildSelectedModule();
  }

  private void ConfigureMaterialSelector()
  {
    this.buildSelectedModuleButton.isInteractable = false;
    if (Object.op_Equality((Object) this.materialSelectionPanel, (Object) null))
    {
      this.materialSelectionPanel = Util.KInstantiateUI<MaterialSelectionPanel>(this.materialSelectionPanelPrefab.gameObject, ((Component) this).gameObject, true);
      ((KMonoBehaviour) this.materialSelectionPanel).transform.SetSiblingIndex(((KMonoBehaviour) this.buildSelectedModuleButton).transform.GetSiblingIndex());
    }
    this.materialSelectionPanel.ClearSelectActions();
    this.materialSelectionPanel.ConfigureScreen(this.selectedModuleDef.CraftRecipe, new MaterialSelectionPanel.GetBuildableStateDelegate(this.IsDefBuildable), new MaterialSelectionPanel.GetBuildableTooltipDelegate(this.GetErrorTooltips));
    this.materialSelectionPanel.ToggleShowDescriptorPanels(false);
    this.materialSelectionPanel.AddSelectAction(new MaterialSelector.SelectMaterialActions(this.UpdateBuildButton));
    this.materialSelectionPanel.AutoSelectAvailableMaterial();
  }

  private bool IsDefBuildable(BuildingDef def) => this.moduleBuildableState.ContainsKey(def) && this.moduleBuildableState[def];

  private void UpdateBuildButton() => this.buildSelectedModuleButton.isInteractable = Object.op_Inequality((Object) this.materialSelectionPanel, (Object) null) && this.materialSelectionPanel.AllSelectorsSelected() && Object.op_Inequality((Object) this.selectedModuleDef, (Object) null) && this.moduleBuildableState[this.selectedModuleDef];

  public void SetButtonColors()
  {
    foreach (KeyValuePair<BuildingDef, GameObject> button in this.buttons)
    {
      MultiToggle component1 = button.Value.GetComponent<MultiToggle>();
      HierarchyReferences component2 = button.Value.GetComponent<HierarchyReferences>();
      if (!this.moduleBuildableState[button.Key])
      {
        ((Graphic) component2.GetReference<Image>("FG")).material = PlanScreen.Instance.desaturatedUIMaterial;
        if (Object.op_Equality((Object) button.Key, (Object) this.selectedModuleDef))
          component1.ChangeState(1);
        else
          component1.ChangeState(0);
      }
      else
      {
        ((Graphic) component2.GetReference<Image>("FG")).material = PlanScreen.Instance.defaultUIMaterial;
        if (Object.op_Equality((Object) button.Key, (Object) this.selectedModuleDef))
          component1.ChangeState(3);
        else
          component1.ChangeState(2);
      }
    }
    this.UpdateBuildButton();
  }

  private bool TestBuildable(BuildingDef def)
  {
    GameObject buildingComplete = def.BuildingComplete;
    SelectModuleCondition.SelectionContext selectionContext = this.GetSelectionContext(def);
    if (selectionContext == SelectModuleCondition.SelectionContext.AddModuleAbove && Object.op_Inequality((Object) this.module, (Object) null))
    {
      BuildingAttachPoint component = ((Component) this.module).GetComponent<BuildingAttachPoint>();
      if (Object.op_Inequality((Object) component, (Object) null) && Object.op_Inequality((Object) component.points[0].attachedBuilding, (Object) null) && !((Component) component.points[0].attachedBuilding).GetComponent<ReorderableBuilding>().CanMoveVertically(def.HeightInCells))
        return false;
    }
    if (selectionContext == SelectModuleCondition.SelectionContext.AddModuleBelow && !((Component) this.module).GetComponent<ReorderableBuilding>().CanMoveVertically(def.HeightInCells) || selectionContext == SelectModuleCondition.SelectionContext.ReplaceModule && Object.op_Inequality((Object) this.module, (Object) null) && Object.op_Inequality((Object) def, (Object) null) && Object.op_Equality((Object) ((Component) this.module).GetComponent<Building>().Def, (Object) def))
      return false;
    foreach (SelectModuleCondition buildCondition in buildingComplete.GetComponent<ReorderableBuilding>().buildConditions)
    {
      if ((!buildCondition.IgnoreInSanboxMode() || !DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive) && !buildCondition.EvaluateCondition(Object.op_Equality((Object) this.module, (Object) null) ? ((Component) this.launchPad).gameObject : ((Component) this.module).gameObject, def, selectionContext))
        return false;
    }
    return true;
  }

  private void ClearButtons()
  {
    foreach (KeyValuePair<BuildingDef, GameObject> button in this.buttons)
      Util.KDestroyGameObject(button.Value);
    for (int index = this.categories.Count - 1; index >= 0; --index)
      Util.KDestroyGameObject(this.categories[index]);
    this.categories.Clear();
    this.buttons.Clear();
  }

  public void SpawnButtons(object data = null)
  {
    this.ClearButtons();
    GameObject gameObject1 = Util.KInstantiateUI(this.categoryPrefab, this.categoryContent, true);
    HierarchyReferences component = gameObject1.GetComponent<HierarchyReferences>();
    this.categories.Add(gameObject1);
    component.GetReference<LocText>("label");
    Transform reference = component.GetReference<Transform>("content");
    List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<RocketModuleCluster>();
    foreach (string str in SelectModuleSideScreen.moduleButtonSortOrder)
    {
      string id = str;
      GameObject part = prefabsWithComponent.Find((Predicate<GameObject>) (p =>
      {
        Tag tag = p.PrefabID();
        return ((Tag) ref tag).Name == id;
      }));
      if (Object.op_Equality((Object) part, (Object) null))
      {
        Debug.LogWarning((object) ("Found an id [" + id + "] in moduleButtonSortOrder in SelectModuleSideScreen.cs that doesn't have a corresponding rocket part!"));
      }
      else
      {
        GameObject gameObject2 = Util.KInstantiateUI(this.moduleButtonPrefab, ((Component) reference).gameObject, true);
        gameObject2.GetComponentsInChildren<Image>()[1].sprite = Def.GetUISprite((object) part).first;
        LocText componentInChildren = gameObject2.GetComponentInChildren<LocText>();
        ((TMP_Text) componentInChildren).text = part.GetProperName();
        ((TMP_Text) componentInChildren).alignment = (TextAlignmentOptions) 1026;
        ((TMP_Text) componentInChildren).enableWordWrapping = true;
        gameObject2.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.SelectModule(part.GetComponent<Building>().Def));
        this.buttons.Add(part.GetComponent<Building>().Def, gameObject2);
        if (Object.op_Inequality((Object) this.selectedModuleDef, (Object) null))
          this.SelectModule(this.selectedModuleDef);
      }
    }
    this.UpdateBuildableStates();
  }

  private void SetupBuildingTooltip(ToolTip tooltip, BuildingDef def)
  {
    tooltip.ClearMultiStringTooltip();
    string name = def.Name;
    string str1 = def.Effect;
    RocketModuleCluster component1 = def.BuildingComplete.GetComponent<RocketModuleCluster>();
    BuildingDef buildingDef = this.GetSelectionContext(def) == SelectModuleCondition.SelectionContext.ReplaceModule ? ((Component) this.module).GetComponent<Building>().Def : (BuildingDef) null;
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      string str2 = str1 + "\n\n" + (string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.TITLE;
      float burden = component1.performanceStats.burden;
      float kilogramPerDistance = component1.performanceStats.FuelKilogramPerDistance;
      float enginePower = component1.performanceStats.enginePower;
      int heightInCells = ((Component) component1).GetComponent<Building>().Def.HeightInCells;
      CraftModuleInterface craftModuleInterface = (CraftModuleInterface) null;
      float num1 = 0.0f;
      float num2 = 0.0f;
      float num3 = 0.0f;
      int num4 = 0;
      if (Object.op_Inequality((Object) ((Component) this).GetComponentInParent<DetailsScreen>(), (Object) null) && Object.op_Inequality((Object) ((Component) this).GetComponentInParent<DetailsScreen>().target.GetComponent<RocketModuleCluster>(), (Object) null))
        craftModuleInterface = ((Component) this).GetComponentInParent<DetailsScreen>().target.GetComponent<RocketModuleCluster>().CraftInterface;
      int num5 = -1;
      if (Object.op_Inequality((Object) craftModuleInterface, (Object) null))
        num5 = craftModuleInterface.MaxHeight;
      RocketEngineCluster component2 = ((Component) component1).GetComponent<RocketEngineCluster>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        num5 = component2.maxHeight;
      float num6;
      float num7;
      float num8;
      float num9;
      float num10;
      int num11;
      if (Object.op_Equality((Object) craftModuleInterface, (Object) null))
      {
        num6 = burden;
        num7 = kilogramPerDistance;
        num8 = enginePower;
        num9 = num8 / num6;
        num10 = num9;
        num11 = heightInCells;
      }
      else
      {
        if (Object.op_Inequality((Object) buildingDef, (Object) null))
        {
          RocketModulePerformance performanceStats = ((Component) this.module).GetComponent<RocketModuleCluster>().performanceStats;
          float num12 = num1 - performanceStats.burden;
          float num13 = num2 - performanceStats.fuelKilogramPerDistance;
          float num14 = num3 - performanceStats.enginePower;
          int num15 = num4 - buildingDef.HeightInCells;
        }
        num6 = burden + craftModuleInterface.TotalBurden;
        num7 = kilogramPerDistance + craftModuleInterface.Range;
        num8 = component1.performanceStats.enginePower + craftModuleInterface.EnginePower;
        num9 = (component1.performanceStats.enginePower + craftModuleInterface.EnginePower) / num6;
        num10 = num9 - craftModuleInterface.EnginePower / craftModuleInterface.TotalBurden;
        num11 = craftModuleInterface.RocketHeight + heightInCells;
      }
      string str3 = (double) burden >= 0.0 ? GameUtil.AddPositiveSign(string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, (object) burden), true) : string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, (object) burden);
      string str4 = (double) kilogramPerDistance >= 0.0 ? GameUtil.AddPositiveSign(string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, (object) Math.Round((double) kilogramPerDistance, 2)), true) : string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, (object) Math.Round((double) kilogramPerDistance, 2));
      string str5 = (double) enginePower >= 0.0 ? GameUtil.AddPositiveSign(string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, (object) enginePower), true) : string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, (object) enginePower);
      string str6 = (double) num10 >= (double) num9 ? GameUtil.AddPositiveSign(string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, (object) Math.Round((double) num10, 3)), true) : string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, (object) Math.Round((double) num10, 2));
      string str7 = heightInCells >= 0 ? GameUtil.AddPositiveSign(string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.POSITIVEDELTA, (object) heightInCells), true) : string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.NEGATIVEDELTA, (object) heightInCells);
      str1 = (num5 == -1 ? str2 + "\n" + string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.HEIGHT_NOMAX, (object) num11, (object) str7) : str2 + "\n" + string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.HEIGHT, (object) num11, (object) str7, (object) num5)) + "\n" + string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.BURDEN, (object) num6, (object) str3) + "\n" + string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.RANGE, (object) Math.Round((double) num7, 2), (object) str4) + "\n" + string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.ENGINEPOWER, (object) num8, (object) str5) + "\n" + string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.MODULESTATCHANGE.SPEED, (object) Math.Round((double) num9, 3), (object) str6);
      if (Object.op_Inequality((Object) ((Component) component1).GetComponent<RocketEngineCluster>(), (Object) null))
        str1 = str1 + "\n\n" + string.Format((string) STRINGS.UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.ENGINE_MAX_HEIGHT, (object) num5);
    }
    tooltip.AddMultiStringTooltip(name, PlanScreen.Instance.buildingToolTipSettings.BuildButtonName);
    tooltip.AddMultiStringTooltip(str1, PlanScreen.Instance.buildingToolTipSettings.BuildButtonDescription);
    this.AddErrorTooltips(tooltip, def);
  }

  private SelectModuleCondition.SelectionContext GetSelectionContext(BuildingDef def)
  {
    SelectModuleCondition.SelectionContext selectionContext = SelectModuleCondition.SelectionContext.AddModuleAbove;
    if (Object.op_Equality((Object) this.launchPad, (Object) null))
    {
      if (!this.addingNewModule)
      {
        selectionContext = SelectModuleCondition.SelectionContext.ReplaceModule;
      }
      else
      {
        List<SelectModuleCondition> buildConditions = Assets.GetPrefab(((Component) this.module).GetComponent<KPrefabID>().PrefabID()).GetComponent<ReorderableBuilding>().buildConditions;
        ReorderableBuilding component = def.BuildingComplete.GetComponent<ReorderableBuilding>();
        if (buildConditions.Find((Predicate<SelectModuleCondition>) (match => match is TopOnly)) != null || component.buildConditions.Find((Predicate<SelectModuleCondition>) (match => match is EngineOnBottom)) != null)
          selectionContext = SelectModuleCondition.SelectionContext.AddModuleBelow;
      }
    }
    return selectionContext;
  }

  private string GetErrorTooltips(BuildingDef def)
  {
    List<SelectModuleCondition> buildConditions = def.BuildingComplete.GetComponent<ReorderableBuilding>().buildConditions;
    SelectModuleCondition.SelectionContext selectionContext = this.GetSelectionContext(def);
    string errorTooltips = "";
    for (int index = 0; index < buildConditions.Count; ++index)
    {
      if (!buildConditions[index].IgnoreInSanboxMode() || !DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive)
      {
        GameObject gameObject = Object.op_Equality((Object) this.module, (Object) null) ? ((Component) this.launchPad).gameObject : ((Component) this.module).gameObject;
        if (!buildConditions[index].EvaluateCondition(gameObject, def, selectionContext))
        {
          if (!string.IsNullOrEmpty(errorTooltips))
            errorTooltips += "\n";
          errorTooltips += buildConditions[index].GetStatusTooltip(false, gameObject, def);
        }
      }
    }
    return errorTooltips;
  }

  private void AddErrorTooltips(ToolTip tooltip, BuildingDef def, bool clearFirst = false)
  {
    if (clearFirst)
      tooltip.ClearMultiStringTooltip();
    if (!clearFirst)
      tooltip.AddMultiStringTooltip("\n", PlanScreen.Instance.buildingToolTipSettings.MaterialRequirement);
    tooltip.AddMultiStringTooltip(this.GetErrorTooltips(def), PlanScreen.Instance.buildingToolTipSettings.MaterialRequirement);
  }

  public void SelectModule(BuildingDef def)
  {
    this.selectedModuleDef = def;
    this.ConfigureMaterialSelector();
    this.SetButtonColors();
    this.UpdateBuildButton();
    this.AddErrorTooltips(((Component) this.buildSelectedModuleButton).GetComponent<ToolTip>(), this.selectedModuleDef, true);
  }

  private void OrderBuildSelectedModule()
  {
    BuildingDef selectedModuleDef = this.selectedModuleDef;
    GameObject gameObject1;
    if (Object.op_Inequality((Object) this.module, (Object) null))
    {
      GameObject gameObject2 = ((Component) this.module).gameObject;
      gameObject1 = !this.addingNewModule ? ((Component) this.module).GetComponent<ReorderableBuilding>().ConvertModule(this.selectedModuleDef, this.materialSelectionPanel.GetSelectedElementAsList) : ((Component) this.module).GetComponent<ReorderableBuilding>().AddModule(this.selectedModuleDef, this.materialSelectionPanel.GetSelectedElementAsList);
    }
    else
      gameObject1 = this.launchPad.AddBaseModule(this.selectedModuleDef, this.materialSelectionPanel.GetSelectedElementAsList);
    if (!Object.op_Inequality((Object) gameObject1, (Object) null))
      return;
    Vector2 anchoredPosition = ((ScrollRect) this.mainContents.GetComponent<KScrollRect>()).content.anchoredPosition;
    ((MonoBehaviour) SelectTool.Instance).StartCoroutine(this.SelectNextFrame(gameObject1.GetComponent<KSelectable>(), selectedModuleDef, anchoredPosition.y));
  }

  private IEnumerator SelectNextFrame(
    KSelectable selectable,
    BuildingDef previousSelectedDef,
    float scrollPosition)
  {
    yield return (object) 0;
    SelectTool.Instance.Select(selectable);
    RocketModuleSideScreen.instance.ClickAddNew(scrollPosition, previousSelectedDef);
  }
}
