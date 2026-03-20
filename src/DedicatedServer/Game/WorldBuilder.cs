using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Database;
using Klei;
using ProcGen;
using ProcGenGame;
using UnityEngine;
using Random = System.Random;

namespace DedicatedServer.Game;

/// <summary>
/// Creates the ONI game world. Delegates to game code as much as possible.
/// </summary>
public class WorldBuilder {

    public int Width { get; private set; } = 256;
    public int Height { get; private set; } = 384;
    public bool IsLoaded { get; private set; }
    public bool SimRunning { get; private set; }
    public int SimTick { get; private set; }
    public GameSpawnData SpawnData { get; private set; }

    public unsafe void TickSimulation() {
        if (!SimRunning) return;
        SimTick++;
        var activeRegions = new List<global::Game.SimActiveRegion> {
            new() { region = new Pair<Vector2I, Vector2I>(new Vector2I(0, 0), new Vector2I(Width, Height)) }
        };
        SimMessages.NewGameFrame(0.2f, activeRegions);
        var visible = new byte[Grid.CellCount];
        for (var i = 0; i < visible.Length; i++) visible[i] = byte.MaxValue;
        var ptr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, visible.Length, visible);
        if (ptr != IntPtr.Zero) {
            var update = (Sim.GameDataUpdate*)(void*)ptr;
            Grid.elementIdx = update->elementIdx;
            Grid.temperature = update->temperature;
            Grid.mass = update->mass;
            Grid.radiation = update->radiation;
        }
    }

    private static readonly Dictionary<string, BuildingDef> _buildingDefCache = new Dictionary<string, BuildingDef>();

    public BuildingDef GetBuildingDef(string id) {
        if (_buildingDefCache.TryGetValue(id, out var def)) return def;
        return Assets.GetBuildingDef(id);
    }

    public void Create(ResourceLoader resources) {
        Console.WriteLine("[WorldBuilder] Initializing game...");
        InitializeWorld(resources);

        Console.WriteLine("[WorldBuilder] Registering buildings...");
        RegisterBuildingDefs();

        Console.WriteLine("[WorldBuilder] Generating world...");
        Sim.Cell[] generatedCells = null;

        var cluster = new Cluster(
            "clusters/SandstoneDefault", 42,
            new List<string>(), assertMissingTraits: false, skipWorldTraits: true);

        // Capture cell data when WorldGen completes
        cluster.PerWorldGenCompleteCallback = (idx, worldGen, cells, diseaseCells) => {
            generatedCells = cells;
            Console.WriteLine($"[WorldBuilder] WorldGen callback: {cells?.Length ?? 0} cells");
        };

        cluster.Generate(
            (key, pct, stage) => { Console.WriteLine($"[WorldGen] {stage}: {pct:P0}"); return true; },
            error => Console.WriteLine($"[WorldGen] ERROR: {error.errorDesc}"),
            worldSeed: 42, layoutSeed: 42, terrainSeed: 42, noiseSeed: 42);

        while (!cluster.IsGenerationComplete) {
            System.Threading.Thread.Sleep(100);
        }
        // Wait for file to be released by generation thread
        System.Threading.Thread.Sleep(500);

        Width = cluster.size.x;
        Height = cluster.size.y;
        AllocateGrid(Width, Height);

        Console.WriteLine($"[WorldBuilder] Callback assigned: {cluster.PerWorldGenCompleteCallback != null}, cells captured: {generatedCells != null}");

        // Copy generated cell data into Grid
        if (generatedCells != null) {
            var n = Math.Min(generatedCells.Length, Width * Height);
            unsafe {
                for (var i = 0; i < n; i++) {
                    Grid.elementIdx[i] = generatedCells[i].elementIdx;
                    Grid.temperature[i] = generatedCells[i].temperature;
                    Grid.mass[i] = generatedCells[i].mass;
                }
            }
            Console.WriteLine($"[WorldBuilder] Copied {n} cells to Grid");
        }

        SpawnData = cluster.currentWorld.SpawnData;
        if (SpawnData != null) AddStarterDuplicants(SpawnData);

        // Initialize SimDLL with generated cell data
        if (generatedCells != null) {
            Console.WriteLine("[WorldBuilder] Initializing SimDLL...");
            try {
                var bgTemp = new float[generatedCells.Length];
                var dc = new Sim.DiseaseCell[generatedCells.Length];
                for (var i = 0; i < generatedCells.Length; i++) bgTemp[i] = generatedCells[i].temperature;

                Sim.SIM_Initialize(Sim.DLL_MessageHandler);
                SimMessages.CreateSimElementsTable(ElementLoader.elements);
                SimMessages.CreateDiseaseTable(Db.Get().Diseases);
                SimMessages.SimDataInitializeFromCells(Width, Height, 42, generatedCells, bgTemp, dc, headless: true);
                Sim.Start();
                SimRunning = true;
                Console.WriteLine("[WorldBuilder] SimDLL running");
            } catch (Exception ex) {
                Console.WriteLine($"[WorldBuilder] SimDLL failed: {ex.Message}");
            }
        }

        IsLoaded = true;
        Console.WriteLine($"[WorldBuilder] World ready: {Width}x{Height}, SimDLL: {SimRunning}");
    }

    private void InitializeWorld(ResourceLoader resources) {
        new GameObject { name = "Canvas" };
        var go = new GameObject();

        // Grid must be allocated before Game.OnPrefabInit (NavGrid accesses Grid cells)
        AllocateGrid(Width, Height);

        KObjectManager.Instance?.OnDestroy();
        go.AddComponent<KObjectManager>().Awake();
        DistributionPlatform.sImpl = go.AddComponent<SteamDistributionPlatform>();
        Global.Instance?.OnDestroy();
        go.AddComponent<Global>().Awake();
        go.AddComponent<World>().Awake();
        go.AddComponent<Pathfinding>().Awake();
        go.AddComponent<GameScenePartitioner>().Awake();
        go.AddComponent<GameClock>().Awake();
        go.AddComponent<GameScheduler>().Awake();
        go.AddComponent<ScheduleManager>().Awake();
        go.AddComponent<MinionGroupProber>().Awake();
        go.AddComponent<NavigationReservations>().Awake();
        go.AddComponent<GlobalChoreProvider>().Awake();
        go.AddComponent<BuildingConfigManager>().Awake();

        // Assets — Game.OnPrefabInit calls Db.Get() which needs Assets
        SetupAssets(go, resources);

        // Db.Get() → Resources.Load → Initialize(). Initialize crashes partially
        // but core data loads fine. Set _Instance explicitly to survive partial init.
        // KAnimBatchManager needed by KAnimFileData.build getter
        KAnimBatchManager.CreateInstance();

        Db._Instance = ScriptableObject.CreateInstance<Db>();
        Db._Instance.researchTreeFileVanilla = new TextAsset(resources.ResearchTreeVanillaXml);
        Db._Instance.researchTreeFileExpansion1 = new TextAsset(resources.ResearchTreeExpansion1Xml);
        Db._Instance.modifiersFile = new TextAsset(resources.ModifiersCsv);
        // Db.Initialize crashes at AccessorySlots (needs KAnimFile data from Unity assets).
        // Everything before it (Techs, TechItems) initializes fine.
        // After catching, manually init the remaining fields that Initialize would have set.
        try { Db._Instance.Initialize(); }
        catch {
            // Db.Initialize crashes at AccessorySlots (needs KAnimFile data from Unity assets).
            // Everything before it initializes fine. Init remaining fields individually —
            // some constructors NPE without full Unity env, log failures explicitly.
            var root = Db._Instance.Root;
            var db = Db._Instance;
            InitDbField(ref db.ScheduleBlockTypes, () => new ScheduleBlockTypes(root), "ScheduleBlockTypes");
            InitDbField(ref db.ScheduleGroups, () => new ScheduleGroups(root), "ScheduleGroups");
            InitDbField(ref db.RoomTypes, () => new RoomTypes(root), "RoomTypes");
            InitDbField(ref db.Diseases, () => new Diseases(root, statsOnly: true), "Diseases");
            InitDbField(ref db.Sicknesses, () => new Database.Sicknesses(root), "Sicknesses");
            InitDbField(ref db.SkillPerks, () => new SkillPerks(root), "SkillPerks");
            InitDbField(ref db.SkillGroups, () => new SkillGroups(root), "SkillGroups");
            InitDbField(ref db.Skills, () => new Skills(root), "Skills");
            InitDbField(ref db.ColonyAchievements, () => new ColonyAchievements(root), "ColonyAchievements");
            InitDbField(ref db.MiscStatusItems, () => new MiscStatusItems(root), "MiscStatusItems");
            InitDbField(ref db.CreatureStatusItems, () => new CreatureStatusItems(root), "CreatureStatusItems");
            InitDbField(ref db.BuildingStatusItems, () => new BuildingStatusItems(root), "BuildingStatusItems");
            InitDbField(ref db.RobotStatusItems, () => new RobotStatusItems(root), "RobotStatusItems");
            InitDbField(ref db.ChoreTypes, () => new ChoreTypes(root), "ChoreTypes");
            InitDbField(ref db.Quests, () => new Quests(root), "Quests");
            InitDbField(ref db.GameplayEvents, () => new GameplayEvents(root), "GameplayEvents");
            InitDbField(ref db.GameplaySeasons, () => new GameplaySeasons(root), "GameplaySeasons");
            InitDbField(ref db.Stories, () => new Stories(root), "Stories");
            InitDbField(ref db.OrbitalTypeCategories, () => new OrbitalTypeCategories(root), "OrbitalTypeCategories");
            InitDbField(ref db.ArtableStatuses, () => new ArtableStatuses(root), "ArtableStatuses");
            InitDbField(ref db.Permits, () => new PermitResources(root), "Permits");
            InitDbField(ref db.Spices, () => new Spices(root), "Spices");
            Console.WriteLine("[WorldBuilder] Db.Initialize: AccessorySlots skipped, remaining fields initialized");
        }
        Console.WriteLine($"[WorldBuilder] Db: Diseases={Db._Instance.Diseases != null}, MiscStatusItems={Db._Instance.MiscStatusItems != null}, ChoreTypes={Db._Instance.ChoreTypes != null}");

        // CustomGameSettings must exist before Game.OnPrefabInit and Cluster constructor
        // .Awake() calls KMonoBehaviour.InitializeComponent() which calls OnPrefabInit()
        go.AddComponent<CustomGameSettings>().Awake();
        WorldGen.LoadSettings();
        Console.WriteLine($"[WorldBuilder] SettingsCache clusters: {ProcGen.SettingsCache.clusterLayouts?.clusterCache?.Count ?? -1}");
        CustomGameSettings.Instance.LoadClusters();
        Console.WriteLine($"[WorldBuilder] ClusterLayout levels loaded");

        // UI/rendering stubs
        StateMachineDebuggerSettings._Instance = new StateMachineDebuggerSettings();
        StateMachineDebuggerSettings._Instance.Initialize();
        GameComps.InfraredVisualizers = new InfraredVisualizerComponents();
        GameScreenManager.Instance = new GameScreenManager();
        GameScreenManager.Instance.worldSpaceCanvas = new GameObject();
        TuningData<CPUBudget.Tuning>._TuningData = new CPUBudget.Tuning();

        // Let the game initialize itself
        var game = go.AddComponent<global::Game>();
        game.maleNamesFile = new TextAsset("Bob");
        game.femaleNamesFile = new TextAsset("Alisa");
        global::Game.Instance = game;
        game.obj = KObjectManager.Instance.GetOrCreateObject(game.gameObject);

        try {
            game.OnPrefabInit();
            Console.WriteLine("[WorldBuilder] Game.OnPrefabInit() completed");
        } catch (Exception ex) {
            Console.WriteLine($"[WorldBuilder] Game.OnPrefabInit() partial: {ex.Message}");
            // OnPrefabInit crashes on ConduitFlowVisualizer (needs Lighting/GlobalResources).
            // Conduit systems created before that line survive. Fill anything still null.
            game.gasConduitSystem ??= new UtilityNetworkManager<FlowUtilityNetwork, Vent>(Width, Height, 13);
            game.liquidConduitSystem ??= new UtilityNetworkManager<FlowUtilityNetwork, Vent>(Width, Height, 17);
            game.electricalConduitSystem ??= new UtilityNetworkManager<ElectricalUtilityNetwork, Wire>(Width, Height, 27);
            game.travelTubeSystem ??= new UtilityNetworkTubesManager(Width, Height, 35);
            game.gasConduitFlow ??= new ConduitFlow(ConduitType.Gas, Width * Height, game.gasConduitSystem, 1f, 0.25f);
            game.liquidConduitFlow ??= new ConduitFlow(ConduitType.Liquid, Width * Height, game.liquidConduitSystem, 10f, 0.75f);
        }


        PathFinder.Initialize();
        new GameNavGrids(Pathfinding.Instance);

        StateMachineManager.Instance.Clear();
        StateMachine.Instance.error = false;
    }

    // TODO: replace with game's own asset loading
    private void SetupAssets(GameObject go, ResourceLoader resources) {
        go.AddComponent<BundledAssetsLoader>().Awake();
        go.AddComponent<BuildingLoader>().Awake();

        var assets = go.AddComponent<Assets>();
        assets.AnimAssets = new List<KAnimFile>();
        assets.SpriteAssets = new List<Sprite>();
        assets.TintedSpriteAssets = new List<TintedSprite>();
        assets.MaterialAssets = new List<Material>();
        assets.TextureAssets = new List<Texture2D>();
        assets.TextureAtlasAssets = new List<TextureAtlas>();
        assets.BlockTileDecorInfoAssets = new List<BlockTileDecorInfo>();
        Assets.ModLoadedKAnims = new List<KAnimFile> { ScriptableObject.CreateInstance<KAnimFile>() };
        assets.elementAudio = new TextAsset("");
        assets.personalitiesFile = new TextAsset(
            !string.IsNullOrEmpty(resources.PersonalitiesCsv) ? resources.PersonalitiesCsv : "");
        Assets.instance = assets;
        Assets.BuildingDefs = new List<BuildingDef>();

        // Initialize static lists that Assets.OnPrefabInit would normally set
        // Without these, GetMaterial/GetBlockTileDecorInfo NPE on null list
        var stubMaterial = UnityRuntime.CreateStub<Material>();
        stubMaterial.name = "tiles_solid";
        Assets.Materials = new List<Material> { stubMaterial };
        Assets.BlockTileDecorInfos = new List<BlockTileDecorInfo>();
        foreach (var decorName in new[] { "tiles_solid_tops_info", "tiles_solid_tops_place_info" }) {
            var info = ScriptableObject.CreateInstance<BlockTileDecorInfo>();
            info.name = decorName;
            info.decor = Array.Empty<BlockTileDecorInfo.Decor>();
            Assets.BlockTileDecorInfos.Add(info);
        }

        // Stub TextureAtlases for tile building defs
        Assets.TextureAtlases = new List<TextureAtlas>();
        foreach (var name in new[] {
            "tiles_solid", "tiles_solid_place", "tiles_bunker", "tiles_bunker_place",
            "tiles_carpet", "tiles_carpet_place", "tiles_glass", "tiles_glass_place",
            "tiles_insulated", "tiles_insulated_place", "tiles_mesh", "tiles_mesh_place",
            "tiles_mesh_spec", "tiles_metal", "tiles_metal_place", "tiles_metal_spec",
            "tiles_moulding", "tiles_moulding_place", "tiles_plastic", "tiles_plastic_place",
            "tiles_rocket_wall_int", "tiles_rocket_wall_int_place",
            "tiles_snow", "tiles_snow_place", "tiles_wood", "tiles_wood_place"
        }) {
            var atlas = ScriptableObject.CreateInstance<TextureAtlas>();
            atlas.name = name;
            atlas.items = Array.Empty<TextureAtlas.Item>();
            Assets.TextureAtlases.Add(atlas);
        }
        try { AsyncLoadManager<IGlobalAsyncLoader>.Run(); }
        catch (Exception ex) { Console.WriteLine($"[WorldBuilder] AsyncLoadManager partially failed (non-fatal): {ex.Message}"); }

        // Populate AnimTable with stub KAnimFiles so AccessorySlots/BuildingDefs don't NPE
        PopulateStubAnims();
    }

    /// <summary>
    /// Dictionary subclass that returns a stub KAnimFile for any missing key.
    /// Replaces Assets.AnimTable so GetAnim() never returns null.
    /// </summary>
    private static void PopulateStubAnims() {
        var bf = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
               | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance;

        // Create stub KAnimFileData with empty Build
        var emptyBuild = new KAnim.Build { symbols = Array.Empty<KAnim.Build.Symbol>() };
        var batchGroup = (KBatchGroupData)System.Runtime.Serialization.FormatterServices
            .GetUninitializedObject(typeof(KBatchGroupData));
        batchGroup.builds = new List<KAnim.Build> { emptyBuild };
        var stubData = new KAnimFileData("stub");
        stubData.buildIndex = 0;
        typeof(KAnimFileData).GetField("batchGroupData", bf)?.SetValue(stubData, batchGroup);

        var stub = ScriptableObject.CreateInstance<KAnimFile>();
        typeof(KAnimFile).GetField("data", bf)?.SetValue(stub, stubData);

        // Get AnimTable and populate with stub for every anim name found in game assembly
        var animTableField = typeof(Assets).GetField("AnimTable", bf);
        if (animTableField == null) { Console.WriteLine("[WorldBuilder] ERROR: AnimTable field not found"); return; }
        var animTable = (Dictionary<HashedString, KAnimFile>)animTableField.GetValue(null);

        // Register stubs for all anim names used by AccessorySlots and building configs
        var animNames = new[] {
            // AccessorySlots
            "head_swap_kanim", "body_comp_default_kanim", "body_swap_kanim",
            "hair_swap_kanim", "hat_swap_kanim", "shoes_basic_black_kanim",
            "body_lonelyminion_kanim", "body_sena_kanim",
            // Tile configs
            "floor_basic_kanim", "floor_bunker_kanim", "floor_carpet_kanim",
            "floor_glass_kanim", "floor_insulated_kanim", "floor_mesh_kanim",
            "floor_metal_kanim", "floor_moulding_kanim", "floor_plastic_kanim",
            "floor_rocket_kanim", "floor_snow_kanim", "floor_wood_kanim",
            "farmtilerotating_kanim", "radbolt_joint_plate_kanim", "storagetile_kanim",
            // SpiceGrinder + other
            "spice_grinder_kanim", "atmo_shoes_cantaloupe_kanim",
            "rocket_window_small_kanim"
        };
        int count = 0;
        foreach (var name in animNames) {
            var key = new HashedString(name);
            if (!animTable.ContainsKey(key)) { animTable[key] = stub; count++; }
        }
        Console.WriteLine($"[WorldBuilder] AnimTable populated with {count} stub anims");
    }

    private void RegisterBuildingDefs() {
        var types = typeof(GeneratedBuildings).Assembly.GetTypes()
            .Where(t => typeof(IBuildingConfig).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();
        foreach (var type in types) {
            var config = (IBuildingConfig)Activator.CreateInstance(type);
            var def = config.CreateBuildingDef();
            if (def != null) {
                _buildingDefCache[def.PrefabID] = def;
            }
        }
        Console.WriteLine($"[WorldBuilder] Registered {_buildingDefCache.Count}/{types.Count} building defs");
    }

    private void AddStarterDuplicants(GameSpawnData spawnData) {
        var startX = spawnData.baseStartPos.x;
        var startY = spawnData.baseStartPos.y;
        var starters = Db.Get()?.Personalities?.GetStartingPersonalities();
        var names = new List<string>();
        if (starters != null && starters.Count >= 3) {
            var rng = new Random();
            var used = new HashSet<int>();
            for (var i = 0; i < 3; i++) {
                int idx;
                do { idx = rng.Next(starters.Count); } while (used.Contains(idx));
                used.Add(idx);
                names.Add(starters[idx].Name);
            }
        } else {
            names.AddRange(new[] { "Meep", "Bubbles", "Stinky" });
        }
        for (var i = 0; i < 3; i++) {
            spawnData.otherEntities.Add(new TemplateClasses.Prefab(
                names[i], TemplateClasses.Prefab.Type.Other, startX + i, startY, (SimHashes)0));
        }
        Console.WriteLine($"[WorldBuilder] Added 3 duplicants: {string.Join(", ", names)}");
    }

    private static unsafe void AllocateGrid(int w, int h) {
        var n = w * h;
        GridSettings.Reset(w, h);
        var ei = new ushort[n]; var eiH = GCHandle.Alloc(ei, GCHandleType.Pinned); Grid.elementIdx = (ushort*)eiH.AddrOfPinnedObject();
        var te = new float[n]; var teH = GCHandle.Alloc(te, GCHandleType.Pinned); Grid.temperature = (float*)teH.AddrOfPinnedObject();
        var ra = new float[n]; var raH = GCHandle.Alloc(ra, GCHandleType.Pinned); Grid.radiation = (float*)raH.AddrOfPinnedObject();
        var ma = new float[n]; var maH = GCHandle.Alloc(ma, GCHandleType.Pinned); Grid.mass = (float*)maH.AddrOfPinnedObject();
        Grid.InitializeCells();
    }

    /// <summary>
    /// Initializes a Db field if null. Logs on failure — some fields need full Unity env.
    /// </summary>
    private static void InitDbField<T>(ref T field, Func<T> factory, string name) where T : class {
        if (field != null) return;
        try { field = factory(); }
        catch (Exception ex) { Console.WriteLine($"[Db] {name} skipped: {ex.GetBaseException().Message}"); }
    }

    public void Shutdown() {
        if (!IsLoaded) return;
        global::Game.Instance = null;
        Global.Instance = null;
        KObjectManager.Instance = null;
        World.Instance = null;
        IsLoaded = false;
    }
}
