// Decompiled with JetBrains decompiler
// Type: ManagementMenuNotificationDisplayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class ManagementMenuNotificationDisplayer : NotificationDisplayer
{
  public List<ManagementMenuNotification> displayedManagementMenuNotifications { get; private set; }

  public event System.Action onNotificationsChanged;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.displayedManagementMenuNotifications = new List<ManagementMenuNotification>();
  }

  public void NotificationWasViewed(ManagementMenuNotification notification) => this.onNotificationsChanged();

  protected override void OnNotificationAdded(Notification notification)
  {
    this.displayedManagementMenuNotifications.Add(notification as ManagementMenuNotification);
    this.onNotificationsChanged();
  }

  protected override void OnNotificationRemoved(Notification notification)
  {
    this.displayedManagementMenuNotifications.Remove(notification as ManagementMenuNotification);
    this.onNotificationsChanged();
  }

  protected override bool ShouldDisplayNotification(Notification notification) => notification is ManagementMenuNotification;

  public List<ManagementMenuNotification> GetNotificationsForAction(Action hotKey)
  {
    List<ManagementMenuNotification> notificationsForAction = new List<ManagementMenuNotification>();
    foreach (ManagementMenuNotification menuNotification in this.displayedManagementMenuNotifications)
    {
      if (menuNotification.targetMenu == hotKey)
        notificationsForAction.Add(menuNotification);
    }
    return notificationsForAction;
  }
}
