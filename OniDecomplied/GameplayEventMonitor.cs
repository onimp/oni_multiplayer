// Decompiled with JetBrains decompiler
// Type: GameplayEventMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayEventMonitor : 
  GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>
{
  public GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.State idle;
  public GameplayEventMonitor.ActiveState activeState;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    base.InitializeStates(out default_state);
    default_state = (StateMachine.BaseState) this.idle;
    this.root.EventHandler(GameHashes.GameplayEventMonitorStart, (GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.GameEvent.Callback) ((smi, data) => smi.OnMonitorStart(data))).EventHandler(GameHashes.GameplayEventMonitorEnd, (GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.GameEvent.Callback) ((smi, data) => smi.OnMonitorEnd(data))).EventHandler(GameHashes.GameplayEventMonitorChanged, (GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.GameEvent.Callback) ((smi, data) => this.UpdateFX(smi)));
    this.idle.EventTransition(GameHashes.GameplayEventMonitorStart, (GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.State) this.activeState, new StateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.Transition.ConditionCallback(this.HasEvents)).Enter(new StateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.State.Callback(this.UpdateEventDisplay));
    this.activeState.DefaultState(this.activeState.unseenEvents);
    this.activeState.unseenEvents.ToggleFX(new Func<GameplayEventMonitor.Instance, StateMachine.Instance>(this.CreateFX)).EventHandler(GameHashes.SelectObject, (GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.GameEvent.Callback) ((smi, data) => smi.OnSelect(data))).EventTransition(GameHashes.GameplayEventMonitorChanged, this.activeState.seenAllEvents, new StateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.Transition.ConditionCallback(this.SeenAll));
    this.activeState.seenAllEvents.EventTransition(GameHashes.GameplayEventMonitorStart, this.activeState.unseenEvents, GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.Not(new StateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.Transition.ConditionCallback(this.SeenAll))).Enter(new StateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.State.Callback(this.UpdateEventDisplay));
  }

  private bool HasEvents(GameplayEventMonitor.Instance smi) => smi.events.Count > 0;

  private bool SeenAll(GameplayEventMonitor.Instance smi) => smi.UnseenCount() == 0;

  private void UpdateFX(GameplayEventMonitor.Instance smi)
  {
    if (smi.fx == null)
      return;
    smi.fx.sm.notificationCount.Set(smi.UnseenCount(), smi.fx);
  }

  private GameplayEventFX.Instance CreateFX(GameplayEventMonitor.Instance smi)
  {
    if (smi.isMasterNull)
      return (GameplayEventFX.Instance) null;
    smi.fx = new GameplayEventFX.Instance(smi.master, new Vector3(0.0f, 0.0f, -0.1f));
    return smi.fx;
  }

  public void UpdateEventDisplay(GameplayEventMonitor.Instance smi)
  {
    if (smi.events.Count == 0 || smi.UnseenCount() > 0)
    {
      NameDisplayScreen.Instance.SetGameplayEventDisplay(smi.master.gameObject, false, (string) null, (Sprite) null);
    }
    else
    {
      int num = -1;
      GameplayEvent gameplayEvent = (GameplayEvent) null;
      foreach (GameplayEventInstance gameplayEventInstance in smi.events)
      {
        Sprite displaySprite = gameplayEventInstance.gameplayEvent.GetDisplaySprite();
        if (gameplayEventInstance.gameplayEvent.importance > num && Object.op_Inequality((Object) displaySprite, (Object) null))
        {
          num = gameplayEventInstance.gameplayEvent.importance;
          gameplayEvent = gameplayEventInstance.gameplayEvent;
        }
      }
      if (gameplayEvent == null)
        return;
      NameDisplayScreen.Instance.SetGameplayEventDisplay(smi.master.gameObject, true, gameplayEvent.GetDisplayString(), gameplayEvent.GetDisplaySprite());
    }
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class ActiveState : 
    GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.State
  {
    public GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.State unseenEvents;
    public GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.State seenAllEvents;
  }

  public new class Instance : 
    GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>.GameInstance
  {
    public List<GameplayEventInstance> events = new List<GameplayEventInstance>();
    public GameplayEventFX.Instance fx;

    public Instance(IStateMachineTarget master, GameplayEventMonitor.Def def)
      : base(master, def)
    {
      NameDisplayScreen.Instance.RegisterComponent(this.gameObject, (object) this);
    }

    public void OnMonitorStart(object data)
    {
      GameplayEventInstance gameplayEventInstance = data as GameplayEventInstance;
      if (!this.events.Contains(gameplayEventInstance))
      {
        this.events.Add(gameplayEventInstance);
        gameplayEventInstance.RegisterMonitorCallback(this.gameObject);
      }
      this.smi.sm.UpdateFX(this.smi);
      this.smi.sm.UpdateEventDisplay(this.smi);
    }

    public void OnMonitorEnd(object data)
    {
      GameplayEventInstance gameplayEventInstance = data as GameplayEventInstance;
      if (this.events.Contains(gameplayEventInstance))
      {
        this.events.Remove(gameplayEventInstance);
        gameplayEventInstance.UnregisterMonitorCallback(this.gameObject);
      }
      this.smi.sm.UpdateFX(this.smi);
      this.smi.sm.UpdateEventDisplay(this.smi);
      if (this.events.Count != 0)
        return;
      this.smi.GoTo((StateMachine.BaseState) this.sm.idle);
    }

    public void OnSelect(object data)
    {
      if (!(bool) data)
        return;
      foreach (GameplayEventInstance gameplayEventInstance in this.events)
      {
        if (!gameplayEventInstance.seenNotification && gameplayEventInstance.GetEventPopupData != null)
        {
          gameplayEventInstance.seenNotification = true;
          EventInfoScreen.ShowPopup(gameplayEventInstance.GetEventPopupData());
          break;
        }
      }
      if (this.UnseenCount() != 0)
        return;
      this.smi.GoTo((StateMachine.BaseState) this.sm.activeState.seenAllEvents);
    }

    public int UnseenCount() => this.events.Count<GameplayEventInstance>((Func<GameplayEventInstance, bool>) (evt => !evt.seenNotification));
  }
}
