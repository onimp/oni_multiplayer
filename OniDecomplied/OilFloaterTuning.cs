// Decompiled with JetBrains decompiler
// Type: OilFloaterTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;

public static class OilFloaterTuning
{
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("OilfloaterEgg"),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("OilfloaterHighTempEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("OilfloaterDecorEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_HIGHTEMP = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("OilfloaterEgg"),
      weight = 0.33f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("OilfloaterHighTempEgg"),
      weight = 0.66f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("OilfloaterDecorEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_DECOR = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("OilfloaterEgg"),
      weight = 0.33f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("OilfloaterHighTempEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("OilfloaterDecorEgg"),
      weight = 0.66f
    }
  };
  public static float STANDARD_CALORIES_PER_CYCLE = 120000f;
  public static float STANDARD_STARVE_CYCLES = 5f;
  public static float STANDARD_STOMACH_SIZE = OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE * OilFloaterTuning.STANDARD_STARVE_CYCLES;
  public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;
  public static float EGG_MASS = 2f;
}
