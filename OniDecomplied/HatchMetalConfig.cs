// Decompiled with JetBrains decompiler
// Type: HatchMetalConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

[EntityConfigOrder(1)]
public class HatchMetalConfig : IEntityConfig
{
  public const string ID = "HatchMetal";
  public const string BASE_TRAIT_ID = "HatchMetalBaseTrait";
  public const string EGG_ID = "HatchMetalEgg";
  private static float KG_ORE_EATEN_PER_CYCLE = 100f;
  private static float CALORIES_PER_KG_OF_ORE = HatchTuning.STANDARD_CALORIES_PER_CYCLE / HatchMetalConfig.KG_ORE_EATEN_PER_CYCLE;
  private static float MIN_POOP_SIZE_IN_KG = 10f;
  public static int EGG_SORT_ORDER = HatchConfig.EGG_SORT_ORDER + 3;

  public static HashSet<Tag> METAL_ORE_TAGS
  {
    get
    {
      HashSet<Tag> tagSet = new HashSet<Tag>();
      tagSet.Add(SimHashes.Cuprite.CreateTag());
      tagSet.Add(SimHashes.GoldAmalgam.CreateTag());
      tagSet.Add(SimHashes.IronOre.CreateTag());
      tagSet.Add(SimHashes.Wolframite.CreateTag());
      tagSet.Add(SimHashes.AluminumOre.CreateTag());
      HashSet<Tag> metalOreTags = tagSet;
      if (DlcManager.IsExpansion1Active())
        metalOreTags.Add(SimHashes.Cobaltite.CreateTag());
      return metalOreTags;
    }
  }

  public static GameObject CreateHatch(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseHatchConfig.BaseHatch(id, name, desc, anim_file, "HatchMetalBaseTrait", is_baby, "mtl_"), HatchTuning.PEN_SIZE_PER_CREATURE);
    Trait trait = Db.Get().CreateTrait("HatchMetalBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, HatchTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) HatchTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 400f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
    List<Diet.Info> diet_infos = BaseHatchConfig.MetalDiet(GameTags.Metal, HatchMetalConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, (string) null, 0.0f);
    double caloriesPerKgOfOre = (double) HatchMetalConfig.CALORIES_PER_KG_OF_ORE;
    double minPoopSizeInKg = (double) HatchMetalConfig.MIN_POOP_SIZE_IN_KG;
    return BaseHatchConfig.SetupDiet(wildCreature, diet_infos, (float) caloriesPerKgOfOre, (float) minPoopSizeInKg);
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFertileCreature(HatchMetalConfig.CreateHatch("HatchMetal", (string) STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.NAME, (string) STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.DESC, "hatch_kanim", false), "HatchMetalEgg", (string) STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.EGG_NAME, (string) STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.DESC, "egg_hatch_kanim", HatchTuning.EGG_MASS, "HatchMetalBaby", 60.0000038f, 20f, HatchTuning.EGG_CHANCES_METAL, HatchMetalConfig.EGG_SORT_ORDER);

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
