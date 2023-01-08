// Decompiled with JetBrains decompiler
// Type: CO2ScrubberConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class CO2ScrubberConfig : IBuildingConfig
{
  public const string ID = "CO2Scrubber";
  private const float CO2_CONSUMPTION_RATE = 0.3f;
  private const float H2O_CONSUMPTION_RATE = 1f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues tieR3 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = tieR3;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CO2Scrubber", 2, 2, "co2scrubber_kanim", 30, 30f, tieR2, rawMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.SelfHeatKilowattsWhenActive = 1f;
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.OutputConduitType = ConduitType.Liquid;
    buildingDef.ViewMode = OverlayModes.Oxygen.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "large";
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(1, 1);
    buildingDef.PermittedRotations = PermittedRotations.FlipH;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
    defaultStorage.showInUI = true;
    defaultStorage.capacityKg = 30000f;
    defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    go.AddOrGet<AirFilter>().filterTag = GameTagExtensions.Create(SimHashes.Water);
    PassiveElementConsumer passiveElementConsumer = go.AddOrGet<PassiveElementConsumer>();
    passiveElementConsumer.elementToConsume = SimHashes.CarbonDioxide;
    passiveElementConsumer.consumptionRate = 0.6f;
    passiveElementConsumer.capacityKG = 0.6f;
    passiveElementConsumer.consumptionRadius = (byte) 3;
    passiveElementConsumer.showInStatusPanel = true;
    passiveElementConsumer.sampleCellOffset = new Vector3(0.0f, 0.0f, 0.0f);
    passiveElementConsumer.isRequired = false;
    passiveElementConsumer.storeOnConsume = true;
    passiveElementConsumer.showDescriptor = false;
    passiveElementConsumer.ignoreActiveChanged = true;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
    {
      new ElementConverter.ConsumedElement(GameTagExtensions.Create(SimHashes.Water), 1f),
      new ElementConverter.ConsumedElement(GameTagExtensions.Create(SimHashes.CarbonDioxide), 0.3f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(1f, SimHashes.DirtyWater, 0.0f, storeOutput: true)
    };
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.consumptionRate = 2f;
    conduitConsumer.capacityKG = 2f;
    conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
    conduitConsumer.forceAlwaysSatisfied = true;
    ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
    conduitDispenser.conduitType = ConduitType.Liquid;
    conduitDispenser.invertElementFilter = true;
    conduitDispenser.elementFilter = new SimHashes[1]
    {
      SimHashes.Water
    };
    go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
