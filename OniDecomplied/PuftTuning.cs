// Decompiled with JetBrains decompiler
// Type: PuftTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;

public static class PuftTuning
{
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftEgg"),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftAlphaEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftOxyliteEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftBleachstoneEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_ALPHA = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftEgg"),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftAlphaEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_OXYLITE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftEgg"),
      weight = 0.31f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftAlphaEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftOxyliteEgg"),
      weight = 0.67f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BLEACHSTONE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftEgg"),
      weight = 0.31f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftAlphaEgg"),
      weight = 0.02f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("PuftBleachstoneEgg"),
      weight = 0.67f
    }
  };
  public static float STANDARD_CALORIES_PER_CYCLE = 200000f;
  public static float STANDARD_STARVE_CYCLES = 6f;
  public static float STANDARD_STOMACH_SIZE = PuftTuning.STANDARD_CALORIES_PER_CYCLE * PuftTuning.STANDARD_STARVE_CYCLES;
  public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER4;
  public static float EGG_MASS = 0.5f;
}
