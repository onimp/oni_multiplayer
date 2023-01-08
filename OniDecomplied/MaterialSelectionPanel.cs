// Decompiled with JetBrains decompiler
// Type: MaterialSelectionPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSelectionPanel : KScreen
{
  public Dictionary<KToggle, Tag> ElementToggles = new Dictionary<KToggle, Tag>();
  private List<MaterialSelector> MaterialSelectors = new List<MaterialSelector>();
  private List<Tag> currentSelectedElements = new List<Tag>();
  [SerializeField]
  protected PriorityScreen priorityScreenPrefab;
  [SerializeField]
  protected GameObject priorityScreenParent;
  [SerializeField]
  protected BuildToolRotateButtonUI buildToolRotateButton;
  private PriorityScreen priorityScreen;
  public GameObject MaterialSelectorTemplate;
  public GameObject ResearchRequired;
  private Recipe activeRecipe;
  private static Dictionary<Tag, List<Tag>> elementsWithTag = new Dictionary<Tag, List<Tag>>();
  private MaterialSelectionPanel.GetBuildableStateDelegate GetBuildableState;
  private MaterialSelectionPanel.GetBuildableTooltipDelegate GetBuildableTooltip;
  private List<int> gameSubscriptionHandles = new List<int>();

  public static void ClearStatics() => MaterialSelectionPanel.elementsWithTag.Clear();

  public Tag CurrentSelectedElement => this.MaterialSelectors[0].CurrentSelectedElement;

  public IList<Tag> GetSelectedElementAsList
  {
    get
    {
      this.currentSelectedElements.Clear();
      foreach (MaterialSelector materialSelector in this.MaterialSelectors)
      {
        if (((Component) materialSelector).gameObject.activeSelf)
        {
          Debug.Assert(Tag.op_Inequality(materialSelector.CurrentSelectedElement, Tag.op_Implicit((string) null)));
          this.currentSelectedElements.Add(materialSelector.CurrentSelectedElement);
        }
      }
      return (IList<Tag>) this.currentSelectedElements;
    }
  }

  public PriorityScreen PriorityScreen => this.priorityScreen;

  protected virtual void OnPrefabInit()
  {
    MaterialSelectionPanel.elementsWithTag.Clear();
    base.OnPrefabInit();
    this.ConsumeMouseScroll = true;
    for (int index = 0; index < 3; ++index)
    {
      MaterialSelector materialSelector = Util.KInstantiateUI<MaterialSelector>(this.MaterialSelectorTemplate, ((Component) this).gameObject, false);
      materialSelector.selectorIndex = index;
      this.MaterialSelectors.Add(materialSelector);
    }
    ((Component) this.MaterialSelectors[0]).gameObject.SetActive(true);
    this.MaterialSelectorTemplate.SetActive(false);
    this.ResearchRequired.SetActive(false);
    this.priorityScreen = Util.KInstantiateUI<PriorityScreen>(((Component) this.priorityScreenPrefab).gameObject, this.priorityScreenParent, false);
    this.priorityScreen.InstantiateButtons(new Action<PrioritySetting>(this.OnPriorityClicked));
    this.priorityScreenParent.transform.SetAsLastSibling();
    this.gameSubscriptionHandles.Add(Game.Instance.Subscribe(-107300940, (Action<object>) (d => this.RefreshSelectors())));
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.activateOnSpawn = true;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    foreach (int subscriptionHandle in this.gameSubscriptionHandles)
      Game.Instance.Unsubscribe(subscriptionHandle);
    this.gameSubscriptionHandles.Clear();
  }

  public void AddSelectAction(MaterialSelector.SelectMaterialActions action) => this.MaterialSelectors.ForEach((Action<MaterialSelector>) (selector => selector.selectMaterialActions += action));

  public void ClearSelectActions() => this.MaterialSelectors.ForEach((Action<MaterialSelector>) (selector => selector.selectMaterialActions = (MaterialSelector.SelectMaterialActions) null));

  public void ClearMaterialToggles() => this.MaterialSelectors.ForEach((Action<MaterialSelector>) (selector => selector.ClearMaterialToggles()));

  public void ConfigureScreen(
    Recipe recipe,
    MaterialSelectionPanel.GetBuildableStateDelegate buildableStateCB,
    MaterialSelectionPanel.GetBuildableTooltipDelegate buildableTooltipCB)
  {
    this.activeRecipe = recipe;
    this.GetBuildableState = buildableStateCB;
    this.GetBuildableTooltip = buildableTooltipCB;
    this.RefreshSelectors();
  }

  public bool AllSelectorsSelected()
  {
    bool flag = false;
    foreach (MaterialSelector materialSelector in this.MaterialSelectors)
    {
      flag = flag || ((Component) materialSelector).gameObject.activeInHierarchy;
      if (((Component) materialSelector).gameObject.activeInHierarchy && Tag.op_Equality(materialSelector.CurrentSelectedElement, Tag.op_Implicit((string) null)))
        return false;
    }
    return flag;
  }

  public void RefreshSelectors()
  {
    if (this.activeRecipe == null || !((Component) this).gameObject.activeInHierarchy)
      return;
    this.MaterialSelectors.ForEach((Action<MaterialSelector>) (selector => ((Component) selector).gameObject.SetActive(false)));
    BuildingDef buildingDef = this.activeRecipe.GetBuildingDef();
    int num = this.GetBuildableState(buildingDef) ? 1 : 0;
    string str = this.GetBuildableTooltip(buildingDef);
    if (num == 0)
    {
      this.ResearchRequired.SetActive(true);
      LocText[] componentsInChildren = this.ResearchRequired.GetComponentsInChildren<LocText>();
      ((TMP_Text) componentsInChildren[0]).text = "";
      ((TMP_Text) componentsInChildren[1]).text = str;
      ((Graphic) componentsInChildren[1]).color = Constants.NEGATIVE_COLOR;
      ((Component) this.priorityScreen).gameObject.SetActive(false);
      ((Component) this.buildToolRotateButton).gameObject.SetActive(false);
    }
    else
    {
      this.ResearchRequired.SetActive(false);
      for (int index = 0; index < this.activeRecipe.Ingredients.Count; ++index)
      {
        ((Component) this.MaterialSelectors[index]).gameObject.SetActive(true);
        this.MaterialSelectors[index].ConfigureScreen(this.activeRecipe.Ingredients[index], this.activeRecipe);
      }
      ((Component) this.priorityScreen).gameObject.SetActive(true);
      ((KMonoBehaviour) this.priorityScreen).transform.SetAsLastSibling();
      ((Component) this.buildToolRotateButton).gameObject.SetActive(true);
      ((Component) this.buildToolRotateButton).transform.SetAsLastSibling();
    }
  }

  public void UpdateResourceToggleValues() => this.MaterialSelectors.ForEach((Action<MaterialSelector>) (selector =>
  {
    if (!((Component) selector).gameObject.activeSelf)
      return;
    selector.RefreshToggleContents();
  }));

  public bool AutoSelectAvailableMaterial()
  {
    bool flag = true;
    for (int index = 0; index < this.MaterialSelectors.Count; ++index)
    {
      if (!this.MaterialSelectors[index].AutoSelectAvailableMaterial())
        flag = false;
    }
    return flag;
  }

  public void SelectSourcesMaterials(Building building)
  {
    Tag[] tagArray = (Tag[]) null;
    Deconstructable component1 = ((Component) building).gameObject.GetComponent<Deconstructable>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      tagArray = component1.constructionElements;
    Constructable component2 = ((Component) building).GetComponent<Constructable>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      tagArray = ((IEnumerable<Tag>) component2.SelectedElementsTags).ToArray<Tag>();
    if (tagArray == null)
      return;
    for (int index = 0; index < Mathf.Min(tagArray.Length, this.MaterialSelectors.Count); ++index)
    {
      if (this.MaterialSelectors[index].ElementToggles.ContainsKey(tagArray[index]))
        this.MaterialSelectors[index].OnSelectMaterial(tagArray[index], this.activeRecipe);
    }
  }

  public static MaterialSelectionPanel.SelectedElemInfo Filter(Tag materialCategoryTag)
  {
    MaterialSelectionPanel.SelectedElemInfo selectedElemInfo = new MaterialSelectionPanel.SelectedElemInfo();
    selectedElemInfo.element = Tag.op_Implicit((string) null);
    selectedElemInfo.kgAvailable = 0.0f;
    if (Object.op_Equality((Object) DiscoveredResources.Instance, (Object) null) || ElementLoader.elements == null || ElementLoader.elements.Count == 0)
      return selectedElemInfo;
    List<Tag> tagList = (List<Tag>) null;
    if (!MaterialSelectionPanel.elementsWithTag.TryGetValue(materialCategoryTag, out tagList))
    {
      tagList = new List<Tag>();
      foreach (Element element in ElementLoader.elements)
      {
        if (Tag.op_Equality(element.tag, materialCategoryTag) || element.HasTag(materialCategoryTag))
          tagList.Add(element.tag);
      }
      foreach (Tag materialBuildingElement in GameTags.MaterialBuildingElements)
      {
        if (Tag.op_Equality(materialBuildingElement, materialCategoryTag))
        {
          foreach (GameObject gameObject in Assets.GetPrefabsWithTag(materialBuildingElement))
          {
            KPrefabID component = gameObject.GetComponent<KPrefabID>();
            if (Object.op_Inequality((Object) component, (Object) null) && !tagList.Contains(component.PrefabTag))
              tagList.Add(component.PrefabTag);
          }
        }
      }
      MaterialSelectionPanel.elementsWithTag[materialCategoryTag] = tagList;
    }
    foreach (Tag tag in tagList)
    {
      float amount = ClusterManager.Instance.activeWorld.worldInventory.GetAmount(tag, true);
      if ((double) amount > (double) selectedElemInfo.kgAvailable)
      {
        selectedElemInfo.kgAvailable = amount;
        selectedElemInfo.element = tag;
      }
    }
    return selectedElemInfo;
  }

  public void ToggleShowDescriptorPanels(bool show)
  {
    for (int index = 0; index < this.MaterialSelectors.Count; ++index)
    {
      if (Object.op_Inequality((Object) this.MaterialSelectors[index], (Object) null))
        this.MaterialSelectors[index].ToggleShowDescriptorsPanel(show);
    }
  }

  private void OnPriorityClicked(PrioritySetting priority) => this.priorityScreen.SetScreenPriority(priority);

  public delegate bool GetBuildableStateDelegate(BuildingDef def);

  public delegate string GetBuildableTooltipDelegate(BuildingDef def);

  public delegate void SelectElement(Element element, float kgAvailable, float recipe_amount);

  public struct SelectedElemInfo
  {
    public Tag element;
    public float kgAvailable;
  }
}
