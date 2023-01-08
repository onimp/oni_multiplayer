// Decompiled with JetBrains decompiler
// Type: RocketModuleCluster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class RocketModuleCluster : RocketModule
{
  public RocketModulePerformance performanceStats;
  private static readonly EventSystem.IntraObjectHandler<RocketModuleCluster> OnNewConstructionDelegate = new EventSystem.IntraObjectHandler<RocketModuleCluster>((Action<RocketModuleCluster, object>) ((component, data) => component.OnNewConstruction(data)));
  private static readonly EventSystem.IntraObjectHandler<RocketModuleCluster> OnLaunchConditionChangedDelegate = new EventSystem.IntraObjectHandler<RocketModuleCluster>((Action<RocketModuleCluster, object>) ((component, data) => component.OnLaunchConditionChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<RocketModuleCluster> OnLandDelegate = new EventSystem.IntraObjectHandler<RocketModuleCluster>((Action<RocketModuleCluster, object>) ((component, data) => component.OnLand(data)));
  private CraftModuleInterface _craftInterface;

  public CraftModuleInterface CraftInterface
  {
    get => this._craftInterface;
    set
    {
      this._craftInterface = value;
      if (!Object.op_Inequality((Object) this._craftInterface, (Object) null))
        return;
      ((Object) this).name = ((Object) this).name + ": " + this.GetParentRocketName();
    }
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<RocketModuleCluster>(2121280625, RocketModuleCluster.OnNewConstructionDelegate);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Equality((Object) this.CraftInterface, (Object) null) && DlcManager.FeatureClusterSpaceEnabled())
      this.RegisterWithCraftModuleInterface();
    if (!Object.op_Equality((Object) ((Component) this).GetComponent<RocketEngine>(), (Object) null) || !Object.op_Equality((Object) ((Component) this).GetComponent<RocketEngineCluster>(), (Object) null) || !Object.op_Equality((Object) ((Component) this).GetComponent<BuildingUnderConstruction>(), (Object) null))
      return;
    this.Subscribe<RocketModuleCluster>(1655598572, RocketModuleCluster.OnLaunchConditionChangedDelegate);
    this.Subscribe<RocketModuleCluster>(-887025858, RocketModuleCluster.OnLandDelegate);
  }

  protected void OnNewConstruction(object data)
  {
    Constructable constructable = (Constructable) data;
    if (Object.op_Equality((Object) constructable, (Object) null))
      return;
    RocketModuleCluster component = ((Component) constructable).GetComponent<RocketModuleCluster>();
    if (Object.op_Equality((Object) component, (Object) null) || !Object.op_Inequality((Object) component.CraftInterface, (Object) null))
      return;
    component.CraftInterface.AddModule(this);
  }

  private void RegisterWithCraftModuleInterface()
  {
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this).GetComponent<AttachableBuilding>()))
    {
      if (!Object.op_Equality((Object) gameObject, (Object) ((Component) this).gameObject))
      {
        RocketModuleCluster component = gameObject.GetComponent<RocketModuleCluster>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          component.CraftInterface.AddModule(this);
          break;
        }
      }
    }
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    this.CraftInterface.RemoveModule(this);
  }

  public override LaunchConditionManager FindLaunchConditionManager() => this.CraftInterface.FindLaunchConditionManager();

  public override string GetParentRocketName() => Object.op_Inequality((Object) this.CraftInterface, (Object) null) ? ((Component) this.CraftInterface).GetComponent<Clustercraft>().Name : this.parentRocketName;

  private void OnLaunchConditionChanged(object data) => this.UpdateAnimations();

  private void OnLand(object data) => this.UpdateAnimations();

  protected void UpdateAnimations()
  {
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    Clustercraft clustercraft = Object.op_Equality((Object) this.CraftInterface, (Object) null) ? (Clustercraft) null : ((Component) this.CraftInterface).GetComponent<Clustercraft>();
    if (Object.op_Inequality((Object) clustercraft, (Object) null) && clustercraft.Status == Clustercraft.CraftStatus.Launching && component.HasAnimation(HashedString.op_Implicit("launch")))
    {
      component.ClearQueue();
      if (component.HasAnimation(HashedString.op_Implicit("launch_pre")))
        component.Play(HashedString.op_Implicit("launch_pre"));
      component.Queue(HashedString.op_Implicit("launch"), (KAnim.PlayMode) 0);
    }
    else if (Object.op_Inequality((Object) this.CraftInterface, (Object) null) && this.CraftInterface.CheckPreppedForLaunch())
    {
      component.initialAnim = "ready_to_launch";
      component.Play(HashedString.op_Implicit("pre_ready_to_launch"));
      component.Queue(HashedString.op_Implicit("ready_to_launch"), (KAnim.PlayMode) 0);
    }
    else
    {
      component.initialAnim = "grounded";
      component.Play(HashedString.op_Implicit("pst_ready_to_launch"));
      component.Queue(HashedString.op_Implicit("grounded"), (KAnim.PlayMode) 0);
    }
  }
}
