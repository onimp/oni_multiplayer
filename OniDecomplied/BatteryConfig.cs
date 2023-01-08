// Decompiled with JetBrains decompiler
// Type: BatteryConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class BatteryConfig : BaseBatteryConfig
{
  public const string ID = "Battery";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = this.CreateBuildingDef("Battery", 1, 2, 30, "batterysm_kanim", 30f, tieR3, allMetals, 800f, 0.25f, 1f, tieR1, noise);
    buildingDef.Breakable = true;
    SoundEventVolumeCache.instance.AddVolume("batterysm_kanim", "Battery_rattle", NOISE_POLLUTION.NOISY.TIER1);
    return buildingDef;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    Battery battery = go.AddOrGet<Battery>();
    battery.capacity = 10000f;
    battery.joulesLostPerSecond = 1.66666663f;
    base.DoPostConfigureComplete(go);
  }
}
