// Decompiled with JetBrains decompiler
// Type: ScheduleMinionWidget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduleMinionWidget")]
public class ScheduleMinionWidget : KMonoBehaviour
{
  [SerializeField]
  private CrewPortrait portrait;
  [SerializeField]
  private DropDown dropDown;
  [SerializeField]
  private LocText label;
  [SerializeField]
  private GameObject nightOwlIcon;
  [SerializeField]
  private GameObject earlyBirdIcon;
  [SerializeField]
  private GameObject worldContainer;

  public Schedulable schedulable { get; private set; }

  public void ChangeAssignment(Schedule targetSchedule, Schedulable schedulable)
  {
    DebugUtil.LogArgs(new object[6]
    {
      (object) "Assigning",
      (object) schedulable,
      (object) "from",
      (object) ScheduleManager.Instance.GetSchedule(schedulable).name,
      (object) "to",
      (object) targetSchedule.name
    });
    ScheduleManager.Instance.GetSchedule(schedulable).Unassign(schedulable);
    targetSchedule.Assign(schedulable);
  }

  public void Setup(Schedulable schedulable)
  {
    this.schedulable = schedulable;
    IAssignableIdentity component1 = ((Component) schedulable).GetComponent<IAssignableIdentity>();
    this.portrait.SetIdentityObject(component1);
    ((TMP_Text) this.label).text = component1.GetProperName();
    MinionIdentity minionIdentity = component1 as MinionIdentity;
    StoredMinionIdentity storedMinionIdentity = component1 as StoredMinionIdentity;
    this.RefreshWidgetWorldData();
    if (Object.op_Inequality((Object) minionIdentity, (Object) null))
    {
      Traits component2 = ((Component) minionIdentity).GetComponent<Traits>();
      if (component2.HasTrait("NightOwl"))
        this.nightOwlIcon.SetActive(true);
      else if (component2.HasTrait("EarlyBird"))
        this.earlyBirdIcon.SetActive(true);
    }
    else if (Object.op_Inequality((Object) storedMinionIdentity, (Object) null))
    {
      if (storedMinionIdentity.traitIDs.Contains("NightOwl"))
        this.nightOwlIcon.SetActive(true);
      else if (storedMinionIdentity.traitIDs.Contains("EarlyBird"))
        this.earlyBirdIcon.SetActive(true);
    }
    this.dropDown.Initialize(ScheduleManager.Instance.GetSchedules().Cast<IListableOption>(), new Action<IListableOption, object>(this.OnDropEntryClick), refreshAction: new Action<DropDownEntry, object>(this.DropEntryRefreshAction), displaySelectedValueWhenClosed: false, targetData: ((object) schedulable));
  }

  public void RefreshWidgetWorldData()
  {
    this.worldContainer.SetActive(DlcManager.IsExpansion1Active());
    MinionIdentity component = ((Component) this.schedulable).GetComponent<IAssignableIdentity>() as MinionIdentity;
    if (Object.op_Equality((Object) component, (Object) null) || !DlcManager.IsExpansion1Active())
      return;
    WorldContainer myWorld = component.GetMyWorld();
    string str = ((Component) myWorld).GetComponent<ClusterGridEntity>().Name;
    Image componentInChildren = this.worldContainer.GetComponentInChildren<Image>();
    componentInChildren.sprite = ((Component) myWorld).GetComponent<ClusterGridEntity>().GetUISprite();
    KMonoBehaviourExtensions.SetAlpha(componentInChildren, Object.op_Equality((Object) ClusterManager.Instance.activeWorld, (Object) myWorld) ? 1f : 0.7f);
    if (Object.op_Inequality((Object) ClusterManager.Instance.activeWorld, (Object) myWorld))
      str = "<color=" + Constants.NEUTRAL_COLOR_STR + ">" + str + "</color>";
    ((TMP_Text) this.worldContainer.GetComponentInChildren<LocText>()).SetText(str);
  }

  private void OnDropEntryClick(IListableOption option, object obj) => this.ChangeAssignment((Schedule) option, this.schedulable);

  private void DropEntryRefreshAction(DropDownEntry entry, object obj)
  {
    Schedule entryData = (Schedule) entry.entryData;
    if (((Schedulable) obj).GetSchedule() == entryData)
    {
      ((TMP_Text) entry.label).text = string.Format((string) STRINGS.UI.SCHEDULESCREEN.SCHEDULE_DROPDOWN_ASSIGNED, (object) entryData.name);
      entry.button.isInteractable = false;
    }
    else
    {
      ((TMP_Text) entry.label).text = entryData.name;
      entry.button.isInteractable = true;
    }
    ((Component) ((Component) entry).gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("worldContainer")).gameObject.SetActive(false);
  }

