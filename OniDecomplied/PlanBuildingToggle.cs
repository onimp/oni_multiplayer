// Decompiled with JetBrains decompiler
// Type: PlanBuildingToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanBuildingToggle : KToggle
{
  private BuildingDef def;
  private HashedString buildingCategory;
  private TechItem techItem;
  private List<int> gameSubscriptions = new List<int>();
  private bool researchComplete;
  private Sprite sprite;
  [SerializeField]
  private ToolTip tooltip;
  [SerializeField]
  private LocText text;
  [SerializeField]
  private ImageToggleState imageToggleState;
  [SerializeField]
  private Image buildingIcon;
  [SerializeField]
  private Image fgIcon;

  public void Config(BuildingDef def)
  {
    this.def = def;
    this.techItem = Db.Get().TechItems.TryGet(def.PrefabID);
    this.gameSubscriptions.Add(Game.Instance.Subscribe(-107300940, new Action<object>(this.CheckResearch)));
    this.gameSubscriptions.Add(Game.Instance.Subscribe(-1948169901, new Action<object>(this.CheckResearch)));
    this.gameSubscriptions.Add(Game.Instance.Subscribe(1557339983, new Action<object>(this.CheckResearch)));
    this.sprite = def.GetUISprite();
    this.onClick += (System.Action) (() => PlanScreen.Instance.OnSelectBuilding(((Component) this).gameObject, def));
    this.CheckResearch();
    this.Refresh();
  }

  protected virtual void OnDestroy()
  {
    if (Object.op_Inequality((Object) Game.Instance, (Object) null))
    {
      foreach (int gameSubscription in this.gameSubscriptions)
        Game.Instance.Unsubscribe(gameSubscription);
    }
    this.gameSubscriptions.Clear();
    ((Toggle) this).OnDestroy();
  }

  private void CheckResearch(object data = null) => this.researchComplete = PlanScreen.TechRequirementsMet(this.techItem);

  public bool Refresh()
  {
    bool flag1 = this.researchComplete || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;
    bool flag2 = false;
    if (((Component) this).gameObject.activeSelf != flag1)
    {
      ((Component) this).gameObject.SetActive(flag1);
      flag2 = true;
    }
    if (!((Component) this).gameObject.activeInHierarchy || Object.op_Equality((Object) this.bgImage, (Object) null))
      return flag2;
    this.PositionTooltip();
    this.RefreshLabel();
    this.RefreshDisplay();
    return flag2;
  }

  private void RefreshLabel()
  {
    if (!Object.op_Inequality((Object) this.text, (Object) null))
      return;
    ((TMP_Text) this.text).fontSize = ScreenResolutionMonitor.UsingGamepadUIMode() ? (float) PlanScreen.fontSizeBigMode : (float) PlanScreen.fontSizeStandardMode;
    ((TMP_Text) this.text).text = this.def.Name;
  }

  private void RefreshDisplay()
  {
    PlanScreen.RequirementsState buildableState = PlanScreen.Instance.GetBuildableState(this.def);
    bool buttonAvailable = buildableState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;
    bool flag = Object.op_Equality((Object) ((Component) this).gameObject, (Object) PlanScreen.Instance.SelectedBuildingGameObject);
    ImageToggleState.State state = buildableState == PlanScreen.RequirementsState.Complete ? (ImageToggleState.State) 1 : (ImageToggleState.State) 0;
    if (flag & buttonAvailable)
      state = (ImageToggleState.State) 2;
    else if (!flag & buttonAvailable)
      state = (ImageToggleState.State) 1;
    else if (flag && !buttonAvailable)
      state = (ImageToggleState.State) 3;
    else if (!flag && !buttonAvailable)
      state = (ImageToggleState.State) 0;
    this.imageToggleState.SetState(state);
    this.RefreshBuildingButtonIconAndColors(buttonAvailable);
    this.RefreshFG(buildableState);
  }

  private void PositionTooltip()
  {
    this.tooltip.overrideParentObject = PlanScreen.Instance.buildingGroupsRoot;
    this.tooltip.tooltipPivot = Vector2.zero;
    this.tooltip.parentPositionAnchor = new Vector2(1f, 0.0f);
    this.tooltip.tooltipPositionOffset = ((Component) PlanScreen.Instance.ProductInfoScreen).gameObject.activeSelf ? new Vector2(16f + Util.rectTransform((Component) PlanScreen.Instance.ProductInfoScreen).sizeDelta.x, 0.0f) : new Vector2(-40f, 0.0f);
    this.tooltip.ClearMultiStringTooltip();
    string name = this.def.Name;
    string effect = this.def.Effect;
    this.tooltip.AddMultiStringTooltip(name, PlanScreen.Instance.buildingToolTipSettings.BuildButtonName);
    this.tooltip.AddMultiStringTooltip(effect, PlanScreen.Instance.buildingToolTipSettings.BuildButtonDescription);
  }

  private void RefreshBuildingButtonIconAndColors(bool buttonAvailable)
  {
    if (Object.op_Equality((Object) this.sprite, (Object) null))
      this.sprite = PlanScreen.Instance.defaultBuildingIconSprite;
    this.buildingIcon.sprite = this.sprite;
    ((Graphic) this.buildingIcon).SetNativeSize();
    float num = ScreenResolutionMonitor.UsingGamepadUIMode() ? 3.25f : 4f;
    RectTransform rectTransform = Util.rectTransform((Component) this.buildingIcon);
    rectTransform.sizeDelta = Vector2.op_Division(rectTransform.sizeDelta, num);
    Material material = buttonAvailable ? PlanScreen.Instance.defaultUIMaterial : PlanScreen.Instance.desaturatedUIMaterial;
    if (!Object.op_Inequality((Object) ((Graphic) this.buildingIcon).material, (Object) material))
      return;
    ((Graphic) this.buildingIcon).material = material;
    if (!buttonAvailable)
    {
      if (this.researchComplete)
        ((Graphic) this.buildingIcon).color = new Color(1f, 1f, 1f, 0.6f);
      else
        ((Graphic) this.buildingIcon).color = new Color(1f, 1f, 1f, 0.15f);
    }
    else
      ((Graphic) this.buildingIcon).color = Color.white;
  }

  private void RefreshFG(PlanScreen.RequirementsState requirementsState)
  {
    if (requirementsState == PlanScreen.RequirementsState.Tech)
    {
      this.fgImage.sprite = PlanScreen.Instance.Overlay_NeedTech;
      ((Component) this.fgImage).gameObject.SetActive(true);
    }
    else
      ((Component) this.fgImage).gameObject.SetActive(false);
    string requirementsState1 = PlanScreen.GetTooltipForRequirementsState(this.def, requirementsState);
    if (requirementsState1 == null)
      return;
    this.tooltip.AddMultiStringTooltip("\n", PlanScreen.Instance.buildingToolTipSettings.ResearchRequirement);
    this.tooltip.AddMultiStringTooltip(requirementsState1, PlanScreen.Instance.buildingToolTipSettings.ResearchRequirement);
  }
}
