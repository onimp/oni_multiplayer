// Decompiled with JetBrains decompiler
// Type: Database.Techs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
  public class Techs : ResourceSet<Tech>
  {
    private readonly List<List<Tuple<string, float>>> TECH_TIERS;

    public Techs(ResourceSet parent)
      : base(nameof (Techs), parent)
    {
      if (!DlcManager.IsExpansion1Active())
      {
        List<List<Tuple<string, float>>> tupleListList = new List<List<Tuple<string, float>>>();
        tupleListList.Add(new List<Tuple<string, float>>());
        List<Tuple<string, float>> tupleList1 = new List<Tuple<string, float>>();
        tupleList1.Add(new Tuple<string, float>("basic", 15f));
        tupleListList.Add(tupleList1);
        List<Tuple<string, float>> tupleList2 = new List<Tuple<string, float>>();
        tupleList2.Add(new Tuple<string, float>("basic", 20f));
        tupleListList.Add(tupleList2);
        List<Tuple<string, float>> tupleList3 = new List<Tuple<string, float>>();
        tupleList3.Add(new Tuple<string, float>("basic", 30f));
        tupleList3.Add(new Tuple<string, float>("advanced", 20f));
        tupleListList.Add(tupleList3);
        List<Tuple<string, float>> tupleList4 = new List<Tuple<string, float>>();
        tupleList4.Add(new Tuple<string, float>("basic", 35f));
        tupleList4.Add(new Tuple<string, float>("advanced", 30f));
        tupleListList.Add(tupleList4);
        List<Tuple<string, float>> tupleList5 = new List<Tuple<string, float>>();
        tupleList5.Add(new Tuple<string, float>("basic", 40f));
        tupleList5.Add(new Tuple<string, float>("advanced", 50f));
        tupleListList.Add(tupleList5);
        List<Tuple<string, float>> tupleList6 = new List<Tuple<string, float>>();
        tupleList6.Add(new Tuple<string, float>("basic", 50f));
        tupleList6.Add(new Tuple<string, float>("advanced", 70f));
        tupleListList.Add(tupleList6);
        List<Tuple<string, float>> tupleList7 = new List<Tuple<string, float>>();
        tupleList7.Add(new Tuple<string, float>("basic", 70f));
        tupleList7.Add(new Tuple<string, float>("advanced", 100f));
        tupleListList.Add(tupleList7);
        List<Tuple<string, float>> tupleList8 = new List<Tuple<string, float>>();
        tupleList8.Add(new Tuple<string, float>("basic", 70f));
        tupleList8.Add(new Tuple<string, float>("advanced", 100f));
        tupleList8.Add(new Tuple<string, float>("space", 200f));
        tupleListList.Add(tupleList8);
        List<Tuple<string, float>> tupleList9 = new List<Tuple<string, float>>();
        tupleList9.Add(new Tuple<string, float>("basic", 70f));
        tupleList9.Add(new Tuple<string, float>("advanced", 100f));
        tupleList9.Add(new Tuple<string, float>("space", 400f));
        tupleListList.Add(tupleList9);
        List<Tuple<string, float>> tupleList10 = new List<Tuple<string, float>>();
        tupleList10.Add(new Tuple<string, float>("basic", 70f));
        tupleList10.Add(new Tuple<string, float>("advanced", 100f));
        tupleList10.Add(new Tuple<string, float>("space", 800f));
        tupleListList.Add(tupleList10);
        List<Tuple<string, float>> tupleList11 = new List<Tuple<string, float>>();
        tupleList11.Add(new Tuple<string, float>("basic", 70f));
        tupleList11.Add(new Tuple<string, float>("advanced", 100f));
        tupleList11.Add(new Tuple<string, float>("space", 1600f));
        tupleListList.Add(tupleList11);
        this.TECH_TIERS = tupleListList;
      }
      else
      {
        List<List<Tuple<string, float>>> tupleListList = new List<List<Tuple<string, float>>>();
        tupleListList.Add(new List<Tuple<string, float>>());
        List<Tuple<string, float>> tupleList12 = new List<Tuple<string, float>>();
        tupleList12.Add(new Tuple<string, float>("basic", 15f));
        tupleListList.Add(tupleList12);
        List<Tuple<string, float>> tupleList13 = new List<Tuple<string, float>>();
        tupleList13.Add(new Tuple<string, float>("basic", 20f));
        tupleListList.Add(tupleList13);
        List<Tuple<string, float>> tupleList14 = new List<Tuple<string, float>>();
        tupleList14.Add(new Tuple<string, float>("basic", 30f));
        tupleList14.Add(new Tuple<string, float>("advanced", 20f));
        tupleListList.Add(tupleList14);
        List<Tuple<string, float>> tupleList15 = new List<Tuple<string, float>>();
        tupleList15.Add(new Tuple<string, float>("basic", 35f));
        tupleList15.Add(new Tuple<string, float>("advanced", 30f));
        tupleListList.Add(tupleList15);
        List<Tuple<string, float>> tupleList16 = new List<Tuple<string, float>>();
        tupleList16.Add(new Tuple<string, float>("basic", 40f));
        tupleList16.Add(new Tuple<string, float>("advanced", 50f));
        tupleList16.Add(new Tuple<string, float>("orbital", 0.0f));
        tupleList16.Add(new Tuple<string, float>("nuclear", 20f));
        tupleListList.Add(tupleList16);
        List<Tuple<string, float>> tupleList17 = new List<Tuple<string, float>>();
        tupleList17.Add(new Tuple<string, float>("basic", 50f));
        tupleList17.Add(new Tuple<string, float>("advanced", 70f));
        tupleList17.Add(new Tuple<string, float>("orbital", 30f));
        tupleList17.Add(new Tuple<string, float>("nuclear", 40f));
        tupleListList.Add(tupleList17);
        List<Tuple<string, float>> tupleList18 = new List<Tuple<string, float>>();
        tupleList18.Add(new Tuple<string, float>("basic", 70f));
        tupleList18.Add(new Tuple<string, float>("advanced", 100f));
        tupleList18.Add(new Tuple<string, float>("orbital", 250f));
        tupleList18.Add(new Tuple<string, float>("nuclear", 370f));
        tupleListList.Add(tupleList18);
        List<Tuple<string, float>> tupleList19 = new List<Tuple<string, float>>();
        tupleList19.Add(new Tuple<string, float>("basic", 100f));
        tupleList19.Add(new Tuple<string, float>("advanced", 130f));
        tupleList19.Add(new Tuple<string, float>("orbital", 400f));
        tupleList19.Add(new Tuple<string, float>("nuclear", 435f));
        tupleListList.Add(tupleList19);
        List<Tuple<string, float>> tupleList20 = new List<Tuple<string, float>>();
        tupleList20.Add(new Tuple<string, float>("basic", 100f));
        tupleList20.Add(new Tuple<string, float>("advanced", 130f));
        tupleList20.Add(new Tuple<string, float>("orbital", 600f));
        tupleListList.Add(tupleList20);
        List<Tuple<string, float>> tupleList21 = new List<Tuple<string, float>>();
        tupleList21.Add(new Tuple<string, float>("basic", 100f));
        tupleList21.Add(new Tuple<string, float>("advanced", 130f));
        tupleList21.Add(new Tuple<string, float>("orbital", 800f));
        tupleListList.Add(tupleList21);
        List<Tuple<string, float>> tupleList22 = new List<Tuple<string, float>>();
        tupleList22.Add(new Tuple<string, float>("basic", 100f));
        tupleList22.Add(new Tuple<string, float>("advanced", 130f));
        tupleList22.Add(new Tuple<string, float>("orbital", 1600f));
        tupleListList.Add(tupleList22);
        this.TECH_TIERS = tupleListList;
      }
    }

    public void Init()
    {
      Tech tech1 = new Tech("FarmingTech", new List<string>()
      {
        "AlgaeHabitat",
        "PlanterBox",
        "RationBox",
        "Compost"
      }, this);
      Tech tech2 = new Tech("FineDining", new List<string>()
      {
        "CookingStation",
        "EggCracker",
        "DiningTable",
        "FarmTile"
      }, this);
      Tech tech3 = new Tech("FoodRepurposing", new List<string>()
      {
        "Juicer",
        "SpiceGrinder"
      }, this);
      Tech tech4 = new Tech("FinerDining", new List<string>()
      {
        "GourmetCookingStation"
      }, this);
      Tech tech5 = new Tech("Agriculture", new List<string>()
      {
        "FarmStation",
        "FertilizerMaker",
        "Refrigerator",
        "HydroponicFarm",
        "ParkSign",
        "RadiationLight"
      }, this);
      Tech tech6 = new Tech("Ranching", new List<string>()
      {
        "RanchStation",
        "CreatureDeliveryPoint",
        "ShearingStation",
        "CreatureFeeder",
        "FlyingCreatureBait",
        "FishDeliveryPoint",
        "FishFeeder"
      }, this);
      Tech tech7 = new Tech("AnimalControl", new List<string>()
      {
        "CreatureTrap",
        "FishTrap",
        "EggIncubator",
        LogicCritterCountSensorConfig.ID
      }, this);
      Tech tech8 = new Tech("ImprovedOxygen", new List<string>()
      {
        "Electrolyzer",
        "RustDeoxidizer"
      }, this);
      Tech tech9 = new Tech("GasPiping", new List<string>()
      {
        "GasConduit",
        "GasConduitBridge",
        "GasPump",
        "GasVent"
      }, this);
      Tech tech10 = new Tech("ImprovedGasPiping", new List<string>()
      {
        "InsulatedGasConduit",
        LogicPressureSensorGasConfig.ID,
        "GasLogicValve",
        "GasVentHighPressure"
      }, this);
      Tech tech11 = new Tech("SpaceGas", new List<string>()
      {
        "CO2Engine",
        "ModularLaunchpadPortGas",
        "ModularLaunchpadPortGasUnloader",
        "GasCargoBaySmall"
      }, this);
      Tech tech12 = new Tech("PressureManagement", new List<string>()
      {
        "LiquidValve",
        "GasValve",
        "GasPermeableMembrane",
        "ManualPressureDoor"
      }, this);
      Tech tech13 = new Tech("DirectedAirStreams", new List<string>()
      {
        "AirFilter",
        "CO2Scrubber",
        "PressureDoor"
      }, this);
      Tech tech14 = new Tech("LiquidFiltering", new List<string>()
      {
        "OreScrubber",
        "Desalinator"
      }, this);
      Tech tech15 = new Tech("MedicineI", new List<string>()
      {
        "Apothecary"
      }, this);
      Tech tech16 = new Tech("MedicineII", new List<string>()
      {
        "DoctorStation",
        "HandSanitizer"
      }, this);
      Tech tech17 = new Tech("MedicineIII", new List<string>()
      {
        GasConduitDiseaseSensorConfig.ID,
        LiquidConduitDiseaseSensorConfig.ID,
        LogicDiseaseSensorConfig.ID
      }, this);
      Tech tech18 = new Tech("MedicineIV", new List<string>()
      {
        "AdvancedDoctorStation",
        "AdvancedApothecary",
        "HotTub",
        LogicRadiationSensorConfig.ID
      }, this);
      Tech tech19 = new Tech("LiquidPiping", new List<string>()
      {
        "LiquidConduit",
        "LiquidConduitBridge",
        "LiquidPump",
        "LiquidVent"
      }, this);
      Tech tech20 = new Tech("ImprovedLiquidPiping", new List<string>()
      {
        "InsulatedLiquidConduit",
        LogicPressureSensorLiquidConfig.ID,
        "LiquidLogicValve",
        "LiquidConduitPreferentialFlow",
        "LiquidConduitOverflow",
        "LiquidReservoir"
      }, this);
      Tech tech21 = new Tech("PrecisionPlumbing", new List<string>()
      {
        "EspressoMachine",
        "LiquidFuelTankCluster"
      }, this);
      Tech tech22 = new Tech("SanitationSciences", new List<string>()
      {
        "FlushToilet",
        "WashSink",
        ShowerConfig.ID,
        "MeshTile"
      }, this);
      Tech tech23 = new Tech("FlowRedirection", new List<string>()
      {
        "MechanicalSurfboard",
        "ModularLaunchpadPortLiquid",
        "ModularLaunchpadPortLiquidUnloader",
        "LiquidCargoBaySmall"
      }, this);
      Tech tech24 = new Tech("LiquidDistribution", new List<string>()
      {
        "RocketInteriorLiquidInput",
        "RocketInteriorLiquidOutput",
        "WallToilet"
      }, this);
      Tech tech25 = new Tech("AdvancedSanitation", new List<string>()
      {
        "DecontaminationShower"
      }, this);
      Tech tech26 = new Tech("AdvancedFiltration", new List<string>()
      {
        "GasFilter",
        "LiquidFilter",
        "SludgePress"
      }, this);
      Tech tech27 = new Tech("Distillation", new List<string>()
      {
        "AlgaeDistillery",
        "EthanolDistillery",
        "WaterPurifier"
      }, this);
      Tech tech28 = new Tech("Catalytics", new List<string>()
      {
        "OxyliteRefinery",
        "SupermaterialRefinery",
        "SodaFountain",
        "GasCargoBayCluster"
      }, this);
      Tech tech29 = new Tech("AdvancedResourceExtraction", new List<string>()
      {
        "NoseconeHarvest"
      }, this);
      Tech tech30 = new Tech("PowerRegulation", new List<string>()
      {
        "BatteryMedium",
        SwitchConfig.ID,
        "WireBridge"
      }, this);
      Tech tech31 = new Tech("AdvancedPowerRegulation", new List<string>()
      {
        "HighWattageWire",
        "WireBridgeHighWattage",
        "HydrogenGenerator",
        LogicPowerRelayConfig.ID,
        "PowerTransformerSmall",
        LogicWattageSensorConfig.ID
      }, this);
      Tech tech32 = new Tech("PrettyGoodConductors", new List<string>()
      {
        "WireRefined",
        "WireRefinedBridge",
        "WireRefinedHighWattage",
        "WireRefinedBridgeHighWattage",
        "PowerTransformer"
      }, this);
      Tech tech33 = new Tech("RenewableEnergy", new List<string>()
      {
        "SteamTurbine2",
        "SolarPanel",
        "Sauna",
        "SteamEngineCluster"
      }, this);
      Tech tech34 = new Tech("Combustion", new List<string>()
      {
        "Generator",
        "WoodGasGenerator"
      }, this);
      Tech tech35 = new Tech("ImprovedCombustion", new List<string>()
      {
        "MethaneGenerator",
        "OilRefinery",
        "PetroleumGenerator"
      }, this);
      Tech tech36 = new Tech("InteriorDecor", new List<string>()
      {
        "FlowerVase",
        "FloorLamp",
        "CeilingLight"
      }, this);
      Tech tech37 = new Tech("Artistry", new List<string>()
      {
        "FlowerVaseWall",
        "FlowerVaseHanging",
        "CornerMoulding",
        "CrownMoulding",
        "ItemPedestal",
        "SmallSculpture",
        "IceSculpture"
      }, this);
      Tech tech38 = new Tech("Clothing", new List<string>()
      {
        "ClothingFabricator",
        "CarpetTile",
        "ExteriorWall"
      }, this);
      Tech tech39 = new Tech("Acoustics", new List<string>()
      {
        "BatterySmart",
        "Phonobox",
        "PowerControlStation"
      }, this);
      Tech tech40 = new Tech("SpacePower", new List<string>()
      {
        "BatteryModule",
        "SolarPanelModule",
        "RocketInteriorPowerPlug"
      }, this);
      Tech tech41 = new Tech("NuclearRefinement", new List<string>()
      {
        "NuclearReactor",
        "UraniumCentrifuge",
        "HEPBridgeTile"
      }, this);
      Tech tech42 = new Tech("FineArt", new List<string>()
      {
        "Canvas",
        "Sculpture"
      }, this);
      Tech tech43 = new Tech("EnvironmentalAppreciation", new List<string>()
      {
        "BeachChair"
      }, this);
      Tech tech44 = new Tech("Luxury", new List<string>()
      {
        LuxuryBedConfig.ID,
        "LadderFast",
        "PlasticTile",
        "ClothingAlterationStation"
      }, this);
      Tech tech45 = new Tech("RefractiveDecor", new List<string>()
      {
        "CanvasWide",
        "MetalSculpture"
      }, this);
      Tech tech46 = new Tech("GlassFurnishings", new List<string>()
      {
        "GlassTile",
        "FlowerVaseHangingFancy",
        "SunLamp"
      }, this);
      Tech tech47 = new Tech("Screens", new List<string>()
      {
        PixelPackConfig.ID
      }, this);
      Tech tech48 = new Tech("RenaissanceArt", new List<string>()
      {
        "CanvasTall",
        "MarbleSculpture"
      }, this);
      Tech tech49 = new Tech("Plastics", new List<string>()
      {
        "Polymerizer",
        "OilWellCap"
      }, this);
      Tech tech50 = new Tech("ValveMiniaturization", new List<string>()
      {
        "LiquidMiniPump",
        "GasMiniPump"
      }, this);
      Tech tech51 = new Tech("HydrocarbonPropulsion", new List<string>()
      {
        "KeroseneEngineClusterSmall",
        "MissionControlCluster"
      }, this);
      Tech tech52 = new Tech("BetterHydroCarbonPropulsion", new List<string>()
      {
        "KeroseneEngineCluster"
      }, this);
      Tech tech53 = new Tech("CryoFuelPropulsion", new List<string>()
      {
        "HydrogenEngineCluster",
        "OxidizerTankLiquidCluster"
      }, this);
      Tech tech54 = new Tech("Suits", new List<string>()
      {
        "SuitsOverlay",
        "AtmoSuit",
        "SuitFabricator",
        "SuitMarker",
        "SuitLocker"
      }, this);
      Tech tech55 = new Tech("Jobs", new List<string>()
      {
        "WaterCooler",
        "CraftingTable"
      }, this);
      Tech tech56 = new Tech("AdvancedResearch", new List<string>()
      {
        "BetaResearchPoint",
        "AdvancedResearchCenter",
        "ResetSkillsStation",
        "ClusterTelescope",
        "ExobaseHeadquarters"
      }, this);
      Tech tech57 = new Tech("SpaceProgram", new List<string>()
      {
        "LaunchPad",
        "HabitatModuleSmall",
        "OrbitalCargoModule",
        RocketControlStationConfig.ID
      }, this);
      Tech tech58 = new Tech("CrashPlan", new List<string>()
      {
        "OrbitalResearchPoint",
        "PioneerModule",
        "OrbitalResearchCenter",
        "DLC1CosmicResearchCenter"
      }, this);
      Tech tech59 = new Tech("DurableLifeSupport", new List<string>()
      {
        "NoseconeBasic",
        "HabitatModuleMedium",
        "ArtifactAnalysisStation",
        "ArtifactCargoBay"
      }, this);
      Tech tech60 = new Tech("NuclearResearch", new List<string>()
      {
        "DeltaResearchPoint",
        "NuclearResearchCenter",
        "ManualHighEnergyParticleSpawner"
      }, this);
      Tech tech61 = new Tech("AdvancedNuclearResearch", new List<string>()
      {
        "HighEnergyParticleSpawner",
        "HighEnergyParticleRedirector"
      }, this);
      Tech tech62 = new Tech("NuclearStorage", new List<string>()
      {
        "HEPBattery"
      }, this);
      Tech tech63 = new Tech("NuclearPropulsion", new List<string>()
      {
        "HEPEngine"
      }, this);
      Tech tech64 = new Tech("NotificationSystems", new List<string>()
      {
        LogicHammerConfig.ID,
        LogicAlarmConfig.ID,
        "Telephone"
      }, this);
      Tech tech65 = new Tech("ArtificialFriends", new List<string>()
      {
        "SweepBotStation",
        "ScoutModule"
      }, this);
      Tech tech66 = new Tech("BasicRefinement", new List<string>()
      {
        "RockCrusher",
        "Kiln"
      }, this);
      Tech tech67 = new Tech("RefinedObjects", new List<string>()
      {
        "FirePole",
        "ThermalBlock",
        LadderBedConfig.ID
      }, this);
      Tech tech68 = new Tech("Smelting", new List<string>()
      {
        "MetalRefinery",
        "MetalTile"
      }, this);
      Tech tech69 = new Tech("HighTempForging", new List<string>()
      {
        "GlassForge",
        "BunkerTile",
        "BunkerDoor",
        "GeoTuner"
      }, this);
      Tech tech70 = new Tech("HighPressureForging", new List<string>()
      {
        "DiamondPress"
      }, this);
      Tech tech71 = new Tech("RadiationProtection", new List<string>()
      {
        "LeadSuit",
        "LeadSuitMarker",
        "LeadSuitLocker",
        LogicHEPSensorConfig.ID
      }, this);
      Tech tech72 = new Tech("TemperatureModulation", new List<string>()
      {
        "LiquidCooledFan",
        "IceCooledFan",
        "IceMachine",
        "InsulationTile",
        "SpaceHeater"
      }, this);
      Tech tech73 = new Tech("HVAC", new List<string>()
      {
        "AirConditioner",
        LogicTemperatureSensorConfig.ID,
        GasConduitTemperatureSensorConfig.ID,
        GasConduitElementSensorConfig.ID,
        "GasConduitRadiant",
        "GasReservoir",
        "GasLimitValve"
      }, this);
      Tech tech74 = new Tech("LiquidTemperature", new List<string>()
      {
        "LiquidConduitRadiant",
        "LiquidConditioner",
        LiquidConduitTemperatureSensorConfig.ID,
        LiquidConduitElementSensorConfig.ID,
        "LiquidHeater",
        "LiquidLimitValve",
        "ContactConductivePipeBridge"
      }, this);
      Tech tech75 = new Tech("LogicControl", new List<string>()
      {
        "AutomationOverlay",
        LogicSwitchConfig.ID,
        "LogicWire",
        "LogicWireBridge",
        "LogicDuplicantSensor"
      }, this);
      Tech tech76 = new Tech("GenericSensors", new List<string>()
      {
        "FloorSwitch",
        LogicElementSensorGasConfig.ID,
        LogicElementSensorLiquidConfig.ID,
        "LogicGateNOT",
        LogicTimeOfDaySensorConfig.ID,
        LogicTimerSensorConfig.ID,
        LogicClusterLocationSensorConfig.ID
      }, this);
      Tech tech77 = new Tech("LogicCircuits", new List<string>()
      {
        "LogicGateAND",
        "LogicGateOR",
        "LogicGateBUFFER",
        "LogicGateFILTER"
      }, this);
      Tech tech78 = new Tech("ParallelAutomation", new List<string>()
      {
        "LogicRibbon",
        "LogicRibbonBridge",
        LogicRibbonWriterConfig.ID,
        LogicRibbonReaderConfig.ID
      }, this);
      Tech tech79 = new Tech("DupeTrafficControl", new List<string>()
      {
        LogicCounterConfig.ID,
        LogicMemoryConfig.ID,
        "LogicGateXOR",
        "ArcadeMachine",
        "Checkpoint",
        "CosmicResearchCenter"
      }, this);
      Tech tech80 = new Tech("Multiplexing", new List<string>()
      {
        "LogicGateMultiplexer",
        "LogicGateDemultiplexer"
      }, this);
      Tech tech81 = new Tech("SkyDetectors", new List<string>()
      {
        CometDetectorConfig.ID,
        "Telescope",
        "ClusterTelescopeEnclosed",
        "AstronautTrainingCenter"
      }, this);
      Tech tech82 = new Tech("TravelTubes", new List<string>()
      {
        "TravelTubeEntrance",
        "TravelTube",
        "TravelTubeWallBridge",
        "VerticalWindTunnel"
      }, this);
      Tech tech83 = new Tech("SmartStorage", new List<string>()
      {
        "ConveyorOverlay",
        "SolidTransferArm",
        "StorageLockerSmart",
        "ObjectDispenser"
      }, this);
      Tech tech84 = new Tech("SolidManagement", new List<string>()
      {
        "SolidFilter",
        SolidConduitTemperatureSensorConfig.ID,
        SolidConduitElementSensorConfig.ID,
        SolidConduitDiseaseSensorConfig.ID,
        "CargoBayCluster"
      }, this);
      Tech tech85 = new Tech("HighVelocityTransport", new List<string>()
      {
        "RailGun",
        "LandingBeacon"
      }, this);
      Tech tech86 = new Tech("BasicRocketry", new List<string>()
      {
        "CommandModule",
        "SteamEngine",
        "ResearchModule",
        "Gantry"
      }, this);
      Tech tech87 = new Tech("CargoI", new List<string>()
      {
        "CargoBay"
      }, this);
      Tech tech88 = new Tech("CargoII", new List<string>()
      {
        "LiquidCargoBay",
        "GasCargoBay"
      }, this);
      Tech tech89 = new Tech("CargoIII", new List<string>()
      {
        "TouristModule",
        "SpecialCargoBay"
      }, this);
      Tech tech90 = new Tech("EnginesI", new List<string>()
      {
        "SolidBooster",
        "MissionControl"
      }, this);
      Tech tech91 = new Tech("EnginesII", new List<string>()
      {
        "KeroseneEngine",
        "LiquidFuelTank",
        "OxidizerTank"
      }, this);
      Tech tech92 = new Tech("EnginesIII", new List<string>()
      {
        "OxidizerTankLiquid",
        "OxidizerTankCluster",
        "HydrogenEngine"
      }, this);
      Tech tech93 = new Tech("Jetpacks", new List<string>()
      {
        "JetSuit",
        "JetSuitMarker",
        "JetSuitLocker",
        "LiquidCargoBayCluster"
      }, this);
      Tech tech94 = new Tech("SolidTransport", new List<string>()
      {
        "SolidConduitInbox",
        "SolidConduit",
        "SolidConduitBridge",
        "SolidVent"
      }, this);
      Tech tech95 = new Tech("Monuments", new List<string>()
      {
        "MonumentBottom",
        "MonumentMiddle",
        "MonumentTop"
      }, this);
      Tech tech96 = new Tech("SolidSpace", new List<string>()
      {
        "SolidLogicValve",
        "SolidConduitOutbox",
        "SolidLimitValve",
        "SolidCargoBaySmall",
        "RocketInteriorSolidInput",
        "RocketInteriorSolidOutput",
        "ModularLaunchpadPortSolid",
        "ModularLaunchpadPortSolidUnloader"
      }, this);
      Tech tech97 = new Tech("RoboticTools", new List<string>()
      {
        "AutoMiner",
        "RailGunPayloadOpener"
      }, this);
      Tech tech98 = new Tech("PortableGasses", new List<string>()
      {
        "GasBottler",
        "BottleEmptierGas",
        "OxygenMask",
        "OxygenMaskLocker",
        "OxygenMaskMarker"
      }, this);
      this.InitExpansion1();
    }

    private void InitExpansion1()
    {
      if (!DlcManager.IsExpansion1Active())
        return;
      this.Get("HighTempForging").AddUnlockedItemIDs("Gantry");
      Tech tech1 = new Tech("Bioengineering", new List<string>()
      {
        "GeneticAnalysisStation"
      }, this);
      Tech tech2 = new Tech("SpaceCombustion", new List<string>()
      {
        "SugarEngine",
        "SmallOxidizerTank"
      }, this);
      Tech tech3 = new Tech("HighVelocityDestruction", new List<string>()
      {
        "NoseconeHarvest"
      }, this);
      Tech tech4 = new Tech("GasDistribution", new List<string>()
      {
        "RocketInteriorGasInput",
        "RocketInteriorGasOutput",
        "OxidizerTankCluster"
      }, this);
      Tech tech5 = new Tech("AdvancedScanners", new List<string>()
      {
        "ScannerModule",
        "LogicInterasteroidSender",
        "LogicInterasteroidReceiver"
      }, this);
    }

    public void PostProcess()
    {
      foreach (Tech resource in this.resources)
      {
        List<TechItem> techItemList = new List<TechItem>();
        foreach (string unlockedItemId in resource.unlockedItemIDs)
        {
          TechItem techItem = Db.Get().TechItems.TryGet(unlockedItemId);
          if (techItem != null)
            techItemList.Add(techItem);
        }
        resource.unlockedItems = techItemList;
      }
    }

    public void Load(TextAsset tree_file)
    {
      ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
      List<TechTreeTitle> techTreeTitleList = new List<TechTreeTitle>();
      for (int index = 0; index < ((ResourceSet) Db.Get().TechTreeTitles).Count; ++index)
        techTreeTitleList.Add(Db.Get().TechTreeTitles[index]);
      techTreeTitleList.Sort((Comparison<TechTreeTitle>) ((a, b) => a.center.y.CompareTo(b.center.y)));
      foreach (ResourceTreeNode node in (ResourceLoader<ResourceTreeNode>) resourceTreeLoader)
      {
        if (!string.Equals(((Resource) node).Id.Substring(0, 1), "_"))
        {
          Tech tech1 = this.TryGet(((Resource) node).Id);
          Debug.Assert(tech1 != null, (object) ("Tech node found in yEd that is not found in DbTechs constructor: " + ((Resource) node).Id));
          string categoryID1 = "";
          for (int index = 0; index < techTreeTitleList.Count; ++index)
          {
            if ((double) techTreeTitleList[index].center.y >= (double) node.center.y)
            {
              categoryID1 = techTreeTitleList[index].Id;
              break;
            }
          }
          tech1.SetNode(node, categoryID1);
          foreach (ResourceTreeNode reference in node.references)
          {
            Tech tech2 = this.TryGet(((Resource) reference).Id);
            Debug.Assert(tech2 != null, (object) ("Tech node found in yEd that is not found in DbTechs constructor: " + ((Resource) reference).Id));
            string categoryID2 = "";
            for (int index = 0; index < techTreeTitleList.Count; ++index)
            {
              if ((double) techTreeTitleList[index].center.y >= (double) node.center.y)
              {
                categoryID2 = techTreeTitleList[index].Id;
                break;
              }
            }
            tech2.SetNode(reference, categoryID2);
            tech2.requiredTech.Add(tech1);
            tech1.unlockedTech.Add(tech2);
          }
        }
      }
      foreach (Tech resource in this.resources)
      {
        resource.tier = Techs.GetTier(resource);
        foreach (Tuple<string, float> tuple in this.TECH_TIERS[resource.tier])
        {
          if (!resource.costsByResearchTypeID.ContainsKey(tuple.first))
            resource.costsByResearchTypeID.Add(tuple.first, tuple.second);
        }
      }
      for (int index = ((ResourceSet) this).Count - 1; index >= 0; --index)
      {
        if (!((Tech) ((ResourceSet) this).GetResource(index)).FoundNode)
          ((ResourceSet) this).Remove(((ResourceSet) this).GetResource(index));
      }
    }

    public static int GetTier(Tech tech)
    {
      if (tech == null)
        return 0;
      int val1 = 0;
      foreach (Tech tech1 in tech.requiredTech)
        val1 = Math.Max(val1, Techs.GetTier(tech1));
      return val1 + 1;
    }

    private void AddPrerequisite(Tech tech, string prerequisite_name)
    {
      Tech tech1 = this.TryGet(prerequisite_name);
      if (tech1 == null)
        return;
      tech.requiredTech.Add(tech1);
      tech1.unlockedTech.Add(tech);
    }

    public Tech TryGetTechForTechItem(string itemId)
    {
      for (int index = 0; index < ((ResourceSet) this).Count; ++index)
      {
        Tech resource = (Tech) ((ResourceSet) this).GetResource(index);
        if (resource.unlockedItemIDs.Find((Predicate<string>) (match => match == itemId)) != null)
          return resource;
      }
      return (Tech) null;
    }

    public bool IsTechItemComplete(string id)
    {
      foreach (Tech resource in this.resources)
      {
        foreach (Resource unlockedItem in resource.unlockedItems)
        {
          if (unlockedItem.Id == id)
            return resource.IsComplete();
        }
      }
      return true;
    }
  }
}
