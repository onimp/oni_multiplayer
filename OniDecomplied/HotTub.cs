// Decompiled with JetBrains decompiler
// Type: HotTub
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class HotTub : StateMachineComponent<HotTub.StatesInstance>, IGameObjectEffectDescriptor
{
  public string specificEffect;
  public string trackingEffect;
  public int basePriority;
  public CellOffset[] choreOffsets = new CellOffset[4]
  {
    new CellOffset(-1, 0),
    new CellOffset(1, 0),
    new CellOffset(0, 0),
    new CellOffset(2, 0)
  };
  private HotTubWorkable[] workables;
  private Chore[] chores;
  public HashSet<int> occupants = new HashSet<int>();
  public float waterCoolingRate;
  public float hotTubCapacity = 100f;
  public float minimumWaterTemperature;
  public float bleachStoneConsumption;
  public float maxOperatingTemperature;
  [MyCmpGet]
  public Storage waterStorage;
  private MeterController waterMeter;
  private MeterController tempMeter;

  public float PercentFull => 100f * this.waterStorage.GetMassAvailable(SimHashes.Water) / this.hotTubCapacity;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule)), (object) null, (SchedulerGroup) null);
    this.workables = new HotTubWorkable[this.choreOffsets.Length];
    this.chores = new Chore[this.choreOffsets.Length];
    for (int index = 0; index < this.workables.Length; ++index)
    {
      GameObject locator = ChoreHelpers.CreateLocator("HotTubWorkable", Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.choreOffsets[index]), Grid.SceneLayer.Move));
      KSelectable kselectable = locator.AddOrGet<KSelectable>();
      kselectable.SetName(((Component) this).GetProperName());
      kselectable.IsSelectable = false;
      HotTubWorkable hotTubWorkable1 = locator.AddOrGet<HotTubWorkable>();
      int player_index = index;
      HotTubWorkable hotTubWorkable2 = hotTubWorkable1;
      hotTubWorkable2.OnWorkableEventCB = hotTubWorkable2.OnWorkableEventCB + (Action<Workable, Workable.WorkableEvent>) ((workable, ev) => this.OnWorkableEvent(player_index, ev));
      this.workables[index] = hotTubWorkable1;
      this.workables[index].hotTub = this;
    }
    this.waterMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_water_target", "meter_water", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]
    {
      "meter_water_target"
    });
    this.smi.UpdateWaterMeter();
    this.tempMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_temperature_target", "meter_temp", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]
    {
      "meter_temperature_target"
    });
    this.smi.TestWaterTemperature();
    this.smi.StartSM();
  }

  protected override void OnCleanUp()
  {
    this.UpdateChores(false);
    for (int index = 0; index < this.workables.Length; ++index)
    {
      if (Object.op_Implicit((Object) this.workables[index]))
      {
        Util.KDestroyGameObject((Component) this.workables[index]);
        this.workables[index] = (HotTubWorkable) null;
      }
    }
    base.OnCleanUp();
  }

  private Chore CreateChore(int i)
  {
    Workable workable = (Workable) this.workables[i];
    ChoreType relax = Db.Get().ChoreTypes.Relax;
    Workable target = workable;
    ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
    Action<Chore> on_end = new Action<Chore>(this.OnSocialChoreEnd);
    ScheduleBlockType schedule_block = recreation;
    WorkChore<HotTubWorkable> chore = new WorkChore<HotTubWorkable>(relax, (IStateMachineTarget) target, on_end: on_end, allow_in_red_alert: false, schedule_block: schedule_block, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
    chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object) workable);
    return (Chore) chore;
  }

  private void OnSocialChoreEnd(Chore chore)
  {
    if (!((Component) this).gameObject.HasTag(GameTags.Operational))
      return;
    this.UpdateChores();
  }

  public void UpdateChores(bool update = true)
  {
    for (int i = 0; i < this.choreOffsets.Length; ++i)
    {
      Chore chore = this.chores[i];
      if (update)
      {
        if (chore == null || chore.isComplete)
          this.chores[i] = this.CreateChore(i);
      }
      else if (chore != null)
      {
        chore.Cancel("locator invalidated");
        this.chores[i] = (Chore) null;
      }
    }
  }

  public void OnWorkableEvent(int player, Workable.WorkableEvent ev)
  {
    if (ev == Workable.WorkableEvent.WorkStarted)
      this.occupants.Add(player);
    else
      this.occupants.Remove(player);
    this.smi.sm.userCount.Set(this.occupants.Count, this.smi);
  }

  List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
  {
    List<Descriptor> descs = new List<Descriptor>();
    Element elementByHash = ElementLoader.FindElementByHash(SimHashes.Water);
    descs.Add(new Descriptor(BUILDINGS.PREFABS.HOTTUB.WATER_REQUIREMENT.Replace("{element}", elementByHash.name).Replace("{amount}", GameUtil.GetFormattedMass(this.hotTubCapacity)), BUILDINGS.PREFABS.HOTTUB.WATER_REQUIREMENT_TOOLTIP.Replace("{element}", elementByHash.name).Replace("{amount}", GameUtil.GetFormattedMass(this.hotTubCapacity)), (Descriptor.DescriptorType) 0, false));
    descs.Add(new Descriptor(BUILDINGS.PREFABS.HOTTUB.TEMPERATURE_REQUIREMENT.Replace("{element}", elementByHash.name).Replace("{temperature}", GameUtil.GetFormattedTemperature(this.minimumWaterTemperature)), BUILDINGS.PREFABS.HOTTUB.TEMPERATURE_REQUIREMENT_TOOLTIP.Replace("{element}", elementByHash.name).Replace("{temperature}", GameUtil.GetFormattedTemperature(this.minimumWaterTemperature)), (Descriptor.DescriptorType) 0, false));
    descs.Add(new Descriptor((string) UI.BUILDINGEFFECTS.RECREATION, (string) UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, (Descriptor.DescriptorType) 1, false));
    Effect.AddModifierDescriptions(((Component) this).gameObject, descs, this.specificEffect, true);
    return descs;
  }

  public class States : GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub>
  {
    public StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.IntParameter userCount;
    public StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.BoolParameter waterTooCold;
    public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State unoperational;
    public HotTub.States.OffStates off;
    public HotTub.States.ReadyStates ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.ready;
      this.root.Update((Action<HotTub.StatesInstance, float>) ((smi, dt) =>
      {
        smi.SapHeatFromWater(dt);
        smi.TestWaterTemperature();
      }), (UpdateRate) 7).EventHandler(GameHashes.OnStorageChange, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State.Callback) (smi =>
      {
        smi.UpdateWaterMeter();
        smi.TestWaterTemperature();
      }));
      this.unoperational.TagTransition(GameTags.Operational, (GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State) this.off).PlayAnim("off");
      this.off.TagTransition(GameTags.Operational, this.unoperational, true).DefaultState((GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State) this.off.filling);
      this.off.filling.DefaultState(this.off.filling.normal).Transition((GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State) this.ready, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Transition.ConditionCallback) (smi => (double) smi.master.waterStorage.GetMassAvailable(SimHashes.Water) >= (double) smi.master.hotTubCapacity)).PlayAnim("off").Enter((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State.Callback) (smi => smi.GetComponent<ConduitConsumer>().SetOnState(true))).Exit((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State.Callback) (smi => smi.GetComponent<ConduitConsumer>().SetOnState(false))).ToggleMainStatusItem(Db.Get().BuildingStatusItems.HotTubFilling, (Func<HotTub.StatesInstance, object>) (smi => (object) smi.master));
      this.off.filling.normal.ParamTransition<bool>((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<bool>) this.waterTooCold, this.off.filling.too_cold, GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.IsTrue);
      this.off.filling.too_cold.ParamTransition<bool>((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<bool>) this.waterTooCold, this.off.filling.normal, GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.IsFalse).ToggleStatusItem(Db.Get().BuildingStatusItems.HotTubWaterTooCold, (Func<HotTub.StatesInstance, object>) (smi => (object) smi.master));
      this.off.draining.Transition((GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State) this.off.filling, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Transition.ConditionCallback) (smi => (double) smi.master.waterStorage.GetMassAvailable(SimHashes.Water) <= 0.0)).ToggleMainStatusItem(Db.Get().BuildingStatusItems.HotTubWaterTooCold, (Func<HotTub.StatesInstance, object>) (smi => (object) smi.master)).PlayAnim("off").Enter((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State.Callback) (smi => smi.GetComponent<ConduitDispenser>().SetOnState(true))).Exit((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State.Callback) (smi => smi.GetComponent<ConduitDispenser>().SetOnState(false)));
      this.off.too_hot.Transition((GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State) this.ready, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Transition.ConditionCallback) (smi => !smi.IsTubTooHot())).PlayAnim("overheated").ToggleMainStatusItem(Db.Get().BuildingStatusItems.HotTubTooHot, (Func<HotTub.StatesInstance, object>) (smi => (object) smi.master));
      this.off.awaiting_delivery.EventTransition(GameHashes.OnStorageChange, (GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State) this.ready, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Transition.ConditionCallback) (smi => smi.HasBleachStone()));
      this.ready.DefaultState(this.ready.idle).Enter("CreateChore", (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State.Callback) (smi => smi.master.UpdateChores())).Exit("CancelChore", (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State.Callback) (smi => smi.master.UpdateChores(false))).TagTransition(GameTags.Operational, this.unoperational, true).ParamTransition<bool>((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<bool>) this.waterTooCold, this.off.draining, GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.IsTrue).EventTransition(GameHashes.OnStorageChange, this.off.awaiting_delivery, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Transition.ConditionCallback) (smi => !smi.HasBleachStone())).Transition((GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State) this.off.filling, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Transition.ConditionCallback) (smi => smi.master.waterStorage.IsEmpty())).Transition(this.off.too_hot, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Transition.ConditionCallback) (smi => smi.IsTubTooHot())).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal);
      this.ready.idle.PlayAnim("on").ParamTransition<int>((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<int>) this.userCount, this.ready.on.pre, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<int>.Callback) ((smi, p) => p > 0));
      this.ready.on.Enter((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State.Callback) (smi => smi.SetActive(true))).Update((Action<HotTub.StatesInstance, float>) ((smi, dt) => smi.ConsumeBleachstone(dt)), (UpdateRate) 7).Exit((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State.Callback) (smi => smi.SetActive(false)));
      this.ready.on.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.ready.on.relaxing);
      this.ready.on.relaxing.PlayAnim("working_loop", (KAnim.PlayMode) 0).ParamTransition<int>((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<int>) this.userCount, this.ready.on.post, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<int>.Callback) ((smi, p) => p == 0)).ParamTransition<int>((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<int>) this.userCount, this.ready.on.relaxing_together, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<int>.Callback) ((smi, p) => p > 1));
      this.ready.on.relaxing_together.PlayAnim("working_loop", (KAnim.PlayMode) 0).ParamTransition<int>((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<int>) this.userCount, this.ready.on.post, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<int>.Callback) ((smi, p) => p == 0)).ParamTransition<int>((StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<int>) this.userCount, this.ready.on.relaxing, (StateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.Parameter<int>.Callback) ((smi, p) => p == 1));
      this.ready.on.post.PlayAnim("working_pst").OnAnimQueueComplete(this.ready.idle);
    }

    private string GetRelaxingAnim(HotTub.StatesInstance smi)
    {
      bool flag1 = smi.master.occupants.Contains(0);
      bool flag2 = smi.master.occupants.Contains(1);
      if (flag1 && !flag2)
        return "working_loop_one_p";
      return flag2 && !flag1 ? "working_loop_two_p" : "working_loop_coop_p";
    }

    public class OffStates : 
      GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State
    {
      public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State draining;
      public HotTub.States.FillingStates filling;
      public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State too_hot;
      public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State awaiting_delivery;
    }

    public class OnStates : 
      GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State
    {
      public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State pre;
      public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State relaxing;
      public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State relaxing_together;
      public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State post;
    }

    public class ReadyStates : 
      GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State
    {
      public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State idle;
      public HotTub.States.OnStates on;
    }

    public class FillingStates : 
      GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State
    {
      public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State normal;
      public GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.State too_cold;
    }
  }

  public class StatesInstance : 
    GameStateMachine<HotTub.States, HotTub.StatesInstance, HotTub, object>.GameInstance
  {
    private Operational operational;

    public StatesInstance(HotTub smi)
      : base(smi)
    {
      this.operational = ((Component) this.master).GetComponent<Operational>();
    }

    public void SetActive(bool active) => this.operational.SetActive(this.operational.IsOperational & active);

    public void UpdateWaterMeter() => this.smi.master.waterMeter.SetPositionPercent(Mathf.Clamp(this.smi.master.waterStorage.GetMassAvailable(SimHashes.Water) / this.smi.master.hotTubCapacity, 0.0f, 1f));

    public void UpdateTemperatureMeter(float waterTemp)
    {
      Element element = ElementLoader.GetElement(SimHashes.Water.CreateTag());
      this.smi.master.tempMeter.SetPositionPercent(Mathf.Clamp((float) (((double) waterTemp - (double) this.smi.master.minimumWaterTemperature) / ((double) element.highTemp - (double) this.smi.master.minimumWaterTemperature)), 0.0f, 1f));
    }

    public void TestWaterTemperature()
    {
      GameObject first = this.smi.master.waterStorage.FindFirst(new Tag(1836671383));
      float waterTemp = 0.0f;
      if (Object.op_Implicit((Object) first))
      {
        float temperature = first.GetComponent<PrimaryElement>().Temperature;
        this.UpdateTemperatureMeter(temperature);
        if ((double) temperature < (double) this.smi.master.minimumWaterTemperature)
          this.smi.sm.waterTooCold.Set(true, this.smi);
        else
          this.smi.sm.waterTooCold.Set(false, this.smi);
      }
      else
      {
        this.UpdateTemperatureMeter(waterTemp);
        this.smi.sm.waterTooCold.Set(false, this.smi);
      }
    }

    public bool IsTubTooHot() => (double) ((Component) this.smi.master).GetComponent<PrimaryElement>().Temperature > (double) this.smi.master.maxOperatingTemperature;

    public bool HasBleachStone()
    {
      GameObject first = this.smi.master.waterStorage.FindFirst(new Tag(-839728230));
      return Object.op_Inequality((Object) first, (Object) null) && (double) first.GetComponent<PrimaryElement>().Mass > 0.0;
    }

    public void SapHeatFromWater(float dt)
    {
      float kilowatts = this.smi.master.waterCoolingRate * dt / (float) this.smi.master.waterStorage.items.Count;
      foreach (GameObject gameObject in this.smi.master.waterStorage.items)
      {
        GameUtil.DeltaThermalEnergy(gameObject.GetComponent<PrimaryElement>(), -kilowatts, this.smi.master.minimumWaterTemperature);
        GameUtil.DeltaThermalEnergy(this.GetComponent<PrimaryElement>(), kilowatts, this.GetComponent<PrimaryElement>().Element.highTemp);
      }
    }

    public void ConsumeBleachstone(float dt) => this.smi.master.waterStorage.ConsumeIgnoringDisease(new Tag(-839728230), this.smi.master.bleachStoneConsumption * dt);
  }
}
