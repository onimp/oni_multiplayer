// Decompiled with JetBrains decompiler
// Type: SwampHarvestPlantConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SwampHarvestPlantConfig : IEntityConfig
{
  public const string ID = "SwampHarvestPlant";
  public const string SEED_ID = "SwampHarvestPlantSeed";
  public const float WATER_RATE = 0.06666667f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.SWAMPHARVESTPLANT.NAME;
    string desc1 = (string) STRINGS.CREATURES.SPECIES.SWAMPHARVESTPLANT.DESC;
    EffectorValues tieR1 = TUNING.DECOR.PENALTY.TIER1;
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("swampcrop_kanim"));
    EffectorValues decor = tieR1;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("SwampHarvestPlant", name1, desc1, 1f, anim1, "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, decor, noise);
    GameObject template = placedEntity;
    string id = SwampFruitConfig.ID;
    SimHashes[] safe_elements = new SimHashes[3]
    {
      SimHashes.Oxygen,
      SimHashes.ContaminatedOxygen,
      SimHashes.CarbonDioxide
    };
    string crop_id = id;
    Tag tag = placedEntity.PrefabID();
    string name2 = ((Tag) ref tag).Name;
    EntityTemplates.ExtendEntityToBasicPlant(template, safe_elements: safe_elements, crop_id: crop_id, max_radiation: 4600f, baseTraitId: "SwampHarvestPlantOriginal", baseTraitName: name2);
    placedEntity.AddOrGet<IlluminationVulnerable>().SetPrefersDarkness(true);
    EntityTemplates.ExtendPlantToIrrigated(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = GameTags.DirtyWater,
        massConsumptionRate = 0.06666667f
      }
    });
    placedEntity.AddOrGet<StandardCropPlant>();
    placedEntity.AddOrGet<LoopingSounds>();
    GameObject plant = placedEntity;
    string name3 = (string) STRINGS.CREATURES.SPECIES.SEEDS.SWAMPHARVESTPLANT.NAME;
    string desc2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.SWAMPHARVESTPLANT.DESC;
    KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("seed_swampcrop_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.CropSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.SWAMPHARVESTPLANT.DOMESTICATEDDESC;
    EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(plant, SeedProducer.ProductionType.Harvest, "SwampHarvestPlantSeed", name3, desc2, anim2, numberOfSeeds: 0, additionalTags: additionalTags, replantGroundTag: replantGroundTag, sortOrder: 2, domesticatedDescription: domesticateddesc, width: 0.3f, height: 0.3f), "SwampHarvestPlant_preview", Assets.GetAnim(HashedString.op_Implicit("swampcrop_kanim")), "place", 1, 2);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
