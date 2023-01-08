// Decompiled with JetBrains decompiler
// Type: Components
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Components
{
  public static Components.Cmps<MinionIdentity> LiveMinionIdentities = new Components.Cmps<MinionIdentity>();
  public static Components.Cmps<MinionIdentity> MinionIdentities = new Components.Cmps<MinionIdentity>();
  public static Components.Cmps<StoredMinionIdentity> StoredMinionIdentities = new Components.Cmps<StoredMinionIdentity>();
  public static Components.Cmps<MinionStorage> MinionStorages = new Components.Cmps<MinionStorage>();
  public static Components.Cmps<MinionResume> MinionResumes = new Components.Cmps<MinionResume>();
  public static Components.Cmps<Sleepable> Sleepables = new Components.Cmps<Sleepable>();
  public static Components.Cmps<IUsable> Toilets = new Components.Cmps<IUsable>();
  public static Components.Cmps<Pickupable> Pickupables = new Components.Cmps<Pickupable>();
  public static Components.Cmps<Brain> Brains = new Components.Cmps<Brain>();
  public static Components.Cmps<BuildingComplete> BuildingCompletes = new Components.Cmps<BuildingComplete>();
  public static Components.Cmps<Notifier> Notifiers = new Components.Cmps<Notifier>();
  public static Components.Cmps<Fabricator> Fabricators = new Components.Cmps<Fabricator>();
  public static Components.Cmps<Refinery> Refineries = new Components.Cmps<Refinery>();
  public static Components.CmpsByWorld<PlantablePlot> PlantablePlots = new Components.CmpsByWorld<PlantablePlot>();
  public static Components.Cmps<Ladder> Ladders = new Components.Cmps<Ladder>();
  public static Components.Cmps<NavTeleporter> NavTeleporters = new Components.Cmps<NavTeleporter>();
  public static Components.Cmps<ITravelTubePiece> ITravelTubePieces = new Components.Cmps<ITravelTubePiece>();
  public static Components.CmpsByWorld<CreatureFeeder> CreatureFeeders = new Components.CmpsByWorld<CreatureFeeder>();
  public static Components.Cmps<Light2D> Light2Ds = new Components.Cmps<Light2D>();
  public static Components.Cmps<Radiator> Radiators = new Components.Cmps<Radiator>();
  public static Components.Cmps<Edible> Edibles = new Components.Cmps<Edible>();
  public static Components.Cmps<Diggable> Diggables = new Components.Cmps<Diggable>();
  public static Components.Cmps<IResearchCenter> ResearchCenters = new Components.Cmps<IResearchCenter>();
  public static Components.Cmps<Harvestable> Harvestables = new Components.Cmps<Harvestable>();
  public static Components.Cmps<HarvestDesignatable> HarvestDesignatables = new Components.Cmps<HarvestDesignatable>();
  public static Components.Cmps<Uprootable> Uprootables = new Components.Cmps<Uprootable>();
  public static Components.Cmps<global::Health> Health = new Components.Cmps<global::Health>();
  public static Components.Cmps<global::Equipment> Equipment = new Components.Cmps<global::Equipment>();
  public static Components.Cmps<FactionAlignment> FactionAlignments = new Components.Cmps<FactionAlignment>();
  public static Components.Cmps<Telepad> Telepads = new Components.Cmps<Telepad>();
  public static Components.Cmps<Generator> Generators = new Components.Cmps<Generator>();
  public static Components.Cmps<EnergyConsumer> EnergyConsumers = new Components.Cmps<EnergyConsumer>();
  public static Components.Cmps<Battery> Batteries = new Components.Cmps<Battery>();
  public static Components.Cmps<Breakable> Breakables = new Components.Cmps<Breakable>();
  public static Components.Cmps<Crop> Crops = new Components.Cmps<Crop>();
  public static Components.Cmps<Prioritizable> Prioritizables = new Components.Cmps<Prioritizable>();
  public static Components.Cmps<Clinic> Clinics = new Components.Cmps<Clinic>();
  public static Components.Cmps<HandSanitizer> HandSanitizers = new Components.Cmps<HandSanitizer>();
  public static Components.Cmps<BuildingCellVisualizer> BuildingCellVisualizers = new Components.Cmps<BuildingCellVisualizer>();
  public static Components.Cmps<RoleStation> RoleStations = new Components.Cmps<RoleStation>();
  public static Components.Cmps<Telescope> Telescopes = new Components.Cmps<Telescope>();
  public static Components.Cmps<Capturable> Capturables = new Components.Cmps<Capturable>();
  public static Components.Cmps<NotCapturable> NotCapturables = new Components.Cmps<NotCapturable>();
  public static Components.Cmps<DiseaseSourceVisualizer> DiseaseSourceVisualizers = new Components.Cmps<DiseaseSourceVisualizer>();
  public static Components.Cmps<DetectorNetwork.Instance> DetectorNetworks = new Components.Cmps<DetectorNetwork.Instance>();
  public static Components.Cmps<Grave> Graves = new Components.Cmps<Grave>();
  public static Components.Cmps<AttachableBuilding> AttachableBuildings = new Components.Cmps<AttachableBuilding>();
  public static Components.Cmps<BuildingAttachPoint> BuildingAttachPoints = new Components.Cmps<BuildingAttachPoint>();
  public static Components.Cmps<global::MinionAssignablesProxy> MinionAssignablesProxy = new Components.Cmps<global::MinionAssignablesProxy>();
  public static Components.Cmps<ComplexFabricator> ComplexFabricators = new Components.Cmps<ComplexFabricator>();
  public static Components.Cmps<MonumentPart> MonumentParts = new Components.Cmps<MonumentPart>();
  public static Components.Cmps<PlantableSeed> PlantableSeeds = new Components.Cmps<PlantableSeed>();
  public static Components.Cmps<IBasicBuilding> BasicBuildings = new Components.Cmps<IBasicBuilding>();
  public static Components.Cmps<Painting> Paintings = new Components.Cmps<Painting>();
  public static Components.Cmps<BuildingComplete> TemplateBuildings = new Components.Cmps<BuildingComplete>();
  public static Components.Cmps<Teleporter> Teleporters = new Components.Cmps<Teleporter>();
  public static Components.Cmps<MutantPlant> MutantPlants = new Components.Cmps<MutantPlant>();
  public static Components.Cmps<LandingBeacon.Instance> LandingBeacons = new Components.Cmps<LandingBeacon.Instance>();
  public static Components.Cmps<HighEnergyParticle> HighEnergyParticles = new Components.Cmps<HighEnergyParticle>();
  public static Components.Cmps<HighEnergyParticlePort> HighEnergyParticlePorts = new Components.Cmps<HighEnergyParticlePort>();
  public static Components.Cmps<Clustercraft> Clustercrafts = new Components.Cmps<Clustercraft>();
  public static Components.Cmps<ClustercraftInteriorDoor> ClusterCraftInteriorDoors = new Components.Cmps<ClustercraftInteriorDoor>();
  public static Components.Cmps<PassengerRocketModule> PassengerRocketModules = new Components.Cmps<PassengerRocketModule>();
  public static Components.Cmps<ClusterTraveler> ClusterTravelers = new Components.Cmps<ClusterTraveler>();
  public static Components.Cmps<LaunchPad> LaunchPads = new Components.Cmps<LaunchPad>();
  public static Components.Cmps<WarpReceiver> WarpReceivers = new Components.Cmps<WarpReceiver>();
  public static Components.Cmps<RocketControlStation> RocketControlStations = new Components.Cmps<RocketControlStation>();
  public static Components.Cmps<Reactor> NuclearReactors = new Components.Cmps<Reactor>();
  public static Components.Cmps<BuildingComplete> EntombedBuildings = new Components.Cmps<BuildingComplete>();
  public static Components.Cmps<SpaceArtifact> SpaceArtifacts = new Components.Cmps<SpaceArtifact>();
  public static Components.Cmps<ArtifactAnalysisStationWorkable> ArtifactAnalysisStations = new Components.Cmps<ArtifactAnalysisStationWorkable>();
  public static Components.Cmps<RocketConduitReceiver> RocketConduitReceivers = new Components.Cmps<RocketConduitReceiver>();
  public static Components.Cmps<RocketConduitSender> RocketConduitSenders = new Components.Cmps<RocketConduitSender>();
  public static Components.Cmps<LogicBroadcaster> LogicBroadcasters = new Components.Cmps<LogicBroadcaster>();
  public static Components.Cmps<Telephone> Telephones = new Components.Cmps<Telephone>();
  public static Components.Cmps<MissionControlWorkable> MissionControlWorkables = new Components.Cmps<MissionControlWorkable>();
  public static Components.Cmps<MissionControlClusterWorkable> MissionControlClusterWorkables = new Components.Cmps<MissionControlClusterWorkable>();
  public static Components.CmpsByWorld<Geyser> Geysers = new Components.CmpsByWorld<Geyser>();
  public static Components.CmpsByWorld<GeoTuner.Instance> GeoTuners = new Components.CmpsByWorld<GeoTuner.Instance>();
  public static Components.Cmps<IncubationMonitor.Instance> IncubationMonitors = new Components.Cmps<IncubationMonitor.Instance>();
  public static Components.Cmps<FixedCapturableMonitor.Instance> FixedCapturableMonitors = new Components.Cmps<FixedCapturableMonitor.Instance>();
  public static Components.Cmps<BeeHive.StatesInstance> BeeHives = new Components.Cmps<BeeHive.StatesInstance>();

  public class Cmps<T> : ICollection, IEnumerable
  {
    private Dictionary<T, HandleVector<int>.Handle> table;
    private KCompactedVector<T> items;

    public List<T> Items => this.items.GetDataList();

    public int Count => this.items.Count;

    public Cmps()
    {
      App.OnPreLoadScene += new System.Action(this.Clear);
      this.items = new KCompactedVector<T>(0);
      this.table = new Dictionary<T, HandleVector<int>.Handle>();
    }

    public T this[int idx] => this.Items[idx];

    private void Clear()
    {
      this.items.Clear();
      this.table.Clear();
      this.OnAdd = (Action<T>) null;
      this.OnRemove = (Action<T>) null;
    }

    public void Add(T cmp)
    {
      HandleVector<int>.Handle handle = this.items.Allocate(cmp);
      this.table[cmp] = handle;
      if (this.OnAdd == null)
        return;
      this.OnAdd(cmp);
    }

    public void Remove(T cmp)
    {
      HandleVector<int>.Handle invalidHandle = HandleVector<int>.InvalidHandle;
      if (!this.table.TryGetValue(cmp, out invalidHandle))
        return;
      this.table.Remove(cmp);
      this.items.Free(invalidHandle);
      if (this.OnRemove == null)
        return;
      this.OnRemove(cmp);
    }

    public void Register(Action<T> on_add, Action<T> on_remove)
    {
      this.OnAdd += on_add;
      this.OnRemove += on_remove;
      foreach (T obj in this.Items)
        this.OnAdd(obj);
    }

    public void Unregister(Action<T> on_add, Action<T> on_remove)
    {
      this.OnAdd -= on_add;
      this.OnRemove -= on_remove;
    }

    public List<T> GetWorldItems(int worldId, bool checkChildWorlds = false)
    {
      List<T> worldItems = new List<T>();
      foreach (T obj in this.Items)
      {
        KMonoBehaviour component = (object) obj as KMonoBehaviour;
        bool flag = component.GetMyWorldId() == worldId;
        if (!flag & checkChildWorlds)
        {
          WorldContainer myWorld = component.GetMyWorld();
          if (Object.op_Inequality((Object) myWorld, (Object) null) && myWorld.ParentWorldId == worldId)
            flag = true;
        }
        if (flag)
          worldItems.Add(obj);
      }
      return worldItems;
    }

    public event Action<T> OnAdd;

    public event Action<T> OnRemove;

    public bool IsSynchronized => throw new NotImplementedException();

    public object SyncRoot => throw new NotImplementedException();

    public void CopyTo(Array array, int index) => throw new NotImplementedException();

    public IEnumerator GetEnumerator() => this.items.GetEnumerator();
  }

  public class CmpsByWorld<T>
  {
    private Dictionary<int, Components.Cmps<T>> m_CmpsByWorld;

    public CmpsByWorld()
    {
      App.OnPreLoadScene += new System.Action(this.Clear);
      this.m_CmpsByWorld = new Dictionary<int, Components.Cmps<T>>();
    }

    public void Clear() => this.m_CmpsByWorld.Clear();

    public Components.Cmps<T> CreateOrGetCmps(int worldId)
    {
      Components.Cmps<T> orGetCmps;
      if (!this.m_CmpsByWorld.TryGetValue(worldId, out orGetCmps))
      {
        orGetCmps = new Components.Cmps<T>();
        this.m_CmpsByWorld[worldId] = orGetCmps;
      }
      return orGetCmps;
    }

    public void Add(int worldId, T cmp)
    {
      DebugUtil.DevAssertArgs((worldId != -1 ? 1 : 0) != 0, new object[2]
      {
        (object) "CmpsByWorld tried to add a component to an invalid world. Did you call this during a state machine's constructor instead of StartSM? ",
        (object) cmp
      });
      this.CreateOrGetCmps(worldId).Add(cmp);
    }

    public void Remove(int worldId, T cmp) => this.CreateOrGetCmps(worldId).Remove(cmp);

    public void Register(int worldId, Action<T> on_add, Action<T> on_remove) => this.CreateOrGetCmps(worldId).Register(on_add, on_remove);

    public void Unregister(int worldId, Action<T> on_add, Action<T> on_remove) => this.CreateOrGetCmps(worldId).Unregister(on_add, on_remove);

    public List<T> GetItems(int worldId) => this.CreateOrGetCmps(worldId).Items;

    public IEnumerator GetWorldEnumerator(int worldId) => this.CreateOrGetCmps(worldId).GetEnumerator();

    public int[] GetWorldsIDs()
    {
      int[] worldsIds = new int[this.m_CmpsByWorld.Keys.Count];
      int num = 0;
      foreach (int key in this.m_CmpsByWorld.Keys)
        worldsIds[num++] = key;
      return worldsIds;
    }

    public int GlobalCount
    {
      get
      {
        int globalCount = 0;
        foreach (KeyValuePair<int, Components.Cmps<T>> keyValuePair in this.m_CmpsByWorld)
          globalCount += this.m_CmpsByWorld.Count;
        return globalCount;
      }
    }
  }
}
