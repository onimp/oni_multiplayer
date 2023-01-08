// Decompiled with JetBrains decompiler
// Type: ManualGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using TUNING;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/Workable/ManualGenerator")]
public class ManualGenerator : Workable, ISingleSliderControl, ISliderControl
{
  [Serialize]
  [SerializeField]
  private float batteryRefillPercent = 0.5f;
  private const float batteryStopRunningPercent = 1f;
  [MyCmpReq]
  private Generator generator;
  [MyCmpReq]
  private Operational operational;
  [MyCmpGet]
  private BuildingEnabledButton buildingEnabledButton;
  private Chore chore;
  private int powerCell;
  private ManualGenerator.GeneratePowerSM.Instance smi;
  private static readonly KAnimHashedString[] symbol_names = new KAnimHashedString[6]
  {
    KAnimHashedString.op_Implicit("meter"),
    KAnimHashedString.op_Implicit("meter_target"),
    KAnimHashedString.op_Implicit("meter_fill"),
    KAnimHashedString.op_Implicit("meter_frame"),
    KAnimHashedString.op_Implicit("meter_light"),
    KAnimHashedString.op_Implicit("meter_tubing")
  };
  private static readonly EventSystem.IntraObjectHandler<ManualGenerator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ManualGenerator>((Action<ManualGenerator, object>) ((component, data) => component.OnOperationalChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<ManualGenerator> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<ManualGenerator>((Action<ManualGenerator, object>) ((component, data) => component.OnActiveChanged(data)));

  public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.TITLE";

  public string SliderUnits => (string) UI.UNITSUFFIXES.PERCENT;

  public int SliderDecimalPlaces(int index) => 0;

  public float GetSliderMin(int index) => 0.0f;

  public float GetSliderMax(int index) => 100f;

  public float GetSliderValue(int index) => this.batteryRefillPercent * 100f;

  public void SetSliderValue(float value, int index) => this.batteryRefillPercent = value / 100f;

  public string GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.TOOLTIP";

  string ISliderControl.GetSliderTooltip() => string.Format(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.UISIDESCREENS.MANUALGENERATORSIDESCREEN.TOOLTIP")), (object) (float) ((double) this.batteryRefillPercent * 100.0));

  public bool IsPowered => this.operational.IsActive;

  private ManualGenerator() => this.showProgressBar = false;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<ManualGenerator>(-592767678, ManualGenerator.OnOperationalChangedDelegate);
    this.Subscribe<ManualGenerator>(824508782, ManualGenerator.OnActiveChangedDelegate);
    this.workerStatusItem = Db.Get().DuplicantStatusItems.GeneratingPower;
    this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    EnergyGenerator.EnsureStatusItemAvailable();
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.SetWorkTime(float.PositiveInfinity);
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    foreach (KAnimHashedString symbolName in ManualGenerator.symbol_names)
      component.SetSymbolVisiblity(symbolName, false);
    this.powerCell = ((Component) this).GetComponent<Building>().GetPowerOutputCell();
    this.OnActiveChanged((object) null);
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_generatormanual_kanim"))
    };
    this.smi = new ManualGenerator.GeneratePowerSM.Instance((IStateMachineTarget) this);
    this.smi.StartSM();
    Game.Instance.energySim.AddManualGenerator(this);
  }

  protected override void OnCleanUp()
  {
    Game.Instance.energySim.RemoveManualGenerator(this);
    this.smi.StopSM("cleanup");
    base.OnCleanUp();
  }

  protected void OnActiveChanged(object is_active)
  {
    if (!this.operational.IsActive)
      return;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.ManualGeneratorChargingUp);
  }

