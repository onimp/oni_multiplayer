// Decompiled with JetBrains decompiler
// Type: Database.MonumentBuilt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class MonumentBuilt : 
    VictoryColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    public override string Name() => (string) COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.BUILT_MONUMENT;

    public override string Description() => (string) COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.BUILT_MONUMENT_DESCRIPTION;

    public override bool Success()
    {
      foreach (MonumentPart monumentPart in Components.MonumentParts)
      {
        if (monumentPart.IsMonumentCompleted())
        {
          Game.Instance.unlocks.Unlock("thriving");
          return true;
        }
      }
      return false;
    }

    public void Deserialize(IReader reader)
    {
    }

    public override string GetProgress(bool complete) => this.Name();
  }
}
