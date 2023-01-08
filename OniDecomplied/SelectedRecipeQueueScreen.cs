// Decompiled with JetBrains decompiler
// Type: SelectedRecipeQueueScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using STRINGS;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedRecipeQueueScreen : KScreen
{
  public Image recipeIcon;
  public LocText recipeName;
  public LocText recipeMainDescription;
  public GameObject IngredientsDescriptorPanel;
  public GameObject EffectsDescriptorPanel;
  public KNumberInputField QueueCount;
  public MultiToggle DecrementButton;
  public MultiToggle IncrementButton;
  public KButton InfiniteButton;
  public GameObject InfiniteIcon;
  private ComplexFabricator target;
  private ComplexFabricatorSideScreen ownerScreen;
  private ComplexRecipe selectedRecipe;
  [SerializeField]
  private GameObject recipeElementDescriptorPrefab;
  private Dictionary<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject> recipeIngredientDescriptorRows = new Dictionary<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject>();
  private Dictionary<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject> recipeEffectsDescriptorRows = new Dictionary<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject>();
  [SerializeField]
  private FullBodyUIMinionWidget minionWidget;
  [SerializeField]
  private MultiToggle previousRecipeButton;
  [SerializeField]
  private MultiToggle nextRecipeButton;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.DecrementButton.onClick = (System.Action) (() =>
    {
      this.target.DecrementRecipeQueueCount(this.selectedRecipe, false);
      this.RefreshQueueCountDisplay();
      this.ownerScreen.RefreshQueueCountDisplayForRecipe(this.selectedRecipe, this.target);
    });
    this.IncrementButton.onClick = (System.Action) (() =>
    {
      this.target.IncrementRecipeQueueCount(this.selectedRecipe);
      this.RefreshQueueCountDisplay();
      this.ownerScreen.RefreshQueueCountDisplayForRecipe(this.selectedRecipe, this.target);
    });
    ((TMP_Text) ((Component) this.InfiniteButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_FOREVER;
    this.InfiniteButton.onClick += (System.Action) (() =>
    {
      if (this.target.GetRecipeQueueCount(this.selectedRecipe) != ComplexFabricator.QUEUE_INFINITE)
        this.target.SetRecipeQueueCount(this.selectedRecipe, ComplexFabricator.QUEUE_INFINITE);
      else
        this.target.SetRecipeQueueCount(this.selectedRecipe, 0);
      this.RefreshQueueCountDisplay();
      this.ownerScreen.RefreshQueueCountDisplayForRecipe(this.selectedRecipe, this.target);
    });
    ((KInputField) this.QueueCount).onEndEdit += (System.Action) (() =>
    {
      this.isEditing = false;
      this.target.SetRecipeQueueCount(this.selectedRecipe, Mathf.RoundToInt(this.QueueCount.currentValue));
      this.RefreshQueueCountDisplay();
      this.ownerScreen.RefreshQueueCountDisplayForRecipe(this.selectedRecipe, this.target);
    });
    ((KInputField) this.QueueCount).onStartEdit += (System.Action) (() =>
    {
      this.isEditing = true;
      KScreenManager.Instance.RefreshStack();
    });
    this.previousRecipeButton.onClick += new System.Action(this.CyclePreviousRecipe);
    this.nextRecipeButton.onClick += new System.Action(this.CycleNextRecipe);
  }

  public void SetRecipe(
    ComplexFabricatorSideScreen owner,
    ComplexFabricator target,
    ComplexRecipe recipe)
  {
    this.ownerScreen = owner;
    this.target = target;
    this.selectedRecipe = recipe;
    ((TMP_Text) this.recipeName).text = recipe.GetUIName(false);
    Tuple<Sprite, Color> tuple = recipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.Ingredient ? Def.GetUISprite((object) recipe.ingredients[0].material) : Def.GetUISprite(recipe.results[0].material, recipe.results[0].facadeID);
    if (recipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.HEP)
    {
      this.recipeIcon.sprite = owner.radboltSprite;
    }
    else
    {
      this.recipeIcon.sprite = tuple.first;
      ((Graphic) this.recipeIcon).color = tuple.second;
    }
    ((TMP_Text) this.recipeMainDescription).SetText(recipe.description);
    this.RefreshIngredientDescriptors();
    this.RefreshResultDescriptors();
    this.RefreshQueueCountDisplay();
    this.ToggleAndRefreshMinionDisplay();
  }

  private void CyclePreviousRecipe() => this.ownerScreen.CycleRecipe(-1);

  private void CycleNextRecipe() => this.ownerScreen.CycleRecipe(1);

  private void ToggleAndRefreshMinionDisplay() => ((Component) this.minionWidget).gameObject.SetActive(this.RefreshMinionDisplayAnim());

  private bool RefreshMinionDisplayAnim()
  {
    GameObject prefab = Assets.GetPrefab(this.selectedRecipe.results[0].material);
    if (Object.op_Equality((Object) prefab, (Object) null))
      return false;
    Equippable component = prefab.GetComponent<Equippable>();
    if (Object.op_Equality((Object) component, (Object) null))
      return false;
    KAnimFile buildOverride = component.GetBuildOverride();
    if (Object.op_Equality((Object) buildOverride, (Object) null))
      return false;
    this.minionWidget.SetDefaultPortraitAnimator();
    KAnimFileData data = buildOverride.GetData();
    this.minionWidget.UpdateClothingOverride(data);
    this.minionWidget.animController.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_neck"), data.build.GetSymbol(KAnimHashedString.op_Implicit("snapto_neck")) != null);
    if (!Util.IsNullOrWhiteSpace(this.selectedRecipe.results[0].facadeID))
    {
      EquippableFacadeResource equippableFacadeResource = Db.GetEquippableFacades().TryGet(this.selectedRecipe.results[0].facadeID);
      if (equippableFacadeResource != null)
        this.minionWidget.UpdateClothingOverride(Assets.GetAnim(HashedString.op_Implicit(equippableFacadeResource.BuildOverride)).GetData());
    }
    return true;
  }

  private void RefreshQueueCountDisplay()
  {
    bool flag = this.target.GetRecipeQueueCount(this.selectedRecipe) == ComplexFabricator.QUEUE_INFINITE;
    if (!flag)
      this.QueueCount.SetAmount((float) this.target.GetRecipeQueueCount(this.selectedRecipe));
    else
      ((KInputField) this.QueueCount).SetDisplayValue("");
    this.InfiniteIcon.gameObject.SetActive(flag);
  }

  private void RefreshResultDescriptors()
  {
    List<SelectedRecipeQueueScreen.DescriptorWithSprite> descriptorWithSpriteList = new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
    descriptorWithSpriteList.AddRange((IEnumerable<SelectedRecipeQueueScreen.DescriptorWithSprite>) this.GetResultDescriptions(this.selectedRecipe));
    foreach (Descriptor desc in this.target.AdditionalEffectsForRecipe(this.selectedRecipe))
      descriptorWithSpriteList.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(desc, (Tuple<Sprite, Color>) null));
    if (descriptorWithSpriteList.Count <= 0)
      return;
    this.EffectsDescriptorPanel.gameObject.SetActive(true);
    foreach (KeyValuePair<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject> effectsDescriptorRow in this.recipeEffectsDescriptorRows)
      Util.KDestroyGameObject(effectsDescriptorRow.Value);
    this.recipeEffectsDescriptorRows.Clear();
    foreach (SelectedRecipeQueueScreen.DescriptorWithSprite key in descriptorWithSpriteList)
    {
      GameObject gameObject = Util.KInstantiateUI(this.recipeElementDescriptorPrefab, this.EffectsDescriptorPanel.gameObject, true);
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      LocText reference = component.GetReference<LocText>("Label");
      Descriptor descriptor = key.descriptor;
      string str = ((Descriptor) ref descriptor).IndentedText();
      ((TMP_Text) reference).SetText(str);
      component.GetReference<Image>("Icon").sprite = key.tintedSprite == null ? (Sprite) null : key.tintedSprite.first;
      ((Graphic) component.GetReference<Image>("Icon")).color = key.tintedSprite == null ? Color.white : key.tintedSprite.second;
      ((Component) component.GetReference<RectTransform>("FilterControls")).gameObject.SetActive(false);
      component.GetReference<ToolTip>("Tooltip").SetSimpleTooltip(key.descriptor.tooltipText);
      this.recipeEffectsDescriptorRows.Add(key, gameObject);
    }
  }

  private List<SelectedRecipeQueueScreen.DescriptorWithSprite> GetResultDescriptions(
    ComplexRecipe recipe)
  {
    List<SelectedRecipeQueueScreen.DescriptorWithSprite> resultDescriptions = new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
    if (recipe.producedHEP > 0)
      resultDescriptions.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(new Descriptor(string.Format("<b>{0}</b>: {1}", (object) STRINGS.UI.FormatAsLink((string) ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, "HEP"), (object) recipe.producedHEP), string.Format("<b>{0}</b>: {1}", (object) ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, (object) recipe.producedHEP), (Descriptor.DescriptorType) 0, false), new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("radbolt")), Color.white)));
    foreach (ComplexRecipe.RecipeElement result in recipe.results)
    {
      GameObject prefab = Assets.GetPrefab(result.material);
      string formattedByTag = GameUtil.GetFormattedByTag(result.material, result.amount);
      resultDescriptions.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(new Descriptor(string.Format((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPEPRODUCT, Util.IsNullOrWhiteSpace(result.facadeID) ? (object) result.material.ProperName() : (object) Tag.op_Implicit(result.facadeID).ProperName(), (object) formattedByTag), string.Format((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPEPRODUCT, Util.IsNullOrWhiteSpace(result.facadeID) ? (object) result.material.ProperName() : (object) Tag.op_Implicit(result.facadeID).ProperName(), (object) formattedByTag), (Descriptor.DescriptorType) 0, false), Def.GetUISprite(result.material, result.facadeID)));
      Element element = ElementLoader.GetElement(result.material);
      if (element != null)
      {
        List<SelectedRecipeQueueScreen.DescriptorWithSprite> collection = new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
        foreach (Descriptor materialDescriptor in GameUtil.GetMaterialDescriptors(element))
          collection.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(materialDescriptor, (Tuple<Sprite, Color>) null));
        foreach (SelectedRecipeQueueScreen.DescriptorWithSprite descriptorWithSprite in collection)
        {
          Descriptor descriptor = descriptorWithSprite.descriptor;
          ((Descriptor) ref descriptor).IncreaseIndent();
        }
        resultDescriptions.AddRange((IEnumerable<SelectedRecipeQueueScreen.DescriptorWithSprite>) collection);
      }
      else
      {
        List<SelectedRecipeQueueScreen.DescriptorWithSprite> collection = new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
        foreach (Descriptor effectDescriptor in GameUtil.GetEffectDescriptors(GameUtil.GetAllDescriptors(prefab)))
          collection.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(effectDescriptor, (Tuple<Sprite, Color>) null));
        foreach (SelectedRecipeQueueScreen.DescriptorWithSprite descriptorWithSprite in collection)
        {
          Descriptor descriptor = descriptorWithSprite.descriptor;
          ((Descriptor) ref descriptor).IncreaseIndent();
        }
        resultDescriptions.AddRange((IEnumerable<SelectedRecipeQueueScreen.DescriptorWithSprite>) collection);
      }
    }
    return resultDescriptions;
  }

  private void RefreshIngredientDescriptors()
  {
    List<SelectedRecipeQueueScreen.DescriptorWithSprite> descriptorWithSpriteList = new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
    List<SelectedRecipeQueueScreen.DescriptorWithSprite> ingredientDescriptions = this.GetIngredientDescriptions(this.selectedRecipe);
    this.IngredientsDescriptorPanel.gameObject.SetActive(true);
    foreach (KeyValuePair<SelectedRecipeQueueScreen.DescriptorWithSprite, GameObject> ingredientDescriptorRow in this.recipeIngredientDescriptorRows)
      Util.KDestroyGameObject(ingredientDescriptorRow.Value);
    this.recipeIngredientDescriptorRows.Clear();
    foreach (SelectedRecipeQueueScreen.DescriptorWithSprite key in ingredientDescriptions)
    {
      GameObject gameObject = Util.KInstantiateUI(this.recipeElementDescriptorPrefab, this.IngredientsDescriptorPanel.gameObject, true);
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      LocText reference = component.GetReference<LocText>("Label");
      Descriptor descriptor = key.descriptor;
      string str = ((Descriptor) ref descriptor).IndentedText();
      ((TMP_Text) reference).SetText(str);
      component.GetReference<Image>("Icon").sprite = key.tintedSprite == null ? (Sprite) null : key.tintedSprite.first;
      ((Graphic) component.GetReference<Image>("Icon")).color = key.tintedSprite == null ? Color.white : key.tintedSprite.second;
      ((Component) component.GetReference<RectTransform>("FilterControls")).gameObject.SetActive(false);
      component.GetReference<ToolTip>("Tooltip").SetSimpleTooltip(key.descriptor.tooltipText);
      this.recipeIngredientDescriptorRows.Add(key, gameObject);
    }
  }

  private List<SelectedRecipeQueueScreen.DescriptorWithSprite> GetIngredientDescriptions(
    ComplexRecipe recipe)
  {
    List<SelectedRecipeQueueScreen.DescriptorWithSprite> ingredientDescriptions = new List<SelectedRecipeQueueScreen.DescriptorWithSprite>();
    foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
    {
      GameObject prefab = Assets.GetPrefab(ingredient.material);
      string formattedByTag1 = GameUtil.GetFormattedByTag(ingredient.material, ingredient.amount);
      float amount = this.target.GetMyWorld().worldInventory.GetAmount(ingredient.material, true);
      string formattedByTag2 = GameUtil.GetFormattedByTag(ingredient.material, amount);
      string str = (double) amount >= (double) ingredient.amount ? string.Format((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, (object) prefab.GetProperName(), (object) formattedByTag1, (object) formattedByTag2) : "<color=#F44A47>" + string.Format((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, (object) prefab.GetProperName(), (object) formattedByTag1, (object) formattedByTag2) + "</color>";
      ingredientDescriptions.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(new Descriptor(str, str, (Descriptor.DescriptorType) 0, false), Def.GetUISprite((object) ingredient.material), Object.op_Inequality((Object) Assets.GetPrefab(ingredient.material).GetComponent<MutantPlant>(), (Object) null)));
    }
    if (recipe.consumedHEP > 0)
    {
      HighEnergyParticleStorage component = ((Component) this.target).GetComponent<HighEnergyParticleStorage>();
      ingredientDescriptions.Add(new SelectedRecipeQueueScreen.DescriptorWithSprite(new Descriptor(string.Format("<b>{0}</b>: {1} / {2}", (object) STRINGS.UI.FormatAsLink((string) ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, "HEP"), (object) recipe.consumedHEP, (object) component.Particles), string.Format("<b>{0}</b>: {1} / {2}", (object) ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, (object) recipe.consumedHEP, (object) component.Particles), (Descriptor.DescriptorType) 0, false), new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("radbolt")), Color.white)));
    }
    return ingredientDescriptions;
  }

  private class DescriptorWithSprite
  {
    public bool showFilterRow;

    public Descriptor descriptor { get; }

    public Tuple<Sprite, Color> tintedSprite { get; }

    public DescriptorWithSprite(
      Descriptor desc,
      Tuple<Sprite, Color> sprite,
      bool filterRowVisible = false)
    {
      this.descriptor = desc;
      this.tintedSprite = sprite;
      this.showFilterRow = filterRowVisible;
    }
  }
}
