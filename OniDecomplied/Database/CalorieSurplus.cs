// Decompiled with JetBrains decompiler
// Type: Database.CalorieSurplus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class CalorieSurplus : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private double surplusAmount;

    public CalorieSurplus(float surplusAmount) => this.surplusAmount = (double) surplusAmount;

    public override bool Success() => (double) ClusterManager.Instance.CountAllRations() / 1000.0 >= this.surplusAmount;

    public override bool Fail() => !this.Success();

    public void Deserialize(IReader reader) => this.surplusAmount = reader.ReadDouble();

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CALORIE_SURPLUS, (object) GameUtil.GetFormattedCalories(complete ? (float) this.surplusAmount : ClusterManager.Instance.CountAllRations()), (object) GameUtil.GetFormattedCalories((float) this.surplusAmount));
  }
}
