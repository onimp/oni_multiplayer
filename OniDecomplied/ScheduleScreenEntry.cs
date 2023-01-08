// Decompiled with JetBrains decompiler
// Type: ScheduleScreenEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduleScreenEntry")]
public class ScheduleScreenEntry : KMonoBehaviour
{
  [SerializeField]
  private ScheduleBlockButton blockButtonPrefab;
  [SerializeField]
  private ScheduleBlockPainter blockButtonContainer;
  [SerializeField]
  private ScheduleMinionWidget minionWidgetPrefab;
  [SerializeField]
  private GameObject minionWidgetContainer;
  private ScheduleMinionWidget blankMinionWidget;
  [SerializeField]
  private EditableTitleBar title;
  [SerializeField]
  private LocText alarmField;
  [SerializeField]
  private KButton optionsButton;
  [SerializeField]
  private DialogPanel optionsPanel;
  [SerializeField]
  private LocText noteEntryLeft;
  [SerializeField]
  private LocText noteEntryRight;
  private List<ScheduleBlockButton> blockButtons;
  private List<ScheduleMinionWidget> minionWidgets;
  private Dictionary<string, int> blockTypeCounts = new Dictionary<string, int>();

  public Schedule schedule { get; private set; }

  public void Setup(
    Schedule schedule,
    Dictionary<string, ColorStyleSetting> paintStyles,
    Action<ScheduleScreenEntry, float> onPaintDragged)
  {
    this.schedule = schedule;
    ((Object) ((Component) this).gameObject).name = "Schedule_" + schedule.name;
    this.title.SetTitle(schedule.name);
    this.title.OnNameChanged += new Action<string>(this.OnNameChanged);
    this.blockButtonContainer.Setup((Action<float>) (f => onPaintDragged(this, f)));
    int num = 0;
    this.blockButtons = new List<ScheduleBlockButton>();
    int count = schedule.GetBlocks().Count;
    foreach (ScheduleBlock block in schedule.GetBlocks())
    {
      ScheduleBlockButton scheduleBlockButton = Util.KInstantiateUI<ScheduleBlockButton>(((Component) this.blockButtonPrefab).gameObject, ((Component) this.blockButtonContainer).gameObject, true);
      scheduleBlockButton.Setup(num++, paintStyles, count);
      scheduleBlockButton.SetBlockTypes(block.allowed_types);
      this.blockButtons.Add(scheduleBlockButton);
    }
    this.minionWidgets = new List<ScheduleMinionWidget>();
    this.blankMinionWidget = Util.KInstantiateUI<ScheduleMinionWidget>(((Component) this.minionWidgetPrefab).gameObject, this.minionWidgetContainer, false);
    this.blankMinionWidget.SetupBlank(schedule);
    this.RebuildMinionWidgets();
    this.RefreshNotes();
    this.RefreshAlarmButton();
    this.optionsButton.onClick += new System.Action(this.OnOptionsClicked);
    HierarchyReferences component = ((Component) this.optionsPanel).GetComponent<HierarchyReferences>();
    component.GetReference<MultiToggle>("AlarmButton").onClick += new System.Action(this.OnAlarmClicked);
    component.GetReference<KButton>("ResetButton").onClick += new System.Action(this.OnResetClicked);
    component.GetReference<KButton>("DeleteButton").onClick += new System.Action(this.OnDeleteClicked);
    schedule.onChanged += new Action<Schedule>(this.OnScheduleChanged);
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (this.schedule == null)
      return;
    this.schedule.onChanged -= new Action<Schedule>(this.OnScheduleChanged);
  }

  public GameObject GetNameInputField() => ((Component) this.title.inputField).gameObject;

  private void RebuildMinionWidgets()
  {
    if (!this.MinionWidgetsNeedRebuild())
      return;
    foreach (Component minionWidget in this.minionWidgets)
      Util.KDestroyGameObject(minionWidget);
    this.minionWidgets.Clear();
    foreach (Ref<Schedulable> @ref in this.schedule.GetAssigned())
    {
      ScheduleMinionWidget scheduleMinionWidget = Util.KInstantiateUI<ScheduleMinionWidget>(((Component) this.minionWidgetPrefab).gameObject, this.minionWidgetContainer, true);
      scheduleMinionWidget.Setup(@ref.Get());
      this.minionWidgets.Add(scheduleMinionWidget);
    }
    if (Components.LiveMinionIdentities.Count > this.schedule.GetAssigned().Count)
    {
      this.blankMinionWidget.transform.SetAsLastSibling();
      ((Component) this.blankMinionWidget).gameObject.SetActive(true);
    }
    else
      ((Component) this.blankMinionWidget).gameObject.SetActive(false);
  }

  private bool MinionWidgetsNeedRebuild()
  {
    List<Ref<Schedulable>> assigned = this.schedule.GetAssigned();
    if (assigned.Count != this.minionWidgets.Count || assigned.Count != Components.LiveMinionIdentities.Count != ((Component) this.blankMinionWidget).gameObject.activeSelf)
      return true;
    for (int index = 0; index < assigned.Count; ++index)
    {
      if (Object.op_Inequality((Object) assigned[index].Get(), (Object) this.minionWidgets[index].schedulable))
        return true;
    }
    return false;
  }

