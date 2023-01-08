// Decompiled with JetBrains decompiler
// Type: STRINGS.CREATURES
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace STRINGS
{
  public class CREATURES
  {
    public static LocString BAGGED_NAME_FMT = (LocString) "Bagged {0}";
    public static LocString BAGGED_DESC_FMT = (LocString) "This {0} has been captured and is now safe to relocate.";

    public class FAMILY
    {
      public static LocString HATCH = (LocString) UI.FormatAsLink("Hatch", "HATCHSPECIES");
      public static LocString LIGHTBUG = (LocString) UI.FormatAsLink("Shine Bug", "LIGHTBUGSPECIES");
      public static LocString OILFLOATER = (LocString) UI.FormatAsLink("Slickster", "OILFLOATERSPECIES");
      public static LocString DRECKO = (LocString) UI.FormatAsLink("Drecko", "DRECKOSPECIES");
      public static LocString GLOM = (LocString) UI.FormatAsLink("Morb", "GLOMSPECIES");
      public static LocString PUFT = (LocString) UI.FormatAsLink("Puft", "PUFTSPECIES");
      public static LocString PACU = (LocString) UI.FormatAsLink("Pacu", "PACUSPECIES");
      public static LocString MOO = (LocString) UI.FormatAsLink("Moo", "MOOSPECIES");
      public static LocString MOLE = (LocString) UI.FormatAsLink("Shove Vole", "MOLESPECIES");
      public static LocString SQUIRREL = (LocString) UI.FormatAsLink("Pip", "SQUIRRELSPECIES");
      public static LocString CRAB = (LocString) UI.FormatAsLink("Pokeshell", "CRABSPECIES");
      public static LocString STATERPILLAR = (LocString) UI.FormatAsLink("Plug Slug", "STATERPILLARSPECIES");
      public static LocString DIVERGENTSPECIES = (LocString) UI.FormatAsLink("Divergent", nameof (DIVERGENTSPECIES));
      public static LocString SWEEPBOT = (LocString) UI.FormatAsLink("Sweepies", nameof (SWEEPBOT));
      public static LocString SCOUTROVER = (LocString) UI.FormatAsLink("Rover", nameof (SCOUTROVER));
    }

    public class FAMILY_PLURAL
    {
      public static LocString HATCHSPECIES = (LocString) UI.FormatAsLink("Hatches", nameof (HATCHSPECIES));
      public static LocString LIGHTBUGSPECIES = (LocString) UI.FormatAsLink("Shine Bugs", nameof (LIGHTBUGSPECIES));
      public static LocString OILFLOATERSPECIES = (LocString) UI.FormatAsLink("Slicksters", nameof (OILFLOATERSPECIES));
      public static LocString DRECKOSPECIES = (LocString) UI.FormatAsLink("Dreckos", nameof (DRECKOSPECIES));
      public static LocString GLOMSPECIES = (LocString) UI.FormatAsLink("Morbs", nameof (GLOMSPECIES));
      public static LocString PUFTSPECIES = (LocString) UI.FormatAsLink("Pufts", nameof (PUFTSPECIES));
      public static LocString PACUSPECIES = (LocString) UI.FormatAsLink("Pacus", nameof (PACUSPECIES));
      public static LocString MOOSPECIES = (LocString) UI.FormatAsLink("Moos", nameof (MOOSPECIES));
      public static LocString MOLESPECIES = (LocString) UI.FormatAsLink("Shove Voles", nameof (MOLESPECIES));
      public static LocString CRABSPECIES = (LocString) UI.FormatAsLink("Pokeshells", nameof (CRABSPECIES));
      public static LocString SQUIRRELSPECIES = (LocString) UI.FormatAsLink("Pips", nameof (SQUIRRELSPECIES));
      public static LocString STATERPILLARSPECIES = (LocString) UI.FormatAsLink("Plug Slugs", nameof (STATERPILLARSPECIES));
      public static LocString BEETASPECIES = (LocString) UI.FormatAsLink("Beetas", nameof (BEETASPECIES));
      public static LocString DIVERGENTSPECIES = (LocString) UI.FormatAsLink("Divergents", nameof (DIVERGENTSPECIES));
      public static LocString SWEEPBOT = (LocString) UI.FormatAsLink("Sweepies", nameof (SWEEPBOT));
      public static LocString SCOUTROVER = (LocString) UI.FormatAsLink("Rovers", nameof (SCOUTROVER));
    }

    public class PLANT_MUTATIONS
    {
      public static LocString PLANT_NAME_FMT = (LocString) "{PlantName} ({MutationList})";
      public static LocString UNIDENTIFIED = (LocString) "Unidentified Subspecies";
      public static LocString UNIDENTIFIED_DESC = (LocString) ("This seed must be identified at the " + (string) BUILDINGS.PREFABS.GENETICANALYSISSTATION.NAME + " before it can be planted.");
      public static LocString BONUS_CROP_FMT = (LocString) "Bonus Crop: +{Amount} {Crop}";

      public class NONE
      {
        public static LocString NAME = (LocString) "Original";
      }

      public class MODERATELYLOOSE
      {
        public static LocString NAME = (LocString) "Easygoing";
        public static LocString DESCRIPTION = (LocString) "Plants with this mutation are easier to take care of, but don't yield as much produce.";
      }

      public class MODERATELYTIGHT
      {
        public static LocString NAME = (LocString) "Specialized";
        public static LocString DESCRIPTION = (LocString) "Plants with this mutation are pickier about their conditions but yield more produce.";
      }

      public class EXTREMELYTIGHT
      {
        public static LocString NAME = (LocString) "Superspecialized";
        public static LocString DESCRIPTION = (LocString) "Plants with this mutation are very difficult to keep alive, but produce a bounty.";
      }

      public class BONUSLICE
      {
        public static LocString NAME = (LocString) "Licey";
        public static LocString DESCRIPTION = (LocString) "Something about this mutation causes Meal Lice to pupate on this plant.";
      }

      public class SUNNYSPEED
      {
        public static LocString NAME = (LocString) "Leafy";
        public static LocString DESCRIPTION = (LocString) "This mutation provides the plant with sun-collecting leaves, allowing faster growth.";
      }

      public class SLOWBURN
      {
        public static LocString NAME = (LocString) "Wildish";
        public static LocString DESCRIPTION = (LocString) "These plants grow almost as slow as their wild cousins, but also consume almost no fertilizer.";
      }

      public class BLOOMS
      {
        public static LocString NAME = (LocString) "Blooming";
        public static LocString DESCRIPTION = (LocString) "Vestigial flowers increase the beauty of this plant. Don't inhale the pollen, though!";
      }

      public class LOADEDWITHFRUIT
      {
        public static LocString NAME = (LocString) "Bountiful";
        public static LocString DESCRIPTION = (LocString) "This mutation produces lots of extra produce, though it also takes a long time to pick it all!";
      }

      public class ROTTENHEAPS
      {
        public static LocString NAME = (LocString) "Exuberant";
        public static LocString DESCRIPTION = (LocString) "Plants with this mutation grow extremely quickly, though the produce they make is sometimes questionable.";
      }

      public class HEAVYFRUIT
      {
        public static LocString NAME = (LocString) "Juicy Fruits";
        public static LocString DESCRIPTION = (LocString) "Extra water in these plump mutant veggies causes them to fall right off the plant! There's no extra nutritional value, though...";
      }
    }

    public class SPECIES
    {
      public class CRAB
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pokeshell", "Crab");
        public static LocString DESC = (LocString) ("Pokeshells are nonhostile critters that eat " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + " and " + UI.FormatAsLink("Rot Piles", "COMPOST") + ".\n\nThe shells they leave behind after molting can be crushed into " + UI.FormatAsLink("Lime", "LIME") + ".");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Pinch Roe", "Crab");

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Pokeshell Spawn", nameof (CRAB));
          public static LocString DESC = (LocString) ("A snippy little Pokeshell Spawn.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Pokeshell", nameof (CRAB)) + ".");
        }

        public class VARIANT_WOOD
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Oakshell", "CRABWOOD");
          public static LocString DESC = (LocString) ("Oakshells are nonhostile critters that eat " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + ", " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " and " + UI.FormatAsLink("Rot Piles", "COMPOST") + ".\n\nThe shells they leave behind after molting can be crushed into " + UI.FormatAsLink("Lumber", "WOOD") + ".\n\nOakshells thrive in " + UI.FormatAsLink("Ethanol", "ETHANOL") + ".");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Oak Pinch Roe", "CRABWOOD");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Oakshell Spawn", "CRABWOOD");
            public static LocString DESC = (LocString) ("A knotty little Oakshell Spawn.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Oakshell", "CRABWOOD") + ".");
          }
        }

        public class VARIANT_FRESH_WATER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Sanishell", "CRABFRESHWATER");
          public static LocString DESC = (LocString) ("Sanishells are nonhostile critters that thrive in  " + UI.FormatAsLink("Water", "WATER") + " and eliminate " + UI.FormatAsLink("Germs", "DISEASE") + " from any liquid it inhabits.\n\nThey eat " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + ", " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " and " + UI.FormatAsLink("Rot Piles", "COMPOST") + ".");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Sani Pinch Roe", "CRABFRESHWATER");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Sanishell Spawn", "CRABFRESHWATER");
            public static LocString DESC = (LocString) ("A picky little Sanishell Spawn.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Sanishell", "CRABFRESHWATER") + ".");
          }
        }
      }

      public class BEE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Beeta", nameof (BEE));
        public static LocString DESC = (LocString) ("Beetas are hostile critters that thrive in " + UI.FormatAsLink("Radioactive", "RADIATION") + " environments.\n\nThey commonly gather " + UI.FormatAsLink("Uranium", "URANIUMORE") + " for their " + UI.FormatAsLink("Beeta Hives", "BEEHIVE") + " to produce " + UI.FormatAsLink("Enriched Uranium", "ENRICHEDURANIUM") + ".");

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Beetiny", nameof (BEE));
          public static LocString DESC = (LocString) ("A harmless little Beetiny.\n\nIn time, it will mature into a vicious adult " + UI.FormatAsLink("Beeta", nameof (BEE)) + ".");
        }
      }

      public class CHLORINEGEYSER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Chlorine Geyser", "GeyserGeneric_CHLORINE_GAS");
        public static LocString DESC = (LocString) ("A highly pressurized geyser that periodically erupts with " + UI.FormatAsLink("Chlorine", "CHLORINEGAS") + ".");
      }

      public class PACU
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pacu", nameof (PACU));
        public static LocString DESC = (LocString) ("Pacus are aquatic creatures that can live in any liquid, such as " + UI.FormatAsLink("Water", "WATER") + " or " + UI.FormatAsLink("Contaminated Water", "DIRTYWATER") + ".\n\nEvery organism in the known universe finds the Pacu extremely delicious.");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Fry Egg", nameof (PACU));

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Pacu Fry", nameof (PACU));
          public static LocString DESC = (LocString) ("A wriggly little Pacu Fry.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Pacu", nameof (PACU)) + ".");
        }

        public class VARIANT_TROPICAL
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Tropical Pacu", "PACUTROPICAL");
          public static LocString DESC = (LocString) "Every organism in the known universe finds the Pacu extremely delicious.";
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Tropical Fry Egg", "PACUTROPICAL");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Tropical Fry", "PACUTROPICAL");
            public static LocString DESC = (LocString) ("A wriggly little Tropical Fry.\n\nIn time it will mature into an adult Pacu morph, the " + UI.FormatAsLink("Tropical Pacu", "PACUTROPICAL") + ".");
          }
        }

        public class VARIANT_CLEANER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Gulp Fish", "PACUCLEANER");
          public static LocString DESC = (LocString) "Every organism in the known universe finds the Pacu extremely delicious.";
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Gulp Fry Egg", "PACUCLEANER");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Gulp Fry", "PACUCLEANER");
            public static LocString DESC = (LocString) ("A wriggly little Gulp Fry.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Gulp Fish", "PACUCLEANER") + ".");
          }
        }
      }

      public class GLOM
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Morb", nameof (GLOM));
        public static LocString DESC = (LocString) ("Morbs are attracted to unhygienic conditions and frequently excrete bursts of " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + ".");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Morb Pod", "MORB");
      }

      public class HATCH
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Hatch", nameof (HATCH));
        public static LocString DESC = (LocString) ("Hatches excrete solid " + UI.FormatAsLink("Coal", "CARBON") + " as waste and may be uncovered by digging up Buried Objects.");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Hatchling Egg", nameof (HATCH));

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Hatchling", nameof (HATCH));
          public static LocString DESC = (LocString) ("An innocent little Hatchling.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Hatch", nameof (HATCH)) + ".");
        }

        public class VARIANT_HARD
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Stone Hatch", "HATCHHARD");
          public static LocString DESC = (LocString) ("Stone Hatches excrete solid " + UI.FormatAsLink("Coal", "CARBON") + " as waste and enjoy burrowing into the ground.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Stone Hatchling Egg", "HATCHHARD");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Stone Hatchling", "HATCHHARD");
            public static LocString DESC = (LocString) ("A doofy little Stone Hatchling.\n\nIt matures into an adult Hatch morph, the " + UI.FormatAsLink("Stone Hatch", "HATCHHARD") + ", which loves nibbling on various rocks and metals.");
          }
        }

        public class VARIANT_VEGGIE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Sage Hatch", "HATCHVEGGIE");
          public static LocString DESC = (LocString) ("Sage Hatches excrete solid " + UI.FormatAsLink("Coal", "CARBON") + " as waste and enjoy burrowing into the ground.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Sage Hatchling Egg", "HATCHVEGGIE");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Sage Hatchling", "HATCHVEGGIE");
            public static LocString DESC = (LocString) ("A doofy little Sage Hatchling.\n\nIt matures into an adult Hatch morph, the " + UI.FormatAsLink("Sage Hatch", "HATCHVEGGIE") + ", which loves nibbling on organic materials.");
          }
        }

        public class VARIANT_METAL
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Smooth Hatch", "HATCHMETAL");
          public static LocString DESC = (LocString) ("Smooth Hatches enjoy burrowing into the ground and excrete " + UI.FormatAsLink("Refined Metal", "REFINEDMETAL") + " when fed " + UI.FormatAsLink("Metal Ore", "RAWMETAL") + ".");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Smooth Hatchling Egg", "HATCHMETAL");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Smooth Hatchling", "HATCHMETAL");
            public static LocString DESC = (LocString) ("A doofy little Smooth Hatchling.\n\nIt matures into an adult Hatch morph, the " + UI.FormatAsLink("Smooth Hatch", "HATCHMETAL") + ", which loves nibbling on different types of metals.");
          }
        }
      }

      public class STATERPILLAR
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Plug Slug", nameof (STATERPILLAR));
        public static LocString DESC = (LocString) ("Plug Slugs are dynamic creatures that generate electrical " + UI.FormatAsLink("Power", "POWER") + " during the night.\n\nTheir power can be harnessed by leaving an exposed wire near areas where they like to sleep.");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Slug Egg", nameof (STATERPILLAR));

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Plug Sluglet", nameof (STATERPILLAR));
          public static LocString DESC = (LocString) ("A chubby little Plug Sluglet.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Plug Slug", nameof (STATERPILLAR)) + ".");
        }

        public class VARIANT_GAS
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Smog Slug", nameof (STATERPILLAR));
          public static LocString DESC = (LocString) ("Smog Slugs are porous creatures that draw in unbreathable " + UI.FormatAsLink("Gases", "ELEMENTS_GAS") + " during the day.\n\nAt night, they sleep near exposed " + UI.FormatAsLink("Gas Pipes,", "GASCONDUIT") + " where they deposit their cache.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Smog Slug Egg", nameof (STATERPILLAR));

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Smog Sluglet", nameof (STATERPILLAR));
            public static LocString DESC = (LocString) ("A tubby little Smog Sluglet.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Smog Slug", nameof (STATERPILLAR)) + ".");
          }
        }

        public class VARIANT_LIQUID
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Sponge Slug", nameof (STATERPILLAR));
          public static LocString DESC = (LocString) ("Sponge Slugs are thirsty creatures that soak up " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " during the day.\n\nThey deposit their stored " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " into the exposed " + UI.FormatAsLink("Liquid Pipes", "LIQUIDCONDUIT") + " they sleep next to at night.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Sponge Slug Egg", nameof (STATERPILLAR));

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Sponge Sluglet", nameof (STATERPILLAR));
            public static LocString DESC = (LocString) ("A chonky little Sponge Sluglet.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Sponge Slug", nameof (STATERPILLAR)) + ".");
          }
        }
      }

      public class DIVERGENT
      {
        public class VARIANT_BEETLE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Sweetle", "DIVERGENTBEETLE");
          public static LocString DESC = (LocString) ("Sweetles are nonhostile critters that excrete large amounts of solid " + UI.FormatAsLink("Sucrose", "SUCROSE") + ".\n\nThey are closely related to the " + UI.FormatAsLink("Grubgrub", "DIVERGENTWORM") + " and exhibit similar, albeit less effective farming behaviors.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Sweetle Egg", "DIVERGENTBEETLE");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Sweetle Larva", "DIVERGENTBEETLE");
            public static LocString DESC = (LocString) ("A crawly little Sweetle Larva.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Sweetle", "DIVERGENTBEETLE") + ".");
          }
        }

        public class VARIANT_WORM
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Grubgrub", "DIVERGENTWORM");
          public static LocString DESC = (LocString) ("Grubgrubs form symbiotic relationships with plants, especially " + UI.FormatAsLink("Grubfruit Plants", "WORMPLANT") + ", and instinctually tend to them.\n\nGrubgrubs are closely related to " + UI.FormatAsLink("Sweetles", "DIVERGENTBEETLE") + ".");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Grubgrub Egg", "DIVERGENTWORM");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Grubgrub Wormling", "DIVERGENTWORM");
            public static LocString DESC = (LocString) ("A squirmy little Grubgrub Wormling.\n\nIn time, it will mature into an adult " + UI.FormatAsLink("Grubgrub", "WORM") + " and drastically grow in size.");
          }
        }
      }

      public class DRECKO
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Drecko", nameof (DRECKO));
        public static LocString DESC = (LocString) ("Dreckos are nonhostile critters that graze on " + UI.FormatAsLink("Pincha Pepperplants", "SPICE_VINE") + ", " + UI.FormatAsLink("Balm Lily", "SWAMPLILY") + " or " + UI.FormatAsLink("Mealwood Plants", "BASICSINGLEHARVESTPLANT") + ".\n\nTheir backsides are covered in thick woolly fibers that only grow in " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + " climates.");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Drecklet Egg", nameof (DRECKO));

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Drecklet", nameof (DRECKO));
          public static LocString DESC = (LocString) ("A little, bug-eyed Drecklet.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Drecko", nameof (DRECKO)) + ".");
        }

        public class VARIANT_PLASTIC
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Glossy Drecko", "DRECKOPLASTIC");
          public static LocString DESC = (LocString) ("Glossy Dreckos are nonhostile critters that graze on live " + UI.FormatAsLink("Mealwood Plants", "BASICSINGLEHARVESTPLANT") + " and " + UI.FormatAsLink("Bristle Blossoms", "PRICKLEFLOWER") + ".\n\nTheir backsides are covered in bioplastic scales that only grow in " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + " climates.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Glossy Drecklet Egg", "DRECKOPLASTIC");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Glossy Drecklet", "DRECKOPLASTIC");
            public static LocString DESC = (LocString) ("A bug-eyed little Glossy Drecklet.\n\nIn time it will mature into an adult Drecko morph, the " + UI.FormatAsLink("Glossy Drecko", "DRECKOPLASTIC") + ".");
          }
        }
      }

      public class SQUIRREL
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pip", nameof (SQUIRREL));
        public static LocString DESC = (LocString) ("Pips are pesky, nonhostile critters that subsist on " + UI.FormatAsLink("Thimble Reeds", "BASICFABRICPLANT") + " and " + UI.FormatAsLink("Arbor Tree", "FOREST_TREE") + " branches.\n\nThey are known to bury " + UI.FormatAsLink("Seeds", "PLANTS") + " in the ground whenever they can find a suitable area with enough space.");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Pip Egg", nameof (SQUIRREL));

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Pipsqueak", nameof (SQUIRREL));
          public static LocString DESC = (LocString) ("A little purring Pipsqueak.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Pip", nameof (SQUIRREL)) + ".");
        }

        public class VARIANT_HUG
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Cuddle Pip", nameof (SQUIRREL));
          public static LocString DESC = (LocString) "Cuddle Pips are fluffy, affectionate critters who exhibit a strong snuggling instinct towards all types of eggs.\n\nThis is temporarily amplified when they are hugged by a passing Duplicant.";
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Cuddle Pip Egg", nameof (SQUIRREL));

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Cuddle Pipsqueak", nameof (SQUIRREL));
            public static LocString DESC = (LocString) ("A fuzzy little Cuddle Pipsqueak.\n\nIn time it will mature into a fully grown " + UI.FormatAsLink("Cuddle Pip", nameof (SQUIRREL)) + ".");
          }
        }
      }

      public class OILFLOATER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Slickster", nameof (OILFLOATER));
        public static LocString DESC = (LocString) ("Slicksters are slimy critters that consume " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " and exude " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + ".");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Larva Egg", nameof (OILFLOATER));

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Slickster Larva", nameof (OILFLOATER));
          public static LocString DESC = (LocString) ("A goopy little Slickster Larva.\n\nOne day it will grow into an adult " + UI.FormatAsLink("Slickster", nameof (OILFLOATER)) + ".");
        }

        public class VARIANT_HIGHTEMP
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Molten Slickster", "OILFLOATERHIGHTEMP");
          public static LocString DESC = (LocString) ("Molten Slicksters are slimy critters that consume " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " and exude " + UI.FormatAsLink("Petroleum", "PETROLEUM") + ".");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Molten Larva Egg", "OILFLOATERHIGHTEMP");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Molten Larva", "OILFLOATERHIGHTEMP");
            public static LocString DESC = (LocString) ("A goopy little Molten Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("Molten Slickster", "OILFLOATERHIGHTEMP") + ".");
          }
        }

        public class VARIANT_DECOR
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Longhair Slickster", "OILFLOATERDECOR");
          public static LocString DESC = (LocString) ("Longhair Slicksters are friendly critters that consume " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and thrive in close contact with Duplicant companions.\n\nLonghairs have extremely beautiful and luxurious coats.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Longhair Larva Egg", "OILFLOATERDECOR");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Longhair Larva", "OILFLOATERDECOR");
            public static LocString DESC = (LocString) ("A snuggly little Longhair Larva.\n\nOne day it will grow into an adult Slickster morph, the " + UI.FormatAsLink("Longhair Slickster", "OILFLOATERDECOR") + ".");
          }
        }
      }

      public class PUFT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Puft", nameof (PUFT));
        public static LocString DESC = (LocString) ("Pufts are non-aggressive critters that excrete lumps of " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " with each breath.");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Puftlet Egg", nameof (PUFT));

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Puftlet", nameof (PUFT));
          public static LocString DESC = (LocString) ("A gassy little Puftlet.\n\nIn time it will grow into an adult " + UI.FormatAsLink("Puft", nameof (PUFT)) + ".");
        }

        public class VARIANT_ALPHA
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Puft Prince", "PUFTALPHA");
          public static LocString DESC = (LocString) ("The Puft Prince is a lazy critter that excretes little " + UI.FormatAsLink("Solid", "SOLID") + " lumps of whatever it has been breathing.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Puftlet Prince Egg", "PUFTALPHA");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Puftlet Prince", "PUFTALPHA");
            public static LocString DESC = (LocString) ("A gassy little Puftlet Prince.\n\nOne day it will grow into an adult Puft morph, the " + UI.FormatAsLink("Puft Prince", "PUFTALPHA") + ".\n\nIt seems a bit snobby...");
          }
        }

        public class VARIANT_OXYLITE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Dense Puft", "PUFTOXYLITE");
          public static LocString DESC = (LocString) ("Dense Pufts are non-aggressive critters that excrete condensed " + UI.FormatAsLink("Oxylite", "OXYROCK") + " with each breath.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Dense Puftlet Egg", "PUFTOXYLITE");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Dense Puftlet", "PUFTOXYLITE");
            public static LocString DESC = (LocString) ("A stocky little Dense Puftlet.\n\nOne day it will grow into an adult Puft morph, the " + UI.FormatAsLink("Dense Puft", "PUFTOXYLITE") + ".");
          }
        }

        public class VARIANT_BLEACHSTONE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Squeaky Puft", "PUFTBLEACHSTONE");
          public static LocString DESC = (LocString) ("Squeaky Pufts are non-aggressive critters that excrete lumps of " + UI.FormatAsLink("Bleachstone", "BLEACHSTONE") + " with each breath.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Squeaky Puftlet Egg", "PUFTBLEACHSTONE");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Squeaky Puftlet", "PUFTBLEACHSTONE");
            public static LocString DESC = (LocString) ("A frazzled little Squeaky Puftlet.\n\nOne day it will grow into an adult Puft morph, the " + UI.FormatAsLink("Squeaky Puft", "PUFTBLEACHSTONE") + ".");
          }
        }
      }

      public class MOO
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Gassy Moo", nameof (MOO));
        public static LocString DESC = (LocString) ("Moos are extraterrestrial critters that feed on " + UI.FormatAsLink("Gas Grass", "GASGRASS") + " and excrete " + UI.FormatAsLink("Natural Gas", "METHANE") + ".");
      }

      public class MOLE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Shove Vole", nameof (MOLE));
        public static LocString DESC = (LocString) ("Shove Voles are burrowing critters that eat the " + UI.FormatAsLink("Regolith", "REGOLITH") + " collected on terrestrial surfaces.\n\nThey cannot burrow through " + UI.FormatAsLink("Refined Metals", "REFINEDMETAL") + ".");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Shove Vole Egg", nameof (MOLE));

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Vole Pup", nameof (MOLE));
          public static LocString DESC = (LocString) ("A snuggly little pup.\n\nOne day it will grow into an adult " + UI.FormatAsLink("Shove Vole", nameof (MOLE)) + ".");
        }

        public class VARIANT_DELICACY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Delecta Vole", "MOLEDELICACY");
          public static LocString DESC = (LocString) ("Delecta Voles are burrowing critters whose bodies sprout shearable " + UI.FormatAsLink("Tonic Root", "GINGER") + " when " + UI.FormatAsLink("Regolith", "REGOLITH") + " is ingested at preferred temperatures.\n\nThey cannot burrow through " + UI.FormatAsLink("Refined Metals", "REFINEDMETAL") + ".");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Delecta Vole Egg", "MOLEDELICACY");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Delecta Vole Pup", "MOLEDELICACY");
            public static LocString DESC = (LocString) ("A tender little Delecta Vole pup.\n\nOne day it will grow into an adult Shove Vole morph, the " + UI.FormatAsLink("Delecta Vole", "MOLEDELICACY") + ".");
          }
        }
      }

      public class GREEDYGREEN
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Avari Vine", nameof (GREEDYGREEN));
        public static LocString DESC = (LocString) ("A rapidly growing, subterranean " + UI.FormatAsLink("Plant", "PLANTS") + ".");
      }

      public class SHOCKWORM
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Shockworm", nameof (SHOCKWORM));
        public static LocString DESC = (LocString) "Shockworms are exceptionally aggressive and discharge electrical shocks to stun their prey.";
      }

      public class LIGHTBUG
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Shine Bug", nameof (LIGHTBUG));
        public static LocString DESC = (LocString) ("Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.");
        public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Shine Nymph Egg", nameof (LIGHTBUG));

        public class BABY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Shine Nymph", nameof (LIGHTBUG));
          public static LocString DESC = (LocString) ("A chubby baby " + UI.FormatAsLink("Shine Bug", nameof (LIGHTBUG)) + ".");
        }

        public class VARIANT_ORANGE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Sun Bug", "LIGHTBUGORANGE");
          public static LocString DESC = (LocString) ("Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.\n\nThe light of the Sun morph has been turned orange through selective breeding.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Sun Nymph Egg", "LIGHTBUGORANGE");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Sun Nymph", "LIGHTBUGORANGE");
            public static LocString DESC = (LocString) ("A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGORANGE") + ".\n\nThis one is a Sun morph.");
          }
        }

        public class VARIANT_PURPLE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Royal Bug", "LIGHTBUGPURPLE");
          public static LocString DESC = (LocString) ("Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.\n\nThe light of the Royal morph has been turned purple through selective breeding.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Royal Nymph Egg", "LIGHTBUGPURPLE");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Royal Nymph", "LIGHTBUGPURPLE");
            public static LocString DESC = (LocString) ("A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGPURPLE") + ".\n\nThis one is a Royal morph.");
          }
        }

        public class VARIANT_PINK
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Coral Bug", "LIGHTBUGPINK");
          public static LocString DESC = (LocString) ("Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.\n\nThe light of the Coral morph has been turned pink through selective breeding.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Coral Nymph Egg", "LIGHTBUGPINK");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Coral Nymph", "LIGHTBUGPINK");
            public static LocString DESC = (LocString) ("A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGPINK") + ".\n\nThis one is a Coral morph.");
          }
        }

        public class VARIANT_BLUE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Azure Bug", "LIGHTBUGBLUE");
          public static LocString DESC = (LocString) ("Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.\n\nThe light of the Azure morph has been turned blue through selective breeding.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Azure Nymph Egg", "LIGHTBUGBLUE");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Azure Nymph", "LIGHTBUGBLUE");
            public static LocString DESC = (LocString) ("A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGBLUE") + ".\n\nThis one is an Azure morph.");
          }
        }

        public class VARIANT_BLACK
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Abyss Bug", "LIGHTBUGBLACK");
          public static LocString DESC = (LocString) ("This Shine Bug emits no " + UI.FormatAsLink("Light", "LIGHT") + ", but it makes up for it by having an excellent personality.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Abyss Nymph Egg", "LIGHTBUGBLACK");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Abyss Nymph", "LIGHTBUGBLACK");
            public static LocString DESC = (LocString) ("A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGBLACK") + ".\n\nThis one is an Abyss morph.");
          }
        }

        public class VARIANT_CRYSTAL
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Radiant Bug", "LIGHTBUGCRYSTAL");
          public static LocString DESC = (LocString) ("Shine Bugs emit a soft " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.\n\nThe light of the Radiant morph has been amplified through selective breeding.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Radiant Nymph Egg", "LIGHTBUGCRYSTAL");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Radiant Nymph", "LIGHTBUGCRYSTAL");
            public static LocString DESC = (LocString) ("A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGCRYSTAL") + ".\n\nThis one is a Radiant morph.");
          }
        }

        public class VARIANT_RADIOACTIVE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Ionizing Bug", "LIGHTBUGRADIOACTIVE");
          public static LocString DESC = (LocString) ("Shine Bugs emit a dangerously radioactive " + UI.FormatAsLink("Light", "LIGHT") + " in hopes of attracting more of their kind for company.");
          public static LocString EGG_NAME = (LocString) UI.FormatAsLink("Ionizing Nymph Egg", "LIGHTBUGCRYSTAL");

          public class BABY
          {
            public static LocString NAME = (LocString) UI.FormatAsLink("Ionizing Nymph", "LIGHTBUGRADIOACTIVE");
            public static LocString DESC = (LocString) ("A chubby baby " + UI.FormatAsLink("Shine Bug", "LIGHTBUGRADIOACTIVE") + ".\n\nThis one is an Ionizing morph.");
          }
        }
      }

      public class GEYSER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Steam Geyser", nameof (GEYSER));
        public static LocString DESC = (LocString) ("A highly pressurized geyser that periodically erupts, spraying " + UI.FormatAsLink("Steam", "STEAM") + " and boiling hot " + UI.FormatAsLink("Water", "WATER") + ".");

        public class STEAM
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Cool Steam Vent", "GeyserGeneric_STEAM");
          public static LocString DESC = (LocString) ("A highly pressurized vent that periodically erupts with " + UI.FormatAsLink("Steam", nameof (STEAM)) + ".");
        }

        public class HOT_STEAM
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Steam Vent", "GeyserGeneric_HOT_STEAM");
          public static LocString DESC = (LocString) ("A highly pressurized vent that periodically erupts with scalding " + UI.FormatAsLink("Steam", "STEAM") + ".");
        }

        public class SALT_WATER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Salt Water Geyser", "GeyserGeneric_SALT_WATER");
          public static LocString DESC = (LocString) ("A highly pressurized geyser that periodically erupts with " + UI.FormatAsLink("Salt Water", "SALTWATER") + ".");
        }

        public class SLUSH_SALT_WATER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Cool Salt Slush Geyser", "GeyserGeneric_SLUSH_SALT_WATER");
          public static LocString DESC = (LocString) ("A highly pressurized geyser that periodically erupts with freezing " + (string) ELEMENTS.BRINE.NAME + ".");
        }

        public class HOT_WATER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Water Geyser", "GeyserGeneric_HOT_WATER");
          public static LocString DESC = (LocString) ("A highly pressurized geyser that periodically erupts with hot " + UI.FormatAsLink("Water", "WATER") + ".");
        }

        public class SLUSH_WATER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Cool Slush Geyser", "GeyserGeneric_SLUSHWATER");
          public static LocString DESC = (LocString) ("A highly pressurized geyser that periodically erupts with freezing " + (string) ELEMENTS.DIRTYWATER.NAME + ".");
        }

        public class FILTHY_WATER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Polluted Water Vent", "GeyserGeneric_FILTHYWATER");
          public static LocString DESC = (LocString) ("A highly pressurized vent that periodically erupts with boiling " + UI.FormatAsLink("Contaminated Water", "DIRTYWATER") + ".");
        }

        public class SMALL_VOLCANO
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Minor Volcano", "GeyserGeneric_SMALL_VOLCANO");
          public static LocString DESC = (LocString) ("A miniature volcano that periodically erupts with molten " + UI.FormatAsLink("Magma", "MAGMA") + ".");
        }

        public class BIG_VOLCANO
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Volcano", "GeyserGeneric_BIG_VOLCANO");
          public static LocString DESC = (LocString) ("A massive volcano that periodically erupts with molten " + UI.FormatAsLink("Magma", "MAGMA") + ".");
        }

        public class LIQUID_CO2
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Carbon Dioxide Geyser", "GeyserGeneric_LIQUID_CO2");
          public static LocString DESC = (LocString) ("A highly pressurized geyser that periodically erupts with boiling liquid " + UI.FormatAsLink("Carbon Dioxide", "LIQUIDCARBONDIOXIDE") + ".");
        }

        public class HOT_CO2
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Carbon Dioxide Vent", "GeyserGeneric_HOT_CO2");
          public static LocString DESC = (LocString) ("A highly pressurized vent that periodically erupts with hot gaseous " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + ".");
        }

        public class HOT_HYDROGEN
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Hydrogen Vent", "GeyserGeneric_HOT_HYDROGEN");
          public static LocString DESC = (LocString) ("A highly pressurized vent that periodically erupts with hot gaseous " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + ".");
        }

        public class HOT_PO2
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Hot Polluted Oxygen Vent", "GeyserGeneric_HOT_PO2");
          public static LocString DESC = (LocString) ("A highly pressurized vent that periodically erupts with hot " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + ".");
        }

        public class SLIMY_PO2
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Infectious Polluted Oxygen Vent", "GeyserGeneric_SLIMY_PO2");
          public static LocString DESC = (LocString) ("A highly pressurized vent that periodically erupts with warm " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + ".");
        }

        public class CHLORINE_GAS
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Chlorine Gas Vent", "GeyserGeneric_CHLORINE_GAS");
          public static LocString DESC = (LocString) ("A highly pressurized vent that periodically erupts with warm " + UI.FormatAsLink("Chlorine", "CHLORINEGAS") + ".");
        }

        public class METHANE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Natural Gas Geyser", "GeyserGeneric_METHANE");
          public static LocString DESC = (LocString) ("A highly pressurized geyser that periodically erupts with hot " + UI.FormatAsLink("Natural Gas", nameof (METHANE)) + ".");
        }

        public class MOLTEN_COPPER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Copper Volcano", "GeyserGeneric_MOLTEN_COPPER");
          public static LocString DESC = (LocString) ("A large volcano that periodically erupts with molten " + UI.FormatAsLink("Copper", "MOLTENCOPPER") + ".");
        }

        public class MOLTEN_IRON
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Iron Volcano", "GeyserGeneric_MOLTEN_IRON");
          public static LocString DESC = (LocString) ("A large volcano that periodically erupts with molten " + UI.FormatAsLink("Iron", "MOLTENIRON") + ".");
        }

        public class MOLTEN_ALUMINUM
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Aluminum Volcano", "GeyserGeneric_MOLTEN_ALUMINUM");
          public static LocString DESC = (LocString) ("A large volcano that periodically erupts with molten " + UI.FormatAsLink("Aluminum", "MOLTENALUMINUM") + ".");
        }

        public class MOLTEN_TUNGSTEN
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Tungsten Volcano", "GeyserGeneric_MOLTEN_TUNGSTEN");
          public static LocString DESC = (LocString) ("A large volcano that periodically erupts with molten " + UI.FormatAsLink("Tungsten", "MOLTENTUNGSTEN") + ".");
        }

        public class MOLTEN_GOLD
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Gold Volcano", "GeyserGeneric_MOLTEN_GOLD");
          public static LocString DESC = (LocString) ("A large volcano that periodically erupts with molten " + UI.FormatAsLink("Gold", "MOLTENGOLD") + ".");
        }

        public class MOLTEN_COBALT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Cobalt Volcano", "GeyserGeneric_MOLTEN_COBALT");
          public static LocString DESC = (LocString) ("A large volcano that periodically erupts with molten " + UI.FormatAsLink("Cobalt", "MOLTENCOBALT") + ".");
        }

        public class MOLTEN_NIOBIUM
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Niobium Volcano", "NiobiumGeyser");
          public static LocString DESC = (LocString) ("A large volcano that periodically erupts with molten " + UI.FormatAsLink("Niobium", "NIOBIUM") + ".");
        }

        public class OIL_DRIP
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Leaky Oil Fissure", "GeyserGeneric_OIL_DRIP");
          public static LocString DESC = (LocString) ("A fissure that periodically erupts with boiling " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + ".");
        }

        public class LIQUID_SULFUR
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Liquid Sulfur Geyser", "GeyserGeneric_LIQUID_SULFUR");
          public static LocString DESC = (LocString) ("A highly pressurized geyser that periodically erupts with boiling " + UI.FormatAsLink("Sulfur", "LIQUIDSULFUR") + ".");
        }
      }

      public class METHANEGEYSER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Natural Gas Geyser", "GeyserGeneric_METHANEGEYSER");
        public static LocString DESC = (LocString) ("A highly pressurized geyser that periodically erupts with " + UI.FormatAsLink("Natural Gas", "METHANE") + ".");
      }

      public class OIL_WELL
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Oil Reservoir", nameof (OIL_WELL));
        public static LocString DESC = (LocString) ("Oil Reservoirs are rock formations with " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " deposits beneath their surface.\n\nOil can be extracted from a reservoir with sufficient pressure.");
      }

      public class MUSHROOMPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Dusk Cap", nameof (MUSHROOMPLANT));
        public static LocString DESC = (LocString) ("Dusk Caps produce " + UI.FormatAsLink("Mushrooms", "MUSHROOM") + ", fungal growths that can be harvested for " + UI.FormatAsLink("Food", "FOOD") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces edible " + UI.FormatAsLink("Mushrooms", "MUSHROOM") + ".");
      }

      public class STEAMSPOUT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Steam Spout", "GEYSERS");
        public static LocString DESC = (LocString) ("A rocky vent that spouts " + UI.FormatAsLink("Steam", "STEAM") + ".");
      }

      public class PROPANESPOUT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Propane Spout", "GEYSERS");
        public static LocString DESC = (LocString) ("A rocky vent that spouts " + (string) ELEMENTS.PROPANE.NAME + ".");
      }

      public class OILSPOUT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Oil Spout", nameof (OILSPOUT));
        public static LocString DESC = (LocString) ("A rocky vent that spouts " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + ".");
      }

      public class HEATBULB
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Fervine", nameof (HEATBULB));
        public static LocString DESC = (LocString) ("A temperature reactive, subterranean " + UI.FormatAsLink("Plant", "PLANTS") + ".");
      }

      public class HEATBULBSEED
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Fervine Bulb", nameof (HEATBULBSEED));
        public static LocString DESC = (LocString) ("A temperature reactive, subterranean " + UI.FormatAsLink("Plant", "PLANTS") + ".");
      }

      public class PACUEGG
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pacu Egg", nameof (PACUEGG));
        public static LocString DESC = (LocString) "A tiny Pacu is nestled inside.\n\nIt is not yet ready for the world.";
      }

      public class MYSTERYEGG
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Mysterious Egg", nameof (MYSTERYEGG));
        public static LocString DESC = (LocString) "What's growing inside? Something nice? Something mean?";
      }

      public class SWAMPLILY
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Balm Lily", nameof (SWAMPLILY));
        public static LocString DESC = (LocString) ("Balm Lilies produce " + (string) ITEMS.INGREDIENTS.SWAMPLILYFLOWER.NAME + ", a lovely bloom with medicinal properties.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces medicinal " + (string) ITEMS.INGREDIENTS.SWAMPLILYFLOWER.NAME + ".");
      }

      public class JUNGLEGASPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Palmera Tree", nameof (JUNGLEGASPLANT));
        public static LocString DESC = (LocString) ("A large, chlorine-dwelling " + UI.FormatAsLink("Plant", "PLANTS") + " that can be grown in farm buildings.\n\nPalmeras grow inedible buds that emit unbreathable hydrogen gas.");
        public static LocString DOMESTICATEDDESC = (LocString) ("A large, chlorine-dwelling " + UI.FormatAsLink("Plant", "PLANTS") + " that grows inedible buds which emit unbreathable hydrogen gas.");
      }

      public class PRICKLEFLOWER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Bristle Blossom", nameof (PRICKLEFLOWER));
        public static LocString DESC = (LocString) ("Bristle Blossoms produce " + (string) ITEMS.FOOD.PRICKLEFRUIT.NAME + ", a prickly edible bud.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces edible " + UI.FormatAsLink("Bristle Berries", UI.StripLinkFormatting((string) ITEMS.FOOD.PRICKLEFRUIT.NAME)) + ".");
      }

      public class COLDWHEAT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Sleet Wheat", nameof (COLDWHEAT));
        public static LocString DESC = (LocString) ("Sleet Wheat produces " + (string) ITEMS.FOOD.COLDWHEATSEED.NAME + ", a chilly grain that can be processed into " + UI.FormatAsLink("Food", "FOOD") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces edible " + (string) ITEMS.FOOD.COLDWHEATSEED.NAME + ".");
      }

      public class GASGRASS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Gas Grass", nameof (GASGRASS));
        public static LocString DESC = (LocString) "Gas grass.";
        public static LocString DOMESTICATEDDESC = (LocString) ("An alien grass variety that is eaten by " + UI.FormatAsLink("Gassy Moos", "MOO") + ".");
      }

      public class PRICKLEGRASS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Bluff Briar", nameof (PRICKLEGRASS));
        public static LocString DESC = (LocString) "Bluff Briars exude pheromones causing critters to view them as especially beautiful.";
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant improves " + UI.FormatAsLink("Decor", "DECOR") + ".");
        public static LocString GROWTH_BONUS = (LocString) "Growth Bonus";
        public static LocString WILT_PENALTY = (LocString) "Wilt Penalty";
      }

      public class CYLINDRICA
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Bliss Burst", nameof (CYLINDRICA));
        public static LocString DESC = (LocString) ("Bliss Bursts release an explosion of " + UI.FormatAsLink("Decor", "DECOR") + " into otherwise dull environments.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".");
        public static LocString GROWTH_BONUS = (LocString) "Growth Bonus";
        public static LocString WILT_PENALTY = (LocString) "Wilt Penalty";
      }

      public class TOEPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Tranquil Toes", nameof (TOEPLANT));
        public static LocString DESC = (LocString) ("Tranquil Toes improve " + UI.FormatAsLink("Decor", "DECOR") + " by giving their surroundings the visual equivalent of a foot rub.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".");
        public static LocString GROWTH_BONUS = (LocString) "Growth Bonus";
        public static LocString WILT_PENALTY = (LocString) "Wilt Penalty";
      }

      public class WINECUPS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Mellow Mallow", nameof (WINECUPS));
        public static LocString DESC = (LocString) ("Mellow Mallows heighten " + UI.FormatAsLink("Decor", "DECOR") + " and alleviate " + UI.FormatAsLink("Stress", "STRESS") + " with their calming color and cradle shape.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".");
        public static LocString GROWTH_BONUS = (LocString) "Growth Bonus";
        public static LocString WILT_PENALTY = (LocString) "Wilt Penalty";
      }

      public class EVILFLOWER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Sporechid", nameof (EVILFLOWER));
        public static LocString DESC = (LocString) "Sporechids have an eerily alluring appearance to mask the fact that they host particularly nasty strain of brain fungus.";
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + " but produces high quantities of " + UI.FormatAsLink("Zombie Spores", "ZOMBIESPORES") + ".");
        public static LocString GROWTH_BONUS = (LocString) "Growth Bonus";
        public static LocString WILT_PENALTY = (LocString) "Wilt Penalty";
      }

      public class LEAFYPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Mirth Leaf", "POTTED_LEAFY");
        public static LocString DESC = (LocString) ("Mirth Leaves sport a calm green hue known for alleviating " + UI.FormatAsLink("Stress", "STRESS") + " and improving " + UI.FormatAsLink("Morale", "MORALE") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".");
        public static LocString GROWTH_BONUS = (LocString) "Growth Bonus";
        public static LocString WILT_PENALTY = (LocString) "Wilt Penalty";
      }

      public class CACTUSPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Jumping Joya", "POTTED_CACTUS");
        public static LocString DESC = (LocString) ("Joyas are " + UI.FormatAsLink("Decorative", "DECOR") + " " + UI.FormatAsLink("Plants", "PLANTS") + " that are colloquially said to make gardeners \"jump for joy\".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".");
        public static LocString GROWTH_BONUS = (LocString) "Growth Bonus";
        public static LocString WILT_PENALTY = (LocString) "Wilt Penalty";
      }

      public class BULBPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Buddy Bud", "POTTED_BULB");
        public static LocString DESC = (LocString) ("Buddy Buds are leafy plants that have a positive effect on " + UI.FormatAsLink("Morale", "MORALE") + ", much like a friend.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant improves ambient " + UI.FormatAsLink("Decor", "DECOR") + ".");
        public static LocString GROWTH_BONUS = (LocString) "Growth Bonus";
        public static LocString WILT_PENALTY = (LocString) "Wilt Penalty";
      }

      public class BASICSINGLEHARVESTPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Mealwood", nameof (BASICSINGLEHARVESTPLANT));
        public static LocString DESC = (LocString) ("Mealwoods produce " + (string) ITEMS.FOOD.BASICPLANTFOOD.NAME + ", an oddly wriggly grain that can be harvested for " + UI.FormatAsLink("Food", "FOOD") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces edible " + (string) ITEMS.FOOD.BASICPLANTFOOD.NAME + ".");
      }

      public class SWAMPHARVESTPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Bog Bucket", nameof (SWAMPHARVESTPLANT));
        public static LocString DESC = (LocString) ("Bog Buckets produce juicy, sweet " + UI.FormatAsLink("Bog Jellies", "SWAMPFRUIT") + " for " + UI.FormatAsLink("Food", "FOOD") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces edible " + UI.FormatAsLink("Bog Jellies", "SWAMPFRUIT") + ".");
      }

      public class WORMPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Spindly Grubfruit Plant", nameof (WORMPLANT));
        public static LocString DESC = (LocString) ("Spindly Grubfruit Plants produce " + UI.FormatAsLink("Spindly Grubfruit", "WORMBASICFRUIT") + " for " + UI.FormatAsLink("Food", "FOOD") + ".\n\nIf it is tended by a " + (string) CREATURES.FAMILY.DIVERGENTSPECIES + " critter, it will produce high quality fruits instead.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces edible " + (string) ITEMS.FOOD.WORMBASICFRUIT.NAME + ".");
      }

      public class SUPERWORMPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Grubfruit Plant", "WORMPLANT");
        public static LocString DESC = (LocString) ("A Grubfruit Plant that has flourished after being tended by a " + (string) CREATURES.FAMILY.DIVERGENTSPECIES + " critter.\n\nIt will produce high quality " + UI.FormatAsLink("Grubfruits", "WORMSUPERFRUIT") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces edible " + (string) ITEMS.FOOD.WORMSUPERFRUIT.NAME + ".");
      }

      public class BASICFABRICMATERIALPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Thimble Reed", "BASICFABRICPLANT");
        public static LocString DESC = (LocString) ("Thimble Reeds produce indescribably soft " + (string) ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME + " for " + UI.FormatAsLink("Clothing", "EQUIPMENT") + " production.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces " + (string) ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME + ".");
      }

      public class BASICFORAGEPLANTPLANTED
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Buried Muckroot", nameof (BASICFORAGEPLANTPLANTED));
        public static LocString DESC = (LocString) ("Muckroots are incapable of propagating but can be harvested for a single " + UI.FormatAsLink("Food", "FOOD") + " serving.");
      }

      public class FORESTFORAGEPLANTPLANTED
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Hexalent", nameof (FORESTFORAGEPLANTPLANTED));
        public static LocString DESC = (LocString) ("Hexalents are incapable of propagating but can be harvested for a single, calorie dense " + UI.FormatAsLink("Food", "FOOD") + " serving.");
      }

      public class SWAMPFORAGEPLANTPLANTED
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Swamp Chard", nameof (SWAMPFORAGEPLANTPLANTED));
        public static LocString DESC = (LocString) ("Swamp Chards are incapable of propagating but can be harvested for a single low quality and calorie dense " + UI.FormatAsLink("Food", "FOOD") + " serving.");
      }

      public class COLDBREATHER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Wheezewort", nameof (COLDBREATHER));
        public static LocString DESC = (LocString) ("Wheezeworts can be planted in " + UI.FormatAsLink("Planter Boxes", "PLANTERBOX") + ", " + UI.FormatAsLink("Farm Tiles", "FARMTILE") + " or " + UI.FormatAsLink("Hydroponic Farms", "HYDROPONICFARM") + ", and absorb " + UI.FormatAsLink("Heat", "Heat") + " by respiring through their porous outer membranes.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant absorbs " + UI.FormatAsLink("Heat", "Heat") + ".");
      }

      public class COLDBREATHERCLUSTER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Wheezewort", nameof (COLDBREATHERCLUSTER));
        public static LocString DESC = (LocString) ("Wheezeworts can be planted in " + UI.FormatAsLink("Planter Boxes", "PLANTERBOX") + ", " + UI.FormatAsLink("Farm Tiles", "FARMTILE") + " or " + UI.FormatAsLink("Hydroponic Farms", "HYDROPONICFARM") + ", and absorb " + UI.FormatAsLink("Heat", "Heat") + " by respiring through their porous outer membranes.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant absorbs " + UI.FormatAsLink("Heat", "Heat") + ".");
      }

      public class SPICE_VINE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pincha Pepperplant", nameof (SPICE_VINE));
        public static LocString DESC = (LocString) ("Pincha Pepperplants produce flavorful " + (string) ITEMS.FOOD.SPICENUT.NAME + " for spicing " + UI.FormatAsLink("Food", "FOOD") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces " + (string) ITEMS.FOOD.SPICENUT.NAME + " spices.");
      }

      public class SALTPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Dasha Saltvine", nameof (SALTPLANT));
        public static LocString DESC = (LocString) ("Dasha Saltvines consume small amounts of " + UI.FormatAsLink("Chlorine Gas", "CHLORINE") + " and form sodium deposits as they grow, producing harvestable " + UI.FormatAsLink("Salt", "SALT") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces unrefined " + UI.FormatAsLink("Salt", "SALT") + ".");
      }

      public class FILTERPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Hydrocactus", nameof (FILTERPLANT));
        public static LocString DESC = (LocString) ("Hydrocacti act as natural " + UI.FormatAsLink("Water", "WATER") + " filters when given access to " + UI.FormatAsLink("Sand", "SAND") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant uses " + UI.FormatAsLink("Sand", "SAND") + " to convert " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " into " + UI.FormatAsLink("Water", "WATER") + ".");
      }

      public class OXYFERN
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Oxyfern", nameof (OXYFERN));
        public static LocString DESC = (LocString) ("Oxyferns absorb " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " and exude breathable " + UI.FormatAsLink("Oxygen", "OXYGEN") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant converts " + UI.FormatAsLink("CO<sub>2</sub>", "CARBONDIOXIDE") + " into " + UI.FormatAsLink("Oxygen", "OXYGEN") + ".");
      }

      public class BEAN_PLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Nosh Sprout", nameof (BEAN_PLANT));
        public static LocString DESC = (LocString) ("Nosh Sprouts thrive in colder climates and produce edible " + UI.FormatAsLink("Nosh Beans", "BEAN") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces " + UI.FormatAsLink("Nosh Beans", "BEAN") + ".");
      }

      public class WOOD_TREE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Arbor Tree", "FOREST_TREE");
        public static LocString DESC = (LocString) ("Arbor Trees grow " + UI.FormatAsLink("Arbor Tree Branches", "FOREST_TREE") + " and can be harvested for lumber.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces " + UI.FormatAsLink("Arbor Tree Branches", "FOREST_TREE") + " that can be harvested for lumber.");
      }

      public class WOOD_TREE_BRANCH
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Arbor Tree Branch", "FOREST_TREE");
        public static LocString DESC = (LocString) "Arbor Trees Branches can be harvested for lumber.";
      }

      public class SEALETTUCE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Waterweed", nameof (SEALETTUCE));
        public static LocString DESC = (LocString) ("Waterweeds thrive in salty water and can be harvested for fresh, edible " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant produces " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".");
      }

      public class CRITTERTRAPPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Saturn Critter Trap", nameof (CRITTERTRAPPLANT));
        public static LocString DESC = (LocString) ("Critter Traps are carnivorous plants that trap unsuspecting critters and consume them, releasing " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + " as waste.");
        public static LocString DOMESTICATEDDESC = (LocString) ("This plant eats critters and produces " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + ".");
      }

      public class SAPTREE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Experiment 52B", nameof (SAPTREE));
        public static LocString DESC = (LocString) ("A " + UI.FormatAsLink("Resin", "RESIN") + "-producing cybernetic tree that shows signs of sentience.\n\nIt is rooted firmly in place, and is waiting for some brave soul to bring it food.");
      }

      public class SEEDS
      {
        public class LEAFYPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Mirth Leaf Seed", nameof (LEAFYPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + UI.FormatAsLink("Mirth Leaf", nameof (LEAFYPLANT)) + ".\n\nDigging up Buried Objects may uncover a Mirth Leaf Seed.");
        }

        public class CACTUSPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Joya Seed", nameof (CACTUSPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + UI.FormatAsLink("Jumping Joya", nameof (CACTUSPLANT)) + ".\n\nDigging up Buried Objects may uncover a Joya Seed.");
        }

        public class BULBPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Buddy Bud Seed", nameof (BULBPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + UI.FormatAsLink("Buddy Bud", nameof (BULBPLANT)) + ".\n\nDigging up Buried Objects may uncover a Buddy Bud Seed.");
        }

        public class JUNGLEGASPLANT
        {
        }

        public class PRICKLEFLOWER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Blossom Seed", nameof (PRICKLEFLOWER));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + UI.FormatAsLink("Bristle Blossom", nameof (PRICKLEFLOWER)) + ".\n\nDigging up Buried Objects may uncover a Blossom Seed.");
        }

        public class MUSHROOMPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Fungal Spore", nameof (MUSHROOMPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.MUSHROOMPLANT.NAME + ".\n\nDigging up Buried Objects may uncover a Fungal Spore.");
        }

        public class COLDWHEAT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Sleet Wheat Grain", nameof (COLDWHEAT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.COLDWHEAT.NAME + " plant.\n\nGrain can be sown to cultivate more Sleet Wheat, or processed into " + UI.FormatAsLink("Food", "FOOD") + ".");
        }

        public class GASGRASS
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Gas Grass Seed", nameof (GASGRASS));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.GASGRASS.NAME + " plant.\n\nUsed as feed for " + UI.FormatAsLink("Gassy Moos", "MOO") + ".");
        }

        public class PRICKLEGRASS
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Briar Seed", nameof (PRICKLEGRASS));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.PRICKLEGRASS.NAME + ".\n\nDigging up Buried Objects may uncover a Briar Seed.");
        }

        public class CYLINDRICA
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Bliss Burst Seed", nameof (CYLINDRICA));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.CYLINDRICA.NAME + ".\n\nDigging up Buried Objects may uncover a Bliss Burst Seed.");
        }

        public class TOEPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Tranquil Toe Seed", nameof (TOEPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.TOEPLANT.NAME + ".\n\nDigging up Buried Objects may uncover a Tranquil Toe Seed.");
        }

        public class WINECUPS
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Mallow Seed", nameof (WINECUPS));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.WINECUPS.NAME + ".\n\nDigging up Buried Objects may uncover a Mallow Seed.");
        }

        public class EVILFLOWER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Sporechid Seed", nameof (EVILFLOWER));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.EVILFLOWER.NAME + ".\n\nDigging up Buried Objects may uncover a " + (string) CREATURES.SPECIES.SEEDS.EVILFLOWER.NAME + ".");
        }

        public class SWAMPLILY
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Balm Lily Seed", nameof (SWAMPLILY));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.SWAMPLILY.NAME + ".\n\nDigging up Buried Objects may uncover a Balm Lily Seed.");
        }

        public class BASICSINGLEHARVESTPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Mealwood Seed", nameof (BASICSINGLEHARVESTPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.BASICSINGLEHARVESTPLANT.NAME + ".\n\nDigging up Buried Objects may uncover a Mealwood Seed.");
        }

        public class SWAMPHARVESTPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Bog Bucket Seed", nameof (SWAMPHARVESTPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.SWAMPHARVESTPLANT.NAME + ".\n\nDigging up Buried Objects may uncover a Bog Bucket Seed.");
        }

        public class WORMPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Grubfruit Seed", nameof (WORMPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.WORMPLANT.NAME + ".\n\nDigging up Buried Objects may uncover a Grubfruit Seed.");
        }

        public class COLDBREATHER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Wort Seed", nameof (COLDBREATHER));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.COLDBREATHER.NAME + ".\n\nDigging up Buried Objects may uncover a Wort Seed.");
        }

        public class BASICFABRICMATERIALPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Thimble Reed Seed", "BASICFABRICPLANT");
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.BASICFABRICMATERIALPLANT.NAME + ".\n\nDigging up Buried Objects may uncover a Thimble Reed Seed.");
        }

        public class SALTPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Dasha Saltvine Seed", nameof (SALTPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.SALTPLANT.NAME + ".\n\nDigging up Buried Objects may uncover a Dasha Saltvine Seed.");
        }

        public class FILTERPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Hydrocactus Seed", nameof (FILTERPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.FILTERPLANT.NAME + ".\n\nDigging up Buried Objects may uncover a Hydrocactus Seed.");
        }

        public class SPICE_VINE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Pincha Pepper Seed", nameof (SPICE_VINE));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + (string) CREATURES.SPECIES.SPICE_VINE.NAME + ".\n\nDigging up Buried Objects may uncover a Pincha Pepper Seed.");
        }

        public class BEAN_PLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Nosh Bean", nameof (BEAN_PLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + UI.FormatAsLink("Nosh Sprout", nameof (BEAN_PLANT)) + ".\n\nDigging up Buried Objects may uncover a Nosh Bean.");
        }

        public class WOOD_TREE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Arbor Acorn", "FOREST_TREE");
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of an " + UI.FormatAsLink("Arbor Tree", "FOREST_TREE") + ".\n\nDigging up Buried Objects may uncover an Arbor Acorn.");
        }

        public class OILEATER
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Ink Bloom Seed", nameof (OILEATER));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + UI.FormatAsLink("Plant", "Ink Bloom") + ".\n\nDigging up Buried Objects may uncover an Ink Bloom Seed.");
        }

        public class OXYFERN
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Oxyfern Seed", nameof (OXYFERN));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of an " + UI.FormatAsLink("Oxyfern", nameof (OXYFERN)) + " plant.");
        }

        public class SEALETTUCE
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Waterweed Seed", nameof (SEALETTUCE));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + UI.FormatAsLink("Waterweed", nameof (SEALETTUCE)) + ".\n\nDigging up Buried Objects may uncover a Waterweed Seed.");
        }

        public class CRITTERTRAPPLANT
        {
          public static LocString NAME = (LocString) UI.FormatAsLink("Saturn Critter Trap Seed", nameof (CRITTERTRAPPLANT));
          public static LocString DESC = (LocString) ("The " + UI.FormatAsLink("Seed", "PLANTS") + " of a " + UI.FormatAsLink("Saturn Critter Trap", nameof (CRITTERTRAPPLANT)) + ".\n\nDigging up Buried Objects may uncover a Saturn Critter Trap Seed.");
        }
      }
    }

    public class STATUSITEMS
    {
      public static LocString NAME_NON_GROWING_PLANT = (LocString) "Wilted";

      public class DROWSY
      {
        public static LocString NAME = (LocString) "Drowsy";
        public static LocString TOOLTIP = (LocString) "This critter is looking for a place to nap";
      }

      public class SLEEPING
      {
        public static LocString NAME = (LocString) "Sleeping";
        public static LocString TOOLTIP = (LocString) ("This critter is replenishing its " + UI.PRE_KEYWORD + "Stamina" + UI.PST_KEYWORD);
      }

      public class CALL_ADULT
      {
        public static LocString NAME = (LocString) "Calling Adult";
        public static LocString TOOLTIP = (LocString) "This baby's craving attention from one of its own kind";
      }

      public class HOT
      {
        public static LocString NAME = (LocString) "Toasty surroundings";
        public static LocString TOOLTIP = (LocString) ("This critter cannot let off enough " + UI.PRE_KEYWORD + "Heat" + UI.PST_KEYWORD + " to keep cool in this environment\n\nIt prefers " + UI.PRE_KEYWORD + "Temperatures" + UI.PST_KEYWORD + " between <b>{0}</b> and <b>{1}</b>");
      }

      public class COLD
      {
        public static LocString NAME = (LocString) "Chilly surroundings";
        public static LocString TOOLTIP = (LocString) ("This critter cannot retain enough " + UI.PRE_KEYWORD + "Heat" + UI.PST_KEYWORD + " to stay warm in this environment\n\nIt prefers " + UI.PRE_KEYWORD + "Temperatures" + UI.PST_KEYWORD + " between <b>{0}</b> and <b>{1}</b>");
      }

      public class CROP_TOO_DARK
      {
        public static LocString NAME = (LocString) ("    • " + (string) CREATURES.STATS.ILLUMINATION.NAME);
        public static LocString TOOLTIP = (LocString) ("Growth will resume when " + UI.PRE_KEYWORD + "Light" + UI.PST_KEYWORD + " requirements are met");
      }

      public class CROP_TOO_BRIGHT
      {
        public static LocString NAME = (LocString) ("    • " + (string) CREATURES.STATS.ILLUMINATION.NAME);
        public static LocString TOOLTIP = (LocString) ("Growth will resume when " + UI.PRE_KEYWORD + "Light" + UI.PST_KEYWORD + " requirements are met");
      }

      public class CROP_BLIGHTED
      {
        public static LocString NAME = (LocString) "    • Blighted";
        public static LocString TOOLTIP = (LocString) "This plant has been struck by blight and will need to be replaced";
      }

      public class HOT_CROP
      {
        public static LocString NAME = (LocString) ("    • " + (string) DUPLICANTS.STATS.TEMPERATURE.NAME);
        public static LocString TOOLTIP = (LocString) ("Growth will resume when ambient " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " is between <b>{low_temperature}</b> and <b>{high_temperature}</b>");
      }

      public class COLD_CROP
      {
        public static LocString NAME = (LocString) ("    • " + (string) DUPLICANTS.STATS.TEMPERATURE.NAME);
        public static LocString TOOLTIP = (LocString) ("Growth will resume when ambient " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " is between <b>{low_temperature}</b> and <b>{high_temperature}</b>");
      }

      public class PERFECTTEMPERATURE
      {
        public static LocString NAME = (LocString) "Ideal Temperature";
        public static LocString TOOLTIP = (LocString) ("This critter finds the current ambient " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " comfortable\n\nIdeal Range: <b>{0}</b> - <b>{1}</b>");
      }

      public class EATING
      {
        public static LocString NAME = (LocString) "Eating";
        public static LocString TOOLTIP = (LocString) "This critter found something tasty";
      }

      public class DIGESTING
      {
        public static LocString NAME = (LocString) "Digesting";
        public static LocString TOOLTIP = (LocString) "This critter is working off a big meal";
      }

      public class COOLING
      {
        public static LocString NAME = (LocString) "Chilly Breath";
        public static LocString TOOLTIP = (LocString) "This critter's respiration is having a cooling effect on the area";
      }

      public class LOOKINGFORFOOD
      {
        public static LocString NAME = (LocString) "Foraging";
        public static LocString TOOLTIP = (LocString) ("This critter is hungry and looking for " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD);
      }

      public class LOOKINGFORLIQUID
      {
        public static LocString NAME = (LocString) "Parched";
        public static LocString TOOLTIP = (LocString) ("This critter is looking for " + UI.PRE_KEYWORD + "Liquids" + UI.PST_KEYWORD + " to mop up");
      }

      public class LOOKINGFORGAS
      {
        public static LocString NAME = (LocString) "Seeking Gas";
        public static LocString TOOLTIP = (LocString) ("This critter is on the lookout for unbreathable " + UI.PRE_KEYWORD + "Gases" + UI.PST_KEYWORD + " to collect");
      }

      public class IDLE
      {
        public static LocString NAME = (LocString) "Idle";
        public static LocString TOOLTIP = (LocString) "Just enjoying life, y'know?";
      }

      public class HIVE_DIGESTING
      {
        public static LocString NAME = (LocString) "Digesting";
        public static LocString TOOLTIP = (LocString) "Digesting yummy food!";
      }

      public class EXCITED_TO_GET_RANCHED
      {
        public static LocString NAME = (LocString) "Excited";
        public static LocString TOOLTIP = (LocString) "This critter heard a Duplicant call for it and is very excited!";
      }

      public class GETTING_RANCHED
      {
        public static LocString NAME = (LocString) "Being Groomed";
        public static LocString TOOLTIP = (LocString) "This critter's going to look so good when they're done";
      }

      public class EXCITED_TO_BE_RANCHED
      {
        public static LocString NAME = (LocString) "Freshly Groomed";
        public static LocString TOOLTIP = (LocString) "This critter just received some attention and feels great";
      }

      public class GETTING_WRANGLED
      {
        public static LocString NAME = (LocString) "Being Wrangled";
        public static LocString TOOLTIP = (LocString) "Someone's trying to capture this critter!";
      }

      public class BAGGED
      {
        public static LocString NAME = (LocString) "Trussed";
        public static LocString TOOLTIP = (LocString) "Tied up and ready for relocation";
      }

      public class IN_INCUBATOR
      {
        public static LocString NAME = (LocString) "Incubation Complete";
        public static LocString TOOLTIP = (LocString) "This critter has hatched and is waiting to be released from its incubator";
      }

      public class HYPOTHERMIA
      {
        public static LocString NAME = (LocString) "Freezing";
        public static LocString TOOLTIP = (LocString) ("Internal " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " is dangerously low");
      }

      public class SCALDING
      {
        public static LocString NAME = (LocString) "Scalding";
        public static LocString TOOLTIP = (LocString) ("Current external " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " is perilously high [<b>{ExternalTemperature}</b> / <b>{TargetTemperature}</b>]");
        public static LocString NOTIFICATION_NAME = (LocString) "Scalding";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) ("Scalding " + UI.PRE_KEYWORD + "Temperatures" + UI.PST_KEYWORD + " are hurting these Duplicants:");
      }

      public class HYPERTHERMIA
      {
        public static LocString NAME = (LocString) "Overheating";
        public static LocString TOOLTIP = (LocString) ("Internal " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " is dangerously high [<b>{InternalTemperature}</b> / <b>{TargetTemperature}</b>]");
      }

      public class TIRED
      {
        public static LocString NAME = (LocString) "Fatigued";
        public static LocString TOOLTIP = (LocString) "This critter needs some sleepytime";
      }

      public class BREATH
      {
        public static LocString NAME = (LocString) "Suffocating";
        public static LocString TOOLTIP = (LocString) "This critter is about to suffocate";
      }

      public class DEAD
      {
        public static LocString NAME = (LocString) "Dead";
        public static LocString TOOLTIP = (LocString) "This critter won't be getting back up...";
      }

      public class PLANTDEATH
      {
        public static LocString NAME = (LocString) "Dead";
        public static LocString TOOLTIP = (LocString) "This plant will produce no more harvests";
        public static LocString NOTIFICATION = (LocString) "Plants have died";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) "These plants have died and will produce no more harvests:\n";
      }

      public class STRUGGLING
      {
        public static LocString NAME = (LocString) "Struggling";
        public static LocString TOOLTIP = (LocString) "This critter is trying to get away";
      }

      public class BURROWING
      {
        public static LocString NAME = (LocString) "Burrowing";
        public static LocString TOOLTIP = (LocString) "This critter is trying to hide";
      }

      public class BURROWED
      {
        public static LocString NAME = (LocString) "Burrowed";
        public static LocString TOOLTIP = (LocString) "Shh! It thinks it's hiding";
      }

      public class EMERGING
      {
        public static LocString NAME = (LocString) "Emerging";
        public static LocString TOOLTIP = (LocString) "This critter is leaving its burrow";
      }

      public class FORAGINGMATERIAL
      {
        public static LocString NAME = (LocString) "Foraging for Materials";
        public static LocString TOOLTIP = (LocString) "This critter is stocking up on supplies for later use";
      }

      public class PLANTINGSEED
      {
        public static LocString NAME = (LocString) "Planting Seed";
        public static LocString TOOLTIP = (LocString) ("This critter is burying a " + UI.PRE_KEYWORD + "Seed" + UI.PST_KEYWORD + " for later");
      }

      public class RUMMAGINGSEED
      {
        public static LocString NAME = (LocString) "Rummaging for seeds";
        public static LocString TOOLTIP = (LocString) ("This critter is searching for tasty " + UI.PRE_KEYWORD + "Seeds" + UI.PST_KEYWORD);
      }

      public class HUGEGG
      {
        public static LocString NAME = (LocString) "Hugging Eggs";
        public static LocString TOOLTIP = (LocString) ("This critter is snuggling up to an " + UI.PRE_KEYWORD + "Egg" + UI.PST_KEYWORD + " ");
      }

      public class HUGMINIONWAITING
      {
        public static LocString NAME = (LocString) "Hoping for hugs";
        public static LocString TOOLTIP = (LocString) "This critter is hoping for a Duplicant to pass by and give it a hug\n\nA hug from a Duplicant will prompt it to cuddle more eggs";
      }

      public class HUGMINION
      {
        public static LocString NAME = (LocString) "Hugging";
        public static LocString TOOLTIP = (LocString) "This critter is happily hugging a Duplicant";
      }

      public class EXPELLING_SOLID
      {
        public static LocString NAME = (LocString) "Expelling Waste";
        public static LocString TOOLTIP = (LocString) "This critter is doing their \"business\"";
      }

      public class EXPELLING_GAS
      {
        public static LocString NAME = (LocString) "Passing Gas";
        public static LocString TOOLTIP = (LocString) ("This critter is emitting " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + "\n\nYuck!");
      }

      public class EXPELLING_LIQUID
      {
        public static LocString NAME = (LocString) "Expelling Waste";
        public static LocString TOOLTIP = (LocString) "This critter is doing their \"business\"";
      }

      public class DEBUGGOTO
      {
        public static LocString NAME = (LocString) "Moving to debug location";
        public static LocString TOOLTIP = (LocString) "All that obedience training paid off";
      }

      public class ATTACK_APPROACH
      {
        public static LocString NAME = (LocString) "Stalking Target";
        public static LocString TOOLTIP = (LocString) "This critter is hostile and readying to pounce!";
      }

      public class ATTACK
      {
        public static LocString NAME = (LocString) "Combat!";
        public static LocString TOOLTIP = (LocString) "This critter is on the attack!";
      }

      public class ATTACKINGENTITY
      {
        public static LocString NAME = (LocString) "Attacking";
        public static LocString TOOLTIP = (LocString) "This critter is violently defending their young";
      }

      public class PROTECTINGENTITY
      {
        public static LocString NAME = (LocString) "Protecting";
        public static LocString TOOLTIP = (LocString) "This creature is guarding something special to them and will likely attack if approached";
      }

      public class LAYINGANEGG
      {
        public static LocString NAME = (LocString) "Laying egg";
        public static LocString TOOLTIP = (LocString) "Witness the miracle of life!";
      }

      public class TENDINGANEGG
      {
        public static LocString NAME = (LocString) "Tending egg";
        public static LocString TOOLTIP = (LocString) "Nurturing the miracle of life!";
      }

      public class GROWINGUP
      {
        public static LocString NAME = (LocString) "Maturing";
        public static LocString TOOLTIP = (LocString) "This baby critter is about to reach adulthood";
      }

      public class SUFFOCATING
      {
        public static LocString NAME = (LocString) "Suffocating";
        public static LocString TOOLTIP = (LocString) "This critter cannot breathe";
      }

      public class HATCHING
      {
        public static LocString NAME = (LocString) "Hatching";
        public static LocString TOOLTIP = (LocString) "Here it comes!";
      }

      public class INCUBATING
      {
        public static LocString NAME = (LocString) "Incubating";
        public static LocString TOOLTIP = (LocString) "Cozily preparing to meet the world";
      }

      public class CONSIDERINGLURE
      {
        public static LocString NAME = (LocString) "Piqued";
        public static LocString TOOLTIP = (LocString) ("This critter is tempted to bite a nearby " + UI.PRE_KEYWORD + "Lure" + UI.PST_KEYWORD);
      }

      public class FALLING
      {
        public static LocString NAME = (LocString) "Falling";
        public static LocString TOOLTIP = (LocString) "AHHHH!";
      }

      public class FLOPPING
      {
        public static LocString NAME = (LocString) "Flopping";
        public static LocString TOOLTIP = (LocString) "Fish out of water!";
      }

      public class DRYINGOUT
      {
        public static LocString NAME = (LocString) "    • Beached";
        public static LocString TOOLTIP = (LocString) ("This plant must be submerged in " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " to grow");
      }

      public class GROWING
      {
        public static LocString NAME = (LocString) "Growing [{PercentGrow}%]";
        public static LocString TOOLTIP = (LocString) "Next harvest: <b>{TimeUntilNextHarvest}</b>";
      }

      public class CROP_SLEEPING
      {
        public static LocString NAME = (LocString) "Sleeping [{REASON}]";
        public static LocString TOOLTIP = (LocString) "Requires: {REQUIREMENTS}";
        public static LocString REQUIREMENT_LUMINANCE = (LocString) "<b>{0}</b> Lux";
        public static LocString REASON_TOO_DARK = (LocString) "Too Dark";
        public static LocString REASON_TOO_BRIGHT = (LocString) "Too Bright";
      }

      public class MOLTING
      {
        public static LocString NAME = (LocString) "Molting";
        public static LocString TOOLTIP = (LocString) "This critter is shedding its skin. Yuck";
      }

      public class CLEANING
      {
        public static LocString NAME = (LocString) "Cleaning";
        public static LocString TOOLTIP = (LocString) "This critter is de-germ-ifying its liquid surroundings";
      }

      public class NEEDSFERTILIZER
      {
        public static LocString NAME = (LocString) ("    • " + (string) CREATURES.STATS.FERTILIZATION.NAME);
        public static LocString TOOLTIP = (LocString) ("Growth will resume when " + UI.PRE_KEYWORD + "Fertilization" + UI.PST_KEYWORD + " requirements are met");
        public static LocString LINE_ITEM = (LocString) "\n            • {Resource}: {Amount}";
      }

      public class NEEDSIRRIGATION
      {
        public static LocString NAME = (LocString) ("    • " + (string) CREATURES.STATS.IRRIGATION.NAME);
        public static LocString TOOLTIP = (LocString) ("Growth will resume when " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " requirements are met");
        public static LocString LINE_ITEM = (LocString) "\n            • {Resource}: {Amount}";
      }

      public class WRONGFERTILIZER
      {
        public static LocString NAME = (LocString) ("    • " + (string) CREATURES.STATS.FERTILIZATION.NAME);
        public static LocString TOOLTIP = (LocString) ("This farm is storing materials that are not suitable for this plant\n\nEmpty this building's " + UI.PRE_KEYWORD + "Storage" + UI.PST_KEYWORD + " to remove the unusable materials");
        public static LocString LINE_ITEM = (LocString) "            • {0}: {1}\n";
      }

      public class WRONGIRRIGATION
      {
        public static LocString NAME = (LocString) ("    • " + (string) CREATURES.STATS.FERTILIZATION.NAME);
        public static LocString TOOLTIP = (LocString) "This farm is storing materials that are not suitable for this plant\n\nEmpty this building's storage to remove the unusable materials";
        public static LocString LINE_ITEM = (LocString) "            • {0}: {1}\n";
      }

      public class WRONGFERTILIZERMAJOR
      {
        public static LocString NAME = (LocString) ("    • " + (string) CREATURES.STATS.FERTILIZATION.NAME);
        public static LocString TOOLTIP = (LocString) ("This farm is storing materials that are not suitable for this plant\n\n" + UI.PRE_KEYWORD + "Empty Storage" + UI.PST_KEYWORD + " on this building to remove the unusable materials");
        public static LocString LINE_ITEM = (LocString) ("        " + (string) CREATURES.STATUSITEMS.WRONGFERTILIZER.LINE_ITEM);
      }

      public class WRONGIRRIGATIONMAJOR
      {
        public static LocString NAME = (LocString) ("    • " + (string) CREATURES.STATS.IRRIGATION.NAME);
        public static LocString TOOLTIP = (LocString) ("This farm is storing materials that are not suitable for this plant\n\n" + UI.PRE_KEYWORD + "Empty Storage" + UI.PST_KEYWORD + " on this building to remove the incorrect materials");
        public static LocString LINE_ITEM = (LocString) ("        " + (string) CREATURES.STATUSITEMS.WRONGIRRIGATION.LINE_ITEM);
      }

      public class CANTACCEPTFERTILIZER
      {
        public static LocString NAME = (LocString) ("    • " + (string) CREATURES.STATS.FERTILIZATION.NAME);
        public static LocString TOOLTIP = (LocString) ("This farm plot does not accept " + UI.PRE_KEYWORD + "Fertilizer" + UI.PST_KEYWORD + "\n\nMove the selected plant to a fertilization capable plot for optimal growth");
      }

      public class CANTACCEPTIRRIGATION
      {
        public static LocString NAME = (LocString) ("    • " + (string) CREATURES.STATS.IRRIGATION.NAME);
        public static LocString TOOLTIP = (LocString) ("This farm plot does not accept " + UI.PRE_KEYWORD + "Irrigation" + UI.PST_KEYWORD + "\n\nMove the selected plant to an irrigation capable plot for optimal growth");
      }

      public class READYFORHARVEST
      {
        public static LocString NAME = (LocString) "Harvest Ready";
        public static LocString TOOLTIP = (LocString) "This plant can be harvested for materials";
      }

      public class LOW_YIELD
      {
        public static LocString NAME = (LocString) "Standard Yield";
        public static LocString TOOLTIP = (LocString) "This plant produced an average yield";
      }

      public class NORMAL_YIELD
      {
        public static LocString NAME = (LocString) "Good Yield";
        public static LocString TOOLTIP = (LocString) "Comfortable conditions allowed this plant to produce a better yield\n{Effects}";
        public static LocString LINE_ITEM = (LocString) "    • {0}\n";
      }

      public class HIGH_YIELD
      {
        public static LocString NAME = (LocString) "Excellent Yield";
        public static LocString TOOLTIP = (LocString) "Consistently ideal conditions allowed this plant to bear a large yield\n{Effects}";
        public static LocString LINE_ITEM = (LocString) "    • {0}\n";
      }

      public class ENTOMBED
      {
        public static LocString NAME = (LocString) "Entombed";
        public static LocString TOOLTIP = (LocString) "This {0} is trapped and needs help digging out";
        public static LocString LINE_ITEM = (LocString) "    • Entombed";
      }

      public class DROWNING
      {
        public static LocString NAME = (LocString) "Drowning";
        public static LocString TOOLTIP = (LocString) ("This critter can't breathe in " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + "!");
      }

      public class DISABLED
      {
        public static LocString NAME = (LocString) "Disabled";
        public static LocString TOOLTIP = (LocString) "Something is preventing this critter from functioning!";
      }

      public class SATURATED
      {
        public static LocString NAME = (LocString) "Too Wet!";
        public static LocString TOOLTIP = (LocString) ("This critter likes " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + ", but not that much!");
      }

      public class WILTING
      {
        public static LocString NAME = (LocString) "Growth Halted{Reasons}";
        public static LocString TOOLTIP = (LocString) "Growth will resume when conditions improve";
      }

      public class WILTINGDOMESTIC
      {
        public static LocString NAME = (LocString) "Growth Halted{Reasons}";
        public static LocString TOOLTIP = (LocString) "Growth will resume when conditions improve";
      }

      public class WILTING_NON_GROWING_PLANT
      {
        public static LocString NAME = (LocString) "Growth Halted{Reasons}";
        public static LocString TOOLTIP = (LocString) "Growth will resume when conditions improve";
      }

      public class BARREN
      {
        public static LocString NAME = (LocString) "Barren";
        public static LocString TOOLTIP = (LocString) ("This plant will produce no more " + UI.PRE_KEYWORD + "Seeds" + UI.PST_KEYWORD);
      }

      public class ATMOSPHERICPRESSURETOOLOW
      {
        public static LocString NAME = (LocString) "    • Pressure";
        public static LocString TOOLTIP = (LocString) "Growth will resume when air pressure is between <b>{low_mass}</b> and <b>{high_mass}</b>";
      }

      public class WRONGATMOSPHERE
      {
        public static LocString NAME = (LocString) "    • Atmosphere";
        public static LocString TOOLTIP = (LocString) ("Growth will resume when submersed in one of the following " + UI.PRE_KEYWORD + "Gases" + UI.PST_KEYWORD + ": {elements}");
      }

      public class ATMOSPHERICPRESSURETOOHIGH
      {
        public static LocString NAME = (LocString) "    • Pressure";
        public static LocString TOOLTIP = (LocString) "Growth will resume when air pressure is between <b>{low_mass}</b> and <b>{high_mass}</b>";
      }

      public class PERFECTATMOSPHERICPRESSURE
      {
        public static LocString NAME = (LocString) "Ideal Air Pressure";
        public static LocString TOOLTIP = (LocString) "This critter is comfortable in the current atmospheric pressure\n\nIdeal Range: <b>{0}</b> - <b>{1}</b>";
      }

      public class HEALTHSTATUS
      {
        public static LocString NAME = (LocString) "Injuries: {healthState}";
        public static LocString TOOLTIP = (LocString) "Current physical status: {healthState}";
      }

      public class FLEEING
      {
        public static LocString NAME = (LocString) "Fleeing";
        public static LocString TOOLTIP = (LocString) "This critter is trying to escape\nGet'em!";
      }

      public class REFRIGERATEDFROZEN
      {
        public static LocString NAME = (LocString) "Deep Freeze";
        public static LocString TOOLTIP = (LocString) (UI.PRE_KEYWORD + "Temperatures" + UI.PST_KEYWORD + " below <b>{PreserveTemperature}</b> are greatly prolonging the shelf-life of this food\n\n" + UI.PRE_KEYWORD + "Temperatures" + UI.PST_KEYWORD + " above <b>{RotTemperature}</b> spoil food more quickly");
      }

      public class REFRIGERATED
      {
        public static LocString NAME = (LocString) "Refrigerated";
        public static LocString TOOLTIP = (LocString) ("Ideal " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " storage is slowing this food's " + UI.PRE_KEYWORD + "Decay Rate" + UI.PST_KEYWORD + "\n\n" + UI.PRE_KEYWORD + "Temperatures" + UI.PST_KEYWORD + " above <b>{RotTemperature}</b> spoil food more quickly\n\nStore food below {PreserveTemperature} to further reduce spoilage.");
      }

      public class UNREFRIGERATED
      {
        public static LocString NAME = (LocString) "Unrefrigerated";
        public static LocString TOOLTIP = (LocString) ("This food is warm\n\n" + UI.PRE_KEYWORD + "Temperatures" + UI.PST_KEYWORD + " above <b>{RotTemperature}</b> spoil food more quickly");
      }

      public class CONTAMINATEDATMOSPHERE
      {
        public static LocString NAME = (LocString) "Pollution Exposure";
        public static LocString TOOLTIP = (LocString) ("Exposure to contaminants is accelerating this food's " + UI.PRE_KEYWORD + "Decay Rate" + UI.PST_KEYWORD);
      }

      public class STERILIZINGATMOSPHERE
      {
        public static LocString NAME = (LocString) "Sterile Atmosphere";
        public static LocString TOOLTIP = (LocString) ("Microbe destroying conditions have decreased this food's " + UI.PRE_KEYWORD + "Decay Rate" + UI.PST_KEYWORD);
      }

      public class EXCHANGINGELEMENTCONSUME
      {
        public static LocString NAME = (LocString) "Consuming {ConsumeElement} at {ConsumeRate}";
        public static LocString TOOLTIP = (LocString) ("{ConsumeElement} is being used at a rate of " + UI.FormatAsNegativeRate("{ConsumeRate}"));
      }

      public class EXCHANGINGELEMENTOUTPUT
      {
        public static LocString NAME = (LocString) "Outputting {OutputElement} at {OutputRate}";
        public static LocString TOOLTIP = (LocString) ("{OutputElement} is being expelled at a rate of " + UI.FormatAsPositiveRate("{OutputRate}"));
      }

      public class FRESH
      {
        public static LocString NAME = (LocString) "Fresh {RotPercentage}";
        public static LocString TOOLTIP = (LocString) "Get'em while they're hot!\n\n{RotTooltip}";
      }

      public class STALE
      {
        public static LocString NAME = (LocString) "Stale {RotPercentage}";
        public static LocString TOOLTIP = (LocString) ("This " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + " is still edible but will soon expire\n{RotTooltip}");
      }

      public class SPOILED
      {
        public static LocString NAME = (LocString) "Rotten";
        public static LocString TOOLTIP = (LocString) ("This " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + " has putrefied and should not be consumed");
      }

      public class STUNTED_SCALE_GROWTH
      {
        public static LocString NAME = (LocString) "Stunted Scales";
        public static LocString TOOLTIP = (LocString) ("This critter's " + UI.PRE_KEYWORD + "Scale Growth" + UI.PST_KEYWORD + " is being stunted by an unfavorable environment");
      }

      public class RECEPTACLEINOPERATIONAL
      {
        public static LocString NAME = (LocString) "    • Farm plot inoperable";
        public static LocString TOOLTIP = (LocString) ("This farm plot cannot grow " + UI.PRE_KEYWORD + "Plants" + UI.PST_KEYWORD + " in its current state");
      }

      public class TRAPPED
      {
        public static LocString NAME = (LocString) "Trapped";
        public static LocString TOOLTIP = (LocString) "This critter has been contained and cannot escape";
      }

      public class EXHALING
      {
        public static LocString NAME = (LocString) "Exhaling";
        public static LocString TOOLTIP = (LocString) ("This critter is expelling " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " from its lungsacs");
      }

      public class INHALING
      {
        public static LocString NAME = (LocString) "Inhaling";
        public static LocString TOOLTIP = (LocString) "This critter is taking a deep breath";
      }

      public class EXTERNALTEMPERATURE
      {
        public static LocString NAME = (LocString) "External Temperature";
        public static LocString TOOLTIP = (LocString) "External Temperature\n\nThis critter's environment is {0}";
      }

      public class RECEPTACLEOPERATIONAL
      {
        public static LocString NAME = (LocString) "Farm plot operational";
        public static LocString TOOLTIP = (LocString) "This plant's farm plot is operational";
      }

      public class DOMESTICATION
      {
        public static LocString NAME = (LocString) "Domestication Level: {LevelName}";
        public static LocString TOOLTIP = (LocString) "{LevelDesc}";
      }

      public class HUNGRY
      {
        public static LocString NAME = (LocString) "Hungry";
        public static LocString TOOLTIP = (LocString) "This critter's tummy is rumbling";
      }

      public class HIVEHUNGRY
      {
        public static LocString NAME = (LocString) "Food Supply Low";
        public static LocString TOOLTIP = (LocString) "The food reserves in this hive are running low";
      }

      public class STARVING
      {
        public static LocString NAME = (LocString) "Starving\nTime until death: {TimeUntilDeath}\n";
        public static LocString TOOLTIP = (LocString) "This critter is starving and will die if it is not fed soon";
        public static LocString NOTIFICATION_NAME = (LocString) "Critter Starvation";
        public static LocString NOTIFICATION_TOOLTIP = (LocString) "These critters are starving and will die if not fed soon:";
      }

      public class OLD
      {
        public static LocString NAME = (LocString) "Elderly";
        public static LocString TOOLTIP = (LocString) "This sweet ol'critter is over the hill and will pass on in <b>{TimeUntilDeath}</b>";
      }

      public class DIVERGENT_WILL_TEND
      {
        public static LocString NAME = (LocString) "Moving to Plant";
        public static LocString TOOLTIP = (LocString) "This critter is off to tend a plant that's caught its attention";
      }

      public class DIVERGENT_TENDING
      {
        public static LocString NAME = (LocString) "Plant Tending";
        public static LocString TOOLTIP = (LocString) "This critter is snuggling a plant to help it grow";
      }

      public class NOSLEEPSPOT
      {
        public static LocString NAME = (LocString) "Nowhere To Sleep";
        public static LocString TOOLTIP = (LocString) "This critter wants to sleep but can't find a good spot to snuggle up!";
      }

      public class PILOTNEEDED
      {
      }

      public class ORIGINALPLANTMUTATION
      {
        public static LocString NAME = (LocString) "Original Plant";
        public static LocString TOOLTIP = (LocString) "This is the original, unmutated variant of this species.";
      }

      public class UNKNOWNMUTATION
      {
        public static LocString NAME = (LocString) "Unknown Mutation";
        public static LocString TOOLTIP = (LocString) ("This seed carries some unexpected genetic markers. Analyze it at the " + UI.FormatAsLink((string) BUILDINGS.PREFABS.GENETICANALYSISSTATION.NAME, "GENETICANALYSISSTATION") + " to learn its secrets.");
      }

      public class SPECIFICPLANTMUTATION
      {
        public static LocString NAME = (LocString) "Mutant Plant: {MutationName}";
        public static LocString TOOLTIP = (LocString) "This plant is mutated with a genetic variant I call {MutationName}.";
      }

      public class CROP_TOO_NONRADIATED
      {
        public static LocString NAME = (LocString) "    • Low Radiation Levels";
        public static LocString TOOLTIP = (LocString) ("Growth will resume when " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " requirements are met");
      }

      public class CROP_TOO_RADIATED
      {
        public static LocString NAME = (LocString) "    • High Radiation Levels";
        public static LocString TOOLTIP = (LocString) ("Growth will resume when " + UI.PRE_KEYWORD + "Radiation" + UI.PST_KEYWORD + " requirements are met");
      }

      public class ELEMENT_GROWTH_GROWING
      {
        public static LocString NAME = (LocString) "Picky Eater: Just Right";
        public static LocString TOOLTIP = (LocString) ("This critter's " + UI.PRE_KEYWORD + "Quill Growth" + UI.PST_KEYWORD + " rate is optimal\n\nPreferred food temperature range: {templo}-{temphi}");
        public static LocString PREFERRED_TEMP = (LocString) "Last eaten: {element} at {temperature}";
      }

      public class ELEMENT_GROWTH_STUNTED
      {
        public static LocString NAME = (LocString) "Picky Eater: {reason}";
        public static LocString TOO_HOT = (LocString) "Too Hot";
        public static LocString TOO_COLD = (LocString) "Too Cold";
        public static LocString TOOLTIP = (LocString) ("This critter's " + UI.PRE_KEYWORD + "Quill Growth" + UI.PST_KEYWORD + " rate has slowed because they ate food outside their preferred temperature range\n\nPreferred food temperature range: {templo}-{temphi}");
      }

      public class ELEMENT_GROWTH_HALTED
      {
        public static LocString NAME = (LocString) "Picky Eater: Hungry";
        public static LocString TOOLTIP = (LocString) ("This critter's " + UI.PRE_KEYWORD + "Quill Growth" + UI.PST_KEYWORD + " is halted because they are hungry\n\nPreferred food temperature range: {templo}-{temphi}");
      }

      public class ELEMENT_GROWTH_COMPLETE
      {
        public static LocString NAME = (LocString) "Picky Eater: All Done";
        public static LocString TOOLTIP = (LocString) ("This critter's " + UI.PRE_KEYWORD + "Tonic Root" + UI.PST_KEYWORD + " quills are fully grown\n\nPreferred food temperature range: {templo}-{temphi}");
      }

      public class GRAVITAS_CREATURE_MANIPULATOR_COOLDOWN
      {
        public static LocString NAME = (LocString) "Processing Sample: {percent}";
        public static LocString TOOLTIP = (LocString) "This building is busy analyzing genetic data from a recently scanned specimen\n\nRemaining: {timeleft}";
      }
    }

    public class STATS
    {
      public class HEALTH
      {
        public static LocString NAME = (LocString) "Health";
      }

      public class AGE
      {
        public static LocString NAME = (LocString) "Age";
        public static LocString TOOLTIP = (LocString) ("This critter will die when its " + UI.PRE_KEYWORD + "Age" + UI.PST_KEYWORD + " reaches its species' maximum lifespan");
      }

      public class MATURITY
      {
        public static LocString NAME = (LocString) "Growth Progress";
        public static LocString TOOLTIP = (LocString) "Growth Progress\n\n";
        public static LocString TOOLTIP_GROWING = (LocString) "Predicted Maturation: <b>{0}</b>";
        public static LocString TOOLTIP_GROWING_CROP = (LocString) "Predicted Maturation Time: <b>{0}</b>\nNext harvest occurs in approximately <b>{1}</b>";
        public static LocString TOOLTIP_GROWN = (LocString) "Growth paused while plant awaits harvest";
        public static LocString TOOLTIP_STALLED = (LocString) "Poor conditions have halted this plant's growth";
        public static LocString AMOUNT_DESC_FMT = (LocString) "{0}: {1}\nNext harvest in <b>{2}</b>";
        public static LocString GROWING = (LocString) "Domestic Growth Rate";
        public static LocString GROWINGWILD = (LocString) "Wild Growth Rate";
      }

      public class FERTILIZATION
      {
        public static LocString NAME = (LocString) "Fertilization";
        public static LocString CONSUME_MODIFIER = (LocString) "Consuming";
        public static LocString ABSORBING_MODIFIER = (LocString) "Absorbing";
      }

      public class DOMESTICATION
      {
        public static LocString NAME = (LocString) "Domestication";
        public static LocString TOOLTIP = (LocString) ("Fully " + UI.PRE_KEYWORD + "Tame" + UI.PST_KEYWORD + " critters produce more materials than wild ones, and may even provide psychological benefits to my colony\n\nThis critter is <b>{0}</b> domesticated");
      }

      public class HAPPINESS
      {
        public static LocString NAME = (LocString) "Happiness";
        public static LocString TOOLTIP = (LocString) ("High " + UI.PRE_KEYWORD + "Happiness" + UI.PST_KEYWORD + " increases a critter's productivity and indirectly improves their " + UI.PRE_KEYWORD + "Egg" + UI.PST_KEYWORD + " laying rates\n\nIt also provides the satisfaction in knowing they're living a good little critter life");
      }

      public class WILDNESS
      {
        public static LocString NAME = (LocString) "Wildness";
        public static LocString TOOLTIP = (LocString) ("At 0% " + UI.PRE_KEYWORD + "Wildness" + UI.PST_KEYWORD + " a critter becomes " + UI.PRE_KEYWORD + "Tame" + UI.PST_KEYWORD + ", increasing its " + UI.PRE_KEYWORD + "Metabolism" + UI.PST_KEYWORD + " and requiring regular care from Duplicants\n\nDuplicants must possess the " + UI.PRE_KEYWORD + "Critter Ranching" + UI.PST_KEYWORD + " Skill to care for critters");
      }

      public class FERTILITY
      {
        public static LocString NAME = (LocString) "Reproduction";
        public static LocString TOOLTIP = (LocString) ("At 100% " + UI.PRE_KEYWORD + "Reproduction" + UI.PST_KEYWORD + ", critters will reach the end of their reproduction cycle and lay a new " + UI.PRE_KEYWORD + "Egg" + UI.PST_KEYWORD + "\n\nAfter an " + UI.PRE_KEYWORD + "Egg" + UI.PST_KEYWORD + " is laid, " + UI.PRE_KEYWORD + "Reproduction" + UI.PST_KEYWORD + " is rolled back to 0%");
      }

      public class INCUBATION
      {
        public static LocString NAME = (LocString) "Incubation";
        public static LocString TOOLTIP = (LocString) ("Eggs hatch into brand new " + UI.FormatAsLink("Critters", nameof (CREATURES)) + " at the end of their incubation period");
      }

      public class VIABILITY
      {
        public static LocString NAME = (LocString) "Viability";
        public static LocString TOOLTIP = (LocString) ("Eggs will lose " + UI.PRE_KEYWORD + "Viability" + UI.PST_KEYWORD + " over time when exposed to poor environmental conditions\n\nAt 0% " + UI.PRE_KEYWORD + "Viability" + UI.PST_KEYWORD + " a critter egg will crack and produce a " + (string) ITEMS.FOOD.RAWEGG.NAME + " and " + (string) ITEMS.INDUSTRIAL_PRODUCTS.EGG_SHELL.NAME);
      }

      public class IRRIGATION
      {
        public static LocString NAME = (LocString) "Irrigation";
        public static LocString CONSUME_MODIFIER = (LocString) "Consuming";
        public static LocString ABSORBING_MODIFIER = (LocString) "Absorbing";
      }

      public class ILLUMINATION
      {
        public static LocString NAME = (LocString) "Illumination";
      }

      public class THERMALCONDUCTIVITYBARRIER
      {
        public static LocString NAME = (LocString) "Thermal Conductivity Barrier";
        public static LocString TOOLTIP = (LocString) ("Thick " + UI.PRE_KEYWORD + "Conductivity Barriers" + UI.PST_KEYWORD + " increase the time it takes an object to heat up or cool down");
      }

      public class ROT
      {
        public static LocString NAME = (LocString) "Freshness";
        public static LocString TOOLTIP = (LocString) ("Food items become stale at fifty percent " + UI.PRE_KEYWORD + "Freshness" + UI.PST_KEYWORD + ", and rot at zero percent");
      }

      public class SCALEGROWTH
      {
        public static LocString NAME = (LocString) "Scale Growth";
        public static LocString TOOLTIP = (LocString) "The amount of time required for this critter to regrow its scales";
      }

      public class ELEMENTGROWTH
      {
        public static LocString NAME = (LocString) "Quill Growth";
        public static LocString TOOLTIP = (LocString) ("The amount of time required for this critter to regrow its " + UI.PRE_KEYWORD + "Tonic Root" + UI.PST_KEYWORD);
      }

      public class AIRPRESSURE
      {
        public static LocString NAME = (LocString) "Air Pressure";
        public static LocString TOOLTIP = (LocString) ("The average " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " density of the air surrounding this plant");
      }
    }

    public class ATTRIBUTES
    {
      public class INCUBATIONDELTA
      {
        public static LocString NAME = (LocString) "Incubation Rate";
        public static LocString DESC = (LocString) "";
      }

      public class POWERCHARGEDELTA
      {
        public static LocString NAME = (LocString) "Power Charge Loss Rate";
        public static LocString DESC = (LocString) "";
      }

      public class VIABILITYDELTA
      {
        public static LocString NAME = (LocString) "Viability Loss Rate";
        public static LocString DESC = (LocString) "";
      }

      public class SCALEGROWTHDELTA
      {
        public static LocString NAME = (LocString) "Scale Growth";
        public static LocString TOOLTIP = (LocString) ("Determines how long this " + UI.PRE_KEYWORD + "Critter's" + UI.PST_KEYWORD + " scales will take to grow back.");
      }

      public class WILDNESSDELTA
      {
        public static LocString NAME = (LocString) "Wildness";
        public static LocString DESC = (LocString) ("Wild creatures can survive on fewer " + UI.PRE_KEYWORD + "Calories" + UI.PST_KEYWORD + " than domesticated ones.");
      }

      public class FERTILITYDELTA
      {
        public static LocString NAME = (LocString) "Reproduction Rate";
        public static LocString DESC = (LocString) ("Determines the amount of time needed for a " + UI.PRE_KEYWORD + "Critter" + UI.PST_KEYWORD + " to lay new " + UI.PRE_KEYWORD + "Eggs" + UI.PST_KEYWORD + ".");
      }

      public class MATURITYDELTA
      {
        public static LocString NAME = (LocString) "Growth Speed";
        public static LocString DESC = (LocString) "Determines the amount of time needed to reach maturation.";
      }

      public class MATURITYMAX
      {
        public static LocString NAME = (LocString) "Life Cycle";
        public static LocString DESC = (LocString) "The amount of time it takes this plant to grow.";
      }

      public class ROTDELTA
      {
        public static LocString NAME = (LocString) "Freshness";
        public static LocString TOOLTIP = (LocString) ("Food items become stale at fifty percent " + UI.PRE_KEYWORD + "Freshness" + UI.PST_KEYWORD + ", and rot at zero percent");
      }
    }

    public class MODIFIERS
    {
      public class DOMESTICATION_INCREASING
      {
        public static LocString NAME = (LocString) "Happiness Increasing";
        public static LocString TOOLTIP = (LocString) "This critter is very happy its needs are being met";
      }

      public class DOMESTICATION_DECREASING
      {
        public static LocString NAME = (LocString) "Happiness Decreasing";
        public static LocString TOOLTIP = (LocString) "Unfavorable conditions are making this critter unhappy";
      }

      public class BASE_FERTILITY
      {
        public static LocString NAME = (LocString) "Base Reproduction";
        public static LocString TOOLTIP = (LocString) ("This is the base speed with which critters produce new " + UI.PRE_KEYWORD + "Eggs" + UI.PST_KEYWORD);
      }

      public class BASE_INCUBATION_RATE
      {
        public static LocString NAME = (LocString) "Base Incubation Rate";
      }

      public class SCALE_GROWTH_RATE
      {
        public static LocString NAME = (LocString) "Scale Regrowth Rate";
      }

      public class ELEMENT_GROWTH_RATE
      {
        public static LocString NAME = (LocString) "Quill Regrowth Rate";
      }

      public class INCUBATOR_SONG
      {
        public static LocString NAME = (LocString) "Lullabied";
        public static LocString TOOLTIP = (LocString) ("This egg was recently sung to by a kind Duplicant\n\nIncreased " + UI.PRE_KEYWORD + "Incubation Rate" + UI.PST_KEYWORD + "\n\nDuplicants must possess the " + UI.PRE_KEYWORD + "Critter Ranching" + UI.PST_KEYWORD + " Skill to sing to eggs");
      }

      public class EGGHUG
      {
        public static LocString NAME = (LocString) "Cuddled";
        public static LocString TOOLTIP = (LocString) ("This egg was recently hugged by an affectionate critter\n\nIncreased " + UI.PRE_KEYWORD + "Incubation Rate" + UI.PST_KEYWORD);
      }

      public class HUGGINGFRENZY
      {
        public static LocString NAME = (LocString) "Hugging Spree";
        public static LocString TOOLTIP = (LocString) ("This critter was recently hugged by a Duplicant and is feeling extra affectionate\n\nWhile in this state, it hugs " + UI.PRE_KEYWORD + "Eggs" + UI.PST_KEYWORD + " more frequently");
      }

      public class INCUBATING
      {
        public static LocString NAME = (LocString) "Incubating";
        public static LocString TOOLTIP = (LocString) "This egg is happily incubating";
      }

      public class INCUBATING_SUPPRESSED
      {
        public static LocString NAME = (LocString) "Growth Suppressed";
        public static LocString TOOLTIP = (LocString) "Environmental conditions are preventing this egg from developing\n\nIt will not hatch if current conditions continue";
      }

      public class RANCHED
      {
        public static LocString NAME = (LocString) "Groomed";
        public static LocString TOOLTIP = (LocString) ("This critter has recently been attended to by a kind Duplicant\n\nDuplicants must possess the " + UI.PRE_KEYWORD + "Critter Ranching" + UI.PST_KEYWORD + " Skill to care for critters");
      }

      public class HAPPY
      {
        public static LocString NAME = (LocString) "Happy";
        public static LocString TOOLTIP = (LocString) "This critter's in high spirits because all of its needs are being met\n\nIt will produce more materials as a result";
      }

      public class UNHAPPY
      {
        public static LocString NAME = (LocString) "Glum";
        public static LocString TOOLTIP = (LocString) "This critter's feeling down because its needs aren't being met\n\nIt will produce less materials as a result";
      }

      public class ATE_FROM_FEEDER
      {
        public static LocString NAME = (LocString) "Ate From Feeder";
        public static LocString TOOLTIP = (LocString) ("This critter is getting more " + UI.PRE_KEYWORD + "Tame" + UI.PST_KEYWORD + " because it ate from a feeder.");
      }

      public class WILD
      {
        public static LocString NAME = (LocString) "Wild";
        public static LocString TOOLTIP = (LocString) "This critter is wild";
      }

      public class AGE
      {
        public static LocString NAME = (LocString) "Aging";
        public static LocString TOOLTIP = (LocString) "Time takes its toll on all things";
      }

      public class BABY
      {
        public static LocString NAME = (LocString) "Tiny Baby!";
        public static LocString TOOLTIP = (LocString) "This critter will grow into an adult as it ages and becomes wise to the ways of the world";
      }

      public class TAME
      {
        public static LocString NAME = (LocString) "Tame";
        public static LocString TOOLTIP = (LocString) ("This critter is " + UI.PRE_KEYWORD + "Tame" + UI.PST_KEYWORD);
      }

      public class OUT_OF_CALORIES
      {
        public static LocString NAME = (LocString) "Starving";
        public static LocString TOOLTIP = (LocString) "Get this critter something to eat!";
      }

      public class FUTURE_OVERCROWDED
      {
        public static LocString NAME = (LocString) "Cramped";
        public static LocString TOOLTIP = (LocString) ("This " + UI.PRE_KEYWORD + "Room" + UI.PST_KEYWORD + " will become overcrowded once all nearby " + UI.PRE_KEYWORD + "Eggs" + UI.PST_KEYWORD + " hatch\n\nThe selected critter has slowed its " + UI.PRE_KEYWORD + "Reproduction" + UI.PST_KEYWORD + " to prevent further overpopulation");
      }

      public class OVERCROWDED
      {
        public static LocString NAME = (LocString) "Overcrowded";
        public static LocString TOOLTIP = (LocString) ("This critter isn't comfortable with so many other critters in a " + UI.PRE_KEYWORD + "Room" + UI.PST_KEYWORD + " of this size");
        public static LocString FISHTOOLTIP = (LocString) "This critter is uncomfortable with either the size of this pool, or the number of other critters sharing it";
      }

      public class CONFINED
      {
        public static LocString NAME = (LocString) "Confined";
        public static LocString TOOLTIP = (LocString) "This critter is trapped inside a door, tile, or confined space\n\nSounds uncomfortable!";
      }

      public class DIVERGENTPLANTTENDED
      {
        public static LocString NAME = (LocString) "Sweetle Tending";
        public static LocString TOOLTIP = (LocString) ("A " + (string) CREATURES.SPECIES.DIVERGENT.VARIANT_BEETLE.NAME + " rubbed against this " + UI.PRE_KEYWORD + "Plant" + UI.PST_KEYWORD + " for a tiny growth boost");
      }

      public class DIVERGENTPLANTTENDEDWORM
      {
        public static LocString NAME = (LocString) "Grubgrub Rub";
        public static LocString TOOLTIP = (LocString) ("A " + (string) CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.NAME + " rubbed against this " + UI.PRE_KEYWORD + "Plant" + UI.PST_KEYWORD + ", dramatically boosting growth");
      }
    }

    public class FERTILITY_MODIFIERS
    {
      public class DIET
      {
        public static LocString NAME = (LocString) "Diet";
        public static LocString DESC = (LocString) "Eats: {0}";
      }

      public class NEARBY_CREATURE
      {
        public static LocString NAME = (LocString) "Nearby Critters";
        public static LocString DESC = (LocString) "Penned with: {0}";
      }

      public class NEARBY_CREATURE_NEG
      {
        public static LocString NAME = (LocString) "No Nearby Critters";
        public static LocString DESC = (LocString) "Not penned with: {0}";
      }

      public class TEMPERATURE
      {
        public static LocString NAME = (LocString) "Temperature";
        public static LocString DESC = (LocString) "Body temperature: Between {0} and {1}";
      }

      public class CROPTENDING
      {
        public static LocString NAME = (LocString) "Crop Tending";
        public static LocString DESC = (LocString) "Tends to: {0}";
      }

      public class LIVING_IN_ELEMENT
      {
        public static LocString NAME = (LocString) "Habitat";
        public static LocString DESC = (LocString) "Dwells in {0}";
        public static LocString UNBREATHABLE = (LocString) ("Dwells in unbreathable" + UI.FormatAsLink("Gas", nameof (UNBREATHABLE)));
        public static LocString LIQUID = (LocString) ("Dwells in " + UI.FormatAsLink("Liquid", nameof (LIQUID)));
      }
    }
  }
}
