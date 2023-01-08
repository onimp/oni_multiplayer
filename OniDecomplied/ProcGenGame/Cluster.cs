// Decompiled with JetBrains decompiler
// Type: ProcGenGame.Cluster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using ProcGen;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace ProcGenGame
{
  [Serializable]
  public class Cluster
  {
    public List<WorldGen> worlds = new List<WorldGen>();
    public WorldGen currentWorld;
    public Vector2I size;
    public string Id;
    public int numRings = 5;
    private int seed;
    private SeededRandom myRandom;
    private bool doSimSettle = true;
    public Action<int, WorldGen> PerWorldGenBeginCallback;
    public Action<int, WorldGen, Sim.Cell[], Sim.DiseaseCell[]> PerWorldGenCompleteCallback;
    public Func<int, WorldGen, bool> ShouldSkipWorldCallback;
    public Dictionary<ClusterLayoutSave.POIType, List<AxialI>> poiLocations = new Dictionary<ClusterLayoutSave.POIType, List<AxialI>>();
    public Dictionary<AxialI, string> poiPlacements = new Dictionary<AxialI, string>();
    public List<WorldTrait> unplacedStoryTraits;
    public List<string> chosenStoryTraitIds;
    private Thread thread;

    public ClusterLayout clusterLayout { get; private set; }

    public bool IsGenerationComplete { get; private set; }

    public bool IsGenerating => this.thread != null && this.thread.IsAlive;

    private Cluster()
    {
    }

    public Cluster(
      string name,
      int seed,
      List<string> chosenStoryTraitIds,
      bool assertMissingTraits,
      bool skipWorldTraits)
    {
      DebugUtil.Assert(!string.IsNullOrEmpty(name), "Cluster file is missing");
      this.seed = seed;
      WorldGen.LoadSettings();
      this.clusterLayout = SettingsCache.clusterLayouts.clusterCache[name];
      this.unplacedStoryTraits = new List<WorldTrait>();
      if (!this.clusterLayout.disableStoryTraits)
      {
        this.chosenStoryTraitIds = chosenStoryTraitIds;
        foreach (string chosenStoryTraitId in chosenStoryTraitIds)
        {
          WorldTrait cachedStoryTrait = SettingsCache.GetCachedStoryTrait(chosenStoryTraitId, assertMissingTraits);
          if (cachedStoryTrait != null)
            this.unplacedStoryTraits.Add(cachedStoryTrait);
        }
      }
      else
        this.chosenStoryTraitIds = new List<string>();
      this.Id = name;
      bool flag = seed > 0 && !skipWorldTraits;
      for (int index = 0; index < this.clusterLayout.worldPlacements.Count; ++index)
      {
        ProcGen.World worldData = SettingsCache.worlds.GetWorldData(this.clusterLayout.worldPlacements[index].world);
        if (worldData != null)
        {
          this.clusterLayout.worldPlacements[index].SetSize(worldData.worldsize);
          if (index == this.clusterLayout.startWorldIndex)
            this.clusterLayout.worldPlacements[index].startWorld = true;
        }
      }
      this.size = BestFit.BestFitWorlds(this.clusterLayout.worldPlacements);
      foreach (WorldPlacement worldPlacement in this.clusterLayout.worldPlacements)
      {
        List<string> chosenWorldTraits = new List<string>();
        if (flag)
        {
          ProcGen.World worldData = SettingsCache.worlds.GetWorldData(worldPlacement.world);
          chosenWorldTraits = SettingsCache.GetRandomTraits(seed, worldData);
          ++seed;
        }
        WorldGen worldGen = new WorldGen(worldPlacement.world, chosenWorldTraits, (List<string>) null, assertMissingTraits);
        Vector2I worldsize = worldGen.Settings.world.worldsize;
        worldGen.SetWorldSize(worldsize.x, worldsize.y);
        worldGen.SetPosition(new Vector2I(worldPlacement.x, worldPlacement.y));
        this.worlds.Add(worldGen);
        if (worldPlacement.startWorld)
        {
          this.currentWorld = worldGen;
          worldGen.isStartingWorld = true;
        }
      }
      if (this.currentWorld == null)
      {
        Debug.LogWarning((object) string.Format("Start world not set. Defaulting to first world {0}", (object) this.worlds[0].Settings.world.name));
        this.currentWorld = this.worlds[0];
      }
      if (this.clusterLayout.numRings <= 0)
        return;
      this.numRings = this.clusterLayout.numRings;
    }

    public void Reset() => this.worlds.Clear();

    private void LogBeginGeneration()
    {
      string str = Object.op_Inequality((Object) CustomGameSettings.Instance, (Object) null) ? CustomGameSettings.Instance.GetSettingsCoordinate() : this.seed.ToString();
      Console.WriteLine("\n\n");
      DebugUtil.LogArgs(new object[1]
      {
        (object) "WORLDGEN START"
      });
      DebugUtil.LogArgs(new object[1]
      {
        (object) (" - seed:     " + str)
      });
      DebugUtil.LogArgs(new object[1]
      {
        (object) (" - cluster:  " + this.clusterLayout.filePath)
      });
      if (this.chosenStoryTraitIds.Count == 0)
      {
        DebugUtil.LogArgs(new object[1]
        {
          (object) " - storytraits: none"
        });
      }
      else
      {
        DebugUtil.LogArgs(new object[1]
        {
          (object) " - storytraits:"
        });
        foreach (string chosenStoryTraitId in this.chosenStoryTraitIds)
          DebugUtil.LogArgs(new object[1]
          {
            (object) ("    - " + chosenStoryTraitId)
          });
      }
    }

    public void Generate(
      WorldGen.OfflineCallbackFunction callbackFn,
      Action<OfflineWorldGen.ErrorInfo> error_cb,
      int worldSeed = -1,
      int layoutSeed = -1,
      int terrainSeed = -1,
      int noiseSeed = -1,
      bool doSimSettle = true,
      bool debug = false)
    {
      this.doSimSettle = doSimSettle;
      for (int index = 0; index != this.worlds.Count; ++index)
        this.worlds[index].Initialise(callbackFn, error_cb, worldSeed + index, layoutSeed + index, terrainSeed + index, noiseSeed + index, debug);
      this.IsGenerationComplete = false;
      this.thread = new Thread(new ThreadStart(this.ThreadMain));
      Util.ApplyInvariantCultureToThread(this.thread);
      this.thread.Start();
    }

    private void BeginGeneration()
    {
      this.LogBeginGeneration();
      int baseId = 0;
      List<WorldGen> worldGenList = new List<WorldGen>((IEnumerable<WorldGen>) this.worlds);
      worldGenList.Sort((Comparison<WorldGen>) ((a, b) => WorldPlacement.CompareLocationType(this.clusterLayout.worldPlacements.Find((Predicate<WorldPlacement>) (x => x.world == a.Settings.world.filePath)), this.clusterLayout.worldPlacements.Find((Predicate<WorldPlacement>) (x => x.world == b.Settings.world.filePath)))));
      for (int index = 0; index < worldGenList.Count; ++index)
      {
        WorldGen worldGen = worldGenList[index];
        if (this.ShouldSkipWorldCallback == null || !this.ShouldSkipWorldCallback(index, worldGen))
        {
          DebugUtil.Separator();
          DebugUtil.LogArgs(new object[1]
          {
            (object) ("Generating world: " + worldGen.Settings.world.filePath)
          });
          if (worldGen.Settings.GetWorldTraitIDs().Length != 0)
            DebugUtil.LogArgs(new object[1]
            {
              (object) (" - worldtraits: " + string.Join(", ", ((IEnumerable<string>) worldGen.Settings.GetWorldTraitIDs()).ToArray<string>()))
            });
          if (this.PerWorldGenBeginCallback != null)
            this.PerWorldGenBeginCallback(index, worldGen);
          List<WorldTrait> worldTraitList = new List<WorldTrait>();
          worldTraitList.AddRange((IEnumerable<WorldTrait>) this.unplacedStoryTraits);
          worldGen.Settings.SetStoryTraitCandidates(worldTraitList);
          GridSettings.Reset(worldGen.GetSize().x, worldGen.GetSize().y);
          worldGen.GenerateOffline();
          worldGen.FinalizeStartLocation();
          Sim.Cell[] cells = (Sim.Cell[]) null;
          Sim.DiseaseCell[] dc = (Sim.DiseaseCell[]) null;
          List<WorldTrait> placedStoryTraits = new List<WorldTrait>();
          if (!worldGen.RenderOffline(this.doSimSettle, ref cells, ref dc, baseId, ref placedStoryTraits, worldGen.isStartingWorld))
          {
            this.thread = (Thread) null;
            return;
          }
          if (this.PerWorldGenCompleteCallback != null)
            this.PerWorldGenCompleteCallback(index, worldGen, cells, dc);
          foreach (WorldTrait worldTrait in placedStoryTraits)
            this.unplacedStoryTraits.Remove(worldTrait);
          ++baseId;
        }
      }
      if (this.unplacedStoryTraits.Count > 0)
      {
        List<string> stringList = new List<string>();
        foreach (WorldTrait unplacedStoryTrait in this.unplacedStoryTraits)
          stringList.Add(unplacedStoryTrait.filePath);
        string message = "Story trait failure, unable to place on any world: " + string.Join(", ", stringList.ToArray());
        if (!this.worlds[0].isRunningDebugGen)
          this.worlds[0].ReportWorldGenError(new Exception(message), (string) UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FAILURE_STORY);
        DebugUtil.LogWarningArgs(Array.Empty<object>());
        this.thread = (Thread) null;
      }
      else
      {
        DebugUtil.Separator();
        DebugUtil.LogArgs(new object[1]
        {
          (object) "Placing worlds on cluster map"
        });
        if (!this.AssignClusterLocations())
        {
          this.thread = (Thread) null;
        }
        else
        {
          this.Save();
          this.thread = (Thread) null;
          DebugUtil.Separator();
          DebugUtil.LogArgs(new object[1]
          {
            (object) "WORLDGEN COMPLETE\n\n\n"
          });
          this.IsGenerationComplete = true;
        }
      }
    }

    private bool IsValidHex(AxialI location) => AxialUtil.IsWithinRadius(location, AxialI.ZERO, this.numRings - 1);

    public bool AssignClusterLocations()
    {
      this.myRandom = new SeededRandom(this.seed);
      ClusterLayout clusterLayout = SettingsCache.clusterLayouts.clusterCache[this.Id];
      List<WorldPlacement> worldPlacementList = new List<WorldPlacement>((IEnumerable<WorldPlacement>) clusterLayout.worldPlacements);
      List<SpaceMapPOIPlacement> spaceMapPoiPlacementList = clusterLayout.poiPlacements == null ? new List<SpaceMapPOIPlacement>() : new List<SpaceMapPOIPlacement>((IEnumerable<SpaceMapPOIPlacement>) clusterLayout.poiPlacements);
      this.currentWorld.SetClusterLocation(AxialI.ZERO);
      HashSet<AxialI> assignedLocations = new HashSet<AxialI>();
      HashSet<AxialI> worldForbiddenLocations = new HashSet<AxialI>();
      HashSet<AxialI> axialISet = new HashSet<AxialI>();
      HashSet<AxialI> poiWorldAvoidance = new HashSet<AxialI>();
      int num1 = 2;
      for (int index = 0; index < this.worlds.Count; ++index)
      {
        WorldGen world = this.worlds[index];
        WorldPlacement worldPlacement = worldPlacementList[index];
        DebugUtil.Assert(worldPlacement != null, "Somehow we're trying to generate a cluster with a world that isn't the cluster .yaml's world list!", world.Settings.world.filePath);
        HashSet<AxialI> antiBuffer = new HashSet<AxialI>();
        foreach (AxialI axialI in assignedLocations)
          antiBuffer.UnionWith((IEnumerable<AxialI>) AxialUtil.GetRings(axialI, 1, worldPlacement.buffer));
        AxialI zero1 = AxialI.ZERO;
        MinMaxI allowedRings1 = worldPlacement.allowedRings;
        int min1 = ((MinMaxI) ref allowedRings1).min;
        MinMaxI allowedRings2 = worldPlacement.allowedRings;
        int num2 = Mathf.Min(((MinMaxI) ref allowedRings2).max, this.numRings - 1);
        List<AxialI> list1 = ((IEnumerable<AxialI>) AxialUtil.GetRings(zero1, min1, num2)).Where<AxialI>((Func<AxialI, bool>) (location => !assignedLocations.Contains(location) && !worldForbiddenLocations.Contains(location) && !antiBuffer.Contains(location))).ToList<AxialI>();
        if (list1.Count > 0)
        {
          AxialI location = list1[this.myRandom.RandomRange(0, list1.Count)];
          world.SetClusterLocation(location);
          assignedLocations.Add(location);
          worldForbiddenLocations.UnionWith((IEnumerable<AxialI>) AxialUtil.GetRings(location, 1, worldPlacement.buffer));
          poiWorldAvoidance.UnionWith((IEnumerable<AxialI>) AxialUtil.GetRings(location, 1, num1));
        }
        else
        {
          DebugUtil.DevLogError("Could not find a spot in the cluster for " + world.Settings.world.filePath + ". Check the placement settings in " + this.Id + ".yaml to ensure there are no conflicts.");
          HashSet<AxialI> minBuffers = new HashSet<AxialI>();
          foreach (AxialI axialI in assignedLocations)
            minBuffers.UnionWith((IEnumerable<AxialI>) AxialUtil.GetRings(axialI, 1, 2));
          AxialI zero2 = AxialI.ZERO;
          MinMaxI allowedRings3 = worldPlacement.allowedRings;
          int min2 = ((MinMaxI) ref allowedRings3).min;
          MinMaxI allowedRings4 = worldPlacement.allowedRings;
          int num3 = Mathf.Min(((MinMaxI) ref allowedRings4).max, this.numRings - 1);
          List<AxialI> list2 = ((IEnumerable<AxialI>) AxialUtil.GetRings(zero2, min2, num3)).Where<AxialI>((Func<AxialI, bool>) (location => !assignedLocations.Contains(location) && !minBuffers.Contains(location))).ToList<AxialI>();
          if (list2.Count > 0)
          {
            AxialI location = list2[this.myRandom.RandomRange(0, list2.Count)];
            world.SetClusterLocation(location);
            assignedLocations.Add(location);
            worldForbiddenLocations.UnionWith((IEnumerable<AxialI>) AxialUtil.GetRings(location, 1, worldPlacement.buffer));
            poiWorldAvoidance.UnionWith((IEnumerable<AxialI>) AxialUtil.GetRings(location, 1, num1));
          }
          else
          {
            string message = "Could not find a spot in the cluster for " + world.Settings.world.filePath + " EVEN AFTER REDUCING BUFFERS. Check the placement settings in " + this.Id + ".yaml to ensure there are no conflicts.";
            DebugUtil.LogErrorArgs(new object[1]
            {
              (object) message
            });
            if (!world.isRunningDebugGen)
              this.currentWorld.ReportWorldGenError(new Exception(message));
            return false;
          }
        }
      }
      if (DlcManager.FeatureClusterSpaceEnabled() && spaceMapPoiPlacementList != null)
      {
        HashSet<AxialI> poiClumpLocations = new HashSet<AxialI>();
        HashSet<AxialI> poiForbiddenLocations = new HashSet<AxialI>();
        float num4 = 0.5f;
        int num5 = 3;
        int num6 = 0;
        foreach (SpaceMapPOIPlacement spaceMapPoiPlacement in spaceMapPoiPlacementList)
        {
          List<string> stringList = new List<string>((IEnumerable<string>) spaceMapPoiPlacement.pois);
          for (int index = 0; index < spaceMapPoiPlacement.numToSpawn; ++index)
          {
            int num7 = (double) this.myRandom.RandomRange(0.0f, 1f) <= (double) num4 ? 1 : 0;
            List<AxialI> axialIList = (List<AxialI>) null;
            if (num7 != 0 && num6 < num5 && !spaceMapPoiPlacement.avoidClumping)
            {
              ++num6;
              AxialI zero = AxialI.ZERO;
              MinMaxI allowedRings = spaceMapPoiPlacement.allowedRings;
              int min = ((MinMaxI) ref allowedRings).min;
              allowedRings = spaceMapPoiPlacement.allowedRings;
              int num8 = Mathf.Min(((MinMaxI) ref allowedRings).max, this.numRings - 1);
              axialIList = ((IEnumerable<AxialI>) AxialUtil.GetRings(zero, min, num8)).Where<AxialI>((Func<AxialI, bool>) (location => !assignedLocations.Contains(location) && poiClumpLocations.Contains(location) && !poiWorldAvoidance.Contains(location))).ToList<AxialI>();
            }
            if (axialIList == null || axialIList.Count <= 0)
            {
              num6 = 0;
              poiClumpLocations.Clear();
              AxialI zero = AxialI.ZERO;
              MinMaxI allowedRings5 = spaceMapPoiPlacement.allowedRings;
              int min = ((MinMaxI) ref allowedRings5).min;
              MinMaxI allowedRings6 = spaceMapPoiPlacement.allowedRings;
              int num9 = Mathf.Min(((MinMaxI) ref allowedRings6).max, this.numRings - 1);
              axialIList = ((IEnumerable<AxialI>) AxialUtil.GetRings(zero, min, num9)).Where<AxialI>((Func<AxialI, bool>) (location => !assignedLocations.Contains(location) && !poiWorldAvoidance.Contains(location) && !poiForbiddenLocations.Contains(location))).ToList<AxialI>();
            }
            if (axialIList != null && axialIList.Count > 0)
            {
              AxialI key = axialIList[this.myRandom.RandomRange(0, axialIList.Count)];
              string str = stringList[this.myRandom.RandomRange(0, stringList.Count)];
              if (!spaceMapPoiPlacement.canSpawnDuplicates)
                stringList.Remove(str);
              this.poiPlacements[key] = str;
              poiForbiddenLocations.UnionWith((IEnumerable<AxialI>) AxialUtil.GetRings(key, 1, 3));
              poiClumpLocations.UnionWith((IEnumerable<AxialI>) AxialUtil.GetRings(key, 1, 1));
              assignedLocations.Add(key);
            }
            else
            {
              MinMaxI allowedRings7 = spaceMapPoiPlacement.allowedRings;
              // ISSUE: variable of a boxed type
              __Boxed<int> min = (ValueType) ((MinMaxI) ref allowedRings7).min;
              MinMaxI allowedRings8 = spaceMapPoiPlacement.allowedRings;
              // ISSUE: variable of a boxed type
              __Boxed<int> max = (ValueType) ((MinMaxI) ref allowedRings8).max;
              Debug.LogWarning((object) string.Format("There is no room for a Space POI in ring range [{0}, {1}]", (object) min, (object) max));
            }
          }
        }
      }
      return true;
    }

    public void AbortGeneration()
    {
      if (this.thread == null || !this.thread.IsAlive)
        return;
      this.thread.Abort();
      this.thread = (Thread) null;
    }

    private void ThreadMain() => this.BeginGeneration();

    private void Save()
    {
      try
      {
        using (MemoryStream output = new MemoryStream())
        {
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
          {
            try
            {
              Manager.Clear();
              ClusterLayoutSave clusterLayoutSave = new ClusterLayoutSave();
              clusterLayoutSave.version = new Vector2I(1, 1);
              clusterLayoutSave.size = this.size;
              clusterLayoutSave.ID = this.Id;
              clusterLayoutSave.numRings = this.numRings;
              clusterLayoutSave.poiLocations = this.poiLocations;
              clusterLayoutSave.poiPlacements = this.poiPlacements;
              for (int index = 0; index != this.worlds.Count; ++index)
              {
                WorldGen world = this.worlds[index];
                if (this.ShouldSkipWorldCallback == null || !this.ShouldSkipWorldCallback(index, world))
                {
                  clusterLayoutSave.worlds.Add(new ClusterLayoutSave.World()
                  {
                    data = world.data,
                    stats = world.stats,
                    name = world.Settings.world.filePath,
                    isDiscovered = world.isStartingWorld,
                    traits = ((IEnumerable<string>) world.Settings.GetWorldTraitIDs()).ToList<string>(),
                    storyTraits = ((IEnumerable<string>) world.Settings.GetStoryTraitIDs()).ToList<string>()
                  });
                  if (world == this.currentWorld)
                    clusterLayoutSave.currentWorldIdx = index;
                }
              }
              Serializer.Serialize((object) clusterLayoutSave, binaryWriter);
            }
            catch (Exception ex)
            {
              DebugUtil.LogErrorArgs(new object[3]
              {
                (object) "Couldn't serialize",
                (object) ex.Message,
                (object) ex.StackTrace
              });
            }
          }
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) File.Open(WorldGen.WORLDGEN_SAVE_FILENAME, FileMode.Create)))
          {
            Manager.SerializeDirectory(binaryWriter);
            binaryWriter.Write(output.ToArray());
          }
        }
      }
      catch (Exception ex)
      {
        DebugUtil.LogErrorArgs(new object[3]
        {
          (object) "Couldn't write",
          (object) ex.Message,
          (object) ex.StackTrace
        });
      }
    }

    public static Cluster Load()
    {
      Cluster cluster = new Cluster();
      try
      {
        FastReader reader = new FastReader(File.ReadAllBytes(WorldGen.WORLDGEN_SAVE_FILENAME));
        Manager.DeserializeDirectory((IReader) reader);
        int position = reader.Position;
        ClusterLayoutSave clusterLayoutSave = new ClusterLayoutSave();
        if (!Deserializer.Deserialize((object) clusterLayoutSave, (IReader) reader))
        {
          reader.Position = position;
          WorldGen worldGen = WorldGen.Load((IReader) reader, true);
          cluster.worlds.Add(worldGen);
          cluster.size = worldGen.GetSize();
          cluster.currentWorld = cluster.worlds[0] ?? (WorldGen) null;
        }
        else
        {
          for (int index = 0; index != clusterLayoutSave.worlds.Count; ++index)
          {
            ClusterLayoutSave.World world = clusterLayoutSave.worlds[index];
            WorldGen worldGen = new WorldGen(world.name, world.data, world.stats, world.traits, world.storyTraits, false);
            cluster.worlds.Add(worldGen);
            if (index == clusterLayoutSave.currentWorldIdx)
            {
              cluster.currentWorld = worldGen;
              cluster.worlds[index].isStartingWorld = true;
            }
          }
          cluster.size = clusterLayoutSave.size;
          cluster.Id = clusterLayoutSave.ID;
          cluster.numRings = clusterLayoutSave.numRings;
          cluster.poiLocations = clusterLayoutSave.poiLocations;
          cluster.poiPlacements = clusterLayoutSave.poiPlacements;
        }
        DebugUtil.Assert(cluster.currentWorld != null);
        if (cluster.currentWorld == null)
        {
          DebugUtil.Assert(0 < cluster.worlds.Count);
          cluster.currentWorld = cluster.worlds[0];
        }
      }
      catch (Exception ex)
      {
        DebugUtil.LogErrorArgs(new object[3]
        {
          (object) "SolarSystem.Load Error!\n",
          (object) ex.Message,
          (object) ex.StackTrace
        });
        cluster = (Cluster) null;
      }
      return cluster;
    }

    public void LoadClusterLayoutSim(List<SimSaveFileStructure> loadedWorlds)
    {
      for (int baseID = 0; baseID != this.worlds.Count; ++baseID)
      {
        SimSaveFileStructure saveFileStructure = new SimSaveFileStructure();
        try
        {
          FastReader fastReader = new FastReader(File.ReadAllBytes(WorldGen.GetSIMSaveFilename(baseID)));
          Manager.DeserializeDirectory((IReader) fastReader);
          Deserializer.Deserialize((object) saveFileStructure, (IReader) fastReader);
        }
        catch (Exception ex)
        {
          if (!GenericGameSettings.instance.devAutoWorldGenActive)
          {
            DebugUtil.LogErrorArgs(new object[3]
            {
              (object) "LoadSim Error!\n",
              (object) ex.Message,
              (object) ex.StackTrace
            });
            break;
          }
        }
        if (saveFileStructure.worldDetail == null)
        {
          if (!GenericGameSettings.instance.devAutoWorldGenActive)
            Debug.LogError((object) ("Detail is null for world " + baseID.ToString()));
        }
        else
          loadedWorlds.Add(saveFileStructure);
      }
    }

    public void SetIsRunningDebug(bool isDebug)
    {
      foreach (WorldGen world in this.worlds)
        world.isRunningDebugGen = isDebug;
    }

    public void DEBUG_UpdateSeed(int seed) => this.seed = seed;
  }
}
