// Decompiled with JetBrains decompiler
// Type: RefrigeratorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class RefrigeratorConfig : IBuildingConfig
{
  public const string ID = "Refrigerator";
  private const int ENERGY_SAVER_POWER = 20;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] rawMinerals = MATERIALS.RAW_MINERALS;
    EffectorValues tieR0 = NOISE_POLLUTION.NOISY.TIER0;
    EffectorValues tieR1 = TUNING.BUILDINGS.DECOR.BONUS.TIER1;
    EffectorValues noise = tieR0;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Refrigerator", 1, 2, "fridge_kanim", 30, 10f, tieR4, rawMinerals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.AddLogicPowerPort = false;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.125f;
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), (string) STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_INACTIVE)
    };
    buildingDef.Floodable = false;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    SoundEventVolumeCache.instance.AddVolume("fridge_kanim", "Refrigerator_open", NOISE_POLLUTION.NOISY.TIER1);
    SoundEventVolumeCache.instance.AddVolume("fridge_kanim", "Refrigerator_close", NOISE_POLLUTION.NOISY.TIER1);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => go.GetComponent<KPrefabID>();

  public override void DoPostConfigureComplete(GameObject go)
  {
    Storage storage = go.AddOrGet<Storage>();
    storage.showInUI = true;
    storage.showDescriptor = true;
    storage.storageFilters = STORAGEFILTERS.FOOD;
    storage.allowItemRemoval = true;
    storage.capacityKg = 100f;
    storage.storageFullMargin = TUNING.STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
    storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
    storage.showCapacityStatusItem = true;
    Prioritizable.AddRef(go);
    go.AddOrGet<TreeFilterable>();
    go.AddOrGet<FoodStorage>();
    go.AddOrGet<Refrigerator>();
    RefrigeratorController.Def def = go.AddOrGetDef<RefrigeratorController.Def>();
    def.powerSaverEnergyUsage = 20f;
    def.coolingHeatKW = 0.375f;
    def.steadyHeatKW = 0.0f;
    go.AddOrGet<UserNameable>();
    go.AddOrGet<DropAllWorkable>();
    go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
    go.AddOrGetDef<StorageController.Def>();
  }
}
