// Decompiled with JetBrains decompiler
// Type: STRINGS.ITEMS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace STRINGS
{
  public class ITEMS
  {
    public class PILLS
    {
      public class PLACEBO
      {
        public static LocString NAME = (LocString) "Placebo";
        public static LocString DESC = (LocString) ("A general, all-purpose " + UI.FormatAsLink("Medicine", "MEDICINE") + ".\n\nThe less one knows about it, the better it works.");
        public static LocString RECIPEDESC = (LocString) ("All-purpose " + UI.FormatAsLink("Medicine", "MEDICINE") + ".");
      }

      public class BASICBOOSTER
      {
        public static LocString NAME = (LocString) "Vitamin Chews";
        public static LocString DESC = (LocString) "Minorly reduces the chance of becoming sick.";
        public static LocString RECIPEDESC = (LocString) ("A supplement that minorly reduces the chance of contracting a " + UI.PRE_KEYWORD + "Germ" + UI.PST_KEYWORD + "-based " + UI.FormatAsLink("Disease", "DISEASE") + ".\n\nMust be taken daily.");
      }

      public class INTERMEDIATEBOOSTER
      {
        public static LocString NAME = (LocString) "Immuno Booster";
        public static LocString DESC = (LocString) "Significantly reduces the chance of becoming sick.";
        public static LocString RECIPEDESC = (LocString) ("A supplement that significantly reduces the chance of contracting a " + UI.PRE_KEYWORD + "Germ" + UI.PST_KEYWORD + "-based " + UI.FormatAsLink("Disease", "DISEASE") + ".\n\nMust be taken daily.");
      }

      public class ANTIHISTAMINE
      {
        public static LocString NAME = (LocString) "Allergy Medication";
        public static LocString DESC = (LocString) "Suppresses and prevents allergic reactions.";
        public static LocString RECIPEDESC = (LocString) ("A strong antihistamine Duplicants can take to halt an allergic reaction. " + (string) ITEMS.PILLS.ANTIHISTAMINE.NAME + " will also prevent further reactions from occurring for a short time after ingestion.");
      }

      public class BASICCURE
      {
        public static LocString NAME = (LocString) "Curative Tablet";
        public static LocString DESC = (LocString) "A simple, easy-to-take remedy for minor germ-based diseases.";
        public static LocString RECIPEDESC = (LocString) ("Duplicants can take this to cure themselves of minor " + UI.PRE_KEYWORD + "Germ" + UI.PST_KEYWORD + "-based " + UI.FormatAsLink("Diseases", "DISEASE") + ".\n\nCurative Tablets are very effective against " + UI.FormatAsLink("Food Poisoning", "FOODSICKNESS") + ".");
      }

      public class INTERMEDIATECURE
      {
        public static LocString NAME = (LocString) "Medical Pack";
        public static LocString DESC = (LocString) "A doctor-administered cure for moderate ailments.";
        public static LocString RECIPEDESC = (LocString) ("A doctor-administered cure for moderate " + UI.FormatAsLink("Diseases", "DISEASE") + ". " + (string) ITEMS.PILLS.INTERMEDIATECURE.NAME + "s are very effective against " + UI.FormatAsLink("Slimelung", "SLIMESICKNESS") + ".\n\nMust be administered by a Duplicant with the " + (string) DUPLICANTS.ROLES.MEDIC.NAME + " Skill.");
      }

      public class ADVANCEDCURE
      {
        public static LocString NAME = (LocString) "Serum Vial";
        public static LocString DESC = (LocString) "A doctor-administered cure for severe ailments.";
        public static LocString RECIPEDESC = (LocString) ("An extremely powerful medication created to treat severe " + UI.FormatAsLink("Diseases", "DISEASE") + ". " + (string) ITEMS.PILLS.ADVANCEDCURE.NAME + " is very effective against " + UI.FormatAsLink("Zombie Spores", "ZOMBIESPORES") + ".\n\nMust be administered by a Duplicant with the " + (string) DUPLICANTS.ROLES.SENIOR_MEDIC.NAME + " Skill.");
      }

      public class BASICRADPILL
      {
        public static LocString NAME = (LocString) "Basic Rad Pill";
        public static LocString DESC = (LocString) "Increases a Duplicant's natural radiation absorption rate.";
        public static LocString RECIPEDESC = (LocString) "A supplement that speeds up the rate at which a Duplicant body absorbs radiation, allowing them to manage increased radiation exposure.\n\nMust be taken daily.";
      }

      public class INTERMEDIATERADPILL
      {
        public static LocString NAME = (LocString) "Intermediate Rad Pill";
        public static LocString DESC = (LocString) "Increases a Duplicant's natural radiation absorption rate.";
        public static LocString RECIPEDESC = (LocString) "A supplement that speeds up the rate at which a Duplicant body absorbs radiation, allowing them to manage increased radiation exposure.\n\nMust be taken daily.";
      }
    }

    public class FOOD
    {
      public static LocString COMPOST = (LocString) "Compost";

      public class FOODSPLAT
      {
        public static LocString NAME = (LocString) "Food Splatter";
        public static LocString DESC = (LocString) "Food smeared on the wall from a recent Food Fight";
      }

      public class BURGER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Frost Burger", nameof (BURGER));
        public static LocString DESC = (LocString) (UI.FormatAsLink("Meat", "MEAT") + " and " + UI.FormatAsLink("Lettuce", "LETTUCE") + " on a chilled " + UI.FormatAsLink("Frost Bun", "COLDWHEATBREAD") + ".\n\nIt's the only burger best served cold.");
        public static LocString RECIPEDESC = (LocString) (UI.FormatAsLink("Meat", "MEAT") + " and " + UI.FormatAsLink("Lettuce", "LETTUCE") + " on a chilled " + UI.FormatAsLink("Frost Bun", "COLDWHEATBREAD") + ".");
      }

      public class FIELDRATION
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Nutrient Bar", nameof (FIELDRATION));
        public static LocString DESC = (LocString) "A nourishing nutrient paste, sandwiched between thin wafer layers.";
      }

      public class MUSHBAR
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Mush Bar", nameof (MUSHBAR));
        public static LocString DESC = (LocString) "An edible, putrefied mudslop.\n\nMush Bars are preferable to starvation, but only just barely.";
        public static LocString RECIPEDESC = (LocString) ("An edible, putrefied mudslop.\n\n" + (string) ITEMS.FOOD.MUSHBAR.NAME + "s are preferable to starvation, but only just barely.");
      }

      public class MUSHROOMWRAP
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Mushroom Wrap", nameof (MUSHROOMWRAP));
        public static LocString DESC = (LocString) ("Flavorful " + UI.FormatAsLink("Mushrooms", "MUSHROOM") + " wrapped in " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".\n\nIt has an earthy flavor punctuated by a refreshing crunch.");
        public static LocString RECIPEDESC = (LocString) ("Flavorful " + UI.FormatAsLink("Mushrooms", "MUSHROOM") + " wrapped in " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".");
      }

      public class MICROWAVEDLETTUCE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Microwaved Lettuce", nameof (MICROWAVEDLETTUCE));
        public static LocString DESC = (LocString) (UI.FormatAsLink("Lettuce", "LETTUCE") + " scrumptiously wilted in the " + (string) BUILDINGS.PREFABS.GAMMARAYOVEN.NAME + ".");
        public static LocString RECIPEDESC = (LocString) (UI.FormatAsLink("Lettuce", "LETTUCE") + " scrumptiously wilted in the " + (string) BUILDINGS.PREFABS.GAMMARAYOVEN.NAME + ".");
      }

      public class GAMMAMUSH
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Gamma Mush", nameof (GAMMAMUSH));
        public static LocString DESC = (LocString) "A disturbingly delicious mixture of irradiated dirt and water.";
        public static LocString RECIPEDESC = (LocString) (UI.FormatAsLink("Mush Fry", "FRIEDMUSHBAR") + " reheated in a " + (string) BUILDINGS.PREFABS.GAMMARAYOVEN.NAME + ".");
      }

      public class FRUITCAKE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Berry Sludge", nameof (FRUITCAKE));
        public static LocString DESC = (LocString) ("A mashed up " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + " sludge with an exceptionally long shelf life.\n\nIts aggressive, overbearing sweetness can leave the tongue feeling temporarily numb.");
        public static LocString RECIPEDESC = (LocString) ("A mashed up " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + " sludge with an exceptionally long shelf life.");
      }

      public class POPCORN
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Popcorn", nameof (POPCORN));
        public static LocString DESC = (LocString) (UI.FormatAsLink("Sleet Wheat Grain", "COLDWHEATSEED") + " popped in a " + (string) BUILDINGS.PREFABS.GAMMARAYOVEN.NAME + ".\n\nCompletely devoid of any fancy flavorings.");
        public static LocString RECIPEDESC = (LocString) ("Gamma-radiated " + UI.FormatAsLink("Sleet Wheat Grain", "COLDWHEATSEED") + ".");
      }

      public class SUSHI
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Sushi", nameof (SUSHI));
        public static LocString DESC = (LocString) ("Raw " + UI.FormatAsLink("Pacu Fillet", "FISHMEAT") + " wrapped with fresh " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".\n\nWhile the salt of the lettuce may initially overpower the flavor, a keen palate can discern the subtle sweetness of the fillet beneath.");
        public static LocString RECIPEDESC = (LocString) ("Raw " + UI.FormatAsLink("Pacu Fillet", "FISHMEAT") + " wrapped with fresh " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".");
      }

      public class HATCHEGG
      {
        public static LocString NAME = CREATURES.SPECIES.HATCH.EGG_NAME;
        public static LocString DESC = (LocString) ("An egg laid by a " + UI.FormatAsLink("Hatch", "HATCH") + ".\n\nIf incubated, it will hatch into a " + UI.FormatAsLink("Hatchling", "HATCH") + ".");
        public static LocString RECIPEDESC = (LocString) ("An egg laid by a " + UI.FormatAsLink("Hatch", "HATCH") + ".");
      }

      public class DRECKOEGG
      {
        public static LocString NAME = CREATURES.SPECIES.DRECKO.EGG_NAME;
        public static LocString DESC = (LocString) ("An egg laid by a " + UI.FormatAsLink("Drecko", "DRECKO") + ".\n\nIf incubated, it will hatch into a new " + UI.FormatAsLink("Drecklet", "DRECKO") + ".");
        public static LocString RECIPEDESC = (LocString) ("An egg laid by a " + UI.FormatAsLink("Drecko", "DRECKO") + ".");
      }

      public class LIGHTBUGEGG
      {
        public static LocString NAME = CREATURES.SPECIES.LIGHTBUG.EGG_NAME;
        public static LocString DESC = (LocString) ("An egg laid by a " + UI.FormatAsLink("Shine Bug", "LIGHTBUG") + ".\n\nIf incubated, it will hatch into a " + UI.FormatAsLink("Shine Nymph", "LIGHTBUG") + ".");
        public static LocString RECIPEDESC = (LocString) ("An egg laid by a " + UI.FormatAsLink("Shine Bug", "LIGHTBUG") + ".");
      }

      public class LETTUCE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Lettuce", nameof (LETTUCE));
        public static LocString DESC = (LocString) ("Crunchy, slightly salty leaves from a " + UI.FormatAsLink("Waterweed", "SEALETTUCE") + " plant.");
        public static LocString RECIPEDESC = (LocString) ("Edible roughage from a " + UI.FormatAsLink("Waterweed", "SEALETTUCE") + ".");
      }

      public class OILFLOATEREGG
      {
        public static LocString NAME = CREATURES.SPECIES.OILFLOATER.EGG_NAME;
        public static LocString DESC = (LocString) ("An egg laid by a " + UI.FormatAsLink("Slickster", "OILFLOATER") + ".\n\nIf incubated, it will hatch into a " + UI.FormatAsLink("Slickster Larva", "OILFLOATER") + ".");
        public static LocString RECIPEDESC = (LocString) ("An egg laid by a " + UI.FormatAsLink("Slickster", "OILFLOATER") + ".");
      }

      public class PUFTEGG
      {
        public static LocString NAME = CREATURES.SPECIES.PUFT.EGG_NAME;
        public static LocString DESC = (LocString) ("An egg laid by a " + UI.FormatAsLink("Puft", "PUFT") + ".\n\nIf incubated, it will hatch into a " + UI.FormatAsLink("Puftlet", "PUFT") + ".");
        public static LocString RECIPEDESC = (LocString) ("An egg laid by a " + (string) CREATURES.SPECIES.PUFT.NAME + ".");
      }

      public class FISHMEAT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pacu Fillet", nameof (FISHMEAT));
        public static LocString DESC = (LocString) ("An uncooked fillet from a very dead " + (string) CREATURES.SPECIES.PACU.NAME + ". Yum!");
      }

      public class MEAT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Meat", nameof (MEAT));
        public static LocString DESC = (LocString) "Uncooked meat from a very dead critter. Yum!";
      }

      public class PLANTMEAT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Plant Meat", nameof (PLANTMEAT));
        public static LocString DESC = (LocString) "Planty plant meat from a plant. How nice!";
      }

      public class SHELLFISHMEAT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Raw Shellfish", nameof (SHELLFISHMEAT));
        public static LocString DESC = (LocString) ("An uncooked chunk of very dead " + (string) CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.NAME + ". Yum!");
      }

      public class MUSHROOM
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Mushroom", nameof (MUSHROOM));
        public static LocString DESC = (LocString) "An edible, flavorless fungus that grew in the dark.";
      }

      public class COOKEDFISH
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Cooked Seafood", nameof (COOKEDFISH));
        public static LocString DESC = (LocString) "A cooked piece of freshly caught aquatic critter.\n\nUnsurprisingly, it tastes a bit fishy.";
        public static LocString RECIPEDESC = (LocString) "A cooked piece of freshly caught aquatic critter.";
      }

      public class COOKEDMEAT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Barbeque", nameof (COOKEDMEAT));
        public static LocString DESC = (LocString) "The cooked meat of a defeated critter.\n\nIt has a delightful smoky aftertaste.";
        public static LocString RECIPEDESC = (LocString) "The cooked meat of a defeated critter.";
      }

      public class PICKLEDMEAL
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pickled Meal", nameof (PICKLEDMEAL));
        public static LocString DESC = (LocString) "Meal Lice preserved in vinegar.\n\nIt's a rarely acquired taste.";
        public static LocString RECIPEDESC = (LocString) ((string) ITEMS.FOOD.BASICPLANTFOOD.NAME + " regrettably preserved in vinegar.");
      }

      public class FRIEDMUSHBAR
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Mush Fry", nameof (FRIEDMUSHBAR));
        public static LocString DESC = (LocString) "Deep fried, solidified mudslop.\n\nThe inside is almost completely uncooked, despite the crunch on the outside.";
        public static LocString RECIPEDESC = (LocString) "Deep fried, solidified mudslop.";
      }

      public class RAWEGG
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Raw Egg", nameof (RAWEGG));
        public static LocString DESC = (LocString) ("A raw Egg that has been cracked open for use in " + UI.FormatAsLink("Food", nameof (FOOD)) + " preparation.\n\nIt will never hatch.");
        public static LocString RECIPEDESC = (LocString) ("A raw egg that has been cracked open for use in " + UI.FormatAsLink("Food", nameof (FOOD)) + " preparation.");
      }

      public class COOKEDEGG
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Omelette", nameof (COOKEDEGG));
        public static LocString DESC = (LocString) "Fluffed and folded Egg innards.\n\nIt turns out you do, in fact, have to break a few eggs to make it.";
        public static LocString RECIPEDESC = (LocString) "Fluffed and folded egg innards.";
      }

      public class FRIEDMUSHROOM
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Fried Mushroom", nameof (FRIEDMUSHROOM));
        public static LocString DESC = (LocString) ("A fried dish made with a fruiting " + UI.FormatAsLink("Dusk Cap", "MUSHROOM") + ".\n\nIt has a thick, savory flavor with subtle earthy undertones.");
        public static LocString RECIPEDESC = (LocString) ("A fried dish made with a fruiting " + UI.FormatAsLink("Dusk Cap", "MUSHROOM") + ".");
      }

      public class PRICKLEFRUIT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Bristle Berry", nameof (PRICKLEFRUIT));
        public static LocString DESC = (LocString) "A sweet, mostly pleasant-tasting fruit covered in prickly barbs.";
      }

      public class GRILLEDPRICKLEFRUIT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Gristle Berry", nameof (GRILLEDPRICKLEFRUIT));
        public static LocString DESC = (LocString) ("The grilled bud of a " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + ".\n\nHeat unlocked an exquisite taste in the fruit, though the burnt spines leave something to be desired.");
        public static LocString RECIPEDESC = (LocString) ("The grilled bud of a " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + ".");
      }

      public class SWAMPFRUIT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Bog Jelly", nameof (SWAMPFRUIT));
        public static LocString DESC = (LocString) "A fruit with an outer film that contains chewy gelatinous cubes.";
      }

      public class SWAMPDELIGHTS
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Swampy Delights", nameof (SWAMPDELIGHTS));
        public static LocString DESC = (LocString) ("Dried gelatinous cubes from a " + UI.FormatAsLink("Bog Jelly", "SWAMPFRUIT") + ".\n\nEach cube has a wonderfully chewy texture and is lightly coated in a delicate powder.");
        public static LocString RECIPEDESC = (LocString) ("Dried gelatinous cubes from a " + UI.FormatAsLink("Bog Jelly", "SWAMPFRUIT") + ".");
      }

      public class WORMBASICFRUIT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Spindly Grubfruit", nameof (WORMBASICFRUIT));
        public static LocString DESC = (LocString) ("A " + UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT") + " that failed to develop properly.\n\nIt is nonetheless edible, and vaguely tasty.");
      }

      public class WORMBASICFOOD
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Roast Grubfruit Nut", nameof (WORMBASICFOOD));
        public static LocString DESC = (LocString) ("Slow roasted " + UI.FormatAsLink("Spindly Grubfruit", "WORMBASICFRUIT") + ".\n\nIt has a smoky aroma and tastes of coziness.");
        public static LocString RECIPEDESC = (LocString) ("Slow roasted " + UI.FormatAsLink("Spindly Grubfruit", "WORMBASICFRUIT") + ".");
      }

      public class WORMSUPERFRUIT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Grubfruit", nameof (WORMSUPERFRUIT));
        public static LocString DESC = (LocString) "A plump, healthy fruit with a honey-like taste.";
      }

      public class WORMSUPERFOOD
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Grubfruit Preserve", nameof (WORMSUPERFOOD));
        public static LocString DESC = (LocString) ("A long lasting " + UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT") + " jam preserved in " + UI.FormatAsLink("Sucrose", "SUCROSE") + ".\n\nThe thick, goopy jam retains the shape of the jar when poured out, but the sweet taste can't be matched.");
        public static LocString RECIPEDESC = (LocString) ("A long lasting " + UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT") + " jam preserved in " + UI.FormatAsLink("Sucrose", "SUCROSE") + ".");
      }

      public class BERRYPIE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Mixed Berry Pie", nameof (BERRYPIE));
        public static LocString DESC = (LocString) ("A pie made primarily of " + UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT") + " and " + UI.FormatAsLink("Gristle Berries", "PRICKLEFRUIT") + ".\n\nThe mixture of berries creates a fragrant, colorful filling that packs a sweet punch.");
        public static LocString RECIPEDESC = (LocString) ("A pie made primarily of " + UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT") + " and " + UI.FormatAsLink("Gristle Berries", "PRICKLEFRUIT") + ".");
      }

      public class COLDWHEATBREAD
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Frost Bun", nameof (COLDWHEATBREAD));
        public static LocString DESC = (LocString) ("A simple bun baked from " + UI.FormatAsLink("Sleet Wheat Grain", "COLDWHEATSEED") + " grain.\n\nEach bite leaves a mild cooling sensation in one's mouth, even when the bun itself is warm.");
        public static LocString RECIPEDESC = (LocString) ("A simple bun baked from " + UI.FormatAsLink("Sleet Wheat Grain", "COLDWHEATSEED") + " grain.");
      }

      public class BEAN
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Nosh Bean", nameof (BEAN));
        public static LocString DESC = (LocString) ("The crisp bean of a " + UI.FormatAsLink("Nosh Sprout", "BEAN_PLANT") + ".\n\nEach bite tastes refreshingly natural and wholesome.");
      }

      public class SPICENUT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pincha Peppernut", nameof (SPICENUT));
        public static LocString DESC = (LocString) ("The flavorful nut of a " + UI.FormatAsLink("Pincha Pepperplant", "SPICE_VINE") + ".\n\nThe bitter outer rind hides a rich, peppery core that is useful in cooking.");
      }

      public class SPICEBREAD
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Pepper Bread", nameof (SPICEBREAD));
        public static LocString DESC = (LocString) ("A loaf of bread, lightly spiced with " + UI.FormatAsLink("Pincha Peppernut", "SPICENUT") + " for a mild bite.\n\nThere's a simple joy to be had in pulling it apart in one's fingers.");
        public static LocString RECIPEDESC = (LocString) ("A loaf of bread, lightly spiced with " + UI.FormatAsLink("Pincha Peppernut", "SPICENUT") + " for a mild bite.");
      }

      public class SURFANDTURF
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Surf'n'Turf", nameof (SURFANDTURF));
        public static LocString DESC = (LocString) ("A bit of " + UI.FormatAsLink("Meat", "MEAT") + " from the land and " + UI.FormatAsLink("Cooked Seafood", "COOKEDFISH") + " from the sea.\n\nIt's hearty and satisfying.");
        public static LocString RECIPEDESC = (LocString) ("A bit of " + UI.FormatAsLink("Meat", "MEAT") + " from the land and " + UI.FormatAsLink("Cooked Seafood", "COOKEDFISH") + " from the sea.");
      }

      public class TOFU
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Tofu", nameof (TOFU));
        public static LocString DESC = (LocString) ("A bland curd made from " + UI.FormatAsLink("Nosh Beans", "BEAN") + ".\n\nIt has an unusual but pleasant consistency.");
        public static LocString RECIPEDESC = (LocString) ("A bland curd made from " + UI.FormatAsLink("Nosh Beans", "BEAN") + ".");
      }

      public class SPICYTOFU
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Spicy Tofu", nameof (SPICYTOFU));
        public static LocString DESC = (LocString) ((string) ITEMS.FOOD.TOFU.NAME + " marinated in a flavorful " + UI.FormatAsLink("Pincha Peppernut", "SPICENUT") + " sauce.\n\nIt packs a delightful punch.");
        public static LocString RECIPEDESC = (LocString) ((string) ITEMS.FOOD.TOFU.NAME + " marinated in a flavorful " + UI.FormatAsLink("Pincha Peppernut", "SPICENUT") + " sauce.");
      }

      public class CURRY
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Curried Beans", nameof (CURRY));
        public static LocString DESC = (LocString) ("Chewy " + UI.FormatAsLink("Nosh Beans", "BEANPLANTSEED") + " simmered with chunks of " + (string) ITEMS.INGREDIENTS.GINGER.NAME + ".\n\nIt's so spicy!");
        public static LocString RECIPEDESC = (LocString) ("Chewy " + UI.FormatAsLink("Nosh Beans", "BEANPLANTSEED") + " simmered with chunks of " + (string) ITEMS.INGREDIENTS.GINGER.NAME + ".");
      }

      public class SALSA
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Stuffed Berry", nameof (SALSA));
        public static LocString DESC = (LocString) ("A baked " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + " stuffed with delectable spices and vibrantly flavored.");
        public static LocString RECIPEDESC = (LocString) ("A baked " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + " stuffed with delectable spices and vibrantly flavored.");
      }

      public class BASICPLANTFOOD
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Meal Lice", nameof (BASICPLANTFOOD));
        public static LocString DESC = (LocString) "A flavorless grain that almost never wiggles on its own.";
      }

      public class BASICPLANTBAR
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Liceloaf", nameof (BASICPLANTBAR));
        public static LocString DESC = (LocString) (UI.FormatAsLink("Meal Lice", "BASICPLANTFOOD") + " compacted into a dense, immobile loaf.");
        public static LocString RECIPEDESC = (LocString) (UI.FormatAsLink("Meal Lice", "BASICPLANTFOOD") + " compacted into a dense, immobile loaf.");
      }

      public class BASICFORAGEPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Muckroot", nameof (BASICFORAGEPLANT));
        public static LocString DESC = (LocString) ("A seedless fruit with an upsettingly bland aftertaste.\n\nIt cannot be replanted.\n\nDigging up Buried Objects may uncover a " + (string) ITEMS.FOOD.BASICFORAGEPLANT.NAME + ".");
      }

      public class FORESTFORAGEPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Hexalent Fruit", nameof (FORESTFORAGEPLANT));
        public static LocString DESC = (LocString) "A seedless fruit with an unusual rubbery texture.\n\nIt cannot be replanted.\n\nHexalent fruit is much more calorie dense than Muckroot fruit.";
      }

      public class SWAMPFORAGEPLANT
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Swamp Chard Heart", nameof (SWAMPFORAGEPLANT));
        public static LocString DESC = (LocString) "A seedless plant with a squishy, juicy center and an awful smell.\n\nIt cannot be replanted.";
      }

      public class ROTPILE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Rot Pile", "COMPOST");
        public static LocString DESC = (LocString) ("An inedible glop of former foodstuff.\n\n" + (string) ITEMS.FOOD.ROTPILE.NAME + "s break down into " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + " over time.");
      }

      public class COLDWHEATSEED
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Sleet Wheat Grain", nameof (COLDWHEATSEED));
        public static LocString DESC = (LocString) "An edible grain that leaves a cool taste on the tongue.";
      }

      public class BEANPLANTSEED
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Nosh Bean", nameof (BEANPLANTSEED));
        public static LocString DESC = (LocString) "An inedible bean that can be processed into delicious foods.";
      }
    }

    public class INGREDIENTS
    {
      public class SWAMPLILYFLOWER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Balm Lily Flower", nameof (SWAMPLILYFLOWER));
        public static LocString DESC = (LocString) "A medicinal flower that soothes most minor maladies.\n\nIt is exceptionally fragrant.";
      }

      public class GINGER
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Tonic Root", nameof (GINGER));
        public static LocString DESC = (LocString) "A chewy, fibrous rhizome with a fiery aftertaste.";
      }
    }

    public class INDUSTRIAL_PRODUCTS
    {
      public class FUEL_BRICK
      {
        public static LocString NAME = (LocString) "Fuel Brick";
        public static LocString DESC = (LocString) ("A densely compressed brick of combustible material.\n\nIt can be burned to produce a one-time burst of " + UI.FormatAsLink("Power", "POWER") + ".");
      }

      public class BASIC_FABRIC
      {
        public static LocString NAME = (LocString) "Reed Fiber";
        public static LocString DESC = (LocString) ("A ball of raw cellulose used in the production of " + UI.FormatAsLink("Clothing", "EQUIPMENT") + " and textiles.");
      }

      public class TRAP_PARTS
      {
        public static LocString NAME = (LocString) "Trap Components";
        public static LocString DESC = (LocString) ("These components can be assembled into a " + (string) BUILDINGS.PREFABS.CREATURETRAP.NAME + " and used to catch " + UI.FormatAsLink("Critters", "CREATURES") + ".");
      }

      public class POWER_STATION_TOOLS
      {
        public static LocString NAME = (LocString) "Microchip";
        public static LocString DESC = (LocString) ("A specialized " + (string) ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME + " created by a professional engineer.\n\nTunes up " + UI.PRE_KEYWORD + "Generators" + UI.PST_KEYWORD + " to increase their " + UI.FormatAsLink("Power", "POWER") + " output.");
        public static LocString TINKER_REQUIREMENT_NAME = (LocString) ("Skill: " + (string) DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME);
        public static LocString TINKER_REQUIREMENT_TOOLTIP = (LocString) ("Can only be used by a Duplicant with " + (string) DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME + " to apply a " + UI.PRE_KEYWORD + "Tune Up" + UI.PST_KEYWORD + ".");
        public static LocString TINKER_EFFECT_NAME = (LocString) "Engie's Tune-Up: {0} {1}";
        public static LocString TINKER_EFFECT_TOOLTIP = (LocString) ("Can be used to " + UI.PRE_KEYWORD + "Tune Up" + UI.PST_KEYWORD + " a generator, increasing its {0} by <b>{1}</b>.");
      }

      public class FARM_STATION_TOOLS
      {
        public static LocString NAME = (LocString) "Micronutrient Fertilizer";
        public static LocString DESC = (LocString) ("Specialized " + UI.FormatAsLink("Fertilizer", "FERTILIZER") + " mixed by a Duplicant with the " + (string) DUPLICANTS.ROLES.FARMER.NAME + " Skill.\n\nIncreases the " + UI.PRE_KEYWORD + "Growth Rate" + UI.PST_KEYWORD + " of one " + UI.FormatAsLink("Plant", "PLANTS") + ".");
      }

      public class MACHINE_PARTS
      {
        public static LocString NAME = (LocString) "Custom Parts";
        public static LocString DESC = (LocString) ("Specialized Parts crafted by a professional engineer.\n\n" + UI.PRE_KEYWORD + "Jerry Rig" + UI.PST_KEYWORD + " machine buildings to increase their efficiency.");
        public static LocString TINKER_REQUIREMENT_NAME = (LocString) ("Job: " + (string) DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME);
        public static LocString TINKER_REQUIREMENT_TOOLTIP = (LocString) ("Can only be used by a Duplicant with " + (string) DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME + " to apply a " + UI.PRE_KEYWORD + "Jerry Rig" + UI.PST_KEYWORD + ".");
        public static LocString TINKER_EFFECT_NAME = (LocString) "Engineer's Jerry Rig: {0} {1}";
        public static LocString TINKER_EFFECT_TOOLTIP = (LocString) ("Can be used to " + UI.PRE_KEYWORD + "Jerry Rig" + UI.PST_KEYWORD + " upgrades to a machine building, increasing its {0} by <b>{1}</b>.");
      }

      public class RESEARCH_DATABANK
      {
        public static LocString NAME = (LocString) "Data Bank";
        public static LocString DESC = (LocString) ("Raw data that can be processed into " + UI.FormatAsLink("Interstellar Research", "RESEARCH") + " points.");
      }

      public class ORBITAL_RESEARCH_DATABANK
      {
        public static LocString NAME = (LocString) "Data Bank";
        public static LocString DESC = (LocString) ("Raw Data that can be processed into " + UI.FormatAsLink("Data Analysis Research", "RESEARCH") + " points.");
        public static LocString RECIPE_DESC = (LocString) ("Databanks of raw data generated from exploring, either by exploring new areas with Duplicants, or by using an " + UI.FormatAsLink("Orbital Data Collection Lab", "ORBITALRESEARCHCENTER") + ".\n\nUsed by the " + UI.FormatAsLink("Virtual Planetarium", "DLC1COSMICRESEARCHCENTER") + " to conduct research.");
      }

      public class EGG_SHELL
      {
        public static LocString NAME = (LocString) "Egg Shell";
        public static LocString DESC = (LocString) ("Can be crushed to produce " + UI.FormatAsLink("Lime", "LIME") + ".");
      }

      public class CRAB_SHELL
      {
        public static LocString NAME = (LocString) "Pokeshell Molt";
        public static LocString DESC = (LocString) ("Can be crushed to produce " + UI.FormatAsLink("Lime", "LIME") + ".");

        public class VARIANT_WOOD
        {
          public static LocString NAME = (LocString) "Oakshell Molt";
          public static LocString DESC = (LocString) ("Can be crushed to produce " + UI.FormatAsLink("Lumber", "WOOD") + ".");
        }
      }

      public class BABY_CRAB_SHELL
      {
        public static LocString NAME = (LocString) "Small Pokeshell Molt";
        public static LocString DESC = (LocString) ("Can be crushed to produce " + UI.FormatAsLink("Lime", "LIME") + ".");

        public class VARIANT_WOOD
        {
          public static LocString NAME = (LocString) "Small Oakshell Molt";
          public static LocString DESC = (LocString) ("Can be crushed to produce " + UI.FormatAsLink("Lumber", "WOOD") + ".");
        }
      }

      public class WOOD
      {
        public static LocString NAME = (LocString) "Lumber";
        public static LocString DESC = (LocString) ("Wood harvested from an " + UI.FormatAsLink("Arbor Tree", "FOREST_TREE") + " or an " + UI.FormatAsLink("Oakshell", "CRABWOOD") + ".");
      }

      public class GENE_SHUFFLER_RECHARGE
      {
        public static LocString NAME = (LocString) "Vacillator Recharge";
        public static LocString DESC = (LocString) ("Replenishes one charge to a depleted " + (string) BUILDINGS.PREFABS.GENESHUFFLER.NAME + ".");
      }

      public class TABLE_SALT
      {
        public static LocString NAME = (LocString) "Table Salt";
        public static LocString DESC = (LocString) ("A seasoning that Duplicants can add to their " + UI.FormatAsLink("Food", "FOOD") + " to boost " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nDuplicants will automatically use Table Salt while sitting at a " + (string) BUILDINGS.PREFABS.DININGTABLE.NAME + " during mealtime.\n\n<i>Only the finest grains are chosen.</i>");
      }

      public class REFINED_SUGAR
      {
        public static LocString NAME = (LocString) "Refined Sugar";
        public static LocString DESC = (LocString) ("A seasoning that Duplicants can add to their " + UI.FormatAsLink("Food", "FOOD") + " to boost " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nDuplicants will automatically use Refined Sugar while sitting at a " + (string) BUILDINGS.PREFABS.DININGTABLE.NAME + " during mealtime.\n\n<i>Only the finest grains are chosen.</i>");
      }
    }

    public class CARGO_CAPSULE
    {
      public static LocString NAME = (LocString) "Care Package";
      public static LocString DESC = (LocString) "A delivery system for recently printed resources.\n\nIt will dematerialize shortly.";
    }

    public class RAILGUNPAYLOAD
    {
      public static LocString NAME = (LocString) UI.FormatAsLink("Interplanetary Payload", nameof (RAILGUNPAYLOAD));
      public static LocString DESC = (LocString) ("Contains resources packed for interstellar shipping.\n\nCan be launched by a " + (string) BUILDINGS.PREFABS.RAILGUN.NAME + " or unpacked with a " + (string) BUILDINGS.PREFABS.RAILGUNPAYLOADOPENER.NAME + ".");
    }

    public class DEBRISPAYLOAD
    {
      public static LocString NAME = (LocString) "Rocket Debris";
      public static LocString DESC = (LocString) "Whatever is left over from a Rocket Self-Destruct can be recovered once it has crash-landed.";
    }

    public class RADIATION
    {
      public class HIGHENERGYPARITCLE
      {
        public static LocString NAME = (LocString) "Radbolts";
        public static LocString DESC = (LocString) ("A concentrated field of " + UI.FormatAsKeyWord("Radbolts") + " that can be largely redirected using a " + UI.FormatAsLink("Radbolt Reflector", "HIGHENERGYPARTICLEREDIRECTOR") + ".");
      }
    }

    public class DREAMJOURNAL
    {
      public static LocString NAME = (LocString) "Dream Journal";
      public static LocString DESC = (LocString) ("A hand-scrawled account of " + UI.FormatAsLink("Pajama", "SLEEP_CLINIC_PAJAMAS") + "-induced dreams.\n\nCan be analyzed using a " + UI.FormatAsLink("Somnium Synthesizer", "MEGABRAINTANK") + ".");
    }

    public class SPICES
    {
      public class MACHINERY_SPICE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Machinist Spice", nameof (MACHINERY_SPICE));
        public static LocString DESC = (LocString) "Improves operating skills when ingested.";
      }

      public class PILOTING_SPICE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Rocketeer Spice", nameof (PILOTING_SPICE));
        public static LocString DESC = (LocString) "Provides a boost to piloting abilities.";
      }

      public class PRESERVING_SPICE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Freshener Spice", nameof (PRESERVING_SPICE));
        public static LocString DESC = (LocString) "Slows the decomposition of perishable foods.";
      }

      public class STRENGTH_SPICE
      {
        public static LocString NAME = (LocString) UI.FormatAsLink("Brawny Spice", nameof (STRENGTH_SPICE));
        public static LocString DESC = (LocString) "Strengthens even the weakest of muscles.";
      }
    }
  }
}
