// Decompiled with JetBrains decompiler
// Type: RustDeoxidizerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class RustDeoxidizerConfig : IBuildingConfig
{
  public const string ID = "RustDeoxidizer";
  private const float RUST_KG_CONSUMPTION_RATE = 0.75f;
  private const float SALT_KG_CONSUMPTION_RATE = 0.25f;
  private const float RUST_KG_PER_REFILL = 585f;
  private const float SALT_KG_PER_REFILL = 195f;
  private const float TOTAL_CONSUMPTION_RATE = 1f;
  private const float IRON_CONVERSION_RATIO = 0.4f;
  private const float OXYGEN_CONVERSION_RATIO = 0.57f;
  private const float CHLORINE_CONVERSION_RATIO = 0.0299999714f;
  public const float OXYGEN_TEMPERATURE = 348.15f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = tieR3_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RustDeoxidizer", 2, 3, "rust_deoxidizer_kanim", 30, 30f, tieR3_1, allMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.PowerInputOffset = new CellOffset(1, 0);
    buildingDef.EnergyConsumptionWhenActive = 60f;
    buildingDef.ExhaustKilowattsWhenActive = 0.125f;
    buildingDef.SelfHeatKilowattsWhenActive = 1f;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
    buildingDef.ViewMode = OverlayModes.Oxygen.ID;
    buildingDef.AudioCategory = "HollowMetal";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<RustDeoxidizer>().maxMass = 1.8f;
    Storage storage = go.AddOrGet<Storage>();
    storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    storage.showInUI = true;
    ManualDeliveryKG manualDeliveryKg1 = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg1.SetStorage(storage);
    manualDeliveryKg1.RequestedItemTag = new Tag("Rust");
    manualDeliveryKg1.capacity = 585f;
    manualDeliveryKg1.refillMass = 193.05f;
    manualDeliveryKg1.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
    ManualDeliveryKG manualDeliveryKg2 = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg2.SetStorage(storage);
    manualDeliveryKg2.RequestedItemTag = new Tag("Salt");
    manualDeliveryKg2.capacity = 195f;
    manualDeliveryKg2.refillMass = 64.3500061f;
    manualDeliveryKg2.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
    {
      new ElementConverter.ConsumedElement(new Tag("Rust"), 0.75f),
      new ElementConverter.ConsumedElement(new Tag("Salt"), 0.25f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[3]
    {
      new ElementConverter.OutputElement(0.57f, SimHashes.Oxygen, 348.15f, outputElementOffsety: 1f),
      new ElementConverter.OutputElement(0.0299999714f, SimHashes.ChlorineGas, 348.15f, outputElementOffsety: 1f),
      new ElementConverter.OutputElement(0.4f, SimHashes.IronOre, 348.15f, storeOutput: true, outputElementOffsety: 1f)
    };
    ElementDropper elementDropper = go.AddComponent<ElementDropper>();
    elementDropper.emitMass = 24f;
    elementDropper.emitTag = SimHashes.IronOre.CreateTag();
    elementDropper.emitOffset = new Vector3(0.0f, 1f, 0.0f);
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
