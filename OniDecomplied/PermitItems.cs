// Decompiled with JetBrains decompiler
// Type: PermitItems
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;

public class PermitItems
{
  private static PermitItems.ItemInfo[] ItemInfos = new PermitItems.ItemInfo[75]
  {
    new PermitItems.ItemInfo("top_basic_black", 1U, "TopBasicBlack"),
    new PermitItems.ItemInfo("top_basic_white", 2U, "TopBasicWhite"),
    new PermitItems.ItemInfo("top_basic_red", 3U, "TopBasicRed"),
    new PermitItems.ItemInfo("top_basic_orange", 4U, "TopBasicOrange"),
    new PermitItems.ItemInfo("top_basic_yellow", 5U, "TopBasicYellow"),
    new PermitItems.ItemInfo("top_basic_green", 6U, "TopBasicGreen"),
    new PermitItems.ItemInfo("top_basic_blue_middle", 7U, "TopBasicAqua"),
    new PermitItems.ItemInfo("top_basic_purple", 8U, "TopBasicPurple"),
    new PermitItems.ItemInfo("top_basic_pink_orchid", 9U, "TopBasicPinkOrchid"),
    new PermitItems.ItemInfo("pants_basic_white", 11U, "BottomBasicWhite"),
    new PermitItems.ItemInfo("pants_basic_red", 12U, "BottomBasicRed"),
    new PermitItems.ItemInfo("pants_basic_orange", 13U, "BottomBasicOrange"),
    new PermitItems.ItemInfo("pants_basic_yellow", 14U, "BottomBasicYellow"),
    new PermitItems.ItemInfo("pants_basic_green", 15U, "BottomBasicGreen"),
    new PermitItems.ItemInfo("pants_basic_blue_middle", 16U, "BottomBasicAqua"),
    new PermitItems.ItemInfo("pants_basic_purple", 17U, "BottomBasicPurple"),
    new PermitItems.ItemInfo("pants_basic_pink_orchid", 18U, "BottomBasicPinkOrchid"),
    new PermitItems.ItemInfo("gloves_basic_black", 19U, "GlovesBasicBlack"),
    new PermitItems.ItemInfo("gloves_basic_white", 20U, "GlovesBasicWhite"),
    new PermitItems.ItemInfo("gloves_basic_red", 21U, "GlovesBasicRed"),
    new PermitItems.ItemInfo("gloves_basic_orange", 22U, "GlovesBasicOrange"),
    new PermitItems.ItemInfo("gloves_basic_yellow", 23U, "GlovesBasicYellow"),
    new PermitItems.ItemInfo("gloves_basic_green", 24U, "GlovesBasicGreen"),
    new PermitItems.ItemInfo("gloves_basic_blue_middle", 25U, "GlovesBasicAqua"),
    new PermitItems.ItemInfo("gloves_basic_purple", 26U, "GlovesBasicPurple"),
    new PermitItems.ItemInfo("gloves_basic_pink_orchid", 27U, "GlovesBasicPinkOrchid"),
    new PermitItems.ItemInfo("shoes_basic_white", 30U, "ShoesBasicWhite"),
    new PermitItems.ItemInfo("shoes_basic_red", 31U, "ShoesBasicRed"),
    new PermitItems.ItemInfo("shoes_basic_orange", 32U, "ShoesBasicOrange"),
    new PermitItems.ItemInfo("shoes_basic_yellow", 33U, "ShoesBasicYellow"),
    new PermitItems.ItemInfo("shoes_basic_green", 34U, "ShoesBasicGreen"),
    new PermitItems.ItemInfo("shoes_basic_blue_middle", 35U, "ShoesBasicAqua"),
    new PermitItems.ItemInfo("shoes_basic_purple", 36U, "ShoesBasicPurple"),
    new PermitItems.ItemInfo("shoes_basic_pink_orchid", 37U, "ShoesBasicPinkOrchid"),
    new PermitItems.ItemInfo("flowervase_retro", 39U, "FlowerVase_retro"),
    new PermitItems.ItemInfo("flowervase_retro_red", 40U, "FlowerVase_retro_red"),
    new PermitItems.ItemInfo("flowervase_retro_white", 41U, "FlowerVase_retro_white"),
    new PermitItems.ItemInfo("flowervase_retro_green", 42U, "FlowerVase_retro_green"),
    new PermitItems.ItemInfo("flowervase_retro_blue", 43U, "FlowerVase_retro_blue"),
    new PermitItems.ItemInfo("elegantbed_boat", 44U, "LuxuryBed_boat"),
    new PermitItems.ItemInfo("elegantbed_bouncy", 45U, "LuxuryBed_bouncy"),
    new PermitItems.ItemInfo("elegantbed_grandprix", 46U, "LuxuryBed_grandprix"),
    new PermitItems.ItemInfo("elegantbed_rocket", 47U, "LuxuryBed_rocket"),
    new PermitItems.ItemInfo("elegantbed_puft", 48U, "LuxuryBed_puft"),
    new PermitItems.ItemInfo("walls_pastel_pink", 49U, "ExteriorWall_pastel_pink"),
    new PermitItems.ItemInfo("walls_pastel_yellow", 50U, "ExteriorWall_pastel_yellow"),
    new PermitItems.ItemInfo("walls_pastel_green", 51U, "ExteriorWall_pastel_green"),
    new PermitItems.ItemInfo("walls_pastel_blue", 52U, "ExteriorWall_pastel_blue"),
    new PermitItems.ItemInfo("walls_pastel_purple", 53U, "ExteriorWall_pastel_purple"),
    new PermitItems.ItemInfo("walls_balm_lily", 54U, "ExteriorWall_balm_lily"),
    new PermitItems.ItemInfo("walls_clouds", 55U, "ExteriorWall_clouds"),
    new PermitItems.ItemInfo("walls_coffee", 56U, "ExteriorWall_coffee"),
    new PermitItems.ItemInfo("walls_mosaic", 57U, "ExteriorWall_mosaic"),
    new PermitItems.ItemInfo("walls_mushbar", 58U, "ExteriorWall_mushbar"),
    new PermitItems.ItemInfo("walls_plaid", 59U, "ExteriorWall_plaid"),
    new PermitItems.ItemInfo("walls_rain", 60U, "ExteriorWall_rain"),
    new PermitItems.ItemInfo("walls_rainbow", 61U, "ExteriorWall_rainbow"),
    new PermitItems.ItemInfo("walls_snow", 62U, "ExteriorWall_snow"),
    new PermitItems.ItemInfo("walls_sun", 63U, "ExteriorWall_sun"),
    new PermitItems.ItemInfo("walls_polka", 64U, "ExteriorWall_polka"),
    new PermitItems.ItemInfo("painting_art_i", 65U, "Canvas_Good7"),
    new PermitItems.ItemInfo("painting_art_j", 66U, "Canvas_Good8"),
    new PermitItems.ItemInfo("painting_art_k", 67U, "Canvas_Good9"),
    new PermitItems.ItemInfo("painting_tall_art_g", 68U, "CanvasTall_Good5"),
    new PermitItems.ItemInfo("painting_tall_art_h", 69U, "CanvasTall_Good6"),
    new PermitItems.ItemInfo("painting_tall_art_i", 70U, "CanvasTall_Good7"),
    new PermitItems.ItemInfo("painting_wide_art_g", 71U, "CanvasWide_Good5"),
    new PermitItems.ItemInfo("painting_wide_art_h", 72U, "CanvasWide_Good6"),
    new PermitItems.ItemInfo("painting_wide_art_i", 73U, "CanvasWide_Good7"),
    new PermitItems.ItemInfo("sculpture_amazing_4", 74U, "Sculpture_Good4"),
    new PermitItems.ItemInfo("sculpture_1x2_amazing_4", 75U, "SmallSculpture_Good4"),
    new PermitItems.ItemInfo("sculpture_metal_amazing_4", 76U, "MetalSculpture_Good4"),
    new PermitItems.ItemInfo("sculpture_marble_amazing_4", 77U, "MarbleSculpture_Good4"),
    new PermitItems.ItemInfo("sculpture_marble_amazing_5", 78U, "MarbleSculpture_Good5"),
    new PermitItems.ItemInfo("icesculpture_idle_2", 79U, "IceSculpture_Average2")
  };
  private static Dictionary<string, PermitItems.ItemInfo> Mappings = ((IEnumerable<PermitItems.ItemInfo>) PermitItems.ItemInfos).ToDictionary<PermitItems.ItemInfo, string>((Func<PermitItems.ItemInfo, string>) (x => x.PermitId));
  private static Dictionary<string, PermitItems.ItemInfo> ReverseMappings = ((IEnumerable<PermitItems.ItemInfo>) PermitItems.ItemInfos).ToDictionary<PermitItems.ItemInfo, string>((Func<PermitItems.ItemInfo, string>) (x => x.ItemType));

