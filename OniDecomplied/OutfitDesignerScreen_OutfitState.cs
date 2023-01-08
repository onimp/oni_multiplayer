// Decompiled with JetBrains decompiler
// Type: OutfitDesignerScreen_OutfitState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using UnityEngine;

public class OutfitDesignerScreen_OutfitState
{
  public string name;
  public Option<ClothingItemResource> hatSlot;
  public Option<ClothingItemResource> topSlot;
  public Option<ClothingItemResource> glovesSlot;
  public Option<ClothingItemResource> bottomSlot;
  public Option<ClothingItemResource> shoesSlot;
  public Option<ClothingItemResource> accessorySlot;
  public ClothingOutfitUtility.OutfitType outfitType;
  public ClothingOutfitTarget sourceTarget;
  public ClothingOutfitTarget destinationTarget;
  private static Option<ClothingItemResource> dummySlot;

  private OutfitDesignerScreen_OutfitState(
    ClothingOutfitTarget sourceTarget,
    ClothingOutfitTarget destinationTarget)
  {
    this.destinationTarget = destinationTarget;
    this.sourceTarget = sourceTarget;
    this.name = sourceTarget.ReadName();
    foreach (ClothingItemResource readItemValue in sourceTarget.ReadItemValues())
      this.ApplyItem(readItemValue);
  }

  public static OutfitDesignerScreen_OutfitState ForTemplateOutfit(
    ClothingOutfitTarget outfitTemplate)
  {
    Debug.Assert(outfitTemplate.IsTemplateOutfit());
    return new OutfitDesignerScreen_OutfitState(outfitTemplate, outfitTemplate);
  }

  public static OutfitDesignerScreen_OutfitState ForMinionInstance(
    ClothingOutfitTarget sourceTarget,
    GameObject minionInstance)
  {
    return new OutfitDesignerScreen_OutfitState(sourceTarget, ClothingOutfitTarget.FromMinion(minionInstance));
  }

  public void ApplyItem(ClothingItemResource item) => this.GetItemSlotForCategory(item.Category) = (Option<ClothingItemResource>) item;

  public ref Option<ClothingItemResource> GetItemSlotForCategory(PermitCategory category)
  {
    switch (category)
    {
      case PermitCategory.DupeTops:
        return ref this.topSlot;
      case PermitCategory.DupeBottoms:
        return ref this.bottomSlot;
      case PermitCategory.DupeGloves:
        return ref this.glovesSlot;
      case PermitCategory.DupeShoes:
        return ref this.shoesSlot;
      case PermitCategory.DupeHats:
        return ref this.hatSlot;
      case PermitCategory.DupeAccessories:
        return ref this.accessorySlot;
      default:
        DebugUtil.DevAssert(false, string.Format("Couldn't get a {0}<{1}> for {2} \"{3}\" on {4} \"{5}\".", (object) "Option", (object) "ClothingItemResource", (object) "PermitCategory", (object) category, (object) nameof (OutfitDesignerScreen_OutfitState), (object) this.name), (Object) null);
        return ref OutfitDesignerScreen_OutfitState.dummySlot;
    }
  }

  public void AddItemValuesTo(ICollection<ClothingItemResource> clothingItems)
  {
    if (this.hatSlot.HasValue)
      clothingItems.Add((ClothingItemResource) this.hatSlot);
    if (this.topSlot.HasValue)
      clothingItems.Add((ClothingItemResource) this.topSlot);
    if (this.glovesSlot.HasValue)
      clothingItems.Add((ClothingItemResource) this.glovesSlot);
    if (this.bottomSlot.HasValue)
      clothingItems.Add((ClothingItemResource) this.bottomSlot);
    if (this.shoesSlot.HasValue)
      clothingItems.Add((ClothingItemResource) this.shoesSlot);
    if (!this.accessorySlot.HasValue)
      return;
    clothingItems.Add((ClothingItemResource) this.accessorySlot);
  }

  public void AddItemsTo(ICollection<string> itemIds)
  {
    if (this.hatSlot.HasValue)
      itemIds.Add(this.hatSlot.Value.Id);
    if (this.topSlot.HasValue)
      itemIds.Add(this.topSlot.Value.Id);
    if (this.glovesSlot.HasValue)
      itemIds.Add(this.glovesSlot.Value.Id);
    if (this.bottomSlot.HasValue)
      itemIds.Add(this.bottomSlot.Value.Id);
    if (this.shoesSlot.HasValue)
      itemIds.Add(this.shoesSlot.Value.Id);
    if (!this.accessorySlot.HasValue)
      return;
    itemIds.Add(this.accessorySlot.Value.Id);
  }

  public string[] GetItems()
  {
    List<string> itemIds = new List<string>();
    this.AddItemsTo((ICollection<string>) itemIds);
    return itemIds.ToArray();
  }

  public bool DoesContainNonOwnedItems()
  {
    using (ListPool<string, OutfitDesignerScreen_OutfitState>.PooledList itemIds = PoolsFor<OutfitDesignerScreen_OutfitState>.AllocateList<string>())
    {
      this.AddItemsTo((ICollection<string>) itemIds);
      return ClothingOutfitTarget.DoesContainNonOwnedItems((IList<string>) itemIds);
    }
  }

  public bool IsDirty()
  {
    using (HashSetPool<string, OutfitDesignerScreen>.PooledHashSet itemIds = PoolsFor<OutfitDesignerScreen>.AllocateHashSet<string>())
    {
      this.AddItemsTo((ICollection<string>) itemIds);
      string[] strArray = this.destinationTarget.ReadItems();
      if (((HashSet<string>) itemIds).Count != strArray.Length)
        return true;
      foreach (string str in strArray)
      {
        if (!((HashSet<string>) itemIds).Contains(str))
          return true;
      }
    }
    return false;
  }
}
