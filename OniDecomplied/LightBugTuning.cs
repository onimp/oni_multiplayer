// Decompiled with JetBrains decompiler
// Type: LightBugTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;

public static class LightBugTuning
{
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugEgg"),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugOrangeEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_ORANGE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugEgg"),
      weight = 0.33f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugOrangeEgg"),
      weight = 0.66f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugPurpleEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_PURPLE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugOrangeEgg"),
      weight = 0.33f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugPurpleEgg"),
      weight = 0.66f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugPinkEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_PINK = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugPurpleEgg"),
      weight = 0.33f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugPinkEgg"),
      weight = 0.66f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugBlueEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BLUE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugPinkEgg"),
      weight = 0.33f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugBlueEgg"),
      weight = 0.66f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugBlackEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BLACK = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugBlueEgg"),
      weight = 0.33f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugBlackEgg"),
      weight = 0.66f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugCrystalEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_CRYSTAL = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugCrystalEgg"),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("LightBugEgg"),
      weight = 0.02f
    }
  };
  public static float STANDARD_CALORIES_PER_CYCLE = 40000f;
  public static float STANDARD_STARVE_CYCLES = 8f;
  public static float STANDARD_STOMACH_SIZE = LightBugTuning.STANDARD_CALORIES_PER_CYCLE * LightBugTuning.STANDARD_STARVE_CYCLES;
  public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;
  public static float EGG_MASS = 0.2f;
}
