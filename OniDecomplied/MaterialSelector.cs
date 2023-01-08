// Decompiled with JetBrains decompiler
// Type: MaterialSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSelector : KScreen
{
  public Tag CurrentSelectedElement;
  public Dictionary<Tag, KToggle> ElementToggles = new Dictionary<Tag, KToggle>();
  public int selectorIndex;
  public MaterialSelector.SelectMaterialActions selectMaterialActions;
  public MaterialSelector.SelectMaterialActions deselectMaterialActions;
  private ToggleGroup toggleGroup;
  public GameObject TogglePrefab;
  public GameObject LayoutContainer;
  public KScrollRect ScrollRect;
  public GameObject Scrollbar;
  public GameObject Headerbar;
  public GameObject BadBG;
  public LocText NoMaterialDiscovered;
  public GameObject MaterialDescriptionPane;
  public LocText MaterialDescriptionText;
  public DescriptorPanel MaterialEffectsPane;
  public GameObject DescriptorsPanel;
  private KToggle selectedToggle;
  private Recipe.Ingredient activeIngredient;
  private Recipe activeRecipe;
  private float activeMass;
  private List<Tag> elementsToSort = new List<Tag>();

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.toggleGroup = ((Component) this).GetComponent<ToggleGroup>();
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyDown(e);
  }

  public void ClearMaterialToggles()
  {
    this.CurrentSelectedElement = Tag.op_Implicit((string) null);
    ((Component) this.NoMaterialDiscovered).gameObject.SetActive(false);
    foreach (KeyValuePair<Tag, KToggle> elementToggle in this.ElementToggles)
    {
      ((Component) elementToggle.Value).gameObject.SetActive(false);
      Util.KDestroyGameObject(((Component) elementToggle.Value).gameObject);
    }
    this.ElementToggles.Clear();
  }

  public void ConfigureScreen(Recipe.Ingredient ingredient, Recipe recipe)
  {
    this.ClearMaterialToggles();
    this.activeIngredient = ingredient;
    this.activeRecipe = recipe;
    this.activeMass = ingredient.amount;
    List<Tag> tagList = new List<Tag>();
    foreach (Element element in ElementLoader.elements)
    {
      if (element.IsSolid && (Tag.op_Equality(element.tag, ingredient.tag) || element.HasTag(ingredient.tag)))
        tagList.Add(element.tag);
    }
    foreach (Tag materialBuildingElement in GameTags.MaterialBuildingElements)
    {
      if (Tag.op_Equality(materialBuildingElement, ingredient.tag))
      {
        foreach (GameObject gameObject in Assets.GetPrefabsWithTag(materialBuildingElement))
        {
          KPrefabID component = gameObject.GetComponent<KPrefabID>();
          if (Object.op_Inequality((Object) component, (Object) null) && !tagList.Contains(component.PrefabTag))
            tagList.Add(component.PrefabTag);
        }
      }
    }
    foreach (Tag tag in tagList)
    {
      if (!this.ElementToggles.ContainsKey(tag))
      {
        GameObject gameObject = Util.KInstantiate(this.TogglePrefab, this.LayoutContainer, "MaterialSelection_" + tag.ProperName());
        gameObject.transform.localScale = Vector3.one;
        gameObject.SetActive(true);
        KToggle component = gameObject.GetComponent<KToggle>();
        this.ElementToggles.Add(tag, component);
        ((Toggle) component).group = this.toggleGroup;
        gameObject.gameObject.GetComponent<ToolTip>().toolTip = tag.ProperName();
      }
    }
    this.ConfigureMaterialTooltips();
    this.RefreshToggleContents();
  }

  private void SetToggleBGImage(KToggle toggle, Tag elem)
  {
    if (Object.op_Equality((Object) toggle, (Object) this.selectedToggle))
    {
      ((Graphic) ((Component) toggle).GetComponentsInChildren<Image>()[1]).material = GlobalResources.Instance().AnimUIMaterial;
      ((Component) toggle).GetComponent<ImageToggleState>().SetActive();
    }
    else if ((double) ClusterManager.Instance.activeWorld.worldInventory.GetAmount(elem, true) >= (double) this.activeMass || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
    {
      ((Graphic) ((Component) toggle).GetComponentsInChildren<Image>()[1]).material = GlobalResources.Instance().AnimUIMaterial;
      ((Graphic) ((Component) toggle).GetComponentsInChildren<Image>()[1]).color = Color.white;
      ((Component) toggle).GetComponent<ImageToggleState>().SetInactive();
    }
    else
    {
      ((Graphic) ((Component) toggle).GetComponentsInChildren<Image>()[1]).material = GlobalResources.Instance().AnimMaterialUIDesaturated;
      ((Graphic) ((Component) toggle).GetComponentsInChildren<Image>()[1]).color = new Color(1f, 1f, 1f, 0.6f);
      if (MaterialSelector.AllowInsufficientMaterialBuild())
        return;
      ((Component) toggle).GetComponent<ImageToggleState>().SetDisabled();
    }
  }

  public void OnSelectMaterial(Tag elem, Recipe recipe, bool focusScrollRect = false)
  {
    KToggle elementToggle1 = this.ElementToggles[elem];
    if (Object.op_Inequality((Object) elementToggle1, (Object) this.selectedToggle))
    {
      this.selectedToggle = elementToggle1;
      if (recipe != null)
        SaveGame.Instance.materialSelectorSerializer.SetSelectedElement(ClusterManager.Instance.activeWorldId, this.selectorIndex, recipe.Result, elem);
      this.CurrentSelectedElement = elem;
      if (this.selectMaterialActions != null)
        this.selectMaterialActions();
      this.UpdateHeader();
      this.SetDescription(elem);
      this.SetEffects(elem);
      if (!this.MaterialDescriptionPane.gameObject.activeSelf && !((Component) this.MaterialEffectsPane).gameObject.activeSelf)
        this.DescriptorsPanel.SetActive(false);
      else
        this.DescriptorsPanel.SetActive(true);
    }
    if (focusScrollRect && this.ElementToggles.Count > 1)
    {
      List<Tag> tagList = new List<Tag>();
      foreach (KeyValuePair<Tag, KToggle> elementToggle2 in this.ElementToggles)
        tagList.Add(elementToggle2.Key);
      tagList.Sort(new Comparison<Tag>(this.ElementSorter));
      int num1 = tagList.IndexOf(elem);
      int constraintCount = this.LayoutContainer.GetComponent<GridLayoutGroup>().constraintCount;
      int num2 = constraintCount;
      ((UnityEngine.UI.ScrollRect) this.ScrollRect).normalizedPosition = new Vector2(0.0f, 1f - (float) (num1 / num2 / Math.Max((tagList.Count - 1) / constraintCount, 1)));
    }
    this.RefreshToggleContents();
  }

  public void RefreshToggleContents()
  {
    foreach (KeyValuePair<Tag, KToggle> elementToggle in this.ElementToggles)
    {
      KToggle ktoggle = elementToggle.Value;
      Tag elem = elementToggle.Key;
      GameObject gameObject = ((Component) ktoggle).gameObject;
      bool flag = DiscoveredResources.Instance.IsDiscovered(elem) || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;
      if (gameObject.activeSelf != flag)
        gameObject.SetActive(flag);
      if (flag)
      {
        LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>();
        LocText locText1 = componentsInChildren[0];
        LocText locText2 = componentsInChildren[1];
        Image componentsInChild = gameObject.GetComponentsInChildren<Image>()[1];
        string str = Util.FormatWholeNumber(ClusterManager.Instance.activeWorld.worldInventory.GetAmount(elem, true));
        ((TMP_Text) locText2).text = str;
        ((TMP_Text) locText1).text = Util.FormatWholeNumber(this.activeMass);
        GameObject prefab = Assets.TryGetPrefab(elementToggle.Key);
        if (Object.op_Inequality((Object) prefab, (Object) null))
        {
          KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
          componentsInChild.sprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0]);
        }
        this.SetToggleBGImage(elementToggle.Value, elementToggle.Key);
        ktoggle.soundPlayer.AcceptClickCondition = (Func<bool>) (() => this.IsEnoughMass(elem));
        ktoggle.ClearOnClick();
        if (this.IsEnoughMass(elem))
          ktoggle.onClick += (System.Action) (() => this.OnSelectMaterial(elem, this.activeRecipe));
      }
    }
    this.SortElementToggles();
    this.UpdateHeader();
  }

  private bool IsEnoughMass(Tag t) => (double) ClusterManager.Instance.activeWorld.worldInventory.GetAmount(t, true) >= (double) this.activeMass || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || MaterialSelector.AllowInsufficientMaterialBuild();

  public bool AutoSelectAvailableMaterial()
  {
    if (this.activeRecipe == null || this.ElementToggles.Count == 0)
      return false;
    Tag previousElement = SaveGame.Instance.materialSelectorSerializer.GetPreviousElement(ClusterManager.Instance.activeWorldId, this.selectorIndex, this.activeRecipe.Result);
    if (Tag.op_Inequality(previousElement, Tag.op_Implicit((string) null)))
    {
      KToggle ktoggle;
      this.ElementToggles.TryGetValue(previousElement, out ktoggle);
      if (Object.op_Inequality((Object) ktoggle, (Object) null) && (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || (double) ClusterManager.Instance.activeWorld.worldInventory.GetAmount(previousElement, true) >= (double) this.activeMass))
      {
        this.OnSelectMaterial(previousElement, this.activeRecipe, true);
        return true;
      }
    }
    float num = -1f;
    List<Tag> tagList = new List<Tag>();
    foreach (KeyValuePair<Tag, KToggle> elementToggle in this.ElementToggles)
      tagList.Add(elementToggle.Key);
    tagList.Sort(new Comparison<Tag>(this.ElementSorter));
    if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
    {
      this.OnSelectMaterial(tagList[0], this.activeRecipe, true);
      return true;
    }
    Tag elem = Tag.op_Implicit((string) null);
    foreach (Tag tag in tagList)
    {
      float amount = ClusterManager.Instance.activeWorld.worldInventory.GetAmount(tag, true);
      if ((double) amount >= (double) this.activeMass && (double) amount > (double) num)
      {
        num = amount;
        elem = tag;
      }
    }
    if (!Tag.op_Inequality(elem, Tag.op_Implicit((string) null)))
      return false;
    this.OnSelectMaterial(elem, this.activeRecipe, true);
    return true;
  }

  private void SortElementToggles()
  {
    bool flag = false;
    int num = -1;
    this.elementsToSort.Clear();
    foreach (KeyValuePair<Tag, KToggle> elementToggle in this.ElementToggles)
    {
      if (((Component) elementToggle.Value).gameObject.activeSelf)
        this.elementsToSort.Add(elementToggle.Key);
    }
    this.elementsToSort.Sort(new Comparison<Tag>(this.ElementSorter));
    for (int index = 0; index < this.elementsToSort.Count; ++index)
    {
      int siblingIndex = ((Component) this.ElementToggles[this.elementsToSort[index]]).transform.GetSiblingIndex();
      if (siblingIndex <= num)
      {
        flag = true;
        break;
      }
      num = siblingIndex;
    }
    if (flag)
    {
      foreach (Tag key in this.elementsToSort)
        ((Component) this.ElementToggles[key]).transform.SetAsLastSibling();
    }
    this.UpdateScrollBar();
  }

  private void ConfigureMaterialTooltips()
  {
    foreach (KeyValuePair<Tag, KToggle> elementToggle in this.ElementToggles)
    {
      ToolTip component = ((Component) elementToggle.Value).gameObject.GetComponent<ToolTip>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.toolTip = GameUtil.GetMaterialTooltips(elementToggle.Key);
    }
  }

  private void UpdateScrollBar()
  {
    int num = 0;
    foreach (KeyValuePair<Tag, KToggle> elementToggle in this.ElementToggles)
    {
      if (((Component) elementToggle.Value).gameObject.activeSelf)
        ++num;
    }
    if (this.Scrollbar.activeSelf != num > 5)
      this.Scrollbar.SetActive(num > 5);
    ((Component) this.ScrollRect).GetComponent<LayoutElement>().minHeight = (float) (74 * (num <= 5 ? 1 : 2));
  }

  private void UpdateHeader()
  {
    if (this.activeIngredient == null)
      return;
    int num = 0;
    foreach (KeyValuePair<Tag, KToggle> elementToggle in this.ElementToggles)
    {
      if (((Component) elementToggle.Value).gameObject.activeSelf)
        ++num;
    }
    LocText componentInChildren = this.Headerbar.GetComponentInChildren<LocText>();
    if (num == 0)
    {
      ((TMP_Text) componentInChildren).text = string.Format((string) STRINGS.UI.PRODUCTINFO_MISSINGRESOURCES_TITLE, (object) this.activeIngredient.tag.ProperName(), (object) GameUtil.GetFormattedMass(this.activeIngredient.amount));
      ((TMP_Text) this.NoMaterialDiscovered).text = string.Format((string) STRINGS.UI.PRODUCTINFO_MISSINGRESOURCES_DESC, (object) this.activeIngredient.tag.ProperName());
      ((Component) this.NoMaterialDiscovered).gameObject.SetActive(true);
      ((Graphic) this.NoMaterialDiscovered).color = Constants.NEGATIVE_COLOR;
      this.BadBG.SetActive(true);
      this.Scrollbar.SetActive(false);
      this.LayoutContainer.SetActive(false);
    }
    else
    {
      ((TMP_Text) componentInChildren).text = string.Format((string) STRINGS.UI.PRODUCTINFO_SELECTMATERIAL, (object) this.activeIngredient.tag.ProperName());
      ((Component) this.NoMaterialDiscovered).gameObject.SetActive(false);
      this.BadBG.SetActive(false);
      this.LayoutContainer.SetActive(true);
      this.UpdateScrollBar();
    }
  }

  public void ToggleShowDescriptorsPanel(bool show) => this.DescriptorsPanel.gameObject.SetActive(show);

  private void SetDescription(Tag element)
  {
    StringEntry stringEntry = (StringEntry) null;
    if (Strings.TryGet(new StringKey("STRINGS.ELEMENTS." + element.ToString().ToUpper() + ".BUILD_DESC"), ref stringEntry))
    {
      ((TMP_Text) this.MaterialDescriptionText).text = ((object) stringEntry).ToString();
      this.MaterialDescriptionPane.SetActive(true);
    }
    else
      this.MaterialDescriptionPane.SetActive(false);
  }

  private void SetEffects(Tag element)
  {
    List<Descriptor> materialDescriptors = GameUtil.GetMaterialDescriptors(element);
    if (materialDescriptors.Count > 0)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) ELEMENTS.MATERIAL_MODIFIERS.EFFECTS_HEADER, (string) ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.EFFECTS_HEADER, (Descriptor.DescriptorType) 1);
      materialDescriptors.Insert(0, descriptor);
      ((Component) this.MaterialEffectsPane).gameObject.SetActive(true);
      this.MaterialEffectsPane.SetDescriptors((IList<Descriptor>) materialDescriptors);
    }
    else
      ((Component) this.MaterialEffectsPane).gameObject.SetActive(false);
  }

  public static bool AllowInsufficientMaterialBuild() => GenericGameSettings.instance.allowInsufficientMaterialBuild;

  private int ElementSorter(Tag at, Tag bt)
  {
    GameObject prefab1 = Assets.TryGetPrefab(at);
    IHasSortOrder hasSortOrder1 = Object.op_Inequality((Object) prefab1, (Object) null) ? prefab1.GetComponent<IHasSortOrder>() : (IHasSortOrder) null;
    GameObject prefab2 = Assets.TryGetPrefab(bt);
    IHasSortOrder hasSortOrder2 = Object.op_Inequality((Object) prefab2, (Object) null) ? prefab2.GetComponent<IHasSortOrder>() : (IHasSortOrder) null;
    if (hasSortOrder1 == null || hasSortOrder2 == null)
      return 0;
    Element element1 = ElementLoader.GetElement(at);
    Element element2 = ElementLoader.GetElement(bt);
    return element1 != null && element2 != null && element1.buildMenuSort == element2.buildMenuSort ? element1.idx.CompareTo(element2.idx) : hasSortOrder1.sortOrder.CompareTo(hasSortOrder2.sortOrder);
  }

  public delegate void SelectMaterialActions();
}
