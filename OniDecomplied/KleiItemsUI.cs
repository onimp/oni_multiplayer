// Decompiled with JetBrains decompiler
// Type: KleiItemsUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using STRINGS;
using UnityEngine;

public static class KleiItemsUI
{
  public static readonly Color TEXT_COLOR__PERMIT_NOT_OWNED = KleiItemsUI.GetColor("#DD992F");

  public static string WrapAsToolTipTitle(string text) => "<b><style=\"KLink\">" + text + "</style></b>";

  public static string WrapWithColor(string text, Color color) => "<color=#" + Util.ToHexString(color) + ">" + text + "</color>";

  public static Sprite GetNoneOutfitIcon() => Assets.GetSprite(HashedString.op_Implicit("NoTraits"));

  public static Sprite GetNoneClothingItemIcon(PermitCategory category) => Assets.GetSprite(HashedString.op_Implicit("NoTraits"));

  public static string GetNoneClothingItemString(PermitCategory category)
  {
    switch (category)
    {
      case PermitCategory.DupeTops:
        return (string) UI.OUTFIT_DESCRIPTION.NO_DUPE_TOPS;
      case PermitCategory.DupeBottoms:
        return (string) UI.OUTFIT_DESCRIPTION.NO_DUPE_BOTTOMS;
      case PermitCategory.DupeGloves:
        return (string) UI.OUTFIT_DESCRIPTION.NO_DUPE_GLOVES;
      case PermitCategory.DupeShoes:
        return (string) UI.OUTFIT_DESCRIPTION.NO_DUPE_SHOES;
      case PermitCategory.DupeHats:
        return (string) UI.OUTFIT_DESCRIPTION.NO_DUPE_HATS;
      case PermitCategory.DupeAccessories:
        return (string) UI.OUTFIT_DESCRIPTION.NO_DUPE_ACCESSORIES;
      default:
        DebugUtil.DevAssert(false, string.Format("Couldn't find \"no item\" string for category {0}", (object) category), (Object) null);
        return "-";
    }
  }

  public static void ConfigureTooltipOn(GameObject gameObject, Option<LocString> tooltipText = default (Option<LocString>)) => KleiItemsUI.ConfigureTooltipOn(gameObject, tooltipText.HasValue ? Option.Some<string>((string) tooltipText.Value) : (Option<string>) Option.None);

  public static void ConfigureTooltipOn(GameObject gameObject, Option<string> tooltipText = default (Option<string>))
  {
    ToolTip toolTip = gameObject.GetComponent<ToolTip>();
    if (Util.IsNullOrDestroyed((object) toolTip))
    {
      toolTip = gameObject.AddComponent<ToolTip>();
      toolTip.tooltipPivot = new Vector2(0.5f, 1f);
      toolTip.tooltipPositionOffset = !Object.op_Implicit((Object) gameObject.GetComponent<KButton>()) ? new Vector2(0.0f, 0.0f) : new Vector2(0.0f, 22f);
      toolTip.parentPositionAnchor = new Vector2(0.5f, 0.0f);
      toolTip.toolTipPosition = (ToolTip.TooltipPosition) 6;
    }
    if (!tooltipText.HasValue)
      toolTip.ClearMultiStringTooltip();
    else
      toolTip.SetSimpleTooltip(tooltipText.Value);
  }

  public static string GetTooltipStringFor(PermitResource permit) => KleiItemsUI.GetTooltipStringFor(permit.GetPermitPresentationInfo());

  public static string GetTooltipStringFor(PermitPresentationInfo permitPresInfo)
  {
    string tooltipStringFor = KleiItemsUI.WrapAsToolTipTitle(permitPresInfo.name);
    if (!string.IsNullOrWhiteSpace(permitPresInfo.description))
      tooltipStringFor = tooltipStringFor + "\n" + permitPresInfo.description;
    if (!string.IsNullOrWhiteSpace(permitPresInfo.rarityDetails))
      tooltipStringFor = tooltipStringFor + "\n\n" + permitPresInfo.rarityDetails;
    if (!permitPresInfo.IsUnlocked())
      tooltipStringFor = tooltipStringFor + "\n\n" + KleiItemsUI.WrapWithColor((string) UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED);
    return tooltipStringFor;
  }

  public static Color GetColor(string input) => input[0] == '#' ? Util.ColorFromHex(input.Substring(1)) : Util.ColorFromHex(input);
}
