// Decompiled with JetBrains decompiler
// Type: KeroseneEngineClusterSmallConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class KeroseneEngineClusterSmallConfig : IBuildingConfig
{
  public const string ID = "KeroseneEngineClusterSmall";
  public const SimHashes FUEL = SimHashes.Petroleum;
  public const float FUEL_CAPACITY = 450f;

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] engineMassSmall = BUILDINGS.ROCKETRY_MASS_KG.ENGINE_MASS_SMALL;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("KeroseneEngineClusterSmall", 3, 4, "rocket_petro_engine_small_kanim", 1000, 60f, engineMassSmall, refinedMetals, 9999f, BuildLocationRule.Anywhere, none, noise);
    BuildingTemplates.CreateRocketBuildingDef(buildingDef);
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    buildingDef.OverheatTemperature = 2273.15f;
    buildingDef.Floodable = false;
    buildingDef.AttachmentSlotTag = GameTags.Rocket;
    buildingDef.ObjectLayer = ObjectLayer.Building;
    buildingDef.attachablePosition = new CellOffset(0, 0);
    buildingDef.UtilityInputOffset = new CellOffset(0, 2);
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.GeneratorWattageRating = 240f;
    buildingDef.GeneratorBaseCapacity = 4000f;
    buildingDef.RequiresPowerInput = false;
    buildingDef.RequiresPowerOutput = false;
    buildingDef.CanMove = true;
    buildingDef.Cancellable = false;
    buildingDef.ShowInBuildMenu = false;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
    {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 4), GameTags.Rocket, (AttachableBuilding) null)
    };
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    RocketEngineCluster rocketEngineCluster = go.AddOrGet<RocketEngineCluster>();
    rocketEngineCluster.maxModules = 4;
    rocketEngineCluster.maxHeight = ROCKETRY.ROCKET_HEIGHT.MEDIUM;
    rocketEngineCluster.fuelTag = SimHashes.Petroleum.CreateTag();
    rocketEngineCluster.efficiency = ROCKETRY.ENGINE_EFFICIENCY.MEDIUM;
    rocketEngineCluster.requireOxidizer = true;
    rocketEngineCluster.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
    rocketEngineCluster.exhaustElement = SimHashes.CarbonDioxide;
    rocketEngineCluster.exhaustTemperature = 1263.15f;
    go.AddOrGet<ModuleGenerator>();
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 450f;
    storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Hide,
      Storage.StoredItemModifier.Seal,
      Storage.StoredItemModifier.Insulate
    });
    FuelTank fuelTank = go.AddOrGet<FuelTank>();
    fuelTank.consumeFuelOnLand = false;
    fuelTank.storage = storage;
    fuelTank.FuelType = SimHashes.Petroleum.CreateTag();
    fuelTank.targetFillMass = storage.capacityKg;
    fuelTank.physicalFuelCapacity = storage.capacityKg;
    BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string) null, ROCKETRY.BURDEN.MODERATE_PLUS, (float) ROCKETRY.ENGINE_POWER.MID_STRONG, ROCKETRY.FUEL_COST_PER_DISTANCE.MEDIUM);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabInitFn += KeroseneEngineClusterSmallConfig.\u003C\u003Ec.\u003C\u003E9__6_0 ?? (KeroseneEngineClusterSmallConfig.\u003C\u003Ec.\u003C\u003E9__6_0 = new KPrefabID.PrefabFn((object) KeroseneEngineClusterSmallConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__6_0)));
  }
}
