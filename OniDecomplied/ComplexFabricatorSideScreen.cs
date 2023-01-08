// Decompiled with JetBrains decompiler
// Type: ComplexFabricatorSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComplexFabricatorSideScreen : SideScreenContent
{
  [Header("Recipe List")]
  [SerializeField]
  private GameObject recipeGrid;
  [Header("Recipe button variants")]
  [SerializeField]
  private GameObject recipeButton;
  [SerializeField]
  private GameObject recipeButtonMultiple;
  [SerializeField]
  private GameObject recipeButtonQueueHybrid;
  [SerializeField]
  private GameObject recipeCategoryHeader;
  [SerializeField]
  private Sprite buttonSelectedBG;
  [SerializeField]
  private Sprite buttonNormalBG;
  [SerializeField]
  private Sprite elementPlaceholderSpr;
  [SerializeField]
  public Sprite radboltSprite;
  private KToggle selectedToggle;
  public LayoutElement buttonScrollContainer;
  public RectTransform buttonContentContainer;
  [SerializeField]
  private GameObject elementContainer;
  [SerializeField]
  private LocText currentOrderLabel;
  [SerializeField]
  private LocText nextOrderLabel;
  private Dictionary<ComplexFabricator, int> selectedRecipeFabricatorMap = new Dictionary<ComplexFabricator, int>();
  public EventReference createOrderSound;
  [SerializeField]
  private RectTransform content;
  [SerializeField]
  private LocText subtitleLabel;
  [SerializeField]
  private LocText noRecipesDiscoveredLabel;
  public TextStyleSetting styleTooltipHeader;
  public TextStyleSetting styleTooltipBody;
  private ComplexFabricator targetFab;
  private ComplexRecipe selectedRecipe;
  private Dictionary<GameObject, ComplexRecipe> recipeMap;
  private Dictionary<string, GameObject> recipeCategories = new Dictionary<string, GameObject>();
  private List<GameObject> recipeToggles = new List<GameObject>();
  public SelectedRecipeQueueScreen recipeScreenPrefab;
  private SelectedRecipeQueueScreen recipeScreen;
  private int targetOrdersUpdatedSubHandle = -1;

  public override string GetTitle() => Object.op_Equality((Object) this.targetFab, (Object) null) ? ((object) Strings.Get(this.titleKey)).ToString().Replace("{0}", "") : string.Format(StringEntry.op_Implicit(Strings.Get(this.titleKey)), (object) ((Component) this.targetFab).GetProperName());

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<ComplexFabricator>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    ComplexFabricator component = target.GetComponent<ComplexFabricator>();
    if (Object.op_Equality((Object) component, (Object) null))
    {
      Debug.LogError((object) "The object selected doesn't have a ComplexFabricator!");
    }
    else
    {
      if (this.targetOrdersUpdatedSubHandle != -1)
        ((KMonoBehaviour) this).Unsubscribe(this.targetOrdersUpdatedSubHandle);
      this.Initialize(component);
      this.targetOrdersUpdatedSubHandle = this.targetFab.Subscribe(1721324763, new Action<object>(this.UpdateQueueCountLabels));
      this.UpdateQueueCountLabels();
    }
  }

  private void UpdateQueueCountLabels(object data = null)
  {
    foreach (ComplexRecipe recipe in this.targetFab.GetRecipes())
    {
      ComplexRecipe r = recipe;
      GameObject entryGO = this.recipeToggles.Find((Predicate<GameObject>) (match => this.recipeMap[match] == r));
      if (Object.op_Inequality((Object) entryGO, (Object) null))
        this.RefreshQueueCountDisplay(entryGO, this.targetFab);
    }
    if (this.targetFab.CurrentWorkingOrder != null)
      ((TMP_Text) this.currentOrderLabel).text = string.Format((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.CURRENT_ORDER, (object) this.targetFab.CurrentWorkingOrder.GetUIName(false));
    else
      ((TMP_Text) this.currentOrderLabel).text = string.Format((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.CURRENT_ORDER, (object) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.NO_WORKABLE_ORDER);
    if (this.targetFab.NextOrder != null)
      ((TMP_Text) this.nextOrderLabel).text = string.Format((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.NEXT_ORDER, (object) this.targetFab.NextOrder.GetUIName(false));
    else
      ((TMP_Text) this.nextOrderLabel).text = string.Format((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.NEXT_ORDER, (object) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.NO_WORKABLE_ORDER);
  }

  protected virtual void OnShow(bool show)
  {
    if (show)
    {
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().FabricatorSideScreenOpenSnapshot);
    }
    else
    {
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FabricatorSideScreenOpenSnapshot);
      DetailsScreen.Instance.ClearSecondarySideScreen();
      this.selectedRecipe = (ComplexRecipe) null;
      this.selectedToggle = (KToggle) null;
    }
    base.OnShow(show);
  }

  public void Initialize(ComplexFabricator target)
  {
    if (Object.op_Equality((Object) target, (Object) null))
    {
      Debug.LogError((object) "ComplexFabricator provided was null.");
    }
    else
    {
      this.targetFab = target;
      ((Component) this).gameObject.SetActive(true);
      this.recipeMap = new Dictionary<GameObject, ComplexRecipe>();
      this.recipeToggles.ForEach((Action<GameObject>) (rbi => Object.Destroy((Object) rbi.gameObject)));
      this.recipeToggles.Clear();
      foreach (KeyValuePair<string, GameObject> recipeCategory in this.recipeCategories)
        Object.Destroy((Object) ((Component) recipeCategory.Value.transform.parent).gameObject);
      this.recipeCategories.Clear();
      int num = 0;
      foreach (ComplexRecipe recipe1 in this.targetFab.GetRecipes())
      {
        ComplexRecipe recipe = recipe1;
        bool flag1 = false;
        if (DebugHandler.InstantBuildMode)
          flag1 = true;
        else if (recipe.RequiresTechUnlock())
        {
          if (recipe.IsRequiredTechUnlocked())
            flag1 = true;
        }
        else if (target.GetRecipeQueueCount(recipe) != 0)
          flag1 = true;
        else if (this.AnyRecipeRequirementsDiscovered(recipe))
          flag1 = true;
        else if (this.HasAnyRecipeRequirements(recipe))
          flag1 = true;
        if (flag1)
        {
          ++num;
          Tuple<Sprite, Color> uiSprite1 = Def.GetUISprite((object) recipe.ingredients[0].material);
          Tuple<Sprite, Color> uiSprite2 = Def.GetUISprite(recipe.results[0].material, recipe.results[0].facadeID);
          KToggle newToggle = (KToggle) null;
          GameObject entryGO;
          switch (target.sideScreenStyle)
          {
            case ComplexFabricatorSideScreen.StyleSetting.ListInputOutput:
            case ComplexFabricatorSideScreen.StyleSetting.GridInputOutput:
              newToggle = Util.KInstantiateUI<KToggle>(this.recipeButtonMultiple, this.recipeGrid, false);
              entryGO = ((Component) newToggle).gameObject;
              HierarchyReferences component1 = ((Component) newToggle).GetComponent<HierarchyReferences>();
              foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
              {
                GameObject gameObject = Util.KInstantiateUI(component1.GetReference("FromIconPrefab").gameObject, component1.GetReference("FromIcons").gameObject, true);
                gameObject.GetComponent<Image>().sprite = Def.GetUISprite((object) ingredient.material).first;
                ((Graphic) gameObject.GetComponent<Image>()).color = Def.GetUISprite((object) ingredient.material).second;
                ((Object) gameObject.gameObject).name = ((Tag) ref ingredient.material).Name;
              }
              foreach (ComplexRecipe.RecipeElement result in recipe.results)
              {
                GameObject gameObject = Util.KInstantiateUI(component1.GetReference("ToIconPrefab").gameObject, component1.GetReference("ToIcons").gameObject, true);
                gameObject.GetComponent<Image>().sprite = Def.GetUISprite((object) result.material).first;
                ((Graphic) gameObject.GetComponent<Image>()).color = Def.GetUISprite((object) result.material).second;
                ((Object) gameObject.gameObject).name = ((Tag) ref result.material).Name;
              }
              break;
            case ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid:
              newToggle = Util.KInstantiateUI<KToggle>(this.recipeButtonQueueHybrid, this.recipeGrid, false);
              entryGO = ((Component) newToggle).gameObject;
              this.recipeMap.Add(entryGO, recipe);
              if (recipe.recipeCategoryID != "")
              {
                if (!this.recipeCategories.ContainsKey(recipe.recipeCategoryID))
                {
                  GameObject gameObject = Util.KInstantiateUI(this.recipeCategoryHeader, this.recipeGrid, true);
                  ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).SetText(Strings.Get("STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_CATEGORIES." + recipe.recipeCategoryID.ToUpper()).String);
                  HierarchyReferences component2 = gameObject.GetComponent<HierarchyReferences>();
                  RectTransform categoryContent = component2.GetReference<RectTransform>("content");
                  component2.GetReference<Image>("icon").sprite = recipe.GetUIIcon();
                  ((Component) categoryContent).gameObject.SetActive(false);
                  MultiToggle toggle = gameObject.GetComponentInChildren<MultiToggle>();
                  toggle.onClick += (System.Action) (() =>
                  {
                    ((Component) categoryContent).gameObject.SetActive(!((Component) categoryContent).gameObject.activeSelf);
                    toggle.ChangeState(((Component) categoryContent).gameObject.activeSelf ? 1 : 0);
                  });
                  this.recipeCategories.Add(recipe.recipeCategoryID, ((Component) categoryContent).gameObject);
                }
                ((Component) newToggle).transform.SetParent((Transform) Util.rectTransform(this.recipeCategories[recipe.recipeCategoryID]));
              }
              Image image = KMonoBehaviourExtensions.GetComponentsInChildrenOnly<Image>(entryGO)[2];
              if (recipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.Ingredient)
              {
                image.sprite = uiSprite1.first;
                ((Graphic) image).color = uiSprite1.second;
              }
              else if (recipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.HEP)
              {
                image.sprite = this.radboltSprite;
              }
              else
              {
                image.sprite = uiSprite2.first;
                ((Graphic) image).color = uiSprite2.second;
              }
              ((TMP_Text) entryGO.GetComponentInChildren<LocText>()).text = recipe.GetUIName(false);
              bool flag2 = this.HasAllRecipeRequirements(recipe);
              ((Graphic) image).material = flag2 ? Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial;
              this.RefreshQueueCountDisplay(entryGO, this.targetFab);
              entryGO.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("DecrementButton").onClick = (System.Action) (() =>
              {
                target.DecrementRecipeQueueCount(recipe, false);
                this.RefreshQueueCountDisplay(entryGO, target);
              });
              entryGO.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("IncrementButton").onClick = (System.Action) (() =>
              {
                target.IncrementRecipeQueueCount(recipe);
                this.RefreshQueueCountDisplay(entryGO, target);
              });
              entryGO.gameObject.SetActive(true);
              break;
            default:
              newToggle = Util.KInstantiateUI<KToggle>(this.recipeButton, this.recipeGrid, false);
              entryGO = ((Component) newToggle).gameObject;
              Image componentInChildrenOnly = KMonoBehaviourExtensions.GetComponentInChildrenOnly<Image>(((Component) newToggle).gameObject);
              if (target.sideScreenStyle == ComplexFabricatorSideScreen.StyleSetting.GridInput || target.sideScreenStyle == ComplexFabricatorSideScreen.StyleSetting.ListInput)
              {
                componentInChildrenOnly.sprite = uiSprite1.first;
                ((Graphic) componentInChildrenOnly).color = uiSprite1.second;
                break;
              }
              componentInChildrenOnly.sprite = uiSprite2.first;
              ((Graphic) componentInChildrenOnly).color = uiSprite2.second;
              break;
          }
          if (this.targetFab.sideScreenStyle == ComplexFabricatorSideScreen.StyleSetting.ClassicFabricator)
            ((TMP_Text) ((Component) newToggle).GetComponentInChildren<LocText>()).text = recipe.results[0].material.ProperName();
          else if (this.targetFab.sideScreenStyle != ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid)
            ((TMP_Text) ((Component) newToggle).GetComponentInChildren<LocText>()).text = string.Format((string) STRINGS.UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_WITH_NEWLINES, (object) recipe.ingredients[0].material.ProperName(), (object) recipe.results[0].material.ProperName());
          ToolTip component3 = entryGO.GetComponent<ToolTip>();
          component3.toolTipPosition = (ToolTip.TooltipPosition) 6;
          component3.parentPositionAnchor = new Vector2(0.0f, 0.5f);
          component3.tooltipPivot = new Vector2(1f, 1f);
          component3.tooltipPositionOffset = new Vector2(-24f, 20f);
          component3.ClearMultiStringTooltip();
          component3.AddMultiStringTooltip(recipe.GetUIName(false), this.styleTooltipHeader);
          component3.AddMultiStringTooltip(recipe.description, this.styleTooltipBody);
          newToggle.onClick += (System.Action) (() => this.ToggleClicked(newToggle));
          entryGO.SetActive(true);
          this.recipeToggles.Add(entryGO);
        }
      }
      if (this.recipeToggles.Count > 0)
      {
        ((Component) this.buttonScrollContainer).GetComponent<LayoutElement>().minHeight = Mathf.Min(451f, (float) (2.0 + (double) num * (double) this.recipeButtonQueueHybrid.GetComponent<LayoutElement>().minHeight));
        ((TMP_Text) this.subtitleLabel).SetText((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.SUBTITLE);
        ((Component) this.noRecipesDiscoveredLabel).gameObject.SetActive(false);
      }
      else
      {
        ((TMP_Text) this.subtitleLabel).SetText((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED);
        ((TMP_Text) this.noRecipesDiscoveredLabel).SetText((string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.NORECIPEDISCOVERED_BODY);
        ((Component) this.noRecipesDiscoveredLabel).gameObject.SetActive(true);
        ((Component) this.buttonScrollContainer).GetComponent<LayoutElement>().minHeight = ((TMP_Text) this.noRecipesDiscoveredLabel).rectTransform.sizeDelta.y + 10f;
      }
      this.RefreshIngredientAvailabilityVis();
    }
  }

  public void RefreshQueueCountDisplayForRecipe(ComplexRecipe recipe, ComplexFabricator fabricator)
  {
    GameObject entryGO = this.recipeToggles.Find((Predicate<GameObject>) (match => this.recipeMap[match] == recipe));
    if (!Object.op_Inequality((Object) entryGO, (Object) null))
      return;
    this.RefreshQueueCountDisplay(entryGO, fabricator);
  }

  private void RefreshQueueCountDisplay(GameObject entryGO, ComplexFabricator fabricator)
  {
    HierarchyReferences component = entryGO.GetComponent<HierarchyReferences>();
    bool flag = fabricator.GetRecipeQueueCount(this.recipeMap[entryGO]) == ComplexFabricator.QUEUE_INFINITE;
    ((TMP_Text) component.GetReference<LocText>("CountLabel")).text = flag ? "" : fabricator.GetRecipeQueueCount(this.recipeMap[entryGO]).ToString();
    ((Component) component.GetReference<RectTransform>("InfiniteIcon")).gameObject.SetActive(flag);
  }

  private void ToggleClicked(KToggle toggle)
  {
    if (!this.recipeMap.ContainsKey(((Component) toggle).gameObject))
    {
      Debug.LogError((object) "Recipe not found on recipe list.");
    }
    else
    {
      if (Object.op_Equality((Object) this.selectedToggle, (Object) toggle))
      {
        this.selectedToggle.isOn = false;
        this.selectedToggle = (KToggle) null;
        this.selectedRecipe = (ComplexRecipe) null;
      }
      else
      {
        this.selectedToggle = toggle;
        this.selectedToggle.isOn = true;
        this.selectedRecipe = this.recipeMap[((Component) toggle).gameObject];
        this.selectedRecipeFabricatorMap[this.targetFab] = this.recipeToggles.IndexOf(((Component) toggle).gameObject);
      }
      this.RefreshIngredientAvailabilityVis();
      if (toggle.isOn)
      {
        this.recipeScreen = (SelectedRecipeQueueScreen) DetailsScreen.Instance.SetSecondarySideScreen((KScreen) this.recipeScreenPrefab, (string) STRINGS.UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_DETAILS);
        this.recipeScreen.SetRecipe(this, this.targetFab, this.selectedRecipe);
      }
      else
        DetailsScreen.Instance.ClearSecondarySideScreen();
    }
  }

  public void CycleRecipe(int increment)
  {
    int num = 0;
    if (Object.op_Inequality((Object) this.selectedToggle, (Object) null))
      num = this.recipeToggles.IndexOf(((Component) this.selectedToggle).gameObject);
    int index = (num + increment) % this.recipeToggles.Count;
    if (index < 0)
      index = this.recipeToggles.Count + index;
    this.ToggleClicked(this.recipeToggles[index].GetComponent<KToggle>());
  }

  private bool HasAnyRecipeRequirements(ComplexRecipe recipe)
  {
    foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
    {
      if ((double) this.targetFab.GetMyWorld().worldInventory.GetAmountWithoutTag(ingredient.material, true, this.targetFab.ForbiddenTags) + (double) this.targetFab.inStorage.GetAmountAvailable(ingredient.material, this.targetFab.ForbiddenTags) + (double) this.targetFab.buildStorage.GetAmountAvailable(ingredient.material, this.targetFab.ForbiddenTags) >= (double) ingredient.amount)
        return true;
    }
    return false;
  }

  private bool HasAllRecipeRequirements(ComplexRecipe recipe)
  {
    bool flag = true;
    foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
    {
      if ((double) this.targetFab.GetMyWorld().worldInventory.GetAmountWithoutTag(ingredient.material, true, this.targetFab.ForbiddenTags) + (double) this.targetFab.inStorage.GetAmountAvailable(ingredient.material, this.targetFab.ForbiddenTags) + (double) this.targetFab.buildStorage.GetAmountAvailable(ingredient.material, this.targetFab.ForbiddenTags) < (double) ingredient.amount)
      {
        flag = false;
        break;
      }
    }
    return flag;
  }

  private bool AnyRecipeRequirementsDiscovered(ComplexRecipe recipe)
  {
    foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
    {
      if (DiscoveredResources.Instance.IsDiscovered(ingredient.material))
        return true;
    }
    return false;
  }

  private void Update() => this.RefreshIngredientAvailabilityVis();

  private void RefreshIngredientAvailabilityVis()
  {
    foreach (KeyValuePair<GameObject, ComplexRecipe> recipe in this.recipeMap)
    {
      HierarchyReferences component1 = recipe.Key.GetComponent<HierarchyReferences>();
      bool flag = this.HasAllRecipeRequirements(recipe.Value);
      KToggle component2 = recipe.Key.GetComponent<KToggle>();
      if (flag)
      {
        if (this.selectedRecipe == recipe.Value)
          component2.ActivateFlourish(true, (ImageToggleState.State) 2);
        else
          component2.ActivateFlourish(false, (ImageToggleState.State) 1);
      }
      else if (this.selectedRecipe == recipe.Value)
        component2.ActivateFlourish(true, (ImageToggleState.State) 3);
      else
        component2.ActivateFlourish(false, (ImageToggleState.State) 0);
      ((Graphic) component1.GetReference<LocText>("Label")).color = flag ? Color.black : new Color(0.22f, 0.22f, 0.22f, 1f);
    }
  }

  private Element[] GetRecipeElements(Recipe recipe)
  {
    Element[] recipeElements = new Element[recipe.Ingredients.Count];
    for (int index = 0; index < recipe.Ingredients.Count; ++index)
    {
      Tag tag = recipe.Ingredients[index].tag;
      foreach (Element element in ElementLoader.elements)
      {
        if (Tag.op_Equality(GameTagExtensions.Create(element.id), tag))
        {
          recipeElements[index] = element;
          break;
        }
      }
    }
    return recipeElements;
  }

  public enum StyleSetting
  {
    GridResult,
    ListResult,
    GridInput,
    ListInput,
    ListInputOutput,
    GridInputOutput,
    ClassicFabricator,
    ListQueueHybrid,
  }
}
