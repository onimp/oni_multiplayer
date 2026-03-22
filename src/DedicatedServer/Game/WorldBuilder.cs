using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Database;
using Klei;
using ProcGen;
using ProcGenGame;
using TemplateClasses;
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

    // HQ/Headquarters cell read directly from SpawnData during SpawnEntities(),
    // used by FindColonySpawnCell() to locate the starter cave without needing the GO.
    private int _hqCell = -1;

    /// <summary>
    /// Maps prefab ID → (w, h) in cells, populated from live spawned GOs during SpawnEntities().
    /// Used by RealWorldState.GetEntitySize to get correct sizes without relying on Assets.GetPrefab.
    /// </summary>
    public IReadOnlyDictionary<string, (int w, int h)> PrefabSizeMap => _prefabSizeMap;
    private readonly Dictionary<string, (int w, int h)> _prefabSizeMap = new();

    /// <summary>
    /// Entities spawned directly (not via spawnData.otherEntities → SpawnEntities).
    /// Used by RealWorldState.GetEntitiesBytes so /api/entities includes them.
    /// </summary>
    public IReadOnlyList<(string id, int x, int y)> DirectlySpawnedEntities => _directlySpawnedEntities;
    private readonly List<(string id, int x, int y)> _directlySpawnedEntities = new();

    public unsafe void TickSimulation() {
        if (!SimRunning) return;
        SimTick++;
        // Multi-tick diagnostic: log at tick 5, 30, 100, 200 to see if movement starts over time.
        if (SimTick == 5 || SimTick == 30 || SimTick == 100 || SimTick == 200) LogDuplicantStatus();
        StepTheSim(0.2f);
    }

    /// <summary>
    /// Sends a NewGameFrame(dt) to SimDLL and reads back PrepareGameData.
    /// dt=0f bootstraps sim state without advancing time
    /// (mirrors Game.UnsafePrefabInit line 880: StepTheSim(0f)).
    /// dt=0.2f is the normal per-frame advance used by TickSimulation().
    /// </summary>
    private unsafe void StepTheSim(float dt) {
        var activeRegions = new List<global::Game.SimActiveRegion> {
            new() { region = new Pair<Vector2I, Vector2I>(new Vector2I(0, 0), new Vector2I(Width, Height)) }
        };
        SimMessages.NewGameFrame(dt, activeRegions);
        var visible = new byte[Grid.CellCount];
        for (var i = 0; i < visible.Length; i++) visible[i] = byte.MaxValue;
        var ptr = Sim.HandleMessage(SimMessageHashes.PrepareGameData, visible.Length, visible);
        if (ptr != IntPtr.Zero) {
            var update = (Sim.GameDataUpdate*)(void*)ptr;
            Grid.elementIdx      = update->elementIdx;
            Grid.temperature     = update->temperature;
            Grid.mass            = update->mass;
            Grid.radiation       = update->radiation;
            Grid.properties      = update->properties;
            Grid.strengthInfo    = update->strengthInfo;
            Grid.insulation      = update->insulation;
            Grid.diseaseIdx      = update->diseaseIdx;
            Grid.diseaseCount    = update->diseaseCount;
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

                // P0 Fix 1 (Sol DS-005): StepTheSim(0f) — bootstrap SimDLL before entity spawn.
                // Mirrors Game.UnsafePrefabInit line 880. dt=0 flushes initial sim state without
                // advancing time. Reads back Grid.elementIdx/temperature/mass/etc pointers from SimDLL.
                StepTheSim(0f);
                Console.WriteLine("[WorldBuilder] StepTheSim(0f) bootstrap done");

                // P0 Fix 2 (Sol DS-005): world.UpdateCellInfo() — mirrors Game.UnsafeOnSpawn line 1002.
                // Called with empty solidInfo so the World/pathfinding layer is primed for solid changes.
                // Actual solid-change propagation happens each tick via TickSimulation → StepTheSim.
                // unsafe block required: UpdateCellInfo takes Sim.SolidSubstanceChangeInfo*/LiquidChangeInfo* params.
                unsafe {
                    World.Instance?.UpdateCellInfo(
                        new System.Collections.Generic.List<SolidInfo>(),
                        new System.Collections.Generic.List<CallbackInfo>(),
                        0, null, 0, null);
                }
                Console.WriteLine("[WorldBuilder] World.UpdateCellInfo() bootstrap done");

                // Diagnostic: count solid / non-vacuum cells across the entire grid.
                // If solidCount=0 and nonVacuumCount=0 → SimDLL did not load world data.
                {
                    int solidCount = 0, vacuumCount = 0, nonVacuumCount = 0;
                    for (var i = 0; i < Grid.CellCount; i++) {
                        if (Grid.Solid[i]) solidCount++;
                        if (Grid.Element[i]?.id == SimHashes.Vacuum) vacuumCount++;
                        else nonVacuumCount++;
                    }
                    Console.WriteLine($"[Grid] After Sim.Start(): solid={solidCount} vacuum={vacuumCount} nonVacuum={nonVacuumCount} total={Grid.CellCount}");
                }

                // N2: Tell SimDLL the world boundaries. Without this, SimDLL doesn't know which
                // cells belong to each world → SimMessages.ClearUnoccupiedCells has no effect →
                // gas cells outside world borders remain as vacuum → printer area stays vacuum.
                // Mirrors SaveLoader line 1005-1013 (new-game path from WorldGen).
                {
                    var sp = SpawnData?.baseStartPos ?? new Vector2I(Width / 2, Height / 2);
                    Console.WriteLine($"[N2] baseStartPos=({sp.x},{sp.y}) — 5×5 cells BEFORE DefineWorldOffsets:");
                    LogCellArea(sp.x, sp.y, 2);

                    var worldOffsets = cluster.worlds.Select(w => new SimMessages.WorldOffsetData {
                        worldOffsetX = w.WorldOffset.x,
                        worldOffsetY = w.WorldOffset.y,
                        worldSizeX   = w.WorldSize.x,
                        worldSizeY   = w.WorldSize.y
                    }).ToList();
                    Console.WriteLine($"[N2] DefineWorldOffsets: {worldOffsets.Count} world(s): " +
                        string.Join(", ", worldOffsets.Select(wo => $"off=({wo.worldOffsetX},{wo.worldOffsetY}) sz=({wo.worldSizeX},{wo.worldSizeY})")));
                    SimMessages.DefineWorldOffsets(worldOffsets);
                    Sim.HandleMessage(SimMessageHashes.ClearUnoccupiedCells, 0, null);
                    Console.WriteLine("[N2] DefineWorldOffsets + ClearUnoccupiedCells sent");

                    Console.WriteLine($"[N2] 5×5 cells AFTER DefineWorldOffsets:");
                    LogCellArea(sp.x, sp.y, 2);
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

        // Diagnostic: log element/mass state around PrintingPod BEFORE and AFTER a SimDLL tick.
        // Helps diagnose whether oxygen vacuum near printer is from stale Grid data (SimMessages
        // queued but not yet processed) or from worldgen generating a genuinely empty room.
        DiagnosePrinterArea("pre-tick");
        if (SimRunning) {
            // Run one full SimDLL frame (12 subticks * 200ms = 1 sim frame) to flush
            // any queued SimMessages.ReplaceElement calls from SpawnEntities/PlaceBuilding.
            for (var t = 0; t < 12; t++) Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
            TickSimulation();
            Console.WriteLine("[WorldBuilder] Ran 1 sim frame post-SpawnEntities to flush SimMessages");
        }
        DiagnosePrinterArea("post-tick");

        // Spawn starter duplicants at the actual colony location.
        // Must run AFTER SpawnEntities so Telepad/PrintingPod is already in the world
        // (FindColonySpawnCell can then locate it via FindObjectsOfType).
        // Also requires Grid.Solid to be populated (only valid after Sim.Start()).
        SpawnStarterMinions();

        // Disable rendering-only components that NPE every tick in headless (no camera/animator).
        // LightSymbolTracker.RenderEveryTick calls IsEnableAndVisible() → KAnimControllerBase NPE.
        // Must run after SpawnStarterMinions so all entities (including manually-spawned dupes)
        // are present. component.enabled=false prevents SimAndRenderScheduler callbacks.
        DisableRenderingOnlyComponents();

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

        // Ensure CreatureBrain.running=true and Navigator SM started for all spawned critters.
        // TriggerLifecycle calls Spawn() on components but fails may leave brain not running or
        // Navigator SM not started → CreatureBrainGroup.RenderEveryTick skips them → no chore picked.
        FixCreatureBrains();

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
        // BuildingLoader must precede BuildingConfigManager: RegisterBuilding() calls
        // BuildingLoader.Instance.CreateBuildingComplete() to build the prefab template.
        Awake("BuildingLoader", () => go.AddComponent<BuildingLoader>().Awake());
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

        // Add AlertStateManager.Def BEFORE InitializeComponent() so CreateSMIS() (called in
        // KPrefabID.OnPrefabInit → InitializeComponent) includes it. If added after, the SMI
        // is never created and WorldContainer.AlertManager returns null → BreathMonitor NPE.
        // In normal game: AsteroidConfig.CreatePrefab() → AddOrGetDef<AlertStateManager.Def>()
        // is called on the prefab before any instance lifecycle fires.
        try {
            worldContainerGo.AddOrGetDef<AlertStateManager.Def>();
            Console.WriteLine("[WorldBuilder] WorldContainer: AlertStateManager.Def registered (pre-init)");
        } catch (Exception ex) {
            Console.WriteLine($"[WorldBuilder] WorldContainer: AlertStateManager.Def failed: {ex.GetBaseException().Message}");
        }

        try { wc.InitializeComponent(); }
        catch (Exception ex) {
            Console.WriteLine($"[WorldBuilder] WorldContainer.InitializeComponent partial: {ex.GetBaseException().Message}");
        }
        wc.SetID(0);  // sets id=0 and ParentWorldId=0
        if (!ClusterManager.Instance.WorldContainers.Contains(wc))
            ClusterManager.Instance.RegisterWorldContainer(wc);

        // Directly create and start AlertStateManager.Instance on the WorldContainer.
        // KPrefabID.CreateSMIS/StartSMIS may not fire in headless (no KPrefabID on this manually-created GO,
        // or SMC defHandle not initialized properly). The task is identical to StartMonitor<T> for dupes:
        //   new AlertStateManager.Instance(wc).StartSM()
        // This populates wc.m_alertManager lazily on next AlertManager access.
        // Start AlertStateManager.Instance so WorldContainer.AlertManager is non-null.
        // BreathMonitor.IsLowBreath calls wc.AlertManager which asserts non-null → NPE every tick.
        // AlertStateManager.Instance takes (target, def) — different from most monitors.
        try {
            var alertSmi = new AlertStateManager.Instance(wc, new AlertStateManager.Def());
            alertSmi.StartSM();
            var am = wc.AlertManager;
            Console.WriteLine($"[WorldBuilder] AlertStateManager.Instance started: AlertManager={(am != null ? "OK" : "null")}");
        } catch (Exception ex) {
            Console.WriteLine($"[WorldBuilder] AlertStateManager.StartSM partial: {ex.GetBaseException().Message}");
        }

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
        // GeneratedBuildings.LoadGeneratedBuildings() has Activator.CreateInstance() OUTSIDE its
        // try-catch, so a single bad config ctor aborts the entire loop → 0 defs registered.
        // Our version wraps BOTH ctor and RegisterBuilding in per-config try-catch.
        //
        // Expected outcome: 1000+ of ~1433 configs succeed.
        // Expected failures: rendering-heavy configs (KBatchedAnimController NPE in
        //   BuildingLoader.CreateBuildingComplete → Add2DComponents). Buildings without
        //   AnimFiles (flag=false) or with BlockTileAtlas take a different path and mostly succeed.
        var configType = typeof(IBuildingConfig);
        var types = typeof(BuildingConfigManager).Assembly.GetTypes()
            .Where(t => configType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();

        var success = 0;
        var failed = 0;
        var skipped = 0;
        foreach (var type in types) {
            try {
                var config = (IBuildingConfig)Activator.CreateInstance(type);
                if (!DlcManager.IsCorrectDlcSubscribed(config)) { skipped++; continue; }
                BuildingConfigManager.Instance.RegisterBuilding(config);
                success++;
            } catch (Exception ex) {
                failed++;
                if (failed <= 5) {
                    Console.WriteLine($"[BuildingDef] FAIL [{type.Name}]: {ex.GetBaseException().GetType().Name}: {ex.GetBaseException().Message}");
                }
            }
        }
        Console.WriteLine($"[BuildingDef] Registered {success}/{types.Count} building defs ({failed} failed, {skipped} DLC-skipped)");
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

    /// <summary>
    /// Spawns 3 starter duplicants, each in their own gas cell with sufficient atmosphere.
    /// Spreading dupes across separate cells prevents atmosphere-sharing that drops mass below
    /// the SafeFlags.IsBreathable threshold, which causes idleCell=own cell (cost=0) → no movement.
    /// </summary>
    private void SpawnStarterMinions() {
        var startCell = FindColonySpawnCell();
        // Only request as many cells as we want, but spawn ONLY the unique cells returned.
        // Do NOT fall back to startCell for missing slots — a dupe sharing a cell halves the
        // atmosphere mass per dupe, pushing mass below IsBreathable threshold → allMet=False.
        var cells = FindSpawnCells(startCell, 3);
        Console.WriteLine($"[SpawnMinion] Spawning {cells.Count} dupe(s) (unique gas cells found={cells.Count}/3)");
        for (var i = 0; i < cells.Count; i++) {
            var cell = cells[i];
            var x = cell % Grid.WidthInCells;
            var y = cell / Grid.WidthInCells;
            // PlaceOtherEntities(entity, rootCell=0) computes cell = Grid.OffsetCell(0, x, y) = x + y*Width
            // so passing global (x,y) directly gives the correct global cell.
            var entity = new TemplateClasses.Prefab("Minion", TemplateClasses.Prefab.Type.Other, x, y, (SimHashes)0);
            var go = TemplateLoader.PlaceOtherEntities(entity, 0);
            if (go != null) {
                Console.WriteLine($"[SpawnMinion] Spawned Minion {i} at ({x},{y}) cell={cell} mass={Grid.Mass[cell]:F2}");
                _spawnedMinions.Add(go);
                _directlySpawnedEntities.Add(("Minion", x, y));
                if (!_prefabSizeMap.ContainsKey("Minion")) CaptureEntitySize("Minion", go);
            } else {
                Console.WriteLine($"[SpawnMinion] Failed to spawn Minion {i} at ({x},{y}) cell={cell}");
            }
        }
        Console.WriteLine($"[WorldBuilder] {_spawnedMinions.Count} starter minion(s) spawned");
    }

    /// <summary>
    /// Starting from startCell, zigzag-scans horizontally to find `count` distinct gas cells,
    /// each with solid floor below and sufficient mass (>0.5 kg) so every dupe has breathable air.
    /// </summary>
    private static List<int> FindSpawnCells(int startCell, int count) {
        var result = new List<int>(count);
        var used = new HashSet<int>();
        // Zigzag: 0,−1,+1,−2,+2,...,−6,+6 (±6 cells max).
        // Narrow range keeps all dupes in the same connected cave pocket.
        // No padding — fewer dupes in distinct cells beats duplicates sharing a cell
        // (shared cells halve per-dupe atmosphere, pushing mass below allMet threshold).
        for (var x = 0; result.Count < count && x <= 12; x++) {
            var candidate = startCell + (x % 2 == 0 ? x / 2 : -(x + 1) / 2);
            if (used.Contains(candidate)) continue;
            var floorCell = candidate - Grid.WidthInCells;
            if (!Grid.IsValidCell(candidate) || !Grid.IsValidCell(floorCell)) continue;
            if (!Grid.Solid[floorCell] || Grid.Solid[candidate]) continue;
            var elem = Grid.Element[candidate];
            if (elem == null || !elem.IsGas) continue;
            if (Grid.Mass[candidate] < 0.5f) continue;
            result.Add(candidate);
            used.Add(candidate);
        }
        if (result.Count < count)
            Console.WriteLine($"[SpawnFinder] Only {result.Count}/{count} unique gas cells in ±6 range — spawning fewer dupes");
        return result;
    }

    /// <summary>
    /// Finds the best cell to spawn duplicants, using the real colony terrain.
    /// Priority 1: locate the Telepad/PrintingPod and stand on its floor.
    /// Priority 2: scan Grid for the first breathable cell (non-Vacuum) above a solid floor.
    /// Requires Sim.Start() to have run (Grid.Solid must be populated).
    /// </summary>
    private int FindColonySpawnCell() {
        var w = Grid.WidthInCells;

        // Priority 1: scan upward from HQ cell (captured from SpawnData during SpawnEntities).
        // The starter cave: CO2 at y≈196 (2 cells, low mass), Oxygen at y≈198+ (40 cells, higher mass).
        // Prefer Oxygen zone — more connected cells = more room for 3 dupes.
        if (_hqCell >= 0) {
            var firstGasCell = -1;  // fallback: first any-gas cell with solid floor
            Console.WriteLine($"[ScanAboveHQ] Starting scan from HQ cell={_hqCell} ({_hqCell % w},{_hqCell / w}), dy=1..30");
            for (var dy = 1; dy <= 30; dy++) {
                var candidate = _hqCell + dy * w;
                if (!Grid.IsValidCell(candidate)) {
                    Console.WriteLine($"[ScanAboveHQ] dy={dy} y={candidate / w} → break (invalid cell)");
                    break;
                }
                var floorCell = candidate - w;
                var solid = Grid.Solid[candidate];
                var elem = Grid.Element[candidate];
                var mass = Grid.Mass[candidate];
                Console.WriteLine($"[ScanAboveHQ] dy={dy} y={candidate / w} solid={solid} elem={elem?.tag ?? "null"} mass={mass:F2} floorSolid={Grid.Solid[floorCell]}");
                if (solid) continue; // skip solid cells (rock layers between HQ and O2 cave)
                if (elem == null || elem.IsLiquid || elem.id == SimHashes.Vacuum) continue;
                if (!Grid.Solid[floorCell]) continue; // no floor to stand on
                // Prefer Oxygen cells (higher mass, more connected space)
                if (elem.id == SimHashes.Oxygen && mass > 1.0f) {
                    Console.WriteLine($"[SpawnFinder] Oxygen zone above HQ: ({candidate % w},{candidate / w}) mass={mass:F2}");
                    return candidate;
                }
                // Record first non-vacuum gas cell as fallback (e.g. CO2 layer)
                if (firstGasCell < 0)
                    firstGasCell = candidate;
            }
            if (firstGasCell >= 0) {
                var e = Grid.Element[firstGasCell];
                Console.WriteLine($"[SpawnFinder] No O2 found above HQ, using first gas cell: ({firstGasCell % w},{firstGasCell / w}) elem={e?.tag} mass={Grid.Mass[firstGasCell]:F2}");
                return firstGasCell;
            }

            // Priority 1b: O2 exists laterally (e.g. y=204-205 has O2 but no floor directly above HQ).
            // Scan a ±15 wide band at dy=8..25 to find an O2 cell that has solid floor below it.
            Console.WriteLine($"[SpawnFinder] Vertical scan found no valid cell — trying horizontal O2 scan (dy=8..25, dx=±15)");
            for (var dy = 8; dy <= 25; dy++) {
                for (var dx = -15; dx <= 15; dx++) {
                    var c = _hqCell + dy * w + dx;
                    if (!Grid.IsValidCell(c) || Grid.Solid[c]) continue;
                    var floorC = c - w;
                    if (!Grid.IsValidCell(floorC) || !Grid.Solid[floorC]) continue;
                    var e = Grid.Element[c];
                    if (e == null || e.IsLiquid || e.id == SimHashes.Vacuum) continue;
                    if (e.id == SimHashes.Oxygen && Grid.Mass[c] > 1.0f) {
                        Console.WriteLine($"[SpawnFinder] O2 with floor found laterally at ({c % w},{c / w}) dx={dx} dy={dy} mass={Grid.Mass[c]:F2}");
                        return c;
                    }
                }
            }
            Console.WriteLine($"[SpawnFinder] HQ known at cell={_hqCell} but no gas cell found in 30-row+lateral scan, falling back");
        } else {
            Console.WriteLine("[SpawnFinder] _hqCell not set — HQ not found in SpawnData");
        }

        // Priority 2: scan the centre half of the map for a gas cell over a solid floor.
        for (var y = Grid.HeightInCells * 3 / 4; y >= Grid.HeightInCells / 4; y--) {
            for (var x = w / 4; x < w * 3 / 4; x++) {
                var cell = y * w + x;
                var floorCell = cell - w;
                if (!Grid.IsValidCell(cell) || !Grid.IsValidCell(floorCell)) continue;
                var elem = Grid.Element[cell];
                if (elem == null) continue;
                if (Grid.Solid[floorCell] && !Grid.Solid[cell]
                    && elem.id != SimHashes.Vacuum && !elem.IsLiquid) {
                    Console.WriteLine($"[SpawnFinder] Auto-detected spawn at ({x},{y}) cell={cell}");
                    return cell;
                }
            }
        }

        // Fallback: world centre
        var fallbackCell = (Grid.HeightInCells / 2) * w + w / 2;
        Console.WriteLine($"[SpawnFinder] WARN: no suitable spawn found, using world centre cell={fallbackCell}");
        return fallbackCell;
    }

    /// <summary>
    /// Instantiates entities from SpawnData for each world in the cluster.
    /// Mirrors WorldGenSpawner.PlaceTemplates() but without SaveLoader/fog-of-war dependency.
    /// Applies world offsets to positions, then calls the appropriate TemplateLoader function
    /// for each entity type: PlaceBuilding / PlaceElementalOres / PlaceOtherEntities / PlacePickupables.
    /// </summary>
    private void SpawnEntities(Cluster cluster) {
        var spawned = 0;
        var skipped = 0;
        _spawnedMinions.Clear();
        foreach (var world in cluster.worlds) {
            var offsetX = world.data?.world?.offset.x ?? 0;
            var offsetY = world.data?.world?.offset.y ?? 0;

            // Apply world offset to spawned entity types (mirrors WorldGenSpawner.PlaceTemplates).
            // Also capture HQ coordinates for FindColonySpawnCell() — done here because offsets
            // are already applied and we don't need the building to be actually spawned.
            foreach (var b in world.SpawnData.buildings) {
                b.location_x += offsetX;
                b.location_y += offsetY;
                b.type = Prefab.Type.Building; // real game sets this explicitly
                if (_hqCell < 0 && (b.id == "Headquarters" || b.id == "GeneShuffler" || b.id == "Telepad")) {
                    _hqCell = b.location_y * Grid.WidthInCells + b.location_x;
                    Console.WriteLine($"[SpawnFinder] HQ from SpawnData: id={b.id} at ({b.location_x},{b.location_y}) cell={_hqCell}");
                }
            }
            foreach (var e in world.SpawnData.otherEntities) {
                e.location_x += offsetX;
                e.location_y += offsetY;
                e.type = Prefab.Type.Other;
            }

            // Diagnose building def availability before attempting spawn
            {
                var blist = world.SpawnData.buildings;
                Console.WriteLine($"[SpawnDiag] BuildingDefs count: {Assets.BuildingDefs?.Count ?? 0}");
                var hqDef = Assets.GetBuildingDef("Headquarters");
                Console.WriteLine($"[SpawnDiag] HQ def: {(hqDef != null ? "OK" : "NULL")}");
                Console.WriteLine($"[SpawnDiag] Buildings in SpawnData: {blist?.Count ?? 0}");
                if (blist != null) {
                    foreach (var b in blist.Take(5)) {
                        var def = Assets.GetBuildingDef(b.id);
                        Console.WriteLine($"[SpawnDiag]   id=\"{b.id}\" type={b.type} def={(def != null ? "OK" : "NULL")} xy=({b.location_x},{b.location_y})");
                    }
                }
            }

            // Spawn buildings (Headquarters, Tiles, etc.) — critical: creates HQ GO so
            // Components.Telepads is populated and FindColonySpawnCell() can locate the printer.
            foreach (var b in world.SpawnData.buildings) {
                var go = TemplateLoader.PlaceBuilding(b, 0);
                if (go != null) {
                    Console.WriteLine($"[Entities] Building spawned: {b.id} at ({b.location_x},{b.location_y})");
                    spawned++;
                    if (!_prefabSizeMap.ContainsKey(b.id)) CaptureEntitySize(b.id, go);

                    // P0 Fix 3 (Sol DS-005): call Spawn() on Telepad/HQ so OnSpawn() runs and
                    // registers the component in Components.Telepads → Immigration.IsHalted()=false.
                    // KMonoBehaviour.Spawn() is public and safe to call if !isSpawned.
                    if (b.id == "Headquarters" || b.id == "Telepad" || b.id == "GeneShuffler") {
                        try {
                            var kmono = go.GetComponent<KMonoBehaviour>();
                            if (kmono != null && !kmono.isSpawned) {
                                kmono.Spawn();
                                Console.WriteLine($"[Entities] {b.id}.Spawn() done: Telepads={Components.Telepads?.Count ?? -1}");
                            }
                        } catch (Exception ex) {
                            Console.WriteLine($"[Entities] {b.id}.Spawn() partial: {ex.GetBaseException().Message}");
                        }
                    }
                } else {
                    Console.WriteLine($"[Entities] Building skipped (no def or invalid cell): {b.id}");
                    skipped++;
                }
            }

            // Spawn other entities (critters, dupes, etc.)
            foreach (var e in world.SpawnData.otherEntities) {
                var go = TemplateLoader.PlaceOtherEntities(e, 0);
                if (go != null) {
                    Console.WriteLine($"[Entities] Spawned: {e.id} at ({e.location_x},{e.location_y})");
                    spawned++;
                    if (e.id == "Minion" || e.id == "BionicMinion") _spawnedMinions.Add(go);
                    if (!_prefabSizeMap.ContainsKey(e.id)) CaptureEntitySize(e.id, go);
                } else {
                    Console.WriteLine($"[Entities] Skipped (no prefab or invalid cell): {e.id}");
                    skipped++;
                }
            }

            // elementalOres and pickupables skipped — PlaceElementalOres/PlacePickupables
            // crash in headless (Substance.SpawnResource → KInstantiate NullRef) and are
            // not required for duplicant navigation/AI.
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
    /// Primary path: call BaseMinionConfig.BaseOnSpawn() directly — this is exactly what the
    /// real game runs via Unity.Start() → KMonoBehaviour.Spawn() → MinionConfig.OnSpawn().
    /// BaseOnSpawn adds ALL 8 sensors, creates RationalAi.Instance (starts all 52 sub-SMs),
    /// and adds 7 navigator transition layers.
    ///
    /// Prerequisite: ValidateProxy() must run BEFORE BaseOnSpawn because
    ///   AssignableReachabilitySensor.ctor calls identity.assignableProxy.Get() → NPE if null.
    ///
    /// Fallback (if BaseOnSpawn crashes): start only safe sensors + critical monitors individually.
    ///   AssignableReachabilitySensor excluded in fallback — still NPEs on assignableProxy.
    ///
    /// Always (regardless of BaseOnSpawn success/failure):
    ///   - Start Navigator SM (BaseOnSpawn adds layers but does NOT call nav.smi.StartSM()).
    ///   - Spawn Brain (sets running=true, registers with BrainScheduler).
    ///   - Pre-add GameTags.Idle (breaks IdleCellSensor deadlock).
    ///   - Spawn Sensors (subscribes OnBrainPreUpdate; must follow Brain.Spawn()).
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

                // ValidateProxy BEFORE BaseOnSpawn: AssignableReachabilitySensor.ctor calls
                // identity.assignableProxy.Get() → NPE if MinionIdentity.OnSpawn() hasn't run.
                // This normally runs in MinionIdentity.OnSpawn() → OnAddDupe callback.
                var identity = go.GetComponent<MinionIdentity>();
                if (identity != null && identity.assignableProxy?.Get() == null) {
                    try {
                        identity.ValidateProxy();
                        Console.WriteLine($"[FixRationalAi] {go.name}: ValidateProxy OK, proxy={identity.assignableProxy?.Get() != null}");
                    } catch (Exception ex) {
                        Console.WriteLine($"[FixRationalAi] {go.name}: ValidateProxy partial: {ex.GetBaseException().Message}");
                    }
                }

                // Primary: BaseMinionConfig.BaseOnSpawn — adds all 8 sensors, starts all 52 SMs
                // via RationalAi.Instance.StartSM(), adds 7 navigator transition layers.
                // Mirrors exactly what the real game does in MinionConfig.OnSpawn().
                var baseOnSpawnOk = false;
                try {
                    BaseMinionConfig.BaseOnSpawn(go, new Tag("Minion"), BaseMinionConfig.BaseRationalAiStateMachines());
                    baseOnSpawnOk = true;
                    Console.WriteLine($"[FixRationalAi] {go.name}: BaseOnSpawn OK (all SMs + sensors + nav layers)");
                } catch (Exception ex) {
                    Console.WriteLine($"[FixRationalAi] {go.name}: BaseOnSpawn FAILED: {ex.GetBaseException().Message}\n  {ex.GetBaseException().StackTrace?.Split('\n')[0]}");
                }

                if (!baseOnSpawnOk) {
                    // Fallback: minimal sensor set + critical monitors only.
                    // PathProberSensor + SafeCellSensor: needed for SafeFlags / IdleCellQuery allMet.
                    // IdleCellSensor: finds the idle cell Brain picks for IdleChore.
                    // AssignableReachabilitySensor EXCLUDED: ctor NPEs on assignableProxy.Get().
                    var sensorsFb = go.GetComponent<Sensors>();
                    if (sensorsFb != null) {
                        sensorsFb.Add(new PathProberSensor(sensorsFb));
                        sensorsFb.Add(new SafeCellSensor(sensorsFb));
                        sensorsFb.Add(new IdleCellSensor(sensorsFb));
                    }

                    // IdleMonitor first so IdleChore exists in ChoreProvider before other monitors.
                    var idleMonitorSmi = new IdleMonitor.Instance(smc);
                    idleMonitorSmi.StartSM();
                    Console.WriteLine($"[FixRationalAi] {go.name}: fallback IdleMonitor started");

                    StartMonitor<BreathMonitor>(smc,  "BreathMonitor");
                    StartMonitor<CalorieMonitor>(smc, "CalorieMonitor");
                    StartMonitor<BladderMonitor>(smc, "BladderMonitor");
                    StartMonitor<StaminaMonitor>(smc, "StaminaMonitor");

                    // Navigator transition layers (BaseOnSpawn adds them; must add manually in fallback).
                    var navFb = go.GetComponent<Navigator>();
                    if (navFb?.transitionDriver != null) {
                        navFb.transitionDriver.overrideLayers.Add(new BipedTransitionLayer(navFb, 3.325f, 2.5f));
                        navFb.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(navFb));
                        navFb.transitionDriver.overrideLayers.Add(new LadderDiseaseTransitionLayer(navFb));
                        navFb.transitionDriver.overrideLayers.Add(new NavTeleportTransitionLayer(navFb));
                    }
                }

                // Navigator SM: BaseOnSpawn adds transition layers but does NOT call nav.smi.StartSM().
                // The SM must be started so normal.moving fires and Navigator.Advance() works.
                var nav3 = go.GetComponent<Navigator>();
                if (nav3 != null && !nav3.IsInitialized()) {
                    try {
                        nav3.InitializeComponent();
                        Console.WriteLine($"[FixRationalAi] {go.name}: Navigator.InitializeComponent OK, NavGrid={nav3.NavGrid?.id ?? "null"}");
                    } catch (Exception ex) {
                        Console.WriteLine($"[FixRationalAi] {go.name}: Navigator.InitializeComponent partial: {ex.GetBaseException().Message}");
                    }
                }
                if (nav3 != null && nav3.GetSMI() == null) {
                    try {
                        nav3.smi.StartSM(); // lazy-creates instance, transitions to normal.stopped
                        Console.WriteLine($"[FixRationalAi] {go.name}: Navigator.smi.StartSM() called, smi={nav3.GetSMI() != null}");
                    } catch (Exception ex) {
                        Console.WriteLine($"[FixRationalAi] {go.name}: Navigator.smi.StartSM partial: {ex.GetBaseException().Message}");
                    }
                }

                // Spawn Brain — sets running=true + choreConsumer + registers with BrainScheduler.
                // Without this: Brain.IsRunning()=false → BrainGroup.RenderEveryTick skips brain
                // → UpdateBrain() never called → chore never picked even though IdleChore exists.
                var brain = go.GetComponent<MinionBrain>();
                if (brain != null && !brain.isSpawned) {
                    brain.Spawn();
                    Console.WriteLine($"[FixRationalAi] {go.name}: Brain spawned (running={brain.IsRunning()})");
                }

                // Pre-add GameTags.Idle to break the IdleCellSensor deadlock.
                // IdleCellSensor.Update() returns InvalidCell when !prefabid.HasTag(GameTags.Idle).
                // IdleChore adds the tag only AFTER Brain picks it — circular dependency.
                // Pre-adding breaks the cycle; IdleChore will re-add/remove via ToggleTag normally.
                go.GetComponent<KPrefabID>()?.AddTag(GameTags.Idle);

                // Spawn Sensors — subscribes OnBrainPreUpdate to Brain.onPreUpdate.
                // Must happen AFTER Brain.Spawn() so Brain.onPreUpdate delegate is initialised.
                var sensors2 = go.GetComponent<Sensors>();
                if (sensors2 != null && !sensors2.isSpawned) {
                    sensors2.Spawn();
                }

                fixedCount++;
                Console.WriteLine($"[FixRationalAi] {go.name}: OK (baseOnSpawnOk={baseOnSpawnOk})");
            } catch (Exception ex) {
                Console.WriteLine($"[FixRationalAi] {go.name}: ERROR: {ex.GetBaseException().Message}\n  {ex.GetBaseException().StackTrace?.Split('\n')[0]}");
            }
        }
        Console.WriteLine($"[WorldBuilder] FixRationalAi done: {fixedCount}/{_spawnedMinions.Count} minion(s) started");
    }

    // Reflection cache for Brain.running (private field)
    private static readonly FieldInfo _brainRunningField =
        typeof(Brain).GetField("running", BindingFlags.Instance | BindingFlags.NonPublic)!;

    /// <summary>
    /// Bootstraps the Brain→Chore→Navigator pipeline for each spawned critter (CreatureBrain).
    ///
    /// Same problem as dupes: TriggerLifecycle fires Brain.OnSpawn() but may partially fail,
    /// leaving brain.running=false or Navigator SM unstarted → CreatureBrainGroup skips the brain.
    ///
    /// Key differences from FixRationalAi (dupe fix):
    ///   - No Schedule/Schedulable — creatures have no work schedule.
    ///   - No RationalAi — creature chores come from ChoreTable + state machine defs (already
    ///     started by KPrefabID.OnSpawn → StartSMIS during TriggerLifecycle).
    ///   - Sensors not present on most creatures (only Rovers/FetchDrones have them).
    ///   - Uses CreatureBrainGroup (GameTags.CreatureBrain) not DupeBrainGroup.
    ///
    /// Steps per creature:
    ///   1. Ensure brain.running=true — if false, re-register in BrainScheduler via Remove+Add.
    ///   2. Ensure Navigator SM started — if nav.GetSMI()==null, call nav.smi.StartSM().
    ///   3. Ensure consumerState != null (ChoreConsumer.OnSpawn may have failed).
    ///   4. Log diagnostic: cell, brain.running, currentChore for each creature.
    /// </summary>
    private void FixCreatureBrains() {
        var fixed_brain = 0;
        var fixed_nav   = 0;
        var fixed_cs    = 0;
        var alreadyOk   = 0;
        var total       = 0;

        foreach (var brain in Components.Brains.Items) {
            if (brain is not CreatureBrain) continue;
            total++;
            var go = brain.gameObject;
            if (go == null) continue;

            try {
                // ── Step 1: ensure brain.running = true ──────────────────────────────────
                // Brain.Spawn() is no-op when isSpawned=true (already set by TriggerLifecycle).
                // If OnSpawn crashed after "running=true" was set, brain is registered but running.
                // If Components.Brains.Add() failed (BrainGroup.HasTag mismatch), brain is not in
                // CreatureBrainGroup → UpdateBrain() never called → creature stuck.
                // Fix: remove from Components.Brains, force running=true via reflection, re-add
                // to retrigger BrainScheduler.OnAddBrain → HasTag(CreatureBrain) → AddBrain().
                if (!brain.IsRunning()) {
                    try { Components.Brains.Remove(brain); } catch { /* ignore if not registered */ }
                    _brainRunningField?.SetValue(brain, true);
                    Components.Brains.Add(brain);
                    Console.WriteLine($"[Animals] {go.name}: brain was NOT running — re-registered");
                    fixed_brain++;
                } else {
                    alreadyOk++;
                }

                // ── Step 2: ensure Navigator SM is running ────────────────────────────────
                // Navigator.OnSpawn() starts the SM (normal.stopped state). In headless it may
                // throw (TargetLocator KInstantiate → NullRef) leaving _smi=null → no movement.
                var nav = go.GetComponent<Navigator>();
                if (nav != null) {
                    if (!nav.IsInitialized()) {
                        try { nav.InitializeComponent(); }
                        catch (Exception ex) {
                            Console.WriteLine($"[Animals] {go.name}: Navigator.InitializeComponent partial: {ex.GetBaseException().Message}");
                        }
                    }
                    if (nav.GetSMI() == null) {
                        try {
                            nav.smi.StartSM();
                            fixed_nav++;
                            Console.WriteLine($"[Animals] {go.name}: Navigator.StartSM() called");
                        } catch (Exception ex) {
                            Console.WriteLine($"[Animals] {go.name}: Navigator.StartSM partial: {ex.GetBaseException().Message}");
                        }
                    }
                }

                // ── Step 3: ensure ChoreConsumer.consumerState != null ────────────────────
                var cc = go.GetComponent<ChoreConsumer>();
                if (cc != null && cc.consumerState == null) {
                    try {
                        cc.consumerState = new ChoreConsumerState(cc);
                        fixed_cs++;
                    } catch (Exception ex) {
                        Console.WriteLine($"[Animals] {go.name}: consumerState init failed: {ex.GetBaseException().Message}");
                    }
                }

                // ── Step 4: Sensors (Rovers/FetchDrones only — standard critters lack them) ──
                var sensors = go.GetComponent<Sensors>();
                if (sensors != null && !sensors.isSpawned) {
                    try { sensors.Spawn(); }
                    catch (Exception ex) {
                        Console.WriteLine($"[Animals] {go.name}: sensors.Spawn partial: {ex.GetBaseException().Message}");
                    }
                }

                // ── Diagnostic: one line per creature ────────────────────────────────────
                var cell   = Grid.PosToCell(go);
                var chore  = go.GetComponent<ChoreDriver>()?.GetCurrentChore();
                Console.WriteLine($"[Animals] {go.name}: cell={cell} running={brain.IsRunning()} nav={(nav?.GetSMI() != null ? "OK" : "null")} chore={chore?.GetType().Name ?? "null"} consumerState={cc?.consumerState != null}");

            } catch (Exception ex) {
                Console.WriteLine($"[Animals] {go.name}: ERROR: {ex.GetBaseException().Message}\n  {ex.GetBaseException().StackTrace?.Split('\n')[0]}");
            }
        }

        Console.WriteLine($"[Animals] FixCreatureBrains: total={total} alreadyOk={alreadyOk} fixed_brain={fixed_brain} fixed_nav={fixed_nav} fixed_cs={fixed_cs}");
    }

    /// <summary>
    /// Starts a StateMachine.Instance for the given SM type on the target.
    /// Mirrors the IdleMonitor pattern: new TSM.Instance(target); instance.StartSM().
    /// Uses reflection to construct TSM.Instance — avoids complex nested generic constraints.
    /// Wrapped in try/catch — monitors access Db.Amounts which may be partial in headless.
    /// </summary>
    private static void StartMonitor<TSM>(IStateMachineTarget target, string name)
        where TSM : StateMachine {
        try {
            var instanceType = typeof(TSM).GetNestedType("Instance");
            if (instanceType == null) {
                Console.WriteLine($"[StartMonitor] {name}: nested Instance type not found");
                return;
            }
            var instance = (StateMachine.Instance)Activator.CreateInstance(instanceType, target);
            instance.StartSM();
            Console.WriteLine($"[StartMonitor] {name}: started OK");
        } catch (Exception ex) {
            Console.WriteLine($"[StartMonitor] {name}: {ex.GetBaseException().Message}");
        }
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
    /// <summary>
    /// Logs element/mass data around the PrintingPod (Telepad) to diagnose oxygen presence.
    /// Call before and after a SimDLL tick to distinguish stale Grid data from genuine vacuum.
    /// </summary>
    private static void DiagnosePrinterArea(string label) {
        try {
            var all = UnityEngine.Object.FindObjectsOfType<KMonoBehaviour>() ?? Array.Empty<KMonoBehaviour>();
            var printer = all.FirstOrDefault(x => {
                var n = x.GetType().Name;
                return n == "Telepad" || n == "StartingTelepad" || n.Contains("Headquarters");
            });
            if (printer == null) {
                Console.WriteLine($"[PrinterDiag:{label}] No Telepad/Headquarters found");
                return;
            }
            var printerCell = Grid.PosToCell(printer.transform.position);
            var px = printerCell % Grid.WidthInCells;
            var py = printerCell / Grid.WidthInCells;
            Console.WriteLine($"[PrinterDiag:{label}] {printer.GetType().Name} at cell={printerCell} ({px},{py})");
            for (var dy = -2; dy <= 2; dy++) {
                for (var dx = -2; dx <= 2; dx++) {
                    var c = printerCell + dy * Grid.WidthInCells + dx;
                    if (!Grid.IsValidCell(c)) continue;
                    var elem = Grid.Element[c];
                    Console.WriteLine($"  ({dx:+0;-0},{dy:+0;-0}) cell={c}: elem={elem?.tag} solid={Grid.Solid[c]} mass={Grid.Mass[c]:F3} gas={elem?.IsGas}");
                }
            }
        } catch (Exception ex) {
            Console.WriteLine($"[PrinterDiag:{label}] Error: {ex.GetBaseException().Message}");
        }
    }

    /// <summary>
    /// Logs element name + mass for a (2*radius+1)×(2*radius+1) area centred on (cx,cy).
    /// Used to diagnose whether gas cells exist around the printer before/after N2 fixes.
    /// </summary>
    private static void LogCellArea(int cx, int cy, int radius) {
        var w = Grid.WidthInCells;
        for (var dy = radius; dy >= -radius; dy--) {
            var row = new System.Text.StringBuilder();
            for (var dx = -radius; dx <= radius; dx++) {
                var cell = (cy + dy) * w + (cx + dx);
                if (!Grid.IsValidCell(cell)) { row.Append(" [INV]"); continue; }
                var elem = Grid.Element[cell];
                var mass = Grid.Mass[cell];
                var solid = Grid.Solid[cell] ? "S" : " ";
                row.Append($" [{solid}{elem?.tag.ToString() ?? "?"}:{mass:F1}]");
            }
            Console.WriteLine($"[CellArea] y={cy + dy}:{row}");
        }
    }

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
