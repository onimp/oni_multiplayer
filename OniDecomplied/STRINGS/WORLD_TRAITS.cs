// Decompiled with JetBrains decompiler
// Type: STRINGS.WORLD_TRAITS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace STRINGS
{
  public static class WORLD_TRAITS
  {
    public static LocString MISSING_TRAIT = (LocString) "<missing traits>";

    public static class NO_TRAITS
    {
      public static LocString NAME = (LocString) "<i>This world is stable and has no unusual features.</i>";
      public static LocString NAME_SHORTHAND = (LocString) "No unusual features";
      public static LocString DESCRIPTION = (LocString) "This world exists in a particularly stable configuration each time it is encountered";
    }

    public static class BOULDERS_LARGE
    {
      public static LocString NAME = (LocString) "Large Boulders";
      public static LocString DESCRIPTION = (LocString) "Huge boulders make digging through this world more difficult";
    }

    public static class BOULDERS_MEDIUM
    {
      public static LocString NAME = (LocString) "Medium Boulders";
      public static LocString DESCRIPTION = (LocString) "Mid-sized boulders make digging through this world more difficult";
    }

    public static class BOULDERS_MIXED
    {
      public static LocString NAME = (LocString) "Mixed Boulders";
      public static LocString DESCRIPTION = (LocString) "Boulders of various sizes make digging through this world more difficult";
    }

    public static class BOULDERS_SMALL
    {
      public static LocString NAME = (LocString) "Small Boulders";
      public static LocString DESCRIPTION = (LocString) "Tiny boulders make digging through this world more difficult";
    }

    public static class DEEP_OIL
    {
      public static LocString NAME = (LocString) "Trapped Oil";
      public static LocString DESCRIPTION = (LocString) ("Most of the " + UI.PRE_KEYWORD + "Oil" + UI.PST_KEYWORD + " in this world will need to be extracted with " + (string) BUILDINGS.PREFABS.OILWELLCAP.NAME + "s");
    }

    public static class FROZEN_CORE
    {
      public static LocString NAME = (LocString) "Frozen Core";
      public static LocString DESCRIPTION = (LocString) ("This world has a chilly core of solid " + (string) ELEMENTS.ICE.NAME);
    }

    public static class GEOACTIVE
    {
      public static LocString NAME = (LocString) "Geoactive";
      public static LocString DESCRIPTION = (LocString) ("This world has more " + UI.PRE_KEYWORD + "Geysers" + UI.PST_KEYWORD + " and " + UI.PRE_KEYWORD + "Vents" + UI.PST_KEYWORD + " than usual");
    }

    public static class GEODES
    {
      public static LocString NAME = (LocString) "Geodes";
      public static LocString DESCRIPTION = (LocString) "Large geodes containing rare material caches are deposited across this world";
    }

    public static class GEODORMANT
    {
      public static LocString NAME = (LocString) "Geodormant";
      public static LocString DESCRIPTION = (LocString) ("This world has fewer " + UI.PRE_KEYWORD + "Geysers" + UI.PST_KEYWORD + " and " + UI.PRE_KEYWORD + "Vents" + UI.PST_KEYWORD + " than usual");
    }

    public static class GLACIERS_LARGE
    {
      public static LocString NAME = (LocString) "Large Glaciers";
      public static LocString DESCRIPTION = (LocString) ("Huge chunks of primordial " + (string) ELEMENTS.ICE.NAME + " are scattered across this world");
    }

    public static class IRREGULAR_OIL
    {
      public static LocString NAME = (LocString) "Irregular Oil";
      public static LocString DESCRIPTION = (LocString) ("The " + UI.PRE_KEYWORD + "Oil" + UI.PST_KEYWORD + " on this asteroid is anything but regular!");
    }

    public static class MAGMA_VENTS
    {
      public static LocString NAME = (LocString) "Magma Channels";
      public static LocString DESCRIPTION = (LocString) ("The " + (string) ELEMENTS.MAGMA.NAME + " from this world's core has leaked into the mantle and crust");
    }

    public static class METAL_POOR
    {
      public static LocString NAME = (LocString) "Metal Poor";
      public static LocString DESCRIPTION = (LocString) ("There is a reduced amount of " + UI.PRE_KEYWORD + "Metal Ore" + UI.PST_KEYWORD + " on this world, proceed with caution!");
    }

    public static class METAL_RICH
    {
      public static LocString NAME = (LocString) "Metal Rich";
      public static LocString DESCRIPTION = (LocString) ("This asteroid is an abundant source of " + UI.PRE_KEYWORD + "Metal Ore" + UI.PST_KEYWORD);
    }

    public static class MISALIGNED_START
    {
      public static LocString NAME = (LocString) "Alternate Pod Location";
      public static LocString DESCRIPTION = (LocString) ("The " + (string) BUILDINGS.PREFABS.HEADQUARTERSCOMPLETE.NAME + " didn't end up in the asteroid's exact center this time... but it's still nowhere near the surface");
    }

    public static class SLIME_SPLATS
    {
      public static LocString NAME = (LocString) "Slime Molds";
      public static LocString DESCRIPTION = (LocString) ("Sickly " + (string) ELEMENTS.SLIMEMOLD.NAME + " growths have crept all over this world");
    }

    public static class SUBSURFACE_OCEAN
    {
      public static LocString NAME = (LocString) "Subsurface Ocean";
      public static LocString DESCRIPTION = (LocString) ("Below the crust of this world is a " + (string) ELEMENTS.SALTWATER.NAME + " sea");
    }

    public static class VOLCANOES
    {
      public static LocString NAME = (LocString) "Volcanic Activity";
      public static LocString DESCRIPTION = (LocString) ("Several active " + UI.PRE_KEYWORD + "Volcanoes" + UI.PST_KEYWORD + " have been detected in this world");
    }

    public static class RADIOACTIVE_CRUST
    {
      public static LocString NAME = (LocString) "Radioactive Crust";
      public static LocString DESCRIPTION = (LocString) ("Deposits of " + (string) ELEMENTS.URANIUMORE.NAME + " are found in this world's crust");
    }

    public static class LUSH_CORE
    {
      public static LocString NAME = (LocString) "Lush Core";
      public static LocString DESCRIPTION = (LocString) "This world has a lush forest core";
    }

    public static class METAL_CAVES
    {
      public static LocString NAME = (LocString) "Metallic Caves";
      public static LocString DESCRIPTION = (LocString) "This world has caves of metal ore";
    }

    public static class DISTRESS_SIGNAL
    {
      public static LocString NAME = (LocString) "Frozen Friend";
      public static LocString DESCRIPTION = (LocString) "This world contains a frozen friend from a long time ago";
    }

    public static class CRASHED_SATELLITES
    {
      public static LocString NAME = (LocString) "Crashed Satellites";
      public static LocString DESCRIPTION = (LocString) "This world contains crashed radioactive satellites";
    }
  }
}
