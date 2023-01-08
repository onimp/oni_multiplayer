// Decompiled with JetBrains decompiler
// Type: WoodGasGeneratorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class WoodGasGeneratorConfig : IBuildingConfig
{
  public const string ID = "WoodGasGenerator";
  private const float BRANCHES_PER_GENERATOR = 8f;
  public const float CONSUMPTION_RATE = 1.2f;
  private const float WOOD_PER_REFILL = 360f;
  private const SimHashes EXHAUST_ELEMENT_GAS = SimHashes.CarbonDioxide;
  private const SimHashes EXHAUST_ELEMENT_GAS2 = SimHashes.Syngas;
  public const float CO2_EXHAUST_RATE = 0.17f;
  private const int WIDTH = 2;
  private const int HEIGHT = 2;

  public override BuildingDef CreateBuildingDef()
  {
    string[] allMetals = MATERIALS.ALL_METALS;
    float[] tieR5_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] construction_materials = allMetals;
    EffectorValues tieR5_2 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR5_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("WoodGasGenerator", 2, 2, "generatorwood_kanim", 100, 120f, tieR5_1, construction_materials, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.GeneratorWattageRating = 300f;
    buildingDef.GeneratorBaseCapacity = 20000f;
    buildingDef.ExhaustKilowattsWhenActive = 8f;
    buildingDef.SelfHeatKilowattsWhenActive = 1f;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.RequiresPowerOutput = true;
    buildingDef.PowerOutputOffset = new CellOffset(0, 0);
    return buildingDef;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<LoopingSounds>();
    Storage storage = go.AddOrGet<Storage>();
    storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    storage.showInUI = true;
    float max_stored_input_mass = 720f;
    go.AddOrGet<LoopingSounds>();
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = WoodLogConfig.TAG;
    manualDeliveryKg.capacity = 360f;
    manualDeliveryKg.refillMass = 180f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
    EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
    energyGenerator.powerDistributionOrder = 8;
    energyGenerator.hasMeter = true;
    energyGenerator.formula = EnergyGenerator.CreateSimpleFormula(WoodLogConfig.TAG, 1.2f, max_stored_input_mass, SimHashes.CarbonDioxide, 0.17f, false, new CellOffset(0, 1), 383.15f);
    Tinkerable.MakePowerTinkerable(go);
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
