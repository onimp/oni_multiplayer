// Decompiled with JetBrains decompiler
// Type: TUNING.DISEASE
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace TUNING
{
  public class DISEASE
  {
    public const int COUNT_SCALER = 1000;
    public const int GENERIC_EMIT_COUNT = 100000;
    public const float GENERIC_EMIT_INTERVAL = 5f;
    public const float GENERIC_INFECTION_RADIUS = 1.5f;
    public const float GENERIC_INFECTION_INTERVAL = 5f;
    public const float STINKY_EMIT_MASS = 0.00250000018f;
    public const float STINKY_EMIT_INTERVAL = 2.5f;
    public const float STORAGE_TRANSFER_RATE = 0.05f;
    public const float WORKABLE_TRANSFER_RATE = 0.33f;
    public const float LADDER_TRANSFER_RATE = 0.005f;
    public const float INTERNAL_GERM_DEATH_MULTIPLIER = -0.00066666666f;
    public const float INTERNAL_GERM_DEATH_ADDEND = -0.8333333f;
    public const float MINIMUM_IMMUNE_DAMAGE = 0.000166666665f;

    public class DURATION
    {
      public const float LONG = 10800f;
      public const float LONGISH = 4620f;
      public const float NORMAL = 2220f;
      public const float SHORT = 1020f;
      public const float TEMPORARY = 180f;
      public const float VERY_BRIEF = 60f;
    }

    public class IMMUNE_ATTACK_STRENGTH_PERCENT
    {
      public const float SLOW_3 = 0.00025f;
      public const float SLOW_2 = 0.0005f;
      public const float SLOW_1 = 0.00125f;
      public const float NORMAL = 0.005f;
      public const float FAST_1 = 0.0125f;
      public const float FAST_2 = 0.05f;
      public const float FAST_3 = 0.125f;
    }

    public class RADIATION_KILL_RATE
    {
      public const float NO_EFFECT = 0.0f;
      public const float SLOW = 1f;
      public const float NORMAL = 2.5f;
      public const float FAST = 5f;
    }

    public static class GROWTH_FACTOR
    {
      public const float NONE = float.PositiveInfinity;
      public const float DEATH_1 = 12000f;
      public const float DEATH_2 = 6000f;
      public const float DEATH_3 = 3000f;
      public const float DEATH_4 = 1200f;
      public const float DEATH_5 = 300f;
      public const float DEATH_MAX = 10f;
      public const float DEATH_INSTANT = 0.0f;
      public const float GROWTH_1 = -12000f;
      public const float GROWTH_2 = -6000f;
      public const float GROWTH_3 = -3000f;
      public const float GROWTH_4 = -1200f;
      public const float GROWTH_5 = -600f;
      public const float GROWTH_6 = -300f;
      public const float GROWTH_7 = -150f;
    }

    public static class UNDERPOPULATION_DEATH_RATE
    {
      public const float NONE = 0.0f;
      private const float BASE_NUM_TO_KILL = 400f;
      public const float SLOW = 0.6666667f;
      public const float FAST = 2.66666675f;
    }
  }
}
