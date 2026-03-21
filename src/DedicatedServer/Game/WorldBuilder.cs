using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// <summary>Singleton world state — created once in Create(), reused by WebServer.</summary>
    public RealWorldState WorldState { get; private set; }

    // Minions actually spawned during SpawnEntities() — more reliable than Components.LiveMinionIdentities
    // which may be empty if MinionIdentity.OnSpawn() didn't complete.
    private readonly List<GameObject> _spawnedMinions = new List<GameObject>();

    /// <summary>
    /// Maps prefab ID → (w, h) in cells, populated from live spawned GOs during SpawnEntities().
    /// Used by RealWorldState.GetEntitySize to get correct sizes without relying on Assets.GetPrefab.
    /// </summary>
    public IReadOnlyDictionary<string, (int w, int h)> PrefabSizeMap => _prefabSizeMap;
    private readonly Dictionary<string, (int w, int h)> _prefabSizeMap = new();

    public unsafe void TickSimulation() {
        if (!SimRunning) return;
        SimTick++;
        // Multi-tick diagnostic: log at tick 5, 30, 100 to see if paths warm up over time.
        if (SimTick == 5 || SimTick == 30 || SimTick == 100) LogDuplicantStatus();
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

                // Sim.Start() internally calls Grid.InitializeCells() after setting
                // Grid.elementIdx = SimDLL-pointer — this is the ONLY correct time to
                // call InitializeCells, because the SimDLL pointer carries the authoritative
                // element state. Any earlier call (e.g. from our managed copy above) gets
                // overwritten by Sim.Start(). So we must rebuild NavGrids AFTER Sim.Start().
                Sim.Start();
                SimRunning = true;
                Console.WriteLine("[WorldBuilder] SimDLL running");

                // Diagnostic: sample solid state at key cells before NavGrid rebuild.
                // Dupe spawn area y=187, x≈128 → cell≈48000; floor at y=186 → cell≈47744.
                if (Grid.IsValidCell(48000) && Grid.IsValidCell(47744)) {
                    Console.WriteLine($"[NavGrid] Pre-rebuild solid check: dupeCell=48000 solid={Grid.Solid[48000]} elem={Grid.Element[48000]?.tag}, floorCell=47744 solid={Grid.Solid[47744]} elem={Grid.Element[47744]?.tag}");
                    // Sample a few floor cells
                    for (var dx = 0; dx < 4; dx++) {
                        var fc = 47744 + dx;
                        Console.WriteLine($"[NavGrid]   floor+{dx} ({fc}): solid={Grid.Solid[fc]} elem={Grid.Element[fc]?.tag}");
                    }
                }

                // NavGrids were built in InitializeWorld() via new GameNavGrids() → NavGrid ctor
                // → InitializeGraph(). At that time Grid.Solid was all-false (all vacuum), so the
                // FloorValidator produced zero valid floor cells → no floor transitions.
                // Now Grid.elementIdx points to SimDLL data (set by Sim.Start()) and
                // Grid.InitializeCells() was called inside Sim.Start() with the correct elements.
                // Grid.Solid is now authoritative — rebuild all NavGrids so FloorValidator marks
                // actual solid tiles as walkable foundations.
                Pathfinding.Instance.ResetNavGrids();
                var navCount = Pathfinding.Instance.GetNavGrids().Count;
                Console.WriteLine($"[WorldBuilder] NavGrids rebuilt post-SimStart: {navCount} grid(s)");

            } catch (Exception ex) {
                Console.WriteLine($"[WorldBuilder] SimDLL failed: {ex.Message}");
            }
        }

        // Bootstrap ScheduleManager BEFORE spawning entities.
        // ScheduleManager.OnSpawn() creates the default schedule and hooks OnAddDupe.
        // Without this call, schedules.Count == 0 → FixChoreConsumers can't assign schedules
        // → ChoreConsumerState ctor NPEs at schedulable.GetSchedule().GetCurrentScheduleBlock().
        InitializeSchedules();

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

        // Start RationalAi (including IdleMonitor) for each Minion.
        // BaseMinionConfig.BaseOnSpawn() — which creates RationalAi.Instance and calls StartSM() —
        // is called from MinionConfig.OnSpawn(), which itself is called by KMonoBehaviour.Spawn()
        // triggered by Unity's Start() callback. In headless Unity's Start() never fires, so
        // RationalAi was never started → IdleMonitor never started → GlobalChoreProvider.chores=0
        // → Brain.FindBetterChore always returns null → dupes never move.
        FixRationalAi();

        IsLoaded = true;
        TickLoop = new GameTickLoop(TickSimulation);
        WorldState = new RealWorldState(Width, Height, this);
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
            // fetchManager: initialized at Game.OnPrefabInit line 835 (after crash at line 820).
            // PickupableSensor.Update() calls Game.Instance.fetchManager.UpdatePickups() every
            // brain tick — NPEs 1,380×/min if null. Just add the component; FetchManager has no
            // custom OnPrefabInit so AddComponent is sufficient.
            game.fetchManager ??= go.AddComponent<FetchManager>();
        }

        // MinionBrain.UpdateBrain() checks discoveredSurface / discoveredOilField to fire
        // "first discovery" events. If false, it calls World.Instance.zoneRenderData.GetSubWorldZoneType()
        // which NPEs because SubworldZoneRenderData (rendering component) is null in headless.
        // Mark both as already discovered — we're in headless, "discovery" UI events are irrelevant.
        // savedInfo is a struct field on Game (public SavedInfo savedInfo), direct write is safe.
        global::Game.Instance.savedInfo.discoveredSurface = true;
        global::Game.Instance.savedInfo.discoveredOilField = true;
        Console.WriteLine("[WorldBuilder] savedInfo.discovered* = true (skips zoneRenderData NPE in MinionBrain)");


        // GlobalChoreProvider.OnPrefabInit → ChoreProvider.OnPrefabInit calls Game.Instance.Subscribe().
        // Must be initialized AFTER Game.Instance is set.
        Awake("GlobalChoreProvider", () => go.AddComponent<GlobalChoreProvider>().Awake());

        // ReportManager is added to Game GO via Unity editor prefab in normal game — not in any Init code.
        // ChoreDriver.States.InitializeStates() lambdas call ReportManager.Instance.ReportValue() on
        // every tick (nochore.Update and haschore.Update). Without this, Instance is null → NPE 81K+/min.
        // OnPrefabInit just sets Instance=this, creates NoteStorage, and subscribes to Game events.
        Awake("ReportManager", () => go.AddComponent<ReportManager>().Awake());
        // todaysReport is only created in OnSaveGameReady (subscribed to game event hash -1917495436).
        // That event never fires in headless. Fire it manually so todaysReport != null
        // and ReportManager.ReportValue() doesn't NPE on every ChoreDriver tick.
        global::Game.Instance.Trigger(-1917495436);

        // Mirrors Game.OnPrefabInit() lines 830-842 (never reached there because it crashes at 820).
        // PathFinder.Initialize() — builds offset tables NavGrid uses.
        // GameNavGrids — registers all nav grids, including "MinionNavGrid" (DuplicantGrid).
        //   MUST complete before SpawnEntities() so Navigator.OnPrefabInit can call
        //   Pathfinding.Instance.GetNavGrid("MinionNavGrid") and receive a non-null NavGrid.
        // AsyncPathProber.CreateInstance(1) — creates the pathfinding worker thread.
        //   Minions have executePathProbeTaskAsync=true, so Navigator.OnSpawn calls
        //   AsyncPathProber.Instance.Register(this). Without this, Instance is null → NPE.
        // No try-catch — failures here are fatal and must not be silently swallowed.
        PathFinder.Initialize();
        new GameNavGrids(Pathfinding.Instance);
        AsyncPathProber.CreateInstance(1);
        var registeredNavGrids = Pathfinding.Instance.GetNavGrids().Count;
        var minionNavGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
        Console.WriteLine($"[WorldBuilder] NavGrids: {registeredNavGrids} registered, MinionNavGrid={minionNavGrid?.id ?? "NULL — Navigator.OnPrefabInit will fail!"}");
        Console.WriteLine($"[WorldBuilder] AsyncPathProber initialized: {AsyncPathProber.Instance != null}");

        StateMachineManager.Instance.Clear();
        StateMachine.Instance.error = false;

        // ClusterManager is needed by ChoreProvider.AddChore / CollectChores via:
        //   GetMyParentWorldId() → GetMyWorld() → ClusterManager.Instance.GetWorld(worldId)
        // Without it: NPE fires 7,000+ times/tick, choreWorldMap never populated → localChores=0.
        //
        // OnPrefabInit() sets Instance=this then crashes on SaveLoader.Instance.OnWorldGenComplete +=
        // (SaveLoader is null in headless). Instance is set BEFORE that line, so we catch the crash.
        //
        // Must be initialized BEFORE SpawnEntities() → FixRationalAi() → IdleMonitor.StartSM()
        // → AddChore → GetMyParentWorldId() → GetMyWorld() → GetWorld(0).
        var clusterManagerGo = new GameObject("ClusterManager");
        var cm = clusterManagerGo.AddComponent<ClusterManager>();
        try { cm.InitializeComponent(); }
        catch (Exception ex) {
            // Expected: SaveLoader.Instance is null → OnWorldGenComplete += NPE.
            // Instance=this is set before that line — verify before continuing.
            Console.WriteLine($"[WorldBuilder] ClusterManager.InitializeComponent partial (expected): {ex.GetBaseException().Message}");
        }
        if (ClusterManager.Instance == null) {
            ClusterManager.Instance = cm;
            Console.WriteLine("[WorldBuilder] ClusterManager.Instance set manually (fallback)");
        }

        // WorldContainer(id=0): a required entry in ClusterManager.m_worldContainers.
        // GetWorld(0) iterates m_worldContainers searching by .id — without a container with
        // id=0, GetWorld(0) returns null → GetMyParentWorldId() returns -1 → AddChore uses key=-1
        // → CollectChores looks for key=0 → never finds chores → Brain.FindBetterChore returns null.
        //
        // WorldContainer.OnPrefabInit() calls RegisterWorldContainer(this) automatically — safe
        // because ClusterManager.Instance is already set above.
        var worldContainerGo = new GameObject("WorldContainer_0");
        var wc = worldContainerGo.AddComponent<WorldContainer>();
        try { wc.InitializeComponent(); }
        catch (Exception ex) {
            Console.WriteLine($"[WorldBuilder] WorldContainer.InitializeComponent partial: {ex.GetBaseException().Message}");
            // Fallback: register manually if OnPrefabInit didn't complete
            if (!ClusterManager.Instance.WorldContainers.Contains(wc))
                ClusterManager.Instance.RegisterWorldContainer(wc);
        }
        wc.SetID(0);  // sets id=0 and ParentWorldId=0
        Console.WriteLine($"[WorldBuilder] ClusterManager ready: Instance={ClusterManager.Instance != null}, worlds={ClusterManager.Instance?.WorldContainers?.Count}, GetWorld(0)={ClusterManager.Instance?.GetWorld(0)?.id}");

        // GridRestrictionSerializer is a KMonoBehaviour singleton needed by
        // MinionPathFinderAbilities.Refresh() → GetTagId(). Without it, every
        // SafeCellSensor.RunAndGetSafeCellQueryResult() call NPEs at [0x00000].
        var grsGo = new GameObject("GridRestrictionSerializer");
        var grs = grsGo.AddComponent<GridRestrictionSerializer>();
        try { grs.InitializeComponent(); }
        catch (Exception ex) {
            Console.WriteLine($"[WorldBuilder] GridRestrictionSerializer.InitializeComponent partial: {ex.GetBaseException().Message}");
        }
        Console.WriteLine($"[WorldBuilder] GridRestrictionSerializer.Instance={GridRestrictionSerializer.Instance != null}");

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
        _spawnedMinions.Clear();
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
                    // Track Minion GOs directly — more reliable than Components.LiveMinionIdentities
                    // which requires MinionIdentity.OnSpawn() to have completed successfully.
                    if (entity.id == "Minion" || entity.id == "BionicMinion") {
                        _spawnedMinions.Add(go);
                    }
                    // Capture size from live GO's OccupyArea once per unique prefab ID.
                    // More reliable than reading from Assets.GetPrefab in headless mode.
                    if (!_prefabSizeMap.ContainsKey(entity.id)) {
                        CaptureEntitySize(entity.id, go);
                    }
                } else {
                    Console.WriteLine($"[Entities] Skipped (no prefab or invalid cell): {entity.id}");
                    skipped++;
                }
            }
        }
        Console.WriteLine($"[WorldBuilder] Entity spawning: {spawned} spawned, {skipped} skipped, {_spawnedMinions.Count} minions tracked");
    }

    /// <summary>
    /// Reads OccupyArea from a live spawned GO and caches its cell bounding box.
    /// Logs [CritterSize] for each unique ID to aid diagnostics.
    /// </summary>
    private void CaptureEntitySize(string id, GameObject go) {
        try {
            var occupy = go.GetComponent<OccupyArea>();
            int w = 1, h = 1;
            int cellCount = 0;

            if (occupy?._UnrotatedOccupiedCellsOffsets?.Length > 0) {
                var offsets = occupy._UnrotatedOccupiedCellsOffsets;
                cellCount = offsets.Length;
                int minX = 0, maxX = 0, minY = 0, maxY = 0;
                foreach (var o in offsets) {
                    if (o.x < minX) minX = o.x;
                    if (o.x > maxX) maxX = o.x;
                    if (o.y < minY) minY = o.y;
                    if (o.y > maxY) maxY = o.y;
                }
                w = maxX - minX + 1;
                h = maxY - minY + 1;
            } else {
                // Fallback: KBoxCollider2D (set in ConfigPlacedEntity alongside OccupyArea)
                var col = go.GetComponent<KBoxCollider2D>();
                if (col != null) {
                    var s = col.size;
                    w = Math.Max(1, (int)Math.Round(s.x));
                    h = Math.Max(1, (int)Math.Round(s.y));
                }
            }

            _prefabSizeMap[id] = (w, h);
            Console.WriteLine($"[CritterSize] id={id} tag={go.GetComponent<KPrefabID>()?.PrefabTag} occupyCells={cellCount} w={w} h={h}");
        } catch (Exception ex) {
            Console.WriteLine($"[CritterSize] id={id} FAILED: {ex.GetBaseException().Message}");
            _prefabSizeMap[id] = (1, 1);
        }
    }

    private static unsafe void AllocateGrid(int w, int h) {
        var n = w * h;
        GridSettings.Reset(w, h);
        // GridSettings.Reset fills Grid.WorldIdx with byte.MaxValue=255 (sentinel for "no world").
        // GetMyWorldId() returns -1 when WorldIdx[cell]==255 → StandardChoreBase.IsValid() fails.
        // We have a single world with ID=0 — mark all cells as belonging to it.
        if (Grid.WorldIdx != null)
            for (var i = 0; i < n; i++) Grid.WorldIdx[i] = 0;
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
    /// Calls ScheduleManager.Spawn() to create the default schedule and register the OnAddDupe hook.
    /// Must be called BEFORE SpawnEntities() so that minion spawning auto-assigns schedules,
    /// and BEFORE FixChoreConsumers() so that ChoreConsumerState ctor can call GetSchedule().
    /// Falls back to AddSchedule() directly if Spawn() throws (e.g. STRINGS not loaded).
    /// </summary>
    private void InitializeSchedules() {
        var sm = ScheduleManager.Instance;
        if (sm == null) {
            Console.WriteLine("[WorldBuilder] InitializeSchedules: ScheduleManager.Instance is null — skipping");
            return;
        }
        try {
            sm.Spawn();
            Console.WriteLine($"[WorldBuilder] ScheduleManager.Spawn() OK — {sm.GetSchedules().Count} schedule(s)");
        } catch (Exception ex) {
            Console.WriteLine($"[WorldBuilder] ScheduleManager.Spawn() failed: {ex.GetBaseException().Message} — adding schedule manually");
            try {
                var scheduleGroups = Db.Get().ScheduleGroups;
                if (scheduleGroups != null && sm.GetSchedules().Count == 0) {
                    sm.AddSchedule(scheduleGroups.allGroups, "Default", alarmOn: false);
                    Console.WriteLine($"[WorldBuilder] Manual schedule added: {sm.GetSchedules().Count} schedule(s)");
                }
            } catch (Exception ex2) {
                Console.WriteLine($"[WorldBuilder] Manual schedule also failed: {ex2.GetBaseException().Message}");
            }
        }
    }

    /// <summary>
    /// Post-spawn fix: ensure ChoreProvider/ChoreDriver are initialized, assign default schedules,
    /// and create ChoreConsumerState for all entities (Minions AND critters) whose
    /// ChoreConsumer.OnSpawn() did not complete.
    /// Called once after SpawnEntities().
    /// </summary>
    private void FixChoreConsumers() {
        // Prefer _spawnedMinions (directly tracked during SpawnEntities) over LiveMinionIdentities
        // which requires MinionIdentity.OnSpawn() to have run successfully.
        // Use HashSet to deduplicate — defensive against double-entries if SpawnEntities
        // processes the same GO more than once across multiple worlds.
        var minionSet = _spawnedMinions.Count > 0
            ? new HashSet<GameObject>(_spawnedMinions)
            : new HashSet<GameObject>(
                Components.LiveMinionIdentities.Items.ConvertAll(id => id.gameObject));
        var minionGOs = new List<GameObject>(minionSet);

        Console.WriteLine($"[WorldBuilder] FixChoreConsumers() called: tracked={_spawnedMinions.Count}, LiveMinions={Components.LiveMinionIdentities.Count}, Brains={Components.Brains.Count}");

        // Step 0: force ChoreProvider and ChoreDriver initialization for any Minion
        // where the lifecycle didn't complete (isInitialized=false → Spawn() bails early).
        foreach (var go in minionGOs) {
            try {
                var provider = go.GetComponent<ChoreProvider>();
                if (provider != null && !provider.IsInitialized()) {
                    provider.InitializeComponent();
                    provider.Spawn();
                    Console.WriteLine($"[WorldBuilder] Force-initialized ChoreProvider for {go.name}");
                }
                var driver = go.GetComponent<ChoreDriver>();
                if (driver != null && !driver.IsInitialized()) {
                    driver.InitializeComponent();
                    driver.Spawn();
                    Console.WriteLine($"[WorldBuilder] Force-initialized ChoreDriver for {go.name}");
                }
            } catch (Exception ex) {
                Console.WriteLine($"[WorldBuilder] ChoreProvider/Driver init failed for {go.name}: {ex.GetBaseException().Message}");
            }
        }

        // Step 1: assign default schedule to all Minions not yet scheduled.
        // Must happen BEFORE creating ChoreConsumerState — its ctor calls
        // schedulable.GetSchedule().GetCurrentScheduleBlock() which NPEs on null schedule.
        var schedules = ScheduleManager.Instance?.GetSchedules();
        if (schedules?.Count > 0) {
            foreach (var go in minionGOs) {
                try {
                    var schedulable = go.GetComponent<Schedulable>();
                    if (schedulable == null) continue;
                    if (ScheduleManager.Instance!.GetSchedule(schedulable) != null) continue;
                    schedules[0].Assign(schedulable);
                } catch (Exception ex) {
                    Console.WriteLine($"[WorldBuilder] Schedule assign failed for {go.name}: {ex.GetBaseException().Message}");
                }
            }
        } else {
            Console.WriteLine($"[WorldBuilder] WARNING: ScheduleManager has no schedules — consumerState will fail!");
        }

        // Step 2: create ChoreConsumerState for ALL entities with null consumerState.
        // Minions: covered via minionGOs (direct tracking from SpawnEntities).
        // Critters/other: covered via Components.Brains — all entities with Brain register there.
        //   Brain.UpdateChores → FindNextChore immediately NPEs if consumerState==null (0x0000b).
        //   ChoreConsumerState ctor is safe for any entity: Schedulable/Navigator/Resume are
        //   null-checked internally. Creatures have no Schedulable so scheduleBlock stays null.
        // Previous code only fixed Minions. Non-Minion entities (critters) caused 2.2K NPEs/min.
        var allBrainGOs = new HashSet<GameObject>(minionGOs);
        foreach (var brain in Components.Brains.Items) {
            if (brain?.gameObject != null) allBrainGOs.Add(brain.gameObject);
        }

        var fixedCount = 0;
        foreach (var go in allBrainGOs) {
            try {
                var cc = go.GetComponent<ChoreConsumer>();
                if (cc == null || cc.consumerState != null) continue;
                cc.consumerState = new ChoreConsumerState(cc);
                fixedCount++;
            } catch (Exception ex) {
                Console.WriteLine($"[WorldBuilder] consumerState init failed for {go.name}: {ex.GetBaseException().Message}");
            }
        }

        // Diagnostic: log ChoreDriver initialization state for each Minion.
        foreach (var go in minionGOs) {
            var driver = go.GetComponent<ChoreDriver>();
            Console.WriteLine($"[FixChoreConsumers] {go.name}: ChoreDriver={driver != null} isInit={driver?.IsInitialized()} consumerState={go.GetComponent<ChoreConsumer>()?.consumerState != null} sensors={go.GetComponent<Sensors>() != null}");
        }
        Console.WriteLine($"[WorldBuilder] FixChoreConsumers done: {fixedCount}/{allBrainGOs.Count} consumerState(s) created (minions={minionGOs.Count} brains={Components.Brains.Count})");
    }

    /// <summary>
    /// Bootstraps the Brain→Chore→Navigator pipeline for each spawned Minion.
    ///
    /// In the live game the full chain runs via Unity.Start() → KMonoBehaviour.Spawn() →
    ///   KPrefabID.OnSpawn() → MinionConfig.OnSpawn() → BaseMinionConfig.BaseOnSpawn()
    ///   → RationalAi.Instance.StartSM() → ToggleStateMachineList → IdleMonitor.StartSM().
    ///   Also Unity.Start() → KMonoBehaviour.Spawn() → Brain.OnSpawn() sets running=true
    ///   and adds brain to Components.Brains → BrainScheduler ticks UpdateBrain().
    ///   And Sensors.OnSpawn() subscribes OnBrainPreUpdate → sensors update before each brain tick.
    ///
    /// In headless Unity.Start() never fires → none of this runs → chore stays null forever.
    ///
    /// Steps performed here (each per Minion GO):
    ///   1. Add safe sensors (PathProberSensor + IdleCellSensor — both are no-ops at Add() time).
    ///      AssignableReachabilitySensor EXCLUDED — its ctor NPEs on MinionIdentity.assignableProxy.
    ///   2. Start IdleMonitor.Instance directly with StateMachineController as master.
    ///      IdleMonitor.Instance ctor = base(master) only — pure C#.
    ///      StartSM() → idle state → ToggleRecurringChore → new IdleChore → registered in ChoreProvider.
    ///   3. Spawn Brain (MinionBrain) directly via KMonoBehaviour.Spawn().
    ///      Brain.OnSpawn(): sets choreConsumer, sets running=true, calls Components.Brains.Add(this)
    ///      → BrainScheduler.OnAddBrain → brain added to DupeBrainGroup → UpdateBrain() scheduled.
    ///   4. Spawn Sensors directly via KMonoBehaviour.Spawn().
    ///      Sensors.OnSpawn(): subscribes OnBrainPreUpdate to Brain.onPreUpdate
    ///      → sensors update before each brain tick → IdleCellSensor finds idle cell.
    ///   5. Add navigator transition override layers (pure List.Add — no callbacks).
    /// </summary>
    private void FixRationalAi() {
        var fixedCount = 0;
        foreach (var go in _spawnedMinions) {
            try {
                var smc = go.GetComponent<StateMachineController>();
                if (smc == null) {
                    Console.WriteLine($"[FixRationalAi] {go.name}: StateMachineController=null, skipping");
                    continue;
                }

                // Step 1: add safe sensors BEFORE Sensors.Spawn() subscribes onPreUpdate.
                // PathProberSensor.Update() → navigator.UpdateProbe() no-op (executePathProbeTaskAsync=true).
                // IdleCellSensor.Update() → prefabid.HasTag(Idle)=false → return immediately.
                // AssignableReachabilitySensor EXCLUDED: ctor calls assignableProxy.Get() → NPE.
                var sensors = go.GetComponent<Sensors>();
                if (sensors != null) {
                    sensors.Add(new PathProberSensor(sensors));
                    sensors.Add(new IdleCellSensor(sensors));
                }

                // Step 2: start IdleMonitor directly (mirrors what RationalAi.alive does via
                // ToggleStateMachineList). Bypasses RationalAi entirely to avoid DeathMonitor
                // and AddUrge() calls that NPE in headless.
                var idleMonitorSmi = new IdleMonitor.Instance(smc);
                idleMonitorSmi.StartSM();

                // Step 3a: ensure Navigator.OnPrefabInit() has run.
                // Navigator.OnPrefabInit() sets NavGrid = Pathfinding.Instance.GetNavGrid(NavGridName).
                // MinionBrain.OnPrefabInit() calls new MinionPathFinderAbilities(Navigator) which
                // accesses Navigator.NavGrid.transitions in its ctor — NPEs if NavGrid is null.
                // When NavGrid is null the ctor throws, brain.OnPrefabInit() silently swallows it,
                // and Navigator.abilities stays null → SafeCellSensor.RunAndGetSafeCellQueryResult
                // NPEs at [0x00000] on every sensor tick.
                var nav3 = go.GetComponent<Navigator>();
                if (nav3 != null && !nav3.IsInitialized()) {
                    try {
                        nav3.InitializeComponent();
                        Console.WriteLine($"[FixRationalAi] {go.name}: Navigator.InitializeComponent OK, NavGrid={nav3.NavGrid?.id ?? "null"}");
                    } catch (Exception ex) {
                        Console.WriteLine($"[FixRationalAi] {go.name}: Navigator.InitializeComponent partial: {ex.GetBaseException().Message}");
                    }
                }

                // Step 3b: ensure MinionIdentity.assignableProxy is set.
                // MinionIdentity.ValidateProxy() creates the proxy GO and sets the assignableProxy ref.
                // This normally runs in MinionIdentity.OnSpawn() → OnAddDupe callback.
                // Without it: MinionPathFinderAbilities.Refresh() calls assignableProxy.Get() → NPE.
                var identity = go.GetComponent<MinionIdentity>();
                if (identity != null && identity.assignableProxy?.Get() == null) {
                    try {
                        identity.ValidateProxy();
                        Console.WriteLine($"[FixRationalAi] {go.name}: ValidateProxy OK, proxy={identity.assignableProxy?.Get() != null}");
                    } catch (Exception ex) {
                        Console.WriteLine($"[FixRationalAi] {go.name}: ValidateProxy partial: {ex.GetBaseException().Message}");
                    }
                }

                // Step 3: spawn Brain — sets running=true + choreConsumer + registers with BrainScheduler.
                // Without this: Brain.IsRunning()=false → BrainGroup.RenderEveryTick skips brain
                // → UpdateBrain() never called → chore never picked even though IdleChore exists.
                // KMonoBehaviour.Spawn() is public; requires isInitialized=true (set by Awake — already done).
                var brain = go.GetComponent<MinionBrain>();
                if (brain != null && !brain.isSpawned) {
                    brain.Spawn();
                    Console.WriteLine($"[FixRationalAi] {go.name}: Brain spawned (running={brain.IsRunning()})");
                }

                // Step 3c: pre-add GameTags.Idle to break the deadlock.
                // IdleCellSensor.Update() returns Grid.InvalidCell immediately when
                //   !prefabid.HasTag(GameTags.Idle) → idleCell=-1 → Brain never finds idleCell.
                // IdleChore.idle.ToggleTag(GameTags.Idle) adds the tag only AFTER Brain picks
                // IdleChore — creating a deadlock: no tag → no idleCell → chore not picked →
                // no tag. Pre-adding the tag breaks the cycle; IdleChore will redundantly
                // re-add/remove it via ToggleTag when it enters/exits the idle state.
                go.GetComponent<KPrefabID>()?.AddTag(GameTags.Idle);

                // Step 4: spawn Sensors — subscribes OnBrainPreUpdate to Brain.onPreUpdate.
                // Without this: sensors never update → IdleCellSensor always returns InvalidCell.
                // Must happen AFTER Brain.Spawn() so Brain.onPreUpdate delegate is initialised.
                if (sensors != null && !sensors.isSpawned) {
                    sensors.Spawn();
                }

                // Step 5: navigator transition layers (pure List.Add — no immediate callbacks).
                var nav = go.GetComponent<Navigator>();
                if (nav?.transitionDriver != null) {
                    nav.transitionDriver.overrideLayers.Add(new BipedTransitionLayer(nav, 3.325f, 2.5f));
                    nav.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(nav));
                    nav.transitionDriver.overrideLayers.Add(new LadderDiseaseTransitionLayer(nav));
                    nav.transitionDriver.overrideLayers.Add(new NavTeleportTransitionLayer(nav));
                }

                fixedCount++;
                Console.WriteLine($"[FixRationalAi] {go.name}: OK");
            } catch (Exception ex) {
                Console.WriteLine($"[FixRationalAi] {go.name}: ERROR: {ex.GetBaseException().Message}\n  {ex.GetBaseException().StackTrace?.Split('\n')[0]}");
            }
        }
        Console.WriteLine($"[WorldBuilder] FixRationalAi done: {fixedCount}/{_spawnedMinions.Count} minion(s) started");
    }

    /// <summary>
    /// Disables rendering-only components that NPE in headless (no camera/animator).
    /// Cannot use Harmony (KMonoBehaviour subclass methods → deadlock).
    /// component.enabled = false prevents RenderEveryTick/SimEveryTick callbacks.
    /// </summary>
    private static void DisableRenderingOnlyComponents() {
        var count = 0;
        foreach (var lst in UnityEngine.Object.FindObjectsOfType<LightSymbolTracker>()) {
            try {
                if (lst != null) { lst.enabled = false; count++; }
            } catch { /* Unity objects may be in partial state post-spawn — safe to skip */ }
        }
        if (count > 0)
            Console.WriteLine($"[WorldBuilder] Disabled {count} LightSymbolTracker component(s)");
    }

    /// <summary>
    /// One-shot diagnostic: logs currentChore and NavType for each spawned Minion.
    /// Called at SimTick==5 to verify Brain→Chore→Navigator pipeline is working.
    /// </summary>
    private void LogDuplicantStatus() {
        Console.WriteLine($"[DupeStatus] tick={SimTick} minions={_spawnedMinions.Count}");

        // 1. GlobalChoreProvider total chore count
        try {
            var gcp = GlobalChoreProvider.Instance;
            var totalChores = 0;
            var totalFetches = 0;
            if (gcp?.choreWorldMap != null)
                foreach (var list in gcp.choreWorldMap.Values) totalChores += list?.Count ?? 0;
            if (gcp?.fetchMap != null)
                foreach (var list in gcp.fetchMap.Values) totalFetches += list?.Count ?? 0;
            Console.WriteLine($"[DupeStatus] GlobalChoreProvider chores={totalChores} fetchChores={totalFetches} gcp={(gcp != null ? "OK" : "null")}");
        } catch (Exception ex) {
            Console.WriteLine($"[DupeStatus] GlobalChoreProvider ERROR: {ex.GetBaseException().Message}");
        }

        // 2. ScheduleManager — current block + IsAllowed(Work)
        try {
            var sm = ScheduleManager.Instance;
            if (sm != null) {
                var schedules = sm.GetSchedules();
                Console.WriteLine($"[DupeStatus] ScheduleManager schedules={schedules?.Count ?? 0}");
                if (schedules != null) {
                    for (var i = 0; i < schedules.Count; i++) {
                        var block = schedules[i].GetCurrentScheduleBlock();
                        var allowsWork = block?.IsAllowed(Db.Get().ScheduleBlockTypes.Work) ?? false;
                        Console.WriteLine($"[DupeStatus]   schedule[{i}] block={block?.name ?? "null"} allowsWork={allowsWork}");
                    }
                }
            } else {
                Console.WriteLine($"[DupeStatus] ScheduleManager.Instance=null");
            }
        } catch (Exception ex) {
            Console.WriteLine($"[DupeStatus] ScheduleManager ERROR: {ex.GetBaseException().Message}");
        }

        // 3. Per-minion state + precondition failure diagnosis
        // Reflection fields cached once (same type for all minions).
        var snapshotField    = typeof(ChoreConsumer).GetField("preconditionSnapshot", BindingFlags.Instance | BindingFlags.NonPublic);
        var providersField   = typeof(ChoreConsumer).GetField("providers",            BindingFlags.Instance | BindingFlags.NonPublic);
        Type? snapshotType   = snapshotField?.GetValue(_spawnedMinions.Count > 0 ? _spawnedMinions[0].GetComponent<ChoreConsumer>() : null)?.GetType();
        var failedCtxField   = snapshotType?.GetField("failedContexts");
        var succeededCtxField = snapshotType?.GetField("succeededContexts");

        foreach (var go in _spawnedMinions) {
            try {
                var driver    = go.GetComponent<ChoreDriver>();
                var nav       = go.GetComponent<Navigator>();
                var chore     = driver?.GetCurrentChore();
                var consumer  = go.GetComponent<ChoreConsumer>();
                var idleSmi   = go.GetSMI<IdleMonitor.Instance>();
                var brain     = go.GetComponent<Brain>();
                var localCP   = go.GetComponent<ChoreProvider>();
                var localChores = localCP?.choreWorldMap?.Values.Sum(l => l?.Count ?? 0) ?? -1;
                var worldId   = go.GetMyWorldId();
                var cell      = Grid.PosToCell(go);
                var worldIdx  = (Grid.WorldIdx != null && Grid.IsValidCell(cell)) ? (int)Grid.WorldIdx[cell] : -99;
                Console.WriteLine($"  [{go.name}] chore={chore?.GetType().Name ?? "null"}  brainRunning={brain?.IsRunning()}  localChores={localChores}  hasChore={driver?.HasChore()}  worldId={worldId}  cell={cell}  gridWorldIdx={worldIdx}  navType={nav?.CurrentNavType}");

                // Sensor state
                var sensors   = go.GetComponent<Sensors>();
                var idleSensor = sensors?.GetSensor<IdleCellSensor>();
                var hasIdleTag = go.GetComponent<KPrefabID>()?.HasTag(GameTags.Idle) ?? false;
                Console.WriteLine($"  [{go.name}] idleCell={idleSensor?.GetCell() ?? -1}  hasIdleTag={hasIdleTag}  ChorePreconditions.instance={(ChorePreconditions.instance != null ? "OK" : "NULL")}");

                // NavGrid + PathGrid diagnostics
                // GetNavigationCost uses PathGrid.GetCost (PathProber-warmed) — -1 = unreachable.
                // RunQuery uses PathFinder.Run BFS directly — doesn't need PathGrid warmup.
                // If all costs == -1, PathGrid is cold (AsyncPathProber hasn't run yet); BFS still works.
                if (nav != null) {
                    Console.WriteLine($"  [{nav.gameObject.name}] NavGrid={nav.NavGrid?.id ?? "NULL"}  PathGrid={nav.PathGrid != null}  abilities={nav.GetComponent<Navigator>()?.GetCurrentAbilities() != null}  navType={nav.CurrentNavType}");
                    var reachable = 0;
                    var sb = new System.Text.StringBuilder();
                    for (var offset = -3; offset <= 3; offset++) {
                        var testCell = cell + offset;
                        var cost = nav.GetNavigationCost(testCell);
                        if (cost != -1) { reachable++; sb.Append($"cell{testCell}:{cost} "); }
                    }
                    Console.WriteLine($"  [{nav.gameObject.name}] PathGrid reachable (±3 cells): {reachable}/7  {(reachable == 0 ? "(PathGrid cold — BFS still works)" : sb.ToString())}");

                    // Force synchronous probe to warm PathGrid and recheck
                    try {
                        nav.UpdateProbe(forceUpdate: true);
                        var reachableAfter = 0;
                        for (var offset = -3; offset <= 3; offset++) {
                            if (nav.GetNavigationCost(cell + offset) != -1) reachableAfter++;
                        }
                        Console.WriteLine($"  [{nav.gameObject.name}] After forceUpdate probe: reachable={reachableAfter}/7");
                    } catch (Exception ex3) {
                        Console.WriteLine($"  [{nav.gameObject.name}] UpdateProbe error: {ex3.GetBaseException().Message}");
                    }
                }

                // SafeFlags for current cell and neighbors — reveals exactly why IdleCellQuery returns -1.
                // IdleCellQuery requires: IsClear & IsNotLadder & IsNotTube & IsBreathable & IsNotLiquid
                var brain4 = go.GetComponent<MinionBrain>();
                if (brain4 != null) {
                    Console.WriteLine($"  [{go.name}] SafeFlags for current cell {cell}: {SafeCellQuery.GetFlags(cell, brain4)}");
                    var bestFlags = (SafeCellQuery.SafeFlags)0;
                    var bestCell = -1;
                    for (var offset = -5; offset <= 5; offset++) {
                        var tc = cell + offset;
                        if (!Grid.IsValidCell(tc)) continue;
                        var f = SafeCellQuery.GetFlags(tc, brain4);
                        if ((int)f > (int)bestFlags) { bestFlags = f; bestCell = tc; }
                    }
                    const SafeCellQuery.SafeFlags idleRequired = SafeCellQuery.SafeFlags.IsClear | SafeCellQuery.SafeFlags.IsNotLadder | SafeCellQuery.SafeFlags.IsNotTube | SafeCellQuery.SafeFlags.IsBreathable | SafeCellQuery.SafeFlags.IsNotLiquid;
                    Console.WriteLine($"  [{go.name}] Best SafeFlags in ±5: cell={bestCell} flags={bestFlags}  idleRequired={idleRequired}  allMet={(bestFlags & idleRequired) == idleRequired}");
                    // Log mass+element for current cell (helps diagnose IsBreathable failures)
                    if (Grid.IsValidCell(cell)) {
                        unsafe { Console.WriteLine($"  [{go.name}] cell {cell}: mass={Grid.mass[cell]:F3}  elementIdx={Grid.elementIdx[cell]}  solid={Grid.Solid[cell]}  temp={Grid.temperature[cell]:F1}K"); }
                    }
                }

                // ChoreConsumer providers list
                if (consumer != null && providersField != null) {
                    var providerList = providersField.GetValue(consumer) as System.Collections.IList;
                    Console.WriteLine($"  [{go.name}] consumer.providers={providerList?.Count ?? -1}");
                }

                // Run FindNextChore to populate preconditionSnapshot, then read failed contexts
                if (consumer?.consumerState != null) {
                    Chore.Precondition.Context dummy = default;
                    consumer.FindNextChore(ref dummy);

                    if (snapshotField != null && failedCtxField != null && succeededCtxField != null) {
                        var snapshot  = snapshotField.GetValue(consumer);
                        var failed    = failedCtxField.GetValue(snapshot)   as List<Chore.Precondition.Context>;
                        var succeeded = succeededCtxField.GetValue(snapshot) as List<Chore.Precondition.Context>;
                        Console.WriteLine($"  [{go.name}] preconditions: succeeded={succeeded?.Count ?? -1}  failed={failed?.Count ?? -1}");
                        if (failed != null) {
                            foreach (var ctx in failed) {
                                try {
                                    var preconditions = ctx.chore?.GetPreconditions();
                                    var failId = ctx.failedPreconditionId;
                                    var failName = (preconditions != null && failId >= 0 && failId < preconditions.Count)
                                        ? preconditions[failId].condition.id
                                        : $"idx={failId}";
                                    Console.WriteLine($"    FAILED: chore={ctx.chore?.GetType().Name ?? "null"}  precondition={failName}  skipped={ctx.skippedPreconditions}");
                                } catch (Exception ex2) {
                                    Console.WriteLine($"    FAILED ctx error: {ex2.GetBaseException().Message}");
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"  [{go.name}] ERROR: {ex.GetBaseException().Message}\n    {ex.GetBaseException().StackTrace?.Split('\n')[0]}");
            }
        }
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
