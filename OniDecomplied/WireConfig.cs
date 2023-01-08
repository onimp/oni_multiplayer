// Decompiled with JetBrains decompiler
// Type: WireConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class WireConfig : BaseWireConfig
{
  public const string ID = "Wire";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR0_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR0_2 = BUILDINGS.DECOR.PENALTY.TIER0;
    EffectorValues noise = none;
    return this.CreateBuildingDef("Wire", "utilities_electric_kanim", 3f, tieR0_1, 0.05f, tieR0_2, noise);
  }

  public override void DoPostConfigureComplete(GameObject go) => this.DoPostConfigureComplete(Wire.WattageRating.Max1000, go);
}
