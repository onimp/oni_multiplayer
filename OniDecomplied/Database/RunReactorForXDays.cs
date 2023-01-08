// Decompiled with JetBrains decompiler
// Type: Database.RunReactorForXDays
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class RunReactorForXDays : ColonyAchievementRequirement
  {
    private int numCycles;

    public RunReactorForXDays(int numCycles) => this.numCycles = numCycles;

    public override string GetProgress(bool complete)
    {
      int num = 0;
      foreach (Reactor reactor in Components.NuclearReactors.Items)
      {
        if (reactor.numCyclesRunning > num)
          num = reactor.numCyclesRunning;
      }
      return string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.RUN_A_REACTOR, (object) (complete ? this.numCycles : num), (object) this.numCycles);
    }

    public override bool Success()
    {
      foreach (Reactor reactor in Components.NuclearReactors.Items)
      {
        if (reactor.numCyclesRunning >= this.numCycles)
          return true;
      }
      return false;
    }
  }
}
