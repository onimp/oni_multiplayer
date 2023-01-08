// Decompiled with JetBrains decompiler
// Type: TUNING.ROBOTS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace TUNING
{
  public class ROBOTS
  {
    public class SCOUTBOT
    {
      public static readonly float DIGGING = 1f;
      public static readonly float CONSTRUCTION = 1f;
      public static readonly float ATHLETICS = 1f;
      public static readonly float HIT_POINTS = 100f;
      public static readonly float BATTERY_DEPLETION_RATE = 30f;
      public static readonly float BATTERY_CAPACITY = (float) ((double) ROBOTS.SCOUTBOT.BATTERY_DEPLETION_RATE * 10.0 * 600.0);
    }
  }
}
