// Decompiled with JetBrains decompiler
// Type: AlgaeHabitatConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class AlgaeHabitatConfig : IBuildingConfig
{
  public const string ID = "AlgaeHabitat";
  private const float ALGAE_RATE = 0.0300000012f;
  private const float WATER_RATE = 0.3f;
  private const float OXYGEN_RATE = 0.0400000028f;
  private const float CO2_RATE = 0.0003333333f;
  private const float ALGAE_CAPACITY = 90f;
  private const float WATER_CAPACITY = 360f;
  private static readonly List<Storage.StoredItemModifier> PollutedWaterStorageModifiers = new List<Storage.StoredItemModifier>()
  {
    Storage.StoredItemModifier.Hide,
    Storage.StoredItemModifier.Seal
  };

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] farmable = MATERIALS.FARMABLE;
    EffectorValues tieR0 = NOISE_POLLUTION.NOISY.TIER0;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = tieR0;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AlgaeHabitat", 1, 2, "algaefarm_kanim", 30, 30f, tieR4, farmable, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.Floodable = false;
    buildingDef.ViewMode = OverlayModes.Oxygen.ID;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
    SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_bubbles", NOISE_POLLUTION.NOISY.TIER0);
    SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_algae_in", NOISE_POLLUTION.NOISY.TIER0);
    SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_algae_out", NOISE_POLLUTION.NOISY.TIER0);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Storage storage1 = go.AddOrGet<Storage>();
    storage1.showInUI = true;
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(SimHashes.DirtyWater.CreateTag());
    List<Tag> tagList2 = tagList1;
    Tag tag1 = SimHashes.Algae.CreateTag();
    Tag tag2 = SimHashes.Water.CreateTag();
    Storage storage2 = go.AddComponent<Storage>();
    storage2.capacityKg = 360f;
    storage2.showInUI = true;
    storage2.SetDefaultStoredItemModifiers(AlgaeHabitatConfig.PollutedWaterStorageModifiers);
    storage2.allowItemRemoval = false;
    storage2.storageFilters = tagList2;
    ManualDeliveryKG manualDeliveryKg1 = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg1.SetStorage(storage1);
    manualDeliveryKg1.RequestedItemTag = tag1;
    manualDeliveryKg1.capacity = 90f;
    manualDeliveryKg1.refillMass = 18f;
    manualDeliveryKg1.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
    ManualDeliveryKG manualDeliveryKg2 = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg2.SetStorage(storage1);
    manualDeliveryKg2.RequestedItemTag = tag2;
    manualDeliveryKg2.capacity = 360f;
    manualDeliveryKg2.refillMass = 72f;
    manualDeliveryKg2.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
    KAnimFile[] kanimFileArray = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_algae_terarrium_kanim"))
    };
    AlgaeHabitatEmpty algaeHabitatEmpty = go.AddOrGet<AlgaeHabitatEmpty>();
    algaeHabitatEmpty.workTime = 5f;
    algaeHabitatEmpty.overrideAnims = kanimFileArray;
    algaeHabitatEmpty.workLayer = Grid.SceneLayer.BuildingFront;
    AlgaeHabitat algaeHabitat = go.AddOrGet<AlgaeHabitat>();
    algaeHabitat.lightBonusMultiplier = 1.1f;
    algaeHabitat.pressureSampleOffset = new CellOffset(0, 1);
    ElementConverter elementConverter = go.AddComponent<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[2]
    {
      new ElementConverter.ConsumedElement(tag1, 0.0300000012f),
      new ElementConverter.ConsumedElement(tag2, 0.3f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(0.0400000028f, SimHashes.Oxygen, 303.15f, outputElementOffsety: 1f)
    };
    go.AddComponent<ElementConverter>().outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(0.29033336f, SimHashes.DirtyWater, 303.15f, storeOutput: true, outputElementOffsety: 1f)
    };
    ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
    elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
    elementConsumer.consumptionRate = 0.0003333333f;
    elementConsumer.consumptionRadius = (byte) 3;
    elementConsumer.showInStatusPanel = true;
    elementConsumer.sampleCellOffset = new Vector3(0.0f, 1f, 0.0f);
    elementConsumer.isRequired = false;
    PassiveElementConsumer passiveElementConsumer = go.AddComponent<PassiveElementConsumer>();
    passiveElementConsumer.elementToConsume = SimHashes.Water;
    passiveElementConsumer.consumptionRate = 1.2f;
    passiveElementConsumer.consumptionRadius = (byte) 1;
    passiveElementConsumer.showDescriptor = false;
    passiveElementConsumer.storeOnConsume = true;
    passiveElementConsumer.capacityKG = 360f;
    passiveElementConsumer.showInStatusPanel = false;
    go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
    go.AddOrGet<AnimTileable>();
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
