// Decompiled with JetBrains decompiler
// Type: Database.PermitCategories
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;

namespace Database
{
  public static class PermitCategories
  {
    private static Dictionary<PermitCategory, PermitCategories.CategoryInfo> CategoryInfos = new Dictionary<PermitCategory, PermitCategories.CategoryInfo>()
    {
      {
        PermitCategory.Equipment,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.EQUIPMENT, "icon_inventory_equipment")
      },
      {
        PermitCategory.DupeTops,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.DUPE_TOPS, "icon_inventory_tops")
      },
      {
        PermitCategory.DupeBottoms,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.DUPE_BOTTOMS, "icon_inventory_bottoms")
      },
      {
        PermitCategory.DupeGloves,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.DUPE_GLOVES, "icon_inventory_gloves")
      },
      {
        PermitCategory.DupeShoes,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.DUPE_SHOES, "icon_inventory_shoes")
      },
      {
        PermitCategory.DupeHats,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.DUPE_HATS, "icon_inventory_hats")
      },
      {
        PermitCategory.DupeAccessories,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.DUPE_ACCESSORIES, "icon_inventory_accessories")
      },
      {
        PermitCategory.Building,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.BUILDINGS, "icon_inventory_buildings")
      },
      {
        PermitCategory.Critter,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.CRITTERS, "icon_inventory_critters")
      },
      {
        PermitCategory.Sweepy,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.SWEEPYS, "icon_inventory_sweepys")
      },
      {
        PermitCategory.Duplicant,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.DUPLICANTS, "icon_inventory_duplicants")
      },
      {
        PermitCategory.Artwork,
        new PermitCategories.CategoryInfo((string) UI.KLEI_INVENTORY_SCREEN.CATEGORIES.ARTWORKS, "icon_inventory_artworks")
      }
    };

    public static string GetDisplayName(PermitCategory category) => PermitCategories.CategoryInfos[category].displayName;

    public static string GetUppercaseDisplayName(PermitCategory category) => PermitCategories.CategoryInfos[category].displayName.ToUpper();

    public static string GetIconName(PermitCategory category) => PermitCategories.CategoryInfos[category].iconName;

    public static PermitCategory GetCategoryForId(string id)
    {
      try
      {
        return (PermitCategory) Enum.Parse(typeof (PermitCategory), id);
      }
      catch (ArgumentException ex)
      {
        Debug.LogError((object) (id + " is not a valid PermitCategory."));
      }
      return PermitCategory.Equipment;
    }

    private class CategoryInfo
    {
      public string displayName;
      public string iconName;

      public CategoryInfo(string name, string icon_name)
      {
        this.displayName = name;
        this.iconName = icon_name;
      }
    }
  }
}
