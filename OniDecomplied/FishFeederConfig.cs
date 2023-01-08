// Decompiled with JetBrains decompiler
// Type: FishFeederConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class FishFeederConfig : IBuildingConfig
{
  public const string ID = "FishFeeder";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FishFeeder", 1, 3, "fishfeeder_kanim", 100, 120f, tieR3, rawMetals, 1600f, BuildLocationRule.Anywhere, tieR2, noise);
    buildingDef.AudioCategory = "Metal";
    buildingDef.Entombable = true;
    buildingDef.Floodable = true;
    buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
    return buildingDef;
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Prioritizable.AddRef(go);
    Storage storage1 = go.AddOrGet<Storage>();
    storage1.capacityKg = 200f;
    storage1.showInUI = true;
    storage1.showDescriptor = true;
    storage1.allowItemRemoval = false;
    storage1.allowSettingOnlyFetchMarkedItems = false;
    storage1.showCapacityStatusItem = true;
    storage1.showCapacityAsMainStatus = true;
    Storage storage2 = go.AddComponent<Storage>();
    storage2.capacityKg = 200f;
    storage2.showInUI = true;
    storage2.showDescriptor = true;
    storage2.allowItemRemoval = false;
    go.AddOrGet<StorageLocker>().choreTypeID = Db.Get().ChoreTypes.RanchingFetch.Id;
    go.AddOrGet<UserNameable>();
    Effect effect = new Effect("AteFromFeeder", (string) STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME, (string) STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.TOOLTIP, 600f, true, false, false);
    effect.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, -0.0333333351f, (string) STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME));
    effect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 2f, (string) STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME));
    Db.Get().effects.Add(effect);
    go.AddOrGet<TreeFilterable>();
    go.AddOrGet<CreatureFeeder>().effectId = effect.Id;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGetDef<StorageController.Def>();
    go.AddOrGetDef<FishFeeder.Def>();
    go.AddOrGetDef<MakeBaseSolid.Def>().solidOffsets = new CellOffset[1]
    {
      new CellOffset(0, 0)
    };
    SymbolOverrideControllerUtil.AddToPrefab(go);
  }

  public override void ConfigurePost(BuildingDef def)
  {
    List<Tag> tagList = new List<Tag>();
    Tag[] target_species = new Tag[1]
    {
      GameTags.Creatures.Species.PacuSpecies
    };
    foreach (KeyValuePair<Tag, Diet> collectDiet in DietManager.CollectDiets(target_species))
      tagList.Add(collectDiet.Key);
    def.BuildingComplete.GetComponent<Storage>().storageFilters = tagList;
  }
}
