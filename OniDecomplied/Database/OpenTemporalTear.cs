// Decompiled with JetBrains decompiler
// Type: Database.OpenTemporalTear
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class OpenTemporalTear : VictoryColonyAchievementRequirement
  {
    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.OPEN_TEMPORAL_TEAR;

    public override string Description() => this.GetProgress(this.Success());

    public override bool Success() => ((Component) ClusterManager.Instance).GetComponent<ClusterPOIManager>().IsTemporalTearOpen();

    public override string Name() => (string) COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.REQUIREMENTS.OPEN_TEMPORAL_TEAR;
  }
}
