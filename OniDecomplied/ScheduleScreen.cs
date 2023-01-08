// Decompiled with JetBrains decompiler
// Type: ScheduleScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScheduleScreen : KScreen
{
  [SerializeField]
  private SchedulePaintButton paintButtonPrefab;
  [SerializeField]
  private GameObject paintButtonContainer;
  [SerializeField]
  private ScheduleScreenEntry scheduleEntryPrefab;
  [SerializeField]
  private GameObject scheduleEntryContainer;
  [SerializeField]
  private KButton addScheduleButton;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private ColorStyleSetting hygene_color;
  [SerializeField]
  private ColorStyleSetting work_color;
  [SerializeField]
  private ColorStyleSetting recreation_color;
  [SerializeField]
  private ColorStyleSetting sleep_color;
  private Dictionary<string, ColorStyleSetting> paintStyles;
  private List<ScheduleScreenEntry> entries;
  private List<SchedulePaintButton> paintButtons;
  private SchedulePaintButton selectedPaint;

  public virtual float GetSortKey() => 50f;

  protected virtual void OnPrefabInit()
  {
    this.ConsumeMouseScroll = true;
    this.entries = new List<ScheduleScreenEntry>();
    this.paintStyles = new Dictionary<string, ColorStyleSetting>();
    this.paintStyles["Hygene"] = this.hygene_color;
    this.paintStyles["Worktime"] = this.work_color;
    this.paintStyles["Recreation"] = this.recreation_color;
    this.paintStyles["Sleep"] = this.sleep_color;
  }

  protected virtual void OnSpawn()
  {
    this.paintButtons = new List<SchedulePaintButton>();
    foreach (ScheduleGroup allGroup in Db.Get().ScheduleGroups.allGroups)
      this.AddPaintButton(allGroup);
    foreach (Schedule schedule in ScheduleManager.Instance.GetSchedules())
      this.AddScheduleEntry(schedule);
    this.addScheduleButton.onClick += new System.Action(this.OnAddScheduleClick);
    this.closeButton.onClick += (System.Action) (() => ManagementMenu.Instance.CloseAll());
    ScheduleManager.Instance.onSchedulesChanged += new Action<List<Schedule>>(this.OnSchedulesChanged);
    Game.Instance.Subscribe(1983128072, new Action<object>(this.RefreshWidgetWorldData));
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    ScheduleManager.Instance.onSchedulesChanged -= new Action<List<Schedule>>(this.OnSchedulesChanged);
  }

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (!show)
      return;
    this.Activate();
  }

  private void AddPaintButton(ScheduleGroup group)
  {
    SchedulePaintButton schedulePaintButton = Util.KInstantiateUI<SchedulePaintButton>(((Component) this.paintButtonPrefab).gameObject, this.paintButtonContainer, true);
    schedulePaintButton.SetGroup(group, this.paintStyles, new Action<SchedulePaintButton>(this.OnPaintButtonClick));
    schedulePaintButton.SetToggle(false);
    this.paintButtons.Add(schedulePaintButton);
  }

  private void OnAddScheduleClick() => ScheduleManager.Instance.AddDefaultSchedule(false);

  private void OnPaintButtonClick(SchedulePaintButton clicked)
  {
    if (Object.op_Inequality((Object) this.selectedPaint, (Object) clicked))
    {
      foreach (SchedulePaintButton paintButton in this.paintButtons)
        paintButton.SetToggle(Object.op_Equality((Object) paintButton, (Object) clicked));
      this.selectedPaint = clicked;
    }
    else
    {
      clicked.SetToggle(false);
      this.selectedPaint = (SchedulePaintButton) null;
    }
  }

  private void OnPaintDragged(ScheduleScreenEntry entry, float ratio)
  {
    if (Object.op_Equality((Object) this.selectedPaint, (Object) null))
      return;
    int idx = Mathf.FloorToInt(ratio * (float) entry.schedule.GetBlocks().Count);
    entry.schedule.SetGroup(idx, this.selectedPaint.group);
  }

  private void AddScheduleEntry(Schedule schedule)
  {
    ScheduleScreenEntry scheduleScreenEntry = Util.KInstantiateUI<ScheduleScreenEntry>(((Component) this.scheduleEntryPrefab).gameObject, this.scheduleEntryContainer, true);
    scheduleScreenEntry.Setup(schedule, this.paintStyles, new Action<ScheduleScreenEntry, float>(this.OnPaintDragged));
    this.entries.Add(scheduleScreenEntry);
  }

  private void OnSchedulesChanged(List<Schedule> schedules)
  {
    foreach (Component entry in this.entries)
      Util.KDestroyGameObject(entry);
    this.entries.Clear();
    foreach (Schedule schedule in schedules)
      this.AddScheduleEntry(schedule);
  }

  private void RefreshWidgetWorldData(object data = null)
  {
    foreach (ScheduleScreenEntry entry in this.entries)
      entry.RefreshWidgetWorldData();
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (this.CheckBlockedInput())
    {
      if (((KInputEvent) e).Consumed)
        return;
      ((KInputEvent) e).Consumed = true;
    }
    else
      base.OnKeyDown(e);
  }

  private bool CheckBlockedInput()
  {
    bool flag = false;
    if (Object.op_Inequality((Object) EventSystem.current, (Object) null))
    {
      GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
      if (Object.op_Inequality((Object) selectedGameObject, (Object) null))
      {
        foreach (ScheduleScreenEntry entry in this.entries)
        {
          if (Object.op_Equality((Object) selectedGameObject, (Object) entry.GetNameInputField()))
          {
            flag = true;
            break;
          }
        }
      }
    }
    return flag;
  }
}
