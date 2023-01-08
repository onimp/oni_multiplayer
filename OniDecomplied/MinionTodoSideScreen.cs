// Decompiled with JetBrains decompiler
// Type: MinionTodoSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinionTodoSideScreen : SideScreenContent
{
  private bool useOffscreenIndicators;
  public MinionTodoChoreEntry taskEntryPrefab;
  public GameObject priorityGroupPrefab;
  public GameObject taskEntryContainer;
  public MinionTodoChoreEntry currentTask;
  public LocText currentScheduleBlockLabel;
  private List<Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>> priorityGroups = new List<Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>>();
  private List<MinionTodoChoreEntry> choreEntries = new List<MinionTodoChoreEntry>();
  private List<GameObject> choreTargets = new List<GameObject>();
  private SchedulerHandle refreshHandle;
  private ChoreConsumer choreConsumer;
  [SerializeField]
  private ColorStyleSetting buttonColorSettingCurrent;
  [SerializeField]
  private ColorStyleSetting buttonColorSettingStandard;
  private static List<JobsTableScreen.PriorityInfo> _priorityInfo;
  private int activeChoreEntries;

  public static List<JobsTableScreen.PriorityInfo> priorityInfo
  {
    get
    {
      if (MinionTodoSideScreen._priorityInfo == null)
        MinionTodoSideScreen._priorityInfo = new List<JobsTableScreen.PriorityInfo>()
        {
          new JobsTableScreen.PriorityInfo(4, Assets.GetSprite(HashedString.op_Implicit("ic_dupe")), STRINGS.UI.JOBSSCREEN.PRIORITY_CLASS.COMPULSORY),
          new JobsTableScreen.PriorityInfo(3, Assets.GetSprite(HashedString.op_Implicit("notification_exclamation")), STRINGS.UI.JOBSSCREEN.PRIORITY_CLASS.EMERGENCY),
          new JobsTableScreen.PriorityInfo(2, Assets.GetSprite(HashedString.op_Implicit("status_item_room_required")), STRINGS.UI.JOBSSCREEN.PRIORITY_CLASS.PERSONAL_NEEDS),
          new JobsTableScreen.PriorityInfo(1, Assets.GetSprite(HashedString.op_Implicit("status_item_prioritized")), STRINGS.UI.JOBSSCREEN.PRIORITY_CLASS.HIGH),
          new JobsTableScreen.PriorityInfo(0, (Sprite) null, STRINGS.UI.JOBSSCREEN.PRIORITY_CLASS.BASIC),
          new JobsTableScreen.PriorityInfo(-1, Assets.GetSprite(HashedString.op_Implicit("icon_gear")), STRINGS.UI.JOBSSCREEN.PRIORITY_CLASS.IDLE)
        };
      return MinionTodoSideScreen._priorityInfo;
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (this.priorityGroups.Count != 0)
      return;
    foreach (JobsTableScreen.PriorityInfo priorityInfo1 in MinionTodoSideScreen.priorityInfo)
    {
      PriorityScreen.PriorityClass priority = (PriorityScreen.PriorityClass) priorityInfo1.priority;
      if (priority == PriorityScreen.PriorityClass.basic)
      {
        for (int index = 5; index >= 0; --index)
        {
          Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences> tuple = new Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>(priority, index, Util.KInstantiateUI<HierarchyReferences>(this.priorityGroupPrefab, this.taskEntryContainer, false));
          ((Object) tuple.third).name = "PriorityGroup_" + (string) priorityInfo1.name + "_" + index.ToString();
          ((Component) tuple.third).gameObject.SetActive(true);
          JobsTableScreen.PriorityInfo priorityInfo2 = JobsTableScreen.priorityInfo[index];
          ((TMP_Text) tuple.third.GetReference<LocText>("Title")).text = priorityInfo2.name.text.ToUpper();
          tuple.third.GetReference<Image>("PriorityIcon").sprite = priorityInfo2.sprite;
          this.priorityGroups.Add(tuple);
        }
      }
      else
      {
        Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences> tuple = new Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences>(priority, 3, Util.KInstantiateUI<HierarchyReferences>(this.priorityGroupPrefab, this.taskEntryContainer, false));
        ((Object) tuple.third).name = "PriorityGroup_" + (string) priorityInfo1.name;
        ((Component) tuple.third).gameObject.SetActive(true);
        ((TMP_Text) tuple.third.GetReference<LocText>("Title")).text = priorityInfo1.name.text.ToUpper();
        tuple.third.GetReference<Image>("PriorityIcon").sprite = priorityInfo1.sprite;
        this.priorityGroups.Add(tuple);
      }
    }
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<MinionIdentity>(), (Object) null) && !target.HasTag(GameTags.Dead);

  public override void ClearTarget()
  {
    base.ClearTarget();
    this.refreshHandle.ClearScheduler();
  }

  public override void SetTarget(GameObject target)
  {
    this.refreshHandle.ClearScheduler();
    if (this.priorityGroups.Count == 0)
      ((KMonoBehaviour) this).OnPrefabInit();
    base.SetTarget(target);
  }

  public virtual void ScreenUpdate(bool topLevel)
  {
    base.ScreenUpdate(topLevel);
    this.PopulateElements();
  }

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    this.refreshHandle.ClearScheduler();
    if (!show)
    {
      if (!this.useOffscreenIndicators)
        return;
      foreach (GameObject choreTarget in this.choreTargets)
        OffscreenIndicator.Instance.DeactivateIndicator(choreTarget);
    }
    else
    {
      if (Object.op_Equality((Object) DetailsScreen.Instance.target, (Object) null))
        return;
      this.choreConsumer = DetailsScreen.Instance.target.GetComponent<ChoreConsumer>();
      this.PopulateElements();
    }
  }

  private void PopulateElements(object data = null)
  {
    this.refreshHandle.ClearScheduler();
    this.refreshHandle = UIScheduler.Instance.Schedule("RefreshToDoList", 0.1f, new Action<object>(this.PopulateElements), (object) null, (SchedulerGroup) null);
    ListPool<Chore.Precondition.Context, BuildingChoresPanel>.PooledList pooledList = ListPool<Chore.Precondition.Context, BuildingChoresPanel>.Allocate();
    ChoreConsumer.PreconditionSnapshot preconditionSnapshot = this.choreConsumer.GetLastPreconditionSnapshot();
    if (preconditionSnapshot.doFailedContextsNeedSorting)
    {
      preconditionSnapshot.failedContexts.Sort();
      preconditionSnapshot.doFailedContextsNeedSorting = false;
    }
    ((List<Chore.Precondition.Context>) pooledList).AddRange((IEnumerable<Chore.Precondition.Context>) preconditionSnapshot.failedContexts);
    ((List<Chore.Precondition.Context>) pooledList).AddRange((IEnumerable<Chore.Precondition.Context>) preconditionSnapshot.succeededContexts);
    Chore.Precondition.Context choreB = new Chore.Precondition.Context();
    MinionTodoChoreEntry minionTodoChoreEntry = (MinionTodoChoreEntry) null;
    int amount = 0;
    Schedulable component = DetailsScreen.Instance.target.GetComponent<Schedulable>();
    string str = "";
    Schedule schedule = component.GetSchedule();
    if (schedule != null)
      str = schedule.GetBlock(Schedule.GetBlockIdx()).name;
    ((TMP_Text) this.currentScheduleBlockLabel).SetText(string.Format((string) STRINGS.UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CURRENT_SCHEDULE_BLOCK, (object) str));
    this.choreTargets.Clear();
    bool flag = false;
    this.activeChoreEntries = 0;
    for (int index = ((List<Chore.Precondition.Context>) pooledList).Count - 1; index >= 0; --index)
    {
      if (((List<Chore.Precondition.Context>) pooledList)[index].chore != null && !((List<Chore.Precondition.Context>) pooledList)[index].chore.target.isNull && !Object.op_Equality((Object) ((List<Chore.Precondition.Context>) pooledList)[index].chore.target.gameObject, (Object) null) && ((List<Chore.Precondition.Context>) pooledList)[index].IsPotentialSuccess())
      {
        if (Object.op_Equality((Object) ((List<Chore.Precondition.Context>) pooledList)[index].chore.driver, (Object) this.choreConsumer.choreDriver))
        {
          this.currentTask.Apply(((List<Chore.Precondition.Context>) pooledList)[index]);
          minionTodoChoreEntry = this.currentTask;
          choreB = ((List<Chore.Precondition.Context>) pooledList)[index];
          amount = 0;
          flag = true;
        }
        else if (!flag && this.activeChoreEntries != 0 && GameUtil.AreChoresUIMergeable(((List<Chore.Precondition.Context>) pooledList)[index], choreB))
        {
          ++amount;
          minionTodoChoreEntry.SetMoreAmount(amount);
        }
        else
        {
          HierarchyReferences hierarchyReferences = this.PriorityGroupForPriority(this.choreConsumer, ((List<Chore.Precondition.Context>) pooledList)[index].chore);
          if (Object.op_Equality((Object) hierarchyReferences, (Object) null))
          {
            DebugUtil.DevLogError(string.Format("Priority group was null for {0} with priority class {1} and personaly priority {2}", (object) ((List<Chore.Precondition.Context>) pooledList)[index].chore.GetReportName(), (object) ((List<Chore.Precondition.Context>) pooledList)[index].chore.masterPriority.priority_class, (object) this.choreConsumer.GetPersonalPriority(((List<Chore.Precondition.Context>) pooledList)[index].chore.choreType)));
          }
          else
          {
            MinionTodoChoreEntry choreEntry = this.GetChoreEntry(hierarchyReferences.GetReference<RectTransform>("EntriesContainer"));
            choreEntry.Apply(((List<Chore.Precondition.Context>) pooledList)[index]);
            minionTodoChoreEntry = choreEntry;
            choreB = ((List<Chore.Precondition.Context>) pooledList)[index];
            amount = 0;
            flag = false;
          }
        }
      }
    }
    pooledList.Recycle();
    for (int index = this.choreEntries.Count - 1; index >= this.activeChoreEntries; --index)
      ((Component) this.choreEntries[index]).gameObject.SetActive(false);
    foreach (Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences> priorityGroup in this.priorityGroups)
      ((Component) priorityGroup.third).gameObject.SetActive(((Transform) priorityGroup.third.GetReference<RectTransform>("EntriesContainer")).childCount > 0);
  }

  private MinionTodoChoreEntry GetChoreEntry(RectTransform parent)
  {
    MinionTodoChoreEntry choreEntry;
    if (this.activeChoreEntries >= this.choreEntries.Count - 1)
    {
      choreEntry = Util.KInstantiateUI<MinionTodoChoreEntry>(((Component) this.taskEntryPrefab).gameObject, ((Component) parent).gameObject, false);
      this.choreEntries.Add(choreEntry);
    }
    else
    {
      choreEntry = this.choreEntries[this.activeChoreEntries];
      choreEntry.transform.SetParent((Transform) parent);
      choreEntry.transform.SetAsLastSibling();
    }
    ++this.activeChoreEntries;
    ((Component) choreEntry).gameObject.SetActive(true);
    return choreEntry;
  }

  private HierarchyReferences PriorityGroupForPriority(ChoreConsumer choreConsumer, Chore chore)
  {
    foreach (Tuple<PriorityScreen.PriorityClass, int, HierarchyReferences> priorityGroup in this.priorityGroups)
    {
      if (priorityGroup.first == chore.masterPriority.priority_class && (chore.masterPriority.priority_class != PriorityScreen.PriorityClass.basic || priorityGroup.second == choreConsumer.GetPersonalPriority(chore.choreType)))
        return priorityGroup.third;
    }
    return (HierarchyReferences) null;
  }

  private void Button_onPointerEnter() => throw new NotImplementedException();
}
