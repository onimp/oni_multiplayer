// Decompiled with JetBrains decompiler
// Type: WireRefinedConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class WireRefinedConfig : BaseWireConfig
{
  public const string ID = "WireRefined";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR0 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = this.CreateBuildingDef("WireRefined", "utilities_electric_conduct_kanim", 3f, tieR0, 0.05f, none2, noise);
    buildingDef.MaterialCategory = MATERIALS.REFINED_METALS;
    return buildingDef;
  }

  public override void DoPostConfigureComplete(GameObject go) => this.DoPostConfigureComplete(Wire.WattageRating.Max2000, go);
}
