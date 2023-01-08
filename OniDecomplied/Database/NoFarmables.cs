// Decompiled with JetBrains decompiler
// Type: Database.NoFarmables
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
  public class NoFarmables : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    public override bool Success()
    {
      foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
      {
        foreach (PlantablePlot plantablePlot in Components.PlantablePlots.GetItems(worldContainer.id))
        {
          if (Object.op_Inequality((Object) plantablePlot.Occupant, (Object) null))
          {
            foreach (Tag depositObjectTag in (IEnumerable<Tag>) plantablePlot.possibleDepositObjectTags)
            {
              if (Tag.op_Inequality(depositObjectTag, GameTags.DecorSeed))
                return false;
            }
          }
        }
      }
      return true;
    }

    public override bool Fail() => !this.Success();

    public void Deserialize(IReader reader)
    {
    }

    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.NO_FARM_TILES;
  }
}
