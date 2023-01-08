// Decompiled with JetBrains decompiler
// Type: BottleEmptierConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class BottleEmptierConfig : IBuildingConfig
{
  public const string ID = "BottleEmptier";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] rawMinerals = MATERIALS.RAW_MINERALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("BottleEmptier", 1, 3, "liquidator_kanim", 30, 10f, tieR4, rawMinerals, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.Floodable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.Overheatable = false;
    buildingDef.PermittedRotations = PermittedRotations.FlipH;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Prioritizable.AddRef(go);
    Storage storage = go.AddOrGet<Storage>();
    storage.storageFilters = STORAGEFILTERS.LIQUIDS;
    storage.showInUI = true;
    storage.showDescriptor = true;
    storage.capacityKg = 200f;
    go.AddOrGet<TreeFilterable>();
    go.AddOrGet<BottleEmptier>();
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
