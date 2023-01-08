// Decompiled with JetBrains decompiler
// Type: RadiationLightConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class RadiationLightConfig : IBuildingConfig
{
  public const string ID = "RadiationLight";
  private Tag FUEL_ELEMENT = SimHashes.UraniumOre.CreateTag();
  private SimHashes WASTE_ELEMENT = SimHashes.DepletedUranium;
  private const float FUEL_PER_CYCLE = 10f;
  private const float CYCLES_PER_REFILL = 5f;
  private const float FUEL_TO_WASTE_RATIO = 0.5f;
  private const float FUEL_STORAGE_AMOUNT = 50f;
  private const float FUEL_CONSUMPTION_RATE = 0.0166666675f;
  private const short RAD_LIGHT_SIZE_X = 16;
  private const short RAD_LIGHT_SIZE_Y = 4;

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RadiationLight", 1, 1, "radiation_lamp_kanim", 10, 10f, tieR1, allMetals, 800f, BuildLocationRule.OnWall, none2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 60f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
    buildingDef.ViewMode = OverlayModes.Radiation.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PermittedRotations = PermittedRotations.FlipH;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    Prioritizable.AddRef(go);
    Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
    defaultStorage.showInUI = true;
    defaultStorage.capacityKg = 50f;
    defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(defaultStorage);
    manualDeliveryKg.RequestedItemTag = this.FUEL_ELEMENT;
    manualDeliveryKg.capacity = 50f;
    manualDeliveryKg.refillMass = 5f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
    RadiationEmitter radiationEmitter = go.AddComponent<RadiationEmitter>();
    radiationEmitter.emitAngle = 90f;
    radiationEmitter.emitDirection = 0.0f;
    radiationEmitter.emissionOffset = Vector3.right;
    radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
    radiationEmitter.emitRadiusX = (short) 16;
    radiationEmitter.emitRadiusY = (short) 4;
    radiationEmitter.emitRads = 240f;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
    {
      new ElementConverter.ConsumedElement(this.FUEL_ELEMENT, 0.0166666675f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(0.008333334f, this.WASTE_ELEMENT, 0.0f, storeOutput: true, diseaseWeight: 0.5f)
    };
    ElementDropper elementDropper = go.AddOrGet<ElementDropper>();
    elementDropper.emitTag = this.WASTE_ELEMENT.CreateTag();
    elementDropper.emitMass = 5f;
    RadiationLight radiationLight = go.AddComponent<RadiationLight>();
    radiationLight.elementToConsume = this.FUEL_ELEMENT;
    radiationLight.consumptionRate = 0.0166666675f;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
  }
}
