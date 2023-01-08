// Decompiled with JetBrains decompiler
// Type: Klei.AI.SatelliteCrashEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

namespace Klei.AI
{
  public class SatelliteCrashEvent : GameplayEvent<SatelliteCrashEvent.StatesInstance>
  {
    public SatelliteCrashEvent()
      : base("SatelliteCrash")
    {
      this.title = (string) GAMEPLAY_EVENTS.EVENT_TYPES.SATELLITE_CRASH.NAME;
      this.description = (string) GAMEPLAY_EVENTS.EVENT_TYPES.SATELLITE_CRASH.DESCRIPTION;
    }

    public override StateMachine.Instance GetSMI(
      GameplayEventManager manager,
      GameplayEventInstance eventInstance)
    {
      return (StateMachine.Instance) new SatelliteCrashEvent.StatesInstance(manager, eventInstance, this);
    }

    public class StatesInstance : 
      GameplayEventStateMachine<SatelliteCrashEvent.States, SatelliteCrashEvent.StatesInstance, GameplayEventManager, SatelliteCrashEvent>.GameplayEventStateMachineInstance
    {
      public StatesInstance(
        GameplayEventManager master,
        GameplayEventInstance eventInstance,
        SatelliteCrashEvent crashEvent)
        : base(master, eventInstance, crashEvent)
      {
      }

      public Notification Plan()
      {
        Vector3 vector3;
        // ISSUE: explicit constructor call
        ((Vector3) ref vector3).\u002Ector((float) (Grid.WidthInCells / 2 + Random.Range(-Grid.WidthInCells / 3, Grid.WidthInCells / 3)), (float) (Grid.HeightInCells - 1), Grid.GetLayerZ(Grid.SceneLayer.FXFront));
        GameObject spawn = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(SatelliteCometConfig.ID)), vector3);
        spawn.SetActive(true);
        Notification notification = EventInfoScreen.CreateNotification(this.smi.sm.GenerateEventPopupData(this.smi));
        notification.clickFocus = spawn.transform;
        spawn.GetComponent<Comet>().OnImpact += (System.Action) (() =>
        {
          GameObject gameObject = new GameObject();
          gameObject.transform.position = spawn.transform.position;
          notification.clickFocus = gameObject.transform;
          GridVisibility.Reveal(Grid.PosToXY(gameObject.transform.position).x, Grid.PosToXY(gameObject.transform.position).y, 6, 4f);
        });
        return notification;
      }
    }

    public class States : 
      GameplayEventStateMachine<SatelliteCrashEvent.States, SatelliteCrashEvent.StatesInstance, GameplayEventManager, SatelliteCrashEvent>
    {
      public GameStateMachine<SatelliteCrashEvent.States, SatelliteCrashEvent.StatesInstance, GameplayEventManager, object>.State notify;
      public GameStateMachine<SatelliteCrashEvent.States, SatelliteCrashEvent.StatesInstance, GameplayEventManager, object>.State ending;

      public override void InitializeStates(out StateMachine.BaseState default_state)
      {
        default_state = (StateMachine.BaseState) this.notify;
        this.notify.ToggleNotification((Func<SatelliteCrashEvent.StatesInstance, Notification>) (smi => smi.Plan()));
        this.ending.ReturnSuccess();
      }

      public override EventInfoData GenerateEventPopupData(SatelliteCrashEvent.StatesInstance smi)
      {
        EventInfoData eventPopupData = new EventInfoData(smi.gameplayEvent.title, smi.gameplayEvent.description, smi.gameplayEvent.animFileName);
        eventPopupData.location = (string) GAMEPLAY_EVENTS.LOCATIONS.SURFACE;
        eventPopupData.whenDescription = (string) GAMEPLAY_EVENTS.TIMES.NOW;
        eventPopupData.AddDefaultOption((System.Action) (() => smi.GoTo((StateMachine.BaseState) smi.sm.ending)));
        return eventPopupData;
      }
    }
  }
}
