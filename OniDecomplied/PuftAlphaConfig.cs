// Decompiled with JetBrains decompiler
// Type: PuftAlphaConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class PuftAlphaConfig : IEntityConfig
{
  public const string ID = "PuftAlpha";
  public const string BASE_TRAIT_ID = "PuftAlphaBaseTrait";
  public const string EGG_ID = "PuftAlphaEgg";
  public const SimHashes CONSUME_ELEMENT = SimHashes.ContaminatedOxygen;
  public const SimHashes EMIT_ELEMENT = SimHashes.SlimeMold;
  public const string EMIT_DISEASE = "SlimeLung";
  public const float EMIT_DISEASE_PER_KG = 1000f;
  private static float KG_ORE_EATEN_PER_CYCLE = 30f;
  private static float CALORIES_PER_KG_OF_ORE = PuftTuning.STANDARD_CALORIES_PER_CYCLE / PuftAlphaConfig.KG_ORE_EATEN_PER_CYCLE;
  private static float MIN_POOP_SIZE_IN_KG = 5f;
  public static int EGG_SORT_ORDER = PuftConfig.EGG_SORT_ORDER + 1;

  public static GameObject CreatePuftAlpha(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    string symbol_override_prefix = "alp_";
    GameObject prefab = BasePuftConfig.BasePuft(id, name, desc, "PuftAlphaBaseTrait", anim_file, is_baby, symbol_override_prefix, 258.15f, 338.15f);
    EntityTemplates.ExtendEntityToWildCreature(prefab, PuftTuning.PEN_SIZE_PER_CREATURE);
    Trait trait = Db.Get().CreateTrait("PuftAlphaBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PuftTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) PuftTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name));
    GameObject go = BasePuftConfig.SetupDiet(prefab, new List<Diet.Info>()
    {
      new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
      {
        SimHashes.ContaminatedOxygen.CreateTag()
      }), SimHashes.SlimeMold.CreateTag(), PuftAlphaConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_2, "SlimeLung", 1000f),
      new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
      {
        SimHashes.ChlorineGas.CreateTag()
      }), SimHashes.BleachStone.CreateTag(), PuftAlphaConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_2, "SlimeLung", 1000f),
      new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
      {
        SimHashes.Oxygen.CreateTag()
      }), SimHashes.OxyRock.CreateTag(), PuftAlphaConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_2, "SlimeLung", 1000f)
    }.ToArray(), PuftAlphaConfig.CALORIES_PER_KG_OF_ORE, PuftAlphaConfig.MIN_POOP_SIZE_IN_KG);
    go.AddOrGet<DiseaseSourceVisualizer>().alwaysShowDisease = "SlimeLung";
    return go;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFertileCreature(PuftAlphaConfig.CreatePuftAlpha("PuftAlpha", (string) STRINGS.CREATURES.SPECIES.PUFT.VARIANT_ALPHA.NAME, (string) STRINGS.CREATURES.SPECIES.PUFT.VARIANT_ALPHA.DESC, "puft_kanim", false), "PuftAlphaEgg", (string) STRINGS.CREATURES.SPECIES.PUFT.VARIANT_ALPHA.EGG_NAME, (string) STRINGS.CREATURES.SPECIES.PUFT.VARIANT_ALPHA.DESC, "egg_puft_kanim", PuftTuning.EGG_MASS, "PuftAlphaBaby", 45f, 15f, PuftTuning.EGG_CHANCES_ALPHA, PuftAlphaConfig.EGG_SORT_ORDER);

  public void OnPrefabInit(GameObject inst) => inst.GetComponent<KBatchedAnimController>().animScale *= 1.1f;

  public void OnSpawn(GameObject inst) => BasePuftConfig.OnSpawn(inst);
}
