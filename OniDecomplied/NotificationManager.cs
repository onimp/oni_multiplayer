// Decompiled with JetBrains decompiler
// Type: NotificationManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : KMonoBehaviour
{
  private List<Notification> pendingNotifications = new List<Notification>();
  private List<Notification> notifications = new List<Notification>();

  public static NotificationManager Instance { get; private set; }

  public event Action<Notification> notificationAdded;

  public event Action<Notification> notificationRemoved;

  protected virtual void OnPrefabInit()
  {
    Debug.Assert(Object.op_Equality((Object) NotificationManager.Instance, (Object) null));
    NotificationManager.Instance = this;
    Components.Notifiers.OnAdd += new Action<Notifier>(this.OnAddNotifier);
    Components.Notifiers.OnRemove += new Action<Notifier>(this.OnRemoveNotifier);
    foreach (Notifier notifier in Components.Notifiers.Items)
      this.OnAddNotifier(notifier);
  }

  protected virtual void OnForcedCleanUp() => NotificationManager.Instance = (NotificationManager) null;

  private void OnAddNotifier(Notifier notifier)
  {
    notifier.OnAdd += new Action<Notification>(this.OnAddNotification);
    notifier.OnRemove += new Action<Notification>(this.OnRemoveNotification);
  }

  private void OnRemoveNotifier(Notifier notifier)
  {
    notifier.OnAdd -= new Action<Notification>(this.OnAddNotification);
    notifier.OnRemove -= new Action<Notification>(this.OnRemoveNotification);
  }

  private void OnAddNotification(Notification notification) => this.pendingNotifications.Add(notification);

  private void OnRemoveNotification(Notification notification)
  {
    this.pendingNotifications.Remove(notification);
    if (!this.notifications.Remove(notification))
      return;
    this.notificationRemoved(notification);
  }

  private void Update()
  {
    int index = 0;
    while (index < this.pendingNotifications.Count)
    {
      if (this.pendingNotifications[index].IsReady())
      {
        this.DoAddNotification(this.pendingNotifications[index]);
        this.pendingNotifications.RemoveAt(index);
      }
      else
        ++index;
    }
  }

  private void DoAddNotification(Notification notification)
  {
    this.notifications.Add(notification);
    if (this.notificationAdded == null)
      return;
    this.notificationAdded(notification);
  }
}
