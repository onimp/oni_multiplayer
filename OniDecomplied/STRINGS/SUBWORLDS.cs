// Decompiled with JetBrains decompiler
// Type: STRINGS.SUBWORLDS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace STRINGS
{
  public class SUBWORLDS
  {
    public static class BARREN
    {
      public static LocString NAME = (LocString) "Barren";
      public static LocString DESC = (LocString) ("Initial scans of this biome yield no signs of either " + UI.FormatAsLink("plant life", "PLANTS") + " or " + UI.FormatAsLink("critters", "CREATURES") + ". It is a land devoid of " + UI.FormatAsLink("liquids", "ELEMENTS_LIQUID") + " and minuscule " + UI.FormatAsLink("gas", "ELEMENTS_GAS") + " deposits. These dry, dusty plains can be mined for building materials but there is little in the way of life sustaining resources here for a colony.");
      public static LocString UTILITY = (LocString) ("The layers of sedimentary rock are predominantly made up of " + UI.FormatAsLink("Granite", "GRANITE") + " deposits, although " + UI.FormatAsLink("Obsidian", "OBSIDIAN") + " and " + UI.FormatAsLink("Igneous Rock", "IGNEOUSROCK") + " are also present. This suggests a history of volcanic activity, though no volcanoes exist here currently.\n\nVeins of " + UI.FormatAsLink("Iron Ore", "IRON") + " deposits are evident from initial scans, as are deposits of " + UI.FormatAsLink("Coal", "CARBON") + ". Both are useful in setting up a colony's power infrastructure.\n\nThough it lacks the crucial resources necessary to sustain a colony, there is nothing inherently dangerous here to harm my Duplicants. It should be safe enough to explore.");
    }

    public static class FOREST
    {
      public static LocString NAME = (LocString) "Forest";
      public static LocString DESC = (LocString) ("Temperate and filled with unique " + UI.FormatAsLink("Plant", "PLANTS") + " life, this biome contains all the necessities for life support, although not in quantities sufficient to sustain a long term colony. Exploration into neighboring biomes should be a priority.");
      public static LocString UTILITY = (LocString) ("Small pockets of " + UI.FormatAsLink("Oxylite", "OXYROCK") + " and " + UI.FormatAsLink("Water", "WATER") + " are present in the Forest Biome, but calculations reveal they will only sustain the colony for a limited time.\n\nAnalysis shows two native plants which should be prioritized for cultivation: The " + UI.FormatAsLink("Oxyfern", "OXYFERN") + ", which releases " + UI.FormatAsLink("Oxygen", "OXYGEN") + " but requires " + UI.FormatAsLink("Water", "WATER") + "; and the " + UI.FormatAsLink("Arbor Tree", "FOREST_TREE") + " which provides lumber as a fuel source.\n\nA symbiotic relationship exists with the " + UI.FormatAsLink("Arbor Tree", "FOREST_TREE") + " and the native " + UI.FormatAsLink("Pips", "SQUIRRELSPECIES") + " which appear to be the only critter that can find the elusive " + UI.FormatAsLink("Arbor Acorns", "PLANTS") + ".\n\nThis biome is really quite beautiful. I've noted that " + UI.FormatAsLink("Shine Bugs", "LIGHTBUGSPECIES") + " and " + UI.FormatAsLink("Mirth Leaf", "LEAFYPLANT") + " both evoke feelings of serenity in my Duplicants.");
    }

    public static class FROZEN
    {
      public static LocString NAME = (LocString) "Tundra";
      public static LocString DESC = (LocString) ("The sub-zero temperatures of the Tundra Biome provide rare frozen deposits of " + UI.FormatAsLink("Ice", "ICE") + " and " + UI.FormatAsLink("Snow", "SNOW") + ", necessary for a colony's " + UI.FormatAsLink("Heat", "HEAT") + " regulation needs.");
      public static LocString UTILITY = (LocString) ("Far from devoid of life, this biome contains some much needed plant life, ripe for cultivation. " + UI.FormatAsLink("Sleet Wheat", "COLDWHEAT") + " provides a nutrient rich ingredient for creating complex foods, though the plants do require sub-zero temperatures to thrive.\n\nFortunately " + UI.FormatAsLink("Wheezewort", "COLDBREATHER") + " can been planted on farms to lower surrounding temperatures.\n\nCrucially, small deposits of " + UI.FormatAsLink("Wolframite", "WOLFRAMITE") + " have been detected here. This is an extremely rare metal that should be preserved for " + UI.FormatAsLink("Tungsten", "TUNGSTEN") + "production.\n\nThough my Duplicants appear more than happy to work in the Tundra Biome for short periods of time, I will need to provide proper " + UI.FormatAsLink("equipment", "EQUIPMENT") + " for them to avoid adverse affects to their well-being if they are working here for longer periods.");
    }

    public static class JUNGLE
    {
      public static LocString NAME = (LocString) "Jungle";
      public static LocString DESC = (LocString) ("Initial investigations of the Jungle Biome reveal an ecosystem filled with unique flora but centered around " + UI.FormatAsLink("Chlorine", "CHLORINE") + " and " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + " gas, toxic to Duplicants. When exploring here, it is worth setting up a good  system.");
      public static LocString UTILITY = (LocString) ("The " + UI.FormatAsLink("Drecko", "DRECKOSPECIES") + " is a relatively benign critter which can be domesticated to aid in textile and food production.\n\nThe " + UI.FormatAsLink("Morb", "GLOMSPECIES") + " only function is to produce " + UI.FormatAsLink("Polluted Oxygen", "POLLUTEDOXYGEN") + " which may be useful when establishing sustainable production loops with other critters.\n\n" + UI.FormatAsLink("Balm Lilies", "SWAMPLILY") + " would be useful to cultivate for the production of critical medical materials.\n\n" + UI.FormatAsLink("Pincha Pepperplants", "SPICE_VINE") + " greatly improve the nutritional and gratification of a colony's food supply. Because of their unique relationship with gravity, the plants must be orientated upside-down for proper growing. This can be accomplished by using a " + UI.FormatAsLink("Farm Tile", "FARMTILE") + ".\n\nGiven the toxic gases I will have to build proper " + UI.FormatAsLink("Ventilation", "BUILDCATEGORYHVAC") + " system to both protect my Duplicants and provide the optimum environments for the native plants and critters.");
    }

    public static class MAGMA
    {
      public static LocString NAME = (LocString) "Magma";
      public static LocString DESC = (LocString) ("Temperatures in the Magma Biome can reach upwards of 1526 degrees, making it a reliable source of extreme heat that can be exploited for the purposes of producing " + UI.FormatAsLink("Power", "POWER") + " and fuel.");
      public static LocString UTILITY = (LocString) (UI.FormatAsLink("Magma", nameof (MAGMA)) + " is source of extreme " + UI.FormatAsLink("Heat", "HEAT") + " which can be used to transform " + UI.FormatAsLink("Water", "WATER") + " in to " + UI.FormatAsLink("Steam", "STEAM") + " or " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " into " + UI.FormatAsLink("Petroleum", "PETROLEUM") + ". In order to prevent the extreme temperatures of this biome invading other parts of my base, suitable insulation must be constructed using materials with high melting points like  " + UI.FormatAsLink("Ceramic", "CERAMIC") + " or " + UI.FormatAsLink("Obsidian", "OBSIDIAN") + ".\n\nThough " + UI.FormatAsLink("Exosuits", "EXOSUIT") + " will provide some protection for my Duplicants, there is still a danger they will overheat if spending an extended amount of time in this Biome. I should ensure that suitable medical facilities have been constructed nearby to take care of any medical emergencies.");
    }

    public static class MARSH
    {
      public static LocString NAME = (LocString) "Marsh";
      public static LocString DESC = (LocString) (UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " dominates the atmosphere of the Marsh Biome as it escapes from the " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + " this biome is known for.");
      public static LocString UTILITY = (LocString) ("Marsh Biomes contain large amounts of " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " which can be converted into " + UI.FormatAsLink("Algae", "ALGAE") + " and provide a valuable resource for growing " + UI.FormatAsLink("Dusk Caps", "MUSHROOMPLANT") + " as well as feeding to " + UI.FormatAsLink("Pacus", "PACUSPECIES") + " for producing some higher tier " + UI.FormatAsLink("food", "FOOD") + ".\n\nBecause of the high degree of probability that this biome will infect my Duplicants with " + UI.FormatAsLink("Slimelung", "SLIMELUNG") + ", it may be prudent to limit access to this area to essential activities only until my Duplicants are able to set up suitable protection.");
    }

    public static class METALLIC
    {
      public static LocString NAME = (LocString) "Metallic";
      public static LocString DESC = (LocString) "A plethora of metals pervade the Metallic Biome making it the go-to destination for a colony ramping up production for technological advancement.";
      public static LocString UTILITY = (LocString) (UI.FormatAsLink("Gold Amalgam", "GOLDAMALGAM") + " " + UI.FormatAsLink("Aluminum Ore", "ALUMINUMORE") + " and " + UI.FormatAsLink("Cobalt Ore", "COBALTITE") + " are in abundant supply throughout this entire biome. Refining these metals with a " + UI.FormatAsLink("Metal Refinery", "METALREFINERY") + " will make them available for building advanced technologies.\n\nThough " + UI.FormatAsLink("Chlorine", "CHLORINEGAS") + " and " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + " are the prevailing gasses in this biome, " + UI.FormatAsLink("Oxylite", "OXYROCK") + " exists in rock form and can provide " + UI.FormatAsLink("Oxygen", "OXYGEN") + " for Duplicants once they uncover it.\n\n" + UI.FormatAsLink("Dirt", "DIRT") + ", " + UI.FormatAsLink("Coal", "CARBON") + " and " + UI.FormatAsLink("Igneous Rock", "IGNEOUSROCK") + " round out the rest of this biome, making it a great deposit of resources for a budding industrialized colony.");
    }

    public static class OCEAN
    {
      public static LocString NAME = (LocString) "Ocean";
      public static LocString DESC = (LocString) (UI.FormatAsLink("Sand", "SAND") + ", " + UI.FormatAsLink("Salt", "SALT") + " and " + UI.FormatAsLink("Bleachstone", "BLEACHSTONE") + " abound in this unique " + UI.FormatAsLink("briny", "BRINE") + " biome.");
      public static LocString UTILITY = (LocString) (UI.FormatAsLink("Pokeshell", "CRABSPECIES") + " molt is an excellent source of " + UI.FormatAsLink("Lime", "LIME") + " but much care must be taken with domesticating this species as it can get aggressive around its eggs.\n\nHarvesting " + UI.FormatAsLink("Waterweed", "SEALETTUCE") + " provides " + UI.FormatAsLink("Lettuce", "LETTUCE") + " for many higher-tier " + UI.FormatAsLink("foods", "FOOD") + ".\n\nAny water will need to be filtered through a " + UI.FormatAsLink("Desalinator", "DESALINATOR") + " to remove the " + UI.FormatAsLink("Salt", "SALT") + " from " + UI.FormatAsLink("Water", "WATER") + " in order to be useful for my Duplicants. Luckily " + UI.FormatAsLink("Table Salt", "SALT") + " can be produced using a " + UI.FormatAsLink("Rock Crusher", "ROCKCRUSHER") + " which, when combined with a " + UI.FormatAsLink("Mess Table", "DININGTABLE") + ", gives my Duplicants a Morale boost.");
    }

    public static class OIL
    {
      public static LocString NAME = (LocString) "Oily";
      public static LocString DESC = (LocString) ("Viscous pools of liquid " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " pepper the " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " rich environment of the Oil Biome.");
      public static LocString UTILITY = (LocString) ("Though " + UI.FormatAsLink("Oxygen", "OXYGEN") + " is more scarce in this biome, it's the perfect place to cultivate flora and fauna that thrive in CO2, such as domesticating " + UI.FormatAsLink("Slicksters", "OILFLOATERSPECIES") + " to create a renewable source of oil.\n\n" + UI.FormatAsLink("Diamond", "DIAMOND") + " deposits can occasionally be found in the Oily biome, which will require a Duplicant with the " + UI.FormatAsLink("Super-Duperhard Digging", "MINING3") + " skill to explore properly.\n\n" + UI.FormatAsLink("Sporechids", "EVIL_FLOWER") + " are beautiful, but should only be approached if a Duplicant is properly " + UI.FormatAsLink("equipped", "EQUIPMENT") + ".\n\nWhile the dangers of this biome should not be underestimated, the benefits " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " and " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " will bring to my colony far outweigh the risks.");
    }

    public static class RADIOACTIVE
    {
      public static LocString NAME = (LocString) "Radioactive";
      public static LocString DESC = (LocString) "A highly volatile environment containing a highly useful resource, this biome is invaluable when venturing into Nuclear technologies.";
      public static LocString UTILITY = (LocString) (UI.FormatAsLink("Lead Suits", "LEAD_SUIT") + " are imperative if my Duplicants are going to start exploring this biome as " + UI.FormatAsLink("Radioactive Contaminants", "RADIATIONSICKNESS") + " is a constant danger here.\n\n" + UI.FormatAsLink("Beetas", "BEE") + " pose a double threat as they are both highly radioactive and very aggressive. If they can be domesticated, however, " + UI.FormatAsLink("Beeta Hives", "BEEHIVE") + ", provide a great service turning " + UI.FormatAsLink("Uranium", "URANIUMORE") + " into " + UI.FormatAsLink("Enriched Uranium", "ENRICHEDURANIUM") + ".\n\nWhile the Radioactive Biome, and the Beetas contained within it, should be avoided at all costs if my Duplicants do not have the correct protection, my colony will need to trek into this dangerous biome if we are going to build any higher tier nuclear technologies.");
    }

    public static class RUST
    {
      public static LocString NAME = (LocString) "Rust";
      public static LocString DESC = (LocString) "The orange-brown oasis of the Rust Biome is home to many unusual flora and fauna. It contains the resources for several intermediate technologies.";
      public static LocString UTILITY = (LocString) ("When combined with the " + UI.FormatAsLink("Rust Deoxidizer", "RUSTDEOXIDIZER") + ", " + UI.FormatAsLink("Rust", nameof (RUST)) + " can produce many of a colony's basic needs.\n\nThe " + UI.FormatAsLink("Squeaky Puft", "PUFTBLEACHSTONE") + ", a frequent resident of the Rust biome, are a renewable source of " + UI.FormatAsLink("Bleachstone", "BLEACHSTONE") + " and can be domesticated for such purposes.\n\n" + UI.FormatAsLink("Dreckos", "DRECKOS") + " can also sometimes be found in these biomes, which are a great source of " + UI.FormatAsLink("Phosphorite", "PHOSPHORITE") + " and fibre for making " + UI.FormatAsLink("Textile Production", "CLOTHING") + "\n\nTwo plants found in this biome: the " + UI.FormatAsLink("Nosh Bean", "BEANPLANTSEED") + " and the " + UI.FormatAsLink("Dasha Saltvine", "SALTPLANT") + " can both produce food that will add significant Morale value for my Duplicants.");
    }

    public static class SANDSTONE
    {
      public static LocString NAME = (LocString) "Sandstone";
      public static LocString DESC = (LocString) "The Sandstone Biome is a temperate oasis with few inherent dangers. It's the perfect spot to get your colony up and running.";
      public static LocString UTILITY = (LocString) (UI.FormatAsLink("Oxylite", "OXYROCK") + " and " + UI.FormatAsLink("Buried Muckroot", "BASICFORAGEPLANTPLANTED") + " are in sufficient supply to sustain your colony while you gather more resources.\n\n" + UI.FormatAsLink("Dirt", "DIRT") + ", " + UI.FormatAsLink("Algae", "ALGAE") + ", " + UI.FormatAsLink("Copper", "COPPER") + " and, of course, " + UI.FormatAsLink("Sandstone", nameof (SANDSTONE)) + " provide all the materials to get basic colony essentials built.\n\nRandom " + UI.FormatAsLink("Shine Bugs", "LIGHTBUGSPECIES") + " provide Morale boosts for my Duplicants but are not a reliable light source.\n\n" + UI.FormatAsLink("Hatch", "HATCHSPECIES") + " can be domesticated for food or " + UI.FormatAsLink("Coal", "CARBON") + ", which will be useful if using a " + UI.FormatAsLink("Coal Generator", "GENERATOR") + " for power.\n\nAll-in-all this biome is the perfect starting spot for my colony to establish a base full of essentials, from which they can then venture out and explore.");
    }

    public static class WASTELAND
    {
      public static LocString NAME = (LocString) "Wasteland";
      public static LocString DESC = (LocString) "While the Wasteland Biome does not look particularly interesting, a pragmatic colony can take advantage of its selection of construction resources.";
      public static LocString UTILITY = (LocString) ("The prevalance of " + UI.FormatAsLink("Copper", "COPPER") + ", " + UI.FormatAsLink("Sandstone", "SANDSTONE") + ", " + UI.FormatAsLink("Igneous Rock", "IGNEOUSROCK") + " and its " + UI.FormatAsLink("Iron", "IRON") + " rich counterpart " + UI.FormatAsLink("Mafic Rock", "MAFICROCK") + ", make this a fruitful biome to explore for construction material. " + UI.FormatAsLink("Sand", "SAND") + " is also in abundance here which is useful as a filtering material.\n\nWhile the wildlife is not in abundance in the Wasteland Biome, the " + UI.FormatAsLink("Sweetle", "DIVERGENTBEETLE") + " and " + UI.FormatAsLink("Grubgrub", "DIVERGENTWORM") + " make interesting creatures to domesticate as they co-exist with the  " + UI.FormatAsLink("Grubfruit Plants", "WORMPLANT") + " to produce a much higher quality food than the " + UI.FormatAsLink("Spindly Grubfruit Plant", "WORMPLANT") + " found in the wild. Additionally, the " + UI.FormatAsLink("Sulfur", "SULFUR") + " found here works both as food for the GrubGrubs and fertilizer for the Grubfruit Plants.\n\nThe abundance of " + UI.FormatAsLink("Oxygen", "OXYGEN") + " found in the Wasteland Biome makes for a low-risk area to send my Duplicants into to collect useful resources to continue with their construction projects.");
    }

    public static class SPACE
    {
      public static LocString NAME = (LocString) "Space";
      public static LocString DESC = (LocString) "The Space Biome is located on the scenic surface of an asteroid. Watch for dazzling meteorites to shower elements down from the sky.";
      public static LocString UTILITY = (LocString) ("Setting up " + UI.FormatAsLink("Solar Panels", "SOLARPANELS") + " on the surface will provide a source of renewable energy. However, much care must be taken to ensure " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " or " + UI.FormatAsLink("Gases", "ELEMENTS_GAS") + " are not sucked out into the " + UI.FormatAsLink("Vacuum", "VACUUM") + " of space. " + UI.FormatAsLink("Shove Voles", "MOLE") + " are native to this biome, and need to be wrangled or contained or they will infest the colony.");
    }

    public static class SWAMP
    {
      public static LocString NAME = (LocString) "Swampy";
      public static LocString DESC = (LocString) ("With its abundence of " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " and lack of clean " + UI.PRE_KEYWORD + "Water" + UI.PST_KEYWORD + " the Swampy Biome presents some challenges for a budding colony. But, with a little hard work, it can also turn into a great starting biome with some valuable resources.");
      public static LocString UTILITY = (LocString) (UI.FormatAsLink("Swamp Chard", "SWAMPFORAGEPLANTPLANTED") + " can provide adequate nutrients for my Duplicants while they establish farms, but it cannot be planted or propogated. " + UI.FormatAsLink("Bog Buckets", "SWAMPHARVESTPLANT") + ", however, provide a sweet source of nutrients that are fairly easy to farm using " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " and require no extra light to grow.\n\nFortunately " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " is abundant in this biome and will require a " + UI.FormatAsLink("Liquid Filter", "LIQUIDFILTER") + " to turn into something my Duplicants can drink. Additionally my Duplicants can use a " + UI.FormatAsLink("Sludge Press", "SLUDGEPRESS") + " to filter clean water from " + UI.FormatAsLink("Mud", "MUD") + " (NOTE: " + UI.FormatAsLink("Polluted Mud", "TOXICMUD") + ", however, does not make clean water).\n\nMeanwhile, rudimentary power can be gained from the energy producing " + UI.FormatAsLink("Plug Slugs", "STATERPILLAR") + " that inhabit this biome.\n\nShiny " + UI.FormatAsLink("Cobalt Ore", "COBALTITE") + " can be found here, providing a adequate source of metal.\n\nWhile much of this biome is dangerous for a new colony with some deliberate research and careful planning my Duplicants can not only survive but thrive here.");
    }

    public static class NIOBIUM
    {
      public static LocString NAME = (LocString) "Niobium";
      public static LocString DESC = (LocString) ("The Niobium Biome features only two resources yet, because " + UI.FormatAsLink("Niobium", nameof (NIOBIUM)) + " is an extremely rare and valuable element, it is worth making a special visit.");
      public static LocString UTILITY = (LocString) ("By itself " + UI.FormatAsLink("Niobium", nameof (NIOBIUM)) + " is not a particularly useful resource, but if processed through a " + UI.FormatAsLink("Metal Refinery", "METALREFINERY") + " it produces the extremely thermal conductive " + UI.FormatAsLink("Thermium", "TEMPCONDUCTORSOLID") + " which goes a long way in solving many extreme temperature issues in a colony.\n\nThe edges of this biome are filled with " + UI.FormatAsLink("Obsidian", "OBSIDIAN") + " so a Duplicant with the " + UI.FormatAsLink("Super-Duperhard Digging", "MINING3") + " skill will be required before my colony can explore here.");
    }

    public static class AQUATIC
    {
      public static LocString NAME = (LocString) "Aquatic";
      public static LocString DESC = (LocString) ("The Aquatic Biome is flush with a huge deposit of precious " + UI.FormatAsLink("Water", "WATER") + ".");
      public static LocString UTILITY = (LocString) ("Initially there is very little solid ground in this biome to establish a temporary base, but once a transportation network can be established to send the " + UI.FormatAsLink("Water", "WATER") + " of the Aquatic Biome to the rest of the colony, the other elements will be easier to reach.\n\n" + UI.FormatAsLink("Sandstone", "SANDSTONE") + ", " + UI.FormatAsLink("Mafic Rock", "MAFICROCK") + ", " + UI.FormatAsLink("Sand", "SAND") + " and " + UI.FormatAsLink("Sedimentary Rock", "SEDIMENTARYROCK") + " provide readily available construction materials for setting up elementary infrastructure. The presence of " + UI.FormatAsLink("Oxylite", "OXYROCK") + " provides invaluable " + UI.FormatAsLink("Oxygen", "OXYGEN") + " which, through careful planning, should be able to sustain any Duplicants working in the area for a limited amount of time.");
    }

    public static class MOO
    {
      public static LocString NAME = (LocString) "Moo";
      public static LocString DESC = (LocString) ("The Moo Biome is the natural habitat of the charismatic " + UI.FormatAsLink("Gassy Moo", nameof (MOO)) + ", a great source of " + UI.FormatAsLink("Natural Gas", "METHANE") + ".");
      public static LocString UTILITY = (LocString) ("In addition to " + UI.FormatAsLink("Natural Gas", "METHANE") + ", the highly toxic " + UI.FormatAsLink("Chlorine", "CHLORINEGAS") + " is also present in gas form. In fact, Chlorine is present here in " + UI.FormatAsLink("gas", "ELEMENTS_GAS") + ", " + UI.FormatAsLink("liquid", "ELEMENTS_LIQUID") + ", and " + UI.FormatAsLink("solid", "ELEMENTS_SOLID") + " states, largely due to the presence of " + UI.FormatAsLink("Bleach Stone", "BLEACHSTONE") + ".\n\n" + UI.FormatAsLink("Igneous Rock", "IGNEOUSROCK") + " and its denser form, " + UI.FormatAsLink("Granite", "GRANITE") + ", provide some useful construction materials, but the real star of this biome are the " + UI.FormatAsLink("Gassy Moos", nameof (MOO)) + " who consume " + UI.FormatAsLink("Gas Grass", "GASGRASS") + " and excrete " + UI.FormatAsLink("Natural Gas", "METHANE") + ". While Gassy Moos cannot be bred domestically, Gassy Mooteors regularly fall from space onto this biome, making it the best way to find a reliable source of these elusive creatures.\n\nWith no breathable " + UI.FormatAsLink("Oxygen", "OXYGEN") + " in this biome my Duplicants will need to be properly prepared before they venture too far into its depths.");
    }

    public static class REGOLITH
    {
      public static LocString NAME = (LocString) "Regolith";
      public static LocString DESC = (LocString) ("The Regolith Biome contains a bounty of " + UI.FormatAsLink("Regolith", nameof (REGOLITH)) + " which is very useful as a material for filtration.");
      public static LocString UTILITY = (LocString) ("Lingering within the layers of Regolith are the pernicious " + UI.FormatAsLink("Shove Voles", "MOLESPECIES") + " which eat valuable resources and excrete them at half the original mass.\n\nFortunately these pests can be wrangled up and used as a good food source for my Duplicants. However, extra care must be taken to contain these critters in pens made from either double thick walls or from " + UI.FormatAsLink("Refined Metal", "REFINEDMETAL") + " since they are capable of burrowing through most other materials.");
    }
  }
}
