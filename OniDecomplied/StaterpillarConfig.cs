// Decompiled with JetBrains decompiler
// Type: StaterpillarConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class StaterpillarConfig : IEntityConfig
{
  public const string ID = "Staterpillar";
  public const string BASE_TRAIT_ID = "StaterpillarBaseTrait";
  public const string EGG_ID = "StaterpillarEgg";
  public const int EGG_SORT_ORDER = 0;
  private static float KG_ORE_EATEN_PER_CYCLE = 60f;
  private static float CALORIES_PER_KG_OF_ORE = StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE / StaterpillarConfig.KG_ORE_EATEN_PER_CYCLE;

  public static GameObject CreateStaterpillar(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseStaterpillarConfig.BaseStaterpillar(id, name, desc, anim_file, "StaterpillarBaseTrait", is_baby, ObjectLayer.Wire, StaterpillarGeneratorConfig.ID, Tag.Invalid), TUNING.CREATURES.SPACE_REQUIREMENTS.TIER3);
    Trait trait = Db.Get().CreateTrait("StaterpillarBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, StaterpillarTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
    List<Diet.Info> infoList = new List<Diet.Info>();
    infoList.AddRange((IEnumerable<Diet.Info>) BaseStaterpillarConfig.RawMetalDiet(SimHashes.Hydrogen.CreateTag(), StaterpillarConfig.CALORIES_PER_KG_OF_ORE, StaterpillarTuning.POOP_CONVERSTION_RATE, (string) null, 0.0f));
    infoList.AddRange((IEnumerable<Diet.Info>) BaseStaterpillarConfig.RefinedMetalDiet(SimHashes.Hydrogen.CreateTag(), StaterpillarConfig.CALORIES_PER_KG_OF_ORE, StaterpillarTuning.POOP_CONVERSTION_RATE, (string) null, 0.0f));
    List<Diet.Info> diet_infos = infoList;
    GameObject go = BaseStaterpillarConfig.SetupDiet(wildCreature, diet_infos);
    go.AddTag(GameTags.OriginalCreature);
    return go;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public virtual GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFertileCreature(StaterpillarConfig.CreateStaterpillar("Staterpillar", (string) STRINGS.CREATURES.SPECIES.STATERPILLAR.NAME, (string) STRINGS.CREATURES.SPECIES.STATERPILLAR.DESC, "caterpillar_kanim", false), "StaterpillarEgg", (string) STRINGS.CREATURES.SPECIES.STATERPILLAR.EGG_NAME, (string) STRINGS.CREATURES.SPECIES.STATERPILLAR.DESC, "egg_caterpillar_kanim", StaterpillarTuning.EGG_MASS, "StaterpillarBaby", 60.0000038f, 20f, StaterpillarTuning.EGG_CHANCES_BASE, 0);

  public void OnPrefabInit(GameObject prefab) => prefab.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("gulp"), false);

  public void OnSpawn(GameObject inst)
  {
  }
}
