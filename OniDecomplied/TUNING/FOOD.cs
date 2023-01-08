// Decompiled with JetBrains decompiler
// Type: TUNING.FOOD
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace TUNING
{
  public class FOOD
  {
    public const float EATING_SECONDS_PER_CALORIE = 2E-05f;
    public const float FOOD_CALORIES_PER_CYCLE = 1000000f;
    public const int FOOD_AMOUNT_INGREDIENT_ONLY = 0;
    public const float KCAL_SMALL_PORTION = 600000f;
    public const float KCAL_BONUS_COOKING_LOW = 250000f;
    public const float KCAL_BASIC_PORTION = 800000f;
    public const float KCAL_PREPARED_FOOD = 4000000f;
    public const float KCAL_BONUS_COOKING_BASIC = 400000f;
    public const float DEFAULT_PRESERVE_TEMPERATURE = 255.15f;
    public const float DEFAULT_ROT_TEMPERATURE = 277.15f;
    public const float HIGH_PRESERVE_TEMPERATURE = 283.15f;
    public const float HIGH_ROT_TEMPERATURE = 308.15f;
    public const float EGG_COOK_TEMPERATURE = 344.15f;
    public const float DEFAULT_MASS = 1f;
    public const float DEFAULT_SPICE_MASS = 1f;
    public const float ROT_TO_ELEMENT_TIME = 600f;
    public const int MUSH_BAR_SPAWN_GERMS = 1000;
    public const float IDEAL_TEMPERATURE_TOLERANCE = 10f;
    public const int FOOD_QUALITY_AWFUL = -1;
    public const int FOOD_QUALITY_TERRIBLE = 0;
    public const int FOOD_QUALITY_MEDIOCRE = 1;
    public const int FOOD_QUALITY_GOOD = 2;
    public const int FOOD_QUALITY_GREAT = 3;
    public const int FOOD_QUALITY_AMAZING = 4;
    public const int FOOD_QUALITY_WONDERFUL = 5;
    public const int FOOD_QUALITY_MORE_WONDERFUL = 6;

    public class SPOIL_TIME
    {
      public const float DEFAULT = 4800f;
      public const float QUICK = 2400f;
      public const float SLOW = 9600f;
      public const float VERYSLOW = 19200f;
    }

    public class FOOD_TYPES
    {
      public static readonly EdiblesManager.FoodInfo FIELDRATION = new EdiblesManager.FoodInfo("FieldRation", "", 800000f, -1, 255.15f, 277.15f, 19200f, false);
      public static readonly EdiblesManager.FoodInfo MUSHBAR = new EdiblesManager.FoodInfo("MushBar", "", 800000f, -1, 255.15f, 277.15f, 4800f, true);
      public static readonly EdiblesManager.FoodInfo BASICPLANTFOOD = new EdiblesManager.FoodInfo("BasicPlantFood", "", 600000f, -1, 255.15f, 277.15f, 4800f, true);
      public static readonly EdiblesManager.FoodInfo BASICFORAGEPLANT = new EdiblesManager.FoodInfo("BasicForagePlant", "", 800000f, -1, 255.15f, 277.15f, 4800f, false);
      public static readonly EdiblesManager.FoodInfo FORESTFORAGEPLANT = new EdiblesManager.FoodInfo("ForestForagePlant", "", 6400000f, -1, 255.15f, 277.15f, 4800f, false);
      public static readonly EdiblesManager.FoodInfo SWAMPFORAGEPLANT = new EdiblesManager.FoodInfo("SwampForagePlant", "EXPANSION1_ID", 2400000f, -1, 255.15f, 277.15f, 4800f, false);
      public static readonly EdiblesManager.FoodInfo MUSHROOM = new EdiblesManager.FoodInfo(MushroomConfig.ID, "", 2400000f, 0, 255.15f, 277.15f, 4800f, true);
      public static readonly EdiblesManager.FoodInfo LETTUCE;
      public static readonly EdiblesManager.FoodInfo MEAT;
      public static readonly EdiblesManager.FoodInfo PLANTMEAT;
      public static readonly EdiblesManager.FoodInfo PRICKLEFRUIT;
      public static readonly EdiblesManager.FoodInfo SWAMPFRUIT;
      public static readonly EdiblesManager.FoodInfo FISH_MEAT;
      public static readonly EdiblesManager.FoodInfo SHELLFISH_MEAT;
      public static readonly EdiblesManager.FoodInfo WORMBASICFRUIT;
      public static readonly EdiblesManager.FoodInfo WORMSUPERFRUIT;
      public static readonly EdiblesManager.FoodInfo PICKLEDMEAL;
      public static readonly EdiblesManager.FoodInfo BASICPLANTBAR;
      public static readonly EdiblesManager.FoodInfo FRIEDMUSHBAR;
      public static readonly EdiblesManager.FoodInfo GAMMAMUSH;
      public static readonly EdiblesManager.FoodInfo GRILLED_PRICKLEFRUIT;
      public static readonly EdiblesManager.FoodInfo SWAMP_DELIGHTS;
      public static readonly EdiblesManager.FoodInfo FRIED_MUSHROOM;
      public static readonly EdiblesManager.FoodInfo COLD_WHEAT_BREAD;
      public static readonly EdiblesManager.FoodInfo COOKED_EGG;
      public static readonly EdiblesManager.FoodInfo COOKED_FISH;
      public static readonly EdiblesManager.FoodInfo COOKED_MEAT;
      public static readonly EdiblesManager.FoodInfo WORMBASICFOOD;
      public static readonly EdiblesManager.FoodInfo WORMSUPERFOOD;
      public static readonly EdiblesManager.FoodInfo FRUITCAKE;
      public static readonly EdiblesManager.FoodInfo SALSA;
      public static readonly EdiblesManager.FoodInfo SURF_AND_TURF;
      public static readonly EdiblesManager.FoodInfo MUSHROOM_WRAP;
      public static readonly EdiblesManager.FoodInfo TOFU;
      public static readonly EdiblesManager.FoodInfo SPICEBREAD;
      public static readonly EdiblesManager.FoodInfo SPICY_TOFU;
      public static readonly EdiblesManager.FoodInfo CURRY;
      public static readonly EdiblesManager.FoodInfo BERRY_PIE;
      public static readonly EdiblesManager.FoodInfo BURGER;
      public static readonly EdiblesManager.FoodInfo BEAN;
      public static readonly EdiblesManager.FoodInfo SPICENUT;
      public static readonly EdiblesManager.FoodInfo COLD_WHEAT_SEED;
      public static readonly EdiblesManager.FoodInfo RAWEGG;

      static FOOD_TYPES()
      {
        EdiblesManager.FoodInfo foodInfo1 = new EdiblesManager.FoodInfo("Lettuce", "", 400000f, 0, 255.15f, 277.15f, 2400f, true);
        List<string> effects1 = new List<string>();
        effects1.Add("SeafoodRadiationResistance");
        string[] availableExpansioN1Only1 = DlcManager.AVAILABLE_EXPANSION1_ONLY;
        FOOD.FOOD_TYPES.LETTUCE = foodInfo1.AddEffects(effects1, availableExpansioN1Only1);
        FOOD.FOOD_TYPES.MEAT = new EdiblesManager.FoodInfo("Meat", "", 1600000f, -1, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.PLANTMEAT = new EdiblesManager.FoodInfo("PlantMeat", "EXPANSION1_ID", 1200000f, 1, 255.15f, 277.15f, 2400f, true);
        FOOD.FOOD_TYPES.PRICKLEFRUIT = new EdiblesManager.FoodInfo(PrickleFruitConfig.ID, "", 1600000f, 0, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.SWAMPFRUIT = new EdiblesManager.FoodInfo(SwampFruitConfig.ID, "EXPANSION1_ID", 1840000f, 0, 255.15f, 277.15f, 2400f, true);
        EdiblesManager.FoodInfo foodInfo2 = new EdiblesManager.FoodInfo("FishMeat", "", 1000000f, 2, 255.15f, 277.15f, 2400f, true);
        List<string> effects2 = new List<string>();
        effects2.Add("SeafoodRadiationResistance");
        string[] availableExpansioN1Only2 = DlcManager.AVAILABLE_EXPANSION1_ONLY;
        FOOD.FOOD_TYPES.FISH_MEAT = foodInfo2.AddEffects(effects2, availableExpansioN1Only2);
        EdiblesManager.FoodInfo foodInfo3 = new EdiblesManager.FoodInfo("ShellfishMeat", "", 1000000f, 2, 255.15f, 277.15f, 2400f, true);
        List<string> effects3 = new List<string>();
        effects3.Add("SeafoodRadiationResistance");
        string[] availableExpansioN1Only3 = DlcManager.AVAILABLE_EXPANSION1_ONLY;
        FOOD.FOOD_TYPES.SHELLFISH_MEAT = foodInfo3.AddEffects(effects3, availableExpansioN1Only3);
        FOOD.FOOD_TYPES.WORMBASICFRUIT = new EdiblesManager.FoodInfo("WormBasicFruit", "EXPANSION1_ID", 800000f, 0, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.WORMSUPERFRUIT = new EdiblesManager.FoodInfo("WormSuperFruit", "EXPANSION1_ID", 250000f, 1, 255.15f, 277.15f, 2400f, true);
        FOOD.FOOD_TYPES.PICKLEDMEAL = new EdiblesManager.FoodInfo("PickledMeal", "", 1800000f, -1, 255.15f, 277.15f, 19200f, true);
        FOOD.FOOD_TYPES.BASICPLANTBAR = new EdiblesManager.FoodInfo("BasicPlantBar", "", 1700000f, 0, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.FRIEDMUSHBAR = new EdiblesManager.FoodInfo("FriedMushBar", "", 1050000f, 0, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.GAMMAMUSH = new EdiblesManager.FoodInfo("GammaMush", "", 1050000f, 1, 255.15f, 277.15f, 2400f, true);
        FOOD.FOOD_TYPES.GRILLED_PRICKLEFRUIT = new EdiblesManager.FoodInfo("GrilledPrickleFruit", "", 2000000f, 1, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.SWAMP_DELIGHTS = new EdiblesManager.FoodInfo("SwampDelights", "EXPANSION1_ID", 2240000f, 1, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.FRIED_MUSHROOM = new EdiblesManager.FoodInfo("FriedMushroom", "", 2800000f, 1, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.COLD_WHEAT_BREAD = new EdiblesManager.FoodInfo("ColdWheatBread", "", 1200000f, 2, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.COOKED_EGG = new EdiblesManager.FoodInfo("CookedEgg", "", 2800000f, 2, 255.15f, 277.15f, 2400f, true);
        EdiblesManager.FoodInfo foodInfo4 = new EdiblesManager.FoodInfo("CookedFish", "", 1600000f, 3, 255.15f, 277.15f, 2400f, true);
        List<string> effects4 = new List<string>();
        effects4.Add("SeafoodRadiationResistance");
        string[] availableExpansioN1Only4 = DlcManager.AVAILABLE_EXPANSION1_ONLY;
        FOOD.FOOD_TYPES.COOKED_FISH = foodInfo4.AddEffects(effects4, availableExpansioN1Only4);
        FOOD.FOOD_TYPES.COOKED_MEAT = new EdiblesManager.FoodInfo("CookedMeat", "", 4000000f, 3, 255.15f, 277.15f, 2400f, true);
        FOOD.FOOD_TYPES.WORMBASICFOOD = new EdiblesManager.FoodInfo("WormBasicFood", "EXPANSION1_ID", 1200000f, 1, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.WORMSUPERFOOD = new EdiblesManager.FoodInfo("WormSuperFood", "EXPANSION1_ID", 2400000f, 3, 255.15f, 277.15f, 19200f, true);
        FOOD.FOOD_TYPES.FRUITCAKE = new EdiblesManager.FoodInfo("FruitCake", "", 4000000f, 3, 255.15f, 277.15f, 19200f, false);
        FOOD.FOOD_TYPES.SALSA = new EdiblesManager.FoodInfo("Salsa", "", 4400000f, 4, 255.15f, 277.15f, 2400f, true);
        EdiblesManager.FoodInfo foodInfo5 = new EdiblesManager.FoodInfo("SurfAndTurf", "", 6000000f, 4, 255.15f, 277.15f, 2400f, true);
        List<string> effects5 = new List<string>();
        effects5.Add("SeafoodRadiationResistance");
        string[] availableExpansioN1Only5 = DlcManager.AVAILABLE_EXPANSION1_ONLY;
        FOOD.FOOD_TYPES.SURF_AND_TURF = foodInfo5.AddEffects(effects5, availableExpansioN1Only5);
        EdiblesManager.FoodInfo foodInfo6 = new EdiblesManager.FoodInfo("MushroomWrap", "", 4800000f, 4, 255.15f, 277.15f, 2400f, true);
        List<string> effects6 = new List<string>();
        effects6.Add("SeafoodRadiationResistance");
        string[] availableExpansioN1Only6 = DlcManager.AVAILABLE_EXPANSION1_ONLY;
        FOOD.FOOD_TYPES.MUSHROOM_WRAP = foodInfo6.AddEffects(effects6, availableExpansioN1Only6);
        FOOD.FOOD_TYPES.TOFU = new EdiblesManager.FoodInfo("Tofu", "", 3600000f, 2, 255.15f, 277.15f, 2400f, true);
        FOOD.FOOD_TYPES.SPICEBREAD = new EdiblesManager.FoodInfo("SpiceBread", "", 4000000f, 5, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.SPICY_TOFU = new EdiblesManager.FoodInfo("SpicyTofu", "", 4000000f, 5, 255.15f, 277.15f, 2400f, true);
        EdiblesManager.FoodInfo foodInfo7 = new EdiblesManager.FoodInfo("Curry", "", 5000000f, 4, 255.15f, 277.15f, 9600f, true);
        List<string> effects7 = new List<string>();
        effects7.Add("HotStuff");
        string[] availableAllVersions1 = DlcManager.AVAILABLE_ALL_VERSIONS;
        FOOD.FOOD_TYPES.CURRY = foodInfo7.AddEffects(effects7, availableAllVersions1);
        FOOD.FOOD_TYPES.BERRY_PIE = new EdiblesManager.FoodInfo("BerryPie", "EXPANSION1_ID", 4200000f, 5, 255.15f, 277.15f, 2400f, true);
        EdiblesManager.FoodInfo foodInfo8 = new EdiblesManager.FoodInfo("Burger", "", 6000000f, 6, 255.15f, 277.15f, 2400f, true);
        List<string> effects8 = new List<string>();
        effects8.Add("GoodEats");
        string[] availableAllVersions2 = DlcManager.AVAILABLE_ALL_VERSIONS;
        EdiblesManager.FoodInfo foodInfo9 = foodInfo8.AddEffects(effects8, availableAllVersions2);
        List<string> effects9 = new List<string>();
        effects9.Add("SeafoodRadiationResistance");
        string[] availableExpansioN1Only7 = DlcManager.AVAILABLE_EXPANSION1_ONLY;
        FOOD.FOOD_TYPES.BURGER = foodInfo9.AddEffects(effects9, availableExpansioN1Only7);
        FOOD.FOOD_TYPES.BEAN = new EdiblesManager.FoodInfo("BeanPlantSeed", "", 0.0f, 3, 255.15f, 277.15f, 4800f, true);
        FOOD.FOOD_TYPES.SPICENUT = new EdiblesManager.FoodInfo(SpiceNutConfig.ID, "", 0.0f, 0, 255.15f, 277.15f, 2400f, true);
        FOOD.FOOD_TYPES.COLD_WHEAT_SEED = new EdiblesManager.FoodInfo("ColdWheatSeed", "", 0.0f, 0, 283.15f, 308.15f, 9600f, true);
        FOOD.FOOD_TYPES.RAWEGG = new EdiblesManager.FoodInfo("RawEgg", "", 0.0f, -1, 255.15f, 277.15f, 4800f, true);
      }
    }

    public class RECIPES
    {
      public static float SMALL_COOK_TIME = 30f;
      public static float STANDARD_COOK_TIME = 50f;
    }
  }
}
