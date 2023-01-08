// Decompiled with JetBrains decompiler
// Type: Database.CycleNumber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class CycleNumber : 
    VictoryColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private int cycleNumber;

    public override string Name() => string.Format((string) COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_CYCLE, (object) this.cycleNumber);

    public override string Description() => string.Format((string) COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_CYCLE_DESCRIPTION, (object) this.cycleNumber);

    public CycleNumber(int cycleNumber = 100) => this.cycleNumber = cycleNumber;

    public override bool Success() => GameClock.Instance.GetCycle() + 1 >= this.cycleNumber;

    public void Deserialize(IReader reader) => this.cycleNumber = reader.ReadInt32();

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CYCLE_NUMBER, (object) (complete ? this.cycleNumber : GameClock.Instance.GetCycle() + 1), (object) this.cycleNumber);
  }
}
