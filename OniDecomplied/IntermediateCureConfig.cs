// Decompiled with JetBrains decompiler
// Type: IntermediateCureConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateCureConfig : IEntityConfig
{
  public const string ID = "IntermediateCure";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject medicine = EntityTemplates.ExtendEntityToMedicine(EntityTemplates.CreateLooseEntity("IntermediateCure", (string) ITEMS.PILLS.INTERMEDIATECURE.NAME, (string) ITEMS.PILLS.INTERMEDIATECURE.DESC, 1f, true, Assets.GetAnim(HashedString.op_Implicit("iv_slimelung_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true), TUNING.MEDICINE.INTERMEDIATECURE);
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(SwampLilyFlowerConfig.ID), 1f),
      new ComplexRecipe.RecipeElement(SimHashes.Phosphorite.CreateTag(), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("IntermediateCure"), 1f)
    };
    string fabricator = "Apothecary";
    ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(fabricator, (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2), recipeElementArray1, recipeElementArray2);
    complexRecipe.time = 100f;
    complexRecipe.description = (string) ITEMS.PILLS.INTERMEDIATECURE.RECIPEDESC;
    complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
    List<Tag> tagList = new List<Tag>();
    tagList.Add(Tag.op_Implicit(fabricator));
    complexRecipe.fabricators = tagList;
    complexRecipe.sortOrder = 10;
    complexRecipe.requiredTech = "MedicineII";
    IntermediateCureConfig.recipe = complexRecipe;
    return medicine;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
