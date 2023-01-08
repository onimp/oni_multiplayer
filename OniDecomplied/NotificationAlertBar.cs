// Decompiled with JetBrains decompiler
// Type: NotificationAlertBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationAlertBar : KMonoBehaviour
{
  public ManagementMenuNotification notification;
  public KButton thisButton;
  public KImage background;
  public LocText text;
  public ToolTip tooltip;
  public KButton muteButton;
  public List<ColorStyleSetting> alertColorStyle;

  public void Init(ManagementMenuNotification notification)
  {
    this.notification = notification;
    this.thisButton.onClick += new System.Action(this.OnThisButtonClicked);
    this.background.colorStyleSetting = this.alertColorStyle[(int) notification.valence];
    this.background.ApplyColorStyleSetting();
    ((TMP_Text) this.text).text = notification.titleText;
    this.tooltip.SetSimpleTooltip(notification.ToolTip((List<Notification>) null, notification.tooltipData));
    this.muteButton.onClick += new System.Action(this.OnMuteButtonClicked);
  }

  private void OnThisButtonClicked()
  {
    NotificationHighlightController componentInParent = ((Component) this).GetComponentInParent<NotificationHighlightController>();
    if (Object.op_Inequality((Object) componentInParent, (Object) null))
      componentInParent.SetActiveTarget(this.notification);
    else
      this.notification.View();
  }

  private void OnMuteButtonClicked()
  {
  }
}
