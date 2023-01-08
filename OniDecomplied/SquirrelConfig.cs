// Decompiled with JetBrains decompiler
// Type: SquirrelConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using UnityEngine;

public class SquirrelConfig : IEntityConfig
{
  public const string ID = "Squirrel";
  public const string BASE_TRAIT_ID = "SquirrelBaseTrait";
  public const string EGG_ID = "SquirrelEgg";
  public const float OXYGEN_RATE = 0.0234375037f;
  public const float BABY_OXYGEN_RATE = 0.0117187519f;
  private const SimHashes EMIT_ELEMENT = SimHashes.Dirt;
  public static float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 0.4f;
  private static float CALORIES_PER_DAY_OF_PLANT_EATEN = SquirrelTuning.STANDARD_CALORIES_PER_CYCLE / SquirrelConfig.DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;
  private static float KG_POOP_PER_DAY_OF_PLANT = 50f;
  private static float MIN_POOP_SIZE_KG = 40f;
  public static int EGG_SORT_ORDER = 0;

  public static GameObject CreateSquirrel(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseSquirrelConfig.BaseSquirrel(id, name, desc, anim_file, "SquirrelBaseTrait", is_baby), SquirrelTuning.PEN_SIZE_PER_CREATURE);
    Trait trait = Db.Get().CreateTrait("SquirrelBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, SquirrelTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) SquirrelTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
    Diet.Info[] diet_infos = BaseSquirrelConfig.BasicDiet(SimHashes.Dirt.CreateTag(), SquirrelConfig.CALORIES_PER_DAY_OF_PLANT_EATEN, SquirrelConfig.KG_POOP_PER_DAY_OF_PLANT, (string) null, 0.0f);
    double minPoopSizeKg = (double) SquirrelConfig.MIN_POOP_SIZE_KG;
    GameObject go = BaseSquirrelConfig.SetupDiet(wildCreature, diet_infos, (float) minPoopSizeKg);
    go.AddTag(GameTags.OriginalCreature);
    return go;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFertileCreature(SquirrelConfig.CreateSquirrel("Squirrel", (string) CREATURES.SPECIES.SQUIRREL.NAME, (string) CREATURES.SPECIES.SQUIRREL.DESC, "squirrel_kanim", false), "SquirrelEgg", (string) CREATURES.SPECIES.SQUIRREL.EGG_NAME, (string) CREATURES.SPECIES.SQUIRREL.DESC, "egg_squirrel_kanim", SquirrelTuning.EGG_MASS, "SquirrelBaby", 60.0000038f, 20f, SquirrelTuning.EGG_CHANCES_BASE, SquirrelConfig.EGG_SORT_ORDER);

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
