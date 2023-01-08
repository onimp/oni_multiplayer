// Decompiled with JetBrains decompiler
// Type: Recipe
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class Recipe : IHasSortOrder
{
  private string nameOverride;
  public string HotKey;
  public string Type;
  public List<Recipe.Ingredient> Ingredients;
  public string recipeDescription;
  public Tag Result;
  public GameObject FabricationVisualizer;
  public SimHashes ResultElementOverride;
  public Sprite Icon;
  public Color IconColor = Color.white;
  public string[] fabricators;
  public float OutputUnits;
  public float FabricationTime;
  public string TechUnlock;

  public int sortOrder { get; set; }

  public string Name
  {
    set => this.nameOverride = value;
    get => this.nameOverride != null ? this.nameOverride : this.Result.ProperName();
  }

  public Recipe()
  {
  }

  public Recipe(
    string prefabId,
    float outputUnits = 1f,
    SimHashes elementOverride = (SimHashes) 0,
    string nameOverride = null,
    string recipeDescription = null,
    int sortOrder = 0)
  {
    Debug.Assert(prefabId != null);
    this.Result = TagManager.Create(prefabId);
    this.ResultElementOverride = elementOverride;
    this.nameOverride = nameOverride;
    this.OutputUnits = outputUnits;
    this.Ingredients = new List<Recipe.Ingredient>();
    this.recipeDescription = recipeDescription;
    this.sortOrder = sortOrder;
    this.FabricationVisualizer = (GameObject) null;
  }

  public Recipe SetFabricator(string fabricator, float fabricationTime)
  {
    this.fabricators = new string[1]{ fabricator };
    this.FabricationTime = fabricationTime;
    RecipeManager.Get().Add(this);
    return this;
  }

  public Recipe SetFabricators(string[] fabricators, float fabricationTime)
  {
    this.fabricators = fabricators;
    this.FabricationTime = fabricationTime;
    RecipeManager.Get().Add(this);
    return this;
  }

  public Recipe SetIcon(Sprite Icon)
  {
    this.Icon = Icon;
    this.IconColor = Color.white;
    return this;
  }

  public Recipe SetIcon(Sprite Icon, Color IconColor)
  {
    this.Icon = Icon;
    this.IconColor = IconColor;
    return this;
  }

  public Recipe AddIngredient(Recipe.Ingredient ingredient)
  {
    this.Ingredients.Add(ingredient);
    return this;
  }

  public Recipe.Ingredient[] GetAllIngredients(IList<Tag> selectedTags)
  {
    List<Recipe.Ingredient> ingredientList = new List<Recipe.Ingredient>();
    for (int index = 0; index < this.Ingredients.Count; ++index)
    {
      float amount = this.Ingredients[index].amount;
      if (index < ((ICollection<Tag>) selectedTags).Count)
        ingredientList.Add(new Recipe.Ingredient(selectedTags[index], amount));
      else
        ingredientList.Add(new Recipe.Ingredient(this.Ingredients[index].tag, amount));
    }
    return ingredientList.ToArray();
  }

  public Recipe.Ingredient[] GetAllIngredients(IList<Element> selected_elements)
  {
    List<Recipe.Ingredient> ingredientList = new List<Recipe.Ingredient>();
    for (int index = 0; index < this.Ingredients.Count; ++index)
    {
      int amount = (int) this.Ingredients[index].amount;
      bool flag = false;
      if (index < selected_elements.Count)
      {
        Element selectedElement = selected_elements[index];
        if (selectedElement != null && selectedElement.HasTag(this.Ingredients[index].tag))
        {
          ingredientList.Add(new Recipe.Ingredient(GameTagExtensions.Create(selectedElement.id), (float) amount));
          flag = true;
        }
      }
      if (!flag)
        ingredientList.Add(new Recipe.Ingredient(this.Ingredients[index].tag, (float) amount));
    }
    return ingredientList.ToArray();
  }

  public GameObject Craft(Storage resource_storage, IList<Tag> selectedTags)
  {
    Recipe.Ingredient[] allIngredients = this.GetAllIngredients(selectedTags);
    return this.CraftRecipe(resource_storage, allIngredients);
  }

  private GameObject CraftRecipe(Storage resource_storage, Recipe.Ingredient[] ingredientTags)
  {
    SimUtil.DiseaseInfo a = SimUtil.DiseaseInfo.Invalid;
    float temp1 = 0.0f;
    float mass1 = 0.0f;
    foreach (Recipe.Ingredient ingredientTag in ingredientTags)
    {
      GameObject first = resource_storage.FindFirst(ingredientTag.tag);
      if (Object.op_Inequality((Object) first, (Object) null))
      {
        Edible component = first.GetComponent<Edible>();
        if (Object.op_Implicit((Object) component))
          ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -component.Calories, StringFormatter.Replace((string) UI.ENDOFDAYREPORT.NOTES.CRAFTED_USED, "{0}", ((Component) component).GetProperName()), (string) UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
      }
      SimUtil.DiseaseInfo disease_info;
      float temperature;
      resource_storage.ConsumeAndGetDisease(ingredientTag, out disease_info, out temperature);
      a = SimUtil.CalculateFinalDiseaseInfo(a, disease_info);
      temp1 = SimUtil.CalculateFinalTemperature(mass1, temp1, ingredientTag.amount, temperature);
      mass1 += ingredientTag.amount;
    }
    GameObject prefab = Assets.GetPrefab(this.Result);
    GameObject gameObject = (GameObject) null;
    if (Object.op_Inequality((Object) prefab, (Object) null))
    {
      gameObject = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore);
      PrimaryElement component1 = gameObject.GetComponent<PrimaryElement>();
      gameObject.GetComponent<KSelectable>().entityName = this.Name;
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        gameObject.GetComponent<KPrefabID>().RemoveTag(TagManager.Create("Vacuum"));
        if (this.ResultElementOverride != (SimHashes) 0)
        {
          if (Object.op_Inequality((Object) ((Component) component1).GetComponent<ElementChunk>(), (Object) null))
            component1.SetElement(this.ResultElementOverride);
          else
            component1.ElementID = this.ResultElementOverride;
        }
        component1.Temperature = temp1;
        component1.Units = this.OutputUnits;
      }
      Edible component2 = gameObject.GetComponent<Edible>();
      if (Object.op_Implicit((Object) component2))
        ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, StringFormatter.Replace((string) UI.ENDOFDAYREPORT.NOTES.CRAFTED, "{0}", ((Component) component2).GetProperName()), (string) UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
      gameObject.SetActive(true);
      if (Object.op_Inequality((Object) component1, (Object) null))
        component1.AddDisease(a.idx, a.count, "Recipe.CraftRecipe");
      gameObject.GetComponent<KMonoBehaviour>().Trigger(748399584, (object) null);
    }
    return gameObject;
  }

  public string[] MaterialOptionNames
  {
    get
    {
      List<string> stringList = new List<string>();
      foreach (Element element in ElementLoader.elements)
      {
        if (Array.IndexOf<Tag>(element.oreTags, this.Ingredients[0].tag) >= 0)
          stringList.Add(element.id.ToString());
      }
      return stringList.ToArray();
    }
  }

  public Element[] MaterialOptions()
  {
    List<Element> elementList = new List<Element>();
    foreach (Element element in ElementLoader.elements)
    {
      if (Array.IndexOf<Tag>(element.oreTags, this.Ingredients[0].tag) >= 0)
        elementList.Add(element);
    }
    return elementList.ToArray();
  }

  public BuildingDef GetBuildingDef()
  {
    BuildingComplete component = Assets.GetPrefab(this.Result).GetComponent<BuildingComplete>();
    return Object.op_Inequality((Object) component, (Object) null) ? component.Def : (BuildingDef) null;
  }

  public Sprite GetUIIcon()
  {
    Sprite uiIcon = (Sprite) null;
    if (Object.op_Inequality((Object) this.Icon, (Object) null))
    {
      uiIcon = this.Icon;
    }
    else
    {
      KBatchedAnimController component = Assets.GetPrefab(this.Result).GetComponent<KBatchedAnimController>();
      if (Object.op_Inequality((Object) component, (Object) null))
        uiIcon = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0]);
    }
    return uiIcon;
  }

  public Color GetUIColor() => !Object.op_Inequality((Object) this.Icon, (Object) null) ? Color.white : this.IconColor;

  [DebuggerDisplay("{tag} {amount}")]
  [Serializable]
  public class Ingredient
  {
    public Tag tag;
    public float amount;

    public Ingredient(string tag, float amount)
    {
      this.tag = TagManager.Create(tag);
      this.amount = amount;
    }

    public Ingredient(Tag tag, float amount)
    {
      this.tag = tag;
      this.amount = amount;
    }

    public List<Element> GetElementOptions()
    {
      List<Element> elementOptions = new List<Element>((IEnumerable<Element>) ElementLoader.elements);
      elementOptions.RemoveAll((Predicate<Element>) (e => !e.IsSolid));
      elementOptions.RemoveAll((Predicate<Element>) (e => !e.HasTag(this.tag)));
      return elementOptions;
    }
  }
}
