// Decompiled with JetBrains decompiler
// Type: KleiInventoryScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KleiInventoryScreen : KModalScreen
{
  [Header("Header")]
  [SerializeField]
  private KButton closeButton;
  [Header("CategoryColumn")]
  [SerializeField]
  private RectTransform categoryListContent;
  [SerializeField]
  private GameObject categoryRowPrefab;
  private Dictionary<PermitCategory, MultiToggle> categoryToggles = new Dictionary<PermitCategory, MultiToggle>();
  private Dictionary<PermitCategory, bool> emptyCategories = new Dictionary<PermitCategory, bool>();
  [Header("ItemGalleryColumn")]
  [SerializeField]
  private LocText galleryHeaderLabel;
  [SerializeField]
  private RectTransform galleryGridContent;
  [SerializeField]
  private GameObject gridItemPrefab;
  private Dictionary<string, MultiToggle> galleryGridButtons = new Dictionary<string, MultiToggle>();
  private List<GameObject> recycledGalleryGridButtons = new List<GameObject>();
  private GridLayouter galleryGridLayouter;
  [Header("SelectionDetailsColumn")]
  [SerializeField]
  private LocText selectionHeaderLabel;
  [SerializeField]
  private KleiPermitDioramaVis permitVis;
  [SerializeField]
  private LocText selectionNameLabel;
  [SerializeField]
  private LocText selectionDescriptionLabel;
  [SerializeField]
  private LocText selectionFacadeForLabel;
  [SerializeField]
  private LocText selectionRarityDetailsLabel;
  [SerializeField]
  private LocText selectionOwnedCount;

  private string SelectedItemFacadeID { get; set; }

  private PermitCategory SelectedCategory { get; set; }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.closeButton.onClick += (System.Action) (() => this.Show(false));
    this.ConsumeMouseScroll = true;
    this.galleryGridLayouter = new GridLayouter()
    {
      minCellSize = 64f,
      maxCellSize = 96f,
      targetGridLayout = ((Component) this.galleryGridContent).GetComponent<GridLayoutGroup>()
    };
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
      this.Show(false);
    base.OnKeyDown(e);
  }

  public override float GetSortKey() => 20f;

  protected override void OnActivate() => base.OnShow(true);

  protected override void OnShow(bool show)
  {
    base.OnShow(show);
    if (!show)
      return;
    this.galleryGridLayouter.RequestGridResize();
    this.PopulateCategories();
    this.PopulateGallery();
    this.SelectCategory(PermitCategory.Building);
  }

  protected override void OnCmpEnable()
  {
    base.OnCmpEnable();
    KleiItemsStatusRefresher.AddOrGetListener((Component) this).OnRefreshUI((System.Action) (() =>
    {
      this.RefreshCategories();
      this.RefreshGallery();
      this.RefreshDetails();
    }));
    KleiItemsStatusRefresher.RequestRefreshFromServer();
  }

  private void Update() => this.galleryGridLayouter.CheckIfShouldResizeGrid();

  private GameObject GetAvailableGridButton()
  {
    if (this.recycledGalleryGridButtons.Count == 0)
      return Util.KInstantiateUI(this.gridItemPrefab, ((Component) this.galleryGridContent).gameObject, true);
    GameObject galleryGridButton = this.recycledGalleryGridButtons[0];
    this.recycledGalleryGridButtons.RemoveAt(0);
    return galleryGridButton;
  }

  private void RecycleGalleryGridButton(GameObject button)
  {
    button.GetComponent<MultiToggle>().onClick = (System.Action) null;
    this.recycledGalleryGridButtons.Add(button);
  }

  public void PopulateCategories()
  {
    foreach (KeyValuePair<PermitCategory, MultiToggle> categoryToggle in this.categoryToggles)
      Object.Destroy((Object) ((Component) categoryToggle.Value).gameObject);
    this.categoryToggles.Clear();
    this.emptyCategories.Clear();
    this.AddPermitCategory(PermitCategory.Building);
    this.AddPermitCategory(PermitCategory.Artwork);
    this.AddPermitCategory(PermitCategory.DupeTops);
    this.AddPermitCategory(PermitCategory.DupeBottoms);
    this.AddPermitCategory(PermitCategory.DupeGloves);
    this.AddPermitCategory(PermitCategory.DupeShoes);
  }

  private void AddPermitCategory(PermitCategory permitCategory)
  {
    GameObject gameObject = Util.KInstantiateUI(this.categoryRowPrefab, ((Component) this.categoryListContent).gameObject, true);
    HierarchyReferences component1 = gameObject.GetComponent<HierarchyReferences>();
    ((TMP_Text) component1.GetReference<LocText>("Label")).SetText(PermitCategories.GetUppercaseDisplayName(permitCategory));
    component1.GetReference<Image>("Icon").sprite = Assets.GetSprite(HashedString.op_Implicit(PermitCategories.GetIconName(permitCategory)));
    MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
    component2.onClick = (System.Action) (() => this.SelectCategory(permitCategory));
    this.categoryToggles.Add(permitCategory, component2);
    this.emptyCategories.Add(permitCategory, true);
  }

  public void PopulateGallery()
  {
    foreach (KeyValuePair<string, MultiToggle> galleryGridButton in this.galleryGridButtons)
      this.RecycleGalleryGridButton(((Component) galleryGridButton.Value).gameObject);
    this.galleryGridButtons.Clear();
    this.galleryGridLayouter.ImmediateSizeGridToScreenResolution();
    foreach (PermitResource resource in Db.Get().Permits.resources)
    {
      if (PermitResources.ShouldDisplayPermitInSupplyCloset(resource.Id))
        this.AddItemToGallery(resource.Id);
    }
  }

  private void AddItemToGallery(string facadeID)
  {
    if (this.galleryGridButtons.ContainsKey(facadeID))
      return;
    PermitPresentationInfo presentationInfo = PermitItems.GetPermitPresentationInfo(facadeID);
    this.emptyCategories[presentationInfo.category] = false;
    GameObject availableGridButton = this.GetAvailableGridButton();
    HierarchyReferences component1 = availableGridButton.GetComponent<HierarchyReferences>();
    Image reference1 = component1.GetReference<Image>("Icon");
    LocText reference2 = component1.GetReference<LocText>("OwnedCountLabel");
    Image reference3 = component1.GetReference<Image>("IsUnownedOverlay");
    MultiToggle component2 = availableGridButton.GetComponent<MultiToggle>();
    reference1.sprite = presentationInfo.sprite;
    if (presentationInfo.ownedCount.HasValue)
    {
      ((TMP_Text) reference2).text = STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", presentationInfo.ownedCount.Value.ToString());
      ((Component) reference2).gameObject.SetActive(presentationInfo.ownedCount.Value > 0);
      ((Component) reference3).gameObject.SetActive(presentationInfo.ownedCount.Value <= 0);
    }
    else
    {
      ((Component) reference2).gameObject.SetActive(false);
      ((Component) reference3).gameObject.SetActive(false);
    }
    component2.onClick += (System.Action) (() => this.SelectItem(facadeID));
    this.galleryGridButtons.Add(facadeID, component2);
    KleiItemsUI.ConfigureTooltipOn(availableGridButton, (Option<string>) KleiItemsUI.GetTooltipStringFor(presentationInfo));
  }

  public void SelectCategory(PermitCategory category)
  {
    if (this.emptyCategories[category])
      return;
    this.SelectedCategory = category;
    ((TMP_Text) this.galleryHeaderLabel).SetText(PermitCategories.GetDisplayName(category));
    this.RefreshCategories();
    this.SelectDefaultCategoryItem();
  }

  private void SelectDefaultCategoryItem()
  {
    foreach (KeyValuePair<string, MultiToggle> galleryGridButton in this.galleryGridButtons)
    {
      if (PermitItems.GetPermitPresentationInfo(galleryGridButton.Key).category == this.SelectedCategory)
      {
        this.SelectItem(galleryGridButton.Key);
        return;
      }
    }
    this.SelectItem((string) null);
  }

  public void SelectItem(string facadeID)
  {
    this.SelectedItemFacadeID = facadeID;
    this.RefreshGallery();
    this.RefreshDetails();
  }

  private void RefreshGallery()
  {
    foreach (KeyValuePair<string, MultiToggle> galleryGridButton in this.galleryGridButtons)
    {
      string str;
      MultiToggle multiToggle1;
      Util.Deconstruct<string, MultiToggle>(galleryGridButton, ref str, ref multiToggle1);
      string id = str;
      MultiToggle multiToggle2 = multiToggle1;
      PermitPresentationInfo presentationInfo = PermitItems.GetPermitPresentationInfo(id);
      ((Component) multiToggle2).gameObject.SetActive(presentationInfo.category == this.SelectedCategory);
      multiToggle2.ChangeState(id == this.SelectedItemFacadeID ? 1 : 0);
      HierarchyReferences component = ((Component) multiToggle2).gameObject.GetComponent<HierarchyReferences>();
      LocText reference1 = component.GetReference<LocText>("OwnedCountLabel");
      Image reference2 = component.GetReference<Image>("IsUnownedOverlay");
      if (presentationInfo.ownedCount.HasValue)
      {
        ((TMP_Text) reference1).text = STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", presentationInfo.ownedCount.Value.ToString());
        ((Component) reference1).gameObject.SetActive(presentationInfo.ownedCount.Value > 0);
        ((Component) reference2).gameObject.SetActive(presentationInfo.ownedCount.Value <= 0);
      }
      else
      {
        ((Component) reference1).gameObject.SetActive(false);
        ((Component) reference2).gameObject.SetActive(false);
      }
    }
  }

  private void RefreshCategories()
  {
    foreach (KeyValuePair<PermitCategory, MultiToggle> categoryToggle in this.categoryToggles)
    {
      PermitCategory key = categoryToggle.Key;
      if (this.emptyCategories[key])
        categoryToggle.Value.ChangeState(2);
      else
        categoryToggle.Value.ChangeState(key == this.SelectedCategory ? 1 : 0);
    }
  }

  private void RefreshDetails()
  {
    PermitResource permit = Db.Get().Permits.TryGet(this.SelectedItemFacadeID);
    PermitPresentationInfo presentationInfo = PermitItems.GetPermitPresentationInfo(this.SelectedItemFacadeID);
    this.permitVis.ConfigureWith(permit, presentationInfo);
    ((TMP_Text) this.selectionHeaderLabel).SetText(presentationInfo.name);
    ((TMP_Text) this.selectionNameLabel).SetText(presentationInfo.name);
    ((Component) this.selectionDescriptionLabel).gameObject.SetActive(!string.IsNullOrWhiteSpace(presentationInfo.description));
    ((TMP_Text) this.selectionDescriptionLabel).SetText(presentationInfo.description);
    ((Component) this.selectionFacadeForLabel).gameObject.SetActive(!string.IsNullOrWhiteSpace(presentationInfo.facadeFor));
    ((TMP_Text) this.selectionFacadeForLabel).SetText(presentationInfo.facadeFor);
    ((Component) this.selectionRarityDetailsLabel).gameObject.SetActive(!string.IsNullOrWhiteSpace(presentationInfo.rarityDetails));
    ((TMP_Text) this.selectionRarityDetailsLabel).SetText(presentationInfo.rarityDetails);
    ((Component) this.selectionOwnedCount).gameObject.SetActive(!presentationInfo.isNone);
    if (presentationInfo.ownedCount.HasValue)
    {
      if (presentationInfo.ownedCount.Value > 0)
        ((TMP_Text) this.selectionOwnedCount).SetText(STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT.Replace("{OwnedCount}", presentationInfo.ownedCount.Value.ToString()));
      else
        ((TMP_Text) this.selectionOwnedCount).SetText(KleiItemsUI.WrapWithColor((string) STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED));
    }
    else
      ((TMP_Text) this.selectionOwnedCount).SetText((string) STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_UNLOCKED_BUT_UNOWNABLE);
  }
}
