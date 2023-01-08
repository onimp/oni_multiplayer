// Decompiled with JetBrains decompiler
// Type: BaseModularLaunchpadPortConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BaseModularLaunchpadPortConfig
{
  public static Tag LinkTag = new Tag("ModularLaunchpadPort");

  public static BuildingDef CreateBaseLaunchpadPort(
    string id,
    string anim,
    ConduitType conduitType,
    bool isLoader,
    int width = 2,
    int height = 3)
  {
    string id1 = id;
    int width1 = width;
    int height1 = height;
    string anim1 = anim;
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id1, width1, height1, anim1, 1000, 60f, tieR4, refinedMetals, 9999f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
    buildingDef.OverheatTemperature = 2273.15f;
    buildingDef.Floodable = false;
    switch (conduitType)
    {
      case ConduitType.Gas:
        buildingDef.ViewMode = OverlayModes.GasConduits.ID;
        break;
      case ConduitType.Liquid:
        buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
        break;
      case ConduitType.Solid:
        buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
        break;
    }
    if (isLoader)
    {
      buildingDef.InputConduitType = conduitType;
      buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    }
    else
    {
      buildingDef.OutputConduitType = conduitType;
      buildingDef.UtilityOutputOffset = new CellOffset(1, 2);
    }
    buildingDef.PowerInputOffset = new CellOffset(1, 0);
    buildingDef.EnergyConsumptionWhenActive = 240f;
    buildingDef.SelfHeatKilowattsWhenActive = 1f;
    buildingDef.ExhaustKilowattsWhenActive = 0.25f;
    buildingDef.ObjectLayer = ObjectLayer.Building;
    buildingDef.RequiresPowerInput = true;
    buildingDef.DefaultAnimState = "idle";
    buildingDef.CanMove = false;
    return buildingDef;
  }

  public static void ConfigureBuildingTemplate(
    GameObject go,
    Tag prefab_tag,
    ConduitType conduitType,
    float storageSize,
    bool isLoader)
  {
    go.AddOrGet<LoopingSounds>();
    KPrefabID component = go.GetComponent<KPrefabID>();
    component.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    component.AddTag(BaseModularLaunchpadPortConfig.LinkTag, false);
    component.AddTag(GameTags.ModularConduitPort, false);
    component.AddTag(GameTags.NotRocketInteriorBuilding, false);
    go.AddOrGetDef<ModularConduitPortController.Def>().mode = isLoader ? ModularConduitPortController.Mode.Load : ModularConduitPortController.Mode.Unload;
    if (!isLoader)
    {
      Storage storage = go.AddComponent<Storage>();
      storage.capacityKg = storageSize;
      storage.allowSettingOnlyFetchMarkedItems = false;
      storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
      {
        Storage.StoredItemModifier.Hide,
        Storage.StoredItemModifier.Seal,
        Storage.StoredItemModifier.Insulate
      });
      switch (conduitType)
      {
        case ConduitType.Gas:
          storage.storageFilters = STORAGEFILTERS.GASES;
          break;
        case ConduitType.Liquid:
          storage.storageFilters = STORAGEFILTERS.LIQUIDS;
          break;
        default:
          storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
          break;
      }
      TreeFilterable treeFilterable = go.AddOrGet<TreeFilterable>();
      treeFilterable.dropIncorrectOnFilterChange = false;
      treeFilterable.autoSelectStoredOnLoad = false;
      if (conduitType == ConduitType.Solid)
      {
        SolidConduitDispenser conduitDispenser = go.AddOrGet<SolidConduitDispenser>();
        conduitDispenser.storage = storage;
        conduitDispenser.elementFilter = (SimHashes[]) null;
      }
      else
      {
        ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
        conduitDispenser.storage = storage;
        conduitDispenser.conduitType = conduitType;
        conduitDispenser.elementFilter = (SimHashes[]) null;
        conduitDispenser.alwaysDispense = true;
      }
    }
    else
    {
      Storage storage = go.AddComponent<Storage>();
      storage.capacityKg = storageSize;
      storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
      {
        Storage.StoredItemModifier.Hide,
        Storage.StoredItemModifier.Seal,
        Storage.StoredItemModifier.Insulate
      });
      if (conduitType == ConduitType.Solid)
      {
        SolidConduitConsumer solidConduitConsumer = go.AddOrGet<SolidConduitConsumer>();
        solidConduitConsumer.storage = storage;
        solidConduitConsumer.capacityTag = GameTags.Any;
        solidConduitConsumer.capacityKG = storageSize;
      }
      else
      {
        ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
        conduitConsumer.storage = storage;
        conduitConsumer.conduitType = conduitType;
        conduitConsumer.capacityTag = GameTags.Any;
        conduitConsumer.capacityKG = storageSize;
      }
    }
    ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
    def.headBuildingTag = TagExtensions.ToTag("LaunchPad");
    def.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
    def.objectLayer = ObjectLayer.Building;
    go.AddOrGet<LogicOperationalController>();
  }

  public static void DoPostConfigureComplete(GameObject go, bool isLoader)
  {
  }
}
