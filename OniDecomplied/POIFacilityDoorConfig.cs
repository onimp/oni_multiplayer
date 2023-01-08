// Decompiled with JetBrains decompiler
// Type: POIFacilityDoorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class POIFacilityDoorConfig : IBuildingConfig
{
  public const string ID = "POIFacilityDoor";

  public override BuildingDef CreateBuildingDef()
  {
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("POIFacilityDoor", 2, 3, "door_facility_kanim", 30, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NONE);
    buildingDef.Overheatable = false;
    buildingDef.Repairable = false;
    buildingDef.Floodable = false;
    buildingDef.Invincible = true;
    buildingDef.IsFoundation = true;
    buildingDef.TileLayer = ObjectLayer.FoundationTile;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PermittedRotations = PermittedRotations.R90;
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
    buildingDef.ShowInBuildMenu = false;
    SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_gear_LP", NOISE_POLLUTION.NOISY.TIER1);
    SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_open", NOISE_POLLUTION.NOISY.TIER2);
    SoundEventVolumeCache.instance.AddVolume("door_manual_kanim", "ManualPressureDoor_close", NOISE_POLLUTION.NOISY.TIER2);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Door door = go.AddOrGet<Door>();
    door.hasComplexUserControls = false;
    door.unpoweredAnimSpeed = 1f;
    door.doorType = Door.DoorType.ManualPressure;
    go.AddOrGet<ZoneTile>();
    go.AddOrGet<AccessControl>();
    go.AddOrGet<Unsealable>();
    go.AddOrGet<KBoxCollider2D>();
    Prioritizable.AddRef(go);
    go.AddOrGet<Workable>().workTime = 5f;
    go.AddOrGet<KBatchedAnimController>().fgLayer = Grid.SceneLayer.BuildingFront;
    PrimaryElement component = go.GetComponent<PrimaryElement>();
    component.SetElement(SimHashes.Steel);
    component.Temperature = 273f;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.GetComponent<AccessControl>().controlEnabled = false;
    go.GetComponent<Deconstructable>().allowDeconstruction = true;
    go.GetComponent<KBatchedAnimController>().initialAnim = "closed";
  }
}
