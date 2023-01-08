// Decompiled with JetBrains decompiler
// Type: JuicerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class JuicerConfig : IBuildingConfig
{
  public const string ID = "Juicer";
  public const float BERRY_CALS = 600000f;
  public const float MUSHROOM_CALS = 300000f;
  public const float LICE_CALS = 500000f;
  public const float WATER_MASS_PER_USE = 1f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.BONUS.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Juicer", 3, 4, "juicer_kanim", 30, 10f, tieR4, rawMetals, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.Floodable = true;
    buildingDef.AudioCategory = "Metal";
    buildingDef.Overheatable = true;
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.UtilityInputOffset = new CellOffset(1, 1);
    buildingDef.RequiresPowerInput = true;
    buildingDef.PowerInputOffset = new CellOffset(1, 0);
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding, false);
    Storage storage = go.AddOrGet<Storage>();
    storage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
    conduitConsumer.capacityKG = 2f;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
    ManualDeliveryKG manualDeliveryKg1 = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg1.SetStorage(storage);
    manualDeliveryKg1.RequestedItemTag = TagExtensions.ToTag(MushroomConfig.ID);
    manualDeliveryKg1.capacity = 10f;
    manualDeliveryKg1.refillMass = 5f;
    manualDeliveryKg1.MinimumMass = 1f;
    manualDeliveryKg1.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    ManualDeliveryKG manualDeliveryKg2 = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg2.SetStorage(storage);
    manualDeliveryKg2.RequestedItemTag = TagExtensions.ToTag(PrickleFruitConfig.ID);
    manualDeliveryKg2.capacity = 10f;
    manualDeliveryKg2.refillMass = 5f;
    manualDeliveryKg2.MinimumMass = 1f;
    manualDeliveryKg2.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    ManualDeliveryKG manualDeliveryKg3 = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg3.SetStorage(storage);
    manualDeliveryKg3.RequestedItemTag = TagExtensions.ToTag("BasicPlantFood");
    manualDeliveryKg3.capacity = 10f;
    manualDeliveryKg3.refillMass = 5f;
    manualDeliveryKg3.MinimumMass = 1f;
    manualDeliveryKg3.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    go.AddOrGet<JuicerWorkable>().basePriority = RELAXATION.PRIORITY.TIER5;
    EdiblesManager.FoodInfo foodInfo1 = EdiblesManager.GetFoodInfo(MushroomConfig.ID);
    EdiblesManager.FoodInfo foodInfo2 = EdiblesManager.GetFoodInfo(PrickleFruitConfig.ID);
    EdiblesManager.FoodInfo foodInfo3 = EdiblesManager.GetFoodInfo("BasicPlantFood");
    Juicer juicer = go.AddOrGet<Juicer>();
    juicer.ingredientTags = new Tag[3]
    {
      TagExtensions.ToTag(MushroomConfig.ID),
      TagExtensions.ToTag(PrickleFruitConfig.ID),
      TagExtensions.ToTag("BasicPlantFood")
    };
    juicer.ingredientMassesPerUse = new float[3]
    {
      300000f / foodInfo1.CaloriesPerUnit,
      600000f / foodInfo2.CaloriesPerUnit,
      500000f / foodInfo3.CaloriesPerUnit
    };
    juicer.specificEffect = "Juicer";
    juicer.trackingEffect = "RecentlyRecDrink";
    juicer.waterMassPerUse = 1f;
    RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
    roomTracker.requirement = RoomTracker.Requirement.Recommended;
    go.AddOrGetDef<RocketUsageRestriction.Def>();
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
