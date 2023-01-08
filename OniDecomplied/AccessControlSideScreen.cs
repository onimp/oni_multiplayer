// Decompiled with JetBrains decompiler
// Type: AccessControlSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AccessControlSideScreen : SideScreenContent
{
  [SerializeField]
  private AccessControlSideScreenRow rowPrefab;
  [SerializeField]
  private GameObject rowGroup;
  [SerializeField]
  private AccessControlSideScreenDoor defaultsRow;
  [SerializeField]
  private Toggle sortByNameToggle;
  [SerializeField]
  private Toggle sortByPermissionToggle;
  [SerializeField]
  private Toggle sortByRoleToggle;
  [SerializeField]
  private GameObject disabledOverlay;
  [SerializeField]
  private KImage headerBG;
  private AccessControl target;
  private Door doorTarget;
  private UIPool<AccessControlSideScreenRow> rowPool;
  private AccessControlSideScreen.MinionIdentitySort.SortInfo sortInfo = AccessControlSideScreen.MinionIdentitySort.SortInfos[0];
  private Dictionary<MinionAssignablesProxy, AccessControlSideScreenRow> identityRowMap = new Dictionary<MinionAssignablesProxy, AccessControlSideScreenRow>();
  private List<MinionAssignablesProxy> identityList = new List<MinionAssignablesProxy>();

  public override string GetTitle() => Object.op_Inequality((Object) this.target, (Object) null) ? string.Format(base.GetTitle(), (object) ((Component) this.target).GetProperName()) : base.GetTitle();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    // ISSUE: method pointer
    ((UnityEvent<bool>) this.sortByNameToggle.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(\u003COnSpawn\u003Eb__15_0)));
    // ISSUE: method pointer
    ((UnityEvent<bool>) this.sortByRoleToggle.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(\u003COnSpawn\u003Eb__15_1)));
    // ISSUE: method pointer
    ((UnityEvent<bool>) this.sortByPermissionToggle.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(SortByPermission)));
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<AccessControl>(), (Object) null) && target.GetComponent<AccessControl>().controlEnabled;

  public override void SetTarget(GameObject target)
  {
    if (Object.op_Inequality((Object) this.target, (Object) null))
      this.ClearTarget();
    this.target = target.GetComponent<AccessControl>();
    this.doorTarget = target.GetComponent<Door>();
    if (Object.op_Equality((Object) this.target, (Object) null))
      return;
    KMonoBehaviourExtensions.Subscribe(target, 1734268753, new Action<object>(this.OnDoorStateChanged));
    KMonoBehaviourExtensions.Subscribe(target, -1525636549, new Action<object>(this.OnAccessControlChanged));
    if (this.rowPool == null)
      this.rowPool = new UIPool<AccessControlSideScreenRow>(this.rowPrefab);
    ((Component) this).gameObject.SetActive(true);
    this.identityList = new List<MinionAssignablesProxy>((IEnumerable<MinionAssignablesProxy>) Components.MinionAssignablesProxy.Items);
    this.Refresh(this.identityList, true);
  }

  public override void ClearTarget()
  {
    base.ClearTarget();
    if (!Object.op_Inequality((Object) this.target, (Object) null))
      return;
    this.target.Unsubscribe(1734268753, new Action<object>(this.OnDoorStateChanged));
    this.target.Unsubscribe(-1525636549, new Action<object>(this.OnAccessControlChanged));
  }

  private void Refresh(List<MinionAssignablesProxy> identities, bool rebuild)
  {
    Rotatable component = ((Component) this.target).GetComponent<Rotatable>();
    bool rotated = Object.op_Inequality((Object) component, (Object) null) && component.IsRotated;
    this.defaultsRow.SetRotated(rotated);
    this.defaultsRow.SetContent(this.target.DefaultPermission, new Action<MinionAssignablesProxy, AccessControl.Permission>(this.OnDefaultPermissionChanged));
    if (rebuild)
      this.ClearContent();
    foreach (MinionAssignablesProxy identity in identities)
    {
      AccessControlSideScreenRow controlSideScreenRow;
      if (rebuild)
      {
        controlSideScreenRow = this.rowPool.GetFreeElement(this.rowGroup, true);
        this.identityRowMap.Add(identity, controlSideScreenRow);
      }
      else
        controlSideScreenRow = this.identityRowMap[identity];
      AccessControl.Permission setPermission = this.target.GetSetPermission(identity);
      bool isDefault = this.target.IsDefaultPermission(identity);
      controlSideScreenRow.SetRotated(rotated);
      controlSideScreenRow.SetMinionContent(identity, setPermission, isDefault, new Action<MinionAssignablesProxy, AccessControl.Permission>(this.OnPermissionChanged), new Action<MinionAssignablesProxy, bool>(this.OnPermissionDefault));
    }
    this.RefreshOnline();
    this.ContentContainer.SetActive(this.target.controlEnabled);
  }

  private void RefreshOnline()
  {
    bool flag = this.target.Online && (Object.op_Equality((Object) this.doorTarget, (Object) null) || this.doorTarget.CurrentState == Door.ControlState.Auto);
    this.disabledOverlay.SetActive(!flag);
    this.headerBG.ColorState = flag ? (KImage.ColorSelector) 0 : (KImage.ColorSelector) 1;
  }

  private void SortByPermission(bool state) => this.ExecuteSort<int>(this.sortByPermissionToggle, state, (Func<MinionAssignablesProxy, int>) (identity => !this.target.IsDefaultPermission(identity) ? (int) this.target.GetSetPermission(identity) : -1));

  private void ExecuteSort<T>(
    Toggle toggle,
    bool state,
    Func<MinionAssignablesProxy, T> sortFunction,
    bool refresh = false)
  {
    ((Component) toggle).GetComponent<ImageToggleState>().SetActiveState(state);
    if (!state)
      return;
    this.identityList = state ? this.identityList.OrderBy<MinionAssignablesProxy, T>(sortFunction).ToList<MinionAssignablesProxy>() : this.identityList.OrderByDescending<MinionAssignablesProxy, T>(sortFunction).ToList<MinionAssignablesProxy>();
    if (refresh)
    {
      this.Refresh(this.identityList, false);
    }
    else
    {
      for (int index = 0; index < this.identityList.Count; ++index)
      {
        if (this.identityRowMap.ContainsKey(this.identityList[index]))
          this.identityRowMap[this.identityList[index]].transform.SetSiblingIndex(index);
      }
    }
  }

  private void SortEntries(bool reverse_sort, Comparison<MinionAssignablesProxy> compare)
  {
    this.identityList.Sort(compare);
    if (reverse_sort)
      this.identityList.Reverse();
    for (int index = 0; index < this.identityList.Count; ++index)
    {
      if (this.identityRowMap.ContainsKey(this.identityList[index]))
        this.identityRowMap[this.identityList[index]].transform.SetSiblingIndex(index);
    }
  }

  private void ClearContent()
  {
    if (this.rowPool != null)
      this.rowPool.ClearAll();
    this.identityRowMap.Clear();
  }

  private void OnDefaultPermissionChanged(
    MinionAssignablesProxy identity,
    AccessControl.Permission permission)
  {
    this.target.DefaultPermission = permission;
    this.Refresh(this.identityList, false);
    foreach (MinionAssignablesProxy identity1 in this.identityList)
    {
      if (this.target.IsDefaultPermission(identity1))
        this.target.ClearPermission(identity1);
    }
  }

  private void OnPermissionChanged(
    MinionAssignablesProxy identity,
    AccessControl.Permission permission)
  {
    this.target.SetPermission(identity, permission);
  }

  private void OnPermissionDefault(MinionAssignablesProxy identity, bool isDefault)
  {
    if (isDefault)
      this.target.ClearPermission(identity);
    else
      this.target.SetPermission(identity, this.target.DefaultPermission);
    this.Refresh(this.identityList, false);
  }

  private void OnAccessControlChanged(object data) => this.RefreshOnline();

  private void OnDoorStateChanged(object data) => this.RefreshOnline();

  private void OnSelectSortFunc(IListableOption role, object data)
  {
    if (role == null)
      return;
    foreach (AccessControlSideScreen.MinionIdentitySort.SortInfo sortInfo in AccessControlSideScreen.MinionIdentitySort.SortInfos)
    {
      if ((string) sortInfo.name == role.GetProperName())
      {
        this.sortInfo = sortInfo;
        this.identityList.Sort(this.sortInfo.compare);
        for (int index = 0; index < this.identityList.Count; ++index)
        {
          if (this.identityRowMap.ContainsKey(this.identityList[index]))
            this.identityRowMap[this.identityList[index]].transform.SetSiblingIndex(index);
        }
        break;
      }
    }
  }

  private static class MinionIdentitySort
  {
    public static readonly AccessControlSideScreen.MinionIdentitySort.SortInfo[] SortInfos = new AccessControlSideScreen.MinionIdentitySort.SortInfo[2]
    {
      new AccessControlSideScreen.MinionIdentitySort.SortInfo()
      {
        name = STRINGS.UI.MINION_IDENTITY_SORT.NAME,
        compare = new Comparison<MinionAssignablesProxy>(AccessControlSideScreen.MinionIdentitySort.CompareByName)
      },
      new AccessControlSideScreen.MinionIdentitySort.SortInfo()
      {
        name = STRINGS.UI.MINION_IDENTITY_SORT.ROLE,
        compare = new Comparison<MinionAssignablesProxy>(AccessControlSideScreen.MinionIdentitySort.CompareByRole)
      }
    };

    public static int CompareByName(MinionAssignablesProxy a, MinionAssignablesProxy b) => a.GetProperName().CompareTo(b.GetProperName());

    public static int CompareByRole(MinionAssignablesProxy a, MinionAssignablesProxy b)
    {
      Debug.Assert(Object.op_Implicit((Object) a), (object) "a was null");
      Debug.Assert(Object.op_Implicit((Object) b), (object) "b was null");
      GameObject targetGameObject1 = a.GetTargetGameObject();
      GameObject targetGameObject2 = b.GetTargetGameObject();
      MinionResume minionResume1 = Object.op_Implicit((Object) targetGameObject1) ? targetGameObject1.GetComponent<MinionResume>() : (MinionResume) null;
      MinionResume minionResume2 = Object.op_Implicit((Object) targetGameObject2) ? targetGameObject2.GetComponent<MinionResume>() : (MinionResume) null;
      if (Object.op_Equality((Object) minionResume2, (Object) null))
        return 1;
      if (Object.op_Equality((Object) minionResume1, (Object) null))
        return -1;
      int num = minionResume1.CurrentRole.CompareTo(minionResume2.CurrentRole);
      return num != 0 ? num : AccessControlSideScreen.MinionIdentitySort.CompareByName(a, b);
    }

    public class SortInfo : IListableOption
    {
      public LocString name;
      public Comparison<MinionAssignablesProxy> compare;

      public string GetProperName() => (string) this.name;
    }
  }
}
