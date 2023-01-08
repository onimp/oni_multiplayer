// Decompiled with JetBrains decompiler
// Type: Database.RadBoltTravelDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class RadBoltTravelDistance : ColonyAchievementRequirement
  {
    private int travelDistance;

    public RadBoltTravelDistance(int travelDistance) => this.travelDistance = travelDistance;

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.RADBOLT_TRAVEL, (object) ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().radBoltTravelDistance, (object) this.travelDistance);

    public override bool Success() => (double) ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().radBoltTravelDistance > (double) this.travelDistance;
  }
}
