// Decompiled with JetBrains decompiler
// Type: Database.CureDisease
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class CureDisease : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    public override bool Success() => Game.Instance.savedInfo.curedDisease;

    public void Deserialize(IReader reader)
    {
    }

    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CURED_DISEASE;
  }
}
