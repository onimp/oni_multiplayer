// Decompiled with JetBrains decompiler
// Type: RocketClusterDestinationSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RocketClusterDestinationSelector : ClusterDestinationSelector
{
  [Serialize]
  private Dictionary<int, Ref<LaunchPad>> m_launchPad = new Dictionary<int, Ref<LaunchPad>>();
  [Serialize]
  private bool m_repeat;
  [Serialize]
  private AxialI m_prevDestination;
  [Serialize]
  private Ref<LaunchPad> m_prevLaunchPad = new Ref<LaunchPad>();
  [Serialize]
  private bool isHarvesting;
  private EventSystem.IntraObjectHandler<RocketClusterDestinationSelector> OnLaunchDelegate = new EventSystem.IntraObjectHandler<RocketClusterDestinationSelector>((Action<RocketClusterDestinationSelector, object>) ((cmp, data) => cmp.OnLaunch(data)));

  public bool Repeat
  {
    get => this.m_repeat;
    set => this.m_repeat = value;
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<RocketClusterDestinationSelector>(-1277991738, this.OnLaunchDelegate);
  }

  protected virtual void OnSpawn()
  {
    if (!this.isHarvesting)
      return;
    this.WaitForPOIHarvest();
  }

  public LaunchPad GetDestinationPad(AxialI destination)
  {
    int worldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(destination);
    return this.m_launchPad.ContainsKey(worldIdAtLocation) ? this.m_launchPad[worldIdAtLocation].Get() : (LaunchPad) null;
  }

  public LaunchPad GetDestinationPad() => this.GetDestinationPad(this.m_destination);

  public override void SetDestination(AxialI location) => base.SetDestination(location);

  public void SetDestinationPad(LaunchPad pad)
  {
    Debug.Assert(Object.op_Equality((Object) pad, (Object) null) || ClusterGrid.Instance.IsInRange(pad.GetMyWorldLocation(), this.m_destination), (object) "Tried sending a rocket to a launchpad that wasn't its destination world.");
    if (Object.op_Inequality((Object) pad, (Object) null))
    {
      this.AddDestinationPad(pad.GetMyWorldLocation(), pad);
      base.SetDestination(pad.GetMyWorldLocation());
    }
    ((Component) this).GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.ClusterDestinationChanged, (object) null);
  }

  private void AddDestinationPad(AxialI location, LaunchPad pad)
  {
    int worldIdAtLocation = ClusterUtil.GetAsteroidWorldIdAtLocation(location);
    if (worldIdAtLocation < 0)
      return;
    if (!this.m_launchPad.ContainsKey(worldIdAtLocation))
      this.m_launchPad.Add(worldIdAtLocation, new Ref<LaunchPad>());
    this.m_launchPad[worldIdAtLocation].Set(pad);
  }

  protected override void OnClusterLocationChanged(object data)
  {
    ClusterLocationChangedEvent locationChangedEvent = (ClusterLocationChangedEvent) data;
    if (!AxialI.op_Equality(locationChangedEvent.newLocation, this.m_destination))
      return;
    ((Component) this).GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.ClusterDestinationReached, (object) null);
    if (!this.m_repeat)
      return;
    if ((!Object.op_Inequality((Object) ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(locationChangedEvent.newLocation, EntityLayer.POI), (Object) null) ? 0 : (this.CanRocketHarvest() ? 1 : 0)) != 0)
      this.WaitForPOIHarvest();
    else
      this.SetUpReturnTrip();
  }

  private void SetUpReturnTrip()
  {
    this.AddDestinationPad(this.m_prevDestination, this.m_prevLaunchPad.Get());
    this.m_destination = this.m_prevDestination;
    this.m_prevDestination = ((Component) this).GetComponent<Clustercraft>().Location;
    this.m_prevLaunchPad.Set(((Component) this).GetComponent<CraftModuleInterface>().CurrentPad);
  }

  private bool CanRocketHarvest()
  {
    bool flag = false;
    List<ResourceHarvestModule.StatesInstance> resourceHarvestModules = ((Component) this).GetComponent<Clustercraft>().GetAllResourceHarvestModules();
    if (resourceHarvestModules.Count > 0)
    {
      foreach (ResourceHarvestModule.StatesInstance statesInstance in resourceHarvestModules)
      {
        if (statesInstance.CheckIfCanHarvest())
          flag = true;
      }
    }
    if (!flag)
    {
      List<ArtifactHarvestModule.StatesInstance> artifactHarvestModules = ((Component) this).GetComponent<Clustercraft>().GetAllArtifactHarvestModules();
      if (artifactHarvestModules.Count > 0)
      {
        foreach (ArtifactHarvestModule.StatesInstance statesInstance in artifactHarvestModules)
        {
          if (statesInstance.CheckIfCanHarvest())
            flag = true;
        }
      }
    }
    return flag;
  }

  private void OnStorageChange(object data)
  {
    if (this.CanRocketHarvest())
      return;
    this.isHarvesting = false;
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) ((Component) this).GetComponent<Clustercraft>().ModuleInterface.ClusterModules)
    {
      if (Object.op_Implicit((Object) ((Component) clusterModule.Get()).GetComponent<Storage>()))
        this.Unsubscribe(((Component) clusterModule.Get()).gameObject, -1697596308, new Action<object>(this.OnStorageChange));
    }
    this.SetUpReturnTrip();
  }

  private void WaitForPOIHarvest()
  {
    this.isHarvesting = true;
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) ((Component) this).GetComponent<Clustercraft>().ModuleInterface.ClusterModules)
    {
      if (Object.op_Implicit((Object) ((Component) clusterModule.Get()).GetComponent<Storage>()))
        this.Subscribe(((Component) clusterModule.Get()).gameObject, -1697596308, new Action<object>(this.OnStorageChange));
    }
  }

  private void OnLaunch(object data)
  {
    this.m_prevLaunchPad.Set(((Component) this).GetComponent<CraftModuleInterface>().CurrentPad);
    this.m_prevDestination = ((Component) this).GetComponent<Clustercraft>().Location;
  }
}
