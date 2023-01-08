// Decompiled with JetBrains decompiler
// Type: RefrigeratorController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RefrigeratorController : 
  GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>
{
  public GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State inoperational;
  public RefrigeratorController.OperationalStates operational;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.inoperational;
    this.inoperational.EventTransition(GameHashes.OperationalChanged, (GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State) this.operational, new StateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.Transition.ConditionCallback(this.IsOperational));
    this.operational.DefaultState(this.operational.steady).EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.Not(new StateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.Transition.ConditionCallback(this.IsOperational))).Enter((StateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State.Callback) (smi => smi.operational.SetActive(true))).Exit((StateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State.Callback) (smi => smi.operational.SetActive(false)));
    this.operational.cooling.Update("Cooling exhaust", (System.Action<RefrigeratorController.StatesInstance, float>) ((smi, dt) => smi.ApplyCoolingExhaust(dt)), load_balance: true).UpdateTransition(this.operational.steady, new Func<RefrigeratorController.StatesInstance, float, bool>(this.AllFoodCool), (UpdateRate) 7, true).ToggleStatusItem(Db.Get().BuildingStatusItems.FridgeCooling, (Func<RefrigeratorController.StatesInstance, object>) (smi => (object) smi), Db.Get().StatusItemCategories.Main);
    this.operational.steady.Update("Cooling exhaust", (System.Action<RefrigeratorController.StatesInstance, float>) ((smi, dt) => smi.ApplySteadyExhaust(dt)), load_balance: true).UpdateTransition(this.operational.cooling, new Func<RefrigeratorController.StatesInstance, float, bool>(this.AnyWarmFood), (UpdateRate) 7, true).ToggleStatusItem(Db.Get().BuildingStatusItems.FridgeSteady, (Func<RefrigeratorController.StatesInstance, object>) (smi => (object) smi), Db.Get().StatusItemCategories.Main).Enter((StateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State.Callback) (smi => smi.SetEnergySaver(true))).Exit((StateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State.Callback) (smi => smi.SetEnergySaver(false)));
  }

  private bool AllFoodCool(RefrigeratorController.StatesInstance smi, float dt)
  {
    foreach (GameObject gameObject in smi.storage.items)
    {
      if (!Object.op_Equality((Object) gameObject, (Object) null))
      {
        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
        if (!Object.op_Equality((Object) component, (Object) null) && (double) component.Mass >= 0.0099999997764825821 && (double) component.Temperature >= (double) smi.def.simulatedInternalTemperature + (double) smi.def.activeCoolingStopBuffer)
          return false;
      }
    }
    return true;
  }

  private bool AnyWarmFood(RefrigeratorController.StatesInstance smi, float dt)
  {
    foreach (GameObject gameObject in smi.storage.items)
    {
      if (!Object.op_Equality((Object) gameObject, (Object) null))
      {
        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
        if (!Object.op_Equality((Object) component, (Object) null) && (double) component.Mass >= 0.0099999997764825821 && (double) component.Temperature >= (double) smi.def.simulatedInternalTemperature + (double) smi.def.activeCoolingStartBuffer)
          return true;
      }
    }
    return false;
  }

  private bool IsOperational(RefrigeratorController.StatesInstance smi) => smi.operational.IsOperational;

  public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
  {
    public float activeCoolingStartBuffer = 2f;
    public float activeCoolingStopBuffer = 0.1f;
    public float simulatedInternalTemperature = 274.15f;
    public float simulatedInternalHeatCapacity = 400f;
    public float simulatedThermalConductivity = 1000f;
    public float powerSaverEnergyUsage;
    public float coolingHeatKW;
    public float steadyHeatKW;

    public List<Descriptor> GetDescriptors(GameObject go)
    {
      List<Descriptor> descriptors = new List<Descriptor>();
      descriptors.AddRange((IEnumerable<Descriptor>) SimulatedTemperatureAdjuster.GetDescriptors(this.simulatedInternalTemperature));
      Descriptor descriptor = new Descriptor();
      string formattedHeatEnergy = GameUtil.GetFormattedHeatEnergy(this.coolingHeatKW * 1000f);
      ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.HEATGENERATED, (object) formattedHeatEnergy), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED, (object) formattedHeatEnergy), (Descriptor.DescriptorType) 1);
      descriptors.Add(descriptor);
      return descriptors;
    }
  }

  public class OperationalStates : 
    GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State
  {
    public GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State cooling;
    public GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.State steady;
  }

  public class StatesInstance : 
    GameStateMachine<RefrigeratorController, RefrigeratorController.StatesInstance, IStateMachineTarget, RefrigeratorController.Def>.GameInstance
  {
    [MyCmpReq]
    public Operational operational;
    [MyCmpReq]
    public Storage storage;
    private HandleVector<int>.Handle structureTemperature;
    private SimulatedTemperatureAdjuster temperatureAdjuster;

    public StatesInstance(IStateMachineTarget master, RefrigeratorController.Def def)
      : base(master, def)
    {
      this.temperatureAdjuster = new SimulatedTemperatureAdjuster(def.simulatedInternalTemperature, def.simulatedInternalHeatCapacity, def.simulatedThermalConductivity, this.storage);
      this.structureTemperature = GameComps.StructureTemperatures.GetHandle(this.gameObject);
    }

    protected override void OnCleanUp()
    {
      this.temperatureAdjuster.CleanUp();
      base.OnCleanUp();
    }

    public float GetSaverPower() => this.def.powerSaverEnergyUsage;

    public float GetNormalPower() => this.GetComponent<EnergyConsumer>().WattsNeededWhenActive;

    public void SetEnergySaver(bool energySaving)
    {
      EnergyConsumer component = this.GetComponent<EnergyConsumer>();
      if (energySaving)
        component.BaseWattageRating = this.GetSaverPower();
      else
        component.BaseWattageRating = this.GetNormalPower();
    }

    public void ApplyCoolingExhaust(float dt) => GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, this.def.coolingHeatKW * dt, (string) BUILDING.STATUSITEMS.OPERATINGENERGY.FOOD_TRANSFER, dt);

    public void ApplySteadyExhaust(float dt) => GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, this.def.steadyHeatKW * dt, (string) BUILDING.STATUSITEMS.OPERATINGENERGY.FOOD_TRANSFER, dt);
  }
}
