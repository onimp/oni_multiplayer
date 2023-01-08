// Decompiled with JetBrains decompiler
// Type: UIMinionOrMannequinITargetExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UIMinionOrMannequinITargetExtensions
{
  public static readonly ClothingItemResource[] EMPTY_OUTFIT = new ClothingItemResource[0];

  public static void SetOutfit(this UIMinionOrMannequin.ITarget self, ClothingOutfitResource outfit) => self.SetOutfit(((IEnumerable<string>) outfit.itemsInOutfit).Select<string, ClothingItemResource>((Func<string, ClothingItemResource>) (itemId => Db.Get().Permits.ClothingItems.Get(itemId))));

  public static void SetOutfit(
    this UIMinionOrMannequin.ITarget self,
    OutfitDesignerScreen_OutfitState outfit)
  {
    self.SetOutfit(((IEnumerable<string>) outfit.GetItems()).Select<string, ClothingItemResource>((Func<string, ClothingItemResource>) (itemId => Db.Get().Permits.ClothingItems.Get(itemId))));
  }

  public static void SetOutfit(this UIMinionOrMannequin.ITarget self, ClothingOutfitTarget outfit) => self.SetOutfit(outfit.ReadItemValues());

  public static void SetOutfit(
    this UIMinionOrMannequin.ITarget self,
    Option<ClothingOutfitTarget> outfit)
  {
    if (outfit.HasValue)
      self.SetOutfit(outfit.Value);
    else
      self.ClearOutfit();
  }

  public static void ClearOutfit(this UIMinionOrMannequin.ITarget self) => self.SetOutfit((IEnumerable<ClothingItemResource>) UIMinionOrMannequinITargetExtensions.EMPTY_OUTFIT);

  public static void React(this UIMinionOrMannequin.ITarget self) => self.React(UIMinionOrMannequinReactSource.None);

  public static void ReactToClothingItemChange(
    this UIMinionOrMannequin.ITarget self,
    PermitCategory clothingChangedCategory)
  {
    self.React(GetSource(clothingChangedCategory));

    static UIMinionOrMannequinReactSource GetSource(PermitCategory clothingChangedCategory)
    {
      switch (clothingChangedCategory)
      {
        case PermitCategory.DupeTops:
          return UIMinionOrMannequinReactSource.OnTopChanged;
        case PermitCategory.DupeBottoms:
          return UIMinionOrMannequinReactSource.OnBottomChanged;
        case PermitCategory.DupeGloves:
          return UIMinionOrMannequinReactSource.OnGlovesChanged;
        case PermitCategory.DupeShoes:
          return UIMinionOrMannequinReactSource.OnShoesChanged;
        default:
          DebugUtil.DevAssert(false, string.Format("Couldn't find a reaction for \"{0}\" clothing item category being changed", (object) clothingChangedCategory), (Object) null);
          return UIMinionOrMannequinReactSource.None;
      }
    }
  }

  public static void ReactToPersonalityChange(this UIMinionOrMannequin.ITarget self) => self.React(UIMinionOrMannequinReactSource.OnPersonalityChanged);

  public static void ReactToFullOutfitChange(this UIMinionOrMannequin.ITarget self) => self.React(UIMinionOrMannequinReactSource.OnWholeOutfitChanged);
}
