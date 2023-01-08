// Decompiled with JetBrains decompiler
// Type: DoorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class DoorConfig : IBuildingConfig
{
  public const string ID = "Door";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR2 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Door", 1, 2, "door_internal_kanim", 30, 10f, tieR2, allMetals, 1600f, BuildLocationRule.Tile, none2, noise, 1f);
    buildingDef.Entombable = true;
    buildingDef.Floodable = false;
    buildingDef.IsFoundation = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PermittedRotations = PermittedRotations.R90;
    buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
    buildingDef.LogicInputPorts = DoorConfig.CreateSingleInputPortList(new CellOffset(0, 0));
    SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
    SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
    return buildingDef;
  }

  public static List<LogicPorts.Port> CreateSingleInputPortList(CellOffset offset) => new List<LogicPorts.Port>()
  {
    LogicPorts.Port.InputPort(Door.OPEN_CLOSE_PORT_ID, offset, (string) STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN, (string) STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN_INACTIVE)
  };

  public override void DoPostConfigureComplete(GameObject go)
  {
    Door door = go.AddOrGet<Door>();
    door.unpoweredAnimSpeed = 1f;
    door.doorType = Door.DoorType.Internal;
    door.doorOpeningSoundEventName = "Open_DoorInternal";
    door.doorClosingSoundEventName = "Close_DoorInternal";
    go.AddOrGet<AccessControl>().controlEnabled = true;
    go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Door;
    go.AddOrGet<Workable>().workTime = 3f;
    go.GetComponent<KBatchedAnimController>().initialAnim = "closed";
    go.AddOrGet<ZoneTile>();
    go.AddOrGet<KBoxCollider2D>();
    Prioritizable.AddRef(go);
    Object.DestroyImmediate((Object) go.GetComponent<BuildingEnabledButton>());
  }

  public override void DoPostConfigureUnderConstruction(GameObject go) => go.AddTag(GameTags.NoCreatureIdling);
}
