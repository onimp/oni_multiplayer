// Decompiled with JetBrains decompiler
// Type: ProcGenGame.WorldGen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Delaunay.Geo;
using Klei;
using KSerialization;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Builder;
using LibNoiseDotNet.Graphics.Tools.Noise.Utils;
using ProcGen;
using ProcGen.Map;
using ProcGen.Noise;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VoronoiTree;

namespace ProcGenGame
{
  [Serializable]
  public class WorldGen
  {
    private const string _SIM_SAVE_FILENAME = "WorldGenSimSave";
    private const string _SIM_SAVE_EXTENSION = ".dat";
    private const string _WORLDGEN_SAVE_FILENAME = "WorldGenDataSave.dat";
    private const int heatScale = 2;
    private const int UNPASSABLE_EDGE_COUNT = 4;
    private const string heat_noise_name = "noise/Heat";
    private const string default_base_noise_name = "noise/Default";
    private const string default_cave_noise_name = "noise/DefaultCave";
    private const string default_density_noise_name = "noise/DefaultDensity";
    public const int WORLDGEN_SAVE_MAJOR_VERSION = 1;
    public const int WORLDGEN_SAVE_MINOR_VERSION = 1;
    private const float EXTREME_TEMPERATURE_BORDER_RANGE = 150f;
    private const float EXTREME_TEMPERATURE_BORDER_MIN_WIDTH = 2f;
    public static Element voidElement;
    public static Element vacuumElement;
    public static Element katairiteElement;
    public static Element unobtaniumElement;
    private static Diseases m_diseasesDb;
    public bool isRunningDebugGen;
    private bool generateNoiseData = true;
    private HashSet<int> claimedCells = new HashSet<int>();
    public Dictionary<int, int> claimedPOICells = new Dictionary<int, int>();
    private HashSet<int> highPriorityClaims = new HashSet<int>();
    public List<RectInt> POIBounds = new List<RectInt>();
    private WorldGen.OfflineCallbackFunction successCallbackFn;
    private bool running = true;
    private Action<OfflineWorldGen.ErrorInfo> errorCallback;
    private SeededRandom myRandom;
    private NoiseMapBuilderPlane heatSource;
    private bool wasLoaded;
    public int polyIndex = -1;
    public bool isStartingWorld;
    public bool isModuleInterior;
    private static Task loadSettingsTask;

    public static string GetSIMSaveFilename(int baseID = -1) => System.IO.Path.Combine(Util.RootFolder(), baseID == -1 ? "WorldGenSimSave.dat" : string.Format("{0}{1}{2}", (object) "WorldGenSimSave", (object) baseID, (object) ".dat"));

    public static string WORLDGEN_SAVE_FILENAME => System.IO.Path.Combine(Util.RootFolder(), "WorldGenDataSave.dat");

    public static Diseases diseaseStats
    {
      get
      {
        if (WorldGen.m_diseasesDb == null)
          WorldGen.m_diseasesDb = new Diseases((ResourceSet) null, true);
        return WorldGen.m_diseasesDb;
      }
    }

    public int BaseLeft => this.Settings.GetBaseLocation().left;

    public int BaseRight => this.Settings.GetBaseLocation().right;

    public int BaseTop => this.Settings.GetBaseLocation().top;

    public int BaseBot => this.Settings.GetBaseLocation().bottom;

    public Dictionary<string, object> stats { get; private set; }

    public Klei.Data data { get; private set; }

    public bool HasData => this.data != null;

    public bool HasNoiseData => this.HasData && this.data.world != null;

    public float[] DensityMap => this.data.world.density;

    public float[] HeatMap => this.data.world.heatOffset;

    public float[] OverrideMap => this.data.world.overrides;

    public float[] BaseNoiseMap => this.data.world.data;

    public float[] DefaultTendMap => this.data.world.defaultTemp;

    public Chunk World => this.data.world;

    public Vector2I WorldSize => this.data.world.size;

    public Vector2I WorldOffset => this.data.world.offset;

    public WorldLayout WorldLayout => this.data.worldLayout;

    public List<TerrainCell> OverworldCells => this.data.overworldCells;

    public List<TerrainCell> TerrainCells => this.data.terrainCells;

    public List<River> Rivers => this.data.rivers;

    public GameSpawnData SpawnData => this.data.gameSpawnData;

    public int ChunkEdgeSize => this.data.chunkEdgeSize;

    public HashSet<int> ClaimedCells => this.claimedCells;

    public HashSet<int> HighPriorityClaimedCells => this.highPriorityClaims;

    public void ClearClaimedCells()
    {
      this.claimedCells.Clear();
      this.highPriorityClaims.Clear();
    }

    public void AddHighPriorityCells(HashSet<int> cells) => this.highPriorityClaims.Union<int>((IEnumerable<int>) cells);

    public WorldGenSettings Settings { get; private set; }

    public WorldGen(
      string worldName,
      List<string> chosenWorldTraits,
      List<string> chosenStoryTraits,
      bool assertMissingTraits)
    {
      WorldGen.LoadSettings();
      this.Settings = new WorldGenSettings(worldName, chosenWorldTraits, chosenStoryTraits, assertMissingTraits);
      this.data = new Klei.Data();
      this.data.chunkEdgeSize = this.Settings.GetIntSetting(nameof (ChunkEdgeSize));
      this.stats = new Dictionary<string, object>();
    }

    public WorldGen(
      string worldName,
      Klei.Data data,
      Dictionary<string, object> stats,
      List<string> chosenTraits,
      List<string> chosenStoryTraits,
      bool assertMissingTraits)
    {
      WorldGen.LoadSettings();
      this.Settings = new WorldGenSettings(worldName, chosenTraits, chosenStoryTraits, assertMissingTraits);
      this.data = data;
      this.stats = stats;
    }

    public static void SetupDefaultElements()
    {
      WorldGen.voidElement = ElementLoader.FindElementByHash(SimHashes.Void);
      WorldGen.vacuumElement = ElementLoader.FindElementByHash(SimHashes.Vacuum);
      WorldGen.katairiteElement = ElementLoader.FindElementByHash(SimHashes.Katairite);
      WorldGen.unobtaniumElement = ElementLoader.FindElementByHash(SimHashes.Unobtanium);
    }

    public void Reset() => this.wasLoaded = false;

    public static void LoadSettings(bool in_async_thread = false)
    {
      bool is_playing = Application.isPlaying;
      if (in_async_thread)
      {
        WorldGen.loadSettingsTask = Task.Run((System.Action) (() => WorldGen.LoadSettings_Internal(is_playing)));
      }
      else
      {
        if (WorldGen.loadSettingsTask != null)
        {
          WorldGen.loadSettingsTask.Wait();
          WorldGen.loadSettingsTask = (Task) null;
        }
        WorldGen.LoadSettings_Internal(is_playing);
      }
    }

    private static void LoadSettings_Internal(bool is_playing)
    {
      ListPool<YamlIO.Error, WorldGen>.PooledList world_gen_errors = ListPool<YamlIO.Error, WorldGen>.Allocate();
      if (SettingsCache.LoadFiles((List<YamlIO.Error>) world_gen_errors))
        TemplateCache.Init();
      Object.op_Inequality((Object) CustomGameSettings.Instance, (Object) null);
      if (is_playing)
      {
        Global.Instance.modManager.HandleErrors((List<YamlIO.Error>) world_gen_errors);
      }
      else
      {
        foreach (YamlIO.Error error in (List<YamlIO.Error>) world_gen_errors)
          YamlIO.LogError(error, false);
      }
      world_gen_errors.Recycle();
    }

    public void InitRandom(int worldSeed, int layoutSeed, int terrainSeed, int noiseSeed)
    {
      this.data.globalWorldSeed = worldSeed;
      this.data.globalWorldLayoutSeed = layoutSeed;
      this.data.globalTerrainSeed = terrainSeed;
      this.data.globalNoiseSeed = noiseSeed;
      this.myRandom = new SeededRandom(worldSeed);
    }

    public void Initialise(
      WorldGen.OfflineCallbackFunction callbackFn,
      Action<OfflineWorldGen.ErrorInfo> error_cb,
      int worldSeed = -1,
      int layoutSeed = -1,
      int terrainSeed = -1,
      int noiseSeed = -1,
      bool debug = false)
    {
      if (this.wasLoaded)
      {
        Debug.LogError((object) "Initialise called after load");
      }
      else
      {
        this.successCallbackFn = callbackFn;
        this.errorCallback = error_cb;
        Debug.Assert(this.successCallbackFn != null);
        this.isRunningDebugGen = debug;
        this.running = false;
        int num1 = Random.Range(0, int.MaxValue);
        if (worldSeed == -1)
          worldSeed = num1;
        if (layoutSeed == -1)
          layoutSeed = num1;
        if (terrainSeed == -1)
          terrainSeed = num1;
        if (noiseSeed == -1)
          noiseSeed = num1;
        this.data.gameSpawnData = new GameSpawnData();
        this.InitRandom(worldSeed, layoutSeed, terrainSeed, noiseSeed);
        int num2 = this.successCallbackFn(UI.WORLDGEN.COMPLETE.key, 0.0f, WorldGenProgressStages.Stages.Failure) ? 1 : 0;
        this.stats["GenerateTime"] = (object) 0;
        this.stats["GenerateNoiseTime"] = (object) 0;
        this.stats["GenerateLayoutTime"] = (object) 0;
        this.stats["ConvertVoroToMapTime"] = (object) 0;
        WorldLayout.SetLayerGradient(SettingsCache.layers.LevelLayers);
      }
    }

