// Decompiled with JetBrains decompiler
// Type: Database.EatXCalories
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class EatXCalories : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private int numCalories;

    public EatXCalories(int numCalories) => this.numCalories = numCalories;

    public override bool Success() => (double) RationTracker.Get().GetCaloriesConsumed() / 1000.0 > (double) this.numCalories;

    public void Deserialize(IReader reader) => this.numCalories = reader.ReadInt32();

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CONSUME_CALORIES, (object) GameUtil.GetFormattedCalories(complete ? (float) this.numCalories * 1000f : RationTracker.Get().GetCaloriesConsumed()), (object) GameUtil.GetFormattedCalories((float) this.numCalories * 1000f));
  }
}
