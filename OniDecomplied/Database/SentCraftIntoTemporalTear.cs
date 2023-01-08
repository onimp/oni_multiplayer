// Decompiled with JetBrains decompiler
// Type: Database.SentCraftIntoTemporalTear
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class SentCraftIntoTemporalTear : VictoryColonyAchievementRequirement
  {
    public override string Name() => string.Format((string) COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.REQUIREMENTS.REACHED_SPACE_DESTINATION, (object) UI.SPACEDESTINATIONS.WORMHOLE.NAME);

    public override string Description() => string.Format((string) COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.REQUIREMENTS.REACHED_SPACE_DESTINATION_DESCRIPTION, (object) UI.SPACEDESTINATIONS.WORMHOLE.NAME);

    public override string GetProgress(bool completed) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.LAUNCHED_ROCKET_TO_WORMHOLE;

    public override bool Success() => ClusterManager.Instance.GetClusterPOIManager().HasTemporalTearConsumedCraft();
  }
}
