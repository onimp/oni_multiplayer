// Decompiled with JetBrains decompiler
// Type: RustDeoxidizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class RustDeoxidizer : StateMachineComponent<RustDeoxidizer.StatesInstance>
{
  [SerializeField]
  public float maxMass = 2.5f;
  [MyCmpAdd]
  private Storage storage;
  [MyCmpGet]
  private ElementConverter emitter;
  [MyCmpReq]
  private Operational operational;
  private MeterController meter;

  protected virtual void OnSpawn()
  {
    this.smi.StartSM();
    Tutorial.Instance.oxygenGenerators.Add(((Component) this).gameObject);
  }

  protected override void OnCleanUp()
  {
    Tutorial.Instance.oxygenGenerators.Remove(((Component) this).gameObject);
    base.OnCleanUp();
  }

  private bool RoomForPressure => !GameUtil.FloodFillCheck<RustDeoxidizer>(new Func<int, RustDeoxidizer, bool>(RustDeoxidizer.OverPressure), this, Grid.CellAbove(Grid.PosToCell(TransformExtensions.GetPosition(this.transform))), 3, true, true);

  private static bool OverPressure(int cell, RustDeoxidizer rustDeoxidizer) => (double) Grid.Mass[cell] > (double) rustDeoxidizer.maxMass;

  public class StatesInstance : 
    GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.GameInstance
  {
    public StatesInstance(RustDeoxidizer smi)
      : base(smi)
    {
    }
  }

  public class States : 
    GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer>
  {
    public GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State disabled;
    public GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State waiting;
    public GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State converting;
    public GameStateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State overpressure;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.disabled;
      this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (StateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational));
      this.disabled.EventTransition(GameHashes.OperationalChanged, this.waiting, (StateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.Transition.ConditionCallback) (smi => smi.master.operational.IsOperational));
      this.waiting.Enter("Waiting", (StateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).EventTransition(GameHashes.OnStorageChange, this.converting, (StateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.Transition.ConditionCallback) (smi => ((Component) smi.master).GetComponent<ElementConverter>().HasEnoughMassToStartConverting()));
      this.converting.Enter("Ready", (StateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Transition(this.waiting, (StateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.Transition.ConditionCallback) (smi => !((Component) smi.master).GetComponent<ElementConverter>().CanConvertAtAll())).Transition(this.overpressure, (StateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.Transition.ConditionCallback) (smi => !smi.master.RoomForPressure));
      this.overpressure.Enter("OverPressure", (StateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk).Transition(this.converting, (StateMachine<RustDeoxidizer.States, RustDeoxidizer.StatesInstance, RustDeoxidizer, object>.Transition.ConditionCallback) (smi => smi.master.RoomForPressure));
    }
  }
}
