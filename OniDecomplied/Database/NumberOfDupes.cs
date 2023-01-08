// Decompiled with JetBrains decompiler
// Type: Database.NumberOfDupes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class NumberOfDupes : 
    VictoryColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private int numDupes;

    public override string Name() => string.Format((string) COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_DUPLICANTS, (object) this.numDupes);

    public override string Description() => string.Format((string) COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_DUPLICANTS_DESCRIPTION, (object) this.numDupes);

    public NumberOfDupes(int num) => this.numDupes = num;

    public override bool Success() => Components.LiveMinionIdentities.Items.Count >= this.numDupes;

    public void Deserialize(IReader reader) => this.numDupes = reader.ReadInt32();

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.POPULATION, (object) (complete ? this.numDupes : Components.LiveMinionIdentities.Items.Count), (object) this.numDupes);
  }
}
