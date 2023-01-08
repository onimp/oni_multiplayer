// Decompiled with JetBrains decompiler
// Type: OilWellCapConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class OilWellCapConfig : IBuildingConfig
{
  private const float WATER_INTAKE_RATE = 1f;
  private const float WATER_TO_OIL_RATIO = 3.33333325f;
  private const float LIQUID_STORAGE = 10f;
  private const float GAS_RATE = 0.0333333351f;
  private const float OVERPRESSURE_TIME = 2400f;
  private const float PRESSURE_RELEASE_TIME = 180f;
  private const float PRESSURE_RELEASE_RATE = 0.444444478f;
  private static readonly Tag INPUT_WATER_TAG = SimHashes.Water.CreateTag();
  public const string ID = "OilWellCap";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("OilWellCap", 4, 4, "geyser_oil_cap_kanim", 100, 120f, tieR3, refinedMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
    buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.EnergyConsumptionWhenActive = 240f;
    buildingDef.SelfHeatKilowattsWhenActive = 2f;
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.UtilityInputOffset = new CellOffset(0, 1);
    buildingDef.PowerInputOffset = new CellOffset(1, 1);
    buildingDef.OverheatTemperature = 2273.15f;
    buildingDef.Floodable = false;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
    buildingDef.AttachmentSlotTag = GameTags.OilWell;
    buildingDef.BuildLocationRule = BuildLocationRule.BuildingAttachPoint;
    buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    BuildingTemplates.CreateDefaultStorage(go).showInUI = true;
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.consumptionRate = 2f;
    conduitConsumer.capacityKG = 10f;
    conduitConsumer.capacityTag = OilWellCapConfig.INPUT_WATER_TAG;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
    {
      new ElementConverter.ConsumedElement(OilWellCapConfig.INPUT_WATER_TAG, 1f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(3.33333325f, SimHashes.CrudeOil, 363.15f, outputElementOffsetx: 2f, outputElementOffsety: 1.5f, diseaseWeight: 0.0f)
    };
    OilWellCap oilWellCap = go.AddOrGet<OilWellCap>();
    oilWellCap.gasElement = SimHashes.Methane;
    oilWellCap.gasTemperature = 573.15f;
    oilWellCap.addGasRate = 0.0333333351f;
    oilWellCap.maxGasPressure = 80.00001f;
    oilWellCap.releaseGasRate = 0.444444478f;
  }

  public override void DoPostConfigureComplete(GameObject go) => go.AddOrGet<LogicOperationalController>();
}
