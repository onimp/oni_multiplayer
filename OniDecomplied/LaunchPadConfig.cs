// Decompiled with JetBrains decompiler
// Type: LaunchPadConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LaunchPadConfig : IBuildingConfig
{
  public const string ID = "LaunchPad";
  private const int WIDTH = 7;
  private const string TRIGGER_LAUNCH_PORT_ID = "TriggerLaunch";
  private const string LAUNCH_READY_PORT_ID = "LaunchReady";
  private const string LANDED_ROCKET_ID = "LandedRocket";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LaunchPad", 7, 2, "rocket_launchpad_kanim", 1000, 120f, tieR5, refinedMetals, 9999f, BuildLocationRule.Anywhere, none, noise);
    buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
    buildingDef.OverheatTemperature = 2273.15f;
    buildingDef.Floodable = false;
    buildingDef.UseStructureTemperature = false;
    buildingDef.AttachmentSlotTag = GameTags.Rocket;
    buildingDef.ObjectLayer = ObjectLayer.Building;
    buildingDef.attachablePosition = new CellOffset(0, 0);
    buildingDef.RequiresPowerInput = false;
    buildingDef.DefaultAnimState = "idle";
    buildingDef.CanMove = false;
    buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.InputPort(HashedString.op_Implicit("TriggerLaunch"), new CellOffset(-1, 0), (string) STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH, (string) STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH_INACTIVE)
    };
    buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.OutputPort(HashedString.op_Implicit("LaunchReady"), new CellOffset(1, 0), (string) STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY, (string) STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY_INACTIVE),
      LogicPorts.Port.OutputPort(HashedString.op_Implicit("LandedRocket"), new CellOffset(0, 1), (string) STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LANDED_ROCKET, (string) STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LANDED_ROCKET_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LANDED_ROCKET_INACTIVE)
    };
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.GetComponent<KPrefabID>().AddTag(GameTags.NotRocketInteriorBuilding, false);
    go.AddOrGet<Storage>().SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Hide,
      Storage.StoredItemModifier.Seal,
      Storage.StoredItemModifier.Insulate
    });
    LaunchPad launchPad = go.AddOrGet<LaunchPad>();
    launchPad.triggerPort = HashedString.op_Implicit("TriggerLaunch");
    launchPad.statusPort = HashedString.op_Implicit("LaunchReady");
    launchPad.landedRocketPort = HashedString.op_Implicit("LandedRocket");
    FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
    fakeFloorAdder.floorOffsets = new CellOffset[7];
    for (int index = 0; index < 7; ++index)
      fakeFloorAdder.floorOffsets[index] = new CellOffset(index - 3, 1);
    go.AddOrGet<LaunchPadConditions>();
    ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
    def.headBuildingTag = TagExtensions.ToTag("LaunchPad");
    def.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
    def.objectLayer = ObjectLayer.Building;
    go.AddOrGetDef<LaunchPadMaterialDistributor.Def>();
    go.AddOrGet<UserNameable>();
    go.AddOrGet<CharacterOverlay>().shouldShowName = true;
    ModularConduitPortTiler conduitPortTiler = go.AddOrGet<ModularConduitPortTiler>();
    conduitPortTiler.manageRightCap = true;
    conduitPortTiler.manageLeftCap = false;
    conduitPortTiler.leftCapDefaultSceneLayerAdjust = 1;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
