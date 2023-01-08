// Decompiled with JetBrains decompiler
// Type: Database.AnalyzeSeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class AnalyzeSeed : ColonyAchievementRequirement
  {
    private string seedName;

    public AnalyzeSeed(string seedname) => this.seedName = seedname;

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ANALYZE_SEED, (object) Tag.op_Implicit(this.seedName).ProperName());

    public override bool Success() => ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().analyzedSeeds.Contains(Tag.op_Implicit(this.seedName));
  }
}
