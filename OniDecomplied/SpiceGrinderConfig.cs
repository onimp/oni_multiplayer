// Decompiled with JetBrains decompiler
// Type: SpiceGrinderConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SpiceGrinderConfig : IBuildingConfig
{
  public const string ID = "SpiceGrinder";
  public static Tag MATERIAL_FOR_TINKER = GameTags.CropSeed;
  public static Tag TINKER_TOOLS = FarmStationToolsConfig.tag;
  public const float MASS_PER_TINKER = 5f;
  public const float OUTPUT_TEMPERATURE = 313.15f;
  public const float WORK_TIME_PER_1000KCAL = 5f;
  public const short SPICE_CAPACITY_PER_INGREDIENT = 10;
  public const string PrimaryColorSymbol = "stripe_anim2";
  public const string SecondaryColorSymbol = "stripe_anim1";
  public const string GrinderColorSymbol = "grinder";
  public static StatusItem SpicedStatus = Db.Get().MiscStatusItems.SpicedFood;
  private static int STORAGE_PRIORITY = Chore.DefaultPrioritySetting.priority_value - 1;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SpiceGrinder", 2, 3, "spice_grinder_kanim", 30, 30f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.ViewMode = OverlayModes.Rooms.ID;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "large";
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.SpiceStation, false);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    SpiceGrinder.InitializeSpices();
    SymbolOverrideControllerUtil.AddToPrefab(go);
    go.AddOrGetDef<SpiceGrinder.Def>();
    go.AddOrGet<SpiceGrinderWorkable>();
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGet<TreeFilterable>().uiHeight = TreeFilterable.UISideScreenHeight.Short;
    go.AddOrGet<Prioritizable>().SetMasterPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, SpiceGrinderConfig.STORAGE_PRIORITY));
    Storage storage1 = go.AddComponent<Storage>();
    storage1.showInUI = true;
    storage1.showDescriptor = true;
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(GameTags.Edible);
    storage1.storageFilters = tagList1;
    storage1.allowItemRemoval = false;
    storage1.capacityKg = 1f;
    storage1.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
    storage1.fetchCategory = Storage.FetchCategory.Building;
    storage1.showCapacityStatusItem = false;
    storage1.allowSettingOnlyFetchMarkedItems = false;
    storage1.showSideScreenTitleBar = true;
    Storage storage2 = go.AddComponent<Storage>();
    storage2.showInUI = true;
    storage2.showDescriptor = true;
    Storage storage3 = storage2;
    List<Tag> tagList2 = new List<Tag>();
    tagList2.Add(GameTags.Seed);
    storage3.storageFilters = tagList2;
    storage2.allowItemRemoval = false;
    storage2.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
    storage2.fetchCategory = Storage.FetchCategory.Building;
    storage2.showCapacityStatusItem = true;
    storage2.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage2);
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.CookFetch.IdHash;
    RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.Kitchen.Id;
    roomTracker.requirement = RoomTracker.Requirement.Required;
  }
}
