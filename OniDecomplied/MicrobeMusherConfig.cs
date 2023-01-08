// Decompiled with JetBrains decompiler
// Type: MicrobeMusherConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MicrobeMusherConfig : IBuildingConfig
{
  public const string ID = "MicrobeMusher";
  public static EffectorValues DECOR = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR3 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues decor = MicrobeMusherConfig.DECOR;
    EffectorValues noise = tieR3;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MicrobeMusher", 2, 3, "microbemusher_kanim", 30, 30f, tieR4, allMetals, 800f, BuildLocationRule.OnFloor, decor, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 240f;
    buildingDef.ExhaustKilowattsWhenActive = 0.5f;
    buildingDef.SelfHeatKilowattsWhenActive = 2f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Glass";
    buildingDef.AudioSize = "large";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<DropAllWorkable>();
    Prioritizable.AddRef(go);
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    go.AddOrGet<ConduitConsumer>().conduitType = ConduitType.Liquid;
    MicrobeMusher fabricator = go.AddOrGet<MicrobeMusher>();
    fabricator.mushbarSpawnOffset = new Vector3(1f, 0.0f, 0.0f);
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_musher_kanim"))
    };
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    BuildingTemplates.CreateComplexFabricatorStorage(go, (ComplexFabricator) fabricator);
    this.ConfigureRecipes();
    go.AddOrGetDef<PoweredController.Def>();
  }

  private void ConfigureRecipes()
  {
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Dirt"), 75f),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Water"), 75f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("MushBar"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe1 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MicrobeMusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2), recipeElementArray1, recipeElementArray2);
    complexRecipe1.time = 40f;
    complexRecipe1.description = (string) ITEMS.FOOD.MUSHBAR.RECIPEDESC;
    complexRecipe1.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(Tag.op_Implicit("MicrobeMusher"));
    complexRecipe1.fabricators = tagList1;
    complexRecipe1.sortOrder = 1;
    MushBarConfig.recipe = complexRecipe1;
    ComplexRecipe.RecipeElement[] recipeElementArray3 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("BasicPlantFood"), 2f),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Water"), 50f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray4 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicPlantBar"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MicrobeMusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray3, (IList<ComplexRecipe.RecipeElement>) recipeElementArray4), recipeElementArray3, recipeElementArray4);
    complexRecipe2.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe2.description = (string) ITEMS.FOOD.BASICPLANTBAR.RECIPEDESC;
    complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList2 = new List<Tag>();
    tagList2.Add(Tag.op_Implicit("MicrobeMusher"));
    complexRecipe2.fabricators = tagList2;
    complexRecipe2.sortOrder = 2;
    BasicPlantBarConfig.recipe = complexRecipe2;
    ComplexRecipe.RecipeElement[] recipeElementArray5 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("BeanPlantSeed"), 6f),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Water"), 50f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray6 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Tofu"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MicrobeMusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray5, (IList<ComplexRecipe.RecipeElement>) recipeElementArray6), recipeElementArray5, recipeElementArray6);
    complexRecipe3.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe3.description = (string) ITEMS.FOOD.TOFU.RECIPEDESC;
    complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList3 = new List<Tag>();
    tagList3.Add(Tag.op_Implicit("MicrobeMusher"));
    complexRecipe3.fabricators = tagList3;
    complexRecipe3.sortOrder = 3;
    TofuConfig.recipe = complexRecipe3;
    ComplexRecipe.RecipeElement[] recipeElementArray7 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("ColdWheatSeed"), 5f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(PrickleFruitConfig.ID), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray8 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("FruitCake"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MicrobeMusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray7, (IList<ComplexRecipe.RecipeElement>) recipeElementArray8), recipeElementArray7, recipeElementArray8);
    complexRecipe4.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe4.description = (string) ITEMS.FOOD.FRUITCAKE.RECIPEDESC;
    complexRecipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList4 = new List<Tag>();
    tagList4.Add(Tag.op_Implicit("MicrobeMusher"));
    complexRecipe4.fabricators = tagList4;
    complexRecipe4.sortOrder = 3;
    FruitCakeConfig.recipe = complexRecipe4;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
