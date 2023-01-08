// Decompiled with JetBrains decompiler
// Type: ClustercraftExteriorDoor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

public class ClustercraftExteriorDoor : KMonoBehaviour
{
  public string interiorTemplateName;
  private ClustercraftInteriorDoor targetDoor;
  [Serialize]
  private int targetWorldId = -1;
  private static readonly EventSystem.IntraObjectHandler<ClustercraftExteriorDoor> OnLaunchDelegate = new EventSystem.IntraObjectHandler<ClustercraftExteriorDoor>((Action<ClustercraftExteriorDoor, object>) ((component, data) => component.OnLaunch(data)));
  private static readonly EventSystem.IntraObjectHandler<ClustercraftExteriorDoor> OnLandDelegate = new EventSystem.IntraObjectHandler<ClustercraftExteriorDoor>((Action<ClustercraftExteriorDoor, object>) ((component, data) => component.OnLand(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.targetWorldId < 0)
    {
      GameObject gameObject = ((Component) ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface).gameObject;
      WorldContainer rocketInteriorWorld = ClusterManager.Instance.CreateRocketInteriorWorld(gameObject, this.interiorTemplateName, (System.Action) (() => this.PairWithInteriorDoor()));
      if (Object.op_Inequality((Object) rocketInteriorWorld, (Object) null))
        this.targetWorldId = rocketInteriorWorld.id;
    }
    else
      this.PairWithInteriorDoor();
    this.Subscribe<ClustercraftExteriorDoor>(-1277991738, ClustercraftExteriorDoor.OnLaunchDelegate);
    this.Subscribe<ClustercraftExteriorDoor>(-887025858, ClustercraftExteriorDoor.OnLandDelegate);
  }

  protected virtual void OnCleanUp()
  {
    ClusterManager.Instance.DestoryRocketInteriorWorld(this.targetWorldId, this);
    base.OnCleanUp();
  }

  private void PairWithInteriorDoor()
  {
    foreach (ClustercraftInteriorDoor craftInteriorDoor in Components.ClusterCraftInteriorDoors)
    {
      if (craftInteriorDoor.GetMyWorldId() == this.targetWorldId)
      {
        this.SetTarget(craftInteriorDoor);
        break;
      }
    }
    if (Object.op_Equality((Object) this.targetDoor, (Object) null))
      Debug.LogWarning((object) "No ClusterCraftInteriorDoor found on world");
    WorldContainer targetWorld = this.GetTargetWorld();
    int myWorldId = this.GetMyWorldId();
    if (Object.op_Inequality((Object) targetWorld, (Object) null) && myWorldId != -1)
      targetWorld.SetParentIdx(myWorldId);
    if (((Component) this).gameObject.GetComponent<KSelectable>().IsSelected)
      RocketModuleSideScreen.instance.UpdateButtonStates();
    this.Trigger(-1118736034, (object) null);
    EventExtensions.Trigger(((Component) targetWorld).gameObject, -1118736034, (object) null);
  }

  public void SetTarget(ClustercraftInteriorDoor target)
  {
    this.targetDoor = target;
    ((Component) target).GetComponent<AssignmentGroupController>().SetGroupID(((Component) this).GetComponent<AssignmentGroupController>().AssignmentGroupID);
    ((Component) this).GetComponent<NavTeleporter>().TwoWayTarget(((Component) target).GetComponent<NavTeleporter>());
  }

  public bool HasTargetWorld() => Object.op_Inequality((Object) this.targetDoor, (Object) null);

  public WorldContainer GetTargetWorld()
  {
    Debug.Assert(Object.op_Inequality((Object) this.targetDoor, (Object) null), (object) "Clustercraft Exterior Door has no targetDoor");
    return this.targetDoor.GetMyWorld();
  }

  public void FerryMinion(GameObject minion)
  {
    Vector3 vector3 = Vector3.op_Multiply(Vector3.left, 3f);
    TransformExtensions.SetPosition(minion.transform, Grid.CellToPos(Grid.PosToCell(Vector3.op_Addition(this.targetDoor.transform.position, vector3)), (CellAlignment) 1, Grid.SceneLayer.Move));
    ClusterManager.Instance.MigrateMinion(minion.GetComponent<MinionIdentity>(), this.targetDoor.GetMyWorldId());
  }

  private void OnLaunch(object data)
  {
    ((Component) this).GetComponent<NavTeleporter>().EnableTwoWayTarget(false);
    WorldContainer targetWorld = this.GetTargetWorld();
    if (!Object.op_Inequality((Object) targetWorld, (Object) null))
      return;
    targetWorld.SetParentIdx(targetWorld.id);
  }

  private void OnLand(object data)
  {
    ((Component) this).GetComponent<NavTeleporter>().EnableTwoWayTarget(true);
    WorldContainer targetWorld = this.GetTargetWorld();
    if (!Object.op_Inequality((Object) targetWorld, (Object) null))
      return;
    int myWorldId = this.GetMyWorldId();
    targetWorld.SetParentIdx(myWorldId);
  }

  public int TargetCell() => ((Component) this.targetDoor).GetComponent<NavTeleporter>().GetCell();

  public ClustercraftInteriorDoor GetInteriorDoor() => this.targetDoor;
}
