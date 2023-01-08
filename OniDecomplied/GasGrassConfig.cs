// Decompiled with JetBrains decompiler
// Type: GasGrassConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GasGrassConfig : IEntityConfig
{
  public const string ID = "GasGrass";
  public const string SEED_ID = "GasGrassSeed";
  public const float FERTILIZATION_RATE = 0.000833333354f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.GASGRASS.NAME;
    string desc1 = (string) STRINGS.CREATURES.SPECIES.GASGRASS.DESC;
    EffectorValues tieR3 = TUNING.DECOR.BONUS.TIER3;
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("gassygrass_kanim"));
    EffectorValues decor = tieR3;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("GasGrass", name1, desc1, 1f, anim1, "idle_empty", Grid.SceneLayer.BuildingFront, 1, 3, decor, noise, defaultTemperature: ((float) byte.MaxValue));
    EntityTemplates.ExtendEntityToBasicPlant(placedEntity, temperature_warning_low: 0.0f, temperature_warning_high: 348.15f, temperature_lethal_high: 373.15f, crop_id: "GasGrassHarvested", max_radiation: 12200f, baseTraitId: "GasGrassOriginal", baseTraitName: ((string) STRINGS.CREATURES.SPECIES.GASGRASS.NAME));
    EntityTemplates.ExtendPlantToIrrigated(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = GameTags.Chlorine,
        massConsumptionRate = 0.000833333354f
      }
    });
    placedEntity.AddOrGet<StandardCropPlant>();
    placedEntity.AddOrGet<HarvestDesignatable>().defaultHarvestStateWhenPlanted = false;
    Modifiers component = placedEntity.GetComponent<Modifiers>();
    Db.Get().traits.Get(component.initialTraits[0]).Add(new AttributeModifier(Db.Get().PlantAttributes.MinLightLux.Id, 20000f, (string) STRINGS.CREATURES.SPECIES.GASGRASS.NAME));
    component.initialAttributes.Add(Db.Get().PlantAttributes.MinLightLux.Id);
    placedEntity.AddOrGetDef<CropSleepingMonitor.Def>().prefersDarkness = false;
    GameObject plant = placedEntity;
    int num = DlcManager.FeaturePlantMutationsEnabled() ? 2 : 0;
    string name2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.GASGRASS.NAME;
    string desc2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.GASGRASS.DESC;
    KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("seed_gassygrass_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.CropSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.GASGRASS.DOMESTICATEDDESC;
    EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(plant, (SeedProducer.ProductionType) num, "GasGrassSeed", name2, desc2, anim2, additionalTags: additionalTags, replantGroundTag: replantGroundTag, sortOrder: 22, domesticatedDescription: domesticateddesc, width: 0.2f, height: 0.2f), "GasGrass_preview", Assets.GetAnim(HashedString.op_Implicit("gassygrass_kanim")), "place", 1, 1);
    SoundEventVolumeCache.instance.AddVolume("gassygrass_kanim", "GasGrass_grow", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("gassygrass_kanim", "GasGrass_harvest", NOISE_POLLUTION.CREATURES.TIER3);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
