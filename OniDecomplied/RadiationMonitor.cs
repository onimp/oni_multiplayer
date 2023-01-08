// Decompiled with JetBrains decompiler
// Type: RadiationMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using Klei.CustomSettings;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class RadiationMonitor : GameStateMachine<RadiationMonitor, RadiationMonitor.Instance>
{
  public const float BASE_ABSORBTION_RATE = 1f;
  public const float MIN_TIME_BETWEEN_EXPOSURE_REACTS = 120f;
  public const float MIN_TIME_BETWEEN_SICK_REACTS = 60f;
  public const int VOMITS_PER_CYCLE_MAJOR = 5;
  public const int VOMITS_PER_CYCLE_EXTREME = 10;
  public StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.FloatParameter radiationExposure;
  public StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.FloatParameter currentExposurePerCycle;
  public StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.BoolParameter isSick;
  public StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.FloatParameter timeUntilNextExposureReact;
  public StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.FloatParameter timeUntilNextSickReact;
  public static string minorSicknessEffect = "RadiationExposureMinor";
  public static string majorSicknessEffect = "RadiationExposureMajor";
  public static string extremeSicknessEffect = "RadiationExposureExtreme";
  public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State init;
  public RadiationMonitor.ActiveStates active;
  public static readonly StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_RECOVERY_IMMEDIATE = (StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p > 100.0 * (double) smi.difficultySettingMod / 2.0);
  public static readonly StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_REACT = (StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p >= 133.0 * (double) smi.difficultySettingMod);
  public static readonly StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_LT_MINOR = (StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p < 100.0 * (double) smi.difficultySettingMod);
  public static readonly StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_GTE_MINOR = (StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p >= 100.0 * (double) smi.difficultySettingMod);
  public static readonly StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_GTE_MAJOR = (StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p >= 300.0 * (double) smi.difficultySettingMod);
  public static readonly StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_GTE_EXTREME = (StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p >= 600.0 * (double) smi.difficultySettingMod);
  public static readonly StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback COMPARE_GTE_DEADLY = (StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p >= 900.0 * (double) smi.difficultySettingMod);

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    default_state = (StateMachine.BaseState) this.init;
    this.init.Transition((GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State) null, (StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !Sim.IsRadiationEnabled())).Transition((GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State) this.active, (StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => Sim.IsRadiationEnabled()));
    this.active.Update(new System.Action<RadiationMonitor.Instance, float>(RadiationMonitor.CheckRadiationLevel), (UpdateRate) 6, false).DefaultState(this.active.idle);
    this.active.idle.DoNothing().ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, this.active.sick.deadly, RadiationMonitor.COMPARE_GTE_DEADLY).ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, (GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State) this.active.sick.extreme, RadiationMonitor.COMPARE_GTE_EXTREME).ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, (GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State) this.active.sick.major, RadiationMonitor.COMPARE_GTE_MAJOR).ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, (GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State) this.active.sick.minor, RadiationMonitor.COMPARE_GTE_MINOR);
    this.active.sick.ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, this.active.idle, RadiationMonitor.COMPARE_LT_MINOR).Enter((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.sm.isSick.Set(true, smi))).Exit((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.sm.isSick.Set(false, smi)));
    this.active.sick.minor.ToggleEffect(RadiationMonitor.minorSicknessEffect).ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, this.active.sick.deadly, RadiationMonitor.COMPARE_GTE_DEADLY).ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, (GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State) this.active.sick.extreme, RadiationMonitor.COMPARE_GTE_EXTREME).ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, (GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State) this.active.sick.major, RadiationMonitor.COMPARE_GTE_MAJOR).ToggleAnims("anim_loco_radiation1_kanim", 4f).ToggleAnims("anim_idle_radiation1_kanim", 4f).ToggleExpression(Db.Get().Expressions.Radiation1).DefaultState(this.active.sick.minor.waiting);
    this.active.sick.minor.reacting.ToggleChore(new Func<RadiationMonitor.Instance, Chore>(this.CreateVomitChore), this.active.sick.minor.waiting);
    this.active.sick.major.ToggleEffect(RadiationMonitor.majorSicknessEffect).ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, this.active.sick.deadly, RadiationMonitor.COMPARE_GTE_DEADLY).ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, (GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State) this.active.sick.extreme, RadiationMonitor.COMPARE_GTE_EXTREME).ToggleAnims("anim_loco_radiation2_kanim", 4f).ToggleAnims("anim_idle_radiation2_kanim", 4f).ToggleExpression(Db.Get().Expressions.Radiation2).DefaultState(this.active.sick.major.waiting);
    this.active.sick.major.waiting.ScheduleGoTo(120f, (StateMachine.BaseState) this.active.sick.major.vomiting);
    this.active.sick.major.vomiting.ToggleChore(new Func<RadiationMonitor.Instance, Chore>(this.CreateVomitChore), this.active.sick.major.waiting);
    this.active.sick.extreme.ParamTransition<float>((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.radiationExposure, this.active.sick.deadly, RadiationMonitor.COMPARE_GTE_DEADLY).ToggleEffect(RadiationMonitor.extremeSicknessEffect).ToggleAnims("anim_loco_radiation3_kanim", 4f).ToggleAnims("anim_idle_radiation3_kanim", 4f).ToggleExpression(Db.Get().Expressions.Radiation3).DefaultState(this.active.sick.extreme.waiting);
    this.active.sick.extreme.waiting.ScheduleGoTo(60f, (StateMachine.BaseState) this.active.sick.extreme.vomiting);
    this.active.sick.extreme.vomiting.ToggleChore(new Func<RadiationMonitor.Instance, Chore>(this.CreateVomitChore), this.active.sick.extreme.waiting);
    this.active.sick.deadly.ToggleAnims("anim_loco_radiation4_kanim", 4f).ToggleAnims("anim_idle_radiation4_kanim", 4f).ToggleExpression(Db.Get().Expressions.Radiation4).Enter((StateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.GetComponent<Health>().Incapacitate(GameTags.RadiationSicknessIncapacitation)));
  }

  private Chore CreateVomitChore(RadiationMonitor.Instance smi)
  {
    Notification notification = new Notification((string) DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_NAME, NotificationType.Bad, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false)));
    return (Chore) new VomitChore(Db.Get().ChoreTypes.Vomit, smi.master, Db.Get().DuplicantStatusItems.Vomiting, notification);
  }

  private static void RadiationRecovery(RadiationMonitor.Instance smi, float dt)
  {
    float delta = Db.Get().Attributes.RadiationRecovery.Lookup(smi.gameObject).GetTotalValue() * dt;
    double num = (double) smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).ApplyDelta(delta);
    smi.master.Trigger(1556680150, (object) delta);
  }

  private static void CheckRadiationLevel(RadiationMonitor.Instance smi, float dt)
  {
    RadiationMonitor.RadiationRecovery(smi, dt);
    double num1 = (double) smi.sm.timeUntilNextExposureReact.Delta(-dt, smi);
    double num2 = (double) smi.sm.timeUntilNextSickReact.Delta(-dt, smi);
    int cell = Grid.PosToCell(smi.gameObject);
    if (Grid.IsValidCell(cell))
    {
      float num3 = Mathf.Clamp01(1f - Db.Get().Attributes.RadiationResistance.Lookup(smi.gameObject).GetTotalValue());
      float delta = (float) ((double) Grid.Radiation[cell] * 1.0 * (double) num3 / 600.0) * dt;
      double num4 = (double) smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).ApplyDelta(delta);
      float p = (float) ((double) delta / (double) dt * 600.0);
      double num5 = (double) smi.sm.currentExposurePerCycle.Set(p, smi);
      if ((double) smi.sm.timeUntilNextExposureReact.Get(smi) <= 0.0 && !smi.HasTag(GameTags.InTransitTube) && RadiationMonitor.COMPARE_REACT(smi, p))
      {
        double num6 = (double) smi.sm.timeUntilNextExposureReact.Set(120f, smi);
        Emote radiationGlare = Db.Get().Emotes.Minion.Radiation_Glare;
        smi.master.gameObject.GetSMI<ReactionMonitor.Instance>().AddSelfEmoteReactable(smi.master.gameObject, HashedString.op_Implicit("RadiationReact"), radiationGlare, true, Db.Get().ChoreTypes.EmoteHighPriority);
      }
    }
    if ((double) smi.sm.timeUntilNextSickReact.Get(smi) <= 0.0 && smi.sm.isSick.Get(smi) && !smi.HasTag(GameTags.InTransitTube))
    {
      double num7 = (double) smi.sm.timeUntilNextSickReact.Set(60f, smi);
      Emote radiationItch = Db.Get().Emotes.Minion.Radiation_Itch;
      smi.master.gameObject.GetSMI<ReactionMonitor.Instance>().AddSelfEmoteReactable(smi.master.gameObject, HashedString.op_Implicit("RadiationReact"), radiationItch, true, Db.Get().ChoreTypes.RadiationPain);
    }
    double num8 = (double) smi.sm.radiationExposure.Set(smi.master.gameObject.GetComponent<KSelectable>().GetAmounts().GetValue("RadiationBalance"), smi);
  }

  public class ActiveStates : 
    GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State idle;
    public RadiationMonitor.SickStates sick;
  }

  public class SickStates : 
    GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State
  {
    public RadiationMonitor.SickStates.MinorStates minor;
    public RadiationMonitor.SickStates.MajorStates major;
    public RadiationMonitor.SickStates.ExtremeStates extreme;
    public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State deadly;

    public class MinorStates : 
      GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State
    {
      public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State waiting;
      public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State reacting;
    }

    public class MajorStates : 
      GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State
    {
      public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State waiting;
      public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State vomiting;
    }

    public class ExtremeStates : 
      GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State
    {
      public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State waiting;
      public GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.State vomiting;
    }
  }

  public new class Instance : 
    GameStateMachine<RadiationMonitor, RadiationMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Effects effects;
    public float difficultySettingMod = 1f;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.effects = this.GetComponent<Effects>();
      if (!Sim.IsRadiationEnabled())
        return;
      SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Radiation);
      if (currentQualitySetting == null)
        return;
      switch (currentQualitySetting.id)
      {
        case "Easiest":
          this.difficultySettingMod = DUPLICANTSTATS.RADIATION_DIFFICULTY_MODIFIERS.EASIEST;
          break;
        case "Easier":
          this.difficultySettingMod = DUPLICANTSTATS.RADIATION_DIFFICULTY_MODIFIERS.EASIER;
          break;
        case "Harder":
          this.difficultySettingMod = DUPLICANTSTATS.RADIATION_DIFFICULTY_MODIFIERS.HARDER;
          break;
        case "Hardest":
          this.difficultySettingMod = DUPLICANTSTATS.RADIATION_DIFFICULTY_MODIFIERS.HARDEST;
          break;
      }
    }

    public float SicknessSecondsRemaining() => (float) (600.0 * ((double) Mathf.Max(0.0f, this.sm.radiationExposure.Get(this.smi) - 100f * this.difficultySettingMod) / 100.0));

    public string GetEffectStatusTooltip()
    {
      if (this.effects.HasEffect(RadiationMonitor.minorSicknessEffect))
        return this.smi.master.gameObject.GetComponent<Effects>().Get(RadiationMonitor.minorSicknessEffect).statusItem.GetTooltip((object) this.effects.Get(RadiationMonitor.minorSicknessEffect));
      if (this.effects.HasEffect(RadiationMonitor.majorSicknessEffect))
        return this.smi.master.gameObject.GetComponent<Effects>().Get(RadiationMonitor.majorSicknessEffect).statusItem.GetTooltip((object) this.effects.Get(RadiationMonitor.majorSicknessEffect));
      return this.effects.HasEffect(RadiationMonitor.extremeSicknessEffect) ? this.smi.master.gameObject.GetComponent<Effects>().Get(RadiationMonitor.extremeSicknessEffect).statusItem.GetTooltip((object) this.effects.Get(RadiationMonitor.extremeSicknessEffect)) : (string) DUPLICANTS.MODIFIERS.RADIATIONEXPOSUREDEADLY.TOOLTIP;
    }
  }
}
