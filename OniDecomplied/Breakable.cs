// Decompiled with JetBrains decompiler
// Type: Breakable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Breakable")]
public class Breakable : Workable
{
  private const float TIME_TO_BREAK_AT_FULL_HEALTH = 20f;
  private Notification notification;
  private float secondsPerTenPercentDamage = float.PositiveInfinity;
  private float elapsedDamageTime;
  private int tenPercentDamage = int.MaxValue;
  [MyCmpGet]
  private BuildingHP hp;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.showProgressBar = false;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_break_kanim"))
    };
    this.SetWorkTime(float.PositiveInfinity);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    Components.Breakables.Add(this);
  }

  public bool isBroken() => Object.op_Equality((Object) this.hp, (Object) null) || this.hp.HitPoints <= 0;

  public Notification CreateDamageNotification()
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    return new Notification((string) BUILDING.STATUSITEMS.ANGERDAMAGE.NOTIFICATION, NotificationType.BadMinor, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) BUILDING.STATUSITEMS.ANGERDAMAGE.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false)), (object) component.GetProperName(), false);
  }

  private static string ToolTipResolver(List<Notification> notificationList, object data)
  {
    string str = "";
    for (int index = 0; index < notificationList.Count; ++index)
    {
      Notification notification = notificationList[index];
      str += (string) notification.tooltipData;
      if (index < notificationList.Count - 1)
        str += "\n";
    }
    return string.Format((string) BUILDING.STATUSITEMS.ANGERDAMAGE.NOTIFICATION_TOOLTIP, (object) str);
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    this.secondsPerTenPercentDamage = 2f;
    this.tenPercentDamage = Mathf.CeilToInt((float) this.hp.MaxHitPoints * 0.1f);
    ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.AngerDamage, (object) this);
    this.notification = this.CreateDamageNotification();
    ((Component) this).gameObject.AddOrGet<Notifier>().Add(this.notification);
    this.elapsedDamageTime = 0.0f;
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if ((double) this.elapsedDamageTime >= (double) this.secondsPerTenPercentDamage)
    {
      this.elapsedDamageTime -= this.elapsedDamageTime;
      this.Trigger(-794517298, (object) new BuildingHP.DamageSourceInfo()
      {
        damage = this.tenPercentDamage,
        source = (string) BUILDINGS.DAMAGESOURCES.MINION_DESTRUCTION,
        popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.MINION_DESTRUCTION
      });
    }
    this.elapsedDamageTime += dt;
    return this.hp.HitPoints <= 0;
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.AngerDamage);
    ((Component) this).gameObject.AddOrGet<Notifier>().Remove(this.notification);
    if (!Object.op_Inequality((Object) worker, (Object) null))
      return;
    worker.Trigger(-1734580852, (object) null);
  }

  public override bool InstantlyFinish(Worker worker) => false;

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Components.Breakables.Remove(this);
  }
}
