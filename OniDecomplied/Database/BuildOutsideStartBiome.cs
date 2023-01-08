// Decompiled with JetBrains decompiler
// Type: Database.BuildOutsideStartBiome
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using ProcGen;
using STRINGS;
using UnityEngine;

namespace Database
{
  public class BuildOutsideStartBiome : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    public override bool Success()
    {
      WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
      foreach (BuildingComplete buildingComplete in Components.BuildingCompletes.Items)
      {
        if (!((Component) buildingComplete).GetComponent<KPrefabID>().HasTag(GameTags.TemplateBuilding))
        {
          for (int index = 0; index < clusterDetailSave.overworldCells.Count; ++index)
          {
            WorldDetailSave.OverworldCell overworldCell = clusterDetailSave.overworldCells[index];
            if (overworldCell.tags != null && !overworldCell.tags.Contains(WorldGenTags.StartWorld) && overworldCell.poly.PointInPolygon(Vector2.op_Implicit(TransformExtensions.GetPosition(buildingComplete.transform))))
            {
              Game.Instance.unlocks.Unlock("buildoutsidestartingbiome");
              return true;
            }
          }
        }
      }
      return false;
    }

    public void Deserialize(IReader reader)
    {
    }

    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_OUTSIDE_START;
  }
}
