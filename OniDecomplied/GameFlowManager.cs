// Decompiled with JetBrains decompiler
// Type: GameFlowManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
public class GameFlowManager : StateMachineComponent<GameFlowManager.StatesInstance>, ISaveLoadable
{
  [MyCmpAdd]
  private Notifier notifier;
  public static GameFlowManager Instance;

  public static void DestroyInstance() => GameFlowManager.Instance = (GameFlowManager) null;

  protected virtual void OnPrefabInit() => GameFlowManager.Instance = this;

  protected virtual void OnSpawn() => this.smi.StartSM();

  public bool IsGameOver() => this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.gameover);

  public class StatesInstance : 
    GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager, object>.GameInstance
  {
    public Notification colonyLostNotification = new Notification((string) MISC.NOTIFICATIONS.COLONYLOST.NAME, NotificationType.Bad, expires: false);

    public bool IsIncapacitated(GameObject go) => false;

    public void CheckForGameOver()
    {
      if (!Game.Instance.GameStarted() || GenericGameSettings.instance.disableGameOver)
        return;
      bool flag;
      if (Components.LiveMinionIdentities.Count == 0)
      {
        flag = true;
      }
      else
      {
        flag = true;
        foreach (Component component in Components.LiveMinionIdentities.Items)
        {
          if (!this.IsIncapacitated(component.gameObject))
          {
            flag = false;
            break;
          }
        }
      }
      if (!flag)
        return;
      this.GoTo((StateMachine.BaseState) this.sm.gameover.pending);
    }

    public StatesInstance(GameFlowManager smi)
      : base(smi)
    {
    }
  }

  public class States : 
    GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager>
  {
    public GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager, object>.State loading;
    public GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager, object>.State running;
    public GameFlowManager.States.GameOverState gameover;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.loading;
      this.loading.ScheduleGoTo(4f, (StateMachine.BaseState) this.running);
      this.running.Update("CheckForGameOver", (Action<GameFlowManager.StatesInstance, float>) ((smi, dt) => smi.CheckForGameOver()));
      this.gameover.TriggerOnEnter(GameHashes.GameOver).ToggleNotification((Func<GameFlowManager.StatesInstance, Notification>) (smi => smi.colonyLostNotification));
      this.gameover.pending.Enter("Goto(gameover.active)", (StateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager, object>.State.Callback) (smi => UIScheduler.Instance.Schedule("Goto(gameover.active)", 4f, (Action<object>) (d => smi.GoTo((StateMachine.BaseState) this.gameover.active)), (object) null, (SchedulerGroup) null)));
      this.gameover.active.Enter((StateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager, object>.State.Callback) (smi =>
      {
        if (GenericGameSettings.instance.demoMode)
          DemoTimer.Instance.EndDemo();
        else
          ((Component) GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.GameOverScreen, (GameObject) null, (GameScreenManager.UIRenderTarget) 2)).GetComponent<KScreen>().Show(true);
      }));
    }

    public class GameOverState : 
      GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager, object>.State
    {
      public GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager, object>.State pending;
      public GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager, object>.State active;
    }
  }
}
