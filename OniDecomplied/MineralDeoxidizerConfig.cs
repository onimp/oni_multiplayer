// Decompiled with JetBrains decompiler
// Type: MineralDeoxidizerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class MineralDeoxidizerConfig : IBuildingConfig
{
  public const string ID = "MineralDeoxidizer";
  private const float ALGAE_BURN_RATE = 0.55f;
  private const float ALGAE_STORAGE = 330f;
  private const float OXYGEN_GENERATION_RATE = 0.5f;
  private const float OXYGEN_TEMPERATURE = 303.15f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = tieR3_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MineralDeoxidizer", 1, 2, "mineraldeoxidizer_kanim", 30, 30f, tieR3_1, allMetals, 800f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.ExhaustKilowattsWhenActive = 0.5f;
    buildingDef.SelfHeatKilowattsWhenActive = 1f;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
    buildingDef.ViewMode = OverlayModes.Oxygen.ID;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.Breakable = true;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    CellOffset cellOffset;
    // ISSUE: explicit constructor call
    ((CellOffset) ref cellOffset).\u002Ector(0, 1);
    Prioritizable.AddRef(go);
    Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
    electrolyzer.maxMass = 1.8f;
    electrolyzer.hasMeter = false;
    electrolyzer.emissionOffset = cellOffset;
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 330f;
    storage.showInUI = true;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
    {
      new ElementConverter.ConsumedElement(new Tag("Algae"), 0.55f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(0.5f, SimHashes.Oxygen, 303.15f, outputElementOffsetx: ((float) cellOffset.x), outputElementOffsety: ((float) cellOffset.y))
    };
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = new Tag("Algae");
    manualDeliveryKg.capacity = 330f;
    manualDeliveryKg.refillMass = 132f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    go.AddOrGetDef<PoweredActiveController.Def>();
  }
}
