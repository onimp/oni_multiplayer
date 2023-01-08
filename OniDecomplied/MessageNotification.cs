// Decompiled with JetBrains decompiler
// Type: MessageNotification
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageNotification : Notification
{
  public Message message;

  private string OnToolTip(List<Notification> notifications, string tooltipText) => tooltipText;

  public MessageNotification(Message m)
    : base(m.GetTitle(), NotificationType.Messages, expires: false, show_dismiss_button: true)
  {
    MessageNotification messageNotification = this;
    this.message = m;
    if (!this.message.PlayNotificationSound())
      this.playSound = false;
    this.ToolTip = (Func<List<Notification>, object, string>) ((notifications, data) => messageNotification.OnToolTip(notifications, m.GetTooltip()));
    this.clickFocus = (Transform) null;
  }
}
