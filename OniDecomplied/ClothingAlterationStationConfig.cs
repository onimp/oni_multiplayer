// Decompiled with JetBrains decompiler
// Type: ClothingAlterationStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ClothingAlterationStationConfig : IBuildingConfig
{
  public const string ID = "ClothingAlterationStation";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ClothingAlterationStation", 4, 3, "super_snazzy_suit_alteration_station_kanim", 100, 240f, tieR3, refinedMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 240f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PowerInputOffset = new CellOffset(0, 0);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    go.AddOrGet<DropAllWorkable>();
    Prioritizable.AddRef(go);
    ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
    fabricator.outputOffset = new Vector3(1f, 0.0f, 0.0f);
    ComplexFabricatorWorkable fabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
    fabricatorWorkable.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_super_snazzy_suit_alteration_station_kanim"))
    };
    fabricatorWorkable.workingPstComplete = new HashedString[1]
    {
      HashedString.op_Implicit("working_pst_complete")
    };
    fabricatorWorkable.AnimOffset = new Vector3(-1f, 0.0f, 0.0f);
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    this.ConfigureRecipes();
    BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
  }

  private void ConfigureRecipes()
  {
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("Funky_Vest"), 1f, false),
      new ComplexRecipe.RecipeElement(TagExtensions.ToTag("BasicFabric"), 3f)
    };
    foreach (EquippableFacadeResource equippableFacadeResource in Db.GetEquippableFacades().resources.FindAll((Predicate<EquippableFacadeResource>) (match => match.DefID == "CustomClothing")))
    {
      ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
      {
        new ComplexRecipe.RecipeElement(TagExtensions.ToTag("CustomClothing"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, equippableFacadeResource.Id)
      };
      ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingAlterationStation", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2, equippableFacadeResource.Id), recipeElementArray1, recipeElementArray2);
      complexRecipe.time = TUNING.EQUIPMENT.VESTS.CUSTOM_CLOTHING_FABTIME;
      complexRecipe.description = (string) STRINGS.EQUIPMENT.PREFABS.CUSTOMCLOTHING.RECIPE_DESC;
      complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
      List<Tag> tagList = new List<Tag>();
      tagList.Add(Tag.op_Implicit("ClothingAlterationStation"));
      complexRecipe.fabricators = tagList;
      complexRecipe.sortOrder = 1;
    }
  }

  public override void DoPostConfigureComplete(GameObject go) => go.GetComponent<KPrefabID>().prefabSpawnFn += ClothingAlterationStationConfig.\u003C\u003Ec.\u003C\u003E9__4_0 ?? (ClothingAlterationStationConfig.\u003C\u003Ec.\u003C\u003E9__4_0 = new KPrefabID.PrefabFn((object) ClothingAlterationStationConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__4_0)));
}
