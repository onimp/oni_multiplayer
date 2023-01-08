// Decompiled with JetBrains decompiler
// Type: LogicMemoryConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LogicMemoryConfig : IBuildingConfig
{
  public static string ID = "LogicMemory";

  public override BuildingDef CreateBuildingDef()
  {
    string id = LogicMemoryConfig.ID;
    float[] tieR0 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 2, 2, "logic_memory_kanim", 10, 30f, tieR0, refinedMetals, 1600f, BuildLocationRule.Anywhere, none2, noise);
    buildingDef.Deprecated = false;
    buildingDef.Overheatable = false;
    buildingDef.Floodable = false;
    buildingDef.Entombable = false;
    buildingDef.PermittedRotations = PermittedRotations.R360;
    buildingDef.InitialOrientation = Orientation.R90;
    buildingDef.ViewMode = OverlayModes.Logic.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
    buildingDef.ObjectLayer = ObjectLayer.LogicGate;
    buildingDef.AlwaysOperational = true;
    buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
    {
      new LogicPorts.Port(LogicMemory.SET_PORT_ID, new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.SET_PORT, (string) STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.SET_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.SET_PORT_INACTIVE, true, LogicPortSpriteType.Input, true),
      new LogicPorts.Port(LogicMemory.RESET_PORT_ID, new CellOffset(1, 0), (string) STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.RESET_PORT, (string) STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.RESET_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.RESET_PORT_INACTIVE, true, LogicPortSpriteType.ResetUpdate, true)
    };
    buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      new LogicPorts.Port(LogicMemory.READ_PORT_ID, new CellOffset(0, 1), (string) STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.READ_PORT, (string) STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.READ_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.READ_PORT_INACTIVE, true, LogicPortSpriteType.Output, true)
    };
    SoundEventVolumeCache.instance.AddVolume("logic_memory_kanim", "PowerMemory_on", NOISE_POLLUTION.NOISY.TIER3);
    SoundEventVolumeCache.instance.AddVolume("logic_memory_kanim", "PowerMemory_off", NOISE_POLLUTION.NOISY.TIER3);
    GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, LogicMemoryConfig.ID);
    return buildingDef;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicMemory>();
    go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
  }
}
