// Decompiled with JetBrains decompiler
// Type: AchievementEarnedMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class AchievementEarnedMessage : Message
{
  public override bool ShowDialog() => false;

  public override string GetSound() => "AI_Notification_ResearchComplete";

  public override string GetMessageBody() => "";

  public override string GetTitle() => (string) MISC.NOTIFICATIONS.COLONY_ACHIEVEMENT_EARNED.NAME;

  public override string GetTooltip() => (string) MISC.NOTIFICATIONS.COLONY_ACHIEVEMENT_EARNED.TOOLTIP;

  public override bool IsValid() => true;

  public override void OnClick()
  {
    RetireColonyUtility.SaveColonySummaryData();
    MainMenu.ActivateRetiredColoniesScreenFromData(((Component) ((KMonoBehaviour) PauseScreen.Instance).transform.parent).gameObject, RetireColonyUtility.GetCurrentColonyRetiredColonyData());
  }
}
