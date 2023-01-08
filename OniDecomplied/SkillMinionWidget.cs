// Decompiled with JetBrains decompiler
// Type: SkillMinionWidget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/SkillMinionWidget")]
public class SkillMinionWidget : 
  KMonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler,
  IPointerClickHandler
{
  [SerializeField]
  private SkillsScreen skillsScreen;
  [SerializeField]
  private CrewPortrait portrait;
  [SerializeField]
  private LocText masteryPoints;
  [SerializeField]
  private LocText morale;
  [SerializeField]
  private Image background;
  [SerializeField]
  private Image hat_background;
  [SerializeField]
  private Color selected_color;
  [SerializeField]
  private Color unselected_color;
  [SerializeField]
  private Color hover_color;
  [SerializeField]
  private DropDown hatDropDown;
  [SerializeField]
  private TextStyleSetting TooltipTextStyle_Header;
  [SerializeField]
  private TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

  public IAssignableIdentity assignableIdentity { get; private set; }

  public void SetMinon(IAssignableIdentity identity)
  {
    this.assignableIdentity = identity;
    this.portrait.SetIdentityObject(this.assignableIdentity);
    ((Component) this).GetComponent<NotificationHighlightTarget>().targetKey = ((Object) ((Component) identity.GetSoleOwner()).gameObject).GetInstanceID().ToString();
  }

  public void OnPointerEnter(PointerEventData eventData) => this.ToggleHover(true);

  public void OnPointerExit(PointerEventData eventData) => this.ToggleHover(false);

  private void ToggleHover(bool on)
  {
    if (this.skillsScreen.CurrentlySelectedMinion == this.assignableIdentity)
      return;
    this.SetColor(on ? this.hover_color : this.unselected_color);
  }

  private void SetColor(Color color)
  {
    ((Graphic) this.background).color = color;
    if (this.assignableIdentity == null || !Object.op_Inequality((Object) (this.assignableIdentity as StoredMinionIdentity), (Object) null))
      return;
    ((Component) this).GetComponent<CanvasGroup>().alpha = 0.6f;
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    this.skillsScreen.CurrentlySelectedMinion = this.assignableIdentity;
    ((Component) this).GetComponent<NotificationHighlightTarget>().View();
    KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
  }

  public void Refresh()
  {
    if (Util.IsNullOrDestroyed((object) this.assignableIdentity))
      return;
    this.portrait.SetIdentityObject(this.assignableIdentity);
    MinionIdentity minionIdentity;
    StoredMinionIdentity storedMinionIdentity;
    this.skillsScreen.GetMinionIdentity(this.assignableIdentity, out minionIdentity, out storedMinionIdentity);
    ((Component) this.hatDropDown).gameObject.SetActive(true);
    string hat;
    if (Object.op_Inequality((Object) minionIdentity, (Object) null))
    {
      MinionResume component = ((Component) minionIdentity).GetComponent<MinionResume>();
      int availableSkillpoints = component.AvailableSkillpoints;
      int skillPointsGained = component.TotalSkillPointsGained;
      ((TMP_Text) this.masteryPoints).text = availableSkillpoints > 0 ? GameUtil.ApplyBoldString(GameUtil.ColourizeString(Color32.op_Implicit(new Color(0.5f, 1f, 0.5f, 1f)), availableSkillpoints.ToString())) : "0";
      AttributeInstance attributeInstance1 = Db.Get().Attributes.QualityOfLife.Lookup((Component) component);
      AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) component);
      ((TMP_Text) this.morale).text = string.Format("{0}/{1}", (object) attributeInstance1.GetTotalValue(), (object) attributeInstance2.GetTotalValue());
      this.RefreshToolTip(component);
      List<IListableOption> contentKeys = new List<IListableOption>();
      foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryBySkillID)
      {
        if (keyValuePair.Value)
          contentKeys.Add((IListableOption) new SkillListable(keyValuePair.Key));
      }
      this.hatDropDown.Initialize((IEnumerable<IListableOption>) contentKeys, new Action<IListableOption, object>(this.OnHatDropEntryClick), new Func<IListableOption, IListableOption, object, int>(this.hatDropDownSort), new Action<DropDownEntry, object>(this.hatDropEntryRefreshAction), false, (object) minionIdentity);
      hat = string.IsNullOrEmpty(component.TargetHat) ? component.CurrentHat : component.TargetHat;
    }
    else
    {
      ToolTip component = ((Component) this).GetComponent<ToolTip>();
      component.ClearMultiStringTooltip();
      component.AddMultiStringTooltip(string.Format((string) STRINGS.UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (object) storedMinionIdentity.GetStorageReason(), (object) storedMinionIdentity.GetProperName()), (TextStyleSetting) null);
      hat = string.IsNullOrEmpty(storedMinionIdentity.targetHat) ? storedMinionIdentity.currentHat : storedMinionIdentity.targetHat;
      ((TMP_Text) this.masteryPoints).text = (string) STRINGS.UI.TABLESCREENS.NA;
      ((TMP_Text) this.morale).text = (string) STRINGS.UI.TABLESCREENS.NA;
    }
    bool flag = this.skillsScreen.CurrentlySelectedMinion == this.assignableIdentity;
    if (this.skillsScreen.CurrentlySelectedMinion != null && this.assignableIdentity != null)
      flag = flag || Object.op_Equality((Object) this.skillsScreen.CurrentlySelectedMinion.GetSoleOwner(), (Object) this.assignableIdentity.GetSoleOwner());
    this.SetColor(flag ? this.selected_color : this.unselected_color);
    HierarchyReferences component1 = ((Component) this).GetComponent<HierarchyReferences>();
    this.RefreshHat(hat);
    component1.GetReference("openButton").gameObject.SetActive(Object.op_Inequality((Object) minionIdentity, (Object) null));
  }

  private void RefreshToolTip(MinionResume resume)
  {
    if (!Object.op_Inequality((Object) resume, (Object) null))
      return;
    AttributeInstance attributeInstance1 = Db.Get().Attributes.QualityOfLife.Lookup((Component) resume);
    AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) resume);
    ToolTip component = ((Component) this).GetComponent<ToolTip>();
    component.ClearMultiStringTooltip();
    component.AddMultiStringTooltip(this.assignableIdentity.GetProperName() + "\n\n", this.TooltipTextStyle_Header);
    component.AddMultiStringTooltip(string.Format((string) STRINGS.UI.SKILLS_SCREEN.CURRENT_MORALE, (object) attributeInstance1.GetTotalValue(), (object) attributeInstance2.GetTotalValue()), (TextStyleSetting) null);
    component.AddMultiStringTooltip("\n" + (string) STRINGS.UI.DETAILTABS.STATS.NAME + "\n\n", this.TooltipTextStyle_Header);
    foreach (AttributeInstance attribute in resume.GetAttributes())
    {
      if (attribute.Attribute.ShowInUI == Klei.AI.Attribute.Display.Skill)
      {
        string str = UIConstants.ColorPrefixWhite;
        if ((double) attribute.GetTotalValue() > 0.0)
          str = UIConstants.ColorPrefixGreen;
        else if ((double) attribute.GetTotalValue() < 0.0)
          str = UIConstants.ColorPrefixRed;
        component.AddMultiStringTooltip("    • " + attribute.Name + ": " + str + attribute.GetTotalValue().ToString() + UIConstants.ColorSuffix, (TextStyleSetting) null);
      }
    }
  }

  public void RefreshHat(string hat) => ((Component) this).GetComponent<HierarchyReferences>().GetReference("selectedHat").GetComponent<Image>().sprite = Assets.GetSprite(HashedString.op_Implicit(string.IsNullOrEmpty(hat) ? "hat_role_none" : hat));

  private void OnHatDropEntryClick(IListableOption skill, object data)
  {
    MinionIdentity minionIdentity;
    this.skillsScreen.GetMinionIdentity(this.assignableIdentity, out minionIdentity, out StoredMinionIdentity _);
    if (Object.op_Equality((Object) minionIdentity, (Object) null))
      return;
    MinionResume component = ((Component) minionIdentity).GetComponent<MinionResume>();
    if (skill != null)
    {
      ((Component) this).GetComponent<HierarchyReferences>().GetReference("selectedHat").GetComponent<Image>().sprite = Assets.GetSprite(HashedString.op_Implicit((skill as SkillListable).skillHat));
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        string skillHat = (skill as SkillListable).skillHat;
        component.SetHats(component.CurrentHat, skillHat);
        if (component.OwnsHat(skillHat))
        {
          PutOnHatChore putOnHatChore = new PutOnHatChore((IStateMachineTarget) component, Db.Get().ChoreTypes.SwitchHat);
        }
      }
    }
    else
    {
      ((Component) this).GetComponent<HierarchyReferences>().GetReference("selectedHat").GetComponent<Image>().sprite = Assets.GetSprite(HashedString.op_Implicit("hat_role_none"));
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        component.SetHats(component.CurrentHat, (string) null);
        component.ApplyTargetHat();
      }
    }
    this.skillsScreen.RefreshAll();
  }

  private void hatDropEntryRefreshAction(DropDownEntry entry, object targetData)
  {
    if (entry.entryData == null)
      return;
    SkillListable entryData = entry.entryData as SkillListable;
    ((Image) entry.image).sprite = Assets.GetSprite(HashedString.op_Implicit(entryData.skillHat));
  }

  private int hatDropDownSort(IListableOption a, IListableOption b, object targetData) => 0;
}
