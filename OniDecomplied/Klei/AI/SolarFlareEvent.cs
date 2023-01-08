// Decompiled with JetBrains decompiler
// Type: Klei.AI.SolarFlareEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Klei.AI
{
  public class SolarFlareEvent : GameplayEvent<SolarFlareEvent.StatesInstance>
  {
    public const string ID = "SolarFlareEvent";
    public const float DURATION = 7f;

    public SolarFlareEvent()
      : base(nameof (SolarFlareEvent))
    {
      this.title = (string) GAMEPLAY_EVENTS.EVENT_TYPES.SOLAR_FLARE.NAME;
      this.description = (string) GAMEPLAY_EVENTS.EVENT_TYPES.SOLAR_FLARE.DESCRIPTION;
    }

    public override StateMachine.Instance GetSMI(
      GameplayEventManager manager,
      GameplayEventInstance eventInstance)
    {
      return (StateMachine.Instance) new SolarFlareEvent.StatesInstance(manager, eventInstance, this);
    }

    public class StatesInstance : 
      GameplayEventStateMachine<SolarFlareEvent.States, SolarFlareEvent.StatesInstance, GameplayEventManager, SolarFlareEvent>.GameplayEventStateMachineInstance
    {
      public StatesInstance(
        GameplayEventManager master,
        GameplayEventInstance eventInstance,
        SolarFlareEvent solarFlareEvent)
        : base(master, eventInstance, solarFlareEvent)
      {
      }
    }

    public class States : 
      GameplayEventStateMachine<SolarFlareEvent.States, SolarFlareEvent.StatesInstance, GameplayEventManager, SolarFlareEvent>
    {
      public GameStateMachine<SolarFlareEvent.States, SolarFlareEvent.StatesInstance, GameplayEventManager, object>.State idle;
      public GameStateMachine<SolarFlareEvent.States, SolarFlareEvent.StatesInstance, GameplayEventManager, object>.State start;
      public GameStateMachine<SolarFlareEvent.States, SolarFlareEvent.StatesInstance, GameplayEventManager, object>.State finished;

      public override void InitializeStates(out StateMachine.BaseState default_state)
      {
        default_state = (StateMachine.BaseState) this.idle;
        this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
        this.idle.DoNothing();
        this.start.ScheduleGoTo(7f, (StateMachine.BaseState) this.finished);
        this.finished.ReturnSuccess();
      }

      public override EventInfoData GenerateEventPopupData(SolarFlareEvent.StatesInstance smi) => new EventInfoData(smi.gameplayEvent.title, smi.gameplayEvent.description, smi.gameplayEvent.animFileName)
      {
        location = (string) GAMEPLAY_EVENTS.LOCATIONS.SUN,
        whenDescription = (string) GAMEPLAY_EVENTS.TIMES.NOW
      };
    }
  }
}
