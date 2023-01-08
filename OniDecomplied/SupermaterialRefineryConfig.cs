// Decompiled with JetBrains decompiler
// Type: SupermaterialRefineryConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SupermaterialRefineryConfig : IBuildingConfig
{
  public const string ID = "SupermaterialRefinery";
  private const float INPUT_KG = 100f;
  private const float OUTPUT_KG = 100f;
  private const float OUTPUT_TEMPERATURE = 313.15f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
    EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR6;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SupermaterialRefinery", 4, 5, "supermaterial_refinery_kanim", 30, 480f, tieR5, allMetals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 1600f;
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
    fabricator.heatedTemperature = 313.15f;
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    fabricator.duplicantOperated = true;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    ComplexFabricatorWorkable fabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
    BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
    KAnimFile[] kanimFileArray = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_supermaterial_refinery_kanim"))
    };
    fabricatorWorkable.overrideAnims = kanimFileArray;
    Prioritizable.AddRef(go);
    float num1 = 0.01f;
    float num2 = (float) ((1.0 - (double) num1) * 0.5);
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[3]
    {
      new ComplexRecipe.RecipeElement(SimHashes.Fullerene.CreateTag(), 100f * num1),
      new ComplexRecipe.RecipeElement(SimHashes.Gold.CreateTag(), 100f * num2),
      new ComplexRecipe.RecipeElement(SimHashes.Petroleum.CreateTag(), 100f * num2)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(SimHashes.SuperCoolant.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe1 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2), recipeElementArray1, recipeElementArray2);
    complexRecipe1.time = 80f;
    complexRecipe1.description = (string) STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
    complexRecipe1.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(TagManager.Create("SupermaterialRefinery"));
    complexRecipe1.fabricators = tagList1;
    if (DlcManager.IsExpansion1Active())
    {
      float num3 = 0.9f;
      float num4 = 1f - num3;
      ComplexRecipe.RecipeElement[] recipeElementArray3 = new ComplexRecipe.RecipeElement[3]
      {
        new ComplexRecipe.RecipeElement(SimHashes.Graphite.CreateTag(), 100f * num3),
        new ComplexRecipe.RecipeElement(SimHashes.Sulfur.CreateTag(), (float) (100.0 * (double) num4 / 2.0)),
        new ComplexRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), (float) (100.0 * (double) num4 / 2.0))
      };
      ComplexRecipe.RecipeElement[] recipeElementArray4 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(SimHashes.Fullerene.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
      };
      ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", (IList<ComplexRecipe.RecipeElement>) recipeElementArray3, (IList<ComplexRecipe.RecipeElement>) recipeElementArray4), recipeElementArray3, recipeElementArray4);
      complexRecipe2.time = 80f;
      complexRecipe2.description = (string) STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.FULLERENE_RECIPE_DESCRIPTION;
      complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
      List<Tag> tagList2 = new List<Tag>();
      tagList2.Add(TagManager.Create("SupermaterialRefinery"));
      complexRecipe2.fabricators = tagList2;
    }
    float num5 = 0.15f;
    float num6 = 0.05f;
    float num7 = 1f - num6 - num5;
    ComplexRecipe.RecipeElement[] recipeElementArray5 = new ComplexRecipe.RecipeElement[3]
    {
      new ComplexRecipe.RecipeElement(SimHashes.Isoresin.CreateTag(), 100f * num5),
      new ComplexRecipe.RecipeElement(SimHashes.Katairite.CreateTag(), 100f * num7),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag(BasicFabricConfig.ID), 100f * num6)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray6 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(SimHashes.SuperInsulator.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", (IList<ComplexRecipe.RecipeElement>) recipeElementArray5, (IList<ComplexRecipe.RecipeElement>) recipeElementArray6), recipeElementArray5, recipeElementArray6);
    complexRecipe3.time = 80f;
    complexRecipe3.description = (string) STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERINSULATOR_RECIPE_DESCRIPTION;
    complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList3 = new List<Tag>();
    tagList3.Add(TagManager.Create("SupermaterialRefinery"));
    complexRecipe3.fabricators = tagList3;
    float num8 = 0.05f;
    ComplexRecipe.RecipeElement[] recipeElementArray7 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(SimHashes.Niobium.CreateTag(), 100f * num8),
      new ComplexRecipe.RecipeElement(SimHashes.Tungsten.CreateTag(), (float) (100.0 * (1.0 - (double) num8)))
    };
    ComplexRecipe.RecipeElement[] recipeElementArray8 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(SimHashes.TempConductorSolid.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", (IList<ComplexRecipe.RecipeElement>) recipeElementArray7, (IList<ComplexRecipe.RecipeElement>) recipeElementArray8), recipeElementArray7, recipeElementArray8);
    complexRecipe4.time = 80f;
    complexRecipe4.description = (string) STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.TEMPCONDUCTORSOLID_RECIPE_DESCRIPTION;
    complexRecipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList4 = new List<Tag>();
    tagList4.Add(TagManager.Create("SupermaterialRefinery"));
    complexRecipe4.fabricators = tagList4;
    float num9 = 0.35f;
    ComplexRecipe.RecipeElement[] recipeElementArray9 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(SimHashes.Isoresin.CreateTag(), 100f * num9),
      new ComplexRecipe.RecipeElement(SimHashes.Petroleum.CreateTag(), (float) (100.0 * (1.0 - (double) num9)))
    };
    ComplexRecipe.RecipeElement[] recipeElementArray10 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(SimHashes.ViscoGel.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated)
    };
    ComplexRecipe complexRecipe5 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", (IList<ComplexRecipe.RecipeElement>) recipeElementArray9, (IList<ComplexRecipe.RecipeElement>) recipeElementArray10), recipeElementArray9, recipeElementArray10);
    complexRecipe5.time = 80f;
    complexRecipe5.description = (string) STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.VISCOGEL_RECIPE_DESCRIPTION;
    complexRecipe5.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList5 = new List<Tag>();
    tagList5.Add(TagManager.Create("SupermaterialRefinery"));
    complexRecipe5.fabricators = tagList5;
  }

  public override void DoPostConfigureComplete(GameObject go) => go.GetComponent<KPrefabID>().prefabSpawnFn += SupermaterialRefineryConfig.\u003C\u003Ec.\u003C\u003E9__6_0 ?? (SupermaterialRefineryConfig.\u003C\u003Ec.\u003C\u003E9__6_0 = new KPrefabID.PrefabFn((object) SupermaterialRefineryConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__6_0)));
}
