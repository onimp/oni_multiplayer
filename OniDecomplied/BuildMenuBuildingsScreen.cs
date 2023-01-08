// Decompiled with JetBrains decompiler
// Type: BuildMenuBuildingsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuBuildingsScreen : KIconToggleMenu
{
  [SerializeField]
  private Image focusIndicator;
  [SerializeField]
  private Color32 focusedColour;
  [SerializeField]
  private Color32 unfocusedColour;
  public Action<BuildingDef> onBuildingSelected;
  [SerializeField]
  private LocText titleLabel;
  [SerializeField]
  private BuildMenuBuildingsScreen.BuildingToolTipSettings buildingToolTipSettings;
  [SerializeField]
  private LayoutElement contentSizeLayout;
  [SerializeField]
  private GridLayoutGroup gridSizer;
  [SerializeField]
  private Sprite Overlay_NeedTech;
  [SerializeField]
  private Material defaultUIMaterial;
  [SerializeField]
  private Material desaturatedUIMaterial;
  private BuildingDef selectedBuilding;

  public virtual float GetSortKey() => 8f;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.UpdateBuildableStates();
    Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
    this.onSelect += new KIconToggleMenu.OnSelect(this.OnClickBuilding);
    Game.Instance.Subscribe(-1190690038, new Action<object>(this.OnBuildToolDeactivated));
  }

  public void Configure(HashedString category, IList<BuildMenu.BuildingInfo> building_infos)
  {
    this.ClearButtons();
    base.SetHasFocus(true);
    List<KIconToggleMenu.ToggleInfo> toggleInfo1 = new List<KIconToggleMenu.ToggleInfo>();
    ((TMP_Text) this.titleLabel).text = StringEntry.op_Implicit(Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + HashCache.Get().Get(category).ToUpper().Replace(" ", "") + ".BUILDMENUTITLE"));
    foreach (BuildMenu.BuildingInfo buildingInfo in (IEnumerable<BuildMenu.BuildingInfo>) building_infos)
    {
      BuildingDef def = Assets.GetBuildingDef(buildingInfo.id);
      if (def.ShouldShowInBuildMenu() && def.IsAvailable())
      {
        KIconToggleMenu.ToggleInfo toggleInfo2 = new KIconToggleMenu.ToggleInfo(def.Name, (object) new BuildMenuBuildingsScreen.UserData(def, PlanScreen.RequirementsState.Tech), def.HotKey, (Func<Sprite>) (() => def.GetUISprite()));
        toggleInfo1.Add(toggleInfo2);
      }
    }
    this.Setup((IList<KIconToggleMenu.ToggleInfo>) toggleInfo1);
    for (int index = 0; index < this.toggleInfo.Count; ++index)
      this.RefreshToggle(this.toggleInfo[index]);
    int num1 = 0;
    foreach (Component component in ((Component) this.gridSizer).transform)
    {
      if (component.gameObject.activeSelf)
        ++num1;
    }
    this.gridSizer.constraintCount = Mathf.Min(num1, 3);
    int num2 = Mathf.Min(num1, this.gridSizer.constraintCount);
    int num3 = (num1 + this.gridSizer.constraintCount - 1) / this.gridSizer.constraintCount;
    int num4 = num2 - 1;
    int num5 = num3 - 1;
    Vector2 vector2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2).\u002Ector((float) ((double) num2 * (double) this.gridSizer.cellSize.x + (double) num4 * (double) this.gridSizer.spacing.x) + (float) ((LayoutGroup) this.gridSizer).padding.left + (float) ((LayoutGroup) this.gridSizer).padding.right, (float) ((double) num3 * (double) this.gridSizer.cellSize.y + (double) num5 * (double) this.gridSizer.spacing.y) + (float) ((LayoutGroup) this.gridSizer).padding.top + (float) ((LayoutGroup) this.gridSizer).padding.bottom);
    this.contentSizeLayout.minWidth = vector2.x;
    this.contentSizeLayout.minHeight = vector2.y;
  }

  private void ConfigureToolTip(ToolTip tooltip, BuildingDef def)
  {
    tooltip.ClearMultiStringTooltip();
    tooltip.AddMultiStringTooltip(def.Name, this.buildingToolTipSettings.BuildButtonName);
    tooltip.AddMultiStringTooltip(def.Effect, this.buildingToolTipSettings.BuildButtonDescription);
  }

  public void CloseRecipe(bool playSound = false)
  {
    if (playSound)
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect"));
    ToolMenu.Instance.ClearSelection();
    this.DeactivateBuildTools();
    if (Object.op_Equality((Object) PlayerController.Instance.ActiveTool, (Object) PrebuildTool.Instance))
      SelectTool.Instance.Activate();
    this.selectedBuilding = (BuildingDef) null;
    this.onBuildingSelected(this.selectedBuilding);
  }

  private void RefreshToggle(KIconToggleMenu.ToggleInfo info)
  {
    if (info == null || Object.op_Equality((Object) info.toggle, (Object) null))
      return;
    BuildingDef def = (info.userData as BuildMenuBuildingsScreen.UserData).def;
    TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
    bool flag1 = DebugHandler.InstantBuildMode || techItem == null || techItem.IsComplete();
    bool flag2 = flag1 || techItem == null || techItem.ParentTech.ArePrerequisitesComplete();
    KToggle toggle = info.toggle;
    if (((Component) toggle).gameObject.activeSelf != flag2)
      ((Component) toggle).gameObject.SetActive(flag2);
    if (Object.op_Equality((Object) toggle.bgImage, (Object) null))
      return;
    Image componentsInChild = ((Component) toggle.bgImage).GetComponentsInChildren<Image>()[1];
    Sprite uiSprite = def.GetUISprite();
    componentsInChild.sprite = uiSprite;
    ((Graphic) componentsInChild).SetNativeSize();
    RectTransform rectTransform = Util.rectTransform((Component) componentsInChild);
    rectTransform.sizeDelta = Vector2.op_Division(rectTransform.sizeDelta, 4f);
    ToolTip component = ((Component) toggle).gameObject.GetComponent<ToolTip>();
    component.ClearMultiStringTooltip();
    string name = def.Name;
    string effect = def.Effect;
    if (def.HotKey != 275)
      name += GameUtil.GetHotkeyString(def.HotKey);
    component.AddMultiStringTooltip(name, this.buildingToolTipSettings.BuildButtonName);
    component.AddMultiStringTooltip(effect, this.buildingToolTipSettings.BuildButtonDescription);
    LocText componentInChildren = ((Component) toggle).GetComponentInChildren<LocText>();
    if (Object.op_Inequality((Object) componentInChildren, (Object) null))
      ((TMP_Text) componentInChildren).text = def.Name;
    PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
    ImageToggleState.State state1 = requirementsState == PlanScreen.RequirementsState.Complete ? (ImageToggleState.State) 1 : (ImageToggleState.State) 0;
    ImageToggleState.State state2 = !Object.op_Equality((Object) def, (Object) this.selectedBuilding) || requirementsState != PlanScreen.RequirementsState.Complete && !DebugHandler.InstantBuildMode ? (requirementsState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode ? (ImageToggleState.State) 1 : (ImageToggleState.State) 0) : (ImageToggleState.State) 2;
    if (Object.op_Equality((Object) def, (Object) this.selectedBuilding) && state2 == null)
      state2 = (ImageToggleState.State) 3;
    else if (state2 == null)
      state2 = (ImageToggleState.State) 0;
    ((Component) toggle).GetComponent<ImageToggleState>().SetState(state2);
    Material material;
    Color color1;
    if (requirementsState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode)
    {
      material = this.defaultUIMaterial;
      color1 = Color.white;
    }
    else
    {
      material = this.desaturatedUIMaterial;
      Color color2;
      if (!flag1)
      {
        Image image = componentsInChild;
        Color color3;
        // ISSUE: explicit constructor call
        ((Color) ref color3).\u002Ector(1f, 1f, 1f, 0.15f);
        Color color4 = color3;
        ((Graphic) image).color = color4;
        color2 = color3;
      }
      else
        color2 = new Color(1f, 1f, 1f, 0.6f);
      color1 = color2;
    }
    if (Object.op_Inequality((Object) ((Graphic) componentsInChild).material, (Object) material))
    {
      ((Graphic) componentsInChild).material = material;
      ((Graphic) componentsInChild).color = color1;
    }
    Image fgImage = ((Component) toggle).gameObject.GetComponent<KToggle>().fgImage;
    ((Component) fgImage).gameObject.SetActive(false);
    if (!flag1)
    {
      fgImage.sprite = this.Overlay_NeedTech;
      ((Component) fgImage).gameObject.SetActive(true);
      string str = string.Format((string) STRINGS.UI.PRODUCTINFO_REQUIRESRESEARCHDESC, (object) techItem.ParentTech.Name);
      component.AddMultiStringTooltip("\n", this.buildingToolTipSettings.ResearchRequirement);
      component.AddMultiStringTooltip(str, this.buildingToolTipSettings.ResearchRequirement);
    }
    else
    {
      if (requirementsState == PlanScreen.RequirementsState.Complete)
        return;
      ((Component) fgImage).gameObject.SetActive(false);
      component.AddMultiStringTooltip("\n", this.buildingToolTipSettings.ResearchRequirement);
      string missingresourcesHover = (string) STRINGS.UI.PRODUCTINFO_MISSINGRESOURCES_HOVER;
      component.AddMultiStringTooltip(missingresourcesHover, this.buildingToolTipSettings.ResearchRequirement);
      foreach (Recipe.Ingredient ingredient in def.CraftRecipe.Ingredients)
      {
        string str = string.Format("{0}{1}: {2}", (object) "• ", (object) ingredient.tag.ProperName(), (object) GameUtil.GetFormattedMass(ingredient.amount));
        component.AddMultiStringTooltip(str, this.buildingToolTipSettings.ResearchRequirement);
      }
      component.AddMultiStringTooltip("", this.buildingToolTipSettings.ResearchRequirement);
    }
  }

  public void ClearUI()
  {
    this.Show(false);
    this.ClearButtons();
  }

  private void ClearButtons()
  {
    foreach (KToggle toggle in this.toggles)
    {
      ((Component) toggle).gameObject.SetActive(false);
      ((Component) toggle).gameObject.transform.SetParent((Transform) null);
      Object.DestroyImmediate((Object) ((Component) toggle).gameObject);
    }
    if (this.toggles != null)
      this.toggles.Clear();
    if (this.toggleInfo == null)
      return;
    this.toggleInfo.Clear();
  }

  private void OnClickBuilding(KIconToggleMenu.ToggleInfo toggle_info) => this.OnSelectBuilding((toggle_info.userData as BuildMenuBuildingsScreen.UserData).def);

  private void OnSelectBuilding(BuildingDef def)
  {
    switch (BuildMenu.Instance.BuildableState(def))
    {
      case PlanScreen.RequirementsState.Materials:
      case PlanScreen.RequirementsState.Complete:
        if (Object.op_Inequality((Object) def, (Object) this.selectedBuilding))
        {
          this.selectedBuilding = def;
          KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
          break;
        }
        this.selectedBuilding = (BuildingDef) null;
        this.ClearSelection();
        this.CloseRecipe(true);
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect"));
        break;
      default:
        this.selectedBuilding = (BuildingDef) null;
        this.ClearSelection();
        this.CloseRecipe(true);
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
        break;
    }
    this.onBuildingSelected(this.selectedBuilding);
  }

  public void UpdateBuildableStates()
  {
    if (this.toggleInfo == null || this.toggleInfo.Count <= 0)
      return;
    BuildingDef def1 = (BuildingDef) null;
    foreach (KIconToggleMenu.ToggleInfo info in (IEnumerable<KIconToggleMenu.ToggleInfo>) this.toggleInfo)
    {
      this.RefreshToggle(info);
      BuildMenuBuildingsScreen.UserData userData = info.userData as BuildMenuBuildingsScreen.UserData;
      BuildingDef def2 = userData.def;
      if (def2.IsAvailable())
      {
        PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def2);
        if (requirementsState != userData.requirementsState)
        {
          if (Object.op_Equality((Object) def2, (Object) BuildMenu.Instance.SelectedBuildingDef))
            def1 = def2;
          this.RefreshToggle(info);
          userData.requirementsState = requirementsState;
        }
      }
    }
    if (!Object.op_Inequality((Object) def1, (Object) null))
      return;
    BuildMenu.Instance.RefreshProductInfoScreen(def1);
  }

  private void OnResearchComplete(object data) => this.UpdateBuildableStates();

  private void DeactivateBuildTools()
  {
    InterfaceTool activeTool = PlayerController.Instance.ActiveTool;
    if (!Object.op_Inequality((Object) activeTool, (Object) null))
      return;
    System.Type type = ((object) activeTool).GetType();
    if (!(type == typeof (BuildTool)) && !typeof (BaseUtilityBuildTool).IsAssignableFrom(type) && !typeof (PrebuildTool).IsAssignableFrom(type))
      return;
    activeTool.DeactivateTool();
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (this.mouseOver && this.ConsumeMouseScroll && !e.TryConsume((Action) 7))
      e.TryConsume((Action) 8);
    if (!this.HasFocus)
      return;
    if (e.TryConsume((Action) 1))
    {
      Game.Instance.Trigger(288942073, (object) null);
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
    }
    else
    {
      base.OnKeyDown(e);
      if (((KInputEvent) e).Consumed)
        return;
      Action action = e.GetAction();
      if (action < 35)
        return;
      e.TryConsume(action);
    }
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (!this.HasFocus)
      return;
    if (Object.op_Inequality((Object) this.selectedBuilding, (Object) null) && PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
    {
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
      Game.Instance.Trigger(288942073, (object) null);
    }
    else
    {
      base.OnKeyUp(e);
      if (((KInputEvent) e).Consumed)
        return;
      Action action = e.GetAction();
      if (action < 35)
        return;
      e.TryConsume(action);
    }
  }

  public override void Close()
  {
    ToolMenu.Instance.ClearSelection();
    this.DeactivateBuildTools();
    if (Object.op_Equality((Object) PlayerController.Instance.ActiveTool, (Object) PrebuildTool.Instance))
      SelectTool.Instance.Activate();
    this.selectedBuilding = (BuildingDef) null;
    this.ClearButtons();
    ((Component) this).gameObject.SetActive(false);
  }

  public virtual void SetHasFocus(bool has_focus)
  {
    base.SetHasFocus(has_focus);
    if (!Object.op_Inequality((Object) this.focusIndicator, (Object) null))
      return;
    ((Graphic) this.focusIndicator).color = Color32.op_Implicit(has_focus ? this.focusedColour : this.unfocusedColour);
  }

  private void OnBuildToolDeactivated(object data) => this.CloseRecipe();

  [Serializable]
  public struct BuildingToolTipSettings
  {
    public TextStyleSetting BuildButtonName;
    public TextStyleSetting BuildButtonDescription;
    public TextStyleSetting MaterialRequirement;
    public TextStyleSetting ResearchRequirement;
  }

  [Serializable]
  public struct BuildingNameTextSetting
  {
    public TextStyleSetting ActiveSelected;
    public TextStyleSetting ActiveDeselected;
    public TextStyleSetting InactiveSelected;
    public TextStyleSetting InactiveDeselected;
  }

  private class UserData
  {
    public BuildingDef def;
    public PlanScreen.RequirementsState requirementsState;

    public UserData(BuildingDef def, PlanScreen.RequirementsState state)
    {
      this.def = def;
      this.requirementsState = state;
    }
  }
}
