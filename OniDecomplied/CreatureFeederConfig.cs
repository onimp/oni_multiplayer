// Decompiled with JetBrains decompiler
// Type: CreatureFeederConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class CreatureFeederConfig : IBuildingConfig
{
  public const string ID = "CreatureFeeder";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CreatureFeeder", 1, 2, "feeder_kanim", 100, 120f, tieR3, rawMetals, 1600f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.AudioCategory = "Metal";
    return buildingDef;
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Prioritizable.AddRef(go);
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 2000f;
    storage.showInUI = true;
    storage.showDescriptor = true;
    storage.allowItemRemoval = false;
    storage.allowSettingOnlyFetchMarkedItems = false;
    storage.showCapacityStatusItem = true;
    storage.showCapacityAsMainStatus = true;
    go.AddOrGet<StorageLocker>().choreTypeID = Db.Get().ChoreTypes.RanchingFetch.Id;
    go.AddOrGet<UserNameable>();
    go.AddOrGet<TreeFilterable>();
    go.AddOrGet<CreatureFeeder>();
  }

  public override void DoPostConfigureComplete(GameObject go) => go.AddOrGetDef<StorageController.Def>();

  public override void ConfigurePost(BuildingDef def)
  {
    List<Tag> tagList = new List<Tag>();
    Tag[] target_species = new Tag[6]
    {
      GameTags.Creatures.Species.LightBugSpecies,
      GameTags.Creatures.Species.HatchSpecies,
      GameTags.Creatures.Species.MoleSpecies,
      GameTags.Creatures.Species.CrabSpecies,
      GameTags.Creatures.Species.StaterpillarSpecies,
      GameTags.Creatures.Species.DivergentSpecies
    };
    foreach (KeyValuePair<Tag, Diet> collectDiet in DietManager.CollectDiets(target_species))
      tagList.Add(collectDiet.Key);
    def.BuildingComplete.GetComponent<Storage>().storageFilters = tagList;
  }
}
