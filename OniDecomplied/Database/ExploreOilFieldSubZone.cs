// Decompiled with JetBrains decompiler
// Type: Database.ExploreOilFieldSubZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class ExploreOilFieldSubZone : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    public override bool Success() => Game.Instance.savedInfo.discoveredOilField;

    public void Deserialize(IReader reader)
    {
    }

    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ENTER_OIL_BIOME;
  }
}
