// Decompiled with JetBrains decompiler
// Type: JobsTableScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JobsTableScreen : TableScreen
{
  [SerializeField]
  private Color32 skillOutlineColourLow = Color32.op_Implicit(Color.white);
  [SerializeField]
  private Color32 skillOutlineColourHigh = Color32.op_Implicit(new Color(0.721568644f, 0.443137258f, 0.5803922f));
  [SerializeField]
  private int skillLevelLow = 1;
  [SerializeField]
  private int skillLevelHigh = 10;
  [SerializeField]
  private KButton settingsButton;
  [SerializeField]
  private KButton resetSettingsButton;
  [SerializeField]
  private KButton toggleAdvancedModeButton;
  [SerializeField]
  private KImage optionsPanel;
  [SerializeField]
  private bool dynamicRowSpacing = true;
  public TextStyleSetting TooltipTextStyle_Ability;
  public TextStyleSetting TooltipTextStyle_AbilityPositiveModifier;
  public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;
  private HashSet<MinionIdentity> dirty_single_minion_rows = new HashSet<MinionIdentity>();
  private static List<JobsTableScreen.PriorityInfo> _priorityInfo;
  private List<Sprite> prioritySprites;
  private List<KeyValuePair<GameObject, JobsTableScreen.SkillEventHandlerID>> EffectListeners = new List<KeyValuePair<GameObject, JobsTableScreen.SkillEventHandlerID>>();

  public virtual float GetSortKey() => 22f;

  public static List<JobsTableScreen.PriorityInfo> priorityInfo
  {
    get
    {
      if (JobsTableScreen._priorityInfo == null)
        JobsTableScreen._priorityInfo = new List<JobsTableScreen.PriorityInfo>()
        {
          new JobsTableScreen.PriorityInfo(0, Assets.GetSprite(HashedString.op_Implicit("icon_priority_disabled")), STRINGS.UI.JOBSSCREEN.PRIORITY.DISABLED),
          new JobsTableScreen.PriorityInfo(1, Assets.GetSprite(HashedString.op_Implicit("icon_priority_down_2")), STRINGS.UI.JOBSSCREEN.PRIORITY.VERYLOW),
          new JobsTableScreen.PriorityInfo(2, Assets.GetSprite(HashedString.op_Implicit("icon_priority_down")), STRINGS.UI.JOBSSCREEN.PRIORITY.LOW),
          new JobsTableScreen.PriorityInfo(3, Assets.GetSprite(HashedString.op_Implicit("icon_priority_flat")), STRINGS.UI.JOBSSCREEN.PRIORITY.STANDARD),
          new JobsTableScreen.PriorityInfo(4, Assets.GetSprite(HashedString.op_Implicit("icon_priority_up")), STRINGS.UI.JOBSSCREEN.PRIORITY.HIGH),
          new JobsTableScreen.PriorityInfo(5, Assets.GetSprite(HashedString.op_Implicit("icon_priority_up_2")), STRINGS.UI.JOBSSCREEN.PRIORITY.VERYHIGH),
          new JobsTableScreen.PriorityInfo(5, Assets.GetSprite(HashedString.op_Implicit("icon_priority_automatic")), STRINGS.UI.JOBSSCREEN.PRIORITY.VERYHIGH)
        };
      return JobsTableScreen._priorityInfo;
    }
  }

  protected override void OnActivate()
  {
    this.title = (string) STRINGS.UI.JOBSSCREEN.TITLE;
    base.OnActivate();
    this.resetSettingsButton.onClick += new System.Action(this.OnResetSettingsClicked);
    this.prioritySprites = new List<Sprite>();
    foreach (JobsTableScreen.PriorityInfo priorityInfo in JobsTableScreen.priorityInfo)
      this.prioritySprites.Add(priorityInfo.sprite);
    this.AddPortraitColumn("Portrait", new Action<IAssignableIdentity, GameObject>(((TableScreen) this).on_load_portrait), (Comparison<IAssignableIdentity>) null);
    this.AddButtonLabelColumn("Names", new Action<IAssignableIdentity, GameObject>(this.ConfigureNameLabel), new Func<IAssignableIdentity, GameObject, string>(((TableScreen) this).get_value_name_label), (Action<GameObject>) (widget_go => this.GetWidgetRow(widget_go).SelectMinion()), (Action<GameObject>) (widget_go => this.GetWidgetRow(widget_go).SelectAndFocusMinion()), new Comparison<IAssignableIdentity>(((TableScreen) this).compare_rows_alphabetical), (Action<IAssignableIdentity, GameObject, ToolTip>) null, new Action<IAssignableIdentity, GameObject, ToolTip>(((TableScreen) this).on_tooltip_sort_alphabetically));
    List<ChoreGroup> source = new List<ChoreGroup>((IEnumerable<ChoreGroup>) Db.Get().ChoreGroups.resources);
    source.OrderByDescending<ChoreGroup, int>((Func<ChoreGroup, int>) (group => group.DefaultPersonalPriority)).ThenBy<ChoreGroup, string>((Func<ChoreGroup, string>) (group => group.Name));
    foreach (ChoreGroup user_data in source)
    {
      if (user_data.userPrioritizable)
      {
        PrioritizationGroupTableColumn new_column = new PrioritizationGroupTableColumn((object) user_data, new Action<IAssignableIdentity, GameObject>(this.LoadValue), new Action<object, int>(this.ChangePersonalPriority), new Func<object, string>(this.HoverPersonalPriority), new Action<object, int>(this.ChangeColumnPriority), new Func<object, string>(this.HoverChangeColumnPriorityButton), new Action<object>(this.OnSortClicked), new Func<object, string>(this.OnSortHovered));
        this.RegisterColumn(user_data.Id, (TableColumn) new_column);
      }
    }
    this.RegisterColumn("prioritize_row", (TableColumn) new PrioritizeRowTableColumn((object) null, new Action<object, int>(this.ChangeRowPriority), new Func<object, int, string>(this.HoverChangeRowPriorityButton)));
    this.settingsButton.onClick += new System.Action(this.OnSettingsButtonClicked);
    this.resetSettingsButton.onClick += new System.Action(this.OnResetSettingsClicked);
    this.toggleAdvancedModeButton.onClick += new System.Action(this.OnAdvancedModeToggleClicked);
    ((Component) this.toggleAdvancedModeButton.fgImage).gameObject.SetActive(Game.Instance.advancedPersonalPriorities);
    this.RefreshEffectListeners();
  }

  private string HoverPersonalPriority(object widget_go_obj)
  {
    GameObject widget_go = widget_go_obj as GameObject;
    ChoreGroup userData = (this.GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn).userData as ChoreGroup;
    string str1 = (string) null;
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    switch (widgetRow.rowType)
    {
      case TableRow.RowType.Header:
        string str2 = STRINGS.UI.JOBSSCREEN.HEADER_TOOLTIP.ToString().Replace("{Job}", userData.Name);
        string str3 = STRINGS.UI.JOBSSCREEN.HEADER_DETAILS_TOOLTIP.ToString().Replace("{Description}", userData.description);
        HashSet<string> stringSet = new HashSet<string>();
        foreach (ChoreType choreType in userData.choreTypes)
          stringSet.Add(choreType.Name);
        StringBuilder stringBuilder = new StringBuilder();
        int num = 0;
        foreach (string str4 in stringSet)
        {
          stringBuilder.Append(str4);
          if (num < stringSet.Count - 1)
            stringBuilder.Append(", ");
          ++num;
        }
        string newValue = str3.Replace("{ChoreList}", stringBuilder.ToString());
        return str2.Replace("{Details}", newValue);
      case TableRow.RowType.Default:
        str1 = STRINGS.UI.JOBSSCREEN.NEW_MINION_ITEM_TOOLTIP.ToString();
        break;
      case TableRow.RowType.Minion:
      case TableRow.RowType.StoredMinon:
        str1 = STRINGS.UI.JOBSSCREEN.ITEM_TOOLTIP.ToString().Replace("{Name}", ((Object) widgetRow).name);
        break;
    }
    ToolTip componentInChildren = widget_go.GetComponentInChildren<ToolTip>();
    IAssignableIdentity identity = widgetRow.GetIdentity();
    MinionIdentity cmp = identity as MinionIdentity;
    if (Object.op_Inequality((Object) cmp, (Object) null))
    {
      IPersonalPriorityManager priorityManager = this.GetPriorityManager(widgetRow);
      int personalPriority = priorityManager.GetPersonalPriority(userData);
      string priorityStr = (string) this.GetPriorityStr(personalPriority);
      string priorityValue = this.GetPriorityValue(personalPriority);
      if (priorityManager.IsChoreGroupDisabled(userData))
      {
        Trait disablingTrait;
        ((Component) cmp).GetComponent<Traits>().IsChoreGroupDisabled(userData, out disablingTrait);
        string str5 = STRINGS.UI.JOBSSCREEN.TRAIT_DISABLED.ToString().Replace("{Name}", cmp.GetProperName()).Replace("{Job}", userData.Name).Replace("{Trait}", disablingTrait.Name);
        componentInChildren.ClearMultiStringTooltip();
        componentInChildren.AddMultiStringTooltip(str5, (TextStyleSetting) null);
      }
      else
      {
        string str6 = str1.Replace("{Job}", userData.Name).Replace("{Priority}", priorityStr).Replace("{PriorityValue}", priorityValue);
        componentInChildren.ClearMultiStringTooltip();
        componentInChildren.AddMultiStringTooltip(str6, (TextStyleSetting) null);
        if (Object.op_Inequality((Object) cmp, (Object) null))
        {
          string str7 = ("\n" + STRINGS.UI.JOBSSCREEN.MINION_SKILL_TOOLTIP.ToString()).Replace("{Name}", cmp.GetProperName()).Replace("{Attribute}", userData.attribute.Name);
          float totalValue = cmp.GetAttributes().Get(userData.attribute).GetTotalValue();
          TextStyleSetting textStyleAbility = this.TooltipTextStyle_Ability;
          string str8 = str7 + GameUtil.ColourizeString(Color32.op_Implicit(textStyleAbility.textColor), totalValue.ToString());
          componentInChildren.AddMultiStringTooltip(str8, (TextStyleSetting) null);
        }
        componentInChildren.AddMultiStringTooltip(STRINGS.UI.HORIZONTAL_RULE + "\n" + this.GetUsageString(), (TextStyleSetting) null);
      }
    }
    else if (Object.op_Inequality((Object) (identity as StoredMinionIdentity), (Object) null))
      componentInChildren.AddMultiStringTooltip(string.Format((string) STRINGS.UI.JOBSSCREEN.CANNOT_ADJUST_PRIORITY, (object) identity.GetProperName(), (object) (identity as StoredMinionIdentity).GetStorageReason()), (TextStyleSetting) null);
    return "";
  }

  private string HoverChangeColumnPriorityButton(object widget_go_obj)
  {
    ChoreGroup userData = (this.GetWidgetColumn(widget_go_obj as GameObject) as PrioritizationGroupTableColumn).userData as ChoreGroup;
    return STRINGS.UI.JOBSSCREEN.HEADER_CHANGE_TOOLTIP.ToString().Replace("{Job}", userData.Name);
  }

  private string GetUsageString() => GameUtil.ReplaceHotkeyString((string) STRINGS.UI.JOBSSCREEN.INCREASE_PRIORITY_TUTORIAL, (Action) 3) + "\n" + GameUtil.ReplaceHotkeyString((string) STRINGS.UI.JOBSSCREEN.DECREASE_PRIORITY_TUTORIAL, (Action) 5);

  private string HoverChangeRowPriorityButton(object widget_go_obj, int delta)
  {
    GameObject widget_go = widget_go_obj as GameObject;
    LocString locString1 = (LocString) null;
    LocString locString2 = (LocString) null;
    string newValue = (string) null;
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    switch (widgetRow.rowType)
    {
      case TableRow.RowType.Header:
        Debug.Assert(false);
        return (string) null;
      case TableRow.RowType.Default:
        locString1 = STRINGS.UI.JOBSSCREEN.INCREASE_ROW_PRIORITY_NEW_MINION_TOOLTIP;
        locString2 = STRINGS.UI.JOBSSCREEN.DECREASE_ROW_PRIORITY_NEW_MINION_TOOLTIP;
        break;
      case TableRow.RowType.Minion:
        locString1 = STRINGS.UI.JOBSSCREEN.INCREASE_ROW_PRIORITY_MINION_TOOLTIP;
        locString2 = STRINGS.UI.JOBSSCREEN.DECREASE_ROW_PRIORITY_MINION_TOOLTIP;
        newValue = widgetRow.GetIdentity().GetProperName();
        break;
      case TableRow.RowType.StoredMinon:
        StoredMinionIdentity identity = widgetRow.GetIdentity() as StoredMinionIdentity;
        if (Object.op_Inequality((Object) identity, (Object) null))
          return string.Format((string) STRINGS.UI.JOBSSCREEN.CANNOT_ADJUST_PRIORITY, (object) identity.GetProperName(), (object) identity.GetStorageReason());
        break;
    }
    string str = (delta > 0 ? (object) locString1 : (object) locString2).ToString();
    if (newValue != null)
      str = str.Replace("{Name}", newValue);
    return str;
  }

  private void OnSortClicked(object widget_go_obj)
  {
    PrioritizationGroupTableColumn widgetColumn = this.GetWidgetColumn(widget_go_obj as GameObject) as PrioritizationGroupTableColumn;
    ChoreGroup chore_group = widgetColumn.userData as ChoreGroup;
    if (this.active_sort_column == widgetColumn)
      this.sort_is_reversed = !this.sort_is_reversed;
    this.active_sort_column = (TableColumn) widgetColumn;
    this.active_sort_method = (Comparison<IAssignableIdentity>) ((a, b) =>
    {
      MinionIdentity minionIdentity1 = a as MinionIdentity;
      MinionIdentity minionIdentity2 = b as MinionIdentity;
      if (Object.op_Equality((Object) minionIdentity1, (Object) null) && Object.op_Equality((Object) minionIdentity2, (Object) null))
        return 0;
      if (Object.op_Equality((Object) minionIdentity1, (Object) null))
        return -1;
      if (Object.op_Equality((Object) minionIdentity2, (Object) null))
        return 1;
      ChoreConsumer component1 = ((Component) minionIdentity1).GetComponent<ChoreConsumer>();
      ChoreConsumer component2 = ((Component) minionIdentity2).GetComponent<ChoreConsumer>();
      if (component1.IsChoreGroupDisabled(chore_group))
        return 1;
      if (component2.IsChoreGroupDisabled(chore_group))
        return -1;
      int personalPriority1 = component1.GetPersonalPriority(chore_group);
      int personalPriority2 = component2.GetPersonalPriority(chore_group);
      return personalPriority1 == personalPriority2 ? ((Object) minionIdentity1).name.CompareTo(((Object) minionIdentity2).name) : personalPriority2 - personalPriority1;
    });
    this.SortRows();
  }

  private string OnSortHovered(object widget_go_obj)
  {
    ChoreGroup userData = (this.GetWidgetColumn(widget_go_obj as GameObject) as PrioritizationGroupTableColumn).userData as ChoreGroup;
    return STRINGS.UI.JOBSSCREEN.SORT_TOOLTIP.ToString().Replace("{Job}", userData.Name);
  }

  private IPersonalPriorityManager GetPriorityManager(TableRow row)
  {
    IPersonalPriorityManager priorityManager = (IPersonalPriorityManager) null;
    switch (row.rowType)
    {
      case TableRow.RowType.Default:
        priorityManager = (IPersonalPriorityManager) Immigration.Instance;
        break;
      case TableRow.RowType.Minion:
        MinionIdentity identity = row.GetIdentity() as MinionIdentity;
        if (Object.op_Inequality((Object) identity, (Object) null))
        {
          priorityManager = (IPersonalPriorityManager) ((Component) identity).GetComponent<ChoreConsumer>();
          break;
        }
        break;
      case TableRow.RowType.StoredMinon:
        priorityManager = (IPersonalPriorityManager) (row.GetIdentity() as StoredMinionIdentity);
        break;
    }
    return priorityManager;
  }

  private LocString GetPriorityStr(int priority)
  {
    priority = Mathf.Clamp(priority, 0, 5);
    LocString priorityStr = (LocString) null;
    foreach (JobsTableScreen.PriorityInfo priorityInfo in JobsTableScreen.priorityInfo)
    {
      if (priorityInfo.priority == priority)
        priorityStr = priorityInfo.name;
    }
    return priorityStr;
  }

  private string GetPriorityValue(int priority) => (priority * 10).ToString();

  private void LoadValue(IAssignableIdentity minion, GameObject widget_go)
  {
    if (Object.op_Equality((Object) widget_go, (Object) null))
      return;
    ChoreGroup userData = (this.GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn).userData as ChoreGroup;
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    switch (widgetRow.rowType)
    {
      case TableRow.RowType.Header:
        this.InitializeHeader(userData, widget_go);
        break;
      case TableRow.RowType.Default:
      case TableRow.RowType.Minion:
      case TableRow.RowType.StoredMinon:
        bool flag = this.GetPriorityManager(widgetRow).IsChoreGroupDisabled(userData);
        HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
        ((Graphic) (component.GetReference("FG") as KImage)).raycastTarget = flag;
        ((Behaviour) (component.GetReference("FGToolTip") as ToolTip)).enabled = flag;
        break;
    }
    IPersonalPriorityManager priorityManager = this.GetPriorityManager(widgetRow);
    if (priorityManager == null)
      return;
    this.UpdateWidget(widget_go, userData, priorityManager);
  }

  private JobsTableScreen.PriorityInfo GetPriorityInfo(int priority)
  {
    JobsTableScreen.PriorityInfo priorityInfo = new JobsTableScreen.PriorityInfo();
    for (int index = 0; index < JobsTableScreen.priorityInfo.Count; ++index)
    {
      if (JobsTableScreen.priorityInfo[index].priority == priority)
      {
        priorityInfo = JobsTableScreen.priorityInfo[index];
        break;
      }
    }
    return priorityInfo;
  }

  private void ChangePersonalPriority(object widget_go_obj, int delta)
  {
    GameObject widget_go = widget_go_obj as GameObject;
    if (widget_go_obj == null)
      return;
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    if (widgetRow.rowType == TableRow.RowType.Header)
      Debug.Assert(false);
    ChoreGroup userData = (this.GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn).userData as ChoreGroup;
    IPersonalPriorityManager priorityManager = this.GetPriorityManager(widgetRow);
    this.ChangePersonalPriority(priorityManager, userData, delta, true);
    this.UpdateWidget(widget_go, userData, priorityManager);
  }

  private void ChangeColumnPriority(object widget_go_obj, int new_priority)
  {
    GameObject widget_go = widget_go_obj as GameObject;
    if (widget_go_obj == null)
      return;
    if (this.GetWidgetRow(widget_go).rowType != TableRow.RowType.Header)
      Debug.Assert(false);
    PrioritizationGroupTableColumn widgetColumn = this.GetWidgetColumn(widget_go) as PrioritizationGroupTableColumn;
    ChoreGroup userData = widgetColumn.userData as ChoreGroup;
    foreach (TableRow row in this.rows)
    {
      IPersonalPriorityManager priorityManager = this.GetPriorityManager(row);
      if (priorityManager != null)
      {
        priorityManager.SetPersonalPriority(userData, new_priority);
        this.UpdateWidget(row.GetWidget((TableColumn) widgetColumn), userData, priorityManager);
      }
    }
  }

  private void ChangeRowPriority(object widget_go_obj, int delta)
  {
    GameObject widget_go = widget_go_obj as GameObject;
    if (widget_go_obj == null)
      return;
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    if (widgetRow.rowType == TableRow.RowType.Header)
    {
      Debug.Assert(false);
    }
    else
    {
      IPersonalPriorityManager priorityManager = this.GetPriorityManager(widgetRow);
      foreach (TableColumn tableColumn in this.columns.Values)
      {
        if (tableColumn is PrioritizationGroupTableColumn column)
        {
          ChoreGroup userData = column.userData as ChoreGroup;
          GameObject widget = widgetRow.GetWidget((TableColumn) column);
          this.ChangePersonalPriority(priorityManager, userData, delta, false);
          this.UpdateWidget(widget, userData, priorityManager);
        }
      }
    }
  }

  private void ChangePersonalPriority(
    IPersonalPriorityManager priority_mgr,
    ChoreGroup chore_group,
    int delta,
    bool wrap_around)
  {
    if (priority_mgr.IsChoreGroupDisabled(chore_group))
    {
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
    }
    else
    {
      int num1 = priority_mgr.GetPersonalPriority(chore_group) + delta;
      if (wrap_around)
      {
        num1 %= 6;
        if (num1 < 0)
          num1 += 6;
      }
      int num2 = Mathf.Clamp(num1, 0, 5);
      priority_mgr.SetPersonalPriority(chore_group, num2);
      if (delta > 0)
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
      else
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect"));
    }
  }

  private void UpdateWidget(
    GameObject widget_go,
    ChoreGroup chore_group,
    IPersonalPriorityManager priority_mgr)
  {
    int num1 = 0;
    int num2 = 0;
    bool flag = priority_mgr.IsChoreGroupDisabled(chore_group);
    if (!flag)
      num2 = priority_mgr.GetPersonalPriority(chore_group);
    int num3 = Mathf.Clamp(num2, 0, 5);
    for (int index = 0; index < JobsTableScreen.priorityInfo.Count - 1; ++index)
    {
      if (JobsTableScreen.priorityInfo[index].priority == num3)
      {
        num1 = index;
        break;
      }
    }
    OptionSelector component = widget_go.GetComponent<OptionSelector>();
    int num4 = priority_mgr != null ? priority_mgr.GetAssociatedSkillLevel(chore_group) : 0;
    Color32 color32;
    // ISSUE: explicit constructor call
    ((Color32) ref color32).\u002Ector(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte) 128);
    if (num4 > 0)
      color32 = Color32.Lerp(this.skillOutlineColourLow, this.skillOutlineColourHigh, (float) (num4 - this.skillLevelLow) / (float) (this.skillLevelHigh - this.skillLevelLow));
    int num5 = flag ? 1 : 0;
    component.ConfigureItem(num5 != 0, new OptionSelector.DisplayOptionInfo()
    {
      bgOptions = (IList<Sprite>) null,
      fgOptions = (IList<Sprite>) this.prioritySprites,
      bgIndex = 0,
      fgIndex = num1,
      fillColour = color32
    });
    ToolTip componentInChildren = ((Component) widget_go.transform).GetComponentInChildren<ToolTip>();
    if (!Object.op_Inequality((Object) componentInChildren, (Object) null))
      return;
    componentInChildren.toolTip = this.HoverPersonalPriority((object) widget_go);
    componentInChildren.forceRefresh = true;
  }

  public void ToggleColumnSortWidgets(bool show)
  {
    foreach (KeyValuePair<string, TableColumn> column in this.columns)
    {
      if (Object.op_Inequality((Object) column.Value.column_sort_toggle, (Object) null))
        ((Component) column.Value.column_sort_toggle).gameObject.SetActive(show);
    }
  }

  public void Refresh(MinionResume minion_resume)
  {
    if (Object.op_Equality((Object) this, (Object) null))
      return;
    foreach (TableRow row in this.rows)
    {
      IAssignableIdentity identity = row.GetIdentity();
      if (!Object.op_Equality((Object) (identity as MinionIdentity), (Object) null) && !Object.op_Inequality((Object) ((Component) (identity as MinionIdentity)).gameObject, (Object) ((Component) minion_resume).gameObject))
      {
        foreach (TableColumn tableColumn in this.columns.Values)
        {
          if (tableColumn is PrioritizationGroupTableColumn column)
            this.UpdateWidget(row.GetWidget((TableColumn) column), column.userData as ChoreGroup, (IPersonalPriorityManager) ((Component) (identity as MinionIdentity)).GetComponent<ChoreConsumer>());
        }
      }
    }
  }

  protected override void RefreshRows()
  {
    base.RefreshRows();
    this.RefreshEffectListeners();
    if (!this.dynamicRowSpacing)
      return;
    this.SizeRows();
  }

  private void SizeRows()
  {
    float num1 = 0.0f;
    int num2 = 0;
    for (int index = 0; index < this.header_row.transform.childCount; ++index)
    {
      Transform child = this.header_row.transform.GetChild(index);
      LayoutElement component1 = ((Component) child).GetComponent<LayoutElement>();
      if (Object.op_Inequality((Object) component1, (Object) null) && !component1.ignoreLayout)
      {
        ++num2;
        num1 += component1.minWidth;
      }
      else
      {
        HorizontalOrVerticalLayoutGroup component2 = ((Component) child).GetComponent<HorizontalOrVerticalLayoutGroup>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          float x = Util.rectTransform((Component) component2).sizeDelta.x;
          num1 += x;
          ++num2;
        }
      }
    }
    Rect rect = Util.rectTransform(((Component) this).gameObject).rect;
    double width = (double) ((Rect) ref rect).width;
    float num3 = 0.0f;
    HorizontalLayoutGroup component = this.header_row.GetComponent<HorizontalLayoutGroup>();
    ((HorizontalOrVerticalLayoutGroup) component).spacing = num3;
    ((LayoutGroup) component).childAlignment = (TextAnchor) 3;
    foreach (KMonoBehaviour row in this.rows)
      ((HorizontalOrVerticalLayoutGroup) ((Component) row.transform).GetComponentInChildren<HorizontalLayoutGroup>()).spacing = num3;
  }

  private void RefreshEffectListeners()
  {
    for (int index = 0; index < this.EffectListeners.Count; ++index)
    {
      KMonoBehaviourExtensions.Unsubscribe(this.EffectListeners[index].Key, this.EffectListeners[index].Value.level_up);
      KMonoBehaviourExtensions.Unsubscribe(this.EffectListeners[index].Key, this.EffectListeners[index].Value.effect_added);
      KMonoBehaviourExtensions.Unsubscribe(this.EffectListeners[index].Key, this.EffectListeners[index].Value.effect_removed);
      KMonoBehaviourExtensions.Unsubscribe(this.EffectListeners[index].Key, this.EffectListeners[index].Value.disease_added);
      KMonoBehaviourExtensions.Unsubscribe(this.EffectListeners[index].Key, this.EffectListeners[index].Value.effect_added);
    }
    this.EffectListeners.Clear();
    for (int idx = 0; idx < Components.LiveMinionIdentities.Count; ++idx)
    {
      JobsTableScreen.SkillEventHandlerID skillEventHandlerId = new JobsTableScreen.SkillEventHandlerID();
      MinionIdentity id = Components.LiveMinionIdentities[idx];
      Action<object> action = (Action<object>) (o => this.MarkSingleMinionRowDirty(id));
      skillEventHandlerId.level_up = KMonoBehaviourExtensions.Subscribe(((Component) Components.LiveMinionIdentities[idx]).gameObject, -110704193, action);
      skillEventHandlerId.effect_added = KMonoBehaviourExtensions.Subscribe(((Component) Components.LiveMinionIdentities[idx]).gameObject, -1901442097, action);
      skillEventHandlerId.effect_removed = KMonoBehaviourExtensions.Subscribe(((Component) Components.LiveMinionIdentities[idx]).gameObject, -1157678353, action);
      skillEventHandlerId.disease_added = KMonoBehaviourExtensions.Subscribe(((Component) Components.LiveMinionIdentities[idx]).gameObject, 1592732331, action);
      skillEventHandlerId.disease_cured = KMonoBehaviourExtensions.Subscribe(((Component) Components.LiveMinionIdentities[idx]).gameObject, 77635178, action);
    }
    for (int idx = 0; idx < Components.LiveMinionIdentities.Count; ++idx)
    {
      MinionIdentity id = Components.LiveMinionIdentities[idx];
      KMonoBehaviourExtensions.Subscribe(((Component) Components.LiveMinionIdentities[idx]).gameObject, 540773776, (Action<object>) (new_role => this.MarkSingleMinionRowDirty(id)));
    }
  }

  public override void ScreenUpdate(bool topLevel)
  {
    base.ScreenUpdate(topLevel);
    if (this.dirty_single_minion_rows.Count == 0)
      return;
    foreach (MinionIdentity dirtySingleMinionRow in this.dirty_single_minion_rows)
    {
      if (!Object.op_Equality((Object) dirtySingleMinionRow, (Object) null))
        this.RefreshSingleMinionRow((IAssignableIdentity) dirtySingleMinionRow);
    }
    this.dirty_single_minion_rows.Clear();
  }

  protected void MarkSingleMinionRowDirty(MinionIdentity id) => this.dirty_single_minion_rows.Add(id);

  private void RefreshSingleMinionRow(IAssignableIdentity id)
  {
    foreach (KeyValuePair<string, TableColumn> column in this.columns)
    {
      if (column.Value != null && column.Value.on_load_action != null)
      {
        foreach (KeyValuePair<TableRow, GameObject> keyValuePair in column.Value.widgets_by_row)
        {
          if (!Object.op_Equality((Object) keyValuePair.Value, (Object) null) && keyValuePair.Key.GetIdentity() == id)
            column.Value.on_load_action(id, keyValuePair.Value);
        }
        column.Value.on_load_action((IAssignableIdentity) null, this.rows[0].GetWidget(column.Value));
      }
    }
  }

  protected virtual void OnCmpDisable()
  {
    EventSystem.current.SetSelectedGameObject((GameObject) null);
    ((KMonoBehaviour) this).OnCmpDisable();
    foreach (TableColumn column in this.columns.Values)
    {
      foreach (TableRow row in this.rows)
      {
        GameObject widget = row.GetWidget(column);
        if (!Object.op_Equality((Object) widget, (Object) null))
        {
          GroupSelectorWidget[] componentsInChildren1 = widget.GetComponentsInChildren<GroupSelectorWidget>();
          if (componentsInChildren1 != null)
          {
            foreach (GroupSelectorWidget groupSelectorWidget in componentsInChildren1)
              groupSelectorWidget.CloseSubPanel();
          }
          GroupSelectorHeaderWidget[] componentsInChildren2 = widget.GetComponentsInChildren<GroupSelectorHeaderWidget>();
          if (componentsInChildren2 != null)
          {
            foreach (GroupSelectorHeaderWidget selectorHeaderWidget in componentsInChildren2)
              selectorHeaderWidget.CloseSubPanel();
          }
          SelectablePanel[] componentsInChildren3 = widget.GetComponentsInChildren<SelectablePanel>();
          if (componentsInChildren3 != null)
          {
            foreach (Component component in componentsInChildren3)
              component.gameObject.SetActive(false);
          }
        }
      }
    }
    ((Component) this.optionsPanel).gameObject.SetActive(false);
  }

  private void GetMouseHoverInfo(out bool is_hovering_screen, out bool is_hovering_button)
  {
    EventSystem current = EventSystem.current;
    if (Object.op_Equality((Object) current, (Object) null))
    {
      is_hovering_button = false;
      is_hovering_screen = false;
    }
    else
    {
      List<RaycastResult> raycastResultList = new List<RaycastResult>();
      current.RaycastAll(new PointerEventData(current)
      {
        position = Vector2.op_Implicit(KInputManager.GetMousePos())
      }, raycastResultList);
      bool flag1 = false;
      bool flag2 = false;
      foreach (RaycastResult raycastResult in raycastResultList)
      {
        if (Object.op_Inequality((Object) ((RaycastResult) ref raycastResult).gameObject.GetComponent<OptionSelector>(), (Object) null) || Object.op_Inequality((Object) ((RaycastResult) ref raycastResult).gameObject.transform.parent, (Object) null) && Object.op_Inequality((Object) ((Component) ((RaycastResult) ref raycastResult).gameObject.transform.parent).GetComponent<OptionSelector>(), (Object) null))
        {
          flag1 = true;
          flag2 = true;
          break;
        }
        if (this.HasParent(((RaycastResult) ref raycastResult).gameObject, ((Component) this).gameObject))
          flag2 = true;
      }
      is_hovering_screen = flag2;
      is_hovering_button = flag1;
    }
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    bool flag = false;
    if (e.IsAction((Action) 5))
    {
      bool is_hovering_button;
      this.GetMouseHoverInfo(out bool _, out is_hovering_button);
      if (is_hovering_button)
      {
        flag = true;
        if (!((KInputEvent) e).Consumed)
          e.TryConsume((Action) 5);
      }
    }
    if (flag)
      return;
    ((KScreen) this).OnKeyDown(e);
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    bool flag = false;
    if (e.IsAction((Action) 5))
    {
      bool is_hovering_button;
      this.GetMouseHoverInfo(out bool _, out is_hovering_button);
      if (is_hovering_button)
      {
        e.TryConsume((Action) 5);
        flag = true;
      }
    }
    if (flag)
      return;
    ((KScreen) this).OnKeyUp(e);
  }

  private bool HasParent(GameObject obj, GameObject parent)
  {
    bool flag = false;
    Transform transform1 = parent.transform;
    for (Transform transform2 = obj.transform; Object.op_Inequality((Object) transform2, (Object) null); transform2 = transform2.parent)
    {
      if (Object.op_Equality((Object) transform2, (Object) transform1))
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  private void ConfigureNameLabel(IAssignableIdentity identity, GameObject widget_go)
  {
    // ISSUE: unable to decompile the method.
  }

  private void InitializeHeader(ChoreGroup chore_group, GameObject widget_go)
  {
    HierarchyReferences component1 = widget_go.GetComponent<HierarchyReferences>();
    HierarchyReferences reference1 = component1.GetReference("PrioritizationWidget") as HierarchyReferences;
    GameObject items_root = reference1.GetReference("ItemPanel").gameObject;
    if (items_root.transform.childCount > 0)
      return;
    items_root.SetActive(false);
    ((TMP_Text) (component1.GetReference("Label") as LocText)).text = chore_group.Name;
    KButton reference2 = component1.GetReference("PrioritizeButton") as KButton;
    Selectable selectable = items_root.GetComponent<Selectable>();
    System.Action action = (System.Action) (() =>
    {
      selectable.Select();
      items_root.SetActive(true);
    });
    reference2.onClick += action;
    GameObject gameObject1 = reference1.GetReference("ItemTemplate").gameObject;
    for (int priority = 5; priority >= 0; --priority)
    {
      JobsTableScreen.PriorityInfo priorityInfo = this.GetPriorityInfo(priority);
      if (priorityInfo.name != null)
      {
        GameObject gameObject2 = Util.KInstantiateUI(gameObject1, items_root, true);
        KButton component2 = gameObject2.GetComponent<KButton>();
        HierarchyReferences component3 = gameObject2.GetComponent<HierarchyReferences>();
        KImage reference3 = component3.GetReference("Icon") as KImage;
        LocText reference4 = component3.GetReference("Label") as LocText;
        int new_priority = priority;
        component2.onClick += (System.Action) (() =>
        {
          this.ChangeColumnPriority((object) widget_go, new_priority);
          EventSystem.current.SetSelectedGameObject((GameObject) null);
        });
        ((Image) reference3).sprite = priorityInfo.sprite;
        string name = (string) priorityInfo.name;
        ((TMP_Text) reference4).text = name;
      }
    }
  }

  private void OnSettingsButtonClicked()
  {
    ((Component) this.optionsPanel).gameObject.SetActive(true);
    ((Component) this.optionsPanel).GetComponent<Selectable>().Select();
  }

  private void OnResetSettingsClicked()
  {
    if (Game.Instance.advancedPersonalPriorities)
    {
      if (Object.op_Inequality((Object) Immigration.Instance, (Object) null))
        Immigration.Instance.ResetPersonalPriorities();
      foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
      {
        if (!Object.op_Equality((Object) minionIdentity, (Object) null))
          Immigration.Instance.ApplyDefaultPersonalPriorities(((Component) minionIdentity).gameObject);
      }
    }
    else
    {
      foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
      {
        if (!Object.op_Equality((Object) minionIdentity, (Object) null))
        {
          ChoreConsumer component = ((Component) minionIdentity).GetComponent<ChoreConsumer>();
          foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
          {
            if (resource.userPrioritizable)
              component.SetPersonalPriority(resource, 3);
          }
        }
      }
    }
    this.MarkRowsDirty();
  }

  private void OnAdvancedModeToggleClicked()
  {
    Game.Instance.advancedPersonalPriorities = !Game.Instance.advancedPersonalPriorities;
    ((Component) this.toggleAdvancedModeButton.fgImage).gameObject.SetActive(Game.Instance.advancedPersonalPriorities);
  }

  public struct PriorityInfo
  {
    public int priority;
    public Sprite sprite;
    public LocString name;

    public PriorityInfo(int priority, Sprite sprite, LocString name)
    {
      this.priority = priority;
      this.sprite = sprite;
      this.name = name;
    }
  }

  private struct SkillEventHandlerID
  {
    public int level_up;
    public int effect_added;
    public int effect_removed;
    public int disease_added;
    public int disease_cured;
  }
}