  public void EnergySim200ms(float dt)
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (this.operational.IsActive)
    {
      this.generator.GenerateJoules(this.generator.WattageRating * dt);
      component.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, (object) this.generator);
    }
    else
    {
      this.generator.ResetJoules();
      component.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.GeneratorOffline);
      if (!this.operational.IsOperational)
        return;
      CircuitManager circuitManager = Game.Instance.circuitManager;
      if (circuitManager == null)
        return;
      ushort circuitId = circuitManager.GetCircuitID((ICircuitConnected) this.generator);
      bool flag1 = circuitManager.HasBatteries(circuitId);
      bool flag2 = false;
      if (!flag1 && circuitManager.HasConsumers(circuitId))
        flag2 = true;
      else if (flag1)
      {
        if ((double) this.batteryRefillPercent <= 0.0 && (double) circuitManager.GetMinBatteryPercentFullOnCircuit(circuitId) <= 0.0)
          flag2 = true;
        else if ((double) circuitManager.GetMinBatteryPercentFullOnCircuit(circuitId) < (double) this.batteryRefillPercent)
          flag2 = true;
      }
      if (flag2)
      {
        if (this.chore == null && this.smi.GetCurrentState() == this.smi.sm.on)
          this.chore = (Chore) new WorkChore<ManualGenerator>(Db.Get().ChoreTypes.GeneratePower, (IStateMachineTarget) this);
      }
      else if (this.chore != null)
      {
        this.chore.Cancel("No refill needed");
        this.chore = (Chore) null;
      }
      component.ToggleStatusItem(EnergyGenerator.BatteriesSufficientlyFull, !flag2);
    }
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    this.operational.SetActive(true);
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    CircuitManager circuitManager = Game.Instance.circuitManager;
    bool flag1 = false;
    if (circuitManager != null)
    {
      ushort circuitId = circuitManager.GetCircuitID((ICircuitConnected) this.generator);
      bool flag2 = circuitManager.HasBatteries(circuitId);
      flag1 = flag2 && (double) circuitManager.GetMinBatteryPercentFullOnCircuit(circuitId) < 1.0 || !flag2 && circuitManager.HasConsumers(circuitId);
    }
    AttributeLevels component = ((Component) worker).GetComponent<AttributeLevels>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.AddExperience(Db.Get().Attributes.Athletics.Id, dt, DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE);
    return !flag1;
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    this.operational.SetActive(false);
  }

  protected override void OnCompleteWork(Worker worker)
  {
    this.operational.SetActive(false);
    if (this.chore == null)
      return;
    this.chore.Cancel("complete");
    this.chore = (Chore) null;
  }

  public override bool InstantlyFinish(Worker worker) => false;

  private void OnOperationalChanged(object data)
  {
    if (this.buildingEnabledButton.IsEnabled)
      return;
    this.generator.ResetJoules();
  }

  public class GeneratePowerSM : 
    GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance>
  {
    public GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State off;
    public GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State on;
    public ManualGenerator.GeneratePowerSM.WorkingStates working;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.off;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.off.EventTransition(GameHashes.OperationalChanged, this.on, (StateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.master.GetComponent<Operational>().IsOperational)).PlayAnim("off");
      this.on.EventTransition(GameHashes.OperationalChanged, this.off, (StateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.master.GetComponent<Operational>().IsOperational)).EventTransition(GameHashes.ActiveChanged, this.working.pre, (StateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.master.GetComponent<Operational>().IsActive)).PlayAnim("on");
      this.working.DefaultState(this.working.pre);
      this.working.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.working.loop);
      this.working.loop.PlayAnim("working_loop", (KAnim.PlayMode) 0).EventTransition(GameHashes.ActiveChanged, this.off, (StateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) this.masterTarget.Get(smi), (Object) null) && !smi.master.GetComponent<Operational>().IsActive));
    }

    public class WorkingStates : 
      GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State
    {
      public GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State pre;
      public GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State loop;
      public GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.State pst;
    }

    public new class Instance : 
      GameStateMachine<ManualGenerator.GeneratePowerSM, ManualGenerator.GeneratePowerSM.Instance, IStateMachineTarget, object>.GameInstance
    {
      public Instance(IStateMachineTarget master)
        : base(master)
      {
      }
    }
  }
}
