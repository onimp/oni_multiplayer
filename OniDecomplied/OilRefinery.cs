// Decompiled with JetBrains decompiler
// Type: OilRefinery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using TUNING;
using UnityEngine;

[SerializationConfig]
public class OilRefinery : StateMachineComponent<OilRefinery.StatesInstance>
{
  private bool wasOverPressure;
  [SerializeField]
  public float overpressureWarningMass = 4.5f;
  [SerializeField]
  public float overpressureMass = 5f;
  private float maxSrcMass;
  private float envPressure;
  private float cellCount;
  [MyCmpGet]
  private Storage storage;
  [MyCmpReq]
  private Operational operational;
  [MyCmpAdd]
  private OilRefinery.WorkableTarget workable;
  [MyCmpReq]
  private OccupyArea occupyArea;
  private const bool hasMeter = true;
  private MeterController meter;
  private static readonly EventSystem.IntraObjectHandler<OilRefinery> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<OilRefinery>((Action<OilRefinery, object>) ((component, data) => component.OnStorageChanged(data)));

  protected virtual void OnSpawn()
  {
    this.Subscribe<OilRefinery>(-1697596308, OilRefinery.OnStorageChangedDelegate);
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, (string[]) null);
    this.smi.StartSM();
    this.maxSrcMass = ((Component) this).GetComponent<ConduitConsumer>().capacityKG;
  }

  private void OnStorageChanged(object data) => this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.GetMassAvailable(SimHashes.CrudeOil) / this.maxSrcMass));

  private static bool UpdateStateCb(int cell, object data)
  {
    OilRefinery oilRefinery = data as OilRefinery;
    if (Grid.Element[cell].IsGas)
    {
      ++oilRefinery.cellCount;
      oilRefinery.envPressure += Grid.Mass[cell];
    }
    return true;
  }

  private void TestAreaPressure()
  {
    this.envPressure = 0.0f;
    this.cellCount = 0.0f;
    if (!Object.op_Inequality((Object) this.occupyArea, (Object) null) || !Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null))
      return;
    this.occupyArea.TestArea(Grid.PosToCell(((Component) this).gameObject), (object) this, new Func<int, object, bool>(OilRefinery.UpdateStateCb));
    this.envPressure /= this.cellCount;
  }

  private bool IsOverPressure() => (double) this.envPressure >= (double) this.overpressureMass;

  private bool IsOverWarningPressure() => (double) this.envPressure >= (double) this.overpressureWarningMass;

  public class StatesInstance : 
    GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.GameInstance
  {
    public StatesInstance(OilRefinery smi)
      : base(smi)
    {
    }

    public void TestAreaPressure()
    {
      this.smi.master.TestAreaPressure();
      int num = this.smi.master.IsOverPressure() ? 1 : 0;
      bool flag = this.smi.master.IsOverWarningPressure();
      if (num != 0)
      {
        this.smi.master.wasOverPressure = true;
        this.sm.isOverPressure.Set(true, this);
      }
      else
      {
        if (!this.smi.master.wasOverPressure || flag)
          return;
        this.sm.isOverPressure.Set(false, this);
      }
    }
  }

  public class States : GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery>
  {
    public StateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.BoolParameter isOverPressure;
    public StateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.BoolParameter isOverPressureWarning;
    public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State disabled;
    public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State overpressure;
    public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State needResources;
    public GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.State ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.disabled;
      this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (StateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational));
      this.disabled.EventTransition(GameHashes.OperationalChanged, this.needResources, (StateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.Transition.ConditionCallback) (smi => smi.master.operational.IsOperational));
      this.needResources.EventTransition(GameHashes.OnStorageChange, this.ready, (StateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.Transition.ConditionCallback) (smi => ((Component) smi.master).GetComponent<ElementConverter>().HasEnoughMassToStartConverting()));
      this.ready.Update("Test Pressure Update", (Action<OilRefinery.StatesInstance, float>) ((smi, dt) => smi.TestAreaPressure()), (UpdateRate) 6).ParamTransition<bool>((StateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.Parameter<bool>) this.isOverPressure, this.overpressure, GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.IsTrue).Transition(this.needResources, (StateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.Transition.ConditionCallback) (smi => !((Component) smi.master).GetComponent<ElementConverter>().HasEnoughMassToStartConverting())).ToggleChore((Func<OilRefinery.StatesInstance, Chore>) (smi => (Chore) new WorkChore<OilRefinery.WorkableTarget>(Db.Get().ChoreTypes.Fabricate, (IStateMachineTarget) smi.master.workable)), this.needResources);
      this.overpressure.Update("Test Pressure Update", (Action<OilRefinery.StatesInstance, float>) ((smi, dt) => smi.TestAreaPressure()), (UpdateRate) 6).ParamTransition<bool>((StateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.Parameter<bool>) this.isOverPressure, this.ready, GameStateMachine<OilRefinery.States, OilRefinery.StatesInstance, OilRefinery, object>.IsFalse).ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk);
    }
  }

  [AddComponentMenu("KMonoBehaviour/Workable/WorkableTarget")]
  public class WorkableTarget : Workable
  {
    [MyCmpGet]
    public Operational operational;

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      this.showProgressBar = false;
      this.workerStatusItem = (StatusItem) null;
      this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
      this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
      this.overrideAnims = new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit("anim_interacts_oilrefinery_kanim"))
      };
    }

    protected override void OnSpawn()
    {
      base.OnSpawn();
      this.SetWorkTime(float.PositiveInfinity);
    }

    protected override void OnStartWork(Worker worker) => this.operational.SetActive(true);

    protected override void OnStopWork(Worker worker) => this.operational.SetActive(false);

    protected override void OnCompleteWork(Worker worker) => this.operational.SetActive(false);

    public override bool InstantlyFinish(Worker worker) => false;
  }
}
