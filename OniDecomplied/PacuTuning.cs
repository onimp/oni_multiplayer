// Decompiled with JetBrains decompiler
// Type: PacuTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;

public static class PacuTuning
{
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PacuEgg"),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PacuTropicalEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PacuCleanerEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_TROPICAL = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PacuEgg"),
      weight = 0.32f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PacuTropicalEgg"),
      weight = 0.65f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PacuCleanerEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_CLEANER = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PacuEgg"),
      weight = 0.32f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PacuCleanerEgg"),
      weight = 0.65f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PacuTropicalEgg"),
      weight = 0.02f
    }
  };
  public static float STANDARD_CALORIES_PER_CYCLE = 100000f;
  public static float STANDARD_STARVE_CYCLES = 5f;
  public static float STANDARD_STOMACH_SIZE = PacuTuning.STANDARD_CALORIES_PER_CYCLE * PacuTuning.STANDARD_STARVE_CYCLES;
  public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER2;
  public static float EGG_MASS = 4f;
}
