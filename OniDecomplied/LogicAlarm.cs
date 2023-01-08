// Decompiled with JetBrains decompiler
// Type: LogicAlarm
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/LogicAlarm")]
public class LogicAlarm : KMonoBehaviour, ISaveLoadable
{
  [Serialize]
  public string notificationName;
  [Serialize]
  public string notificationTooltip;
  [Serialize]
  public NotificationType notificationType;
  [Serialize]
  public bool pauseOnNotify;
  [Serialize]
  public bool zoomOnNotify;
  [Serialize]
  public float cooldown;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private bool wasOn;
  private Notifier notifier;
  private Notification notification;
  private Notification lastNotificationCreated;
  private static readonly EventSystem.IntraObjectHandler<LogicAlarm> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicAlarm>((Action<LogicAlarm, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly EventSystem.IntraObjectHandler<LogicAlarm> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicAlarm>((Action<LogicAlarm, object>) ((component, data) => component.OnLogicValueChanged(data)));
  public static readonly HashedString INPUT_PORT_ID = new HashedString("LogicAlarmInput");
  protected static readonly HashedString[] ON_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("on_pre"),
    HashedString.op_Implicit("on_loop")
  };
  protected static readonly HashedString[] OFF_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("on_pst"),
    HashedString.op_Implicit("off")
  };

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<LogicAlarm>(-905833192, LogicAlarm.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    LogicAlarm component = ((GameObject) data).GetComponent<LogicAlarm>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.notificationName = component.notificationName;
    this.notificationType = component.notificationType;
    this.pauseOnNotify = component.pauseOnNotify;
    this.zoomOnNotify = component.zoomOnNotify;
    this.cooldown = component.cooldown;
    this.notificationTooltip = component.notificationTooltip;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.notifier = ((Component) this).gameObject.AddComponent<Notifier>();
    this.Subscribe<LogicAlarm>(-801688580, LogicAlarm.OnLogicValueChangedDelegate);
    if (string.IsNullOrEmpty(this.notificationName))
      this.notificationName = (string) UI.UISIDESCREENS.LOGICALARMSIDESCREEN.NAME_DEFAULT;
    if (string.IsNullOrEmpty(this.notificationTooltip))
      this.notificationTooltip = (string) UI.UISIDESCREENS.LOGICALARMSIDESCREEN.TOOLTIP_DEFAULT;
    this.UpdateVisualState();
    this.UpdateNotification(false);
  }

  private void UpdateVisualState() => ((Component) this).GetComponent<KBatchedAnimController>().Play(this.wasOn ? LogicAlarm.ON_ANIMS : LogicAlarm.OFF_ANIMS);

  public void OnLogicValueChanged(object data)
  {
    LogicValueChanged logicValueChanged = (LogicValueChanged) data;
    if (HashedString.op_Inequality(logicValueChanged.portID, LogicAlarm.INPUT_PORT_ID))
      return;
    if (LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue))
    {
      if (this.wasOn)
        return;
      this.PushNotification();
      this.wasOn = true;
      if (this.pauseOnNotify && !SpeedControlScreen.Instance.IsPaused)
        SpeedControlScreen.Instance.Pause(false);
      if (this.zoomOnNotify)
        CameraController.Instance.ActiveWorldStarWipe(((Component) this).gameObject.GetMyWorldId(), TransformExtensions.GetPosition(this.transform), 8f);
      this.UpdateVisualState();
    }
    else
    {
      if (!this.wasOn)
        return;
      this.wasOn = false;
      this.UpdateVisualState();
    }
  }

  private void PushNotification()
  {
    this.notification.Clear();
    this.notifier.Add(this.notification);
  }

  public void UpdateNotification(bool clear)
  {
    if (this.notification != null & clear)
    {
      this.notification.Clear();
      this.lastNotificationCreated = (Notification) null;
    }
    if (this.notification == this.lastNotificationCreated && this.lastNotificationCreated != null)
      return;
    this.notification = this.CreateNotification();
  }

  public Notification CreateNotification()
  {
    ((Component) this).GetComponent<KSelectable>();
    Notification notification = new Notification(this.notificationName, this.notificationType, (Func<List<Notification>, object, string>) ((n, d) => this.notificationTooltip), volume_attenuation: false);
    this.lastNotificationCreated = notification;
    return notification;
  }
}
