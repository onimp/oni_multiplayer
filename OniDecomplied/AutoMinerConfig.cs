// Decompiled with JetBrains decompiler
// Type: AutoMinerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class AutoMinerConfig : IBuildingConfig
{
  public const string ID = "AutoMiner";
  private const int RANGE = 7;
  private const int X = -7;
  private const int Y = 0;
  private const int WIDTH = 16;
  private const int HEIGHT = 9;
  private const int VISION_OFFSET = 1;

  public override BuildingDef CreateBuildingDef()
  {
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AutoMiner", 2, 2, "auto_miner_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFoundationRotatable, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0);
    buildingDef.Floodable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.SelfHeatKilowattsWhenActive = 2f;
    buildingDef.PermittedRotations = PermittedRotations.R360;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "AutoMiner");
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<Operational>();
    go.AddOrGet<LoopingSounds>();
    go.AddOrGet<MiningSounds>();
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => AutoMinerConfig.AddVisualizer(go, true);

  public override void DoPostConfigureUnderConstruction(GameObject go) => AutoMinerConfig.AddVisualizer(go, false);

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    AutoMiner autoMiner = go.AddOrGet<AutoMiner>();
    autoMiner.x = -7;
    autoMiner.y = 0;
    autoMiner.width = 16;
    autoMiner.height = 9;
    autoMiner.vision_offset = new CellOffset(0, 1);
    AutoMinerConfig.AddVisualizer(go, false);
  }

  private static void AddVisualizer(GameObject prefab, bool movable)
  {
    StationaryChoreRangeVisualizer choreRangeVisualizer = prefab.AddOrGet<StationaryChoreRangeVisualizer>();
    choreRangeVisualizer.x = -7;
    choreRangeVisualizer.y = 0;
    choreRangeVisualizer.width = 16;
    choreRangeVisualizer.height = 9;
    choreRangeVisualizer.vision_offset = new CellOffset(0, 1);
    choreRangeVisualizer.movable = movable;
    choreRangeVisualizer.blocking_tile_visible = false;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    prefab.GetComponent<KPrefabID>().instantiateFn += AutoMinerConfig.\u003C\u003Ec.\u003C\u003E9__12_0 ?? (AutoMinerConfig.\u003C\u003Ec.\u003C\u003E9__12_0 = new KPrefabID.PrefabFn((object) AutoMinerConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CAddVisualizer\u003Eb__12_0)));
  }
}
