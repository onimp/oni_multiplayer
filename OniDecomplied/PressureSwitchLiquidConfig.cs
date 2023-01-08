// Decompiled with JetBrains decompiler
// Type: PressureSwitchLiquidConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class PressureSwitchLiquidConfig : IBuildingConfig
{
  public static string ID = "PressureSwitchLiquid";

  public override BuildingDef CreateBuildingDef()
  {
    string id = PressureSwitchLiquidConfig.ID;
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR0 = BUILDINGS.DECOR.PENALTY.TIER0;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 1, 1, "switchliquidpressure_kanim", 30, 30f, tieR3, allMetals, 1600f, BuildLocationRule.Anywhere, tieR0, noise);
    buildingDef.Deprecated = true;
    buildingDef.Overheatable = false;
    buildingDef.Floodable = false;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    SoundEventVolumeCache.instance.AddVolume("switchliquidpressure_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
    SoundEventVolumeCache.instance.AddVolume("switchliquidpressure_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    GeneratedBuildings.MakeBuildingAlwaysOperational(go);
    PressureSwitch pressureSwitch = go.AddOrGet<PressureSwitch>();
    pressureSwitch.objectLayer = ObjectLayer.Wire;
    pressureSwitch.rangeMin = 0.0f;
    pressureSwitch.rangeMax = 2000f;
    pressureSwitch.Threshold = 500f;
    pressureSwitch.ActivateAboveThreshold = false;
    pressureSwitch.manuallyControlled = false;
    pressureSwitch.desiredState = Element.State.Liquid;
  }

  public override void DoPostConfigureComplete(GameObject go) => go.AddComponent<BuildingCellVisualizer>();
}
