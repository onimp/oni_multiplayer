// Decompiled with JetBrains decompiler
// Type: Database.LandOnAllWorlds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;

namespace Database
{
  public class LandOnAllWorlds : ColonyAchievementRequirement
  {
    public override string GetProgress(bool complete)
    {
      int num1 = 0;
      int num2 = 0;
      foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
      {
        if (!worldContainer.IsModuleInterior)
        {
          ++num1;
          if (worldContainer.IsDupeVisited || worldContainer.IsRoverVisted)
            ++num2;
        }
      }
      return string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.LAND_DUPES_ON_ALL_WORLDS, (object) num2, (object) num1);
    }

    public override bool Success()
    {
      foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
      {
        if (!worldContainer.IsModuleInterior && !worldContainer.IsDupeVisited && !worldContainer.IsRoverVisted)
          return false;
      }
      return true;
    }
  }
}
