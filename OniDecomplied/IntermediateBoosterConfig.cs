// Decompiled with JetBrains decompiler
// Type: IntermediateBoosterConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateBoosterConfig : IEntityConfig
{
  public const string ID = "IntermediateBooster";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("IntermediateBooster", (string) ITEMS.PILLS.INTERMEDIATEBOOSTER.NAME, (string) ITEMS.PILLS.INTERMEDIATEBOOSTER.DESC, 1f, true, Assets.GetAnim(HashedString.op_Implicit("pill_3_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true);
    EntityTemplates.ExtendEntityToMedicine(looseEntity, TUNING.MEDICINE.INTERMEDIATEBOOSTER);
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(SpiceNutConfig.ID), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("IntermediateBooster"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("Apothecary", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2), recipeElementArray1, recipeElementArray2);
    complexRecipe.time = 100f;
    complexRecipe.description = (string) ITEMS.PILLS.INTERMEDIATEBOOSTER.RECIPEDESC;
    complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList = new List<Tag>();
    tagList.Add(Tag.op_Implicit("Apothecary"));
    complexRecipe.fabricators = tagList;
    complexRecipe.sortOrder = 5;
    IntermediateBoosterConfig.recipe = complexRecipe;
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
