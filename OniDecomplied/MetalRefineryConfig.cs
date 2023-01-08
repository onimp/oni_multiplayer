// Decompiled with JetBrains decompiler
// Type: MetalRefineryConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MetalRefineryConfig : IBuildingConfig
{
  public const string ID = "MetalRefinery";
  private const float INPUT_KG = 100f;
  private const float LIQUID_COOLED_HEAT_PORTION = 0.8f;
  private static readonly Tag COOLANT_TAG = GameTags.Liquid;
  private const float COOLANT_MASS = 400f;
  private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>()
  {
    Storage.StoredItemModifier.Hide,
    Storage.StoredItemModifier.Preserve,
    Storage.StoredItemModifier.Insulate,
    Storage.StoredItemModifier.Seal
  };

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] allMinerals = MATERIALS.ALL_MINERALS;
    EffectorValues tieR6 = NOISE_POLLUTION.NOISY.TIER6;
    EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = tieR6;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MetalRefinery", 3, 4, "metalrefinery_kanim", 30, 60f, tieR5, allMinerals, 2400f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 1200f;
    buildingDef.SelfHeatKilowattsWhenActive = 16f;
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.UtilityInputOffset = new CellOffset(-1, 1);
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
    LiquidCooledRefinery fabricator = go.AddOrGet<LiquidCooledRefinery>();
    fabricator.duplicantOperated = true;
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    fabricator.keepExcessLiquids = true;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    ComplexFabricatorWorkable fabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
    BuildingTemplates.CreateComplexFabricatorStorage(go, (ComplexFabricator) fabricator);
    fabricator.coolantTag = MetalRefineryConfig.COOLANT_TAG;
    fabricator.minCoolantMass = 400f;
    fabricator.outStorage.capacityKg = 2000f;
    fabricator.thermalFudge = 0.8f;
    fabricator.inStorage.SetDefaultStoredItemModifiers(MetalRefineryConfig.RefineryStoredItemModifiers);
    fabricator.buildStorage.SetDefaultStoredItemModifiers(MetalRefineryConfig.RefineryStoredItemModifiers);
    fabricator.outStorage.SetDefaultStoredItemModifiers(MetalRefineryConfig.RefineryStoredItemModifiers);
    fabricator.outputOffset = new Vector3(1f, 0.5f);
    KAnimFile[] kanimFileArray = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_metalrefinery_kanim"))
    };
    fabricatorWorkable.overrideAnims = kanimFileArray;
    go.AddOrGet<RequireOutputs>().ignoreFullPipe = true;
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.capacityTag = GameTags.Liquid;
    conduitConsumer.capacityKG = 800f;
    conduitConsumer.storage = fabricator.inStorage;
    conduitConsumer.alwaysConsume = true;
    conduitConsumer.forceAlwaysSatisfied = true;
    ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
    conduitDispenser.storage = fabricator.outStorage;
    conduitDispenser.conduitType = ConduitType.Liquid;
    conduitDispenser.elementFilter = (SimHashes[]) null;
    conduitDispenser.alwaysDispense = true;
    foreach (Element element in ElementLoader.elements.FindAll((Predicate<Element>) (e => e.IsSolid && e.HasTag(GameTags.Metal))))
    {
      if (!element.HasTag(GameTags.Noncrushable))
      {
        Element lowTempTransition = element.highTempTransition.lowTempTransition;
        if (lowTempTransition != element)
        {
          ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[1]
          {
            new ComplexRecipe.RecipeElement(element.tag, 100f)
          };
          ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
          {
            new ComplexRecipe.RecipeElement(lowTempTransition.tag, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
          };
          string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID("MetalRefinery", element.tag);
          string str = ComplexRecipeManager.MakeRecipeID("MetalRefinery", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2);
          ComplexRecipe complexRecipe = new ComplexRecipe(str, recipeElementArray1, recipeElementArray2);
          complexRecipe.time = 40f;
          complexRecipe.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.METALREFINERY.RECIPE_DESCRIPTION, (object) lowTempTransition.name, (object) element.name);
          complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
          List<Tag> tagList = new List<Tag>();
          tagList.Add(TagManager.Create("MetalRefinery"));
          complexRecipe.fabricators = tagList;
          ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, str);
        }
      }
    }
    Element elementByHash = ElementLoader.FindElementByHash(SimHashes.Steel);
    ComplexRecipe.RecipeElement[] recipeElementArray3 = new ComplexRecipe.RecipeElement[3]
    {
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Iron).tag, 70f),
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.RefinedCarbon).tag, 20f),
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 10f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray4 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Steel).tag, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    string obsolete_id1 = ComplexRecipeManager.MakeObsoleteRecipeID("MetalRefinery", elementByHash.tag);
    string str1 = ComplexRecipeManager.MakeRecipeID("MetalRefinery", (IList<ComplexRecipe.RecipeElement>) recipeElementArray3, (IList<ComplexRecipe.RecipeElement>) recipeElementArray4);
    ComplexRecipe complexRecipe1 = new ComplexRecipe(str1, recipeElementArray3, recipeElementArray4);
    complexRecipe1.time = 40f;
    complexRecipe1.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
    complexRecipe1.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.METALREFINERY.RECIPE_DESCRIPTION, (object) ElementLoader.FindElementByHash(SimHashes.Steel).name, (object) ElementLoader.FindElementByHash(SimHashes.Iron).name);
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(TagManager.Create("MetalRefinery"));
    complexRecipe1.fabricators = tagList1;
    ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id1, str1);
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    SymbolOverrideControllerUtil.AddToPrefab(go);
    go.AddOrGetDef<PoweredActiveStoppableController.Def>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabSpawnFn += MetalRefineryConfig.\u003C\u003Ec.\u003C\u003E9__8_0 ?? (MetalRefineryConfig.\u003C\u003Ec.\u003C\u003E9__8_0 = new KPrefabID.PrefabFn((object) MetalRefineryConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__8_0)));
  }
}
