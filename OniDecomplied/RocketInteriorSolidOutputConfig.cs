// Decompiled with JetBrains decompiler
// Type: RocketInteriorSolidOutputConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class RocketInteriorSolidOutputConfig : IBuildingConfig
{
  private const ConduitType CONDUIT_TYPE = ConduitType.Solid;
  private const CargoBay.CargoType CARGO_TYPE = CargoBay.CargoType.Solids;
  public const string ID = "RocketInteriorSolidOutput";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR0 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RocketInteriorSolidOutput", 1, 1, "rocket_floor_plug_solid_out_kanim", 30, 3f, tieR0, allMetals, 1600f, BuildLocationRule.OnRocketEnvelope, tieR2, noise);
    buildingDef.OutputConduitType = ConduitType.Solid;
    buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 60f;
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
    buildingDef.Overheatable = false;
    buildingDef.Floodable = false;
    buildingDef.Entombable = false;
    buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
    buildingDef.ObjectLayer = ObjectLayer.Building;
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "small";
    buildingDef.BaseTimeUntilRepair = -1f;
    buildingDef.PermittedRotations = PermittedRotations.R360;
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "RocketInteriorSolidOutput");
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    base.ConfigureBuildingTemplate(go, prefab_tag);
    go.GetComponent<KPrefabID>().AddTag(GameTags.RocketInteriorBuilding, false);
    go.AddComponent<RequireInputs>();
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGetDef<PoweredActiveController.Def>();
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 20f;
    go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Solid;
    RocketConduitStorageAccess conduitStorageAccess = go.AddOrGet<RocketConduitStorageAccess>();
    conduitStorageAccess.storage = storage;
    conduitStorageAccess.cargoType = CargoBay.CargoType.Solids;
    conduitStorageAccess.targetLevel = 20f;
    SolidConduitDispenser conduitDispenser = go.AddOrGet<SolidConduitDispenser>();
    conduitDispenser.alwaysDispense = true;
    conduitDispenser.elementFilter = (SimHashes[]) null;
  }
}
