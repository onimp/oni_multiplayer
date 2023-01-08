// Decompiled with JetBrains decompiler
// Type: SludgePressConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SludgePressConfig : IBuildingConfig
{
  public const string ID = "SludgePress";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMinerals = MATERIALS.ALL_MINERALS;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR1 = TUNING.BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SludgePress", 4, 3, "sludge_press_kanim", 100, 30f, tieR3, allMinerals, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.SelfHeatKilowattsWhenActive = 4f;
    buildingDef.OutputConduitType = ConduitType.Liquid;
    buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
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
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_sludge_press_kanim"))
    };
    fabricatorWorkable.workingPstComplete = new HashedString[1]
    {
      HashedString.op_Implicit("working_pst_complete")
    };
    ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
    conduitDispenser.conduitType = ConduitType.Liquid;
    conduitDispenser.alwaysDispense = true;
    conduitDispenser.elementFilter = (SimHashes[]) null;
    conduitDispenser.storage = go.GetComponent<ComplexFabricator>().outStorage;
    this.AddRecipes(go);
    Prioritizable.AddRef(go);
  }

  private void AddRecipes(GameObject go)
  {
    float amount = 150f;
    foreach (Element element in ElementLoader.elements.FindAll((Predicate<Element>) (e => e.elementComposition != null)))
    {
      ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(element.tag, amount)
      };
      ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[element.elementComposition.Length];
      for (int index = 0; index < element.elementComposition.Length; ++index)
      {
        ElementLoader.ElementComposition elementComposition = element.elementComposition[index];
        Element elementByName = ElementLoader.FindElementByName(elementComposition.elementID);
        bool isLiquid = elementByName.IsLiquid;
        recipeElementArray2[index] = new ComplexRecipe.RecipeElement(elementByName.tag, amount * elementComposition.percentage, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, isLiquid);
      }
      string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("SludgePress", element.tag);
      string str = ComplexRecipeManager.MakeRecipeID("SludgePress", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2);
      ComplexRecipe complexRecipe = new ComplexRecipe(str, recipeElementArray1, recipeElementArray2);
      complexRecipe.time = 20f;
      complexRecipe.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.SLUDGEPRESS.RECIPE_DESCRIPTION, (object) element.name);
      complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Composite;
      List<Tag> tagList = new List<Tag>();
      tagList.Add(TagManager.Create("SludgePress"));
      complexRecipe.fabricators = tagList;
      ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, str);
    }
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    SymbolOverrideControllerUtil.AddToPrefab(go);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabSpawnFn += SludgePressConfig.\u003C\u003Ec.\u003C\u003E9__5_0 ?? (SludgePressConfig.\u003C\u003Ec.\u003C\u003E9__5_0 = new KPrefabID.PrefabFn((object) SludgePressConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__5_0)));
  }
}
