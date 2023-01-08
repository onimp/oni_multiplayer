// Decompiled with JetBrains decompiler
// Type: IceMachineConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class IceMachineConfig : IBuildingConfig
{
  public const string ID = "IceMachine";
  private const float WATER_STORAGE = 30f;
  private const float ICE_STORAGE = 150f;
  private const float WATER_INPUT_RATE = 0.5f;
  private const float ICE_OUTPUT_RATE = 0.5f;
  private const float ICE_PER_LOAD = 30f;
  private const float TARGET_ICE_TEMP = 253.15f;
  private const float KDTU_TRANSFER_RATE = 20f;
  private const float THERMAL_CONSERVATION = 0.8f;
  private float energyConsumption = 60f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("IceMachine", 2, 3, "freezerator_kanim", 30, 30f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = this.energyConsumption;
    buildingDef.ExhaustKilowattsWhenActive = 4f;
    buildingDef.SelfHeatKilowattsWhenActive = 12f;
    buildingDef.ViewMode = OverlayModes.Temperature.ID;
    buildingDef.AudioCategory = "Metal";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Storage storage = go.AddOrGet<Storage>();
    storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    storage.showInUI = true;
    storage.capacityKg = 30f;
    Storage iceStorage = go.AddComponent<Storage>();
    iceStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    iceStorage.showInUI = true;
    iceStorage.capacityKg = 150f;
    iceStorage.allowItemRemoval = true;
    iceStorage.ignoreSourcePriority = true;
    iceStorage.allowUIItemRemoval = true;
    go.AddOrGet<LoopingSounds>();
    Prioritizable.AddRef(go);
    IceMachine iceMachine = go.AddOrGet<IceMachine>();
    iceMachine.SetStorages(storage, iceStorage);
    iceMachine.targetTemperature = 253.15f;
    iceMachine.heatRemovalRate = 20f;
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = GameTags.Water;
    manualDeliveryKg.capacity = 30f;
    manualDeliveryKg.refillMass = 6f;
    manualDeliveryKg.MinimumMass = 10f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
