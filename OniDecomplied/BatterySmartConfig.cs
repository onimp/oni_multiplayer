// Decompiled with JetBrains decompiler
// Type: BatterySmartConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BatterySmartConfig : BaseBatteryConfig
{
  public const string ID = "BatterySmart";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
    EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR1;
    BuildingDef buildingDef = this.CreateBuildingDef("BatterySmart", 2, 2, 30, "smartbattery_kanim", 60f, tieR3, refinedMetals, 800f, 0.0f, 0.5f, tieR2, noise);
    SoundEventVolumeCache.instance.AddVolume("batterymed_kanim", "Battery_med_rattle", NOISE_POLLUTION.NOISY.TIER2);
    buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.OutputPort(BatterySmart.PORT_ID, new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT_INACTIVE, true)
    };
    return buildingDef;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    BatterySmart batterySmart = go.AddOrGet<BatterySmart>();
    batterySmart.capacity = 20000f;
    batterySmart.joulesLostPerSecond = 0.6666667f;
    batterySmart.powerSortOrder = 1000;
    base.DoPostConfigureComplete(go);
  }
}
