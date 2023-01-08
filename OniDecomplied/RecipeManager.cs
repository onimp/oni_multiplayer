// Decompiled with JetBrains decompiler
// Type: RecipeManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class RecipeManager
{
  private static RecipeManager _Instance;
  public List<Recipe> recipes = new List<Recipe>();

  public static RecipeManager Get()
  {
    if (RecipeManager._Instance == null)
      RecipeManager._Instance = new RecipeManager();
    return RecipeManager._Instance;
  }

  public static void DestroyInstance() => RecipeManager._Instance = (RecipeManager) null;

  public void Add(Recipe recipe)
  {
    this.recipes.Add(recipe);
    if (!Object.op_Inequality((Object) recipe.FabricationVisualizer, (Object) null))
      return;
    Object.DontDestroyOnLoad((Object) recipe.FabricationVisualizer);
  }
}
