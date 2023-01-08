// Decompiled with JetBrains decompiler
// Type: FilterPlantConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class FilterPlantConfig : IEntityConfig
{
  public const string ID = "FilterPlant";
  public const string SEED_ID = "FilterPlantSeed";
  public const float SAND_CONSUMPTION_RATE = 0.008333334f;
  public const float WATER_CONSUMPTION_RATE = 0.108333334f;
  public const float OXYGEN_CONSUMPTION_RATE = 0.008333334f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.FILTERPLANT.NAME;
    string desc1 = (string) STRINGS.CREATURES.SPECIES.FILTERPLANT.DESC;
    EffectorValues tieR1 = TUNING.DECOR.PENALTY.TIER1;
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("cactus_kanim"));
    EffectorValues decor = tieR1;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("FilterPlant", name1, desc1, 2f, anim1, "idle_empty", Grid.SceneLayer.BuildingFront, 1, 2, decor, noise, defaultTemperature: 348.15f);
    GameObject template = placedEntity;
    string str = SimHashes.Water.ToString();
    string name2 = (string) STRINGS.CREATURES.SPECIES.FILTERPLANT.NAME;
    SimHashes[] safe_elements = new SimHashes[1]
    {
      SimHashes.Oxygen
    };
    string crop_id = str;
    string baseTraitName = name2;
    EntityTemplates.ExtendEntityToBasicPlant(template, 253.15f, 293.15f, 383.15f, 443.15f, safe_elements, pressure_warning_low: 0.025f, crop_id: crop_id, baseTraitId: "FilterPlantOriginal", baseTraitName: baseTraitName);
    EntityTemplates.ExtendPlantToFertilizable(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = SimHashes.Sand.CreateTag(),
        massConsumptionRate = 0.008333334f
      }
    });
    EntityTemplates.ExtendPlantToIrrigated(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = GameTags.DirtyWater,
        massConsumptionRate = 0.108333334f
      }
    });
    placedEntity.AddOrGet<StandardCropPlant>();
    placedEntity.AddOrGet<SaltPlant>();
    ElementConsumer elementConsumer = placedEntity.AddOrGet<ElementConsumer>();
    elementConsumer.showInStatusPanel = true;
    elementConsumer.showDescriptor = true;
    elementConsumer.storeOnConsume = false;
    elementConsumer.elementToConsume = SimHashes.Oxygen;
    elementConsumer.configuration = ElementConsumer.Configuration.Element;
    elementConsumer.consumptionRadius = (byte) 4;
    elementConsumer.sampleCellOffset = new Vector3(0.0f, 0.0f);
    elementConsumer.consumptionRate = 0.008333334f;
    GameObject plant = placedEntity;
    string name3 = (string) STRINGS.CREATURES.SPECIES.SEEDS.FILTERPLANT.NAME;
    string desc2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.FILTERPLANT.DESC;
    KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("seed_cactus_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.CropSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.FILTERPLANT.DOMESTICATEDDESC;
    EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(plant, SeedProducer.ProductionType.Harvest, "FilterPlantSeed", name3, desc2, anim2, additionalTags: additionalTags, replantGroundTag: replantGroundTag, sortOrder: 21, domesticatedDescription: domesticateddesc, width: 0.35f, height: 0.35f), "FilterPlant_preview", Assets.GetAnim(HashedString.op_Implicit("cactus_kanim")), "place", 1, 2);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
