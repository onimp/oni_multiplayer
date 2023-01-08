// Decompiled with JetBrains decompiler
// Type: Bed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Bed")]
public class Bed : Workable, IGameObjectEffectDescriptor, IBasicBuilding
{
  [MyCmpReq]
  private Sleepable sleepable;
  private Worker targetWorker;
  public string[] effects;
  private static Dictionary<string, string> roomSleepingEffects = new Dictionary<string, string>()
  {
    {
      "Barracks",
      "BarracksStamina"
    },
    {
      "Luxury Barracks",
      "BarracksStamina"
    },
    {
      "Private Bedroom",
      "BedroomStamina"
    }
  };

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.showProgressBar = false;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    Components.BasicBuildings.Add((IBasicBuilding) this);
    this.sleepable = ((Component) this).GetComponent<Sleepable>();
    Sleepable sleepable = this.sleepable;
    sleepable.OnWorkableEventCB = sleepable.OnWorkableEventCB + new Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent);
  }

  private void OnWorkableEvent(Workable workable, Workable.WorkableEvent workable_event)
  {
    if (workable_event == Workable.WorkableEvent.WorkStarted)
    {
      this.AddEffects();
    }
    else
    {
      if (workable_event != Workable.WorkableEvent.WorkStopped)
        return;
      this.RemoveEffects();
    }
  }

  private void AddEffects()
  {
    this.targetWorker = this.sleepable.worker;
    if (this.effects != null)
    {
      foreach (string effect in this.effects)
        ((Component) this.targetWorker).GetComponent<Effects>().Add(effect, false);
    }
    Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(((Component) this).gameObject);
    if (roomOfGameObject == null)
      return;
    RoomType roomType = roomOfGameObject.roomType;
    foreach (KeyValuePair<string, string> roomSleepingEffect in Bed.roomSleepingEffects)
    {
      if (roomSleepingEffect.Key == roomType.Id)
        ((Component) this.targetWorker).GetComponent<Effects>().Add(roomSleepingEffect.Value, false);
    }
    roomType.TriggerRoomEffects(((Component) this).GetComponent<KPrefabID>(), ((Component) this.targetWorker).GetComponent<Effects>());
  }

  private void RemoveEffects()
  {
    if (Object.op_Equality((Object) this.targetWorker, (Object) null))
      return;
    if (this.effects != null)
    {
      foreach (string effect in this.effects)
        ((Component) this.targetWorker).GetComponent<Effects>().Remove(effect);
    }
    foreach (KeyValuePair<string, string> roomSleepingEffect in Bed.roomSleepingEffects)
      ((Component) this.targetWorker).GetComponent<Effects>().Remove(roomSleepingEffect.Value);
    this.targetWorker = (Worker) null;
  }

  public override List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descs = new List<Descriptor>();
    if (this.effects != null)
    {
      foreach (string effect in this.effects)
      {
        if (effect != null && effect != "")
          Effect.AddModifierDescriptions(((Component) this).gameObject, descs, effect);
      }
    }
    return descs;
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Components.BasicBuildings.Remove((IBasicBuilding) this);
    if (!Object.op_Inequality((Object) this.sleepable, (Object) null))
      return;
    Sleepable sleepable = this.sleepable;
    sleepable.OnWorkableEventCB = sleepable.OnWorkableEventCB - new Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent);
  }
}
