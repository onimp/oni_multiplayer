// Decompiled with JetBrains decompiler
// Type: ScoutRoverConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoutRoverConfig : IEntityConfig
{
  public const string ID = "ScoutRover";
  public static string ROVER_BASE_TRAIT_ID = "ScoutRoverBaseTrait";
  public const int MAXIMUM_TECH_CONSTRUCTION_TIER = 2;
  public const float MASS = 100f;
  private const float WIDTH = 1f;
  private const float HEIGHT = 2f;

  public static GameObject CreateScout(string id, string name, string desc, string anim_file)
  {
    string id1 = id;
    string name1 = name;
    string desc1 = desc;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.Experimental);
    GameObject basicEntity = EntityTemplates.CreateBasicEntity(id1, name1, desc1, 100f, true, anim, "idle_loop", Grid.SceneLayer.Creatures, additionalTags: additionalTags);
    KBatchedAnimController component1 = basicEntity.GetComponent<KBatchedAnimController>();
    component1.isMovable = true;
    basicEntity.AddOrGet<Modifiers>();
    basicEntity.AddOrGet<LoopingSounds>();
    KBoxCollider2D kboxCollider2D = basicEntity.AddOrGet<KBoxCollider2D>();
    kboxCollider2D.size = new Vector2(1f, 2f);
    kboxCollider2D.offset = Vector2f.op_Implicit(new Vector2f(0.0f, 1f));
    Modifiers component2 = basicEntity.GetComponent<Modifiers>();
    component2.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
    component2.initialAmounts.Add(Db.Get().Amounts.InternalChemicalBattery.Id);
    component2.initialAttributes.Add(Db.Get().Attributes.Construction.Id);
    component2.initialAttributes.Add(Db.Get().Attributes.Digging.Id);
    component2.initialAttributes.Add(Db.Get().Attributes.CarryAmount.Id);
    component2.initialAttributes.Add(Db.Get().Attributes.Machinery.Id);
    component2.initialAttributes.Add(Db.Get().Attributes.Athletics.Id);
    ChoreGroup[] disabled_chore_groups = new ChoreGroup[12]
    {
      Db.Get().ChoreGroups.Basekeeping,
      Db.Get().ChoreGroups.Cook,
      Db.Get().ChoreGroups.Art,
      Db.Get().ChoreGroups.Research,
      Db.Get().ChoreGroups.Farming,
      Db.Get().ChoreGroups.Ranching,
      Db.Get().ChoreGroups.MachineOperating,
      Db.Get().ChoreGroups.MedicalAid,
      Db.Get().ChoreGroups.Combat,
      Db.Get().ChoreGroups.LifeSupport,
      Db.Get().ChoreGroups.Recreation,
      Db.Get().ChoreGroups.Toggle
    };
    basicEntity.AddOrGet<Traits>();
    Trait trait = Db.Get().CreateTrait(ScoutRoverConfig.ROVER_BASE_TRAIT_ID, (string) STRINGS.ROBOTS.MODELS.SCOUT.NAME, (string) STRINGS.ROBOTS.MODELS.SCOUT.NAME, (string) null, false, disabled_chore_groups, true, true);
    trait.Add(new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, 200f, (string) STRINGS.ROBOTS.MODELS.SCOUT.NAME));
    trait.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, TUNING.ROBOTS.SCOUTBOT.DIGGING, (string) STRINGS.ROBOTS.MODELS.SCOUT.NAME));
    trait.Add(new AttributeModifier(Db.Get().Attributes.Construction.Id, TUNING.ROBOTS.SCOUTBOT.CONSTRUCTION, (string) STRINGS.ROBOTS.MODELS.SCOUT.NAME));
    trait.Add(new AttributeModifier(Db.Get().Attributes.Athletics.Id, TUNING.ROBOTS.SCOUTBOT.ATHLETICS, (string) STRINGS.ROBOTS.MODELS.SCOUT.NAME));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, TUNING.ROBOTS.SCOUTBOT.HIT_POINTS, (string) STRINGS.ROBOTS.MODELS.SCOUT.NAME));
    trait.Add(new AttributeModifier(Db.Get().Amounts.InternalChemicalBattery.maxAttribute.Id, TUNING.ROBOTS.SCOUTBOT.BATTERY_CAPACITY, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.InternalChemicalBattery.deltaAttribute.Id, -TUNING.ROBOTS.SCOUTBOT.BATTERY_DEPLETION_RATE, name));
    component2.initialTraits.Add(ScoutRoverConfig.ROVER_BASE_TRAIT_ID);
    basicEntity.AddOrGet<AttributeConverters>();
    GridVisibility gridVisibility = basicEntity.AddOrGet<GridVisibility>();
    gridVisibility.radius = 30;
    gridVisibility.innerRadius = 20f;
    basicEntity.AddOrGet<Worker>();
    basicEntity.AddOrGet<Effects>();
    basicEntity.AddOrGet<Traits>();
    basicEntity.AddOrGet<AnimEventHandler>();
    basicEntity.AddOrGet<Health>();
    MoverLayerOccupier moverLayerOccupier = basicEntity.AddOrGet<MoverLayerOccupier>();
    moverLayerOccupier.objectLayers = new ObjectLayer[2]
    {
      ObjectLayer.Rover,
      ObjectLayer.Mover
    };
    moverLayerOccupier.cellOffsets = new CellOffset[2]
    {
      CellOffset.none,
      new CellOffset(0, 1)
    };
    RobotBatteryMonitor.Def def = basicEntity.AddOrGetDef<RobotBatteryMonitor.Def>();
    def.batteryAmountId = Db.Get().Amounts.InternalChemicalBattery.Id;
    def.canCharge = false;
    def.lowBatteryWarningPercent = 0.2f;
    Storage storage = basicEntity.AddOrGet<Storage>();
    storage.fxPrefix = Storage.FXPrefix.PickedUp;
    storage.dropOnLoad = true;
    storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Preserve,
      Storage.StoredItemModifier.Seal
    });
    basicEntity.AddOrGetDef<CreatureDebugGoToMonitor.Def>();
    Deconstructable deconstructable = basicEntity.AddOrGet<Deconstructable>();
    ((Behaviour) deconstructable).enabled = false;
    deconstructable.audioSize = "medium";
    deconstructable.looseEntityDeconstructable = true;
    basicEntity.AddOrGetDef<RobotAi.Def>();
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new RobotDeathStates.Def()).Add((StateMachine.BaseDef) new FallStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new IdleStates.Def(), forcePriority: Db.Get().ChoreTypes.Idle.priority);
    EntityTemplates.AddCreatureBrain(basicEntity, chore_table, GameTags.Robots.Models.ScoutRover, (string) null);
    KPrefabID kprefabId = basicEntity.AddOrGet<KPrefabID>();
    kprefabId.RemoveTag(GameTags.CreatureBrain);
    kprefabId.AddTag(GameTags.DupeBrain, false);
    kprefabId.AddTag(GameTags.Robot, false);
    Navigator navigator = basicEntity.AddOrGet<Navigator>();
    navigator.NavGridName = "RobotNavGrid";
    navigator.CurrentNavType = NavType.Floor;
    navigator.defaultSpeed = 2f;
    navigator.updateProber = true;
    navigator.sceneLayer = Grid.SceneLayer.Creatures;
    basicEntity.AddOrGet<Sensors>();
    basicEntity.AddOrGet<Pickupable>().SetWorkTime(5f);
    basicEntity.AddOrGet<SnapOn>();
    component1.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_pivot"), false);
    component1.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_radar"), false);
    return basicEntity;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject scout = ScoutRoverConfig.CreateScout("ScoutRover", (string) STRINGS.ROBOTS.MODELS.SCOUT.NAME, (string) STRINGS.ROBOTS.MODELS.SCOUT.DESC, "scout_bot_kanim");
    this.SetupLaserEffects(scout);
    return scout;
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
    ScoutRoverConfig.LaserEffect[] laserEffectArray = new ScoutRoverConfig.LaserEffect[14]
    {
      new ScoutRoverConfig.LaserEffect()
      {
        id = "DigEffect",
        animFile = "laser_kanim",
        anim = "idle",
        context = HashedString.op_Implicit("dig")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "BuildEffect",
        animFile = "construct_beam_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("build")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "FetchLiquidEffect",
        animFile = "hose_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("fetchliquid")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "PaintEffect",
        animFile = "paint_beam_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("paint")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "HarvestEffect",
        animFile = "plant_harvest_beam_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("harvest")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "CaptureEffect",
        animFile = "net_gun_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("capture")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "AttackEffect",
        animFile = "attack_beam_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("attack")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "PickupEffect",
        animFile = "vacuum_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("pickup")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "StoreEffect",
        animFile = "vacuum_reverse_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("store")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "DisinfectEffect",
        animFile = "plant_spray_beam_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("disinfect")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "TendEffect",
        animFile = "plant_tending_beam_fx_kanim",
        anim = "loop",
        context = HashedString.op_Implicit("tend")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "PowerTinkerEffect",
        animFile = "electrician_beam_fx_kanim",
        anim = "idle",
        context = HashedString.op_Implicit("powertinker")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "SpecialistDigEffect",
        animFile = "senior_miner_beam_fx_kanim",
        anim = "idle",
        context = HashedString.op_Implicit("specialistdig")
      },
      new ScoutRoverConfig.LaserEffect()
      {
        id = "DemolishEffect",
        animFile = "poi_demolish_fx_kanim",
        anim = "idle",
        context = HashedString.op_Implicit("demolish")
      }
    };
    KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
    foreach (ScoutRoverConfig.LaserEffect laserEffect in laserEffectArray)
    {
      GameObject go = new GameObject(laserEffect.id);
      go.transform.parent = gameObject.transform;
      go.AddOrGet<KPrefabID>().PrefabTag = new Tag(laserEffect.id);
      KBatchedAnimTracker kbatchedAnimTracker = go.AddOrGet<KBatchedAnimTracker>();
      kbatchedAnimTracker.controller = component;
      kbatchedAnimTracker.symbol = new HashedString("snapto_radar");
      kbatchedAnimTracker.offset = new Vector3(40f, 0.0f, 0.0f);
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

  public void OnPrefabInit(GameObject inst)
  {
    ChoreConsumer component = inst.GetComponent<ChoreConsumer>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.AddProvider((ChoreProvider) GlobalChoreProvider.Instance);
    AmountInstance amountInstance = Db.Get().Amounts.InternalChemicalBattery.Lookup(inst);
    amountInstance.value = amountInstance.GetMax();
  }

  public void OnSpawn(GameObject inst)
  {
    Sensors component1 = inst.GetComponent<Sensors>();
    component1.Add((Sensor) new PathProberSensor(component1));
    component1.Add((Sensor) new PickupableSensor(component1));
    Navigator component2 = inst.GetComponent<Navigator>();
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new BipedTransitionLayer(component2, 3.325f, 2.5f));
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new DoorTransitionLayer(component2));
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new LadderDiseaseTransitionLayer(component2));
    component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new SplashTransitionLayer(component2));
    component2.SetFlags(PathFinder.PotentialPath.Flags.None);
    component2.CurrentNavType = NavType.Floor;
    PathProber component3 = inst.GetComponent<PathProber>();
    if (Object.op_Inequality((Object) component3, (Object) null))
      component3.SetGroupProber((IGroupProber) MinionGroupProber.Get());
    Effects effects = inst.GetComponent<Effects>();
    if (Object.op_Equality((Object) inst.transform.parent, (Object) null))
    {
      if (effects.HasEffect("ScoutBotCharging"))
        effects.Remove("ScoutBotCharging");
    }
    else if (!effects.HasEffect("ScoutBotCharging"))
      effects.Add("ScoutBotCharging", false);
    KMonoBehaviourExtensions.Subscribe(inst, 856640610, (Action<object>) (data =>
    {
      if (Object.op_Equality((Object) inst.transform.parent, (Object) null))
      {
        if (!effects.HasEffect("ScoutBotCharging"))
          return;
        effects.Remove("ScoutBotCharging");
      }
      else
      {
        if (effects.HasEffect("ScoutBotCharging"))
          return;
        effects.Add("ScoutBotCharging", false);
      }
    }));
  }

  public struct LaserEffect
  {
    public string id;
    public string animFile;
    public string anim;
    public HashedString context;
  }
}
