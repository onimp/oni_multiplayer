// Decompiled with JetBrains decompiler
// Type: GameplayEventInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class GameplayEventInstance : ISaveLoadable
{
  [Serialize]
  public readonly HashedString eventID;
  [Serialize]
  public List<Tag> tags;
  [Serialize]
  public float eventStartTime;
  [Serialize]
  public readonly int worldId;
  [Serialize]
  private bool _seenNotification;
  public List<GameObject> monitorCallbackObjects;
  public GameplayEventInstance.GameplayEventPopupDataCallback GetEventPopupData;
  private GameplayEvent _gameplayEvent;

  public StateMachine.Instance smi { get; private set; }

  public bool seenNotification
  {
    get => this._seenNotification;
    set
    {
      this._seenNotification = value;
      this.monitorCallbackObjects.ForEach((Action<GameObject>) (x => EventExtensions.Trigger(x, -1122598290, (object) this)));
    }
  }

  public GameplayEvent gameplayEvent
  {
    get
    {
      if (this._gameplayEvent == null)
        this._gameplayEvent = Db.Get().GameplayEvents.TryGet(this.eventID);
      return this._gameplayEvent;
    }
  }

  public GameplayEventInstance(GameplayEvent gameplayEvent, int worldId)
  {
    this.eventID = HashedString.op_Implicit(gameplayEvent.Id);
    this.tags = new List<Tag>();
    this.eventStartTime = GameUtil.GetCurrentTimeInCycles();
    this.worldId = worldId;
  }

  public StateMachine.Instance PrepareEvent(GameplayEventManager manager)
  {
    this.smi = this.gameplayEvent.GetSMI(manager, this);
    return this.smi;
  }

  public void StartEvent()
  {
    GameplayEventManager.Instance.Trigger(1491341646, (object) this);
    this.smi.OnStop += new Action<string, StateMachine.Status>(this.OnStop);
    this.smi.StartSM();
  }

  public void RegisterMonitorCallback(GameObject go)
  {
    if (this.monitorCallbackObjects == null)
      this.monitorCallbackObjects = new List<GameObject>();
    if (this.monitorCallbackObjects.Contains(go))
      return;
    this.monitorCallbackObjects.Add(go);
  }

  public void UnregisterMonitorCallback(GameObject go)
  {
    if (this.monitorCallbackObjects == null)
      this.monitorCallbackObjects = new List<GameObject>();
    this.monitorCallbackObjects.Remove(go);
  }

  public void OnStop(string reason, StateMachine.Status status)
  {
    GameplayEventManager.Instance.Trigger(1287635015, (object) this);
    if (this.monitorCallbackObjects != null)
      this.monitorCallbackObjects.ForEach((Action<GameObject>) (x => EventExtensions.Trigger(x, 1287635015, (object) this)));
    switch (status)
    {
      case StateMachine.Status.Failed:
        using (List<HashedString>.Enumerator enumerator = this.gameplayEvent.failureEvents.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            HashedString current = enumerator.Current;
            GameplayEvent eventType = Db.Get().GameplayEvents.TryGet(current);
            DebugUtil.DevAssert(eventType != null, string.Format("GameplayEvent {0} is null", (object) current), (Object) null);
            if (eventType != null && eventType.IsAllowed())
              GameplayEventManager.Instance.StartNewEvent(eventType);
          }
          break;
        }
      case StateMachine.Status.Success:
        using (List<HashedString>.Enumerator enumerator = this.gameplayEvent.successEvents.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            HashedString current = enumerator.Current;
            GameplayEvent eventType = Db.Get().GameplayEvents.TryGet(current);
            DebugUtil.DevAssert(eventType != null, string.Format("GameplayEvent {0} is null", (object) current), (Object) null);
            if (eventType != null && eventType.IsAllowed())
              GameplayEventManager.Instance.StartNewEvent(eventType);
          }
          break;
        }
    }
  }

  public float AgeInCycles() => GameUtil.GetCurrentTimeInCycles() - this.eventStartTime;

  public delegate EventInfoData GameplayEventPopupDataCallback();
}
