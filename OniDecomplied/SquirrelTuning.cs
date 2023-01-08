// Decompiled with JetBrains decompiler
// Type: SquirrelTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;

public static class SquirrelTuning
{
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("SquirrelEgg"),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("SquirrelHugEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_HUG = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("SquirrelEgg"),
      weight = 0.35f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("SquirrelHugEgg"),
      weight = 0.65f
    }
  };
  public static float STANDARD_CALORIES_PER_CYCLE = 100000f;
  public static float STANDARD_STARVE_CYCLES = 10f;
  public static float STANDARD_STOMACH_SIZE = SquirrelTuning.STANDARD_CALORIES_PER_CYCLE * SquirrelTuning.STANDARD_STARVE_CYCLES;
  public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;
  public static int PEN_SIZE_PER_CREATURE_HUG = CREATURES.SPACE_REQUIREMENTS.TIER1;
  public static float EGG_MASS = 2f;
}
