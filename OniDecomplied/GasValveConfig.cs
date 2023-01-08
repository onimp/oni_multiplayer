// Decompiled with JetBrains decompiler
// Type: GasValveConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class GasValveConfig : IBuildingConfig
{
  public const string ID = "GasValve";
  private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR1_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues tieR1_2 = NOISE_POLLUTION.NOISY.TIER1;
    EffectorValues tieR0 = BUILDINGS.DECOR.PENALTY.TIER0;
    EffectorValues noise = tieR1_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GasValve", 1, 2, "valvegas_kanim", 30, 10f, tieR1_1, rawMetals, 1600f, BuildLocationRule.Anywhere, tieR0, noise);
    buildingDef.InputConduitType = ConduitType.Gas;
    buildingDef.OutputConduitType = ConduitType.Gas;
    buildingDef.Floodable = false;
    buildingDef.ViewMode = OverlayModes.GasConduits.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PermittedRotations = PermittedRotations.R360;
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasValve");
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    GeneratedBuildings.MakeBuildingAlwaysOperational(go);
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
    ValveBase valveBase = go.AddOrGet<ValveBase>();
    valveBase.conduitType = ConduitType.Gas;
    valveBase.maxFlow = 1f;
    valveBase.animFlowRanges = new ValveBase.AnimRangeInfo[3]
    {
      new ValveBase.AnimRangeInfo(0.25f, "lo"),
      new ValveBase.AnimRangeInfo(0.5f, "med"),
      new ValveBase.AnimRangeInfo(0.75f, "hi")
    };
    go.AddOrGet<Valve>();
    go.AddOrGet<Workable>().workTime = 5f;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    Object.DestroyImmediate((Object) go.GetComponent<RequireInputs>());
    Object.DestroyImmediate((Object) go.GetComponent<ConduitConsumer>());
    Object.DestroyImmediate((Object) go.GetComponent<ConduitDispenser>());
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits, false);
  }
}
