// Decompiled with JetBrains decompiler
// Type: SaltPlantConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SaltPlantConfig : IEntityConfig
{
  public const string ID = "SaltPlant";
  public const string SEED_ID = "SaltPlantSeed";
  public const float FERTILIZATION_RATE = 0.0116666667f;
  public const float CHLORINE_CONSUMPTION_RATE = 0.006f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.SALTPLANT.NAME;
    string desc1 = (string) STRINGS.CREATURES.SPECIES.SALTPLANT.DESC;
    EffectorValues tieR1 = TUNING.DECOR.PENALTY.TIER1;
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("saltplant_kanim"));
    EffectorValues decor = tieR1;
    EffectorValues noise = new EffectorValues();
    List<Tag> additionalTags1 = new List<Tag>();
    additionalTags1.Add(GameTags.Hanging);
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("SaltPlant", name1, desc1, 2f, anim1, "idle_empty", Grid.SceneLayer.BuildingFront, 1, 2, decor, noise, additionalTags: additionalTags1, defaultTemperature: 258.15f);
    EntityTemplates.MakeHangingOffsets(placedEntity, 1, 2);
    GameObject template = placedEntity;
    string str = SimHashes.Salt.ToString();
    string name2 = (string) STRINGS.CREATURES.SPECIES.SALTPLANT.NAME;
    SimHashes[] safe_elements = new SimHashes[1]
    {
      SimHashes.ChlorineGas
    };
    string crop_id = str;
    string baseTraitName = name2;
    EntityTemplates.ExtendEntityToBasicPlant(template, 198.15f, 248.15f, 323.15f, 393.15f, safe_elements, pressure_warning_low: 0.025f, crop_id: crop_id, max_radiation: 7400f, baseTraitId: "SaltPlantOriginal", baseTraitName: baseTraitName);
    placedEntity.AddOrGet<SaltPlant>();
    EntityTemplates.ExtendPlantToFertilizable(placedEntity, new PlantElementAbsorber.ConsumeInfo[1]
    {
      new PlantElementAbsorber.ConsumeInfo()
      {
        tag = SimHashes.Sand.CreateTag(),
        massConsumptionRate = 7f / 600f
      }
    });
    Storage storage = placedEntity.AddOrGet<Storage>();
    storage.showInUI = false;
    storage.capacityKg = 1f;
    ElementConsumer elementConsumer = placedEntity.AddOrGet<ElementConsumer>();
    elementConsumer.showInStatusPanel = true;
    elementConsumer.showDescriptor = true;
    elementConsumer.storeOnConsume = false;
    elementConsumer.elementToConsume = SimHashes.ChlorineGas;
    elementConsumer.configuration = ElementConsumer.Configuration.Element;
    elementConsumer.consumptionRadius = (byte) 4;
    elementConsumer.sampleCellOffset = new Vector3(0.0f, -1f);
    elementConsumer.consumptionRate = 3f / 500f;
    placedEntity.GetComponent<UprootedMonitor>().monitorCells = new CellOffset[1]
    {
      new CellOffset(0, 1)
    };
    placedEntity.AddOrGet<StandardCropPlant>();
    GameObject plant = placedEntity;
    string name3 = (string) STRINGS.CREATURES.SPECIES.SEEDS.SALTPLANT.NAME;
    string desc2 = (string) STRINGS.CREATURES.SPECIES.SEEDS.SALTPLANT.DESC;
    KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("seed_saltplant_kanim"));
    List<Tag> additionalTags2 = new List<Tag>();
    additionalTags2.Add(GameTags.CropSeed);
    Tag replantGroundTag = new Tag();
    string domesticateddesc = (string) STRINGS.CREATURES.SPECIES.SALTPLANT.DOMESTICATEDDESC;
    EntityTemplates.MakeHangingOffsets(EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(plant, SeedProducer.ProductionType.Harvest, "SaltPlantSeed", name3, desc2, anim2, additionalTags: additionalTags2, planterDirection: SingleEntityReceptacle.ReceptacleDirection.Bottom, replantGroundTag: replantGroundTag, sortOrder: 5, domesticatedDescription: domesticateddesc, width: 0.35f, height: 0.35f), "SaltPlant_preview", Assets.GetAnim(HashedString.op_Implicit("saltplant_kanim")), "place", 1, 2), 1, 2);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst) => inst.GetComponent<ElementConsumer>().EnableConsumption(true);
}
