// Decompiled with JetBrains decompiler
// Type: AdvancedCureConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedCureConfig : IEntityConfig
{
  public const string ID = "AdvancedCure";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject medicine = EntityTemplates.ExtendEntityToMedicine(EntityTemplates.CreateLooseEntity("AdvancedCure", (string) ITEMS.PILLS.ADVANCEDCURE.NAME, (string) ITEMS.PILLS.ADVANCEDCURE.DESC, 1f, true, Assets.GetAnim(HashedString.op_Implicit("vial_spore_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true), TUNING.MEDICINE.ADVANCEDCURE);
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 1f),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("LightBugOrangeEgg"), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("AdvancedCure"), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    string fabricator = "Apothecary";
    ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(fabricator, (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2), recipeElementArray1, recipeElementArray2);
    complexRecipe.time = 200f;
    complexRecipe.description = (string) ITEMS.PILLS.ADVANCEDCURE.RECIPEDESC;
    complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList = new List<Tag>();
    tagList.Add(Tag.op_Implicit(fabricator));
    complexRecipe.fabricators = tagList;
    complexRecipe.sortOrder = 20;
    complexRecipe.requiredTech = "MedicineIV";
    AdvancedCureConfig.recipe = complexRecipe;
    return medicine;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
