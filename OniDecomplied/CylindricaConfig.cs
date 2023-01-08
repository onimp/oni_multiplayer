// Decompiled with JetBrains decompiler
// Type: CylindricaConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class CylindricaConfig : IEntityConfig
{
  public const string ID = "Cylindrica";
  public const string SEED_ID = "CylindricaSeed";
  public static readonly EffectorValues POSITIVE_DECOR_EFFECT = TUNING.DECOR.BONUS.TIER3;
  public static readonly EffectorValues NEGATIVE_DECOR_EFFECT = TUNING.DECOR.PENALTY.TIER3;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.CYLINDRICA.NAME;
    string desc1 = (string) STRINGS.CREATURES.SPECIES.CYLINDRICA.DESC;
    EffectorValues positiveDecorEffect = CylindricaConfig.POSITIVE_DECOR_EFFECT;
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("potted_cylindricafan_kanim"));
    EffectorValues decor = positiveDecorEffect;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("Cylindrica", name1, desc1, 1f, anim1, "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, decor, noise, defaultTemperature: 298.15f);
    EntityTemplates.ExtendEntityToBasicPlant(placedEntity, 288.15f, 293.15f, 323.15f, 373.15f, new SimHashes[3]
    {
      SimHashes.Oxygen,
      SimHashes.ContaminatedOxygen,
      SimHashes.CarbonDioxide
    }, can_tinker: false, baseTraitId: "CylindricaOriginal", baseTraitName: ((string) STRINGS.CREATURES.SPECIES.CYLINDRICA.NAME));
    PrickleGrass prickleGrass = placedEntity.AddOrGet<PrickleGrass>();
    prickleGrass.positive_decor_effect = CylindricaConfig.POSITIVE_DECOR_EFFECT;
    prickleGrass.negative_decor_effect = CylindricaConfig.NEGATIVE_DECOR_EFFECT;
    GameObject plant = placedEntity;
    string name2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.CYLINDRICA.NAME;
    string desc2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.CYLINDRICA.DESC;
    KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("seed_potted_cylindricafan_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.DecorSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.CYLINDRICA.DOMESTICATEDDESC;
    EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(plant, SeedProducer.ProductionType.Hidden, "CylindricaSeed", name2, desc2, anim2, additionalTags: additionalTags, replantGroundTag: replantGroundTag, sortOrder: 12, domesticatedDescription: domesticateddesc), "Cylindrica_preview", Assets.GetAnim(HashedString.op_Implicit("potted_cylindricafan_kanim")), "place", 1, 1);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
