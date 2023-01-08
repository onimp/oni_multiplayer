// Decompiled with JetBrains decompiler
// Type: SolidConsumerMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SolidConsumerMonitor : 
  GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>
{
  private GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.State satisfied;
  private GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.State lookingforfood;
  private static Tag[] creatureTags = new Tag[2]
  {
    GameTags.Creatures.ReservedByCreature,
    GameTags.CreatureBrain
  };

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.root.EventHandler(GameHashes.EatSolidComplete, (GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.GameEvent.Callback) ((smi, data) => smi.OnEatSolidComplete(data))).ToggleBehaviour(GameTags.Creatures.WantsToEat, (StateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.targetEdible, (Object) null) && !smi.targetEdible.HasTag(GameTags.Creatures.ReservedByCreature)));
    this.satisfied.TagTransition(GameTags.Creatures.Hungry, this.lookingforfood);
    this.lookingforfood.TagTransition(GameTags.Creatures.Hungry, this.satisfied, true).Update(new System.Action<SolidConsumerMonitor.Instance, float>(SolidConsumerMonitor.FindFood), (UpdateRate) 7, true);
  }

  [Conditional("DETAILED_SOLID_CONSUMER_MONITOR_PROFILE")]
  private static void BeginDetailedSample(string region_name)
  {
  }

  [Conditional("DETAILED_SOLID_CONSUMER_MONITOR_PROFILE")]
  private static void EndDetailedSample(string region_name)
  {
  }

  private static void FindFood(SolidConsumerMonitor.Instance smi, float dt)
  {
    ListPool<GameObject, SolidConsumerMonitor>.PooledList pooledList1 = ListPool<GameObject, SolidConsumerMonitor>.Allocate();
    Diet diet = smi.def.diet;
    int x1 = 0;
    int y1 = 0;
    Grid.PosToXY(TransformExtensions.GetPosition(smi.gameObject.transform), out x1, out y1);
    int x_bottomLeft = x1 - 8;
    int y_bottomLeft = y1 - 8;
    ListPool<Storage, SolidConsumerMonitor>.PooledList pooledList2 = ListPool<Storage, SolidConsumerMonitor>.Allocate();
    if (!diet.eatsPlantsDirectly)
    {
      foreach (CreatureFeeder creatureFeeder in Components.CreatureFeeders.GetItems(smi.GetMyWorldId()))
      {
        int x2;
        int y2;
        Grid.PosToXY(TransformExtensions.GetPosition(creatureFeeder.transform), out x2, out y2);
        if (x2 >= x_bottomLeft && x2 <= x_bottomLeft + 16 && y2 >= y_bottomLeft && y2 <= y_bottomLeft + 16)
        {
          ((Component) creatureFeeder).GetComponents<Storage>((List<Storage>) pooledList2);
          foreach (Storage storage in (List<Storage>) pooledList2)
          {
            if (!Object.op_Equality((Object) storage, (Object) null))
            {
              foreach (GameObject gameObject in storage.items)
              {
                if (!Object.op_Equality((Object) gameObject, (Object) null))
                {
                  KPrefabID component = gameObject.GetComponent<KPrefabID>();
                  if (!component.HasAnyTags(SolidConsumerMonitor.creatureTags) && diet.GetDietInfo(component.PrefabTag) != null)
                    ((List<GameObject>) pooledList1).Add(gameObject);
                }
              }
            }
          }
        }
      }
    }
    pooledList2.Recycle();
    ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
    if (diet.eatsPlantsDirectly)
    {
      GameScenePartitioner.Instance.GatherEntries(x_bottomLeft, y_bottomLeft, 16, 16, GameScenePartitioner.Instance.plants, (List<ScenePartitionerEntry>) gathered_entries);
      foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
      {
        KPrefabID cmp = (KPrefabID) partitionerEntry.obj;
        if (!cmp.HasAnyTags(SolidConsumerMonitor.creatureTags) && diet.GetDietInfo(cmp.PrefabTag) != null)
        {
          if (cmp.HasTag(GameTags.Plant))
          {
            float num1 = 0.25f;
            float num2 = 0.0f;
            BuddingTrunk component = ((Component) cmp).GetComponent<BuddingTrunk>();
            if (Object.op_Implicit((Object) component))
            {
              num2 = component.GetMaxBranchMaturity();
            }
            else
            {
              AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup((Component) cmp);
              if (amountInstance != null)
                num2 = amountInstance.value / amountInstance.GetMax();
            }
            if ((double) num2 < (double) num1)
              continue;
          }
          ((List<GameObject>) pooledList1).Add(((Component) cmp).gameObject);
        }
      }
    }
    else
    {
      GameScenePartitioner.Instance.GatherEntries(x_bottomLeft, y_bottomLeft, 16, 16, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
      foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
      {
        Pickupable pickupable = (Pickupable) partitionerEntry.obj;
        KPrefabID component = ((Component) pickupable).GetComponent<KPrefabID>();
        if (!component.HasAnyTags(SolidConsumerMonitor.creatureTags) && diet.GetDietInfo(component.PrefabTag) != null)
          ((List<GameObject>) pooledList1).Add(((Component) pickupable).gameObject);
      }
    }
    gathered_entries.Recycle();
    Navigator component1 = smi.GetComponent<Navigator>();
    DrowningMonitor component2 = smi.GetComponent<DrowningMonitor>();
    bool flag = Object.op_Inequality((Object) component2, (Object) null) && component2.canDrownToDeath && !component2.livesUnderWater;
    smi.targetEdible = (GameObject) null;
    int num = -1;
    foreach (GameObject gameObject in (List<GameObject>) pooledList1)
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(gameObject.transform));
      if (!flag || component2.IsCellSafe(cell))
      {
        int navigationCost = component1.GetNavigationCost(cell);
        if (navigationCost != -1 && (navigationCost < num || num == -1))
        {
          num = navigationCost;
          smi.targetEdible = gameObject.gameObject;
        }
      }
    }
    pooledList1.Recycle();
  }

  public class Def : StateMachine.BaseDef
  {
    public Diet diet;
  }

  public new class Instance : 
    GameStateMachine<SolidConsumerMonitor, SolidConsumerMonitor.Instance, IStateMachineTarget, SolidConsumerMonitor.Def>.GameInstance
  {
    public GameObject targetEdible;

    public Instance(IStateMachineTarget master, SolidConsumerMonitor.Def def)
      : base(master, def)
    {
    }

    public void OnEatSolidComplete(object data)
    {
      KPrefabID cmp = data as KPrefabID;
      if (Object.op_Equality((Object) cmp, (Object) null))
        return;
      PrimaryElement component1 = ((Component) cmp).GetComponent<PrimaryElement>();
      if (Object.op_Equality((Object) component1, (Object) null))
        return;
      Diet.Info dietInfo = this.def.diet.GetDietInfo(cmp.PrefabTag);
      if (dietInfo == null)
        return;
      AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(this.smi.gameObject);
      string properName = ((Component) cmp).GetProperName();
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, properName, ((KMonoBehaviour) cmp).transform);
      float calories1 = amountInstance.GetMax() - amountInstance.value;
      float consumptionMass = dietInfo.ConvertCaloriesToConsumptionMass(calories1);
      Growing component2 = ((Component) cmp).GetComponent<Growing>();
      float num1;
      if (Object.op_Inequality((Object) component2, (Object) null))
      {
        BuddingTrunk component3 = ((Component) cmp).GetComponent<BuddingTrunk>();
        if (Object.op_Implicit((Object) component3))
        {
          float maxBranchMaturity = component3.GetMaxBranchMaturity();
          num1 = Mathf.Min(consumptionMass, maxBranchMaturity);
          component3.ConsumeMass(num1);
        }
        else
        {
          float num2 = Db.Get().Amounts.Maturity.Lookup(((Component) component2).gameObject).value;
          num1 = Mathf.Min(consumptionMass, num2);
          component2.ConsumeMass(num1);
        }
      }
      else
      {
        num1 = Mathf.Min(consumptionMass, component1.Mass);
        component1.Mass -= num1;
        Pickupable component4 = ((Component) component1).GetComponent<Pickupable>();
        if (Object.op_Inequality((Object) component4.storage, (Object) null))
        {
          component4.storage.Trigger(-1452790913, (object) this.gameObject);
          component4.storage.Trigger(-1697596308, (object) this.gameObject);
        }
      }
      float calories2 = dietInfo.ConvertConsumptionMassToCalories(num1);
      this.Trigger(-2038961714, (object) new CreatureCalorieMonitor.CaloriesConsumedEvent()
      {
        tag = cmp.PrefabTag,
        calories = calories2
      });
      this.targetEdible = (GameObject) null;
    }
  }
}
