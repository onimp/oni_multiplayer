// Decompiled with JetBrains decompiler
// Type: HeatCompressor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

public class HeatCompressor : StateMachineComponent<HeatCompressor.StatesInstance>
{
  [MyCmpReq]
  private Operational operational;
  private MeterController meter;
  public Storage inputStorage;
  public Storage outputStorage;
  public Storage heatCubeStorage;
  public float heatRemovalRate = 100f;
  public float heatRemovalTime = 100f;
  [Serialize]
  public float energyCompressed;
  public float heat_sink_active_time = 9000f;
  [Serialize]
  public float time_active;
  public float MAX_CUBE_TEMPERATURE = 3000f;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[4]
    {
      "meter_target",
      "meter_fill",
      "meter_frame",
      "meter_OL"
    });
    this.meter.gameObject.GetComponent<KBatchedAnimController>().SetDirty();
    GameObject go = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("HeatCube")), TransformExtensions.GetPosition(this.transform));
    go.SetActive(true);
    this.heatCubeStorage.Store(go, true);
    this.smi.StartSM();
  }

  public void SetStorage(Storage inputStorage, Storage outputStorage, Storage heatCubeStorage)
  {
    this.inputStorage = inputStorage;
    this.outputStorage = outputStorage;
    this.heatCubeStorage = heatCubeStorage;
  }

  public void CompressHeat(HeatCompressor.StatesInstance smi, float dt)
  {
    smi.heatRemovalTimer -= dt;
    float num = this.heatRemovalRate * dt / (float) this.inputStorage.items.Count;
    foreach (GameObject gameObject in this.inputStorage.items)
    {
      PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
      float lowTemp = component.Element.lowTemp;
      GameUtil.DeltaThermalEnergy(component, -num, lowTemp);
      this.energyCompressed += num;
    }
    if ((double) smi.heatRemovalTimer <= 0.0)
    {
      for (int count = this.inputStorage.items.Count; count > 0; --count)
      {
        GameObject go = this.inputStorage.items[count - 1];
        if (Object.op_Implicit((Object) go))
          this.inputStorage.Transfer(go, this.outputStorage, hide_popups: true);
      }
      smi.StartNewHeatRemoval();
    }
    foreach (GameObject gameObject in this.heatCubeStorage.items)
      GameUtil.DeltaThermalEnergy(gameObject.GetComponent<PrimaryElement>(), this.energyCompressed / (float) this.heatCubeStorage.items.Count, 100000f);
    this.energyCompressed = 0.0f;
  }

  public void EjectHeatCube() => this.heatCubeStorage.DropAll(TransformExtensions.GetPosition(this.transform), offset: new Vector3());

  public class StatesInstance : 
    GameStateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.GameInstance
  {
    [Serialize]
    public float heatRemovalTimer;

    public StatesInstance(HeatCompressor master)
      : base(master)
    {
    }

    public void UpdateMeter() => this.master.meter.SetPositionPercent(this.GetRemainingCharge());

    public float GetRemainingCharge()
    {
      PrimaryElement firstWithMass = this.smi.master.heatCubeStorage.FindFirstWithMass(GameTags.IndustrialIngredient);
      float remainingCharge = 1f;
      if (Object.op_Inequality((Object) firstWithMass, (Object) null))
        remainingCharge = Mathf.Clamp01(((Component) firstWithMass).GetComponent<PrimaryElement>().Temperature / this.smi.master.MAX_CUBE_TEMPERATURE);
      return remainingCharge;
    }

    public bool CanWork() => (double) this.GetRemainingCharge() < 1.0 && this.smi.master.heatCubeStorage.items.Count > 0;

    public void StartNewHeatRemoval() => this.heatRemovalTimer = this.smi.master.heatRemovalTime;
  }

  public class States : 
    GameStateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor>
  {
    public GameStateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.State active;
    public GameStateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.State inactive;
    public GameStateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.State dropCube;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.inactive;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.root.EventTransition(GameHashes.OperationalChanged, this.inactive, (StateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational));
      this.inactive.Enter((StateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.State.Callback) (smi => smi.UpdateMeter())).PlayAnim("idle").Transition(this.dropCube, (StateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.Transition.ConditionCallback) (smi => (double) smi.GetRemainingCharge() >= 1.0)).Transition(this.active, (StateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational && smi.CanWork()));
      this.active.Enter((StateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.State.Callback) (smi =>
      {
        smi.GetComponent<Operational>().SetActive(true);
        smi.StartNewHeatRemoval();
      })).PlayAnim("working_loop", (KAnim.PlayMode) 0).Update((Action<HeatCompressor.StatesInstance, float>) ((smi, dt) =>
      {
        smi.master.time_active += dt;
        smi.UpdateMeter();
        smi.master.CompressHeat(smi, dt);
      })).Transition(this.dropCube, (StateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.Transition.ConditionCallback) (smi => (double) smi.GetRemainingCharge() >= 1.0)).Transition(this.inactive, (StateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.Transition.ConditionCallback) (smi => !smi.CanWork())).Exit((StateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(false)));
      this.dropCube.Enter((StateMachine<HeatCompressor.States, HeatCompressor.StatesInstance, HeatCompressor, object>.State.Callback) (smi =>
      {
        smi.master.EjectHeatCube();
        smi.GoTo((StateMachine.BaseState) this.inactive);
      }));
    }
  }
}
