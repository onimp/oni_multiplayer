// Decompiled with JetBrains decompiler
// Type: CritterTrapPlantConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class CritterTrapPlantConfig : IEntityConfig
{
  public const string ID = "CritterTrapPlant";
  public const float WATER_RATE = 0.0166666675f;
  public const float GAS_RATE = 0.0416666679f;
  public const float GAS_VENT_THRESHOLD = 33.25f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.CRITTERTRAPPLANT.NAME;
    string desc1 = (string) STRINGS.CREATURES.SPECIES.CRITTERTRAPPLANT.DESC;
    EffectorValues tieR1 = TUNING.DECOR.BONUS.TIER1;
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("venus_critter_trap_kanim"));
    EffectorValues decor = tieR1;
    EffectorValues noise = new EffectorValues();
    double freezing3 = (double) TUNING.CREATURES.TEMPERATURE.FREEZING_3;
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("CritterTrapPlant", name1, desc1, 4f, anim1, "idle_open", Grid.SceneLayer.BuildingBack, 1, 2, decor, noise, defaultTemperature: ((float) freezing3));
    EntityTemplates.ExtendEntityToBasicPlant(placedEntity, TUNING.CREATURES.TEMPERATURE.FREEZING_10, TUNING.CREATURES.TEMPERATURE.FREEZING_9, TUNING.CREATURES.TEMPERATURE.FREEZING, TUNING.CREATURES.TEMPERATURE.COOL, pressure_sensitive: false, crop_id: "PlantMeat", should_grow_old: false, baseTraitId: "CritterTrapPlantOriginal", baseTraitName: ((string) STRINGS.CREATURES.SPECIES.CRITTERTRAPPLANT.NAME));
    Object.DestroyImmediate((Object) placedEntity.GetComponent<MutantPlant>());
    TrapTrigger trapTrigger = placedEntity.AddOrGet<TrapTrigger>();
    trapTrigger.trappableCreatures = new Tag[2]
    {
      GameTags.Creatures.Walker,
      GameTags.Creatures.Hoverer
    };
    trapTrigger.trappedOffset = new Vector2(0.5f, 0.0f);
    ((Behaviour) trapTrigger).enabled = false;
    CritterTrapPlant critterTrapPlant = placedEntity.AddOrGet<CritterTrapPlant>();
    critterTrapPlant.gasOutputRate = 0.0416666679f;
    critterTrapPlant.outputElement = SimHashes.Hydrogen;
    critterTrapPlant.gasVentThreshold = 33.25f;
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.AddOrGet<Storage>();
    Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
    EntityTemplates.ExtendPlantToIrrigated(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = tag,
        massConsumptionRate = 0.0166666675f
      }
    });
    GameObject plant = placedEntity;
    string name2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.CRITTERTRAPPLANT.NAME;
    string desc2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.CRITTERTRAPPLANT.DESC;
    KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("seed_critter_trap_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.CropSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.CRITTERTRAPPLANT.DOMESTICATEDDESC;
    EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(plant, SeedProducer.ProductionType.Hidden, "CritterTrapPlantSeed", name2, desc2, anim2, additionalTags: additionalTags, replantGroundTag: replantGroundTag, sortOrder: 21, domesticatedDescription: domesticateddesc, width: 0.3f, height: 0.3f), "CritterTrapPlant_preview", Assets.GetAnim(HashedString.op_Implicit("venus_critter_trap_kanim")), "place", 1, 2);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
