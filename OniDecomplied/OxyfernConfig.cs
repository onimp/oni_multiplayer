// Decompiled with JetBrains decompiler
// Type: OxyfernConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class OxyfernConfig : IEntityConfig
{
  public const string ID = "Oxyfern";
  public const string SEED_ID = "OxyfernSeed";
  public const float WATER_CONSUMPTION_RATE = 0.0316666663f;
  public const float FERTILIZATION_RATE = 0.006666667f;
  public const float CO2_RATE = 0.000625000044f;
  private const float CONVERSION_RATIO = 50f;
  public const float OXYGEN_RATE = 0.0312500037f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.OXYFERN.NAME;
    string desc1 = (string) STRINGS.CREATURES.SPECIES.OXYFERN.DESC;
    EffectorValues tieR1 = TUNING.DECOR.PENALTY.TIER1;
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("oxy_fern_kanim"));
    EffectorValues decor = tieR1;
    EffectorValues noise = new EffectorValues();
    GameObject basicPlant = EntityTemplates.ExtendEntityToBasicPlant(EntityTemplates.CreatePlacedEntity("Oxyfern", name1, desc1, 1f, anim1, "idle_full", Grid.SceneLayer.BuildingBack, 1, 2, decor, noise), 253.15f, 273.15f, 313.15f, 373.15f, new SimHashes[1]
    {
      SimHashes.CarbonDioxide
    }, pressure_warning_low: 0.025f, can_tinker: false, baseTraitId: "OxyfernOriginal", baseTraitName: ((string) STRINGS.CREATURES.SPECIES.OXYFERN.NAME));
    Tag tag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
    EntityTemplates.ExtendPlantToIrrigated(basicPlant, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = tag,
        massConsumptionRate = 0.0316666663f
      }
    });
    EntityTemplates.ExtendPlantToFertilizable(basicPlant, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = GameTags.Dirt,
        massConsumptionRate = 0.006666667f
      }
    });
    basicPlant.AddOrGet<Oxyfern>();
    basicPlant.AddOrGet<LoopingSounds>();
    Storage storage = basicPlant.AddOrGet<Storage>();
    storage.showInUI = false;
    storage.capacityKg = 1f;
    ElementConsumer elementConsumer = basicPlant.AddOrGet<ElementConsumer>();
    elementConsumer.showInStatusPanel = false;
    elementConsumer.storeOnConsume = true;
    elementConsumer.storage = storage;
    elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
    elementConsumer.configuration = ElementConsumer.Configuration.Element;
    elementConsumer.consumptionRadius = (byte) 2;
    elementConsumer.EnableConsumption(true);
    elementConsumer.sampleCellOffset = new Vector3(0.0f, 0.0f);
    elementConsumer.consumptionRate = 0.000156250011f;
    ElementConverter elementConverter = basicPlant.AddOrGet<ElementConverter>();
    elementConverter.OutputMultiplier = 50f;
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
    {
      new ElementConverter.ConsumedElement(TagExtensions.ToTag(SimHashes.CarbonDioxide.ToString()), 0.000625000044f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(0.0312500037f, SimHashes.Oxygen, 0.0f, true, outputElementOffsety: 1f, diseaseWeight: 0.75f)
    };
    GameObject plant = basicPlant;
    string name2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.OXYFERN.NAME;
    string desc2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.OXYFERN.DESC;
    KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("seed_oxyfern_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.CropSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.OXYFERN.DOMESTICATEDDESC;
    EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(plant, SeedProducer.ProductionType.Hidden, "OxyfernSeed", name2, desc2, anim2, additionalTags: additionalTags, replantGroundTag: replantGroundTag, sortOrder: 20, domesticatedDescription: domesticateddesc, width: 0.3f, height: 0.3f), "Oxyfern_preview", Assets.GetAnim(HashedString.op_Implicit("oxy_fern_kanim")), "place", 1, 2);
    SoundEventVolumeCache.instance.AddVolume("oxy_fern_kanim", "MealLice_harvest", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("oxy_fern_kanim", "MealLice_LP", NOISE_POLLUTION.CREATURES.TIER4);
    return basicPlant;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst) => inst.GetComponent<Oxyfern>().SetConsumptionRate();
}
