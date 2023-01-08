// Decompiled with JetBrains decompiler
// Type: SkillsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsScreen : KModalScreen
{
  [SerializeField]
  private KButton CloseButton;
  [Header("Prefabs")]
  [SerializeField]
  private GameObject Prefab_skillWidget;
  [SerializeField]
  private GameObject Prefab_skillColumn;
  [SerializeField]
  private GameObject Prefab_minion;
  [SerializeField]
  private GameObject Prefab_minionLayout;
  [SerializeField]
  private GameObject Prefab_tableLayout;
  [SerializeField]
  private GameObject Prefab_worldDivider;
  [Header("Sort Toggles")]
  [SerializeField]
  private MultiToggle dupeSortingToggle;
  [SerializeField]
  private MultiToggle experienceSortingToggle;
  [SerializeField]
  private MultiToggle moraleSortingToggle;
  private MultiToggle activeSortToggle;
  private bool sortReversed;
  private Comparison<IAssignableIdentity> active_sort_method;
  [Header("Duplicant Animation")]
  [SerializeField]
  private FullBodyUIMinionWidget minionAnimWidget;
  [Header("Progress Bars")]
  [SerializeField]
  private ToolTip expectationsTooltip;
  [SerializeField]
  private LocText moraleProgressLabel;
  [SerializeField]
  private GameObject moraleWarning;
  [SerializeField]
  private GameObject moraleNotch;
  [SerializeField]
  private Color moraleNotchColor;
  private List<GameObject> moraleNotches = new List<GameObject>();
  [SerializeField]
  private LocText expectationsProgressLabel;
  [SerializeField]
  private GameObject expectationWarning;
  [SerializeField]
  private GameObject expectationNotch;
  [SerializeField]
  private Color expectationNotchColor;
  [SerializeField]
  private Color expectationNotchProspectColor;
  private List<GameObject> expectationNotches = new List<GameObject>();
  [SerializeField]
  private ToolTip experienceBarTooltip;
  [SerializeField]
  private Image experienceProgressFill;
  [SerializeField]
  private LocText EXPCount;
  [SerializeField]
  private LocText duplicantLevelIndicator;
  [SerializeField]
  private KScrollRect scrollRect;
  [SerializeField]
  private float scrollSpeed = 7f;
  [SerializeField]
  private DropDown hatDropDown;
  [SerializeField]
  public Image selectedHat;
  private IAssignableIdentity currentlySelectedMinion;
  private List<GameObject> rows = new List<GameObject>();
  private List<SkillMinionWidget> sortableRows = new List<SkillMinionWidget>();
  private Dictionary<int, GameObject> worldDividers = new Dictionary<int, GameObject>();
  private string hoveredSkillID = "";
  private Dictionary<string, GameObject> skillWidgets = new Dictionary<string, GameObject>();
  private Dictionary<string, int> skillGroupRow = new Dictionary<string, int>();
  private List<GameObject> skillColumns = new List<GameObject>();
  private bool dirty;
  private bool linesPending;
  private int layoutRowHeight = 80;
  private Coroutine delayRefreshRoutine;
  protected Comparison<IAssignableIdentity> compareByExperience = (Comparison<IAssignableIdentity>) ((a, b) =>
  {
    GameObject targetGameObject1 = ((MinionAssignablesProxy) a).GetTargetGameObject();
    GameObject targetGameObject2 = ((MinionAssignablesProxy) b).GetTargetGameObject();
    if (Object.op_Equality((Object) targetGameObject1, (Object) null) && Object.op_Equality((Object) targetGameObject2, (Object) null))
      return 0;
    if (Object.op_Equality((Object) targetGameObject1, (Object) null))
      return -1;
    if (Object.op_Equality((Object) targetGameObject2, (Object) null))
      return 1;
    MinionResume component1 = targetGameObject1.GetComponent<MinionResume>();
    MinionResume component2 = targetGameObject2.GetComponent<MinionResume>();
    if (Object.op_Equality((Object) component1, (Object) null) && Object.op_Equality((Object) component2, (Object) null))
      return 0;
    if (Object.op_Equality((Object) component1, (Object) null))
      return -1;
    return Object.op_Equality((Object) component2, (Object) null) ? 1 : ((float) component1.AvailableSkillpoints).CompareTo((float) component2.AvailableSkillpoints);
  });
  protected Comparison<IAssignableIdentity> compareByMinion = (Comparison<IAssignableIdentity>) ((a, b) => a.GetProperName().CompareTo(b.GetProperName()));
  protected Comparison<IAssignableIdentity> compareByMorale = (Comparison<IAssignableIdentity>) ((a, b) =>
  {
    GameObject targetGameObject3 = ((MinionAssignablesProxy) a).GetTargetGameObject();
    GameObject targetGameObject4 = ((MinionAssignablesProxy) b).GetTargetGameObject();
    if (Object.op_Equality((Object) targetGameObject3, (Object) null) && Object.op_Equality((Object) targetGameObject4, (Object) null))
      return 0;
    if (Object.op_Equality((Object) targetGameObject3, (Object) null))
      return -1;
    if (Object.op_Equality((Object) targetGameObject4, (Object) null))
      return 1;
    MinionResume component3 = targetGameObject3.GetComponent<MinionResume>();
    MinionResume component4 = targetGameObject4.GetComponent<MinionResume>();
    if (Object.op_Equality((Object) component3, (Object) null) && Object.op_Equality((Object) component4, (Object) null))
      return 0;
    if (Object.op_Equality((Object) component3, (Object) null))
      return -1;
    if (Object.op_Equality((Object) component4, (Object) null))
      return 1;
    AttributeInstance attributeInstance1 = Db.Get().Attributes.QualityOfLife.Lookup((Component) component3);
    Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) component3);
    AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLife.Lookup((Component) component4);
    Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) component4);
    return attributeInstance1.GetTotalValue().CompareTo(attributeInstance2.GetTotalValue());
  });

  public override float GetSortKey() => this.isEditing ? 50f : 20f;

  public IAssignableIdentity CurrentlySelectedMinion
  {
    get => this.currentlySelectedMinion == null || this.currentlySelectedMinion.IsNull() ? (IAssignableIdentity) null : this.currentlySelectedMinion;
    set
    {
      this.currentlySelectedMinion = value;
      if (!this.IsActive())
        return;
      this.RefreshSelectedMinion();
      this.RefreshSkillWidgets();
    }
  }

  protected virtual void OnSpawn() => ClusterManager.Instance.Subscribe(-1078710002, new Action<object>(this.WorldRemoved));

  protected override void OnActivate()
  {
    this.ConsumeMouseScroll = true;
    base.OnActivate();
    this.BuildMinions();
    this.RefreshAll();
    this.SortRows(this.active_sort_method == null ? this.compareByMinion : this.active_sort_method);
    Components.LiveMinionIdentities.OnAdd += new Action<MinionIdentity>(this.OnAddMinionIdentity);
    Components.LiveMinionIdentities.OnRemove += new Action<MinionIdentity>(this.OnRemoveMinionIdentity);
    this.CloseButton.onClick += (System.Action) (() => ManagementMenu.Instance.CloseAll());
    this.dupeSortingToggle.onClick += (System.Action) (() => this.SortRows(this.compareByMinion));
    this.moraleSortingToggle.onClick += (System.Action) (() => this.SortRows(this.compareByMorale));
    this.experienceSortingToggle.onClick += (System.Action) (() => this.SortRows(this.compareByExperience));
  }

  protected override void OnShow(bool show)
  {
    if (show)
    {
      if (this.CurrentlySelectedMinion == null && Components.LiveMinionIdentities.Count > 0)
        this.CurrentlySelectedMinion = (IAssignableIdentity) Components.LiveMinionIdentities.Items[0];
      this.BuildMinions();
      this.RefreshAll();
      this.SortRows(this.active_sort_method == null ? this.compareByMinion : this.active_sort_method);
    }
    base.OnShow(show);
  }

  public void RefreshAll()
  {
    this.dirty = false;
    this.RefreshSkillWidgets();
    this.RefreshSelectedMinion();
    this.linesPending = true;
  }

  private void RefreshSelectedMinion()
  {
    this.minionAnimWidget.SetPortraitAnimator(this.currentlySelectedMinion);
    this.RefreshProgressBars();
    this.RefreshHat();
  }

  public void GetMinionIdentity(
    IAssignableIdentity assignableIdentity,
    out MinionIdentity minionIdentity,
    out StoredMinionIdentity storedMinionIdentity)
  {
    if (assignableIdentity is MinionAssignablesProxy)
    {
      minionIdentity = ((MinionAssignablesProxy) assignableIdentity).GetTargetGameObject().GetComponent<MinionIdentity>();
      storedMinionIdentity = ((MinionAssignablesProxy) assignableIdentity).GetTargetGameObject().GetComponent<StoredMinionIdentity>();
    }
    else
    {
      minionIdentity = assignableIdentity as MinionIdentity;
      storedMinionIdentity = assignableIdentity as StoredMinionIdentity;
    }
  }

  private void RefreshProgressBars()
  {
    if (this.currentlySelectedMinion == null || this.currentlySelectedMinion.IsNull())
      return;
    MinionIdentity minionIdentity;
    StoredMinionIdentity storedMinionIdentity;
    this.GetMinionIdentity(this.currentlySelectedMinion, out minionIdentity, out storedMinionIdentity);
    HierarchyReferences component1 = ((Component) this.expectationsTooltip).GetComponent<HierarchyReferences>();
    component1.GetReference("Labels").gameObject.SetActive(Object.op_Inequality((Object) minionIdentity, (Object) null));
    component1.GetReference("MoraleBar").gameObject.SetActive(Object.op_Inequality((Object) minionIdentity, (Object) null));
    component1.GetReference("ExpectationBar").gameObject.SetActive(Object.op_Inequality((Object) minionIdentity, (Object) null));
    component1.GetReference("StoredMinion").gameObject.SetActive(Object.op_Equality((Object) minionIdentity, (Object) null));
    ((Component) this.experienceProgressFill).gameObject.SetActive(Object.op_Inequality((Object) minionIdentity, (Object) null));
    if (Object.op_Equality((Object) minionIdentity, (Object) null))
    {
      this.expectationsTooltip.SetSimpleTooltip(string.Format((string) STRINGS.UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (object) storedMinionIdentity.GetStorageReason(), (object) this.currentlySelectedMinion.GetProperName()));
      this.experienceBarTooltip.SetSimpleTooltip(string.Format((string) STRINGS.UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (object) storedMinionIdentity.GetStorageReason(), (object) this.currentlySelectedMinion.GetProperName()));
      ((TMP_Text) this.EXPCount).text = "";
      ((TMP_Text) this.duplicantLevelIndicator).text = (string) STRINGS.UI.TABLESCREENS.NA;
    }
    else
    {
      MinionResume component2 = ((Component) minionIdentity).GetComponent<MinionResume>();
      float previousExperienceBar = MinionResume.CalculatePreviousExperienceBar(component2.TotalSkillPointsGained);
      float nextExperienceBar = MinionResume.CalculateNextExperienceBar(component2.TotalSkillPointsGained);
      float num1 = (float) (((double) component2.TotalExperienceGained - (double) previousExperienceBar) / ((double) nextExperienceBar - (double) previousExperienceBar));
      ((TMP_Text) this.EXPCount).text = Mathf.RoundToInt(component2.TotalExperienceGained - previousExperienceBar).ToString() + " / " + Mathf.RoundToInt(nextExperienceBar - previousExperienceBar).ToString();
      ((TMP_Text) this.duplicantLevelIndicator).text = component2.AvailableSkillpoints.ToString();
      this.experienceProgressFill.fillAmount = num1;
      this.experienceBarTooltip.SetSimpleTooltip(string.Format((string) STRINGS.UI.SKILLS_SCREEN.EXPERIENCE_TOOLTIP, (object) (Mathf.RoundToInt(nextExperienceBar - previousExperienceBar) - Mathf.RoundToInt(component2.TotalExperienceGained - previousExperienceBar))));
      AttributeInstance attributeInstance1 = Db.Get().Attributes.QualityOfLife.Lookup((Component) component2);
      AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) component2);
      float num2 = 0.0f;
      float num3 = 0.0f;
      if (!string.IsNullOrEmpty(this.hoveredSkillID) && !component2.HasMasteredSkill(this.hoveredSkillID))
      {
        List<string> stringList = new List<string>();
        List<string> collection = new List<string>();
        stringList.Add(this.hoveredSkillID);
        while (stringList.Count > 0)
        {
          for (int index = stringList.Count - 1; index >= 0; --index)
          {
            if (!component2.HasMasteredSkill(stringList[index]))
            {
              num2 += (float) (Db.Get().Skills.Get(stringList[index]).tier + 1);
              if (component2.AptitudeBySkillGroup.ContainsKey(HashedString.op_Implicit(Db.Get().Skills.Get(stringList[index]).skillGroup)) && (double) component2.AptitudeBySkillGroup[HashedString.op_Implicit(Db.Get().Skills.Get(stringList[index]).skillGroup)] > 0.0)
                ++num3;
              foreach (string priorSkill in Db.Get().Skills.Get(stringList[index]).priorSkills)
                collection.Add(priorSkill);
            }
          }
          stringList.Clear();
          stringList.AddRange((IEnumerable<string>) collection);
          collection.Clear();
        }
      }
      float num4 = attributeInstance1.GetTotalValue() + num3 / (attributeInstance2.GetTotalValue() + num2);
      float num5 = Mathf.Max(attributeInstance1.GetTotalValue() + num3, attributeInstance2.GetTotalValue() + num2);
      while (this.moraleNotches.Count < Mathf.RoundToInt(num5))
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.moraleNotch, this.moraleNotch.transform.parent);
        gameObject.SetActive(true);
        this.moraleNotches.Add(gameObject);
      }
      while (this.moraleNotches.Count > Mathf.RoundToInt(num5))
      {
        GameObject moraleNotch = this.moraleNotches[this.moraleNotches.Count - 1];
        this.moraleNotches.Remove(moraleNotch);
        Object.Destroy((Object) moraleNotch);
      }
      for (int index = 0; index < this.moraleNotches.Count; ++index)
      {
        if ((double) index < (double) attributeInstance1.GetTotalValue() + (double) num3)
          ((Graphic) this.moraleNotches[index].GetComponentsInChildren<Image>()[1]).color = this.moraleNotchColor;
        else
          ((Graphic) this.moraleNotches[index].GetComponentsInChildren<Image>()[1]).color = Color.clear;
      }
      ((TMP_Text) this.moraleProgressLabel).text = (string) STRINGS.UI.SKILLS_SCREEN.MORALE + ": " + attributeInstance1.GetTotalValue().ToString();
      if ((double) num3 > 0.0)
      {
        LocText moraleProgressLabel = this.moraleProgressLabel;
        ((TMP_Text) moraleProgressLabel).text = ((TMP_Text) moraleProgressLabel).text + " + " + GameUtil.ApplyBoldString(GameUtil.ColourizeString(Color32.op_Implicit(this.moraleNotchColor), num3.ToString()));
      }
      while (this.expectationNotches.Count < Mathf.RoundToInt(num5))
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.expectationNotch, this.expectationNotch.transform.parent);
        gameObject.SetActive(true);
        this.expectationNotches.Add(gameObject);
      }
      while (this.expectationNotches.Count > Mathf.RoundToInt(num5))
      {
        GameObject expectationNotch = this.expectationNotches[this.expectationNotches.Count - 1];
        this.expectationNotches.Remove(expectationNotch);
        Object.Destroy((Object) expectationNotch);
      }
      for (int index = 0; index < this.expectationNotches.Count; ++index)
      {
        if ((double) index < (double) attributeInstance2.GetTotalValue() + (double) num2)
        {
          if ((double) index < (double) attributeInstance2.GetTotalValue())
            ((Graphic) this.expectationNotches[index].GetComponentsInChildren<Image>()[1]).color = this.expectationNotchColor;
          else
            ((Graphic) this.expectationNotches[index].GetComponentsInChildren<Image>()[1]).color = this.expectationNotchProspectColor;
        }
        else
          ((Graphic) this.expectationNotches[index].GetComponentsInChildren<Image>()[1]).color = Color.clear;
      }
      ((TMP_Text) this.expectationsProgressLabel).text = (string) STRINGS.UI.SKILLS_SCREEN.MORALE_EXPECTATION + ": " + attributeInstance2.GetTotalValue().ToString();
      if ((double) num2 > 0.0)
      {
        LocText expectationsProgressLabel = this.expectationsProgressLabel;
        ((TMP_Text) expectationsProgressLabel).text = ((TMP_Text) expectationsProgressLabel).text + " + " + GameUtil.ApplyBoldString(GameUtil.ColourizeString(Color32.op_Implicit(this.expectationNotchColor), num2.ToString()));
      }
      if ((double) num4 < 1.0)
      {
        this.expectationWarning.SetActive(true);
        this.moraleWarning.SetActive(false);
      }
      else
      {
        this.expectationWarning.SetActive(false);
        this.moraleWarning.SetActive(true);
      }
      string str1 = "";
      Dictionary<string, float> source = new Dictionary<string, float>();
      string str2 = str1 + GameUtil.ApplyBoldString((string) STRINGS.UI.SKILLS_SCREEN.MORALE) + ": " + attributeInstance1.GetTotalValue().ToString() + "\n";
      for (int index = 0; index < attributeInstance1.Modifiers.Count; ++index)
        source.Add(attributeInstance1.Modifiers[index].GetDescription(), attributeInstance1.Modifiers[index].Value);
      List<KeyValuePair<string, float>> list = source.ToList<KeyValuePair<string, float>>();
      list.Sort((Comparison<KeyValuePair<string, float>>) ((pair1, pair2) => pair2.Value.CompareTo(pair1.Value)));
      foreach (KeyValuePair<string, float> keyValuePair in list)
        str2 = str2 + "    • " + keyValuePair.Key + ": " + ((double) keyValuePair.Value > 0.0 ? UIConstants.ColorPrefixGreen : UIConstants.ColorPrefixRed) + keyValuePair.Value.ToString() + UIConstants.ColorSuffix + "\n";
      string str3 = str2 + "\n" + GameUtil.ApplyBoldString((string) STRINGS.UI.SKILLS_SCREEN.MORALE_EXPECTATION) + ": " + attributeInstance2.GetTotalValue().ToString() + "\n";
      for (int index = 0; index < attributeInstance2.Modifiers.Count; ++index)
        str3 = str3 + "    • " + attributeInstance2.Modifiers[index].GetDescription() + ": " + ((double) attributeInstance2.Modifiers[index].Value > 0.0 ? UIConstants.ColorPrefixRed : UIConstants.ColorPrefixGreen) + attributeInstance2.Modifiers[index].GetFormattedString() + UIConstants.ColorSuffix + "\n";
      this.expectationsTooltip.SetSimpleTooltip(str3);
    }
  }

  private void RefreshHat()
  {
    if (this.currentlySelectedMinion == null || this.currentlySelectedMinion.IsNull())
      return;
    List<IListableOption> contentKeys = new List<IListableOption>();
    MinionIdentity minionIdentity;
    StoredMinionIdentity storedMinionIdentity;
    this.GetMinionIdentity(this.currentlySelectedMinion, out minionIdentity, out storedMinionIdentity);
    string str;
    if (Object.op_Inequality((Object) minionIdentity, (Object) null))
    {
      MinionResume component = ((Component) minionIdentity).GetComponent<MinionResume>();
      str = string.IsNullOrEmpty(component.TargetHat) ? component.CurrentHat : component.TargetHat;
      foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryBySkillID)
      {
        if (keyValuePair.Value)
          contentKeys.Add((IListableOption) new SkillListable(keyValuePair.Key));
      }
      this.hatDropDown.Initialize((IEnumerable<IListableOption>) contentKeys, new Action<IListableOption, object>(this.OnHatDropEntryClick), new Func<IListableOption, IListableOption, object, int>(this.hatDropDownSort), new Action<DropDownEntry, object>(this.hatDropEntryRefreshAction), false, (object) this.currentlySelectedMinion);
    }
    else
      str = string.IsNullOrEmpty(storedMinionIdentity.targetHat) ? storedMinionIdentity.currentHat : storedMinionIdentity.targetHat;
    ((Behaviour) this.hatDropDown.openButton).enabled = Object.op_Inequality((Object) minionIdentity, (Object) null);
    ((Component) ((Component) this.selectedHat).transform.Find("Arrow")).gameObject.SetActive(Object.op_Inequality((Object) minionIdentity, (Object) null));
    this.selectedHat.sprite = Assets.GetSprite(HashedString.op_Implicit(string.IsNullOrEmpty(str) ? "hat_role_none" : str));
  }

  private void OnHatDropEntryClick(IListableOption skill, object data)
  {
    MinionIdentity minionIdentity;
    this.GetMinionIdentity(this.currentlySelectedMinion, out minionIdentity, out StoredMinionIdentity _);
    if (Object.op_Equality((Object) minionIdentity, (Object) null))
      return;
    MinionResume component = ((Component) minionIdentity).GetComponent<MinionResume>();
    string str = "hat_role_none";
    if (skill != null)
    {
      this.selectedHat.sprite = Assets.GetSprite(HashedString.op_Implicit((skill as SkillListable).skillHat));
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
      this.selectedHat.sprite = Assets.GetSprite(HashedString.op_Implicit(str));
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        component.SetHats(component.CurrentHat, (string) null);
        component.ApplyTargetHat();
      }
    }
    IAssignableIdentity assignableIdentity = (IAssignableIdentity) minionIdentity.assignableProxy.Get();
    foreach (SkillMinionWidget sortableRow in this.sortableRows)
    {
      if (sortableRow.assignableIdentity == assignableIdentity)
        sortableRow.RefreshHat(component.TargetHat);
    }
  }

  private void hatDropEntryRefreshAction(DropDownEntry entry, object targetData)
  {
    if (entry.entryData == null)
      return;
    SkillListable entryData = entry.entryData as SkillListable;
    ((Image) entry.image).sprite = Assets.GetSprite(HashedString.op_Implicit(entryData.skillHat));
  }

  private int hatDropDownSort(IListableOption a, IListableOption b, object targetData) => 0;

  private void Update()
  {
    if (this.dirty)
      this.RefreshAll();
    if (this.linesPending)
    {
      foreach (GameObject gameObject in this.skillWidgets.Values)
        gameObject.GetComponent<SkillWidget>().RefreshLines();
      this.linesPending = false;
    }
    if (!KInputManager.currentControllerIsGamepad)
      return;
    this.scrollRect.AnalogUpdate(Vector2.op_Multiply(KInputManager.steamInputInterpreter.GetSteamCameraMovement(), this.scrollSpeed));
  }

  private void RefreshSkillWidgets()
  {
    int num1 = 1;
    foreach (SkillGroup resource in Db.Get().SkillGroups.resources)
    {
      List<Skill> skillsBySkillGroup = this.GetSkillsBySkillGroup(resource.Id);
      if (skillsBySkillGroup.Count > 0)
      {
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        for (int index = 0; index < skillsBySkillGroup.Count; ++index)
        {
          Skill skill = skillsBySkillGroup[index];
          if (!skill.deprecated)
          {
            if (!this.skillWidgets.ContainsKey(skill.Id))
            {
              while (skill.tier >= this.skillColumns.Count)
              {
                GameObject gameObject = Util.KInstantiateUI(this.Prefab_skillColumn, this.Prefab_tableLayout, true);
                this.skillColumns.Add(gameObject);
                HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
                if (this.skillColumns.Count % 2 == 0)
                  component.GetReference("BG").gameObject.SetActive(false);
              }
              int num2 = 0;
              dictionary.TryGetValue(skill.tier, out num2);
              dictionary[skill.tier] = num2 + 1;
              GameObject gameObject1 = Util.KInstantiateUI(this.Prefab_skillWidget, this.skillColumns[skill.tier], true);
              this.skillWidgets.Add(skill.Id, gameObject1);
            }
            this.skillWidgets[skill.Id].GetComponent<SkillWidget>().Refresh(skill.Id);
          }
        }
        if (!this.skillGroupRow.ContainsKey(resource.Id))
        {
          int num3 = 1;
          foreach (KeyValuePair<int, int> keyValuePair in dictionary)
            num3 = Mathf.Max(num3, keyValuePair.Value);
          this.skillGroupRow.Add(resource.Id, num1);
          num1 += num3;
        }
      }
    }
    foreach (SkillMinionWidget sortableRow in this.sortableRows)
      sortableRow.Refresh();
    this.RefreshWidgetPositions();
  }

  public void HoverSkill(string skillID)
  {
    this.hoveredSkillID = skillID;
    if (this.delayRefreshRoutine != null)
    {
      ((MonoBehaviour) this).StopCoroutine(this.delayRefreshRoutine);
      this.delayRefreshRoutine = (Coroutine) null;
    }
    if (string.IsNullOrEmpty(this.hoveredSkillID))
      this.delayRefreshRoutine = ((MonoBehaviour) this).StartCoroutine(this.DelayRefreshProgressBars());
    else
      this.RefreshProgressBars();
  }

  private IEnumerator DelayRefreshProgressBars()
  {
    yield return (object) SequenceUtil.WaitForSecondsRealtime(0.1f);
    this.RefreshProgressBars();
  }

  public void RefreshWidgetPositions()
  {
    float num1 = 0.0f;
    foreach (KeyValuePair<string, GameObject> skillWidget in this.skillWidgets)
    {
      float rowPosition = this.GetRowPosition(skillWidget.Key);
      num1 = Mathf.Max(rowPosition, num1);
      Util.rectTransform(skillWidget.Value).anchoredPosition = Vector2.op_Multiply(Vector2.down, rowPosition);
    }
    float num2 = Mathf.Max(num1, (float) this.layoutRowHeight);
    float layoutRowHeight = (float) this.layoutRowHeight;
    foreach (GameObject skillColumn in this.skillColumns)
      skillColumn.GetComponent<LayoutElement>().minHeight = num2 + layoutRowHeight;
    this.linesPending = true;
  }

  public float GetRowPosition(string skillID)
  {
    Skill skill1 = Db.Get().Skills.Get(skillID);
    int num1 = this.skillGroupRow[skill1.skillGroup];
    List<Skill> skillsBySkillGroup = this.GetSkillsBySkillGroup(skill1.skillGroup);
    int num2 = 0;
    foreach (Skill skill2 in skillsBySkillGroup)
    {
      if (skill2 != skill1)
      {
        if (skill2.tier == skill1.tier)
          ++num2;
      }
      else
        break;
    }
    return (float) (this.layoutRowHeight * (num2 + num1 - 1));
  }

  private void OnAddMinionIdentity(MinionIdentity add)
  {
    this.BuildMinions();
    this.RefreshAll();
  }

  private void OnRemoveMinionIdentity(MinionIdentity remove)
  {
    if (this.CurrentlySelectedMinion == remove)
      this.CurrentlySelectedMinion = (IAssignableIdentity) null;
    this.BuildMinions();
    this.RefreshAll();
  }

  private void BuildMinions()
  {
    for (int index = this.sortableRows.Count - 1; index >= 0; --index)
      TracesExtesions.DeleteObject((Component) this.sortableRows[index]);
    this.sortableRows.Clear();
    foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
    {
      GameObject gameObject = Util.KInstantiateUI(this.Prefab_minion, this.Prefab_minionLayout, true);
      gameObject.GetComponent<SkillMinionWidget>().SetMinon((IAssignableIdentity) minionIdentity.assignableProxy.Get());
      this.sortableRows.Add(gameObject.GetComponent<SkillMinionWidget>());
    }
    foreach (MinionStorage minionStorage in Components.MinionStorages.Items)
    {
      foreach (MinionStorage.Info info in minionStorage.GetStoredMinionInfo())
      {
        if (info.serializedMinion != null)
        {
          StoredMinionIdentity storedMinionIdentity = info.serializedMinion.Get<StoredMinionIdentity>();
          GameObject gameObject = Util.KInstantiateUI(this.Prefab_minion, this.Prefab_minionLayout, true);
          gameObject.GetComponent<SkillMinionWidget>().SetMinon((IAssignableIdentity) storedMinionIdentity.assignableProxy.Get());
          this.sortableRows.Add(gameObject.GetComponent<SkillMinionWidget>());
        }
      }
    }
    foreach (int num in ClusterManager.Instance.GetWorldIDsSorted())
    {
      if (ClusterManager.Instance.GetWorld(num).IsDiscovered)
        this.AddWorldDivider(num);
    }
    foreach (KeyValuePair<int, GameObject> worldDivider in this.worldDividers)
    {
      worldDivider.Value.SetActive(ClusterManager.Instance.GetWorld(worldDivider.Key).IsDiscovered && DlcManager.FeatureClusterSpaceEnabled());
      Component reference = worldDivider.Value.GetComponent<HierarchyReferences>().GetReference("NobodyRow");
      reference.gameObject.SetActive(true);
      foreach (MinionAssignablesProxy assignablesProxy in Components.MinionAssignablesProxy)
      {
        if (assignablesProxy.GetTargetGameObject().GetComponent<KMonoBehaviour>().GetMyWorld().id == worldDivider.Key)
        {
          reference.gameObject.SetActive(false);
          break;
        }
      }
    }
    if (this.CurrentlySelectedMinion != null || Components.LiveMinionIdentities.Count <= 0)
      return;
    this.CurrentlySelectedMinion = (IAssignableIdentity) Components.LiveMinionIdentities.Items[0];
  }

  protected void AddWorldDivider(int worldId)
  {
    if (this.worldDividers.ContainsKey(worldId))
      return;
    GameObject gameObject = Util.KInstantiateUI(this.Prefab_worldDivider, this.Prefab_minionLayout, true);
    ((Graphic) gameObject.GetComponentInChildren<Image>()).color = ClusterManager.worldColors[worldId % ClusterManager.worldColors.Length];
    ClusterGridEntity component = ((Component) ClusterManager.Instance.GetWorld(worldId)).GetComponent<ClusterGridEntity>();
    ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).SetText(component.Name);
    gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = component.GetUISprite();
    this.worldDividers.Add(worldId, gameObject);
  }

  private void WorldRemoved(object worldId)
  {
    int key = (int) worldId;
    GameObject gameObject;
    if (!this.worldDividers.TryGetValue(key, out gameObject))
      return;
    Object.Destroy((Object) gameObject);
    this.worldDividers.Remove(key);
  }

  public Vector2 GetSkillWidgetLineTargetPosition(string skillID) => Vector2.op_Implicit(TransformExtensions.GetPosition((Transform) this.skillWidgets[skillID].GetComponent<SkillWidget>().lines_right));

  public SkillWidget GetSkillWidget(string skill) => this.skillWidgets[skill].GetComponent<SkillWidget>();

  public List<Skill> GetSkillsBySkillGroup(string skillGrp)
  {
    List<Skill> skillsBySkillGroup = new List<Skill>();
    foreach (Skill resource in Db.Get().Skills.resources)
    {
      if (resource.skillGroup == skillGrp && !resource.deprecated)
        skillsBySkillGroup.Add(resource);
    }
    return skillsBySkillGroup;
  }

  private void SelectSortToggle(MultiToggle toggle)
  {
    this.dupeSortingToggle.ChangeState(0);
    this.experienceSortingToggle.ChangeState(0);
    this.moraleSortingToggle.ChangeState(0);
    if (Object.op_Inequality((Object) toggle, (Object) null))
    {
      if (Object.op_Equality((Object) this.activeSortToggle, (Object) toggle))
        this.sortReversed = !this.sortReversed;
      this.activeSortToggle = toggle;
    }
    this.activeSortToggle.ChangeState(this.sortReversed ? 2 : 1);
  }

  private void SortRows(Comparison<IAssignableIdentity> comparison)
  {
    this.active_sort_method = comparison;
    Dictionary<IAssignableIdentity, SkillMinionWidget> dictionary1 = new Dictionary<IAssignableIdentity, SkillMinionWidget>();
    foreach (SkillMinionWidget sortableRow in this.sortableRows)
      dictionary1.Add(sortableRow.assignableIdentity, sortableRow);
    Dictionary<int, List<IAssignableIdentity>> minionsByWorld = ClusterManager.Instance.MinionsByWorld;
    this.sortableRows.Clear();
    Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
    int num1 = 0;
    int num2 = 0;
    foreach (KeyValuePair<int, List<IAssignableIdentity>> keyValuePair in minionsByWorld)
    {
      dictionary2.Add(keyValuePair.Key, num1);
      int num3 = num1 + 1;
      List<IAssignableIdentity> assignableIdentityList = new List<IAssignableIdentity>();
      foreach (IAssignableIdentity assignableIdentity in keyValuePair.Value)
        assignableIdentityList.Add(assignableIdentity);
      if (comparison != null)
      {
        assignableIdentityList.Sort(comparison);
        if (this.sortReversed)
          assignableIdentityList.Reverse();
      }
      num1 = num3 + assignableIdentityList.Count;
      num2 += assignableIdentityList.Count;
      for (int index = 0; index < assignableIdentityList.Count; ++index)
      {
        IAssignableIdentity key = assignableIdentityList[index];
        this.sortableRows.Add(dictionary1[key]);
      }
    }
    for (int index = 0; index < this.sortableRows.Count; ++index)
      ((Component) this.sortableRows[index]).gameObject.transform.SetSiblingIndex(index);
    foreach (KeyValuePair<int, int> keyValuePair in dictionary2)
      this.worldDividers[keyValuePair.Key].transform.SetSiblingIndex(keyValuePair.Value);
  }
}
