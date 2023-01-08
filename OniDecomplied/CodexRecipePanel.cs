// Decompiled with JetBrains decompiler
// Type: CodexRecipePanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexRecipePanel : CodexWidget<CodexRecipePanel>
{
  private LocText title;
  private GameObject materialPrefab;
  private GameObject fabricatorPrefab;
  private GameObject ingredientsContainer;
  private GameObject resultsContainer;
  private GameObject fabricatorContainer;
  private ComplexRecipe complexRecipe;
  private Recipe recipe;

  public string linkID { get; set; }

  public CodexRecipePanel()
  {
  }

  public CodexRecipePanel(ComplexRecipe recipe) => this.complexRecipe = recipe;

  public CodexRecipePanel(Recipe rec) => this.recipe = rec;

  public override void Configure(
    GameObject contentGameObject,
    Transform displayPane,
    Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
  {
    HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
    this.title = component.GetReference<LocText>("Title");
    this.materialPrefab = ((Component) component.GetReference<RectTransform>("MaterialPrefab")).gameObject;
    this.fabricatorPrefab = ((Component) component.GetReference<RectTransform>("FabricatorPrefab")).gameObject;
    this.ingredientsContainer = ((Component) component.GetReference<RectTransform>("IngredientsContainer")).gameObject;
    this.resultsContainer = ((Component) component.GetReference<RectTransform>("ResultsContainer")).gameObject;
    this.fabricatorContainer = ((Component) component.GetReference<RectTransform>("FabricatorContainer")).gameObject;
    this.ClearPanel();
    if (this.recipe != null)
    {
      this.ConfigureRecipe();
    }
    else
    {
      if (this.complexRecipe == null)
        return;
      this.ConfigureComplexRecipe();
    }
  }

  private void ConfigureRecipe()
  {
    ((TMP_Text) this.title).text = this.recipe.Result.ProperName();
    foreach (Recipe.Ingredient ingredient in this.recipe.Ingredients)
    {
      GameObject gameObject = Util.KInstantiateUI(this.materialPrefab, this.ingredientsContainer, true);
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) ingredient.tag);
      component.GetReference<Image>("Icon").sprite = uiSprite.first;
      ((Graphic) component.GetReference<Image>("Icon")).color = uiSprite.second;
      ((TMP_Text) component.GetReference<LocText>("Amount")).text = GameUtil.GetFormattedByTag(ingredient.tag, ingredient.amount);
      ((Graphic) component.GetReference<LocText>("Amount")).color = Color.black;
      string str = ingredient.tag.ProperName();
      GameObject prefab = Assets.GetPrefab(ingredient.tag);
      if (Object.op_Inequality((Object) prefab.GetComponent<Edible>(), (Object) null))
        str = str + "\n    • " + string.Format((string) STRINGS.UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, (object) GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
      gameObject.GetComponent<ToolTip>().toolTip = str;
    }
    GameObject gameObject1 = Util.KInstantiateUI(this.materialPrefab, this.resultsContainer, true);
    HierarchyReferences component1 = gameObject1.GetComponent<HierarchyReferences>();
    Tuple<Sprite, Color> uiSprite1 = Def.GetUISprite((object) this.recipe.Result);
    component1.GetReference<Image>("Icon").sprite = uiSprite1.first;
    ((Graphic) component1.GetReference<Image>("Icon")).color = uiSprite1.second;
    ((TMP_Text) component1.GetReference<LocText>("Amount")).text = GameUtil.GetFormattedByTag(this.recipe.Result, this.recipe.OutputUnits);
    ((Graphic) component1.GetReference<LocText>("Amount")).color = Color.black;
    string str1 = this.recipe.Result.ProperName();
    GameObject prefab1 = Assets.GetPrefab(this.recipe.Result);
    if (Object.op_Inequality((Object) prefab1.GetComponent<Edible>(), (Object) null))
      str1 = str1 + "\n    • " + string.Format((string) STRINGS.UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, (object) GameUtil.GetFormattedFoodQuality(prefab1.GetComponent<Edible>().GetQuality()));
    gameObject1.GetComponent<ToolTip>().toolTip = str1;
  }

  private void ConfigureComplexRecipe()
  {
    ((TMP_Text) this.title).text = this.complexRecipe.results[0].material.ProperName();
    foreach (ComplexRecipe.RecipeElement ingredient in this.complexRecipe.ingredients)
    {
      ComplexRecipe.RecipeElement ing = ingredient;
      HierarchyReferences component = Util.KInstantiateUI(this.materialPrefab, this.ingredientsContainer, true).GetComponent<HierarchyReferences>();
      Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) ing.material);
      component.GetReference<Image>("Icon").sprite = uiSprite.first;
      ((Graphic) component.GetReference<Image>("Icon")).color = uiSprite.second;
      ((TMP_Text) component.GetReference<LocText>("Amount")).text = GameUtil.GetFormattedByTag(ing.material, ing.amount);
      ((Graphic) component.GetReference<LocText>("Amount")).color = Color.black;
      string str = ing.material.ProperName();
      GameObject prefab = Assets.GetPrefab(ing.material);
      if (Object.op_Inequality((Object) prefab.GetComponent<Edible>(), (Object) null))
        str = str + "\n    • " + string.Format((string) STRINGS.UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, (object) GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
      component.GetReference<ToolTip>("Tooltip").toolTip = str;
      component.GetReference<KButton>("Button").onClick += (System.Action) (() => ManagementMenu.Instance.codexScreen.ChangeArticle(STRINGS.UI.ExtractLinkID(Assets.GetPrefab(ing.material).GetProperName()), targetPosition: new Vector3()));
    }
    foreach (ComplexRecipe.RecipeElement result in this.complexRecipe.results)
    {
      ComplexRecipe.RecipeElement res = result;
      HierarchyReferences component = Util.KInstantiateUI(this.materialPrefab, this.resultsContainer, true).GetComponent<HierarchyReferences>();
      Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) res.material);
      component.GetReference<Image>("Icon").sprite = uiSprite.first;
      ((Graphic) component.GetReference<Image>("Icon")).color = uiSprite.second;
      ((TMP_Text) component.GetReference<LocText>("Amount")).text = GameUtil.GetFormattedByTag(res.material, res.amount);
      ((Graphic) component.GetReference<LocText>("Amount")).color = Color.black;
      string str = res.material.ProperName();
      GameObject prefab = Assets.GetPrefab(res.material);
      if (Object.op_Inequality((Object) prefab.GetComponent<Edible>(), (Object) null))
        str = str + "\n    • " + string.Format((string) STRINGS.UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, (object) GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
      component.GetReference<ToolTip>("Tooltip").toolTip = str;
      component.GetReference<KButton>("Button").onClick += (System.Action) (() => ManagementMenu.Instance.codexScreen.ChangeArticle(STRINGS.UI.ExtractLinkID(Assets.GetPrefab(res.material).GetProperName()), targetPosition: new Vector3()));
    }
    string str1 = this.complexRecipe.id.Substring(0, this.complexRecipe.id.IndexOf('_'));
    HierarchyReferences component1 = Util.KInstantiateUI(this.fabricatorPrefab, this.fabricatorContainer, true).GetComponent<HierarchyReferences>();
    Tuple<Sprite, Color> uiSprite1 = Def.GetUISprite((object) str1);
    component1.GetReference<Image>("Icon").sprite = uiSprite1.first;
    ((Graphic) component1.GetReference<Image>("Icon")).color = uiSprite1.second;
    ((TMP_Text) component1.GetReference<LocText>("Time")).text = GameUtil.GetFormattedTime(this.complexRecipe.time);
    ((Graphic) component1.GetReference<LocText>("Time")).color = Color.black;
    GameObject fabricator = Assets.GetPrefab(TagExtensions.ToTag(str1));
    component1.GetReference<ToolTip>("Tooltip").toolTip = fabricator.GetProperName();
    component1.GetReference<KButton>("Button").onClick += (System.Action) (() => ManagementMenu.Instance.codexScreen.ChangeArticle(STRINGS.UI.ExtractLinkID(fabricator.GetProperName()), targetPosition: new Vector3()));
  }

  private void ClearPanel()
  {
    foreach (Component component in this.ingredientsContainer.transform)
      Object.Destroy((Object) component.gameObject);
    foreach (Component component in this.resultsContainer.transform)
      Object.Destroy((Object) component.gameObject);
    foreach (Component component in this.fabricatorContainer.transform)
      Object.Destroy((Object) component.gameObject);
  }
}
