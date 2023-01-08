// Decompiled with JetBrains decompiler
// Type: SquirrelHugConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class SquirrelHugConfig : IEntityConfig
{
  public const string ID = "SquirrelHug";
  public const string BASE_TRAIT_ID = "SquirrelHugBaseTrait";
  public const string EGG_ID = "SquirrelHugEgg";
  public const float OXYGEN_RATE = 0.0234375037f;
  public const float BABY_OXYGEN_RATE = 0.0117187519f;
  private const SimHashes EMIT_ELEMENT = SimHashes.Dirt;
  public static float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 0.5f;
  private static float CALORIES_PER_DAY_OF_PLANT_EATEN = SquirrelTuning.STANDARD_CALORIES_PER_CYCLE / SquirrelHugConfig.DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;
  private static float KG_POOP_PER_DAY_OF_PLANT = 25f;
  private static float MIN_POOP_SIZE_KG = 40f;
  public static int EGG_SORT_ORDER = 0;

  public static GameObject CreateSquirrelHug(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseSquirrelConfig.BaseSquirrel(id, name, desc, anim_file, "SquirrelHugBaseTrait", is_baby, "hug_", true), SquirrelTuning.PEN_SIZE_PER_CREATURE_HUG);
    wildCreature.AddOrGet<DecorProvider>().SetValues(TUNING.DECOR.BONUS.TIER3);
    Trait trait = Db.Get().CreateTrait("SquirrelHugBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, SquirrelTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) SquirrelTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
    Diet.Info[] diet_infos = BaseSquirrelConfig.BasicDiet(SimHashes.Dirt.CreateTag(), SquirrelHugConfig.CALORIES_PER_DAY_OF_PLANT_EATEN, SquirrelHugConfig.KG_POOP_PER_DAY_OF_PLANT, (string) null, 0.0f);
    GameObject go = BaseSquirrelConfig.SetupDiet(wildCreature, diet_infos, SquirrelHugConfig.MIN_POOP_SIZE_KG);
    if (!is_baby)
      go.AddOrGetDef<HugMonitor.Def>();
    return go;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFertileCreature(SquirrelHugConfig.CreateSquirrelHug("SquirrelHug", (string) STRINGS.CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.NAME, (string) STRINGS.CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.DESC, "squirrel_kanim", false), "SquirrelHugEgg", (string) STRINGS.CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.EGG_NAME, (string) STRINGS.CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.DESC, "egg_squirrel_kanim", SquirrelTuning.EGG_MASS, "SquirrelHugBaby", 60.0000038f, 20f, SquirrelTuning.EGG_CHANCES_HUG, SquirrelHugConfig.EGG_SORT_ORDER);

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
