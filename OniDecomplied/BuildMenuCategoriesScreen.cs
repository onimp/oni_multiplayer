// Decompiled with JetBrains decompiler
// Type: BuildMenuCategoriesScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuCategoriesScreen : KIconToggleMenu
{
  public Action<HashedString, int> onCategoryClicked;
  [SerializeField]
  public bool modalKeyInputBehaviour;
  [SerializeField]
  private Image focusIndicator;
  [SerializeField]
  private Color32 focusedColour;
  [SerializeField]
  private Color32 unfocusedColour;
  private IList<HashedString> subcategories;
  private Dictionary<HashedString, List<BuildingDef>> categorizedBuildingMap;
  private Dictionary<HashedString, List<HashedString>> categorizedCategoryMap;
  private BuildMenuBuildingsScreen buildingsScreen;
  private HashedString category;
  private IList<BuildMenu.BuildingInfo> buildingInfos;
  private HashedString selectedCategory = HashedString.Invalid;

  public virtual float GetSortKey() => 7f;

  public HashedString Category => this.category;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.onSelect += new KIconToggleMenu.OnSelect(this.OnClickCategory);
  }

  public void Configure(
    HashedString category,
    int depth,
    object data,
    Dictionary<HashedString, List<BuildingDef>> categorized_building_map,
    Dictionary<HashedString, List<HashedString>> categorized_category_map,
    BuildMenuBuildingsScreen buildings_screen)
  {
    this.category = category;
    this.categorizedBuildingMap = categorized_building_map;
    this.categorizedCategoryMap = categorized_category_map;
    this.buildingsScreen = buildings_screen;
    List<KIconToggleMenu.ToggleInfo> toggleInfo1 = new List<KIconToggleMenu.ToggleInfo>();
    if (typeof (IList<BuildMenu.BuildingInfo>).IsAssignableFrom(data.GetType()))
      this.buildingInfos = (IList<BuildMenu.BuildingInfo>) data;
    else if (typeof (IList<BuildMenu.DisplayInfo>).IsAssignableFrom(data.GetType()))
    {
      this.subcategories = (IList<HashedString>) new List<HashedString>();
      foreach (BuildMenu.DisplayInfo displayInfo in (IEnumerable<BuildMenu.DisplayInfo>) data)
      {
        string iconName = displayInfo.iconName;
        string str = HashCache.Get().Get(displayInfo.category).ToUpper().Replace(" ", "");
        string text = StringEntry.op_Implicit(Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + str + ".NAME"));
        string icon = iconName;
        BuildMenuCategoriesScreen.UserData user_data = new BuildMenuCategoriesScreen.UserData();
        user_data.category = displayInfo.category;
        user_data.depth = depth;
        user_data.requirementsState = PlanScreen.RequirementsState.Tech;
        Action hotkey = displayInfo.hotkey;
        string tooltip = StringEntry.op_Implicit(Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + str + ".TOOLTIP"));
        KIconToggleMenu.ToggleInfo toggleInfo2 = new KIconToggleMenu.ToggleInfo(text, icon, (object) user_data, hotkey, tooltip);
        toggleInfo1.Add(toggleInfo2);
        ((ICollection<HashedString>) this.subcategories).Add(displayInfo.category);
      }
      this.Setup((IList<KIconToggleMenu.ToggleInfo>) toggleInfo1);
      this.toggles.ForEach((Action<KToggle>) (to =>
      {
        foreach (ImageToggleState component in ((Component) to).GetComponents<ImageToggleState>())
        {
          if (Object.op_Inequality((Object) component.TargetImage.sprite, (Object) null) && ((Object) component.TargetImage).name == "FG" && !component.useSprites)
            component.SetSprites(Assets.GetSprite(HashedString.op_Implicit(((Object) component.TargetImage.sprite).name + "_disabled")), component.TargetImage.sprite, component.TargetImage.sprite, Assets.GetSprite(HashedString.op_Implicit(((Object) component.TargetImage.sprite).name + "_disabled")));
        }
        ((WidgetSoundPlayer) ((Component) to).GetComponent<KToggle>().soundPlayer).Enabled = false;
      }));
    }
    this.UpdateBuildableStates(true);
  }

  private void OnClickCategory(KIconToggleMenu.ToggleInfo toggle_info)
  {
    BuildMenuCategoriesScreen.UserData userData = (BuildMenuCategoriesScreen.UserData) toggle_info.userData;
    switch (userData.requirementsState)
    {
      case PlanScreen.RequirementsState.Materials:
      case PlanScreen.RequirementsState.Complete:
        if (HashedString.op_Inequality(this.selectedCategory, userData.category))
        {
          this.selectedCategory = userData.category;
          KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
          break;
        }
        this.selectedCategory = HashedString.Invalid;
        this.ClearSelection();
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect"));
        break;
      default:
        this.selectedCategory = HashedString.Invalid;
        this.ClearSelection();
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
        break;
    }
    ((Component) toggle_info.toggle).GetComponent<PlanCategoryNotifications>().ToggleAttention(false);
    if (this.onCategoryClicked == null)
      return;
    this.onCategoryClicked(this.selectedCategory, userData.depth);
  }

  private void UpdateButtonStates()
  {
    if (this.toggleInfo == null || this.toggleInfo.Count <= 0)
      return;
    foreach (KIconToggleMenu.ToggleInfo toggleInfo in (IEnumerable<KIconToggleMenu.ToggleInfo>) this.toggleInfo)
    {
      BuildMenuCategoriesScreen.UserData userData = (BuildMenuCategoriesScreen.UserData) toggleInfo.userData;
      HashedString category = userData.category;
      PlanScreen.RequirementsState categoryRequirements = this.GetCategoryRequirements(category);
      bool flag = categoryRequirements == PlanScreen.RequirementsState.Tech;
      ((Component) toggleInfo.toggle).gameObject.SetActive(!flag);
      switch (categoryRequirements)
      {
        case PlanScreen.RequirementsState.Materials:
          KMonoBehaviourExtensions.SetAlpha(toggleInfo.toggle.fgImage, flag ? 0.2509804f : 1f);
          ImageToggleState.State state1 = !((HashedString) ref this.selectedCategory).IsValid || !HashedString.op_Equality(category, this.selectedCategory) ? (ImageToggleState.State) 0 : (ImageToggleState.State) 3;
          if (!userData.currentToggleState.HasValue || userData.currentToggleState.GetValueOrDefault() != state1)
          {
            userData.currentToggleState = new ImageToggleState.State?(state1);
            this.SetImageToggleState(((Component) toggleInfo.toggle).gameObject, state1);
            break;
          }
          break;
        case PlanScreen.RequirementsState.Complete:
          ImageToggleState.State state2 = !((HashedString) ref this.selectedCategory).IsValid || HashedString.op_Inequality(category, this.selectedCategory) ? (ImageToggleState.State) 1 : (ImageToggleState.State) 2;
          if (!userData.currentToggleState.HasValue || userData.currentToggleState.GetValueOrDefault() != state2)
          {
            userData.currentToggleState = new ImageToggleState.State?(state2);
            this.SetImageToggleState(((Component) toggleInfo.toggle).gameObject, state2);
            break;
          }
          break;
      }
      ((Component) ((Component) toggleInfo.toggle.fgImage).transform.Find("ResearchIcon")).gameObject.gameObject.SetActive(flag);
    }
  }

  private void SetImageToggleState(GameObject target, ImageToggleState.State state)
  {
    foreach (ImageToggleState component in target.GetComponents<ImageToggleState>())
      component.SetState(state);
  }

  private PlanScreen.RequirementsState GetCategoryRequirements(HashedString category)
  {
    bool flag1 = true;
    bool flag2 = true;
    List<BuildingDef> buildingDefList;
    if (this.categorizedBuildingMap.TryGetValue(category, out buildingDefList))
    {
      if (buildingDefList.Count > 0)
      {
        foreach (BuildingDef def in buildingDefList)
        {
          if (def.ShowInBuildMenu && def.IsAvailable())
          {
            PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
            flag1 = flag1 && requirementsState == PlanScreen.RequirementsState.Tech;
            flag2 = flag2 && (requirementsState == PlanScreen.RequirementsState.Materials || requirementsState == PlanScreen.RequirementsState.Tech);
          }
        }
      }
    }
    else
    {
      List<HashedString> hashedStringList;
      if (this.categorizedCategoryMap.TryGetValue(category, out hashedStringList))
      {
        foreach (HashedString category1 in hashedStringList)
        {
          PlanScreen.RequirementsState categoryRequirements = this.GetCategoryRequirements(category1);
          flag1 = flag1 && categoryRequirements == PlanScreen.RequirementsState.Tech;
          flag2 = flag2 && (categoryRequirements == PlanScreen.RequirementsState.Materials || categoryRequirements == PlanScreen.RequirementsState.Tech);
        }
      }
    }
    PlanScreen.RequirementsState categoryRequirements1 = !flag1 ? (!flag2 ? PlanScreen.RequirementsState.Complete : PlanScreen.RequirementsState.Materials) : PlanScreen.RequirementsState.Tech;
    if (DebugHandler.InstantBuildMode)
      categoryRequirements1 = PlanScreen.RequirementsState.Complete;
    return categoryRequirements1;
  }

  public void UpdateNotifications(ICollection<HashedString> updated_categories)
  {
    if (this.toggleInfo == null)
      return;
    this.UpdateBuildableStates(false);
    foreach (KIconToggleMenu.ToggleInfo toggleInfo in (IEnumerable<KIconToggleMenu.ToggleInfo>) this.toggleInfo)
    {
      HashedString category = ((BuildMenuCategoriesScreen.UserData) toggleInfo.userData).category;
      if (updated_categories.Contains(category))
        ((Component) toggleInfo.toggle).gameObject.GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
    }
  }

  public override void Close()
  {
    base.Close();
    this.selectedCategory = HashedString.Invalid;
    base.SetHasFocus(false);
    if (this.buildingInfos == null)
      return;
    this.buildingsScreen.Close();
  }

  [ContextMenu("ForceUpdateBuildableStates")]
  private void ForceUpdateBuildableStates() => this.UpdateBuildableStates(true);

  public void UpdateBuildableStates(bool skip_flourish)
  {
    if (this.subcategories != null && ((ICollection<HashedString>) this.subcategories).Count > 0)
    {
      this.UpdateButtonStates();
      foreach (KIconToggleMenu.ToggleInfo toggleInfo in (IEnumerable<KIconToggleMenu.ToggleInfo>) this.toggleInfo)
      {
        BuildMenuCategoriesScreen.UserData userData = (BuildMenuCategoriesScreen.UserData) toggleInfo.userData;
        PlanScreen.RequirementsState categoryRequirements = this.GetCategoryRequirements(userData.category);
        if (userData.requirementsState != categoryRequirements)
        {
          userData.requirementsState = categoryRequirements;
          toggleInfo.userData = (object) userData;
          if (!skip_flourish)
          {
            toggleInfo.toggle.ActivateFlourish(false);
            string str = "NotificationPing";
            AnimatorStateInfo animatorStateInfo = ((Component) toggleInfo.toggle).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            if (!((AnimatorStateInfo) ref animatorStateInfo).IsTag(str))
            {
              ((Component) toggleInfo.toggle).gameObject.GetComponent<Animator>().Play(str);
              BuildMenu.Instance.PlayNewBuildingSounds();
            }
          }
        }
      }
    }
    else
      this.buildingsScreen.UpdateBuildableStates();
  }

  protected virtual void OnShow(bool show)
  {
    if (this.buildingInfos != null)
    {
      if (show)
      {
        this.buildingsScreen.Configure(this.category, this.buildingInfos);
        this.buildingsScreen.Show(true);
      }
      else
        this.buildingsScreen.Close();
    }
    base.OnShow(show);
  }

  public override void ClearSelection()
  {
    this.selectedCategory = HashedString.Invalid;
    base.ClearSelection();
    foreach (KToggle toggle in this.toggles)
      toggle.isOn = false;
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (this.modalKeyInputBehaviour)
    {
      if (!this.HasFocus)
        return;
      if (e.TryConsume((Action) 1))
      {
        Game.Instance.Trigger(288942073, (object) null);
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
    else
    {
      base.OnKeyDown(e);
      if (!((KInputEvent) e).Consumed)
        return;
      this.UpdateButtonStates();
    }
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (this.modalKeyInputBehaviour)
    {
      if (!this.HasFocus)
        return;
      if (e.TryConsume((Action) 1))
      {
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
    else
      base.OnKeyUp(e);
  }

  public virtual void SetHasFocus(bool has_focus)
  {
    base.SetHasFocus(has_focus);
    if (!Object.op_Inequality((Object) this.focusIndicator, (Object) null))
      return;
    ((Graphic) this.focusIndicator).color = Color32.op_Implicit(has_focus ? this.focusedColour : this.unfocusedColour);
  }

  private class UserData
  {
    public HashedString category;
    public int depth;
    public PlanScreen.RequirementsState requirementsState;
    public ImageToggleState.State? currentToggleState;
  }
}
