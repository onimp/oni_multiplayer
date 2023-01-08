// Decompiled with JetBrains decompiler
// Type: FabricatorIngredientStatusManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FabricatorIngredientStatusManager")]
public class FabricatorIngredientStatusManager : KMonoBehaviour, ISim1000ms
{
  private KSelectable selectable;
  private ComplexFabricator fabricator;
  private Dictionary<ComplexRecipe, Guid> statusItems = new Dictionary<ComplexRecipe, Guid>();
  private Dictionary<ComplexRecipe, Dictionary<Tag, float>> recipeRequiredResourceBalances = new Dictionary<ComplexRecipe, Dictionary<Tag, float>>();
  private List<ComplexRecipe> deadOrderKeys = new List<ComplexRecipe>();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.selectable = ((Component) this).GetComponent<KSelectable>();
    this.fabricator = ((Component) this).GetComponent<ComplexFabricator>();
    this.InitializeBalances();
  }

  private void InitializeBalances()
  {
    foreach (ComplexRecipe recipe in this.fabricator.GetRecipes())
    {
      this.recipeRequiredResourceBalances.Add(recipe, new Dictionary<Tag, float>());
      foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
        this.recipeRequiredResourceBalances[recipe].Add(ingredient.material, 0.0f);
    }
  }

  public void Sim1000ms(float dt) => this.RefreshStatusItems();

  private void RefreshStatusItems()
  {
    foreach (KeyValuePair<ComplexRecipe, Guid> statusItem in this.statusItems)
    {
      if (!this.fabricator.IsRecipeQueued(statusItem.Key))
        this.deadOrderKeys.Add(statusItem.Key);
    }
    foreach (ComplexRecipe deadOrderKey in this.deadOrderKeys)
    {
      this.recipeRequiredResourceBalances[deadOrderKey].Clear();
      foreach (ComplexRecipe.RecipeElement ingredient in deadOrderKey.ingredients)
        this.recipeRequiredResourceBalances[deadOrderKey].Add(ingredient.material, 0.0f);
      this.selectable.RemoveStatusItem(this.statusItems[deadOrderKey]);
      this.statusItems.Remove(deadOrderKey);
    }
    this.deadOrderKeys.Clear();
    foreach (ComplexRecipe recipe in this.fabricator.GetRecipes())
    {
      if (this.fabricator.IsRecipeQueued(recipe))
      {
        bool flag = false;
        foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
        {
          float newBalance = this.fabricator.inStorage.GetAmountAvailable(ingredient.material) + this.fabricator.buildStorage.GetAmountAvailable(ingredient.material) + this.fabricator.GetMyWorld().worldInventory.GetTotalAmount(ingredient.material, true) - ingredient.amount;
          flag = flag || this.ChangeRecipeRequiredResourceBalance(recipe, ingredient.material, newBalance) || this.statusItems.ContainsKey(recipe) && this.fabricator.GetRecipeQueueCount(recipe) == 0;
        }
        if (flag)
        {
          if (this.statusItems.ContainsKey(recipe))
          {
            this.selectable.RemoveStatusItem(this.statusItems[recipe]);
            this.statusItems.Remove(recipe);
          }
          if (this.fabricator.IsRecipeQueued(recipe))
          {
            foreach (double num in this.recipeRequiredResourceBalances[recipe].Values)
            {
              if (num < 0.0)
              {
                Dictionary<Tag, float> data = new Dictionary<Tag, float>();
                foreach (KeyValuePair<Tag, float> keyValuePair in this.recipeRequiredResourceBalances[recipe])
                {
                  if ((double) keyValuePair.Value < 0.0)
                    data.Add(keyValuePair.Key, -keyValuePair.Value);
                }
                Guid guid = this.selectable.AddStatusItem((StatusItem) Db.Get().BuildingStatusItems.MaterialsUnavailable, (object) data);
                this.statusItems.Add(recipe, guid);
                break;
              }
            }
          }
        }
      }
    }
  }

  private bool ChangeRecipeRequiredResourceBalance(ComplexRecipe recipe, Tag tag, float newBalance)
  {
    bool flag = false;
    if ((double) this.recipeRequiredResourceBalances[recipe][tag] >= 0.0 != (double) newBalance >= 0.0)
      flag = true;
    this.recipeRequiredResourceBalances[recipe][tag] = newBalance;
    return flag;
  }
}
