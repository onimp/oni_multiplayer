// Decompiled with JetBrains decompiler
// Type: WallToiletConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class WallToiletConfig : IBuildingConfig
{
  private const float WATER_USAGE = 2.5f;
  public const string ID = "WallToilet";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    string[] plastics = MATERIALS.PLASTICS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("WallToilet", 1, 3, "toilet_wall_kanim", 30, 30f, tieR2, plastics, 800f, BuildLocationRule.WallFloor, tieR1, noise);
    buildingDef.Overheatable = false;
    buildingDef.ExhaustKilowattsWhenActive = 0.25f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.DiseaseCellVisName = "FoodPoisoning";
    buildingDef.UtilityOutputOffset = new CellOffset(-2, 0);
    buildingDef.AudioCategory = "Metal";
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.PermittedRotations = PermittedRotations.FlipH;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.ToiletType, false);
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.FlushToiletType, false);
    FlushToilet flushToilet = go.AddOrGet<FlushToilet>();
    flushToilet.massConsumedPerUse = 2.5f;
    flushToilet.massEmittedPerUse = 9.2f;
    flushToilet.newPeeTemperature = 310.15f;
    flushToilet.diseaseId = "FoodPoisoning";
    flushToilet.diseasePerFlush = 100000;
    flushToilet.diseaseOnDupePerFlush = 20000;
    flushToilet.requireOutput = false;
    flushToilet.meterOffset = Meter.Offset.Infront;
    KAnimFile[] kanimFileArray = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_toilet_wall_kanim"))
    };
    ToiletWorkableUse toiletWorkableUse = go.AddOrGet<ToiletWorkableUse>();
    toiletWorkableUse.overrideAnims = kanimFileArray;
    toiletWorkableUse.workLayer = Grid.SceneLayer.BuildingUse;
    toiletWorkableUse.resetProgressOnStop = true;
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
    conduitConsumer.capacityKG = 2.5f;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
    AutoStorageDropper.Def def = go.AddOrGetDef<AutoStorageDropper.Def>();
    def.dropOffset = new CellOffset(-2, 0);
    def.elementFilter = new SimHashes[1]{ SimHashes.Water };
    def.invertElementFilter = true;
    def.blockedBySubstantialLiquid = true;
    def.fxOffset = new Vector3(0.5f, 0.0f, 0.0f);
    def.leftFx = new AutoStorageDropper.DropperFxConfig()
    {
      animFile = "liquidleak_kanim",
      animName = "side",
      flipX = true,
      layer = Grid.SceneLayer.BuildingBack
    };
    def.rightFx = new AutoStorageDropper.DropperFxConfig()
    {
      animFile = "liquidleak_kanim",
      animName = "side",
      flipX = false,
      layer = Grid.SceneLayer.BuildingBack
    };
    def.delay = 0.0f;
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 12.5f;
    storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    Ownable ownable = go.AddOrGet<Ownable>();
    ownable.slotID = Db.Get().AssignableSlots.Toilet.Id;
    ownable.canBePublic = true;
    go.AddOrGetDef<RocketUsageRestriction.Def>();
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
