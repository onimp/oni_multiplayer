// Decompiled with JetBrains decompiler
// Type: RocketInteriorGasInputPortConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class RocketInteriorGasInputPortConfig : IBuildingConfig
{
  public const string ID = "RocketInteriorGasInputPort";
  private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(0, 0));

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR0 = BUILDINGS.DECOR.BONUS.TIER0;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RocketInteriorGasInputPort", 1, 1, "rocket_interior_port_gas_in_kanim", 100, 30f, tieR2, refinedMetals, 9999f, BuildLocationRule.Tile, tieR0, noise);
    buildingDef.DefaultAnimState = "gas_in";
    buildingDef.Floodable = false;
    buildingDef.Entombable = false;
    buildingDef.Overheatable = false;
    buildingDef.UseStructureTemperature = false;
    buildingDef.Replaceable = false;
    buildingDef.Invincible = true;
    buildingDef.BaseTimeUntilRepair = -1f;
    buildingDef.AudioCategory = "Metal";
    buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
    return buildingDef;
  }

  private void AttachPort(GameObject go) => go.AddComponent<ConduitSecondaryInput>().portInfo = this.gasInputPort;

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    GeneratedBuildings.MakeBuildingAlwaysOperational(go);
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
    go.AddOrGet<SimCellOccupier>().notifyOnMelt = true;
    go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
    Storage storage = go.AddComponent<Storage>();
    storage.showInUI = false;
    storage.capacityKg = 1f;
    RocketConduitSender rocketConduitSender = go.AddComponent<RocketConduitSender>();
    rocketConduitSender.conduitStorage = storage;
    rocketConduitSender.conduitPortInfo = this.gasInputPort;
    AutoStorageDropper.Def def = go.AddOrGetDef<AutoStorageDropper.Def>();
    def.elementFilter = new SimHashes[1]
    {
      SimHashes.Unobtanium
    };
    def.dropOffset = new CellOffset(0, -1);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    GeneratedBuildings.RemoveLoopingSounds(go);
    KPrefabID component = go.GetComponent<KPrefabID>();
    component.AddTag(GameTags.Bunker, false);
    component.AddTag(GameTags.FloorTiles, false);
    component.AddTag(GameTags.NoRocketRefund, false);
    go.AddOrGetDef<MakeBaseSolid.Def>().solidOffsets = new CellOffset[1]
    {
      new CellOffset(0, 0)
    };
    go.AddOrGet<BuildingCellVisualizer>();
    go.GetComponent<Deconstructable>().allowDeconstruction = false;
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
    base.DoPostConfigurePreview(def, go);
    go.AddOrGet<BuildingCellVisualizer>();
    this.AttachPort(go);
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
    base.DoPostConfigureUnderConstruction(go);
    go.AddOrGet<BuildingCellVisualizer>();
    this.AttachPort(go);
  }
}
