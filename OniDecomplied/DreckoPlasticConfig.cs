// Decompiled with JetBrains decompiler
// Type: DreckoPlasticConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DreckoPlasticConfig : IEntityConfig
{
  public const string ID = "DreckoPlastic";
  public const string BASE_TRAIT_ID = "DreckoPlasticBaseTrait";
  public const string EGG_ID = "DreckoPlasticEgg";
  public static Tag POOP_ELEMENT = SimHashes.Phosphorite.CreateTag();
  public static Tag EMIT_ELEMENT = SimHashes.Polypropylene.CreateTag();
  private static float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 1f;
  private static float CALORIES_PER_DAY_OF_PLANT_EATEN = DreckoTuning.STANDARD_CALORIES_PER_CYCLE / DreckoPlasticConfig.DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;
  private static float KG_POOP_PER_DAY_OF_PLANT = 9f;
  private static float MIN_POOP_SIZE_IN_KG = 1.5f;
  private static float MIN_POOP_SIZE_IN_CALORIES = DreckoPlasticConfig.CALORIES_PER_DAY_OF_PLANT_EATEN * DreckoPlasticConfig.MIN_POOP_SIZE_IN_KG / DreckoPlasticConfig.KG_POOP_PER_DAY_OF_PLANT;
  public static float SCALE_GROWTH_TIME_IN_CYCLES = 3f;
  public static float PLASTIC_PER_CYCLE = 50f;
  public static int EGG_SORT_ORDER = 800;

  public static GameObject CreateDrecko(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseDreckoConfig.BaseDrecko(id, name, desc, anim_file, "DreckoPlasticBaseTrait", is_baby, (string) null, 298.15f, 333.15f), DreckoTuning.PEN_SIZE_PER_CREATURE);
    Trait trait = Db.Get().CreateTrait("DreckoPlasticBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, DreckoTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) DreckoTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name));
    HashSet<Tag> consumed_tags = new HashSet<Tag>();
    consumed_tags.Add(TagExtensions.ToTag("BasicSingleHarvestPlant"));
    consumed_tags.Add(TagExtensions.ToTag("PrickleFlower"));
    Diet diet = new Diet(new Diet.Info[1]
    {
      new Diet.Info(consumed_tags, DreckoPlasticConfig.POOP_ELEMENT, DreckoPlasticConfig.CALORIES_PER_DAY_OF_PLANT_EATEN, DreckoPlasticConfig.KG_POOP_PER_DAY_OF_PLANT, eats_plants_directly: true)
    });
    CreatureCalorieMonitor.Def def1 = wildCreature.AddOrGetDef<CreatureCalorieMonitor.Def>();
    def1.diet = diet;
    def1.minPoopSizeInCalories = DreckoPlasticConfig.MIN_POOP_SIZE_IN_CALORIES;
    ScaleGrowthMonitor.Def def2 = wildCreature.AddOrGetDef<ScaleGrowthMonitor.Def>();
    def2.defaultGrowthRate = (float) (1.0 / (double) DreckoPlasticConfig.SCALE_GROWTH_TIME_IN_CYCLES / 600.0);
    def2.dropMass = DreckoPlasticConfig.PLASTIC_PER_CYCLE * DreckoPlasticConfig.SCALE_GROWTH_TIME_IN_CYCLES;
    def2.itemDroppedOnShear = DreckoPlasticConfig.EMIT_ELEMENT;
    def2.levelCount = 6;
    def2.targetAtmosphere = SimHashes.Hydrogen;
    wildCreature.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
    return wildCreature;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public virtual GameObject CreatePrefab()
  {
    GameObject drecko = DreckoPlasticConfig.CreateDrecko("DreckoPlastic", (string) CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.NAME, (string) CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.DESC, "drecko_kanim", false);
    string eggName = (string) CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.EGG_NAME;
    string desc = (string) CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.DESC;
    double eggMass = (double) DreckoTuning.EGG_MASS;
    int eggSortOrder1 = DreckoPlasticConfig.EGG_SORT_ORDER;
    List<FertilityMonitor.BreedingChance> eggChancesPlastic = DreckoTuning.EGG_CHANCES_PLASTIC;
    int eggSortOrder2 = eggSortOrder1;
    return EntityTemplates.ExtendEntityToFertileCreature(drecko, "DreckoPlasticEgg", eggName, desc, "egg_drecko_kanim", (float) eggMass, "DreckoPlasticBaby", 90f, 30f, eggChancesPlastic, eggSortOrder2);
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