  public void RefreshWidgetWorldData()
  {
    foreach (ScheduleMinionWidget minionWidget in this.minionWidgets)
    {
      if (!Util.IsNullOrDestroyed((object) minionWidget))
        minionWidget.RefreshWidgetWorldData();
    }
  }

  private void OnNameChanged(string newName)
  {
    this.schedule.name = newName;
    ((Object) ((Component) this).gameObject).name = "Schedule_" + this.schedule.name;
  }

  private void OnOptionsClicked()
  {
    ((Component) this.optionsPanel).gameObject.SetActive(!((Component) this.optionsPanel).gameObject.activeSelf);
    ((Component) this.optionsPanel).GetComponent<Selectable>().Select();
  }

  private void OnAlarmClicked()
  {
    this.schedule.alarmActivated = !this.schedule.alarmActivated;
    this.RefreshAlarmButton();
  }

  private void RefreshAlarmButton()
  {
    MultiToggle reference = ((Component) this.optionsPanel).GetComponent<HierarchyReferences>().GetReference<MultiToggle>("AlarmButton");
    reference.ChangeState(this.schedule.alarmActivated ? 1 : 0);
    ToolTip component = ((Component) reference).GetComponent<ToolTip>();
    component.SetSimpleTooltip((string) (this.schedule.alarmActivated ? STRINGS.UI.SCHEDULESCREEN.ALARM_BUTTON_ON_TOOLTIP : STRINGS.UI.SCHEDULESCREEN.ALARM_BUTTON_OFF_TOOLTIP));
    ToolTipScreen.Instance.MarkTooltipDirty(component);
    ((TMP_Text) this.alarmField).text = (string) (this.schedule.alarmActivated ? STRINGS.UI.SCHEDULESCREEN.ALARM_TITLE_ENABLED : STRINGS.UI.SCHEDULESCREEN.ALARM_TITLE_DISABLED);
  }

  private void OnResetClicked() => this.schedule.SetBlocksToGroupDefaults(Db.Get().ScheduleGroups.allGroups);

  private void OnDeleteClicked() => ScheduleManager.Instance.DeleteSchedule(this.schedule);

  private void OnScheduleChanged(Schedule changedSchedule)
  {
    foreach (ScheduleBlockButton blockButton in this.blockButtons)
      blockButton.SetBlockTypes(changedSchedule.GetBlock(blockButton.idx).allowed_types);
    this.RefreshNotes();
    this.RebuildMinionWidgets();
  }

  private void RefreshNotes()
  {
    this.blockTypeCounts.Clear();
    foreach (Resource resource in Db.Get().ScheduleBlockTypes.resources)
      this.blockTypeCounts[resource.Id] = 0;
    foreach (ScheduleBlock block in this.schedule.GetBlocks())
    {
      foreach (Resource allowedType in block.allowed_types)
        this.blockTypeCounts[allowedType.Id]++;
    }
    if (Object.op_Equality((Object) this.noteEntryRight, (Object) null))
      return;
    ToolTip component = ((Component) this.noteEntryRight).GetComponent<ToolTip>();
    component.ClearMultiStringTooltip();
    int num = 0;
    foreach (KeyValuePair<string, int> blockTypeCount in this.blockTypeCounts)
    {
      if (blockTypeCount.Value == 0)
      {
        ++num;
        component.AddMultiStringTooltip(string.Format((string) STRINGS.UI.SCHEDULEGROUPS.NOTIME, (object) Db.Get().ScheduleBlockTypes.Get(blockTypeCount.Key).Name), (TextStyleSetting) null);
      }
    }
    if (num > 0)
      ((TMP_Text) this.noteEntryRight).text = string.Format((string) STRINGS.UI.SCHEDULEGROUPS.MISSINGBLOCKS, (object) num);
    else
      ((TMP_Text) this.noteEntryRight).text = "";
    string breakBonus = QualityOfLifeNeed.GetBreakBonus(this.blockTypeCounts[Db.Get().ScheduleBlockTypes.Recreation.Id]);
    if (breakBonus == null)
      return;
    Effect effect = Db.Get().effects.Get(breakBonus);
    if (effect == null)
      return;
    foreach (AttributeModifier selfModifier in effect.SelfModifiers)
    {
      if (selfModifier.AttributeId == Db.Get().Attributes.QualityOfLife.Id)
      {
        ((TMP_Text) this.noteEntryLeft).text = string.Format((string) STRINGS.UI.SCHEDULESCREEN.DOWNTIME_MORALE, (object) selfModifier.GetFormattedString());
        ((Component) this.noteEntryLeft).GetComponent<ToolTip>().SetSimpleTooltip(string.Format((string) STRINGS.UI.SCHEDULESCREEN.SCHEDULE_DOWNTIME_MORALE, (object) selfModifier.GetFormattedString()));
      }
    }
  }
}
