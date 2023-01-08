// Decompiled with JetBrains decompiler
// Type: RadiationLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class RadiationLight : StateMachineComponent<RadiationLight.StatesInstance>
{
  [MyCmpGet]
  private Operational operational;
  [MyCmpGet]
  private Storage storage;
  [MyCmpGet]
  private RadiationEmitter emitter;
  [MyCmpGet]
  private ElementConverter elementConverter;
  private MeterController meter;
  public Tag elementToConsume;
  public float consumptionRate;

  public void UpdateMeter() => this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg));

  public bool HasEnoughFuel() => this.elementConverter.HasEnoughMassToStartConverting();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    this.UpdateMeter();
  }

  public class StatesInstance : 
    GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.GameInstance
  {
    public StatesInstance(RadiationLight smi)
      : base(smi)
    {
      if (this.GetComponent<Rotatable>().IsRotated)
      {
        RadiationEmitter component = this.GetComponent<RadiationEmitter>();
        component.emitDirection = 180f;
        component.emissionOffset = Vector3.left;
      }
      this.ToggleEmitter(false);
      smi.meter = new MeterController((KAnimControllerBase) this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]
      {
        "meter_target"
      });
      Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Radiation);
    }

    public void ToggleEmitter(bool on)
    {
      this.smi.master.operational.SetActive(on);
      this.smi.master.emitter.SetEmitting(on);
    }
  }

  public class States : 
    GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight>
  {
    public GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State waiting;
    public RadiationLight.States.ReadyStates ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.ready.idle;
      this.root.EventHandler(GameHashes.OnStorageChange, (StateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State.Callback) (smi => smi.master.UpdateMeter()));
      this.waiting.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.ready.idle, (StateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.Transition.ConditionCallback) (smi => smi.master.operational.IsOperational));
      this.ready.EventTransition(GameHashes.OperationalChanged, this.waiting, (StateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational)).DefaultState(this.ready.idle);
      this.ready.idle.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, this.ready.on, (StateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.Transition.ConditionCallback) (smi => smi.master.HasEnoughFuel()));
      this.ready.on.PlayAnim("on").Enter((StateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State.Callback) (smi => smi.ToggleEmitter(true))).EventTransition(GameHashes.OnStorageChange, this.ready.idle, (StateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.Transition.ConditionCallback) (smi => !smi.master.HasEnoughFuel())).Exit((StateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State.Callback) (smi => smi.ToggleEmitter(false)));
    }

    public class ReadyStates : 
      GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State
    {
      public GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State idle;
      public GameStateMachine<RadiationLight.States, RadiationLight.StatesInstance, RadiationLight, object>.State on;
    }
  }
}
