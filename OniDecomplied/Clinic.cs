// Decompiled with JetBrains decompiler
// Type: Clinic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Clinic")]
public class Clinic : Workable, IGameObjectEffectDescriptor, ISingleSliderControl, ISliderControl
{
  [MyCmpReq]
  private Assignable assignable;
  private static readonly string[] EffectsRemoved = new string[1]
  {
    "SoreBack"
  };
  private const int MAX_RANGE = 10;
  private const float CHECK_RANGE_INTERVAL = 10f;
  public float doctorVisitInterval = 300f;
  public KAnimFile[] workerInjuredAnims;
  public KAnimFile[] workerDiseasedAnims;
  public string diseaseEffect;
  public string healthEffect;
  public string doctoredDiseaseEffect;
  public string doctoredHealthEffect;
  public string doctoredPlaceholderEffect;
  private Clinic.ClinicSM.Instance clinicSMI;
  public static readonly Chore.Precondition IsOverSicknessThreshold = new Chore.Precondition()
  {
    id = nameof (IsOverSicknessThreshold),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_BEING_ATTACKED,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => ((Clinic) data).IsHealthBelowThreshold(context.consumerState.gameObject))
  };
  [Serialize]
  private float sicknessSliderValue = 70f;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.showProgressBar = false;
    this.assignable.subSlots = new AssignableSlot[1]
    {
      Db.Get().AssignableSlots.MedicalBed
    };
    this.assignable.AddAutoassignPrecondition(new Func<MinionAssignablesProxy, bool>(this.CanAutoAssignTo));
    this.assignable.AddAssignPrecondition(new Func<MinionAssignablesProxy, bool>(this.CanManuallyAssignTo));
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    Prioritizable.AddRef(((Component) this).gameObject);
    Components.Clinics.Add(this);
    this.SetWorkTime(float.PositiveInfinity);
    this.clinicSMI = new Clinic.ClinicSM.Instance(this);
    this.clinicSMI.StartSM();
  }

  protected override void OnCleanUp()
  {
    Prioritizable.RemoveRef(((Component) this).gameObject);
    Components.Clinics.Remove(this);
    base.OnCleanUp();
  }

  private KAnimFile[] GetAppropriateOverrideAnims(Worker worker)
  {
    KAnimFile[] appropriateOverrideAnims = (KAnimFile[]) null;
    if (!((Component) worker).GetSMI<WoundMonitor.Instance>().ShouldExitInfirmary())
      appropriateOverrideAnims = this.workerInjuredAnims;
    else if (this.workerDiseasedAnims != null && this.IsValidEffect(this.diseaseEffect) && ((Component) worker).GetSMI<SicknessMonitor.Instance>().IsSick())
      appropriateOverrideAnims = this.workerDiseasedAnims;
    return appropriateOverrideAnims;
  }

  public override Workable.AnimInfo GetAnim(Worker worker)
  {
    this.overrideAnims = this.GetAppropriateOverrideAnims(worker);
    return base.GetAnim(worker);
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    ((Component) worker).GetComponent<Effects>().Add("Sleep", false);
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    KAnimFile[] appropriateOverrideAnims = this.GetAppropriateOverrideAnims(worker);
    if (appropriateOverrideAnims == null || appropriateOverrideAnims != this.overrideAnims)
      return true;
    base.OnWorkTick(worker, dt);
    return false;
  }

  protected override void OnStopWork(Worker worker)
  {
    ((Component) worker).GetComponent<Effects>().Remove("Sleep");
    base.OnStopWork(worker);
  }

  protected override void OnCompleteWork(Worker worker)
  {
    this.assignable.Unassign();
    base.OnCompleteWork(worker);
    Effects component = ((Component) worker).GetComponent<Effects>();
    for (int index = 0; index < Clinic.EffectsRemoved.Length; ++index)
    {
      string effect_id = Clinic.EffectsRemoved[index];
      component.Remove(effect_id);
    }
  }

  public override bool InstantlyFinish(Worker worker) => false;

  private Chore CreateWorkChore(
    ChoreType chore_type,
    bool allow_prioritization,
    bool allow_in_red_alert,
    PriorityScreen.PriorityClass priority_class,
    bool ignore_schedule_block = false)
  {
    ChoreType chore_type1 = chore_type;
    bool flag = allow_prioritization;
    int num1 = allow_in_red_alert ? 1 : 0;
    PriorityScreen.PriorityClass priorityClass = priority_class;
    int num2 = ignore_schedule_block ? 1 : 0;
    int num3 = flag ? 1 : 0;
    int priority_class1 = (int) priorityClass;
    return (Chore) new WorkChore<Clinic>(chore_type1, (IStateMachineTarget) this, allow_in_red_alert: (num1 != 0), ignore_schedule_block: (num2 != 0), allow_prioritization: (num3 != 0), priority_class: ((PriorityScreen.PriorityClass) priority_class1), add_to_daily_report: false);
  }

  private bool CanAutoAssignTo(MinionAssignablesProxy worker)
  {
    bool flag = false;
    MinionIdentity target = worker.target as MinionIdentity;
    if (Object.op_Inequality((Object) target, (Object) null))
    {
      if (this.IsValidEffect(this.healthEffect))
      {
        Health component = ((Component) target).GetComponent<Health>();
        if (Object.op_Inequality((Object) component, (Object) null) && (double) component.hitPoints < (double) component.maxHitPoints)
          flag = true;
      }
      if (!flag && this.IsValidEffect(this.diseaseEffect))
        flag = ((Component) target).GetComponent<MinionModifiers>().sicknesses.Count > 0;
    }
    return flag;
  }

  private bool CanManuallyAssignTo(MinionAssignablesProxy worker)
  {
    bool flag = false;
    MinionIdentity target = worker.target as MinionIdentity;
    if (Object.op_Inequality((Object) target, (Object) null))
      flag = this.IsHealthBelowThreshold(((Component) target).gameObject);
    return flag;
  }

  private bool IsHealthBelowThreshold(GameObject minion)
  {
    Health health = Object.op_Inequality((Object) minion, (Object) null) ? minion.GetComponent<Health>() : (Health) null;
    if (Object.op_Inequality((Object) health, (Object) null))
    {
      float num = health.hitPoints / health.maxHitPoints;
      if (Object.op_Inequality((Object) health, (Object) null))
        return (double) num < (double) this.MedicalAttentionMinimum;
    }
    return false;
  }

  private bool IsValidEffect(string effect) => effect != null && effect != "";

  private bool AllowDoctoring() => this.IsValidEffect(this.doctoredDiseaseEffect) || this.IsValidEffect(this.doctoredHealthEffect);

  public override List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = base.GetDescriptors(go);
    if (this.IsValidEffect(this.healthEffect))
      Effect.AddModifierDescriptions(((Component) this).gameObject, descriptors, this.healthEffect);
    if (this.diseaseEffect != this.healthEffect && this.IsValidEffect(this.diseaseEffect))
      Effect.AddModifierDescriptions(((Component) this).gameObject, descriptors, this.diseaseEffect);
    if (this.AllowDoctoring())
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.DOCTORING, (string) UI.BUILDINGEFFECTS.TOOLTIPS.DOCTORING, (Descriptor.DescriptorType) 1);
      descriptors.Add(descriptor);
      if (this.IsValidEffect(this.doctoredHealthEffect))
        Effect.AddModifierDescriptions(((Component) this).gameObject, descriptors, this.doctoredHealthEffect, true);
      if (this.doctoredDiseaseEffect != this.doctoredHealthEffect && this.IsValidEffect(this.doctoredDiseaseEffect))
        Effect.AddModifierDescriptions(((Component) this).gameObject, descriptors, this.doctoredDiseaseEffect, true);
    }
    return descriptors;
  }

  public float MedicalAttentionMinimum => this.sicknessSliderValue / 100f;

  string ISliderControl.SliderTitleKey => "STRINGS.UI.UISIDESCREENS.MEDICALCOTSIDESCREEN.TITLE";

  string ISliderControl.SliderUnits => (string) UI.UNITSUFFIXES.PERCENT;

  int ISliderControl.SliderDecimalPlaces(int index) => 0;

  float ISliderControl.GetSliderMin(int index) => 0.0f;

  float ISliderControl.GetSliderMax(int index) => 100f;

  float ISliderControl.GetSliderValue(int index) => this.sicknessSliderValue;

  void ISliderControl.SetSliderValue(float percent, int index)
  {
    if ((double) percent == (double) this.sicknessSliderValue)
      return;
    this.sicknessSliderValue = (float) Mathf.RoundToInt(percent);
    Game.Instance.Trigger(875045922, (object) null);
  }

  string ISliderControl.GetSliderTooltip() => string.Format(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.UISIDESCREENS.MEDICALCOTSIDESCREEN.TOOLTIP")), (object) this.sicknessSliderValue);

  string ISliderControl.GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.MEDICALCOTSIDESCREEN.TOOLTIP";

  public class ClinicSM : GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic>
  {
    public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State unoperational;
    public Clinic.ClinicSM.OperationalStates operational;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      this.serializable = StateMachine.SerializeType.Never;
      default_state = (StateMachine.BaseState) this.unoperational;
      this.unoperational.EventTransition(GameHashes.OperationalChanged, (GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State) this.operational, (StateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational)).Enter((StateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State.Callback) (smi => ((Component) smi.master).GetComponent<Assignable>().Unassign()));
      this.operational.DefaultState(this.operational.idle).EventTransition(GameHashes.OperationalChanged, this.unoperational, (StateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.Transition.ConditionCallback) (smi => !((Component) smi.master).GetComponent<Operational>().IsOperational)).EventTransition(GameHashes.AssigneeChanged, this.unoperational).ToggleRecurringChore((Func<Clinic.ClinicSM.Instance, Chore>) (smi => smi.master.CreateWorkChore(Db.Get().ChoreTypes.Heal, false, true, PriorityScreen.PriorityClass.personalNeeds)), (Func<Clinic.ClinicSM.Instance, bool>) (smi => !string.IsNullOrEmpty(smi.master.healthEffect))).ToggleRecurringChore((Func<Clinic.ClinicSM.Instance, Chore>) (smi => smi.master.CreateWorkChore(Db.Get().ChoreTypes.HealCritical, false, true, PriorityScreen.PriorityClass.personalNeeds)), (Func<Clinic.ClinicSM.Instance, bool>) (smi => !string.IsNullOrEmpty(smi.master.healthEffect))).ToggleRecurringChore((Func<Clinic.ClinicSM.Instance, Chore>) (smi => smi.master.CreateWorkChore(Db.Get().ChoreTypes.RestDueToDisease, false, true, PriorityScreen.PriorityClass.personalNeeds, true)), (Func<Clinic.ClinicSM.Instance, bool>) (smi => !string.IsNullOrEmpty(smi.master.diseaseEffect))).ToggleRecurringChore((Func<Clinic.ClinicSM.Instance, Chore>) (smi => smi.master.CreateWorkChore(Db.Get().ChoreTypes.SleepDueToDisease, false, true, PriorityScreen.PriorityClass.personalNeeds, true)), (Func<Clinic.ClinicSM.Instance, bool>) (smi => !string.IsNullOrEmpty(smi.master.diseaseEffect)));
      this.operational.idle.WorkableStartTransition((Func<Clinic.ClinicSM.Instance, Workable>) (smi => (Workable) smi.master), (GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State) this.operational.healing);
      this.operational.healing.DefaultState(this.operational.healing.undoctored).WorkableStopTransition((Func<Clinic.ClinicSM.Instance, Workable>) (smi => (Workable) smi.GetComponent<Clinic>()), this.operational.idle).Enter((StateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State.Callback) (smi => ((Component) smi.master).GetComponent<Operational>().SetActive(true))).Exit((StateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State.Callback) (smi => ((Component) smi.master).GetComponent<Operational>().SetActive(false)));
      this.operational.healing.undoctored.Enter((StateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State.Callback) (smi =>
      {
        smi.StartEffect(smi.master.healthEffect, false);
        smi.StartEffect(smi.master.diseaseEffect, false);
        bool flag = false;
        if (Object.op_Inequality((Object) smi.master.worker, (Object) null))
          flag = smi.HasEffect(smi.master.doctoredHealthEffect) || smi.HasEffect(smi.master.doctoredDiseaseEffect) || smi.HasEffect(smi.master.doctoredPlaceholderEffect);
        if (!smi.master.AllowDoctoring())
          return;
        if (flag)
          smi.GoTo((StateMachine.BaseState) this.operational.healing.doctored);
        else
          smi.StartDoctorChore();
      })).Exit((StateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State.Callback) (smi =>
      {
        smi.StopEffect(smi.master.healthEffect);
        smi.StopEffect(smi.master.diseaseEffect);
        smi.StopDoctorChore();
      }));
      this.operational.healing.newlyDoctored.Enter((StateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State.Callback) (smi =>
      {
        smi.StartEffect(smi.master.doctoredDiseaseEffect, true);
        smi.StartEffect(smi.master.doctoredHealthEffect, true);
        smi.GoTo((StateMachine.BaseState) this.operational.healing.doctored);
      }));
      this.operational.healing.doctored.Enter((StateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State.Callback) (smi =>
      {
        Effects component = ((Component) smi.master.worker).GetComponent<Effects>();
        if (!smi.HasEffect(smi.master.doctoredPlaceholderEffect))
          return;
        EffectInstance effectInstance1 = component.Get(smi.master.doctoredPlaceholderEffect);
        EffectInstance effectInstance2 = smi.StartEffect(smi.master.doctoredDiseaseEffect, true);
        if (effectInstance2 != null)
        {
          float num = effectInstance1.effect.duration - effectInstance1.timeRemaining;
          effectInstance2.timeRemaining = effectInstance2.effect.duration - num;
        }
        EffectInstance effectInstance3 = smi.StartEffect(smi.master.doctoredHealthEffect, true);
        if (effectInstance3 != null)
        {
          float num = effectInstance1.effect.duration - effectInstance1.timeRemaining;
          effectInstance3.timeRemaining = effectInstance3.effect.duration - num;
        }
        component.Remove(smi.master.doctoredPlaceholderEffect);
      })).ScheduleGoTo((Func<Clinic.ClinicSM.Instance, float>) (smi =>
      {
        Effects component = ((Component) smi.master.worker).GetComponent<Effects>();
        float num = smi.master.doctorVisitInterval;
        if (smi.HasEffect(smi.master.doctoredHealthEffect))
        {
          EffectInstance effectInstance = component.Get(smi.master.doctoredHealthEffect);
          num = Mathf.Min(num, effectInstance.GetTimeRemaining());
        }
        if (smi.HasEffect(smi.master.doctoredDiseaseEffect))
        {
          EffectInstance effectInstance = component.Get(smi.master.doctoredDiseaseEffect);
          num = Mathf.Min(num, effectInstance.GetTimeRemaining());
        }
        return num;
      }), (StateMachine.BaseState) this.operational.healing.undoctored).Exit((StateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State.Callback) (smi =>
      {
        Effects component = ((Component) smi.master.worker).GetComponent<Effects>();
        if (!smi.HasEffect(smi.master.doctoredDiseaseEffect) && !smi.HasEffect(smi.master.doctoredHealthEffect))
          return;
        EffectInstance effectInstance4 = component.Get(smi.master.doctoredDiseaseEffect) ?? component.Get(smi.master.doctoredHealthEffect);
        EffectInstance effectInstance5 = smi.StartEffect(smi.master.doctoredPlaceholderEffect, true);
        effectInstance5.timeRemaining = effectInstance5.effect.duration - (effectInstance4.effect.duration - effectInstance4.timeRemaining);
        component.Remove(smi.master.doctoredDiseaseEffect);
        component.Remove(smi.master.doctoredHealthEffect);
      }));
    }

    public class OperationalStates : 
      GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State
    {
      public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State idle;
      public Clinic.ClinicSM.HealingStates healing;
    }

    public class HealingStates : 
      GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State
    {
      public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State undoctored;
      public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State doctored;
      public GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.State newlyDoctored;
    }

    public new class Instance : 
      GameStateMachine<Clinic.ClinicSM, Clinic.ClinicSM.Instance, Clinic, object>.GameInstance
    {
      private WorkChore<DoctorChoreWorkable> doctorChore;

      public Instance(Clinic master)
        : base(master)
      {
      }

      public void StartDoctorChore()
      {
        if (!this.master.IsValidEffect(this.master.doctoredHealthEffect) && !this.master.IsValidEffect(this.master.doctoredDiseaseEffect))
          return;
        this.doctorChore = new WorkChore<DoctorChoreWorkable>(Db.Get().ChoreTypes.Doctor, (IStateMachineTarget) this.smi.master, ignore_building_assignment: true);
        WorkChore<DoctorChoreWorkable> doctorChore = this.doctorChore;
        doctorChore.onComplete = doctorChore.onComplete + (Action<Chore>) (chore => this.smi.GoTo((StateMachine.BaseState) this.smi.sm.operational.healing.newlyDoctored));
      }

      public void StopDoctorChore()
      {
        if (this.doctorChore == null)
          return;
        this.doctorChore.Cancel(nameof (StopDoctorChore));
        this.doctorChore = (WorkChore<DoctorChoreWorkable>) null;
      }

      public bool HasEffect(string effect)
      {
        bool flag = false;
        if (this.master.IsValidEffect(effect))
          flag = ((Component) this.smi.master.worker).GetComponent<Effects>().HasEffect(effect);
        return flag;
      }

      public EffectInstance StartEffect(string effect, bool should_save)
      {
        if (this.master.IsValidEffect(effect))
        {
          Worker worker = this.smi.master.worker;
          if (Object.op_Inequality((Object) worker, (Object) null))
          {
            Effects component = ((Component) worker).GetComponent<Effects>();
            if (!component.HasEffect(effect))
              return component.Add(effect, should_save);
          }
        }
        return (EffectInstance) null;
      }

      public void StopEffect(string effect)
      {
        if (!this.master.IsValidEffect(effect))
          return;
        Worker worker = this.smi.master.worker;
        if (!Object.op_Inequality((Object) worker, (Object) null))
          return;
        Effects component = ((Component) worker).GetComponent<Effects>();
        if (!component.HasEffect(effect))
          return;
        component.Remove(effect);
      }
    }
  }
}
