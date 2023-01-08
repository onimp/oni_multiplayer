// Decompiled with JetBrains decompiler
// Type: WireHighWattageConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class WireHighWattageConfig : BaseWireConfig
{
  public const string ID = "HighWattageWire";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR5 = BUILDINGS.DECOR.PENALTY.TIER5;
    EffectorValues noise = none;
    BuildingDef buildingDef = this.CreateBuildingDef("HighWattageWire", "utilities_electric_insulated_kanim", 3f, tieR2, 0.05f, tieR5, noise);
    buildingDef.BuildLocationRule = BuildLocationRule.NotInTiles;
    return buildingDef;
  }

  public override void DoPostConfigureComplete(GameObject go) => this.DoPostConfigureComplete(Wire.WattageRating.Max20000, go);

  public override void DoPostConfigureUnderConstruction(GameObject go) => base.DoPostConfigureUnderConstruction(go);
}
