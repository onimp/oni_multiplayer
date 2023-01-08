// Decompiled with JetBrains decompiler
// Type: ComplexRecipeManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ComplexRecipeManager
{
  private static ComplexRecipeManager _Instance;
  public List<ComplexRecipe> recipes = new List<ComplexRecipe>();
  private Dictionary<string, string> obsoleteIDMapping = new Dictionary<string, string>();

  public static ComplexRecipeManager Get()
  {
    if (ComplexRecipeManager._Instance == null)
      ComplexRecipeManager._Instance = new ComplexRecipeManager();
    return ComplexRecipeManager._Instance;
  }

  public static void DestroyInstance() => ComplexRecipeManager._Instance = (ComplexRecipeManager) null;

  public static string MakeObsoleteRecipeID(string fabricator, Tag signatureElement) => fabricator + "_" + signatureElement.ToString();

  public static string MakeRecipeID(
    string fabricator,
    IList<ComplexRecipe.RecipeElement> inputs,
    IList<ComplexRecipe.RecipeElement> outputs)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(fabricator);
    stringBuilder.Append("_I");
    foreach (ComplexRecipe.RecipeElement input in (IEnumerable<ComplexRecipe.RecipeElement>) inputs)
    {
      stringBuilder.Append("_");
      stringBuilder.Append(input.material.ToString());
    }
    stringBuilder.Append("_O");
    foreach (ComplexRecipe.RecipeElement output in (IEnumerable<ComplexRecipe.RecipeElement>) outputs)
    {
      stringBuilder.Append("_");
      stringBuilder.Append(output.material.ToString());
    }
    return stringBuilder.ToString();
  }

  public static string MakeRecipeID(
    string fabricator,
    IList<ComplexRecipe.RecipeElement> inputs,
    IList<ComplexRecipe.RecipeElement> outputs,
    string facadeID)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(fabricator);
    stringBuilder.Append("_I");
    foreach (ComplexRecipe.RecipeElement input in (IEnumerable<ComplexRecipe.RecipeElement>) inputs)
    {
      stringBuilder.Append("_");
      stringBuilder.Append(input.material.ToString());
    }
    stringBuilder.Append("_O");
    foreach (ComplexRecipe.RecipeElement output in (IEnumerable<ComplexRecipe.RecipeElement>) outputs)
    {
      stringBuilder.Append("_");
      stringBuilder.Append(output.material.ToString());
    }
    stringBuilder.Append("_" + facadeID);
    return stringBuilder.ToString();
  }

  public void Add(ComplexRecipe recipe)
  {
    foreach (ComplexRecipe recipe1 in this.recipes)
    {
      if (recipe1.id == recipe.id)
        Debug.LogError((object) string.Format("DUPLICATE RECIPE ID! '{0}' is being added to the recipe manager multiple times. This will result in the failure to save/load certain queued recipes at fabricators.", (object) recipe.id));
    }
    this.recipes.Add(recipe);
    if (!Object.op_Inequality((Object) recipe.FabricationVisualizer, (Object) null))
      return;
    Object.DontDestroyOnLoad((Object) recipe.FabricationVisualizer);
  }

  public ComplexRecipe GetRecipe(string id) => string.IsNullOrEmpty(id) ? (ComplexRecipe) null : this.recipes.Find((Predicate<ComplexRecipe>) (r => r.id == id));

  public void AddObsoleteIDMapping(string obsolete_id, string new_id) => this.obsoleteIDMapping[obsolete_id] = new_id;

  public ComplexRecipe GetObsoleteRecipe(string id)
  {
    if (string.IsNullOrEmpty(id))
      return (ComplexRecipe) null;
    ComplexRecipe obsoleteRecipe = (ComplexRecipe) null;
    string id1 = (string) null;
    if (this.obsoleteIDMapping.TryGetValue(id, out id1))
      obsoleteRecipe = this.GetRecipe(id1);
    return obsoleteRecipe;
  }
}
