// Decompiled with JetBrains decompiler
// Type: MegaBrainTankConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using TUNING;
using UnityEngine;

public class MegaBrainTankConfig : IBuildingConfig
{
  public const string ID = "MegaBrainTank";
  public const string INITIAL_LORE_UNLOCK_ID = "story_trait_mega_brain_tank_initial";
  public const string COMPLETED_LORE_UNLOCK_ID = "story_trait_mega_brain_tank_competed";
  public const string ACTIVE_EFFECT_ID = "MegaBrainTankBonus";
  public static object[,] STAT_BONUSES = new object[6, 3]
  {
    {
      (object) Db.Get().Amounts.Stress.deltaAttribute.Id,
      (object) -25f,
      (object) (Units) 1
    },
    {
      (object) Db.Get().Attributes.Athletics.Id,
      (object) 5f,
      (object) (Units) 0
    },
    {
      (object) Db.Get().Attributes.Strength.Id,
      (object) 5f,
      (object) (Units) 0
    },
    {
      (object) Db.Get().Attributes.Learning.Id,
      (object) 5f,
      (object) (Units) 0
    },
    {
      (object) Db.Get().Attributes.SpaceNavigation.Id,
      (object) 5f,
      (object) (Units) 0
    },
    {
      (object) Db.Get().Attributes.Machinery.Id,
      (object) 5f,
      (object) (Units) 0
    }
  };
  private const float KG_OXYGEN_CONSUMED_PER_SECOND = 0.5f;
  public const float MIN_OXYGEN_TO_WAKE_UP = 1f;
  private const float KG_OXYGEN_STORAGE_CAPACITY = 5f;
  public const short JOURNALS_TO_ACTIVATE = 25;
  public const float DIGESTION_RATE = 60f;
  public const float MAX_DIGESTION_TIME = 1500f;
  public const float REFILL_THESHOLD_ADJUSTMENT = 1f;
  public const short MAX_PHYSICAL_JOURNALS = 5;
  public const ConduitType CONDUIT_TYPE = ConduitType.Gas;
  private const string ANIM_FILE = "gravitas_megabrain_kanim";
  public const string METER_ANIM = "meter";
  public const string METER_TARGET = "meter_oxygen_target";
  public static string[] METER_SYMBOLS = new string[3]
  {
    "meter_oxygen_target",
    "meter_oxygen_frame",
    "meter_oxygen_fill"
  };
  public const short TOTAL_BRAINS = 5;
  public const string BRAIN_HUM_EVENT = "MegaBrainTank_brain_wave_LP";
  public const float METER_INCREMENT_SPEED = 0.04f;
  public static HashedString ACTIVATE_ALL = new HashedString("brains_up");
  public static HashedString DEACTIVATE_ALL = new HashedString("brains_down");
  public static HashedString[] ACTIVATION_ANIMS = new HashedString[10]
  {
    new HashedString("brain1_pre"),
    new HashedString("brain2_pre"),
    new HashedString("brain3_pre"),
    new HashedString("brain4_pre"),
    new HashedString("brain5_pre"),
    new HashedString("brain1_loop"),
    new HashedString("brain2_loop"),
    new HashedString("brain3_loop"),
    new HashedString("brain4_loop"),
    new HashedString("idle")
  };
  public const short MAX_STORAGE_WORK_TIME = 2;
  private const string KACHUNK_ANIM = "kachunk";
  public static HashedString KACHUNK = new HashedString("kachunk");
  public static HashedString JOURNAL_SHELF = new HashedString("meter_journals_target");
  public static HashedString[] JOURNAL_SYMBOLS = new HashedString[5]
  {
    new HashedString("journal1"),
    new HashedString("journal2"),
    new HashedString("journal3"),
    new HashedString("journal4"),
    new HashedString("journal5")
  };
  public static StatusItem MaximumAptitude = new StatusItem(nameof (MaximumAptitude), (string) DUPLICANTS.MODIFIERS.MEGABRAINTANKBONUS.NAME, (string) DUPLICANTS.MODIFIERS.MEGABRAINTANKBONUS.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Messages, false, OverlayModes.None.ID);

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues tieR5_2 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR4 = TUNING.BUILDINGS.DECOR.PENALTY.TIER4;
    EffectorValues noise = tieR5_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MegaBrainTank", 7, 7, "gravitas_megabrain_kanim", 100, 120f, tieR5_1, rawMetals, 2400f, BuildLocationRule.OnFloor, tieR4, noise);
    buildingDef.Floodable = true;
    buildingDef.Entombable = false;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.ShowInBuildMenu = false;
    buildingDef.InputConduitType = ConduitType.Gas;
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    base.ConfigureBuildingTemplate(go, prefab_tag);
    Light2D light2D = go.AddOrGet<Light2D>();
    light2D.Color = LIGHT2D.HEADQUARTERS_COLOR;
    light2D.Range = 7f;
    light2D.Lux = 7200;
    light2D.overlayColour = LIGHT2D.HEADQUARTERS_OVERLAYCOLOR;
    light2D.shape = LightShape.Circle;
    light2D.drawOverlay = true;
    light2D.Offset = new Vector2(0.0f, 2f);
    go.GetComponent<BuildingHP>().invincible = true;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LoopingSounds>();
    go.AddOrGet<Demolishable>();
    go.AddOrGet<MegaBrainTank>();
    go.AddOrGet<Notifier>();
    KPrefabID component1 = go.GetComponent<KPrefabID>();
    component1.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    component1.AddTag(GameTags.Gravitas, false);
    this.ConfigureJournalShelf(component1);
    Activatable activatable = go.AddOrGet<Activatable>();
    activatable.SetWorkTime(5f);
    activatable.ActivationFlagType = Operational.Flag.Type.Functional;
    activatable.synchronizeAnims = false;
    activatable.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_use_remote_kanim"))
    };
    PrimaryElement component2 = go.GetComponent<PrimaryElement>();
    component2.SetElement(SimHashes.Steel);
    component2.Temperature = 294.15f;
    Storage storage = go.AddOrGet<Storage>();
    storage.showInUI = true;
    storage.storageWorkTime = 2f;
    storage.capacityKg = 30f;
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestItem(DreamJournalConfig.ID, 1f);
    manualDeliveryKg.refillMass = 24f;
    manualDeliveryKg.capacity = 25f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.Fetch.IdHash;
    manualDeliveryKg.operationalRequirement = Operational.State.Functional;
    manualDeliveryKg.ShowStatusItem = false;
    manualDeliveryKg.RoundFetchAmountToInt = true;
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.consumptionRate = 10f;
    conduitConsumer.forceAlwaysSatisfied = true;
    conduitConsumer.conduitType = ConduitType.Gas;
    conduitConsumer.capacityKG = 5f;
    conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Oxygen);
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
    conduitConsumer.OperatingRequirement = Operational.State.Functional;
    RequireInputs requireInputs = go.AddOrGet<RequireInputs>();
    requireInputs.requireConduitHasMass = false;
    requireInputs.visualizeRequirements = RequireInputs.Requirements.NoWire;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
    {
      new ElementConverter.ConsumedElement(ElementLoader.FindElementByHash(SimHashes.Oxygen).tag, 0.5f),
      new ElementConverter.ConsumedElement(DreamJournalConfig.ID, 0.0166666675f)
    };
    elementConverter.OperationalRequirement = Operational.State.Operational;
    elementConverter.ShowInUI = false;
    go.GetComponent<Deconstructable>().allowDeconstruction = false;
  }

  private void ConfigureJournalShelf(KPrefabID parentId)
  {
    KBatchedAnimController component = ((Component) parentId).GetComponent<KBatchedAnimController>();
    GameObject gameObject = new GameObject("Journal Shelf");
    gameObject.transform.SetParent(((KMonoBehaviour) parentId).transform);
    gameObject.transform.localPosition = Vector3.op_Multiply(Vector3.forward, -0.1f);
    gameObject.AddComponent<KPrefabID>().PrefabTag = parentId.PrefabTag;
    KBatchedAnimController controller = gameObject.AddComponent<KBatchedAnimController>();
    controller.AnimFiles = component.AnimFiles;
    controller.fgLayer = Grid.SceneLayer.NoLayer;
    controller.initialAnim = "kachunk";
    controller.initialMode = (KAnim.PlayMode) 2;
    controller.isMovable = true;
    controller.FlipX = component.FlipX;
    controller.FlipY = component.FlipY;
    KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
    kbatchedAnimTracker.SetAnimControllers(controller, component);
    kbatchedAnimTracker.symbol = MegaBrainTankConfig.JOURNAL_SHELF;
    kbatchedAnimTracker.offset = Vector3.zero;
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit(MegaBrainTankConfig.JOURNAL_SHELF), false);
    for (int index = 0; index < MegaBrainTankConfig.JOURNAL_SYMBOLS.Length; ++index)
    {
      component.SetSymbolVisiblity(KAnimHashedString.op_Implicit(MegaBrainTankConfig.JOURNAL_SYMBOLS[index]), false);
      controller.SetSymbolVisiblity(KAnimHashedString.op_Implicit(MegaBrainTankConfig.JOURNAL_SYMBOLS[index]), false);
    }
  }
}
