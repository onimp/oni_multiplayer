// Decompiled with JetBrains decompiler
// Type: OutfitDesignerScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OutfitDesignerScreen : KMonoBehaviour
{
  [Header("CategoryColumn")]
  [SerializeField]
  private RectTransform categoryListContent;
  [SerializeField]
  private GameObject categoryRowPrefab;
  private UIPrefabLocalPool categoryRowPool;
  [Header("ItemGalleryColumn")]
  [SerializeField]
  private LocText galleryHeaderLabel;
  [SerializeField]
  private RectTransform galleryGridContent;
  [SerializeField]
  private GameObject gridItemPrefab;
  private UIPrefabLocalPool galleryGridItemPool;
  private GridLayouter galleryGridLayouter;
  [Header("SelectionDetailsColumn")]
  [SerializeField]
  private LocText selectionHeaderLabel;
  [SerializeField]
  private UIMinionOrMannequin minionOrMannequin;
  [SerializeField]
  private KButton primaryButton;
  [SerializeField]
  private KButton secondaryButton;
  [SerializeField]
  private OutfitDescriptionPanel outfitDescriptionPanel;
  [SerializeField]
  private KInputTextField inputFieldPrefab;
  public static Dictionary<ClothingOutfitUtility.OutfitType, PermitCategory[]> outfitTypeToCategoriesDict;
  private bool postponeConfiguration = true;
  private System.Action updateSaveButtonsFn;
  private System.Action RefreshCategoriesFn;
  private System.Action RefreshGalleryFn;
  private Func<bool> preventScreenPopFn;

  public OutfitDesignerScreenConfig Config { get; private set; }

  public Option<string> SelectedPermitID { get; private set; }

  public PermitCategory SelectedCategory { get; private set; }

  public OutfitDesignerScreen_OutfitState outfitState { get; private set; }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Debug.Assert(Object.op_Equality((Object) this.categoryRowPrefab.transform.parent, (Object) ((Component) this.categoryListContent).transform));
    Debug.Assert(Object.op_Equality((Object) this.gridItemPrefab.transform.parent, (Object) ((Component) this.galleryGridContent).transform));
    this.categoryRowPrefab.SetActive(false);
    this.gridItemPrefab.SetActive(false);
    this.galleryGridLayouter = new GridLayouter()
    {
      minCellSize = 64f,
      maxCellSize = 96f,
      targetGridLayout = ((Component) this.galleryGridContent).GetComponent<GridLayoutGroup>()
    };
    this.categoryRowPool = new UIPrefabLocalPool(this.categoryRowPrefab, ((Component) this.categoryListContent).gameObject);
    this.galleryGridItemPool = new UIPrefabLocalPool(this.gridItemPrefab, ((Component) this.galleryGridContent).gameObject);
    if (OutfitDesignerScreen.outfitTypeToCategoriesDict != null)
      return;
    OutfitDesignerScreen.outfitTypeToCategoriesDict = new Dictionary<ClothingOutfitUtility.OutfitType, PermitCategory[]>()
    {
      [ClothingOutfitUtility.OutfitType.Clothing] = new PermitCategory[4]
      {
        PermitCategory.DupeTops,
        PermitCategory.DupeGloves,
        PermitCategory.DupeBottoms,
        PermitCategory.DupeShoes
      }
    };
  }

  private void Update() => this.galleryGridLayouter.CheckIfShouldResizeGrid();

  protected virtual void OnSpawn()
  {
    this.postponeConfiguration = false;
    this.minionOrMannequin.TrySpawn();
    if (this.Config.isValid)
      this.Configure(this.Config);
    else
      this.Configure(OutfitDesignerScreenConfig.Mannequin((Option<ClothingOutfitTarget>) ClothingOutfitTarget.ForNewOutfit()));
  }

  protected virtual void OnCmpEnable()
  {
    base.OnCmpEnable();
    KleiItemsStatusRefresher.AddOrGetListener((Component) this).OnRefreshUI((System.Action) (() =>
    {
      this.RefreshCategories();
      this.RefreshGallery();
      this.RefreshOutfitState();
    }));
    KleiItemsStatusRefresher.RequestRefreshFromServer();
  }

  protected virtual void OnCmpDisable()
  {
    base.OnCmpDisable();
    this.UnregisterPreventScreenPop();
  }

  private void UpdateSaveButtons()
  {
    if (this.updateSaveButtonsFn == null)
      return;
    this.updateSaveButtonsFn();
  }

  public void Configure(OutfitDesignerScreenConfig config)
  {
    this.Config = config;
    this.outfitState = !config.targetMinionInstance.HasValue ? OutfitDesignerScreen_OutfitState.ForTemplateOutfit(this.Config.sourceTarget) : OutfitDesignerScreen_OutfitState.ForMinionInstance(this.Config.sourceTarget, config.targetMinionInstance.Value);
    if (this.postponeConfiguration)
      return;
    this.RegisterPreventScreenPop();
    Accessorizer component = this.minionOrMannequin.SetFrom(config.minionPersonality).SpawnedAvatar.GetComponent<Accessorizer>();
    using (ListPool<ClothingItemResource, OutfitDesignerScreen>.PooledList pooledList = PoolsFor<OutfitDesignerScreen>.AllocateList<ClothingItemResource>())
    {
      this.outfitState.AddItemValuesTo((ICollection<ClothingItemResource>) pooledList);
      component.ApplyClothingItems((IEnumerable<ClothingItemResource>) pooledList);
    }
    this.PopulateCategories();
    this.SelectCategory(OutfitDesignerScreen.outfitTypeToCategoriesDict[this.outfitState.outfitType][0]);
    this.galleryGridLayouter.RequestGridResize();
    this.RefreshOutfitState();
    if (this.Config.targetMinionInstance.HasValue)
    {
      this.updateSaveButtonsFn = (System.Action) null;
      this.primaryButton.ClearOnClick();
      ((TMP_Text) ((Component) this.primaryButton).GetComponentInChildren<LocText>()).SetText(STRINGS.UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.BUTTON_APPLY_TO_MINION.Replace("{MinionName}", this.Config.targetMinionInstance.Value.GetProperName()));
      this.primaryButton.onClick += (System.Action) (() =>
      {
        ClothingOutfitTarget clothingOutfitTarget = ClothingOutfitTarget.FromMinion(this.Config.targetMinionInstance.Value);
        clothingOutfitTarget.WriteItems(this.outfitState.GetItems());
        if (this.Config.onWriteToOutfitTargetFn != null)
          this.Config.onWriteToOutfitTargetFn(clothingOutfitTarget);
        LockerNavigator.Instance.PopScreen();
      });
      this.secondaryButton.ClearOnClick();
      ((TMP_Text) ((Component) this.secondaryButton).GetComponentInChildren<LocText>()).SetText((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.BUTTON_APPLY_TO_TEMPLATE);
      this.secondaryButton.onClick += (System.Action) (() => OutfitDesignerScreen.MakeApplyToTemplatePopup(this.inputFieldPrefab, this.outfitState, this.Config.targetMinionInstance.Value, this.Config.outfitTemplate, this.Config.onWriteToOutfitTargetFn));
      this.updateSaveButtonsFn += (System.Action) (() =>
      {
        if (this.outfitState.DoesContainNonOwnedItems())
        {
          this.primaryButton.isInteractable = false;
          ((Component) this.primaryButton).gameObject.AddOrGet<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.TOOLTIP_SAVE_ERROR_LOCKED);
          this.secondaryButton.isInteractable = false;
          ((Component) this.secondaryButton).gameObject.AddOrGet<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.TOOLTIP_SAVE_ERROR_LOCKED);
        }
        else
        {
          this.primaryButton.isInteractable = true;
          ((Component) this.primaryButton).gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
          this.secondaryButton.isInteractable = true;
          ((Component) this.secondaryButton).gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
        }
      });
    }
    else
    {
      if (!this.Config.outfitTemplate.HasValue)
        throw new NotSupportedException();
      this.updateSaveButtonsFn = (System.Action) null;
      this.primaryButton.ClearOnClick();
      ((TMP_Text) ((Component) this.primaryButton).GetComponentInChildren<LocText>()).SetText((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.BUTTON_SAVE);
      this.primaryButton.onClick += (System.Action) (() =>
      {
        this.outfitState.destinationTarget.WriteName(this.outfitState.name);
        this.outfitState.destinationTarget.WriteItems(this.outfitState.GetItems());
        if (this.Config.minionPersonality.HasValue)
          Db.Get().Permits.ClothingOutfits.SetDuplicantPersonalityOutfit(this.Config.minionPersonality.Value.Id, (Option<string>) this.outfitState.destinationTarget.Id);
        if (this.Config.onWriteToOutfitTargetFn != null)
          this.Config.onWriteToOutfitTargetFn(this.outfitState.destinationTarget);
        LockerNavigator.Instance.PopScreen();
      });
      this.secondaryButton.ClearOnClick();
      ((TMP_Text) ((Component) this.secondaryButton).GetComponentInChildren<LocText>()).SetText((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.BUTTON_COPY);
      this.secondaryButton.onClick += (System.Action) (() => OutfitDesignerScreen.MakeCopyPopup(this, this.inputFieldPrefab, this.outfitState, this.Config.outfitTemplate.Value, this.Config.minionPersonality, this.Config.onWriteToOutfitTargetFn));
      this.updateSaveButtonsFn += (System.Action) (() =>
      {
        if (!this.outfitState.destinationTarget.CanWriteItems)
        {
          this.primaryButton.isInteractable = false;
          ((Component) this.primaryButton).gameObject.AddOrGet<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.TOOLTIP_SAVE_ERROR_READONLY);
          this.secondaryButton.isInteractable = true;
          ((Component) this.secondaryButton).gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
        }
        else if (this.outfitState.DoesContainNonOwnedItems())
        {
          this.primaryButton.isInteractable = false;
          ((Component) this.primaryButton).gameObject.AddOrGet<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.TOOLTIP_SAVE_ERROR_LOCKED);
          this.secondaryButton.isInteractable = false;
          ((Component) this.secondaryButton).gameObject.AddOrGet<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.TOOLTIP_SAVE_ERROR_LOCKED);
        }
        else
        {
          this.primaryButton.isInteractable = true;
          ((Component) this.primaryButton).gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
          this.secondaryButton.isInteractable = true;
          ((Component) this.secondaryButton).gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
        }
      });
    }
    this.UpdateSaveButtons();
  }

  private void RefreshOutfitState()
  {
    ((TMP_Text) this.selectionHeaderLabel).text = this.outfitState.name;
    this.outfitDescriptionPanel.Refresh(this.outfitState);
    this.UpdateSaveButtons();
  }

  private void RefreshCategories()
  {
    if (this.RefreshCategoriesFn == null)
      return;
    this.RefreshCategoriesFn();
  }

  public void PopulateCategories()
  {
    this.RefreshCategoriesFn = (System.Action) null;
    this.categoryRowPool.ReturnAll();
    foreach (PermitCategory permitCategory1 in OutfitDesignerScreen.outfitTypeToCategoriesDict[this.outfitState.outfitType])
    {
      PermitCategory permitCategory = permitCategory1;
      GameObject gameObject = this.categoryRowPool.Borrow();
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      ((TMP_Text) component.GetReference<LocText>("Label")).SetText(PermitCategories.GetUppercaseDisplayName(permitCategory));
      component.GetReference<Image>("Icon").sprite = Assets.GetSprite(HashedString.op_Implicit(PermitCategories.GetIconName(permitCategory)));
      MultiToggle toggle = gameObject.GetComponent<MultiToggle>();
      toggle.onClick = (System.Action) (() => this.SelectCategory(permitCategory));
      this.RefreshCategoriesFn += (System.Action) (() => toggle.ChangeState(permitCategory == this.SelectedCategory ? 1 : 0));
    }
  }

  public void SelectCategory(PermitCategory permitCategory)
  {
    this.SelectedCategory = permitCategory;
    ((TMP_Text) this.galleryHeaderLabel).text = PermitCategories.GetDisplayName(permitCategory);
    this.RefreshCategories();
    this.PopulateGallery();
    ref Option<ClothingItemResource> local = ref this.outfitState.GetItemSlotForCategory(permitCategory);
    if (local.HasValue)
      this.SelectPermit((Option<string>) local.Value.Id);
    else
      this.SelectPermit((Option<string>) Option.None);
  }

  private void RefreshGallery()
  {
    if (this.RefreshGalleryFn == null)
      return;
    this.RefreshGalleryFn();
  }

  public void PopulateGallery()
  {
    this.RefreshGalleryFn = (System.Action) null;
    this.galleryGridItemPool.ReturnAll();
    AddGridIconForPermitId((Option<string>) Option.None);
    foreach (PermitResource resource in Db.Get().Permits.resources)
    {
      if (PermitResources.ShouldDisplayPermitInSupplyCloset(resource.Id) && resource.PermitCategory == this.SelectedCategory)
        AddGridIconForPermitId((Option<string>) resource.Id);
    }
    this.RefreshGallery();

    void AddGridIconForPermitId(Option<string> permitId)
    {
      GameObject gameObject = this.galleryGridItemPool.Borrow();
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      Image reference = component.GetReference<Image>("Icon");
      MultiToggle toggle = gameObject.GetComponent<MultiToggle>();
      Image isUnownedOverlay = component.GetReference<Image>("IsUnownedOverlay");
      if (!permitId.HasValue)
      {
        reference.sprite = KleiItemsUI.GetNoneClothingItemIcon(this.SelectedCategory);
        KleiItemsUI.ConfigureTooltipOn(gameObject, (Option<string>) KleiItemsUI.GetNoneClothingItemString(this.SelectedCategory));
        ((Component) isUnownedOverlay).gameObject.SetActive(false);
      }
      else
      {
        PermitPresentationInfo permitPresInfo = Db.Get().Permits.Get((string) permitId).GetPermitPresentationInfo();
        reference.sprite = permitPresInfo.sprite;
        KleiItemsUI.ConfigureTooltipOn(gameObject, (Option<string>) KleiItemsUI.GetTooltipStringFor(permitPresInfo));
        this.RefreshGalleryFn += (System.Action) (() => ((Component) isUnownedOverlay).gameObject.SetActive(!permitPresInfo.IsUnlocked()));
      }
      toggle.onClick = (System.Action) (() => this.SelectPermit(permitId));
      this.RefreshGalleryFn += (System.Action) (() => toggle.ChangeState(permitId == this.SelectedPermitID ? 1 : 0));
    }
  }

  public void SelectPermit(Option<string> permitId)
  {
    this.SelectedPermitID = permitId;
    this.RefreshGallery();
    this.UpdateSelectedItemDetails();
    this.UpdateSaveButtons();
  }

  public void UpdateSelectedItemDetails()
  {
    Option<ClothingItemResource> option = (Option<ClothingItemResource>) Option.None;
    if (this.SelectedPermitID.HasValue)
    {
      PermitResource permitResource = Db.Get().Permits.Get((string) this.SelectedPermitID);
      permitResource.GetPermitPresentationInfo();
      if (permitResource is ClothingItemResource clothingItemResource)
        option = (Option<ClothingItemResource>) clothingItemResource;
    }
    this.outfitState.GetItemSlotForCategory(this.SelectedCategory) = option;
    this.minionOrMannequin.current.SetOutfit(this.outfitState);
    this.minionOrMannequin.current.ReactToClothingItemChange(this.SelectedCategory);
    this.outfitDescriptionPanel.Refresh(this.outfitState);
  }

  private void RegisterPreventScreenPop()
  {
    this.UnregisterPreventScreenPop();
    this.preventScreenPopFn = (Func<bool>) (() =>
    {
      if (!this.outfitState.IsDirty())
        return false;
      this.RegisterPreventScreenPop();
      OutfitDesignerScreen.MakeSaveWarningPopup(this.outfitState, (System.Action) (() =>
      {
        this.UnregisterPreventScreenPop();
        LockerNavigator.Instance.PopScreen();
      }));
      return true;
    });
    LockerNavigator.Instance.preventScreenPop.Add(this.preventScreenPopFn);
  }

  private void UnregisterPreventScreenPop()
  {
    if (this.preventScreenPopFn == null)
      return;
    LockerNavigator.Instance.preventScreenPop.Remove(this.preventScreenPopFn);
    this.preventScreenPopFn = (Func<bool>) null;
  }

  public static void MakeSaveWarningPopup(
    OutfitDesignerScreen_OutfitState outfitState,
    System.Action discardChangesFn)
  {
    LockerNavigator.Instance.ShowDialogPopup((Action<InfoDialogScreen>) (dialog => dialog.SetHeader(STRINGS.UI.OUTFIT_DESIGNER_SCREEN.CHANGES_NOT_SAVED_WARNING_POPUP.HEADER.Replace("{OutfitName}", outfitState.name)).AddPlainText((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.CHANGES_NOT_SAVED_WARNING_POPUP.BODY).AddOption((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.CHANGES_NOT_SAVED_WARNING_POPUP.BUTTON_DISCARD, (Action<InfoDialogScreen>) (d =>
    {
      d.Deactivate();
      discardChangesFn();
    }), true).AddOption((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.CHANGES_NOT_SAVED_WARNING_POPUP.BUTTON_RETURN, (Action<InfoDialogScreen>) (d => d.Deactivate()))));
  }

  public static void MakeApplyToTemplatePopup(
    KInputTextField inputFieldPrefab,
    OutfitDesignerScreen_OutfitState outfitState,
    GameObject targetMinionInstance,
    Option<ClothingOutfitTarget> existingOutfitTemplate,
    Action<ClothingOutfitTarget> onWriteToOutfitTargetFn)
  {
    ClothingOutfitNameProposal proposal = new ClothingOutfitNameProposal();
    Color errorTextColor = Util.ColorFromHex("F44A47");
    Color defaultTextColor = Util.ColorFromHex("FFFFFF");
    KInputTextField inputField;
    InfoScreenPlainText descLabel;
    KButton saveButton;
    LocText saveButtonText;
    LocText descLocText;
    LockerNavigator.Instance.ShowDialogPopup((Action<InfoDialogScreen>) (dialog =>
    {
      dialog.SetHeader(STRINGS.UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.APPLY_TEMPLATE_POPUP.HEADER.Replace("{OutfitName}", outfitState.name)).AddUI<KInputTextField>(inputFieldPrefab, out inputField).AddSpacer(8f).AddUI<InfoScreenPlainText>(dialog.GetPlainTextPrefab(), out descLabel).AddOption(true, out saveButton, out saveButtonText).AddDefaultCancel();
      descLocText = ((Component) descLabel).gameObject.GetComponent<LocText>();
      descLocText.allowOverride = true;
      ((TMP_Text) descLocText).alignment = (TextAlignmentOptions) 1025;
      ((Graphic) descLocText).color = errorTextColor;
      ((TMP_Text) descLocText).fontSize = 14f;
      descLabel.SetText("");
      // ISSUE: method pointer
      ((UnityEvent<string>) ((TMP_InputField) inputField).onValueChanged).AddListener(new UnityAction<string>((object) this, __methodptr(\u003CMakeApplyToTemplatePopup\u003Eg__Refresh\u007C1)));
      saveButton.onClick += (System.Action) (() =>
      {
        ClothingOutfitTarget clothingOutfitTarget;
        switch (proposal.result)
        {
          case ClothingOutfitNameProposal.Result.NewOutfit:
            clothingOutfitTarget = ClothingOutfitTarget.ForNewOutfit(proposal.candidateName);
            break;
          case ClothingOutfitNameProposal.Result.SameOutfit:
            clothingOutfitTarget = existingOutfitTemplate.Value;
            break;
          default:
            throw new NotSupportedException(string.Format("Can't save outfit with name \"{0}\", failed with result: {1}", (object) proposal.candidateName, (object) proposal.result));
        }
        clothingOutfitTarget.WriteItems(outfitState.GetItems());
        ClothingOutfitTarget.FromMinion(targetMinionInstance).WriteItems(outfitState.GetItems());
        if (onWriteToOutfitTargetFn != null)
          onWriteToOutfitTargetFn(clothingOutfitTarget);
        dialog.Deactivate();
        LockerNavigator.Instance.PopScreen();
      });
      if (existingOutfitTemplate.HasValue)
      {
        ClothingOutfitTarget clothingOutfitTarget = existingOutfitTemplate.Value;
        if (clothingOutfitTarget.CanWriteName)
        {
          clothingOutfitTarget = existingOutfitTemplate.Value;
          if (clothingOutfitTarget.CanWriteItems)
          {
            clothingOutfitTarget = existingOutfitTemplate.Value;
            Refresh(clothingOutfitTarget.Id);
            return;
          }
        }
        clothingOutfitTarget = ClothingOutfitTarget.ForCopyOf(existingOutfitTemplate.Value);
        Refresh(clothingOutfitTarget.Id);
      }
      else
        Refresh(outfitState.name);
    }));

    void Refresh(string candidateName)
    {
      proposal = !existingOutfitTemplate.HasValue ? ClothingOutfitNameProposal.ForNewOutfit(candidateName) : ClothingOutfitNameProposal.FromExistingOutfit(candidateName, (ClothingOutfitTarget) existingOutfitTemplate, true);
      ((TMP_InputField) inputField).text = candidateName;
      switch (proposal.result)
      {
        case ClothingOutfitNameProposal.Result.NewOutfit:
          ((Component) descLabel).gameObject.SetActive(true);
          descLabel.SetText(STRINGS.UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.APPLY_TEMPLATE_POPUP.DESC_SAVE_NEW.Replace("{OutfitName}", candidateName).Replace("{MinionName}", targetMinionInstance.GetProperName()));
          ((Graphic) descLocText).color = defaultTextColor;
          ((TMP_Text) saveButtonText).text = (string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.APPLY_TEMPLATE_POPUP.BUTTON_SAVE_NEW;
          saveButton.isInteractable = true;
          break;
        case ClothingOutfitNameProposal.Result.SameOutfit:
          ((Component) descLabel).gameObject.SetActive(true);
          descLabel.SetText(STRINGS.UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.APPLY_TEMPLATE_POPUP.DESC_SAVE_EXISTING.Replace("{OutfitName}", candidateName).Replace("{MinionName}", targetMinionInstance.GetProperName()));
          ((Graphic) descLocText).color = defaultTextColor;
          ((TMP_Text) saveButtonText).text = (string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.APPLY_TEMPLATE_POPUP.BUTTON_SAVE_EXISTING;
          saveButton.isInteractable = true;
          break;
        case ClothingOutfitNameProposal.Result.Error_NoInputName:
          ((Component) descLabel).gameObject.SetActive(false);
          ((TMP_Text) saveButtonText).text = (string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.APPLY_TEMPLATE_POPUP.BUTTON_SAVE_NEW;
          saveButton.isInteractable = false;
          break;
        case ClothingOutfitNameProposal.Result.Error_NameAlreadyExists:
        case ClothingOutfitNameProposal.Result.Error_SameOutfitReadonly:
          ((Component) descLabel).gameObject.SetActive(true);
          descLabel.SetText(STRINGS.UI.OUTFIT_NAME.ERROR_NAME_EXISTS.Replace("{OutfitName}", candidateName));
          ((Graphic) descLocText).color = errorTextColor;
          ((TMP_Text) saveButtonText).text = (string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.APPLY_TEMPLATE_POPUP.BUTTON_SAVE_NEW;
          saveButton.isInteractable = false;
          break;
        default:
          DebugUtil.DevAssert(false, string.Format("Unhandled name proposal case: {0}", (object) proposal.result), (Object) null);
          break;
      }
    }
  }

  public static void MakeCopyPopup(
    OutfitDesignerScreen screen,
    KInputTextField inputFieldPrefab,
    OutfitDesignerScreen_OutfitState outfitState,
    ClothingOutfitTarget outfitTemplate,
    Option<Personality> minionPersonality,
    Action<ClothingOutfitTarget> onWriteToOutfitTargetFn)
  {
    ClothingOutfitNameProposal proposal = new ClothingOutfitNameProposal();
    KInputTextField inputField;
    InfoScreenPlainText errorText;
    KButton okButton;
    LocText okButtonText;
    LockerNavigator.Instance.ShowDialogPopup((Action<InfoDialogScreen>) (dialog =>
    {
      dialog.SetHeader((string) STRINGS.UI.OUTFIT_DESIGNER_SCREEN.COPY_POPUP.HEADER).AddUI<KInputTextField>(inputFieldPrefab, out inputField).AddSpacer(8f).AddUI<InfoScreenPlainText>(dialog.GetPlainTextPrefab(), out errorText).AddOption(true, out okButton, out okButtonText).AddOption((string) STRINGS.UI.CONFIRMDIALOG.CANCEL, (Action<InfoDialogScreen>) (d => d.Deactivate()));
      // ISSUE: method pointer
      ((UnityEvent<string>) ((TMP_InputField) inputField).onValueChanged).AddListener(new UnityAction<string>((object) this, __methodptr(\u003CMakeCopyPopup\u003Eg__Refresh\u007C1)));
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
        ClothingOutfitTarget sourceTarget = proposal.result == ClothingOutfitNameProposal.Result.NewOutfit ? ClothingOutfitTarget.ForNewOutfit(proposal.candidateName) : throw new NotSupportedException(string.Format("Can't save outfit with name \"{0}\", failed with result: {1}", (object) proposal.candidateName, (object) proposal.result));
        sourceTarget.WriteItems(outfitState.GetItems());
        if (minionPersonality.HasValue)
          Db.Get().Permits.ClothingOutfits.SetDuplicantPersonalityOutfit(minionPersonality.Value.Id, (Option<string>) sourceTarget.Id);
        if (onWriteToOutfitTargetFn != null)
          onWriteToOutfitTargetFn(sourceTarget);
        dialog.Deactivate();
        screen.Configure(screen.Config.WithOutfit((Option<ClothingOutfitTarget>) sourceTarget));
      });
      Refresh(ClothingOutfitTarget.ForCopyOf(outfitTemplate).Id);
    }));

    void Refresh(string candidateName)
    {
      proposal = ClothingOutfitNameProposal.FromExistingOutfit(candidateName, outfitTemplate, false);
      ((TMP_InputField) inputField).text = candidateName;
      switch (proposal.result)
      {
        case ClothingOutfitNameProposal.Result.NewOutfit:
          ((Component) errorText).gameObject.SetActive(false);
          okButton.isInteractable = true;
          break;
        case ClothingOutfitNameProposal.Result.SameOutfit:
        case ClothingOutfitNameProposal.Result.Error_NameAlreadyExists:
        case ClothingOutfitNameProposal.Result.Error_SameOutfitReadonly:
          ((Component) errorText).gameObject.SetActive(true);
          errorText.SetText(STRINGS.UI.OUTFIT_NAME.ERROR_NAME_EXISTS.Replace("{OutfitName}", candidateName));
          okButton.isInteractable = false;
          break;
        case ClothingOutfitNameProposal.Result.Error_NoInputName:
          ((Component) errorText).gameObject.SetActive(false);
          okButton.isInteractable = false;
          break;
        default:
          DebugUtil.DevAssert(false, string.Format("Unhandled name proposal case: {0}", (object) proposal.result), (Object) null);
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
