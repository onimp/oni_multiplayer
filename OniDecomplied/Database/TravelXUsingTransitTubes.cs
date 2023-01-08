// Decompiled with JetBrains decompiler
// Type: Database.TravelXUsingTransitTubes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class TravelXUsingTransitTubes : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private int distanceToTravel;
    private NavType navType;

    public TravelXUsingTransitTubes(NavType navType, int distanceToTravel)
    {
      this.navType = navType;
      this.distanceToTravel = distanceToTravel;
    }

    public override bool Success()
    {
      int num = 0;
      foreach (Component component1 in Components.MinionIdentities.Items)
      {
        Navigator component2 = component1.GetComponent<Navigator>();
        if (Object.op_Inequality((Object) component2, (Object) null) && component2.distanceTravelledByNavType.ContainsKey(this.navType))
          num += component2.distanceTravelledByNavType[this.navType];
      }
      return num >= this.distanceToTravel;
    }

    public void Deserialize(IReader reader)
    {
      this.navType = (NavType) reader.ReadByte();
      this.distanceToTravel = reader.ReadInt32();
    }

    public override string GetProgress(bool complete)
    {
      int num = 0;
      foreach (Component component1 in Components.MinionIdentities.Items)
      {
        Navigator component2 = component1.GetComponent<Navigator>();
        if (Object.op_Inequality((Object) component2, (Object) null) && component2.distanceTravelledByNavType.ContainsKey(this.navType))
          num += component2.distanceTravelledByNavType[this.navType];
      }
      return string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TRAVELED_IN_TUBES, (object) (complete ? this.distanceToTravel : num), (object) this.distanceToTravel);
    }
  }
}
