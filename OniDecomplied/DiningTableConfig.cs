// Decompiled with JetBrains decompiler
// Type: DiningTableConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class DiningTableConfig : IBuildingConfig
{
  public const string ID = "DiningTable";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.BONUS.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("DiningTable", 1, 1, "diningtable_kanim", 10, 10f, tieR3, allMetals, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.WorkTime = 20f;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.MessTable, false);
    go.AddOrGet<MessStation>();
    go.AddOrGet<AnimTileable>();
    go.AddOrGetDef<RocketUsageRestriction.Def>();
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.GetComponent<KAnimControllerBase>().initialAnim = "off";
    go.AddOrGet<Ownable>().slotID = Db.Get().AssignableSlots.MessStation.Id;
    Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
    defaultStorage.showInUI = true;
    defaultStorage.capacityKg = TableSaltTuning.SALTSHAKERSTORAGEMASS;
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(defaultStorage);
    manualDeliveryKg.RequestedItemTag = TagExtensions.ToTag(TableSaltConfig.ID);
    manualDeliveryKg.capacity = TableSaltTuning.SALTSHAKERSTORAGEMASS;
    manualDeliveryKg.refillMass = TableSaltTuning.CONSUMABLE_RATE;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FoodFetch.IdHash;
    manualDeliveryKg.ShowStatusItem = false;
  }
}
