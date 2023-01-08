// Decompiled with JetBrains decompiler
// Type: MooTuning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;

public static class MooTuning
{
  public static float STANDARD_CALORIES_PER_CYCLE = 200000f;
  public static float STANDARD_STARVE_CYCLES = 6f;
  public static float STANDARD_STOMACH_SIZE = MooTuning.STANDARD_CALORIES_PER_CYCLE * MooTuning.STANDARD_STARVE_CYCLES;
  public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER4;
  public static float EGG_MASS = 0.5f;
}
