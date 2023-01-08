// Decompiled with JetBrains decompiler
// Type: Growing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Growing : StateMachineComponent<Growing.StatesInstance>, IGameObjectEffectDescriptor
{
  public bool shouldGrowOld = true;
  public float maxAge = 2400f;
  private AmountInstance maturity;
  private AmountInstance oldAge;
  [MyCmpGet]
  private WiltCondition wiltCondition;
  [MyCmpReq]
  private KSelectable selectable;
  [MyCmpReq]
  private Modifiers modifiers;
  [MyCmpReq]
  private ReceptacleMonitor rm;
  private Crop _crop;
  private static readonly EventSystem.IntraObjectHandler<Growing> OnNewGameSpawnDelegate = new EventSystem.IntraObjectHandler<Growing>((Action<Growing, object>) ((component, data) => component.OnNewGameSpawn(data)));
  private static readonly EventSystem.IntraObjectHandler<Growing> ResetGrowthDelegate = new EventSystem.IntraObjectHandler<Growing>((Action<Growing, object>) ((component, data) => component.ResetGrowth(data)));

  private Crop crop
  {
    get
    {
      if (Object.op_Equality((Object) this._crop, (Object) null))
        this._crop = ((Component) this).GetComponent<Crop>();
      return this._crop;
    }
  }

  protected virtual void OnPrefabInit()
  {
    Amounts amounts = ((Component) this).gameObject.GetAmounts();
    this.maturity = amounts.Get(Db.Get().Amounts.Maturity);
    this.oldAge = amounts.Add(new AmountInstance(Db.Get().Amounts.OldAge, ((Component) this).gameObject));
    this.oldAge.maxAttribute.ClearModifiers();
    this.oldAge.maxAttribute.Add(new AttributeModifier(Db.Get().Amounts.OldAge.maxAttribute.Id, this.maxAge));
    base.OnPrefabInit();
    this.Subscribe<Growing>(1119167081, Growing.OnNewGameSpawnDelegate);
    this.Subscribe<Growing>(1272413801, Growing.ResetGrowthDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    ((Component) this).gameObject.AddTag(GameTags.GrowingPlant);
  }

  private void OnNewGameSpawn(object data)
  {
    double num = (double) this.maturity.SetValue(this.maturity.maxAttribute.GetTotalValue() * Random.Range(0.0f, 1f));
  }

  public void OverrideMaturityLevel(float percent)
  {
    double num = (double) this.maturity.SetValue(this.maturity.GetMax() * percent);
  }

  public bool ReachedNextHarvest() => (double) this.PercentOfCurrentHarvest() >= 1.0;

  public bool IsGrown() => (double) this.maturity.value == (double) this.maturity.GetMax();

  public bool CanGrow() => !this.IsGrown();

  public bool IsGrowing() => (double) this.maturity.GetDelta() > 0.0;

  public void ClampGrowthToHarvest() => this.maturity.value = this.maturity.GetMax();

  public float GetMaxMaturity() => this.maturity.GetMax();

  public float PercentOfCurrentHarvest() => this.maturity.value / this.maturity.GetMax();

  public float TimeUntilNextHarvest() => (this.maturity.GetMax() - this.maturity.value) / this.maturity.GetDelta();

  public float DomesticGrowthTime() => this.maturity.GetMax() / this.smi.baseGrowingRate.Value;

  public float WildGrowthTime() => this.maturity.GetMax() / this.smi.wildGrowingRate.Value;

  public float PercentGrown() => this.maturity.value / this.maturity.GetMax();

  public void ResetGrowth(object data = null) => this.maturity.value = 0.0f;

  public float PercentOldAge() => !this.shouldGrowOld ? 0.0f : this.oldAge.value / this.oldAge.GetMax();

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    Klei.AI.Attribute maxAttribute = Db.Get().Amounts.Maturity.maxAttribute;
    descriptors.Add(new Descriptor(go.GetComponent<Modifiers>().GetPreModifiedAttributeDescription(maxAttribute), go.GetComponent<Modifiers>().GetPreModifiedAttributeToolTip(maxAttribute), (Descriptor.DescriptorType) 0, false));
    return descriptors;
  }

  public void ConsumeMass(float mass_to_consume)
  {
    float num = this.maturity.value;
    mass_to_consume = Mathf.Min(mass_to_consume, num);
    this.maturity.value -= mass_to_consume;
    EventExtensions.Trigger(((Component) this).gameObject, -1793167409, (object) null);
  }

  public class StatesInstance : 
    GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.GameInstance
  {
    public AttributeModifier baseGrowingRate;
    public AttributeModifier wildGrowingRate;
    public AttributeModifier getOldRate;

    public StatesInstance(Growing master)
      : base(master)
    {
      this.baseGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 1f / 600f, (string) CREATURES.STATS.MATURITY.GROWING);
      this.wildGrowingRate = new AttributeModifier(master.maturity.deltaAttribute.Id, 0.000416666677f, (string) CREATURES.STATS.MATURITY.GROWINGWILD);
      this.getOldRate = new AttributeModifier(master.oldAge.deltaAttribute.Id, master.shouldGrowOld ? 1f : 0.0f);
    }

    public bool IsGrown() => this.master.IsGrown();

    public bool ReachedNextHarvest() => this.master.ReachedNextHarvest();

    public void ClampGrowthToHarvest() => this.master.ClampGrowthToHarvest();

    public bool IsWilting() => Object.op_Inequality((Object) this.master.wiltCondition, (Object) null) && this.master.wiltCondition.IsWilting();

    public bool IsSleeping()
    {
      CropSleepingMonitor.Instance smi = ((Component) this.master).GetSMI<CropSleepingMonitor.Instance>();
      return smi != null && smi.IsSleeping();
    }

    public bool CanExitStalled() => !this.IsWilting() && !this.IsSleeping();
  }

  public class States : GameStateMachine<Growing.States, Growing.StatesInstance, Growing>
  {
    public Growing.States.GrowingStates growing;
    public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State stalled;
    public Growing.States.GrownStates grown;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.growing;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.growing.EventTransition(GameHashes.Wilt, this.stalled, (StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.Transition.ConditionCallback) (smi => smi.IsWilting())).EventTransition(GameHashes.CropSleep, this.stalled, (StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.Transition.ConditionCallback) (smi => smi.IsSleeping())).EventTransition(GameHashes.PlanterStorage, this.growing.planted, (StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.Transition.ConditionCallback) (smi => smi.master.rm.Replanted)).EventTransition(GameHashes.PlanterStorage, this.growing.wild, (StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.Transition.ConditionCallback) (smi => !smi.master.rm.Replanted)).TriggerOnEnter(GameHashes.Grow).Update("CheckGrown", (Action<Growing.StatesInstance, float>) ((smi, dt) =>
      {
        if (!smi.ReachedNextHarvest())
          return;
        smi.GoTo((StateMachine.BaseState) this.grown);
      }), (UpdateRate) 7).ToggleStatusItem(Db.Get().CreatureStatusItems.Growing, (Func<Growing.StatesInstance, object>) (smi => (object) ((Component) smi.master).GetComponent<Growing>())).Enter((StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State.Callback) (smi =>
      {
        GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State state = smi.master.rm.Replanted ? this.growing.planted : this.growing.wild;
        smi.GoTo((StateMachine.BaseState) state);
      }));
      this.growing.wild.ToggleAttributeModifier("GrowingWild", (Func<Growing.StatesInstance, AttributeModifier>) (smi => smi.wildGrowingRate));
      this.growing.planted.ToggleAttributeModifier(nameof (Growing), (Func<Growing.StatesInstance, AttributeModifier>) (smi => smi.baseGrowingRate));
      this.stalled.EventTransition(GameHashes.WiltRecover, (GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State) this.growing, (StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.Transition.ConditionCallback) (smi => smi.CanExitStalled())).EventTransition(GameHashes.CropWakeUp, (GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State) this.growing, (StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.Transition.ConditionCallback) (smi => smi.CanExitStalled()));
      double num1;
      this.grown.DefaultState(this.grown.idle).TriggerOnEnter(GameHashes.Grow).Update("CheckNotGrown", (Action<Growing.StatesInstance, float>) ((smi, dt) =>
      {
        if (smi.ReachedNextHarvest())
          return;
        smi.GoTo((StateMachine.BaseState) this.growing);
      }), (UpdateRate) 7).ToggleAttributeModifier("GettingOld", (Func<Growing.StatesInstance, AttributeModifier>) (smi => smi.getOldRate)).Enter((StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State.Callback) (smi => smi.ClampGrowthToHarvest())).Exit((StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State.Callback) (smi => num1 = (double) smi.master.oldAge.SetValue(0.0f)));
      this.grown.idle.Update("CheckNotGrown", (Action<Growing.StatesInstance, float>) ((smi, dt) =>
      {
        if (!smi.master.shouldGrowOld || (double) smi.master.oldAge.value < (double) smi.master.oldAge.GetMax())
          return;
        smi.GoTo((StateMachine.BaseState) this.grown.try_self_harvest);
      }), (UpdateRate) 7);
      this.grown.try_self_harvest.Enter((StateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State.Callback) (smi =>
      {
        Harvestable component = ((Component) smi.master).GetComponent<Harvestable>();
        if (Object.op_Implicit((Object) component) && component.CanBeHarvested)
        {
          int num2 = component.harvestDesignatable.HarvestWhenReady ? 1 : 0;
          component.ForceCancelHarvest();
          component.Harvest();
          if (num2 != 0 && Object.op_Inequality((Object) component, (Object) null))
            component.harvestDesignatable.SetHarvestWhenReady(true);
        }
        double num3 = (double) smi.master.maturity.SetValue(0.0f);
        double num4 = (double) smi.master.oldAge.SetValue(0.0f);
      })).GoTo(this.grown.idle);
    }

    public class GrowingStates : 
      GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State
    {
      public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State wild;
      public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State planted;
    }

    public class GrownStates : 
      GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State
    {
      public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State idle;
      public GameStateMachine<Growing.States, Growing.StatesInstance, Growing, object>.State try_self_harvest;
    }
  }
}