  public void SetupBlank(Schedule schedule)
  {
    ((TMP_Text) this.label).text = (string) STRINGS.UI.SCHEDULESCREEN.SCHEDULE_DROPDOWN_BLANK;
    this.dropDown.Initialize(Components.LiveMinionIdentities.Items.Cast<IListableOption>(), new Action<IListableOption, object>(this.OnBlankDropEntryClick), new Func<IListableOption, IListableOption, object, int>(this.BlankDropEntrySort), new Action<DropDownEntry, object>(this.BlankDropEntryRefreshAction), false, (object) schedule);
    Components.LiveMinionIdentities.OnAdd += new Action<MinionIdentity>(this.OnLivingMinionsChanged);
    Components.LiveMinionIdentities.OnRemove += new Action<MinionIdentity>(this.OnLivingMinionsChanged);
  }

  private void OnLivingMinionsChanged(MinionIdentity minion) => this.dropDown.ChangeContent(Components.LiveMinionIdentities.Items.Cast<IListableOption>());

  private void OnBlankDropEntryClick(IListableOption option, object obj)
  {
    Schedule targetSchedule = (Schedule) obj;
    MinionIdentity cmp = (MinionIdentity) option;
    if (Object.op_Equality((Object) cmp, (Object) null) || ((Component) cmp).HasTag(GameTags.Dead))
      return;
    this.ChangeAssignment(targetSchedule, ((Component) cmp).GetComponent<Schedulable>());
  }

  private void BlankDropEntryRefreshAction(DropDownEntry entry, object obj)
  {
    Schedule schedule = (Schedule) obj;
    MinionIdentity entryData = (MinionIdentity) entry.entryData;
    WorldContainer myWorld = entryData.GetMyWorld();
    ((Component) ((Component) entry).gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("worldContainer")).gameObject.SetActive(DlcManager.IsExpansion1Active());
    Image reference = ((Component) entry).gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("worldIcon");
    reference.sprite = ((Component) myWorld).GetComponent<ClusterGridEntity>().GetUISprite();
    KMonoBehaviourExtensions.SetAlpha(reference, Object.op_Equality((Object) ClusterManager.Instance.activeWorld, (Object) myWorld) ? 1f : 0.7f);
    string str = ((Component) myWorld).GetComponent<ClusterGridEntity>().Name;
    if (Object.op_Inequality((Object) ClusterManager.Instance.activeWorld, (Object) myWorld))
      str = "<color=" + Constants.NEUTRAL_COLOR_STR + ">" + str + "</color>";
    ((TMP_Text) ((Component) entry).gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("worldLabel")).SetText(str);
    if (schedule.IsAssigned(((Component) entryData).GetComponent<Schedulable>()))
    {
      ((TMP_Text) entry.label).text = string.Format((string) STRINGS.UI.SCHEDULESCREEN.SCHEDULE_DROPDOWN_ASSIGNED, (object) entryData.GetProperName());
      entry.button.isInteractable = false;
    }
    else
    {
      ((TMP_Text) entry.label).text = entryData.GetProperName();
      entry.button.isInteractable = true;
    }
    Traits component = ((Component) entryData).GetComponent<Traits>();
    ((Component) ((Component) entry).gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("NightOwlIcon")).gameObject.SetActive(component.HasTrait("NightOwl"));
    ((Component) ((Component) entry).gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("EarlyBirdIcon")).gameObject.SetActive(component.HasTrait("EarlyBird"));
  }

  private int BlankDropEntrySort(IListableOption a, IListableOption b, object obj)
  {
    Schedule schedule = (Schedule) obj;
    MinionIdentity minionIdentity1 = (MinionIdentity) a;
    MinionIdentity minionIdentity2 = (MinionIdentity) b;
    bool flag1 = schedule.IsAssigned(((Component) minionIdentity1).GetComponent<Schedulable>());
    bool flag2 = schedule.IsAssigned(((Component) minionIdentity2).GetComponent<Schedulable>());
    if (flag1 && !flag2)
      return -1;
    return !flag1 & flag2 ? 1 : 0;
  }

  protected virtual void OnCleanUp()
  {
    Components.LiveMinionIdentities.OnAdd -= new Action<MinionIdentity>(this.OnLivingMinionsChanged);
    Components.LiveMinionIdentities.OnRemove -= new Action<MinionIdentity>(this.OnLivingMinionsChanged);
  }
}
