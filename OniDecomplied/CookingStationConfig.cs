// Decompiled with JetBrains decompiler
// Type: CookingStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class CookingStationConfig : IBuildingConfig
{
  public const string ID = "CookingStation";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR3 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR3;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CookingStation", 3, 2, "cookstation_kanim", 30, 30f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "large";
    buildingDef.EnergyConsumptionWhenActive = 60f;
    buildingDef.ExhaustKilowattsWhenActive = 0.5f;
    buildingDef.SelfHeatKilowattsWhenActive = 4f;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    CookingStation fabricator = go.AddOrGet<CookingStation>();
    fabricator.heatedTemperature = 368.15f;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_cookstation_kanim"))
    };
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    Prioritizable.AddRef(go);
    go.AddOrGet<DropAllWorkable>();
    this.ConfigureRecipes();
    go.AddOrGetDef<PoweredController.Def>();
    BuildingTemplates.CreateComplexFabricatorStorage(go, (ComplexFabricator) fabricator);
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.CookTop, false);
  }

  private void ConfigureRecipes()
  {
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("BasicPlantFood"), 3f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("PickledMeal"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe1 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2), recipeElementArray1, recipeElementArray2);
    complexRecipe1.time = TUNING.FOOD.RECIPES.SMALL_COOK_TIME;
    complexRecipe1.description = (string) ITEMS.FOOD.PICKLEDMEAL.RECIPEDESC;
    complexRecipe1.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(Tag.op_Implicit("CookingStation"));
    complexRecipe1.fabricators = tagList1;
    complexRecipe1.sortOrder = 21;
    PickledMealConfig.recipe = complexRecipe1;
    ComplexRecipe.RecipeElement[] recipeElementArray3 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("MushBar"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray4 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("FriedMushBar"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray3, (IList<ComplexRecipe.RecipeElement>) recipeElementArray4), recipeElementArray3, recipeElementArray4);
    complexRecipe2.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe2.description = (string) ITEMS.FOOD.FRIEDMUSHBAR.RECIPEDESC;
    complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList2 = new List<Tag>();
    tagList2.Add(Tag.op_Implicit("CookingStation"));
    complexRecipe2.fabricators = tagList2;
    complexRecipe2.sortOrder = 1;
    FriedMushBarConfig.recipe = complexRecipe2;
    ComplexRecipe.RecipeElement[] recipeElementArray5 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(MushroomConfig.ID), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray6 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("FriedMushroom"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray5, (IList<ComplexRecipe.RecipeElement>) recipeElementArray6), recipeElementArray5, recipeElementArray6);
    complexRecipe3.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe3.description = (string) ITEMS.FOOD.FRIEDMUSHROOM.RECIPEDESC;
    complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList3 = new List<Tag>();
    tagList3.Add(Tag.op_Implicit("CookingStation"));
    complexRecipe3.fabricators = tagList3;
    complexRecipe3.sortOrder = 20;
    FriedMushroomConfig.recipe = complexRecipe3;
    ComplexRecipe.RecipeElement[] recipeElementArray7 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("Meat"), 2f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray8 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("CookedMeat"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray7, (IList<ComplexRecipe.RecipeElement>) recipeElementArray8), recipeElementArray7, recipeElementArray8);
    complexRecipe4.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe4.description = (string) ITEMS.FOOD.COOKEDMEAT.RECIPEDESC;
    complexRecipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList4 = new List<Tag>();
    tagList4.Add(Tag.op_Implicit("CookingStation"));
    complexRecipe4.fabricators = tagList4;
    complexRecipe4.sortOrder = 21;
    CookedMeatConfig.recipe = complexRecipe4;
    ComplexRecipe.RecipeElement[] recipeElementArray9 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("FishMeat"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray10 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("CookedFish"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe5 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray9, (IList<ComplexRecipe.RecipeElement>) recipeElementArray10), recipeElementArray9, recipeElementArray10);
    complexRecipe5.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe5.description = (string) ITEMS.FOOD.COOKEDMEAT.RECIPEDESC;
    complexRecipe5.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
    List<Tag> tagList5 = new List<Tag>();
    tagList5.Add(Tag.op_Implicit("CookingStation"));
    complexRecipe5.fabricators = tagList5;
    complexRecipe5.sortOrder = 22;
    CookedMeatConfig.recipe = complexRecipe5;
    ComplexRecipe.RecipeElement[] recipeElementArray11 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("ShellfishMeat"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray12 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("CookedFish"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe6 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray11, (IList<ComplexRecipe.RecipeElement>) recipeElementArray12), recipeElementArray11, recipeElementArray12);
    complexRecipe6.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe6.description = (string) ITEMS.FOOD.COOKEDMEAT.RECIPEDESC;
    complexRecipe6.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
    List<Tag> tagList6 = new List<Tag>();
    tagList6.Add(Tag.op_Implicit("CookingStation"));
    complexRecipe6.fabricators = tagList6;
    complexRecipe6.sortOrder = 22;
    CookedMeatConfig.recipe = complexRecipe6;
    ComplexRecipe.RecipeElement[] recipeElementArray13 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(PrickleFruitConfig.ID), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray14 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("GrilledPrickleFruit"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe7 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray13, (IList<ComplexRecipe.RecipeElement>) recipeElementArray14), recipeElementArray13, recipeElementArray14);
    complexRecipe7.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe7.description = (string) ITEMS.FOOD.GRILLEDPRICKLEFRUIT.RECIPEDESC;
    complexRecipe7.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList7 = new List<Tag>();
    tagList7.Add(Tag.op_Implicit("CookingStation"));
    complexRecipe7.fabricators = tagList7;
    complexRecipe7.sortOrder = 20;
    GrilledPrickleFruitConfig.recipe = complexRecipe7;
    if (DlcManager.IsExpansion1Active())
    {
      ComplexRecipe.RecipeElement[] recipeElementArray15 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(Tag.op_Implicit(SwampFruitConfig.ID), 1f)
      };
      ComplexRecipe.RecipeElement[] recipeElementArray16 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(Tag.op_Implicit("SwampDelights"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
      };
      ComplexRecipe complexRecipe8 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray15, (IList<ComplexRecipe.RecipeElement>) recipeElementArray16), recipeElementArray15, recipeElementArray16);
      complexRecipe8.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
      complexRecipe8.description = (string) ITEMS.FOOD.SWAMPDELIGHTS.RECIPEDESC;
      complexRecipe8.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
      List<Tag> tagList8 = new List<Tag>();
      tagList8.Add(Tag.op_Implicit("CookingStation"));
      complexRecipe8.fabricators = tagList8;
      complexRecipe8.sortOrder = 20;
      CookedEggConfig.recipe = complexRecipe8;
    }
    ComplexRecipe.RecipeElement[] recipeElementArray17 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("ColdWheatSeed"), 3f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray18 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("ColdWheatBread"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe9 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray17, (IList<ComplexRecipe.RecipeElement>) recipeElementArray18), recipeElementArray17, recipeElementArray18);
    complexRecipe9.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe9.description = (string) ITEMS.FOOD.COLDWHEATBREAD.RECIPEDESC;
    complexRecipe9.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList9 = new List<Tag>();
    tagList9.Add(Tag.op_Implicit("CookingStation"));
    complexRecipe9.fabricators = tagList9;
    complexRecipe9.sortOrder = 50;
    ColdWheatBreadConfig.recipe = complexRecipe9;
    ComplexRecipe.RecipeElement[] recipeElementArray19 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("RawEgg"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray20 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("CookedEgg"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe10 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray19, (IList<ComplexRecipe.RecipeElement>) recipeElementArray20), recipeElementArray19, recipeElementArray20);
    complexRecipe10.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe10.description = (string) ITEMS.FOOD.COOKEDEGG.RECIPEDESC;
    complexRecipe10.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList10 = new List<Tag>();
    tagList10.Add(Tag.op_Implicit("CookingStation"));
    complexRecipe10.fabricators = tagList10;
    complexRecipe10.sortOrder = 1;
    CookedEggConfig.recipe = complexRecipe10;
    if (DlcManager.IsExpansion1Active())
    {
      ComplexRecipe.RecipeElement[] recipeElementArray21 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(Tag.op_Implicit("WormBasicFruit"), 1f)
      };
      ComplexRecipe.RecipeElement[] recipeElementArray22 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(Tag.op_Implicit("WormBasicFood"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
      };
      ComplexRecipe complexRecipe11 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray21, (IList<ComplexRecipe.RecipeElement>) recipeElementArray22), recipeElementArray21, recipeElementArray22);
      complexRecipe11.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
      complexRecipe11.description = (string) ITEMS.FOOD.WORMBASICFOOD.RECIPEDESC;
      complexRecipe11.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
      List<Tag> tagList11 = new List<Tag>();
      tagList11.Add(Tag.op_Implicit("CookingStation"));
      complexRecipe11.fabricators = tagList11;
      complexRecipe11.sortOrder = 20;
      WormBasicFoodConfig.recipe = complexRecipe11;
    }
    if (!DlcManager.IsExpansion1Active())
      return;
    ComplexRecipe.RecipeElement[] recipeElementArray23 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("WormSuperFruit"), 8f),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Sucrose"), 4f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray24 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("WormSuperFood"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe12 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray23, (IList<ComplexRecipe.RecipeElement>) recipeElementArray24), recipeElementArray23, recipeElementArray24);
    complexRecipe12.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe12.description = (string) ITEMS.FOOD.WORMSUPERFOOD.RECIPEDESC;
    complexRecipe12.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList12 = new List<Tag>();
    tagList12.Add(Tag.op_Implicit("CookingStation"));
    complexRecipe12.fabricators = tagList12;
    complexRecipe12.sortOrder = 20;
    WormSuperFoodConfig.recipe = complexRecipe12;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
