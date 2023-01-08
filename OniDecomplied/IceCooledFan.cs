// Decompiled with JetBrains decompiler
// Type: IceCooledFan
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class IceCooledFan : StateMachineComponent<IceCooledFan.StatesInstance>
{
  [SerializeField]
  public float minCooledTemperature;
  [SerializeField]
  public float minEnvironmentMass;
  [SerializeField]
  public float coolingRate;
  [SerializeField]
  public float targetTemperature;
  [SerializeField]
  public Vector2I minCoolingRange;
  [SerializeField]
  public Vector2I maxCoolingRange;
  [SerializeField]
  public Storage iceStorage;
  [SerializeField]
  public Storage liquidStorage;
  [SerializeField]
  public Tag consumptionTag;
  private float LOW_ICE_TEMP = 173.15f;
  [MyCmpAdd]
  private IceCooledFanWorkable workable;
  [MyCmpGet]
  private Operational operational;
  private MeterController meter;

  public bool HasMaterial()
  {
    this.UpdateMeter();
    return (double) this.iceStorage.MassStored() > 0.0;
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
        component.AddStatusItem(Db.Get().BuildingStatusItems.CannotCoolFurther, (object) this.minCooledTemperature);
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
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[3]
    {
      "meter_target",
      "meter_waterbody",
      "meter_waterlevel"
    });
    this.smi.StartSM();
    ((Component) this).GetComponent<ManualDeliveryKG>().SetStorage(this.iceStorage);
  }

  private void UpdateMeter()
  {
    float num = 0.0f;
    foreach (GameObject gameObject in this.iceStorage.items)
    {
      PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
      num += component.Temperature;
    }
    this.meter.SetPositionPercent(1f - Mathf.Clamp01((float) (((double) (num / (float) this.iceStorage.items.Count) - (double) this.LOW_ICE_TEMP) / ((double) this.targetTemperature - (double) this.LOW_ICE_TEMP))));
  }

  private void DoCooling(float dt)
  {
    float kilowatts = this.coolingRate * dt;
    foreach (GameObject gameObject in this.iceStorage.items)
      GameUtil.DeltaThermalEnergy(gameObject.GetComponent<PrimaryElement>(), kilowatts, this.targetTemperature);
    for (int count = this.iceStorage.items.Count; count > 0; --count)
    {
      GameObject item_go = this.iceStorage.items[count - 1];
      if (Object.op_Inequality((Object) item_go, (Object) null) && (double) item_go.GetComponent<PrimaryElement>().Temperature > (double) item_go.GetComponent<PrimaryElement>().Element.highTemp && item_go.GetComponent<PrimaryElement>().Element.HasTransitionUp)
      {
        PrimaryElement component = item_go.GetComponent<PrimaryElement>();
        this.iceStorage.AddLiquid(component.Element.highTempTransitionTarget, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount);
        this.iceStorage.ConsumeIgnoringDisease(item_go);
      }
    }
    for (int count = this.iceStorage.items.Count; count > 0; --count)
    {
      GameObject go = this.iceStorage.items[count - 1];
      if (Object.op_Inequality((Object) go, (Object) null) && (double) go.GetComponent<PrimaryElement>().Temperature >= (double) this.targetTemperature)
        this.iceStorage.Transfer(go, this.liquidStorage, true, true);
    }
    if (!this.liquidStorage.IsEmpty())
      this.liquidStorage.DropAll(false, false, new Vector3(1f, 0.0f, 0.0f), true, (List<GameObject>) null);
    this.UpdateMeter();
  }

  public class StatesInstance : 
    GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.GameInstance
  {
    public StatesInstance(IceCooledFan smi)
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
    GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan>
  {
    public IceCooledFan.States.Workable workable;
    public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State unworkable;
    public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State work_pst;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.unworkable;
      this.root.Enter((StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State.Callback) (smi => smi.master.workable.SetWorkTime(float.PositiveInfinity)));
      this.workable.ToggleChore(new Func<IceCooledFan.StatesInstance, Chore>(this.CreateUseChore), this.work_pst).EventTransition(GameHashes.ActiveChanged, this.workable.cooling, (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.master.workable.worker, (Object) null))).EventTransition(GameHashes.OperationalChanged, this.workable.cooling, (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.master.workable.worker, (Object) null))).Transition(this.unworkable, (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.Transition.ConditionCallback) (smi => !smi.IsWorkable()));
      this.workable.cooling.EventTransition(GameHashes.OperationalChanged, this.unworkable, (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.Transition.ConditionCallback) (smi => Object.op_Equality((Object) smi.master.workable.worker, (Object) null))).EventHandler(GameHashes.ActiveChanged, (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State.Callback) (smi => smi.master.CheckWorking())).Enter((StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State.Callback) (smi =>
      {
        ((Component) smi.master).gameObject.GetComponent<ManualDeliveryKG>().Pause(true, "Working");
        if (smi.EnvironmentNeedsCooling() && smi.master.HasMaterial() && smi.EnvironmentHighEnoughPressure())
          return;
        smi.GoTo((StateMachine.BaseState) this.unworkable);
      })).Update("IceCooledFanCooling", (Action<IceCooledFan.StatesInstance, float>) ((smi, dt) => smi.master.DoCooling(dt))).Exit((StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State.Callback) (smi =>
      {
        if (!smi.master.HasMaterial())
          ((Component) smi.master).gameObject.GetComponent<ManualDeliveryKG>().Pause(false, "Working");
        smi.master.liquidStorage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
      }));
      this.work_pst.ScheduleGoTo(2f, (StateMachine.BaseState) this.unworkable);
      this.unworkable.Update("IceFanUnworkableStatusItems", (Action<IceCooledFan.StatesInstance, float>) ((smi, dt) => smi.master.UpdateUnworkableStatusItems())).Transition(this.workable.waiting, (StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.Transition.ConditionCallback) (smi => smi.IsWorkable())).Enter((StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State.Callback) (smi => smi.master.UpdateUnworkableStatusItems())).Exit((StateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State.Callback) (smi => smi.master.UpdateUnworkableStatusItems()));
    }

    private Chore CreateUseChore(IceCooledFan.StatesInstance smi) => (Chore) new WorkChore<IceCooledFanWorkable>(Db.Get().ChoreTypes.IceCooledFan, (IStateMachineTarget) smi.master.workable);

    public class Workable : 
      GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State
    {
      public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State waiting;
      public GameStateMachine<IceCooledFan.States, IceCooledFan.StatesInstance, IceCooledFan, object>.State cooling;
    }
  }
}
