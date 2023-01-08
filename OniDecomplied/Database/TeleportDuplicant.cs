// Decompiled with JetBrains decompiler
// Type: Database.TeleportDuplicant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class TeleportDuplicant : ColonyAchievementRequirement
  {
    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TELEPORT_DUPLICANT;

    public override bool Success()
    {
      foreach (WarpReceiver warpReceiver in Components.WarpReceivers.Items)
      {
        if (warpReceiver.Used)
          return true;
      }
      return false;
    }
  }
}
