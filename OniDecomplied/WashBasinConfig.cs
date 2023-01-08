// Decompiled with JetBrains decompiler
// Type: WashBasinConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class WashBasinConfig : IBuildingConfig
{
  public const string ID = "WashBasin";
  public const int DISEASE_REMOVAL_COUNT = 120000;
  public const float WATER_PER_USE = 5f;
  public const int USES_PER_FLUSH = 40;
  public const float WORK_TIME = 5f;

  public override BuildingDef CreateBuildingDef()
  {
    string[] rawMinerals = MATERIALS.RAW_MINERALS;
    float[] tieR1_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
    string[] construction_materials = rawMinerals;
    EffectorValues tieR0 = NOISE_POLLUTION.NOISY.TIER0;
    EffectorValues tieR1_2 = BUILDINGS.DECOR.BONUS.TIER1;
    EffectorValues noise = tieR0;
    return BuildingTemplates.CreateBuildingDef("WashBasin", 2, 3, "wash_basin_kanim", 30, 30f, tieR1_1, construction_materials, 1600f, BuildLocationRule.OnFloor, tieR1_2, noise);
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.WashStation, false);
    HandSanitizer handSanitizer = go.AddOrGet<HandSanitizer>();
    handSanitizer.massConsumedPerUse = 5f;
    handSanitizer.consumedElement = SimHashes.Water;
    handSanitizer.outputElement = SimHashes.DirtyWater;
    handSanitizer.diseaseRemovalCount = 120000;
    handSanitizer.maxUses = 40;
    handSanitizer.dumpWhenFull = true;
    go.AddOrGet<DirectionControl>();
    HandSanitizer.Work work = go.AddOrGet<HandSanitizer.Work>();
    work.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_washbasin_kanim"))
    };
    work.workTime = 5f;
    work.trackUses = true;
    Storage storage = go.AddOrGet<Storage>();
    storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = GameTagExtensions.Create(SimHashes.Water);
    manualDeliveryKg.MinimumMass = 5f;
    manualDeliveryKg.capacity = 200f;
    manualDeliveryKg.refillMass = 40f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
    go.AddOrGet<LoopingSounds>();
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
