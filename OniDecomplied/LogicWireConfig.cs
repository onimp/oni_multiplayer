// Decompiled with JetBrains decompiler
// Type: LogicWireConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class LogicWireConfig : BaseLogicWireConfig
{
  public const string ID = "LogicWire";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tierTiny = BUILDINGS.CONSTRUCTION_MASS_KG.TIER_TINY;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR0 = BUILDINGS.DECOR.PENALTY.TIER0;
    EffectorValues noise = none;
    return this.CreateBuildingDef("LogicWire", "logic_wires_kanim", 3f, tierTiny, tieR0, noise);
  }

  public override void DoPostConfigureComplete(GameObject go) => this.DoPostConfigureComplete(LogicWire.BitDepth.OneBit, go);
}
