// Decompiled with JetBrains decompiler
// Type: Geyser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Geyser : StateMachineComponent<Geyser.StatesInstance>, IGameObjectEffectDescriptor
{
  public static Geyser.ModificationMethod massModificationMethod = Geyser.ModificationMethod.Percentages;
  public static Geyser.ModificationMethod temperatureModificationMethod = Geyser.ModificationMethod.Values;
  public static Geyser.ModificationMethod IterationDurationModificationMethod = Geyser.ModificationMethod.Percentages;
  public static Geyser.ModificationMethod IterationPercentageModificationMethod = Geyser.ModificationMethod.Percentages;
  public static Geyser.ModificationMethod yearDurationModificationMethod = Geyser.ModificationMethod.Percentages;
  public static Geyser.ModificationMethod yearPercentageModificationMethod = Geyser.ModificationMethod.Percentages;
  public static Geyser.ModificationMethod maxPressureModificationMethod = Geyser.ModificationMethod.Percentages;
  [MyCmpAdd]
  private ElementEmitter emitter;
  [MyCmpAdd]
  private UserNameable nameable;
  [MyCmpGet]
  private Studyable studyable;
  [Serialize]
  public GeyserConfigurator.GeyserInstanceConfiguration configuration;
  public Vector2I outputOffset;
  public List<Geyser.GeyserModification> modifications = new List<Geyser.GeyserModification>();
  private Geyser.GeyserModification modifier;
  private const float PRE_PCT = 0.1f;
  private const float POST_PCT = 0.05f;

  public float timeShift { private set; get; }

  public float GetCurrentLifeTime() => GameClock.Instance.GetTime() + this.timeShift;

  public void AlterTime(float timeOffset)
  {
    this.timeShift = Mathf.Max(timeOffset, -GameClock.Instance.GetTime());
    float num1 = this.RemainingEruptTime();
    float num2 = this.RemainingNonEruptTime();
    float num3 = this.RemainingActiveTime();
    float num4 = this.RemainingDormantTime();
    double yearLength = (double) this.configuration.GetYearLength();
    if ((double) num2 == 0.0)
    {
      if ((((double) num4 != 0.0 ? 0 : ((double) this.configuration.GetYearOnDuration() - (double) num3 < (double) this.configuration.GetOnDuration() - (double) num1 ? 1 : 0)) | ((double) num3 != 0.0 ? 0 : ((double) this.configuration.GetYearOffDuration() - (double) num4 >= (double) this.configuration.GetOnDuration() - (double) num1 ? 1 : 0))) != 0)
        this.smi.GoTo((StateMachine.BaseState) this.smi.sm.dormant);
      else
        this.smi.GoTo((StateMachine.BaseState) this.smi.sm.erupt);
    }
    else
    {
      int num5 = ((double) num4 != 0.0 ? 0 : ((double) this.configuration.GetYearOnDuration() - (double) num3 < (double) this.configuration.GetIterationLength() - (double) num2 ? 1 : 0)) | ((double) num3 != 0.0 ? 0 : ((double) this.configuration.GetYearOffDuration() - (double) num4 >= (double) this.configuration.GetIterationLength() - (double) num2 ? 1 : 0));
      float num6 = this.RemainingEruptPreTime();
      if (num5 != 0 && (double) num6 <= 0.0)
        this.smi.GoTo((StateMachine.BaseState) this.smi.sm.dormant);
      else if ((double) num6 <= 0.0)
      {
        this.smi.GoTo((StateMachine.BaseState) this.smi.sm.idle);
      }
      else
      {
        float num7 = this.PreDuration() - num6;
        if (((double) num3 == 0.0 ? ((double) this.configuration.GetYearOffDuration() - (double) num4 > (double) num7 ? 1 : 0) : ((double) num7 > (double) this.configuration.GetYearOnDuration() - (double) num3 ? 1 : 0)) != 0)
          this.smi.GoTo((StateMachine.BaseState) this.smi.sm.dormant);
        else
          this.smi.GoTo((StateMachine.BaseState) this.smi.sm.pre_erupt);
      }
    }
  }

  public void ShiftTimeTo(Geyser.TimeShiftStep step)
  {
    float num1 = this.RemainingEruptTime();
    float num2 = this.RemainingNonEruptTime();
    float num3 = this.RemainingActiveTime();
    float num4 = this.RemainingDormantTime();
    float yearLength = this.configuration.GetYearLength();
    switch (step)
    {
      case Geyser.TimeShiftStep.ActiveState:
        this.AlterTime(this.timeShift - ((double) num3 > 0.0 ? this.configuration.GetYearOnDuration() - num3 : yearLength - num4));
        break;
      case Geyser.TimeShiftStep.DormantState:
        this.AlterTime(this.timeShift + ((double) num3 > 0.0 ? num3 : (float) -((double) this.configuration.GetYearOffDuration() - (double) num4)));
        break;
      case Geyser.TimeShiftStep.NextIteration:
        this.AlterTime(this.timeShift + ((double) num1 > 0.0 ? num1 + this.configuration.GetOffDuration() : num2));
        break;
      case Geyser.TimeShiftStep.PreviousIteration:
        float num5 = (double) num1 > 0.0 ? (float) -((double) this.configuration.GetOnDuration() - (double) num1) : (float) -((double) this.configuration.GetIterationLength() - (double) num2);
        if ((double) num1 > 0.0 && (double) Mathf.Abs(num5) < (double) this.configuration.GetOnDuration() * 0.05000000074505806)
          num5 -= this.configuration.GetIterationLength();
        this.AlterTime(this.timeShift + num5);
        break;
    }
  }

  public void AddModification(Geyser.GeyserModification modification)
  {
    this.modifications.Add(modification);
    this.UpdateModifier();
  }

  public void RemoveModification(Geyser.GeyserModification modification)
  {
    this.modifications.Remove(modification);
    this.UpdateModifier();
  }

  private void UpdateModifier()
  {
    this.modifier.Clear();
    foreach (Geyser.GeyserModification modification in this.modifications)
      this.modifier.AddValues(modification);
    this.configuration.SetModifier(this.modifier);
    this.ApplyConfigurationEmissionValues(this.configuration);
    this.RefreshGeotunerFeedback();
  }

  public void RefreshGeotunerFeedback()
  {
    this.RefreshGeotunerStatusItem();
    this.RefreshStudiedMeter();
  }

  private void RefreshGeotunerStatusItem()
  {
    KSelectable component = ((Component) this).gameObject.GetComponent<KSelectable>();
    if (this.GetAmountOfGeotunersPointingThisGeyser() > 0)
      component.AddStatusItem(Db.Get().BuildingStatusItems.GeyserGeotuned, (object) this);
    else
      component.RemoveStatusItem(Db.Get().BuildingStatusItems.GeyserGeotuned, Object.op_Implicit((Object) this));
  }

  private void RefreshStudiedMeter()
  {
    if (!this.studyable.Studied)
      return;
    int num = this.GetAmountOfGeotunersPointingThisGeyser() > 0 ? 1 : 0;
    GeyserConfig.TrackerMeterAnimNames trackerMeterAnimNames = GeyserConfig.TrackerMeterAnimNames.tracker;
    if (num != 0)
    {
      trackerMeterAnimNames = GeyserConfig.TrackerMeterAnimNames.geotracker;
      int affectingThisGeyser = this.GetAmountOfGeotunersAffectingThisGeyser();
      if (affectingThisGeyser > 0)
        trackerMeterAnimNames = GeyserConfig.TrackerMeterAnimNames.geotracker_minor;
      if (affectingThisGeyser >= 5)
        trackerMeterAnimNames = GeyserConfig.TrackerMeterAnimNames.geotracker_major;
    }
    this.studyable.studiedIndicator.meterController.Play(HashedString.op_Implicit(trackerMeterAnimNames.ToString()), (KAnim.PlayMode) 0);
  }

  public int GetAmountOfGeotunersPointingThisGeyser() => Components.GeoTuners.GetItems(((Component) this).gameObject.GetMyWorldId()).Count<GeoTuner.Instance>((Func<GeoTuner.Instance, bool>) (x => Object.op_Equality((Object) x.GetAssignedGeyser(), (Object) this)));

  public int GetAmountOfGeotunersPointingOrWillPointAtThisGeyser() => Components.GeoTuners.GetItems(((Component) this).gameObject.GetMyWorldId()).Count<GeoTuner.Instance>((Func<GeoTuner.Instance, bool>) (x => Object.op_Equality((Object) x.GetAssignedGeyser(), (Object) this) || Object.op_Equality((Object) x.GetFutureGeyser(), (Object) this)));

  public int GetAmountOfGeotunersAffectingThisGeyser()
  {
    int affectingThisGeyser = 0;
    for (int index = 0; index < this.modifications.Count; ++index)
    {
      if (this.modifications[index].originID.Contains("GeoTuner"))
        ++affectingThisGeyser;
    }
    return affectingThisGeyser;
  }

  private void OnGeotunerChanged(object o) => this.RefreshGeotunerFeedback();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Prioritizable.AddRef(((Component) this).gameObject);
    if (this.configuration == null || HashedString.op_Equality(this.configuration.typeId, HashedString.Invalid))
      this.configuration = ((Component) this).GetComponent<GeyserConfigurator>().MakeConfiguration();
    else if ((double) this.configuration.geyserType.geyserTemperature - (double) ((Component) this).gameObject.GetComponent<PrimaryElement>().Temperature != 0.0)
      ((Component) this).gameObject.GetComponent<SimTemperatureTransfer>().onSimRegistered += new Action<SimTemperatureTransfer>(this.OnSimRegistered);
    this.ApplyConfigurationEmissionValues(this.configuration);
    this.GenerateName();
    this.smi.StartSM();
    Workable component = (Workable) ((Component) this).GetComponent<Studyable>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.alwaysShowProgressBar = true;
    Components.Geysers.Add(((Component) this).gameObject.GetMyWorldId(), this);
    KMonoBehaviourExtensions.Subscribe(((Component) this).gameObject, 1763323737, new Action<object>(this.OnGeotunerChanged));
    this.RefreshStudiedMeter();
    this.UpdateModifier();
  }

  private void GenerateName()
  {
    StringKey stringKey;
    // ISSUE: explicit constructor call
    ((StringKey) ref stringKey).\u002Ector("STRINGS.CREATURES.SPECIES.GEYSER." + this.configuration.geyserType.id.ToUpper() + ".NAME");
    if (!(this.nameable.savedName == StringEntry.op_Implicit(Strings.Get(stringKey))))
      return;
    int cell = Grid.PosToCell(((Component) this).gameObject);
    Quadrant[] quadrantOfCell = ((Component) this).gameObject.GetMyWorld().GetQuadrantOfCell(cell, 2);
    string str1 = ((int) quadrantOfCell[0]).ToString();
    int num = (int) quadrantOfCell[1];
    string str2 = num.ToString();
    string str3 = str1 + str2;
    string[] strArray1 = NAMEGEN.GEYSER_IDS.IDs.ToString().Split('\n');
    string str4 = strArray1[Random.Range(0, strArray1.Length)];
    string[] strArray2 = new string[6]
    {
      UI.StripLinkFormatting(((Component) this).gameObject.GetProperName()),
      " ",
      str4,
      str3,
      "‑",
      null
    };
    num = Random.Range(0, 10);
    strArray2[5] = num.ToString();
    this.nameable.SetName(string.Concat(strArray2));
  }

  public void ApplyConfigurationEmissionValues(
    GeyserConfigurator.GeyserInstanceConfiguration config)
  {
    this.emitter.emitRange = (byte) 2;
    this.emitter.maxPressure = config.GetMaxPressure();
    this.emitter.outputElement = new ElementConverter.OutputElement(config.GetEmitRate(), config.GetElement(), config.GetTemperature(), outputElementOffsetx: ((float) this.outputOffset.x), outputElementOffsety: ((float) this.outputOffset.y), addedDiseaseIdx: config.GetDiseaseIdx(), addedDiseaseCount: Mathf.RoundToInt((float) config.GetDiseaseCount() * config.GetEmitRate()));
    if (!this.emitter.IsSimActive)
      return;
    this.emitter.SetSimActive(true);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    KMonoBehaviourExtensions.Unsubscribe(((Component) this).gameObject, 1763323737, new Action<object>(this.OnGeotunerChanged));
    Components.Geysers.Remove(((Component) this).gameObject.GetMyWorldId(), this);
  }

  private void OnSimRegistered(SimTemperatureTransfer stt)
  {
    PrimaryElement component = ((Component) this).gameObject.GetComponent<PrimaryElement>();
    if ((double) this.configuration.geyserType.geyserTemperature - (double) component.Temperature != 0.0)
      component.Temperature = this.configuration.geyserType.geyserTemperature;
    stt.onSimRegistered -= new Action<SimTemperatureTransfer>(this.OnSimRegistered);
  }

  public float RemainingPhaseTimeFrom2(
    float onDuration,
    float offDuration,
    float time,
    Geyser.Phase expectedPhase)
  {
    float num1 = onDuration + offDuration;
    float num2 = time % num1;
    float num3;
    Geyser.Phase phase;
    if ((double) num2 < (double) onDuration)
    {
      num3 = Mathf.Max(onDuration - num2, 0.0f);
      phase = Geyser.Phase.On;
    }
    else
    {
      num3 = Mathf.Max(onDuration + offDuration - num2, 0.0f);
      phase = Geyser.Phase.Off;
    }
    return expectedPhase != Geyser.Phase.Any && phase != expectedPhase ? 0.0f : num3;
  }

  public float RemainingPhaseTimeFrom4(
    float onDuration,
    float pstDuration,
    float offDuration,
    float preDuration,
    float time,
    Geyser.Phase expectedPhase)
  {
    float num1 = onDuration + pstDuration + offDuration + preDuration;
    float num2 = time % num1;
    float num3;
    Geyser.Phase phase;
    if ((double) num2 < (double) onDuration)
    {
      num3 = onDuration - num2;
      phase = Geyser.Phase.On;
    }
    else if ((double) num2 < (double) onDuration + (double) pstDuration)
    {
      num3 = onDuration + pstDuration - num2;
      phase = Geyser.Phase.Pst;
    }
    else if ((double) num2 < (double) onDuration + (double) pstDuration + (double) offDuration)
    {
      num3 = onDuration + pstDuration + offDuration - num2;
      phase = Geyser.Phase.Off;
    }
    else
    {
      num3 = onDuration + pstDuration + offDuration + preDuration - num2;
      phase = Geyser.Phase.Pre;
    }
    return expectedPhase != Geyser.Phase.Any && phase != expectedPhase ? 0.0f : num3;
  }

  private float IdleDuration() => this.configuration.GetOffDuration() * 0.849999964f;

  private float PreDuration() => this.configuration.GetOffDuration() * 0.1f;

  private float PostDuration() => this.configuration.GetOffDuration() * 0.05f;

  private float EruptDuration() => this.configuration.GetOnDuration();

  public bool ShouldGoDormant() => (double) this.RemainingActiveTime() <= 0.0;

  public float RemainingIdleTime() => this.RemainingPhaseTimeFrom4(this.EruptDuration(), this.PostDuration(), this.IdleDuration(), this.PreDuration(), this.GetCurrentLifeTime(), Geyser.Phase.Off);

  public float RemainingEruptPreTime() => this.RemainingPhaseTimeFrom4(this.EruptDuration(), this.PostDuration(), this.IdleDuration(), this.PreDuration(), this.GetCurrentLifeTime(), Geyser.Phase.Pre);

  public float RemainingEruptTime() => this.RemainingPhaseTimeFrom2(this.configuration.GetOnDuration(), this.configuration.GetOffDuration(), this.GetCurrentLifeTime(), Geyser.Phase.On);

  public float RemainingEruptPostTime() => this.RemainingPhaseTimeFrom4(this.EruptDuration(), this.PostDuration(), this.IdleDuration(), this.PreDuration(), this.GetCurrentLifeTime(), Geyser.Phase.Pst);

  public float RemainingNonEruptTime() => this.RemainingPhaseTimeFrom2(this.configuration.GetOnDuration(), this.configuration.GetOffDuration(), this.GetCurrentLifeTime(), Geyser.Phase.Off);

  public float RemainingDormantTime() => this.RemainingPhaseTimeFrom2(this.configuration.GetYearOnDuration(), this.configuration.GetYearOffDuration(), this.GetCurrentLifeTime(), Geyser.Phase.Off);

  public float RemainingActiveTime() => this.RemainingPhaseTimeFrom2(this.configuration.GetYearOnDuration(), this.configuration.GetYearOffDuration(), this.GetCurrentLifeTime(), Geyser.Phase.On);

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    string str1 = ElementLoader.FindElementByHash(this.configuration.GetElement()).tag.ProperName();
    List<GeoTuner.Instance> items = Components.GeoTuners.GetItems(((Component) this).gameObject.GetMyWorldId());
    GeoTuner.Instance instance = items.Find((Predicate<GeoTuner.Instance>) (g => Object.op_Equality((Object) g.GetAssignedGeyser(), (Object) this)));
    int num1 = items.Count<GeoTuner.Instance>((Func<GeoTuner.Instance, bool>) (x => Object.op_Equality((Object) x.GetAssignedGeyser(), (Object) this)));
    bool flag = num1 > 0;
    string str2 = string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION, (object) ElementLoader.FindElementByHash(this.configuration.GetElement()).name, (object) GameUtil.GetFormattedMass(this.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond), (object) GameUtil.GetFormattedTemperature(this.configuration.GetTemperature()));
    if (flag)
    {
      Func<float, float> func = (Func<float, float>) (emissionPerCycleModifier =>
      {
        float num2 = 600f / this.configuration.GetIterationLength();
        return emissionPerCycleModifier / num2 / this.configuration.GetOnDuration();
      });
      int affectingThisGeyser = this.GetAmountOfGeotunersAffectingThisGeyser();
      float temp1 = Geyser.temperatureModificationMethod == Geyser.ModificationMethod.Percentages ? instance.currentGeyserModification.temperatureModifier * this.configuration.geyserType.temperature : instance.currentGeyserModification.temperatureModifier;
      double num3 = Geyser.massModificationMethod == Geyser.ModificationMethod.Percentages ? (double) instance.currentGeyserModification.massPerCycleModifier * (double) this.configuration.scaledRate : (double) instance.currentGeyserModification.massPerCycleModifier;
      float mass1 = func((float) num3);
      float temp2 = (float) affectingThisGeyser * temp1;
      float mass2 = (float) affectingThisGeyser * mass1;
      string str3 = ((double) temp2 > 0.0 ? "+" : "") + GameUtil.GetFormattedTemperature(temp2, interpretation: GameUtil.TemperatureInterpretation.Relative);
      string str4 = ((double) mass2 > 0.0 ? "+" : "") + GameUtil.GetFormattedMass(mass2, GameUtil.TimeSlice.PerSecond, floatFormat: "{0:0.##}");
      string str5 = ((double) temp1 > 0.0 ? "+" : "") + GameUtil.GetFormattedTemperature(temp1, interpretation: GameUtil.TemperatureInterpretation.Relative);
      string str6 = ((double) mass1 > 0.0 ? "+" : "") + GameUtil.GetFormattedMass(mass1, GameUtil.TimeSlice.PerSecond, floatFormat: "{0:0.##}");
      str2 = string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION_GEOTUNED, (object) ElementLoader.FindElementByHash(this.configuration.GetElement()).name, (object) GameUtil.GetFormattedMass(this.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond), (object) GameUtil.GetFormattedTemperature(this.configuration.GetTemperature())) + "\n" + "\n" + string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION_GEOTUNED_COUNT, (object) affectingThisGeyser.ToString(), (object) num1.ToString()) + "\n" + "\n" + string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PRODUCTION_GEOTUNED_TOTAL, (object) str4, (object) str3);
      for (int index = 0; index < affectingThisGeyser; ++index)
      {
        string str7 = "\n    • " + UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP_GEOTUNER_MODIFIER_ROW_TITLE.ToString() + str6 + " " + str5;
        str2 += str7;
      }
    }
    descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.GEYSER_PRODUCTION, (object) str1, (object) GameUtil.GetFormattedMass(this.configuration.GetEmitRate(), GameUtil.TimeSlice.PerSecond), (object) GameUtil.GetFormattedTemperature(this.configuration.GetTemperature())), str2, (Descriptor.DescriptorType) 1, false));
    if (this.configuration.GetDiseaseIdx() != byte.MaxValue)
      descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.GEYSER_DISEASE, (object) GameUtil.GetFormattedDiseaseName(this.configuration.GetDiseaseIdx())), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_DISEASE, (object) GameUtil.GetFormattedDiseaseName(this.configuration.GetDiseaseIdx())), (Descriptor.DescriptorType) 1, false));
    descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.GEYSER_PERIOD, (object) GameUtil.GetFormattedTime(this.configuration.GetOnDuration()), (object) GameUtil.GetFormattedTime(this.configuration.GetIterationLength())), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_PERIOD, (object) GameUtil.GetFormattedTime(this.configuration.GetOnDuration()), (object) GameUtil.GetFormattedTime(this.configuration.GetIterationLength())), (Descriptor.DescriptorType) 1, false));
    Studyable component = ((Component) this).GetComponent<Studyable>();
    if (Object.op_Implicit((Object) component) && !component.Studied)
    {
      descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.GEYSER_YEAR_UNSTUDIED), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_UNSTUDIED), (Descriptor.DescriptorType) 1, false));
      descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.GEYSER_YEAR_AVR_OUTPUT_UNSTUDIED), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT_UNSTUDIED), (Descriptor.DescriptorType) 1, false));
    }
    else
    {
      descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.GEYSER_YEAR_PERIOD, (object) GameUtil.GetFormattedCycles(this.configuration.GetYearOnDuration()), (object) GameUtil.GetFormattedCycles(this.configuration.GetYearLength())), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_PERIOD, (object) GameUtil.GetFormattedCycles(this.configuration.GetYearOnDuration()), (object) GameUtil.GetFormattedCycles(this.configuration.GetYearLength())), (Descriptor.DescriptorType) 1, false));
      if (this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.dormant))
        descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.GEYSER_YEAR_NEXT_ACTIVE, (object) GameUtil.GetFormattedCycles(this.RemainingDormantTime())), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_NEXT_ACTIVE, (object) GameUtil.GetFormattedCycles(this.RemainingDormantTime())), (Descriptor.DescriptorType) 1, false));
      else
        descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.GEYSER_YEAR_NEXT_DORMANT, (object) GameUtil.GetFormattedCycles(this.RemainingActiveTime())), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_NEXT_DORMANT, (object) GameUtil.GetFormattedCycles(this.RemainingActiveTime())), (Descriptor.DescriptorType) 1, false));
      string str8 = UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT.Replace("{average}", GameUtil.GetFormattedMass(this.configuration.GetAverageEmission(), GameUtil.TimeSlice.PerSecond)).Replace("{element}", this.configuration.geyserType.element.CreateTag().ProperName());
      if (flag)
      {
        string str9 = str8 + "\n" + "\n" + (string) UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT_BREAKDOWN_TITLE;
        int affectingThisGeyser = this.GetAmountOfGeotunersAffectingThisGeyser();
        float num4 = Geyser.massModificationMethod == Geyser.ModificationMethod.Percentages ? instance.currentGeyserModification.massPerCycleModifier * 100f : instance.currentGeyserModification.massPerCycleModifier * 100f / this.configuration.scaledRate;
        float num5 = num4 * (float) affectingThisGeyser;
        str8 = str9 + GameUtil.AddPositiveSign(num5.ToString("0.0"), (double) num5 > 0.0) + "%";
        for (int index = 0; index < affectingThisGeyser; ++index)
        {
          string str10 = "\n    • " + UI.BUILDINGEFFECTS.TOOLTIPS.GEYSER_YEAR_AVR_OUTPUT_BREAKDOWN_ROW.ToString() + GameUtil.AddPositiveSign(num4.ToString("0.0"), (double) num4 > 0.0) + "%";
          str8 += str10;
        }
      }
      descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.GEYSER_YEAR_AVR_OUTPUT, (object) GameUtil.GetFormattedMass(this.configuration.GetAverageEmission(), GameUtil.TimeSlice.PerSecond)), str8, (Descriptor.DescriptorType) 1, false));
    }
    return descriptors;
  }

  public enum ModificationMethod
  {
    Values,
    Percentages,
  }

  public struct GeyserModification
  {
    public string originID;
    public float massPerCycleModifier;
    public float temperatureModifier;
    public float iterationDurationModifier;
    public float iterationPercentageModifier;
    public float yearDurationModifier;
    public float yearPercentageModifier;
    public float maxPressureModifier;
    public bool modifyElement;
    public SimHashes newElement;

    public void Clear()
    {
      this.massPerCycleModifier = 0.0f;
      this.temperatureModifier = 0.0f;
      this.iterationDurationModifier = 0.0f;
      this.iterationPercentageModifier = 0.0f;
      this.yearDurationModifier = 0.0f;
      this.yearPercentageModifier = 0.0f;
      this.maxPressureModifier = 0.0f;
      this.modifyElement = false;
      this.newElement = (SimHashes) 0;
    }

    public void AddValues(Geyser.GeyserModification modification)
    {
      this.massPerCycleModifier += modification.massPerCycleModifier;
      this.temperatureModifier += modification.temperatureModifier;
      this.iterationDurationModifier += modification.iterationDurationModifier;
      this.iterationPercentageModifier += modification.iterationPercentageModifier;
      this.yearDurationModifier += modification.yearDurationModifier;
      this.yearPercentageModifier += modification.yearPercentageModifier;
      this.maxPressureModifier += modification.maxPressureModifier;
      this.modifyElement |= modification.modifyElement;
      this.newElement = modification.newElement == (SimHashes) 0 ? this.newElement : modification.newElement;
    }

    public bool IsNewElementInUse() => this.modifyElement && this.newElement != 0;
  }

  public class StatesInstance : 
    GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.GameInstance
  {
    public StatesInstance(Geyser smi)
      : base(smi)
    {
    }
  }

  public class States : GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser>
  {
    public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State dormant;
    public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State idle;
    public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State pre_erupt;
    public Geyser.States.EruptState erupt;
    public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State post_erupt;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.root.DefaultState(this.idle).Enter((StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State.Callback) (smi => smi.master.emitter.SetEmitting(false)));
      this.dormant.PlayAnim("inactive", (KAnim.PlayMode) 0).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutDormant).ScheduleGoTo((Func<Geyser.StatesInstance, float>) (smi => smi.master.RemainingDormantTime()), (StateMachine.BaseState) this.pre_erupt);
      this.idle.PlayAnim("inactive", (KAnim.PlayMode) 0).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutIdle).Enter((StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State.Callback) (smi =>
      {
        if (!smi.master.ShouldGoDormant())
          return;
        smi.GoTo((StateMachine.BaseState) this.dormant);
      })).ScheduleGoTo((Func<Geyser.StatesInstance, float>) (smi => smi.master.RemainingIdleTime()), (StateMachine.BaseState) this.pre_erupt);
      this.pre_erupt.PlayAnim("shake", (KAnim.PlayMode) 0).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutPressureBuilding).ScheduleGoTo((Func<Geyser.StatesInstance, float>) (smi => smi.master.RemainingEruptPreTime()), (StateMachine.BaseState) this.erupt);
      this.erupt.TriggerOnEnter(GameHashes.GeyserEruption, (Func<Geyser.StatesInstance, object>) (smi => (object) true)).TriggerOnExit(GameHashes.GeyserEruption, (Func<Geyser.StatesInstance, object>) (smi => (object) false)).DefaultState(this.erupt.erupting).ScheduleGoTo((Func<Geyser.StatesInstance, float>) (smi => smi.master.RemainingEruptTime()), (StateMachine.BaseState) this.post_erupt).Enter((StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State.Callback) (smi => smi.master.emitter.SetEmitting(true))).Exit((StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State.Callback) (smi => smi.master.emitter.SetEmitting(false)));
      this.erupt.erupting.EventTransition(GameHashes.EmitterBlocked, this.erupt.overpressure, (StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.Transition.ConditionCallback) (smi => smi.GetComponent<ElementEmitter>().isEmitterBlocked)).PlayAnim("erupt", (KAnim.PlayMode) 0);
      this.erupt.overpressure.EventTransition(GameHashes.EmitterUnblocked, this.erupt.erupting, (StateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<ElementEmitter>().isEmitterBlocked)).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutOverPressure).PlayAnim("inactive", (KAnim.PlayMode) 0);
      this.post_erupt.PlayAnim("shake", (KAnim.PlayMode) 0).ToggleMainStatusItem(Db.Get().MiscStatusItems.SpoutIdle).ScheduleGoTo((Func<Geyser.StatesInstance, float>) (smi => smi.master.RemainingEruptPostTime()), (StateMachine.BaseState) this.idle);
    }

    public class EruptState : 
      GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State
    {
      public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State erupting;
      public GameStateMachine<Geyser.States, Geyser.StatesInstance, Geyser, object>.State overpressure;
    }
  }

  public enum TimeShiftStep
  {
    ActiveState,
    DormantState,
    NextIteration,
    PreviousIteration,
  }

  public enum Phase
  {
    Pre,
    On,
    Pst,
    Off,
    Any,
  }
}
