// Decompiled with JetBrains decompiler
// Type: SuitFabricatorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SuitFabricatorConfig : IBuildingConfig
{
  public const string ID = "SuitFabricator";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR3 = NOISE_POLLUTION.NOISY.TIER3;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR3;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SuitFabricator", 4, 3, "suit_maker_kanim", 100, 240f, tieR4, refinedMetals, 800f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 480f;
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
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_suit_fabricator_kanim"))
    };
    Prioritizable.AddRef(go);
    BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
    this.ConfigureRecipes();
  }

  private void ConfigureRecipes()
  {
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(SimHashes.Copper.CreateTag(), 300f, true),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicFabric"), 2f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Atmo_Suit"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe1 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2), recipeElementArray1, recipeElementArray2);
    complexRecipe1.time = (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
    complexRecipe1.description = (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
    complexRecipe1.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(Tag.op_Implicit("SuitFabricator"));
    complexRecipe1.fabricators = tagList1;
    complexRecipe1.requiredTech = Db.Get().TechItems.atmoSuit.parentTechId;
    complexRecipe1.sortOrder = 1;
    AtmoSuitConfig.recipe = complexRecipe1;
    ComplexRecipe.RecipeElement[] recipeElementArray3 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), 300f, true),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicFabric"), 2f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray4 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Atmo_Suit"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray3, (IList<ComplexRecipe.RecipeElement>) recipeElementArray4), recipeElementArray3, recipeElementArray4);
    complexRecipe2.time = (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
    complexRecipe2.description = (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
    complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
    List<Tag> tagList2 = new List<Tag>();
    tagList2.Add(Tag.op_Implicit("SuitFabricator"));
    complexRecipe2.fabricators = tagList2;
    complexRecipe2.requiredTech = Db.Get().TechItems.atmoSuit.parentTechId;
    complexRecipe2.sortOrder = 1;
    AtmoSuitConfig.recipe = complexRecipe2;
    ComplexRecipe.RecipeElement[] recipeElementArray5 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 300f, true),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicFabric"), 2f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray6 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Atmo_Suit"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray5, (IList<ComplexRecipe.RecipeElement>) recipeElementArray6), recipeElementArray5, recipeElementArray6);
    complexRecipe3.time = (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
    complexRecipe3.description = (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
    complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
    List<Tag> tagList3 = new List<Tag>();
    tagList3.Add(Tag.op_Implicit("SuitFabricator"));
    complexRecipe3.fabricators = tagList3;
    complexRecipe3.requiredTech = Db.Get().TechItems.atmoSuit.parentTechId;
    complexRecipe3.sortOrder = 1;
    AtmoSuitConfig.recipe = complexRecipe3;
    if (ElementLoader.FindElementByHash(SimHashes.Cobalt) != null)
    {
      ComplexRecipe.RecipeElement[] recipeElementArray7 = new ComplexRecipe.RecipeElement[2]
      {
        new ComplexRecipe.RecipeElement(SimHashes.Cobalt.CreateTag(), 300f, true),
        new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicFabric"), 2f)
      };
      ComplexRecipe.RecipeElement[] recipeElementArray8 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Atmo_Suit"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
      };
      ComplexRecipe complexRecipe4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray7, (IList<ComplexRecipe.RecipeElement>) recipeElementArray8), recipeElementArray7, recipeElementArray8);
      complexRecipe4.time = (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
      complexRecipe4.description = (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC;
      complexRecipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
      List<Tag> tagList4 = new List<Tag>();
      tagList4.Add(Tag.op_Implicit("SuitFabricator"));
      complexRecipe4.fabricators = tagList4;
      complexRecipe4.requiredTech = Db.Get().TechItems.atmoSuit.parentTechId;
      complexRecipe4.sortOrder = 1;
      AtmoSuitConfig.recipe = complexRecipe4;
    }
    ComplexRecipe.RecipeElement[] recipeElementArray9 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Worn_Atmo_Suit"), 1f, true),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicFabric"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray10 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Atmo_Suit"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe5 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray9, (IList<ComplexRecipe.RecipeElement>) recipeElementArray10), recipeElementArray9, recipeElementArray10);
    complexRecipe5.time = (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
    complexRecipe5.description = (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.REPAIR_WORN_DESC;
    complexRecipe5.nameDisplay = ComplexRecipe.RecipeNameDisplay.Custom;
    List<Tag> tagList5 = new List<Tag>();
    tagList5.Add(Tag.op_Implicit("SuitFabricator"));
    complexRecipe5.fabricators = tagList5;
    complexRecipe5.requiredTech = Db.Get().TechItems.atmoSuit.parentTechId;
    complexRecipe5.sortOrder = 2;
    AtmoSuitConfig.recipe = complexRecipe5;
    AtmoSuitConfig.recipe.customName = (string) STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.REPAIR_WORN_RECIPE_NAME;
    AtmoSuitConfig.recipe.ProductHasFacade = true;
    ComplexRecipe.RecipeElement[] recipeElementArray11 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(SimHashes.Steel.ToString()), 200f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(SimHashes.Petroleum.ToString()), 25f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray12 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Jet_Suit"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe6 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray11, (IList<ComplexRecipe.RecipeElement>) recipeElementArray12), recipeElementArray11, recipeElementArray12);
    complexRecipe6.time = (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
    complexRecipe6.description = (string) STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC;
    complexRecipe6.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
    List<Tag> tagList6 = new List<Tag>();
    tagList6.Add(Tag.op_Implicit("SuitFabricator"));
    complexRecipe6.fabricators = tagList6;
    complexRecipe6.requiredTech = Db.Get().TechItems.jetSuit.parentTechId;
    complexRecipe6.sortOrder = 3;
    JetSuitConfig.recipe = complexRecipe6;
    ComplexRecipe.RecipeElement[] recipeElementArray13 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Worn_Jet_Suit"), 1f),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicFabric"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray14 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Jet_Suit"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe7 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray13, (IList<ComplexRecipe.RecipeElement>) recipeElementArray14), recipeElementArray13, recipeElementArray14);
    complexRecipe7.time = (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
    complexRecipe7.description = (string) STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC;
    complexRecipe7.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
    List<Tag> tagList7 = new List<Tag>();
    tagList7.Add(Tag.op_Implicit("SuitFabricator"));
    complexRecipe7.fabricators = tagList7;
    complexRecipe7.requiredTech = Db.Get().TechItems.jetSuit.parentTechId;
    complexRecipe7.sortOrder = 4;
    JetSuitConfig.recipe = complexRecipe7;
    SimHashes simHashes;
    if (DlcManager.FeatureRadiationEnabled())
    {
      ComplexRecipe.RecipeElement[] recipeElementArray15 = new ComplexRecipe.RecipeElement[2]
      {
        new ComplexRecipe.RecipeElement(Tag.op_Implicit(SimHashes.Lead.ToString()), 200f),
        null
      };
      simHashes = SimHashes.Glass;
      recipeElementArray15[1] = new ComplexRecipe.RecipeElement(Tag.op_Implicit(simHashes.ToString()), 10f);
      ComplexRecipe.RecipeElement[] recipeElementArray16 = recipeElementArray15;
      ComplexRecipe.RecipeElement[] recipeElementArray17 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Lead_Suit"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
      };
      ComplexRecipe complexRecipe8 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray16, (IList<ComplexRecipe.RecipeElement>) recipeElementArray17), recipeElementArray16, recipeElementArray17);
      complexRecipe8.time = (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
      complexRecipe8.description = (string) STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.RECIPE_DESC;
      complexRecipe8.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
      List<Tag> tagList8 = new List<Tag>();
      tagList8.Add(Tag.op_Implicit("SuitFabricator"));
      complexRecipe8.fabricators = tagList8;
      complexRecipe8.requiredTech = Db.Get().TechItems.leadSuit.parentTechId;
      complexRecipe8.sortOrder = 5;
      LeadSuitConfig.recipe = complexRecipe8;
    }
    if (!DlcManager.FeatureRadiationEnabled())
      return;
    ComplexRecipe.RecipeElement[] recipeElementArray18 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Worn_Lead_Suit"), 1f),
      null
    };
    simHashes = SimHashes.Glass;
    recipeElementArray18[1] = new ComplexRecipe.RecipeElement(Tag.op_Implicit(simHashes.ToString()), 5f);
    ComplexRecipe.RecipeElement[] recipeElementArray19 = recipeElementArray18;
    ComplexRecipe.RecipeElement[] recipeElementArray20 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Lead_Suit"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe9 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", (IList<ComplexRecipe.RecipeElement>) recipeElementArray19, (IList<ComplexRecipe.RecipeElement>) recipeElementArray20), recipeElementArray19, recipeElementArray20);
    complexRecipe9.time = (float) TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME;
    complexRecipe9.description = (string) STRINGS.EQUIPMENT.PREFABS.LEAD_SUIT.RECIPE_DESC;
    complexRecipe9.nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient;
    List<Tag> tagList9 = new List<Tag>();
    tagList9.Add(Tag.op_Implicit("SuitFabricator"));
    complexRecipe9.fabricators = tagList9;
    complexRecipe9.requiredTech = Db.Get().TechItems.leadSuit.parentTechId;
    complexRecipe9.sortOrder = 6;
    LeadSuitConfig.recipe = complexRecipe9;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabInitFn += SuitFabricatorConfig.\u003C\u003Ec.\u003C\u003E9__4_0 ?? (SuitFabricatorConfig.\u003C\u003Ec.\u003C\u003E9__4_0 = new KPrefabID.PrefabFn((object) SuitFabricatorConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__4_0)));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabSpawnFn += SuitFabricatorConfig.\u003C\u003Ec.\u003C\u003E9__4_1 ?? (SuitFabricatorConfig.\u003C\u003Ec.\u003C\u003E9__4_1 = new KPrefabID.PrefabFn((object) SuitFabricatorConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__4_1)));
  }
}
