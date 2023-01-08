// Decompiled with JetBrains decompiler
// Type: EthanolDistilleryConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class EthanolDistilleryConfig : IBuildingConfig
{
  public const string ID = "EthanolDistillery";
  public const float ORGANICS_CONSUME_PER_SECOND = 1f;
  public const float ORGANICS_STORAGE_AMOUNT = 600f;
  public const float ETHANOL_RATE = 0.5f;
  public const float SOLID_WASTE_RATE = 0.333333343f;
  public const float CO2_WASTE_RATE = 0.166666672f;
  public const float OUTPUT_TEMPERATURE = 346.5f;
  public const float WASTE_OUTPUT_TEMPERATURE = 366.5f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("EthanolDistillery", 4, 3, "ethanoldistillery_kanim", 100, 30f, tieR3, allMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.Overheatable = false;
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 240f;
    buildingDef.ExhaustKilowattsWhenActive = 0.5f;
    buildingDef.SelfHeatKilowattsWhenActive = 4f;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.OutputConduitType = ConduitType.Liquid;
    buildingDef.PowerInputOffset = new CellOffset(2, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(-1, 0);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
    conduitDispenser.conduitType = ConduitType.Liquid;
    conduitDispenser.elementFilter = new SimHashes[1]
    {
      SimHashes.Ethanol
    };
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 1000f;
    storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    storage.showInUI = true;
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = WoodLogConfig.TAG;
    manualDeliveryKg.capacity = 600f;
    manualDeliveryKg.refillMass = 150f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
    {
      new ElementConverter.ConsumedElement(WoodLogConfig.TAG, 1f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[3]
    {
      new ElementConverter.OutputElement(0.5f, SimHashes.Ethanol, 346.5f, storeOutput: true),
      new ElementConverter.OutputElement(0.333333343f, SimHashes.ToxicSand, 366.5f, storeOutput: true),
      new ElementConverter.OutputElement(0.166666672f, SimHashes.CarbonDioxide, 366.5f)
    };
    AlgaeDistillery algaeDistillery = go.AddOrGet<AlgaeDistillery>();
    algaeDistillery.emitMass = 20f;
    algaeDistillery.emitTag = new Tag("ToxicSand");
    algaeDistillery.emitOffset = new Vector3(2f, 1f);
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
