// Decompiled with JetBrains decompiler
// Type: SublimationStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class SublimationStationConfig : IBuildingConfig
{
  public const string ID = "SublimationStation";
  private const float DIRT_CONSUME_RATE = 1f;
  private const float DIRT_STORAGE = 600f;
  private const float OXYGEN_GENERATION_RATE = 0.66f;
  private const float OXYGEN_TEMPERATURE = 303.15f;

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = tieR3_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SublimationStation", 2, 1, "sublimation_station_kanim", 30, 30f, tieR3_1, allMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 60f;
    buildingDef.ExhaustKilowattsWhenActive = 0.5f;
    buildingDef.SelfHeatKilowattsWhenActive = 1f;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
    buildingDef.ViewMode = OverlayModes.Oxygen.ID;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.Breakable = true;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Prioritizable.AddRef(go);
    CellOffset cellOffset;
    // ISSUE: explicit constructor call
    ((CellOffset) ref cellOffset).\u002Ector(0, 0);
    Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
    electrolyzer.maxMass = 1.8f;
    electrolyzer.hasMeter = false;
    electrolyzer.emissionOffset = cellOffset;
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 600f;
    storage.showInUI = true;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
    {
      new ElementConverter.ConsumedElement(SimHashes.ToxicSand.CreateTag(), 1f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(0.66f, SimHashes.ContaminatedOxygen, 303.15f, outputElementOffsetx: ((float) cellOffset.x), outputElementOffsety: ((float) cellOffset.y))
    };
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = SimHashes.ToxicSand.CreateTag();
    manualDeliveryKg.capacity = 600f;
    manualDeliveryKg.refillMass = 240f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
