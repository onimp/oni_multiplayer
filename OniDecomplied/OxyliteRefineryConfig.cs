// Decompiled with JetBrains decompiler
// Type: OxyliteRefineryConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class OxyliteRefineryConfig : IBuildingConfig
{
  public const string ID = "OxyliteRefinery";
  public const float EMIT_MASS = 10f;
  public const float INPUT_O2_PER_SECOND = 0.6f;
  public const float OXYLITE_PER_SECOND = 0.6f;
  public const float GOLD_PER_SECOND = 0.003f;
  public const float OUTPUT_TEMP = 303.15f;
  public const float REFILL_RATE = 2400f;
  public const float GOLD_STORAGE_AMOUNT = 7.20000029f;
  public const float O2_STORAGE_AMOUNT = 6f;
  public const float STORAGE_CAPACITY = 23.2f;

  public override BuildingDef CreateBuildingDef()
  {
    string[] strArray = new string[2]
    {
      "RefinedMetal",
      "Plastic"
    };
    float[] construction_mass = new float[2]
    {
      BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
      BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
    };
    string[] construction_materials = strArray;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("OxyliteRefinery", 3, 4, "oxylite_refinery_kanim", 100, 480f, construction_mass, construction_materials, 2400f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.Overheatable = false;
    buildingDef.RequiresPowerInput = true;
    buildingDef.PowerInputOffset = new CellOffset(0, 0);
    buildingDef.EnergyConsumptionWhenActive = 1200f;
    buildingDef.ExhaustKilowattsWhenActive = 8f;
    buildingDef.SelfHeatKilowattsWhenActive = 4f;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.InputConduitType = ConduitType.Gas;
    buildingDef.UtilityInputOffset = new CellOffset(1, 0);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Tag tag1 = SimHashes.Oxygen.CreateTag();
    Tag tag2 = SimHashes.Gold.CreateTag();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    OxyliteRefinery oxyliteRefinery = go.AddOrGet<OxyliteRefinery>();
    oxyliteRefinery.emitTag = SimHashes.OxyRock.CreateTag();
    oxyliteRefinery.emitMass = 10f;
    oxyliteRefinery.dropOffset = new Vector3(0.0f, 1f);
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Gas;
    conduitConsumer.consumptionRate = 1.2f;
    conduitConsumer.capacityTag = tag1;
    conduitConsumer.capacityKG = 6f;
    conduitConsumer.forceAlwaysSatisfied = true;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 23.2f;
    storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    storage.showInUI = true;
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = tag2;
    manualDeliveryKg.refillMass = 1.80000007f;
    manualDeliveryKg.capacity = 7.20000029f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
    {
      new ElementConverter.ConsumedElement(tag1, 0.6f),
      new ElementConverter.ConsumedElement(tag2, 3f / 1000f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(0.6f, SimHashes.OxyRock, 303.15f, storeOutput: true)
    };
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
