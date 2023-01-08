// Decompiled with JetBrains decompiler
// Type: ClothingOutfitTargetExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using STRINGS;
using System.Collections.Generic;

public static class ClothingOutfitTargetExtensions
{
  public static readonly string[] NO_ITEMS = new string[0];
  public static readonly ClothingItemResource[] NO_ITEM_VALUES = new ClothingItemResource[0];

  public static string ReadName(this Option<ClothingOutfitTarget> self) => self.HasValue ? self.Value.ReadName() : (string) UI.OUTFIT_NAME.NONE;

  public static IEnumerable<string> ReadItems(this Option<ClothingOutfitTarget> self) => self.HasValue ? (IEnumerable<string>) self.Value.ReadItems() : (IEnumerable<string>) ClothingOutfitTargetExtensions.NO_ITEMS;

  public static IEnumerable<ClothingItemResource> ReadItemValues(
    this Option<ClothingOutfitTarget> self)
  {
    return self.HasValue ? self.Value.ReadItemValues() : (IEnumerable<ClothingItemResource>) ClothingOutfitTargetExtensions.NO_ITEM_VALUES;
  }

  public static Option<string> GetId(this Option<ClothingOutfitTarget> self) => self.HasValue ? (Option<string>) self.Value.Id : (Option<string>) Option.None;
}
