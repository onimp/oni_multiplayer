// Decompiled with JetBrains decompiler
// Type: HiveEatingStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using UnityEngine;

public class HiveEatingStates : 
  GameStateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>
{
  public HiveEatingStates.EatingStates eating;
  public GameStateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.eating;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.eating.ToggleStatusItem((string) CREATURES.STATUSITEMS.HIVE_DIGESTING.NAME, (string) CREATURES.STATUSITEMS.HIVE_DIGESTING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).DefaultState(this.eating.pre).Enter((StateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>.State.Callback) (smi => smi.TurnOn())).Exit((StateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>.State.Callback) (smi => smi.TurnOff()));
    this.eating.pre.PlayAnim("eating_pre", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.eating.loop);
    this.eating.loop.PlayAnim("eating_loop", (KAnim.PlayMode) 0).Update((System.Action<HiveEatingStates.Instance, float>) ((smi, dt) => smi.EatOreFromStorage(smi, dt)), (UpdateRate) 7).EventTransition(GameHashes.OnStorageChange, this.eating.pst, (StateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>.Transition.ConditionCallback) (smi => !Object.op_Implicit((Object) smi.storage.FindFirst(smi.def.consumedOre))));
    this.eating.pst.PlayAnim("eating_pst", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToEat);
  }

  public class Def : StateMachine.BaseDef
  {
    public Tag consumedOre;

    public Def(Tag consumedOre) => this.consumedOre = consumedOre;
  }

  public class EatingStates : 
    GameStateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>.State
  {
    public GameStateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>.State pre;
    public GameStateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>.State loop;
    public GameStateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>.State pst;
  }

  public new class Instance : 
    GameStateMachine<HiveEatingStates, HiveEatingStates.Instance, IStateMachineTarget, HiveEatingStates.Def>.GameInstance
  {
    [MyCmpReq]
    public Storage storage;
    [MyCmpReq]
    private RadiationEmitter emitter;

    public Instance(Chore<HiveEatingStates.Instance> chore, HiveEatingStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.WantsToEat);
    }

    public void TurnOn()
    {
      this.emitter.emitRads = 600f * this.emitter.emitRate;
      this.emitter.Refresh();
    }

    public void TurnOff()
    {
      this.emitter.emitRads = 0.0f;
      this.emitter.Refresh();
    }

    public void EatOreFromStorage(HiveEatingStates.Instance smi, float dt)
    {
      GameObject first = smi.storage.FindFirst(smi.def.consumedOre);
      if (!Object.op_Implicit((Object) first))
        return;
      float num1 = 0.25f;
      KPrefabID component1 = first.GetComponent<KPrefabID>();
      if (Object.op_Equality((Object) component1, (Object) null))
        return;
      PrimaryElement component2 = ((Component) component1).GetComponent<PrimaryElement>();
      if (Object.op_Equality((Object) component2, (Object) null))
        return;
      Diet.Info dietInfo = smi.gameObject.AddOrGetDef<BeehiveCalorieMonitor.Def>().diet.GetDietInfo(component1.PrefabTag);
      if (dietInfo == null)
        return;
      AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(smi.gameObject);
      float calories1 = amountInstance.GetMax() - amountInstance.value;
      float consumptionMass = dietInfo.ConvertCaloriesToConsumptionMass(calories1);
      float num2 = num1 * dt;
      if ((double) consumptionMass < (double) num2)
        num2 = consumptionMass;
      float mass = Mathf.Min(num2, component2.Mass);
      component2.Mass -= mass;
      Pickupable component3 = ((Component) component2).GetComponent<Pickupable>();
      if (Object.op_Inequality((Object) component3.storage, (Object) null))
      {
        component3.storage.Trigger(-1452790913, (object) smi.gameObject);
        component3.storage.Trigger(-1697596308, (object) smi.gameObject);
      }
      float calories2 = dietInfo.ConvertConsumptionMassToCalories(mass);
      CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = new CreatureCalorieMonitor.CaloriesConsumedEvent()
      {
        tag = component1.PrefabTag,
        calories = calories2
      };
      EventExtensions.Trigger(smi.gameObject, -2038961714, (object) caloriesConsumedEvent);
    }
  }
}
