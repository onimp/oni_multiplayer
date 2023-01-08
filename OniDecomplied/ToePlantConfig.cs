// Decompiled with JetBrains decompiler
// Type: ToePlantConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ToePlantConfig : IEntityConfig
{
  public const string ID = "ToePlant";
  public const string SEED_ID = "ToePlantSeed";
  public static readonly EffectorValues POSITIVE_DECOR_EFFECT = TUNING.DECOR.BONUS.TIER3;
  public static readonly EffectorValues NEGATIVE_DECOR_EFFECT = TUNING.DECOR.PENALTY.TIER3;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.TOEPLANT.NAME;
    string desc1 = (string) STRINGS.CREATURES.SPECIES.TOEPLANT.DESC;
    EffectorValues positiveDecorEffect = ToePlantConfig.POSITIVE_DECOR_EFFECT;
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("potted_toes_kanim"));
    EffectorValues decor = positiveDecorEffect;
    EffectorValues noise = new EffectorValues();
    double freezing3 = (double) TUNING.CREATURES.TEMPERATURE.FREEZING_3;
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("ToePlant", name1, desc1, 1f, anim1, "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, decor, noise, defaultTemperature: ((float) freezing3));
    GameObject template = placedEntity;
    SimHashes[] simHashesArray = new SimHashes[3]
    {
      SimHashes.Oxygen,
      SimHashes.ContaminatedOxygen,
      SimHashes.CarbonDioxide
    };
    double freezing10 = (double) TUNING.CREATURES.TEMPERATURE.FREEZING_10;
    double freezing9 = (double) TUNING.CREATURES.TEMPERATURE.FREEZING_9;
    double freezing = (double) TUNING.CREATURES.TEMPERATURE.FREEZING;
    double cool = (double) TUNING.CREATURES.TEMPERATURE.COOL;
    SimHashes[] safe_elements = simHashesArray;
    string name2 = (string) STRINGS.CREATURES.SPECIES.TOEPLANT.NAME;
    EntityTemplates.ExtendEntityToBasicPlant(template, (float) freezing10, (float) freezing9, (float) freezing, (float) cool, safe_elements, can_tinker: false, baseTraitId: "ToePlantOriginal", baseTraitName: name2);
    PrickleGrass prickleGrass = placedEntity.AddOrGet<PrickleGrass>();
    prickleGrass.positive_decor_effect = ToePlantConfig.POSITIVE_DECOR_EFFECT;
    prickleGrass.negative_decor_effect = ToePlantConfig.NEGATIVE_DECOR_EFFECT;
    GameObject plant = placedEntity;
    string name3 = (string) STRINGS.CREATURES.SPECIES.SEEDS.TOEPLANT.NAME;
    string desc2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.TOEPLANT.DESC;
    KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("seed_potted_toes_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.DecorSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.TOEPLANT.DOMESTICATEDDESC;
    EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(plant, SeedProducer.ProductionType.Hidden, "ToePlantSeed", name3, desc2, anim2, additionalTags: additionalTags, replantGroundTag: replantGroundTag, sortOrder: 12, domesticatedDescription: domesticateddesc), "ToePlant_preview", Assets.GetAnim(HashedString.op_Implicit("potted_toes_kanim")), "place", 1, 1);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
