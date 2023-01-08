// Decompiled with JetBrains decompiler
// Type: LiquidCooledFan
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class LiquidCooledFan : 
  StateMachineComponent<LiquidCooledFan.StatesInstance>,
  IGameObjectEffectDescriptor
{
  [SerializeField]
  public float coolingKilowatts;
  [SerializeField]
  public float minCooledTemperature;
  [SerializeField]
  public float minEnvironmentMass;
  [SerializeField]
  public float waterKGConsumedPerKJ;
  [SerializeField]
  public Vector2I minCoolingRange;
  [SerializeField]
  public Vector2I maxCoolingRange;
  private float flowRate = 0.3f;
  [SerializeField]
  public Storage gasStorage;
  [SerializeField]
  public Storage liquidStorage;
  [MyCmpAdd]
  private LiquidCooledFanWorkable workable;
  [MyCmpGet]
  private Operational operational;
  private HandleVector<int>.Handle waterConsumptionAccumulator = HandleVector<int>.InvalidHandle;
  private MeterController meter;

  public bool HasMaterial()
  {
    ListPool<GameObject, LiquidCooledFan>.PooledList result = ListPool<GameObject, LiquidCooledFan>.Allocate();
    this.smi.master.gasStorage.Find(GameTags.Water, (List<GameObject>) result);
    if (((List<GameObject>) result).Count > 0)
    {
      Debug.LogWarning((object) "Liquid Cooled fan Gas storage contains water - A duplicant probably delivered to the wrong storage - moving it to liquid storage.");
      foreach (GameObject go in (List<GameObject>) result)
        this.smi.master.gasStorage.Transfer(go, this.smi.master.liquidStorage);
    }
    result.Recycle();
    this.UpdateMeter();
    return (double) this.liquidStorage.MassStored() > 0.0;
  }

  public void CheckWorking()
  {
    if (!Object.op_Equality((Object) this.smi.master.workable.worker, (Object) null))
      return;
    this.smi.GoTo((StateMachine.BaseState) this.smi.sm.unworkable);
  }

  private void UpdateUnworkableStatusItems()
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (!this.smi.EnvironmentNeedsCooling())
    {
      if (!component.HasStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther))
        component.AddStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther);
    }
    else if (component.HasStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther))
      component.RemoveStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther);
    if (!this.smi.EnvironmentHighEnoughPressure())
    {
      if (component.HasStatusItem(Db.Get().BuildingStatusItems.UnderPressure))
        return;
      component.AddStatusItem(Db.Get().BuildingStatusItems.UnderPressure, (object) this.minEnvironmentMass);
    }
    else
    {
      if (!component.HasStatusItem(Db.Get().BuildingStatusItems.UnderPressure))
        return;
      component.RemoveStatusItem(Db.Get().BuildingStatusItems.UnderPressure);
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[3]
    {
      "meter_target",
      "meter_waterbody",
      "meter_waterlevel"
    });
    ((Component) this).GetComponent<ElementConsumer>().EnableConsumption(true);
    this.smi.StartSM();
    this.smi.master.waterConsumptionAccumulator = Game.Instance.accumulators.Add("waterConsumptionAccumulator", (KMonoBehaviour) this);
    ((Component) this).GetComponent<ElementConsumer>().storage = this.gasStorage;
    ((Component) this).GetComponent<ManualDeliveryKG>().SetStorage(this.liquidStorage);
  }

  private void UpdateMeter() => this.meter.SetPositionPercent(Mathf.Clamp01(this.liquidStorage.MassStored() / this.liquidStorage.capacityKg));

  private void EmitContents()
  {
    if (this.gasStorage.items.Count == 0)
      return;
    float num = 0.1f;
    PrimaryElement primaryElement = (PrimaryElement) null;
    for (int index = 0; index < this.gasStorage.items.Count; ++index)
    {
      PrimaryElement component = this.gasStorage.items[index].GetComponent<PrimaryElement>();
      if ((double) component.Mass > (double) num && component.Element.IsGas)
      {
        primaryElement = component;
        num = primaryElement.Mass;
      }
    }
    if (!Object.op_Inequality((Object) primaryElement, (Object) null))
      return;
    SimMessages.AddRemoveSubstance(Grid.CellRight(Grid.CellAbove(Grid.PosToCell(((Component) this).gameObject))), ElementLoader.GetElementIndex(primaryElement.ElementID), CellEventLogger.Instance.ExhaustSimUpdate, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
    this.gasStorage.ConsumeIgnoringDisease(((Component) primaryElement).gameObject);
  }

  private void CoolContents(float dt)
  {
    if (this.gasStorage.items.Count == 0)
      return;
    float num1 = float.PositiveInfinity;
    float num2 = 0.0f;
    foreach (GameObject gameObject in this.gasStorage.items)
    {
      PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
      if (!Object.op_Equality((Object) component, (Object) null) && (double) component.Mass >= 0.10000000149011612 && (double) component.Temperature >= (double) this.minCooledTemperature)
      {
        float thermalEnergy = GameUtil.GetThermalEnergy(component);
        if ((double) num1 > (double) thermalEnergy)
          num1 = thermalEnergy;
      }
    }
    foreach (GameObject gameObject in this.gasStorage.items)
    {
      PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
      if (!Object.op_Equality((Object) component, (Object) null) && (double) component.Mass >= 0.10000000149011612 && (double) component.Temperature >= (double) this.minCooledTemperature)
      {
        float num3 = Mathf.Min(num1, 10f);
        GameUtil.DeltaThermalEnergy(component, -num3, this.minCooledTemperature);
        num2 += num3;
      }
    }
    float amount = Mathf.Abs(num2 * this.waterKGConsumedPerKJ);
    Game.Instance.accumulators.Accumulate(this.smi.master.waterConsumptionAccumulator, amount);
    if ((double) amount == 0.0)
      return;
    SimUtil.DiseaseInfo disease_info;
    this.liquidStorage.ConsumeAndGetDisease(GameTags.Water, amount, out float _, out disease_info, out float _);
    SimMessages.ModifyDiseaseOnCell(Grid.PosToCell(((Component) this).gameObject), disease_info.idx, disease_info.count);
    this.UpdateMeter();
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.HEATCONSUMED, (object) GameUtil.GetFormattedHeatEnergy(this.coolingKilowatts)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.HEATCONSUMED, (object) GameUtil.GetFormattedHeatEnergy(this.coolingKilowatts)), (Descriptor.DescriptorType) 1);
    descriptors.Add(descriptor);
    return descriptors;
  }

  public class StatesInstance : 
    GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.GameInstance
  {
    public StatesInstance(LiquidCooledFan smi)
      : base(smi)
    {
    }

    public bool IsWorkable()
    {
      bool flag = false;
      if (this.master.operational.IsOperational && this.EnvironmentNeedsCooling() && this.smi.master.HasMaterial() && this.smi.EnvironmentHighEnoughPressure())
        flag = true;
      return flag;
    }

    public bool EnvironmentNeedsCooling()
    {
      bool flag = false;
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      for (int y = this.master.minCoolingRange.y; y < this.master.maxCoolingRange.y; ++y)
      {
        for (int x = this.master.minCoolingRange.x; x < this.master.maxCoolingRange.x; ++x)
        {
          CellOffset offset;
          // ISSUE: explicit constructor call
          ((CellOffset) ref offset).\u002Ector(x, y);
          int i = Grid.OffsetCell(cell, offset);
          if ((double) Grid.Temperature[i] > (double) this.master.minCooledTemperature)
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    public bool EnvironmentHighEnoughPressure()
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      for (int y = this.master.minCoolingRange.y; y < this.master.maxCoolingRange.y; ++y)
      {
        for (int x = this.master.minCoolingRange.x; x < this.master.maxCoolingRange.x; ++x)
        {
          CellOffset offset;
          // ISSUE: explicit constructor call
          ((CellOffset) ref offset).\u002Ector(x, y);
          int i = Grid.OffsetCell(cell, offset);
          if ((double) Grid.Mass[i] >= (double) this.master.minEnvironmentMass)
            return true;
        }
      }
      return false;
    }
  }

  public class States : 
    GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan>
  {
    public LiquidCooledFan.States.Workable workable;
    public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State unworkable;
    public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State work_pst;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.unworkable;
      this.root.Enter((StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State.Callback) (smi => smi.master.workable.SetWorkTime(float.PositiveInfinity)));
      this.workable.ToggleChore(new Func<LiquidCooledFan.StatesInstance, Chore>(this.CreateUseChore), this.work_pst).EventTransition(GameHashes.ActiveChanged, this.workable.consuming, (StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.master.workable.worker, (Object) null))).EventTransition(GameHashes.OperationalChanged, this.workable.consuming, (StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.master.workable.worker, (Object) null))).Transition(this.unworkable, (StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.Transition.ConditionCallback) (smi => !smi.IsWorkable()));
      this.work_pst.Update("LiquidFanEmitCooledContents", (Action<LiquidCooledFan.StatesInstance, float>) ((smi, dt) => smi.master.EmitContents())).ScheduleGoTo(2f, (StateMachine.BaseState) this.unworkable);
      this.unworkable.Update("LiquidFanEmitCooledContents", (Action<LiquidCooledFan.StatesInstance, float>) ((smi, dt) => smi.master.EmitContents())).Update("LiquidFanUnworkableStatusItems", (Action<LiquidCooledFan.StatesInstance, float>) ((smi, dt) => smi.master.UpdateUnworkableStatusItems())).Transition(this.workable.waiting, (StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.Transition.ConditionCallback) (smi => smi.IsWorkable())).Enter((StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State.Callback) (smi => smi.master.UpdateUnworkableStatusItems())).Exit((StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State.Callback) (smi => smi.master.UpdateUnworkableStatusItems()));
      this.workable.consuming.EventTransition(GameHashes.OperationalChanged, this.unworkable, (StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.Transition.ConditionCallback) (smi => Object.op_Equality((Object) smi.master.workable.worker, (Object) null))).EventHandler(GameHashes.ActiveChanged, (StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State.Callback) (smi => smi.master.CheckWorking())).Enter((StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State.Callback) (smi =>
      {
        if (!smi.EnvironmentNeedsCooling() || !smi.master.HasMaterial() || !smi.EnvironmentHighEnoughPressure())
          smi.GoTo((StateMachine.BaseState) this.unworkable);
        ElementConsumer component = ((Component) smi.master).GetComponent<ElementConsumer>();
        component.consumptionRate = smi.master.flowRate;
        component.RefreshConsumptionRate();
      })).Update((Action<LiquidCooledFan.StatesInstance, float>) ((smi, dt) => smi.master.CoolContents(dt))).ScheduleGoTo(12f, (StateMachine.BaseState) this.workable.emitting).Exit((StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State.Callback) (smi =>
      {
        ElementConsumer component = ((Component) smi.master).GetComponent<ElementConsumer>();
        component.consumptionRate = 0.0f;
        component.RefreshConsumptionRate();
      }));
      this.workable.emitting.EventTransition(GameHashes.ActiveChanged, this.unworkable, (StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.Transition.ConditionCallback) (smi => Object.op_Equality((Object) smi.master.workable.worker, (Object) null))).EventTransition(GameHashes.OperationalChanged, this.unworkable, (StateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.Transition.ConditionCallback) (smi => Object.op_Equality((Object) smi.master.workable.worker, (Object) null))).ScheduleGoTo(3f, (StateMachine.BaseState) this.workable.consuming).Update((Action<LiquidCooledFan.StatesInstance, float>) ((smi, dt) => smi.master.CoolContents(dt))).Update("LiquidFanEmitCooledContents", (Action<LiquidCooledFan.StatesInstance, float>) ((smi, dt) => smi.master.EmitContents()));
    }

    private Chore CreateUseChore(LiquidCooledFan.StatesInstance smi) => (Chore) new WorkChore<LiquidCooledFanWorkable>(Db.Get().ChoreTypes.LiquidCooledFan, (IStateMachineTarget) smi.master.workable);

    public class Workable : 
      GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State
    {
      public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State waiting;
      public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State consuming;
      public GameStateMachine<LiquidCooledFan.States, LiquidCooledFan.StatesInstance, LiquidCooledFan, object>.State emitting;
    }
  }
}
