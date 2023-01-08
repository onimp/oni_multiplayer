// Decompiled with JetBrains decompiler
// Type: OilWellCap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using TUNING;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/Workable/OilWellCap")]
public class OilWellCap : Workable, ISingleSliderControl, ISliderControl, IElementEmitter
{
  private OilWellCap.StatesInstance smi;
  [MyCmpReq]
  private Operational operational;
  [MyCmpReq]
  private Storage storage;
  public SimHashes gasElement;
  public float gasTemperature;
  public float addGasRate = 1f;
  public float maxGasPressure = 10f;
  public float releaseGasRate = 10f;
  [Serialize]
  private float depressurizePercent = 0.75f;
  private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
  private MeterController pressureMeter;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<OilWellCap> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<OilWellCap>((Action<OilWellCap, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly Chore.Precondition AllowedToDepressurize = new Chore.Precondition()
  {
    id = nameof (AllowedToDepressurize),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.ALLOWED_TO_DEPRESSURIZE,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => ((OilWellCap) data).NeedsDepressurizing())
  };

  public SimHashes Element => this.gasElement;

  public float AverageEmitRate => Game.Instance.accumulators.GetAverageRate(this.accumulator);

  public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TITLE";

  public string SliderUnits => (string) UI.UNITSUFFIXES.PERCENT;

  public int SliderDecimalPlaces(int index) => 0;

  public float GetSliderMin(int index) => 0.0f;

  public float GetSliderMax(int index) => 100f;

  public float GetSliderValue(int index) => this.depressurizePercent * 100f;

  public void SetSliderValue(float value, int index) => this.depressurizePercent = value / 100f;

  public string GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TOOLTIP";

  string ISliderControl.GetSliderTooltip() => string.Format(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TOOLTIP")), (object) (float) ((double) this.depressurizePercent * 100.0));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<OilWellCap>(-905833192, OilWellCap.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    OilWellCap component = ((GameObject) data).GetComponent<OilWellCap>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.depressurizePercent = component.depressurizePercent;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    Prioritizable.AddRef(((Component) this).gameObject);
    this.accumulator = Game.Instance.accumulators.Add("pressuregas", (KMonoBehaviour) this);
    this.showProgressBar = false;
    this.SetWorkTime(float.PositiveInfinity);
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_oil_cap_kanim"))
    };
    this.workingStatusItem = Db.Get().BuildingStatusItems.ReleasingPressure;
    this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.pressureMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new Vector3(0.0f, 0.0f, 0.0f), (string[]) null);
    this.smi = new OilWellCap.StatesInstance(this);
    this.smi.StartSM();
    this.UpdatePressurePercent();
  }

  protected override void OnCleanUp()
  {
    Game.Instance.accumulators.Remove(this.accumulator);
    Prioritizable.RemoveRef(((Component) this).gameObject);
    base.OnCleanUp();
  }

  public void AddGasPressure(float dt)
  {
    this.storage.AddGasChunk(this.gasElement, this.addGasRate * dt, this.gasTemperature, (byte) 0, 0, true);
    this.UpdatePressurePercent();
  }

  public void ReleaseGasPressure(float dt)
  {
    PrimaryElement primaryElement = this.storage.FindPrimaryElement(this.gasElement);
    if (Object.op_Inequality((Object) primaryElement, (Object) null) && (double) primaryElement.Mass > 0.0)
    {
      float num1 = this.releaseGasRate * dt;
      if (Object.op_Inequality((Object) this.worker, (Object) null))
        num1 *= this.GetEfficiencyMultiplier(this.worker);
      float num2 = Mathf.Min(num1, primaryElement.Mass);
      SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(primaryElement, num2 / primaryElement.Mass);
      primaryElement.Mass -= num2;
      Game.Instance.accumulators.Accumulate(this.accumulator, num2);
      SimMessages.AddRemoveSubstance(Grid.PosToCell((KMonoBehaviour) this), ElementLoader.GetElementIndex(this.gasElement), (CellAddRemoveSubstanceEvent) null, num2, primaryElement.Temperature, percentOfDisease.idx, percentOfDisease.count);
    }
    this.UpdatePressurePercent();
  }

  private void UpdatePressurePercent()
  {
    float percent_full = Mathf.Clamp01(this.storage.GetMassAvailable(this.gasElement) / this.maxGasPressure);
    double num = (double) this.smi.sm.pressurePercent.Set(percent_full, this.smi);
    this.pressureMeter.SetPositionPercent(percent_full);
  }

  public bool NeedsDepressurizing() => (double) this.smi.GetPressurePercent() >= (double) this.depressurizePercent;

  private WorkChore<OilWellCap> CreateWorkChore()
  {
    WorkChore<OilWellCap> workChore = new WorkChore<OilWellCap>(Db.Get().ChoreTypes.Depressurize, (IStateMachineTarget) this, only_when_operational: false);
    workChore.AddPrecondition(OilWellCap.AllowedToDepressurize, (object) this);
    return workChore;
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    this.smi.sm.working.Set(true, this.smi);
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    this.smi.sm.working.Set(false, this.smi);
  }

  protected override bool OnWorkTick(Worker worker, float dt) => (double) this.smi.GetPressurePercent() <= 0.0;

  public override bool InstantlyFinish(Worker worker)
  {
    this.ReleaseGasPressure(60f);
    return true;
  }

  public class StatesInstance : 
    GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.GameInstance
  {
    public StatesInstance(OilWellCap master)
      : base(master)
    {
    }

    public float GetPressurePercent() => this.sm.pressurePercent.Get(this.smi);
  }

  public class States : GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap>
  {
    public StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.FloatParameter pressurePercent;
    public StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.BoolParameter working;
    public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State inoperational;
    public OilWellCap.States.OperationalStates operational;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.inoperational;
      this.inoperational.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, (GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State) this.operational, new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.IsOperational));
      this.operational.ToggleRecurringChore((Func<OilWellCap.StatesInstance, Chore>) (smi => (Chore) smi.master.CreateWorkChore())).DefaultState(this.operational.idle);
      this.operational.idle.PlayAnim("off").ToggleStatusItem(Db.Get().BuildingStatusItems.WellPressurizing).ParamTransition<float>((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Parameter<float>) this.pressurePercent, this.operational.overpressure, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsGTEOne).ParamTransition<bool>((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Parameter<bool>) this.working, (GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State) this.operational.releasing_pressure, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsTrue).EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Not(new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.IsOperational))).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State) this.operational.active, new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.IsAbleToPump));
      this.operational.active.DefaultState(this.operational.active.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.WellPressurizing).Enter((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Exit((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).Update((Action<OilWellCap.StatesInstance, float>) ((smi, dt) => smi.master.AddGasPressure(dt)));
      this.operational.active.pre.PlayAnim("working_pre").ParamTransition<float>((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Parameter<float>) this.pressurePercent, this.operational.overpressure, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsGTEOne).ParamTransition<bool>((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Parameter<bool>) this.working, (GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State) this.operational.releasing_pressure, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsTrue).OnAnimQueueComplete(this.operational.active.loop);
      this.operational.active.loop.PlayAnim("working_loop", (KAnim.PlayMode) 0).ParamTransition<float>((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Parameter<float>) this.pressurePercent, this.operational.active.pst, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsGTEOne).ParamTransition<bool>((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Parameter<bool>) this.working, this.operational.active.pst, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsTrue).EventTransition(GameHashes.OperationalChanged, this.operational.active.pst, new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.MustStopPumping)).EventTransition(GameHashes.OnStorageChange, this.operational.active.pst, new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.MustStopPumping));
      this.operational.active.pst.PlayAnim("working_pst").OnAnimQueueComplete(this.operational.idle);
      this.operational.overpressure.PlayAnim("over_pressured_pre", (KAnim.PlayMode) 1).QueueAnim("over_pressured_loop", true).ToggleStatusItem(Db.Get().BuildingStatusItems.WellOverpressure).ParamTransition<float>((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Parameter<float>) this.pressurePercent, this.operational.idle, (StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Parameter<float>.Callback) ((smi, p) => (double) p <= 0.0)).ParamTransition<bool>((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Parameter<bool>) this.working, (GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State) this.operational.releasing_pressure, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsTrue);
      this.operational.releasing_pressure.DefaultState(this.operational.releasing_pressure.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingElement, (Func<OilWellCap.StatesInstance, object>) (smi => (object) smi.master));
      this.operational.releasing_pressure.pre.PlayAnim("steam_out_pre").OnAnimQueueComplete(this.operational.releasing_pressure.loop);
      this.operational.releasing_pressure.loop.PlayAnim("steam_out_loop", (KAnim.PlayMode) 0).EventTransition(GameHashes.OperationalChanged, this.operational.releasing_pressure.pst, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Not(new StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Transition.ConditionCallback(this.IsOperational))).ParamTransition<bool>((StateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.Parameter<bool>) this.working, this.operational.releasing_pressure.pst, GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.IsFalse).Update((Action<OilWellCap.StatesInstance, float>) ((smi, dt) => smi.master.ReleaseGasPressure(dt)));
      this.operational.releasing_pressure.pst.PlayAnim("steam_out_pst").OnAnimQueueComplete(this.operational.idle);
    }

    private bool IsOperational(OilWellCap.StatesInstance smi) => smi.master.operational.IsOperational;

    private bool IsAbleToPump(OilWellCap.StatesInstance smi) => smi.master.operational.IsOperational && smi.GetComponent<ElementConverter>().HasEnoughMassToStartConverting();

    private bool MustStopPumping(OilWellCap.StatesInstance smi) => !smi.master.operational.IsOperational || !smi.GetComponent<ElementConverter>().CanConvertAtAll();

    public class OperationalStates : 
      GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State
    {
      public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State idle;
      public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.PreLoopPostState active;
      public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.State overpressure;
      public GameStateMachine<OilWellCap.States, OilWellCap.StatesInstance, OilWellCap, object>.PreLoopPostState releasing_pressure;
    }
  }
}
