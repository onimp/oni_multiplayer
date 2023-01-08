// Decompiled with JetBrains decompiler
// Type: Clustercraft
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Clustercraft : ClusterGridEntity, IClusterRange, ISim4000ms, ISim1000ms
{
  [Serialize]
  private string m_name;
  [MyCmpReq]
  private ClusterTraveler m_clusterTraveler;
  [MyCmpReq]
  private CraftModuleInterface m_moduleInterface;
  private Guid mainStatusHandle;
  private Guid cargoStatusHandle;
  private Guid missionControlStatusHandle = Guid.Empty;
  public static Dictionary<Tag, float> dlc1OxidizerEfficiencies;
  [Serialize]
  [Range(0.0f, 1f)]
  public float AutoPilotMultiplier = 1f;
  [Serialize]
  [Range(0.0f, 2f)]
  public float PilotSkillMultiplier = 1f;
  [Serialize]
  public float controlStationBuffTimeRemaining;
  [Serialize]
  private bool m_launchRequested;
  [Serialize]
  private Clustercraft.CraftStatus status;
  [MyCmpGet]
  private KSelectable selectable;
  private static EventSystem.IntraObjectHandler<Clustercraft> RocketModuleChangedHandler;
  private static EventSystem.IntraObjectHandler<Clustercraft> ClusterDestinationChangedHandler;
  private static EventSystem.IntraObjectHandler<Clustercraft> ClusterDestinationReachedHandler;
  private static EventSystem.IntraObjectHandler<Clustercraft> NameChangedHandler;

  public override string Name => this.m_name;

  public bool Exploding { get; protected set; }

  public override EntityLayer Layer => EntityLayer.Craft;

  public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
  {
    new ClusterGridEntity.AnimConfig()
    {
      animFile = Assets.GetAnim(HashedString.op_Implicit("rocket01_kanim")),
      initialAnim = "idle_loop"
    }
  };

  public override Sprite GetUISprite() => Def.GetUISprite((object) ((Component) this.m_moduleInterface.GetPassengerModule()).gameObject).first;

  public override bool IsVisible => !this.Exploding;

  public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;

  public override bool SpaceOutInSameHex() => true;

  public CraftModuleInterface ModuleInterface => this.m_moduleInterface;

  public AxialI Destination => this.m_moduleInterface.GetClusterDestinationSelector().GetDestination();

  public float Speed
  {
    get
    {
      float num = this.EnginePower / this.TotalBurden;
      float speed = num * this.AutoPilotMultiplier * this.PilotSkillMultiplier;
      if ((double) this.controlStationBuffTimeRemaining > 0.0)
        speed += num * 0.200000048f;
      return speed;
    }
  }

  public float EnginePower
  {
    get
    {
      float enginePower = 0.0f;
      foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
        enginePower += clusterModule.Get().performanceStats.EnginePower;
      return enginePower;
    }
  }

  public float FuelPerDistance
  {
    get
    {
      float fuelPerDistance = 0.0f;
      foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
        fuelPerDistance += clusterModule.Get().performanceStats.FuelKilogramPerDistance;
      return fuelPerDistance;
    }
  }

  public float TotalBurden
  {
    get
    {
      float totalBurden = 0.0f;
      foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
        totalBurden += clusterModule.Get().performanceStats.Burden;
      Debug.Assert((double) totalBurden > 0.0);
      return totalBurden;
    }
  }

  public bool LaunchRequested
  {
    get => this.m_launchRequested;
    private set
    {
      this.m_launchRequested = value;
      this.m_moduleInterface.TriggerEventOnCraftAndRocket(GameHashes.RocketRequestLaunch, (object) this);
    }
  }

  public Clustercraft.CraftStatus Status => this.status;

  public void SetCraftStatus(Clustercraft.CraftStatus craft_status)
  {
    this.status = craft_status;
    this.UpdateGroundTags();
  }

  public void SetExploding() => this.Exploding = true;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Components.Clustercrafts.Add(this);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.m_clusterTraveler.getSpeedCB = new Func<float>(this.GetSpeed);
    this.m_clusterTraveler.getCanTravelCB = new Func<bool, bool>(this.CanTravel);
    this.m_clusterTraveler.onTravelCB = new System.Action(this.BurnFuelForTravel);
    this.m_clusterTraveler.validateTravelCB = new Func<AxialI, bool>(this.CanTravelToCell);
    this.UpdateGroundTags();
    this.Subscribe<Clustercraft>(1512695988, Clustercraft.RocketModuleChangedHandler);
    this.Subscribe<Clustercraft>(543433792, Clustercraft.ClusterDestinationChangedHandler);
    this.Subscribe<Clustercraft>(1796608350, Clustercraft.ClusterDestinationReachedHandler);
    this.Subscribe(-688990705, (Action<object>) (o => this.UpdateStatusItem()));
    this.Subscribe<Clustercraft>(1102426921, Clustercraft.NameChangedHandler);
    this.SetRocketName(this.m_name);
    this.UpdateStatusItem();
  }

  public void Sim1000ms(float dt)
  {
    this.controlStationBuffTimeRemaining = Mathf.Max(this.controlStationBuffTimeRemaining - dt, 0.0f);
    if ((double) this.controlStationBuffTimeRemaining > 0.0)
    {
      this.missionControlStatusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.MissionControlBoosted, (object) this);
    }
    else
    {
      this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.MissionControlBoosted);
      this.missionControlStatusHandle = Guid.Empty;
    }
  }

  public void Sim4000ms(float dt)
  {
    RocketClusterDestinationSelector destinationSelector = this.m_moduleInterface.GetClusterDestinationSelector();
    if (this.Status != Clustercraft.CraftStatus.InFlight || !AxialI.op_Equality(this.m_location, destinationSelector.GetDestination()))
      return;
    this.OnClusterDestinationReached((object) null);
  }

  public void Init(AxialI location, LaunchPad pad)
  {
    this.m_location = location;
    ((Component) this).GetComponent<RocketClusterDestinationSelector>().SetDestination(this.m_location);
    this.SetRocketName(GameUtil.GenerateRandomRocketName());
    if (Object.op_Inequality((Object) pad, (Object) null))
      this.Land(pad, true);
    this.UpdateStatusItem();
  }

  protected override void OnCleanUp()
  {
    Components.Clustercrafts.Remove(this);
    base.OnCleanUp();
  }

  private bool CanTravel(bool tryingToLand)
  {
    if (!((Component) this).HasTag(GameTags.RocketInSpace))
      return false;
    return tryingToLand || this.HasResourcesToMove();
  }

  private bool CanTravelToCell(AxialI location) => !Object.op_Inequality((Object) ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(location, EntityLayer.Asteroid), (Object) null) || this.CanLandAtAsteroid(location, true);

  private float GetSpeed() => this.Speed;

  private void RocketModuleChanged(object data)
  {
    RocketModuleCluster rocketModuleCluster = (RocketModuleCluster) data;
    if (!Object.op_Inequality((Object) rocketModuleCluster, (Object) null))
      return;
    this.UpdateGroundTags(((Component) rocketModuleCluster).gameObject);
  }

  private void OnClusterDestinationChanged(object data) => this.UpdateStatusItem();

  private void OnClusterDestinationReached(object data)
  {
    RocketClusterDestinationSelector destinationSelector = this.m_moduleInterface.GetClusterDestinationSelector();
    Debug.Assert(AxialI.op_Equality(this.Location, destinationSelector.GetDestination()));
    if (destinationSelector.HasAsteroidDestination())
      this.Land(this.Location, destinationSelector.GetDestinationPad());
    this.UpdateStatusItem();
  }

  public void SetRocketName(object newName) => this.SetRocketName((string) newName);

  public void SetRocketName(string newName)
  {
    this.m_name = newName;
    ((Object) this).name = "Clustercraft: " + newName;
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
    {
      CharacterOverlay component = ((Component) clusterModule.Get()).GetComponent<CharacterOverlay>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        NameDisplayScreen.Instance.UpdateName(((Component) component).gameObject);
        break;
      }
    }
    ClusterManager.Instance.Trigger(1943181844, (object) newName);
  }

  public bool CheckPreppedForLaunch() => this.m_moduleInterface.CheckPreppedForLaunch();

  public bool CheckReadyToLaunch() => this.m_moduleInterface.CheckReadyToLaunch();

  public bool IsFlightInProgress() => this.Status == Clustercraft.CraftStatus.InFlight && this.m_clusterTraveler.IsTraveling();

  public ClusterGridEntity GetPOIAtCurrentLocation() => this.status != Clustercraft.CraftStatus.InFlight || this.IsFlightInProgress() ? (ClusterGridEntity) null : ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(this.m_location, EntityLayer.POI);

  public ClusterGridEntity GetStableOrbitAsteroid() => this.status != Clustercraft.CraftStatus.InFlight || this.IsFlightInProgress() ? (ClusterGridEntity) null : ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(this.m_location, EntityLayer.Asteroid);

  public ClusterGridEntity GetOrbitAsteroid() => this.status != Clustercraft.CraftStatus.InFlight ? (ClusterGridEntity) null : ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(this.m_location, EntityLayer.Asteroid);

  public ClusterGridEntity GetAdjacentAsteroid() => ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(this.m_location, EntityLayer.Asteroid);

  private bool CheckDesinationInRange() => this.m_clusterTraveler.CurrentPath != null && (double) this.Speed * (double) this.m_clusterTraveler.TravelETA() <= (double) this.ModuleInterface.Range;

  public bool HasResourcesToMove(int hexes = 1, Clustercraft.CombustionResource combustionResource = Clustercraft.CombustionResource.All)
  {
    switch (combustionResource)
    {
      case Clustercraft.CombustionResource.Fuel:
        return (double) this.m_moduleInterface.FuelRemaining / (double) this.FuelPerDistance >= 600.0 * (double) hexes - 1.0 / 1000.0;
      case Clustercraft.CombustionResource.Oxidizer:
        return (double) this.m_moduleInterface.OxidizerPowerRemaining / (double) this.FuelPerDistance >= 600.0 * (double) hexes - 1.0 / 1000.0;
      case Clustercraft.CombustionResource.All:
        return (double) this.m_moduleInterface.BurnableMassRemaining / (double) this.FuelPerDistance >= 600.0 * (double) hexes - 1.0 / 1000.0;
      default:
        return false;
    }
  }

  private void BurnFuelForTravel()
  {
    float attemptTravelAmount = 600f;
    foreach (Ref<RocketModuleCluster> clusterModule1 in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
    {
      RocketEngineCluster component1 = ((Component) clusterModule1.Get()).GetComponent<RocketEngineCluster>();
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        Tag fuelTag = component1.fuelTag;
        float totalOxidizerRemaining = 0.0f;
        if (component1.requireOxidizer)
          totalOxidizerRemaining = this.ModuleInterface.OxidizerPowerRemaining;
        if ((double) attemptTravelAmount > 0.0)
        {
          foreach (Ref<RocketModuleCluster> clusterModule2 in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
          {
            IFuelTank component2 = ((Component) clusterModule2.Get()).GetComponent<IFuelTank>();
            if (!Util.IsNullOrDestroyed((object) component2))
              attemptTravelAmount -= this.BurnFromTank(attemptTravelAmount, component1, fuelTag, component2.Storage, ref totalOxidizerRemaining);
            if ((double) attemptTravelAmount <= 0.0)
              break;
          }
        }
      }
    }
    this.UpdateStatusItem();
  }

  private float BurnFromTank(
    float attemptTravelAmount,
    RocketEngineCluster engine,
    Tag fuelTag,
    IStorage storage,
    ref float totalOxidizerRemaining)
  {
    float num1 = attemptTravelAmount * ((Component) engine).GetComponent<RocketModuleCluster>().performanceStats.FuelKilogramPerDistance;
    float num2 = Mathf.Min(storage.GetAmountAvailable(fuelTag), num1);
    if (engine.requireOxidizer)
      num2 = Mathf.Min(num2, totalOxidizerRemaining);
    storage.ConsumeIgnoringDisease(fuelTag, num2);
    if (engine.requireOxidizer)
    {
      this.BurnOxidizer(num2);
      totalOxidizerRemaining -= num2;
    }
    return num2 / ((Component) engine).GetComponent<RocketModuleCluster>().performanceStats.FuelKilogramPerDistance;
  }

  private void BurnOxidizer(float fuelEquivalentKGs)
  {
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
    {
      OxidizerTank component = ((Component) clusterModule.Get()).GetComponent<OxidizerTank>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        foreach (KeyValuePair<Tag, float> keyValuePair in component.GetOxidizersAvailable())
        {
          float oxidizerEfficiency = Clustercraft.dlc1OxidizerEfficiencies[keyValuePair.Key];
          float amount = Mathf.Min(fuelEquivalentKGs / oxidizerEfficiency, keyValuePair.Value);
          if ((double) amount > 0.0)
          {
            component.storage.ConsumeIgnoringDisease(keyValuePair.Key, amount);
            fuelEquivalentKGs -= amount * oxidizerEfficiency;
          }
        }
      }
      if ((double) fuelEquivalentKGs <= 0.0)
        break;
    }
  }

  public List<ResourceHarvestModule.StatesInstance> GetAllResourceHarvestModules()
  {
    List<ResourceHarvestModule.StatesInstance> resourceHarvestModules = new List<ResourceHarvestModule.StatesInstance>();
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
    {
      ResourceHarvestModule.StatesInstance smi = ((Component) clusterModule.Get()).GetSMI<ResourceHarvestModule.StatesInstance>();
      if (smi != null)
        resourceHarvestModules.Add(smi);
    }
    return resourceHarvestModules;
  }

  public List<ArtifactHarvestModule.StatesInstance> GetAllArtifactHarvestModules()
  {
    List<ArtifactHarvestModule.StatesInstance> artifactHarvestModules = new List<ArtifactHarvestModule.StatesInstance>();
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
    {
      ArtifactHarvestModule.StatesInstance smi = ((Component) clusterModule.Get()).GetSMI<ArtifactHarvestModule.StatesInstance>();
      if (smi != null)
        artifactHarvestModules.Add(smi);
    }
    return artifactHarvestModules;
  }

  public List<CargoBayCluster> GetAllCargoBays()
  {
    List<CargoBayCluster> allCargoBays = new List<CargoBayCluster>();
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
    {
      CargoBayCluster component = ((Component) clusterModule.Get()).GetComponent<CargoBayCluster>();
      if (Object.op_Inequality((Object) component, (Object) null))
        allCargoBays.Add(component);
    }
    return allCargoBays;
  }

  public List<CargoBayCluster> GetCargoBaysOfType(CargoBay.CargoType cargoType)
  {
    List<CargoBayCluster> cargoBaysOfType = new List<CargoBayCluster>();
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules)
    {
      CargoBayCluster component = ((Component) clusterModule.Get()).GetComponent<CargoBayCluster>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.storageType == cargoType)
        cargoBaysOfType.Add(component);
    }
    return cargoBaysOfType;
  }

  public void DestroyCraftAndModules()
  {
    List<RocketModuleCluster> list = ((IEnumerable<Ref<RocketModuleCluster>>) this.m_moduleInterface.ClusterModules).Select<Ref<RocketModuleCluster>, RocketModuleCluster>((Func<Ref<RocketModuleCluster>, RocketModuleCluster>) (x => x.Get())).ToList<RocketModuleCluster>();
    for (int index1 = list.Count - 1; index1 >= 0; --index1)
    {
      RocketModuleCluster rocketModuleCluster = list[index1];
      Storage component1 = ((Component) rocketModuleCluster).GetComponent<Storage>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        component1.ConsumeAllIgnoringDisease();
      MinionStorage component2 = ((Component) rocketModuleCluster).GetComponent<MinionStorage>();
      if (Object.op_Inequality((Object) component2, (Object) null))
      {
        List<MinionStorage.Info> storedMinionInfo = component2.GetStoredMinionInfo();
        for (int index2 = storedMinionInfo.Count - 1; index2 >= 0; --index2)
          component2.DeleteStoredMinion(storedMinionInfo[index2].id);
      }
      Util.KDestroyGameObject(((Component) rocketModuleCluster).gameObject);
    }
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  public void CancelLaunch()
  {
    if (!this.LaunchRequested)
      return;
    Debug.Log((object) "Cancelling launch!");
    this.LaunchRequested = false;
  }

  public void RequestLaunch(bool automated = false)
  {
    if (((Component) this).HasTag(GameTags.RocketNotOnGround) || this.m_moduleInterface.GetClusterDestinationSelector().IsAtDestination())
      return;
    if (DebugHandler.InstantBuildMode && !automated)
      this.Launch();
    if (this.LaunchRequested || !this.CheckPreppedForLaunch())
      return;
    Debug.Log((object) "Triggering launch!");
    this.LaunchRequested = true;
  }

  public void Launch(bool automated = false)
  {
    if (((Component) this).HasTag(GameTags.RocketNotOnGround) || this.m_moduleInterface.GetClusterDestinationSelector().IsAtDestination())
    {
      this.LaunchRequested = false;
    }
    else
    {
      if ((!DebugHandler.InstantBuildMode || automated) && !this.CheckReadyToLaunch())
        return;
      if (automated && !this.m_moduleInterface.CheckReadyForAutomatedLaunchCommand())
      {
        this.LaunchRequested = false;
      }
      else
      {
        this.LaunchRequested = false;
        this.SetCraftStatus(Clustercraft.CraftStatus.Launching);
        this.m_moduleInterface.DoLaunch();
        this.BurnFuelForTravel();
        this.m_clusterTraveler.AdvancePathOneStep();
        this.UpdateStatusItem();
      }
    }
  }

  public void LandAtPad(LaunchPad pad) => this.m_moduleInterface.GetClusterDestinationSelector().SetDestinationPad(pad);

  public Clustercraft.PadLandingStatus CanLandAtPad(LaunchPad pad, out string failReason)
  {
    if (Object.op_Equality((Object) pad, (Object) null))
    {
      failReason = (string) UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.NONEAVAILABLE;
      return Clustercraft.PadLandingStatus.CanNeverLand;
    }
    if (pad.HasRocket() && Object.op_Inequality((Object) pad.LandedRocket.CraftInterface, (Object) this.m_moduleInterface))
    {
      failReason = "<TEMP>The pad already has a rocket on it!<TEMP>";
      return Clustercraft.PadLandingStatus.CanLandEventually;
    }
    if (ConditionFlightPathIsClear.PadTopEdgeDistanceToCeilingEdge(((Component) pad).gameObject) < this.ModuleInterface.RocketHeight)
    {
      failReason = (string) UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_TOO_SHORT;
      return Clustercraft.PadLandingStatus.CanNeverLand;
    }
    int obstruction = -1;
    if (!ConditionFlightPathIsClear.CheckFlightPathClear(this.ModuleInterface, ((Component) pad).gameObject, out obstruction))
    {
      failReason = string.Format((string) UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_PATH_OBSTRUCTED, (object) pad.GetProperName());
      return Clustercraft.PadLandingStatus.CanNeverLand;
    }
    if (!((Component) pad).GetComponent<Operational>().IsOperational)
    {
      failReason = (string) UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_PAD_DISABLED;
      return Clustercraft.PadLandingStatus.CanNeverLand;
    }
    int rocketBottomPosition = pad.RocketBottomPosition;
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.ModuleInterface.ClusterModules)
    {
      GameObject gameObject = ((Component) clusterModule.Get()).gameObject;
      int verticalPosition = this.ModuleInterface.GetModuleRelativeVerticalPosition(gameObject);
      Building component1 = gameObject.GetComponent<Building>();
      BuildingUnderConstruction component2 = gameObject.GetComponent<BuildingUnderConstruction>();
      BuildingDef buildingDef = Object.op_Inequality((Object) component1, (Object) null) ? component1.Def : component2.Def;
      for (int index = 0; index < buildingDef.WidthInCells; ++index)
      {
        for (int y = 0; y < buildingDef.HeightInCells; ++y)
        {
          int i = Grid.OffsetCell(Grid.OffsetCell(rocketBottomPosition, 0, verticalPosition), -(buildingDef.WidthInCells / 2) + index, y);
          if (Grid.Solid[i])
          {
            failReason = string.Format((string) UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_SITE_OBSTRUCTED, (object) pad.GetProperName());
            return Clustercraft.PadLandingStatus.CanNeverLand;
          }
        }
      }
    }
    failReason = (string) null;
    return Clustercraft.PadLandingStatus.CanLandImmediately;
  }

  private LaunchPad FindValidLandingPad(AxialI location, bool mustLandImmediately)
  {
    LaunchPad validLandingPad = (LaunchPad) null;
    LaunchPad launchPadForWorld = this.m_moduleInterface.GetPreferredLaunchPadForWorld(ClusterUtil.GetAsteroidWorldIdAtLocation(location));
    if (Object.op_Inequality((Object) launchPadForWorld, (Object) null) && this.CanLandAtPad(launchPadForWorld, out string _) == Clustercraft.PadLandingStatus.CanLandImmediately)
      return launchPadForWorld;
    foreach (LaunchPad launchPad in Components.LaunchPads)
    {
      if (AxialI.op_Equality(launchPad.GetMyWorldLocation(), location))
      {
        Clustercraft.PadLandingStatus padLandingStatus = this.CanLandAtPad(launchPad, out string _);
        if (padLandingStatus == Clustercraft.PadLandingStatus.CanLandImmediately)
          return launchPad;
        if (!mustLandImmediately && padLandingStatus == Clustercraft.PadLandingStatus.CanLandEventually)
          validLandingPad = launchPad;
      }
    }
    return validLandingPad;
  }

  public bool CanLandAtAsteroid(AxialI location, bool mustLandImmediately)
  {
    LaunchPad destinationPad = this.m_moduleInterface.GetClusterDestinationSelector().GetDestinationPad();
    Debug.Assert(Object.op_Equality((Object) destinationPad, (Object) null) || AxialI.op_Equality(destinationPad.GetMyWorldLocation(), location), (object) "A rocket is trying to travel to an asteroid but has selected a landing pad at a different asteroid!");
    if (!Object.op_Inequality((Object) destinationPad, (Object) null))
      return Object.op_Inequality((Object) this.FindValidLandingPad(location, mustLandImmediately), (Object) null);
    Clustercraft.PadLandingStatus padLandingStatus = this.CanLandAtPad(destinationPad, out string _);
    if (padLandingStatus == Clustercraft.PadLandingStatus.CanLandImmediately)
      return true;
    return !mustLandImmediately && padLandingStatus == Clustercraft.PadLandingStatus.CanLandEventually;
  }

  private void Land(LaunchPad pad, bool forceGrounded)
  {
    if (this.CanLandAtPad(pad, out string _) != Clustercraft.PadLandingStatus.CanLandImmediately)
      return;
    this.BurnFuelForTravel();
    this.m_location = pad.GetMyWorldLocation();
    this.SetCraftStatus(forceGrounded ? Clustercraft.CraftStatus.Grounded : Clustercraft.CraftStatus.Landing);
    this.m_moduleInterface.DoLand(pad);
    this.UpdateStatusItem();
  }

  private void Land(AxialI destination, LaunchPad chosenPad)
  {
    if (Object.op_Equality((Object) chosenPad, (Object) null))
      chosenPad = this.FindValidLandingPad(destination, true);
    Debug.Assert(Object.op_Equality((Object) chosenPad, (Object) null) || AxialI.op_Equality(chosenPad.GetMyWorldLocation(), this.m_location), (object) "Attempting to land on a pad that isn't at our current position");
    this.Land(chosenPad, false);
  }

  public void UpdateStatusItem()
  {
    if (ClusterGrid.Instance == null)
      return;
    if (this.mainStatusHandle != Guid.Empty)
      this.selectable.RemoveStatusItem(this.mainStatusHandle);
    ClusterGridEntity entityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(this.m_location, EntityLayer.Asteroid);
    ClusterGridEntity orbitAsteroid = this.GetOrbitAsteroid();
    bool flag1 = false;
    if (Object.op_Inequality((Object) orbitAsteroid, (Object) null))
    {
      foreach (KMonoBehaviour launchPad in Components.LaunchPads)
      {
        if (AxialI.op_Equality(launchPad.GetMyWorldLocation(), orbitAsteroid.Location))
        {
          flag1 = true;
          break;
        }
      }
    }
    bool flag2 = false;
    if (Object.op_Inequality((Object) entityOfLayerAtCell, (Object) null))
      this.mainStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InFlight, (object) this.m_clusterTraveler);
    else if (!this.HasResourcesToMove() && !flag1)
    {
      flag2 = true;
      this.mainStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.RocketStranded, (object) orbitAsteroid);
    }
    else
      this.mainStatusHandle = this.m_moduleInterface.GetClusterDestinationSelector().IsAtDestination() || this.CheckDesinationInRange() ? (this.IsFlightInProgress() || this.Status == Clustercraft.CraftStatus.Launching ? this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InFlight, (object) this.m_clusterTraveler) : (!Object.op_Inequality((Object) orbitAsteroid, (Object) null) ? this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal) : this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.InOrbit, (object) orbitAsteroid))) : this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.DestinationOutOfRange, (object) this.m_clusterTraveler);
    ((Component) this).GetComponent<KPrefabID>().SetTag(GameTags.RocketStranded, flag2);
    float num = 0.0f;
    float data = 0.0f;
    foreach (CargoBayCluster allCargoBay in this.GetAllCargoBays())
    {
      num += allCargoBay.MaxCapacity;
      data += allCargoBay.RemainingCapacity;
    }
    if (this.Status != Clustercraft.CraftStatus.Grounded && (double) num > 0.0)
    {
      if ((double) data == 0.0)
      {
        this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.FlightAllCargoFull);
        this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining);
      }
      else
      {
        this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightAllCargoFull);
        if (this.cargoStatusHandle == Guid.Empty)
        {
          this.cargoStatusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, (object) data);
        }
        else
        {
          this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, true);
          this.cargoStatusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining, (object) data);
        }
      }
    }
    else
    {
      this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightCargoRemaining);
      this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.FlightAllCargoFull);
    }
  }

  private void UpdateGroundTags()
  {
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.ModuleInterface.ClusterModules)
    {
      if (clusterModule != null && !Object.op_Equality((Object) clusterModule.Get(), (Object) null))
        this.UpdateGroundTags(((Component) clusterModule.Get()).gameObject);
    }
    this.UpdateGroundTags(((Component) this).gameObject);
  }

  private void UpdateGroundTags(GameObject go)
  {
    this.SetTagOnGameObject(go, GameTags.RocketOnGround, this.status == Clustercraft.CraftStatus.Grounded);
    this.SetTagOnGameObject(go, GameTags.RocketNotOnGround, this.status != 0);
    this.SetTagOnGameObject(go, GameTags.RocketInSpace, this.status == Clustercraft.CraftStatus.InFlight);
    this.SetTagOnGameObject(go, GameTags.EntityInSpace, this.status == Clustercraft.CraftStatus.InFlight);
  }

  private void SetTagOnGameObject(GameObject go, Tag tag, bool set)
  {
    if (set)
      go.AddTag(tag);
    else
      go.RemoveTag(tag);
  }

  public override bool ShowName() => this.status != 0;

  public override bool ShowPath() => this.status != 0;

  public bool IsTravellingAndFueled() => this.HasResourcesToMove() && this.m_clusterTraveler.IsTraveling();

  public override bool ShowProgressBar() => this.IsTravellingAndFueled();

  public override float GetProgress() => this.m_clusterTraveler.GetMoveProgress();

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (this.Status == Clustercraft.CraftStatus.Grounded || !SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 27))
      return;
    UIScheduler.Instance.ScheduleNextFrame("Check Fuel Costs", (Action<object>) (o =>
    {
      foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.ModuleInterface.ClusterModules)
      {
        RocketModuleCluster rocketModuleCluster = clusterModule.Get();
        IFuelTank component1 = ((Component) rocketModuleCluster).GetComponent<IFuelTank>();
        if (component1 != null && !component1.Storage.IsEmpty())
          component1.DEBUG_FillTank();
        OxidizerTank component2 = ((Component) rocketModuleCluster).GetComponent<OxidizerTank>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          Dictionary<Tag, float> oxidizersAvailable = component2.GetOxidizersAvailable();
          if (oxidizersAvailable.Count > 0)
          {
            foreach (KeyValuePair<Tag, float> keyValuePair in oxidizersAvailable)
            {
              if ((double) keyValuePair.Value > 0.0)
              {
                component2.DEBUG_FillTank(ElementLoader.GetElementID(keyValuePair.Key));
                break;
              }
            }
          }
        }
      }
    }));
  }

  public float GetRange() => this.ModuleInterface.Range;

  static Clustercraft()
  {
    Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
    dictionary.Add(SimHashes.OxyRock.CreateTag(), TUNING.ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.LOW);
    dictionary.Add(SimHashes.LiquidOxygen.CreateTag(), TUNING.ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.HIGH);
    dictionary.Add(SimHashes.Fertilizer.CreateTag(), TUNING.ROCKETRY.DLC1_OXIDIZER_EFFICIENCY.VERY_LOW);
    Clustercraft.dlc1OxidizerEfficiencies = dictionary;
    Clustercraft.RocketModuleChangedHandler = new EventSystem.IntraObjectHandler<Clustercraft>((Action<Clustercraft, object>) ((cmp, data) => cmp.RocketModuleChanged(data)));
    Clustercraft.ClusterDestinationChangedHandler = new EventSystem.IntraObjectHandler<Clustercraft>((Action<Clustercraft, object>) ((cmp, data) => cmp.OnClusterDestinationChanged(data)));
    Clustercraft.ClusterDestinationReachedHandler = new EventSystem.IntraObjectHandler<Clustercraft>((Action<Clustercraft, object>) ((cmp, data) => cmp.OnClusterDestinationReached(data)));
    Clustercraft.NameChangedHandler = new EventSystem.IntraObjectHandler<Clustercraft>((Action<Clustercraft, object>) ((cmp, data) => cmp.SetRocketName(data)));
  }

  public enum CraftStatus
  {
    Grounded,
    Launching,
    InFlight,
    Landing,
  }

  public enum CombustionResource
  {
    Fuel,
    Oxidizer,
    All,
  }

  public enum PadLandingStatus
  {
    CanLandImmediately,
    CanLandEventually,
    CanNeverLand,
  }
}
