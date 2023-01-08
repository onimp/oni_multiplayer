// Decompiled with JetBrains decompiler
// Type: POIDoorInternalConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class POIDoorInternalConfig : IBuildingConfig
{
  public static string ID = "POIDoorInternal";

  public override BuildingDef CreateBuildingDef()
  {
    string id = POIDoorInternalConfig.ID;
    float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 1, 2, "door_poi_internal_kanim", 30, 10f, tieR2, allMetals, 1600f, BuildLocationRule.Tile, none2, noise, 1f);
    buildingDef.ShowInBuildMenu = false;
    buildingDef.Entombable = false;
    buildingDef.Floodable = false;
    buildingDef.Invincible = true;
    buildingDef.IsFoundation = true;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PermittedRotations = PermittedRotations.R90;
    buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
    buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
    buildingDef.LogicInputPorts = DoorConfig.CreateSingleInputPortList(new CellOffset(0, 0));
    SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
    SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Door door = go.AddOrGet<Door>();
    door.unpoweredAnimSpeed = 1f;
    door.doorType = Door.DoorType.Internal;
    go.AddOrGet<ZoneTile>();
    go.AddOrGet<AccessControl>();
    go.AddOrGet<Workable>().workTime = 3f;
    go.AddOrGet<KBoxCollider2D>();
    Prioritizable.AddRef(go);
    Object.DestroyImmediate((Object) go.GetComponent<BuildingEnabledButton>());
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddTag(GameTags.Gravitas);
    AccessControl component = go.GetComponent<AccessControl>();
    go.GetComponent<Door>().hasComplexUserControls = false;
    component.controlEnabled = false;
    go.GetComponent<Deconstructable>().allowDeconstruction = true;
    go.GetComponent<KBatchedAnimController>().initialAnim = "closed";
  }
}
