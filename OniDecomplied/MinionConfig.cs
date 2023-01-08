// Decompiled with JetBrains decompiler
// Type: MinionConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class MinionConfig : IEntityConfig
{
  public static string ID = "Minion";
  public static string MINION_BASE_TRAIT_ID = MinionConfig.ID + "BaseTrait";
  public static string MINION_NAV_GRID_NAME = "MinionNavGrid";
  public const int MINION_BASE_SYMBOL_LAYER = 0;
  public const int MINION_HAIR_ALWAYS_HACK_LAYER = 1;
  public const int MINION_EXPRESSION_SYMBOL_LAYER = 2;
  public const int MINION_MOUTH_FLAP_LAYER = 3;
  public const int MINION_CLOTHING_SYMBOL_LAYER = 4;
  public const int MINION_PICKUP_SYMBOL_LAYER = 5;
  public const int MINION_SUIT_SYMBOL_LAYER = 6;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME;
    GameObject entity = EntityTemplates.CreateEntity(MinionConfig.ID, name);
    entity.AddOrGet<StateMachineController>();
    MinionModifiers minionModifiers = entity.AddOrGet<MinionModifiers>();
    entity.AddOrGet<Traits>();
    entity.AddOrGet<Effects>();
    entity.AddOrGet<AttributeLevels>();
    entity.AddOrGet<AttributeConverters>();
    MinionConfig.AddMinionAmounts((Modifiers) minionModifiers);
    MinionConfig.AddMinionTraits(name, (Modifiers) minionModifiers);
    entity.AddOrGet<MinionBrain>();
    entity.AddOrGet<KPrefabID>().AddTag(GameTags.DupeBrain, false);
    entity.AddOrGet<Worker>();
    entity.AddOrGet<ChoreConsumer>();
    Storage storage = entity.AddOrGet<Storage>();
    storage.fxPrefix = Storage.FXPrefix.PickedUp;
    storage.dropOnLoad = true;
    storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Preserve,
      Storage.StoredItemModifier.Seal
    });
    entity.AddTag(GameTags.CorrosionProof);
    entity.AddOrGet<Health>();
    OxygenBreather oxygenBreather = entity.AddOrGet<OxygenBreather>();
    oxygenBreather.O2toCO2conversion = 0.02f;
    oxygenBreather.lowOxygenThreshold = 0.52f;
    oxygenBreather.noOxygenThreshold = 0.05f;
    oxygenBreather.mouthOffset = Vector2f.op_Implicit(new Vector2f(0.25f, 0.97f));
    oxygenBreather.minCO2ToEmit = 0.02f;
    oxygenBreather.breathableCells = OxygenBreather.DEFAULT_BREATHABLE_OFFSETS;
    entity.AddOrGet<WarmBlooded>();
    entity.AddOrGet<MinionIdentity>();
    GridVisibility gridVisibility = entity.AddOrGet<GridVisibility>();
    gridVisibility.radius = 30;
    gridVisibility.innerRadius = 20f;
    entity.AddOrGet<MiningSounds>();
    entity.AddOrGet<SaveLoadRoot>();
    MoverLayerOccupier moverLayerOccupier = entity.AddOrGet<MoverLayerOccupier>();
    moverLayerOccupier.objectLayers = new ObjectLayer[2]
    {
      ObjectLayer.Minion,
      ObjectLayer.Mover
    };
    moverLayerOccupier.cellOffsets = new CellOffset[2]
    {
      CellOffset.none,
      new CellOffset(0, 1)
    };
    Navigator navigator = entity.AddOrGet<Navigator>();
    navigator.NavGridName = MinionConfig.MINION_NAV_GRID_NAME;
    navigator.CurrentNavType = NavType.Floor;
    KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.isMovable = true;
    kbatchedAnimController.sceneLayer = Grid.SceneLayer.Move;
    kbatchedAnimController.AnimFiles = new KAnimFile[8]
    {
      Assets.GetAnim(HashedString.op_Implicit("body_comp_default_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_construction_default_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_idles_default_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_loco_firepole_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_loco_new_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_loco_tube_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_construction_firepole_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_construction_jetsuit_kanim"))
    };
    KBoxCollider2D kboxCollider2D = entity.AddOrGet<KBoxCollider2D>();
    kboxCollider2D.offset = new Vector2(0.0f, 0.75f);
    kboxCollider2D.size = new Vector2(1f, 1.5f);
    entity.AddOrGet<SnapOn>().snapPoints = new List<SnapOn.SnapPoint>((IEnumerable<SnapOn.SnapPoint>) new SnapOn.SnapPoint[19]
    {
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("dig"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("excavator_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("build"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("constructor_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("fetchliquid"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("water_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("paint"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("painting_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("harvest"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("plant_harvester_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("capture"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("net_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("attack"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("attack_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("pickup"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("pickupdrop_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("store"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("pickupdrop_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("disinfect"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("plant_spray_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("tend"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("plant_harvester_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "carry",
        automatic = false,
        context = HashedString.op_Implicit(""),
        buildFile = (KAnimFile) null,
        overrideSymbol = HashedString.op_Implicit("snapTo_chest")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "build",
        automatic = false,
        context = HashedString.op_Implicit(""),
        buildFile = (KAnimFile) null,
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "remote",
        automatic = false,
        context = HashedString.op_Implicit(""),
        buildFile = (KAnimFile) null,
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "snapTo_neck",
        automatic = false,
        context = HashedString.op_Implicit(""),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("body_oxygen_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_neck")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("powertinker"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("electrician_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("specialistdig"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("excavator_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "mask_oxygen",
        automatic = false,
        context = HashedString.op_Implicit(""),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("mask_oxygen_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_goggles")
      },
      new SnapOn.SnapPoint()
      {
        pointName = "dig",
        automatic = false,
        context = HashedString.op_Implicit("demolish"),
        buildFile = Assets.GetAnim(HashedString.op_Implicit("poi_demolish_gun_kanim")),
        overrideSymbol = HashedString.op_Implicit("snapTo_rgtHand")
      }
    });
    PrimaryElement primaryElement = entity.AddOrGet<PrimaryElement>();
    primaryElement.InternalTemperature = 310.15f;
    primaryElement.MassPerUnit = 30f;
    primaryElement.ElementID = SimHashes.Creature;
    entity.AddOrGet<ChoreProvider>();
    entity.AddOrGetDef<DebugGoToMonitor.Def>();
    entity.AddOrGet<Sensors>();
    entity.AddOrGet<Chattable>();
    entity.AddOrGet<FaceGraph>();
    entity.AddOrGet<Accessorizer>();
    entity.AddOrGet<Schedulable>();
    entity.AddOrGet<LoopingSounds>().updatePosition = true;
    entity.AddOrGet<AnimEventHandler>();
    entity.AddOrGet<FactionAlignment>().Alignment = FactionManager.FactionID.Duplicant;
    entity.AddOrGet<Weapon>();
    entity.AddOrGet<RangedAttackable>();
    entity.AddOrGet<CharacterOverlay>().shouldShowName = true;
    OccupyArea occupyArea = entity.AddOrGet<OccupyArea>();
    occupyArea.objectLayers = new ObjectLayer[1];
    occupyArea.ApplyToCells = false;
    occupyArea.OccupiedCellsOffsets = new CellOffset[2]
    {
      new CellOffset(0, 0),
      new CellOffset(0, 1)
    };
    entity.AddOrGet<Pickupable>();
    CreatureSimTemperatureTransfer temperatureTransfer = entity.AddOrGet<CreatureSimTemperatureTransfer>();
    temperatureTransfer.SurfaceArea = 10f;
    temperatureTransfer.Thickness = 0.01f;
    entity.AddOrGet<SicknessTrigger>();
    entity.AddOrGet<ClothingWearer>();
    entity.AddOrGet<SuitEquipper>();
    entity.AddOrGet<DecorProvider>().baseRadius = 3f;
    entity.AddOrGet<ConsumableConsumer>();
    entity.AddOrGet<NoiseListener>();
    entity.AddOrGet<MinionResume>();
    DuplicantNoiseLevels.SetupNoiseLevels();
    this.SetupLaserEffects(entity);
    this.SetupDreams(entity);
    SymbolOverrideControllerUtil.AddToPrefab(entity).applySymbolOverridesEveryFrame = true;
    MinionConfig.ConfigureSymbols(entity);
    return entity;
  }

  private void SetupDreams(GameObject prefab)
  {
    GameObject gameObject = new GameObject("Dreams");
    gameObject.transform.SetParent(prefab.transform, false);
    KBatchedAnimEventToggler animEventToggler = gameObject.AddComponent<KBatchedAnimEventToggler>();
    animEventToggler.eventSource = prefab;
    animEventToggler.enableEvent = "DreamsOn";
    animEventToggler.disableEvent = "DreamsOff";
    animEventToggler.entries = new List<KBatchedAnimEventToggler.Entry>();
    MinionConfig.Dream[] dreamArray = new MinionConfig.Dream[1]
    {
      new MinionConfig.Dream()
      {
        id = "Common Dream",
        animFile = "dream_tear_swirly_kanim",
        anim = "dream_loop",
        context = HashedString.op_Implicit("sleep")
      }
    };
    KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
    for (int index = 0; index < dreamArray.Length; ++index)
    {
      MinionConfig.Dream dream = dreamArray[index];
      GameObject go = new GameObject(dream.id);
      go.transform.SetParent(gameObject.transform, false);
      go.AddOrGet<KPrefabID>().PrefabTag = new Tag(dream.id);
      KBatchedAnimTracker kbatchedAnimTracker = go.AddOrGet<KBatchedAnimTracker>();
      kbatchedAnimTracker.controller = component;
      kbatchedAnimTracker.symbol = new HashedString("snapto_pivot");
      kbatchedAnimTracker.offset = new Vector3(180f, -300f, 0.0f);
      kbatchedAnimTracker.useTargetPoint = true;
      KBatchedAnimController kbatchedAnimController = go.AddOrGet<KBatchedAnimController>();
      kbatchedAnimController.AnimFiles = new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit(dream.animFile))
      };
      KBatchedAnimEventToggler.Entry entry = new KBatchedAnimEventToggler.Entry()
      {
        anim = dream.anim,
        context = dream.context,
        controller = kbatchedAnimController
      };
      animEventToggler.entries.Add(entry);
      go.AddOrGet<LoopingSounds>();
    }
  }

  private void SetupLaserEffects(GameObject prefab)
  {
    GameObject gameObject = new GameObject("LaserEffect");
    gameObject.transform.parent = prefab.transform;
    KBatchedAnimEventToggler animEventToggler = gameObject.AddComponent<KBatchedAnimEventToggler>();
    animEventToggler.eventSource = prefab;
    animEventToggler.enableEvent = "LaserOn";
    animEventToggler.disableEvent = "LaserOff";
    animEventToggler.entries = new List<KBatchedAnimEventToggler.Entry>();
    MinionConfig.LaserEffect[] laserEffectArray = new MinionConfig.LaserEffect[14]
    {
      new MinionConfig.LaserEffect()
      {
        id = "DigEffect",
        animFile = "laser_kanim",
        anim = "idle",
        context = HashedString.op_Implicit("dig")
      },
      new MinionConfig.LaserEffect()
      {
        id = "BuildEffect",
        animFile = "construct_beam_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("build")
      },
      new MinionConfig.LaserEffect()
      {
        id = "FetchLiquidEffect",
        animFile = "hose_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("fetchliquid")
      },
      new MinionConfig.LaserEffect()
      {
        id = "PaintEffect",
        animFile = "paint_beam_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("paint")
      },
      new MinionConfig.LaserEffect()
      {
        id = "HarvestEffect",
        animFile = "plant_harvest_beam_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("harvest")
      },
      new MinionConfig.LaserEffect()
      {
        id = "CaptureEffect",
        animFile = "net_gun_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("capture")
      },
      new MinionConfig.LaserEffect()
      {
        id = "AttackEffect",
        animFile = "attack_beam_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("attack")
      },
      new MinionConfig.LaserEffect()
      {
        id = "PickupEffect",
        animFile = "vacuum_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("pickup")
      },
      new MinionConfig.LaserEffect()
      {
        id = "StoreEffect",
        animFile = "vacuum_reverse_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("store")
      },
      new MinionConfig.LaserEffect()
      {
        id = "DisinfectEffect",
        animFile = "plant_spray_beam_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("disinfect")
      },
      new MinionConfig.LaserEffect()
      {
        id = "TendEffect",
        animFile = "plant_tending_beam_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("tend")
      },
      new MinionConfig.LaserEffect()
      {
        id = "PowerTinkerEffect",
        animFile = "electrician_beam_fx_kanim",
        anim = "idle",
        context = HashedString.op_Implicit("powertinker")
      },
      new MinionConfig.LaserEffect()
      {
        id = "SpecialistDigEffect",
        animFile = "senior_miner_beam_fx_kanim",
        anim = "idle",
        context = HashedString.op_Implicit("specialistdig")
      },
      new MinionConfig.LaserEffect()
      {
        id = "DemolishEffect",
        animFile = "poi_demolish_fx_kanim",
        anim = "idle",
        context = HashedString.op_Implicit("demolish")
      }
    };
    KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
    foreach (MinionConfig.LaserEffect laserEffect in laserEffectArray)
    {
      GameObject go = new GameObject(laserEffect.id);
      go.transform.parent = gameObject.transform;
      go.AddOrGet<KPrefabID>().PrefabTag = new Tag(laserEffect.id);
      KBatchedAnimTracker kbatchedAnimTracker = go.AddOrGet<KBatchedAnimTracker>();
      kbatchedAnimTracker.controller = component;
      kbatchedAnimTracker.symbol = new HashedString("snapTo_rgtHand");
      kbatchedAnimTracker.offset = new Vector3(195f, -35f, 0.0f);
      kbatchedAnimTracker.useTargetPoint = true;
      KBatchedAnimController kbatchedAnimController = go.AddOrGet<KBatchedAnimController>();
      kbatchedAnimController.AnimFiles = new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit(laserEffect.animFile))
      };
      KBatchedAnimEventToggler.Entry entry = new KBatchedAnimEventToggler.Entry()
      {
        anim = laserEffect.anim,
        context = laserEffect.context,
        controller = kbatchedAnimController
      };
      animEventToggler.entries.Add(entry);
      go.AddOrGet<LoopingSounds>();
    }
  }

  public void OnPrefabInit(GameObject go)
  {
    AmountInstance amountInstance1 = Db.Get().Amounts.ImmuneLevel.Lookup(go);
    amountInstance1.value = amountInstance1.GetMax();
    Db.Get().Amounts.Bladder.Lookup(go).value = Random.Range(0.0f, 10f);
    Db.Get().Amounts.Stress.Lookup(go).value = 5f;
    Db.Get().Amounts.Temperature.Lookup(go).value = 310.15f;
    AmountInstance amountInstance2 = Db.Get().Amounts.Stamina.Lookup(go);
    amountInstance2.value = amountInstance2.GetMax();
    AmountInstance amountInstance3 = Db.Get().Amounts.Breath.Lookup(go);
    amountInstance3.value = amountInstance3.GetMax();
    AmountInstance amountInstance4 = Db.Get().Amounts.Calories.Lookup(go);
    amountInstance4.value = 0.8875f * amountInstance4.GetMax();
  }

  public void OnSpawn(GameObject go)
  {
    Sensors component1 = go.GetComponent<Sensors>();
    component1.Add((Sensor) new PathProberSensor(component1));
    component1.Add((Sensor) new SafeCellSensor(component1));
    component1.Add((Sensor) new IdleCellSensor(component1));
    component1.Add((Sensor) new PickupableSensor(component1));
    component1.Add((Sensor) new ClosestEdibleSensor(component1));
    component1.Add((Sensor) new BreathableAreaSensor(component1));
    component1.Add((Sensor) new AssignableReachabilitySensor(component1));
    component1.Add((Sensor) new ToiletSensor(component1));
    component1.Add((Sensor) new MingleCellSensor(component1));
    component1.Add((Sensor) new BalloonStandCellSensor(component1));
    new RationalAi.Instance((IStateMachineTarget) go.GetComponent<StateMachineController>()).StartSM();
    if (go.GetComponent<OxygenBreather>().GetGasProvider() == null)
      go.GetComponent<OxygenBreather>().SetGasProvider((OxygenBreather.IGasProvider) new GasBreatherFromWorldProvider());
    Navigator component2 = go.GetComponent<Navigator>();
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new BipedTransitionLayer(component2, 3.325f, 2.5f));
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new DoorTransitionLayer(component2));
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new TubeTransitionLayer(component2));
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new LadderDiseaseTransitionLayer(component2));
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new ReactableTransitionLayer(component2));
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new NavTeleportTransitionLayer(component2));
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new SplashTransitionLayer(component2));
    ThreatMonitor.Instance smi = go.GetSMI<ThreatMonitor.Instance>();
    if (smi == null)
      return;
    smi.def.fleethresholdState = Health.HealthState.Critical;
  }

  public static void AddMinionAmounts(Modifiers modifiers)
  {
    modifiers.initialAttributes.Add(Db.Get().Attributes.AirConsumptionRate.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.MaxUnderwaterTravelCost.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.DecorExpectation.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.FoodExpectation.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.ToiletEfficiency.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.RoomTemperaturePreference.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.CarryAmount.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.QualityOfLife.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.SpaceNavigation.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.Sneezyness.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.RadiationResistance.Id);
    modifiers.initialAttributes.Add(Db.Get().Attributes.RadiationRecovery.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.Stamina.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.Calories.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.ImmuneLevel.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.Breath.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.Stress.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.Toxicity.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.Bladder.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.Temperature.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.ExternalTemperature.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.Decor.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.RadiationBalance.Id);
  }

  public static void AddMinionTraits(string name, Modifiers modifiers)
  {
    Trait trait = Db.Get().CreateTrait(MinionConfig.MINION_BASE_TRAIT_ID, name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Attributes.AirConsumptionRate.Id, 0.1f, name));
    trait.Add(new AttributeModifier(Db.Get().Attributes.MaxUnderwaterTravelCost.Id, 8f, name));
    trait.Add(new AttributeModifier(Db.Get().Attributes.DecorExpectation.Id, 0.0f, name));
    trait.Add(new AttributeModifier(Db.Get().Attributes.FoodExpectation.Id, 0.0f, name));
    trait.Add(new AttributeModifier(Db.Get().Attributes.ToiletEfficiency.Id, 1f, name));
    trait.Add(new AttributeModifier(Db.Get().Attributes.RoomTemperaturePreference.Id, 0.0f, name));
    trait.Add(new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, 200f, name));
    trait.Add(new AttributeModifier(Db.Get().Attributes.QualityOfLife.Id, 1f, name));
    if (!DlcManager.IsExpansion1Active())
      trait.Add(new AttributeModifier(Db.Get().Attributes.SpaceNavigation.Id, 1f, name));
    trait.Add(new AttributeModifier(Db.Get().Attributes.Sneezyness.Id, 0.0f, name));
    trait.Add(new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, 0.0f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Stamina.deltaAttribute.Id, -0.116666667f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -1666.66663f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, 4000000f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Toxicity.deltaAttribute.Id, 0.0f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, 0.166666672f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 100f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, 0.025f, name));
    modifiers.initialTraits.Add(MinionConfig.MINION_BASE_TRAIT_ID);
  }

  public static void ConfigureSymbols(GameObject go, bool show_defaults = false)
  {
    KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_hat"), false);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_hat_hair"), false);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_headfx"), false);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_chest"), false);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_neck"), false);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_goggles"), false);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_pivot"), false);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_rgtHand"), false);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("neck"), show_defaults);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("belt"), show_defaults);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("pelvis"), show_defaults);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("foot"), show_defaults);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("leg"), show_defaults);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("cuff"), show_defaults);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("arm_sleeve"), show_defaults);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("arm_lower_sleeve"), show_defaults);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("necklace"), false);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("skirt"), false);
  }

  public struct LaserEffect
  {
    public string id;
    public string animFile;
    public string anim;
    public HashedString context;
  }

  public struct Dream
  {
    public string id;
    public string animFile;
    public string anim;
    public HashedString context;
  }
}