  public static bool IsPermitUnlocked(string permitId)
  {
    PermitItems.ItemInfo itemInfo;
    return !PermitItems.Mappings.TryGetValue(permitId, out itemInfo) || KleiItems.HasItem(itemInfo.ItemType);
  }

  public static bool IsPermitOwnable(string permitId) => PermitItems.Mappings.ContainsKey(permitId);

  public static string GetPermitIDByKleiItemType(string itemType)
  {
    if (PermitItems.ReverseMappings.ContainsKey(itemType))
      return PermitItems.ReverseMappings[itemType].PermitId;
    Debug.LogError((object) ("Could not find PermitItem with requested Klei itemType: " + itemType + ". It may be that this is a non Klei Item permit, or you just have a bad ID."));
    return (string) null;
  }

  public static Option<int> GetOwnedCount(string permitId)
  {
    PermitResource permit = Db.Get().Permits.TryGet(permitId);
    return permit == null ? (Option<int>) Option.None : PermitItems.GetOwnedCount(permit);
  }

  public static Option<int> GetOwnedCount(PermitResource permit)
  {
    PermitItems.ItemInfo itemInfo;
    return !PermitItems.Mappings.TryGetValue(permit.Id, out itemInfo) ? (Option<int>) Option.None : (Option<int>) KleiItems.GetOwnedItemCount(itemInfo.ItemType);
  }

  public static PermitPresentationInfo GetPermitPresentationInfo(string id)
  {
    PermitResource permitResource = Db.Get().Permits.TryGet(id);
    if (permitResource != null)
      return permitResource.GetPermitPresentationInfo();
    return new PermitPresentationInfo()
    {
      name = (string) UI.KLEI_INVENTORY_SCREEN.ITEM_UNKNOWN_NAME,
      description = (string) UI.KLEI_INVENTORY_SCREEN.ITEM_UNKNOWN_DESCRIPTION,
      sprite = PermitPresentationInfo.GetUnknownSprite(),
      isNone = true
    };
  }

  private struct ItemInfo
  {
    public string ItemType;
    public uint ItemId;
    public string PermitId;

    public ItemInfo(string itemType, uint itemId, string permitId)
    {
      this.ItemType = itemType;
      this.PermitId = permitId;
      this.ItemId = itemId;
    }
  }
}
