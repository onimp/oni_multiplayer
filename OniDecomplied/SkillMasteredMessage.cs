// Decompiled with JetBrains decompiler
// Type: SkillMasteredMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

public class SkillMasteredMessage : Message
{
  [Serialize]
  private string minionName;

  public SkillMasteredMessage()
  {
  }

  public SkillMasteredMessage(MinionResume resume) => this.minionName = ((Component) resume).GetProperName();

  public override string GetSound() => "AI_Notification_ResearchComplete";

  public override string GetMessageBody()
  {
    Debug.Assert(this.minionName != null);
    string str = string.Format((string) MISC.NOTIFICATIONS.SKILL_POINT_EARNED.LINE, (object) this.minionName);
    return string.Format((string) MISC.NOTIFICATIONS.SKILL_POINT_EARNED.MESSAGEBODY, (object) str);
  }

  public override string GetTitle() => MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME.Replace("{Duplicant}", this.minionName);

  public override string GetTooltip() => MISC.NOTIFICATIONS.SKILL_POINT_EARNED.TOOLTIP.Replace("{Duplicant}", this.minionName);

  public override bool IsValid() => this.minionName != null;
}
