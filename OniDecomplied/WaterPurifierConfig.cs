// Decompiled with JetBrains decompiler
// Type: WaterPurifierConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class WaterPurifierConfig : IBuildingConfig
{
  public const string ID = "WaterPurifier";
  private const float FILTER_INPUT_RATE = 1f;
  private const float DIRTY_WATER_INPUT_RATE = 5f;
  private const float FILTER_CAPACITY = 1200f;
  private const float USED_FILTER_OUTPUT_RATE = 0.2f;
  private const float CLEAN_WATER_OUTPUT_RATE = 5f;
  private const float TARGET_OUTPUT_TEMPERATURE = 313.15f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR3_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("WaterPurifier", 4, 3, "waterpurifier_kanim", 100, 30f, tieR3_1, allMetals, 800f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.SelfHeatKilowattsWhenActive = 4f;
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.OutputConduitType = ConduitType.Liquid;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 0));
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.PowerInputOffset = new CellOffset(2, 0);
    buildingDef.UtilityInputOffset = new CellOffset(-1, 2);
    buildingDef.UtilityOutputOffset = new CellOffset(2, 2);
    buildingDef.PermittedRotations = PermittedRotations.FlipH;
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "WaterPurifier");
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
    defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    go.AddOrGet<WaterPurifier>();
    Prioritizable.AddRef(go);
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
    {
      new ElementConverter.ConsumedElement(new Tag("Filter"), 1f),
      new ElementConverter.ConsumedElement(new Tag("DirtyWater"), 5f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[2]
    {
      new ElementConverter.OutputElement(5f, SimHashes.Water, 0.0f, storeOutput: true, diseaseWeight: 0.75f),
      new ElementConverter.OutputElement(0.2f, SimHashes.ToxicSand, 0.0f, storeOutput: true, diseaseWeight: 0.25f)
    };
    ElementDropper elementDropper = go.AddComponent<ElementDropper>();
    elementDropper.emitMass = 10f;
    elementDropper.emitTag = new Tag("ToxicSand");
    elementDropper.emitOffset = new Vector3(0.0f, 1f, 0.0f);
    ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(defaultStorage);
    manualDeliveryKg.RequestedItemTag = new Tag("Filter");
    manualDeliveryKg.capacity = 1200f;
    manualDeliveryKg.refillMass = 300f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.consumptionRate = 10f;
    conduitConsumer.capacityKG = 20f;
    conduitConsumer.capacityTag = GameTags.AnyWater;
    conduitConsumer.forceAlwaysSatisfied = true;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
    ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
    conduitDispenser.conduitType = ConduitType.Liquid;
    conduitDispenser.invertElementFilter = true;
    conduitDispenser.elementFilter = new SimHashes[1]
    {
      SimHashes.DirtyWater
    };
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGetDef<PoweredActiveController.Def>();
    go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
  }
}
