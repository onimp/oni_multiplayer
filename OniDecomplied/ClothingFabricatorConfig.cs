// Decompiled with JetBrains decompiler
// Type: ClothingFabricatorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ClothingFabricatorConfig : IBuildingConfig
{
  public const string ID = "ClothingFabricator";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ClothingFabricator", 4, 3, "clothingfactory_kanim", 100, 240f, tieR3, refinedMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 240f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PowerInputOffset = new CellOffset(2, 0);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    go.AddOrGet<DropAllWorkable>();
    Prioritizable.AddRef(go);
    ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
    go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_clothingfactory_kanim"))
    };
    go.AddOrGet<ComplexFabricatorWorkable>().AnimOffset = new Vector3(-1f, 0.0f, 0.0f);
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    this.ConfigureRecipes();
    BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
  }

  private void ConfigureRecipes()
  {
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicFabric"), (float) TUNING.EQUIPMENT.VESTS.WARM_VEST_MASS)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Warm_Vest"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe1 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2), recipeElementArray1, recipeElementArray2);
    complexRecipe1.time = TUNING.EQUIPMENT.VESTS.WARM_VEST_FABTIME;
    complexRecipe1.description = (string) STRINGS.EQUIPMENT.PREFABS.WARM_VEST.RECIPE_DESC;
    complexRecipe1.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(Tag.op_Implicit("ClothingFabricator"));
    complexRecipe1.fabricators = tagList1;
    complexRecipe1.sortOrder = 1;
    WarmVestConfig.recipe = complexRecipe1;
    ComplexRecipe.RecipeElement[] recipeElementArray3 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicFabric"), (float) TUNING.EQUIPMENT.VESTS.COOL_VEST_MASS)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray4 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Cool_Vest"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray3, (IList<ComplexRecipe.RecipeElement>) recipeElementArray4), recipeElementArray3, recipeElementArray4);
    complexRecipe2.time = TUNING.EQUIPMENT.VESTS.COOL_VEST_FABTIME;
    complexRecipe2.description = (string) STRINGS.EQUIPMENT.PREFABS.COOL_VEST.RECIPE_DESC;
    complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList2 = new List<Tag>();
    tagList2.Add(Tag.op_Implicit("ClothingFabricator"));
    complexRecipe2.fabricators = tagList2;
    complexRecipe2.sortOrder = 1;
    CoolVestConfig.recipe = complexRecipe2;
    ComplexRecipe.RecipeElement[] recipeElementArray5 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicFabric"), (float) TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray6 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Funky_Vest"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray5, (IList<ComplexRecipe.RecipeElement>) recipeElementArray6), recipeElementArray5, recipeElementArray6);
    complexRecipe3.time = TUNING.EQUIPMENT.VESTS.FUNKY_VEST_FABTIME;
    complexRecipe3.description = (string) STRINGS.EQUIPMENT.PREFABS.FUNKY_VEST.RECIPE_DESC;
    complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList3 = new List<Tag>();
    tagList3.Add(Tag.op_Implicit("ClothingFabricator"));
    complexRecipe3.fabricators = tagList3;
    complexRecipe3.sortOrder = 1;
    FunkyVestConfig.recipe = complexRecipe3;
  }

  public override void DoPostConfigureComplete(GameObject go) => go.GetComponent<KPrefabID>().prefabSpawnFn += ClothingFabricatorConfig.\u003C\u003Ec.\u003C\u003E9__4_0 ?? (ClothingFabricatorConfig.\u003C\u003Ec.\u003C\u003E9__4_0 = new KPrefabID.PrefabFn((object) ClothingFabricatorConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__4_0)));
}
