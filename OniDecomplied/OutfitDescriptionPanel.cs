// Decompiled with JetBrains decompiler
// Type: OutfitDescriptionPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutfitDescriptionPanel : KMonoBehaviour
{
  [SerializeField]
  public LocText outfitNameLabel;
  [SerializeField]
  private GameObject itemDescriptionRowPrefab;
  [SerializeField]
  private GameObject itemDescriptionContainer;
  [SerializeField]
  private LocText usesUnownedItemsLabel;
  private List<GameObject> itemDescriptionRows = new List<GameObject>();
  public static readonly string[] NO_ITEMS = new string[0];

  public void Refresh(Option<ClothingOutfitTarget> outfit)
  {
    if (outfit.HasValue)
      this.Refresh(outfit.Value.ReadName(), outfit.Value.ReadItems());
    else
      this.Refresh((string) STRINGS.UI.OUTFIT_NAME.NONE, OutfitDescriptionPanel.NO_ITEMS);
  }

  public void Refresh(OutfitDesignerScreen_OutfitState outfitState) => this.Refresh(outfitState.name, outfitState.GetItems());

  public void Refresh(string name, string[] itemIds)
  {
    ((TMP_Text) this.outfitNameLabel).SetText(name);
    this.ClearItemDescRows();
    using (DictionaryPool<PermitCategory, Option<ClothingItemResource>, OutfitDescriptionPanel>.PooledDictionary pooledDictionary = PoolsFor<OutfitDescriptionPanel>.AllocateDict<PermitCategory, Option<ClothingItemResource>>())
    {
      using (ListPool<ClothingItemResource, OutfitDescriptionPanel>.PooledList pooledList = PoolsFor<OutfitDescriptionPanel>.AllocateList<ClothingItemResource>())
      {
        ((Dictionary<PermitCategory, Option<ClothingItemResource>>) pooledDictionary).Add(PermitCategory.DupeTops, (Option<ClothingItemResource>) Option.None);
        ((Dictionary<PermitCategory, Option<ClothingItemResource>>) pooledDictionary).Add(PermitCategory.DupeGloves, (Option<ClothingItemResource>) Option.None);
        ((Dictionary<PermitCategory, Option<ClothingItemResource>>) pooledDictionary).Add(PermitCategory.DupeBottoms, (Option<ClothingItemResource>) Option.None);
        ((Dictionary<PermitCategory, Option<ClothingItemResource>>) pooledDictionary).Add(PermitCategory.DupeShoes, (Option<ClothingItemResource>) Option.None);
        foreach (string itemId in itemIds)
        {
          ClothingItemResource clothingItemResource = (ClothingItemResource) Db.Get().Permits.Get(itemId);
          Option<ClothingItemResource> option;
          if (((Dictionary<PermitCategory, Option<ClothingItemResource>>) pooledDictionary).TryGetValue(clothingItemResource.PermitCategory, out option) && !option.HasValue)
            ((Dictionary<PermitCategory, Option<ClothingItemResource>>) pooledDictionary)[clothingItemResource.PermitCategory] = (Option<ClothingItemResource>) clothingItemResource;
          else
            ((List<ClothingItemResource>) pooledList).Add(clothingItemResource);
        }
        foreach (KeyValuePair<PermitCategory, Option<ClothingItemResource>> keyValuePair in (Dictionary<PermitCategory, Option<ClothingItemResource>>) pooledDictionary)
        {
          PermitCategory permitCategory;
          Option<ClothingItemResource> option1;
          Util.Deconstruct<PermitCategory, Option<ClothingItemResource>>(keyValuePair, ref permitCategory, ref option1);
          PermitCategory category = permitCategory;
          Option<ClothingItemResource> option2 = option1;
          if (option2.HasValue)
            this.AddItemDescRow(option2.Value.GetPermitPresentationInfo());
          else
            this.AddItemDescRow(KleiItemsUI.GetNoneClothingItemIcon(category), KleiItemsUI.GetNoneClothingItemString(category));
        }
        foreach (PermitResource permitResource in (List<ClothingItemResource>) pooledList)
          this.AddItemDescRow(permitResource.GetPermitPresentationInfo());
      }
    }
    if (!ClothingOutfitTarget.DoesContainNonOwnedItems((IList<string>) itemIds))
    {
      ((Component) this.usesUnownedItemsLabel).gameObject.SetActive(false);
    }
    else
    {
      ((TMP_Text) this.usesUnownedItemsLabel).transform.SetAsLastSibling();
      ((TMP_Text) this.usesUnownedItemsLabel).SetText(KleiItemsUI.WrapWithColor((string) STRINGS.UI.OUTFIT_DESCRIPTION.CONTAINS_NON_OWNED_ITEMS, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED));
      ((Component) this.usesUnownedItemsLabel).gameObject.SetActive(true);
    }
    KleiItemsStatusRefresher.AddOrGetListener((Component) this).OnRefreshUI((System.Action) (() => this.Refresh(name, itemIds)));
  }

  private void ClearItemDescRows()
  {
    for (int index = 0; index < this.itemDescriptionRows.Count; ++index)
      Object.Destroy((Object) this.itemDescriptionRows[index]);
    this.itemDescriptionRows.Clear();
  }

  private void AddItemDescRow(PermitPresentationInfo presInfo)
  {
    Option<string> tooltip = presInfo.IsUnlocked() ? (Option<string>) Option.None : Option.Some<string>((string) STRINGS.UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE);
    this.AddItemDescRow(presInfo.sprite, presInfo.name, tooltip, presInfo.IsUnlocked() ? 1f : 0.7f);
  }

  private void AddItemDescRow(Sprite icon, string text, Option<string> tooltip = default (Option<string>), float alpha = 1f)
  {
    GameObject go = Util.KInstantiateUI(this.itemDescriptionRowPrefab, this.itemDescriptionContainer, true);
    this.itemDescriptionRows.Add(go);
    HierarchyReferences component = go.GetComponent<HierarchyReferences>();
    component.GetReference<Image>("Icon").sprite = icon;
    ((TMP_Text) component.GetReference<LocText>("Label")).SetText(text);
    go.AddOrGet<CanvasGroup>().alpha = alpha;
    go.AddOrGet<NonDrawingGraphic>();
    if (tooltip.HasValue)
      go.AddOrGet<ToolTip>().SetSimpleTooltip(tooltip.Value);
    else
      go.AddOrGet<ToolTip>().ClearMultiStringTooltip();
  }
}
