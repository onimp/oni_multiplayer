// Decompiled with JetBrains decompiler
// Type: AirFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class AirFilter : StateMachineComponent<AirFilter.StatesInstance>, IGameObjectEffectDescriptor
{
  [MyCmpGet]
  private Operational operational;
  [MyCmpGet]
  private Storage storage;
  [MyCmpGet]
  private ElementConverter elementConverter;
  [MyCmpGet]
  private ElementConsumer elementConsumer;
  public Tag filterTag;

  public bool HasFilter() => this.elementConverter.HasEnoughMass(this.filterTag);

  public bool IsConvertable() => this.elementConverter.HasEnoughMassToStartConverting();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public List<Descriptor> GetDescriptors(GameObject go) => (List<Descriptor>) null;

  public class StatesInstance : 
    GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.GameInstance
  {
    public StatesInstance(AirFilter smi)
      : base(smi)
    {
    }
  }

  public class States : GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter>
  {
    public AirFilter.States.ReadyStates hasFilter;
    public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State waiting;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.waiting;
      this.waiting.EventTransition(GameHashes.OnStorageChange, (GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State) this.hasFilter, (StateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.Transition.ConditionCallback) (smi => smi.master.HasFilter() && smi.master.operational.IsOperational)).EventTransition(GameHashes.OperationalChanged, (GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State) this.hasFilter, (StateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.Transition.ConditionCallback) (smi => smi.master.HasFilter() && smi.master.operational.IsOperational));
      this.hasFilter.EventTransition(GameHashes.OperationalChanged, this.waiting, (StateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational)).Enter("EnableConsumption", (StateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State.Callback) (smi => smi.master.elementConsumer.EnableConsumption(true))).Exit("DisableConsumption", (StateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State.Callback) (smi => smi.master.elementConsumer.EnableConsumption(false))).DefaultState(this.hasFilter.idle);
      this.hasFilter.idle.EventTransition(GameHashes.OnStorageChange, this.hasFilter.converting, (StateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.Transition.ConditionCallback) (smi => smi.master.IsConvertable()));
      this.hasFilter.converting.Enter("SetActive(true)", (StateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Exit("SetActive(false)", (StateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).EventTransition(GameHashes.OnStorageChange, this.hasFilter.idle, (StateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.Transition.ConditionCallback) (smi => !smi.master.IsConvertable()));
    }

    public class ReadyStates : 
      GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State
    {
      public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State idle;
      public GameStateMachine<AirFilter.States, AirFilter.StatesInstance, AirFilter, object>.State converting;
    }
  }
}
