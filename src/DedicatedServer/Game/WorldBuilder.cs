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
    public GameTickLoop TickLoop { get; private set; }

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

        Console.WriteLine("[WorldBuilder] Registering entities...");
        RegisterEntities();

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

        Console.WriteLine("[WorldBuilder] Spawning entities...");
        SpawnEntities(cluster);

        // Reset SM error flag — some entity OnSpawn() may have tripped it during boot.
        // Without this reset, StateMachineUpdater would skip all SM ticks.
        StateMachine.Instance.error = false;

        // Fix consumerState for all Brains after spawn.
        // ChoreConsumer.OnSpawn() may fail before initializing consumerState for two reasons:
        //   1. Minions: schedulable.GetSchedule() returns null because MinionIdentity.OnSpawn()
        //      (which triggers OnAddDupe → schedule assignment) runs AFTER ChoreConsumer.OnSpawn()
        //      in the BaseMinionConfig component order.
        //   2. Creatures: ChoreTable.Instance ctor throws when SMI creation fails in headless.
        // Both leave consumerState null → Brain.UpdateChores() NPEs on every tick.
        FixChoreConsumers();

        IsLoaded = true;
        TickLoop = new GameTickLoop(TickSimulation);
        Console.WriteLine($"[WorldBuilder] World ready: {Width}x{Height}, SimDLL: {SimRunning}");
    }

    private static void Awake(string name, System.Action awake) {
        try { awake(); Console.WriteLine($"[InitWorld] {name} OK"); }
        catch (Exception ex) { Console.WriteLine($"[InitWorld] {name} CRASHED: {ex.GetBaseException().Message}\n{ex.GetBaseException().StackTrace?.Split('\n')[0]}"); throw; }
    }

    private void InitializeWorld(ResourceLoader resources) {
        new GameObject { name = "Canvas" };
        var go = new GameObject();

        // Grid must be allocated before Game.OnPrefabInit (NavGrid accesses Grid cells)
        AllocateGrid(Width, Height);

        KObjectManager.Instance?.OnDestroy();
        Awake("KObjectManager", () => go.AddComponent<KObjectManager>().Awake());
        DistributionPlatform.sImpl = go.AddComponent<SteamDistributionPlatform>();
        Global.Instance?.OnDestroy();
        Awake("Global", () => go.AddComponent<Global>().Awake());
        Awake("World", () => go.AddComponent<World>().Awake());
        Awake("Pathfinding", () => go.AddComponent<Pathfinding>().Awake());
        Awake("GameScenePartitioner", () => go.AddComponent<GameScenePartitioner>().Awake());
        Awake("GameClock", () => go.AddComponent<GameClock>().Awake());
        Awake("GameScheduler", () => go.AddComponent<GameScheduler>().Awake());
        Awake("ScheduleManager", () => go.AddComponent<ScheduleManager>().Awake());
        Awake("MinionGroupProber", () => go.AddComponent<MinionGroupProber>().Awake());
        Awake("NavigationReservations", () => go.AddComponent<NavigationReservations>().Awake());
        Awake("BuildingConfigManager", () => go.AddComponent<BuildingConfigManager>().Awake());
        Awake("EntityConfigManager", () => go.AddComponent<EntityConfigManager>().Awake());

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


        // GlobalChoreProvider.OnPrefabInit → ChoreProvider.OnPrefabInit calls Game.Instance.Subscribe().
        // Must be initialized AFTER Game.Instance is set.
        Awake("GlobalChoreProvider", () => go.AddComponent<GlobalChoreProvider>().Awake());

        PathFinder.Initialize();
        new GameNavGrids(Pathfinding.Instance);

        StateMachineManager.Instance.Clear();
        StateMachine.Instance.error = false;

        // BrainScheduler manages Dupe + Creature AI brain groups.
        // Must be initialized here — BEFORE SpawnEntities() — so that Brain.OnSpawn()
        // callbacks (Components.Brains.Add) find a registered handler and end up in
        // a brain group. Calling Spawn() registers it with SimAndRenderScheduler
        // RENDER_EVERY_TICK, which our GameTickLoop drives via RenderEveryTick().
        var brainScheduler = go.AddComponent<BrainScheduler>();
        Awake("BrainScheduler", () => brainScheduler.Awake());
        brainScheduler.Spawn();
        global::Game.BrainScheduler = brainScheduler;
        Console.WriteLine("[WorldBuilder] BrainScheduler ready");
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

    private static void PopulateStubAnims() {
        // Assets.GetAnim() is patched by PatchInternalCalls (at build time in Assembly-CSharp.dll)
        // to return UnityRuntime.GetStubAnim() when AnimTable is empty (headless mode).
        // No pre-population needed — the stub is returned automatically.
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

    private void RegisterEntities() {
        EntityTemplates.CreateTemplates();
        var entityConfigType = typeof(IEntityConfig);
        var types = typeof(EntityConfigManager).Assembly.GetTypes()
            .Where(t => entityConfigType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();
        int registered = 0, failed = 0;
        foreach (var type in types) {
            try {
                var config = (IEntityConfig)Activator.CreateInstance(type);
                // DLC check: cast the CONFIG INSTANCE (not the Type object) to IHasDlcRestrictions
                string[] required = null, forbidden = null;
                if (config is IHasDlcRestrictions dlcRestrictions) {
                    required = dlcRestrictions.GetRequiredDlcIds();
                    forbidden = dlcRestrictions.GetForbiddenDlcIds();
                }
                if (!DlcManager.IsCorrectDlcSubscribed(required, forbidden)) continue;
                EntityConfigManager.Instance.RegisterEntity(config, required, forbidden);
                registered++;
            } catch (Exception ex) {
                failed++;
                if (failed <= 3) {
                    var inner = ex.InnerException;
                    Console.WriteLine($"[WorldBuilder] Entity FAIL [{type.Name}]: {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine($"  Stack: {(inner ?? ex).StackTrace?.Split('\n')[0]}");
                }
            }
        }
        Console.WriteLine($"[WorldBuilder] Registered {registered}/{types.Count} entities ({failed} failed), prefabs: {Assets.PrefabsByTag?.Count ?? 0}");
    }

    private void AddStarterDuplicants(GameSpawnData spawnData) {
        var startX = spawnData.baseStartPos.x;
        var startY = spawnData.baseStartPos.y;
        // Prefab ID must match the registered entity tag — "Minion" from MinionConfig.
        // Personalities are applied post-spawn; for now just place 3 Minion prefabs.
        for (var i = 0; i < 3; i++) {
            spawnData.otherEntities.Add(new TemplateClasses.Prefab(
                "Minion", TemplateClasses.Prefab.Type.Other, startX + i, startY, (SimHashes)0));
        }
        Console.WriteLine($"[WorldBuilder] Added 3 Minion prefabs at ({startX},{startY})");
    }

    /// <summary>
    /// Instantiates entities from SpawnData for each world in the cluster.
    /// Mirrors WorldGenSpawner.PlaceTemplates() but without SaveLoader dependency.
    /// Applies world offsets to positions, then calls TemplateLoader.PlaceOtherEntities per entity.
    /// </summary>
    private void SpawnEntities(Cluster cluster) {
        var spawned = 0;
        var skipped = 0;
        foreach (var world in cluster.worlds) {
            var offsetX = world.data?.world?.offset.x ?? 0;
            var offsetY = world.data?.world?.offset.y ?? 0;
            // Apply world offset to positions (WorldGenSpawner.PlaceTemplates does the same)
            foreach (var entity in world.SpawnData.otherEntities) {
                entity.location_x += offsetX;
                entity.location_y += offsetY;
            }
            foreach (var entity in world.SpawnData.otherEntities) {
                var go = TemplateLoader.PlaceOtherEntities(entity, 0);
                if (go != null) {
                    Console.WriteLine($"[Entities] Spawned: {entity.id} at ({entity.location_x},{entity.location_y})");
                    spawned++;
                } else {
                    Console.WriteLine($"[Entities] Skipped (no prefab or invalid cell): {entity.id}");
                    skipped++;
                }
            }
        }
        Console.WriteLine($"[WorldBuilder] Entity spawning: {spawned} spawned, {skipped} skipped");
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

    /// <summary>
    /// Post-spawn fix: ensure ChoreProvider/ChoreDriver are initialized, assign default schedules,
    /// and create ChoreConsumerState for all Minions whose ChoreConsumer.OnSpawn() did not complete.
    /// Called once after SpawnEntities(). Only touches LiveMinionIdentities — creatures have a
    /// different ChoreTable and can NPE if processed here.
    /// </summary>
    private void FixChoreConsumers() {
        Console.WriteLine($"[WorldBuilder] FixChoreConsumers() called: LiveMinions={Components.LiveMinionIdentities.Count}, Brains={Components.Brains.Count}");

        // Step 0: force ChoreProvider and ChoreDriver initialization for any Minion
        // where the lifecycle didn't complete (isInitialized=false → Spawn() bailed early).
        foreach (var identity in Components.LiveMinionIdentities.Items) {
            try {
                var provider = identity.GetComponent<ChoreProvider>();
                if (provider != null && !provider.IsInitialized()) {
                    provider.InitializeComponent();
                    provider.Spawn();
                    Console.WriteLine($"[WorldBuilder] Force-initialized ChoreProvider for {identity.name}");
                }
                var driver = identity.GetComponent<ChoreDriver>();
                if (driver != null && !driver.IsInitialized()) {
                    driver.InitializeComponent();
                    driver.Spawn();
                    Console.WriteLine($"[WorldBuilder] Force-initialized ChoreDriver for {identity.name}");
                }
            } catch (Exception ex) {
                Console.WriteLine($"[WorldBuilder] ChoreProvider/Driver init failed for {identity.name}: {ex.GetBaseException().Message}");
            }
        }

        // Step 1: assign default schedule to all Minions not yet scheduled.
        // Must happen BEFORE creating ChoreConsumerState — its ctor calls
        // schedulable.GetSchedule().GetCurrentScheduleBlock() which NPEs on null schedule.
        var schedules = ScheduleManager.Instance?.GetSchedules();
        if (schedules?.Count > 0) {
            foreach (var identity in Components.LiveMinionIdentities.Items) {
                try {
                    var schedulable = identity.GetComponent<Schedulable>();
                    if (schedulable == null) continue;
                    if (ScheduleManager.Instance!.GetSchedule(schedulable) != null) continue;
                    schedules[0].Assign(schedulable);
                } catch (Exception ex) {
                    Console.WriteLine($"[WorldBuilder] Schedule assign failed for {identity.name}: {ex.GetBaseException().Message}");
                }
            }
        } else {
            Console.WriteLine($"[WorldBuilder] WARNING: ScheduleManager has no schedules — consumerState will fail!");
        }

        // Step 2: create ChoreConsumerState for Minions that are still missing it.
        // Only LiveMinionIdentities — creature ChoreTable.Instance ctor NPEs in headless.
        var fixedCount = 0;
        foreach (var identity in Components.LiveMinionIdentities.Items) {
            try {
                var cc = identity.GetComponent<ChoreConsumer>();
                if (cc == null || cc.consumerState != null) continue;
                cc.consumerState = new ChoreConsumerState(cc);
                fixedCount++;
            } catch (Exception ex) {
                Console.WriteLine($"[WorldBuilder] consumerState init failed for {identity.name}: {ex.GetBaseException().Message}");
            }
        }

        Console.WriteLine($"[WorldBuilder] FixChoreConsumers done: {fixedCount}/{Components.LiveMinionIdentities.Count} minion consumerState(s) created, Brains={Components.Brains.Count}");
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
