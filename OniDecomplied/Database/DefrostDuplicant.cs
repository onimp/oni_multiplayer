// Decompiled with JetBrains decompiler
// Type: Database.DefrostDuplicant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class DefrostDuplicant : ColonyAchievementRequirement
  {
    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.DEFROST_DUPLICANT;

    public override bool Success() => ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().defrostedDuplicant;
  }
}
