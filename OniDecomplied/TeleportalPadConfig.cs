// Decompiled with JetBrains decompiler
// Type: TeleportalPadConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class TeleportalPadConfig : IBuildingConfig
{
  public const string ID = "TeleportalPad";
  public const string PORTAL_ID_PORT_0 = "TeleportalPad_ID_PORT_0";
  public const string PORTAL_ID_PORT_1 = "TeleportalPad_ID_PORT_1";
  public const string PORTAL_ID_PORT_2 = "TeleportalPad_ID_PORT_2";
  public const string PORTAL_ID_PORT_3 = "TeleportalPad_ID_PORT_3";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR7 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER7;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR5 = TUNING.BUILDINGS.DECOR.BONUS.TIER5;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("TeleportalPad", 4, 4, "hqbase_kanim", 250, 30f, tieR7, allMetals, 1600f, BuildLocationRule.OnFloor, tieR5, noise);
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.BaseTimeUntilRepair = 400f;
    buildingDef.DefaultAnimState = "idle";
    buildingDef.RequiresPowerInput = true;
    buildingDef.PowerInputOffset = new CellOffset(2, 0);
    buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.InputPort(HashedString.op_Implicit("TeleportalPad_ID_PORT_0"), new CellOffset(-1, 0), (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE),
      LogicPorts.Port.InputPort(HashedString.op_Implicit("TeleportalPad_ID_PORT_1"), new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE),
      LogicPorts.Port.InputPort(HashedString.op_Implicit("TeleportalPad_ID_PORT_2"), new CellOffset(1, 0), (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE),
      LogicPorts.Port.InputPort(HashedString.op_Implicit("TeleportalPad_ID_PORT_3"), new CellOffset(2, 0), (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE),
      LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(-1, 1), (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE)
    };
    buildingDef.EnergyConsumptionWhenActive = 1600f;
    buildingDef.ExhaustKilowattsWhenActive = 16f;
    buildingDef.SelfHeatKilowattsWhenActive = 64f;
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_LP", NOISE_POLLUTION.NOISY.TIER3);
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_open", NOISE_POLLUTION.NOISY.TIER4);
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_close", NOISE_POLLUTION.NOISY.TIER4);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<TeleportalPad>();
    go.AddOrGet<Teleporter>();
    go.AddOrGet<PrimaryElement>().SetElement(SimHashes.Unobtanium);
  }

  public override void DoPostConfigureComplete(GameObject go) => go.AddOrGet<LogicOperationalController>();
}
