// Decompiled with JetBrains decompiler
// Type: FertilizerMakerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class FertilizerMakerConfig : IBuildingConfig
{
  public const string ID = "FertilizerMaker";
  private const float FERTILIZER_PER_LOAD = 10f;
  private const float FERTILIZER_PRODUCTION_RATE = 0.12f;
  private const float METHANE_PRODUCTION_RATE = 0.01f;
  private const float _TOTAL_PRODUCTION = 0.13f;
  private const float DIRT_CONSUMPTION_RATE = 0.065f;
  private const float DIRTY_WATER_CONSUMPTION_RATE = 0.039f;
  private const float PHOSPHORITE_CONSUMPTION_RATE = 0.0259999987f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FertilizerMaker", 4, 3, "fertilizer_maker_kanim", 30, 30f, tieR3, allMetals, 800f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.ExhaustKilowattsWhenActive = 1f;
    buildingDef.SelfHeatKilowattsWhenActive = 2f;
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.PowerInputOffset = new CellOffset(1, 0);
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 0));
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
    defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    go.AddOrGet<WaterPurifier>();
    ManualDeliveryKG manualDeliveryKg1 = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg1.SetStorage(defaultStorage);
    manualDeliveryKg1.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    manualDeliveryKg1.RequestedItemTag = new Tag("Dirt");
    manualDeliveryKg1.capacity = 136.5f;
    manualDeliveryKg1.refillMass = 19.5f;
    ManualDeliveryKG manualDeliveryKg2 = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg2.SetStorage(defaultStorage);
    manualDeliveryKg2.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    manualDeliveryKg2.RequestedItemTag = new Tag("Phosphorite");
    manualDeliveryKg2.capacity = 54.6f;
    manualDeliveryKg2.refillMass = 7.79999971f;
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.consumptionRate = 10f;
    conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
    conduitConsumer.capacityKG = 0.195000008f;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
    conduitConsumer.forceAlwaysSatisfied = true;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[3]
    {
      new ElementConverter.ConsumedElement(new Tag("DirtyWater"), 0.039f),
      new ElementConverter.ConsumedElement(new Tag("Dirt"), 0.065f),
      new ElementConverter.ConsumedElement(new Tag("Phosphorite"), 0.0259999987f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(0.12f, SimHashes.Fertilizer, 323.15f, storeOutput: true)
    };
    BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
    buildingElementEmitter.emitRate = 0.01f;
    buildingElementEmitter.temperature = 349.15f;
    buildingElementEmitter.element = SimHashes.Methane;
    buildingElementEmitter.modifierOffset = new Vector2(2f, 2f);
    ElementDropper elementDropper = go.AddComponent<ElementDropper>();
    elementDropper.emitMass = 10f;
    elementDropper.emitTag = new Tag("Fertilizer");
    elementDropper.emitOffset = new Vector3(0.0f, 1f, 0.0f);
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
