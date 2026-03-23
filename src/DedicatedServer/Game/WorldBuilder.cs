using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Database;
using Klei;
using Klei.AI;
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

    /// <summary>Live list of all spawned duplicant GameObjects (Minion + BionicMinion).
    /// Used by RealWorldState to read current chore, SM state, and nav position each tick.</summary>
    public IReadOnlyList<GameObject> SpawnedMinions => _spawnedMinions;

    // HQ/Headquarters cell read directly from SpawnData during SpawnEntities(),
    // used by FindColonySpawnCell() to locate the starter cave without needing the GO.
    private int _hqCell = -1;

    /// <summary>
    /// Maps prefab ID → (w, h) in cells, populated from live spawned GOs during SpawnEntities().
    /// Used by RealWorldState.GetEntitySize to get correct sizes without relying on Assets.GetPrefab.
    /// </summary>
    public IReadOnlyDictionary<string, (int w, int h)> PrefabSizeMap => _prefabSizeMap;
    public static IReadOnlyDictionary<string, BuildingDef> BuildingDefCache => _buildingDefCache;
    private readonly Dictionary<string, (int w, int h)> _prefabSizeMap = new();

    /// <summary>
    /// Entities spawned directly (not via spawnData.otherEntities → SpawnEntities).
    /// Used by RealWorldState.GetEntitiesBytes so /api/entities includes them.
    /// </summary>
    public IReadOnlyList<(string id, int x, int y)> DirectlySpawnedEntities => _directlySpawnedEntities;
    private readonly List<(string id, int x, int y)> _directlySpawnedEntities = new();

    /// <summary>
    /// Buildings collected from SpawnData during SpawnEntities() with world offsets already applied.
    /// Used by RealWorldState.GetEntitiesBytes instead of world.SpawnData.buildings so the
    /// buildings branch is independent of SpawnData validity at query time.
    /// Populated once in SpawnEntities(); never cleared.
    /// </summary>
    public IReadOnlyList<(string id, int x, int y)> TrackedBuildings => _trackedBuildings;
    private readonly List<(string id, int x, int y)> _trackedBuildings = new();

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

    // Reflection accessor for ChoreConsumer.providers (List<ChoreProvider>).
    // IL field name confirmed via Cecil: "providers" (private in original game DLL).
    // DedicatedServer ships the AssemblyExposer-patched DLL (Private="true" in csproj)
    // where all private/internal members are rewritten to Public for compile-time access.
    // At runtime that exposed DLL is loaded → field is Public → BindingFlags.NonPublic alone
    // returns null.  Use Public|NonPublic to cover both the exposed and the original DLL.
    private static readonly FieldInfo _ccProvidersField =
        typeof(ChoreConsumer).GetField("providers",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

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

        // Remove render-only components from critter PREFABS before any instance is spawned.
        // CharacterOverlay.OnSpawn → NameDisplayScreen.AddNewEntry NPEs (no canvas in headless).
        // AnimEventHandler.OnSpawn[IL_0x4c] → animCollider (KBoxCollider2D) NPEs.
        // These must be removed from PREFABS (not instances) so that KInstantiate copies don't
        // inherit them. Removal in FixCreatureBrains() (post-spawn) is too late — TriggerLifecycle
        // has already fired OnSpawn for both, crashing before ChoreConsumer.InitializeComponent()
        // adds ChoreProvider/ChoreDriver/User, leaving those components not initialized.
        FixCreaturePrefabsPreSpawn();

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
                    // P2: ClearUnoccupiedCells is NOT called here.
                    // In SaveLoader new-game path it runs BEFORE Sim.Load() (before cell data exists).
                    // Calling it AFTER SimDataInitializeFromCells+Sim.Start() zeros max_mass for cells
                    // outside declared world bounds → O2 max_mass=0 → BreathMonitor treats as vacuum.
                    Console.WriteLine("[N2] DefineWorldOffsets sent (ClearUnoccupiedCells skipped — would zero O2 mass)");

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

        // MinionGroupProber singleton must exist before SpawnEntities so Pickupable.OnSpawn
        // (→ ReachabilityMonitor.Instance ctor → UpdateReachability → MinionGroupProber.Get().IsAllReachable)
        // does not NPE. OnPrefabInit is invoked via reflection to bypass KObject setup requirement.
        // All cells default to 0 → IsAllReachable returns false (correct: no dupe has probed yet).
        {
            var minionProberGo = new GameObject("MinionGroupProber");
            var prober = minionProberGo.AddComponent<MinionGroupProber>();
            typeof(MinionGroupProber).GetMethod("OnPrefabInit", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(prober, null);
            Console.WriteLine($"[WorldBuilder] MinionGroupProber.Instance={MinionGroupProber.Get() != null}");
        }

        // CellChangeMonitor singleton must exist before SpawnEntities so Pickupable.OnSpawn
        // (→ OnTagsChanged → UpdateListeners → Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler)
        // does not NPE. CellChangeMonitor is a plain C# class (Singleton<T> pattern, no Unity lifecycle)
        // so CreateInstance() is all that's needed. SetGridSize sets gridWidth so MarkDirty is not a no-op.
        if (Singleton<CellChangeMonitor>.Instance == null) {
            Singleton<CellChangeMonitor>.CreateInstance();
            Singleton<CellChangeMonitor>.Instance.SetGridSize(Width, Height);
            Console.WriteLine("[WorldBuilder] CellChangeMonitor initialized");
        }

        // NameDisplayScreen: MUST be initialized BEFORE SpawnEntities() so that all three callers
        // that fire during entity spawning find a non-null Instance:
        //   • OxygenBreather.OnSpawn() [dupe]: Instance.RegisterComponent(go, this)
        //   • CreatureThoughtGraph.Instance ctor [critter]: Instance.RegisterComponent(go, this)
        //   • CritterEmoteMonitor.cooldown.Enter [critter]: Instance.SetThoughtBubbleDisplay(go, ...)
        // All three calls are safe with an initialized-but-empty NameDisplayScreen: RegisterComponent
        // and SetThoughtBubble* both guard on GetEntry(go) returning non-null before doing any work.
        // Creature GOs have CharacterOverlay removed by FixCreaturePrefabsPreSpawn() → GetEntry
        // returns null → all calls return early with no rendering side effects.
        // Previously this block was placed AFTER SpawnEntities (line ~968) — too late for any
        // creature SM ctor / OxygenBreather.OnSpawn to find it. Moving it here fixes the NPE.
        // OnPrefabInit just sets Instance=this. OnSpawn (not called) registers UI overlay events.
        if (NameDisplayScreen.Instance == null) {
            try {
                var ndsGo = new GameObject("NameDisplayScreen");
                var nds = ndsGo.AddComponent<NameDisplayScreen>();
                nds.InitializeComponent(); // OnPrefabInit → Instance = this
                // Fallback: if InitializeComponent crashed before "Instance = this" ran,
                // set Instance directly.
                if (NameDisplayScreen.Instance == null) NameDisplayScreen.Instance = nds;
                Console.WriteLine($"[WorldBuilder] NameDisplayScreen.Instance ready: {NameDisplayScreen.Instance != null}");
            } catch (Exception ex) {
                Console.WriteLine($"[WorldBuilder] NameDisplayScreen init partial: {ex.GetBaseException().Message}");
            }
        }

        // DietManager: KMonoBehaviour singleton that maps creature prefab tag → Diet.
        // DietManager.OnPrefabInit calls CollectSaveDiets(null) which iterates Assets.Prefabs
        // (already populated by RegisterEntities() above) and stores Tag → Diet entries.
        // MUST be initialized AFTER RegisterEntities() and BEFORE SpawnEntities():
        //   • CreatureCalorieMonitor.Stomach ctor: DietManager.Instance.GetPrefabDiet(owner) → NPE
        //   • SolidConsumerMonitor.Instance ctor: DietManager.Instance.GetPrefabDiet(gameObject) → NPE
        // Without this, both SMs were skipped (CreaturePrefab step 6c, MinionPrefab.IsHeadlessUnsafeSM).
        // DietManager.OnSpawn subscribes to DiscoveredResources — not called here (headless, not needed).
        if (DietManager.Instance == null) {
            var dietManagerGo = new GameObject("DietManager");
            var dm = dietManagerGo.AddComponent<DietManager>();
            dm.InitializeComponent(); // OnPrefabInit: CollectSaveDiets → diets populated, Instance = this
            Console.WriteLine($"[WorldBuilder] DietManager.Instance={DietManager.Instance != null}");
        }

        // [GEYSER-DLC] Step A: diagnose DlcManager state vs save DLC state.
        // GeyserGenericConfig.prefabInitFn filters by Game.IsCorrectDlcActiveForCurrentSave
        // (save header), but CreatePrefabs used DlcManager.IsCorrectDlcSubscribed (Steam).
        // Mismatch → list2 smaller than configs → KRandom index out of range → 12 GOs survive.
        {
            var activeDlcs = DlcManager.GetActiveDLCIds();
            var saveDlcIds = SaveLoader.Instance?.GameInfo.dlcIds;
            Console.WriteLine($"[GEYSER-DLC] IsExpansion1Active={DlcManager.IsExpansion1Active()}");
            Console.WriteLine($"[GEYSER-DLC] active DLCs={string.Join(",", activeDlcs)}");
            Console.WriteLine($"[GEYSER-DLC] save dlcIds={string.Join(",", saveDlcIds ?? new System.Collections.Generic.List<string>())}");
        }

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

        // Ensure Game.Instance.accumulators is non-null before ANY dupe TriggerLifecycle fires.
        // Game.OnPrefabInit() crashes at line ~820 (ConduitFlowVisualizer needs Lighting/
        // GlobalResources — absent in headless). Lines 823-824 are never reached:
        //   accumulators = new Accumulators();        // line 823 — NEVER REACHED
        //   plantElementAbsorbers = new ...();        // line 824 — NEVER REACHED
        // OxygenBreather.OnSpawn[IL_0x0021] calls Game.Instance.accumulators.Add("O2", this)
        // → NullReferenceException → kills all 3 dupes during TriggerLifecycle Phase 2
        // → StateMachine.Instance.error=true → all SM ticks halted.
        // Must be set here, BEFORE SpawnStarterMinions (not in a catch block).
        global::Game.Instance.accumulators ??= new Accumulators();
        global::Game.Instance.plantElementAbsorbers ??= new PlantElementAbsorbers();

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

        // Ensure Clearable + Prioritizable on all spawned pickupable / plant GOs.
        // EntityTemplates.CreateBaseOreTemplates / ExtendEntityToBasicPlant add these to every
        // ore and plant prefab but the headless code path sometimes misses them.
        // Must run AFTER SpawnEntities so all entity GOs are live; BEFORE FixCreatureBrains
        // so creature GOs that also have Pickupable are handled by the creature path.
        FixPickupables();

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

        // Register AmountInstance batch updater for the SIM_200ms bucket.
        // AmountInstance.Sim200ms() is intentionally empty — actual updates happen via BatchUpdate,
        // which must be registered explicitly (Game.OnSpawn() line 969 normally does this but
        // OnSpawn is never called in headless). Without this: all Amounts (Stamina, Calories,
        // Stress, Bladder, Breath) are frozen at their initial save-file values — dupes never
        // get hungry, never tire, never wake from sleep (ShouldExitSleep needs stamina >= max).
        SimAndRenderScheduler.instance.RegisterBatchUpdate<ISim200ms, AmountInstance>(AmountInstance.BatchUpdate);
        Console.WriteLine("[WorldBuilder] AmountInstance.BatchUpdate registered for SIM_200ms");

        // ── From Game.OnSpawn() — simulation-critical pieces (lines 949, 965–977) ──────────
        //
        // Game.OnSpawn() is never called in headless: SpawnPlayer() at line 930 requires
        // a full UI chain (null playerPrefab → Player/ScreenPrefabs/CameraController/
        // KInputManager stubs) that would cascade into ~50 lines of fragile stub code.
        // We replicate ONLY the simulation-critical lines below, following the same pattern
        // used above for OnPrefabInit pieces (accumulators, roomProber, mingleCellTracker…).

        // Line 949: TagManager.FillMissingProperNames() — assign display names to Tags that
        // were not explicitly named. Safe post-prefab-load; no-op if already done.
        TagManager.FillMissingProperNames();
        Console.WriteLine("[WorldBuilder] TagManager.FillMissingProperNames() done");

        // Line 965: solidConduitFlow.Initialize() — registers conveyor belt tick callbacks.
        // solidConduitFlow is created at OnPrefabInit line 819 (before the crash at 820),
        // so it is always non-null here. Without Initialize(), solid conveyors never tick.
        global::Game.Instance.solidConduitFlow?.Initialize();
        Console.WriteLine("[WorldBuilder] solidConduitFlow.Initialize() done");

        // Line 966: SimAndRenderScheduler.instance.Add(roomProber)
        // roomProber was created manually above; it must ALSO be registered with the scheduler
        // so ISim1000ms.Sim1000ms() fires each tick and rooms are re-evaluated.
        if (global::Game.Instance.roomProber != null)
            SimAndRenderScheduler.instance?.Add(global::Game.Instance.roomProber);
        Console.WriteLine("[WorldBuilder] roomProber added to SimAndRenderScheduler");

        // Line 967: spaceScannerNetworkManager — created at OnPrefabInit line 834 (after crash).
        // Without this, space scanners, bunker doors and telescope networks never update.
        global::Game.Instance.spaceScannerNetworkManager ??= new SpaceScannerNetworkManager();
        SimAndRenderScheduler.instance?.Add(global::Game.Instance.spaceScannerNetworkManager);
        Console.WriteLine("[WorldBuilder] spaceScannerNetworkManager added to SimAndRenderScheduler");

        // Line 968: KComponentSpawn — normally set by KComponentsInitializer.Awake(), a private
        // Unity callback that never fires in headless. Create an instance and invoke Awake()
        // manually via reflection so KComponentSpawn.instance and comps are initialised.
        if (KComponentSpawn.instance == null) {
            var kcsi = (KComponentsInitializer)FormatterServices
                .GetUninitializedObject(typeof(KComponentsInitializer));
            typeof(KComponentsInitializer)
                .GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(kcsi, null);
            Console.WriteLine($"[WorldBuilder] KComponentsInitializer.Awake() invoked (instance={KComponentSpawn.instance != null})");
        }
        if (KComponentSpawn.instance != null)
            SimAndRenderScheduler.instance?.Add(KComponentSpawn.instance);
        Console.WriteLine($"[WorldBuilder] KComponentSpawn registered: {KComponentSpawn.instance != null}");

        // Line 970: SolidTransferArm batch update for ISim1000ms — conveyor arm ticking.
        // Without this, solid conveyor arms never call BatchUpdate and always sit idle.
        SimAndRenderScheduler.instance?.RegisterBatchUpdate<ISim1000ms, SolidTransferArm>(SolidTransferArm.BatchUpdate);
        Console.WriteLine("[WorldBuilder] SolidTransferArm.BatchUpdate registered for SIM_1000ms");

        // Lines 952–956: mark base already created (loaded-from-save) and fire creation events.
        // Prevents dupes from repeating new-colony intro sequences on reload.
        if (SaveLoader.Instance?.loadedFromSave == true) {
            global::Game.Instance.baseAlreadyCreated = true;
            global::Game.Instance.Trigger(-1992507039);
            global::Game.Instance.Trigger(-838649377);
            Console.WriteLine("[WorldBuilder] base-already-created events fired (loadedFromSave=true)");
        }

        Console.WriteLine("[WorldBuilder] Game.OnSpawn() simulation pieces done");

        // ── From ClusterManager.OnSpawn() lines 164–167 ─────────────────────────────────
        //
        // ClusterManager.OnSpawn() is blocked by UpdateWorldReverbSnapshot() which calls
        // AudioMixer.instance.PauseSpaceVisibleSnapshot() — NPE when AudioMixer.instance is
        // null in headless. We replicate the only simulation-critical piece: m_grid init.
        // m_grid stores the cluster-space asteroid grid used by world navigation and travel.
        {
            var cm = ClusterManager.Instance;
            if (cm != null) {
                var cmType = typeof(ClusterManager);
                var mGridField = cmType.GetField(
                    "m_grid", BindingFlags.Instance | BindingFlags.NonPublic);
                if (mGridField?.GetValue(cm) == null) {
                    var mNumRingsField = cmType.GetField(
                        "m_numRings", BindingFlags.Instance | BindingFlags.NonPublic);
                    int numRings = (int)(mNumRingsField?.GetValue(cm) ?? 1);
                    mGridField?.SetValue(cm, new ClusterGrid(numRings));
                    Console.WriteLine($"[WorldBuilder] ClusterManager.m_grid initialized (numRings={numRings})");
                } else {
                    Console.WriteLine("[WorldBuilder] ClusterManager.m_grid already set (from save)");
                }
            }
        }

        // Snap any swimming creature (Pacu) that spawned in a non-liquid cell to the nearest
        // liquid cell. World gen can place fish at biome-boundary cells that happen to be gas/vacuum
        // → CanSwimAtCurrentLocation()=false → ShouldFall()=true → FallStates.PlayAnim NPE in
        // headless → StateMachine.Instance.error=true → SM transitions blocked → fish stuck forever.
        ValidateSwimmerPositions();

        // Safety net: clear global SM error flag after all SMs are started.
        // A single SM crash during Setup (e.g. dupe 0's StressMonitor) sets
        // StateMachine.Instance.error=True, which halts ALL GoTo calls on ALL dupes.
        // After Setup we know which SMs are running; a stale error flag must not poison tick 1+.
        StateMachine.Instance.error = false;

        // Restart AsyncPathProber worker threads AFTER the world is fully loaded and
        // UpdateNavGrids() has been run at least once.
        //
        // ROOT CAUSE of original TickFrame() crash:
        //   AsyncPathProbeWorker.main() allocates PotentialScratchPad sized from
        //   Pathfinding.MaxLinksPerCell() AT THREAD START TIME. If any NavGrid's
        //   maxLinksPerCell is larger at this point than it appeared when the thread
        //   started (e.g. because not all grids were fully registered/initialized yet),
        //   AddPotentials will index linksWithCorrectNavType beyond its allocated size
        //   → ArgumentOutOfRangeException → agentException → rethrown in TickFrame()
        //   → server crash.
        // FIX: run UpdateNavGrids() once to fully populate all NavGrid links, then
        //   shut down and restart the worker threads. The new threads read
        //   MaxLinksPerCell() AFTER all grids are initialized → correct scratch pad size.
        Pathfinding.Instance?.UpdateNavGrids();
        if (AsyncPathProber.Instance != null) {
            AsyncPathProber.Instance.Shutdown();
            AsyncPathProber.Instance.Start(1);
            Console.WriteLine($"[WorldBuilder] AsyncPathProber workers restarted (MaxLinksPerCell={Pathfinding.Instance?.MaxLinksPerCell()})");
        }


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
        // SaveGame stub: runs after Global.Awake() (StateMachineManager + StateMachineUpdater)
        // and KObjectManager.Awake(). Both are needed by ColonyRationMonitor.Instance ctor
        // via StartSM() -> GenericInstance -> Singleton<StateMachineManager>.Instance.
        // Must remain before SpawnStarterMinions (called from Create() after InitializeWorld).
        // Must also run before SpawnStarterMinions (in Create()) so RationMonitor can register
        // EventTransitions on SaveGame.Instance at SM startup.
        if (SaveGame.Instance == null) {
            Console.WriteLine("[WorldBuilder] SaveGame.Instance null — creating headless stub");
            var saveGameGo = new GameObject("SaveGame_headless");
            UnityEngine.Object.DontDestroyOnLoad(saveGameGo);
            // StateMachineController must be on the same GO before OnPrefabInit() runs so that
            // SaveGame.OnPrefabInit → new ColonyRationMonitor.Instance(this).StartSM() can call
            // master.GetComponent<StateMachineController>().AddStateMachineInstance() without NPE.
            saveGameGo.AddComponent<StateMachineController>();
            var saveGame = saveGameGo.AddComponent<SaveGame>();
            // Set obj directly — same pattern as game.obj below.
            // InitializeComponent() gates lastObj update on Application.isPlaying && lastGameObject != go,
            // which is fragile and left saveGame.obj null => ColonyRationMonitor.Subscribe() NPE.
            var saveGameSmc = saveGameGo.GetComponent<StateMachineController>();
            saveGameSmc.obj = KObjectManager.Instance.GetOrCreateObject(saveGameGo);
            saveGame.obj = KObjectManager.Instance.GetOrCreateObject(saveGameGo);
            saveGame.OnPrefabInit(); // sets SaveGame.Instance + creates ColonyRationMonitor
            if (SaveGame.Instance == null) SaveGame.Instance = saveGame; // fallback if OnPrefabInit crashed before Instance = this

            // GameplayEventManager lives on the SaveGame GO in the real game.
            // StandardWorker.CompleteWork() calls GameplayEventManager.Instance.Trigger(UseBuilding, ...)
            // which NPEs if Instance is null. OnPrefabInit: sets Instance=this, caches Notifier (null OK).
            // OnSpawn: RestoreEvents() iterates activeEvents (empty) → no-op. Completely safe headless.
            if (GameplayEventManager.Instance == null) {
                var gem = saveGameGo.AddOrGet<GameplayEventManager>();
                gem.InitializeComponent();
                gem.Spawn();
                Console.WriteLine("[WorldBuilder] GameplayEventManager.Instance initialized");
            }
        }
        SaveGame.Instance.AutoSaveCycleInterval = 0;
        Console.WriteLine("[WorldBuilder] AutoSaveCycleInterval set to 0 (headless: no autosave)");

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
            // WORKAROUND: statsOnly=true skips Amount/cureSpeedBase creation (Disease ctor lines 123-139).
            // Replicate data-only portion of Disease ctor lines 128-138 — omit visual overlayColourName
            // (line 125: Assets.instance.DiseaseVisualization crash in headless). Without this,
            // MinionModifiers.OnPrefabInit iterates Diseases and calls AddAmount(disease.amount) where
            // disease.amount == null (statsOnly=true) → NPE for every dupe spawn.
            if (db.Diseases != null) {
                foreach (var disease in db.Diseases.resources) {
                    if (disease.amount != null) continue; // already initialized (non-statsOnly path)
                    var dId = disease.Id;
                    var attrMin = new Klei.AI.Attribute(dId + "Min", "Minimum" + dId, "", "", 0f, Klei.AI.Attribute.Display.Normal, is_trainable: false);
                    var attrMax = new Klei.AI.Attribute(dId + "Max", "Maximum" + dId, "", "", 10000000f, Klei.AI.Attribute.Display.Normal, is_trainable: false);
                    disease.amountDeltaAttribute = new Klei.AI.Attribute(dId + "Delta", dId, "", "", 0f, Klei.AI.Attribute.Display.Normal, is_trainable: false);
                    disease.amount = new Amount(dId, dId + " " + STRINGS.DUPLICANTS.DISEASES.GERMS, dId + " " + STRINGS.DUPLICANTS.DISEASES.GERMS, attrMin, attrMax, disease.amountDeltaAttribute, show_max: false, Units.Flat, 0.01f, show_in_ui: true);
                    Db.Get().Attributes.Add(attrMin);
                    Db.Get().Attributes.Add(attrMax);
                    Db.Get().Attributes.Add(disease.amountDeltaAttribute);
                    disease.cureSpeedBase = new Klei.AI.Attribute(dId + "CureSpeed", is_trainable: false, Klei.AI.Attribute.Display.Normal, is_profession: false);
                    disease.cureSpeedBase.BaseValue = 1f;
                    disease.cureSpeedBase.SetFormatter(new ToPercentAttributeFormatter(1f));
                    Db.Get().Attributes.Add(disease.cureSpeedBase);
                }
                Console.WriteLine($"[WorldBuilder] Disease amounts post-initialized for {db.Diseases.Count} diseases");
            }
            InitDbField(ref db.Sicknesses, () => new Database.Sicknesses(root), "Sicknesses");
            InitDbField(ref db.AssignableSlots, () => new AssignableSlots(), "AssignableSlots");
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

            // AccessorySlots constructor crashes in its AddAccessories() loop (null KAnimFiles in headless).
            // However ALL slot fields (Eyes, Hair, HeadEffects, etc.) are assigned in the constructor
            // BEFORE the crash. We create an uninitialized instance via FormatterServices, then invoke
            // the constructor on it. After the constructor throws, the instance retains all slot fields.
            // Without this: FaceGraph.UpdateFace() → Db.Get().AccessorySlots.HeadEffects → NPE →
            //   StressMonitor sm crashes → globalSMError=True → ALL dupe SMs frozen.
            if (db.AccessorySlots == null) {
                var slotsObj = (AccessorySlots)System.Runtime.Serialization.FormatterServices
                    .GetUninitializedObject(typeof(AccessorySlots));
                var slotsCtor = typeof(AccessorySlots).GetConstructor(
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public,
                    null, new[] { typeof(ResourceSet) }, null);
                try { slotsCtor?.Invoke(slotsObj, new object[] { root }); }
                catch { /* AddAccessories(null) throws — slot fields (incl HeadEffects) already set */ }
                db.AccessorySlots = slotsObj;
                Console.WriteLine($"[Db] AccessorySlots partial init: HeadEffects={db.AccessorySlots.HeadEffects?.Id ?? "NULL"}");
            }

            Console.WriteLine("[WorldBuilder] Db.Initialize: AccessorySlots skipped, remaining fields initialized");
        }
        Console.WriteLine($"[WorldBuilder] Db: Diseases={Db._Instance.Diseases != null}, MiscStatusItems={Db._Instance.MiscStatusItems != null}, ChoreTypes={Db._Instance.ChoreTypes != null}, AssignableSlots={Db._Instance.AssignableSlots != null}");

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
        // Initialize ALL GameComps static fields (InfraredVisualizers, StructureTemperatures,
        // DiseaseContainers, Gravities, etc.) exactly as KComponentsInitializer would do.
        // Without this, PrimaryElement.OnPrefabInit() NPEs:
        //   InfraredVisualizerData(go): GameComps.StructureTemperatures.GetHandle(go) → null
        //   PrimaryElement.OnPrefabInit: GameComps.DiseaseContainers.Add(...) → null
        // Use DeclaredOnly+Static to avoid inherited non-static KComponents fields that
        // AssemblyExposer promotes to Public — SetValue(null, ...) on instance fields → TargetException.
        foreach (var f in typeof(GameComps).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            f.SetValue(null, Activator.CreateInstance(f.FieldType));
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

        // mingleCellTracker: assigned in Game.OnSpawn() line 977 via AddComponent<MingleCellTracker>().
        // Game.OnSpawn() is NEVER called in headless (WorldBuilder only calls OnPrefabInit, which crashes).
        // MingleCellSensor.Update[IL_0x0028]: ldfld mingleCellTracker on non-null Game.Instance → NPE.
        // BalloonStandCellSensor.Update has the same access pattern — same fix covers both.
        // MingleCellTracker has no custom OnPrefabInit; mingleCells List<int> is field-initialized.
        // Sim1000ms will populate mingleCells once rooms exist. Adding the component is sufficient.
        if (game.mingleCellTracker == null)
            game.mingleCellTracker = go.AddComponent<MingleCellTracker>();
        Console.WriteLine($"[WorldBuilder] mingleCellTracker ready: {game.mingleCellTracker != null}");

        // roomProber: assigned in Game.OnPrefabInit() line 833 via new RoomProber().
        // Game.OnPrefabInit() crashes at ~line 820 (ConduitFlowVisualizer) in headless,
        // so roomProber is never initialized.
        // RoomMonitor.UpdateRoomType[IL_0x0015]: Game.Instance.roomProber.GetRoomOfGameObject(...)
        // → NPE when roomProber==null. Fires on PathAdvanced event during Navigator.AdvancePath
        // (i.e., the MOMENT a dupe first tries to walk). Without this fix, movement never starts.
        // RoomProber() ctor is safe in headless: uses Grid.CellCount (valid post-load),
        // Game.Instance, World.Instance, and GameScenePartitioner.Instance (initialized above).
        if (game.roomProber == null)
            game.roomProber = new RoomProber();
        Console.WriteLine($"[WorldBuilder] roomProber ready: {game.roomProber != null}");

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

        // FactionManager: needed by FactionAlignment.OnSpawn[IL_0x64] on every dupe and critter.
        // FactionAlignment.OnSpawn line 66: FactionManager.Instance.GetFaction(Alignment).Members.Add(this)
        // fires when alignmentActive=true (default).  FactionManager.Instance null → NPE at 0x64.
        // OnPrefabInit just sets Instance=this; OnSpawn() is empty (base only).
        // Must be BEFORE SpawnEntities() so all entity OnSpawn calls find Instance non-null.
        if (FactionManager.Instance == null)
            Awake("FactionManager", () => go.AddComponent<FactionManager>().Awake());

        // TemperatureVulnerableUpdater: needed by TemperatureVulnerable.OnSpawn line 201:
        //   SlicedUpdaterSim1000ms<TemperatureVulnerable>.instance.RegisterUpdate1000ms(this)
        // instance is set in SlicedUpdaterSim1000ms.OnPrefabInit → instance = this.
        // TemperatureVulnerable is on ALL plants (EntityTemplates.CreatePlant). Without this: NPE on every plant spawn.
        // Dedicated GO avoids KObject conflicts with the shared bootstrap GO.
        if (SlicedUpdaterSim1000ms<TemperatureVulnerable>.instance == null) {
            var tvUpdGo = new GameObject("TemperatureVulnerableUpdater");
            tvUpdGo.AddComponent<TemperatureVulnerableUpdater>().Awake();
            Console.WriteLine("[WorldBuilder] TemperatureVulnerableUpdater initialized");
        }

        // PressureVulnerableUpdater: needed by PressureVulnerable.OnSpawn line 231:
        //   SlicedUpdaterSim1000ms<PressureVulnerable>.instance.RegisterUpdate1000ms(this)
        // PressureVulnerable is on ALL plants. Without this: NPE on every plant spawn.
        // Dedicated GO avoids KObject conflicts with the shared bootstrap GO.
        if (SlicedUpdaterSim1000ms<PressureVulnerable>.instance == null) {
            var pvUpdGo = new GameObject("PressureVulnerableUpdater");
            pvUpdGo.AddComponent<PressureVulnerableUpdater>().Awake();
            Console.WriteLine("[WorldBuilder] PressureVulnerableUpdater initialized");
        }

        // DrowningMonitorUpdater: needed by DrowningMonitor.OnSpawn[IL_0x6] on critters/plants.
        // DrowningMonitor.OnSpawn: SlicedUpdaterSim1000ms<DrowningMonitor>.instance.RegisterUpdate1000ms(this)
        // The static .instance field is set in SlicedUpdaterSim1000ms.OnPrefabInit → instance = this.
        // Without this, any GO with DrowningMonitor (drownVulnerable critters, plants) NPEs on spawn.
        // Must be BEFORE SpawnEntities().
        if (SlicedUpdaterSim1000ms<DrowningMonitor>.instance == null)
            Awake("DrowningMonitorUpdater", () => go.AddComponent<DrowningMonitorUpdater>().Awake());

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

        // StateMachineManager.Clear() calls scheduler.FreeResources() on the Scheduler that
        // GameScheduler.OnPrefabInit() registered at line 345. FreeResources() nulls out
        // the Scheduler's internal entries list AND clock field:
        //   entries = null → Scheduler.Update(): Count => entries.Count → NPE (15k/90s)
        //   clock   = null → scheduler.Schedule() → time + clock.GetTime() → NPE
        //                   (ClothingWearer.OnSpawn: GameScheduler.Instance.Schedule("ApplySpawnClothes"...))
        //
        // Fix: after Clear(), give GameScheduler a fresh Scheduler and re-register it with
        // StateMachineManager so both share one clean instance going forward.
        if (GameScheduler.Instance != null) {
            var freshScheduler = new Scheduler(new GameScheduler.GameSchedulerClock());
            // BindingFlags.Public is required: AssemblyExposer rewrites private→public at
            // compile time, so the field is Public in the loaded exposed DLL at runtime.
            // NonPublic alone → GetField returns null → SetValue NPEs ([0x00908] regression).
            typeof(GameScheduler)
                .GetField("scheduler", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(GameScheduler.Instance, freshScheduler);
            Singleton<StateMachineManager>.Instance.RegisterScheduler(freshScheduler);
            Console.WriteLine("[WorldBuilder] GameScheduler.scheduler refreshed after StateMachineManager.Clear()");
        } else {
            // Fallback: Awake() at line 345 crashed entirely — re-create from scratch.
            Console.WriteLine("[WorldBuilder] GameScheduler.Instance was null — re-initializing from scratch");
            var gsGo = new GameObject("GameScheduler");
            var gs = gsGo.AddComponent<GameScheduler>();
            try { gs.InitializeComponent(); }
            catch (Exception ex) {
                Console.WriteLine($"[WorldBuilder] GameScheduler.InitializeComponent partial: {ex.GetBaseException().Message}");
            }
            Console.WriteLine($"[WorldBuilder] GameScheduler.Instance after re-init: {GameScheduler.Instance != null}");
        }

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

        // AlertStateManager is required so BreathMonitor.IsLowBreath() can call
        //   wc.AlertManager.IsRedAlert()
        // without NPE. Without it: WorldContainer.AlertManager getter returns null (Debug.Assert
        // is non-throwing), and null.IsRedAlert() throws NullReferenceException ~600×/frame.
        //
        // Normal game path (AsteroidConfig.CreatePrefab → AddOrGetDef, KPrefabID.OnSpawn →
        // StateMachineController.CreateSMIS/StartSMIS) never fires in headless because the
        // WorldContainer is created manually, not via the prefab instantiation pipeline.
        //
        // We mirror the game path explicitly:
        //   1. AddOrGetDef<AlertStateManager.Def>() → registers def + creates SMC (defHandle valid)
        //   2. smc.CreateSMIS() → creates AlertStateManager.Instance, adds to smc.stateMachines
        //   3. smc.StartSMIS() → calls alertSmi.StartSM(), enters default 'off' state (no NPE)
        // This ensures smc.GetSMI<AlertStateManager.Instance>() returns non-null so
        // WorldContainer.AlertManager populates m_alertManager correctly.
        worldContainerGo.AddOrGetDef<AlertStateManager.Def>();
        wc.SetID(0);  // set id=0 BEFORE InitializeComponent so OnPrefabInit registers with correct id
        // WorldInventory MUST be added BEFORE wc.InitializeComponent() so that
        // WorldContainer.OnPrefabInit() finds it via GetComponent<WorldInventory>():
        //   worldInventory = GetComponent<WorldInventory>()  ← must be non-null
        //
        // Without it, worldInventory == null and RotPile.TryCreateNotification() NPEs at:
        //   myWorld.worldInventory.IsReachable(pickupable)
        // which propagates through decomposing.Enter() and sets globalSMError=true
        // for every RotPile entity, halting all state machines.
        //
        // We do NOT call wi.InitializeComponent() here: WorldInventory.OnPrefabInit()
        // subscribes to Game events for inventory tracking — not needed in headless.
        // IsReachable() only calls MinionGroupProber.Get().IsReachable() which is
        // safe (returns false with no minions).
        worldContainerGo.AddComponent<WorldInventory>();
        // Ensure KMonoBehaviour.obj is initialized BEFORE CreateSMIS.
        // In headless, AddComponent<WorldContainer>() does NOT trigger Awake() → obj stays null on
        // the WorldContainer's KMonoBehaviour. When CreateSMIS() runs, GenericInstance.ctor calls
        //   masterTarget.Set(worldContainerGo, smi)
        //   → worldContainerGo.GetComponent<KMonoBehaviour>().Subscribe(1969584890, ...)
        //   → obj.GetOrCreateEventSystem() → NullReferenceException at [0x00000]  (obj is null)
        // InitializeComponent() sets obj = KObjectManager.GetOrCreateObject(go) AND calls
        // WorldContainer.OnPrefabInit() which:
        //   1. RegisterWorldContainer(this) — ClusterManager.Instance is already set above ✓
        //   2. Game.Instance.Subscribe(880851192, ...) — Game.Instance is set at line 433 ✓
        //   3. ClusterManager.Instance.Subscribe(-1078710002, ...) — ClusterManager.Instance ✓
        //   4. worldInventory = GetComponent<WorldInventory>() — non-null (added above) ✓
        // The guard below is now a safety no-op (OnPrefabInit already registered wc with id=0).
        wc.InitializeComponent();
        if (!ClusterManager.Instance.WorldContainers.Contains(wc))
            ClusterManager.Instance.RegisterWorldContainer(wc);
        var smc = worldContainerGo.GetComponent<StateMachineController>();
        smc.CreateSMIS();
        smc.StartSMIS();
        // Fail-fast: if AlertManager is still null, surface the bug here rather than letting
        // BreathMonitor NPE 600×/frame (which the old try/catch was masking).
        var alertManagerCheck = wc.AlertManager;
        if (alertManagerCheck == null)
            throw new InvalidOperationException(
                "[WorldBuilder] wc.AlertManager is null after CreateSMIS/StartSMIS. " +
                "BreathMonitor.IsLowBreath would NPE every tick. " +
                "Check that StateMachineManager.scheduler was refreshed before this call.");
        Console.WriteLine($"[WorldBuilder] AlertStateManager ready: IsRedAlert={alertManagerCheck.IsRedAlert()}");
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

        // AssignmentManager — needed by MinionAssignablesProxy.OnSpawn() which calls
        // Game.Instance.assignmentManager.AddToAssignmentGroup("public", this).
        //
        // Bootstrap paradox: AssignmentManager field initializer does:
        //   assignment_groups = new Dictionary { { "public", new AssignmentGroup(...) } }
        // AssignmentGroup.ctor calls Game.Instance.assignmentManager.assignment_groups.Add(id, this).
        // At that point Game.Instance.assignmentManager == null → NPE → assignment never completes
        // → assignment_groups stays null.
        //
        // Fix order:
        //   1. AddComponent — groups=null after NPE in field initializer (expected)
        //   2. Assign Game.Instance.assignmentManager = am  ← FIRST
        //   3. Manually init assignment_groups dict
        //   4. new AssignmentGroup(...) — ctor now finds assignmentManager non-null, adds itself
        //
        // Must be initialized BEFORE SpawnEntities() / EnsureAssignableProxy().
        if (global::Game.Instance.assignmentManager == null) {
            // Bootstrap paradox: AssignmentManager field initializer calls new AssignmentGroup()
            // whose ctor calls Game.Instance.assignmentManager.assignment_groups.Add(id, this)
            // while assignmentManager is still null → NPE during AddComponent (before we assign).
            //
            // Fix: FormatterServices.GetUninitializedObject bypasses constructor + field initializers
            // entirely. We then manually initialize required private fields via reflection, set
            // game.assignmentManager = am FIRST, then init assignment_groups, then create the
            // "public" group — at which point the ctor's Add() call succeeds.
            // Confirmed field names from runtime AM_FIELDS output (ca9c645).
            // FormatterServices bypasses ctor + all field initializers — every field is default/null.
            // We must manually initialize all fields that are used before OnSpawn:
            //   assignables              private List<Assignable>              (GetEnumerator, Add, Remove)
            //   PreferredAssignableResults private List<Assignable>             (GetPreferredAssignables lock target)
            //   assignment_groups        public  Dictionary<string, AssignGroup> (set directly below)
            //   isInitialized            bool (KMonoBehaviour base) → must be true or lifecycle guards fire
            var tf = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            var am = (AssignmentManager)FormatterServices.GetUninitializedObject(typeof(AssignmentManager));
            typeof(AssignmentManager).GetField("assignables", tf)
                ?.SetValue(am, new List<Assignable>());
            typeof(AssignmentManager).GetField("PreferredAssignableResults", tf)
                ?.SetValue(am, new List<Assignable>());
            // isInitialized may be declared on AssignmentManager directly or on a base type —
            // walk the hierarchy to be safe.
            for (var t = typeof(AssignmentManager); t != null; t = t.BaseType) {
                var fi = t.GetField("isInitialized", tf);
                if (fi != null) { fi.SetValue(am, true); break; }
            }
            // Set game.assignmentManager BEFORE AssignmentGroup.ctor runs — ctor calls
            // Game.Instance.assignmentManager.assignment_groups.Add(id, this).
            global::Game.Instance.assignmentManager = am;
            am.assignment_groups = new Dictionary<string, AssignmentGroup>();
            // UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.PUBLIC == "Public" — use literal (no UI ns here).
            new AssignmentGroup("public", new IAssignableIdentity[0], "Public");
            Console.WriteLine($"[WorldBuilder] AssignmentManager ready: groups={am.assignment_groups?.Count}");
        }
        // AM_CHECK: verify the field is actually set on the Game instance after the block above.
        // If NULL here, the assignmentManager property setter is mapping to a different backing field
        // than the one we set via reflection (e.g. AssemblyExposer renamed it).
        var amHash = global::Game.Instance.assignmentManager?.GetHashCode().ToString() ?? "NULL";
        Console.WriteLine($"[AM_CHECK] game.assignmentManager after init block: {amHash}");

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

        // NameDisplayScreen was here — moved to before SpawnEntities (see block above, near DietManager).
        // Must be initialized before SpawnEntities so creature SM ctors and OxygenBreather.OnSpawn
        // find a non-null Instance. Keeping this comment as a breadcrumb for future debugging.

        // SaveLoader: needed by Traits.OnSpawn() line 41:
        //   SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 15)
        // Without Instance, this NPEs and logs an error for EVERY dupe spawned.
        // Only InitializeComponent() (→ OnPrefabInit → Instance = this) is called.
        // OnSpawn() is intentionally NOT called: it triggers full save-file loading (fatal in headless).
        // GameInfo is set to a modern version (7.37) so IsVersionOlderThan(7, 15) returns false →
        // Traits.OnSpawn() returns early and skips the legacy joy-trait migration (correct for
        // freshly-generated headless worlds).
        if (SaveLoader.Instance == null) {
            try {
                var slGo = new GameObject("SaveLoader");
                // SaveLoader.OnPrefabInit() at line 221: saveManager = GetComponent<SaveManager>()
                // SaveManager must exist on the SAME GO before InitializeComponent() runs, or saveManager
                // stays null. Every spawned entity has SaveLoadRoot (EntityTemplates line 40) and
                // SaveLoadRoot.OnSpawn() calls SaveLoader.Instance.saveManager.Register(this) →
                // NullReferenceException (saveManager null) on EVERY entity spawn.
                slGo.AddComponent<SaveManager>().Awake();
                var sl = slGo.AddComponent<SaveLoader>(); // OnPrefabInit → Instance = this (called below)
                sl.InitializeComponent();
                // Default SaveGame.GameInfo is zeroed (saveMajorVersion=0) → IsVersionOlderThan(7,15)=true
                // → migration path runs and tries to call game APIs unavailable in headless.
                // Set to a current version (7.37) so the migration check returns false and exits early.
                var gameInfoField = typeof(SaveLoader).GetField(
                    "<GameInfo>k__BackingField",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (gameInfoField != null && SaveLoader.Instance != null) {
                    var info = default(SaveGame.GameInfo);
                    info.saveMajorVersion = 7;
                    info.saveMinorVersion = 37;
                    gameInfoField.SetValue(SaveLoader.Instance, info);
                }
                Console.WriteLine($"[WorldBuilder] SaveLoader.Instance ready: {SaveLoader.Instance != null}, " +
                    $"GameInfo={SaveLoader.Instance?.GameInfo.saveMajorVersion}.{SaveLoader.Instance?.GameInfo.saveMinorVersion}");
            } catch (Exception ex) {
                Console.WriteLine($"[WorldBuilder] SaveLoader init partial: {ex.GetBaseException().Message}");
            }
        }

        // SaveManager: SaveLoadRoot.OnSpawn line 54 calls SaveLoader.Instance.saveManager.Register(this).
        // SaveLoader.OnPrefabInit sets saveManager = GetComponent<SaveManager>() on its own GO.
        // If SaveManager.Awake() threw during the SaveLoader init block above, the component may be
        // destroyed → saveManager stays null → NPE on every entity spawn (all have SaveLoadRoot).
        if (SaveLoader.Instance != null && SaveLoader.Instance.saveManager == null) {
            var sm = SaveLoader.Instance.gameObject.AddComponent<SaveManager>();
            sm.InitializeComponent();
            SaveLoader.Instance.saveManager = sm;
            Console.WriteLine("[WorldBuilder] SaveManager initialized");
        }

        // DiscoveredResources: needed by EntityTemplates.CreateAndRegisterBaggedCreature()
        // prefabSpawnFn lambda fired from KPrefabID.OnSpawn():
        //   DiscoveredResources.Instance.Discover(creature_prefab_id.PrefabTag, ...)
        // All baggable critters (Puft, Hatch, Pacu, Drecko, etc.) have this lambda.
        // Without Instance → NPE on every critter spawn during SpawnEntities().
        // Only InitializeComponent() (OnPrefabInit → Instance = this) needed — OnSpawn()
        // calls FilterDisabledContent() which accesses Assets/ElementLoader (safe in headless).
        if (DiscoveredResources.Instance == null) {
            var drGo = new GameObject("DiscoveredResources");
            drGo.AddComponent<DiscoveredResources>().InitializeComponent();
            Console.WriteLine($"[WorldBuilder] DiscoveredResources.Instance ready: {DiscoveredResources.Instance != null}");
        }

        // ScheduleManager.Spawn() creates the default 24-block schedule (Sleep/Work/Leisure)
        // and registers the OnAddDupe handler for future minion spawns.
        // Must run after Game.Instance is set (AddDefaultSchedule → FastWorkersModeActive).
        // Dupes are not yet spawned at this point (SpawnStarterMinions runs in Create()).
        // When each dupe spawns and MinionIdentity is added to Components.LiveMinionIdentities,
        // the OnAddDupe handler assigns them to schedules[0] automatically.
        //
        // Without this: schedules list stays empty → ScheduleManager.IsAllowed() returns false
        // for ALL block types → ShouldExitSleep() never returns true from schedule check →
        // dupes stuck sleeping until stamina reaches max (which also never happens because
        // AmountInstance.BatchUpdate is not registered — separate issue for Sol).
        ScheduleManager.Instance.Spawn();
        var schedList = ScheduleManager.Instance.GetSchedules();
        Console.WriteLine($"[Schedule] Spawn done: count={schedList.Count} blocks={schedList.FirstOrDefault()?.GetBlocks()?.Count}");
        if (schedList.Count == 0)
            throw new InvalidOperationException("[WorldBuilder] ScheduleManager.Spawn() produced no schedules — check Db.ScheduleGroups.");
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
        // Actual outcome: 463/463 (all non-DLC-excluded) configs register successfully.
        // KBatchedAnimController does NOT NPE because Assets.GetAnim() is pre-patched
        // by PatchInternalCalls (4th arg) to return a stub KAnimFile in headless mode.
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
                    Console.WriteLine($"[BuildingDef] FAIL [{type.Name}]: {ex}");
                }
            }
        }
        Console.WriteLine($"[BuildingDef] Registered {success}/{types.Count} building defs ({failed} failed, {skipped} DLC-skipped)");

        // Harvest BuildingDef sizes from configTable even for configs whose full registration
        // failed at BuildingLoader.Add2DComponents (rendering NPE).
        // configTable[config] = buildingDef is set at RegisterBuilding line 70, BEFORE
        // CreateBuildingComplete/Add2DComponents crash at line 85. So configTable holds defs
        // for ALL successfully CreateBuildingDef()-able configs regardless of Add2DComponents.
        // This populates _prefabSizeMap and _buildingDefCache so HQ/Telepad/GeneShuffler etc.
        // have correct sizes in /api/entities even when PlaceBuilding returns null for them.
        var sizePopulated = 0;
        try {
            foreach (var kvp in BuildingConfigManager.Instance.configTable) {
                var def = kvp.Value;
                if (def == null) continue;
                var size = ReadBuildingDefSize(def);
                if (!size.HasValue) continue;
                if (!_prefabSizeMap.ContainsKey(def.PrefabID)) {
                    _prefabSizeMap[def.PrefabID] = size.Value;
                    sizePopulated++;
                }
                if (!_buildingDefCache.ContainsKey(def.PrefabID))
                    _buildingDefCache[def.PrefabID] = def;
            }
        } catch (Exception ex) {
            Console.WriteLine($"[BuildingDef] configTable harvest failed: {ex.GetBaseException().Message}");
        }
        Console.WriteLine($"[BuildingDef] SizeMap populated from configTable: {sizePopulated} entries. " +
            $"HQ size: {(_prefabSizeMap.TryGetValue("Headquarters", out var hqSz) ? $"{hqSz.w}x{hqSz.h}" : "missing")}");
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
        // Also register IMultiEntityConfig types (e.g. GeyserGenericConfig) — these are
        // separate from IEntityConfig and are skipped by the loop above.  Without this,
        // Assets.GetPrefab("GeyserGeneric_*") returns null and CaptureEntitySize falls back
        // to 1×1 for every geyser.  Mirrors EntityConfigManager.Awake() lines 84-88.
        var multiEntityConfigType = typeof(IMultiEntityConfig);
        var multiTypes = typeof(EntityConfigManager).Assembly.GetTypes()
            .Where(t => multiEntityConfigType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();
        foreach (var type in multiTypes) {
            try {
                var config = (IMultiEntityConfig)Activator.CreateInstance(type);
                EntityConfigManager.Instance.RegisterEntities(config);
                registered++;
            } catch (Exception ex) {
                failed++;
                if (failed <= 3) {
                    var inner = ex.InnerException;
                    Console.WriteLine($"[WorldBuilder] MultiEntity FAIL [{type.Name}]: {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine($"  Stack: {(inner ?? ex).StackTrace?.Split('\n')[0]}");
                }
            }
        }

        Console.WriteLine($"[WorldBuilder] Registered {registered}/{types.Count + multiTypes.Count} entities ({failed} failed), prefabs: {Assets.PrefabsByTag?.Count ?? 0}");
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
    ///
    /// Mirrors NewBaseScreen.SpawnMinions() — the real game spawns dupes at the same Y as the
    /// PrintingPod, offset +1 to the right: x2 = hqX + i + 1; y2 = hqY. The printer sits on a
    /// solid-rock foundation (y=hqY-1), so cells at (hqX+1, hqY), (hqX+2, hqY) have a real solid
    /// floor and are valid spawn positions. The prior vertical scan found the cave 10 cells above
    /// (where Grid.Solid[floorCell] was finally True), which was wrong.
    ///
    /// Priority 1: scan horizontally at hqY for a gas cell with solid floor (mirrors real game).
    /// Priority 2: scan vertically above hq for any gas cell with solid floor (legacy fallback).
    /// Priority 3: scan the centre half of the map (full fallback).
    /// Requires Sim.Start() to have run (Grid.Solid must be populated).
    /// </summary>
    private int FindColonySpawnCell() {
        var w = Grid.WidthInCells;

        if (_hqCell >= 0) {
            var hqX = _hqCell % w;
            var hqY = _hqCell / w;

            // Priority 1: scan HORIZONTALLY at printer level — same logic as real game.
            // NewBaseScreen.SpawnMinions(): x2 = x + i + 1; y2 = y  (same Y, right of printer).
            // Cells at (hqX+1..hqX+6, hqY) have solid rock floor at y=hqY-1 (the cave foundation)
            // and gas atmosphere in the printer cavity. The printer cell itself (hqX, hqY) is a
            // building — not solid — so Grid.Solid is false there, but adjacent cells are valid.
            for (var dx = 1; dx <= 6; dx++) {
                var candidate = _hqCell + dx;
                if (!Grid.IsValidCell(candidate)) break;
                var floorCell = candidate - w;
                if (!Grid.IsValidCell(floorCell)) continue;
                if (Grid.Solid[candidate]) continue; // skip solid tiles (walls adjacent to printer)
                if (!Grid.Solid[floorCell]) continue; // must have solid floor
                var elem = Grid.Element[candidate];
                if (elem == null || elem.IsLiquid || elem.id == SimHashes.Vacuum) continue;
                Console.WriteLine($"[SpawnFinder] Printer-adjacent spawn at ({candidate % w},{candidate / w}) dx=+{dx} elem={elem.tag} mass={Grid.Mass[candidate]:F2}");
                return candidate;
            }
            // Try left side too (in case right wall is immediately adjacent to printer).
            for (var dx = 1; dx <= 6; dx++) {
                var candidate = _hqCell - dx;
                if (!Grid.IsValidCell(candidate)) break;
                var floorCell = candidate - w;
                if (!Grid.IsValidCell(floorCell)) continue;
                if (Grid.Solid[candidate]) continue;
                if (!Grid.Solid[floorCell]) continue;
                var elem = Grid.Element[candidate];
                if (elem == null || elem.IsLiquid || elem.id == SimHashes.Vacuum) continue;
                Console.WriteLine($"[SpawnFinder] Printer-adjacent spawn at ({candidate % w},{candidate / w}) dx=-{dx} elem={elem.tag} mass={Grid.Mass[candidate]:F2}");
                return candidate;
            }

            // Priority 2 (legacy fallback): scan vertically above HQ for a solid-floor gas cell.
            // Kept for saves where the printer is embedded in a wall and horizontal cells are solid.
            Console.WriteLine($"[SpawnFinder] No horizontal cell at printer level — falling back to vertical scan above ({hqX},{hqY})");
            for (var dy = 1; dy <= 30; dy++) {
                var candidate = _hqCell + dy * w;
                if (!Grid.IsValidCell(candidate)) break;
                var floorCell = candidate - w;
                if (Grid.Solid[candidate]) continue;
                if (!Grid.Solid[floorCell]) continue;
                var elem = Grid.Element[candidate];
                if (elem == null || elem.IsLiquid || elem.id == SimHashes.Vacuum) continue;
                Console.WriteLine($"[SpawnFinder] Vertical fallback spawn at ({candidate % w},{candidate / w}) dy={dy} elem={elem.tag} mass={Grid.Mass[candidate]:F2}");
                return candidate;
            }
            Console.WriteLine($"[SpawnFinder] HQ known at ({hqX},{hqY}) but no valid spawn cell found — falling through");
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
        Console.WriteLine($"[SpawnEntities] NDS={NameDisplayScreen.Instance != null}");
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
                // Track building for GetEntitiesBytes — independent of SpawnData validity at
                // query time (SpawnData reference may be stale if WorldGen objects are GC'd).
                _trackedBuildings.Add((b.id, b.location_x, b.location_y));
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
                    var skipDef  = Assets.GetBuildingDef(b.id);
                    var skipCell = b.location_y * Grid.WidthInCells + b.location_x;
                    Console.WriteLine($"[BuildingSkip] {b.id}: def={skipDef != null} " +
                        $"cell={skipCell} cellValid={Grid.IsValidCell(skipCell)} " +
                        $"xy=({b.location_x},{b.location_y}) sizeMapHasIt={_prefabSizeMap.ContainsKey(b.id)}");
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
    /// Reads entity size from a live spawned GO (or its registered prefab) and caches it.
    /// Priority order:
    ///   1. Building.Def on spawned GO — authoritative for buildings spawned successfully
    ///   2. Assets.GetBuildingDef — for buildings whose spawned GO lacks a Building component
    ///   3. OccupyArea on spawned GO — for entities (critters, geysers, etc.)
    ///   4. KBoxCollider2D on spawned GO — for dupes and entities without OccupyArea
    ///   5. OccupyArea on registered prefab (Assets.GetPrefab) — fallback when spawned GO has
    ///      null OccupyArea offsets in headless mode (geysers, some critters)
    /// </summary>
    private void CaptureEntitySize(string id, GameObject go) {
        try {
            // 1. Building.Def on spawned GO (most reliable for buildings)
            var building = go.GetComponent<Building>();
            var defSize = ReadBuildingDefSize(building?.Def);
            if (defSize.HasValue) {
                _prefabSizeMap[id] = defSize.Value;
                Console.WriteLine($"[EntitySize] {id} → {defSize.Value.w}×{defSize.Value.h} (Building.Def)");
                return;
            }

            // 2. Assets.GetBuildingDef — for buildings without a Building component on the GO
            if (building == null) {
                var registeredSize = ReadBuildingDefSize(Assets.GetBuildingDef(id));
                if (registeredSize.HasValue) {
                    _prefabSizeMap[id] = registeredSize.Value;
                    Console.WriteLine($"[EntitySize] {id} → {registeredSize.Value.w}×{registeredSize.Value.h} (Assets.GetBuildingDef)");
                    return;
                }
            }

            // 3+4. OccupyArea offsets → KBoxCollider2D on spawned GO
            var occupy = go.GetComponent<OccupyArea>();
            int ew = 1, eh = 1;

            if (occupy?._UnrotatedOccupiedCellsOffsets?.Length > 0) {
                (ew, eh) = ComputeSizeFromOffsets(occupy._UnrotatedOccupiedCellsOffsets);
                Console.WriteLine($"[EntitySize] {id} → {ew}×{eh} (OccupyArea on spawned GO, cells={occupy._UnrotatedOccupiedCellsOffsets.Length})");
            } else {
                var col = go.GetComponent<KBoxCollider2D>();
                if (col != null) {
                    ew = Math.Max(1, (int)Math.Round(col.size.x));
                    eh = Math.Max(1, (int)Math.Round(col.size.y));
                    Console.WriteLine($"[EntitySize] {id} → {ew}×{eh} (KBoxCollider2D on spawned GO)");
                }
            }

            // 5. Registered prefab fallback: spawned GO may have null OccupyArea offsets in
            // headless mode (common for geysers, some critters) even though the registered
            // prefab created by EntityTemplates.CreatePlacedEntity has correct offsets.
            if (ew == 1 && eh == 1) {
                var prefab = Assets.GetPrefab(new Tag(id));
                var prefabOccupy = prefab?.GetComponent<OccupyArea>();
                if (prefabOccupy?._UnrotatedOccupiedCellsOffsets?.Length > 0) {
                    (ew, eh) = ComputeSizeFromOffsets(prefabOccupy._UnrotatedOccupiedCellsOffsets);
                    Console.WriteLine($"[EntitySize] {id} → {ew}×{eh} (OccupyArea on registered prefab, cells={prefabOccupy._UnrotatedOccupiedCellsOffsets.Length})");
                }
            }

            // 6. GeyserConfigurator — explicit fallback for geyser entities.
            // Geysers use EntityTemplates.CreatePlacedEntity and have GeyserConfigurator added
            // via AddOrGet in both GeyserConfig and GeyserGenericConfig.CreateGeyser().
            // GeyserGeneric_* prefabs are registered in Assets by the IMultiEntityConfig pass in
            // RegisterEntities(); if the prefab is still missing here, log a clear warning.
            if (ew == 1 && eh == 1) {
                var gc = go.GetComponent<GeyserConfigurator>();
                if (gc != null) {
                    var prefab = Assets.TryGetPrefab(new Tag(id));
                    var gcOccupy = prefab?.GetComponent<OccupyArea>();
                    if (gcOccupy?._UnrotatedOccupiedCellsOffsets?.Length > 0) {
                        (ew, eh) = ComputeSizeFromOffsets(gcOccupy._UnrotatedOccupiedCellsOffsets);
                        Console.WriteLine($"[EntitySize] {id} → {ew}×{eh} (GeyserConfigurator fallback, prefab OccupyArea)");
                    } else {
                        Console.WriteLine($"[EntitySize] {id} WARNING: GeyserConfigurator present but prefab not in Assets — defaulting to 1×1");
                    }
                }
            }

            if (ew == 1 && eh == 1)
                Console.Error.WriteLine($"[EntitySize] {id}: no size source found — defaulting to 1×1");

            _prefabSizeMap[id] = (ew, eh);
        } catch (Exception ex) {
            Console.Error.WriteLine($"[EntitySize] {id} FAILED: {ex.GetBaseException().Message} — defaulting to 1×1");
            _prefabSizeMap[id] = (1, 1);
        }
    }

    internal static (int w, int h) ComputeSizeFromOffsets(CellOffset[] offsets) {
        int minX = 0, maxX = 0, minY = 0, maxY = 0;
        foreach (var o in offsets) {
            if (o.x < minX) minX = o.x;
            if (o.x > maxX) maxX = o.x;
            if (o.y < minY) minY = o.y;
            if (o.y > maxY) maxY = o.y;
        }
        return (maxX - minX + 1, maxY - minY + 1);
    }

    /// <summary>
    /// Reads building dimensions from a <see cref="BuildingDef"/>.
    /// Returns null if def is null or dimensions are degenerate (≤0).
    /// Extracted as a public static method so it can be unit-tested without a live GameObject.
    /// </summary>
    public static (int w, int h)? ReadBuildingDefSize(BuildingDef def) {
        if (def == null) return null;
        var w = def.WidthInCells;
        var h = def.HeightInCells;
        if (w <= 0 || h <= 0) return null;
        return (w, h);
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

        // Step 0: force ChoreProvider and ChoreDriver initialization for any entity
        // where the lifecycle didn't complete (isInitialized=false → Spawn() bails early,
        // OR isInitialized=true but isSpawned=false → OnSpawn never ran → SM not started).
        // Cover ALL brains (dupes + critters), not just minionGOs.
        foreach (var go in allBrainGOs) {
            var provider = go.GetComponent<ChoreProvider>();
            if (provider != null) {
                if (!provider.IsInitialized()) {
                    // Case 1: never initialized → Init then Spawn (mirrors ChoreDriver Case 1)
                    provider.InitializeComponent();
                    provider.Spawn();
                    Console.WriteLine($"[FixChoreConsumers] Force-initialized ChoreProvider for {go.name}");
                } else if (!provider.isSpawned) {
                    // Case 2: initialized but OnSpawn() never ran → Spawn only
                    // Root cause of 336x "ChoreProvider not initialized" errors:
                    // isInitialized=true, isSpawned=false was silently skipped before this fix.
                    provider.Spawn();
                    Console.WriteLine($"[FixChoreConsumers] Force-spawned ChoreProvider (was init but !isSpawned) for {go.name}");
                }
            }
            var driver = go.GetComponent<ChoreDriver>();
            if (driver != null) {
                var needsInit  = !driver.IsInitialized();
                var needsSpawn = driver.IsInitialized() && !driver.isSpawned;
                // DS-006 fix: add StandardWorker BEFORE Spawn() so StatesInstance.ctor
                // finds GetComponent<WorkerBase>() non-null → worker set → no NPE at
                // haschore.Update b__5_3 IL[0x0075] (smi.worker.GetWorkable()).
                // Must run before Cases 1 and 2 (both call Spawn which triggers StartSM).
                if (needsInit || needsSpawn) {
                    go.AddOrGet<StandardWorker>();
                }
                // Case 1: never initialized → Init then Spawn
                if (needsInit) {
                    driver.InitializeComponent();
                    driver.Spawn();
                    Console.WriteLine($"[FixChoreConsumers] Force-initialized+spawned ChoreDriver for {go.name}");
                }
                // Case 2: initialized but OnSpawn() never ran (SM not started) → Spawn only
                else if (needsSpawn) {
                    driver.Spawn();
                    Console.WriteLine($"[FixChoreConsumers] Force-spawned ChoreDriver (was init but !isSpawned) for {go.name}");
                }
                // Case 3: initialized + spawned but SM not running → start SMI directly
                else if (driver.isSpawned) {
                    var smiInst = driver.GetSMI() as ChoreDriver.StatesInstance;
                    if (smiInst != null && !smiInst.IsRunning()) {
                        smiInst.StartSM();
                        Console.WriteLine($"[FixChoreConsumers] Force-started ChoreDriver SM for {go.name}");
                    }
                }
            }
        }

        var fixedCount = 0;
        foreach (var go in allBrainGOs) {
            var cc = go.GetComponent<ChoreConsumer>();
            if (cc == null || cc.consumerState != null) continue;
            cc.consumerState = new ChoreConsumerState(cc);
            fixedCount++;
        }

        // Step 3: fix ChoreConsumer.providers list.
        // Normal game path: Modifiers.OnPrefabInit() calls consumer.AddProvider(GlobalChoreProvider.Instance).
        // If Modifiers.OnPrefabInit() crashed (reported exception source), GlobalChoreProvider
        // is never added → FindNextChore iterates 0 providers → succeededContexts empty →
        // ChooseChore returns false → SetChore never called → chore=null → no movement.
        // Also ensure entity's own ChoreProvider is present (added by ChoreConsumer.OnPrefabInit).
        var providersFixed = 0;
        foreach (var go in allBrainGOs) {
            var cc = go.GetComponent<ChoreConsumer>();
            if (cc == null) continue;
            var providersList = _ccProvidersField?.GetValue(cc) as List<ChoreProvider>;
            if (providersList == null) continue;

            // GlobalChoreProvider: registered by Modifiers.OnPrefabInit() — may be missing.
            if (GlobalChoreProvider.Instance != null && !providersList.Contains(GlobalChoreProvider.Instance)) {
                cc.AddProvider(GlobalChoreProvider.Instance);
                Debug.LogWarning($"[FixChoreConsumers] {go.name}: added GlobalChoreProvider (had {providersList.Count - 1} providers before)");
                providersFixed++;
            }
            // Own ChoreProvider: registered by ChoreConsumer.OnPrefabInit() — may be null if
            // MyAttributes didn't resolve [MyCmpAdd] fields before OnPrefabInit ran.
            var ownProvider = go.GetComponent<ChoreProvider>();
            if (ownProvider != null && !providersList.Contains(ownProvider)) {
                cc.AddProvider(ownProvider);
                Debug.LogWarning($"[FixChoreConsumers] {go.name}: added own ChoreProvider (was missing)");
                providersFixed++;
            }
        }
        Debug.LogWarning($"[WorldBuilder] FixChoreConsumers Step3: providers patched on {providersFixed} consumer(s)");

        // Diagnostic: log ChoreDriver/Brain/providers state for each brain.
        foreach (var go in allBrainGOs) {
            var driver = go.GetComponent<ChoreDriver>();
            var brain  = go.GetComponent<Brain>();
            var cc     = go.GetComponent<ChoreConsumer>();
            var smiInst = driver?.GetSMI() as ChoreDriver.StatesInstance;
            var providersList = _ccProvidersField?.GetValue(cc) as List<ChoreProvider>;
            var providerNames = providersList != null
                ? string.Join(",", providersList.ConvertAll(p => p?.GetType().Name ?? "null"))
                : "?";
            Debug.LogWarning($"[FixChoreConsumers] {go.name}: " +
                $"driver={driver != null} init={driver?.IsInitialized()} spawned={driver?.isSpawned} smRunning={smiInst?.IsRunning()} " +
                $"brainRunning={brain?.IsRunning()} consumerState={cc?.consumerState != null} " +
                $"providers=[{providerNames}]");
        }
        Debug.LogWarning($"[WorldBuilder] FixChoreConsumers done: {fixedCount}/{allBrainGOs.Count} consumerState(s) created, {providersFixed} providers patched (minions={minionGOs.Count} brains={Components.Brains.Count})");
    }

    private void FixRationalAi() {
        // Catch-and-continue per dupe: diagnostic pass must see all 3 dupes' failures,
        // not just the first. MinionPrefab.Setup() logs+rethrows on each step failure;
        // the catch here prevents one dupe's failure from masking the others.
        int ok = 0, failed = 0;
        foreach (var go in _spawnedMinions) {
            try {
                MinionPrefab.Setup(go);
                ok++;
            } catch (Exception ex) {
                Console.WriteLine($"[FixRationalAi] {go.name} Setup FAILED: {ex.GetType().Name}: {ex.Message}");
                failed++;
            }
        }
        Console.WriteLine($"[WorldBuilder] FixRationalAi done: {ok} OK, {failed} FAILED (of {_spawnedMinions.Count})");
    }

    /// <summary>
    /// Removes render-only components from critter PREFABS before SpawnEntities fires.
    ///
    /// CharacterOverlay.OnSpawn() and AnimEventHandler.OnSpawn() crash in headless:
    ///   CharacterOverlay.OnSpawn → Register() → "Could not find component ChoreProvider" NPE
    ///   AnimEventHandler.OnSpawn[IL_0x4c] → animCollider (KBoxCollider2D) NPE
    ///
    /// Both are added by ExtendEntityToBasicCreature() to every critter prefab.
    /// Removing them from PREFABS (not instances) ensures KInstantiate copies don't inherit them.
    /// Removal happens AFTER RegisterEntities() so all prefabs are registered, and BEFORE
    /// SpawnEntities() so TriggerLifecycle never fires OnSpawn() on these components.
    /// </summary>
    private static void FixCreaturePrefabsPreSpawn() {
        var total = 0;
        foreach (var prefab in Assets.PrefabsByTag.Values) {
            if (prefab == null || prefab.GetComponent<CreatureBrain>() == null) continue;
            var charOverlay = prefab.GetComponent<CharacterOverlay>();
            if (charOverlay != null) UnityEngine.Object.DestroyImmediate(charOverlay);
            var animHandler = prefab.GetComponent<AnimEventHandler>();
            if (animHandler != null) UnityEngine.Object.DestroyImmediate(animHandler);
            total++;
        }
        Console.WriteLine($"[WorldBuilder] FixCreaturePrefabsPreSpawn: patched {total} critter prefab(s)");
    }

    private void FixPickupables() {
        var total = 0;
        foreach (var pickupable in Components.Pickupables.Items) {
            var go = pickupable.gameObject;
            if (go == null) continue;
            if (go.GetComponent<MinionBrain>() != null) continue;   // dupes handled by FixRationalAi
            if (go.GetComponent<CreatureBrain>() != null) continue; // creatures handled by FixCreatureBrains
            if (go.GetComponent<Building>() != null) continue;      // buildings not loose pickupables
            total++;
            PickupablePrefab.Setup(go);
        }
        Console.WriteLine($"[WorldBuilder] FixPickupables: total={total}");
    }

    private void FixCreatureBrains() {
        var total = 0;
        foreach (var brain in Components.Brains.Items) {
            if (brain is not CreatureBrain) continue;
            var smc = brain.gameObject.GetComponent<StateMachineController>();
            if (smc == null || smc.stateMachines == null) continue; // REQUIRED: critter SMCs may be null in headless — do not remove.
            total++;
            CreaturePrefab.Setup((CreatureBrain)brain);
        }
        Console.WriteLine($"[Animals] FixCreatureBrains: total={total}");
    }

    /// <summary>
    /// Snaps swimming creatures (Pacu) that spawned in non-liquid cells to the nearest liquid cell.
    ///
    /// Root cause: world gen can place fish at biome-boundary cells with a gas element.
    /// CanSwimAtCurrentLocation() calls Grid.IsSubstantialLiquid() which requires
    /// mass >= defaultMass * 0.35. Boundary cells have low/zero mass → returns false →
    /// ShouldFall()=true → FallStates starts → PlayAnim NPE in headless →
    /// StateMachine.Instance.error=true → all SM transitions blocked → fish permanently stuck.
    ///
    /// Fix: one-time positional correction at startup. No Harmony, no runtime overhead.
    /// Uses Grid.IsSubstantialLiquid (mass >= defaultMass*0.35) for both body and head cell —
    /// same check as CreatureFallMonitor.CanSwimAtCurrentLocation(), ensuring consistent safety.
    /// </summary>
    private static void ValidateSwimmerPositions() {
        var moved = 0;
        foreach (var brain in Components.Brains.Items) {
            if (brain == null || brain.gameObject == null) continue;
            if (brain is MinionBrain) continue;
            var nav = brain.gameObject.GetComponent<Navigator>();
            if (nav?.NavGrid == null || nav.NavGrid.id != "SwimmerNavGrid") continue;
            var cell = Grid.PosToCell(brain.gameObject);
            if (!Grid.IsValidCell(cell) || (Grid.IsSubstantialLiquid(cell) && Grid.IsSubstantialLiquid(cell + Grid.WidthInCells))) continue;
            var fixCell = FindNearestLiquidCell(cell, 20);
            if (fixCell < 0) {
                Console.WriteLine($"[WorldBuilder] Swimmer {brain.gameObject.name} at cell={cell}: no liquid cell within radius 20, leaving in place");
                continue;
            }
            var newPos = Grid.CellToPosCBC(fixCell, Grid.SceneLayer.Creatures);
            brain.gameObject.transform.SetPosition(newPos);
            Console.WriteLine($"[WorldBuilder] Moved swimmer {brain.gameObject.name} from gas cell {cell} to liquid cell {fixCell}");
            moved++;
        }
        Console.WriteLine($"[WorldBuilder] ValidateSwimmerPositions: moved {moved} swimmer(s) to liquid cells");
    }

    /// <summary>
    /// Spiral search (expanding rings) for the nearest valid liquid cell within <paramref name="radius"/>.
    /// Returns the cell index, or -1 if none found.
    /// </summary>
    private static int FindNearestLiquidCell(int originCell, int radius) {
        var origin = Grid.CellToXY(originCell);
        for (var r = 1; r <= radius; r++) {
            for (var dx = -r; dx <= r; dx++) {
                for (var dy = -r; dy <= r; dy++) {
                    if (System.Math.Abs(dx) != r && System.Math.Abs(dy) != r) continue; // shell only
                    var cx = origin.x + dx;
                    var cy = origin.y + dy;
                    var c  = Grid.XYToCell(cx, cy);
                    if (Grid.IsValidCell(c) && Grid.IsSubstantialLiquid(c) && Grid.IsSubstantialLiquid(c + Grid.WidthInCells)) return c;
                }
            }
        }
        return -1;
    }

    /// <summary>
    /// Disables rendering-only components that NPE in headless (no camera/animator).
    /// Cannot use Harmony (KMonoBehaviour subclass methods → deadlock).
    /// component.enabled = false prevents RenderEveryTick/SimEveryTick callbacks.
    /// </summary>
    /// <summary>
    /// Unregisters render-only components from SimAndRenderScheduler.
    ///
    /// Root cause: KMonoBehaviour.Spawn() calls SimAndRenderScheduler.instance.Add(this)
    /// for all components with autoRegisterSimRender=true, regardless of enabled state.
    /// SimAndRenderScheduler.RenderEveryTickUpdater.Update() calls RenderEveryTick() directly
    /// with NO enabled check — so disabled=true alone does not prevent the crash.
    ///
    /// Proper fix: call renderEveryTick.Remove(component) to unregister from the scheduler,
    /// AND set enabled=false so any future re-registration via OnEnable is suppressed.
    ///
    /// Only skip visual-only components — NOT AI/physics ones (see architectural principle).
    /// </summary>
    // Types to evict from the renderEveryTick scheduler — render/UI/audio only, no game logic.
    // Determined from Cecil inspection of IRenderEveryTick implementors in Assembly-CSharp.dll.
    // DO NOT add: BrainScheduler, KCollider2D, SolidTransferArm, LogicPorts, ColonyAchievementTracker.
    private static readonly HashSet<Type> _renderOnlyRenderEveryTickTypes = new HashSet<Type> {
        typeof(LightSymbolTracker),     // NPE: CameraController.Instance == null
        typeof(LoopingSoundManager),    // audio only — no-op in headless
        typeof(SpriteSheetAnimManager), // sprite rendering — no-op in headless
        typeof(UIShake),                // UI visual effect
    };

    // Reflection accessor for UpdaterManager<IRenderEveryTick>.updaterHandles.
    // Private Dictionary<IRenderEveryTick, SimAndRenderScheduler.Handle> — the canonical
    // registry of everything registered in the renderEveryTick bucket.
    // Use Public|NonPublic: server runs the exposed DLL where private → public.
    private static readonly FieldInfo _updaterHandlesField =
        typeof(SimAndRenderScheduler.RenderEveryTickUpdater)
            .BaseType  // UpdaterManager<IRenderEveryTick>
            ?.GetField("updaterHandles",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    /// <summary>
    /// Iterates the renderEveryTick scheduler bucket directly and removes all render/UI/audio
    /// components that have no purpose in a headless dedicated server.
    ///
    /// FindObjectsOfType returns 0 in headless (Unity object registry inactive), so we go
    /// straight to the source: UpdaterManager.updaterHandles dictionary.
    /// </summary>
    private static void DisableRenderingOnlyComponents() {
        try {
            var scheduler = SimAndRenderScheduler.instance;
            if (scheduler?.renderEveryTick == null) {
                Debug.LogWarning("[WorldBuilder] DisableRenderingOnlyComponents: scheduler or renderEveryTick is null");
                return;
            }

            // Read the private updaterHandles dictionary via reflection.
            if (_updaterHandlesField == null) {
                Debug.LogWarning("[WorldBuilder] DisableRenderingOnlyComponents: updaterHandles field not found via reflection");
                return;
            }
            var handles = _updaterHandlesField.GetValue(scheduler.renderEveryTick)
                as System.Collections.IDictionary;
            if (handles == null) {
                Debug.LogWarning("[WorldBuilder] DisableRenderingOnlyComponents: updaterHandles cast to IDictionary failed");
                return;
            }

            // Snapshot keys before removing to avoid modifying-while-iterating.
            var toRemove = new List<IRenderEveryTick>();
            foreach (IRenderEveryTick entry in handles.Keys) {
                if (entry != null && _renderOnlyRenderEveryTickTypes.Contains(entry.GetType()))
                    toRemove.Add(entry);
            }

            foreach (var entry in toRemove) {
                scheduler.renderEveryTick.Remove(entry);
                // Also set enabled=false so any future re-registration via OnEnable is suppressed.
                if (entry is UnityEngine.Behaviour b) b.enabled = false;
            }

            Debug.LogWarning($"[WorldBuilder] DisableRenderingOnlyComponents: removed {toRemove.Count} render-only entries" +
                $" from renderEveryTick (bucket had {handles.Count} total entries)");
        } catch (Exception ex) {
            Debug.LogWarning($"[WorldBuilder] DisableRenderingOnlyComponents failed:\n{ex}");
        }
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
