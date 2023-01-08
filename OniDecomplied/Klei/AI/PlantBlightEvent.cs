// Decompiled with JetBrains decompiler
// Type: Klei.AI.PlantBlightEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
  public class PlantBlightEvent : GameplayEvent<PlantBlightEvent.StatesInstance>
  {
    private const float BLIGHT_DISTANCE = 6f;
    public string targetPlantPrefab;
    public float infectionDuration;
    public float incubationDuration;

    public PlantBlightEvent(
      string id,
      string targetPlantPrefab,
      float infectionDuration,
      float incubationDuration)
      : base(id)
    {
      this.targetPlantPrefab = targetPlantPrefab;
      this.infectionDuration = infectionDuration;
      this.incubationDuration = incubationDuration;
      this.title = (string) GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.NAME;
      this.description = (string) GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.DESCRIPTION;
    }

    public override StateMachine.Instance GetSMI(
      GameplayEventManager manager,
      GameplayEventInstance eventInstance)
    {
      return (StateMachine.Instance) new PlantBlightEvent.StatesInstance(manager, eventInstance, this);
    }

    public class States : 
      GameplayEventStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, PlantBlightEvent>
    {
      public GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State planning;
      public PlantBlightEvent.States.RunningStates running;
      public GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State finished;
      public StateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.Signal doFinish;
      public StateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.FloatParameter nextInfection;

      public override void InitializeStates(out StateMachine.BaseState default_state)
      {
        base.InitializeStates(out default_state);
        default_state = (StateMachine.BaseState) this.planning;
        this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
        this.planning.Enter((StateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi => smi.InfectAPlant(true))).GoTo((GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State) this.running);
        this.running.ToggleNotification((Func<PlantBlightEvent.StatesInstance, Notification>) (smi => EventInfoScreen.CreateNotification(this.GenerateEventPopupData(smi)))).EventHandlerTransition(GameHashes.Uprooted, this.finished, new Func<PlantBlightEvent.StatesInstance, object, bool>(this.NoBlightedPlants)).DefaultState(this.running.waiting).OnSignal(this.doFinish, this.finished);
        double num;
        this.running.waiting.ParamTransition<float>((StateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.Parameter<float>) this.nextInfection, this.running.infect, (StateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.Parameter<float>.Callback) ((smi, p) => (double) p <= 0.0)).Update((Action<PlantBlightEvent.StatesInstance, float>) ((smi, dt) => num = (double) this.nextInfection.Delta(-dt, smi)), (UpdateRate) 7);
        this.running.infect.Enter((StateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi => smi.InfectAPlant(false))).GoTo(this.running.waiting);
        this.finished.DoNotification((Func<PlantBlightEvent.StatesInstance, Notification>) (smi => this.CreateSuccessNotification(smi, this.GenerateEventPopupData(smi)))).ReturnSuccess();
      }

      public override EventInfoData GenerateEventPopupData(PlantBlightEvent.StatesInstance smi)
      {
        EventInfoData eventPopupData = new EventInfoData(smi.gameplayEvent.title, smi.gameplayEvent.description, smi.gameplayEvent.animFileName);
        string str = TagExtensions.ToTag(smi.gameplayEvent.targetPlantPrefab).ProperName();
        eventPopupData.location = (string) GAMEPLAY_EVENTS.LOCATIONS.COLONY_WIDE;
        eventPopupData.whenDescription = (string) GAMEPLAY_EVENTS.TIMES.NOW;
        eventPopupData.SetTextParameter("plant", str);
        return eventPopupData;
      }

      private Notification CreateSuccessNotification(
        PlantBlightEvent.StatesInstance smi,
        EventInfoData eventInfoData)
      {
        string plantName = TagExtensions.ToTag(smi.gameplayEvent.targetPlantPrefab).ProperName();
        return new Notification(GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.SUCCESS.Replace("{plant}", plantName), NotificationType.Neutral, (Func<List<Notification>, object, string>) ((list, data) => GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.SUCCESS_TOOLTIP.Replace("{plant}", plantName)));
      }

      private bool NoBlightedPlants(PlantBlightEvent.StatesInstance smi, object obj)
      {
        GameObject go = (GameObject) obj;
        if (!go.HasTag(GameTags.Blighted))
          return true;
        foreach (Crop cmp in Components.Crops.Items.FindAll((Predicate<Crop>) (p => ((Object) p).name == smi.gameplayEvent.targetPlantPrefab)))
        {
          if (!Object.op_Equality((Object) go.gameObject, (Object) ((Component) cmp).gameObject) && ((Component) cmp).HasTag(GameTags.Blighted))
            return false;
        }
        return true;
      }

      public class RunningStates : 
        GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State
      {
        public GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State waiting;
        public GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State infect;
      }
    }

    public class StatesInstance : 
      GameplayEventStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, PlantBlightEvent>.GameplayEventStateMachineInstance
    {
      [Serialize]
      private float startTime;

      public StatesInstance(
        GameplayEventManager master,
        GameplayEventInstance eventInstance,
        PlantBlightEvent blightEvent)
        : base(master, eventInstance, blightEvent)
      {
        this.startTime = Time.time;
      }

      public void InfectAPlant(bool initialInfection)
      {
        if ((double) Time.time - (double) this.startTime > (double) this.smi.gameplayEvent.infectionDuration)
        {
          this.sm.doFinish.Trigger(this.smi);
        }
        else
        {
          List<Crop> all = Components.Crops.Items.FindAll((Predicate<Crop>) (p => ((Object) p).name == this.smi.gameplayEvent.targetPlantPrefab));
          if (all.Count == 0)
          {
            this.sm.doFinish.Trigger(this.smi);
          }
          else
          {
            if (all.Count > 0)
            {
              List<Crop> cropList1 = new List<Crop>();
              List<Crop> cropList2 = new List<Crop>();
              foreach (Crop cmp in all)
              {
                if (((Component) cmp).HasTag(GameTags.Blighted))
                  cropList1.Add(cmp);
                else
                  cropList2.Add(cmp);
              }
              if (cropList1.Count == 0)
              {
                if (initialInfection)
                {
                  Crop crop = cropList2[Random.Range(0, cropList2.Count)];
                  Debug.Log((object) "Blighting a random plant", (Object) crop);
                  ((Component) crop).GetComponent<BlightVulnerable>().MakeBlighted();
                }
                else
                  this.sm.doFinish.Trigger(this.smi);
              }
              else if (cropList2.Count > 0)
              {
                Crop crop1 = cropList1[Random.Range(0, cropList1.Count)];
                Debug.Log((object) "Spreading blight from a plant", (Object) crop1);
                Util.Shuffle<Crop>((IList<Crop>) cropList2);
                foreach (Crop crop2 in cropList2)
                {
                  Vector3 vector3 = Vector3.op_Subtraction(TransformExtensions.GetPosition(crop2.transform), TransformExtensions.GetPosition(crop1.transform));
                  if ((double) ((Vector3) ref vector3).magnitude < 6.0)
                  {
                    ((Component) crop2).GetComponent<BlightVulnerable>().MakeBlighted();
                    break;
                  }
                }
              }
            }
            double num = (double) this.sm.nextInfection.Set(this.smi.gameplayEvent.incubationDuration, this);
          }
        }
      }
    }
  }
}
