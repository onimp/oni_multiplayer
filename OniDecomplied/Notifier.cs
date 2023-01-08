// Decompiled with JetBrains decompiler
// Type: Notifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Notifier")]
public class Notifier : KMonoBehaviour
{
  [MyCmpGet]
  private KSelectable Selectable;
  public Action<Notification> OnAdd;
  public Action<Notification> OnRemove;
  public bool DisableNotifications;
  public bool AutoClickFocus = true;

  protected virtual void OnPrefabInit() => Components.Notifiers.Add(this);

  protected virtual void OnCleanUp() => Components.Notifiers.Remove(this);

  public void Add(Notification notification, string suffix = "")
  {
    if (Object.op_Equality((Object) KScreenManager.Instance, (Object) null) || this.DisableNotifications || DebugHandler.NotificationsDisabled)
      return;
    DebugUtil.DevAssert(notification != null, "Trying to add null notification. It's safe to continue playing, the notification won't be displayed.", (Object) null);
    if (notification == null)
      return;
    if (Object.op_Equality((Object) notification.Notifier, (Object) null))
    {
      notification.NotifierName = !Object.op_Inequality((Object) this.Selectable, (Object) null) ? "• " + ((Object) this).name + suffix : "• " + this.Selectable.GetName() + suffix;
      notification.Notifier = this;
      if (this.AutoClickFocus && Object.op_Equality((Object) notification.clickFocus, (Object) null))
        notification.clickFocus = this.transform;
      if (this.OnAdd != null)
        this.OnAdd(notification);
      notification.GameTime = Time.time;
    }
    else
      DebugUtil.Assert(Object.op_Equality((Object) notification.Notifier, (Object) this));
    notification.Time = KTime.Instance.UnscaledGameTime;
  }

  public void Remove(Notification notification)
  {
    if (notification == null || !Object.op_Inequality((Object) notification.Notifier, (Object) null))
      return;
    notification.Notifier = (Notifier) null;
    if (this.OnRemove == null)
      return;
    this.OnRemove(notification);
  }
}
