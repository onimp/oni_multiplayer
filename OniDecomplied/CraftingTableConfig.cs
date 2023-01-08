// Decompiled with JetBrains decompiler
// Type: CraftingTableConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class CraftingTableConfig : IBuildingConfig
{
  public const string ID = "CraftingTable";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues tieR3_2 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR3_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CraftingTable", 2, 2, "craftingStation_kanim", 100, 30f, tieR3_1, rawMetals, 800f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 60f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PowerInputOffset = new CellOffset(1, 0);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<DropAllWorkable>();
    go.AddOrGet<Prioritizable>();
    ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
    fabricator.heatedTemperature = 318.15f;
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_craftingstation_kanim"))
    };
    Prioritizable.AddRef(go);
    BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
    this.ConfigureRecipes();
  }

  private void ConfigureRecipes()
  {
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(SimHashes.Cuprite.CreateTag(), 50f, true)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Oxygen_Mask"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe1 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2), recipeElementArray1, recipeElementArray2);
    complexRecipe1.time = (float) TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME;
    complexRecipe1.description = (string) STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC;
    complexRecipe1.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(Tag.op_Implicit("CraftingTable"));
    complexRecipe1.fabricators = tagList1;
    complexRecipe1.requiredTech = Db.Get().TechItems.oxygenMask.parentTechId;
    AtmoSuitConfig.recipe = complexRecipe1;
    ComplexRecipe.RecipeElement[] recipeElementArray3 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(SimHashes.AluminumOre.CreateTag(), 50f, true)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray4 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Oxygen_Mask"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", (IList<ComplexRecipe.RecipeElement>) recipeElementArray3, (IList<ComplexRecipe.RecipeElement>) recipeElementArray4), recipeElementArray3, recipeElementArray4);
    complexRecipe2.time = (float) TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME;
    complexRecipe2.description = (string) STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC;
    complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
    List<Tag> tagList2 = new List<Tag>();
    tagList2.Add(Tag.op_Implicit("CraftingTable"));
    complexRecipe2.fabricators = tagList2;
    complexRecipe2.requiredTech = Db.Get().TechItems.oxygenMask.parentTechId;
    AtmoSuitConfig.recipe = complexRecipe2;
    ComplexRecipe.RecipeElement[] recipeElementArray5 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(SimHashes.IronOre.CreateTag(), 50f, true)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray6 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Oxygen_Mask"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", (IList<ComplexRecipe.RecipeElement>) recipeElementArray5, (IList<ComplexRecipe.RecipeElement>) recipeElementArray6), recipeElementArray5, recipeElementArray6);
    complexRecipe3.time = (float) TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME;
    complexRecipe3.description = (string) STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC;
    complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
    List<Tag> tagList3 = new List<Tag>();
    tagList3.Add(Tag.op_Implicit("CraftingTable"));
    complexRecipe3.fabricators = tagList3;
    complexRecipe3.requiredTech = Db.Get().TechItems.oxygenMask.parentTechId;
    AtmoSuitConfig.recipe = complexRecipe3;
    if (ElementLoader.FindElementByHash(SimHashes.Cobaltite) != null)
    {
      ComplexRecipe.RecipeElement[] recipeElementArray7 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(SimHashes.Cobaltite.CreateTag(), 50f, true)
      };
      ComplexRecipe.RecipeElement[] recipeElementArray8 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Oxygen_Mask"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
      };
      ComplexRecipe complexRecipe4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", (IList<ComplexRecipe.RecipeElement>) recipeElementArray7, (IList<ComplexRecipe.RecipeElement>) recipeElementArray8), recipeElementArray7, recipeElementArray8);
      complexRecipe4.time = (float) TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME;
      complexRecipe4.description = (string) STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC;
      complexRecipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
      List<Tag> tagList4 = new List<Tag>();
      tagList4.Add(Tag.op_Implicit("CraftingTable"));
      complexRecipe4.fabricators = tagList4;
      complexRecipe4.requiredTech = Db.Get().TechItems.oxygenMask.parentTechId;
      AtmoSuitConfig.recipe = complexRecipe4;
    }
    ComplexRecipe.RecipeElement[] recipeElementArray9 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Worn_Oxygen_Mask"), 1f, true)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray10 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Oxygen_Mask"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe5 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", (IList<ComplexRecipe.RecipeElement>) recipeElementArray9, (IList<ComplexRecipe.RecipeElement>) recipeElementArray10), recipeElementArray9, recipeElementArray10);
    complexRecipe5.time = (float) TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME;
    complexRecipe5.description = (string) STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC;
    complexRecipe5.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
    List<Tag> tagList5 = new List<Tag>();
    tagList5.Add(Tag.op_Implicit("CraftingTable"));
    complexRecipe5.fabricators = tagList5;
    complexRecipe5.requiredTech = Db.Get().TechItems.oxygenMask.parentTechId;
    AtmoSuitConfig.recipe = complexRecipe5;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabInitFn += CraftingTableConfig.\u003C\u003Ec.\u003C\u003E9__5_0 ?? (CraftingTableConfig.\u003C\u003Ec.\u003C\u003E9__5_0 = new KPrefabID.PrefabFn((object) CraftingTableConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__5_0)));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabSpawnFn += CraftingTableConfig.\u003C\u003Ec.\u003C\u003E9__5_1 ?? (CraftingTableConfig.\u003C\u003Ec.\u003C\u003E9__5_1 = new KPrefabID.PrefabFn((object) CraftingTableConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__5_1)));
  }
}
