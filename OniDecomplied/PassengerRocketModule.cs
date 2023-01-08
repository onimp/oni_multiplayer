// Decompiled with JetBrains decompiler
// Type: PassengerRocketModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PassengerRocketModule : KMonoBehaviour
{
  public EventReference interiorReverbSnapshot;
  [Serialize]
  private PassengerRocketModule.RequestCrewState passengersRequested;
  private static readonly EventSystem.IntraObjectHandler<PassengerRocketModule> OnRocketOnGroundTagDelegate = GameUtil.CreateHasTagHandler<PassengerRocketModule>(GameTags.RocketOnGround, (Action<PassengerRocketModule, object>) ((component, data) => component.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Release)));
  private static EventSystem.IntraObjectHandler<PassengerRocketModule> RefreshDelegate = new EventSystem.IntraObjectHandler<PassengerRocketModule>((Action<PassengerRocketModule, object>) ((cmp, data) => cmp.RefreshOrders()));
  private static EventSystem.IntraObjectHandler<PassengerRocketModule> OnLaunchDelegate = new EventSystem.IntraObjectHandler<PassengerRocketModule>((Action<PassengerRocketModule, object>) ((component, data) => component.ClearMinionAssignments(data)));
  private static readonly EventSystem.IntraObjectHandler<PassengerRocketModule> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<PassengerRocketModule>((Action<PassengerRocketModule, object>) ((component, data) => component.OnReachableChanged(data)));

  public PassengerRocketModule.RequestCrewState PassengersRequested => this.passengersRequested;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Game.Instance.Subscribe(-1123234494, new Action<object>(this.OnAssignmentGroupChanged));
    GameUtil.SubscribeToTags<PassengerRocketModule>(this, PassengerRocketModule.OnRocketOnGroundTagDelegate, false);
    this.Subscribe<PassengerRocketModule>(1655598572, PassengerRocketModule.RefreshDelegate);
    this.Subscribe<PassengerRocketModule>(191901966, PassengerRocketModule.RefreshDelegate);
    this.Subscribe<PassengerRocketModule>(-71801987, PassengerRocketModule.RefreshDelegate);
    this.Subscribe<PassengerRocketModule>(-1277991738, PassengerRocketModule.OnLaunchDelegate);
    this.Subscribe<PassengerRocketModule>(-1432940121, PassengerRocketModule.OnReachableChangedDelegate);
    new ReachabilityMonitor.Instance(((Component) this).GetComponent<Workable>()).StartSM();
  }

  protected virtual void OnCleanUp()
  {
    Game.Instance.Unsubscribe(-1123234494, new Action<object>(this.OnAssignmentGroupChanged));
    base.OnCleanUp();
  }

  private void OnAssignmentGroupChanged(object data) => this.RefreshOrders();

  private void OnReachableChanged(object data)
  {
    int num = (bool) data ? 1 : 0;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (num != 0)
      component.RemoveStatusItem(Db.Get().BuildingStatusItems.PassengerModuleUnreachable);
    else
      component.AddStatusItem(Db.Get().BuildingStatusItems.PassengerModuleUnreachable, (object) this);
  }

  public void RequestCrewBoard(
    PassengerRocketModule.RequestCrewState requestBoard)
  {
    this.passengersRequested = requestBoard;
    this.RefreshOrders();
  }

  public bool ShouldCrewGetIn()
  {
    CraftModuleInterface craftInterface = ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface;
    if (this.passengersRequested == PassengerRocketModule.RequestCrewState.Request)
      return true;
    return craftInterface.IsLaunchRequested() && craftInterface.CheckPreppedForLaunch();
  }

  private void RefreshOrders()
  {
    if (!((Component) this).HasTag(GameTags.RocketOnGround) || !((Component) this).GetComponent<ClustercraftExteriorDoor>().HasTargetWorld())
      return;
    int cell = ((Component) this).GetComponent<NavTeleporter>().GetCell();
    int index = ((Component) this).GetComponent<ClustercraftExteriorDoor>().TargetCell();
    bool restrict = this.ShouldCrewGetIn();
    if (restrict)
    {
      foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
      {
        bool flag1 = Game.Instance.assignmentManager.assignment_groups[((Component) this).GetComponent<AssignmentGroupController>().AssignmentGroupID].HasMember((IAssignableIdentity) minionIdentity.assignableProxy.Get());
        bool flag2 = minionIdentity.GetMyWorldId() == (int) Grid.WorldIdx[index];
        if (!flag2 & flag1)
          ((Component) minionIdentity).GetSMI<RocketPassengerMonitor.Instance>().SetMoveTarget(index);
        else if (flag2 && !flag1)
          ((Component) minionIdentity).GetSMI<RocketPassengerMonitor.Instance>().SetMoveTarget(cell);
        else
          ((Component) minionIdentity).GetSMI<RocketPassengerMonitor.Instance>().ClearMoveTarget(index);
      }
    }
    else
    {
      foreach (MinionIdentity cmp in Components.LiveMinionIdentities.Items)
      {
        ((Component) cmp).GetSMI<RocketPassengerMonitor.Instance>().ClearMoveTarget(cell);
        ((Component) cmp).GetSMI<RocketPassengerMonitor.Instance>().ClearMoveTarget(index);
      }
    }
    for (int idx = 0; idx < Components.LiveMinionIdentities.Count; ++idx)
      this.RefreshAccessStatus(Components.LiveMinionIdentities[idx], restrict);
  }

  private void RefreshAccessStatus(MinionIdentity minion, bool restrict)
  {
    ClustercraftInteriorDoor interiorDoor = ((Component) this).GetComponent<ClustercraftExteriorDoor>().GetInteriorDoor();
    AccessControl component1 = ((Component) this).GetComponent<AccessControl>();
    AccessControl component2 = ((Component) interiorDoor).GetComponent<AccessControl>();
    if (restrict)
    {
      if (Game.Instance.assignmentManager.assignment_groups[((Component) this).GetComponent<AssignmentGroupController>().AssignmentGroupID].HasMember((IAssignableIdentity) minion.assignableProxy.Get()))
      {
        component1.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Both);
        component2.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Neither);
      }
      else
      {
        component1.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Neither);
        component2.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Both);
      }
    }
    else
    {
      component1.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Both);
      component2.SetPermission(minion.assignableProxy.Get(), AccessControl.Permission.Both);
    }
  }

  public bool CheckPilotBoarded()
  {
    ICollection<IAssignableIdentity> members = (ICollection<IAssignableIdentity>) ((Component) this).GetComponent<AssignmentGroupController>().GetMembers();
    if (members.Count == 0)
      return false;
    List<IAssignableIdentity> assignableIdentityList = new List<IAssignableIdentity>();
    foreach (IAssignableIdentity assignableIdentity in (IEnumerable<IAssignableIdentity>) members)
    {
      MinionAssignablesProxy assignablesProxy = (MinionAssignablesProxy) assignableIdentity;
      if (Object.op_Inequality((Object) assignablesProxy, (Object) null))
      {
        MinionResume component = assignablesProxy.GetTargetGameObject().GetComponent<MinionResume>();
        if (Object.op_Inequality((Object) component, (Object) null) && component.HasPerk(Db.Get().SkillPerks.CanUseRocketControlStation))
          assignableIdentityList.Add(assignableIdentity);
      }
    }
    if (assignableIdentityList.Count == 0)
      return false;
    foreach (MinionAssignablesProxy assignablesProxy in assignableIdentityList)
    {
      if (assignablesProxy.GetTargetGameObject().GetMyWorldId() == (int) Grid.WorldIdx[((Component) this).GetComponent<ClustercraftExteriorDoor>().TargetCell()])
        return true;
    }
    return false;
  }

  public Tuple<int, int> GetCrewBoardedFraction()
  {
    ICollection<IAssignableIdentity> members = (ICollection<IAssignableIdentity>) ((Component) this).GetComponent<AssignmentGroupController>().GetMembers();
    if (members.Count == 0)
      return new Tuple<int, int>(0, 0);
    int num = 0;
    foreach (MinionAssignablesProxy assignablesProxy in (IEnumerable<IAssignableIdentity>) members)
    {
      if (assignablesProxy.GetTargetGameObject().GetMyWorldId() != (int) Grid.WorldIdx[((Component) this).GetComponent<ClustercraftExteriorDoor>().TargetCell()])
        ++num;
    }
    return new Tuple<int, int>(members.Count - num, members.Count);
  }

  public bool CheckPassengersBoarded()
  {
    ICollection<IAssignableIdentity> members = (ICollection<IAssignableIdentity>) ((Component) this).GetComponent<AssignmentGroupController>().GetMembers();
    if (members.Count == 0)
      return false;
    bool flag = false;
    foreach (MinionAssignablesProxy assignablesProxy in (IEnumerable<IAssignableIdentity>) members)
    {
      if (Object.op_Inequality((Object) assignablesProxy, (Object) null))
      {
        MinionResume component = assignablesProxy.GetTargetGameObject().GetComponent<MinionResume>();
        if (Object.op_Inequality((Object) component, (Object) null) && component.HasPerk(Db.Get().SkillPerks.CanUseRocketControlStation))
        {
          flag = true;
          break;
        }
      }
    }
    if (!flag)
      return false;
    foreach (MinionAssignablesProxy assignablesProxy in (IEnumerable<IAssignableIdentity>) members)
    {
      if (assignablesProxy.GetTargetGameObject().GetMyWorldId() != (int) Grid.WorldIdx[((Component) this).GetComponent<ClustercraftExteriorDoor>().TargetCell()])
        return false;
    }
    return true;
  }

  public bool CheckExtraPassengers()
  {
    ClustercraftExteriorDoor component = ((Component) this).GetComponent<ClustercraftExteriorDoor>();
    if (component.HasTargetWorld())
    {
      byte worldId = Grid.WorldIdx[component.TargetCell()];
      List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems((int) worldId);
      for (int index = 0; index < worldItems.Count; ++index)
      {
        if (!Game.Instance.assignmentManager.assignment_groups[((Component) this).GetComponent<AssignmentGroupController>().AssignmentGroupID].HasMember((IAssignableIdentity) worldItems[index].assignableProxy.Get()))
          return true;
      }
    }
    return false;
  }

  public void RemoveRocketPassenger(MinionIdentity minion)
  {
    if (!Object.op_Inequality((Object) minion, (Object) null))
      return;
    string assignmentGroupId = ((Component) this).GetComponent<AssignmentGroupController>().AssignmentGroupID;
    MinionAssignablesProxy member = minion.assignableProxy.Get();
    if (Game.Instance.assignmentManager.assignment_groups[assignmentGroupId].HasMember((IAssignableIdentity) member))
      Game.Instance.assignmentManager.assignment_groups[assignmentGroupId].RemoveMember((IAssignableIdentity) member);
    this.RefreshOrders();
  }

  public void ClearMinionAssignments(object data)
  {
    foreach (IAssignableIdentity member in Game.Instance.assignmentManager.assignment_groups[((Component) this).GetComponent<AssignmentGroupController>().AssignmentGroupID].GetMembers())
      Game.Instance.assignmentManager.RemoveFromWorld(member, this.GetMyWorldId());
  }

  public enum RequestCrewState
  {
    Release,
    Request,
  }
}
