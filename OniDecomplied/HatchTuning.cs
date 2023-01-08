// Decompiled with JetBrains decompiler
// Type: HatchTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;

public static class HatchTuning
{
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchEgg"),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchHardEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchVeggieEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_HARD = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchEgg"),
      weight = 0.32f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchHardEgg"),
      weight = 0.65f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchMetalEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_VEGGIE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchEgg"),
      weight = 0.33f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchVeggieEgg"),
      weight = 0.67f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_METAL = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchEgg"),
      weight = 0.11f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchHardEgg"),
      weight = 0.22f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("HatchMetalEgg"),
      weight = 0.67f
    }
  };
  public static float STANDARD_CALORIES_PER_CYCLE = 700000f;
  public static float STANDARD_STARVE_CYCLES = 10f;
  public static float STANDARD_STOMACH_SIZE = HatchTuning.STANDARD_CALORIES_PER_CYCLE * HatchTuning.STANDARD_STARVE_CYCLES;
  public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;
  public static float EGG_MASS = 2f;
}
