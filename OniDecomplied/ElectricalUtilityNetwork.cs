// Decompiled with JetBrains decompiler
// Type: ElectricalUtilityNetwork
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalUtilityNetwork : UtilityNetwork
{
  private Notification overloadedNotification;
  private List<Wire>[] wireGroups = new List<Wire>[5];
  public List<Wire> allWires = new List<Wire>();
  private const float MIN_OVERLOAD_TIME_FOR_DAMAGE = 6f;
  private const float MIN_OVERLOAD_NOTIFICATION_DISPLAY_TIME = 5f;
  private GameObject targetOverloadedWire;
  private float timeOverloaded;
  private float timeOverloadNotificationDisplayed;

  public override void AddItem(object item)
  {
    if (!(item.GetType() == typeof (Wire)))
      return;
    Wire wire = (Wire) item;
    Wire.WattageRating maxWattageRating = wire.MaxWattageRating;
    List<Wire> wireList = this.wireGroups[(int) maxWattageRating];
    if (wireList == null)
    {
      wireList = new List<Wire>();
      this.wireGroups[(int) maxWattageRating] = wireList;
    }
    wireList.Add(wire);
    this.allWires.Add(wire);
    this.timeOverloaded = Mathf.Max(this.timeOverloaded, wire.circuitOverloadTime);
  }

  public override void Reset(UtilityNetworkGridNode[] grid)
  {
    for (int index1 = 0; index1 < 5; ++index1)
    {
      List<Wire> wireGroup = this.wireGroups[index1];
      if (wireGroup != null)
      {
        for (int index2 = 0; index2 < wireGroup.Count; ++index2)
        {
          Wire wire = wireGroup[index2];
          if (Object.op_Inequality((Object) wire, (Object) null))
          {
            wire.circuitOverloadTime = this.timeOverloaded;
            int cell = Grid.PosToCell(TransformExtensions.GetPosition(wire.transform));
            UtilityNetworkGridNode utilityNetworkGridNode = grid[cell] with
            {
              networkIdx = -1
            };
            grid[cell] = utilityNetworkGridNode;
          }
        }
        wireGroup.Clear();
      }
    }
    this.allWires.Clear();
    this.RemoveOverloadedNotification();
  }

  public void UpdateOverloadTime(
    float dt,
    float watts_used,
    List<WireUtilityNetworkLink>[] bridgeGroups)
  {
    bool flag = false;
    List<Wire> wireList = (List<Wire>) null;
    List<WireUtilityNetworkLink> utilityNetworkLinkList = (List<WireUtilityNetworkLink>) null;
    for (int rating = 0; rating < 5; ++rating)
    {
      List<Wire> wireGroup = this.wireGroups[rating];
      List<WireUtilityNetworkLink> bridgeGroup = bridgeGroups[rating];
      float num = Wire.GetMaxWattageAsFloat((Wire.WattageRating) rating) + TUNING.POWER.FLOAT_FUDGE_FACTOR;
      if ((double) watts_used > (double) num && (bridgeGroup != null && bridgeGroup.Count > 0 || wireGroup != null && wireGroup.Count > 0))
      {
        flag = true;
        wireList = wireGroup;
        utilityNetworkLinkList = bridgeGroup;
        break;
      }
    }
    wireList?.RemoveAll((Predicate<Wire>) (x => Object.op_Equality((Object) x, (Object) null)));
    utilityNetworkLinkList?.RemoveAll((Predicate<WireUtilityNetworkLink>) (x => Object.op_Equality((Object) x, (Object) null)));
    if (flag)
    {
      this.timeOverloaded += dt;
      if ((double) this.timeOverloaded <= 6.0)
        return;
      this.timeOverloaded = 0.0f;
      if (Object.op_Equality((Object) this.targetOverloadedWire, (Object) null))
      {
        if (utilityNetworkLinkList != null && utilityNetworkLinkList.Count > 0)
        {
          int index = Random.Range(0, utilityNetworkLinkList.Count);
          this.targetOverloadedWire = ((Component) utilityNetworkLinkList[index]).gameObject;
        }
        else if (wireList != null && wireList.Count > 0)
        {
          int index = Random.Range(0, wireList.Count);
          this.targetOverloadedWire = ((Component) wireList[index]).gameObject;
        }
      }
      if (Object.op_Inequality((Object) this.targetOverloadedWire, (Object) null))
        EventExtensions.Trigger(this.targetOverloadedWire, -794517298, (object) new BuildingHP.DamageSourceInfo()
        {
          damage = 1,
          source = (string) STRINGS.BUILDINGS.DAMAGESOURCES.CIRCUIT_OVERLOADED,
          popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CIRCUIT_OVERLOADED,
          takeDamageEffect = SpawnFXHashes.BuildingSpark,
          fullDamageEffectName = "spark_damage_kanim",
          statusItemID = Db.Get().BuildingStatusItems.Overloaded.Id
        });
      if (this.overloadedNotification != null)
        return;
      this.timeOverloadNotificationDisplayed = 0.0f;
      this.overloadedNotification = new Notification((string) MISC.NOTIFICATIONS.CIRCUIT_OVERLOADED.NAME, NotificationType.BadMinor, click_focus: this.targetOverloadedWire.transform);
      GameScheduler.Instance.Schedule("Power Tutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Power)), (object) null, (SchedulerGroup) null);
      Game.Instance.FindOrAdd<Notifier>().Add(this.overloadedNotification);
    }
    else
    {
      this.timeOverloaded = Mathf.Max(0.0f, this.timeOverloaded - dt * 0.95f);
      this.timeOverloadNotificationDisplayed += dt;
      if ((double) this.timeOverloadNotificationDisplayed <= 5.0)
        return;
      this.RemoveOverloadedNotification();
    }
  }

  private void RemoveOverloadedNotification()
  {
    if (this.overloadedNotification == null)
      return;
    Game.Instance.FindOrAdd<Notifier>().Remove(this.overloadedNotification);
    this.overloadedNotification = (Notification) null;
  }

  public float GetMaxSafeWattage()
  {
    for (int rating = 0; rating < this.wireGroups.Length; ++rating)
    {
      List<Wire> wireGroup = this.wireGroups[rating];
      if ((wireGroup == null ? 0 : (wireGroup.Count > 0 ? 1 : 0)) != 0)
        return Wire.GetMaxWattageAsFloat((Wire.WattageRating) rating);
    }
    return 0.0f;
  }

  public override void RemoveItem(object item)
  {
    if (!(item.GetType() == typeof (Wire)))
      return;
    Wire wire = (Wire) item;
    wire.circuitOverloadTime = 0.0f;
    this.allWires.Remove(wire);
  }
}
