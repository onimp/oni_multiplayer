// Decompiled with JetBrains decompiler
// Type: Database.SurviveARocketWithMinimumMorale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
  public class SurviveARocketWithMinimumMorale : ColonyAchievementRequirement
  {
    public float minimumMorale;
    public int numberOfCycles;

    public SurviveARocketWithMinimumMorale(float minimumMorale, int numberOfCycles)
    {
      this.minimumMorale = minimumMorale;
      this.numberOfCycles = numberOfCycles;
    }

    public override string GetProgress(bool complete) => complete ? string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.SURVIVE_SPACE_COMPLETE, (object) this.minimumMorale, (object) this.numberOfCycles) : base.GetProgress(complete);

    public override bool Success()
    {
      foreach (KeyValuePair<int, int> keyValuePair in ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().cyclesRocketDupeMoraleAboveRequirement)
      {
        if (keyValuePair.Value >= this.numberOfCycles)
          return true;
      }
      return false;
    }
  }
}
