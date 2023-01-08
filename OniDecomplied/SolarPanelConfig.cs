// Decompiled with JetBrains decompiler
// Type: SolarPanelConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class SolarPanelConfig : IBuildingConfig
{
  public const string ID = "SolarPanel";
  public const float WATTS_PER_LUX = 0.00053f;
  public const float MAX_WATTS = 380f;
  private const int WIDTH = 7;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] glasses = MATERIALS.GLASSES;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolarPanel", 7, 3, "solar_panel_kanim", 100, 120f, tieR3, glasses, 2400f, BuildLocationRule.Anywhere, tieR2, noise);
    buildingDef.GeneratorWattageRating = 380f;
    buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
    buildingDef.BuildLocationRule = BuildLocationRule.Anywhere;
    buildingDef.HitPoints = 10;
    buildingDef.RequiresPowerOutput = true;
    buildingDef.PowerOutputOffset = new CellOffset(0, 0);
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.AudioSize = "large";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<LoopingSounds>();
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<Repairable>().expectedRepairTime = 52.5f;
    go.AddOrGet<SolarPanel>().powerDistributionOrder = 9;
    go.AddOrGetDef<PoweredActiveController.Def>();
    MakeBaseSolid.Def def = go.AddOrGetDef<MakeBaseSolid.Def>();
    def.occupyFoundationLayer = false;
    def.solidOffsets = new CellOffset[7];
    for (int index = 0; index < 7; ++index)
      def.solidOffsets[index] = new CellOffset(index - 3, 0);
  }
}
