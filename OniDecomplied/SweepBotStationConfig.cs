// Decompiled with JetBrains decompiler
// Type: SweepBotStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class SweepBotStationConfig : IBuildingConfig
{
  public const string ID = "SweepBotStation";
  public const float POWER_USAGE = 240f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] construction_mass = new float[1]
    {
      BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0] - SweepBotConfig.MASS
    };
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SweepBotStation", 2, 2, "sweep_bot_base_station_kanim", 30, 30f, construction_mass, refinedMetals, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.Floodable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 240f;
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.SelfHeatKilowattsWhenActive = 1f;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Prioritizable.AddRef(go);
    Storage botMaterialStorage = go.AddComponent<Storage>();
    botMaterialStorage.showInUI = true;
    botMaterialStorage.allowItemRemoval = false;
    botMaterialStorage.ignoreSourcePriority = true;
    botMaterialStorage.showDescriptor = false;
    botMaterialStorage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
    botMaterialStorage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
    botMaterialStorage.fetchCategory = Storage.FetchCategory.Building;
    botMaterialStorage.capacityKg = 25f;
    botMaterialStorage.allowClearable = false;
    Storage sweepStorage = go.AddComponent<Storage>();
    sweepStorage.showInUI = true;
    sweepStorage.allowItemRemoval = true;
    sweepStorage.ignoreSourcePriority = true;
    sweepStorage.showDescriptor = true;
    sweepStorage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
    sweepStorage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
    sweepStorage.fetchCategory = Storage.FetchCategory.StorageSweepOnly;
    sweepStorage.capacityKg = 1000f;
    sweepStorage.allowClearable = true;
    sweepStorage.showCapacityStatusItem = true;
    go.AddOrGet<CharacterOverlay>().shouldShowName = true;
    go.AddOrGet<SweepBotStation>().SetStorages(botMaterialStorage, sweepStorage);
  }

  public override void DoPostConfigureComplete(GameObject go) => go.AddOrGetDef<StorageController.Def>();
}
