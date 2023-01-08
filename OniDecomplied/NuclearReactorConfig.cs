// Decompiled with JetBrains decompiler
// Type: NuclearReactorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class NuclearReactorConfig : IBuildingConfig
{
  public const string ID = "NuclearReactor";
  private const float FUEL_CAPACITY = 180f;
  public const float VENT_STEAM_TEMPERATURE = 673.15f;
  public const float MELT_DOWN_TEMPERATURE = 3000f;
  public const float MAX_VENT_PRESSURE = 150f;
  public const float INCREASED_CONDUCTION_SCALE = 5f;
  public const float REACTION_STRENGTH = 100f;
  public const int RADIATION_EMITTER_RANGE = 25;
  public const float OPERATIONAL_RADIATOR_INTENSITY = 2400f;
  public const float MELT_DOWN_RADIATOR_INTENSITY = 4800f;
  public const float FUEL_CONSUMPTION_SPEED = 0.0166666675f;
  public const float BEGIN_REACTION_MASS = 0.5f;
  public const float STOP_REACTION_MASS = 0.25f;
  public const float DUMP_WASTE_AMOUNT = 100f;
  public const float WASTE_MASS_MULTIPLIER = 100f;
  public const float REACTION_MASS_TARGET = 60f;
  public const float COOLANT_AMOUNT = 30f;
  public const float COOLANT_CAPACITY = 90f;
  public const float MINIMUM_COOLANT_MASS = 30f;
  public const float WASTE_GERMS_PER_KG = 50f;
  public const float PST_MELTDOWN_COOLING_TIME = 3000f;
  public const string INPUT_PORT_ID = "CONTROL_FUEL_DELIVERY";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR5_2 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR5_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("NuclearReactor", 5, 6, "generatornuclear_kanim", 100, 480f, tieR5_1, refinedMetals, 9999f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.GeneratorWattageRating = 0.0f;
    buildingDef.GeneratorBaseCapacity = 10000f;
    buildingDef.RequiresPowerInput = false;
    buildingDef.RequiresPowerOutput = false;
    buildingDef.ThermalConductivity = 0.1f;
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
    buildingDef.Overheatable = false;
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.UtilityInputOffset = new CellOffset(-2, 2);
    buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.InputPort(HashedString.op_Implicit("CONTROL_FUEL_DELIVERY"), new CellOffset(0, 0), (string) STRINGS.BUILDINGS.PREFABS.NUCLEARREACTOR.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.NUCLEARREACTOR.INPUT_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.NUCLEARREACTOR.INPUT_PORT_INACTIVE, display_custom_name: true)
    };
    buildingDef.ViewMode = OverlayModes.Temperature.ID;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.AudioSize = "large";
    buildingDef.Floodable = false;
    buildingDef.Entombable = false;
    buildingDef.Breakable = false;
    buildingDef.Invincible = true;
    buildingDef.Deprecated = !Sim.IsRadiationEnabled();
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    Object.Destroy((Object) go.GetComponent<BuildingEnabledButton>());
    RadiationEmitter radiationEmitter = go.AddComponent<RadiationEmitter>();
    radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
    radiationEmitter.emitRadiusX = (short) 25;
    radiationEmitter.emitRadiusY = (short) 25;
    radiationEmitter.radiusProportionalToRads = false;
    radiationEmitter.emissionOffset = new Vector3(0.0f, 2f, 0.0f);
    Storage storage = go.AddComponent<Storage>();
    storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Seal,
      Storage.StoredItemModifier.Insulate,
      Storage.StoredItemModifier.Hide
    });
    go.AddComponent<Storage>().SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Seal,
      Storage.StoredItemModifier.Insulate,
      Storage.StoredItemModifier.Hide
    });
    go.AddComponent<Storage>().SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Seal,
      Storage.StoredItemModifier.Insulate,
      Storage.StoredItemModifier.Hide
    });
    ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg.RequestedItemTag = ElementLoader.FindElementByHash(SimHashes.EnrichedUranium).tag;
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;
    manualDeliveryKg.capacity = 180f;
    manualDeliveryKg.MinimumMass = 0.5f;
    go.AddOrGet<Reactor>();
    go.AddOrGet<LoopingSounds>();
    Prioritizable.AddRef(go);
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.consumptionRate = 10f;
    conduitConsumer.capacityKG = 90f;
    conduitConsumer.capacityTag = GameTags.AnyWater;
    conduitConsumer.forceAlwaysSatisfied = true;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
    conduitConsumer.storage = storage;
  }

  public override void DoPostConfigureComplete(GameObject go) => go.AddTag(GameTags.CorrosionProof);
}
