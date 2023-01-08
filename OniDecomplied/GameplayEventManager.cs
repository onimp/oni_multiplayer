// Decompiled with JetBrains decompiler
// Type: GameplayEventManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayEventManager : KMonoBehaviour
{
  public static GameplayEventManager Instance;
  public Notifier notifier;
  [Serialize]
  private List<GameplayEventInstance> activeEvents = new List<GameplayEventInstance>();
  [Serialize]
  private Dictionary<HashedString, int> pastEvents = new Dictionary<HashedString, int>();

  public static void DestroyInstance() => GameplayEventManager.Instance = (GameplayEventManager) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    GameplayEventManager.Instance = this;
    this.notifier = ((Component) this).GetComponent<Notifier>();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    GameScheduler.Instance.ScheduleNextFrame(nameof (GameplayEventManager), (Action<object>) (obj => this.RestoreEvents()));
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    GameplayEventManager.Instance = (GameplayEventManager) null;
  }

  private void RestoreEvents()
  {
    this.activeEvents.RemoveAll((Predicate<GameplayEventInstance>) (x => Db.Get().GameplayEvents.TryGet(x.eventID) == null));
    foreach (GameplayEventInstance activeEvent in this.activeEvents)
      this.StartEventInstance(activeEvent);
  }

  public bool IsGameplayEventActive(GameplayEvent eventType) => this.activeEvents.Find((Predicate<GameplayEventInstance>) (e => HashedString.op_Equality(e.eventID, eventType.IdHash))) != null;

  public bool IsGameplayEventRunningWithTag(Tag tag)
  {
    foreach (GameplayEventInstance activeEvent in this.activeEvents)
    {
      if (activeEvent.tags.Contains(tag))
        return true;
    }
    return false;
  }

  public void GetActiveEventsOfType<T>(int worldID, ref List<GameplayEventInstance> results) where T : GameplayEvent
  {
    foreach (GameplayEventInstance activeEvent in this.activeEvents)
    {
      if (activeEvent.worldId == worldID && (object) (activeEvent.gameplayEvent as T) != null)
        results.Add(activeEvent);
    }
  }

  private GameplayEventInstance CreateGameplayEvent(GameplayEvent gameplayEvent, int worldId) => gameplayEvent.CreateInstance(worldId);

  public GameplayEventInstance GetGameplayEventInstance(HashedString eventID, int worldId = -1) => this.activeEvents.Find((Predicate<GameplayEventInstance>) (e =>
  {
    if (!HashedString.op_Equality(e.eventID, eventID))
      return false;
    return worldId == -1 || e.worldId == worldId;
  }));

  public GameplayEventInstance CreateOrGetEventInstance(GameplayEvent eventType, int worldId = -1) => this.GetGameplayEventInstance(HashedString.op_Implicit(eventType.Id), worldId) ?? this.StartNewEvent(eventType, worldId);

  public GameplayEventInstance StartNewEvent(GameplayEvent eventType, int worldId = -1)
  {
    GameplayEventInstance gameplayEvent = this.CreateGameplayEvent(eventType, worldId);
    this.StartEventInstance(gameplayEvent);
    this.activeEvents.Add(gameplayEvent);
    int num;
    this.pastEvents.TryGetValue(gameplayEvent.eventID, out num);
    this.pastEvents[gameplayEvent.eventID] = num + 1;
    return gameplayEvent;
  }

  private void StartEventInstance(GameplayEventInstance gameplayEventInstance)
  {
    gameplayEventInstance.PrepareEvent(this).OnStop += (Action<string, StateMachine.Status>) ((reason, status) => this.activeEvents.Remove(gameplayEventInstance));
    gameplayEventInstance.StartEvent();
  }

  public int NumberOfPastEvents(HashedString eventID)
  {
    int num;
    this.pastEvents.TryGetValue(eventID, out num);
    return num;
  }

  public static Notification CreateStandardCancelledNotification(EventInfoData eventInfoData)
  {
    if (eventInfoData == null)
    {
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) "eventPopup is null in CreateStandardCancelledNotification"
      });
      return (Notification) null;
    }
    eventInfoData.FinalizeText();
    return new Notification(string.Format((string) GAMEPLAY_EVENTS.CANCELED, (object) eventInfoData.title), NotificationType.Event, (Func<List<Notification>, object, string>) ((list, data) => string.Format((string) GAMEPLAY_EVENTS.CANCELED_TOOLTIP, (object) eventInfoData.title)));
  }
}
