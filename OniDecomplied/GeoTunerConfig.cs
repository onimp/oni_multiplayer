// Decompiled with JetBrains decompiler
// Type: GeoTunerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GeoTunerConfig : IBuildingConfig
{
  public const int MAX_GEOTUNED = 5;
  public static Dictionary<GeoTunerConfig.Category, GeoTunerConfig.GeotunedGeyserSettings> CategorySettings = new Dictionary<GeoTunerConfig.Category, GeoTunerConfig.GeotunedGeyserSettings>()
  {
    [GeoTunerConfig.Category.DEFAULT_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings()
    {
      material = SimHashes.Dirt.CreateTag(),
      quantity = 50f,
      duration = 600f,
      template = new Geyser.GeyserModification()
      {
        massPerCycleModifier = 0.1f,
        temperatureModifier = 10f,
        iterationDurationModifier = 0.0f,
        iterationPercentageModifier = 0.0f,
        yearDurationModifier = 0.0f,
        yearPercentageModifier = 0.0f,
        maxPressureModifier = 0.0f
      }
    },
    [GeoTunerConfig.Category.WATER_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings()
    {
      material = SimHashes.BleachStone.CreateTag(),
      quantity = 50f,
      duration = 600f,
      template = new Geyser.GeyserModification()
      {
        massPerCycleModifier = 0.2f,
        temperatureModifier = 20f,
        iterationDurationModifier = 0.0f,
        iterationPercentageModifier = 0.0f,
        yearDurationModifier = 0.0f,
        yearPercentageModifier = 0.0f,
        maxPressureModifier = 0.0f
      }
    },
    [GeoTunerConfig.Category.ORGANIC_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings()
    {
      material = SimHashes.Salt.CreateTag(),
      quantity = 50f,
      duration = 600f,
      template = new Geyser.GeyserModification()
      {
        massPerCycleModifier = 0.2f,
        temperatureModifier = 15f,
        iterationDurationModifier = 0.0f,
        iterationPercentageModifier = 0.0f,
        yearDurationModifier = 0.0f,
        yearPercentageModifier = 0.0f,
        maxPressureModifier = 0.0f
      }
    },
    [GeoTunerConfig.Category.HYDROCARBON_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings()
    {
      material = SimHashes.Katairite.CreateTag(),
      quantity = 100f,
      duration = 600f,
      template = new Geyser.GeyserModification()
      {
        massPerCycleModifier = 0.2f,
        temperatureModifier = 15f,
        iterationDurationModifier = 0.0f,
        iterationPercentageModifier = 0.0f,
        yearDurationModifier = 0.0f,
        yearPercentageModifier = 0.0f,
        maxPressureModifier = 0.0f
      }
    },
    [GeoTunerConfig.Category.VOLCANO_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings()
    {
      material = SimHashes.Katairite.CreateTag(),
      quantity = 100f,
      duration = 600f,
      template = new Geyser.GeyserModification()
      {
        massPerCycleModifier = 0.2f,
        temperatureModifier = 150f,
        iterationDurationModifier = 0.0f,
        iterationPercentageModifier = 0.0f,
        yearDurationModifier = 0.0f,
        yearPercentageModifier = 0.0f,
        maxPressureModifier = 0.0f
      }
    },
    [GeoTunerConfig.Category.METALS_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings()
    {
      material = SimHashes.Phosphorus.CreateTag(),
      quantity = 80f,
      duration = 600f,
      template = new Geyser.GeyserModification()
      {
        massPerCycleModifier = 0.2f,
        temperatureModifier = 50f,
        iterationDurationModifier = 0.0f,
        iterationPercentageModifier = 0.0f,
        yearDurationModifier = 0.0f,
        yearPercentageModifier = 0.0f,
        maxPressureModifier = 0.0f
      }
    },
    [GeoTunerConfig.Category.CO2_CATEGORY] = new GeoTunerConfig.GeotunedGeyserSettings()
    {
      material = SimHashes.ToxicSand.CreateTag(),
      quantity = 50f,
      duration = 600f,
      template = new Geyser.GeyserModification()
      {
        massPerCycleModifier = 0.2f,
        temperatureModifier = 5f,
        iterationDurationModifier = 0.0f,
        iterationPercentageModifier = 0.0f,
        yearDurationModifier = 0.0f,
        yearPercentageModifier = 0.0f,
        maxPressureModifier = 0.0f
      }
    }
  };
  public static Dictionary<HashedString, GeoTunerConfig.GeotunedGeyserSettings> geotunerGeyserSettings;
  public const string ID = "GeoTuner";
  public const string OUTPUT_LOGIC_PORT_ID = "GEYSER_ERUPTION_STATUS_PORT";
  public const string GeyserAnimationModelTarget = "geyser_target";
  public const string GeyserAnimation_GeyserSymbols_LogicLightSymbol = "light_bloom";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GeoTuner", 4, 3, "geoTuner_kanim", 30, 120f, tieR4, refinedMetals, 2400f, BuildLocationRule.OnFloor, none2, noise);
    buildingDef.Floodable = true;
    buildingDef.Entombable = true;
    buildingDef.Overheatable = false;
    buildingDef.ObjectLayer = ObjectLayer.Building;
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "medium";
    buildingDef.PermittedRotations = PermittedRotations.FlipH;
    buildingDef.UseStructureTemperature = true;
    buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.OutputPort(HashedString.op_Implicit("GEYSER_ERUPTION_STATUS_PORT"), new CellOffset(-1, 1), (string) STRINGS.BUILDINGS.PREFABS.GEOTUNER.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.GEOTUNER.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.GEOTUNER.LOGIC_PORT_INACTIVE)
    };
    buildingDef.RequiresPowerInput = true;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.ExhaustKilowattsWhenActive = 0.5f;
    buildingDef.SelfHeatKilowattsWhenActive = 4f;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.ScienceBuilding, false);
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 0.0f;
    List<Storage.StoredItemModifier> modifiers = new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Hide,
      Storage.StoredItemModifier.Seal,
      Storage.StoredItemModifier.Insulate,
      Storage.StoredItemModifier.Preserve
    };
    storage.SetDefaultStoredItemModifiers(modifiers);
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.Fetch.IdHash;
    manualDeliveryKg.capacity = 0.0f;
    manualDeliveryKg.refillMass = 0.0f;
    manualDeliveryKg.SetStorage(storage);
    go.AddOrGet<GeoTunerWorkable>();
    go.AddOrGet<GeoTunerSwitchGeyserWorkable>();
    go.AddOrGet<CopyBuildingSettings>();
    GeoTuner.Def def = go.AddOrGetDef<GeoTuner.Def>();
    def.OUTPUT_LOGIC_PORT_ID = "GEYSER_ERUPTION_STATUS_PORT";
    def.geotunedGeyserSettings = GeoTunerConfig.geotunerGeyserSettings;
    def.defaultSetting = GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.DEFAULT_CATEGORY];
    RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.Laboratory.Id;
    roomTracker.requirement = RoomTracker.Requirement.Required;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }

  static GeoTunerConfig()
  {
    Dictionary<HashedString, GeoTunerConfig.GeotunedGeyserSettings> dictionary = new Dictionary<HashedString, GeoTunerConfig.GeotunedGeyserSettings>();
    dictionary.Add(HashedString.op_Implicit("steam"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("hot_steam"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("slimy_po2"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.ORGANIC_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("hot_po2"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.ORGANIC_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("methane"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.HYDROCARBON_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("chlorine_gas"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.ORGANIC_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("hot_co2"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.CO2_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("hot_hydrogen"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.HYDROCARBON_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("hot_water"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("salt_water"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("slush_salt_water"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("filthy_water"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("slush_water"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.WATER_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("liquid_sulfur"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.HYDROCARBON_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("liquid_co2"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.CO2_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("oil_drip"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.HYDROCARBON_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("small_volcano"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.VOLCANO_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("big_volcano"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.VOLCANO_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("molten_copper"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("molten_gold"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("molten_iron"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("molten_aluminum"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("molten_cobalt"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("molten_niobium"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]);
    dictionary.Add(HashedString.op_Implicit("molten_tungsten"), GeoTunerConfig.CategorySettings[GeoTunerConfig.Category.METALS_CATEGORY]);
    GeoTunerConfig.geotunerGeyserSettings = dictionary;
  }

  public struct GeotunedGeyserSettings
  {
    public Tag material;
    public float quantity;
    public Geyser.GeyserModification template;
    public float duration;

    public GeotunedGeyserSettings(
      Tag material,
      float quantity,
      float duration,
      Geyser.GeyserModification template)
    {
      this.quantity = quantity;
      this.material = material;
      this.template = template;
      this.duration = duration;
    }
  }

  public enum Category
  {
    DEFAULT_CATEGORY,
    WATER_CATEGORY,
    ORGANIC_CATEGORY,
    HYDROCARBON_CATEGORY,
    VOLCANO_CATEGORY,
    METALS_CATEGORY,
    CO2_CATEGORY,
  }
}