    public void DontGenerateNoiseData() => this.generateNoiseData = false;

    public void GenerateOffline()
    {
      int num1 = 1;
      for (int completePercent = 0; completePercent < num1 && !this.GenerateWorldData(); ++completePercent)
      {
        DebugUtil.DevLogError("Failed worldgen");
        int num2 = this.successCallbackFn(UI.WORLDGEN.RETRYCOUNT.key, (float) completePercent, WorldGenProgressStages.Stages.Failure) ? 1 : 0;
      }
    }

    private void PlaceTemplateSpawners(
      Vector2I position,
      TemplateContainer template,
      ref Dictionary<int, int> claimedCells)
    {
      this.data.gameSpawnData.AddTemplate(template, position, ref claimedCells);
    }

    public bool RenderOffline(
      bool doSettle,
      ref Sim.Cell[] cells,
      ref Sim.DiseaseCell[] dc,
      int baseId,
      ref List<WorldTrait> placedStoryTraits,
      bool isStartingWorld = false)
    {
      float[] bgTemp = (float[]) null;
      dc = (Sim.DiseaseCell[]) null;
      HashSet<int> borderCells = new HashSet<int>();
      this.POIBounds = new List<RectInt>();
      this.WriteOverWorldNoise(this.successCallbackFn);
      if (!this.RenderToMap(this.successCallbackFn, ref cells, ref bgTemp, ref dc, ref borderCells, ref this.POIBounds))
      {
        int num = this.successCallbackFn(UI.WORLDGEN.FAILED.key, -100f, WorldGenProgressStages.Stages.Failure) ? 1 : 0;
        if (!this.isRunningDebugGen)
          return false;
      }
      foreach (int key in borderCells)
      {
        cells[key].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
        this.claimedPOICells[key] = 1;
      }
      List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets = (List<KeyValuePair<Vector2I, TemplateContainer>>) null;
      try
      {
        templateSpawnTargets = TemplateSpawning.DetermineTemplatesForWorld(this.Settings, this.data.terrainCells, this.myRandom, ref this.POIBounds, this.isRunningDebugGen, ref placedStoryTraits, this.successCallbackFn);
      }
      catch (TemplateSpawningException ex)
      {
        if (!this.isRunningDebugGen)
        {
          this.ReportWorldGenError((Exception) ex, ex.userMessage);
          return false;
        }
      }
      catch (Exception ex)
      {
        if (!this.isRunningDebugGen)
        {
          this.ReportWorldGenError(ex);
          return false;
        }
      }
      if (isStartingWorld)
        this.EnsureEnoughElementsInStartingBiome(cells);
      List<TerrainCell> terrainCellsForTag = this.GetTerrainCellsForTag(WorldGenTags.StartWorld);
      foreach (TerrainCell overworldCell in this.OverworldCells)
      {
        foreach (TerrainCell terrainCell in terrainCellsForTag)
        {
          if (overworldCell.poly.PointInPolygon(terrainCell.poly.Centroid()))
          {
            ((ProcGen.Node) overworldCell.node).tags.Add(WorldGenTags.StartWorld);
            break;
          }
        }
      }
      if (doSettle)
        this.running = WorldGenSimUtil.DoSettleSim(this.Settings, ref cells, ref bgTemp, ref dc, this.successCallbackFn, this.data, templateSpawnTargets, this.errorCallback, baseId);
      foreach (KeyValuePair<Vector2I, TemplateContainer> keyValuePair in templateSpawnTargets)
        this.PlaceTemplateSpawners(keyValuePair.Key, keyValuePair.Value, ref this.claimedPOICells);
      if (doSettle)
        this.SpawnMobsAndTemplates(cells, bgTemp, dc, new HashSet<int>((IEnumerable<int>) this.claimedPOICells.Keys));
      int num1 = this.successCallbackFn(UI.WORLDGEN.COMPLETE.key, 1f, WorldGenProgressStages.Stages.Complete) ? 1 : 0;
      this.running = false;
      return true;
    }

    private void SpawnMobsAndTemplates(
      Sim.Cell[] cells,
      float[] bgTemp,
      Sim.DiseaseCell[] dc,
      HashSet<int> claimedCells)
    {
      MobSpawning.DetectNaturalCavities(this.TerrainCells, this.successCallbackFn, cells);
      SeededRandom rnd = new SeededRandom(this.data.globalTerrainSeed);
      for (int index = 0; index < this.TerrainCells.Count; ++index)
      {
        float completePercent = (float) index / (float) this.TerrainCells.Count;
        int num = this.successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, completePercent, WorldGenProgressStages.Stages.PlacingCreatures) ? 1 : 0;
        TerrainCell terrainCell = this.TerrainCells[index];
        Dictionary<int, string> newItems1 = MobSpawning.PlaceFeatureAmbientMobs(this.Settings, terrainCell, rnd, cells, bgTemp, dc, claimedCells, this.isRunningDebugGen);
        if (newItems1 != null)
          this.data.gameSpawnData.AddRange((IEnumerable<KeyValuePair<int, string>>) newItems1);
        Dictionary<int, string> newItems2 = MobSpawning.PlaceBiomeAmbientMobs(this.Settings, terrainCell, rnd, cells, bgTemp, dc, claimedCells, this.isRunningDebugGen);
        if (newItems2 != null)
          this.data.gameSpawnData.AddRange((IEnumerable<KeyValuePair<int, string>>) newItems2);
      }
      int num1 = this.successCallbackFn(UI.WORLDGEN.PLACINGCREATURES.key, 1f, WorldGenProgressStages.Stages.PlacingCreatures) ? 1 : 0;
    }

    public void ReportWorldGenError(Exception e, string errorMessage = null)
    {
      if (errorMessage == null)
        errorMessage = (string) UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FAILURE;
      bool flag = FileSystem.IsModdedFile(SettingsCache.RewriteWorldgenPathYaml(this.Settings.world.filePath));
      string str = Object.op_Inequality((Object) CustomGameSettings.Instance, (Object) null) ? CustomGameSettings.Instance.GetSettingsCoordinate() : this.data.globalWorldLayoutSeed.ToString();
      Debug.LogWarning((object) string.Format("Worldgen Failure on seed {0}, modded={1}", (object) str, (object) flag));
      if (this.errorCallback != null)
        this.errorCallback(new OfflineWorldGen.ErrorInfo()
        {
          errorDesc = string.Format(errorMessage, (object) str),
          exception = e
        });
      if (flag)
        return;
      KCrashReporter.ReportErrorDevNotification("WorldgenFailure", e.StackTrace, str + " - " + e.Message);
    }

    public void SetWorldSize(int width, int height)
    {
      if (this.data.world != null && Vector2I.op_Inequality(this.data.world.offset, Vector2I.zero))
        Debug.LogWarning((object) "Resetting world chunk to defaults.");
      this.data.world = new Chunk(0, 0, width, height);
    }

    public Vector2I GetSize() => this.data.world.size;

    public void SetPosition(Vector2I position) => this.data.world.offset = position;

    public Vector2I GetPosition() => this.data.world.offset;

    public void SetClusterLocation(AxialI location) => this.data.clusterLocation = location;

    public AxialI GetClusterLocation() => this.data.clusterLocation;

