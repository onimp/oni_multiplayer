// Decompiled with JetBrains decompiler
// Type: CrabTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;

public static class CrabTuning
{
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("CrabEgg"),
      weight = 0.97f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("CrabWoodEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("CrabFreshWaterEgg"),
      weight = 0.01f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_WOOD = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("CrabEgg"),
      weight = 0.32f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("CrabWoodEgg"),
      weight = 0.65f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("CrabFreshWaterEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_FRESH = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("CrabEgg"),
      weight = 0.32f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("CrabWoodEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("CrabFreshWaterEgg"),
      weight = 0.65f
    }
  };
  public static float STANDARD_CALORIES_PER_CYCLE = 100000f;
  public static float STANDARD_STARVE_CYCLES = 10f;
  public static float STANDARD_STOMACH_SIZE = CrabTuning.STANDARD_CALORIES_PER_CYCLE * CrabTuning.STANDARD_STARVE_CYCLES;
  public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;
  public static float EGG_MASS = 2f;
}
