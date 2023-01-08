// Decompiled with JetBrains decompiler
// Type: Database.TuneUpGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

namespace Database
{
  public class TuneUpGenerator : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private float numChoreseToComplete;
    private float choresCompleted;

    public TuneUpGenerator(float numChoreseToComplete) => this.numChoreseToComplete = numChoreseToComplete;

    public override bool Success()
    {
      float num = 0.0f;
      ReportManager.ReportEntry entry1 = ReportManager.Instance.TodaysReport.GetEntry(ReportManager.ReportType.ChoreStatus);
      for (int index = 0; index < entry1.contextEntries.Count; ++index)
      {
        ReportManager.ReportEntry contextEntry = entry1.contextEntries[index];
        if (contextEntry.context == Db.Get().ChoreTypes.PowerTinker.Name)
          num += contextEntry.Negative;
      }
      string name = Db.Get().ChoreTypes.PowerTinker.Name;
      int count1 = ReportManager.Instance.reports.Count;
      for (int index1 = 0; index1 < count1; ++index1)
      {
        ReportManager.ReportEntry entry2 = ReportManager.Instance.reports[index1].GetEntry(ReportManager.ReportType.ChoreStatus);
        int count2 = entry2.contextEntries.Count;
        for (int index2 = 0; index2 < count2; ++index2)
        {
          ReportManager.ReportEntry contextEntry = entry2.contextEntries[index2];
          if (contextEntry.context == name)
            num += contextEntry.Negative;
        }
      }
      this.choresCompleted = Math.Abs(num);
      return (double) Math.Abs(num) >= (double) this.numChoreseToComplete;
    }

    public void Deserialize(IReader reader) => this.numChoreseToComplete = reader.ReadSingle();

    public override string GetProgress(bool complete) => string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CHORES_OF_TYPE, (object) (float) (complete ? (double) this.numChoreseToComplete : (double) this.choresCompleted), (object) this.numChoreseToComplete, (object) Db.Get().ChoreTypes.PowerTinker.Name);
  }
}
