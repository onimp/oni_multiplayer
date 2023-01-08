// Decompiled with JetBrains decompiler
// Type: ArtifactAnalysisStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class ArtifactAnalysisStationConfig : IBuildingConfig
{
  public const string ID = "ArtifactAnalysisStation";
  public const float WORK_TIME = 150f;

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
    EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR6;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ArtifactAnalysisStation", 4, 4, "artifact_analysis_kanim", 30, 60f, tieR5, allMetals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 480f;
    buildingDef.SelfHeatKilowattsWhenActive = 1f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.AudioSize = "large";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<DropAllWorkable>();
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    go.AddOrGetDef<ArtifactAnalysisStation.Def>();
    go.AddOrGet<ArtifactAnalysisStationWorkable>();
    Prioritizable.AddRef(go);
    Storage storage = go.AddOrGet<Storage>();
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    manualDeliveryKg.RequestedItemTag = GameTags.CharmedArtifact;
    manualDeliveryKg.refillMass = 1f;
    manualDeliveryKg.MinimumMass = 1f;
    manualDeliveryKg.capacity = 1f;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
