// Decompiled with JetBrains decompiler
// Type: BunkerDoorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class BunkerDoorConfig : IBuildingConfig
{
  public const string ID = "BunkerDoor";

  public override BuildingDef CreateBuildingDef()
  {
    float[] construction_mass = new float[1]{ 500f };
    string[] construction_materials = new string[1]
    {
      SimHashes.Steel.ToString()
    };
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("BunkerDoor", 4, 1, "door_bunker_kanim", 1000, 120f, construction_mass, construction_materials, 1600f, BuildLocationRule.Tile, none2, noise, 1f);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.OverheatTemperature = 1273.15f;
    buildingDef.Entombable = false;
    buildingDef.IsFoundation = true;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PermittedRotations = PermittedRotations.R90;
    buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
    buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
    buildingDef.TileLayer = ObjectLayer.FoundationTile;
    buildingDef.LogicInputPorts = DoorConfig.CreateSingleInputPortList(new CellOffset(-1, 0));
    SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
    SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
    return buildingDef;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    Door door = go.AddOrGet<Door>();
    door.unpoweredAnimSpeed = 0.01f;
    door.poweredAnimSpeed = 0.1f;
    door.hasComplexUserControls = true;
    door.allowAutoControl = false;
    door.doorOpeningSoundEventName = "BunkerDoor_opening";
    door.doorClosingSoundEventName = "BunkerDoor_closing";
    door.verticalOrientation = Orientation.R90;
    go.AddOrGet<Workable>().workTime = 3f;
    KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
    component.initialAnim = "closed";
    component.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
    go.AddOrGet<ZoneTile>();
    go.AddOrGet<KBoxCollider2D>();
    Prioritizable.AddRef(go);
    Object.DestroyImmediate((Object) go.GetComponent<BuildingEnabledButton>());
    go.GetComponent<KPrefabID>().AddTag(GameTags.Bunker, false);
  }
}
