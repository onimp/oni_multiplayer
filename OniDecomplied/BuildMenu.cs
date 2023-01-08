// Decompiled with JetBrains decompiler
// Type: BuildMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : KScreen
{
  public const string ENABLE_HOTKEY_BUILD_MENU_KEY = "ENABLE_HOTKEY_BUILD_MENU";
  [SerializeField]
  private BuildMenuCategoriesScreen categoriesMenuPrefab;
  [SerializeField]
  private BuildMenuBuildingsScreen buildingsMenuPrefab;
  [SerializeField]
  private GameObject productInfoScreenPrefab;
  private ProductInfoScreen productInfoScreen;
  private BuildMenuBuildingsScreen buildingsScreen;
  private BuildingDef selectedBuilding;
  private HashedString selectedCategory;
  private static readonly HashedString ROOT_HASHSTR = new HashedString("ROOT");
  private Dictionary<HashedString, BuildMenuCategoriesScreen> submenus = new Dictionary<HashedString, BuildMenuCategoriesScreen>();
  private Stack<KIconToggleMenu> submenuStack = new Stack<KIconToggleMenu>();
  private bool selecting;
  private bool updating;
  private bool deactivateToolQueued;
  [SerializeField]
  private Vector2 rootMenuOffset = Vector2.zero;
  [SerializeField]
  private BuildMenu.PadInfo rootMenuPadding;
  [SerializeField]
  private Vector2 nestedMenuOffset = Vector2.zero;
  [SerializeField]
  private BuildMenu.PadInfo nestedMenuPadding;
  [SerializeField]
  private Vector2 buildingsMenuOffset = Vector2.zero;
  public static BuildMenu.DisplayInfo OrderedBuildings = new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("ROOT"), "icon_category_base", (Action) 275, (KKeyCode) 0, (object) new List<BuildMenu.DisplayInfo>()
  {
    new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Base"), "icon_category_base", (Action) 36, (KKeyCode) 0, (object) new List<BuildMenu.DisplayInfo>()
    {
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Tiles"), "icon_category_base", (Action) 52, (KKeyCode) 116, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("Tile", (Action) 101),
        new BuildMenu.BuildingInfo("GasPermeableMembrane", (Action) 82),
        new BuildMenu.BuildingInfo("MeshTile", (Action) 86),
        new BuildMenu.BuildingInfo("InsulationTile", (Action) 85),
        new BuildMenu.BuildingInfo("PlasticTile", (Action) 84),
        new BuildMenu.BuildingInfo("MetalTile", (Action) 105),
        new BuildMenu.BuildingInfo("GlassTile", (Action) 104),
        new BuildMenu.BuildingInfo("BunkerTile", (Action) 83),
        new BuildMenu.BuildingInfo("CarpetTile", (Action) 93),
        new BuildMenu.BuildingInfo("ExteriorWall", (Action) 85),
        new BuildMenu.BuildingInfo("ExobaseHeadquarters", (Action) 89)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Ladders"), "icon_category_base", (Action) 51, (KKeyCode) 97, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("Ladder", (Action) 82),
        new BuildMenu.BuildingInfo("LadderFast", (Action) 84),
        new BuildMenu.BuildingInfo("FirePole", (Action) 87)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Doors"), "icon_category_base", (Action) 53, (KKeyCode) 100, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("Door", (Action) 85),
        new BuildMenu.BuildingInfo("ManualPressureDoor", (Action) 82),
        new BuildMenu.BuildingInfo("PressureDoor", (Action) 86),
        new BuildMenu.BuildingInfo("BunkerDoor", (Action) 83)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Storage"), "icon_category_base", (Action) 54, (KKeyCode) 115, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("StorageLocker", (Action) 100),
        new BuildMenu.BuildingInfo("RationBox", (Action) 99),
        new BuildMenu.BuildingInfo("Refrigerator", (Action) 87),
        new BuildMenu.BuildingInfo("StorageLockerSmart", (Action) 82),
        new BuildMenu.BuildingInfo("LiquidReservoir", (Action) 98),
        new BuildMenu.BuildingInfo("GasReservoir", (Action) 88),
        new BuildMenu.BuildingInfo("ObjectDispenser", (Action) 96)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Research"), "icon_category_misc", (Action) 71, (KKeyCode) 114, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("ResearchCenter", (Action) 99),
        new BuildMenu.BuildingInfo("AdvancedResearchCenter", (Action) 100),
        new BuildMenu.BuildingInfo("CosmicResearchCenter", (Action) 84),
        new BuildMenu.BuildingInfo("DLC1CosmicResearchCenter", (Action) 84),
        new BuildMenu.BuildingInfo("NuclearResearchCenter", (Action) 95),
        new BuildMenu.BuildingInfo("Telescope", (Action) 101)
      })
    }),
    new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Food And Agriculture"), "icon_category_food", (Action) 37, (KKeyCode) 0, (object) new List<BuildMenu.DisplayInfo>()
    {
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Farming"), "icon_category_food", (Action) 69, (KKeyCode) 102, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("PlanterBox", (Action) 83),
        new BuildMenu.BuildingInfo("FarmTile", (Action) 87),
        new BuildMenu.BuildingInfo("HydroponicFarm", (Action) 85),
        new BuildMenu.BuildingInfo("Compost", (Action) 84),
        new BuildMenu.BuildingInfo("FertilizerMaker", (Action) 99)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Cooking"), "icon_category_food", (Action) 68, (KKeyCode) 99, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("MicrobeMusher", (Action) 84),
        new BuildMenu.BuildingInfo("CookingStation", (Action) 88),
        new BuildMenu.BuildingInfo("SpiceGrinder", (Action) 88),
        new BuildMenu.BuildingInfo("GourmetCookingStation", (Action) 100),
        new BuildMenu.BuildingInfo("EggCracker", (Action) 86)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Ranching"), "icon_category_food", (Action) 70, (KKeyCode) 114, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("CreatureDeliveryPoint", (Action) 85),
        new BuildMenu.BuildingInfo("FishDeliveryPoint", (Action) 88),
        new BuildMenu.BuildingInfo("CreatureFeeder", (Action) 87),
        new BuildMenu.BuildingInfo("FishFeeder", (Action) 86),
        new BuildMenu.BuildingInfo("RanchStation", (Action) 99),
        new BuildMenu.BuildingInfo("ShearingStation", (Action) 100),
        new BuildMenu.BuildingInfo("EggIncubator", (Action) 90),
        new BuildMenu.BuildingInfo("CreatureTrap", (Action) 101),
        new BuildMenu.BuildingInfo("FishTrap", (Action) 82),
        new BuildMenu.BuildingInfo("AirborneCreatureLure", (Action) 93),
        new BuildMenu.BuildingInfo("FlyingCreatureBait", (Action) 83)
      })
    }),
    new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Health And Happiness"), "icon_category_medical", (Action) 38, (KKeyCode) 0, (object) new List<BuildMenu.DisplayInfo>()
    {
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Medical"), "icon_category_medical", (Action) 73, (KKeyCode) 99, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("Apothecary", (Action) 82),
        new BuildMenu.BuildingInfo("DoctorStation", (Action) 85),
        new BuildMenu.BuildingInfo("AdvancedDoctorStation", (Action) 96),
        new BuildMenu.BuildingInfo("MedicalCot", (Action) 83),
        new BuildMenu.BuildingInfo("MassageTable", (Action) 101),
        new BuildMenu.BuildingInfo("Grave", (Action) 99)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Hygiene"), "icon_category_medical", (Action) 72, (KKeyCode) 101, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("Outhouse", (Action) 101),
        new BuildMenu.BuildingInfo("FlushToilet", (Action) 103),
        new BuildMenu.BuildingInfo(ShowerConfig.ID, (Action) 100),
        new BuildMenu.BuildingInfo("WashBasin", (Action) 83),
        new BuildMenu.BuildingInfo("WashSink", (Action) 104),
        new BuildMenu.BuildingInfo("HandSanitizer", (Action) 82),
        new BuildMenu.BuildingInfo("DecontaminationShower", (Action) 85)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Furniture"), "icon_category_furniture", (Action) 75, (KKeyCode) 102, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo(BedConfig.ID, (Action) 84),
        new BuildMenu.BuildingInfo(LuxuryBedConfig.ID, (Action) 105),
        new BuildMenu.BuildingInfo(LadderBedConfig.ID, (Action) 93),
        new BuildMenu.BuildingInfo("DiningTable", (Action) 85),
        new BuildMenu.BuildingInfo("FloorLamp", (Action) 87),
        new BuildMenu.BuildingInfo("CeilingLight", (Action) 101),
        new BuildMenu.BuildingInfo("SunLamp", (Action) 100),
        new BuildMenu.BuildingInfo("RadiationLight", (Action) 99)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Decor"), "icon_category_furniture", (Action) 76, (KKeyCode) 100, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("FlowerVase", (Action) 87),
        new BuildMenu.BuildingInfo("Canvas", (Action) 84),
        new BuildMenu.BuildingInfo("CanvasWide", (Action) 104),
        new BuildMenu.BuildingInfo("CanvasTall", (Action) 101),
        new BuildMenu.BuildingInfo("Sculpture", (Action) 100),
        new BuildMenu.BuildingInfo("IceSculpture", (Action) 86),
        new BuildMenu.BuildingInfo("ItemPedestal", (Action) 85),
        new BuildMenu.BuildingInfo("CrownMoulding", (Action) 94),
        new BuildMenu.BuildingInfo("CornerMoulding", (Action) 95)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Recreation"), "icon_category_medical", (Action) 74, (KKeyCode) 114, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("WaterCooler", (Action) 84),
        new BuildMenu.BuildingInfo("ArcadeMachine", (Action) 82),
        new BuildMenu.BuildingInfo("Phonobox", (Action) 97),
        new BuildMenu.BuildingInfo("EspressoMachine", (Action) 86),
        new BuildMenu.BuildingInfo("HotTub", (Action) 101),
        new BuildMenu.BuildingInfo("MechanicalSurfboard", (Action) 94),
        new BuildMenu.BuildingInfo("Sauna", (Action) 100),
        new BuildMenu.BuildingInfo("BeachChair", (Action) 83),
        new BuildMenu.BuildingInfo("Juicer", (Action) 91),
        new BuildMenu.BuildingInfo("SodaFountain", (Action) 87),
        new BuildMenu.BuildingInfo("VerticalWindTunnel", (Action) 104),
        new BuildMenu.BuildingInfo("ParkSign", (Action) 99),
        new BuildMenu.BuildingInfo("Telephone", (Action) 101)
      })
    }),
    new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Infrastructure"), "icon_category_utilities", (Action) 39, (KKeyCode) 0, (object) new List<BuildMenu.DisplayInfo>()
    {
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Wires"), "icon_category_electrical", (Action) 56, (KKeyCode) 119, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("Wire", (Action) 104),
        new BuildMenu.BuildingInfo("WireBridge", (Action) 83),
        new BuildMenu.BuildingInfo("HighWattageWire", (Action) 101),
        new BuildMenu.BuildingInfo("WireBridgeHighWattage", (Action) 88),
        new BuildMenu.BuildingInfo("WireRefined", (Action) 99),
        new BuildMenu.BuildingInfo("WireRefinedBridge", (Action) 98),
        new BuildMenu.BuildingInfo("WireRefinedHighWattage", (Action) 86),
        new BuildMenu.BuildingInfo("WireRefinedBridgeHighWattage", (Action) 82)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Generators"), "icon_category_electrical", (Action) 55, (KKeyCode) 103, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("ManualGenerator", (Action) 88),
        new BuildMenu.BuildingInfo("Generator", (Action) 84),
        new BuildMenu.BuildingInfo("WoodGasGenerator", (Action) 104),
        new BuildMenu.BuildingInfo("NuclearReactor", (Action) 95),
        new BuildMenu.BuildingInfo("HydrogenGenerator", (Action) 85),
        new BuildMenu.BuildingInfo("MethaneGenerator", (Action) 82),
        new BuildMenu.BuildingInfo("PetroleumGenerator", (Action) 99),
        new BuildMenu.BuildingInfo("SteamTurbine", (Action) 101),
        new BuildMenu.BuildingInfo("SteamTurbine2", (Action) 101),
        new BuildMenu.BuildingInfo("SolarPanel", (Action) 100),
        new BuildMenu.BuildingInfo("DevGenerator", (Action) 105)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("PowerControl"), "icon_category_electrical", (Action) 57, (KKeyCode) 114, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("Battery", (Action) 83),
        new BuildMenu.BuildingInfo("BatteryMedium", (Action) 86),
        new BuildMenu.BuildingInfo("BatterySmart", (Action) 100),
        new BuildMenu.BuildingInfo("PowerTransformerSmall", (Action) 101),
        new BuildMenu.BuildingInfo("PowerTransformer", (Action) 99),
        new BuildMenu.BuildingInfo(SwitchConfig.ID, (Action) 84),
        new BuildMenu.BuildingInfo(TemperatureControlledSwitchConfig.ID, (Action) 82),
        new BuildMenu.BuildingInfo(PressureSwitchLiquidConfig.ID, (Action) 98),
        new BuildMenu.BuildingInfo(PressureSwitchGasConfig.ID, (Action) 88),
        new BuildMenu.BuildingInfo(LogicPowerRelayConfig.ID, (Action) 105)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Pipes"), "icon_category_plumbing", (Action) 59, (KKeyCode) 101, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("LiquidConduit", (Action) 98),
        new BuildMenu.BuildingInfo("LiquidConduitBridge", (Action) 83),
        new BuildMenu.BuildingInfo("InsulatedLiquidConduit", (Action) 104),
        new BuildMenu.BuildingInfo("LiquidConduitRadiant", (Action) 86),
        new BuildMenu.BuildingInfo("GasConduit", (Action) 88),
        new BuildMenu.BuildingInfo("GasConduitBridge", (Action) 87),
        new BuildMenu.BuildingInfo("InsulatedGasConduit", (Action) 85),
        new BuildMenu.BuildingInfo("GasConduitRadiant", (Action) 99),
        new BuildMenu.BuildingInfo("ContactConductivePipeBridge", (Action) 82)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Plumbing Structures"), "icon_category_plumbing", (Action) 58, (KKeyCode) 98, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("LiquidPumpingStation", (Action) 85),
        new BuildMenu.BuildingInfo("BottleEmptier", (Action) 83),
        new BuildMenu.BuildingInfo("LiquidPump", (Action) 98),
        new BuildMenu.BuildingInfo("LiquidMiniPump", (Action) 105),
        new BuildMenu.BuildingInfo("LiquidValve", (Action) 82),
        new BuildMenu.BuildingInfo("LiquidLogicValve", (Action) 93),
        new BuildMenu.BuildingInfo("LiquidVent", (Action) 103),
        new BuildMenu.BuildingInfo("LiquidFilter", (Action) 87),
        new BuildMenu.BuildingInfo("LiquidConduitPreferentialFlow", (Action) 104),
        new BuildMenu.BuildingInfo("LiquidConduitOverflow", (Action) 99),
        new BuildMenu.BuildingInfo("LiquidLimitValve", (Action) 84),
        new BuildMenu.BuildingInfo("ModularLaunchpadPortLiquid", (Action) 94),
        new BuildMenu.BuildingInfo("ModularLaunchpadPortLiquidUnloader", (Action) 102)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Ventilation Structures"), "icon_category_ventilation", (Action) 60, (KKeyCode) 118, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("GasPump", (Action) 98),
        new BuildMenu.BuildingInfo("GasMiniPump", (Action) 105),
        new BuildMenu.BuildingInfo("GasValve", (Action) 82),
        new BuildMenu.BuildingInfo("GasLogicValve", (Action) 84),
        new BuildMenu.BuildingInfo("GasVent", (Action) 103),
        new BuildMenu.BuildingInfo("GasVentHighPressure", (Action) 86),
        new BuildMenu.BuildingInfo("GasFilter", (Action) 87),
        new BuildMenu.BuildingInfo("GasBottler", (Action) 83),
        new BuildMenu.BuildingInfo("BottleEmptierGas", (Action) 83),
        new BuildMenu.BuildingInfo("GasConduitPreferentialFlow", (Action) 104),
        new BuildMenu.BuildingInfo("GasConduitOverflow", (Action) 99),
        new BuildMenu.BuildingInfo("GasLimitValve", (Action) 93),
        new BuildMenu.BuildingInfo("ModularLaunchpadPortGas", (Action) 88),
        new BuildMenu.BuildingInfo("ModularLaunchpadPortGasUnloader", (Action) 102)
      })
    }),
    new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Industrial"), "icon_category_refinery", (Action) 40, (KKeyCode) 0, (object) new List<BuildMenu.DisplayInfo>()
    {
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Oxygen"), "icon_category_oxygen", (Action) 77, (KKeyCode) 120, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("MineralDeoxidizer", (Action) 105),
        new BuildMenu.BuildingInfo("SublimationStation", (Action) 100),
        new BuildMenu.BuildingInfo("AlgaeHabitat", (Action) 82),
        new BuildMenu.BuildingInfo("AirFilter", (Action) 85),
        new BuildMenu.BuildingInfo("CO2Scrubber", (Action) 84),
        new BuildMenu.BuildingInfo("Electrolyzer", (Action) 86),
        new BuildMenu.BuildingInfo("RustDeoxidizer", (Action) 87)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Utilities"), "icon_category_utilities", (Action) 78, (KKeyCode) 116, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("SpaceHeater", (Action) 100),
        new BuildMenu.BuildingInfo("LiquidHeater", (Action) 101),
        new BuildMenu.BuildingInfo("IceCooledFan", (Action) 98),
        new BuildMenu.BuildingInfo("IceMachine", (Action) 90),
        new BuildMenu.BuildingInfo("AirConditioner", (Action) 99),
        new BuildMenu.BuildingInfo("LiquidConditioner", (Action) 82),
        new BuildMenu.BuildingInfo("OreScrubber", (Action) 84),
        new BuildMenu.BuildingInfo("ThermalBlock", (Action) 87),
        new BuildMenu.BuildingInfo("HighEnergyParticleRedirector", (Action) 97)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Refining"), "icon_category_refinery", (Action) 79, (KKeyCode) 114, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("WaterPurifier", (Action) 104),
        new BuildMenu.BuildingInfo("AlgaeDistillery", (Action) 82),
        new BuildMenu.BuildingInfo("EthanolDistillery", (Action) 105),
        new BuildMenu.BuildingInfo("RockCrusher", (Action) 88),
        new BuildMenu.BuildingInfo("SludgePress", (Action) 97),
        new BuildMenu.BuildingInfo("Kiln", (Action) 107),
        new BuildMenu.BuildingInfo("OilWellCap", (Action) 84),
        new BuildMenu.BuildingInfo("OilRefinery", (Action) 99),
        new BuildMenu.BuildingInfo("Polymerizer", (Action) 86),
        new BuildMenu.BuildingInfo("MetalRefinery", (Action) 101),
        new BuildMenu.BuildingInfo("GlassForge", (Action) 87),
        new BuildMenu.BuildingInfo("OxyliteRefinery", (Action) 96),
        new BuildMenu.BuildingInfo("SupermaterialRefinery", (Action) 100),
        new BuildMenu.BuildingInfo("UraniumCentrifuge", (Action) 102)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Equipment"), "icon_category_misc", (Action) 80, (KKeyCode) 115, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("RoleStation", (Action) 83),
        new BuildMenu.BuildingInfo("FarmStation", (Action) 87),
        new BuildMenu.BuildingInfo("PowerControlStation", (Action) 84),
        new BuildMenu.BuildingInfo("AstronautTrainingCenter", (Action) 82),
        new BuildMenu.BuildingInfo("ResetSkillsStation", (Action) 99),
        new BuildMenu.BuildingInfo("CraftingTable", (Action) 107),
        new BuildMenu.BuildingInfo("OxygenMaskMarker", (Action) 98),
        new BuildMenu.BuildingInfo("OxygenMaskLocker", (Action) 106),
        new BuildMenu.BuildingInfo("ClothingFabricator", (Action) 101),
        new BuildMenu.BuildingInfo("SuitFabricator", (Action) 105),
        new BuildMenu.BuildingInfo("SuitMarker", (Action) 86),
        new BuildMenu.BuildingInfo("SuitLocker", (Action) 85),
        new BuildMenu.BuildingInfo("JetSuitMarker", (Action) 91),
        new BuildMenu.BuildingInfo("JetSuitLocker", (Action) 96),
        new BuildMenu.BuildingInfo("LeadSuitMarker", (Action) 86),
        new BuildMenu.BuildingInfo("LeadSuitLocker", (Action) 85)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Rocketry"), "icon_category_rocketry", (Action) 81, (KKeyCode) 99, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("Gantry", (Action) 101),
        new BuildMenu.BuildingInfo("KeroseneEngine", (Action) 86),
        new BuildMenu.BuildingInfo("SolidBooster", (Action) 83),
        new BuildMenu.BuildingInfo("SteamEngine", (Action) 100),
        new BuildMenu.BuildingInfo("LiquidFuelTank", (Action) 98),
        new BuildMenu.BuildingInfo("CargoBay", (Action) 83),
        new BuildMenu.BuildingInfo("GasCargoBay", (Action) 88),
        new BuildMenu.BuildingInfo("LiquidCargoBay", (Action) 98),
        new BuildMenu.BuildingInfo("SpecialCargoBay", (Action) 82),
        new BuildMenu.BuildingInfo("CommandModule", (Action) 84),
        new BuildMenu.BuildingInfo("TouristModule", (Action) 106),
        new BuildMenu.BuildingInfo("ResearchModule", (Action) 99),
        new BuildMenu.BuildingInfo("HydrogenEngine", (Action) 89),
        new BuildMenu.BuildingInfo("RailGun", (Action) 97),
        new BuildMenu.BuildingInfo("LandingBeacon", (Action) 93)
      })
    }),
    new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Logistics"), "icon_category_ventilation", (Action) 41, (KKeyCode) 0, (object) new List<BuildMenu.DisplayInfo>()
    {
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("TravelTubes"), "icon_category_ventilation", (Action) 62, (KKeyCode) 116, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("TravelTube", (Action) 101),
        new BuildMenu.BuildingInfo("TravelTubeEntrance", (Action) 86),
        new BuildMenu.BuildingInfo("TravelTubeWallBridge", (Action) 83)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("Conveyance"), "icon_category_ventilation", (Action) 63, (KKeyCode) 99, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("SolidTransferArm", (Action) 82),
        new BuildMenu.BuildingInfo("SolidConduit", (Action) 84),
        new BuildMenu.BuildingInfo("SolidConduitInbox", (Action) 90),
        new BuildMenu.BuildingInfo("SolidConduitOutbox", (Action) 96),
        new BuildMenu.BuildingInfo("SolidVent", (Action) 103),
        new BuildMenu.BuildingInfo("SolidLogicValve", (Action) 93),
        new BuildMenu.BuildingInfo("SolidLimitValve", (Action) 85),
        new BuildMenu.BuildingInfo("SolidConduitBridge", (Action) 83),
        new BuildMenu.BuildingInfo("AutoMiner", (Action) 94),
        new BuildMenu.BuildingInfo("ModularLaunchpadPortSolid", (Action) 100),
        new BuildMenu.BuildingInfo("ModularLaunchpadPortSolidUnloader", (Action) 102)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("LogicWiring"), "icon_category_automation", (Action) 64, (KKeyCode) 119, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("LogicWire", (Action) 104),
        new BuildMenu.BuildingInfo("LogicWireBridge", (Action) 83),
        new BuildMenu.BuildingInfo("LogicRibbon", (Action) 99),
        new BuildMenu.BuildingInfo("LogicRibbonBridge", (Action) 103)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("LogicGates"), "icon_category_automation", (Action) 65, (KKeyCode) 103, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo("LogicGateAND", (Action) 82),
        new BuildMenu.BuildingInfo("LogicGateOR", (Action) 99),
        new BuildMenu.BuildingInfo("LogicGateXOR", (Action) 105),
        new BuildMenu.BuildingInfo("LogicGateNOT", (Action) 101),
        new BuildMenu.BuildingInfo("LogicGateBUFFER", (Action) 83),
        new BuildMenu.BuildingInfo("LogicGateFILTER", (Action) 87),
        new BuildMenu.BuildingInfo(LogicMemoryConfig.ID, (Action) 103)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("LogicSwitches"), "icon_category_automation", (Action) 66, (KKeyCode) 115, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo(LogicSwitchConfig.ID, (Action) 100),
        new BuildMenu.BuildingInfo(LogicPressureSensorGasConfig.ID, (Action) 82),
        new BuildMenu.BuildingInfo(LogicPressureSensorLiquidConfig.ID, (Action) 98),
        new BuildMenu.BuildingInfo(LogicTemperatureSensorConfig.ID, (Action) 101),
        new BuildMenu.BuildingInfo(LogicTimeOfDaySensorConfig.ID, (Action) 85),
        new BuildMenu.BuildingInfo(LogicTimerSensorConfig.ID, (Action) 87),
        new BuildMenu.BuildingInfo(LogicCritterCountSensorConfig.ID, (Action) 103),
        new BuildMenu.BuildingInfo(LogicDiseaseSensorConfig.ID, (Action) 88),
        new BuildMenu.BuildingInfo(LogicElementSensorGasConfig.ID, (Action) 86),
        new BuildMenu.BuildingInfo(LogicWattageSensorConfig.ID, (Action) 97),
        new BuildMenu.BuildingInfo("FloorSwitch", (Action) 104),
        new BuildMenu.BuildingInfo("Checkpoint", (Action) 84),
        new BuildMenu.BuildingInfo(CometDetectorConfig.ID, (Action) 99),
        new BuildMenu.BuildingInfo("LogicDuplicantSensor", (Action) 87)
      }),
      new BuildMenu.DisplayInfo(BuildMenu.CacheHashString("ConduitSensors"), "icon_category_automation", (Action) 67, (KKeyCode) 120, (object) new List<BuildMenu.BuildingInfo>()
      {
        new BuildMenu.BuildingInfo(LiquidConduitTemperatureSensorConfig.ID, (Action) 101),
        new BuildMenu.BuildingInfo(LiquidConduitDiseaseSensorConfig.ID, (Action) 88),
        new BuildMenu.BuildingInfo(LiquidConduitElementSensorConfig.ID, (Action) 86),
        new BuildMenu.BuildingInfo(GasConduitTemperatureSensorConfig.ID, (Action) 99),
        new BuildMenu.BuildingInfo(GasConduitDiseaseSensorConfig.ID, (Action) 87),
        new BuildMenu.BuildingInfo(GasConduitElementSensorConfig.ID, (Action) 100)
      })
    })
  });
  private Dictionary<HashedString, List<BuildingDef>> categorizedBuildingMap;
  private Dictionary<HashedString, List<HashedString>> categorizedCategoryMap;
  private Dictionary<Tag, HashedString> tagCategoryMap;
  private Dictionary<Tag, int> tagOrderMap;
  private const float NotificationPingExpire = 0.5f;
  private const float SpecialNotificationEmbellishDelay = 8f;
  private float timeSinceNotificationPing;
  private int notificationPingCount;
  private float initTime;
  private float updateInterval = 1f;
  private float elapsedTime;

  public virtual float GetSortKey() => 6f;

  public static BuildMenu Instance { get; private set; }

  public static void DestroyInstance() => BuildMenu.Instance = (BuildMenu) null;

  public BuildingDef SelectedBuildingDef => this.selectedBuilding;

  private static HashedString CacheHashString(string str) => HashCache.Get().Add(str);

  public static bool UseHotkeyBuildMenu() => KPlayerPrefs.GetInt("ENABLE_HOTKEY_BUILD_MENU") != 0;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.ConsumeMouseScroll = true;
    this.initTime = KTime.Instance.UnscaledGameTime;
    bool flag = BuildMenu.UseHotkeyBuildMenu();
    if (flag)
    {
      BuildMenu.Instance = this;
      this.productInfoScreen = Util.KInstantiateUI<ProductInfoScreen>(this.productInfoScreenPrefab, ((Component) this).gameObject, true);
      Util.rectTransform((Component) this.productInfoScreen).pivot = new Vector2(0.0f, 0.0f);
      this.productInfoScreen.onElementsFullySelected = new System.Action(this.OnRecipeElementsFullySelected);
      this.productInfoScreen.Show(false);
      this.buildingsScreen = Util.KInstantiateUI<BuildMenuBuildingsScreen>(((Component) this.buildingsMenuPrefab).gameObject, ((Component) this).gameObject, true);
      this.buildingsScreen.onBuildingSelected += new Action<BuildingDef>(this.OnBuildingSelected);
      this.buildingsScreen.Show(false);
      Game.Instance.Subscribe(288942073, new Action<object>(this.OnUIClear));
      Game.Instance.Subscribe(-1190690038, new Action<object>(this.OnBuildToolDeactivated));
      this.Initialize();
      Util.rectTransform((Component) this).anchoredPosition = Vector2.zero;
    }
    else
      ((Component) this).gameObject.SetActive(flag);
  }

  private void Initialize()
  {
    foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu in this.submenus)
    {
      BuildMenuCategoriesScreen categoriesScreen = submenu.Value;
      categoriesScreen.Close();
      Object.DestroyImmediate((Object) ((Component) categoriesScreen).gameObject);
    }
    this.submenuStack.Clear();
    this.tagCategoryMap = new Dictionary<Tag, HashedString>();
    this.tagOrderMap = new Dictionary<Tag, int>();
    this.categorizedBuildingMap = new Dictionary<HashedString, List<BuildingDef>>();
    this.categorizedCategoryMap = new Dictionary<HashedString, List<HashedString>>();
    int building_index = 0;
    BuildMenu.DisplayInfo orderedBuildings = BuildMenu.OrderedBuildings;
    this.PopulateCategorizedMaps(orderedBuildings.category, 0, orderedBuildings.data, this.tagCategoryMap, this.tagOrderMap, ref building_index, this.categorizedBuildingMap, this.categorizedCategoryMap);
    BuildMenuCategoriesScreen submenu1 = this.submenus[BuildMenu.ROOT_HASHSTR];
    submenu1.Show(true);
    submenu1.modalKeyInputBehaviour = false;
    foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu2 in this.submenus)
    {
      HashedString key = submenu2.Key;
      List<HashedString> hashedStringList;
      if (!HashedString.op_Equality(key, BuildMenu.ROOT_HASHSTR) && this.categorizedCategoryMap.TryGetValue(key, out hashedStringList))
      {
        Image component = ((Component) submenu2.Value).GetComponent<Image>();
        if (Object.op_Inequality((Object) component, (Object) null))
          ((Behaviour) component).enabled = hashedStringList.Count > 0;
      }
    }
    this.PositionMenus();
  }

  [ContextMenu("PositionMenus")]
  private void PositionMenus()
  {
    foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu in this.submenus)
    {
      HashedString key = submenu.Key;
      BuildMenuCategoriesScreen categoriesScreen = submenu.Value;
      LayoutGroup component = ((Component) categoriesScreen).GetComponent<LayoutGroup>();
      HashedString rootHashstr = BuildMenu.ROOT_HASHSTR;
      Vector2 vector2;
      BuildMenu.PadInfo padInfo;
      if (HashedString.op_Equality(key, rootHashstr))
      {
        vector2 = this.rootMenuOffset;
        padInfo = this.rootMenuPadding;
        ((Behaviour) ((Component) categoriesScreen).GetComponent<Image>()).enabled = false;
      }
      else
      {
        vector2 = this.nestedMenuOffset;
        padInfo = this.nestedMenuPadding;
      }
      Util.rectTransform((Component) categoriesScreen).anchoredPosition = vector2;
      component.padding.left = padInfo.left;
      component.padding.right = padInfo.right;
      component.padding.top = padInfo.top;
      component.padding.bottom = padInfo.bottom;
    }
    Util.rectTransform((Component) this.buildingsScreen).anchoredPosition = this.buildingsMenuOffset;
  }

  public void Refresh()
  {
    foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu in this.submenus)
      submenu.Value.UpdateBuildableStates(true);
  }

  protected virtual void OnCmpEnable()
  {
    ((KMonoBehaviour) this).OnCmpEnable();
    Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
  }

  protected virtual void OnCmpDisable()
  {
    Game.Instance.Unsubscribe(-107300940, new Action<object>(this.OnResearchComplete));
    ((KMonoBehaviour) this).OnCmpDisable();
  }

  private BuildMenuCategoriesScreen CreateCategorySubMenu(
    HashedString category,
    int depth,
    object data,
    Dictionary<HashedString, List<BuildingDef>> categorized_building_map,
    Dictionary<HashedString, List<HashedString>> categorized_category_map,
    Dictionary<Tag, HashedString> tag_category_map,
    BuildMenuBuildingsScreen buildings_screen)
  {
    BuildMenuCategoriesScreen categorySubMenu = Util.KInstantiateUI<BuildMenuCategoriesScreen>(((Component) this.categoriesMenuPrefab).gameObject, ((Component) this).gameObject, true);
    categorySubMenu.Show(false);
    categorySubMenu.Configure(category, depth, data, this.categorizedBuildingMap, this.categorizedCategoryMap, this.buildingsScreen);
    categorySubMenu.onCategoryClicked += new Action<HashedString, int>(this.OnCategoryClicked);
    ((Object) categorySubMenu).name = "BuildMenu_" + category.ToString();
    return categorySubMenu;
  }

  private void PopulateCategorizedMaps(
    HashedString category,
    int depth,
    object data,
    Dictionary<Tag, HashedString> category_map,
    Dictionary<Tag, int> order_map,
    ref int building_index,
    Dictionary<HashedString, List<BuildingDef>> categorized_building_map,
    Dictionary<HashedString, List<HashedString>> categorized_category_map)
  {
    System.Type type = data.GetType();
    if (type == typeof (BuildMenu.DisplayInfo))
    {
      BuildMenu.DisplayInfo displayInfo = (BuildMenu.DisplayInfo) data;
      List<HashedString> hashedStringList;
      if (!categorized_category_map.TryGetValue(category, out hashedStringList))
      {
        hashedStringList = new List<HashedString>();
        categorized_category_map[category] = hashedStringList;
      }
      hashedStringList.Add(displayInfo.category);
      this.PopulateCategorizedMaps(displayInfo.category, depth + 1, displayInfo.data, category_map, order_map, ref building_index, categorized_building_map, categorized_category_map);
    }
    else if (typeof (IList<BuildMenu.DisplayInfo>).IsAssignableFrom(type))
    {
      IList<BuildMenu.DisplayInfo> displayInfoList = (IList<BuildMenu.DisplayInfo>) data;
      List<HashedString> hashedStringList;
      if (!categorized_category_map.TryGetValue(category, out hashedStringList))
      {
        hashedStringList = new List<HashedString>();
        categorized_category_map[category] = hashedStringList;
      }
      foreach (BuildMenu.DisplayInfo displayInfo in (IEnumerable<BuildMenu.DisplayInfo>) displayInfoList)
      {
        hashedStringList.Add(displayInfo.category);
        this.PopulateCategorizedMaps(displayInfo.category, depth + 1, displayInfo.data, category_map, order_map, ref building_index, categorized_building_map, categorized_category_map);
      }
    }
    else
    {
      foreach (BuildMenu.BuildingInfo buildingInfo in (IEnumerable<BuildMenu.BuildingInfo>) data)
      {
        Tag key;
        // ISSUE: explicit constructor call
        ((Tag) ref key).\u002Ector(buildingInfo.id);
        category_map[key] = category;
        order_map[key] = building_index;
        ++building_index;
        List<BuildingDef> buildingDefList;
        if (!categorized_building_map.TryGetValue(category, out buildingDefList))
        {
          buildingDefList = new List<BuildingDef>();
          categorized_building_map[category] = buildingDefList;
        }
        BuildingDef buildingDef = Assets.GetBuildingDef(buildingInfo.id);
        buildingDef.HotKey = buildingInfo.hotkey;
        buildingDefList.Add(buildingDef);
      }
    }
    this.submenus[category] = this.CreateCategorySubMenu(category, depth, data, this.categorizedBuildingMap, this.categorizedCategoryMap, this.tagCategoryMap, this.buildingsScreen);
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed)
      return;
    if (this.mouseOver && this.ConsumeMouseScroll && !e.TryConsume((Action) 7))
      e.TryConsume((Action) 8);
    if (!((KInputEvent) e).Consumed && ((HashedString) ref this.selectedCategory).IsValid && e.TryConsume((Action) 1))
    {
      this.OnUIClear((object) null);
    }
    else
    {
      if (((KInputEvent) e).Consumed)
        return;
      base.OnKeyDown(e);
    }
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (((HashedString) ref this.selectedCategory).IsValid && PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
    {
      this.OnUIClear((object) null);
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect"));
    }
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyUp(e);
  }

  private void OnUIClear(object data)
  {
    SelectTool.Instance.Activate();
    PlayerController.Instance.ActivateTool((InterfaceTool) SelectTool.Instance);
    SelectTool.Instance.Select((KSelectable) null, true);
    this.productInfoScreen.materialSelectionPanel.PriorityScreen.ResetPriority();
    this.CloseMenus();
  }

  private void OnBuildToolDeactivated(object data)
  {
    if (this.updating)
    {
      this.deactivateToolQueued = true;
    }
    else
    {
      this.CloseMenus();
      this.productInfoScreen.materialSelectionPanel.PriorityScreen.ResetPriority();
    }
  }

  private void CloseMenus()
  {
    this.productInfoScreen.Close();
    while (this.submenuStack.Count > 0)
    {
      this.submenuStack.Pop().Close();
      this.productInfoScreen.Close();
    }
    this.selectedCategory = HashedString.Invalid;
    this.submenus[BuildMenu.ROOT_HASHSTR].ClearSelection();
  }

  public virtual void ScreenUpdate(bool topLevel)
  {
    base.ScreenUpdate(topLevel);
    if ((double) this.timeSinceNotificationPing < 8.0)
      this.timeSinceNotificationPing += Time.unscaledDeltaTime;
    if ((double) this.timeSinceNotificationPing < 0.5)
      return;
    this.notificationPingCount = 0;
  }

  public void PlayNewBuildingSounds()
  {
    if ((double) KTime.Instance.UnscaledGameTime - (double) this.initTime > 1.5)
    {
      if ((double) BuildMenu.Instance.timeSinceNotificationPing >= 8.0)
      {
        string sound = GlobalAssets.GetSound("NewBuildable_Embellishment");
        if (sound != null)
          SoundEvent.EndOneShot(SoundEvent.BeginOneShot(sound, TransformExtensions.GetPosition(((Component) SoundListenerController.Instance).transform)));
      }
      string sound1 = GlobalAssets.GetSound("NewBuildable");
      if (sound1 != null)
      {
        EventInstance instance = SoundEvent.BeginOneShot(sound1, TransformExtensions.GetPosition(((Component) SoundListenerController.Instance).transform));
        ((EventInstance) ref instance).setParameterByName("playCount", (float) BuildMenu.Instance.notificationPingCount, false);
        SoundEvent.EndOneShot(instance);
      }
    }
    this.timeSinceNotificationPing = 0.0f;
    ++this.notificationPingCount;
  }

  public PlanScreen.RequirementsState BuildableState(BuildingDef def)
  {
    PlanScreen.RequirementsState requirementsState = PlanScreen.RequirementsState.Complete;
    if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive)
    {
      if (!Db.Get().TechItems.IsTechItemComplete(def.PrefabID))
        requirementsState = PlanScreen.RequirementsState.Tech;
      else if (!ProductInfoScreen.MaterialsMet(def.CraftRecipe))
        requirementsState = PlanScreen.RequirementsState.Materials;
    }
    return requirementsState;
  }

  private void CloseProductInfoScreen()
  {
    this.productInfoScreen.ClearProduct();
    this.productInfoScreen.Show(false);
  }

  private void Update()
  {
    if (this.deactivateToolQueued)
    {
      this.deactivateToolQueued = false;
      this.OnBuildToolDeactivated((object) null);
    }
    this.elapsedTime += Time.unscaledDeltaTime;
    if ((double) this.elapsedTime <= (double) this.updateInterval)
      return;
    this.elapsedTime = 0.0f;
    this.updating = true;
    if (((Component) this.productInfoScreen).gameObject.activeSelf)
      this.productInfoScreen.materialSelectionPanel.UpdateResourceToggleValues();
    foreach (KIconToggleMenu submenu in this.submenuStack)
    {
      if (submenu is BuildMenuCategoriesScreen)
        (submenu as BuildMenuCategoriesScreen).UpdateBuildableStates(false);
    }
    this.submenus[BuildMenu.ROOT_HASHSTR].UpdateBuildableStates(false);
    this.updating = false;
  }

  private void OnRecipeElementsFullySelected()
  {
    if (Object.op_Equality((Object) this.selectedBuilding, (Object) null))
      Debug.Log((object) "No def!");
    if (this.selectedBuilding.isKAnimTile && this.selectedBuilding.isUtility)
    {
      IList<Tag> selectedElementAsList = this.productInfoScreen.materialSelectionPanel.GetSelectedElementAsList;
      (Object.op_Inequality((Object) this.selectedBuilding.BuildingComplete.GetComponent<Wire>(), (Object) null) ? (BaseUtilityBuildTool) WireBuildTool.Instance : (BaseUtilityBuildTool) UtilityBuildTool.Instance).Activate(this.selectedBuilding, selectedElementAsList);
    }
    else
      BuildTool.Instance.Activate(this.selectedBuilding, this.productInfoScreen.materialSelectionPanel.GetSelectedElementAsList);
  }

  private void OnBuildingSelected(BuildingDef def)
  {
    if (this.selecting)
      return;
    this.selecting = true;
    this.selectedBuilding = def;
    ((KScreen) this.buildingsScreen).SetHasFocus(false);
    foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu in this.submenus)
      ((KScreen) submenu.Value).SetHasFocus(false);
    ToolMenu.Instance.ClearSelection();
    if (Object.op_Inequality((Object) def, (Object) null))
    {
      Vector2 anchoredPosition = Util.rectTransform((Component) this.productInfoScreen).anchoredPosition;
      RectTransform rectTransform = Util.rectTransform((Component) this.buildingsScreen);
      anchoredPosition.y = rectTransform.anchoredPosition.y;
      anchoredPosition.x = (float) ((double) rectTransform.anchoredPosition.x + (double) rectTransform.sizeDelta.x + 10.0);
      Util.rectTransform((Component) this.productInfoScreen).anchoredPosition = anchoredPosition;
      this.productInfoScreen.ClearProduct(false);
      this.productInfoScreen.Show(true);
      this.productInfoScreen.ConfigureScreen(def);
    }
    else
      this.productInfoScreen.Close();
    this.selecting = false;
  }

  private void OnCategoryClicked(HashedString new_category, int depth)
  {
    while (this.submenuStack.Count > depth)
    {
      KIconToggleMenu kiconToggleMenu = this.submenuStack.Pop();
      kiconToggleMenu.ClearSelection();
      kiconToggleMenu.Close();
    }
    this.productInfoScreen.Close();
    if (HashedString.op_Inequality(new_category, this.selectedCategory) && ((HashedString) ref new_category).IsValid)
    {
      foreach (KIconToggleMenu submenu in this.submenuStack)
      {
        if (submenu is BuildMenuCategoriesScreen)
          ((KScreen) (submenu as BuildMenuCategoriesScreen)).SetHasFocus(false);
      }
      this.selectedCategory = new_category;
      BuildMenuCategoriesScreen categoriesScreen;
      this.submenus.TryGetValue(new_category, out categoriesScreen);
      if (Object.op_Inequality((Object) categoriesScreen, (Object) null))
      {
        categoriesScreen.Show(true);
        ((KScreen) categoriesScreen).SetHasFocus(true);
        this.submenuStack.Push((KIconToggleMenu) categoriesScreen);
      }
    }
    else
      this.selectedCategory = HashedString.Invalid;
    foreach (KIconToggleMenu submenu in this.submenuStack)
    {
      if (submenu is BuildMenuCategoriesScreen)
        (submenu as BuildMenuCategoriesScreen).UpdateBuildableStates(true);
    }
    this.submenus[BuildMenu.ROOT_HASHSTR].UpdateBuildableStates(true);
  }

  public void RefreshProductInfoScreen(BuildingDef def)
  {
    if (!Object.op_Equality((Object) this.productInfoScreen.currentDef, (Object) def))
      return;
    this.productInfoScreen.ClearProduct(false);
    this.productInfoScreen.Show(true);
    this.productInfoScreen.ConfigureScreen(def);
  }

  private HashedString GetParentCategory(HashedString desired_category)
  {
    foreach (KeyValuePair<HashedString, List<HashedString>> categorizedCategory in this.categorizedCategoryMap)
    {
      foreach (HashedString hashedString in categorizedCategory.Value)
      {
        if (HashedString.op_Equality(hashedString, desired_category))
          return categorizedCategory.Key;
      }
    }
    return HashedString.Invalid;
  }

  private void AddParentCategories(
    HashedString child_category,
    ICollection<HashedString> categories)
  {
    while (true)
    {
      HashedString parentCategory = this.GetParentCategory(child_category);
      if (!HashedString.op_Equality(parentCategory, HashedString.Invalid))
      {
        categories.Add(parentCategory);
        child_category = parentCategory;
      }
      else
        break;
    }
  }

  private void OnResearchComplete(object data)
  {
    HashSet<HashedString> hashedStringSet = new HashSet<HashedString>();
    Tech tech = (Tech) data;
    foreach (TechItem unlockedItem in tech.unlockedItems)
    {
      BuildingDef buildingDef = Assets.GetBuildingDef(unlockedItem.Id);
      if (Object.op_Equality((Object) buildingDef, (Object) null))
      {
        DebugUtil.LogWarningArgs(new object[1]
        {
          (object) string.Format("Tech '{0}' unlocked building '{1}' but no such building exists", (object) tech.Name, (object) unlockedItem.Id)
        });
      }
      else
      {
        HashedString tagCategory = this.tagCategoryMap[buildingDef.Tag];
        hashedStringSet.Add(tagCategory);
        this.AddParentCategories(tagCategory, (ICollection<HashedString>) hashedStringSet);
      }
    }
    this.UpdateNotifications((ICollection<HashedString>) hashedStringSet, (object) BuildMenu.OrderedBuildings);
  }

  private void UpdateNotifications(ICollection<HashedString> updated_categories, object data)
  {
    foreach (KeyValuePair<HashedString, BuildMenuCategoriesScreen> submenu in this.submenus)
      submenu.Value.UpdateNotifications(updated_categories);
  }

  public PrioritySetting GetBuildingPriority() => this.productInfoScreen.materialSelectionPanel.PriorityScreen.GetLastSelectedPriority();

  [Serializable]
  private struct PadInfo
  {
    public int left;
    public int right;
    public int top;
    public int bottom;
  }

  public struct BuildingInfo
  {
    public string id;
    public Action hotkey;

    public BuildingInfo(string id, Action hotkey)
    {
      this.id = id;
      this.hotkey = hotkey;
    }
  }

  public struct DisplayInfo
  {
    public HashedString category;
    public string iconName;
    public Action hotkey;
    public KKeyCode keyCode;
    public object data;

    public DisplayInfo(
      HashedString category,
      string icon_name,
      Action hotkey,
      KKeyCode key_code,
      object data)
    {
      this.category = category;
      this.iconName = icon_name;
      this.hotkey = hotkey;
      this.keyCode = key_code;
      this.data = data;
    }

    public BuildMenu.DisplayInfo GetInfo(HashedString category)
    {
      BuildMenu.DisplayInfo info = new BuildMenu.DisplayInfo();
      if (this.data != null && typeof (IList<BuildMenu.DisplayInfo>).IsAssignableFrom(this.data.GetType()))
      {
        foreach (BuildMenu.DisplayInfo displayInfo in (IEnumerable<BuildMenu.DisplayInfo>) this.data)
        {
          info = displayInfo.GetInfo(category);
          if (!HashedString.op_Equality(info.category, category))
          {
            if (HashedString.op_Equality(displayInfo.category, category))
            {
              info = displayInfo;
              break;
            }
          }
          else
            break;
        }
      }
      return info;
    }
  }
}
