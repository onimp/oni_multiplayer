// Decompiled with JetBrains decompiler
// Type: BottleEmptierGasConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class BottleEmptierGasConfig : IBuildingConfig
{
  public const string ID = "BottleEmptierGas";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR2_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
    EffectorValues tieR2_2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("BottleEmptierGas", 1, 3, "gas_emptying_station_kanim", 30, 60f, tieR2_1, refinedMetals, 1600f, BuildLocationRule.OnFloor, tieR2_2, noise);
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
    storage.storageFilters = STORAGEFILTERS.GASES;
    storage.showInUI = true;
    storage.showDescriptor = true;
    storage.capacityKg = 200f;
    go.AddOrGet<TreeFilterable>();
    BottleEmptier bottleEmptier = go.AddOrGet<BottleEmptier>();
    bottleEmptier.isGasEmptier = true;
    bottleEmptier.emptyRate = 0.25f;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
