// Decompiled with JetBrains decompiler
// Type: BaseBatteryConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public abstract class BaseBatteryConfig : IBuildingConfig
{
  public BuildingDef CreateBuildingDef(
    string id,
    int width,
    int height,
    int hitpoints,
    string anim,
    float construction_time,
    float[] construction_mass,
    string[] construction_materials,
    float melting_point,
    float exhaust_temperature_active,
    float self_heat_kilowatts_active,
    EffectorValues decor,
    EffectorValues noise)
  {
    string id1 = id;
    int width1 = width;
    int height1 = height;
    int num = hitpoints;
    string anim1 = anim;
    int hitpoints1 = num;
    double construction_time1 = (double) construction_time;
    float[] construction_mass1 = construction_mass;
    string[] construction_materials1 = construction_materials;
    double melting_point1 = (double) melting_point;
    EffectorValues tieR0 = NOISE_POLLUTION.NOISY.TIER0;
    EffectorValues decor1 = decor;
    EffectorValues noise1 = tieR0;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id1, width1, height1, anim1, hitpoints1, (float) construction_time1, construction_mass1, construction_materials1, (float) melting_point1, BuildLocationRule.OnFloor, decor1, noise1);
    buildingDef.ExhaustKilowattsWhenActive = exhaust_temperature_active;
    buildingDef.SelfHeatKilowattsWhenActive = self_heat_kilowatts_active;
    buildingDef.Entombable = false;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.RequiresPowerOutput = true;
    buildingDef.UseWhitePowerOutputConnectorColour = true;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => go.AddComponent<RequireInputs>();

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<Battery>().powerSortOrder = 1000;
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
