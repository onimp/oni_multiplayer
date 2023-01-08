// Decompiled with JetBrains decompiler
// Type: ClusterManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using ProcGenGame;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ClusterManager : KMonoBehaviour, ISaveLoadable
{
  public static int MAX_ROCKET_INTERIOR_COUNT = 16;
  public static ClusterManager Instance;
  private ClusterGrid m_grid;
  [Serialize]
  private int m_numRings = 9;
  [Serialize]
  private int activeWorldIdx;
  public static byte INVALID_WORLD_IDX = byte.MaxValue;
  public static Color[] worldColors = new Color[6]
  {
    Color.HSVToRGB(0.15f, 0.3f, 0.5f),
    Color.HSVToRGB(0.3f, 0.3f, 0.5f),
    Color.HSVToRGB(0.45f, 0.3f, 0.5f),
    Color.HSVToRGB(0.6f, 0.3f, 0.5f),
    Color.HSVToRGB(0.75f, 0.3f, 0.5f),
    Color.HSVToRGB(0.9f, 0.3f, 0.5f)
  };
  private List<WorldContainer> m_worldContainers = new List<WorldContainer>();
  [MyCmpGet]
  private ClusterPOIManager m_clusterPOIsManager;
  private Dictionary<int, List<IAssignableIdentity>> minionsByWorld = new Dictionary<int, List<IAssignableIdentity>>();
  private MinionMigrationEventArgs migrationEvArg = new MinionMigrationEventArgs();
  private List<int> _worldIDs = new List<int>();

  public static void DestroyInstance() => ClusterManager.Instance = (ClusterManager) null;

  public int worldCount => this.m_worldContainers.Count;

  public int activeWorldId => this.activeWorldIdx;

  public IList<WorldContainer> WorldContainers => (IList<WorldContainer>) this.m_worldContainers.AsReadOnly();

  public ClusterPOIManager GetClusterPOIManager() => this.m_clusterPOIsManager;

  public Dictionary<int, List<IAssignableIdentity>> MinionsByWorld
  {
    get
    {
      this.minionsByWorld.Clear();
      for (int idx = 0; idx < Components.MinionAssignablesProxy.Count; ++idx)
      {
        if (!Components.MinionAssignablesProxy[idx].GetTargetGameObject().HasTag(GameTags.Dead))
        {
          int id = Components.MinionAssignablesProxy[idx].GetTargetGameObject().GetComponent<KMonoBehaviour>().GetMyWorld().id;
          if (!this.minionsByWorld.ContainsKey(id))
            this.minionsByWorld.Add(id, new List<IAssignableIdentity>());
          this.minionsByWorld[id].Add((IAssignableIdentity) Components.MinionAssignablesProxy[idx]);
        }
      }
      return this.minionsByWorld;
    }
  }

  public void RegisterWorldContainer(WorldContainer worldContainer) => this.m_worldContainers.Add(worldContainer);

  public void UnregisterWorldContainer(WorldContainer worldContainer)
  {
    this.Trigger(-1078710002, (object) worldContainer.id);
    this.m_worldContainers.Remove(worldContainer);
  }

  public List<int> GetWorldIDsSorted()
  {
    this.m_worldContainers.Sort((Comparison<WorldContainer>) ((a, b) => a.DiscoveryTimestamp.CompareTo(b.DiscoveryTimestamp)));
    this._worldIDs.Clear();
    foreach (WorldContainer worldContainer in this.m_worldContainers)
      this._worldIDs.Add(worldContainer.id);
    return this._worldIDs;
  }

  public List<int> GetDiscoveredAsteroidIDsSorted()
  {
    this.m_worldContainers.Sort((Comparison<WorldContainer>) ((a, b) => a.DiscoveryTimestamp.CompareTo(b.DiscoveryTimestamp)));
    List<int> asteroidIdsSorted = new List<int>();
    for (int index = 0; index < this.m_worldContainers.Count; ++index)
    {
      if (this.m_worldContainers[index].IsDiscovered && !this.m_worldContainers[index].IsModuleInterior)
        asteroidIdsSorted.Add(this.m_worldContainers[index].id);
    }
    return asteroidIdsSorted;
  }

  public WorldContainer GetStartWorld()
  {
    foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) this.WorldContainers)
    {
      if (worldContainer.IsStartWorld)
        return worldContainer;
    }
    return this.WorldContainers[0];
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ClusterManager.Instance = this;
    SaveLoader.Instance.OnWorldGenComplete += new Action<Cluster>(this.OnWorldGenComplete);
  }

  protected virtual void OnSpawn()
  {
    if (this.m_grid == null)
      this.m_grid = new ClusterGrid(this.m_numRings);
    this.UpdateWorldReverbSnapshot(this.activeWorldId);
    base.OnSpawn();
  }

  protected virtual void OnCleanUp() => base.OnCleanUp();

  public WorldContainer activeWorld => this.GetWorld(this.activeWorldId);

  private void OnWorldGenComplete(Cluster clusterLayout)
  {
    this.m_numRings = clusterLayout.numRings;
    this.m_grid = new ClusterGrid(this.m_numRings);
    AxialI location = AxialI.ZERO;
    foreach (WorldGen world in clusterLayout.worlds)
    {
      int id = this.CreateAsteroidWorldContainer(world).id;
      Vector2I position = world.GetPosition();
      Vector2I vector2I = Vector2I.op_Addition(position, world.GetSize());
      if (world.isStartingWorld)
        location = world.GetClusterLocation();
      for (int y = position.y; y < vector2I.y; ++y)
      {
        for (int x = position.x; x < vector2I.x; ++x)
        {
          int cell = Grid.XYToCell(x, y);
          Grid.WorldIdx[cell] = (byte) id;
          Pathfinding.Instance.AddDirtyNavGridCell(cell);
        }
      }
      if (world.isStartingWorld)
        this.activeWorldIdx = id;
    }
    ((Component) this).GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(location, 1);
    this.m_clusterPOIsManager.PopulatePOIsFromWorldGen(clusterLayout);
  }

  private int GetNextWorldId()
  {
    HashSetPool<int, ClusterManager>.PooledHashSet pooledHashSet = HashSetPool<int, ClusterManager>.Allocate();
    foreach (WorldContainer worldContainer in this.m_worldContainers)
      ((HashSet<int>) pooledHashSet).Add(worldContainer.id);
    Debug.Assert(this.m_worldContainers.Count < (int) byte.MaxValue, (object) "Oh no! We're trying to generate our 255th world in this save, things are going to start going badly...");
    for (int nextWorldId = 0; nextWorldId < (int) byte.MaxValue; ++nextWorldId)
    {
      if (!((HashSet<int>) pooledHashSet).Contains(nextWorldId))
      {
        pooledHashSet.Recycle();
        return nextWorldId;
      }
    }
    pooledHashSet.Recycle();
    return (int) ClusterManager.INVALID_WORLD_IDX;
  }

  private WorldContainer CreateAsteroidWorldContainer(WorldGen world)
  {
    int nextWorldId = this.GetNextWorldId();
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("Asteroid")), (GameObject) null, (string) null);
    WorldContainer component1 = gameObject.GetComponent<WorldContainer>();
    component1.SetID(nextWorldId);
    component1.SetWorldDetails(world);
    AsteroidGridEntity component2 = gameObject.GetComponent<AsteroidGridEntity>();
    if (world != null)
    {
      AxialI clusterLocation = world.GetClusterLocation();
      component2.Init(component1.GetRandomName(), clusterLocation, world.Settings.world.asteroidIcon);
    }
    else
      component2.Init("", AxialI.ZERO, "");
    if (component1.IsStartWorld)
    {
      OrbitalMechanics component3 = gameObject.GetComponent<OrbitalMechanics>();
      if (Object.op_Inequality((Object) component3, (Object) null))
        component3.CreateOrbitalObject(Db.Get().OrbitalTypeCategories.backgroundEarth.Id);
    }
    gameObject.SetActive(true);
    return component1;
  }

  private void CreateDefaultAsteroidWorldContainer()
  {
    if (this.m_worldContainers.Count != 0)
      return;
    Debug.LogWarning((object) "Cluster manager has no world containers, create a default using Grid settings.");
    WorldContainer asteroidWorldContainer = this.CreateAsteroidWorldContainer((WorldGen) null);
    int id = asteroidWorldContainer.id;
    for (int y = (int) asteroidWorldContainer.minimumBounds.y; (double) y <= (double) asteroidWorldContainer.maximumBounds.y; ++y)
    {
      for (int x = (int) asteroidWorldContainer.minimumBounds.x; (double) x <= (double) asteroidWorldContainer.maximumBounds.x; ++x)
      {
        int cell = Grid.XYToCell(x, y);
        Grid.WorldIdx[cell] = (byte) id;
        Pathfinding.Instance.AddDirtyNavGridCell(cell);
      }
    }
  }

  public void InitializeWorldGrid()
  {
    if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 20))
      this.CreateDefaultAsteroidWorldContainer();
    bool flag = false;
    foreach (WorldContainer worldContainer in this.m_worldContainers)
    {
      Vector2I worldOffset = worldContainer.WorldOffset;
      Vector2I vector2I = Vector2I.op_Addition(worldOffset, worldContainer.WorldSize);
      for (int y = worldOffset.y; y < vector2I.y; ++y)
      {
        for (int x = worldOffset.x; x < vector2I.x; ++x)
        {
          int cell = Grid.XYToCell(x, y);
          Grid.WorldIdx[cell] = (byte) worldContainer.id;
          Pathfinding.Instance.AddDirtyNavGridCell(cell);
        }
      }
      flag |= worldContainer.IsDiscovered;
    }
    if (flag)
      return;
    Debug.LogWarning((object) "No worlds have been discovered. Setting the active world to discovered");
    this.activeWorld.SetDiscovered();
  }

  public void SetActiveWorld(int worldIdx)
  {
    int activeWorldIdx = this.activeWorldIdx;
    if (activeWorldIdx == worldIdx)
      return;
    this.activeWorldIdx = worldIdx;
    Game.Instance.Trigger(1983128072, (object) new Tuple<int, int>(this.activeWorldIdx, activeWorldIdx));
  }

  public void TimelapseModeOverrideActiveWorld(int overrideValue) => this.activeWorldIdx = overrideValue;

  public WorldContainer GetWorld(int id)
  {
    for (int index = 0; index < this.m_worldContainers.Count; ++index)
    {
      if (this.m_worldContainers[index].id == id)
        return this.m_worldContainers[index];
    }
    return (WorldContainer) null;
  }

  public WorldContainer GetWorldFromPosition(Vector3 position)
  {
    foreach (WorldContainer worldContainer in this.m_worldContainers)
    {
      if (worldContainer.ContainsPoint(Vector2.op_Implicit(position)))
        return worldContainer;
    }
    return (WorldContainer) null;
  }

  public float CountAllRations()
  {
    float num1 = 0.0f;
    foreach (WorldContainer worldContainer in this.m_worldContainers)
    {
      double num2 = (double) RationTracker.Get().CountRations((Dictionary<string, float>) null, worldContainer.worldInventory);
    }
    return num1;
  }

  public Dictionary<Tag, float> GetAllWorldsAccessibleAmounts()
  {
    Dictionary<Tag, float> accessibleAmounts = new Dictionary<Tag, float>();
    foreach (WorldContainer worldContainer in this.m_worldContainers)
    {
      foreach (KeyValuePair<Tag, float> accessibleAmount in worldContainer.worldInventory.GetAccessibleAmounts())
      {
        if (accessibleAmounts.ContainsKey(accessibleAmount.Key))
          accessibleAmounts[accessibleAmount.Key] += accessibleAmount.Value;
        else
          accessibleAmounts.Add(accessibleAmount.Key, accessibleAmount.Value);
      }
    }
    return accessibleAmounts;
  }

  public void MigrateMinion(MinionIdentity minion, int targetID) => this.MigrateMinion(minion, targetID, minion.GetMyWorldId());

  public void MigrateMinion(MinionIdentity minion, int targetID, int prevID)
  {
    if (!ClusterManager.Instance.GetWorld(targetID).IsDiscovered)
      ClusterManager.Instance.GetWorld(targetID).SetDiscovered();
    if (!ClusterManager.Instance.GetWorld(targetID).IsDupeVisited)
      ClusterManager.Instance.GetWorld(targetID).SetDupeVisited();
    this.migrationEvArg.minionId = minion;
    this.migrationEvArg.prevWorldId = prevID;
    this.migrationEvArg.targetWorldId = targetID;
    Game.Instance.assignmentManager.RemoveFromWorld((IAssignableIdentity) minion, this.migrationEvArg.prevWorldId);
    Game.Instance.Trigger(586301400, (object) this.migrationEvArg);
  }

  public int GetLandingBeaconLocation(int worldId)
  {
    foreach (LandingBeacon.Instance landingBeacon in Components.LandingBeacons)
    {
      if (landingBeacon.GetMyWorldId() == worldId && landingBeacon.CanBeTargeted())
        return Grid.PosToCell((StateMachine.Instance) landingBeacon);
    }
    return Grid.InvalidCell;
  }

  public int GetRandomClearCell(int worldId)
  {
    bool flag = false;
    int num1 = 0;
    while (!flag && num1 < 1000)
    {
      ++num1;
      int randomClearCell = Random.Range(0, Grid.CellCount);
      if (!Grid.Solid[randomClearCell] && !Grid.IsLiquid(randomClearCell) && (int) Grid.WorldIdx[randomClearCell] == worldId)
        return randomClearCell;
    }
    int num2 = 0;
    while (!flag && num2 < 1000)
    {
      ++num2;
      int i = Random.Range(0, Grid.CellCount);
      if (!Grid.Solid[i] && (int) Grid.WorldIdx[i] == worldId)
        return i;
    }
    return Grid.InvalidCell;
  }

  private bool NotObstructedCell(int x, int y)
  {
    int cell = Grid.XYToCell(x, y);
    return Grid.IsValidCell(cell) && Object.op_Equality((Object) Grid.Objects[cell, 1], (Object) null);
  }

  private int LowestYThatSeesSky(int topCellYPos, int x)
  {
    int y = topCellYPos;
    while (!this.ValidSurfaceCell(x, y))
      --y;
    return y;
  }

  private bool ValidSurfaceCell(int x, int y)
  {
    int cell = Grid.XYToCell(x, y - 1);
    return Grid.Solid[cell] || Grid.Foundation[cell];
  }

  public int GetRandomSurfaceCell(int worldID, int width = 1, bool excludeTopBorderHeight = true)
  {
    WorldContainer worldContainer = this.m_worldContainers.Find((Predicate<WorldContainer>) (match => match.id == worldID));
    int num1 = Mathf.RoundToInt(Random.Range(worldContainer.minimumBounds.x + (float) (worldContainer.Width / 10), worldContainer.maximumBounds.x - (float) (worldContainer.Width / 10)));
    int topCellYPos = Mathf.RoundToInt(worldContainer.maximumBounds.y);
    if (excludeTopBorderHeight)
      topCellYPos -= Grid.TopBorderHeight;
    int x = num1;
    int y1 = this.LowestYThatSeesSky(topCellYPos, x);
    int num2 = !this.NotObstructedCell(x, y1) ? 0 : 1;
    while (x + 1 != num1 && num2 < width)
    {
      ++x;
      if ((double) x > (double) worldContainer.maximumBounds.x)
      {
        num2 = 0;
        x = (int) worldContainer.minimumBounds.x;
      }
      int y2 = this.LowestYThatSeesSky(topCellYPos, x);
      bool flag = this.NotObstructedCell(x, y2);
      if (y2 == y1 & flag)
        ++num2;
      else
        num2 = !flag ? 0 : 1;
      y1 = y2;
    }
    return num2 < width ? -1 : Grid.XYToCell(x, y1);
  }

  public bool IsPositionInActiveWorld(Vector3 pos)
  {
    if (Object.op_Inequality((Object) this.activeWorld, (Object) null) && !CameraController.Instance.ignoreClusterFX)
    {
      Vector2 vector2_1 = Vector2.op_Multiply(this.activeWorld.maximumBounds, Grid.CellSizeInMeters);
      Vector2 vector2_2 = Vector2.op_Multiply(this.activeWorld.minimumBounds, Grid.CellSizeInMeters);
      if ((double) pos.x < (double) vector2_2.x || (double) pos.x > (double) vector2_1.x || (double) pos.y < (double) vector2_2.y || (double) pos.y > (double) vector2_1.y)
        return false;
    }
    return true;
  }

  public WorldContainer CreateRocketInteriorWorld(
    GameObject craft_go,
    string interiorTemplateName,
    System.Action callback)
  {
    Vector2I rocketInteriorSize = ROCKETRY.ROCKET_INTERIOR_SIZE;
    Vector2I offset;
    if (Grid.GetFreeGridSpace(rocketInteriorSize, out offset))
    {
      int nextWorldId = this.GetNextWorldId();
      craft_go.AddComponent<WorldInventory>();
      WorldContainer rocketInteriorWorld = craft_go.AddComponent<WorldContainer>();
      rocketInteriorWorld.SetRocketInteriorWorldDetails(nextWorldId, rocketInteriorSize, offset);
      Vector2I vector2I = Vector2I.op_Addition(offset, rocketInteriorSize);
      for (int y = offset.y; y < vector2I.y; ++y)
      {
        for (int x = offset.x; x < vector2I.x; ++x)
        {
          int cell = Grid.XYToCell(x, y);
          Grid.WorldIdx[cell] = (byte) nextWorldId;
          Pathfinding.Instance.AddDirtyNavGridCell(cell);
        }
      }
      Debug.Log((object) string.Format("Created new rocket interior id: {0}, at {1} with size {2}", (object) nextWorldId, (object) offset, (object) rocketInteriorSize));
      rocketInteriorWorld.PlaceInteriorTemplate(interiorTemplateName, (System.Action) (() =>
      {
        if (callback != null)
          callback();
        craft_go.GetComponent<CraftModuleInterface>().TriggerEventOnCraftAndRocket(GameHashes.RocketInteriorComplete, (object) null);
      }));
      craft_go.AddOrGet<OrbitalMechanics>().CreateOrbitalObject(Db.Get().OrbitalTypeCategories.landed.Id);
      this.Trigger(-1280433810, (object) rocketInteriorWorld.id);
      return rocketInteriorWorld;
    }
    Debug.LogError((object) "Failed to create rocket interior.");
    return (WorldContainer) null;
  }

  public void DestoryRocketInteriorWorld(int world_id, ClustercraftExteriorDoor door)
  {
    WorldContainer world = this.GetWorld(world_id);
    if (Object.op_Equality((Object) world, (Object) null) || !world.IsModuleInterior)
    {
      Debug.LogError((object) string.Format("Attempting to destroy world id {0}. The world is not a valid rocket interior", (object) world_id));
    }
    else
    {
      GameObject gameObject = ((Component) ((Component) door).GetComponent<RocketModuleCluster>().CraftInterface).gameObject;
      if (this.activeWorldId == world_id)
      {
        if (gameObject.GetComponent<WorldContainer>().ParentWorldId == world_id)
          this.SetActiveWorld(ClusterManager.Instance.GetStartWorld().id);
        else
          this.SetActiveWorld(gameObject.GetComponent<WorldContainer>().ParentWorldId);
      }
      OrbitalMechanics component = gameObject.GetComponent<OrbitalMechanics>();
      if (!Util.IsNullOrDestroyed((object) component))
        Object.Destroy((Object) component);
      int num = gameObject.GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.InFlight ? 1 : 0;
      PrimaryElement moduleElemet = ((Component) door).GetComponent<PrimaryElement>();
      AxialI clusterLocation = ((Component) world).GetComponent<ClusterGridEntity>().Location;
      Vector3 rocketModuleWorldPos = door.transform.position;
      if (num == 0)
        world.EjectAllDupes(rocketModuleWorldPos);
      else
        world.SpacePodAllDupes(clusterLocation, moduleElemet.ElementID);
      world.CancelChores();
      HashSet<int> noRefundTiles;
      world.DestroyWorldBuildings(out noRefundTiles);
      this.UnregisterWorldContainer(world);
      if (num == 0)
      {
        GameScheduler.Instance.ScheduleNextFrame("ClusterManager.world.TransferResourcesToParentWorld", (Action<object>) (obj => world.TransferResourcesToParentWorld(Vector3.op_Addition(rocketModuleWorldPos, new Vector3(0.0f, 0.5f, 0.0f)), noRefundTiles)));
        GameScheduler.Instance.ScheduleNextFrame("ClusterManager.DeleteWorldObjects", (Action<object>) (obj => this.DeleteWorldObjects(world)));
      }
      else
      {
        GameScheduler.Instance.ScheduleNextFrame("ClusterManager.world.TransferResourcesToDebris", (Action<object>) (obj => world.TransferResourcesToDebris(clusterLocation, noRefundTiles, moduleElemet.ElementID)));
        GameScheduler.Instance.ScheduleNextFrame("ClusterManager.DeleteWorldObjects", (Action<object>) (obj => this.DeleteWorldObjects(world)));
      }
    }
  }

  public void UpdateWorldReverbSnapshot(int worldId)
  {
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().SmallRocketInteriorReverbSnapshot);
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MediumRocketInteriorReverbSnapshot);
    WorldContainer world = this.GetWorld(worldId);
    if (!world.IsModuleInterior)
      return;
    AudioMixer.instance.Start(((Component) world).GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule().interiorReverbSnapshot);
  }

  private void DeleteWorldObjects(WorldContainer world)
  {
    Grid.FreeGridSpace(world.WorldSize, world.WorldOffset);
    WorldInventory worldInventory = (WorldInventory) null;
    if (Object.op_Inequality((Object) world, (Object) null))
      worldInventory = ((Component) world).GetComponent<WorldInventory>();
    if (Object.op_Inequality((Object) worldInventory, (Object) null))
      Object.Destroy((Object) worldInventory);
    if (!Object.op_Inequality((Object) world, (Object) null))
      return;
    Object.Destroy((Object) world);
  }
}
