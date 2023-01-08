// Decompiled with JetBrains decompiler
// Type: TUNING.SKILLS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace TUNING
{
  public class SKILLS
  {
    public static int TARGET_SKILLS_EARNED = 15;
    public static int TARGET_SKILLS_CYCLE = 250;
    public static float EXPERIENCE_LEVEL_POWER = 1.44f;
    public static float PASSIVE_EXPERIENCE_PORTION = 0.5f;
    public static float ACTIVE_EXPERIENCE_PORTION = 0.6f;
    public static float FULL_EXPERIENCE = 1f;
    public static float ALL_DAY_EXPERIENCE = SKILLS.FULL_EXPERIENCE / 0.9f;
    public static float MOST_DAY_EXPERIENCE = SKILLS.FULL_EXPERIENCE / 0.75f;
    public static float PART_DAY_EXPERIENCE = SKILLS.FULL_EXPERIENCE / 0.5f;
    public static float BARELY_EVER_EXPERIENCE = SKILLS.FULL_EXPERIENCE / 0.25f;
    public static float APTITUDE_EXPERIENCE_MULTIPLIER = 0.5f;
    public static int[] SKILL_TIER_MORALE_COST = new int[7]
    {
      1,
      2,
      3,
      4,
      5,
      6,
      7
    };
  }
}
