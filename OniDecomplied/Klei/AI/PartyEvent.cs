// Decompiled with JetBrains decompiler
// Type: Klei.AI.PartyEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Klei.AI
{
  public class PartyEvent : GameplayEvent<PartyEvent.StatesInstance>
  {
    public const string cancelEffect = "NoFunAllowed";
    public const float FUTURE_TIME = 60f;
    public const float DURATION = 60f;

    public PartyEvent()
      : base("Party")
    {
      this.animFileName = HashedString.op_Implicit("event_pop_up_assets_kanim");
      this.title = (string) GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.NAME;
      this.description = (string) GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.DESCRIPTION;
    }

    public override StateMachine.Instance GetSMI(
      GameplayEventManager manager,
      GameplayEventInstance eventInstance)
    {
      return (StateMachine.Instance) new PartyEvent.StatesInstance(manager, eventInstance, this);
    }

    public class StatesInstance : 
      GameplayEventStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, PartyEvent>.GameplayEventStateMachineInstance
    {
      private List<Chore> chores;
      public Notification mainNotification;

      public StatesInstance(
        GameplayEventManager master,
        GameplayEventInstance eventInstance,
        PartyEvent partyEvent)
        : base(master, eventInstance, partyEvent)
      {
      }

      public void AddNewChore(Room room)
      {
        List<KPrefabID> all = room.buildings.FindAll((Predicate<KPrefabID>) (match => match.HasTag(RoomConstraints.ConstraintTags.RecBuilding)));
        if (all.Count == 0)
        {
          DebugUtil.LogWarningArgs(new object[1]
          {
            (object) "Tried adding a party chore but the room wasn't valid! This probably happened on load? It's because rooms aren't built yet!"
          });
        }
        else
        {
          int num = 0;
          bool flag = false;
          int locator_cell = Grid.InvalidCell;
          while (num < 20 && !flag)
          {
            ++num;
            KPrefabID cmp = all[Random.Range(0, all.Count)];
            CellOffset offset;
            // ISSUE: explicit constructor call
            ((CellOffset) ref offset).\u002Ector(Random.Range(-2, 3), 0);
            locator_cell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) cmp), offset);
            if (!Grid.HasDoor[locator_cell] && Game.Instance.roomProber.GetCavityForCell(locator_cell) == room.cavity && this.chores.Find((Predicate<Chore>) (match => Grid.PosToCell(match.target.gameObject) == locator_cell)) == null)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            return;
          GameObject locator = ChoreHelpers.CreateLocator("PartyWorkable", Grid.CellToPosCBC(locator_cell, Grid.SceneLayer.Move));
          PartyPointWorkable chat_workable = locator.AddOrGet<PartyPointWorkable>();
          chat_workable.SetWorkTime((float) Random.Range(10, 30));
          chat_workable.basePriority = RELAXATION.PRIORITY.SPECIAL_EVENT;
          chat_workable.faceTargetWhenWorking = true;
          this.chores.Add((Chore) new PartyChore(locator.GetComponent<IStateMachineTarget>(), (Workable) chat_workable, on_end: ((Action<Chore>) (data =>
          {
            if (this.chores == null)
              return;
            this.chores.Remove(data);
            Util.KDestroyGameObject(locator);
          }))));
        }
      }

      public void ClearChores()
      {
        if (this.chores != null)
        {
          for (int index = this.chores.Count - 1; index >= 0; --index)
          {
            if (this.chores[index] != null)
              Util.KDestroyGameObject(this.chores[index].gameObject);
          }
        }
        this.chores = (List<Chore>) null;
      }

      public void UpdateChores(Room room)
      {
        if (room == null)
          return;
        if (this.chores == null)
          this.chores = new List<Chore>();
        for (int count = this.chores.Count; count < Components.LiveMinionIdentities.Count * 2; ++count)
          this.AddNewChore(room);
      }
    }

    public class States : 
      GameplayEventStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, PartyEvent>
    {
      public StateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.TargetParameter roomObject;
      public StateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.TargetParameter planner;
      public StateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.TargetParameter guest;
      public PartyEvent.States.PlanningStates planning;
      public PartyEvent.States.WarmupStates warmup;
      public GameStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State partying;
      public GameStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State ending;
      public GameStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State canceled;

      public override void InitializeStates(out StateMachine.BaseState default_state)
      {
        base.InitializeStates(out default_state);
        default_state = (StateMachine.BaseState) this.planning.prepare_entities;
        this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
        this.root.Enter(new StateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State.Callback(this.PopulateTargetsAndText));
        this.planning.DefaultState(this.planning.prepare_entities);
        this.planning.prepare_entities.Enter((StateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi =>
        {
          if (Object.op_Inequality((Object) this.planner.Get(smi), (Object) null) && Object.op_Inequality((Object) this.guest.Get(smi), (Object) null))
            smi.GoTo((StateMachine.BaseState) this.planning.wait_for_input);
          else
            smi.GoTo((StateMachine.BaseState) this.ending);
        }));
        this.planning.wait_for_input.ToggleNotification((Func<PartyEvent.StatesInstance, Notification>) (smi => EventInfoScreen.CreateNotification(this.GenerateEventPopupData(smi))));
        this.warmup.ToggleNotification((Func<PartyEvent.StatesInstance, Notification>) (smi => EventInfoScreen.CreateNotification(this.GenerateEventPopupData(smi))));
        this.warmup.wait.ScheduleGoTo(60f, (StateMachine.BaseState) this.warmup.start);
        this.warmup.start.Enter(new StateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State.Callback(this.PopulateTargetsAndText)).Enter((StateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi =>
        {
          if (this.GetChosenRoom(smi) == null)
            smi.GoTo((StateMachine.BaseState) this.canceled);
          else
            smi.GoTo((StateMachine.BaseState) this.partying);
        }));
        this.partying.ToggleNotification((Func<PartyEvent.StatesInstance, Notification>) (smi => new Notification((string) GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.UNDERWAY, NotificationType.Good, (Func<List<Notification>, object, string>) ((a, b) => (string) GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.UNDERWAY_TOOLTIP), expires: false, click_focus: this.roomObject.Get(smi).transform))).Update((Action<PartyEvent.StatesInstance, float>) ((smi, dt) => smi.UpdateChores(this.GetChosenRoom(smi))), (UpdateRate) 7).ScheduleGoTo(60f, (StateMachine.BaseState) this.ending);
        this.ending.ReturnSuccess();
        this.canceled.DoNotification((Func<PartyEvent.StatesInstance, Notification>) (smi => GameplayEventManager.CreateStandardCancelledNotification(this.GenerateEventPopupData(smi)))).Enter((StateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi =>
        {
          if (Object.op_Inequality((Object) this.planner.Get(smi), (Object) null))
            this.planner.Get(smi).GetComponent<Effects>().Add("NoFunAllowed", true);
          if (!Object.op_Inequality((Object) this.guest.Get(smi), (Object) null))
            return;
          this.guest.Get(smi).GetComponent<Effects>().Add("NoFunAllowed", true);
        })).ReturnFailure();
      }

      public Room GetChosenRoom(PartyEvent.StatesInstance smi) => Game.Instance.roomProber.GetRoomOfGameObject(this.roomObject.Get(smi));

      public override EventInfoData GenerateEventPopupData(PartyEvent.StatesInstance smi)
      {
        EventInfoData eventPopupData = new EventInfoData(smi.gameplayEvent.title, smi.gameplayEvent.description, smi.gameplayEvent.animFileName);
        Room chosenRoom = this.GetChosenRoom(smi);
        string str = chosenRoom != null ? chosenRoom.GetProperName() : GAMEPLAY_EVENTS.LOCATIONS.NONE_AVAILABLE.ToString();
        Effect effect1 = Db.Get().effects.Get("Socialized");
        Effect effect2 = Db.Get().effects.Get("NoFunAllowed");
        eventPopupData.location = str;
        eventPopupData.whenDescription = string.Format((string) GAMEPLAY_EVENTS.TIMES.IN_CYCLES, (object) 0.1f);
        eventPopupData.minions = new GameObject[2]
        {
          smi.sm.guest.Get(smi),
          smi.sm.planner.Get(smi)
        };
        EventInfoData.Option option1 = eventPopupData.AddOption((string) GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.ACCEPT_OPTION_NAME, (string) GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.ACCEPT_OPTION_DESC);
        option1.callback = (System.Action) (() => smi.GoTo((StateMachine.BaseState) smi.sm.warmup.wait));
        option1.AddPositiveIcon(Assets.GetSprite(HashedString.op_Implicit("overlay_materials")), Effect.CreateFullTooltip(effect1, true));
        option1.tooltip = (string) GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.ACCEPT_OPTION_DESC;
        if (false)
        {
          option1.AddInformationIcon("Cake must be built");
          option1.allowed = false;
          option1.tooltip = (string) GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.ACCEPT_OPTION_INVALID_TOOLTIP;
        }
        EventInfoData.Option option2 = eventPopupData.AddOption((string) GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.REJECT_OPTION_NAME, (string) GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.REJECT_OPTION_DESC);
        option2.callback = (System.Action) (() => smi.GoTo((StateMachine.BaseState) smi.sm.canceled));
        option2.AddNegativeIcon(Assets.GetSprite(HashedString.op_Implicit("overlay_decor")), Effect.CreateFullTooltip(effect2, true));
        eventPopupData.AddDefaultConsiderLaterOption();
        eventPopupData.SetTextParameter("host", this.planner.Get(smi).GetProperName());
        eventPopupData.SetTextParameter("dupe", this.guest.Get(smi).GetProperName());
        eventPopupData.SetTextParameter("goodEffect", effect1.Name);
        eventPopupData.SetTextParameter("badEffect", effect2.Name);
        return eventPopupData;
      }

      public void PopulateTargetsAndText(PartyEvent.StatesInstance smi)
      {
        if (Object.op_Equality((Object) this.roomObject.Get(smi), (Object) null))
          this.roomObject.Set((KMonoBehaviour) Game.Instance.roomProber.rooms.Find((Predicate<Room>) (match => match.roomType == Db.Get().RoomTypes.RecRoom))?.GetPrimaryEntities()[0], smi);
        if (Components.LiveMinionIdentities.Count <= 0)
          return;
        if (Object.op_Equality((Object) this.planner.Get(smi), (Object) null))
          this.planner.Set((KMonoBehaviour) Components.LiveMinionIdentities[Random.Range(0, Components.LiveMinionIdentities.Count)], smi);
        if (!Object.op_Equality((Object) this.guest.Get(smi), (Object) null))
          return;
        this.guest.Set((KMonoBehaviour) Components.LiveMinionIdentities[Random.Range(0, Components.LiveMinionIdentities.Count)], smi);
      }

      public class PlanningStates : 
        GameStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State
      {
        public GameStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State prepare_entities;
        public GameStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State wait_for_input;
      }

      public class WarmupStates : 
        GameStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State
      {
        public GameStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State wait;
        public GameStateMachine<PartyEvent.States, PartyEvent.StatesInstance, GameplayEventManager, object>.State start;
      }
    }
  }
}
