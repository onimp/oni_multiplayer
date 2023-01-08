// Decompiled with JetBrains decompiler
// Type: Database.ActivateLorePOI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class ActivateLorePOI : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    public void Deserialize(IReader reader)
    {
    }

    public override bool Success()
    {
      foreach (BuildingComplete buildingComplete in Components.TemplateBuildings.Items)
      {
        if (!Object.op_Equality((Object) buildingComplete, (Object) null))
        {
          Unsealable component = ((Component) buildingComplete).GetComponent<Unsealable>();
          if (Object.op_Inequality((Object) component, (Object) null) && component.unsealed)
            return true;
        }
      }
      return false;
    }

    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.INVESTIGATE_A_POI;
  }
}
