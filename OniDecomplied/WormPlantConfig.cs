// Decompiled with JetBrains decompiler
// Type: WormPlantConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class WormPlantConfig : IEntityConfig
{
  public const string ID = "WormPlant";
  public const string SEED_ID = "WormPlantSeed";
  public const float SULFUR_CONSUMPTION_RATE = 0.0166666675f;
  public static readonly EffectorValues BASIC_DECOR = TUNING.DECOR.PENALTY.TIER0;
  public const string BASIC_CROP_ID = "WormBasicFruit";
  private static StandardCropPlant.AnimSet animSet = new StandardCropPlant.AnimSet()
  {
    grow = "basic_grow",
    grow_pst = "basic_grow_pst",
    idle_full = "basic_idle_full",
    wilt_base = "basic_wilt",
    harvest = "basic_harvest"
  };

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public static GameObject BaseWormPlant(
    string id,
    string name,
    string desc,
    string animFile,
    EffectorValues decor,
    string cropID)
  {
    string id1 = id;
    string name1 = name;
    string desc1 = desc;
    EffectorValues effectorValues = decor;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(animFile));
    EffectorValues decor1 = effectorValues;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc1, 1f, anim, "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, decor1, noise, defaultTemperature: 307.15f);
    GameObject template = placedEntity;
    string str = cropID;
    SimHashes[] safe_elements = new SimHashes[3]
    {
      SimHashes.Oxygen,
      SimHashes.ContaminatedOxygen,
      SimHashes.CarbonDioxide
    };
    string crop_id = str;
    string baseTraitId = id + "Original";
    string baseTraitName = name;
    EntityTemplates.ExtendEntityToBasicPlant(template, 273.15f, 288.15f, 323.15f, 373.15f, safe_elements, crop_id: crop_id, max_radiation: 9800f, baseTraitId: baseTraitId, baseTraitName: baseTraitName);
    EntityTemplates.ExtendPlantToFertilizable(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = SimHashes.Sulfur.CreateTag(),
        massConsumptionRate = 0.0166666675f
      }
    });
    placedEntity.AddOrGet<StandardCropPlant>();
    placedEntity.AddOrGet<LoopingSounds>();
    return placedEntity;
  }

  public GameObject CreatePrefab()
  {
    GameObject plant = WormPlantConfig.BaseWormPlant("WormPlant", (string) STRINGS.CREATURES.SPECIES.WORMPLANT.NAME, (string) STRINGS.CREATURES.SPECIES.WORMPLANT.DESC, "wormwood_kanim", WormPlantConfig.BASIC_DECOR, "WormBasicFruit");
    string name = (string) STRINGS.CREATURES.SPECIES.SEEDS.WORMPLANT.NAME;
    string desc = (string) STRINGS.CREATURES.SPECIES.SEEDS.WORMPLANT.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("seed_wormwood_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.CropSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.WORMPLANT.DOMESTICATEDDESC;
    EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(plant, SeedProducer.ProductionType.Harvest, "WormPlantSeed", name, desc, anim, numberOfSeeds: 0, additionalTags: additionalTags, replantGroundTag: replantGroundTag, sortOrder: 3, domesticatedDescription: domesticateddesc, width: 0.3f, height: 0.3f), "WormPlant_preview", Assets.GetAnim(HashedString.op_Implicit("wormwood_kanim")), "place", 1, 2);
    return plant;
  }

  public void OnPrefabInit(GameObject prefab)
  {
    TransformingPlant transformingPlant = prefab.AddOrGet<TransformingPlant>();
    transformingPlant.transformPlantId = "SuperWormPlant";
    transformingPlant.SubscribeToTransformEvent(GameHashes.CropTended);
    transformingPlant.useGrowthTimeRatio = true;
    transformingPlant.eventDataCondition = (Func<object, bool>) (data =>
    {
      CropTendingStates.CropTendingEventData tendingEventData = (CropTendingStates.CropTendingEventData) data;
      if (tendingEventData != null)
      {
        CreatureBrain component = tendingEventData.source.GetComponent<CreatureBrain>();
        if (Object.op_Inequality((Object) component, (Object) null) && Tag.op_Equality(component.species, GameTags.Creatures.Species.DivergentSpecies))
          return true;
      }
      return false;
    });
    transformingPlant.fxKAnim = "plant_transform_fx_kanim";
    transformingPlant.fxAnim = "plant_transform";
    prefab.AddOrGet<StandardCropPlant>().anims = WormPlantConfig.animSet;
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
