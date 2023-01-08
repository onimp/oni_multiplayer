// Decompiled with JetBrains decompiler
// Type: GeneticAnalysisStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GeneticAnalysisStationConfig : IBuildingConfig
{
  public const string ID = "GeneticAnalysisStation";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR3 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR3;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GeneticAnalysisStation", 7, 2, "genetic_analysisstation_kanim", 30, 30f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "large";
    buildingDef.EnergyConsumptionWhenActive = 480f;
    buildingDef.ExhaustKilowattsWhenActive = 0.5f;
    buildingDef.SelfHeatKilowattsWhenActive = 4f;
    buildingDef.Deprecated = !DlcManager.FeaturePlantMutationsEnabled();
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.ScienceBuilding, false);
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    go.AddOrGetDef<GeneticAnalysisStation.Def>();
    go.AddOrGet<GeneticAnalysisStationWorkable>().finishedSeedDropOffset = new Vector3(-3f, 1.5f, 0.0f);
    Prioritizable.AddRef(go);
    go.AddOrGet<DropAllWorkable>();
    go.AddOrGetDef<PoweredActiveController.Def>();
    Storage storage = go.AddOrGet<Storage>();
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    manualDeliveryKg.RequestedItemTag = GameTags.UnidentifiedSeed;
    manualDeliveryKg.refillMass = 1.1f;
    manualDeliveryKg.MinimumMass = 1f;
    manualDeliveryKg.capacity = 5f;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }

  public override void ConfigurePost(BuildingDef def)
  {
    List<Tag> tagList = new List<Tag>();
    foreach (GameObject go in Assets.GetPrefabsWithTag(GameTags.CropSeed))
    {
      if (Object.op_Inequality((Object) go.GetComponent<MutantPlant>(), (Object) null))
        tagList.Add(go.PrefabID());
    }
    def.BuildingComplete.GetComponent<Storage>().storageFilters = tagList;
  }
}
