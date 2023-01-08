// Decompiled with JetBrains decompiler
// Type: AlgaeDistilleryConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class AlgaeDistilleryConfig : IBuildingConfig
{
  public const string ID = "AlgaeDistillery";
  public const float INPUT_SLIME_PER_SECOND = 0.6f;
  public const float ALGAE_PER_SECOND = 0.2f;
  public const float DIRTY_WATER_PER_SECOND = 0.400000036f;
  public const float OUTPUT_TEMP = 303.15f;
  public const float REFILL_RATE = 2400f;
  public const float ALGAE_STORAGE_AMOUNT = 480f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AlgaeDistillery", 3, 4, "algae_distillery_kanim", 100, 30f, tieR3, allMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.Overheatable = false;
    buildingDef.RequiresPowerInput = true;
    buildingDef.PowerInputOffset = new CellOffset(1, 0);
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.ExhaustKilowattsWhenActive = 0.5f;
    buildingDef.SelfHeatKilowattsWhenActive = 1f;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.OutputConduitType = ConduitType.Liquid;
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    AlgaeDistillery algaeDistillery = go.AddOrGet<AlgaeDistillery>();
    algaeDistillery.emitTag = new Tag("Algae");
    algaeDistillery.emitMass = 30f;
    ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
    conduitDispenser.conduitType = ConduitType.Liquid;
    conduitDispenser.elementFilter = new SimHashes[1]
    {
      SimHashes.DirtyWater
    };
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 1000f;
    storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    storage.showInUI = true;
    Tag tag = SimHashes.SlimeMold.CreateTag();
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = tag;
    manualDeliveryKg.refillMass = 120f;
    manualDeliveryKg.capacity = 480f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
    {
      new ElementConverter.ConsumedElement(tag, 0.6f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[2]
    {
      new ElementConverter.OutputElement(0.2f, SimHashes.Algae, 303.15f, storeOutput: true, outputElementOffsety: 1f),
      new ElementConverter.OutputElement(0.400000036f, SimHashes.DirtyWater, 303.15f, storeOutput: true)
    };
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
