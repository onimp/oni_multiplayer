// Decompiled with JetBrains decompiler
// Type: PuftConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class PuftConfig : IEntityConfig
{
  public const string ID = "Puft";
  public const string BASE_TRAIT_ID = "PuftBaseTrait";
  public const string EGG_ID = "PuftEgg";
  public const SimHashes CONSUME_ELEMENT = SimHashes.ContaminatedOxygen;
  public const SimHashes EMIT_ELEMENT = SimHashes.SlimeMold;
  public const string EMIT_DISEASE = "SlimeLung";
  public const float EMIT_DISEASE_PER_KG = 1000f;
  private static float KG_ORE_EATEN_PER_CYCLE = 50f;
  private static float CALORIES_PER_KG_OF_ORE = PuftTuning.STANDARD_CALORIES_PER_CYCLE / PuftConfig.KG_ORE_EATEN_PER_CYCLE;
  private static float MIN_POOP_SIZE_IN_KG = 15f;
  public static int EGG_SORT_ORDER = 300;

  public static GameObject CreatePuft(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    GameObject prefab = BasePuftConfig.BasePuft(id, name, (string) STRINGS.CREATURES.SPECIES.PUFT.DESC, "PuftBaseTrait", anim_file, is_baby, (string) null, 288.15f, 328.15f);
    EntityTemplates.ExtendEntityToWildCreature(prefab, PuftTuning.PEN_SIZE_PER_CREATURE);
    Trait trait = Db.Get().CreateTrait("PuftBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PuftTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) PuftTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name));
    GameObject go = BasePuftConfig.SetupDiet(prefab, SimHashes.ContaminatedOxygen.CreateTag(), SimHashes.SlimeMold.CreateTag(), PuftConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_2, "SlimeLung", 1000f, PuftConfig.MIN_POOP_SIZE_IN_KG);
    go.AddOrGet<DiseaseSourceVisualizer>().alwaysShowDisease = "SlimeLung";
    go.AddTag(GameTags.OriginalCreature);
    return go;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFertileCreature(PuftConfig.CreatePuft("Puft", (string) STRINGS.CREATURES.SPECIES.PUFT.NAME, (string) STRINGS.CREATURES.SPECIES.PUFT.DESC, "puft_kanim", false), "PuftEgg", (string) STRINGS.CREATURES.SPECIES.PUFT.EGG_NAME, (string) STRINGS.CREATURES.SPECIES.PUFT.DESC, "egg_puft_kanim", PuftTuning.EGG_MASS, "PuftBaby", 45f, 15f, PuftTuning.EGG_CHANCES_BASE, PuftConfig.EGG_SORT_ORDER);

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst) => BasePuftConfig.OnSpawn(inst);
}
