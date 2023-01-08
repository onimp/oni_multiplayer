// Decompiled with JetBrains decompiler
// Type: CrabConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

[EntityConfigOrder(1)]
public class CrabConfig : IEntityConfig
{
  public const string ID = "Crab";
  public const string BASE_TRAIT_ID = "CrabBaseTrait";
  public const string EGG_ID = "CrabEgg";
  private const SimHashes EMIT_ELEMENT = SimHashes.Sand;
  private static float KG_ORE_EATEN_PER_CYCLE = 70f;
  private static float CALORIES_PER_KG_OF_ORE = CrabTuning.STANDARD_CALORIES_PER_CYCLE / CrabConfig.KG_ORE_EATEN_PER_CYCLE;
  private static float MIN_POOP_SIZE_IN_KG = 25f;
  public static int EGG_SORT_ORDER = 0;

  public static GameObject CreateCrab(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby,
    string deathDropID)
  {
    GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseCrabConfig.BaseCrab(id, name, desc, anim_file, "CrabBaseTrait", is_baby, onDeathDropID: deathDropID), CrabTuning.PEN_SIZE_PER_CREATURE);
    Trait trait = Db.Get().CreateTrait("CrabBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, CrabTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) CrabTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
    List<Diet.Info> diet_infos = BaseCrabConfig.BasicDiet(SimHashes.Sand.CreateTag(), CrabConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, (string) null, 0.0f);
    double caloriesPerKgOfOre = (double) CrabConfig.CALORIES_PER_KG_OF_ORE;
    double minPoopSizeInKg = (double) CrabConfig.MIN_POOP_SIZE_IN_KG;
    GameObject go = BaseCrabConfig.SetupDiet(wildCreature, diet_infos, (float) caloriesPerKgOfOre, (float) minPoopSizeInKg);
    go.AddTag(GameTags.OriginalCreature);
    return go;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject fertileCreature = EntityTemplates.ExtendEntityToFertileCreature(CrabConfig.CreateCrab("Crab", (string) STRINGS.CREATURES.SPECIES.CRAB.NAME, (string) STRINGS.CREATURES.SPECIES.CRAB.DESC, "pincher_kanim", false, "CrabShell"), "CrabEgg", (string) STRINGS.CREATURES.SPECIES.CRAB.EGG_NAME, (string) STRINGS.CREATURES.SPECIES.CRAB.DESC, "egg_pincher_kanim", CrabTuning.EGG_MASS, "CrabBaby", 60.0000038f, 20f, CrabTuning.EGG_CHANCES_BASE, CrabConfig.EGG_SORT_ORDER);
    fertileCreature.AddOrGetDef<EggProtectionMonitor.Def>().allyTags = new Tag[1]
    {
      GameTags.Creatures.CrabFriend
    };
    return fertileCreature;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
