// Decompiled with JetBrains decompiler
// Type: OuthouseConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class OuthouseConfig : IBuildingConfig
{
  public const string ID = "Outhouse";
  private const int USES_PER_REFILL = 15;
  private const float DIRT_PER_REFILL = 200f;
  private const float DIRT_PER_USE = 13f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] rawMinerals = MATERIALS.RAW_MINERALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR4 = BUILDINGS.DECOR.PENALTY.TIER4;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Outhouse", 2, 3, "outhouse_kanim", 30, 30f, tieR3, rawMinerals, 800f, BuildLocationRule.OnFloor, tieR4, noise);
    buildingDef.Overheatable = false;
    buildingDef.ExhaustKilowattsWhenActive = 0.25f;
    buildingDef.DiseaseCellVisName = "FoodPoisoning";
    buildingDef.AudioCategory = "Metal";
    SoundEventVolumeCache.instance.AddVolume("outhouse_kanim", "Latrine_door_open", NOISE_POLLUTION.NOISY.TIER1);
    SoundEventVolumeCache.instance.AddVolume("outhouse_kanim", "Latrine_door_close", NOISE_POLLUTION.NOISY.TIER1);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.ToiletType, false);
    Toilet toilet = go.AddOrGet<Toilet>();
    toilet.maxFlushes = 15;
    toilet.dirtUsedPerFlush = 13f;
    toilet.solidWastePerUse = new Toilet.SpawnInfo(SimHashes.ToxicSand, 6.7f, 0.0f);
    toilet.solidWasteTemperature = 310.15f;
    toilet.diseaseId = "FoodPoisoning";
    toilet.diseasePerFlush = 100000;
    toilet.diseaseOnDupePerFlush = 100000;
    KAnimFile[] kanimFileArray = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_outhouse_kanim"))
    };
    ToiletWorkableUse toiletWorkableUse = go.AddOrGet<ToiletWorkableUse>();
    toiletWorkableUse.overrideAnims = kanimFileArray;
    toiletWorkableUse.workLayer = Grid.SceneLayer.BuildingFront;
    ToiletWorkableClean toiletWorkableClean = go.AddOrGet<ToiletWorkableClean>();
    toiletWorkableClean.workTime = 90f;
    toiletWorkableClean.overrideAnims = kanimFileArray;
    toiletWorkableClean.workLayer = Grid.SceneLayer.BuildingFront;
    Prioritizable.AddRef(go);
    Storage storage = go.AddOrGet<Storage>();
    storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    storage.showInUI = true;
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = new Tag("Dirt");
    manualDeliveryKg.capacity = 200f;
    manualDeliveryKg.refillMass = 0.01f;
    manualDeliveryKg.MinimumMass = 200f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
    manualDeliveryKg.operationalRequirement = Operational.State.Functional;
    Ownable ownable = go.AddOrGet<Ownable>();
    ownable.slotID = Db.Get().AssignableSlots.Toilet.Id;
    ownable.canBePublic = true;
    go.AddOrGetDef<RocketUsageRestriction.Def>();
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
