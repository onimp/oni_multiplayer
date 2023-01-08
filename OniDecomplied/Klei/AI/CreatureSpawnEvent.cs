// Decompiled with JetBrains decompiler
// Type: Klei.AI.CreatureSpawnEvent
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
  public class CreatureSpawnEvent : GameplayEvent<CreatureSpawnEvent.StatesInstance>
  {
    public const string ID = "HatchSpawnEvent";
    public const float UPDATE_TIME = 4f;
    public const float NUM_TO_SPAWN = 10f;
    public const float duration = 40f;
    public static List<string> CreatureSpawnEventIDs = new List<string>()
    {
      "Hatch",
      "Squirrel",
      "Puft",
      "Crab",
      "Drecko",
      "Mole",
      "LightBug",
      "Pacu"
    };

    public CreatureSpawnEvent()
      : base("HatchSpawnEvent")
    {
      this.title = (string) GAMEPLAY_EVENTS.EVENT_TYPES.CREATURE_SPAWN.NAME;
      this.description = (string) GAMEPLAY_EVENTS.EVENT_TYPES.CREATURE_SPAWN.DESCRIPTION;
    }

    public override StateMachine.Instance GetSMI(
      GameplayEventManager manager,
      GameplayEventInstance eventInstance)
    {
      return (StateMachine.Instance) new CreatureSpawnEvent.StatesInstance(manager, eventInstance, this);
    }

    public class StatesInstance : 
      GameplayEventStateMachine<CreatureSpawnEvent.States, CreatureSpawnEvent.StatesInstance, GameplayEventManager, CreatureSpawnEvent>.GameplayEventStateMachineInstance
    {
      [Serialize]
      private List<Vector3> spawnPositions = new List<Vector3>();
      [Serialize]
      private string creatureID;

      public StatesInstance(
        GameplayEventManager master,
        GameplayEventInstance eventInstance,
        CreatureSpawnEvent creatureEvent)
        : base(master, eventInstance, creatureEvent)
      {
      }

      private void PickCreatureToSpawn() => this.creatureID = Util.GetRandom<string>(CreatureSpawnEvent.CreatureSpawnEventIDs);

      private void PickSpawnLocations()
      {
        Vector3 position = TransformExtensions.GetPosition(Util.GetRandom<Telepad>(Components.Telepads.Items).transform);
        int num = 100;
        ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
        GameScenePartitioner.Instance.GatherEntries((int) position.x - num / 2, (int) position.y - num / 2, num, num, GameScenePartitioner.Instance.plants, (List<ScenePartitionerEntry>) gathered_entries);
        foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
        {
          KPrefabID kprefabId = (KPrefabID) partitionerEntry.obj;
          if (!Object.op_Implicit((Object) ((Component) kprefabId).GetComponent<TreeBud>()))
            this.smi.spawnPositions.Add(TransformExtensions.GetPosition(((KMonoBehaviour) kprefabId).transform));
        }
        gathered_entries.Recycle();
      }

      public void InitializeEvent()
      {
        this.PickCreatureToSpawn();
        this.PickSpawnLocations();
      }

      public void EndEvent()
      {
        this.creatureID = (string) null;
        this.spawnPositions.Clear();
      }

      public void SpawnCreature()
      {
        if (this.spawnPositions.Count <= 0)
          return;
        Vector3 random = Util.GetRandom<Vector3>(this.spawnPositions);
        Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(this.creatureID)), random).SetActive(true);
      }
    }

    public class States : 
      GameplayEventStateMachine<CreatureSpawnEvent.States, CreatureSpawnEvent.StatesInstance, GameplayEventManager, CreatureSpawnEvent>
    {
      public GameStateMachine<CreatureSpawnEvent.States, CreatureSpawnEvent.StatesInstance, GameplayEventManager, object>.State initialize_event;
      public GameStateMachine<CreatureSpawnEvent.States, CreatureSpawnEvent.StatesInstance, GameplayEventManager, object>.State spawn_season;
      public GameStateMachine<CreatureSpawnEvent.States, CreatureSpawnEvent.StatesInstance, GameplayEventManager, object>.State start;

      public override void InitializeStates(out StateMachine.BaseState default_state)
      {
        default_state = (StateMachine.BaseState) this.initialize_event;
        this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
        this.initialize_event.Enter((StateMachine<CreatureSpawnEvent.States, CreatureSpawnEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi =>
        {
          smi.InitializeEvent();
          smi.GoTo((StateMachine.BaseState) this.spawn_season);
        }));
        this.start.DoNothing();
        this.spawn_season.Update((Action<CreatureSpawnEvent.StatesInstance, float>) ((smi, dt) => smi.SpawnCreature()), (UpdateRate) 7).Exit((StateMachine<CreatureSpawnEvent.States, CreatureSpawnEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi => smi.EndEvent()));
      }

      public override EventInfoData GenerateEventPopupData(CreatureSpawnEvent.StatesInstance smi) => new EventInfoData(smi.gameplayEvent.title, smi.gameplayEvent.description, smi.gameplayEvent.animFileName)
      {
        location = (string) GAMEPLAY_EVENTS.LOCATIONS.PRINTING_POD,
        whenDescription = (string) GAMEPLAY_EVENTS.TIMES.NOW
      };
    }
  }
}
