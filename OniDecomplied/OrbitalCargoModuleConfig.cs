// Decompiled with JetBrains decompiler
// Type: OrbitalCargoModuleConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class OrbitalCargoModuleConfig : IBuildingConfig
{
  public const string ID = "OrbitalCargoModule";
  public static int NUM_CAPSULES = 3 * Mathf.RoundToInt(ROCKETRY.CARGO_CAPACITY_SCALE);
  public static float TOTAL_STORAGE_MASS = 200f * (float) OrbitalCargoModuleConfig.NUM_CAPSULES;

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] hollowTieR2 = BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER2;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("OrbitalCargoModule", 3, 2, "rocket_orbital_deploy_cargo_module_kanim", 1000, 30f, hollowTieR2, rawMetals, 9999f, BuildLocationRule.Anywhere, none, noise);
    BuildingTemplates.CreateRocketBuildingDef(buildingDef);
    buildingDef.DefaultAnimState = "deployed";
    buildingDef.AttachmentSlotTag = GameTags.Rocket;
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
    buildingDef.OverheatTemperature = 2273.15f;
    buildingDef.Floodable = false;
    buildingDef.ObjectLayer = ObjectLayer.Building;
    buildingDef.RequiresPowerInput = false;
    buildingDef.CanMove = true;
    buildingDef.Cancellable = false;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    List<Tag> tagList = new List<Tag>();
    tagList.AddRange((IEnumerable<Tag>) STORAGEFILTERS.NOT_EDIBLE_SOLIDS);
    tagList.AddRange((IEnumerable<Tag>) STORAGEFILTERS.FOOD);
    Storage storage = go.AddComponent<Storage>();
    storage.showInUI = true;
    storage.capacityKg = OrbitalCargoModuleConfig.TOTAL_STORAGE_MASS;
    storage.showCapacityStatusItem = true;
    storage.showDescriptor = true;
    storage.storageFilters = tagList;
    storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    go.AddOrGet<StorageLocker>();
    go.AddOrGetDef<OrbitalDeployCargoModule.Def>().numCapsules = (float) OrbitalCargoModuleConfig.NUM_CAPSULES;
    go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
    {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket, (AttachableBuilding) null)
    };
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    Prioritizable.AddRef(go);
    BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string) null, ROCKETRY.BURDEN.MODERATE);
    FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
    fakeFloorAdder.floorOffsets = new CellOffset[3]
    {
      new CellOffset(-1, -1),
      new CellOffset(0, -1),
      new CellOffset(1, -1)
    };
    fakeFloorAdder.initiallyActive = false;
  }
}
