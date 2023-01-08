// Decompiled with JetBrains decompiler
// Type: LiquidHeaterConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class LiquidHeaterConfig : IBuildingConfig
{
  public const string ID = "LiquidHeater";
  public const float CONSUMPTION_RATE = 1f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidHeater", 4, 1, "boiler_kanim", 30, 30f, tieR4, allMetals, 3200f, BuildLocationRule.Anywhere, tieR1, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.Floodable = false;
    buildingDef.EnergyConsumptionWhenActive = 960f;
    buildingDef.ExhaustKilowattsWhenActive = 4000f;
    buildingDef.SelfHeatKilowattsWhenActive = 64f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "SolidMetal";
    buildingDef.OverheatTemperature = 398.15f;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    SpaceHeater spaceHeater = go.AddOrGet<SpaceHeater>();
    spaceHeater.SetLiquidHeater();
    spaceHeater.targetTemperature = 358.15f;
    spaceHeater.minimumCellMass = 400f;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
