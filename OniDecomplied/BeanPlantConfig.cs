// Decompiled with JetBrains decompiler
// Type: BeanPlantConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BeanPlantConfig : IEntityConfig
{
  public const string ID = "BeanPlant";
  public const string SEED_ID = "BeanPlantSeed";
  public const float FERTILIZATION_RATE = 0.008333334f;
  public const float WATER_RATE = 0.0333333351f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.BEAN_PLANT.NAME;
    string desc1 = (string) STRINGS.CREATURES.SPECIES.BEAN_PLANT.DESC;
    EffectorValues tieR1 = TUNING.DECOR.BONUS.TIER1;
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("beanplant_kanim"));
    EffectorValues decor = tieR1;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("BeanPlant", name1, desc1, 2f, anim1, "idle_empty", Grid.SceneLayer.BuildingFront, 1, 2, decor, noise, defaultTemperature: 258.15f);
    GameObject template = placedEntity;
    string name2 = (string) STRINGS.CREATURES.SPECIES.BEAN_PLANT.NAME;
    SimHashes[] safe_elements = new SimHashes[1]
    {
      SimHashes.CarbonDioxide
    };
    string baseTraitName = name2;
    EntityTemplates.ExtendEntityToBasicPlant(template, 198.15f, 248.15f, 273.15f, 323.15f, safe_elements, pressure_warning_low: 0.025f, crop_id: "BeanPlantSeed", max_radiation: 9800f, baseTraitId: "BeanPlantOriginal", baseTraitName: baseTraitName);
    EntityTemplates.ExtendPlantToIrrigated(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = SimHashes.Ethanol.CreateTag(),
        massConsumptionRate = 0.0333333351f
      }
    });
    EntityTemplates.ExtendPlantToFertilizable(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = SimHashes.Dirt.CreateTag(),
        massConsumptionRate = 0.008333334f
      }
    });
    placedEntity.AddOrGet<StandardCropPlant>();
    GameObject plant = placedEntity;
    string name3 = (string) STRINGS.CREATURES.SPECIES.SEEDS.BEAN_PLANT.NAME;
    string desc2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.BEAN_PLANT.DESC;
    KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("seed_beanplant_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.CropSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.BEAN_PLANT.DOMESTICATEDDESC;
    GameObject registerSeedForPlant = EntityTemplates.CreateAndRegisterSeedForPlant(plant, SeedProducer.ProductionType.DigOnly, "BeanPlantSeed", name3, desc2, anim2, additionalTags: additionalTags, replantGroundTag: replantGroundTag, sortOrder: 3, domesticatedDescription: domesticateddesc, collisionShape: EntityTemplates.CollisionShape.RECTANGLE, width: 0.6f, height: 0.3f, ignoreDefaultSeedTag: true);
    EntityTemplates.ExtendEntityToFood(registerSeedForPlant, TUNING.FOOD.FOOD_TYPES.BEAN);
    EntityTemplates.CreateAndRegisterPreviewForPlant(registerSeedForPlant, "BeanPlant_preview", Assets.GetAnim(HashedString.op_Implicit("beanplant_kanim")), "place", 1, 2);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
