// Decompiled with JetBrains decompiler
// Type: Database.LaunchedCraft
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class LaunchedCraft : ColonyAchievementRequirement
  {
    public override string GetProgress(bool completed) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.LAUNCHED_ROCKET;

    public override bool Success()
    {
      foreach (Clustercraft clustercraft in Components.Clustercrafts)
      {
        if (clustercraft.Status == Clustercraft.CraftStatus.InFlight)
          return true;
      }
      return false;
    }
  }
}
