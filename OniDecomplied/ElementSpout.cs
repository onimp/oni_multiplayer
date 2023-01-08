// Decompiled with JetBrains decompiler
// Type: ElementSpout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ElementSpout : StateMachineComponent<ElementSpout.StatesInstance>
{
  [SerializeField]
  private ElementEmitter emitter;
  [MyCmpAdd]
  private KBatchedAnimController anim;
  public float maxPressure = 1.5f;
  public float emissionPollFrequency = 3f;
  public float emissionIrregularity = 1.5f;
  public float perEmitAmount = 0.5f;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    Grid.Objects[cell, 2] = ((Component) this).gameObject;
    this.smi.StartSM();
  }

  public void SetEmitter(ElementEmitter emitter) => this.emitter = emitter;

  public void ConfigureEmissionSettings(
    float emissionPollFrequency = 3f,
    float emissionIrregularity = 1.5f,
    float maxPressure = 1.5f,
    float perEmitAmount = 0.5f)
  {
    this.maxPressure = maxPressure;
    this.emissionPollFrequency = emissionPollFrequency;
    this.emissionIrregularity = emissionIrregularity;
    this.perEmitAmount = perEmitAmount;
  }

  public class StatesInstance : 
    GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.GameInstance
  {
    public StatesInstance(ElementSpout smi)
      : base(smi)
    {
    }

    private bool CanEmitOnCell(int cell, float max_pressure, Element.State expected_state)
    {
      if ((double) Grid.Mass[cell] >= (double) max_pressure)
        return false;
      return Grid.Element[cell].IsState(expected_state) || Grid.Element[cell].IsVacuum;
    }

    public bool CanEmitAnywhere()
    {
      int cell1 = Grid.PosToCell(TransformExtensions.GetPosition(this.smi.transform));
      int cell2 = Grid.CellLeft(cell1);
      int cell3 = Grid.CellRight(cell1);
      int cell4 = Grid.CellAbove(cell1);
      Element.State state = ElementLoader.FindElementByHash(this.smi.master.emitter.outputElement.elementHash).state;
      return (((false ? 1 : (this.CanEmitOnCell(cell1, this.smi.master.maxPressure, state) ? 1 : 0)) != 0 ? 1 : (this.CanEmitOnCell(cell2, this.smi.master.maxPressure, state) ? 1 : 0)) != 0 ? 1 : (this.CanEmitOnCell(cell3, this.smi.master.maxPressure, state) ? 1 : 0)) != 0 || this.CanEmitOnCell(cell4, this.smi.master.maxPressure, state);
    }
  }

  public class States : 
    GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout>
  {
    public ElementSpout.States.Idle idle;
    public ElementSpout.States.Emitting emit;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.idle.DefaultState(this.idle.unblocked).Enter((StateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State.Callback) (smi => smi.Play("idle"))).ScheduleGoTo((Func<ElementSpout.StatesInstance, float>) (smi => smi.master.emissionPollFrequency), (StateMachine.BaseState) this.emit);
      this.idle.unblocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutPressureBuilding).Transition(this.idle.blocked, (StateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.Transition.ConditionCallback) (smi => !smi.CanEmitAnywhere()));
      this.idle.blocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure).Transition(this.idle.blocked, (StateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.Transition.ConditionCallback) (smi => smi.CanEmitAnywhere()));
      this.emit.DefaultState(this.emit.unblocked).Enter((StateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State.Callback) (smi =>
      {
        float time = 1f + Random.Range(0.0f, smi.master.emissionIrregularity);
        float num = smi.master.perEmitAmount / time;
        smi.master.emitter.SetEmitting(true);
        smi.master.emitter.emissionFrequency = 1f;
        smi.master.emitter.outputElement.massGenerationRate = num;
        smi.ScheduleGoTo(time, (StateMachine.BaseState) this.idle);
      }));
      this.emit.unblocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutEmitting).Enter((StateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State.Callback) (smi =>
      {
        smi.Play("emit");
        smi.master.emitter.SetEmitting(true);
      })).Transition(this.emit.blocked, (StateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.Transition.ConditionCallback) (smi => !smi.CanEmitAnywhere()));
      this.emit.blocked.ToggleStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure).Enter((StateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State.Callback) (smi =>
      {
        smi.Play("idle");
        smi.master.emitter.SetEmitting(false);
      })).Transition(this.emit.unblocked, (StateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.Transition.ConditionCallback) (smi => smi.CanEmitAnywhere()));
    }

    public class Idle : 
      GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State
    {
      public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State unblocked;
      public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State blocked;
    }

    public class Emitting : 
      GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State
    {
      public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State unblocked;
      public GameStateMachine<ElementSpout.States, ElementSpout.StatesInstance, ElementSpout, object>.State blocked;
    }
  }
}
