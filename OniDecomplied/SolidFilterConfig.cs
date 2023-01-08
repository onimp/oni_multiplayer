// Decompiled with JetBrains decompiler
// Type: SolidFilterConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class SolidFilterConfig : IBuildingConfig
{
  public const string ID = "SolidFilter";
  private const ConduitType CONDUIT_TYPE = ConduitType.Solid;
  private ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(0, 0));

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR1_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues tieR1_2 = NOISE_POLLUTION.NOISY.TIER1;
    EffectorValues tieR0 = BUILDINGS.DECOR.PENALTY.TIER0;
    EffectorValues noise = tieR1_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolidFilter", 3, 1, "filter_material_conveyor_kanim", 30, 10f, tieR1_1, rawMetals, 1600f, BuildLocationRule.Anywhere, tieR0, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.InputConduitType = ConduitType.Solid;
    buildingDef.OutputConduitType = ConduitType.Solid;
    buildingDef.Floodable = false;
    buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
    buildingDef.PermittedRotations = PermittedRotations.R360;
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidFilter");
    return buildingDef;
  }

  private void AttachPort(GameObject go) => go.AddComponent<ConduitSecondaryOutput>().portInfo = this.secondaryPort;

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
    base.DoPostConfigurePreview(def, go);
    this.AttachPort(go);
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
    base.DoPostConfigureUnderConstruction(go);
    this.AttachPort(go);
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<ElementFilter>().portInfo = this.secondaryPort;
    go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Solid;
  }

  public override void DoPostConfigureComplete(GameObject go) => go.AddOrGetDef<PoweredActiveController.Def>().showWorkingStatus = true;
}
