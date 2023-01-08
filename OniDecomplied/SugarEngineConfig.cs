// Decompiled with JetBrains decompiler
// Type: SugarEngineConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SugarEngineConfig : IBuildingConfig
{
  public const string ID = "SugarEngine";
  public const SimHashes FUEL = SimHashes.Sucrose;
  public const float FUEL_CAPACITY = 450f;
  public static float FUEL_EFFICIENCY = 0.125f;

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] denseTieR1 = BUILDINGS.ROCKETRY_MASS_KG.DENSE_TIER1;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SugarEngine", 3, 3, "rocket_sugar_engine_kanim", 1000, 30f, denseTieR1, rawMetals, 9999f, BuildLocationRule.Anywhere, none, noise);
    BuildingTemplates.CreateRocketBuildingDef(buildingDef);
    buildingDef.AttachmentSlotTag = GameTags.Rocket;
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    buildingDef.OverheatTemperature = 2273.15f;
    buildingDef.Floodable = false;
    buildingDef.attachablePosition = new CellOffset(0, 0);
    buildingDef.ObjectLayer = ObjectLayer.Building;
    buildingDef.InputConduitType = ConduitType.None;
    buildingDef.GeneratorWattageRating = 60f;
    buildingDef.GeneratorBaseCapacity = 2000f;
    buildingDef.RequiresPowerInput = false;
    buildingDef.RequiresPowerOutput = false;
    buildingDef.CanMove = true;
    buildingDef.Cancellable = false;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
    {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 3), GameTags.Rocket, (AttachableBuilding) null)
    };
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    RocketEngineCluster rocketEngineCluster = go.AddOrGet<RocketEngineCluster>();
    rocketEngineCluster.maxModules = 5;
    rocketEngineCluster.maxHeight = ROCKETRY.ROCKET_HEIGHT.SHORT;
    rocketEngineCluster.fuelTag = SimHashes.Sucrose.CreateTag();
    rocketEngineCluster.efficiency = ROCKETRY.ENGINE_EFFICIENCY.STRONG;
    rocketEngineCluster.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
    rocketEngineCluster.requireOxidizer = true;
    rocketEngineCluster.exhaustElement = SimHashes.CarbonDioxide;
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
    fuelTank.FuelType = SimHashes.Sucrose.CreateTag();
    fuelTank.targetFillMass = storage.capacityKg;
    fuelTank.physicalFuelCapacity = storage.capacityKg;
    go.AddOrGet<CopyBuildingSettings>();
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = ElementLoader.FindElementByHash(SimHashes.Sucrose).tag;
    manualDeliveryKg.refillMass = storage.capacityKg;
    manualDeliveryKg.capacity = storage.capacityKg;
    manualDeliveryKg.operationalRequirement = Operational.State.None;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string) null, ROCKETRY.BURDEN.INSIGNIFICANT, (float) ROCKETRY.ENGINE_POWER.EARLY_WEAK, SugarEngineConfig.FUEL_EFFICIENCY);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabInitFn += SugarEngineConfig.\u003C\u003Ec.\u003C\u003E9__9_0 ?? (SugarEngineConfig.\u003C\u003Ec.\u003C\u003E9__9_0 = new KPrefabID.PrefabFn((object) SugarEngineConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__9_0)));
  }
}
