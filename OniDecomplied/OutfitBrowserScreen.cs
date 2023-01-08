// Decompiled with JetBrains decompiler
// Type: OutfitBrowserScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OutfitBrowserScreen : KMonoBehaviour
{
  [Header("ItemGalleryColumn")]
  [SerializeField]
  private LocText galleryHeaderLabel;
  [SerializeField]
  private RectTransform galleryGridContent;
  [SerializeField]
  private GameObject gridItemPrefab;
  [SerializeField]
  private GameObject addButtonGridItem;
  private UIPrefabLocalPool galleryGridItemPool;
  private GridLayouter gridLayouter;
  [Header("SelectionDetailsColumn")]
  [SerializeField]
  private LocText selectionHeaderLabel;
  [SerializeField]
  private UIMinionOrMannequin dioramaMinionOrMannequin;
  [SerializeField]
  private OutfitDescriptionPanel outfitDescriptionPanel;
  [SerializeField]
  private KButton pickOutfitButton;
  [SerializeField]
  private KButton editOutfitButton;
  [SerializeField]
  private KButton renameOutfitButton;
  [SerializeField]
  private KButton deleteOutfitButton;
  [SerializeField]
  private KInputTextField inputFieldPrefab;
  private Option<ClothingOutfitTarget> lastMannequinSelectedOutfit;
  private Option<ClothingOutfitTarget> selectedOutfit;
  private Dictionary<string, MultiToggle> outfits = new Dictionary<string, MultiToggle>();
  private bool postponeConfiguration = true;
  private bool isFirstDisplay = true;
  private System.Action RefreshGalleryFn;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.galleryGridItemPool = new UIPrefabLocalPool(this.gridItemPrefab, ((Component) this.galleryGridContent).gameObject);
    this.gridLayouter = new GridLayouter()
    {
      minCellSize = 112f,
      maxCellSize = 144f,
      targetGridLayout = ((Component) this.galleryGridContent).GetComponent<GridLayoutGroup>()
    };
    this.pickOutfitButton.onClick += new System.Action(this.OnClickPickOutfit);
    this.editOutfitButton.onClick += (System.Action) (() => new OutfitDesignerScreenConfig(this.selectedOutfit, this.Config.minionPersonality, this.Config.targetMinionInstance, new Action<ClothingOutfitTarget>(this.OnOutfitDesignerWritesToOutfitTarget)).ApplyAndOpenScreen());
    this.renameOutfitButton.onClick += (System.Action) (() => OutfitBrowserScreen.MakeRenamePopup(this.inputFieldPrefab, this.selectedOutfit.Value, (Func<string>) (() => this.selectedOutfit.Value.ReadName()), (Action<string>) (new_name =>
    {
      this.selectedOutfit.Value.WriteName(new_name);
      if (!this.Config.minionPersonality.HasValue)
        this.lastMannequinSelectedOutfit = this.selectedOutfit;
      this.Configure(this.Config.WithOutfit(this.selectedOutfit));
    })));
    this.deleteOutfitButton.onClick += (System.Action) (() => OutfitBrowserScreen.MakeDeletePopup(this.selectedOutfit.Value, (System.Action) (() =>
    {
      this.selectedOutfit.Value.Delete();
      this.Configure(this.Config.WithOutfit((Option<ClothingOutfitTarget>) Option.None));
    })));
  }

  public OutfitBrowserScreenConfig Config { get; private set; }

  protected virtual void OnCmpEnable()
  {
    if (this.isFirstDisplay)
    {
      this.isFirstDisplay = false;
      this.dioramaMinionOrMannequin.TrySpawn();
      this.postponeConfiguration = false;
      this.Configure(this.Config);
    }
    this.PopulateGallery();
    this.SelectOutfit(this.selectedOutfit, true);
    KleiItemsStatusRefresher.AddOrGetListener((Component) this).OnRefreshUI((System.Action) (() =>
    {
      this.RefreshGallery();
      this.outfitDescriptionPanel.Refresh(this.selectedOutfit);
    }));
    KleiItemsStatusRefresher.RequestRefreshFromServer();
  }

  public void Configure(OutfitBrowserScreenConfig config)
  {
    this.Config = config;
    if (this.postponeConfiguration)
      return;
    this.dioramaMinionOrMannequin.SetFrom(config.minionPersonality);
    if (config.targetMinionInstance.HasValue)
      ((TMP_Text) this.galleryHeaderLabel).text = STRINGS.UI.OUTFIT_BROWSER_SCREEN.COLUMN_HEADERS.MINION_GALLERY_HEADER.Replace("{MinionName}", config.targetMinionInstance.Value.GetProperName());
    else if (config.minionPersonality.HasValue)
      ((TMP_Text) this.galleryHeaderLabel).text = STRINGS.UI.OUTFIT_BROWSER_SCREEN.COLUMN_HEADERS.MINION_GALLERY_HEADER.Replace("{MinionName}", config.minionPersonality.Value.Name);
    else
      ((TMP_Text) this.galleryHeaderLabel).text = (string) STRINGS.UI.OUTFIT_BROWSER_SCREEN.COLUMN_HEADERS.GALLERY_HEADER;
    Option<ClothingOutfitTarget> outfit = config.minionPersonality.HasValue || config.selectedTarget.HasValue ? config.selectedTarget : (!this.lastMannequinSelectedOutfit.HasValue ? (Option<ClothingOutfitTarget>) ClothingOutfitTarget.GetRandom() : this.lastMannequinSelectedOutfit);
    if (outfit.HasValue && outfit.Value.DoesExist())
      this.SelectOutfit(outfit, true);
    else
      this.SelectOutfit((Option<ClothingOutfitTarget>) Option.None, true);
    ((Component) this.pickOutfitButton).gameObject.SetActive(config.isPickingOutfitForDupe);
    ((Component) this.renameOutfitButton).gameObject.SetActive(false);
    ((Component) this.deleteOutfitButton).gameObject.SetActive(false);
    if (!((Component) this).gameObject.activeInHierarchy)
      return;
    ((Component) this).gameObject.SetActive(false);
    ((Component) this).gameObject.SetActive(true);
  }

  private void RefreshGallery()
  {
    if (this.RefreshGalleryFn == null)
      return;
    this.RefreshGalleryFn();
  }

  private void PopulateGallery()
  {
    this.outfits.Clear();
    this.galleryGridItemPool.ReturnAll();
    this.RefreshGalleryFn = (System.Action) null;
    if (this.Config.isPickingOutfitForDupe)
      AddGridIconForTarget((Option<ClothingOutfitTarget>) Option.None);
    if (this.Config.targetMinionInstance.HasValue)
      AddGridIconForTarget((Option<ClothingOutfitTarget>) ClothingOutfitTarget.FromMinion(this.Config.targetMinionInstance.Value));
    foreach (ClothingOutfitTarget target in ClothingOutfitTarget.GetAll())
      AddGridIconForTarget((Option<ClothingOutfitTarget>) target);
    this.addButtonGridItem.transform.SetAsLastSibling();
    this.addButtonGridItem.SetActive(true);
    this.addButtonGridItem.GetComponent<MultiToggle>().onClick = (System.Action) (() => new OutfitDesignerScreenConfig((Option<ClothingOutfitTarget>) ClothingOutfitTarget.ForNewOutfit(), this.Config.minionPersonality, this.Config.targetMinionInstance, new Action<ClothingOutfitTarget>(this.OnOutfitDesignerWritesToOutfitTarget)).ApplyAndOpenScreen());
    this.RefreshGallery();

    void AddGridIconForTarget(Option<ClothingOutfitTarget> target)
    {
      GameObject spawn = this.galleryGridItemPool.Borrow();
      GameObject gameObject1 = ((Component) spawn.transform.GetChild(1)).gameObject;
      GameObject gameObject2 = ((Component) spawn.transform.GetChild(2)).gameObject;
      GameObject isUnownedOverlayGO = ((Component) spawn.transform.GetChild(3)).gameObject;
      gameObject1.SetActive(true);
      gameObject2.SetActive(false);
      gameObject1.GetComponentInChildren<UIMannequin>().SetOutfit(target);
      if (!target.HasValue)
      {
        gameObject2.SetActive(true);
        gameObject2.GetComponent<Image>().sprite = KleiItemsUI.GetNoneOutfitIcon();
      }
      MultiToggle button = spawn.GetComponent<MultiToggle>();
      button.onClick = (System.Action) (() => this.SelectOutfit(target));
      this.RefreshGalleryFn += (System.Action) (() =>
      {
        button.ChangeState(target == this.selectedOutfit ? 1 : 0);
        if (!target.HasValue)
        {
          KleiItemsUI.ConfigureTooltipOn(spawn, (Option<string>) KleiItemsUI.WrapAsToolTipTitle((string) STRINGS.UI.OUTFIT_NAME.NONE));
          isUnownedOverlayGO.SetActive(false);
        }
        else
        {
          KleiItemsUI.ConfigureTooltipOn(spawn, (Option<string>) KleiItemsUI.WrapAsToolTipTitle(target.Value.ReadName()));
          isUnownedOverlayGO.SetActive(target.Value.DoesContainNonOwnedItems());
        }
      });
    }
  }

  private void OnOutfitDesignerWritesToOutfitTarget(ClothingOutfitTarget outfit) => this.Configure(this.Config.WithOutfit((Option<ClothingOutfitTarget>) outfit));

  private void Update() => this.gridLayouter.CheckIfShouldResizeGrid();

  private void SelectOutfit(string id, bool isFirstOpen = false) => this.SelectOutfit((Option<ClothingOutfitTarget>) ClothingOutfitTarget.FromId(id), isFirstOpen);

  private void SelectOutfit(Option<ClothingOutfitTarget> outfit, bool isFirstOpen = false)
  {
    ((TMP_Text) this.selectionHeaderLabel).text = outfit.ReadName();
    this.selectedOutfit = outfit;
    this.dioramaMinionOrMannequin.current.SetOutfit(outfit);
    this.dioramaMinionOrMannequin.current.ReactToFullOutfitChange();
    this.outfitDescriptionPanel.Refresh(outfit);
    if (!this.Config.minionPersonality.HasValue)
      this.lastMannequinSelectedOutfit = outfit;
    if (this.Config.minionPersonality.HasValue)
    {
      this.pickOutfitButton.isInteractable = !outfit.HasValue || !outfit.Value.DoesContainNonOwnedItems();
      KleiItemsUI.ConfigureTooltipOn(((Component) this.pickOutfitButton).gameObject, this.pickOutfitButton.isInteractable ? (Option<string>) Option.None : Option.Some<string>(STRINGS.UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_PICK_OUTFIT_ERROR_LOCKED.Replace("{MinionName}", this.Config.GetMinionName())));
    }
    this.editOutfitButton.isInteractable = outfit.HasValue;
    ((Component) this.renameOutfitButton).gameObject.SetActive(true);
    this.renameOutfitButton.isInteractable = outfit.HasValue && outfit.Value.CanWriteName;
    KleiItemsUI.ConfigureTooltipOn(((Component) this.renameOutfitButton).gameObject, (Option<LocString>) (this.renameOutfitButton.isInteractable ? STRINGS.UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_RENAME_OUTFIT : STRINGS.UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_RENAME_OUTFIT_ERROR_READONLY));
    ((Component) this.deleteOutfitButton).gameObject.SetActive(true);
    this.deleteOutfitButton.isInteractable = outfit.HasValue && outfit.Value.CanDelete;
    KleiItemsUI.ConfigureTooltipOn(((Component) this.deleteOutfitButton).gameObject, (Option<LocString>) (this.deleteOutfitButton.isInteractable ? STRINGS.UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_DELETE_OUTFIT : STRINGS.UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_DELETE_OUTFIT_ERROR_READONLY));
    this.RefreshGallery();
  }

  private void OnClickPickOutfit()
  {
    if (this.Config.targetMinionInstance.HasValue)
      this.Config.targetMinionInstance.Value.GetComponent<Accessorizer>().ApplyClothingItems(this.selectedOutfit.ReadItemValues());
    else if (this.Config.minionPersonality.HasValue)
    {
      Db.Get().Permits.ClothingOutfits.SetDuplicantPersonalityOutfit(this.Config.minionPersonality.Value.Id, this.selectedOutfit.GetId());
      LockerNavigator.Instance.duplicantCatalogueScreen.GetComponent<MinionBrowserScreen>().RefreshPreview();
    }
    LockerNavigator.Instance.PopScreen();
  }

  public static void MakeDeletePopup(ClothingOutfitTarget sourceTarget, System.Action deleteFn) => LockerNavigator.Instance.ShowDialogPopup((Action<InfoDialogScreen>) (dialog => dialog.SetHeader(STRINGS.UI.OUTFIT_BROWSER_SCREEN.DELETE_WARNING_POPUP.HEADER.Replace("{OutfitName}", sourceTarget.ReadName())).AddPlainText(STRINGS.UI.OUTFIT_BROWSER_SCREEN.DELETE_WARNING_POPUP.BODY.Replace("{OutfitName}", sourceTarget.ReadName())).AddOption((string) STRINGS.UI.OUTFIT_BROWSER_SCREEN.DELETE_WARNING_POPUP.BUTTON_YES_DELETE, (Action<InfoDialogScreen>) (d =>
  {
    deleteFn();
    d.Deactivate();
  }), true).AddOption((string) STRINGS.UI.OUTFIT_BROWSER_SCREEN.DELETE_WARNING_POPUP.BUTTON_DONT_DELETE, (Action<InfoDialogScreen>) (d => d.Deactivate()))));

  public static void MakeRenamePopup(
    KInputTextField inputFieldPrefab,
    ClothingOutfitTarget sourceTarget,
    Func<string> readName,
    Action<string> writeName)
  {
    KInputTextField inputField;
    InfoScreenPlainText errorText;
    KButton okButton;
    LocText okButtonText;
    LockerNavigator.Instance.ShowDialogPopup((Action<InfoDialogScreen>) (dialog =>
    {
      dialog.SetHeader((string) STRINGS.UI.OUTFIT_BROWSER_SCREEN.RENAME_POPUP.HEADER).AddUI<KInputTextField>(inputFieldPrefab, out inputField).AddSpacer(8f).AddUI<InfoScreenPlainText>(dialog.GetPlainTextPrefab(), out errorText).AddOption(true, out okButton, out okButtonText).AddOption((string) STRINGS.UI.CONFIRMDIALOG.CANCEL, (Action<InfoDialogScreen>) (d => d.Deactivate()));
      // ISSUE: method pointer
      ((UnityEvent<string>) ((TMP_InputField) inputField).onValueChanged).AddListener(new UnityAction<string>((object) this, __methodptr(\u003CMakeRenamePopup\u003Eg__Refresh\u007C1)));
      ((Component) errorText).gameObject.SetActive(false);
      LocText component = ((Component) errorText).gameObject.GetComponent<LocText>();
      component.allowOverride = true;
      ((TMP_Text) component).alignment = (TextAlignmentOptions) 1025;
      ((Graphic) component).color = Util.ColorFromHex("F44A47");
      ((TMP_Text) component).fontSize = 14f;
      errorText.SetText("");
      ((TMP_Text) okButtonText).text = (string) STRINGS.UI.CONFIRMDIALOG.OK;
      okButton.onClick += (System.Action) (() =>
      {
        writeName(((TMP_InputField) inputField).text);
        dialog.Deactivate();
      });
      Refresh(readName());
    }));

    void Refresh(string candidateName)
    {
      ClothingOutfitNameProposal outfitNameProposal = ClothingOutfitNameProposal.FromExistingOutfit(candidateName, sourceTarget, true);
      ((TMP_InputField) inputField).text = candidateName;
      switch (outfitNameProposal.result)
      {
        case ClothingOutfitNameProposal.Result.NewOutfit:
        case ClothingOutfitNameProposal.Result.SameOutfit:
          ((Component) errorText).gameObject.SetActive(false);
          okButton.isInteractable = true;
          break;
        case ClothingOutfitNameProposal.Result.Error_NoInputName:
          ((Component) errorText).gameObject.SetActive(false);
          okButton.isInteractable = false;
          break;
        case ClothingOutfitNameProposal.Result.Error_NameAlreadyExists:
        case ClothingOutfitNameProposal.Result.Error_SameOutfitReadonly:
          ((Component) errorText).gameObject.SetActive(true);
          errorText.SetText(STRINGS.UI.OUTFIT_NAME.ERROR_NAME_EXISTS.Replace("{OutfitName}", candidateName));
          okButton.isInteractable = false;
          break;
        default:
          DebugUtil.DevAssert(false, string.Format("Unhandled name proposal case: {0}", (object) outfitNameProposal.result), (Object) null);
          break;
      }
    }
  }

  private enum MultiToggleState
  {
    Default,
    Selected,
    NonInteractable,
  }
}
