// Decompiled with JetBrains decompiler
// Type: STRINGS.RESEARCH
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace STRINGS
{
  public class RESEARCH
  {
    public class MESSAGING
    {
      public static LocString NORESEARCHSELECTED = (LocString) "No research selected";
      public static LocString RESEARCHTYPEREQUIRED = (LocString) "{0} required";
      public static LocString RESEARCHTYPEALSOREQUIRED = (LocString) "{0} also required";
      public static LocString NO_RESEARCHER_SKILL = (LocString) "No Researchers assigned";
      public static LocString NO_RESEARCHER_SKILL_TOOLTIP = (LocString) ("The selected research focus requires {ResearchType} to complete\n\nOpen the " + UI.FormatAsManagementMenu("Skills Panel", (Action) 116) + " and teach a Duplicant the {ResearchType} Skill to use this building");
      public static LocString MISSING_RESEARCH_STATION = (LocString) "Missing Research Station";
      public static LocString MISSING_RESEARCH_STATION_TOOLTIP = (LocString) ("The selected research focus requires a {0} to perform\n\nOpen the " + UI.FormatAsBuildMenuTab("Stations Tab", (Action) 45) + " of the Build Menu to construct one");

      public static class DLC
      {
        public static LocString EXPANSION1 = (LocString) (UI.PRE_KEYWORD + "\n\n<i>" + (string) UI.DLC1.NAME + "</i>" + UI.PST_KEYWORD + " DLC Content");
      }
    }

    public class TYPES
    {
      public static LocString MISSINGRECIPEDESC = (LocString) "Missing Recipe Description";

      public class ALPHA
      {
        public static LocString NAME = (LocString) "Novice Research";
        public static LocString DESC = (LocString) (UI.FormatAsLink("Novice Research", nameof (RESEARCH)) + " is required to unlock basic technologies.\nIt can be conducted at a " + UI.FormatAsLink("Research Station", "RESEARCHCENTER") + ".");
        public static LocString RECIPEDESC = (LocString) "Unlocks rudimentary technologies.";
      }

      public class BETA
      {
        public static LocString NAME = (LocString) "Advanced Research";
        public static LocString DESC = (LocString) (UI.FormatAsLink("Advanced Research", nameof (RESEARCH)) + " is required to unlock improved technologies.\nIt can be conducted at a " + UI.FormatAsLink("Super Computer", "ADVANCEDRESEARCHCENTER") + ".");
        public static LocString RECIPEDESC = (LocString) "Unlocks improved technologies.";
      }

      public class GAMMA
      {
        public static LocString NAME = (LocString) "Interstellar Research";
        public static LocString DESC = (LocString) (UI.FormatAsLink("Interstellar Research", nameof (RESEARCH)) + " is required to unlock space technologies.\nIt can be conducted at a " + UI.FormatAsLink("Virtual Planetarium", "COSMICRESEARCHCENTER") + ".");
        public static LocString RECIPEDESC = (LocString) "Unlocks cutting-edge technologies.";
      }

      public class DELTA
      {
        public static LocString NAME = (LocString) "Applied Sciences Research";
        public static LocString DESC = (LocString) (UI.FormatAsLink("Applied Sciences Research", nameof (RESEARCH)) + " is required to unlock materials science technologies.\nIt can be conducted at a " + UI.FormatAsLink("Materials Study Terminal", "NUCLEARRESEARCHCENTER") + ".");
        public static LocString RECIPEDESC = (LocString) "Unlocks next wave technologies.";
      }

      public class ORBITAL
      {
        public static LocString NAME = (LocString) "Data Analysis Research";
        public static LocString DESC = (LocString) (UI.FormatAsLink("Data Analysis Research", nameof (RESEARCH)) + " is required to unlock Data Analysis technologies.\nIt can be conducted at a " + UI.FormatAsLink("Orbital Data Collection Lab", "ORBITALRESEARCHCENTER") + ".");
        public static LocString RECIPEDESC = (LocString) "Unlocks out-of-this-world technologies.";
      }
    }

    public class OTHER_TECH_ITEMS
    {
      public class AUTOMATION_OVERLAY
      {
        public static LocString NAME = (LocString) UI.FormatAsOverlay("Automation Overlay");
        public static LocString DESC = (LocString) ("Enables access to the " + UI.FormatAsOverlay("Automation Overlay") + ".");
      }

      public class SUITS_OVERLAY
      {
        public static LocString NAME = (LocString) UI.FormatAsOverlay("Exosuit Overlay");
        public static LocString DESC = (LocString) ("Enables access to the " + UI.FormatAsOverlay("Exosuit Overlay") + ".");
      }

      public class JET_SUIT
      {
        public static LocString NAME = (LocString) (UI.PRE_KEYWORD + "Jet Suit" + UI.PST_KEYWORD + " Pattern");
        public static LocString DESC = (LocString) ("Enables fabrication of " + UI.PRE_KEYWORD + "Jet Suits" + UI.PST_KEYWORD + " at the " + (string) BUILDINGS.PREFABS.SUITFABRICATOR.NAME);
      }

      public class OXYGEN_MASK
      {
        public static LocString NAME = (LocString) (UI.PRE_KEYWORD + "Oxygen Mask" + UI.PST_KEYWORD + " Pattern");
        public static LocString DESC = (LocString) ("Enables fabrication of " + UI.PRE_KEYWORD + "Oxygen Masks" + UI.PST_KEYWORD + " at the " + (string) BUILDINGS.PREFABS.CRAFTINGTABLE.NAME);
      }

      public class LEAD_SUIT
      {
        public static LocString NAME = (LocString) (UI.PRE_KEYWORD + "Lead Suit" + UI.PST_KEYWORD + " Pattern");
        public static LocString DESC = (LocString) ("Enables fabrication of " + UI.PRE_KEYWORD + "Lead Suits" + UI.PST_KEYWORD + " at the " + (string) BUILDINGS.PREFABS.SUITFABRICATOR.NAME);
      }

      public class ATMO_SUIT
      {
        public static LocString NAME = (LocString) (UI.PRE_KEYWORD + "Atmo Suit" + UI.PST_KEYWORD + " Pattern");
        public static LocString DESC = (LocString) ("Enables fabrication of " + UI.PRE_KEYWORD + "Atmo Suits" + UI.PST_KEYWORD + " at the " + (string) BUILDINGS.PREFABS.SUITFABRICATOR.NAME);
      }

      public class BETA_RESEARCH_POINT
      {
        public static LocString NAME = (LocString) (UI.PRE_KEYWORD + "Advanced Research" + UI.PST_KEYWORD + " Capability");
        public static LocString DESC = (LocString) ("Allows " + UI.PRE_KEYWORD + "Advanced Research" + UI.PST_KEYWORD + " points to be accumulated, unlocking higher technology tiers.");
      }

      public class GAMMA_RESEARCH_POINT
      {
        public static LocString NAME = (LocString) (UI.PRE_KEYWORD + "Interstellar Research" + UI.PST_KEYWORD + " Capability");
        public static LocString DESC = (LocString) ("Allows " + UI.PRE_KEYWORD + "Interstellar Research" + UI.PST_KEYWORD + " points to be accumulated, unlocking higher technology tiers.");
      }

      public class DELTA_RESEARCH_POINT
      {
        public static LocString NAME = (LocString) (UI.PRE_KEYWORD + "Materials Science Research" + UI.PST_KEYWORD + " Capability");
        public static LocString DESC = (LocString) ("Allows " + UI.PRE_KEYWORD + "Materials Science Research" + UI.PST_KEYWORD + " points to be accumulated, unlocking higher technology tiers.");
      }

      public class ORBITAL_RESEARCH_POINT
      {
        public static LocString NAME = (LocString) (UI.PRE_KEYWORD + "Data Analysis Research" + UI.PST_KEYWORD + " Capability");
        public static LocString DESC = (LocString) ("Allows " + UI.PRE_KEYWORD + "Data Analysis Research" + UI.PST_KEYWORD + " points to be accumulated, unlocking higher technology tiers.");
      }

      public class CONVEYOR_OVERLAY
      {
        public static LocString NAME = (LocString) UI.FormatAsOverlay("Conveyor Overlay");
        public static LocString DESC = (LocString) ("Enables access to the " + UI.FormatAsOverlay("Conveyor Overlay") + ".");
      }
    }

    public class TREES
    {
      public static LocString TITLE_FOOD = (LocString) "Food";
      public static LocString TITLE_POWER = (LocString) "Power";
      public static LocString TITLE_SOLIDS = (LocString) "Solid Material";
      public static LocString TITLE_COLONYDEVELOPMENT = (LocString) "Colony Development";
      public static LocString TITLE_RADIATIONTECH = (LocString) "Radiation Technologies";
      public static LocString TITLE_MEDICINE = (LocString) "Medicine";
      public static LocString TITLE_LIQUIDS = (LocString) "Liquids";
      public static LocString TITLE_GASES = (LocString) "Gases";
      public static LocString TITLE_SUITS = (LocString) "Exosuits";
      public static LocString TITLE_DECOR = (LocString) "Decor";
      public static LocString TITLE_COMPUTERS = (LocString) "Computers";
      public static LocString TITLE_ROCKETS = (LocString) "Rocketry";
    }

    public class TECHS
    {
      public class JOBS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Employment", nameof (JOBS));
        public static LocString DESC = (LocString) "Exchange the skill points earned by Duplicants for new traits and abilities.";
      }

      public class IMPROVEDOXYGEN
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Air Systems", nameof (IMPROVEDOXYGEN));
        public static LocString DESC = (LocString) "Maintain clean, breathable air in the colony.";
      }

      public class FARMINGTECH
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Basic Farming", nameof (FARMINGTECH));
        public static LocString DESC = (LocString) ("Learn the introductory principles of " + UI.FormatAsLink("Plant", "PLANTS") + " domestication.");
      }

      public class AGRICULTURE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Agriculture", nameof (AGRICULTURE));
        public static LocString DESC = (LocString) "Master the agricultural art of crop raising.";
      }

      public class RANCHING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Ranching", nameof (RANCHING));
        public static LocString DESC = (LocString) "Tame and care for wild critters.";
      }

      public class ANIMALCONTROL
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Animal Control", nameof (ANIMALCONTROL));
        public static LocString DESC = (LocString) "Useful techniques to manage critter populations in the colony.";
      }

      public class FOODREPURPOSING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Food Repurposing", nameof (FOODREPURPOSING));
        public static LocString DESC = (LocString) ("Blend that leftover " + UI.FormatAsLink("Food", "FOOD") + " into a " + UI.FormatAsLink("Morale", "MORALE") + " boosting slurry.");
      }

      public class FINEDINING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Meal Preparation", nameof (FINEDINING));
        public static LocString DESC = (LocString) ("Prepare more nutritious " + UI.FormatAsLink("Food", "FOOD") + " and store it longer before spoiling.");
      }

      public class FINERDINING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Gourmet Meal Preparation", nameof (FINERDINING));
        public static LocString DESC = (LocString) ("Raise colony Morale by cooking the most delicious, high-quality " + UI.FormatAsLink("Foods", "FOOD") + ".");
      }

      public class GASPIPING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Ventilation", nameof (GASPIPING));
        public static LocString DESC = (LocString) ("Rudimentary technologies for installing " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " infrastructure.");
      }

      public class IMPROVEDGASPIPING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Ventilation", nameof (IMPROVEDGASPIPING));
        public static LocString DESC = (LocString) (UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " infrastructure capable of withstanding more intense conditions, such as " + UI.FormatAsLink("Heat", "Heat") + " and pressure.");
      }

      public class FLOWREDIRECTION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Flow Redirection", nameof (FLOWREDIRECTION));
        public static LocString DESC = (LocString) ("Balance on irrigated concave platforms for a " + UI.FormatAsLink("Morale", "MORALE") + " boost.");
      }

      public class LIQUIDDISTRIBUTION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Liquid Distribution", nameof (LIQUIDDISTRIBUTION));
        public static LocString DESC = (LocString) "Internal rocket hookups for liquid resources.";
      }

      public class TEMPERATUREMODULATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Temperature Modulation", nameof (TEMPERATUREMODULATION));
        public static LocString DESC = (LocString) ("Precise " + UI.FormatAsLink("Temperature", "HEAT") + " altering technologies to keep my colony at the perfect Kelvin.");
      }

      public class HVAC
      {
        public static LocString NAME = (LocString) UI.FormatAsLink(nameof (HVAC), nameof (HVAC));
        public static LocString DESC = (LocString) ("Regulate " + UI.FormatAsLink("Temperature", "HEAT") + " in the colony for " + UI.FormatAsLink("Plant", "PLANTS") + " cultivation and Duplicant comfort.");
      }

      public class GASDISTRIBUTION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Gas Distribution", nameof (GASDISTRIBUTION));
        public static LocString DESC = (LocString) "Internal rocket hookups for gas resources.";
      }

      public class LIQUIDTEMPERATURE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Liquid Tuning", nameof (LIQUIDTEMPERATURE));
        public static LocString DESC = (LocString) ("Easily manipulate " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " " + UI.FormatAsLink("Heat", "Temperatures") + " with these temperature regulating technologies.");
      }

      public class INSULATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Insulation", nameof (INSULATION));
        public static LocString DESC = (LocString) ("Improve " + UI.FormatAsLink("Heat", "Heat") + " distribution within the colony and guard buildings from extreme temperatures.");
      }

      public class PRESSUREMANAGEMENT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pressure Management", nameof (PRESSUREMANAGEMENT));
        public static LocString DESC = (LocString) "Unlock technologies to manage colony pressure and atmosphere.";
      }

      public class PORTABLEGASSES
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Portable Gases", nameof (PORTABLEGASSES));
        public static LocString DESC = (LocString) "Unlock technologies to easily move gases around your colony.";
      }

      public class DIRECTEDAIRSTREAMS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Decontamination", nameof (DIRECTEDAIRSTREAMS));
        public static LocString DESC = (LocString) ("Instruments to help reduce " + UI.FormatAsLink("Germ", "DISEASE") + " spread within the base.");
      }

      public class LIQUIDFILTERING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Liquid-Based Refinement Processes", nameof (LIQUIDFILTERING));
        public static LocString DESC = (LocString) ("Use pumped " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " to filter out unwanted elements.");
      }

      public class LIQUIDPIPING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Plumbing", nameof (LIQUIDPIPING));
        public static LocString DESC = (LocString) ("Rudimentary technologies for installing " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " infrastructure.");
      }

      public class IMPROVEDLIQUIDPIPING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Plumbing", nameof (IMPROVEDLIQUIDPIPING));
        public static LocString DESC = (LocString) (UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " infrastructure capable of withstanding more intense conditions, such as " + UI.FormatAsLink("Heat", "Heat") + " and pressure.");
      }

      public class PRECISIONPLUMBING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Advanced Caffeination", nameof (PRECISIONPLUMBING));
        public static LocString DESC = (LocString) "Let Duplicants relax after a long day of subterranean digging with a shot of warm beanjuice.";
      }

      public class SANITATIONSCIENCES
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Sanitation", nameof (SANITATIONSCIENCES));
        public static LocString DESC = (LocString) "Make daily ablutions less of a hassle.";
      }

      public class ADVANCEDSANITATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Advanced Sanitation", nameof (ADVANCEDSANITATION));
        public static LocString DESC = (LocString) "Clean up dirty Duplicants.";
      }

      public class MEDICINEI
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pharmacology", nameof (MEDICINEI));
        public static LocString DESC = (LocString) ("Compound natural cures to fight the most common " + UI.FormatAsLink("Sicknesses", "SICKNESSES") + " that plague Duplicants.");
      }

      public class MEDICINEII
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Medical Equipment", nameof (MEDICINEII));
        public static LocString DESC = (LocString) "The basic necessities doctors need to facilitate patient care.";
      }

      public class MEDICINEIII
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pathogen Diagnostics", nameof (MEDICINEIII));
        public static LocString DESC = (LocString) "Stop Germs at the source using special medical automation technology.";
      }

      public class MEDICINEIV
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Micro-Targeted Medicine", nameof (MEDICINEIV));
        public static LocString DESC = (LocString) "State of the art equipment to conquer the most stubborn of illnesses.";
      }

      public class ADVANCEDFILTRATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Filtration", nameof (ADVANCEDFILTRATION));
        public static LocString DESC = (LocString) ("Basic technologies for filtering " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Gases", "ELEMENTS_GAS") + ".");
      }

      public class POWERREGULATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Power Regulation", nameof (POWERREGULATION));
        public static LocString DESC = (LocString) ("Prevent wasted " + UI.FormatAsLink("Power", "POWER") + " with improved electrical tools.");
      }

      public class COMBUSTION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Internal Combustion", nameof (COMBUSTION));
        public static LocString DESC = (LocString) ("Fuel-powered generators for crude yet powerful " + UI.FormatAsLink("Power", "POWER") + " production.");
      }

      public class IMPROVEDCOMBUSTION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Fossil Fuels", nameof (IMPROVEDCOMBUSTION));
        public static LocString DESC = (LocString) ("Burn dirty fuels for exceptional " + UI.FormatAsLink("Power", "POWER") + " production.");
      }

      public class INTERIORDECOR
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Interior Decor", nameof (INTERIORDECOR));
        public static LocString DESC = (LocString) (UI.FormatAsLink("Decor", "DECOR") + " boosting items to counteract the gloom of underground living.");
      }

      public class ARTISTRY
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Artistic Expression", nameof (ARTISTRY));
        public static LocString DESC = (LocString) ("Majorly improve " + UI.FormatAsLink("Decor", "DECOR") + " by giving Duplicants the tools of artistic and emotional expression.");
      }

      public class CLOTHING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Textile Production", nameof (CLOTHING));
        public static LocString DESC = (LocString) ("Bring Duplicants the " + UI.FormatAsLink("Morale", "MORALE") + " boosting benefits of soft, cushy fabrics.");
      }

      public class ACOUSTICS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Sound Amplifiers", nameof (ACOUSTICS));
        public static LocString DESC = (LocString) "Precise control of the audio spectrum allows Duplicants to get funky.";
      }

      public class SPACEPOWER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Space Power", nameof (SPACEPOWER));
        public static LocString DESC = (LocString) "It's like power... in space!";
      }

      public class AMPLIFIERS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Power Amplifiers", nameof (AMPLIFIERS));
        public static LocString DESC = (LocString) ("Further increased efficacy of " + UI.FormatAsLink("Power", "POWER") + " management to prevent those wasted joules.");
      }

      public class LUXURY
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Home Luxuries", nameof (LUXURY));
        public static LocString DESC = (LocString) ("Luxury amenities for advanced " + UI.FormatAsLink("Stress", "STRESS") + " reduction.");
      }

      public class ENVIRONMENTALAPPRECIATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Environmental Appreciation", nameof (ENVIRONMENTALAPPRECIATION));
        public static LocString DESC = (LocString) ("Improve " + UI.FormatAsLink("Morale", "MORALE") + " by lazing around in " + UI.FormatAsLink("Light", "LIGHT") + " with a high Lux value.");
      }

      public class FINEART
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Fine Art", nameof (FINEART));
        public static LocString DESC = (LocString) ("Broader options for artistic " + UI.FormatAsLink("Decor", "DECOR") + " improvements.");
      }

      public class REFRACTIVEDECOR
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("High Culture", nameof (REFRACTIVEDECOR));
        public static LocString DESC = (LocString) "New methods for working with extremely high quality art materials.";
      }

      public class RENAISSANCEART
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Renaissance Art", nameof (RENAISSANCEART));
        public static LocString DESC = (LocString) "The kind of art that culture legacies are made of.";
      }

      public class GLASSFURNISHINGS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Glass Blowing", nameof (GLASSFURNISHINGS));
        public static LocString DESC = (LocString) "The decorative benefits of glass are both apparent and transparent.";
      }

      public class SCREENS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("New Media", nameof (SCREENS));
        public static LocString DESC = (LocString) "High tech displays with lots of pretty colors.";
      }

      public class ADVANCEDPOWERREGULATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Advanced Power Regulation", nameof (ADVANCEDPOWERREGULATION));
        public static LocString DESC = (LocString) ("Circuit components required for large scale " + UI.FormatAsLink("Power", "POWER") + " management.");
      }

      public class PLASTICS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Plastic Manufacturing", nameof (PLASTICS));
        public static LocString DESC = (LocString) "Stable, lightweight, durable. Plastics are useful for a wide array of applications.";
      }

      public class SUITS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Hazard Protection", nameof (SUITS));
        public static LocString DESC = (LocString) "Vital gear for surviving in extreme conditions and environments.";
      }

      public class DISTILLATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Distillation", nameof (DISTILLATION));
        public static LocString DESC = (LocString) "Distill difficult mixtures down to their most useful parts.";
      }

      public class CATALYTICS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Catalytics", nameof (CATALYTICS));
        public static LocString DESC = (LocString) "Advanced gas manipulation using unique catalysts.";
      }

      public class ADVANCEDRESEARCH
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Advanced Research", nameof (ADVANCEDRESEARCH));
        public static LocString DESC = (LocString) "The tools my colony needs to conduct more advanced, in-depth research.";
      }

      public class SPACEPROGRAM
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Space Program", nameof (SPACEPROGRAM));
        public static LocString DESC = (LocString) "The first steps in getting a Duplicant to space.";
      }

      public class CRASHPLAN
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Crash Plan", nameof (CRASHPLAN));
        public static LocString DESC = (LocString) "What goes up, must come down.";
      }

      public class DURABLELIFESUPPORT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Durable Life Support", nameof (DURABLELIFESUPPORT));
        public static LocString DESC = (LocString) "Improved devices for extended missions into space.";
      }

      public class ARTIFICIALFRIENDS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Artificial Friends", nameof (ARTIFICIALFRIENDS));
        public static LocString DESC = (LocString) "Sweeping advances in companion technology.";
      }

      public class ROBOTICTOOLS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Robotic Tools", nameof (ROBOTICTOOLS));
        public static LocString DESC = (LocString) "The goal of every great civilization is to one day make itself obsolete.";
      }

      public class LOGICCONTROL
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Smart Home", nameof (LOGICCONTROL));
        public static LocString DESC = (LocString) "Switches that grant full control of building operations within the colony.";
      }

      public class LOGICCIRCUITS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Advanced Automation", nameof (LOGICCIRCUITS));
        public static LocString DESC = (LocString) "The only limit to colony automation is my own imagination.";
      }

      public class PARALLELAUTOMATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Parallel Automation", nameof (PARALLELAUTOMATION));
        public static LocString DESC = (LocString) "Multi-wire automation at a fraction of the space.";
      }

      public class MULTIPLEXING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Multiplexing", nameof (MULTIPLEXING));
        public static LocString DESC = (LocString) "More choices for Automation signal distribution.";
      }

      public class VALVEMINIATURIZATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Valve Miniaturization", nameof (VALVEMINIATURIZATION));
        public static LocString DESC = (LocString) "Smaller, more efficient pumps for those low-throughput situations.";
      }

      public class HYDROCARBONPROPULSION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Hydrocarbon Propulsion", nameof (HYDROCARBONPROPULSION));
        public static LocString DESC = (LocString) "Low-range rocket engines with lots of smoke.";
      }

      public class BETTERHYDROCARBONPROPULSION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Improved Hydrocarbon Propulsion", nameof (BETTERHYDROCARBONPROPULSION));
        public static LocString DESC = (LocString) "Mid-range rocket engines with lots of smoke.";
      }

      public class PRETTYGOODCONDUCTORS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Low-Resistance Conductors", nameof (PRETTYGOODCONDUCTORS));
        public static LocString DESC = (LocString) ("Pure-core wires that can handle more " + UI.FormatAsLink("Electrical", "POWER") + " current without overloading.");
      }

      public class RENEWABLEENERGY
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Renewable Energy", nameof (RENEWABLEENERGY));
        public static LocString DESC = (LocString) ("Clean, sustainable " + UI.FormatAsLink("Power", "POWER") + " production that produces little to no waste.");
      }

      public class BASICREFINEMENT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Brute-Force Refinement", nameof (BASICREFINEMENT));
        public static LocString DESC = (LocString) "Low-tech refinement methods for producing clay and renewable sources of sand.";
      }

      public class REFINEDOBJECTS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Refined Renovations", nameof (REFINEDOBJECTS));
        public static LocString DESC = (LocString) ("Improve base infrastructure with new objects crafted from " + UI.FormatAsLink("Refined Metals", "REFINEDMETAL") + ".");
      }

      public class GENERICSENSORS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Generic Sensors", nameof (GENERICSENSORS));
        public static LocString DESC = (LocString) "Drive automation in a variety of new, inventive ways.";
      }

      public class DUPETRAFFICCONTROL
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Computing", nameof (DUPETRAFFICCONTROL));
        public static LocString DESC = (LocString) "Virtually extend the boundaries of Duplicant imagination.";
      }

      public class ADVANCEDSCANNERS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Sensitive Microimaging", nameof (ADVANCEDSCANNERS));
        public static LocString DESC = (LocString) "Computerized systems do the looking, so Duplicants don't have to.";
      }

      public class SMELTING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Smelting", nameof (SMELTING));
        public static LocString DESC = (LocString) "High temperatures facilitate the production of purer, special use metal resources.";
      }

      public class TRAVELTUBES
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Transit Tubes", nameof (TRAVELTUBES));
        public static LocString DESC = (LocString) "A wholly futuristic way to move Duplicants around the base.";
      }

      public class SMARTSTORAGE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Smart Storage", nameof (SMARTSTORAGE));
        public static LocString DESC = (LocString) "Completely automate the storage of solid resources.";
      }

      public class SOLIDTRANSPORT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Solid Transport", nameof (SOLIDTRANSPORT));
        public static LocString DESC = (LocString) "Free Duplicants from the drudgery of day-to-day material deliveries with new methods of automation.";
      }

      public class SOLIDMANAGEMENT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Solid Management", nameof (SOLIDMANAGEMENT));
        public static LocString DESC = (LocString) ("Make solid decisions in " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " sorting.");
      }

      public class SOLIDDISTRIBUTION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Solid Distribution", nameof (SOLIDDISTRIBUTION));
        public static LocString DESC = (LocString) ("Internal rocket hookups for " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " resources.");
      }

      public class HIGHTEMPFORGING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Superheated Forging", nameof (HIGHTEMPFORGING));
        public static LocString DESC = (LocString) "Craft entirely new materials by harnessing the most extreme temperatures.";
      }

      public class HIGHPRESSUREFORGING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pressurized Forging", nameof (HIGHPRESSUREFORGING));
        public static LocString DESC = (LocString) "High pressure diamond forging.";
      }

      public class RADIATIONPROTECTION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Radiation Protection", nameof (RADIATIONPROTECTION));
        public static LocString DESC = (LocString) "Shield Duplicants from dangerous amounts of radiation.";
      }

      public class SKYDETECTORS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Celestial Detection", nameof (SKYDETECTORS));
        public static LocString DESC = (LocString) "Turn Duplicants' eyes to the skies and discover what undiscovered wonders await out there.";
      }

      public class JETPACKS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Jetpacks", nameof (JETPACKS));
        public static LocString DESC = (LocString) "Objectively the most stylish way for Duplicants to get around.";
      }

      public class BASICROCKETRY
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Introductory Rocketry", nameof (BASICROCKETRY));
        public static LocString DESC = (LocString) "Everything required for launching the colony's very first space program.";
      }

      public class ENGINESI
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Solid Fuel Combustion", nameof (ENGINESI));
        public static LocString DESC = (LocString) "Rockets that fly further, longer.";
      }

      public class ENGINESII
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Hydrocarbon Combustion", nameof (ENGINESII));
        public static LocString DESC = (LocString) "Delve deeper into the vastness of space than ever before.";
      }

      public class ENGINESIII
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Cryofuel Combustion", nameof (ENGINESIII));
        public static LocString DESC = (LocString) "With this technology, the sky is your oyster. Go exploring!";
      }

      public class CRYOFUELPROPULSION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Cryofuel Propulsion", nameof (CRYOFUELPROPULSION));
        public static LocString DESC = (LocString) "A semi-powerful engine to propel you further into the galaxy.";
      }

      public class NUCLEARPROPULSION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Radbolt Propulsion", nameof (NUCLEARPROPULSION));
        public static LocString DESC = (LocString) "Radical technology to get you to the stars.";
      }

      public class ADVANCEDRESOURCEEXTRACTION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Advanced Resource Extraction", nameof (ADVANCEDRESOURCEEXTRACTION));
        public static LocString DESC = (LocString) "Bring back souvieners from the stars.";
      }

      public class CARGOI
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Solid Cargo", nameof (CARGOI));
        public static LocString DESC = (LocString) "Make extra use of journeys into space by mining and storing useful resources.";
      }

      public class CARGOII
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Liquid and Gas Cargo", nameof (CARGOII));
        public static LocString DESC = (LocString) "Extract precious liquids and gases from the far reaches of space, and return with them to the colony.";
      }

      public class CARGOIII
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Unique Cargo", nameof (CARGOIII));
        public static LocString DESC = (LocString) "Allow Duplicants to take their friends to see the stars... or simply bring souvenirs back from their travels.";
      }

      public class NOTIFICATIONSYSTEMS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Notification Systems", nameof (NOTIFICATIONSYSTEMS));
        public static LocString DESC = (LocString) "Get all the news you need to know about your complex colony.";
      }

      public class NUCLEARREFINEMENT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Radiation Refinement", "NUCLEAR");
        public static LocString DESC = (LocString) "Refine uranium and generate radiation.";
      }

      public class NUCLEARRESEARCH
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Materials Science Research", "ATOMIC");
        public static LocString DESC = (LocString) "Harness sub-atomic particles to study the properties of matter.";
      }

      public class ADVANCEDNUCLEARRESEARCH
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("More Materials Science Research", "ATOMIC");
        public static LocString DESC = (LocString) "Harness sub-atomic particles to study the properties of matter even more.";
      }

      public class NUCLEARSTORAGE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Radbolt Containment", "ATOMIC");
        public static LocString DESC = (LocString) "Build a quality cache of radbolts.";
      }

      public class SOLIDSPACE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Solid Control", nameof (SOLIDSPACE));
        public static LocString DESC = (LocString) ("Transport and sort " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " resources.");
      }

      public class HIGHVELOCITYTRANSPORT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("High Velocity Transport", "HIGHVELOCITY");
        public static LocString DESC = (LocString) "Hurl things through space.";
      }

      public class MONUMENTS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Monuments", nameof (MONUMENTS));
        public static LocString DESC = (LocString) "Monumental art projects.";
      }

      public class BIOENGINEERING
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Bioengineering", nameof (BIOENGINEERING));
        public static LocString DESC = (LocString) "Mutation station.";
      }

      public class SPACECOMBUSTION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Advanced Combustion", nameof (SPACECOMBUSTION));
        public static LocString DESC = (LocString) "Sweet advancements in rocket engines.";
      }

      public class HIGHVELOCITYDESTRUCTION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("High Velocity Destruction", nameof (HIGHVELOCITYDESTRUCTION));
        public static LocString DESC = (LocString) "Mine the skies.";
      }

      public class SPACEGAS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Advanced Gas Flow", nameof (SPACEGAS));
        public static LocString DESC = (LocString) (UI.FormatAsLink("Gas", "ELEMENTS_GASSES") + " engines and transportation for rockets.");
      }
    }
  }
}
