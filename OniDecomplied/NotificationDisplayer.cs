// Decompiled with JetBrains decompiler
// Type: NotificationDisplayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public abstract class NotificationDisplayer : KMonoBehaviour
{
  protected List<Notification> displayedNotifications;

  protected virtual void OnSpawn()
  {
    this.displayedNotifications = new List<Notification>();
    NotificationManager.Instance.notificationAdded += new Action<Notification>(this.NotificationAdded);
    NotificationManager.Instance.notificationRemoved += new Action<Notification>(this.NotificationRemoved);
  }

  public void NotificationAdded(Notification notification)
  {
    if (!this.ShouldDisplayNotification(notification))
      return;
    this.displayedNotifications.Add(notification);
    this.OnNotificationAdded(notification);
  }

  protected abstract void OnNotificationAdded(Notification notification);

  public void NotificationRemoved(Notification notification)
  {
    if (!this.displayedNotifications.Contains(notification))
      return;
    this.displayedNotifications.Remove(notification);
    this.OnNotificationRemoved(notification);
  }

  protected abstract void OnNotificationRemoved(Notification notification);

  protected abstract bool ShouldDisplayNotification(Notification notification);
}
