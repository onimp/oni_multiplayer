// Decompiled with JetBrains decompiler
// Type: ComplexRecipe
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ComplexRecipe
{
  public string id;
  public ComplexRecipe.RecipeElement[] ingredients;
  public ComplexRecipe.RecipeElement[] results;
  public float time;
  public GameObject FabricationVisualizer;
  public int consumedHEP;
  public int producedHEP;
  public string recipeCategoryID = "";
  public ComplexRecipe.RecipeNameDisplay nameDisplay;
  public string customName;
  public string description;
  public List<Tag> fabricators;
  public int sortOrder;
  public string requiredTech;

  public bool ProductHasFacade { get; set; }

  public Tag FirstResult => this.results[0].material;

  public ComplexRecipe(
    string id,
    ComplexRecipe.RecipeElement[] ingredients,
    ComplexRecipe.RecipeElement[] results)
  {
    this.id = id;
    this.ingredients = ingredients;
    this.results = results;
    ComplexRecipeManager.Get().Add(this);
  }

  public ComplexRecipe(
    string id,
    ComplexRecipe.RecipeElement[] ingredients,
    ComplexRecipe.RecipeElement[] results,
    int consumedHEP,
    int producedHEP)
    : this(id, ingredients, results)
  {
    this.consumedHEP = consumedHEP;
    this.producedHEP = producedHEP;
  }

  public ComplexRecipe(
    string id,
    ComplexRecipe.RecipeElement[] ingredients,
    ComplexRecipe.RecipeElement[] results,
    int consumedHEP)
    : this(id, ingredients, results, consumedHEP, 0)
  {
  }

  public float TotalResultUnits()
  {
    float num = 0.0f;
    foreach (ComplexRecipe.RecipeElement result in this.results)
      num += result.amount;
    return num;
  }

  public bool RequiresTechUnlock() => !string.IsNullOrEmpty(this.requiredTech);

  public bool IsRequiredTechUnlocked() => string.IsNullOrEmpty(this.requiredTech) || Db.Get().Techs.Get(this.requiredTech).IsComplete();

  public Sprite GetUIIcon()
  {
    Sprite uiIcon = (Sprite) null;
    KBatchedAnimController component = Assets.GetPrefab(this.nameDisplay == ComplexRecipe.RecipeNameDisplay.Ingredient ? this.ingredients[0].material : this.results[0].material).GetComponent<KBatchedAnimController>();
    if (Object.op_Inequality((Object) component, (Object) null))
      uiIcon = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0]);
    return uiIcon;
  }

  public Color GetUIColor() => Color.white;

  public string GetUIName(bool includeAmounts)
  {
    string str = Util.IsNullOrWhiteSpace(this.results[0].facadeID) ? this.results[0].material.ProperName() : Tag.op_Implicit(this.results[0].facadeID).ProperName();
    switch (this.nameDisplay)
    {
      case ComplexRecipe.RecipeNameDisplay.Result:
        return includeAmounts ? string.Format((string) UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_SIMPLE_INCLUDE_AMOUNTS, (object) str, (object) this.results[0].amount) : str;
      case ComplexRecipe.RecipeNameDisplay.IngredientToResult:
        if (!includeAmounts)
          return string.Format((string) UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, (object) this.ingredients[0].material.ProperName(), (object) str);
        return string.Format((string) UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_INCLUDE_AMOUNTS, (object) this.ingredients[0].material.ProperName(), (object) str, (object) this.ingredients[0].amount, (object) this.results[0].amount);
      case ComplexRecipe.RecipeNameDisplay.ResultWithIngredient:
        if (!includeAmounts)
          return string.Format((string) UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_WITH, (object) this.ingredients[0].material.ProperName(), (object) str);
        return string.Format((string) UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_WITH_INCLUDE_AMOUNTS, (object) this.ingredients[0].material.ProperName(), (object) str, (object) this.ingredients[0].amount, (object) this.results[0].amount);
      case ComplexRecipe.RecipeNameDisplay.Composite:
        if (!includeAmounts)
          return string.Format((string) UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_COMPOSITE, (object) this.ingredients[0].material.ProperName(), (object) str, (object) this.results[1].material.ProperName());
        return string.Format((string) UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_COMPOSITE_INCLUDE_AMOUNTS, (object) this.ingredients[0].material.ProperName(), (object) str, (object) this.results[1].material.ProperName(), (object) this.ingredients[0].amount, (object) this.results[0].amount, (object) this.results[1].amount);
      case ComplexRecipe.RecipeNameDisplay.HEP:
        if (!includeAmounts)
          return string.Format((string) UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_HEP, (object) this.ingredients[0].material.ProperName(), (object) str);
        return string.Format((string) UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_HEP_INCLUDE_AMOUNTS, (object) this.ingredients[0].material.ProperName(), (object) this.results[1].material.ProperName(), (object) this.ingredients[0].amount, (object) this.producedHEP, (object) this.results[1].amount);
      case ComplexRecipe.RecipeNameDisplay.Custom:
        return this.customName;
      default:
        return includeAmounts ? string.Format((string) UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_SIMPLE_INCLUDE_AMOUNTS, (object) this.ingredients[0].material.ProperName(), (object) this.ingredients[0].amount) : this.ingredients[0].material.ProperName();
    }
  }

  public enum RecipeNameDisplay
  {
    Ingredient,
    Result,
    IngredientToResult,
    ResultWithIngredient,
    Composite,
    HEP,
    Custom,
  }

  public class RecipeElement
  {
    public Tag material;
    public ComplexRecipe.RecipeElement.TemperatureOperation temperatureOperation;
    public bool storeElement;
    public bool inheritElement;
    public string facadeID;

    public RecipeElement(Tag material, float amount, bool inheritElement)
    {
      this.material = material;
      this.amount = amount;
      this.temperatureOperation = ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature;
      this.inheritElement = inheritElement;
    }

    public RecipeElement(Tag material, float amount)
    {
      this.material = material;
      this.amount = amount;
      this.temperatureOperation = ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature;
    }

    public RecipeElement(
      Tag material,
      float amount,
      ComplexRecipe.RecipeElement.TemperatureOperation temperatureOperation,
      bool storeElement = false)
    {
      this.material = material;
      this.amount = amount;
      this.temperatureOperation = temperatureOperation;
      this.storeElement = storeElement;
    }

    public RecipeElement(
      Tag material,
      float amount,
      ComplexRecipe.RecipeElement.TemperatureOperation temperatureOperation,
      string facadeID,
      bool storeElement = false)
    {
      this.material = material;
      this.amount = amount;
      this.temperatureOperation = temperatureOperation;
      this.storeElement = storeElement;
      this.facadeID = facadeID;
    }

    public float amount { get; private set; }

    public enum TemperatureOperation
    {
      AverageTemperature,
      Heated,
      Melted,
    }
  }
}
