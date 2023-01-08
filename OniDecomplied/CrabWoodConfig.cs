// Decompiled with JetBrains decompiler
// Type: CrabWoodConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

[EntityConfigOrder(1)]
public class CrabWoodConfig : IEntityConfig
{
  public const string ID = "CrabWood";
  public const string BASE_TRAIT_ID = "CrabWoodBaseTrait";
  public const string EGG_ID = "CrabWoodEgg";
  private const SimHashes EMIT_ELEMENT = SimHashes.Sand;
  private static float KG_ORE_EATEN_PER_CYCLE = 70f;
  private static float CALORIES_PER_KG_OF_ORE = CrabTuning.STANDARD_CALORIES_PER_CYCLE / CrabWoodConfig.KG_ORE_EATEN_PER_CYCLE;
  private static float MIN_POOP_SIZE_IN_KG = 25f;
  public static int EGG_SORT_ORDER = 0;
  private static string animPrefix = "wood_";

  public static GameObject CreateCrabWood(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby,
    string deathDropID = "CrabWoodShell")
  {
    GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseCrabConfig.BaseCrab(id, name, desc, anim_file, "CrabWoodBaseTrait", is_baby, CrabWoodConfig.animPrefix, deathDropID, 5), CrabTuning.PEN_SIZE_PER_CREATURE);
    Trait trait = Db.Get().CreateTrait("CrabWoodBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, CrabTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) CrabTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
    List<Diet.Info> diet_infos = BaseCrabConfig.DietWithSlime(SimHashes.Sand.CreateTag(), CrabWoodConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_1, (string) null, 0.0f);
    double caloriesPerKgOfOre = (double) CrabWoodConfig.CALORIES_PER_KG_OF_ORE;
    double minPoopSizeInKg = (double) CrabWoodConfig.MIN_POOP_SIZE_IN_KG;
    return BaseCrabConfig.SetupDiet(wildCreature, diet_infos, (float) caloriesPerKgOfOre, (float) minPoopSizeInKg);
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject fertileCreature = EntityTemplates.ExtendEntityToFertileCreature(CrabWoodConfig.CreateCrabWood("CrabWood", (string) STRINGS.CREATURES.SPECIES.CRAB.VARIANT_WOOD.NAME, (string) STRINGS.CREATURES.SPECIES.CRAB.VARIANT_WOOD.DESC, "pincher_kanim", false), "CrabWoodEgg", (string) STRINGS.CREATURES.SPECIES.CRAB.VARIANT_WOOD.EGG_NAME, (string) STRINGS.CREATURES.SPECIES.CRAB.VARIANT_WOOD.DESC, "egg_pincher_kanim", CrabTuning.EGG_MASS, "CrabWoodBaby", 60.0000038f, 20f, CrabTuning.EGG_CHANCES_WOOD, CrabWoodConfig.EGG_SORT_ORDER);
    EggProtectionMonitor.Def def1 = fertileCreature.AddOrGetDef<EggProtectionMonitor.Def>();
    def1.allyTags = new Tag[1]
    {
      GameTags.Creatures.CrabFriend
    };
    def1.animPrefix = CrabWoodConfig.animPrefix;
    MoltDropperMonitor.Def def2 = fertileCreature.AddOrGetDef<MoltDropperMonitor.Def>();
    def2.onGrowDropID = "CrabWoodShell";
    def2.massToDrop = 100f;
    def2.blockedElement = SimHashes.Ethanol;
    return fertileCreature;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
