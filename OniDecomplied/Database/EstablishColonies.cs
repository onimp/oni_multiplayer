// Decompiled with JetBrains decompiler
// Type: Database.EstablishColonies
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class EstablishColonies : VictoryColonyAchievementRequirement
  {
    public static int BASE_COUNT = 5;

    public override string GetProgress(bool complete) => ((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ESTABLISH_COLONIES).Replace("{goalBaseCount}", EstablishColonies.BASE_COUNT.ToString()).Replace("{baseCount}", this.GetColonyCount().ToString()).Replace("{neededCount}", EstablishColonies.BASE_COUNT.ToString());

    public override string Description() => this.GetProgress(this.Success());

    public override bool Success() => this.GetColonyCount() >= EstablishColonies.BASE_COUNT;

    public override string Name() => (string) COLONY_ACHIEVEMENTS.STUDY_ARTIFACTS.REQUIREMENTS.SEVERAL_COLONIES;

    private int GetColonyCount()
    {
      int colonyCount = 0;
      for (int idx = 0; idx < Components.Telepads.Count; ++idx)
      {
        Activatable component = ((Component) Components.Telepads[idx]).GetComponent<Activatable>();
        if (Object.op_Equality((Object) component, (Object) null) || component.IsActivated)
          ++colonyCount;
      }
      return colonyCount;
    }
  }
}
