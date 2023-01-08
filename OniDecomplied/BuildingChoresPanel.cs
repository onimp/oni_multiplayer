// Decompiled with JetBrains decompiler
// Type: BuildingChoresPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChoresPanel : TargetScreen
{
  public GameObject choreGroupPrefab;
  public GameObject chorePrefab;
  public BuildingChoresPanelDupeRow dupePrefab;
  private GameObject detailsPanel;
  private DetailsPanelDrawer drawer;
  private HierarchyReferences choreGroup;
  private List<HierarchyReferences> choreEntries = new List<HierarchyReferences>();
  private int activeChoreEntries;
  private List<BuildingChoresPanelDupeRow> dupeEntries = new List<BuildingChoresPanelDupeRow>();
  private int activeDupeEntries;
  private List<BuildingChoresPanel.DupeEntryData> DupeEntryDatas = new List<BuildingChoresPanel.DupeEntryData>();

  public override bool IsValidForTarget(GameObject target)
  {
    KPrefabID component = target.GetComponent<KPrefabID>();
    return Object.op_Inequality((Object) component, (Object) null) && component.HasTag(GameTags.HasChores) && !component.IsPrefabID(GameTags.Minion);
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.choreGroup = Util.KInstantiateUI<HierarchyReferences>(this.choreGroupPrefab, ((Component) this).gameObject, false);
    ((Component) this.choreGroup).gameObject.SetActive(true);
  }

  private void Update() => this.Refresh();

  public override void OnSelectTarget(GameObject target)
  {
    base.OnSelectTarget(target);
    this.Refresh();
  }

  public override void OnDeselectTarget(GameObject target) => base.OnDeselectTarget(target);

  private void Refresh() => this.RefreshDetails();

  private void RefreshDetails()
  {
    int myParentWorldId = this.selectedTarget.GetMyParentWorldId();
    List<Chore> choreList = (List<Chore>) null;
    GlobalChoreProvider.Instance.choreWorldMap.TryGetValue(myParentWorldId, out choreList);
    for (int index = 0; choreList != null && index < choreList.Count; ++index)
    {
      Chore chore = choreList[index];
      if (!chore.isNull && Object.op_Equality((Object) chore.gameObject, (Object) this.selectedTarget))
        this.AddChoreEntry(chore);
    }
    List<FetchChore> fetchChoreList = (List<FetchChore>) null;
    GlobalChoreProvider.Instance.fetchMap.TryGetValue(myParentWorldId, out fetchChoreList);
    for (int index = 0; fetchChoreList != null && index < fetchChoreList.Count; ++index)
    {
      FetchChore fetchChore = fetchChoreList[index];
      if (!fetchChore.isNull && Object.op_Equality((Object) fetchChore.gameObject, (Object) this.selectedTarget))
        this.AddChoreEntry((Chore) fetchChore);
    }
    for (int activeDupeEntries = this.activeDupeEntries; activeDupeEntries < this.dupeEntries.Count; ++activeDupeEntries)
      ((Component) this.dupeEntries[activeDupeEntries]).gameObject.SetActive(false);
    this.activeDupeEntries = 0;
    for (int activeChoreEntries = this.activeChoreEntries; activeChoreEntries < this.choreEntries.Count; ++activeChoreEntries)
      ((Component) this.choreEntries[activeChoreEntries]).gameObject.SetActive(false);
    this.activeChoreEntries = 0;
  }

  private void AddChoreEntry(Chore chore)
  {
    HierarchyReferences choreEntry = this.GetChoreEntry(GameUtil.GetChoreName(chore, (object) null), chore.choreType, this.choreGroup.GetReference<RectTransform>("EntriesContainer"));
    FetchChore fetch = chore as FetchChore;
    ListPool<Chore.Precondition.Context, BuildingChoresPanel>.PooledList pooledList = ListPool<Chore.Precondition.Context, BuildingChoresPanel>.Allocate();
    foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
    {
      ((List<Chore.Precondition.Context>) pooledList).Clear();
      ChoreConsumer component = ((Component) minionIdentity).GetComponent<ChoreConsumer>();
      Chore.Precondition.Context context = new Chore.Precondition.Context();
      ChoreConsumer.PreconditionSnapshot preconditionSnapshot = component.GetLastPreconditionSnapshot();
      if (preconditionSnapshot.doFailedContextsNeedSorting)
      {
        preconditionSnapshot.failedContexts.Sort();
        preconditionSnapshot.doFailedContextsNeedSorting = false;
      }
      ((List<Chore.Precondition.Context>) pooledList).AddRange((IEnumerable<Chore.Precondition.Context>) preconditionSnapshot.failedContexts);
      ((List<Chore.Precondition.Context>) pooledList).AddRange((IEnumerable<Chore.Precondition.Context>) preconditionSnapshot.succeededContexts);
      int num1 = -1;
      int num2 = 0;
      for (int index = ((List<Chore.Precondition.Context>) pooledList).Count - 1; index >= 0; --index)
      {
        if (!Object.op_Inequality((Object) ((List<Chore.Precondition.Context>) pooledList)[index].chore.driver, (Object) null) || !Object.op_Inequality((Object) ((List<Chore.Precondition.Context>) pooledList)[index].chore.driver, (Object) component.choreDriver))
        {
          bool flag = ((List<Chore.Precondition.Context>) pooledList)[index].IsPotentialSuccess();
          if (flag)
            ++num2;
          FetchAreaChore chore1 = ((List<Chore.Precondition.Context>) pooledList)[index].chore as FetchAreaChore;
          if (((List<Chore.Precondition.Context>) pooledList)[index].chore == chore || fetch != null && chore1 != null && chore1.smi.SameDestination(fetch))
          {
            num1 = flag ? num2 : int.MaxValue;
            context = ((List<Chore.Precondition.Context>) pooledList)[index];
            break;
          }
        }
      }
      if (num1 >= 0)
        this.DupeEntryDatas.Add(new BuildingChoresPanel.DupeEntryData()
        {
          consumer = component,
          context = context,
          personalPriority = component.GetPersonalPriority(chore.choreType),
          rank = num1
        });
    }
    pooledList.Recycle();
    this.DupeEntryDatas.Sort();
    foreach (BuildingChoresPanel.DupeEntryData dupeEntryData in this.DupeEntryDatas)
      this.GetDupeEntry(dupeEntryData, choreEntry.GetReference<RectTransform>("DupeContainer"));
    this.DupeEntryDatas.Clear();
  }

  private HierarchyReferences GetChoreEntry(
    string label,
    ChoreType choreType,
    RectTransform parent)
  {
    HierarchyReferences choreEntry;
    if (this.activeChoreEntries >= this.choreEntries.Count)
    {
      choreEntry = Util.KInstantiateUI<HierarchyReferences>(this.chorePrefab, ((Component) parent).gameObject, false);
      this.choreEntries.Add(choreEntry);
    }
    else
    {
      choreEntry = this.choreEntries[this.activeChoreEntries];
      choreEntry.transform.SetParent((Transform) parent);
      choreEntry.transform.SetAsLastSibling();
    }
    ++this.activeChoreEntries;
    ((TMP_Text) choreEntry.GetReference<LocText>("ChoreLabel")).text = label;
    ((TMP_Text) choreEntry.GetReference<LocText>("ChoreSubLabel")).text = GameUtil.ChoreGroupsForChoreType(choreType);
    Image reference1 = choreEntry.GetReference<Image>("Icon");
    if (choreType.groups.Length != 0)
    {
      Sprite sprite = Assets.GetSprite(HashedString.op_Implicit(choreType.groups[0].sprite));
      reference1.sprite = sprite;
      ((Component) reference1).gameObject.SetActive(true);
      ((Component) reference1).GetComponent<ToolTip>().toolTip = string.Format((string) STRINGS.UI.DETAILTABS.BUILDING_CHORES.CHORE_TYPE_TOOLTIP, (object) choreType.groups[0].Name);
    }
    else
      ((Component) reference1).gameObject.SetActive(false);
    Image reference2 = choreEntry.GetReference<Image>("Icon2");
    if (choreType.groups.Length > 1)
    {
      Sprite sprite = Assets.GetSprite(HashedString.op_Implicit(choreType.groups[1].sprite));
      reference2.sprite = sprite;
      ((Component) reference2).gameObject.SetActive(true);
      ((Component) reference2).GetComponent<ToolTip>().toolTip = string.Format((string) STRINGS.UI.DETAILTABS.BUILDING_CHORES.CHORE_TYPE_TOOLTIP, (object) choreType.groups[1].Name);
    }
    else
      ((Component) reference2).gameObject.SetActive(false);
    ((Component) choreEntry).gameObject.SetActive(true);
    return choreEntry;
  }

  private BuildingChoresPanelDupeRow GetDupeEntry(
    BuildingChoresPanel.DupeEntryData data,
    RectTransform parent)
  {
    BuildingChoresPanelDupeRow dupeEntry;
    if (this.activeDupeEntries >= this.dupeEntries.Count)
    {
      dupeEntry = Util.KInstantiateUI<BuildingChoresPanelDupeRow>(((Component) this.dupePrefab).gameObject, ((Component) parent).gameObject, false);
      this.dupeEntries.Add(dupeEntry);
    }
    else
    {
      dupeEntry = this.dupeEntries[this.activeDupeEntries];
      dupeEntry.transform.SetParent((Transform) parent);
      dupeEntry.transform.SetAsLastSibling();
    }
    ++this.activeDupeEntries;
    dupeEntry.Init(data);
    ((Component) dupeEntry).gameObject.SetActive(true);
    return dupeEntry;
  }

  public class DupeEntryData : IComparable<BuildingChoresPanel.DupeEntryData>
  {
    public ChoreConsumer consumer;
    public Chore.Precondition.Context context;
    public int personalPriority;
    public int rank;

    public int CompareTo(BuildingChoresPanel.DupeEntryData other)
    {
      if (this.personalPriority != other.personalPriority)
        return other.personalPriority.CompareTo(this.personalPriority);
      if (this.rank != other.rank)
        return this.rank.CompareTo(other.rank);
      return ((Component) this.consumer).GetProperName() != ((Component) other.consumer).GetProperName() ? ((Component) this.consumer).GetProperName().CompareTo(((Component) other.consumer).GetProperName()) : ((Object) this.consumer).GetInstanceID().CompareTo(((Object) other.consumer).GetInstanceID());
    }
  }
}
