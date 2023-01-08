// Decompiled with JetBrains decompiler
// Type: PrickleGrassConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PrickleGrassConfig : IEntityConfig
{
  public const string ID = "PrickleGrass";
  public const string SEED_ID = "PrickleGrassSeed";
  public static readonly EffectorValues POSITIVE_DECOR_EFFECT = TUNING.DECOR.BONUS.TIER3;
  public static readonly EffectorValues NEGATIVE_DECOR_EFFECT = TUNING.DECOR.PENALTY.TIER3;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.PRICKLEGRASS.NAME;
    string desc1 = (string) STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DESC;
    EffectorValues positiveDecorEffect = PrickleGrassConfig.POSITIVE_DECOR_EFFECT;
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("bristlebriar_kanim"));
    EffectorValues decor = positiveDecorEffect;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("PrickleGrass", name1, desc1, 1f, anim1, "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, decor, noise);
    EntityTemplates.ExtendEntityToBasicPlant(placedEntity, safe_elements: new SimHashes[3]
    {
      SimHashes.Oxygen,
      SimHashes.ContaminatedOxygen,
      SimHashes.CarbonDioxide
    }, can_tinker: false, max_radiation: 900f, baseTraitId: "PrickleGrassOriginal", baseTraitName: ((string) STRINGS.CREATURES.SPECIES.PRICKLEGRASS.NAME));
    PrickleGrass prickleGrass = placedEntity.AddOrGet<PrickleGrass>();
    prickleGrass.positive_decor_effect = PrickleGrassConfig.POSITIVE_DECOR_EFFECT;
    prickleGrass.negative_decor_effect = PrickleGrassConfig.NEGATIVE_DECOR_EFFECT;
    GameObject plant = placedEntity;
    string name2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.NAME;
    string desc2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.DESC;
    KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("seed_bristlebriar_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.DecorSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DOMESTICATEDDESC;
    EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(plant, SeedProducer.ProductionType.Hidden, "PrickleGrassSeed", name2, desc2, anim2, additionalTags: additionalTags, replantGroundTag: replantGroundTag, sortOrder: 10, domesticatedDescription: domesticateddesc), "PrickleGrass_preview", Assets.GetAnim(HashedString.op_Implicit("bristlebriar_kanim")), "place", 1, 1);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
