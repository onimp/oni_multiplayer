// Decompiled with JetBrains decompiler
// Type: ManagementScreenNotificationOverlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ManagementScreenNotificationOverlay : KMonoBehaviour
{
  public Action currentMenu;
  public NotificationAlertBar alertBarPrefab;
  public RectTransform alertContainer;
  private List<NotificationAlertBar> alertBars = new List<NotificationAlertBar>();

  protected void OnEnable()
  {
  }

  protected void OnDisable()
  {
  }

  private NotificationAlertBar CreateAlertBar(ManagementMenuNotification notification)
  {
    NotificationAlertBar alertBar = Util.KInstantiateUI<NotificationAlertBar>(((Component) this.alertBarPrefab).gameObject, ((Component) this.alertContainer).gameObject, false);
    alertBar.Init(notification);
    ((Component) alertBar).gameObject.SetActive(true);
    return alertBar;
  }

  private void NotificationsChanged()
  {
  }
}
