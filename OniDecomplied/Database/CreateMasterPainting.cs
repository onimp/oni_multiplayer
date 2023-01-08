// Decompiled with JetBrains decompiler
// Type: Database.CreateMasterPainting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class CreateMasterPainting : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    public override bool Success()
    {
      foreach (Painting painting in Components.Paintings.Items)
      {
        if (Object.op_Inequality((Object) painting, (Object) null))
        {
          ArtableStage artableStage = Db.GetArtableStages().TryGet(painting.CurrentStage);
          if (artableStage != null && artableStage.statusItem == Db.Get().ArtableStatuses.LookingGreat)
            return true;
        }
      }
      return false;
    }

    public void Deserialize(IReader reader)
    {
    }

    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CREATE_A_PAINTING;
  }
}