    public bool GenerateNoiseData(WorldGen.OfflineCallbackFunction updateProgressFn)
    {
      this.stats["GenerateNoiseTime"] = (object) System.DateTime.Now.Ticks;
      try
      {
        this.running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 0.0f, WorldGenProgressStages.Stages.SetupNoise);
        if (!this.running)
        {
          this.stats["GenerateNoiseTime"] = (object) 0;
          return false;
        }
        this.SetupNoise(updateProgressFn);
        this.running = updateProgressFn(UI.WORLDGEN.SETUPNOISE.key, 1f, WorldGenProgressStages.Stages.SetupNoise);
        if (!this.running)
        {
          this.stats["GenerateNoiseTime"] = (object) 0;
          return false;
        }
        this.GenerateUnChunkedNoise(updateProgressFn);
      }
      catch (Exception ex)
      {
        string message = ex.Message;
        string stackTrace = ex.StackTrace;
        this.ReportWorldGenError(ex);
        string stack = stackTrace;
        WorldGenLogger.LogException(message, stack);
        this.running = this.successCallbackFn(new StringKey("Exception in GenerateNoiseData"), -1f, WorldGenProgressStages.Stages.Failure);
        return false;
      }
      this.stats["GenerateNoiseTime"] = (object) (System.DateTime.Now.Ticks - (long) this.stats["GenerateNoiseTime"]);
      return true;
    }

    public bool GenerateLayout(WorldGen.OfflineCallbackFunction updateProgressFn)
    {
      this.stats["GenerateLayoutTime"] = (object) System.DateTime.Now.Ticks;
      try
      {
        this.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 0.0f, WorldGenProgressStages.Stages.WorldLayout);
        if (!this.running)
          return false;
        Debug.Assert(this.data.world.size.x != 0 && this.data.world.size.y != 0, (object) "Map size has not been set");
        this.data.worldLayout = new WorldLayout(this, this.data.world.size.x, this.data.world.size.y, this.data.globalWorldLayoutSeed);
        this.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 1f, WorldGenProgressStages.Stages.WorldLayout);
        this.data.voronoiTree = (Tree) null;
        try
        {
          this.data.voronoiTree = this.WorldLayout.GenerateOverworld(this.Settings.world.layoutMethod == 1, this.isRunningDebugGen);
          this.WorldLayout.PopulateSubworlds();
          this.CompleteLayout(updateProgressFn);
        }
        catch (Exception ex)
        {
          WorldGenLogger.LogException(ex.Message, ex.StackTrace);
          this.ReportWorldGenError(ex);
          this.running = updateProgressFn(new StringKey("Exception in InitVoronoiTree"), -1f, WorldGenProgressStages.Stages.Failure);
          return false;
        }
        this.data.overworldCells = new List<TerrainCell>(40);
        for (int index = 0; index < this.data.voronoiTree.ChildCount(); ++index)
        {
          Tree child = this.data.voronoiTree.GetChild(index) as Tree;
          this.data.overworldCells.Add((TerrainCell) new TerrainCellLogged(((Graph<Cell, Edge>) this.data.worldLayout.overworldGraph).FindNodeByID(((VoronoiTree.Node) child).site.id), ((VoronoiTree.Node) child).site, ((VoronoiTree.Node) child).minDistanceToTag));
        }
        this.running = updateProgressFn(UI.WORLDGEN.WORLDLAYOUT.key, 1f, WorldGenProgressStages.Stages.WorldLayout);
      }
      catch (Exception ex)
      {
        WorldGenLogger.LogException(ex.Message, ex.StackTrace);
        this.ReportWorldGenError(ex);
        int num = this.successCallbackFn(new StringKey("Exception in GenerateLayout"), -1f, WorldGenProgressStages.Stages.Failure) ? 1 : 0;
        return false;
      }
      this.stats["GenerateLayoutTime"] = (object) (System.DateTime.Now.Ticks - (long) this.stats["GenerateLayoutTime"]);
      return true;
    }

    public bool CompleteLayout(WorldGen.OfflineCallbackFunction updateProgressFn)
    {
      long ticks = System.DateTime.Now.Ticks;
      try
      {
        this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0.0f, WorldGenProgressStages.Stages.CompleteLayout);
        if (!this.running)
          return false;
        this.data.terrainCells = (List<TerrainCell>) null;
        this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0.65f, WorldGenProgressStages.Stages.CompleteLayout);
        if (!this.running)
          return false;
        this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 0.75f, WorldGenProgressStages.Stages.CompleteLayout);
        if (!this.running)
          return false;
        this.data.terrainCells = new List<TerrainCell>(4000);
        List<VoronoiTree.Node> nodes = new List<VoronoiTree.Node>();
        this.data.voronoiTree.ForceLowestToLeaf();
        this.ApplyStartNode();
        this.ApplySwapTags();
        this.data.voronoiTree.GetLeafNodes(nodes, (Tree.LeafNodeTest) null);
        WorldLayout.ResetMapGraphFromVoronoiTree(nodes, this.WorldLayout.localGraph, true);
        for (int index = 0; index < nodes.Count; ++index)
        {
          VoronoiTree.Node node = nodes[index];
          Cell tn = ((Graph<Cell, Edge>) this.data.worldLayout.localGraph).FindNodeByID(node.site.id);
          if (tn != null)
          {
            TerrainCell terrainCell = this.data.terrainCells.Find((Predicate<TerrainCell>) (c => c.node == tn));
            if (terrainCell == null)
              this.data.terrainCells.Add((TerrainCell) new TerrainCellLogged(tn, node.site, ((VoronoiTree.Node) node.parent).minDistanceToTag));
            else
              Debug.LogWarning((object) ("Duplicate cell found" + terrainCell.node.NodeId.ToString()));
          }
        }
        for (int index1 = 0; index1 < this.data.terrainCells.Count; ++index1)
        {
          TerrainCell terrainCell1 = this.data.terrainCells[index1];
          for (int index2 = index1 + 1; index2 < this.data.terrainCells.Count; ++index2)
          {
            int num = 0;
            TerrainCell terrainCell2 = this.data.terrainCells[index2];
            LineSegment lineSegment;
            if (terrainCell2.poly.SharesEdge(terrainCell1.poly, ref num, ref lineSegment) == 2)
            {
              terrainCell1.neighbourTerrainCells.Add(index2);
              terrainCell2.neighbourTerrainCells.Add(index1);
            }
          }
        }
        this.running = updateProgressFn(UI.WORLDGEN.COMPLETELAYOUT.key, 1f, WorldGenProgressStages.Stages.CompleteLayout);
      }
      catch (Exception ex)
      {
        WorldGenLogger.LogException(ex.Message, ex.StackTrace);
        int num = this.successCallbackFn(new StringKey("Exception in CompleteLayout"), -1f, WorldGenProgressStages.Stages.Failure) ? 1 : 0;
        return false;
      }
      this.stats["GenerateLayoutTime"] = (object) ((long) this.stats["GenerateLayoutTime"] + (System.DateTime.Now.Ticks - ticks));
      return true;
    }

    public void UpdateVoronoiNodeTags(VoronoiTree.Node node) => (!node.tags.Contains(WorldGenTags.Overworld) ? (ProcGen.Node) ((Graph<Cell, Edge>) this.WorldLayout.localGraph).FindNodeByID(node.site.id) : (ProcGen.Node) ((Graph<Cell, Edge>) this.WorldLayout.overworldGraph).FindNodeByID(node.site.id))?.tags.Union(node.tags);

    public bool GenerateWorldData()
    {
      this.stats["GenerateDataTime"] = (object) System.DateTime.Now.Ticks;
      if (this.generateNoiseData && !this.GenerateNoiseData(this.successCallbackFn) || !this.GenerateLayout(this.successCallbackFn))
        return false;
      this.stats["GenerateDataTime"] = (object) (System.DateTime.Now.Ticks - (long) this.stats["GenerateDataTime"]);
      return true;
    }

    public void EnsureEnoughElementsInStartingBiome(Sim.Cell[] cells)
    {
      List<StartingWorldElementSetting> startingElements = this.Settings.GetDefaultStartingElements();
      List<TerrainCell> terrainCellsForTag = this.GetTerrainCellsForTag(WorldGenTags.StartWorld);
      foreach (StartingWorldElementSetting worldElementSetting in startingElements)
      {
        float amount = worldElementSetting.amount;
        Element element = ElementLoader.GetElement(new Tag(((SimHashes) Enum.Parse(typeof (SimHashes), worldElementSetting.element, true)).ToString()));
        float num1 = 0.0f;
        int num2 = 0;
        foreach (TerrainCell terrainCell in terrainCellsForTag)
        {
          foreach (int allCell in terrainCell.GetAllCells())
          {
            if ((int) element.idx == (int) cells[allCell].elementIdx)
            {
              ++num2;
              num1 += cells[allCell].mass;
            }
          }
        }
        DebugUtil.DevAssert(num2 > 0, string.Format("No {0} found in starting biome and trying to ensure at least {1}. Skipping.", (object) element.id, (object) amount), (Object) null);
        if ((double) num1 < (double) amount && num2 > 0)
        {
          float num3 = num1 / (float) num2;
          float num4 = (amount - num1) / (float) num2;
          DebugUtil.DevAssert(((double) num3 + (double) num4 <= 2.0 * (double) element.maxMass ? 1 : 0) != 0, string.Format("Number of cells ({0}) of {1} in the starting biome is insufficient, this will result in extremely dense cells. {2} but expecting less than {3}", (object) num2, (object) element.id, (object) (float) ((double) num3 + (double) num4), (object) (float) (2.0 * (double) element.maxMass)), (Object) null);
          foreach (TerrainCell terrainCell in terrainCellsForTag)
          {
            foreach (int allCell in terrainCell.GetAllCells())
            {
              if ((int) element.idx == (int) cells[allCell].elementIdx)
                cells[allCell].mass += num4;
            }
          }
        }
      }
    }

    public bool RenderToMap(
      WorldGen.OfflineCallbackFunction updateProgressFn,
      ref Sim.Cell[] cells,
      ref float[] bgTemp,
      ref Sim.DiseaseCell[] dcs,
      ref HashSet<int> borderCells,
      ref List<RectInt> poiBounds)
    {
      Debug.Assert(Grid.WidthInCells == this.Settings.world.worldsize.x);
      Debug.Assert(Grid.HeightInCells == this.Settings.world.worldsize.y);
      Debug.Assert(Grid.CellCount == Grid.WidthInCells * Grid.HeightInCells);
      Debug.Assert((double) Grid.CellSizeInMeters != 0.0);
      borderCells = new HashSet<int>();
      cells = new Sim.Cell[Grid.CellCount];
      bgTemp = new float[Grid.CellCount];
      dcs = new Sim.DiseaseCell[Grid.CellCount];
      this.running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 0.0f, WorldGenProgressStages.Stages.ClearingLevel);
      if (!this.running)
        return false;
      for (int index = 0; index < cells.Length; ++index)
      {
        cells[index].SetValues(WorldGen.katairiteElement, ElementLoader.elements);
        bgTemp[index] = -1f;
        dcs[index] = new Sim.DiseaseCell();
        dcs[index].diseaseIdx = byte.MaxValue;
        this.running = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, (float) index / (float) Grid.CellCount, WorldGenProgressStages.Stages.ClearingLevel);
        if (!this.running)
          return false;
      }
      int num1 = updateProgressFn(UI.WORLDGEN.CLEARINGLEVEL.key, 1f, WorldGenProgressStages.Stages.ClearingLevel) ? 1 : 0;
      try
      {
        this.ProcessByTerrainCell(cells, bgTemp, dcs, updateProgressFn, this.highPriorityClaims);
      }
      catch (Exception ex)
      {
        WorldGenLogger.LogException(ex.Message, ex.StackTrace);
        this.running = updateProgressFn(new StringKey("Exception in ProcessByTerrainCell"), -1f, WorldGenProgressStages.Stages.Failure);
        return false;
      }
      if (this.Settings.GetBoolSetting("DrawWorldBorder"))
      {
        SeededRandom rnd = new SeededRandom(0);
        this.DrawWorldBorder(cells, this.data.world, rnd, ref borderCells, ref poiBounds, updateProgressFn);
        int num2 = updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 1f, WorldGenProgressStages.Stages.DrawWorldBorder) ? 1 : 0;
      }
      this.data.gameSpawnData.baseStartPos = this.data.worldLayout.GetStartLocation();
      return true;
    }

    public SubWorld GetSubWorldForNode(Tree node)
    {
      ProcGen.Node nodeById = (ProcGen.Node) ((Graph<Cell, Edge>) this.WorldLayout.overworldGraph).FindNodeByID(((VoronoiTree.Node) node).site.id);
      if (nodeById == null)
        return (SubWorld) null;
      return !this.Settings.HasSubworld(nodeById.type) ? (SubWorld) null : this.Settings.GetSubWorld(nodeById.type);
    }

    public Tree GetOverworldForNode(Leaf leaf) => leaf == null ? (Tree) null : this.data.worldLayout.GetVoronoiTree().GetChildContainingLeaf(leaf);

    public Leaf GetLeafForTerrainCell(TerrainCell cell) => cell == null ? (Leaf) null : this.data.worldLayout.GetVoronoiTree().GetNodeForSite(cell.site) as Leaf;

    public List<TerrainCell> GetTerrainCellsForTag(Tag tag)
    {
      List<TerrainCell> terrainCellsForTag = new List<TerrainCell>();
      List<VoronoiTree.Node> leafNodesWithTag = this.WorldLayout.GetLeafNodesWithTag(tag);
      for (int index = 0; index < leafNodesWithTag.Count; ++index)
      {
        VoronoiTree.Node node = leafNodesWithTag[index];
        TerrainCell terrainCell = this.data.terrainCells.Find((Predicate<TerrainCell>) (cell => (int) cell.site.id == (int) node.site.id));
        if (terrainCell != null)
          terrainCellsForTag.Add(terrainCell);
      }
      return terrainCellsForTag;
    }

    private void GetStartCells(out int baseX, out int baseY)
    {
      Vector2I startLocation;
      // ISSUE: explicit constructor call
      ((Vector2I) ref startLocation).\u002Ector(this.data.world.size.x / 2, (int) ((double) this.data.world.size.y * 0.699999988079071));
      if (this.data.worldLayout != null)
        startLocation = this.data.worldLayout.GetStartLocation();
      baseX = startLocation.x;
      baseY = startLocation.y;
    }

    public void FinalizeStartLocation()
    {
      if (string.IsNullOrEmpty(this.Settings.world.startSubworldName))
        return;
      List<VoronoiTree.Node> startNodes = this.WorldLayout.GetStartNodes();
      Debug.Assert(startNodes.Count > 0, (object) "Couldn't find a start node on a world that expects it!!");
      TagSet tagSet1 = new TagSet();
      tagSet1.Add(WorldGenTags.StartLocation);
      TagSet tagSet2 = tagSet1;
      for (int index = 1; index < startNodes.Count; ++index)
        startNodes[index].tags.Remove(tagSet2);
    }

    private void SwitchNodes(VoronoiTree.Node n1, VoronoiTree.Node n2)
    {
      if (n1 is Tree || n2 is Tree)
      {
        Debug.Log((object) "WorldGen::SwitchNodes() Skipping tree node");
      }
      else
      {
        Diagram.Site site = n1.site;
        n1.site = n2.site;
        n2.site = site;
        Cell nodeById1 = ((Graph<Cell, Edge>) this.data.worldLayout.localGraph).FindNodeByID(n1.site.id);
        ProcGen.Node nodeById2 = (ProcGen.Node) ((Graph<Cell, Edge>) this.data.worldLayout.localGraph).FindNodeByID(n2.site.id);
        string type = ((ProcGen.Node) nodeById1).type;
        ((ProcGen.Node) nodeById1).SetType(nodeById2.type);
        nodeById2.SetType(type);
      }
    }

    private void ApplyStartNode()
    {
      List<VoronoiTree.Node> leafNodesWithTag = this.data.worldLayout.GetLeafNodesWithTag(WorldGenTags.StartLocation);
      if (leafNodesWithTag.Count == 0)
        return;
      VoronoiTree.Node node = leafNodesWithTag[0];
      Tree parent = node.parent;
      node.parent.AddTagToChildren(WorldGenTags.IgnoreCaveOverride);
      ((VoronoiTree.Node) node.parent).tags.Remove(WorldGenTags.StartLocation);
    }

    private void ApplySwapTags()
    {
      List<VoronoiTree.Node> nodeList = new List<VoronoiTree.Node>();
      for (int index = 0; index < this.data.voronoiTree.ChildCount(); ++index)
      {
        if (this.data.voronoiTree.GetChild(index).tags.Contains(WorldGenTags.SwapLakesToBelow))
          nodeList.Add(this.data.voronoiTree.GetChild(index));
      }
      foreach (VoronoiTree.Node node in nodeList)
      {
        if (!node.tags.Contains(WorldGenTags.CenteralFeature))
        {
          List<VoronoiTree.Node> nodes = new List<VoronoiTree.Node>();
          ((Tree) node).GetNodesWithoutTag(WorldGenTags.CenteralFeature, nodes);
          this.SwapNodesAround(WorldGenTags.Wet, true, nodes, node.site.poly.Centroid());
        }
      }
    }

    private void SwapNodesAround(
      Tag swapTag,
      bool sendTagToBottom,
      List<VoronoiTree.Node> nodes,
      Vector2 pivot)
    {
      WorldGenUtil.ShuffleSeeded<VoronoiTree.Node>((IList<VoronoiTree.Node>) nodes, this.myRandom.RandomSource());
      List<VoronoiTree.Node> nodeList1 = new List<VoronoiTree.Node>();
      List<VoronoiTree.Node> nodeList2 = new List<VoronoiTree.Node>();
      foreach (VoronoiTree.Node node in nodes)
      {
        bool flag1 = node.tags.Contains(swapTag);
        bool flag2 = (double) node.site.poly.Centroid().y > (double) pivot.y;
        bool flag3 = flag2 & sendTagToBottom || !flag2 && !sendTagToBottom;
        if (flag1 & flag3)
        {
          if (nodeList2.Count > 0)
          {
            this.SwitchNodes(node, nodeList2[0]);
            nodeList2.RemoveAt(0);
          }
          else
            nodeList1.Add(node);
        }
        else if (!flag1 && !flag3)
        {
          if (nodeList1.Count > 0)
          {
            this.SwitchNodes(node, nodeList1[0]);
            nodeList1.RemoveAt(0);
          }
          else
            nodeList2.Add(node);
        }
      }
      if (nodeList2.Count <= 0)
        return;
      for (int index = 0; index < nodeList1.Count && nodeList2.Count > 0; ++index)
      {
        this.SwitchNodes(nodeList1[index], nodeList2[0]);
        nodeList2.RemoveAt(0);
      }
    }

    public void GetElementForBiomePoint(
      Chunk chunk,
      ElementBandConfiguration elementBands,
      Vector2I pos,
      out Element element,
      out Sim.PhysicsData pd,
      out Sim.DiseaseCell dc,
      float erode)
    {
      TerrainCell.GetElementOverride(WorldGen.voidElement.tag.ToString(), (SampleDescriber.Override) null);
      TerrainCell.ElementOverride biomeElementTable = this.GetElementFromBiomeElementTable(chunk, pos, (List<ElementGradient>) elementBands, erode);
      element = biomeElementTable.element;
      pd = biomeElementTable.pdelement;
      dc = biomeElementTable.dc;
    }

    public void ConvertIntersectingCellsToType(MathUtil.Pair<Vector2, Vector2> segment, string type)
    {
      List<Vector2I> line = Util.GetLine(segment.First, segment.Second);
      for (int index1 = 0; index1 < this.data.terrainCells.Count; ++index1)
      {
        if (((ProcGen.Node) this.data.terrainCells[index1].node).type != type)
        {
          for (int index2 = 0; index2 < line.Count; ++index2)
          {
            if (this.data.terrainCells[index1].poly.Contains(Vector2I.op_Implicit(line[index2])))
              ((ProcGen.Node) this.data.terrainCells[index1].node).SetType(type);
          }
        }
      }
    }

    public string GetSubWorldType(Vector2I pos)
    {
      for (int index = 0; index < this.data.overworldCells.Count; ++index)
      {
        if (this.data.overworldCells[index].poly.Contains(Vector2I.op_Implicit(pos)))
          return ((ProcGen.Node) this.data.overworldCells[index].node).type;
      }
      return (string) null;
    }

    private void ProcessByTerrainCell(
      Sim.Cell[] map_cells,
      float[] bgTemp,
      Sim.DiseaseCell[] dcs,
      WorldGen.OfflineCallbackFunction updateProgressFn,
      HashSet<int> hightPriorityCells)
    {
      int num1 = updateProgressFn(UI.WORLDGEN.PROCESSING.key, 0.0f, WorldGenProgressStages.Stages.Processing) ? 1 : 0;
      SeededRandom rnd = new SeededRandom(this.data.globalTerrainSeed);
      try
      {
        for (int index = 0; index < this.data.terrainCells.Count; ++index)
        {
          int num2 = updateProgressFn(UI.WORLDGEN.PROCESSING.key, (float) index / (float) this.data.terrainCells.Count, WorldGenProgressStages.Stages.Processing) ? 1 : 0;
          this.data.terrainCells[index].Process(this, map_cells, bgTemp, dcs, this.data.world, rnd);
        }
      }
      catch (Exception ex)
      {
        string message = ex.Message;
        string stackTrace = ex.StackTrace;
        int num3 = updateProgressFn(new StringKey("Exception in TerrainCell.Process"), -1f, WorldGenProgressStages.Stages.Failure) ? 1 : 0;
        Debug.LogError((object) ("Error:" + message + "\n" + stackTrace));
      }
      List<Border> borderList = new List<Border>();
      int num4 = updateProgressFn(UI.WORLDGEN.BORDERS.key, 0.0f, WorldGenProgressStages.Stages.Borders) ? 1 : 0;
      try
      {
        List<Edge> edgesWithTag1 = this.data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeUnpassable);
        for (int index = 0; index < edgesWithTag1.Count; ++index)
        {
          Edge edge = edgesWithTag1[index];
          List<Cell> cells = ((Graph<Cell, Edge>) this.data.worldLayout.overworldGraph).GetNodes(edge);
          Debug.Assert(cells[0] != cells[1], (object) "Both nodes on an arc were the same. Allegedly this means it was a world border but I don't think we do that anymore.");
          TerrainCell a = this.data.overworldCells.Find((Predicate<TerrainCell>) (c => c.node == cells[0]));
          TerrainCell b = this.data.overworldCells.Find((Predicate<TerrainCell>) (c => c.node == cells[1]));
          Debug.Assert(a != null && b != null, (object) "NULL Terrainell nodes with EdgeUnpassable");
          a.LogInfo("BORDER WITH " + b.site.id.ToString(), "UNPASSABLE", 0.0f);
          b.LogInfo("BORDER WITH " + a.site.id.ToString(), "UNPASSABLE", 0.0f);
          borderList.Add(new Border(new Neighbors(a, b), edge.corner0.position, edge.corner1.position)
          {
            element = SettingsCache.borders["impenetrable"],
            width = (float) rnd.RandomRange(2, 3)
          });
        }
        List<Edge> edgesWithTag2 = this.data.worldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeClosed);
        for (int index = 0; index < edgesWithTag2.Count; ++index)
        {
          Edge edge = edgesWithTag2[index];
          if (!edgesWithTag1.Contains(edge))
          {
            List<Cell> cells = ((Graph<Cell, Edge>) this.data.worldLayout.overworldGraph).GetNodes(edge);
            Debug.Assert(cells[0] != cells[1], (object) "Both nodes on an arc were the same. Allegedly this means it was a world border but I don't think we do that anymore.");
            TerrainCell a = this.data.overworldCells.Find((Predicate<TerrainCell>) (c => c.node == cells[0]));
            TerrainCell b = this.data.overworldCells.Find((Predicate<TerrainCell>) (c => c.node == cells[1]));
            Debug.Assert(a != null && b != null, (object) "NULL Terraincell nodes with EdgeClosed");
            string borderOverride1 = this.Settings.GetSubWorld(((ProcGen.Node) a.node).type).borderOverride;
            string borderOverride2 = this.Settings.GetSubWorld(((ProcGen.Node) b.node).type).borderOverride;
            string str;
            if (!string.IsNullOrEmpty(borderOverride2) && !string.IsNullOrEmpty(borderOverride1))
            {
              int overridePriority1 = this.Settings.GetSubWorld(((ProcGen.Node) a.node).type).borderOverridePriority;
              int overridePriority2 = this.Settings.GetSubWorld(((ProcGen.Node) b.node).type).borderOverridePriority;
              if (overridePriority1 == overridePriority2)
              {
                str = (double) rnd.RandomValue() > 0.5 ? borderOverride2 : borderOverride1;
                a.LogInfo("BORDER WITH " + b.site.id.ToString(), "Picked Random:" + str, 0.0f);
                b.LogInfo("BORDER WITH " + a.site.id.ToString(), "Picked Random:" + str, 0.0f);
              }
              else
              {
                str = overridePriority1 > overridePriority2 ? borderOverride1 : borderOverride2;
                a.LogInfo("BORDER WITH " + b.site.id.ToString(), "Picked priority:" + str, 0.0f);
                b.LogInfo("BORDER WITH " + a.site.id.ToString(), "Picked priority:" + str, 0.0f);
              }
            }
            else if (string.IsNullOrEmpty(borderOverride2) && string.IsNullOrEmpty(borderOverride1))
            {
              str = "hardToDig";
              a.LogInfo("BORDER WITH " + b.site.id.ToString(), "Both null", 0.0f);
              b.LogInfo("BORDER WITH " + a.site.id.ToString(), "Both null", 0.0f);
            }
            else
            {
              str = !string.IsNullOrEmpty(borderOverride2) ? borderOverride2 : borderOverride1;
              a.LogInfo("BORDER WITH " + b.site.id.ToString(), "Picked specific " + str, 0.0f);
              b.LogInfo("BORDER WITH " + a.site.id.ToString(), "Picked specific " + str, 0.0f);
            }
            if (!(str == "NONE"))
            {
              Border border = new Border(new Neighbors(a, b), edge.corner0.position, edge.corner1.position);
              border.element = SettingsCache.borders[str];
              MinMax minMax;
              // ISSUE: explicit constructor call
              ((MinMax) ref minMax).\u002Ector(1.5f, 2f);
              MinMax borderSizeOverride1 = this.Settings.GetSubWorld(((ProcGen.Node) a.node).type).borderSizeOverride;
              MinMax borderSizeOverride2 = this.Settings.GetSubWorld(((ProcGen.Node) b.node).type).borderSizeOverride;
              bool flag1 = (double) ((MinMax) ref borderSizeOverride1).min != 0.0 || (double) ((MinMax) ref borderSizeOverride1).max != 0.0;
              bool flag2 = (double) ((MinMax) ref borderSizeOverride2).min != 0.0 || (double) ((MinMax) ref borderSizeOverride2).max != 0.0;
              if (flag1 & flag2)
                minMax = (double) ((MinMax) ref borderSizeOverride1).max > (double) ((MinMax) ref borderSizeOverride2).max ? borderSizeOverride1 : borderSizeOverride2;
              else if (flag1)
                minMax = borderSizeOverride1;
              else if (flag2)
                minMax = borderSizeOverride2;
              border.width = rnd.RandomRange(((MinMax) ref minMax).min, ((MinMax) ref minMax).max);
              borderList.Add(border);
            }
          }
        }
      }
      catch (Exception ex)
      {
        string message = ex.Message;
        string stackTrace = ex.StackTrace;
        int num5 = updateProgressFn(new StringKey("Exception in Border creation"), -1f, WorldGenProgressStages.Stages.Failure) ? 1 : 0;
        Debug.LogError((object) ("Error:" + message + " " + stackTrace));
      }
      try
      {
        if (this.data.world.defaultTemp == null)
          this.data.world.defaultTemp = new float[this.data.world.density.Length];
        for (int index = 0; index < this.data.world.defaultTemp.Length; ++index)
          this.data.world.defaultTemp[index] = bgTemp[index];
      }
      catch (Exception ex)
      {
        string message = ex.Message;
        string stackTrace = ex.StackTrace;
        int num6 = updateProgressFn(new StringKey("Exception in border.defaultTemp"), -1f, WorldGenProgressStages.Stages.Failure) ? 1 : 0;
        Debug.LogError((object) ("Error:" + message + " " + stackTrace));
      }
      try
      {
        TerrainCell.SetValuesFunction SetValues = (TerrainCell.SetValuesFunction) ((index, elem, pd, dc) =>
        {
          if (Grid.IsValidCell(index))
          {
            if (this.highPriorityClaims.Contains(index))
              return;
            if ((elem as Element).HasTag(GameTags.Special))
              pd = (elem as Element).defaultValues;
            map_cells[index].SetValues(elem as Element, pd, ElementLoader.elements);
            dcs[index] = dc;
          }
          else
            Debug.LogError((object) ("Process::SetValuesFunction Index [" + index.ToString() + "] is not valid. cells.Length [" + map_cells.Length.ToString() + "]"));
        });
        for (int index = 0; index < borderList.Count; ++index)
        {
          Border border = borderList[index];
          SubWorld subWorld1 = this.Settings.GetSubWorld(((ProcGen.Node) border.neighbors.n0.node).type);
          SubWorld subWorld2 = this.Settings.GetSubWorld(((ProcGen.Node) border.neighbors.n1.node).type);
          float neighbour0Temperature = (float) (((double) SettingsCache.temperatures[subWorld1.temperatureRange].min + (double) SettingsCache.temperatures[subWorld1.temperatureRange].max) / 2.0);
          float neighbour1Temperature = (float) (((double) SettingsCache.temperatures[subWorld2.temperatureRange].min + (double) SettingsCache.temperatures[subWorld2.temperatureRange].max) / 2.0);
          float num7 = Mathf.Min(SettingsCache.temperatures[subWorld1.temperatureRange].min, SettingsCache.temperatures[subWorld2.temperatureRange].min);
          double num8 = (double) Mathf.Max(SettingsCache.temperatures[subWorld1.temperatureRange].max, SettingsCache.temperatures[subWorld2.temperatureRange].max);
          float midTemp = (float) (((double) neighbour0Temperature + (double) neighbour1Temperature) / 2.0);
          double num9 = (double) num7;
          double num10 = num8 - num9;
          float num11 = 2f;
          float num12 = 5f;
          int snapLastCells = 1;
          if (num10 >= 150.0)
          {
            num11 = 0.0f;
            num12 = border.width * 0.2f;
            snapLastCells = 2;
            border.width = Mathf.Max(border.width, 2f);
            double num13 = (double) neighbour0Temperature - 273.14999389648438;
            float num14 = neighbour1Temperature - 273.15f;
            midTemp = (double) Mathf.Abs((float) num13) >= (double) Mathf.Abs(num14) ? neighbour1Temperature : neighbour0Temperature;
          }
          border.Stagger(rnd, (float) rnd.RandomRange(8, 13), rnd.RandomRange(num11, num12));
          border.ConvertToMap(this.data.world, SetValues, neighbour0Temperature, neighbour1Temperature, midTemp, rnd, snapLastCells);
        }
      }
      catch (Exception ex)
      {
        string message = ex.Message;
        string stackTrace = ex.StackTrace;
        int num15 = updateProgressFn(new StringKey("Exception in border.ConvertToMap"), -1f, WorldGenProgressStages.Stages.Failure) ? 1 : 0;
        Debug.LogError((object) ("Error:" + message + " " + stackTrace));
      }
    }

    private void DrawWorldBorder(
      Sim.Cell[] cells,
      Chunk world,
      SeededRandom rnd,
      ref HashSet<int> borderCells,
      ref List<RectInt> poiBounds,
      WorldGen.OfflineCallbackFunction updateProgressFn)
    {
      bool boolSetting = this.Settings.GetBoolSetting("DrawWorldBorderForce");
      int intSetting1 = this.Settings.GetIntSetting("WorldBorderThickness");
      int intSetting2 = this.Settings.GetIntSetting("WorldBorderRange");
      ushort idx1 = WorldGen.vacuumElement.idx;
      ushort idx2 = WorldGen.voidElement.idx;
      ushort idx3 = WorldGen.unobtaniumElement.idx;
      float temperature = WorldGen.unobtaniumElement.defaultValues.temperature;
      float mass = WorldGen.unobtaniumElement.defaultValues.mass;
      int num1 = 0;
      int num2 = 0;
      int num3 = updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, 0.0f, WorldGenProgressStages.Stages.DrawWorldBorder) ? 1 : 0;
      int num4 = world.size.y - 1;
      int num5 = 0;
      int num6 = world.size.x - 1;
      List<TerrainCell> terrainCellsForTag = this.GetTerrainCellsForTag(WorldGenTags.RemoveWorldBorderOverVacuum);
      for (int y = num4; y >= 0; y--)
      {
        int num7 = updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, (float) ((double) y / (double) num4 * 0.33000001311302185), WorldGenProgressStages.Stages.DrawWorldBorder) ? 1 : 0;
        num1 = Mathf.Max(-intSetting2, Mathf.Min(num1 + rnd.RandomRange(-2, 2), intSetting2));
        bool flag1 = terrainCellsForTag.Find((Predicate<TerrainCell>) (n => n.poly.Contains(new Vector2(0.0f, (float) y)))) != null;
        for (int x = 0; x < intSetting1 + num1; ++x)
        {
          int cell = Grid.XYToCell(x, y);
          if (boolSetting || (((int) cells[cell].elementIdx == (int) idx1 ? 0 : ((int) cells[cell].elementIdx != (int) idx2 ? 1 : 0)) & (flag1 ? 1 : 0)) != 0 || !flag1)
          {
            borderCells.Add(cell);
            cells[cell].SetValues(idx3, temperature, mass);
            num5 = Mathf.Max(num5, x);
          }
        }
        num2 = Mathf.Max(-intSetting2, Mathf.Min(num2 + rnd.RandomRange(-2, 2), intSetting2));
        bool flag2 = terrainCellsForTag.Find((Predicate<TerrainCell>) (n => n.poly.Contains(new Vector2((float) (world.size.x - 1), (float) y)))) != null;
        for (int index = 0; index < intSetting1 + num2; ++index)
        {
          int x = world.size.x - 1 - index;
          int cell = Grid.XYToCell(x, y);
          if (boolSetting || (((int) cells[cell].elementIdx == (int) idx1 ? 0 : ((int) cells[cell].elementIdx != (int) idx2 ? 1 : 0)) & (flag2 ? 1 : 0)) != 0 || !flag2)
          {
            borderCells.Add(cell);
            cells[cell].SetValues(idx3, temperature, mass);
            num6 = Mathf.Min(num6, x);
          }
        }
      }
      this.POIBounds.Add(new RectInt(0, 0, num5 + 1, this.World.size.y));
      this.POIBounds.Add(new RectInt(num6, 0, world.size.x - num6, this.World.size.y));
      int num8 = 0;
      int num9 = 0;
      int num10 = 0;
      int num11 = this.World.size.y - 1;
      for (int x = 0; x < world.size.x; x++)
      {
        int num12 = updateProgressFn(UI.WORLDGEN.DRAWWORLDBORDER.key, (float) ((double) x / (double) world.size.x * 0.6600000262260437 + 0.33000001311302185), WorldGenProgressStages.Stages.DrawWorldBorder) ? 1 : 0;
        num8 = Mathf.Max(-intSetting2, Mathf.Min(num8 + rnd.RandomRange(-2, 2), intSetting2));
        bool flag3 = terrainCellsForTag.Find((Predicate<TerrainCell>) (n => n.poly.Contains(new Vector2((float) x, 0.0f)))) != null;
        for (int y = 0; y < intSetting1 + num8; ++y)
        {
          int cell = Grid.XYToCell(x, y);
          if (boolSetting || (((int) cells[cell].elementIdx == (int) idx1 ? 0 : ((int) cells[cell].elementIdx != (int) idx2 ? 1 : 0)) & (flag3 ? 1 : 0)) != 0 || !flag3)
          {
            borderCells.Add(cell);
            cells[cell].SetValues(idx3, temperature, mass);
            num10 = Mathf.Max(num10, y);
          }
        }
        num9 = Mathf.Max(-intSetting2, Mathf.Min(num9 + rnd.RandomRange(-2, 2), intSetting2));
        bool flag4 = terrainCellsForTag.Find((Predicate<TerrainCell>) (n => n.poly.Contains(new Vector2((float) x, (float) (world.size.y - 1))))) != null;
        for (int index = 0; index < intSetting1 + num9; ++index)
        {
          int y = world.size.y - 1 - index;
          int cell = Grid.XYToCell(x, y);
          if (boolSetting || (((int) cells[cell].elementIdx == (int) idx1 ? 0 : ((int) cells[cell].elementIdx != (int) idx2 ? 1 : 0)) & (flag4 ? 1 : 0)) != 0 || !flag4)
          {
            borderCells.Add(cell);
            cells[cell].SetValues(idx3, temperature, mass);
            num11 = Mathf.Min(num11, y);
          }
        }
      }
      this.POIBounds.Add(new RectInt(0, 0, this.World.size.x, num10 + 1));
      this.POIBounds.Add(new RectInt(0, num11, this.World.size.x, this.World.size.y - num11));
    }

    private void SetupNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
    {
      int num1 = updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 0.0f, WorldGenProgressStages.Stages.SetupNoise) ? 1 : 0;
      this.heatSource = this.BuildNoiseSource(this.data.world.size.x, this.data.world.size.y, "noise/Heat");
      int num2 = updateProgressFn(UI.WORLDGEN.BUILDNOISESOURCE.key, 1f, WorldGenProgressStages.Stages.SetupNoise) ? 1 : 0;
    }

    public NoiseMapBuilderPlane BuildNoiseSource(int width, int height, string name)
    {
      Tree tree = SettingsCache.noise.GetTree(name);
      Debug.Assert(tree != null, (object) name);
      return this.BuildNoiseSource(width, height, tree);
    }

    public NoiseMapBuilderPlane BuildNoiseSource(int width, int height, Tree tree)
    {
      Vector2f lowerBound = tree.settings.lowerBound;
      Vector2f upperBound = tree.settings.upperBound;
      Debug.Assert(((double) lowerBound.x < (double) upperBound.x ? 1 : 0) != 0, (object) ("BuildNoiseSource X range broken [l: " + lowerBound.x.ToString() + " h: " + upperBound.x.ToString() + "]"));
      Debug.Assert(((double) lowerBound.y < (double) upperBound.y ? 1 : 0) != 0, (object) ("BuildNoiseSource Y range broken [l: " + lowerBound.y.ToString() + " h: " + upperBound.y.ToString() + "]"));
      Debug.Assert(width > 0, (object) ("BuildNoiseSource width <=0: [" + width.ToString() + "]"));
      Debug.Assert(height > 0, (object) ("BuildNoiseSource height <=0: [" + height.ToString() + "]"));
      NoiseMapBuilderPlane noiseMapBuilderPlane = new NoiseMapBuilderPlane(lowerBound.x, upperBound.x, lowerBound.y, upperBound.y, false);
      ((NoiseMapBuilder) noiseMapBuilderPlane).SetSize(width, height);
      ((NoiseMapBuilder) noiseMapBuilderPlane).SourceModule = (IModule) tree.BuildFinalModule(this.data.globalNoiseSeed);
      return noiseMapBuilderPlane;
    }

    private void GetMinMaxDataValues(float[] data, int width, int height)
    {
    }

    public static NoiseMap BuildNoiseMap(
      Vector2 offset,
      float zoom,
      NoiseMapBuilderPlane nmbp,
      int width,
      int height,
      NoiseMapBuilderCallback cb = null)
    {
      double x = (double) offset.x;
      double y = (double) offset.y;
      if ((double) zoom == 0.0)
        zoom = 0.01f;
      double num1 = x * (double) zoom;
      double num2 = (x + (double) width) * (double) zoom;
      double num3 = y * (double) zoom;
      double num4 = (y + (double) height) * (double) zoom;
      NoiseMap noiseMap = new NoiseMap(width, height);
      ((NoiseMapBuilder) nmbp).NoiseMap = (IMap2D<float>) noiseMap;
      nmbp.SetBounds((float) num1, (float) num2, (float) num3, (float) num4);
      ((NoiseMapBuilder) nmbp).CallBack = cb;
      ((NoiseMapBuilder) nmbp).Build();
      return noiseMap;
    }

    public static float[] GenerateNoise(
      Vector2 offset,
      float zoom,
      NoiseMapBuilderPlane nmbp,
      int width,
      int height,
      NoiseMapBuilderCallback cb = null)
    {
      NoiseMap noiseMap = WorldGen.BuildNoiseMap(offset, zoom, nmbp, width, height, cb);
      float[] noise = new float[((DataMap<float>) noiseMap).Width * ((DataMap<float>) noiseMap).Height];
      ((DataMap<float>) noiseMap).CopyTo(ref noise);
      return noise;
    }

    public static void Normalise(float[] data)
    {
      Debug.Assert(data != null && data.Length != 0, (object) "MISSING DATA FOR NORMALIZE");
      float num1 = float.MaxValue;
      float num2 = float.MinValue;
      for (int index = 0; index < data.Length; ++index)
      {
        num1 = Mathf.Min(data[index], num1);
        num2 = Mathf.Max(data[index], num2);
      }
      float num3 = num2 - num1;
      for (int index = 0; index < data.Length; ++index)
        data[index] = (data[index] - num1) / num3;
    }

    private void GenerateUnChunkedNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      WorldGen.\u003C\u003Ec__DisplayClass148_0 displayClass1480 = new WorldGen.\u003C\u003Ec__DisplayClass148_0();
      // ISSUE: reference to a compiler-generated field
      displayClass1480.updateProgressFn = updateProgressFn;
      // ISSUE: reference to a compiler-generated field
      displayClass1480.\u003C\u003E4__this = this;
      Vector2 offset;
      // ISSUE: explicit constructor call
      ((Vector2) ref offset).\u002Ector(0.0f, 0.0f);
      // ISSUE: reference to a compiler-generated field
      int num1 = displayClass1480.updateProgressFn(UI.WORLDGEN.GENERATENOISE.key, 0.0f, WorldGenProgressStages.Stages.GenerateNoise) ? 1 : 0;
      // ISSUE: method pointer
      NoiseMapBuilderCallback mapBuilderCallback = new NoiseMapBuilderCallback((object) displayClass1480, __methodptr(\u003CGenerateUnChunkedNoise\u003Eb__0));
      // ISSUE: method pointer
      NoiseMapBuilderCallback cb = new NoiseMapBuilderCallback((object) displayClass1480, __methodptr(\u003CGenerateUnChunkedNoise\u003Eb__1));
      if (cb == null)
        Debug.LogError((object) "nupd is null");
      this.data.world.heatOffset = WorldGen.GenerateNoise(offset, SettingsCache.noise.GetZoomForTree("noise/Heat"), this.heatSource, this.data.world.size.x, this.data.world.size.y, cb);
      this.data.world.data = new float[this.data.world.heatOffset.Length];
      this.data.world.density = new float[this.data.world.heatOffset.Length];
      this.data.world.overrides = new float[this.data.world.heatOffset.Length];
      // ISSUE: reference to a compiler-generated field
      int num2 = displayClass1480.updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 0.5f, WorldGenProgressStages.Stages.GenerateNoise) ? 1 : 0;
      if (SettingsCache.noise.ShouldNormaliseTree("noise/Heat"))
        WorldGen.Normalise(this.data.world.heatOffset);
      // ISSUE: reference to a compiler-generated field
      int num3 = displayClass1480.updateProgressFn(UI.WORLDGEN.NORMALISENOISE.key, 1f, WorldGenProgressStages.Stages.GenerateNoise) ? 1 : 0;
    }

    public void WriteOverWorldNoise(WorldGen.OfflineCallbackFunction updateProgressFn)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      WorldGen.\u003C\u003Ec__DisplayClass150_0 displayClass1500 = new WorldGen.\u003C\u003Ec__DisplayClass150_0();
      // ISSUE: reference to a compiler-generated field
      displayClass1500.updateProgressFn = updateProgressFn;
      Dictionary<HashedString, WorldGen.NoiseNormalizationStats> dictionary = new Dictionary<HashedString, WorldGen.NoiseNormalizationStats>();
      float count = (float) this.OverworldCells.Count;
      // ISSUE: reference to a compiler-generated field
      displayClass1500.perCell = 1f / count;
      // ISSUE: reference to a compiler-generated field
      displayClass1500.currentProgress = 0.0f;
      using (List<TerrainCell>.Enumerator enumerator = this.OverworldCells.GetEnumerator())
      {
label_33:
        while (enumerator.MoveNext())
        {
          TerrainCell current = enumerator.Current;
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: variable of a compiler-generated type
          WorldGen.\u003C\u003Ec__DisplayClass150_1 displayClass1501 = new WorldGen.\u003C\u003Ec__DisplayClass150_1();
          // ISSUE: reference to a compiler-generated field
          displayClass1501.CS\u0024\u003C\u003E8__locals1 = displayClass1500;
          Tree tree1 = SettingsCache.noise.GetTree("noise/Default");
          Tree tree2 = SettingsCache.noise.GetTree("noise/DefaultCave");
          Tree tree3 = SettingsCache.noise.GetTree("noise/DefaultDensity");
          string str1 = "noise/Default";
          string str2 = "noise/DefaultCave";
          string str3 = "noise/DefaultDensity";
          SubWorld subWorld = this.Settings.GetSubWorld(((ProcGen.Node) current.node).type);
          if (subWorld == null)
          {
            Debug.Log((object) ("Couldnt find Subworld for overworld node [" + ((ProcGen.Node) current.node).type + "] using defaults"));
          }
          else
          {
            if (subWorld.biomeNoise != null)
            {
              Tree tree4 = SettingsCache.noise.GetTree(subWorld.biomeNoise);
              if (tree4 != null)
              {
                tree1 = tree4;
                str1 = subWorld.biomeNoise;
              }
            }
            if (subWorld.overrideNoise != null)
            {
              Tree tree5 = SettingsCache.noise.GetTree(subWorld.overrideNoise);
              if (tree5 != null)
              {
                tree2 = tree5;
                str2 = subWorld.overrideNoise;
              }
            }
            if (subWorld.densityNoise != null)
            {
              Tree tree6 = SettingsCache.noise.GetTree(subWorld.densityNoise);
              if (tree6 != null)
              {
                tree3 = tree6;
                str3 = subWorld.densityNoise;
              }
            }
          }
          WorldGen.NoiseNormalizationStats normalizationStats1;
          if (!dictionary.TryGetValue(HashedString.op_Implicit(str1), out normalizationStats1))
          {
            normalizationStats1 = new WorldGen.NoiseNormalizationStats(this.BaseNoiseMap);
            dictionary.Add(HashedString.op_Implicit(str1), normalizationStats1);
          }
          WorldGen.NoiseNormalizationStats normalizationStats2;
          if (!dictionary.TryGetValue(HashedString.op_Implicit(str2), out normalizationStats2))
          {
            normalizationStats2 = new WorldGen.NoiseNormalizationStats(this.OverrideMap);
            dictionary.Add(HashedString.op_Implicit(str2), normalizationStats2);
          }
          WorldGen.NoiseNormalizationStats normalizationStats3;
          if (!dictionary.TryGetValue(HashedString.op_Implicit(str3), out normalizationStats3))
          {
            normalizationStats3 = new WorldGen.NoiseNormalizationStats(this.DensityMap);
            dictionary.Add(HashedString.op_Implicit(str3), normalizationStats3);
          }
          Rect bounds = current.poly.bounds;
          int width = (int) Mathf.Ceil(((Rect) ref bounds).width + 2f);
          bounds = current.poly.bounds;
          int num1 = (int) Mathf.Ceil(((Rect) ref bounds).height + 2f);
          // ISSUE: reference to a compiler-generated field
          displayClass1501.height = num1;
          bounds = current.poly.bounds;
          int num2 = (int) Mathf.Floor(((Rect) ref bounds).xMin - 1f);
          bounds = current.poly.bounds;
          int num3 = (int) Mathf.Floor(((Rect) ref bounds).yMin - 1f);
          Vector2 offset;
          Vector2 vector2 = offset = new Vector2((float) num2, (float) num3);
          // ISSUE: method pointer
          NoiseMapBuilderCallback cb = new NoiseMapBuilderCallback((object) displayClass1501, __methodptr(\u003CWriteOverWorldNoise\u003Eb__0));
          // ISSUE: reference to a compiler-generated field
          NoiseMapBuilderPlane nmbp1 = this.BuildNoiseSource(width, displayClass1501.height, tree1);
          // ISSUE: reference to a compiler-generated field
          NoiseMap noiseMap1 = WorldGen.BuildNoiseMap(offset, tree1.settings.zoom, nmbp1, width, displayClass1501.height, cb);
          // ISSUE: reference to a compiler-generated field
          NoiseMapBuilderPlane nmbp2 = this.BuildNoiseSource(width, displayClass1501.height, tree2);
          // ISSUE: reference to a compiler-generated field
          NoiseMap noiseMap2 = WorldGen.BuildNoiseMap(offset, tree2.settings.zoom, nmbp2, width, displayClass1501.height, cb);
          // ISSUE: reference to a compiler-generated field
          NoiseMapBuilderPlane nmbp3 = this.BuildNoiseSource(width, displayClass1501.height, tree3);
          // ISSUE: reference to a compiler-generated field
          NoiseMap noiseMap3 = WorldGen.BuildNoiseMap(offset, tree3.settings.zoom, nmbp3, width, displayClass1501.height, cb);
          ref Vector2 local1 = ref vector2;
          bounds = current.poly.bounds;
          double num4 = (double) (int) Mathf.Floor(((Rect) ref bounds).xMin);
          local1.x = (float) num4;
          while (true)
          {
            double x = (double) vector2.x;
            bounds = current.poly.bounds;
            double num5 = (double) (int) Mathf.Ceil(((Rect) ref bounds).xMax);
            if (x <= num5)
            {
              ref Vector2 local2 = ref vector2;
              bounds = current.poly.bounds;
              double num6 = (double) (int) Mathf.Floor(((Rect) ref bounds).yMin);
              local2.y = (float) num6;
              while (true)
              {
                double y = (double) vector2.y;
                bounds = current.poly.bounds;
                double num7 = (double) (int) Mathf.Ceil(((Rect) ref bounds).yMax);
                if (y <= num7)
                {
                  if (current.poly.PointInPolygon(vector2))
                  {
                    int cell = Grid.XYToCell((int) vector2.x, (int) vector2.y);
                    if (tree1.settings.normalise)
                      normalizationStats1.cells.Add(cell);
                    if (tree2.settings.normalise)
                      normalizationStats2.cells.Add(cell);
                    if (tree3.settings.normalise)
                      normalizationStats3.cells.Add(cell);
                    int num8 = (int) vector2.x - num2;
                    int num9 = (int) vector2.y - num3;
                    this.BaseNoiseMap[cell] = ((DataMap<float>) noiseMap1).GetValue(num8, num9);
                    this.OverrideMap[cell] = ((DataMap<float>) noiseMap2).GetValue(num8, num9);
                    this.DensityMap[cell] = ((DataMap<float>) noiseMap3).GetValue(num8, num9);
                    normalizationStats1.min = Mathf.Min(this.BaseNoiseMap[cell], normalizationStats1.min);
                    normalizationStats1.max = Mathf.Max(this.BaseNoiseMap[cell], normalizationStats1.max);
                    normalizationStats2.min = Mathf.Min(this.OverrideMap[cell], normalizationStats2.min);
                    normalizationStats2.max = Mathf.Max(this.OverrideMap[cell], normalizationStats2.max);
                    normalizationStats3.min = Mathf.Min(this.DensityMap[cell], normalizationStats3.min);
                    normalizationStats3.max = Mathf.Max(this.DensityMap[cell], normalizationStats3.max);
                  }
                  ++vector2.y;
                }
                else
                  break;
              }
              ++vector2.x;
            }
            else
              goto label_33;
          }
        }
      }
      foreach (KeyValuePair<HashedString, WorldGen.NoiseNormalizationStats> keyValuePair in dictionary)
      {
        float num = keyValuePair.Value.max - keyValuePair.Value.min;
        foreach (int cell in keyValuePair.Value.cells)
          keyValuePair.Value.noise[cell] = (keyValuePair.Value.noise[cell] - keyValuePair.Value.min) / num;
      }
    }

    private float GetValue(Chunk chunk, Vector2I pos)
    {
      int index = pos.x + this.data.world.size.x * pos.y;
      if (index < 0 || index >= chunk.data.Length)
        throw new ArgumentOutOfRangeException("chunkDataIndex [" + index.ToString() + "]", "chunk data length [" + chunk.data.Length.ToString() + "]");
      return chunk.data[index];
    }

    public bool InChunkRange(Chunk chunk, Vector2I pos)
    {
      int num = pos.x + this.data.world.size.x * pos.y;
      return num >= 0 && num < chunk.data.Length;
    }

    private TerrainCell.ElementOverride GetElementFromBiomeElementTable(
      Chunk chunk,
      Vector2I pos,
      List<ElementGradient> table,
      float erode)
    {
      float num = this.GetValue(chunk, pos) * erode;
      TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(WorldGen.voidElement.tag.ToString(), (SampleDescriber.Override) null);
      if (table.Count == 0)
        return elementOverride;
      for (int index = 0; index < table.Count; ++index)
      {
        Debug.Assert(((Gradient<string>) table[index]).content != null, (object) index.ToString());
        if ((double) num < (double) ((Gradient<string>) table[index]).maxValue)
          return TerrainCell.GetElementOverride(((Gradient<string>) table[index]).content, table[index].overrides);
      }
      return TerrainCell.GetElementOverride(((Gradient<string>) table[table.Count - 1]).content, table[table.Count - 1].overrides);
    }

    public static bool CanLoad(string fileName)
    {
      if (fileName != null)
      {
        if (!(fileName == ""))
        {
          try
          {
            if (!File.Exists(fileName))
              return false;
            using (BinaryReader binaryReader = new BinaryReader((Stream) File.Open(fileName, FileMode.Open)))
              return binaryReader.BaseStream.CanRead;
          }
          catch (FileNotFoundException ex)
          {
            return false;
          }
          catch (Exception ex)
          {
            DebugUtil.LogWarningArgs(new object[1]
            {
              (object) ("Failed to read " + fileName + "\n" + ex.ToString())
            });
            return false;
          }
        }
      }
      return false;
    }

    public void SaveWorldGen()
    {
      try
      {
        Manager.Clear();
        WorldGenSave worldGenSave = new WorldGenSave();
        worldGenSave.version = new Vector2I(1, 1);
        worldGenSave.stats = this.stats;
        worldGenSave.data = this.data;
        worldGenSave.worldID = this.Settings.world.filePath;
        worldGenSave.traitIDs = new List<string>((IEnumerable<string>) this.Settings.GetWorldTraitIDs());
        worldGenSave.storyTraitIDs = new List<string>((IEnumerable<string>) this.Settings.GetStoryTraitIDs());
        using (MemoryStream output = new MemoryStream())
        {
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
          {
            try
            {
              Serializer.Serialize((object) worldGenSave, binaryWriter);
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

    public static WorldGen Load(IReader reader, bool defaultDiscovered)
    {
      try
      {
        WorldGenSave worldGenSave = new WorldGenSave();
        Deserializer.Deserialize((object) worldGenSave, reader);
        WorldGen worldGen = new WorldGen(worldGenSave.worldID, worldGenSave.data, worldGenSave.stats, worldGenSave.traitIDs, worldGenSave.storyTraitIDs, false);
        worldGen.isStartingWorld = true;
        if (worldGenSave.version.x != 1 || worldGenSave.version.y > 1)
        {
          DebugUtil.LogErrorArgs(new object[1]
          {
            (object) ("LoadWorldGenSim Error! Wrong save version Current: [" + 1.ToString() + "." + 1.ToString() + "] File: [" + worldGenSave.version.x.ToString() + "." + worldGenSave.version.y.ToString() + "]")
          });
          worldGen.wasLoaded = false;
        }
        else
          worldGen.wasLoaded = true;
        return worldGen;
      }
      catch (Exception ex)
      {
        DebugUtil.LogErrorArgs(new object[3]
        {
          (object) "WorldGen.Load Error!\n",
          (object) ex.Message,
          (object) ex.StackTrace
        });
        return (WorldGen) null;
      }
    }

    public void DrawDebug()
    {
    }

    public delegate bool OfflineCallbackFunction(
      StringKey stringKeyRoot,
      float completePercent,
      WorldGenProgressStages.Stages stage);

    public enum GenerateSection
    {
      SolarSystem,
      WorldNoise,
      WorldLayout,
      RenderToMap,
      CollectSpawners,
    }

    private class NoiseNormalizationStats
    {
      public float[] noise;
      public float min = float.MaxValue;
      public float max = float.MinValue;
      public HashSet<int> cells = new HashSet<int>();

      public NoiseNormalizationStats(float[] noise) => this.noise = noise;
    }
  }
}
