// Decompiled with JetBrains decompiler
// Type: HeatCompressorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class HeatCompressorConfig : IBuildingConfig
{
  public const string ID = "HeatCompressor";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR7 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER7;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR5 = TUNING.BUILDINGS.DECOR.BONUS.TIER5;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("HeatCompressor", 4, 4, "hqbase_kanim", 250, 30f, tieR7, allMetals, 1600f, BuildLocationRule.OnFloor, tieR5, noise);
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.BaseTimeUntilRepair = 400f;
    buildingDef.DefaultAnimState = "idle";
    buildingDef.RequiresPowerInput = true;
    buildingDef.PowerInputOffset = new CellOffset(2, 0);
    buildingDef.EnergyConsumptionWhenActive = 1600f;
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.OutputConduitType = ConduitType.Liquid;
    buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(2, 0);
    buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(-1, 1), (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE)
    };
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_LP", NOISE_POLLUTION.NOISY.TIER3);
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_open", NOISE_POLLUTION.NOISY.TIER4);
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_close", NOISE_POLLUTION.NOISY.TIER4);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Storage inputStorage = go.AddComponent<Storage>();
    inputStorage.showDescriptor = false;
    inputStorage.showInUI = true;
    inputStorage.storageFilters = STORAGEFILTERS.LIQUIDS;
    inputStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    Storage outputStorage = go.AddComponent<Storage>();
    outputStorage.showDescriptor = false;
    outputStorage.showInUI = true;
    outputStorage.storageFilters = STORAGEFILTERS.LIQUIDS;
    outputStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    Storage heatCubeStorage = go.AddComponent<Storage>();
    heatCubeStorage.showDescriptor = false;
    heatCubeStorage.showInUI = true;
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.capacityKG = 100f;
    conduitConsumer.storage = inputStorage;
    ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
    conduitDispenser.conduitType = ConduitType.Liquid;
    conduitDispenser.storage = outputStorage;
    conduitDispenser.alwaysDispense = true;
    go.AddOrGet<HeatCompressor>().SetStorage(inputStorage, outputStorage, heatCubeStorage);
  }

  public override void DoPostConfigureComplete(GameObject go) => go.AddOrGet<LogicOperationalController>();
}
