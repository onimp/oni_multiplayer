// Decompiled with JetBrains decompiler
// Type: SkillWidget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[AddComponentMenu("KMonoBehaviour/scripts/SkillWidget")]
public class SkillWidget : 
  KMonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler,
  IPointerClickHandler,
  IPointerDownHandler
{
  [SerializeField]
  private LocText Name;
  [SerializeField]
  private LocText Description;
  [SerializeField]
  private Image TitleBarBG;
  [SerializeField]
  private SkillsScreen skillsScreen;
  [SerializeField]
  private ToolTip tooltip;
  [SerializeField]
  private RectTransform lines_left;
  [SerializeField]
  public RectTransform lines_right;
  [SerializeField]
  private Color header_color_has_skill;
  [SerializeField]
  private Color header_color_can_assign;
  [SerializeField]
  private Color header_color_disabled;
  [SerializeField]
  private Color line_color_default;
  [SerializeField]
  private Color line_color_active;
  [SerializeField]
  private Image hatImage;
  [SerializeField]
  private GameObject borderHighlight;
  [SerializeField]
  private ToolTip masteryCount;
  [SerializeField]
  private GameObject aptitudeBox;
  [SerializeField]
  private GameObject grantedBox;
  [SerializeField]
  private GameObject traitDisabledIcon;
  public TextStyleSetting TooltipTextStyle_Header;
  public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;
  private List<SkillWidget> prerequisiteSkillWidgets = new List<SkillWidget>();
  private UILineRenderer[] lines;
  private List<Vector2> linePoints = new List<Vector2>();
  public Material defaultMaterial;
  public Material desaturatedMaterial;

  public string skillID { get; private set; }

  public void Refresh(string skillID)
  {
    Skill skill = Db.Get().Skills.Get(skillID);
    if (skill == null)
    {
      Debug.LogWarning((object) ("DbSkills is missing skillId " + skillID));
    }
    else
    {
      ((TMP_Text) this.Name).text = skill.Name;
      LocText name = this.Name;
      ((TMP_Text) name).text = ((TMP_Text) name).text + "\n(" + Db.Get().SkillGroups.Get(skill.skillGroup).Name + ")";
      this.skillID = skillID;
      this.tooltip.SetSimpleTooltip(this.SkillTooltip(skill));
      MinionIdentity minionIdentity;
      StoredMinionIdentity storedMinionIdentity1;
      this.skillsScreen.GetMinionIdentity(this.skillsScreen.CurrentlySelectedMinion, out minionIdentity, out storedMinionIdentity1);
      MinionResume minionResume = (MinionResume) null;
      if (Object.op_Inequality((Object) minionIdentity, (Object) null))
      {
        minionResume = ((Component) minionIdentity).GetComponent<MinionResume>();
        MinionResume.SkillMasteryConditions[] masteryConditions = minionResume.GetSkillMasteryConditions(skillID);
        bool flag = minionResume.CanMasterSkill(masteryConditions);
        if ((Object.op_Equality((Object) minionResume, (Object) null) ? 1 : (minionResume.HasMasteredSkill(skillID) ? 0 : (!flag ? 1 : 0))) == 0)
        {
          ((Graphic) this.TitleBarBG).color = minionResume.HasMasteredSkill(skillID) ? this.header_color_has_skill : this.header_color_can_assign;
          ((Graphic) this.hatImage).material = this.defaultMaterial;
        }
        else
        {
          ((Graphic) this.TitleBarBG).color = this.header_color_disabled;
          ((Graphic) this.hatImage).material = this.desaturatedMaterial;
        }
      }
      else if (Object.op_Inequality((Object) storedMinionIdentity1, (Object) null))
      {
        if (storedMinionIdentity1.HasMasteredSkill(skillID))
        {
          ((Graphic) this.TitleBarBG).color = this.header_color_has_skill;
          ((Graphic) this.hatImage).material = this.defaultMaterial;
        }
        else
        {
          ((Graphic) this.TitleBarBG).color = this.header_color_disabled;
          ((Graphic) this.hatImage).material = this.desaturatedMaterial;
        }
      }
      this.hatImage.sprite = Assets.GetSprite(HashedString.op_Implicit(skill.badge));
      bool flag1 = false;
      bool flag2 = false;
      if (Object.op_Inequality((Object) minionResume, (Object) null))
      {
        flag2 = minionResume.HasBeenGrantedSkill(skill);
        float num;
        minionResume.AptitudeBySkillGroup.TryGetValue(HashedString.op_Implicit(skill.skillGroup), out num);
        flag1 = (double) num > 0.0 && !flag2;
      }
      this.aptitudeBox.SetActive(flag1);
      this.grantedBox.SetActive(flag2);
      this.traitDisabledIcon.SetActive(Object.op_Inequality((Object) minionResume, (Object) null) && !minionResume.IsAbleToLearnSkill(skill.Id));
      string str1 = "";
      List<string> stringList = new List<string>();
      foreach (Component component1 in Components.LiveMinionIdentities.Items)
      {
        MinionResume component2 = component1.GetComponent<MinionResume>();
        if (Object.op_Inequality((Object) component2, (Object) null) && component2.HasMasteredSkill(skillID))
          stringList.Add(((Component) component2).GetProperName());
      }
      foreach (MinionStorage minionStorage in Components.MinionStorages.Items)
      {
        foreach (MinionStorage.Info info in minionStorage.GetStoredMinionInfo())
        {
          if (info.serializedMinion != null)
          {
            StoredMinionIdentity storedMinionIdentity2 = info.serializedMinion.Get<StoredMinionIdentity>();
            if (Object.op_Inequality((Object) storedMinionIdentity2, (Object) null) && storedMinionIdentity2.HasMasteredSkill(skillID))
              stringList.Add(storedMinionIdentity2.GetProperName());
          }
        }
      }
      ((Component) this.masteryCount).gameObject.SetActive(stringList.Count > 0);
      foreach (string str2 in stringList)
        str1 = str1 + "\n    • " + str2;
      this.masteryCount.SetSimpleTooltip(stringList.Count > 0 ? string.Format((string) STRINGS.UI.ROLES_SCREEN.WIDGET.NUMBER_OF_MASTERS_TOOLTIP, (object) str1) : STRINGS.UI.ROLES_SCREEN.WIDGET.NO_MASTERS_TOOLTIP.text);
      ((TMP_Text) ((Component) this.masteryCount).GetComponentInChildren<LocText>()).text = stringList.Count.ToString();
    }
  }

  public void RefreshLines()
  {
    this.prerequisiteSkillWidgets.Clear();
    List<Vector2> vector2List = new List<Vector2>();
    foreach (string priorSkill in Db.Get().Skills.Get(this.skillID).priorSkills)
    {
      vector2List.Add(this.skillsScreen.GetSkillWidgetLineTargetPosition(priorSkill));
      this.prerequisiteSkillWidgets.Add(this.skillsScreen.GetSkillWidget(priorSkill));
    }
    if (this.lines != null)
    {
      for (int index = this.lines.Length - 1; index >= 0; --index)
        Object.Destroy((Object) ((Component) this.lines[index]).gameObject);
    }
    this.linePoints.Clear();
    for (int index = 0; index < vector2List.Count; ++index)
    {
      float num1 = (float) ((double) TransformExtensions.GetPosition((Transform) this.lines_left).x - (double) vector2List[index].x - 12.0);
      float num2 = 0.0f;
      this.linePoints.Add(new Vector2(0.0f, num2));
      this.linePoints.Add(new Vector2(-num1, num2));
      this.linePoints.Add(new Vector2(-num1, num2));
      this.linePoints.Add(new Vector2(-num1, (float) -((double) TransformExtensions.GetPosition((Transform) this.lines_left).y - (double) vector2List[index].y)));
      this.linePoints.Add(new Vector2(-num1, (float) -((double) TransformExtensions.GetPosition((Transform) this.lines_left).y - (double) vector2List[index].y)));
      this.linePoints.Add(new Vector2((float) -((double) TransformExtensions.GetPosition((Transform) this.lines_left).x - (double) vector2List[index].x), (float) -((double) TransformExtensions.GetPosition((Transform) this.lines_left).y - (double) vector2List[index].y)));
    }
    this.lines = new UILineRenderer[this.linePoints.Count / 2];
    int index1 = 0;
    for (int index2 = 0; index2 < this.linePoints.Count; index2 += 2)
    {
      GameObject gameObject = new GameObject("Line");
      gameObject.AddComponent<RectTransform>();
      gameObject.transform.SetParent(((Component) this.lines_left).transform);
      TransformExtensions.SetLocalPosition(gameObject.transform, Vector3.zero);
      Util.rectTransform(gameObject).sizeDelta = Vector2.zero;
      this.lines[index1] = gameObject.AddComponent<UILineRenderer>();
      ((Graphic) this.lines[index1]).color = new Color(0.6509804f, 0.6509804f, 0.6509804f, 1f);
      this.lines[index1].Points = new Vector2[2]
      {
        this.linePoints[index2],
        this.linePoints[index2 + 1]
      };
      ++index1;
    }
  }

  public void ToggleBorderHighlight(bool on)
  {
    this.borderHighlight.SetActive(on);
    if (this.lines != null)
    {
      foreach (UILineRenderer line in this.lines)
      {
        ((Graphic) line).color = on ? this.line_color_active : this.line_color_default;
        line.LineThickness = on ? 4f : 2f;
        ((Graphic) line).SetAllDirty();
      }
    }
    for (int index = 0; index < this.prerequisiteSkillWidgets.Count; ++index)
      this.prerequisiteSkillWidgets[index].ToggleBorderHighlight(on);
  }

  public string SkillTooltip(Skill skill) => "" + SkillWidget.SkillPerksString(skill) + "\n" + this.DuplicantSkillString(skill);

  public static string SkillPerksString(Skill skill)
  {
    string str = "";
    foreach (SkillPerk perk in skill.perks)
    {
      if (!string.IsNullOrEmpty(str))
        str += "\n";
      str = str + "• " + perk.Name;
    }
    return str;
  }

  public string CriteriaString(Skill skill)
  {
    bool flag = false;
    string str = "" + "<b>" + (string) STRINGS.UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.TITLE + "</b>\n";
    SkillGroup skillGroup = Db.Get().SkillGroups.Get(skill.skillGroup);
    if (skillGroup != null && skillGroup.relevantAttributes != null)
    {
      foreach (Klei.AI.Attribute relevantAttribute in skillGroup.relevantAttributes)
      {
        if (relevantAttribute != null)
        {
          str = str + "    • " + string.Format((string) STRINGS.UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.SKILLGROUP_ENABLED.DESCRIPTION, (object) relevantAttribute.Name) + "\n";
          flag = true;
        }
      }
    }
    if (skill.priorSkills.Count > 0)
    {
      flag = true;
      for (int index = 0; index < skill.priorSkills.Count; ++index)
      {
        str = str + "    • " + string.Format("{0}", (object) Db.Get().Skills.Get(skill.priorSkills[index]).Name) + "</color>";
        if (index != skill.priorSkills.Count - 1)
          str += "\n";
      }
    }
    if (!flag)
      str = str + "    • " + string.Format((string) STRINGS.UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.NONE, (object) skill.Name);
    return str;
  }

  public string DuplicantSkillString(Skill skill)
  {
    string str = "";
    MinionIdentity minionIdentity;
    this.skillsScreen.GetMinionIdentity(this.skillsScreen.CurrentlySelectedMinion, out minionIdentity, out StoredMinionIdentity _);
    if (Object.op_Inequality((Object) minionIdentity, (Object) null))
    {
      MinionResume component = ((Component) minionIdentity).GetComponent<MinionResume>();
      if (Object.op_Equality((Object) component, (Object) null))
        return "";
      LocString canMaster = STRINGS.UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.CAN_MASTER;
      if (component.HasMasteredSkill(skill.Id))
      {
        if (component.HasBeenGrantedSkill(skill))
          str = str + "\n" + string.Format((string) STRINGS.UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.SKILL_GRANTED, (object) minionIdentity.GetProperName(), (object) skill.Name);
      }
      else
      {
        MinionResume.SkillMasteryConditions[] masteryConditions = component.GetSkillMasteryConditions(skill.Id);
        if (!component.CanMasterSkill(masteryConditions))
        {
          bool flag = false;
          str = str + "\n" + string.Format((string) STRINGS.UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.CANNOT_MASTER, (object) minionIdentity.GetProperName(), (object) skill.Name);
          if (Array.Exists<MinionResume.SkillMasteryConditions>(masteryConditions, (Predicate<MinionResume.SkillMasteryConditions>) (element => element == MinionResume.SkillMasteryConditions.UnableToLearn)))
          {
            flag = true;
            string choreGroupId = Db.Get().SkillGroups.Get(skill.skillGroup).choreGroupID;
            Trait disablingTrait;
            ((Component) minionIdentity).GetComponent<Traits>().IsChoreGroupDisabled(HashedString.op_Implicit(choreGroupId), out disablingTrait);
            str = str + "\n" + string.Format((string) STRINGS.UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.PREVENTED_BY_TRAIT, (object) disablingTrait.Name);
          }
          if (!flag && Array.Exists<MinionResume.SkillMasteryConditions>(masteryConditions, (Predicate<MinionResume.SkillMasteryConditions>) (element => element == MinionResume.SkillMasteryConditions.MissingPreviousSkill)))
            str = str + "\n" + string.Format((string) STRINGS.UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.REQUIRES_PREVIOUS_SKILLS);
          if (!flag && Array.Exists<MinionResume.SkillMasteryConditions>(masteryConditions, (Predicate<MinionResume.SkillMasteryConditions>) (element => element == MinionResume.SkillMasteryConditions.NeedsSkillPoints)))
            str = str + "\n" + string.Format((string) STRINGS.UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.REQUIRES_MORE_SKILL_POINTS);
        }
        else
        {
          if (Array.Exists<MinionResume.SkillMasteryConditions>(masteryConditions, (Predicate<MinionResume.SkillMasteryConditions>) (element => element == MinionResume.SkillMasteryConditions.StressWarning)))
            str = str + "\n" + string.Format((string) STRINGS.UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.STRESS_WARNING_MESSAGE, (object) skill.Name, (object) minionIdentity.GetProperName());
          if (Array.Exists<MinionResume.SkillMasteryConditions>(masteryConditions, (Predicate<MinionResume.SkillMasteryConditions>) (element => element == MinionResume.SkillMasteryConditions.SkillAptitude)))
            str = str + "\n" + string.Format((string) STRINGS.UI.SKILLS_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERY.SKILL_APTITUDE, (object) minionIdentity.GetProperName(), (object) skill.Name);
        }
      }
    }
    return str;
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    this.ToggleBorderHighlight(true);
    this.skillsScreen.HoverSkill(this.skillID);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    this.ToggleBorderHighlight(false);
    this.skillsScreen.HoverSkill((string) null);
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    MinionIdentity minionIdentity;
    this.skillsScreen.GetMinionIdentity(this.skillsScreen.CurrentlySelectedMinion, out minionIdentity, out StoredMinionIdentity _);
    if (!Object.op_Inequality((Object) minionIdentity, (Object) null))
      return;
    MinionResume component = ((Component) minionIdentity).GetComponent<MinionResume>();
    if (DebugHandler.InstantBuildMode && component.AvailableSkillpoints < 1)
      component.ForceAddSkillPoint();
    MinionResume.SkillMasteryConditions[] masteryConditions = component.GetSkillMasteryConditions(this.skillID);
    bool flag = component.CanMasterSkill(masteryConditions);
    if (((!Object.op_Inequality((Object) component, (Object) null) ? 0 : (!component.HasMasteredSkill(this.skillID) ? 1 : 0)) & (flag ? 1 : 0)) == 0)
      return;
    component.MasterSkill(this.skillID);
    this.skillsScreen.RefreshAll();
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    MinionIdentity minionIdentity;
    this.skillsScreen.GetMinionIdentity(this.skillsScreen.CurrentlySelectedMinion, out minionIdentity, out StoredMinionIdentity _);
    MinionResume minionResume = (MinionResume) null;
    bool flag = false;
    if (Object.op_Inequality((Object) minionIdentity, (Object) null))
    {
      minionResume = ((Component) minionIdentity).GetComponent<MinionResume>();
      MinionResume.SkillMasteryConditions[] masteryConditions = minionResume.GetSkillMasteryConditions(this.skillID);
      flag = minionResume.CanMasterSkill(masteryConditions);
    }
    if (((!Object.op_Inequality((Object) minionResume, (Object) null) ? 0 : (!minionResume.HasMasteredSkill(this.skillID) ? 1 : 0)) & (flag ? 1 : 0)) != 0)
      KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
    else
      KFMOD.PlayUISound(GlobalAssets.GetSound("Negative"));
  }
}
