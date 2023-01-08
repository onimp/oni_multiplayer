// Decompiled with JetBrains decompiler
// Type: CritterTrapPlant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CritterTrapPlant : StateMachineComponent<CritterTrapPlant.StatesInstance>
{
  [MyCmpReq]
  private Crop crop;
  [MyCmpReq]
  private WiltCondition wiltCondition;
  [MyCmpReq]
  private ReceptacleMonitor rm;
  [MyCmpReq]
  private Growing growing;
  [MyCmpReq]
  private KAnimControllerBase animController;
  [MyCmpReq]
  private Harvestable harvestable;
  [MyCmpReq]
  private Storage storage;
  public float gasOutputRate;
  public float gasVentThreshold;
  public SimHashes outputElement;
  private float GAS_TEMPERATURE_DELTA = 10f;
  private static readonly EventSystem.IntraObjectHandler<CritterTrapPlant> OnUprootedDelegate = new EventSystem.IntraObjectHandler<CritterTrapPlant>((Action<CritterTrapPlant, object>) ((component, data) => component.OnUprooted(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Behaviour) this.smi.master.growing).enabled = false;
    this.Subscribe<CritterTrapPlant>(-216549700, CritterTrapPlant.OnUprootedDelegate);
    this.smi.StartSM();
  }

  public void RefreshPositionPercent() => this.animController.SetPositionPercent(this.growing.PercentOfCurrentHarvest());

  private void OnUprooted(object data = null)
  {
    GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.PlantDeathId)), TransformExtensions.GetPosition(((Component) this).gameObject.transform), Grid.SceneLayer.FXFront).SetActive(true);
    EventExtensions.Trigger(((Component) this).gameObject, 1623392196, (object) null);
    ((Component) this).gameObject.GetComponent<KBatchedAnimController>().StopAndClear();
    Object.Destroy((Object) ((Component) this).gameObject.GetComponent<KBatchedAnimController>());
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  protected void DestroySelf(object callbackParam)
  {
    CreatureHelpers.DeselectCreature(((Component) this).gameObject);
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  public Notification CreateDeathNotification() => new Notification((string) CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false)), (object) ("/t• " + ((Component) this).gameObject.GetProperName()));

  public class StatesInstance : 
    GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.GameInstance
  {
    public StatesInstance(CritterTrapPlant master)
      : base(master)
    {
    }

    public void OnTrapTriggered(object data) => this.smi.sm.trapTriggered.Trigger(this.smi);

    public void AddGas(float dt)
    {
      float temperature = this.smi.GetComponent<PrimaryElement>().Temperature + this.smi.master.GAS_TEMPERATURE_DELTA;
      this.smi.master.storage.AddGasChunk(this.smi.master.outputElement, this.smi.master.gasOutputRate * dt, temperature, byte.MaxValue, 0, false);
      if (!this.ShouldVentGas())
        return;
      this.smi.sm.ventGas.Trigger(this.smi);
    }

    public void VentGas()
    {
      PrimaryElement primaryElement = this.smi.master.storage.FindPrimaryElement(this.smi.master.outputElement);
      if (!Object.op_Inequality((Object) primaryElement, (Object) null))
        return;
      SimMessages.AddRemoveSubstance(Grid.PosToCell(TransformExtensions.GetPosition(this.smi.transform)), primaryElement.ElementID, CellEventLogger.Instance.Dumpable, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
      this.smi.master.storage.ConsumeIgnoringDisease(((Component) primaryElement).gameObject);
    }

    public bool ShouldVentGas()
    {
      PrimaryElement primaryElement = this.smi.master.storage.FindPrimaryElement(this.smi.master.outputElement);
      return !Object.op_Equality((Object) primaryElement, (Object) null) && (double) primaryElement.Mass >= (double) this.smi.master.gasVentThreshold;
    }
  }

  public class States : 
    GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant>
  {
    public StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Signal trapTriggered;
    public StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Signal ventGas;
    public StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.BoolParameter hasEatenCreature;
    public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State dead;
    public CritterTrapPlant.States.FruitingStates fruiting;
    public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State harvest;
    public CritterTrapPlant.States.TrapStates trap;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      default_state = (StateMachine.BaseState) this.trap;
      this.trap.DefaultState(this.trap.open);
      this.trap.open.ToggleComponent<TrapTrigger>().Enter((StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State.Callback) (smi =>
      {
        smi.VentGas();
        smi.master.storage.ConsumeAllIgnoringDisease();
      })).EventHandler(GameHashes.TrapTriggered, (GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.GameEvent.Callback) ((smi, data) => smi.OnTrapTriggered(data))).EventTransition(GameHashes.Wilt, this.trap.wilting).OnSignal(this.trapTriggered, this.trap.trigger).ParamTransition<bool>((StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Parameter<bool>) this.hasEatenCreature, (GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State) this.trap.digesting, GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.IsTrue).PlayAnim("idle_open", (KAnim.PlayMode) 0);
      this.trap.trigger.PlayAnim("trap", (KAnim.PlayMode) 1).Enter((StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State.Callback) (smi =>
      {
        smi.master.storage.ConsumeAllIgnoringDisease();
        smi.sm.hasEatenCreature.Set(true, smi);
      })).OnAnimQueueComplete((GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State) this.trap.digesting);
      this.trap.digesting.PlayAnim("digesting_loop", (KAnim.PlayMode) 0).ToggleComponent<Growing>().EventTransition(GameHashes.Grow, this.fruiting.enter, (StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Transition.ConditionCallback) (smi => smi.master.growing.ReachedNextHarvest())).EventTransition(GameHashes.Wilt, this.trap.wilting).DefaultState(this.trap.digesting.idle);
      this.trap.digesting.idle.PlayAnim("digesting_loop", (KAnim.PlayMode) 0).Update((Action<CritterTrapPlant.StatesInstance, float>) ((smi, dt) => smi.AddGas(dt)), (UpdateRate) 7).OnSignal(this.ventGas, this.trap.digesting.vent_pre);
      this.trap.digesting.vent_pre.PlayAnim("vent_pre").Exit((StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State.Callback) (smi => smi.VentGas())).OnAnimQueueComplete(this.trap.digesting.vent);
      this.trap.digesting.vent.PlayAnim("vent_loop", (KAnim.PlayMode) 1).QueueAnim("vent_pst").OnAnimQueueComplete(this.trap.digesting.idle);
      this.trap.wilting.PlayAnim("wilt1", (KAnim.PlayMode) 0).EventTransition(GameHashes.WiltRecover, (GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State) this.trap, (StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Transition.ConditionCallback) (smi => !smi.master.wiltCondition.IsWilting()));
      this.fruiting.EventTransition(GameHashes.Wilt, this.fruiting.wilting, (StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Transition.ConditionCallback) null).EventTransition(GameHashes.Harvest, this.harvest).DefaultState(this.fruiting.idle);
      this.fruiting.enter.PlayAnim("open_harvest", (KAnim.PlayMode) 1).Exit((StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State.Callback) (smi =>
      {
        smi.VentGas();
        smi.master.storage.ConsumeAllIgnoringDisease();
      })).OnAnimQueueComplete(this.fruiting.idle);
      this.fruiting.idle.PlayAnim("harvestable_loop", (KAnim.PlayMode) 1).Enter((StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) smi.master.harvestable, (Object) null))
          return;
        smi.master.harvestable.SetCanBeHarvested(true);
      })).Transition(this.fruiting.old, new StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Transition.ConditionCallback(this.IsOld), (UpdateRate) 7);
      this.fruiting.old.PlayAnim("wilt1", (KAnim.PlayMode) 1).Enter((StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) smi.master.harvestable, (Object) null))
          return;
        smi.master.harvestable.SetCanBeHarvested(true);
      })).Transition(this.fruiting.idle, GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Not(new StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Transition.ConditionCallback(this.IsOld)), (UpdateRate) 7);
      this.fruiting.wilting.PlayAnim("wilt1", (KAnim.PlayMode) 1).EventTransition(GameHashes.WiltRecover, (GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State) this.fruiting, (StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.Transition.ConditionCallback) (smi => !smi.master.wiltCondition.IsWilting()));
      this.harvest.PlayAnim("harvest", (KAnim.PlayMode) 1).Enter((StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State.Callback) (smi =>
      {
        if (Object.op_Inequality((Object) GameScheduler.Instance, (Object) null) && Object.op_Inequality((Object) smi.master, (Object) null))
          GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, new Action<object>(smi.master.crop.SpawnConfiguredFruit), (object) null, (SchedulerGroup) null);
        smi.master.harvestable.SetCanBeHarvested(false);
      })).Exit((StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State.Callback) (smi => smi.sm.hasEatenCreature.Set(false, smi))).OnAnimQueueComplete(this.trap.open);
      this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).Enter((StateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State.Callback) (smi =>
      {
        if (smi.master.rm.Replanted && !((Component) smi.master).GetComponent<KPrefabID>().HasTag(GameTags.Uprooted))
          ((Component) smi.master).gameObject.AddOrGet<Notifier>().Add(smi.master.CreateDeathNotification());
        GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.PlantDeathId)), TransformExtensions.GetPosition(smi.master.transform), Grid.SceneLayer.FXFront).SetActive(true);
        Harvestable harvestable = smi.master.harvestable;
        if (Object.op_Inequality((Object) harvestable, (Object) null) && harvestable.CanBeHarvested && Object.op_Inequality((Object) GameScheduler.Instance, (Object) null))
          GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, new Action<object>(smi.master.crop.SpawnConfiguredFruit), (object) null, (SchedulerGroup) null);
        smi.master.Trigger(1623392196, (object) null);
        ((Component) smi.master).GetComponent<KBatchedAnimController>().StopAndClear();
        Object.Destroy((Object) ((Component) smi.master).GetComponent<KBatchedAnimController>());
        smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), (object) null);
      }));
    }

    public bool IsOld(CritterTrapPlant.StatesInstance smi) => (double) smi.master.growing.PercentOldAge() > 0.5;

    public class DigestingStates : 
      GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State
    {
      public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State idle;
      public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State vent_pre;
      public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State vent;
    }

    public class TrapStates : 
      GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State
    {
      public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State open;
      public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State trigger;
      public CritterTrapPlant.States.DigestingStates digesting;
      public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State wilting;
    }

    public class FruitingStates : 
      GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State
    {
      public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State enter;
      public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State idle;
      public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State old;
      public GameStateMachine<CritterTrapPlant.States, CritterTrapPlant.StatesInstance, CritterTrapPlant, object>.State wilting;
    }
  }
}
