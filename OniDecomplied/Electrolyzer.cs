// Decompiled with JetBrains decompiler
// Type: Electrolyzer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class Electrolyzer : StateMachineComponent<Electrolyzer.StatesInstance>
{
  [SerializeField]
  public float maxMass = 2.5f;
  [SerializeField]
  public bool hasMeter = true;
  [SerializeField]
  public CellOffset emissionOffset = CellOffset.none;
  [MyCmpAdd]
  private Storage storage;
  [MyCmpGet]
  private ElementConverter emitter;
  [MyCmpReq]
  private Operational operational;
  private MeterController meter;

  protected virtual void OnSpawn()
  {
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    if (this.hasMeter)
      this.meter = new MeterController((KAnimControllerBase) component, "U2H_meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new Vector3(-0.4f, 0.5f, -0.1f), new string[4]
      {
        "U2H_meter_target",
        "U2H_meter_tank",
        "U2H_meter_waterbody",
        "U2H_meter_level"
      });
    this.smi.StartSM();
    this.UpdateMeter();
    Tutorial.Instance.oxygenGenerators.Add(((Component) this).gameObject);
  }

  protected override void OnCleanUp()
  {
    Tutorial.Instance.oxygenGenerators.Remove(((Component) this).gameObject);
    base.OnCleanUp();
  }

  public void UpdateMeter()
  {
    if (!this.hasMeter)
      return;
    this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg));
  }

  private bool RoomForPressure => !GameUtil.FloodFillCheck<Electrolyzer>(new Func<int, Electrolyzer, bool>(Electrolyzer.OverPressure), this, Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), this.emissionOffset), 3, true, true);

  private static bool OverPressure(int cell, Electrolyzer electrolyzer) => (double) Grid.Mass[cell] > (double) electrolyzer.maxMass;

  public class StatesInstance : 
    GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.GameInstance
  {
    public StatesInstance(Electrolyzer smi)
      : base(smi)
    {
    }
  }

  public class States : 
    GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer>
  {
    public GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State disabled;
    public GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State waiting;
    public GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State converting;
    public GameStateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State overpressure;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.disabled;
      this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (StateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational)).EventHandler(GameHashes.OnStorageChange, (StateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State.Callback) (smi => smi.master.UpdateMeter()));
      this.disabled.EventTransition(GameHashes.OperationalChanged, this.waiting, (StateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.Transition.ConditionCallback) (smi => smi.master.operational.IsOperational));
      this.waiting.Enter("Waiting", (StateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).EventTransition(GameHashes.OnStorageChange, this.converting, (StateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.Transition.ConditionCallback) (smi => ((Component) smi.master).GetComponent<ElementConverter>().HasEnoughMassToStartConverting()));
      this.converting.Enter("Ready", (StateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Transition(this.waiting, (StateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.Transition.ConditionCallback) (smi => !((Component) smi.master).GetComponent<ElementConverter>().CanConvertAtAll())).Transition(this.overpressure, (StateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.Transition.ConditionCallback) (smi => !smi.master.RoomForPressure));
      this.overpressure.Enter("OverPressure", (StateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk).Transition(this.converting, (StateMachine<Electrolyzer.States, Electrolyzer.StatesInstance, Electrolyzer, object>.Transition.ConditionCallback) (smi => smi.master.RoomForPressure));
    }
  }
}
