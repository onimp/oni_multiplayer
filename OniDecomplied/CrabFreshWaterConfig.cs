// Decompiled with JetBrains decompiler
// Type: CrabFreshWaterConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

[EntityConfigOrder(1)]
public class CrabFreshWaterConfig : IEntityConfig
{
  public const string ID = "CrabFreshWater";
  public const string BASE_TRAIT_ID = "CrabFreshWaterBaseTrait";
  public const string EGG_ID = "CrabFreshWaterEgg";
  private const SimHashes EMIT_ELEMENT = SimHashes.Sand;
  private static float KG_ORE_EATEN_PER_CYCLE = 70f;
  private static float CALORIES_PER_KG_OF_ORE = CrabTuning.STANDARD_CALORIES_PER_CYCLE / CrabFreshWaterConfig.KG_ORE_EATEN_PER_CYCLE;
  private static float MIN_POOP_SIZE_IN_KG = 25f;
  public static int EGG_SORT_ORDER = 0;
  private static string animPrefix = "fresh_";

  public static GameObject CreateCrabFreshWater(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby,
    string deathDropID = null)
  {
    GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseCrabConfig.BaseCrab(id, name, desc, anim_file, "CrabFreshWaterBaseTrait", is_baby, CrabFreshWaterConfig.animPrefix, deathDropID), CrabTuning.PEN_SIZE_PER_CREATURE);
    Trait trait = Db.Get().CreateTrait("CrabFreshWaterBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, CrabTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) CrabTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
    List<Diet.Info> diet_infos = BaseCrabConfig.DietWithSlime(SimHashes.Sand.CreateTag(), CrabFreshWaterConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, (string) null, 0.0f);
    GameObject crabFreshWater = BaseCrabConfig.SetupDiet(wildCreature, diet_infos, CrabFreshWaterConfig.CALORIES_PER_KG_OF_ORE, CrabFreshWaterConfig.MIN_POOP_SIZE_IN_KG);
    Butcherable component = crabFreshWater.GetComponent<Butcherable>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      string[] drops = new string[4]
      {
        "ShellfishMeat",
        "ShellfishMeat",
        "ShellfishMeat",
        "ShellfishMeat"
      };
      component.SetDrops(drops);
    }
    return crabFreshWater;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject fertileCreature = EntityTemplates.ExtendEntityToFertileCreature(CrabFreshWaterConfig.CreateCrabFreshWater("CrabFreshWater", (string) STRINGS.CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.NAME, (string) STRINGS.CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.DESC, "pincher_kanim", false), "CrabFreshWaterEgg", (string) STRINGS.CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.EGG_NAME, (string) STRINGS.CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.DESC, "egg_pincher_kanim", CrabTuning.EGG_MASS, "CrabFreshWaterBaby", 60.0000038f, 20f, CrabTuning.EGG_CHANCES_FRESH, CrabFreshWaterConfig.EGG_SORT_ORDER);
    EggProtectionMonitor.Def def1 = fertileCreature.AddOrGetDef<EggProtectionMonitor.Def>();
    def1.allyTags = new Tag[1]
    {
      GameTags.Creatures.CrabFriend
    };
    def1.animPrefix = CrabFreshWaterConfig.animPrefix;
    DiseaseEmitter diseaseEmitter = fertileCreature.AddComponent<DiseaseEmitter>();
    List<Klei.AI.Disease> diseases = new List<Klei.AI.Disease>()
    {
      Db.Get().Diseases.FoodGerms,
      Db.Get().Diseases.PollenGerms,
      Db.Get().Diseases.SlimeGerms,
      Db.Get().Diseases.ZombieSpores
    };
    if (DlcManager.IsExpansion1Active())
      diseases.Add(Db.Get().Diseases.RadiationPoisoning);
    diseaseEmitter.SetDiseases(diseases);
    diseaseEmitter.emitRange = (byte) 2;
    diseaseEmitter.emitCount = -1 * Mathf.RoundToInt(888.8889f);
    CleaningMonitor.Def def2 = fertileCreature.AddOrGetDef<CleaningMonitor.Def>();
    def2.elementState = Element.State.Liquid;
    def2.cellOffsets = new CellOffset[5]
    {
      new CellOffset(1, 0),
      new CellOffset(-1, 0),
      new CellOffset(0, 1),
      new CellOffset(-1, 1),
      new CellOffset(1, 1)
    };
    def2.coolDown = 30f;
    return fertileCreature;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
