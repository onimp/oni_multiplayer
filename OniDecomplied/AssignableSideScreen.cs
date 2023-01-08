// Decompiled with JetBrains decompiler
// Type: AssignableSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssignableSideScreen : SideScreenContent
{
  [SerializeField]
  private AssignableSideScreenRow rowPrefab;
  [SerializeField]
  private GameObject rowGroup;
  [SerializeField]
  private LocText currentOwnerText;
  [SerializeField]
  private MultiToggle dupeSortingToggle;
  [SerializeField]
  private MultiToggle generalSortingToggle;
  private MultiToggle activeSortToggle;
  private Comparison<IAssignableIdentity> activeSortFunction;
  private bool sortReversed;
  private int targetAssignableSubscriptionHandle = -1;
  private UIPool<AssignableSideScreenRow> rowPool;
  private Dictionary<IAssignableIdentity, AssignableSideScreenRow> identityRowMap = new Dictionary<IAssignableIdentity, AssignableSideScreenRow>();
  private List<MinionAssignablesProxy> identityList = new List<MinionAssignablesProxy>();

  public Assignable targetAssignable { get; private set; }

  public override string GetTitle() => Object.op_Inequality((Object) this.targetAssignable, (Object) null) ? string.Format(base.GetTitle(), (object) ((Component) this.targetAssignable).GetProperName()) : base.GetTitle();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.dupeSortingToggle.onClick += (System.Action) (() => this.SortByName(true));
    this.generalSortingToggle.onClick += (System.Action) (() => this.SortByAssignment(true));
    ((KMonoBehaviour) this).Subscribe(((Component) Game.Instance).gameObject, 875045922, new Action<object>(this.OnRefreshData));
  }

  private void OnRefreshData(object obj) => this.SetTarget(((Component) this.targetAssignable).gameObject);

  public override void ClearTarget()
  {
    if (this.targetAssignableSubscriptionHandle != -1 && Object.op_Inequality((Object) this.targetAssignable, (Object) null))
    {
      this.targetAssignable.Unsubscribe(this.targetAssignableSubscriptionHandle);
      this.targetAssignableSubscriptionHandle = -1;
    }
    this.targetAssignable = (Assignable) null;
    Components.LiveMinionIdentities.OnAdd -= new Action<MinionIdentity>(this.OnMinionIdentitiesChanged);
    Components.LiveMinionIdentities.OnRemove -= new Action<MinionIdentity>(this.OnMinionIdentitiesChanged);
    base.ClearTarget();
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<Assignable>(), (Object) null) && target.GetComponent<Assignable>().CanBeAssigned && Object.op_Equality((Object) target.GetComponent<AssignmentGroupController>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    Components.LiveMinionIdentities.OnAdd += new Action<MinionIdentity>(this.OnMinionIdentitiesChanged);
    Components.LiveMinionIdentities.OnRemove += new Action<MinionIdentity>(this.OnMinionIdentitiesChanged);
    if (this.targetAssignableSubscriptionHandle != -1 && Object.op_Inequality((Object) this.targetAssignable, (Object) null))
      this.targetAssignable.Unsubscribe(this.targetAssignableSubscriptionHandle);
    this.targetAssignable = target.GetComponent<Assignable>();
    if (Object.op_Equality((Object) this.targetAssignable, (Object) null))
    {
      Debug.LogError((object) string.Format("{0} selected has no Assignable component.", (object) target.GetProperName()));
    }
    else
    {
      if (this.rowPool == null)
        this.rowPool = new UIPool<AssignableSideScreenRow>(this.rowPrefab);
      ((Component) this).gameObject.SetActive(true);
      this.identityList = new List<MinionAssignablesProxy>((IEnumerable<MinionAssignablesProxy>) Components.MinionAssignablesProxy.Items);
      this.dupeSortingToggle.ChangeState(0);
      this.generalSortingToggle.ChangeState(0);
      this.activeSortToggle = (MultiToggle) null;
      this.activeSortFunction = (Comparison<IAssignableIdentity>) null;
      if (!this.targetAssignable.CanBeAssigned)
        this.HideScreen(true);
      else
        this.HideScreen(false);
      this.targetAssignableSubscriptionHandle = this.targetAssignable.Subscribe(684616645, new Action<object>(this.OnAssigneeChanged));
      this.Refresh(this.identityList);
      this.SortByAssignment(false);
    }
  }

  private void OnMinionIdentitiesChanged(MinionIdentity change)
  {
    this.identityList = new List<MinionAssignablesProxy>((IEnumerable<MinionAssignablesProxy>) Components.MinionAssignablesProxy.Items);
    this.Refresh(this.identityList);
  }

  private void OnAssigneeChanged(object data = null)
  {
    foreach (KeyValuePair<IAssignableIdentity, AssignableSideScreenRow> identityRow in this.identityRowMap)
      identityRow.Value.Refresh();
  }

  private void Refresh(List<MinionAssignablesProxy> identities)
  {
    this.ClearContent();
    ((TMP_Text) this.currentOwnerText).text = string.Format((string) UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.UNASSIGNED);
    if (Object.op_Equality((Object) this.targetAssignable, (Object) null))
      return;
    if (Object.op_Equality((Object) ((Component) this.targetAssignable).GetComponent<Equippable>(), (Object) null) && !((Component) this.targetAssignable).HasTag(GameTags.NotRoomAssignable))
    {
      Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(((Component) this.targetAssignable).gameObject);
      if (roomOfGameObject != null)
      {
        RoomType roomType = roomOfGameObject.roomType;
        if (roomType.primary_constraint != null && !roomType.primary_constraint.building_criteria(((Component) this.targetAssignable).GetComponent<KPrefabID>()))
        {
          AssignableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
          freeElement.sideScreen = this;
          this.identityRowMap.Add((IAssignableIdentity) roomOfGameObject, freeElement);
          freeElement.SetContent((IAssignableIdentity) roomOfGameObject, new Action<IAssignableIdentity>(this.OnRowClicked), this);
          return;
        }
      }
    }
    if (this.targetAssignable.canBePublic)
    {
      AssignableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
      freeElement.sideScreen = this;
      freeElement.transform.SetAsFirstSibling();
      this.identityRowMap.Add((IAssignableIdentity) Game.Instance.assignmentManager.assignment_groups["public"], freeElement);
      freeElement.SetContent((IAssignableIdentity) Game.Instance.assignmentManager.assignment_groups["public"], new Action<IAssignableIdentity>(this.OnRowClicked), this);
    }
    foreach (MinionAssignablesProxy identity in identities)
    {
      AssignableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
      freeElement.sideScreen = this;
      this.identityRowMap.Add((IAssignableIdentity) identity, freeElement);
      freeElement.SetContent((IAssignableIdentity) identity, new Action<IAssignableIdentity>(this.OnRowClicked), this);
    }
    this.ExecuteSort(this.activeSortFunction);
  }

  private void SortByName(bool reselect)
  {
    this.SelectSortToggle(this.dupeSortingToggle, reselect);
    this.ExecuteSort((Comparison<IAssignableIdentity>) ((i1, i2) => i1.GetProperName().CompareTo(i2.GetProperName()) * (this.sortReversed ? -1 : 1)));
  }

  private void SortByAssignment(bool reselect)
  {
    this.SelectSortToggle(this.generalSortingToggle, reselect);
    this.ExecuteSort((Comparison<IAssignableIdentity>) ((i1, i2) =>
    {
      int num1 = this.targetAssignable.CanAssignTo(i1).CompareTo(this.targetAssignable.CanAssignTo(i2));
      if (num1 != 0)
        return num1 * -1;
      int num2 = this.identityRowMap[i1].currentState.CompareTo((object) this.identityRowMap[i2].currentState);
      return num2 != 0 ? num2 * (this.sortReversed ? -1 : 1) : i1.GetProperName().CompareTo(i2.GetProperName());
    }));
  }

  private void SelectSortToggle(MultiToggle toggle, bool reselect)
  {
    this.dupeSortingToggle.ChangeState(0);
    this.generalSortingToggle.ChangeState(0);
    if (Object.op_Inequality((Object) toggle, (Object) null))
    {
      if (reselect && Object.op_Equality((Object) this.activeSortToggle, (Object) toggle))
        this.sortReversed = !this.sortReversed;
      this.activeSortToggle = toggle;
    }
    this.activeSortToggle.ChangeState(this.sortReversed ? 2 : 1);
  }

  private void ExecuteSort(Comparison<IAssignableIdentity> sortFunction)
  {
    if (sortFunction == null)
      return;
    List<IAssignableIdentity> assignableIdentityList = new List<IAssignableIdentity>((IEnumerable<IAssignableIdentity>) this.identityRowMap.Keys);
    assignableIdentityList.Sort(sortFunction);
    for (int index = 0; index < assignableIdentityList.Count; ++index)
      this.identityRowMap[assignableIdentityList[index]].transform.SetSiblingIndex(index);
    this.activeSortFunction = sortFunction;
  }

  private void ClearContent()
  {
    if (this.rowPool != null)
      this.rowPool.DestroyAll();
    foreach (KeyValuePair<IAssignableIdentity, AssignableSideScreenRow> identityRow in this.identityRowMap)
      identityRow.Value.targetIdentity = (IAssignableIdentity) null;
    this.identityRowMap.Clear();
  }

  private void HideScreen(bool hide)
  {
    if (hide)
    {
      ((KMonoBehaviour) this).transform.localScale = Vector3.zero;
    }
    else
    {
      if (!Vector3.op_Inequality(((KMonoBehaviour) this).transform.localScale, Vector3.one))
        return;
      ((KMonoBehaviour) this).transform.localScale = Vector3.one;
    }
  }

  private void OnRowClicked(IAssignableIdentity identity)
  {
    if (this.targetAssignable.assignee != identity)
    {
      this.ChangeAssignment(identity);
    }
    else
    {
      if (!this.CanDeselect(identity))
        return;
      this.ChangeAssignment((IAssignableIdentity) null);
    }
  }

  private bool CanDeselect(IAssignableIdentity identity) => identity is MinionAssignablesProxy;

  private void ChangeAssignment(IAssignableIdentity new_identity)
  {
    this.targetAssignable.Unassign();
    if (Util.IsNullOrDestroyed((object) new_identity))
      return;
    this.targetAssignable.Assign(new_identity);
  }

  private void OnValidStateChanged(bool state)
  {
    if (!((Component) this).gameObject.activeInHierarchy)
      return;
    this.Refresh(this.identityList);
  }
}
