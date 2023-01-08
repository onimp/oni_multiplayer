// Decompiled with JetBrains decompiler
// Type: TUNING.NOISE_POLLUTION
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace TUNING
{
  public class NOISE_POLLUTION
  {
    public static readonly EffectorValues NONE = new EffectorValues()
    {
      amount = 0,
      radius = 0
    };
    public static readonly EffectorValues CONE_OF_SILENCE = new EffectorValues()
    {
      amount = -120,
      radius = 5
    };
    public static float DUPLICANT_TIME_THRESHOLD = 3f;

    public class LENGTHS
    {
      public static float VERYSHORT = 0.25f;
      public static float SHORT = 0.5f;
      public static float NORMAL = 1f;
      public static float LONG = 1.5f;
      public static float VERYLONG = 2f;
    }

    public class NOISY
    {
      public static readonly EffectorValues TIER0 = new EffectorValues()
      {
        amount = 45,
        radius = 10
      };
      public static readonly EffectorValues TIER1 = new EffectorValues()
      {
        amount = 55,
        radius = 10
      };
      public static readonly EffectorValues TIER2 = new EffectorValues()
      {
        amount = 65,
        radius = 10
      };
      public static readonly EffectorValues TIER3 = new EffectorValues()
      {
        amount = 75,
        radius = 15
      };
      public static readonly EffectorValues TIER4 = new EffectorValues()
      {
        amount = 90,
        radius = 15
      };
      public static readonly EffectorValues TIER5 = new EffectorValues()
      {
        amount = 105,
        radius = 20
      };
      public static readonly EffectorValues TIER6 = new EffectorValues()
      {
        amount = 125,
        radius = 20
      };
    }

    public class CREATURES
    {
      public static readonly EffectorValues TIER0 = new EffectorValues()
      {
        amount = 30,
        radius = 5
      };
      public static readonly EffectorValues TIER1 = new EffectorValues()
      {
        amount = 35,
        radius = 5
      };
      public static readonly EffectorValues TIER2 = new EffectorValues()
      {
        amount = 45,
        radius = 5
      };
      public static readonly EffectorValues TIER3 = new EffectorValues()
      {
        amount = 55,
        radius = 5
      };
      public static readonly EffectorValues TIER4 = new EffectorValues()
      {
        amount = 65,
        radius = 5
      };
      public static readonly EffectorValues TIER5 = new EffectorValues()
      {
        amount = 75,
        radius = 5
      };
      public static readonly EffectorValues TIER6 = new EffectorValues()
      {
        amount = 90,
        radius = 10
      };
      public static readonly EffectorValues TIER7 = new EffectorValues()
      {
        amount = 105,
        radius = 10
      };
    }

    public class DAMPEN
    {
      public static readonly EffectorValues TIER0 = new EffectorValues()
      {
        amount = -5,
        radius = 1
      };
      public static readonly EffectorValues TIER1 = new EffectorValues()
      {
        amount = -10,
        radius = 2
      };
      public static readonly EffectorValues TIER2 = new EffectorValues()
      {
        amount = -15,
        radius = 3
      };
      public static readonly EffectorValues TIER3 = new EffectorValues()
      {
        amount = -20,
        radius = 4
      };
      public static readonly EffectorValues TIER4 = new EffectorValues()
      {
        amount = -20,
        radius = 5
      };
      public static readonly EffectorValues TIER5 = new EffectorValues()
      {
        amount = -25,
        radius = 6
      };
    }
  }
}
