// Decompiled with JetBrains decompiler
// Type: GourmetCookingStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GourmetCookingStationConfig : IBuildingConfig
{
  public const string ID = "GourmetCookingStation";
  private const float FUEL_STORE_CAPACITY = 10f;
  private const float FUEL_CONSUME_RATE = 0.1f;
  private const float CO2_EMIT_RATE = 0.025f;
  private Tag FUEL_TAG = new Tag("Methane");
  private static readonly List<Storage.StoredItemModifier> GourmetCookingStationStoredItemModifiers = new List<Storage.StoredItemModifier>()
  {
    Storage.StoredItemModifier.Hide,
    Storage.StoredItemModifier.Preserve,
    Storage.StoredItemModifier.Insulate,
    Storage.StoredItemModifier.Seal
  };

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR3 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR3;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GourmetCookingStation", 3, 3, "cookstation_gourmet_kanim", 30, 30f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "large";
    buildingDef.EnergyConsumptionWhenActive = 240f;
    buildingDef.ExhaustKilowattsWhenActive = 1f;
    buildingDef.SelfHeatKilowattsWhenActive = 8f;
    buildingDef.InputConduitType = ConduitType.Gas;
    buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
    buildingDef.PowerInputOffset = new CellOffset(1, 0);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<DropAllWorkable>();
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    GourmetCookingStation fabricator = go.AddOrGet<GourmetCookingStation>();
    fabricator.heatedTemperature = 368.15f;
    fabricator.duplicantOperated = true;
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    go.AddOrGet<ComplexFabricatorWorkable>();
    BuildingTemplates.CreateComplexFabricatorStorage(go, (ComplexFabricator) fabricator);
    fabricator.fuelTag = this.FUEL_TAG;
    fabricator.outStorage.capacityKg = 10f;
    fabricator.inStorage.SetDefaultStoredItemModifiers(GourmetCookingStationConfig.GourmetCookingStationStoredItemModifiers);
    fabricator.buildStorage.SetDefaultStoredItemModifiers(GourmetCookingStationConfig.GourmetCookingStationStoredItemModifiers);
    fabricator.outStorage.SetDefaultStoredItemModifiers(GourmetCookingStationConfig.GourmetCookingStationStoredItemModifiers);
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.capacityTag = this.FUEL_TAG;
    conduitConsumer.capacityKG = 10f;
    conduitConsumer.alwaysConsume = true;
    conduitConsumer.storage = fabricator.inStorage;
    conduitConsumer.forceAlwaysSatisfied = true;
    ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
    elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
    {
      new ElementConverter.ConsumedElement(this.FUEL_TAG, 0.1f)
    };
    elementConverter.outputElements = new ElementConverter.OutputElement[1]
    {
      new ElementConverter.OutputElement(0.025f, SimHashes.CarbonDioxide, 348.15f, outputElementOffsety: 2f)
    };
    this.ConfigureRecipes();
    Prioritizable.AddRef(go);
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.CookTop, false);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGetDef<PoweredActiveStoppableController.Def>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabSpawnFn += GourmetCookingStationConfig.\u003C\u003Ec.\u003C\u003E9__8_0 ?? (GourmetCookingStationConfig.\u003C\u003Ec.\u003C\u003E9__8_0 = new KPrefabID.PrefabFn((object) GourmetCookingStationConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__8_0)));
  }

  private void ConfigureRecipes()
  {
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("GrilledPrickleFruit"), 2f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(SpiceNutConfig.ID), 2f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("Salsa"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe1 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2), recipeElementArray1, recipeElementArray2);
    complexRecipe1.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe1.description = (string) ITEMS.FOOD.SALSA.RECIPEDESC;
    complexRecipe1.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(Tag.op_Implicit("GourmetCookingStation"));
    complexRecipe1.fabricators = tagList1;
    complexRecipe1.sortOrder = 300;
    SalsaConfig.recipe = complexRecipe1;
    ComplexRecipe.RecipeElement[] recipeElementArray3 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("FriedMushroom"), 1f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("Lettuce"), 4f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray4 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("MushroomWrap"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray3, (IList<ComplexRecipe.RecipeElement>) recipeElementArray4), recipeElementArray3, recipeElementArray4);
    complexRecipe2.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe2.description = (string) ITEMS.FOOD.MUSHROOMWRAP.RECIPEDESC;
    complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList2 = new List<Tag>();
    tagList2.Add(Tag.op_Implicit("GourmetCookingStation"));
    complexRecipe2.fabricators = tagList2;
    complexRecipe2.sortOrder = 400;
    MushroomWrapConfig.recipe = complexRecipe2;
    ComplexRecipe.RecipeElement[] recipeElementArray5 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("CookedMeat"), 1f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("CookedFish"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray6 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("SurfAndTurf"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray5, (IList<ComplexRecipe.RecipeElement>) recipeElementArray6), recipeElementArray5, recipeElementArray6);
    complexRecipe3.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe3.description = (string) ITEMS.FOOD.SURFANDTURF.RECIPEDESC;
    complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList3 = new List<Tag>();
    tagList3.Add(Tag.op_Implicit("GourmetCookingStation"));
    complexRecipe3.fabricators = tagList3;
    complexRecipe3.sortOrder = 500;
    SurfAndTurfConfig.recipe = complexRecipe3;
    ComplexRecipe.RecipeElement[] recipeElementArray7 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("ColdWheatSeed"), 10f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(SpiceNutConfig.ID), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray8 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("SpiceBread"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray7, (IList<ComplexRecipe.RecipeElement>) recipeElementArray8), recipeElementArray7, recipeElementArray8);
    complexRecipe4.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe4.description = (string) ITEMS.FOOD.SPICEBREAD.RECIPEDESC;
    complexRecipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList4 = new List<Tag>();
    tagList4.Add(Tag.op_Implicit("GourmetCookingStation"));
    complexRecipe4.fabricators = tagList4;
    complexRecipe4.sortOrder = 600;
    SpiceBreadConfig.recipe = complexRecipe4;
    ComplexRecipe.RecipeElement[] recipeElementArray9 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("Tofu"), 1f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(SpiceNutConfig.ID), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray10 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("SpicyTofu"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe5 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray9, (IList<ComplexRecipe.RecipeElement>) recipeElementArray10), recipeElementArray9, recipeElementArray10);
    complexRecipe5.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe5.description = (string) ITEMS.FOOD.SPICYTOFU.RECIPEDESC;
    complexRecipe5.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList5 = new List<Tag>();
    tagList5.Add(Tag.op_Implicit("GourmetCookingStation"));
    complexRecipe5.fabricators = tagList5;
    complexRecipe5.sortOrder = 800;
    SpicyTofuConfig.recipe = complexRecipe5;
    ComplexRecipe.RecipeElement[] recipeElementArray11 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(GingerConfig.ID), 4f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("BeanPlantSeed"), 4f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray12 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("Curry"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe6 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray11, (IList<ComplexRecipe.RecipeElement>) recipeElementArray12), recipeElementArray11, recipeElementArray12);
    complexRecipe6.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe6.description = (string) ITEMS.FOOD.CURRY.RECIPEDESC;
    complexRecipe6.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList6 = new List<Tag>();
    tagList6.Add(Tag.op_Implicit("GourmetCookingStation"));
    complexRecipe6.fabricators = tagList6;
    complexRecipe6.sortOrder = 800;
    SpicyTofuConfig.recipe = complexRecipe6;
    ComplexRecipe.RecipeElement[] recipeElementArray13 = new ComplexRecipe.RecipeElement[3]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("ColdWheatBread"), 1f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("Lettuce"), 1f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("CookedMeat"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray14 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("Burger"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe7 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray13, (IList<ComplexRecipe.RecipeElement>) recipeElementArray14), recipeElementArray13, recipeElementArray14);
    complexRecipe7.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe7.description = (string) ITEMS.FOOD.BURGER.RECIPEDESC;
    complexRecipe7.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList7 = new List<Tag>();
    tagList7.Add(Tag.op_Implicit("GourmetCookingStation"));
    complexRecipe7.fabricators = tagList7;
    complexRecipe7.sortOrder = 900;
    BurgerConfig.recipe = complexRecipe7;
    if (!DlcManager.IsExpansion1Active())
      return;
    ComplexRecipe.RecipeElement[] recipeElementArray15 = new ComplexRecipe.RecipeElement[3]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("ColdWheatSeed"), 3f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("WormSuperFruit"), 4f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("GrilledPrickleFruit"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray16 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("BerryPie"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe8 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("GourmetCookingStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray15, (IList<ComplexRecipe.RecipeElement>) recipeElementArray16), recipeElementArray15, recipeElementArray16);
    complexRecipe8.time = TUNING.FOOD.RECIPES.STANDARD_COOK_TIME;
    complexRecipe8.description = (string) ITEMS.FOOD.BERRYPIE.RECIPEDESC;
    complexRecipe8.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList8 = new List<Tag>();
    tagList8.Add(Tag.op_Implicit("GourmetCookingStation"));
    complexRecipe8.fabricators = tagList8;
    complexRecipe8.sortOrder = 900;
    BerryPieConfig.recipe = complexRecipe8;
  }
}
