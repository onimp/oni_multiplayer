// Decompiled with JetBrains decompiler
// Type: MoleTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;

public static class MoleTuning
{
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("MoleEgg"),
      weight = 0.98f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("MoleDelicacyEgg"),
      weight = 0.02f
    }
  };
  public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_DELICACY = new List<FertilityMonitor.BreedingChance>()
  {
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("MoleEgg"),
      weight = 0.32f
    },
    new FertilityMonitor.BreedingChance()
    {
      egg = TagExtensions.ToTag("MoleDelicacyEgg"),
      weight = 0.65f
    }
  };
  public static float STANDARD_CALORIES_PER_CYCLE = 4800000f;
  public static float STANDARD_STARVE_CYCLES = 10f;
  public static float STANDARD_STOMACH_SIZE = MoleTuning.STANDARD_CALORIES_PER_CYCLE * MoleTuning.STANDARD_STARVE_CYCLES;
  public static float DELICACY_STOMACH_SIZE = MoleTuning.STANDARD_STOMACH_SIZE / 2f;
  public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER2;
  public static float EGG_MASS = 2f;
  public static int DEPTH_TO_HIDE = 2;
  public static HashedString[] GINGER_SYMBOL_NAMES = new HashedString[6]
  {
    HashedString.op_Implicit("del_ginger"),
    HashedString.op_Implicit("del_ginger1"),
    HashedString.op_Implicit("del_ginger2"),
    HashedString.op_Implicit("del_ginger3"),
    HashedString.op_Implicit("del_ginger4"),
    HashedString.op_Implicit("del_ginger5")
  };
}
