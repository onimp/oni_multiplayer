// Decompiled with JetBrains decompiler
// Type: Database.BuildALaunchPad
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class BuildALaunchPad : ColonyAchievementRequirement
  {
    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILD_A_LAUNCHPAD;

    public override bool Success()
    {
      foreach (KMonoBehaviour component in Components.LaunchPads.Items)
      {
        WorldContainer myWorld = component.GetMyWorld();
        if (!myWorld.IsStartWorld && Components.WarpReceivers.GetWorldItems(myWorld.id).Count == 0)
          return true;
      }
      return false;
    }
  }
}
