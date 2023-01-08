// Decompiled with JetBrains decompiler
// Type: PetroleumGeneratorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class PetroleumGeneratorConfig : IBuildingConfig
{
  public const string ID = "PetroleumGenerator";
  public const float CONSUMPTION_RATE = 2f;
  private const SimHashes INPUT_ELEMENT = SimHashes.Petroleum;
  private const SimHashes EXHAUST_ELEMENT_GAS = SimHashes.CarbonDioxide;
  private const SimHashes EXHAUST_ELEMENT_LIQUID = SimHashes.DirtyWater;
  public const float EFFICIENCY_RATE = 0.5f;
  public const float EXHAUST_GAS_RATE = 0.5f;
  public const float EXHAUST_LIQUID_RATE = 0.75f;
  private const int WIDTH = 3;
  private const int HEIGHT = 4;

  public override BuildingDef CreateBuildingDef()
  {
    string[] strArray = new string[1]{ "Metal" };
    float[] construction_mass = new float[1]
    {
      BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0]
    };
    string[] construction_materials = strArray;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("PetroleumGenerator", 3, 4, "generatorpetrol_kanim", 100, 480f, construction_mass, construction_materials, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.GeneratorWattageRating = 2000f;
    buildingDef.GeneratorBaseCapacity = 2000f;
    buildingDef.ExhaustKilowattsWhenActive = 4f;
    buildingDef.SelfHeatKilowattsWhenActive = 16f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
    buildingDef.RequiresPowerOutput = true;
    buildingDef.PowerOutputOffset = new CellOffset(1, 0);
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
    buildingDef.InputConduitType = ConduitType.Liquid;
    return buildingDef;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<LoopingSounds>();
    go.AddOrGet<Storage>();
    BuildingDef def = go.GetComponent<Building>().Def;
    float max_stored_mass = 20f;
    go.AddOrGet<LoopingSounds>();
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = def.InputConduitType;
    conduitConsumer.consumptionRate = 10f;
    conduitConsumer.capacityTag = GameTags.CombustibleLiquid;
    conduitConsumer.capacityKG = max_stored_mass;
    conduitConsumer.forceAlwaysSatisfied = true;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
    EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
    energyGenerator.powerDistributionOrder = 8;
    energyGenerator.ignoreBatteryRefillPercent = true;
    energyGenerator.hasMeter = true;
    energyGenerator.formula = new EnergyGenerator.Formula()
    {
      inputs = new EnergyGenerator.InputItem[1]
      {
        new EnergyGenerator.InputItem(GameTags.CombustibleLiquid, 2f, max_stored_mass)
      },
      outputs = new EnergyGenerator.OutputItem[2]
      {
        new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.5f, false, new CellOffset(0, 3), 383.15f),
        new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.75f, false, new CellOffset(1, 1), 313.15f)
      }
    };
    Tinkerable.MakePowerTinkerable(go);
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
