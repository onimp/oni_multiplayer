// Decompiled with JetBrains decompiler
// Type: Database.BeforeCycleNumber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class BeforeCycleNumber : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private int cycleNumber;

    public BeforeCycleNumber(int cycleNumber = 100) => this.cycleNumber = cycleNumber;

    public override bool Success() => GameClock.Instance.GetCycle() + 1 <= this.cycleNumber;

    public override bool Fail() => !this.Success();

    public void Deserialize(IReader reader) => this.cycleNumber = reader.ReadInt32();

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.REMAINING_CYCLES, (object) Mathf.Max(this.cycleNumber - GameClock.Instance.GetCycle(), 0), (object) this.cycleNumber);
  }
}
