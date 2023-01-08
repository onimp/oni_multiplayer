// Decompiled with JetBrains decompiler
// Type: AssignmentGroupControllerSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssignmentGroupControllerSideScreen : KScreen
{
  [SerializeField]
  private GameObject header;
  [SerializeField]
  private GameObject minionRowPrefab;
  [SerializeField]
  private GameObject footer;
  [SerializeField]
  private GameObject minionRowContainer;
  private AssignmentGroupController target;
  private List<GameObject> identityRowMap = new List<GameObject>();

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (!show)
      return;
    this.RefreshRows();
  }

  protected virtual void OnCmpDisable()
  {
    for (int index = 0; index < this.identityRowMap.Count; ++index)
      Object.Destroy((Object) this.identityRowMap[index]);
    this.identityRowMap.Clear();
    ((KMonoBehaviour) this).OnCmpDisable();
  }

  public void SetTarget(GameObject target)
  {
    this.target = target.GetComponent<AssignmentGroupController>();
    this.RefreshRows();
  }

  private void RefreshRows()
  {
    int index1 = 0;
    WorldContainer myWorld1 = this.target.GetMyWorld();
    ClustercraftExteriorDoor component1 = ((Component) this.target).GetComponent<ClustercraftExteriorDoor>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      myWorld1 = component1.GetInteriorDoor().GetMyWorld();
    List<AssignmentGroupControllerSideScreen.RowSortHelper> rowSortHelperList = new List<AssignmentGroupControllerSideScreen.RowSortHelper>();
    for (int idx = 0; idx < Components.MinionAssignablesProxy.Count; ++idx)
    {
      MinionAssignablesProxy assignablesProxy = Components.MinionAssignablesProxy[idx];
      GameObject targetGameObject = assignablesProxy.GetTargetGameObject();
      WorldContainer myWorld2 = targetGameObject.GetMyWorld();
      if (!Object.op_Equality((Object) targetGameObject, (Object) null) && !targetGameObject.HasTag(GameTags.Dead))
      {
        MinionResume component2 = assignablesProxy.GetTargetGameObject().GetComponent<MinionResume>();
        bool flag1 = false;
        if (Object.op_Inequality((Object) component2, (Object) null) && component2.HasPerk(Db.Get().SkillPerks.CanUseRocketControlStation))
          flag1 = true;
        bool flag2 = myWorld2.ParentWorldId == myWorld1.ParentWorldId;
        rowSortHelperList.Add(new AssignmentGroupControllerSideScreen.RowSortHelper()
        {
          minion = assignablesProxy,
          isPilot = flag1,
          isSameWorld = flag2
        });
      }
    }
    rowSortHelperList.Sort((Comparison<AssignmentGroupControllerSideScreen.RowSortHelper>) ((a, b) =>
    {
      int num = b.isSameWorld.CompareTo(a.isSameWorld);
      return num != 0 ? num : b.isPilot.CompareTo(a.isPilot);
    }));
    foreach (AssignmentGroupControllerSideScreen.RowSortHelper rowSortHelper in rowSortHelperList)
    {
      MinionAssignablesProxy minion = rowSortHelper.minion;
      GameObject gameObject;
      if (index1 >= this.identityRowMap.Count)
      {
        gameObject = Util.KInstantiateUI(this.minionRowPrefab, this.minionRowContainer, true);
        this.identityRowMap.Add(gameObject);
      }
      else
      {
        gameObject = this.identityRowMap[index1];
        gameObject.SetActive(true);
      }
      ++index1;
      HierarchyReferences component3 = gameObject.GetComponent<HierarchyReferences>();
      MultiToggle toggle = component3.GetReference<MultiToggle>("Toggle");
      toggle.ChangeState(this.target.CheckMinionIsMember(minion) ? 1 : 0);
      component3.GetReference<CrewPortrait>("Portrait").SetIdentityObject((IAssignableIdentity) minion, false);
      LocText reference1 = component3.GetReference<LocText>("Label");
      LocText reference2 = component3.GetReference<LocText>("Designation");
      if (rowSortHelper.isSameWorld)
      {
        if (rowSortHelper.isPilot)
          ((TMP_Text) reference2).text = (string) STRINGS.UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.PILOT;
        else
          ((TMP_Text) reference2).text = "";
        ((Graphic) reference1).color = Color.black;
        ((Graphic) reference2).color = Color.black;
      }
      else
      {
        ((Graphic) reference1).color = Color.grey;
        ((Graphic) reference2).color = Color.grey;
        ((TMP_Text) reference2).text = (string) STRINGS.UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.OFFWORLD;
        gameObject.transform.SetAsLastSibling();
      }
      toggle.onClick = (System.Action) (() =>
      {
        this.target.SetMember(minion, !this.target.CheckMinionIsMember(minion));
        toggle.ChangeState(this.target.CheckMinionIsMember(minion) ? 1 : 0);
        this.RefreshRows();
      });
      string str = this.UpdateToolTip(minion, !rowSortHelper.isSameWorld);
      ((Component) toggle).GetComponent<ToolTip>().SetSimpleTooltip(str);
    }
    for (int index2 = index1; index2 < this.identityRowMap.Count; ++index2)
      this.identityRowMap[index2].SetActive(false);
    this.minionRowContainer.GetComponent<QuickLayout>().ForceUpdate();
  }

  private string UpdateToolTip(MinionAssignablesProxy minion, bool offworld)
  {
    string str = (string) (this.target.CheckMinionIsMember(minion) ? STRINGS.UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TOOLTIPS.UNASSIGN : STRINGS.UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TOOLTIPS.ASSIGN);
    if (offworld)
      str = str + "\n\n" + UIConstants.ColorPrefixYellow + (string) STRINGS.UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TOOLTIPS.DIFFERENT_WORLD + UIConstants.ColorSuffix;
    return str;
  }

  private struct RowSortHelper
  {
    public MinionAssignablesProxy minion;
    public bool isPilot;
    public bool isSameWorld;
  }
}
