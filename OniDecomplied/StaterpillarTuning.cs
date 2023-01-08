// Decompiled with JetBrains decompiler
// Type: StaterpillarTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public static class StaterpillarTuning
{
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("StaterpillarEgg"),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("StaterpillarGasEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("StaterpillarLiquidEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_GAS = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("StaterpillarEgg"),
      weight = 0.32f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("StaterpillarGasEgg"),
      weight = 0.66f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("StaterpillarLiquidEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_LIQUID = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("StaterpillarEgg"),
      weight = 0.32f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("StaterpillarGasEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("StaterpillarLiquidEgg"),
      weight = 0.66f
    }
  };
  public static float STANDARD_CALORIES_PER_CYCLE = 2000000f;
  public static float STANDARD_STARVE_CYCLES = 5f;
  public static float STANDARD_STOMACH_SIZE = StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE * StaterpillarTuning.STANDARD_STARVE_CYCLES;
  public static float POOP_CONVERSTION_RATE = 0.05f;
  public static float EGG_MASS = 2f;
}
