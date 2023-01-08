// Decompiled with JetBrains decompiler
// Type: TravelTubeEntranceConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class TravelTubeEntranceConfig : IBuildingConfig
{
  public const string ID = "TravelTubeEntrance";
  private const float JOULES_PER_LAUNCH = 10000f;
  private const float LAUNCHES_FROM_FULL_CHARGE = 4f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("TravelTubeEntrance", 3, 2, "tube_launcher_kanim", 100, 120f, tieR5, refinedMetals, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.Overheatable = false;
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 960f;
    buildingDef.Entombable = true;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PowerInputOffset = new CellOffset(1, 0);
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    TravelTubeEntrance travelTubeEntrance = go.AddOrGet<TravelTubeEntrance>();
    travelTubeEntrance.joulesPerLaunch = 10000f;
    travelTubeEntrance.jouleCapacity = 40000f;
    go.AddOrGet<TravelTubeEntrance.Work>();
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGet<EnergyConsumerSelfSustaining>();
  }

  public override void DoPostConfigureComplete(GameObject go) => go.GetComponent<RequireInputs>().visualizeRequirements = RequireInputs.Requirements.NoWire;
}
