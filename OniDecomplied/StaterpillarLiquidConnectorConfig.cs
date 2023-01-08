// Decompiled with JetBrains decompiler
// Type: StaterpillarLiquidConnectorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class StaterpillarLiquidConnectorConfig : IBuildingConfig
{
  public static readonly string ID = "StaterpillarLiquidConnector";
  private const int WIDTH = 1;
  private const int HEIGHT = 2;

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    string id = StaterpillarLiquidConnectorConfig.ID;
    string[] allMetals = MATERIALS.ALL_METALS;
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] construction_materials = allMetals;
    EffectorValues tieR0 = NOISE_POLLUTION.NOISY.TIER0;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR0;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 1, 2, "egg_caterpillar_kanim", 1000, 10f, tieR3, construction_materials, 9999f, BuildLocationRule.OnFoundationRotatable, none, noise);
    buildingDef.Overheatable = false;
    buildingDef.Floodable = false;
    buildingDef.OverheatTemperature = 423.15f;
    buildingDef.PermittedRotations = PermittedRotations.FlipV;
    buildingDef.ViewMode = OverlayModes.GasConduits.ID;
    buildingDef.AudioCategory = "Plastic";
    buildingDef.OutputConduitType = ConduitType.Liquid;
    buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
    buildingDef.PlayConstructionSounds = false;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<Storage>();
    ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
    conduitDispenser.conduitType = ConduitType.Liquid;
    conduitDispenser.alwaysDispense = true;
    conduitDispenser.elementFilter = (SimHashes[]) null;
    conduitDispenser.isOn = false;
    go.GetComponent<Deconstructable>().SetAllowDeconstruction(false);
    go.GetComponent<KSelectable>().IsSelectable = false;
  }
}
