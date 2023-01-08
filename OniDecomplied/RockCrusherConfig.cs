// Decompiled with JetBrains decompiler
// Type: RockCrusherConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class RockCrusherConfig : IBuildingConfig
{
  public const string ID = "RockCrusher";
  private const float INPUT_KG = 100f;
  private const float METAL_ORE_EFFICIENCY = 0.5f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
    EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR6;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RockCrusher", 4, 4, "rockrefinery_kanim", 30, 60f, tieR5, allMetals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 240f;
    buildingDef.SelfHeatKilowattsWhenActive = 16f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "HollowMetal";
    buildingDef.AudioSize = "large";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<DropAllWorkable>();
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    fabricator.duplicantOperated = true;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    ComplexFabricatorWorkable fabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
    BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
    fabricatorWorkable.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_rockrefinery_kanim"))
    };
    fabricatorWorkable.workingPstComplete = new HashedString[1]
    {
      HashedString.op_Implicit("working_pst_complete")
    };
    Tag tag = SimHashes.Sand.CreateTag();
    foreach (Element element in ElementLoader.elements.FindAll((Predicate<Element>) (e => e.HasTag(GameTags.Crushable))))
    {
      ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(element.tag, 100f)
      };
      ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(tag, 100f)
      };
      string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", element.tag);
      string str = ComplexRecipeManager.MakeRecipeID("RockCrusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2);
      ComplexRecipe complexRecipe = new ComplexRecipe(str, recipeElementArray1, recipeElementArray2);
      complexRecipe.time = 40f;
      complexRecipe.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, (object) element.name, (object) tag.ProperName());
      complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
      List<Tag> tagList = new List<Tag>();
      tagList.Add(TagManager.Create("RockCrusher"));
      complexRecipe.fabricators = tagList;
      ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, str);
    }
    foreach (Element element in ElementLoader.elements.FindAll((Predicate<Element>) (e => e.IsSolid && e.HasTag(GameTags.Metal))))
    {
      if (!element.HasTag(GameTags.Noncrushable))
      {
        Element lowTempTransition = element.highTempTransition.lowTempTransition;
        if (lowTempTransition != element)
        {
          ComplexRecipe.RecipeElement[] recipeElementArray3 = new ComplexRecipe.RecipeElement[1]
          {
            new ComplexRecipe.RecipeElement(element.tag, 100f)
          };
          ComplexRecipe.RecipeElement[] recipeElementArray4 = new ComplexRecipe.RecipeElement[2]
          {
            new ComplexRecipe.RecipeElement(lowTempTransition.tag, 50f),
            new ComplexRecipe.RecipeElement(tag, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
          };
          string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", lowTempTransition.tag);
          string str = ComplexRecipeManager.MakeRecipeID("RockCrusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray3, (IList<ComplexRecipe.RecipeElement>) recipeElementArray4);
          ComplexRecipe complexRecipe = new ComplexRecipe(str, recipeElementArray3, recipeElementArray4);
          complexRecipe.time = 40f;
          complexRecipe.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.METAL_RECIPE_DESCRIPTION, (object) lowTempTransition.name, (object) element.name);
          complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
          List<Tag> tagList = new List<Tag>();
          tagList.Add(TagManager.Create("RockCrusher"));
          complexRecipe.fabricators = tagList;
          ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, str);
        }
      }
    }
    Element elementByHash1 = ElementLoader.FindElementByHash(SimHashes.Lime);
    ComplexRecipe.RecipeElement[] recipeElementArray5 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("EggShell"), 5f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray6 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    string obsolete_id1 = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", elementByHash1.tag);
    string str1 = ComplexRecipeManager.MakeRecipeID("RockCrusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray5, (IList<ComplexRecipe.RecipeElement>) recipeElementArray6);
    ComplexRecipe complexRecipe1 = new ComplexRecipe(str1, recipeElementArray5, recipeElementArray6);
    complexRecipe1.time = 40f;
    complexRecipe1.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, (object) SimHashes.Lime.CreateTag().ProperName(), (object) MISC.TAGS.EGGSHELL);
    complexRecipe1.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(TagManager.Create("RockCrusher"));
    complexRecipe1.fabricators = tagList1;
    ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id1, str1);
    Element elementByHash2 = ElementLoader.FindElementByHash(SimHashes.Lime);
    ComplexRecipe.RecipeElement[] recipeElementArray7 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("BabyCrabShell"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray8 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(elementByHash2.tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray7, (IList<ComplexRecipe.RecipeElement>) recipeElementArray8), recipeElementArray7, recipeElementArray8);
    complexRecipe2.time = 40f;
    complexRecipe2.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, (object) SimHashes.Lime.CreateTag().ProperName(), (object) ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME);
    complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
    List<Tag> tagList2 = new List<Tag>();
    tagList2.Add(TagManager.Create("RockCrusher"));
    complexRecipe2.fabricators = tagList2;
    Element elementByHash3 = ElementLoader.FindElementByHash(SimHashes.Lime);
    ComplexRecipe.RecipeElement[] recipeElementArray9 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("CrabShell"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray10 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(elementByHash3.tag, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray9, (IList<ComplexRecipe.RecipeElement>) recipeElementArray10), recipeElementArray9, recipeElementArray10);
    complexRecipe3.time = 40f;
    complexRecipe3.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, (object) SimHashes.Lime.CreateTag().ProperName(), (object) ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME);
    complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
    List<Tag> tagList3 = new List<Tag>();
    tagList3.Add(TagManager.Create("RockCrusher"));
    complexRecipe3.fabricators = tagList3;
    ComplexRecipe.RecipeElement[] recipeElementArray11 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("BabyCrabWoodShell"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray12 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("WoodLog"), 10f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray11, (IList<ComplexRecipe.RecipeElement>) recipeElementArray12), recipeElementArray11, recipeElementArray12);
    complexRecipe4.time = 40f;
    complexRecipe4.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, (object) WoodLogConfig.TAG.ProperName(), (object) ITEMS.INDUSTRIAL_PRODUCTS.BABY_CRAB_SHELL.VARIANT_WOOD.NAME);
    complexRecipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
    List<Tag> tagList4 = new List<Tag>();
    tagList4.Add(TagManager.Create("RockCrusher"));
    complexRecipe4.fabricators = tagList4;
    float amount = 5f;
    ComplexRecipe.RecipeElement[] recipeElementArray13 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("CrabWoodShell"), amount)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray14 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("WoodLog"), 100f * amount, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe5 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray13, (IList<ComplexRecipe.RecipeElement>) recipeElementArray14), recipeElementArray13, recipeElementArray14);
    complexRecipe5.time = 40f;
    complexRecipe5.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, (object) WoodLogConfig.TAG.ProperName(), (object) ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.VARIANT_WOOD.NAME);
    complexRecipe5.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
    List<Tag> tagList5 = new List<Tag>();
    tagList5.Add(TagManager.Create("RockCrusher"));
    complexRecipe5.fabricators = tagList5;
    ComplexRecipe.RecipeElement[] recipeElementArray15 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Fossil).tag, 100f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray16 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.SedimentaryRock).tag, 95f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe6 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray15, (IList<ComplexRecipe.RecipeElement>) recipeElementArray16), recipeElementArray15, recipeElementArray16);
    complexRecipe6.time = 40f;
    complexRecipe6.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_FROM_LIMESTONE_RECIPE_DESCRIPTION, (object) SimHashes.Fossil.CreateTag().ProperName(), (object) SimHashes.SedimentaryRock.CreateTag().ProperName(), (object) SimHashes.Lime.CreateTag().ProperName());
    complexRecipe6.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
    List<Tag> tagList6 = new List<Tag>();
    tagList6.Add(TagManager.Create("RockCrusher"));
    complexRecipe6.fabricators = tagList6;
    float num1 = 5E-05f;
    ComplexRecipe.RecipeElement[] recipeElementArray17 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 100f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray18 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag(TableSaltConfig.ID), 100f * num1, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
      new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), (float) (100.0 * (1.0 - (double) num1)), ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe7 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray17, (IList<ComplexRecipe.RecipeElement>) recipeElementArray18), recipeElementArray17, recipeElementArray18);
    complexRecipe7.time = 40f;
    complexRecipe7.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, (object) SimHashes.Salt.CreateTag().ProperName(), (object) ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.NAME);
    complexRecipe7.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
    List<Tag> tagList7 = new List<Tag>();
    tagList7.Add(TagManager.Create("RockCrusher"));
    complexRecipe7.fabricators = tagList7;
    if (ElementLoader.FindElementByHash(SimHashes.Graphite) != null)
    {
      float num2 = 0.9f;
      ComplexRecipe.RecipeElement[] recipeElementArray19 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(SimHashes.Fullerene.CreateTag(), 100f)
      };
      ComplexRecipe.RecipeElement[] recipeElementArray20 = new ComplexRecipe.RecipeElement[2]
      {
        new ComplexRecipe.RecipeElement(SimHashes.Graphite.CreateTag(), 100f * num2, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
        new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), (float) (100.0 * (1.0 - (double) num2)), ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
      };
      ComplexRecipe complexRecipe8 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", (IList<ComplexRecipe.RecipeElement>) recipeElementArray19, (IList<ComplexRecipe.RecipeElement>) recipeElementArray20), recipeElementArray19, recipeElementArray20);
      complexRecipe8.time = 40f;
      complexRecipe8.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, (object) SimHashes.Fullerene.CreateTag().ProperName(), (object) SimHashes.Graphite.CreateTag().ProperName());
      complexRecipe8.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
      List<Tag> tagList8 = new List<Tag>();
      tagList8.Add(TagManager.Create("RockCrusher"));
      complexRecipe8.fabricators = tagList8;
    }
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    SymbolOverrideControllerUtil.AddToPrefab(go);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabSpawnFn += RockCrusherConfig.\u003C\u003Ec.\u003C\u003E9__5_0 ?? (RockCrusherConfig.\u003C\u003Ec.\u003C\u003E9__5_0 = new KPrefabID.PrefabFn((object) RockCrusherConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__5_0)));
  }
}
