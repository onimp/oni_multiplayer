// Decompiled with JetBrains decompiler
// Type: GameUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class GameUtil
{
  public static GameUtil.TemperatureUnit temperatureUnit;
  public static GameUtil.MassUnit massUnit;
  private static string[] adjectives;
  [ThreadStatic]
  public static Queue<GameUtil.FloodFillInfo> FloodFillNext = new Queue<GameUtil.FloodFillInfo>();
  [ThreadStatic]
  public static HashSet<int> FloodFillVisited = new HashSet<int>();
  public static TagSet foodTags;
  public static TagSet solidTags;

  public static string GetTemperatureUnitSuffix()
  {
    string temperatureUnitSuffix;
    switch (GameUtil.temperatureUnit)
    {
      case GameUtil.TemperatureUnit.Celsius:
        temperatureUnitSuffix = (string) UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
        break;
      case GameUtil.TemperatureUnit.Fahrenheit:
        temperatureUnitSuffix = (string) UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
        break;
      default:
        temperatureUnitSuffix = (string) UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
        break;
    }
    return temperatureUnitSuffix;
  }

  private static string AddTemperatureUnitSuffix(string text) => text + GameUtil.GetTemperatureUnitSuffix();

  public static float GetTemperatureConvertedFromKelvin(
    float temperature,
    GameUtil.TemperatureUnit targetUnit)
  {
    if (targetUnit == GameUtil.TemperatureUnit.Celsius)
      return temperature - 273.15f;
    return targetUnit == GameUtil.TemperatureUnit.Fahrenheit ? (float) ((double) temperature * 1.7999999523162842 - 459.67001342773438) : temperature;
  }

  public static float GetConvertedTemperature(float temperature, bool roundOutput = false)
  {
    switch (GameUtil.temperatureUnit)
    {
      case GameUtil.TemperatureUnit.Celsius:
        float num1 = temperature - 273.15f;
        return !roundOutput ? num1 : Mathf.Round(num1);
      case GameUtil.TemperatureUnit.Fahrenheit:
        float num2 = (float) ((double) temperature * 1.7999999523162842 - 459.67001342773438);
        return !roundOutput ? num2 : Mathf.Round(num2);
      default:
        return !roundOutput ? temperature : Mathf.Round(temperature);
    }
  }

  public static float GetTemperatureConvertedToKelvin(
    float temperature,
    GameUtil.TemperatureUnit fromUnit)
  {
    if (fromUnit == GameUtil.TemperatureUnit.Celsius)
      return temperature + 273.15f;
    return fromUnit == GameUtil.TemperatureUnit.Fahrenheit ? (float) (((double) temperature + 459.67001342773438) * 5.0 / 9.0) : temperature;
  }

  public static float GetTemperatureConvertedToKelvin(float temperature)
  {
    switch (GameUtil.temperatureUnit)
    {
      case GameUtil.TemperatureUnit.Celsius:
        return temperature + 273.15f;
      case GameUtil.TemperatureUnit.Fahrenheit:
        return (float) (((double) temperature + 459.67001342773438) * 5.0 / 9.0);
      default:
        return temperature;
    }
  }

  private static float GetConvertedTemperatureDelta(float kelvin_delta)
  {
    switch (GameUtil.temperatureUnit)
    {
      case GameUtil.TemperatureUnit.Celsius:
        return kelvin_delta;
      case GameUtil.TemperatureUnit.Fahrenheit:
        return kelvin_delta * 1.8f;
      case GameUtil.TemperatureUnit.Kelvin:
        return kelvin_delta;
      default:
        return kelvin_delta;
    }
  }

  public static float ApplyTimeSlice(float val, GameUtil.TimeSlice timeSlice) => timeSlice == GameUtil.TimeSlice.PerCycle ? val * 600f : val;

  public static float ApplyTimeSlice(int val, GameUtil.TimeSlice timeSlice) => timeSlice == GameUtil.TimeSlice.PerCycle ? (float) val * 600f : (float) val;

  public static string AddTimeSliceText(string text, GameUtil.TimeSlice timeSlice)
  {
    switch (timeSlice)
    {
      case GameUtil.TimeSlice.PerSecond:
        return text + (string) UI.UNITSUFFIXES.PERSECOND;
      case GameUtil.TimeSlice.PerCycle:
        return text + (string) UI.UNITSUFFIXES.PERCYCLE;
      default:
        return text;
    }
  }

  public static string AddPositiveSign(string text, bool positive) => positive ? string.Format((string) UI.POSITIVE_FORMAT, (object) text) : text;

  public static float AttributeSkillToAlpha(AttributeInstance attributeInstance) => Mathf.Min(attributeInstance.GetTotalValue() / 10f, 1f);

  public static float AttributeSkillToAlpha(float attributeSkill) => Mathf.Min(attributeSkill / 10f, 1f);

  public static float AptitudeToAlpha(float aptitude) => Mathf.Min(aptitude / 10f, 1f);

  public static float GetThermalEnergy(PrimaryElement pe) => pe.Temperature * pe.Mass * pe.Element.specificHeatCapacity;

  public static float CalculateTemperatureChange(float shc, float mass, float kilowatts) => kilowatts / (shc * mass);

  public static void DeltaThermalEnergy(
    PrimaryElement pe,
    float kilowatts,
    float targetTemperature)
  {
    float temperatureChange = GameUtil.CalculateTemperatureChange(pe.Element.specificHeatCapacity, pe.Mass, kilowatts);
    float num1 = pe.Temperature + temperatureChange;
    float num2 = (double) targetTemperature <= (double) pe.Temperature ? Mathf.Clamp(num1, targetTemperature, pe.Temperature) : Mathf.Clamp(num1, pe.Temperature, targetTemperature);
    pe.Temperature = num2;
  }

  public static BindingEntry ActionToBinding(Action action)
  {
    foreach (BindingEntry keyBinding in GameInputMapping.KeyBindings)
    {
      if (keyBinding.mAction == action)
        return keyBinding;
    }
    throw new ArgumentException(action.ToString() + " is not bound in GameInputBindings");
  }

  public static string GetIdentityDescriptor(GameObject go, GameUtil.IdentityDescriptorTense tense = GameUtil.IdentityDescriptorTense.Normal)
  {
    if (Object.op_Implicit((Object) go.GetComponent<MinionIdentity>()))
    {
      switch (tense)
      {
        case GameUtil.IdentityDescriptorTense.Normal:
          return (string) DUPLICANTS.STATS.SUBJECTS.DUPLICANT;
        case GameUtil.IdentityDescriptorTense.Possessive:
          return (string) DUPLICANTS.STATS.SUBJECTS.DUPLICANT_POSSESSIVE;
        case GameUtil.IdentityDescriptorTense.Plural:
          return (string) DUPLICANTS.STATS.SUBJECTS.DUPLICANT_PLURAL;
      }
    }
    else if (Object.op_Implicit((Object) go.GetComponent<CreatureBrain>()))
    {
      switch (tense)
      {
        case GameUtil.IdentityDescriptorTense.Normal:
          return (string) DUPLICANTS.STATS.SUBJECTS.CREATURE;
        case GameUtil.IdentityDescriptorTense.Possessive:
          return (string) DUPLICANTS.STATS.SUBJECTS.CREATURE_POSSESSIVE;
        case GameUtil.IdentityDescriptorTense.Plural:
          return (string) DUPLICANTS.STATS.SUBJECTS.CREATURE_PLURAL;
      }
    }
    else
    {
      switch (tense)
      {
        case GameUtil.IdentityDescriptorTense.Normal:
          return (string) DUPLICANTS.STATS.SUBJECTS.PLANT;
        case GameUtil.IdentityDescriptorTense.Possessive:
          return (string) DUPLICANTS.STATS.SUBJECTS.PLANT_POSESSIVE;
        case GameUtil.IdentityDescriptorTense.Plural:
          return (string) DUPLICANTS.STATS.SUBJECTS.PLANT_PLURAL;
      }
    }
    return "";
  }

  public static float GetEnergyInPrimaryElement(PrimaryElement element) => (float) (1.0 / 1000.0 * ((double) element.Temperature * ((double) element.Mass * 1000.0 * (double) element.Element.specificHeatCapacity)));

  public static float EnergyToTemperatureDelta(float kilojoules, PrimaryElement element)
  {
    Debug.Assert((double) element.Mass > 0.0);
    double num1 = (double) Mathf.Max(GameUtil.GetEnergyInPrimaryElement(element) - kilojoules, 1f);
    float temperature = element.Temperature;
    double num2 = 1.0 / 1000.0 * ((double) element.Mass * ((double) element.Element.specificHeatCapacity * 1000.0));
    return (float) (num1 / num2) - temperature;
  }

  public static float CalculateEnergyDeltaForElement(
    PrimaryElement element,
    float startTemp,
    float endTemp)
  {
    return GameUtil.CalculateEnergyDeltaForElementChange(element.Mass, element.Element.specificHeatCapacity, startTemp, endTemp);
  }

  public static float CalculateEnergyDeltaForElementChange(
    float mass,
    float shc,
    float startTemp,
    float endTemp)
  {
    return (endTemp - startTemp) * mass * shc;
  }

  public static float GetFinalTemperature(float t1, float m1, float t2, float m2)
  {
    float num1 = m1 + m2;
    float num2 = (float) ((double) t1 * (double) m1 + (double) t2 * (double) m2) / num1;
    float num3 = Mathf.Min(t1, t2);
    float num4 = Mathf.Max(t1, t2);
    float f = Mathf.Clamp(num2, num3, num4);
    if (float.IsNaN(f) || float.IsInfinity(f))
      Debug.LogError((object) string.Format("Calculated an invalid temperature: t1={0}, m1={1}, t2={2}, m2={3}, min_temp={4}, max_temp={5}", (object) t1, (object) m1, (object) t2, (object) m2, (object) num3, (object) num4));
    return f;
  }

  public static void ForceConduction(PrimaryElement a, PrimaryElement b, float dt)
  {
    float num1 = a.Temperature * a.Element.specificHeatCapacity * a.Mass;
    float num2 = b.Temperature * b.Element.specificHeatCapacity * b.Mass;
    float num3 = Math.Min(a.Element.thermalConductivity, b.Element.thermalConductivity);
    float num4 = Math.Min(a.Mass, b.Mass);
    float val1 = (float) (((double) b.Temperature - (double) a.Temperature) * ((double) num3 * (double) num4)) * dt;
    double num5 = ((double) num1 + (double) num2) / ((double) a.Element.specificHeatCapacity * (double) a.Mass + (double) b.Element.specificHeatCapacity * (double) b.Mass);
    float val2 = Math.Min(Math.Abs(((float) num5 - a.Temperature) * a.Element.specificHeatCapacity * a.Mass), Math.Abs(((float) num5 - b.Temperature) * b.Element.specificHeatCapacity * b.Mass));
    float num6 = Math.Max(Math.Min(val1, val2), -val2);
    a.Temperature = (num1 + num6) / a.Element.specificHeatCapacity / a.Mass;
    b.Temperature = (num2 - num6) / b.Element.specificHeatCapacity / b.Mass;
  }

  public static string FloatToString(float f, string format = null)
  {
    if (float.IsPositiveInfinity(f))
      return (string) UI.POS_INFINITY;
    return float.IsNegativeInfinity(f) ? (string) UI.NEG_INFINITY : f.ToString(format);
  }

  public static string GetFloatWithDecimalPoint(float f)
  {
    string format = (double) f != 0.0 ? ((double) Mathf.Abs(f) >= 1.0 ? "#,###.#" : "#,##0.#") : "0";
    return GameUtil.FloatToString(f, format);
  }

  public static string GetStandardFloat(float f)
  {
    string format = (double) f != 0.0 ? ((double) Mathf.Abs(f) >= 1.0 ? ((double) Mathf.Abs(f) >= 10.0 ? "#,###" : "#,###.#") : "#,##0.#") : "0";
    return GameUtil.FloatToString(f, format);
  }

  public static string GetStandardPercentageFloat(float f, bool allowHundredths = false)
  {
    string format = (double) Mathf.Abs(f) != 0.0 ? (!((double) Mathf.Abs(f) < 0.10000000149011612 & allowHundredths) ? ((double) Mathf.Abs(f) >= 1.0 ? "##0" : "##0.#") : "##0.##") : "0";
    return GameUtil.FloatToString(f, format);
  }

  public static string GetUnitFormattedName(GameObject go, bool upperName = false)
  {
    KPrefabID component1 = go.GetComponent<KPrefabID>();
    if (Object.op_Inequality((Object) component1, (Object) null) && Assets.IsTagCountable(component1.PrefabTag))
    {
      PrimaryElement component2 = go.GetComponent<PrimaryElement>();
      return GameUtil.GetUnitFormattedName(go.GetProperName(), component2.Units, upperName);
    }
    return !upperName ? go.GetProperName() : StringFormatter.ToUpper(go.GetProperName());
  }

  public static string GetUnitFormattedName(string name, float count, bool upperName = false)
  {
    if (upperName)
      name = name.ToUpper();
    return StringFormatter.Replace((string) UI.NAME_WITH_UNITS, "{0}", name).Replace("{1}", string.Format("{0:0.##}", (object) count));
  }

  public static string GetFormattedUnits(
    float units,
    GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None,
    bool displaySuffix = true,
    string floatFormatOverride = "")
  {
    string str = (string) ((double) units == 1.0 ? UI.UNITSUFFIXES.UNIT : UI.UNITSUFFIXES.UNITS);
    units = GameUtil.ApplyTimeSlice(units, timeSlice);
    string text = GameUtil.GetStandardFloat(units);
    if (!Util.IsNullOrWhiteSpace(floatFormatOverride))
      text = string.Format(floatFormatOverride, (object) units);
    if (displaySuffix)
      text += str;
    return GameUtil.AddTimeSliceText(text, timeSlice);
  }

  public static string GetFormattedRocketRange(
    float range,
    GameUtil.TimeSlice timeSlice,
    bool displaySuffix = true)
  {
    if (timeSlice == GameUtil.TimeSlice.PerCycle)
      return range.ToString("N1") + (displaySuffix ? " " + (string) UI.CLUSTERMAP.TILES_PER_CYCLE : "");
    float num = range / 600f;
    if (Mathf.Approximately(num, Mathf.Round(num)))
      num = Mathf.Round(num);
    return Mathf.Floor(num).ToString() + (displaySuffix ? " " + (string) UI.CLUSTERMAP.TILES : "");
  }

  public static string ApplyBoldString(string source) => "<b>" + source + "</b>";

  public static float GetRoundedTemperatureInKelvin(float kelvin)
  {
    float temperatureInKelvin = 0.0f;
    switch (GameUtil.temperatureUnit)
    {
      case GameUtil.TemperatureUnit.Celsius:
        temperatureInKelvin = GameUtil.GetTemperatureConvertedToKelvin(Mathf.Round(GameUtil.GetConvertedTemperature(Mathf.Round(kelvin), true)));
        break;
      case GameUtil.TemperatureUnit.Fahrenheit:
        temperatureInKelvin = GameUtil.GetTemperatureConvertedToKelvin((float) Mathf.RoundToInt(GameUtil.GetTemperatureConvertedFromKelvin(kelvin, GameUtil.TemperatureUnit.Fahrenheit)), GameUtil.TemperatureUnit.Fahrenheit);
        break;
      case GameUtil.TemperatureUnit.Kelvin:
        temperatureInKelvin = (float) Mathf.RoundToInt(kelvin);
        break;
    }
    return temperatureInKelvin;
  }

  public static string GetFormattedTemperature(
    float temp,
    GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None,
    GameUtil.TemperatureInterpretation interpretation = GameUtil.TemperatureInterpretation.Absolute,
    bool displayUnits = true,
    bool roundInDestinationFormat = false)
  {
    if (interpretation != GameUtil.TemperatureInterpretation.Absolute)
    {
      if (interpretation == GameUtil.TemperatureInterpretation.Relative)
        ;
      temp = GameUtil.GetConvertedTemperatureDelta(temp);
    }
    else
      temp = GameUtil.GetConvertedTemperature(temp, roundInDestinationFormat);
    temp = GameUtil.ApplyTimeSlice(temp, timeSlice);
    string text = (double) Mathf.Abs(temp) >= 0.10000000149011612 ? GameUtil.FloatToString(temp, "##0.#") : GameUtil.FloatToString(temp, "##0.####");
    if (displayUnits)
      text = GameUtil.AddTemperatureUnitSuffix(text);
    return GameUtil.AddTimeSliceText(text, timeSlice);
  }

  public static string GetFormattedCaloriesForItem(
    Tag tag,
    float amount,
    GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None,
    bool forceKcal = true)
  {
    EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(((Tag) ref tag).Name);
    return GameUtil.GetFormattedCalories(foodInfo != null ? foodInfo.CaloriesPerUnit * amount : -1f, timeSlice, forceKcal);
  }

  public static string GetFormattedCalories(
    float calories,
    GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None,
    bool forceKcal = true)
  {
    string str = (string) UI.UNITSUFFIXES.CALORIES.CALORIE;
    if ((double) Mathf.Abs(calories) >= 1000.0 | forceKcal)
    {
      calories /= 1000f;
      str = (string) UI.UNITSUFFIXES.CALORIES.KILOCALORIE;
    }
    calories = GameUtil.ApplyTimeSlice(calories, timeSlice);
    return GameUtil.AddTimeSliceText(GameUtil.GetStandardFloat(calories) + str, timeSlice);
  }

  public static string GetFormattedPlantGrowth(float percent, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
  {
    percent = GameUtil.ApplyTimeSlice(percent, timeSlice);
    return GameUtil.AddTimeSliceText(GameUtil.GetStandardPercentageFloat(percent, true) + (string) UI.UNITSUFFIXES.PERCENT + " " + (string) UI.UNITSUFFIXES.GROWTH, timeSlice);
  }

  public static string GetFormattedPercent(float percent, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
  {
    percent = GameUtil.ApplyTimeSlice(percent, timeSlice);
    return GameUtil.AddTimeSliceText(GameUtil.GetStandardPercentageFloat(percent, true) + (string) UI.UNITSUFFIXES.PERCENT, timeSlice);
  }

  public static string GetFormattedRoundedJoules(float joules) => (double) Mathf.Abs(joules) > 1000.0 ? GameUtil.FloatToString(joules / 1000f, "F1") + (string) UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE : GameUtil.FloatToString(joules, "F1") + (string) UI.UNITSUFFIXES.ELECTRICAL.JOULE;

  public static string GetFormattedJoules(
    float joules,
    string floatFormat = "F1",
    GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
  {
    if (timeSlice == GameUtil.TimeSlice.PerSecond)
      return GameUtil.GetFormattedWattage(joules);
    joules = GameUtil.ApplyTimeSlice(joules, timeSlice);
    return GameUtil.AddTimeSliceText((double) Math.Abs(joules) <= 1000000.0 ? ((double) Mathf.Abs(joules) <= 1000.0 ? GameUtil.FloatToString(joules, floatFormat) + (string) UI.UNITSUFFIXES.ELECTRICAL.JOULE : GameUtil.FloatToString(joules / 1000f, floatFormat) + (string) UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE) : GameUtil.FloatToString(joules / 1000000f, floatFormat) + (string) UI.UNITSUFFIXES.ELECTRICAL.MEGAJOULE, timeSlice);
  }

  public static string GetFormattedRads(float rads, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
  {
    rads = GameUtil.ApplyTimeSlice(rads, timeSlice);
    return GameUtil.AddTimeSliceText(GameUtil.GetStandardFloat(rads) + (string) UI.UNITSUFFIXES.RADIATION.RADS, timeSlice);
  }

  public static string GetFormattedHighEnergyParticles(
    float units,
    GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None,
    bool displayUnits = true)
  {
    string str = (string) ((double) units == 1.0 ? UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLE : UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES);
    units = GameUtil.ApplyTimeSlice(units, timeSlice);
    return GameUtil.AddTimeSliceText(displayUnits ? GameUtil.GetFloatWithDecimalPoint(units) + str : GameUtil.GetFloatWithDecimalPoint(units), timeSlice);
  }

  public static string GetFormattedWattage(
    float watts,
    GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Automatic,
    bool displayUnits = true)
  {
    LocString locString = (LocString) "";
    switch (unit)
    {
      case GameUtil.WattageFormatterUnit.Watts:
        locString = UI.UNITSUFFIXES.ELECTRICAL.WATT;
        break;
      case GameUtil.WattageFormatterUnit.Kilowatts:
        watts /= 1000f;
        locString = UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
        break;
      case GameUtil.WattageFormatterUnit.Automatic:
        if ((double) Mathf.Abs(watts) > 1000.0)
        {
          watts /= 1000f;
          locString = UI.UNITSUFFIXES.ELECTRICAL.KILOWATT;
          break;
        }
        locString = UI.UNITSUFFIXES.ELECTRICAL.WATT;
        break;
    }
    return displayUnits ? GameUtil.FloatToString(watts, "###0.##") + (string) locString : GameUtil.FloatToString(watts, "###0.##");
  }

  public static string GetFormattedHeatEnergy(float dtu, GameUtil.HeatEnergyFormatterUnit unit = GameUtil.HeatEnergyFormatterUnit.Automatic)
  {
    LocString locString1 = (LocString) "";
    LocString locString2;
    string format;
    switch (unit)
    {
      case GameUtil.HeatEnergyFormatterUnit.DTU_S:
        locString2 = UI.UNITSUFFIXES.HEAT.DTU;
        format = "###0.";
        break;
      case GameUtil.HeatEnergyFormatterUnit.KDTU_S:
        dtu /= 1000f;
        locString2 = UI.UNITSUFFIXES.HEAT.KDTU;
        format = "###0.##";
        break;
      default:
        if ((double) Mathf.Abs(dtu) > 1000.0)
        {
          dtu /= 1000f;
          locString2 = UI.UNITSUFFIXES.HEAT.KDTU;
          format = "###0.##";
          break;
        }
        locString2 = UI.UNITSUFFIXES.HEAT.DTU;
        format = "###0.";
        break;
    }
    return GameUtil.FloatToString(dtu, format) + (string) locString2;
  }

  public static string GetFormattedHeatEnergyRate(
    float dtu_s,
    GameUtil.HeatEnergyFormatterUnit unit = GameUtil.HeatEnergyFormatterUnit.Automatic)
  {
    LocString locString = (LocString) "";
    switch (unit)
    {
      case GameUtil.HeatEnergyFormatterUnit.DTU_S:
        locString = UI.UNITSUFFIXES.HEAT.DTU_S;
        break;
      case GameUtil.HeatEnergyFormatterUnit.KDTU_S:
        dtu_s /= 1000f;
        locString = UI.UNITSUFFIXES.HEAT.KDTU_S;
        break;
      case GameUtil.HeatEnergyFormatterUnit.Automatic:
        if ((double) Mathf.Abs(dtu_s) > 1000.0)
        {
          dtu_s /= 1000f;
          locString = UI.UNITSUFFIXES.HEAT.KDTU_S;
          break;
        }
        locString = UI.UNITSUFFIXES.HEAT.DTU_S;
        break;
    }
    return GameUtil.FloatToString(dtu_s, "###0.##") + (string) locString;
  }

  public static string GetFormattedInt(float num, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
  {
    num = GameUtil.ApplyTimeSlice(num, timeSlice);
    return GameUtil.AddTimeSliceText(GameUtil.FloatToString(num, "F0"), timeSlice);
  }

  public static string GetFormattedSimple(
    float num,
    GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None,
    string formatString = null)
  {
    num = GameUtil.ApplyTimeSlice(num, timeSlice);
    return GameUtil.AddTimeSliceText(formatString == null ? ((double) num != 0.0 ? ((double) Mathf.Abs(num) >= 1.0 ? ((double) Mathf.Abs(num) >= 10.0 ? GameUtil.FloatToString(num, "#,###.##") : GameUtil.FloatToString(num, "#,###.##")) : GameUtil.FloatToString(num, "#,##0.##")) : "0") : GameUtil.FloatToString(num, formatString), timeSlice);
  }

  public static string GetFormattedLux(int lux) => lux.ToString() + (string) UI.UNITSUFFIXES.LIGHT.LUX;

  public static string GetLightDescription(int lux)
  {
    if (lux == 0)
      return (string) UI.OVERLAYS.LIGHTING.RANGES.NO_LIGHT;
    if (lux < 100)
      return (string) UI.OVERLAYS.LIGHTING.RANGES.VERY_LOW_LIGHT;
    if (lux < 1000)
      return (string) UI.OVERLAYS.LIGHTING.RANGES.LOW_LIGHT;
    if (lux < 10000)
      return (string) UI.OVERLAYS.LIGHTING.RANGES.MEDIUM_LIGHT;
    if (lux < 50000)
      return (string) UI.OVERLAYS.LIGHTING.RANGES.HIGH_LIGHT;
    return lux < 100000 ? (string) UI.OVERLAYS.LIGHTING.RANGES.VERY_HIGH_LIGHT : (string) UI.OVERLAYS.LIGHTING.RANGES.MAX_LIGHT;
  }

  public static string GetRadiationDescription(float radsPerCycle)
  {
    if ((double) radsPerCycle == 0.0)
      return (string) UI.OVERLAYS.RADIATION.RANGES.NONE;
    if ((double) radsPerCycle < 100.0)
      return (string) UI.OVERLAYS.RADIATION.RANGES.VERY_LOW;
    if ((double) radsPerCycle < 200.0)
      return (string) UI.OVERLAYS.RADIATION.RANGES.LOW;
    if ((double) radsPerCycle < 400.0)
      return (string) UI.OVERLAYS.RADIATION.RANGES.MEDIUM;
    if ((double) radsPerCycle < 2000.0)
      return (string) UI.OVERLAYS.RADIATION.RANGES.HIGH;
    return (double) radsPerCycle < 4000.0 ? (string) UI.OVERLAYS.RADIATION.RANGES.VERY_HIGH : (string) UI.OVERLAYS.RADIATION.RANGES.MAX;
  }

  public static string GetFormattedByTag(Tag tag, float amount, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
  {
    if (GameTags.DisplayAsCalories.Contains(tag))
      return GameUtil.GetFormattedCaloriesForItem(tag, amount, timeSlice);
    return GameTags.DisplayAsUnits.Contains(tag) ? GameUtil.GetFormattedUnits(amount, timeSlice) : GameUtil.GetFormattedMass(amount, timeSlice);
  }

  public static string GetFormattedFoodQuality(int quality)
  {
    if (GameUtil.adjectives == null)
      GameUtil.adjectives = LocString.GetStrings(typeof (DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVES));
    LocString format = quality >= 0 ? DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_FORMAT_POSITIVE : DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_FORMAT_NEGATIVE;
    int index = Mathf.Clamp(quality - DUPLICANTS.NEEDS.FOOD_QUALITY.ADJECTIVE_INDEX_OFFSET, 0, GameUtil.adjectives.Length);
    return string.Format((string) format, (object) GameUtil.adjectives[index], (object) GameUtil.AddPositiveSign(quality.ToString(), quality > 0));
  }

  public static string GetFormattedBytes(ulong amount)
  {
    string[] strArray = new string[5]
    {
      (string) UI.UNITSUFFIXES.INFORMATION.BYTE,
      (string) UI.UNITSUFFIXES.INFORMATION.KILOBYTE,
      (string) UI.UNITSUFFIXES.INFORMATION.MEGABYTE,
      (string) UI.UNITSUFFIXES.INFORMATION.GIGABYTE,
      (string) UI.UNITSUFFIXES.INFORMATION.TERABYTE
    };
    int y = amount == 0UL ? 0 : (int) Math.Floor(Math.Floor(Math.Log((double) amount)) / Math.Log(1024.0));
    double num = (double) amount / Math.Pow(1024.0, (double) y);
    Debug.Assert(y >= 0 && y < strArray.Length);
    return string.Format("{0:F} {1}", (object) num, (object) strArray[y]);
  }

  public static string GetFormattedInfomation(float amount, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
  {
    amount = GameUtil.ApplyTimeSlice(amount, timeSlice);
    string str = "";
    if ((double) amount < 1024.0)
      str = (string) UI.UNITSUFFIXES.INFORMATION.KILOBYTE;
    else if ((double) amount < 1048576.0)
    {
      amount /= 1000f;
      str = (string) UI.UNITSUFFIXES.INFORMATION.MEGABYTE;
    }
    else if ((double) amount < 1073741824.0)
    {
      amount /= 1048576f;
      str = (string) UI.UNITSUFFIXES.INFORMATION.GIGABYTE;
    }
    return GameUtil.AddTimeSliceText(amount.ToString() + str, timeSlice);
  }

  public static LocString GetCurrentMassUnit(bool useSmallUnit = false)
  {
    LocString currentMassUnit = (LocString) null;
    switch (GameUtil.massUnit)
    {
      case GameUtil.MassUnit.Kilograms:
        currentMassUnit = !useSmallUnit ? UI.UNITSUFFIXES.MASS.KILOGRAM : UI.UNITSUFFIXES.MASS.GRAM;
        break;
      case GameUtil.MassUnit.Pounds:
        currentMassUnit = UI.UNITSUFFIXES.MASS.POUND;
        break;
    }
    return currentMassUnit;
  }

  public static string GetFormattedMass(
    float mass,
    GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None,
    GameUtil.MetricMassFormat massFormat = GameUtil.MetricMassFormat.UseThreshold,
    bool includeSuffix = true,
    string floatFormat = "{0:0.#}")
  {
    if ((double) mass == -3.4028234663852886E+38)
      return (string) UI.CALCULATING;
    mass = GameUtil.ApplyTimeSlice(mass, timeSlice);
    string str;
    if (GameUtil.massUnit == GameUtil.MassUnit.Kilograms)
    {
      str = (string) UI.UNITSUFFIXES.MASS.TONNE;
      switch (massFormat)
      {
        case GameUtil.MetricMassFormat.UseThreshold:
          float num = Mathf.Abs(mass);
          if (0.0 < (double) num)
          {
            if ((double) num < 4.9999998736893758E-06)
            {
              str = (string) UI.UNITSUFFIXES.MASS.MICROGRAM;
              mass = Mathf.Floor(mass * 1E+09f);
              break;
            }
            if ((double) num < 0.004999999888241291)
            {
              mass *= 1000000f;
              str = (string) UI.UNITSUFFIXES.MASS.MILLIGRAM;
              break;
            }
            if ((double) Mathf.Abs(mass) < 5.0)
            {
              mass *= 1000f;
              str = (string) UI.UNITSUFFIXES.MASS.GRAM;
              break;
            }
            if ((double) Mathf.Abs(mass) < 5000.0)
            {
              str = (string) UI.UNITSUFFIXES.MASS.KILOGRAM;
              break;
            }
            mass /= 1000f;
            str = (string) UI.UNITSUFFIXES.MASS.TONNE;
            break;
          }
          str = (string) UI.UNITSUFFIXES.MASS.KILOGRAM;
          break;
        case GameUtil.MetricMassFormat.Kilogram:
          str = (string) UI.UNITSUFFIXES.MASS.KILOGRAM;
          break;
        case GameUtil.MetricMassFormat.Gram:
          mass *= 1000f;
          str = (string) UI.UNITSUFFIXES.MASS.GRAM;
          break;
        case GameUtil.MetricMassFormat.Tonne:
          mass /= 1000f;
          str = (string) UI.UNITSUFFIXES.MASS.TONNE;
          break;
      }
    }
    else
    {
      mass /= 2.2f;
      str = (string) UI.UNITSUFFIXES.MASS.POUND;
      if (massFormat == GameUtil.MetricMassFormat.UseThreshold)
      {
        float num = Mathf.Abs(mass);
        if ((double) num < 5.0 && (double) num > 1.0 / 1000.0)
        {
          mass *= 256f;
          str = (string) UI.UNITSUFFIXES.MASS.DRACHMA;
        }
        else
        {
          mass *= 7000f;
          str = (string) UI.UNITSUFFIXES.MASS.GRAIN;
        }
      }
    }
    if (!includeSuffix)
    {
      str = "";
      timeSlice = GameUtil.TimeSlice.None;
    }
    return GameUtil.AddTimeSliceText(string.Format(floatFormat, (object) mass) + str, timeSlice);
  }

  public static string GetFormattedTime(float seconds, string floatFormat = "F0") => string.Format((string) UI.FORMATSECONDS, (object) seconds.ToString(floatFormat));

  public static string GetFormattedEngineEfficiency(float amount) => amount.ToString() + " km /" + (string) UI.UNITSUFFIXES.MASS.KILOGRAM;

  public static string GetFormattedDistance(float meters)
  {
    if ((double) Mathf.Abs(meters) < 1.0)
    {
      string str1 = (meters * 100f).ToString();
      string str2 = str1.Substring(0, str1.LastIndexOf('.') + Mathf.Min(3, str1.Length - str1.LastIndexOf('.')));
      if (str2 == "-0.0")
        str2 = "0";
      return str2 + " cm";
    }
    return (double) meters < 1000.0 ? meters.ToString() + " m" : Util.FormatOneDecimalPlace(meters / 1000f) + " km";
  }

  public static string GetFormattedCycles(float seconds, string formatString = "F1", bool forceCycles = false) => forceCycles || (double) Mathf.Abs(seconds) > 100.0 ? string.Format((string) UI.FORMATDAY, (object) GameUtil.FloatToString(seconds / 600f, formatString)) : GameUtil.GetFormattedTime(seconds);

  public static float GetDisplaySHC(float shc)
  {
    if (GameUtil.temperatureUnit == GameUtil.TemperatureUnit.Fahrenheit)
      shc /= 1.8f;
    return shc;
  }

  public static string GetSHCSuffix() => string.Format("(DTU/g)/{0}", (object) GameUtil.GetTemperatureUnitSuffix());

  public static string GetFormattedSHC(float shc)
  {
    shc = GameUtil.GetDisplaySHC(shc);
    return string.Format("{0} (DTU/g)/{1}", (object) shc.ToString("0.000"), (object) GameUtil.GetTemperatureUnitSuffix());
  }

  public static float GetDisplayThermalConductivity(float tc)
  {
    if (GameUtil.temperatureUnit == GameUtil.TemperatureUnit.Fahrenheit)
      tc /= 1.8f;
    return tc;
  }

  public static string GetThermalConductivitySuffix() => string.Format("(DTU/(m*s))/{0}", (object) GameUtil.GetTemperatureUnitSuffix());

  public static string GetFormattedThermalConductivity(float tc)
  {
    tc = GameUtil.GetDisplayThermalConductivity(tc);
    return string.Format("{0} (DTU/(m*s))/{1}", (object) tc.ToString("0.000"), (object) GameUtil.GetTemperatureUnitSuffix());
  }

  public static string GetElementNameByElementHash(SimHashes elementHash) => ElementLoader.FindElementByHash(elementHash).tag.ProperName();

  public static bool HasTrait(GameObject go, string traitName)
  {
    Traits component = go.GetComponent<Traits>();
    return !Object.op_Equality((Object) component, (Object) null) && component.HasTrait(traitName);
  }

  public static HashSet<int> GetFloodFillCavity(int startCell, bool allowLiquid)
  {
    HashSet<int> intSet = new HashSet<int>();
    return !allowLiquid ? GameUtil.FloodCollectCells(startCell, (Func<int, bool>) (cell => Grid.Element[cell].IsVacuum || Grid.Element[cell].IsGas)) : GameUtil.FloodCollectCells(startCell, (Func<int, bool>) (cell => !Grid.Solid[cell]));
  }

  public static float GetRadiationAbsorptionPercentage(int cell) => Grid.IsValidCell(cell) ? GameUtil.GetRadiationAbsorptionPercentage(Grid.Element[cell], Grid.Mass[cell], Grid.IsSolidCell(cell) && ((int) Grid.Properties[cell] & 128) == 128) : 0.0f;

  public static float GetRadiationAbsorptionPercentage(
    Element elem,
    float mass,
    bool isConstructed)
  {
    float num1 = 2000f;
    float num2 = 0.3f;
    float num3 = 0.7f;
    float num4 = 0.8f;
    return Mathf.Clamp(!isConstructed ? (float) ((double) elem.radiationAbsorptionFactor * (double) num2 + (double) mass / (double) num1 * (double) elem.radiationAbsorptionFactor * (double) num3) : elem.radiationAbsorptionFactor * num4, 0.0f, 1f);
  }

  public static HashSet<int> CollectCellsBreadthFirst(
    int start_cell,
    Func<int, bool> test_func,
    int max_depth = 10)
  {
    HashSet<int> intSet1 = new HashSet<int>();
    HashSet<int> intSet2 = new HashSet<int>();
    HashSet<int> intSet3 = new HashSet<int>();
    intSet3.Add(start_cell);
    Vector2Int[] vector2IntArray = new Vector2Int[4]
    {
      new Vector2Int(1, 0),
      new Vector2Int(-1, 0),
      new Vector2Int(0, 1),
      new Vector2Int(0, -1)
    };
    for (int index = 0; index < max_depth; ++index)
    {
      List<int> intList = new List<int>();
      foreach (int cell1 in intSet3)
      {
        foreach (Vector2Int vector2Int in vector2IntArray)
        {
          int cell2 = Grid.OffsetCell(cell1, ((Vector2Int) ref vector2Int).x, ((Vector2Int) ref vector2Int).y);
          if (!intSet2.Contains(cell2) && !intSet1.Contains(cell2))
          {
            if ((!Grid.IsValidCell(cell2) ? 0 : (test_func(cell2) ? 1 : 0)) != 0)
            {
              intSet1.Add(cell2);
              intList.Add(cell2);
            }
            else
              intSet2.Add(cell2);
          }
        }
      }
      intSet3.Clear();
      foreach (int num in intList)
        intSet3.Add(num);
      intList.Clear();
      if (intSet3.Count == 0)
        break;
    }
    return intSet1;
  }

  public static HashSet<int> FloodCollectCells(
    int start_cell,
    Func<int, bool> is_valid,
    int maxSize = 300,
    HashSet<int> AddInvalidCellsToSet = null,
    bool clearOversizedResults = true)
  {
    HashSet<int> intSet1 = new HashSet<int>();
    HashSet<int> intSet2 = new HashSet<int>();
    GameUtil.probeFromCell(start_cell, is_valid, intSet1, intSet2, maxSize);
    if (AddInvalidCellsToSet != null)
    {
      AddInvalidCellsToSet.UnionWith((IEnumerable<int>) intSet2);
      if (intSet1.Count > maxSize)
        AddInvalidCellsToSet.UnionWith((IEnumerable<int>) intSet1);
    }
    if (intSet1.Count > maxSize & clearOversizedResults)
      intSet1.Clear();
    return intSet1;
  }

  public static HashSet<int> FloodCollectCells(
    HashSet<int> results,
    int start_cell,
    Func<int, bool> is_valid,
    int maxSize = 300,
    HashSet<int> AddInvalidCellsToSet = null,
    bool clearOversizedResults = true)
  {
    HashSet<int> intSet = new HashSet<int>();
    GameUtil.probeFromCell(start_cell, is_valid, results, intSet, maxSize);
    if (AddInvalidCellsToSet != null)
    {
      AddInvalidCellsToSet.UnionWith((IEnumerable<int>) intSet);
      if (results.Count > maxSize)
        AddInvalidCellsToSet.UnionWith((IEnumerable<int>) results);
    }
    if (results.Count > maxSize & clearOversizedResults)
      results.Clear();
    return results;
  }

  private static void probeFromCell(
    int start_cell,
    Func<int, bool> is_valid,
    HashSet<int> cells,
    HashSet<int> invalidCells,
    int maxSize = 300)
  {
    if (cells.Count > maxSize || !Grid.IsValidCell(start_cell) || invalidCells.Contains(start_cell) || cells.Contains(start_cell) || !is_valid(start_cell))
    {
      invalidCells.Add(start_cell);
    }
    else
    {
      cells.Add(start_cell);
      GameUtil.probeFromCell(Grid.CellLeft(start_cell), is_valid, cells, invalidCells, maxSize);
      GameUtil.probeFromCell(Grid.CellRight(start_cell), is_valid, cells, invalidCells, maxSize);
      GameUtil.probeFromCell(Grid.CellAbove(start_cell), is_valid, cells, invalidCells, maxSize);
      GameUtil.probeFromCell(Grid.CellBelow(start_cell), is_valid, cells, invalidCells, maxSize);
    }
  }

  public static bool FloodFillCheck<ArgType>(
    Func<int, ArgType, bool> fn,
    ArgType arg,
    int start_cell,
    int max_depth,
    bool stop_at_solid,
    bool stop_at_liquid)
  {
    return GameUtil.FloodFillFind<ArgType>(fn, arg, start_cell, max_depth, stop_at_solid, stop_at_liquid) != -1;
  }

  public static int FloodFillFind<ArgType>(
    Func<int, ArgType, bool> fn,
    ArgType arg,
    int start_cell,
    int max_depth,
    bool stop_at_solid,
    bool stop_at_liquid)
  {
    GameUtil.FloodFillNext.Enqueue(new GameUtil.FloodFillInfo()
    {
      cell = start_cell,
      depth = 0
    });
    int num = -1;
    while (GameUtil.FloodFillNext.Count > 0)
    {
      GameUtil.FloodFillInfo floodFillInfo1 = GameUtil.FloodFillNext.Dequeue();
      if (floodFillInfo1.depth < max_depth && Grid.IsValidCell(floodFillInfo1.cell))
      {
        Element element = Grid.Element[floodFillInfo1.cell];
        if ((!stop_at_solid || !element.IsSolid) && (!stop_at_liquid || !element.IsLiquid) && !GameUtil.FloodFillVisited.Contains(floodFillInfo1.cell))
        {
          GameUtil.FloodFillVisited.Add(floodFillInfo1.cell);
          if (fn(floodFillInfo1.cell, arg))
          {
            num = floodFillInfo1.cell;
            break;
          }
          Queue<GameUtil.FloodFillInfo> floodFillNext1 = GameUtil.FloodFillNext;
          GameUtil.FloodFillInfo floodFillInfo2 = new GameUtil.FloodFillInfo();
          floodFillInfo2.cell = Grid.CellLeft(floodFillInfo1.cell);
          floodFillInfo2.depth = floodFillInfo1.depth + 1;
          GameUtil.FloodFillInfo floodFillInfo3 = floodFillInfo2;
          floodFillNext1.Enqueue(floodFillInfo3);
          Queue<GameUtil.FloodFillInfo> floodFillNext2 = GameUtil.FloodFillNext;
          floodFillInfo2 = new GameUtil.FloodFillInfo();
          floodFillInfo2.cell = Grid.CellRight(floodFillInfo1.cell);
          floodFillInfo2.depth = floodFillInfo1.depth + 1;
          GameUtil.FloodFillInfo floodFillInfo4 = floodFillInfo2;
          floodFillNext2.Enqueue(floodFillInfo4);
          Queue<GameUtil.FloodFillInfo> floodFillNext3 = GameUtil.FloodFillNext;
          floodFillInfo2 = new GameUtil.FloodFillInfo();
          floodFillInfo2.cell = Grid.CellAbove(floodFillInfo1.cell);
          floodFillInfo2.depth = floodFillInfo1.depth + 1;
          GameUtil.FloodFillInfo floodFillInfo5 = floodFillInfo2;
          floodFillNext3.Enqueue(floodFillInfo5);
          Queue<GameUtil.FloodFillInfo> floodFillNext4 = GameUtil.FloodFillNext;
          floodFillInfo2 = new GameUtil.FloodFillInfo();
          floodFillInfo2.cell = Grid.CellBelow(floodFillInfo1.cell);
          floodFillInfo2.depth = floodFillInfo1.depth + 1;
          GameUtil.FloodFillInfo floodFillInfo6 = floodFillInfo2;
          floodFillNext4.Enqueue(floodFillInfo6);
        }
      }
    }
    GameUtil.FloodFillVisited.Clear();
    GameUtil.FloodFillNext.Clear();
    return num;
  }

  public static void FloodFillConditional(
    int start_cell,
    Func<int, bool> condition,
    ICollection<int> visited_cells,
    ICollection<int> valid_cells = null)
  {
    GameUtil.FloodFillNext.Enqueue(new GameUtil.FloodFillInfo()
    {
      cell = start_cell,
      depth = 0
    });
    GameUtil.FloodFillConditional(GameUtil.FloodFillNext, condition, visited_cells, valid_cells);
  }

  public static void FloodFillConditional(
    Queue<GameUtil.FloodFillInfo> queue,
    Func<int, bool> condition,
    ICollection<int> visited_cells,
    ICollection<int> valid_cells = null,
    int max_depth = 10000)
  {
    while (queue.Count > 0)
    {
      GameUtil.FloodFillInfo floodFillInfo1 = queue.Dequeue();
      if (floodFillInfo1.depth < max_depth && Grid.IsValidCell(floodFillInfo1.cell) && !visited_cells.Contains(floodFillInfo1.cell))
      {
        visited_cells.Add(floodFillInfo1.cell);
        if (condition(floodFillInfo1.cell))
        {
          valid_cells?.Add(floodFillInfo1.cell);
          int num = floodFillInfo1.depth + 1;
          Queue<GameUtil.FloodFillInfo> floodFillInfoQueue1 = queue;
          GameUtil.FloodFillInfo floodFillInfo2 = new GameUtil.FloodFillInfo();
          floodFillInfo2.cell = Grid.CellLeft(floodFillInfo1.cell);
          floodFillInfo2.depth = num;
          GameUtil.FloodFillInfo floodFillInfo3 = floodFillInfo2;
          floodFillInfoQueue1.Enqueue(floodFillInfo3);
          Queue<GameUtil.FloodFillInfo> floodFillInfoQueue2 = queue;
          floodFillInfo2 = new GameUtil.FloodFillInfo();
          floodFillInfo2.cell = Grid.CellRight(floodFillInfo1.cell);
          floodFillInfo2.depth = num;
          GameUtil.FloodFillInfo floodFillInfo4 = floodFillInfo2;
          floodFillInfoQueue2.Enqueue(floodFillInfo4);
          Queue<GameUtil.FloodFillInfo> floodFillInfoQueue3 = queue;
          floodFillInfo2 = new GameUtil.FloodFillInfo();
          floodFillInfo2.cell = Grid.CellAbove(floodFillInfo1.cell);
          floodFillInfo2.depth = num;
          GameUtil.FloodFillInfo floodFillInfo5 = floodFillInfo2;
          floodFillInfoQueue3.Enqueue(floodFillInfo5);
          Queue<GameUtil.FloodFillInfo> floodFillInfoQueue4 = queue;
          floodFillInfo2 = new GameUtil.FloodFillInfo();
          floodFillInfo2.cell = Grid.CellBelow(floodFillInfo1.cell);
          floodFillInfo2.depth = num;
          GameUtil.FloodFillInfo floodFillInfo6 = floodFillInfo2;
          floodFillInfoQueue4.Enqueue(floodFillInfo6);
        }
      }
    }
    queue.Clear();
  }

  public static string GetHardnessString(Element element, bool addColor = true)
  {
    if (!element.IsSolid)
      return (string) ELEMENTS.HARDNESS.NA;
    Color firmColor = GameUtil.Hardness.firmColor;
    Color color;
    string hardnessString;
    if (element.hardness >= byte.MaxValue)
    {
      color = GameUtil.Hardness.ImpenetrableColor;
      hardnessString = string.Format((string) ELEMENTS.HARDNESS.IMPENETRABLE, (object) element.hardness);
    }
    else if (element.hardness >= (byte) 150)
    {
      color = GameUtil.Hardness.nearlyImpenetrableColor;
      hardnessString = string.Format((string) ELEMENTS.HARDNESS.NEARLYIMPENETRABLE, (object) element.hardness);
    }
    else if (element.hardness >= (byte) 50)
    {
      color = GameUtil.Hardness.veryFirmColor;
      hardnessString = string.Format((string) ELEMENTS.HARDNESS.VERYFIRM, (object) element.hardness);
    }
    else if (element.hardness >= (byte) 25)
    {
      color = GameUtil.Hardness.firmColor;
      hardnessString = string.Format((string) ELEMENTS.HARDNESS.FIRM, (object) element.hardness);
    }
    else if (element.hardness >= (byte) 10)
    {
      color = GameUtil.Hardness.softColor;
      hardnessString = string.Format((string) ELEMENTS.HARDNESS.SOFT, (object) element.hardness);
    }
    else
    {
      color = GameUtil.Hardness.verySoftColor;
      hardnessString = string.Format((string) ELEMENTS.HARDNESS.VERYSOFT, (object) element.hardness);
    }
    if (addColor)
      hardnessString = string.Format("<color=#{0}>{1}</color>", (object) Util.ToHexString(color), (object) hardnessString);
    return hardnessString;
  }

  public static string GetGermResistanceModifierString(float modifier, bool addColor = true)
  {
    Color color = Color.black;
    string resistanceModifierString = "";
    if ((double) modifier > 0.0)
    {
      if ((double) modifier >= 5.0)
      {
        color = GameUtil.GermResistanceValues.PositiveLargeColor;
        resistanceModifierString = string.Format((string) DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_LARGE, (object) modifier);
      }
      else if ((double) modifier >= 2.0)
      {
        color = GameUtil.GermResistanceValues.PositiveMediumColor;
        resistanceModifierString = string.Format((string) DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_MEDIUM, (object) modifier);
      }
      else if ((double) modifier > 0.0)
      {
        color = GameUtil.GermResistanceValues.PositiveSmallColor;
        resistanceModifierString = string.Format((string) DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.POSITIVE_SMALL, (object) modifier);
      }
    }
    else if ((double) modifier < 0.0)
    {
      if ((double) modifier <= -5.0)
      {
        color = GameUtil.GermResistanceValues.NegativeLargeColor;
        resistanceModifierString = string.Format((string) DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_LARGE, (object) modifier);
      }
      else if ((double) modifier <= -2.0)
      {
        color = GameUtil.GermResistanceValues.NegativeMediumColor;
        resistanceModifierString = string.Format((string) DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_MEDIUM, (object) modifier);
      }
      else if ((double) modifier < 0.0)
      {
        color = GameUtil.GermResistanceValues.NegativeSmallColor;
        resistanceModifierString = string.Format((string) DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NEGATIVE_SMALL, (object) modifier);
      }
    }
    else
    {
      addColor = false;
      resistanceModifierString = string.Format((string) DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.MODIFIER_DESCRIPTORS.NONE, (object) modifier);
    }
    if (addColor)
      resistanceModifierString = string.Format("<color=#{0}>{1}</color>", (object) Util.ToHexString(color), (object) resistanceModifierString);
    return resistanceModifierString;
  }

  public static string GetThermalConductivityString(Element element, bool addColor = true, bool addValue = true)
  {
    Color conductivityColor1 = GameUtil.ThermalConductivityValues.mediumConductivityColor;
    Color conductivityColor2;
    string conductivityString;
    if ((double) element.thermalConductivity >= 50.0)
    {
      conductivityColor2 = GameUtil.ThermalConductivityValues.veryHighConductivityColor;
      conductivityString = (string) UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VERY_HIGH_CONDUCTIVITY;
    }
    else if ((double) element.thermalConductivity >= 10.0)
    {
      conductivityColor2 = GameUtil.ThermalConductivityValues.highConductivityColor;
      conductivityString = (string) UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.HIGH_CONDUCTIVITY;
    }
    else if ((double) element.thermalConductivity >= 2.0)
    {
      conductivityColor2 = GameUtil.ThermalConductivityValues.mediumConductivityColor;
      conductivityString = (string) UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.MEDIUM_CONDUCTIVITY;
    }
    else if ((double) element.thermalConductivity >= 1.0)
    {
      conductivityColor2 = GameUtil.ThermalConductivityValues.lowConductivityColor;
      conductivityString = (string) UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.LOW_CONDUCTIVITY;
    }
    else
    {
      conductivityColor2 = GameUtil.ThermalConductivityValues.veryLowConductivityColor;
      conductivityString = (string) UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VERY_LOW_CONDUCTIVITY;
    }
    if (addColor)
      conductivityString = string.Format("<color=#{0}>{1}</color>", (object) Util.ToHexString(conductivityColor2), (object) conductivityString);
    if (addValue)
      conductivityString = string.Format((string) UI.ELEMENTAL.THERMALCONDUCTIVITY.ADJECTIVES.VALUE_WITH_ADJECTIVE, (object) element.thermalConductivity.ToString(), (object) conductivityString);
    return conductivityString;
  }

  public static string GetBreathableString(Element element, float Mass)
  {
    if (!element.IsGas && !element.IsVacuum)
      return "";
    Color positiveColor = GameUtil.BreathableValues.positiveColor;
    Color color;
    LocString locString;
    switch (element.id)
    {
      case SimHashes.Oxygen:
        if ((double) Mass >= (double) SimDebugView.optimallyBreathable)
        {
          color = GameUtil.BreathableValues.positiveColor;
          locString = UI.OVERLAYS.OXYGEN.LEGEND1;
          break;
        }
        if ((double) Mass >= (double) SimDebugView.minimumBreathable + ((double) SimDebugView.optimallyBreathable - (double) SimDebugView.minimumBreathable) / 2.0)
        {
          color = GameUtil.BreathableValues.positiveColor;
          locString = UI.OVERLAYS.OXYGEN.LEGEND2;
          break;
        }
        if ((double) Mass >= (double) SimDebugView.minimumBreathable)
        {
          color = GameUtil.BreathableValues.warningColor;
          locString = UI.OVERLAYS.OXYGEN.LEGEND3;
          break;
        }
        color = GameUtil.BreathableValues.negativeColor;
        locString = UI.OVERLAYS.OXYGEN.LEGEND4;
        break;
      case SimHashes.ContaminatedOxygen:
        if ((double) Mass >= (double) SimDebugView.optimallyBreathable)
        {
          color = GameUtil.BreathableValues.positiveColor;
          locString = UI.OVERLAYS.OXYGEN.LEGEND1;
          break;
        }
        if ((double) Mass >= (double) SimDebugView.minimumBreathable + ((double) SimDebugView.optimallyBreathable - (double) SimDebugView.minimumBreathable) / 2.0)
        {
          color = GameUtil.BreathableValues.positiveColor;
          locString = UI.OVERLAYS.OXYGEN.LEGEND2;
          break;
        }
        if ((double) Mass >= (double) SimDebugView.minimumBreathable)
        {
          color = GameUtil.BreathableValues.warningColor;
          locString = UI.OVERLAYS.OXYGEN.LEGEND3;
          break;
        }
        color = GameUtil.BreathableValues.negativeColor;
        locString = UI.OVERLAYS.OXYGEN.LEGEND4;
        break;
      default:
        color = GameUtil.BreathableValues.negativeColor;
        locString = UI.OVERLAYS.OXYGEN.LEGEND4;
        break;
    }
    return string.Format((string) ELEMENTS.BREATHABLEDESC, (object) Util.ToHexString(color), (object) locString);
  }

  public static string GetWireLoadColor(float load, float maxLoad, float potentialLoad) => Util.ToHexString((double) load <= (double) maxLoad + (double) TUNING.POWER.FLOAT_FUDGE_FACTOR ? ((double) potentialLoad <= (double) maxLoad || (double) load / (double) maxLoad < 0.75 ? Color.white : GameUtil.WireLoadValues.warningColor) : GameUtil.WireLoadValues.negativeColor);

  public static string GetHotkeyString(Action action) => KInputManager.currentControllerIsGamepad ? UI.FormatAsHotkey(GameUtil.GetActionString(action)) : UI.FormatAsHotkey("[" + GameUtil.GetActionString(action) + "]");

  public static string ReplaceHotkeyString(string template, Action action) => template.Replace("{Hotkey}", GameUtil.GetHotkeyString(action));

  public static string ReplaceHotkeyString(string template, Action action1, Action action2) => template.Replace("{Hotkey}", GameUtil.GetHotkeyString(action1) + GameUtil.GetHotkeyString(action2));

  public static string GetKeycodeLocalized(KKeyCode key_code)
  {
    string keycodeLocalized = key_code.ToString();
    if (key_code <= 47)
    {
      if (key_code <= 9)
      {
        if (key_code != null)
        {
          if (key_code != 8)
          {
            if (key_code == 9)
            {
              keycodeLocalized = (string) INPUT.TAB;
              goto label_75;
            }
          }
          else
          {
            keycodeLocalized = (string) INPUT.BACKSPACE;
            goto label_75;
          }
        }
        else
          goto label_75;
      }
      else if (key_code <= 27)
      {
        if (key_code != 13)
        {
          if (key_code == 27)
          {
            keycodeLocalized = (string) INPUT.ESCAPE;
            goto label_75;
          }
        }
        else
        {
          keycodeLocalized = (string) INPUT.ENTER;
          goto label_75;
        }
      }
      else if (key_code != 32)
      {
        switch (key_code - 43)
        {
          case 0:
            keycodeLocalized = "+";
            goto label_75;
          case 1:
            keycodeLocalized = ",";
            goto label_75;
          case 2:
            keycodeLocalized = "-";
            goto label_75;
          case 3:
            keycodeLocalized = (string) INPUT.PERIOD;
            goto label_75;
          case 4:
            keycodeLocalized = "/";
            goto label_75;
        }
      }
      else
      {
        keycodeLocalized = (string) INPUT.SPACE;
        goto label_75;
      }
    }
    else if (key_code <= 277)
    {
      switch (key_code - 58)
      {
        case 0:
          keycodeLocalized = ":";
          goto label_75;
        case 1:
          keycodeLocalized = ";";
          goto label_75;
        case 2:
          break;
        case 3:
          keycodeLocalized = "=";
          goto label_75;
        default:
          switch (key_code - 91)
          {
            case 0:
              keycodeLocalized = "[";
              goto label_75;
            case 1:
              keycodeLocalized = "\\";
              goto label_75;
            case 2:
              keycodeLocalized = "]";
              goto label_75;
            case 3:
            case 4:
              break;
            case 5:
              keycodeLocalized = (string) INPUT.BACKQUOTE;
              goto label_75;
            default:
              switch (key_code - 256)
              {
                case 0:
                  keycodeLocalized = (string) INPUT.NUM + " 0";
                  goto label_75;
                case 1:
                  keycodeLocalized = (string) INPUT.NUM + " 1";
                  goto label_75;
                case 2:
                  keycodeLocalized = (string) INPUT.NUM + " 2";
                  goto label_75;
                case 3:
                  keycodeLocalized = (string) INPUT.NUM + " 3";
                  goto label_75;
                case 4:
                  keycodeLocalized = (string) INPUT.NUM + " 4";
                  goto label_75;
                case 5:
                  keycodeLocalized = (string) INPUT.NUM + " 5";
                  goto label_75;
                case 6:
                  keycodeLocalized = (string) INPUT.NUM + " 6";
                  goto label_75;
                case 7:
                  keycodeLocalized = (string) INPUT.NUM + " 7";
                  goto label_75;
                case 8:
                  keycodeLocalized = (string) INPUT.NUM + " 8";
                  goto label_75;
                case 9:
                  keycodeLocalized = (string) INPUT.NUM + " 9";
                  goto label_75;
                case 10:
                  keycodeLocalized = (string) INPUT.NUM + " " + (string) INPUT.PERIOD;
                  goto label_75;
                case 11:
                  keycodeLocalized = (string) INPUT.NUM + " /";
                  goto label_75;
                case 12:
                  keycodeLocalized = (string) INPUT.NUM + " *";
                  goto label_75;
                case 13:
                  keycodeLocalized = (string) INPUT.NUM + " -";
                  goto label_75;
                case 14:
                  keycodeLocalized = (string) INPUT.NUM + " +";
                  goto label_75;
                case 15:
                  keycodeLocalized = (string) INPUT.NUM + " " + (string) INPUT.ENTER;
                  goto label_75;
                case 21:
                  keycodeLocalized = (string) INPUT.INSERT;
                  goto label_75;
              }
              break;
          }
          break;
      }
    }
    else if (key_code <= 329)
    {
      switch (key_code - 303)
      {
        case 0:
          keycodeLocalized = (string) INPUT.RIGHT_SHIFT;
          goto label_75;
        case 1:
          keycodeLocalized = (string) INPUT.LEFT_SHIFT;
          goto label_75;
        case 2:
          keycodeLocalized = (string) INPUT.RIGHT_CTRL;
          goto label_75;
        case 3:
          keycodeLocalized = (string) INPUT.LEFT_CTRL;
          goto label_75;
        case 4:
          keycodeLocalized = (string) INPUT.RIGHT_ALT;
          goto label_75;
        case 5:
          keycodeLocalized = (string) INPUT.LEFT_ALT;
          goto label_75;
        default:
          switch (key_code - 323)
          {
            case 0:
              keycodeLocalized = (string) INPUT.MOUSE + " 0";
              goto label_75;
            case 1:
              keycodeLocalized = (string) INPUT.MOUSE + " 1";
              goto label_75;
            case 2:
              keycodeLocalized = (string) INPUT.MOUSE + " 2";
              goto label_75;
            case 3:
              keycodeLocalized = (string) INPUT.MOUSE + " 3";
              goto label_75;
            case 4:
              keycodeLocalized = (string) INPUT.MOUSE + " 4";
              goto label_75;
            case 5:
              keycodeLocalized = (string) INPUT.MOUSE + " 5";
              goto label_75;
            case 6:
              keycodeLocalized = (string) INPUT.MOUSE + " 6";
              goto label_75;
          }
          break;
      }
    }
    else if (key_code != 1001)
    {
      if (key_code == 1002)
      {
        keycodeLocalized = (string) INPUT.MOUSE_SCROLL_UP;
        goto label_75;
      }
    }
    else
    {
      keycodeLocalized = (string) INPUT.MOUSE_SCROLL_DOWN;
      goto label_75;
    }
    if (97 <= key_code && key_code <= 122)
      keycodeLocalized = ((char) (65 + (key_code - 97))).ToString();
    else if (48 <= key_code && key_code <= 57)
      keycodeLocalized = ((char) (48 + (key_code - 48))).ToString();
    else if (282 <= key_code && key_code <= 293)
      keycodeLocalized = "F" + (key_code - 282 + 1).ToString();
    else
      Debug.LogWarning((object) ("Unable to find proper string for KKeyCode: " + key_code.ToString() + " using key_code.ToString()"));
label_75:
    return keycodeLocalized;
  }

  public static string GetActionString(Action action)
  {
    string actionString = "";
    if (action == 275)
      return actionString;
    BindingEntry binding = GameUtil.ActionToBinding(action);
    KKeyCode mKeyCode = binding.mKeyCode;
    if (KInputManager.currentControllerIsGamepad)
      return KInputManager.steamInputInterpreter.GetActionGlyph(action);
    if (binding.mModifier == null)
      return GameUtil.GetKeycodeLocalized(mKeyCode).ToUpper();
    string str = "";
    Modifier mModifier = binding.mModifier;
    switch (mModifier - 1)
    {
      case 0:
        str = GameUtil.GetKeycodeLocalized((KKeyCode) 308).ToUpper();
        goto case 2;
      case 1:
        str = GameUtil.GetKeycodeLocalized((KKeyCode) 306).ToUpper();
        goto case 2;
      case 2:
        return str + " + " + GameUtil.GetKeycodeLocalized(mKeyCode).ToUpper();
      case 3:
        str = GameUtil.GetKeycodeLocalized((KKeyCode) 304).ToUpper();
        goto case 2;
      default:
        if (mModifier != 8)
        {
          if (mModifier == 16)
          {
            str = GameUtil.GetKeycodeLocalized((KKeyCode) 96).ToUpper();
            goto case 2;
          }
          else
            goto case 2;
        }
        else
        {
          str = GameUtil.GetKeycodeLocalized((KKeyCode) 301).ToUpper();
          goto case 2;
        }
    }
  }

  public static void CreateExplosion(Vector3 explosion_pos)
  {
    Vector2 vector2_1;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_1).\u002Ector(explosion_pos.x, explosion_pos.y);
    double num1 = 5.0;
    float num2 = (float) (num1 * num1);
    foreach (Health health in Components.Health.Items)
    {
      Vector3 position = TransformExtensions.GetPosition(health.transform);
      Vector2 vector2_2 = Vector2.op_Subtraction(new Vector2(position.x, position.y), vector2_1);
      float sqrMagnitude = ((Vector2) ref vector2_2).sqrMagnitude;
      if ((double) num2 >= (double) sqrMagnitude && Object.op_Inequality((Object) health, (Object) null))
        health.Damage(health.maxHitPoints);
    }
  }

  private static void GetNonSolidCells(
    int x,
    int y,
    List<int> cells,
    int min_x,
    int min_y,
    int max_x,
    int max_y)
  {
    int cell = Grid.XYToCell(x, y);
    if (!Grid.IsValidCell(cell) || Grid.Solid[cell] || Grid.DupePassable[cell] || x < min_x || x > max_x || y < min_y || y > max_y || cells.Contains(cell))
      return;
    cells.Add(cell);
    GameUtil.GetNonSolidCells(x + 1, y, cells, min_x, min_y, max_x, max_y);
    GameUtil.GetNonSolidCells(x - 1, y, cells, min_x, min_y, max_x, max_y);
    GameUtil.GetNonSolidCells(x, y + 1, cells, min_x, min_y, max_x, max_y);
    GameUtil.GetNonSolidCells(x, y - 1, cells, min_x, min_y, max_x, max_y);
  }

  public static void GetNonSolidCells(int cell, int radius, List<int> cells)
  {
    int x = 0;
    int y = 0;
    Grid.CellToXY(cell, out x, out y);
    GameUtil.GetNonSolidCells(x, y, cells, x - radius, y - radius, x + radius, y + radius);
  }

  public static float GetMaxStressInActiveWorld()
  {
    if (Components.LiveMinionIdentities.Count <= 0)
      return 0.0f;
    float stressInActiveWorld = 0.0f;
    foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
    {
      if (!Util.IsNullOrDestroyed((object) minionIdentity) && minionIdentity.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
      {
        AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup((Component) minionIdentity);
        if (amountInstance != null)
          stressInActiveWorld = Mathf.Max(stressInActiveWorld, amountInstance.value);
      }
    }
    return stressInActiveWorld;
  }

  public static float GetAverageStressInActiveWorld()
  {
    if (Components.LiveMinionIdentities.Count <= 0)
      return 0.0f;
    float num1 = 0.0f;
    int num2 = 0;
    foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
    {
      if (!Util.IsNullOrDestroyed((object) minionIdentity) && minionIdentity.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
      {
        num1 += Db.Get().Amounts.Stress.Lookup((Component) minionIdentity).value;
        ++num2;
      }
    }
    return num1 / (float) num2;
  }

  public static string MigrateFMOD(FMODAsset asset)
  {
    if (Object.op_Equality((Object) asset, (Object) null))
      return (string) null;
    return asset.path == null ? ((Object) asset).name : asset.path;
  }

  private static void SortGameObjectDescriptors(List<IGameObjectEffectDescriptor> descriptorList) => descriptorList.Sort((Comparison<IGameObjectEffectDescriptor>) ((e1, e2) => TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e1.GetType()).CompareTo(TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER.IndexOf(e2.GetType()))));

  public static void IndentListOfDescriptors(List<Descriptor> list, int indentCount = 1)
  {
    for (int index1 = 0; index1 < list.Count; ++index1)
    {
      Descriptor descriptor = list[index1];
      for (int index2 = 0; index2 < indentCount; ++index2)
        ((Descriptor) ref descriptor).IncreaseIndent();
      list[index1] = descriptor;
    }
  }

  public static List<Descriptor> GetAllDescriptors(GameObject go, bool simpleInfoScreen = false)
  {
    List<Descriptor> allDescriptors = new List<Descriptor>();
    List<IGameObjectEffectDescriptor> descriptorList = new List<IGameObjectEffectDescriptor>((IEnumerable<IGameObjectEffectDescriptor>) go.GetComponents<IGameObjectEffectDescriptor>());
    StateMachineController component1 = go.GetComponent<StateMachineController>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      descriptorList.AddRange((IEnumerable<IGameObjectEffectDescriptor>) component1.GetDescriptors());
    GameUtil.SortGameObjectDescriptors(descriptorList);
    foreach (IGameObjectEffectDescriptor effectDescriptor in descriptorList)
    {
      List<Descriptor> descriptors = effectDescriptor.GetDescriptors(go);
      if (descriptors != null)
      {
        foreach (Descriptor descriptor in descriptors)
        {
          if (!descriptor.onlyForSimpleInfoScreen || simpleInfoScreen)
            allDescriptors.Add(descriptor);
        }
      }
    }
    KPrefabID component2 = go.GetComponent<KPrefabID>();
    if (Object.op_Inequality((Object) component2, (Object) null) && component2.AdditionalRequirements != null)
    {
      foreach (Descriptor additionalRequirement in component2.AdditionalRequirements)
      {
        if (!additionalRequirement.onlyForSimpleInfoScreen || simpleInfoScreen)
          allDescriptors.Add(additionalRequirement);
      }
    }
    if (Object.op_Inequality((Object) component2, (Object) null) && component2.AdditionalEffects != null)
    {
      foreach (Descriptor additionalEffect in component2.AdditionalEffects)
      {
        if (!additionalEffect.onlyForSimpleInfoScreen || simpleInfoScreen)
          allDescriptors.Add(additionalEffect);
      }
    }
    return allDescriptors;
  }

  public static List<Descriptor> GetDetailDescriptors(List<Descriptor> descriptors)
  {
    List<Descriptor> list = new List<Descriptor>();
    foreach (Descriptor descriptor in descriptors)
    {
      if (descriptor.type == 5)
        list.Add(descriptor);
    }
    GameUtil.IndentListOfDescriptors(list);
    return list;
  }

  public static List<Descriptor> GetRequirementDescriptors(List<Descriptor> descriptors)
  {
    List<Descriptor> list = new List<Descriptor>();
    foreach (Descriptor descriptor in descriptors)
    {
      if (descriptor.type == null)
        list.Add(descriptor);
    }
    GameUtil.IndentListOfDescriptors(list);
    return list;
  }

  public static List<Descriptor> GetEffectDescriptors(List<Descriptor> descriptors)
  {
    List<Descriptor> list = new List<Descriptor>();
    foreach (Descriptor descriptor in descriptors)
    {
      if (descriptor.type == 1 || descriptor.type == 4)
        list.Add(descriptor);
    }
    GameUtil.IndentListOfDescriptors(list);
    return list;
  }

  public static List<Descriptor> GetInformationDescriptors(List<Descriptor> descriptors)
  {
    List<Descriptor> list = new List<Descriptor>();
    foreach (Descriptor descriptor in descriptors)
    {
      if (descriptor.type == 2)
        list.Add(descriptor);
    }
    GameUtil.IndentListOfDescriptors(list);
    return list;
  }

  public static List<Descriptor> GetCropOptimumConditionDescriptors(List<Descriptor> descriptors)
  {
    List<Descriptor> list = new List<Descriptor>();
    foreach (Descriptor descriptor1 in descriptors)
    {
      if (descriptor1.type == 2)
      {
        Descriptor descriptor2 = descriptor1;
        descriptor2.text = "• " + descriptor2.text;
        list.Add(descriptor2);
      }
    }
    GameUtil.IndentListOfDescriptors(list);
    return list;
  }

  public static List<Descriptor> GetGameObjectRequirements(GameObject go)
  {
    List<Descriptor> objectRequirements = new List<Descriptor>();
    List<IGameObjectEffectDescriptor> descriptorList = new List<IGameObjectEffectDescriptor>((IEnumerable<IGameObjectEffectDescriptor>) go.GetComponents<IGameObjectEffectDescriptor>());
    StateMachineController component1 = go.GetComponent<StateMachineController>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      descriptorList.AddRange((IEnumerable<IGameObjectEffectDescriptor>) component1.GetDescriptors());
    GameUtil.SortGameObjectDescriptors(descriptorList);
    foreach (IGameObjectEffectDescriptor effectDescriptor in descriptorList)
    {
      List<Descriptor> descriptors = effectDescriptor.GetDescriptors(go);
      if (descriptors != null)
      {
        foreach (Descriptor descriptor in descriptors)
        {
          if (descriptor.type == null)
            objectRequirements.Add(descriptor);
        }
      }
    }
    KPrefabID component2 = go.GetComponent<KPrefabID>();
    if (component2.AdditionalRequirements != null)
      objectRequirements.AddRange((IEnumerable<Descriptor>) component2.AdditionalRequirements);
    return objectRequirements;
  }

  public static List<Descriptor> GetGameObjectEffects(GameObject go, bool simpleInfoScreen = false)
  {
    List<Descriptor> gameObjectEffects = new List<Descriptor>();
    List<IGameObjectEffectDescriptor> descriptorList = new List<IGameObjectEffectDescriptor>((IEnumerable<IGameObjectEffectDescriptor>) go.GetComponents<IGameObjectEffectDescriptor>());
    StateMachineController component1 = go.GetComponent<StateMachineController>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      descriptorList.AddRange((IEnumerable<IGameObjectEffectDescriptor>) component1.GetDescriptors());
    GameUtil.SortGameObjectDescriptors(descriptorList);
    foreach (IGameObjectEffectDescriptor effectDescriptor in descriptorList)
    {
      List<Descriptor> descriptors = effectDescriptor.GetDescriptors(go);
      if (descriptors != null)
      {
        foreach (Descriptor descriptor in descriptors)
        {
          if ((!descriptor.onlyForSimpleInfoScreen || simpleInfoScreen) && (descriptor.type == 1 || descriptor.type == 4))
            gameObjectEffects.Add(descriptor);
        }
      }
    }
    KPrefabID component2 = go.GetComponent<KPrefabID>();
    if (Object.op_Inequality((Object) component2, (Object) null) && component2.AdditionalEffects != null)
    {
      foreach (Descriptor additionalEffect in component2.AdditionalEffects)
      {
        if (!additionalEffect.onlyForSimpleInfoScreen || simpleInfoScreen)
          gameObjectEffects.Add(additionalEffect);
      }
    }
    return gameObjectEffects;
  }

  public static List<Descriptor> GetPlantRequirementDescriptors(GameObject go)
  {
    List<Descriptor> requirementDescriptors1 = new List<Descriptor>();
    List<Descriptor> requirementDescriptors2 = GameUtil.GetRequirementDescriptors(GameUtil.GetAllDescriptors(go));
    if (requirementDescriptors2.Count > 0)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.PLANTREQUIREMENTS, (string) UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTREQUIREMENTS, (Descriptor.DescriptorType) 0);
      requirementDescriptors1.Add(descriptor);
      requirementDescriptors1.AddRange((IEnumerable<Descriptor>) requirementDescriptors2);
    }
    return requirementDescriptors1;
  }

  public static List<Descriptor> GetPlantLifeCycleDescriptors(GameObject go)
  {
    List<Descriptor> cycleDescriptors = new List<Descriptor>();
    List<Descriptor> informationDescriptors = GameUtil.GetInformationDescriptors(GameUtil.GetAllDescriptors(go));
    if (informationDescriptors.Count > 0)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.LIFECYCLE, (string) UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTLIFECYCLE, (Descriptor.DescriptorType) 2);
      cycleDescriptors.Add(descriptor);
      cycleDescriptors.AddRange((IEnumerable<Descriptor>) informationDescriptors);
    }
    return cycleDescriptors;
  }

  public static List<Descriptor> GetPlantEffectDescriptors(GameObject go)
  {
    List<Descriptor> effectDescriptors = new List<Descriptor>();
    if (Object.op_Equality((Object) go.GetComponent<Growing>(), (Object) null))
      return effectDescriptors;
    List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(go);
    List<Descriptor> collection = new List<Descriptor>();
    collection.AddRange((IEnumerable<Descriptor>) GameUtil.GetEffectDescriptors(allDescriptors));
    if (collection.Count > 0)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.PLANTEFFECTS, (string) UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.PLANTEFFECTS, (Descriptor.DescriptorType) 1);
      effectDescriptors.Add(descriptor);
      effectDescriptors.AddRange((IEnumerable<Descriptor>) collection);
    }
    return effectDescriptors;
  }

  public static string GetGameObjectEffectsTooltipString(GameObject go)
  {
    string effectsTooltipString = "";
    List<Descriptor> gameObjectEffects = GameUtil.GetGameObjectEffects(go);
    if (gameObjectEffects.Count > 0)
      effectsTooltipString = effectsTooltipString + (string) UI.BUILDINGEFFECTS.OPERATIONEFFECTS + "\n";
    foreach (Descriptor descriptor in gameObjectEffects)
      effectsTooltipString = effectsTooltipString + ((Descriptor) ref descriptor).IndentedText() + "\n";
    return effectsTooltipString;
  }

  public static List<Descriptor> GetEquipmentEffects(EquipmentDef def)
  {
    Debug.Assert(Object.op_Inequality((Object) def, (Object) null));
    List<Descriptor> equipmentEffects = new List<Descriptor>();
    List<AttributeModifier> attributeModifiers = def.AttributeModifiers;
    if (attributeModifiers != null)
    {
      foreach (AttributeModifier attributeModifier in attributeModifiers)
      {
        string name = Db.Get().Attributes.Get(attributeModifier.AttributeId).Name;
        string formattedString = attributeModifier.GetFormattedString();
        string newValue = (double) attributeModifier.Value >= 0.0 ? "produced" : "consumed";
        string str = UI.GAMEOBJECTEFFECTS.EQUIPMENT_MODS.text.Replace("{Attribute}", name).Replace("{Style}", newValue).Replace("{Value}", formattedString);
        equipmentEffects.Add(new Descriptor(str, str, (Descriptor.DescriptorType) 1, false));
      }
    }
    return equipmentEffects;
  }

  public static string GetRecipeDescription(Recipe recipe)
  {
    string recipeDescription = (string) null;
    if (recipe != null)
      recipeDescription = recipe.recipeDescription;
    if (recipeDescription == null)
    {
      recipeDescription = (string) RESEARCH.TYPES.MISSINGRECIPEDESC;
      Debug.LogWarning((object) "Missing recipeDescription");
    }
    return recipeDescription;
  }

  public static int GetCurrentCycle() => GameClock.Instance.GetCycle() + 1;

  public static float GetCurrentTimeInCycles() => GameClock.Instance.GetTimeInCycles() + 1f;

  public static GameObject GetActiveTelepad()
  {
    GameObject telepad = GameUtil.GetTelepad(ClusterManager.Instance.activeWorldId);
    if (Object.op_Equality((Object) telepad, (Object) null))
      telepad = GameUtil.GetTelepad(ClusterManager.Instance.GetStartWorld().id);
    return telepad;
  }

  public static GameObject GetTelepad(int worldId)
  {
    if (Components.Telepads.Count > 0)
    {
      for (int idx = 0; idx < Components.Telepads.Count; ++idx)
      {
        if (Components.Telepads[idx].GetMyWorldId() == worldId)
          return ((Component) Components.Telepads[idx]).gameObject;
      }
    }
    return (GameObject) null;
  }

  public static GameObject KInstantiate(
    GameObject original,
    Vector3 position,
    Grid.SceneLayer sceneLayer,
    string name = null,
    int gameLayer = 0)
  {
    return GameUtil.KInstantiate(original, position, sceneLayer, (GameObject) null, name, gameLayer);
  }

  public static GameObject KInstantiate(
    GameObject original,
    Vector3 position,
    Grid.SceneLayer sceneLayer,
    GameObject parent,
    string name = null,
    int gameLayer = 0)
  {
    position.z = Grid.GetLayerZ(sceneLayer);
    return Util.KInstantiate(original, position, Quaternion.identity, parent, name, true, gameLayer);
  }

  public static GameObject KInstantiate(
    GameObject original,
    Grid.SceneLayer sceneLayer,
    string name = null,
    int gameLayer = 0)
  {
    return GameUtil.KInstantiate(original, Vector3.zero, sceneLayer, name, gameLayer);
  }

  public static GameObject KInstantiate(
    Component original,
    Grid.SceneLayer sceneLayer,
    string name = null,
    int gameLayer = 0)
  {
    return GameUtil.KInstantiate(original.gameObject, Vector3.zero, sceneLayer, name, gameLayer);
  }

  public static unsafe void IsEmissionBlocked(
    int cell,
    out bool all_not_gaseous,
    out bool all_over_pressure)
  {
    int* numPtr = stackalloc int[4];
    numPtr[0] = Grid.CellBelow(cell);
    numPtr[1] = Grid.CellLeft(cell);
    numPtr[2] = Grid.CellRight(cell);
    numPtr[3] = Grid.CellAbove(cell);
    all_not_gaseous = true;
    all_over_pressure = true;
    for (int index1 = 0; index1 < 4; ++index1)
    {
      int index2 = numPtr[index1];
      if (Grid.IsValidCell(index2))
      {
        Element element = Grid.Element[index2];
        all_not_gaseous = all_not_gaseous && !element.IsGas && !element.IsVacuum;
        all_over_pressure = all_over_pressure && (!element.IsGas && !element.IsVacuum || (double) Grid.Mass[index2] >= 1.7999999523162842);
      }
    }
  }

  public static float GetDecorAtCell(int cell)
  {
    float decorAtCell = 0.0f;
    if (!Grid.Solid[cell])
      decorAtCell = Grid.Decor[cell] + (float) DecorProvider.GetLightDecorBonus(cell);
    return decorAtCell;
  }

  public static string GetUnitTypeMassOrUnit(GameObject go)
  {
    string unitTypeMassOrUnit = (string) UI.UNITSUFFIXES.UNITS;
    KPrefabID component = go.GetComponent<KPrefabID>();
    if (Object.op_Inequality((Object) component, (Object) null))
      unitTypeMassOrUnit = (string) (component.Tags.Contains(GameTags.Seed) ? UI.UNITSUFFIXES.UNITS : UI.UNITSUFFIXES.MASS.KILOGRAM);
    return unitTypeMassOrUnit;
  }

  public static string GetKeywordStyle(Tag tag)
  {
    Element element = ElementLoader.GetElement(tag);
    return element == null ? (!GameUtil.foodTags.Contains(tag) ? (!GameUtil.solidTags.Contains(tag) ? (string) null : "solid") : "food") : GameUtil.GetKeywordStyle(element);
  }

  public static string GetKeywordStyle(SimHashes hash)
  {
    Element elementByHash = ElementLoader.FindElementByHash(hash);
    return elementByHash != null ? GameUtil.GetKeywordStyle(elementByHash) : (string) null;
  }

  public static string GetKeywordStyle(Element element)
  {
    if (element.id == SimHashes.Oxygen)
      return "oxygen";
    if (element.IsSolid)
      return "solid";
    if (element.IsLiquid)
      return "liquid";
    if (element.IsGas)
      return "gas";
    return element.IsVacuum ? "vacuum" : (string) null;
  }

  public static string GetKeywordStyle(GameObject go)
  {
    string keywordStyle = "";
    Edible component1 = go.GetComponent<Edible>();
    Equippable component2 = go.GetComponent<Equippable>();
    MedicinalPill component3 = go.GetComponent<MedicinalPill>();
    ResearchPointObject component4 = go.GetComponent<ResearchPointObject>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      keywordStyle = "food";
    else if (Object.op_Inequality((Object) component2, (Object) null))
      keywordStyle = "equipment";
    else if (Object.op_Inequality((Object) component3, (Object) null))
      keywordStyle = "medicine";
    else if (Object.op_Inequality((Object) component4, (Object) null))
      keywordStyle = "research";
    return keywordStyle;
  }

  public static Sprite GetBiomeSprite(string id)
  {
    string str = "biomeIcon" + char.ToUpper(id[0]).ToString() + id.Substring(1).ToLower();
    Sprite sprite = Assets.GetSprite(HashedString.op_Implicit(str));
    if (Object.op_Inequality((Object) sprite, (Object) null))
      return new Tuple<Sprite, Color>(sprite, Color.white).first;
    Debug.LogWarning((object) ("Missing codex biome icon: " + str));
    return (Sprite) null;
  }

  public static string GenerateRandomDuplicantName()
  {
    string str1 = "";
    string str2 = "";
    bool flag = (double) Random.Range(0.0f, 1f) >= 0.5;
    List<string> stringList1 = new List<string>((IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.DUPLICANT.NAME.NB)));
    stringList1.AddRange(flag ? (IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.DUPLICANT.NAME.MALE)) : (IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.DUPLICANT.NAME.FEMALE)));
    string random = Util.GetRandom<string>(stringList1);
    if ((double) Random.Range(0.0f, 1f) > 0.699999988079071)
    {
      List<string> stringList2 = new List<string>((IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.DUPLICANT.PREFIX.NB)));
      stringList2.AddRange(flag ? (IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.DUPLICANT.PREFIX.MALE)) : (IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.DUPLICANT.PREFIX.FEMALE)));
      str1 = Util.GetRandom<string>(stringList2);
    }
    if (!string.IsNullOrEmpty(str1))
      str1 += " ";
    if ((double) Random.Range(0.0f, 1f) >= 0.89999997615814209)
    {
      List<string> stringList3 = new List<string>((IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.DUPLICANT.SUFFIX.NB)));
      stringList3.AddRange(flag ? (IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.DUPLICANT.SUFFIX.MALE)) : (IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.DUPLICANT.SUFFIX.FEMALE)));
      str2 = Util.GetRandom<string>(stringList3);
    }
    if (!string.IsNullOrEmpty(str2))
      str2 = " " + str2;
    return str1 + random + str2;
  }

  public static string GenerateRandomLaunchPadName() => NAMEGEN.LAUNCHPAD.FORMAT.Replace("{Name}", Random.Range(1, 1000).ToString());

  public static string GenerateRandomRocketName()
  {
    string newValue1 = "";
    string newValue2 = "";
    string newValue3 = "";
    int num1 = 1;
    int num2 = 2;
    int num3 = 4;
    string random = Util.GetRandom<string>(new List<string>((IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.ROCKET.NOUN))));
    int num4 = 0;
    if ((double) Random.value > 0.699999988079071)
    {
      newValue1 = Util.GetRandom<string>(new List<string>((IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.ROCKET.PREFIX))));
      num4 |= num1;
    }
    if ((double) Random.value > 0.5)
    {
      newValue2 = Util.GetRandom<string>(new List<string>((IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.ROCKET.ADJECTIVE))));
      num4 |= num2;
    }
    if ((double) Random.value > 0.10000000149011612)
    {
      newValue3 = Util.GetRandom<string>(new List<string>((IEnumerable<string>) LocString.GetStrings(typeof (NAMEGEN.ROCKET.SUFFIX))));
      num4 |= num3;
    }
    string str = num4 != (num1 | num2 | num3) ? (num4 != (num2 | num3) ? (num4 != (num1 | num3) ? (num4 != num3 ? (num4 != (num1 | num2) ? (num4 != num1 ? (num4 != num2 ? (string) NAMEGEN.ROCKET.FMT_NOUN : (string) NAMEGEN.ROCKET.FMT_ADJECTIVE_NOUN) : (string) NAMEGEN.ROCKET.FMT_PREFIX_NOUN) : (string) NAMEGEN.ROCKET.FMT_PREFIX_ADJECTIVE_NOUN) : (string) NAMEGEN.ROCKET.FMT_NOUN_SUFFIX) : (string) NAMEGEN.ROCKET.FMT_PREFIX_NOUN_SUFFIX) : (string) NAMEGEN.ROCKET.FMT_ADJECTIVE_NOUN_SUFFIX) : (string) NAMEGEN.ROCKET.FMT_PREFIX_ADJECTIVE_NOUN_SUFFIX;
    DebugUtil.LogArgs(new object[2]
    {
      (object) "Rocket name bits:",
      (object) Convert.ToString(num4, 2)
    });
    return str.Replace("{Prefix}", newValue1).Replace("{Adjective}", newValue2).Replace("{Noun}", random).Replace("{Suffix}", newValue3);
  }

  public static string GenerateRandomWorldName(string[] nameTables)
  {
    if (nameTables == null)
    {
      Debug.LogWarning((object) "No name tables provided to generate world name. Using GENERIC");
      nameTables = new string[1]{ "GENERIC" };
    }
    string source = "";
    foreach (string nameTable in nameTables)
      source += StringEntry.op_Implicit(Strings.Get("STRINGS.NAMEGEN.WORLD.ROOTS." + nameTable.ToUpper()));
    string str1 = GameUtil.RandomValueFromSeparatedString(source);
    if (string.IsNullOrEmpty(str1))
      str1 = GameUtil.RandomValueFromSeparatedString(StringEntry.op_Implicit(Strings.Get((string) NAMEGEN.WORLD.ROOTS.GENERIC)));
    string str2 = GameUtil.RandomValueFromSeparatedString((string) NAMEGEN.WORLD.SUFFIXES.GENERICLIST);
    return str1 + str2;
  }

  public static float GetThermalComfort(int cell, float tolerance = -0.08368001f)
  {
    float num1 = tolerance;
    float num2 = 0.0f;
    Element elementByHash = ElementLoader.FindElementByHash(SimHashes.Creature);
    if ((double) Grid.Element[cell].thermalConductivity != 0.0)
      num2 = SimUtil.CalculateEnergyFlowCreatures(cell, 310.15f, elementByHash.specificHeatCapacity, elementByHash.thermalConductivity, creature_surface_thickness: 0.0045f);
    return (num2 - num1) * 1000f;
  }

  public static string RandomValueFromSeparatedString(string source, string separator = "\n")
  {
    int startIndex1 = 0;
    int num1 = 0;
    while (true)
    {
      int num2 = source.IndexOf(separator, startIndex1);
      if (num2 != -1)
      {
        startIndex1 = num2 + separator.Length;
        ++num1;
      }
      else
        break;
    }
    if (num1 == 0)
      return "";
    int num3 = Random.Range(0, num1);
    int startIndex2 = 0;
    for (int index = 0; index < num3; ++index)
      startIndex2 = source.IndexOf(separator, startIndex2) + separator.Length;
    int num4 = source.IndexOf(separator, startIndex2);
    return source.Substring(startIndex2, num4 == -1 ? source.Length - startIndex2 : num4 - startIndex2);
  }

  public static string GetFormattedDiseaseName(byte idx, bool color = false)
  {
    Klei.AI.Disease disease = Db.Get().Diseases[(int) idx];
    return color ? string.Format((string) UI.OVERLAYS.DISEASE.DISEASE_NAME_FORMAT, (object) disease.Name, (object) GameUtil.ColourToHex(GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName))) : string.Format((string) UI.OVERLAYS.DISEASE.DISEASE_NAME_FORMAT_NO_COLOR, (object) disease.Name);
  }

  public static string GetFormattedDisease(byte idx, int units, bool color = false)
  {
    if (idx == byte.MaxValue || units <= 0)
      return (string) UI.OVERLAYS.DISEASE.NO_DISEASE;
    Klei.AI.Disease disease = Db.Get().Diseases[(int) idx];
    return color ? string.Format((string) UI.OVERLAYS.DISEASE.DISEASE_FORMAT, (object) disease.Name, (object) GameUtil.GetFormattedDiseaseAmount(units), (object) GameUtil.ColourToHex(GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName))) : string.Format((string) UI.OVERLAYS.DISEASE.DISEASE_FORMAT_NO_COLOR, (object) disease.Name, (object) GameUtil.GetFormattedDiseaseAmount(units));
  }

  public static string GetFormattedDiseaseAmount(int units, GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None)
  {
    double num = (double) GameUtil.ApplyTimeSlice(units, timeSlice);
    return GameUtil.AddTimeSliceText(units.ToString("#,##0") + (string) UI.UNITSUFFIXES.DISEASE.UNITS, timeSlice);
  }

  public static string ColourizeString(Color32 colour, string str) => string.Format("<color=#{0}>{1}</color>", (object) GameUtil.ColourToHex(colour), (object) str);

  public static string ColourToHex(Color32 colour) => string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (object) colour.r, (object) colour.g, (object) colour.b, (object) colour.a);

  public static string GetFormattedDecor(float value, bool enforce_max = false)
  {
    string str = "";
    LocString format = (double) value > (double) DecorMonitor.MAXIMUM_DECOR_VALUE & enforce_max ? UI.OVERLAYS.DECOR.MAXIMUM_DECOR : UI.OVERLAYS.DECOR.VALUE;
    if (enforce_max)
      value = Math.Min(value, DecorMonitor.MAXIMUM_DECOR_VALUE);
    if ((double) value > 0.0)
      str = "+";
    else if ((double) value >= 0.0)
      format = UI.OVERLAYS.DECOR.VALUE_ZERO;
    return string.Format((string) format, (object) str, (object) value);
  }

  public static Color GetDecorColourFromValue(int decor)
  {
    Color black = Color.black;
    float num = (float) decor / 100f;
    return (double) num <= 0.0 ? Color.Lerp(new Color(0.15f, 0.0f, 0.0f), new Color(1f, 0.0f, 0.0f), Mathf.Abs(num)) : Color.Lerp(new Color(0.15f, 0.0f, 0.0f), new Color(0.0f, 1f, 0.0f), Mathf.Abs(num));
  }

  public static List<Descriptor> GetMaterialDescriptors(Element element)
  {
    List<Descriptor> materialDescriptors = new List<Descriptor>();
    if (element.attributeModifiers.Count > 0)
    {
      foreach (AttributeModifier attributeModifier in element.attributeModifiers)
      {
        string str1 = string.Format(StringEntry.op_Implicit(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier.AttributeId.ToUpper()))), (object) attributeModifier.GetFormattedString());
        string str2 = string.Format(StringEntry.op_Implicit(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier.AttributeId.ToUpper()))), (object) attributeModifier.GetFormattedString());
        Descriptor descriptor = new Descriptor();
        ((Descriptor) ref descriptor).SetupDescriptor(str1, str2, (Descriptor.DescriptorType) 1);
        ((Descriptor) ref descriptor).IncreaseIndent();
        materialDescriptors.Add(descriptor);
      }
    }
    materialDescriptors.AddRange((IEnumerable<Descriptor>) GameUtil.GetSignificantMaterialPropertyDescriptors(element));
    return materialDescriptors;
  }

  public static string GetMaterialTooltips(Element element)
  {
    string str = element.tag.ProperName();
    foreach (AttributeModifier attributeModifier in element.attributeModifiers)
    {
      string name = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
      string formattedString = attributeModifier.GetFormattedString();
      str = str + "\n    • " + string.Format((string) DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, (object) name, (object) formattedString);
    }
    return str + GameUtil.GetSignificantMaterialPropertyTooltips(element);
  }

  public static string GetSignificantMaterialPropertyTooltips(Element element)
  {
    string propertyTooltips = "";
    List<Descriptor> propertyDescriptors = GameUtil.GetSignificantMaterialPropertyDescriptors(element);
    if (propertyDescriptors.Count > 0)
    {
      propertyTooltips += "\n";
      for (int index = 0; index < propertyDescriptors.Count; ++index)
        propertyTooltips = propertyTooltips + "    • " + Util.StripTextFormatting(propertyDescriptors[index].text) + "\n";
    }
    return propertyTooltips;
  }

  public static List<Descriptor> GetSignificantMaterialPropertyDescriptors(Element element)
  {
    List<Descriptor> propertyDescriptors = new List<Descriptor>();
    if ((double) element.thermalConductivity > 10.0)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) ELEMENTS.MATERIAL_MODIFIERS.HIGH_THERMAL_CONDUCTIVITY, (object) GameUtil.GetThermalConductivityString(element, false, false)), string.Format((string) ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.HIGH_THERMAL_CONDUCTIVITY, (object) element.name, (object) element.thermalConductivity.ToString("0.#####")), (Descriptor.DescriptorType) 1);
      ((Descriptor) ref descriptor).IncreaseIndent();
      propertyDescriptors.Add(descriptor);
    }
    if ((double) element.thermalConductivity < 1.0)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) ELEMENTS.MATERIAL_MODIFIERS.LOW_THERMAL_CONDUCTIVITY, (object) GameUtil.GetThermalConductivityString(element, false, false)), string.Format((string) ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.LOW_THERMAL_CONDUCTIVITY, (object) element.name, (object) element.thermalConductivity.ToString("0.#####")), (Descriptor.DescriptorType) 1);
      ((Descriptor) ref descriptor).IncreaseIndent();
      propertyDescriptors.Add(descriptor);
    }
    if ((double) element.specificHeatCapacity <= 0.20000000298023224)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) ELEMENTS.MATERIAL_MODIFIERS.LOW_SPECIFIC_HEAT_CAPACITY, string.Format((string) ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.LOW_SPECIFIC_HEAT_CAPACITY, (object) element.name, (object) (float) ((double) element.specificHeatCapacity * 1.0)), (Descriptor.DescriptorType) 1);
      ((Descriptor) ref descriptor).IncreaseIndent();
      propertyDescriptors.Add(descriptor);
    }
    if ((double) element.specificHeatCapacity >= 1.0)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) ELEMENTS.MATERIAL_MODIFIERS.HIGH_SPECIFIC_HEAT_CAPACITY, string.Format((string) ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.HIGH_SPECIFIC_HEAT_CAPACITY, (object) element.name, (object) (float) ((double) element.specificHeatCapacity * 1.0)), (Descriptor.DescriptorType) 1);
      ((Descriptor) ref descriptor).IncreaseIndent();
      propertyDescriptors.Add(descriptor);
    }
    if (Sim.IsRadiationEnabled() && (double) element.radiationAbsorptionFactor >= 0.800000011920929)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) ELEMENTS.MATERIAL_MODIFIERS.EXCELLENT_RADIATION_SHIELD, string.Format((string) ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.EXCELLENT_RADIATION_SHIELD, (object) element.name, (object) element.radiationAbsorptionFactor), (Descriptor.DescriptorType) 1);
      ((Descriptor) ref descriptor).IncreaseIndent();
      propertyDescriptors.Add(descriptor);
    }
    return propertyDescriptors;
  }

  public static int NaturalBuildingCell(this KMonoBehaviour cmp) => Grid.PosToCell(TransformExtensions.GetPosition(cmp.transform));

  public static List<Descriptor> GetMaterialDescriptors(Tag tag)
  {
    List<Descriptor> materialDescriptors = new List<Descriptor>();
    Element element = ElementLoader.GetElement(tag);
    if (element != null)
    {
      if (element.attributeModifiers.Count > 0)
      {
        foreach (AttributeModifier attributeModifier in element.attributeModifiers)
        {
          string str1 = string.Format(StringEntry.op_Implicit(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + attributeModifier.AttributeId.ToUpper()))), (object) attributeModifier.GetFormattedString());
          string str2 = string.Format(StringEntry.op_Implicit(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + attributeModifier.AttributeId.ToUpper()))), (object) attributeModifier.GetFormattedString());
          Descriptor descriptor = new Descriptor();
          ((Descriptor) ref descriptor).SetupDescriptor(str1, str2, (Descriptor.DescriptorType) 1);
          ((Descriptor) ref descriptor).IncreaseIndent();
          materialDescriptors.Add(descriptor);
        }
      }
      materialDescriptors.AddRange((IEnumerable<Descriptor>) GameUtil.GetSignificantMaterialPropertyDescriptors(element));
    }
    else
    {
      GameObject prefab = Assets.TryGetPrefab(tag);
      if (Object.op_Inequality((Object) prefab, (Object) null))
      {
        PrefabAttributeModifiers component = prefab.GetComponent<PrefabAttributeModifiers>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          foreach (AttributeModifier descriptor1 in component.descriptors)
          {
            string str3 = string.Format(StringEntry.op_Implicit(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS." + descriptor1.AttributeId.ToUpper()))), (object) descriptor1.GetFormattedString());
            string str4 = string.Format(StringEntry.op_Implicit(Strings.Get(new StringKey("STRINGS.ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP." + descriptor1.AttributeId.ToUpper()))), (object) descriptor1.GetFormattedString());
            Descriptor descriptor2 = new Descriptor();
            ((Descriptor) ref descriptor2).SetupDescriptor(str3, str4, (Descriptor.DescriptorType) 1);
            ((Descriptor) ref descriptor2).IncreaseIndent();
            materialDescriptors.Add(descriptor2);
          }
        }
      }
    }
    return materialDescriptors;
  }

  public static string GetMaterialTooltips(Tag tag)
  {
    string materialTooltips = tag.ProperName();
    Element element = ElementLoader.GetElement(tag);
    if (element != null)
    {
      foreach (AttributeModifier attributeModifier in element.attributeModifiers)
      {
        string name = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId).Name;
        string formattedString = attributeModifier.GetFormattedString();
        materialTooltips = materialTooltips + "\n    • " + string.Format((string) DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, (object) name, (object) formattedString);
      }
      materialTooltips += GameUtil.GetSignificantMaterialPropertyTooltips(element);
    }
    else
    {
      GameObject prefab = Assets.TryGetPrefab(tag);
      if (Object.op_Inequality((Object) prefab, (Object) null))
      {
        PrefabAttributeModifiers component = prefab.GetComponent<PrefabAttributeModifiers>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          foreach (AttributeModifier descriptor in component.descriptors)
          {
            string name = Db.Get().BuildingAttributes.Get(descriptor.AttributeId).Name;
            string formattedString = descriptor.GetFormattedString();
            materialTooltips = materialTooltips + "\n    • " + string.Format((string) DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, (object) name, (object) formattedString);
          }
        }
      }
    }
    return materialTooltips;
  }

  public static bool AreChoresUIMergeable(
    Chore.Precondition.Context choreA,
    Chore.Precondition.Context choreB)
  {
    if (choreA.chore.target.isNull || choreB.chore.target.isNull)
      return false;
    ChoreType choreType1 = choreB.chore.choreType;
    ChoreType choreType2 = choreA.chore.choreType;
    return choreA.chore.choreType == choreB.chore.choreType && Tag.op_Equality(choreA.chore.target.GetComponent<KPrefabID>().PrefabTag, choreB.chore.target.GetComponent<KPrefabID>().PrefabTag) || choreA.chore.choreType == Db.Get().ChoreTypes.Dig && choreB.chore.choreType == Db.Get().ChoreTypes.Dig || choreA.chore.choreType == Db.Get().ChoreTypes.Relax && choreB.chore.choreType == Db.Get().ChoreTypes.Relax || (choreType2 == Db.Get().ChoreTypes.ReturnSuitIdle || choreType2 == Db.Get().ChoreTypes.ReturnSuitUrgent) && (choreType1 == Db.Get().ChoreTypes.ReturnSuitIdle || choreType1 == Db.Get().ChoreTypes.ReturnSuitUrgent) || Object.op_Equality((Object) choreA.chore.target.gameObject, (Object) choreB.chore.target.gameObject) && choreA.chore.choreType == choreB.chore.choreType;
  }

  public static string GetChoreName(Chore chore, object choreData)
  {
    string choreName = "";
    if (chore.choreType == Db.Get().ChoreTypes.Fetch || chore.choreType == Db.Get().ChoreTypes.MachineFetch || chore.choreType == Db.Get().ChoreTypes.FabricateFetch || chore.choreType == Db.Get().ChoreTypes.FetchCritical || chore.choreType == Db.Get().ChoreTypes.PowerFetch)
      choreName = chore.GetReportName(chore.gameObject.GetProperName());
    else if (chore.choreType == Db.Get().ChoreTypes.StorageFetch || chore.choreType == Db.Get().ChoreTypes.FoodFetch)
    {
      FetchChore fetchChore = chore as FetchChore;
      if (chore is FetchAreaChore fetchAreaChore)
      {
        GameObject getFetchTarget = fetchAreaChore.GetFetchTarget;
        KMonoBehaviour cmp = choreData as KMonoBehaviour;
        choreName = !Object.op_Inequality((Object) getFetchTarget, (Object) null) ? (!Object.op_Inequality((Object) cmp, (Object) null) ? chore.GetReportName() : chore.GetReportName(((Component) cmp).GetProperName())) : chore.GetReportName(getFetchTarget.GetProperName());
      }
      else if (fetchChore != null)
      {
        Pickupable fetchTarget = fetchChore.fetchTarget;
        KMonoBehaviour cmp = choreData as KMonoBehaviour;
        choreName = !Object.op_Inequality((Object) fetchTarget, (Object) null) ? (!Object.op_Inequality((Object) cmp, (Object) null) ? chore.GetReportName() : chore.GetReportName(((Component) cmp).GetProperName())) : chore.GetReportName(((Component) fetchTarget).GetProperName());
      }
    }
    else
      choreName = chore.GetReportName();
    return choreName;
  }

  public static string ChoreGroupsForChoreType(ChoreType choreType)
  {
    if (choreType.groups == null || choreType.groups.Length == 0)
      return (string) null;
    string str = "";
    for (int index = 0; index < choreType.groups.Length; ++index)
    {
      if (index != 0)
        str += (string) UI.UISIDESCREENS.MINIONTODOSIDESCREEN.CHORE_GROUP_SEPARATOR;
      str += choreType.groups[index].Name;
    }
    return str;
  }

  public static bool IsCapturingTimeLapse() => Object.op_Inequality((Object) Game.Instance, (Object) null) && Object.op_Inequality((Object) Game.Instance.timelapser, (Object) null) && Game.Instance.timelapser.CapturingTimelapseScreenshot;

  public static ExposureType GetExposureTypeForDisease(Klei.AI.Disease disease)
  {
    for (int index = 0; index < GERM_EXPOSURE.TYPES.Length; ++index)
    {
      if (HashedString.op_Equality(disease.id, HashedString.op_Implicit(GERM_EXPOSURE.TYPES[index].germ_id)))
        return GERM_EXPOSURE.TYPES[index];
    }
    return (ExposureType) null;
  }

  public static Sickness GetSicknessForDisease(Klei.AI.Disease disease)
  {
    for (int index = 0; index < GERM_EXPOSURE.TYPES.Length; ++index)
    {
      if (HashedString.op_Equality(disease.id, HashedString.op_Implicit(GERM_EXPOSURE.TYPES[index].germ_id)))
        return GERM_EXPOSURE.TYPES[index].sickness_id == null ? (Sickness) null : Db.Get().Sicknesses.Get(GERM_EXPOSURE.TYPES[index].sickness_id);
    }
    return (Sickness) null;
  }

  public static void SubscribeToTags<T>(
    T target,
    EventSystem.IntraObjectHandler<T> handler,
    bool triggerImmediately)
    where T : KMonoBehaviour
  {
    if (triggerImmediately)
      ((EventSystem.IntraObjectHandlerBase) handler).Trigger(((Component) (object) target).gameObject, (object) new TagChangedEventData(Tag.Invalid, false));
    ((KMonoBehaviour) (object) target).Subscribe<T>(-1582839653, handler);
  }

  public static void UnsubscribeToTags<T>(T target, EventSystem.IntraObjectHandler<T> handler) where T : KMonoBehaviour => ((KMonoBehaviour) (object) target).Unsubscribe<T>(-1582839653, handler, false);

  public static EventSystem.IntraObjectHandler<T> CreateHasTagHandler<T>(
    Tag tag,
    Action<T, object> callback)
    where T : KMonoBehaviour
  {
    return new EventSystem.IntraObjectHandler<T>((Action<T, object>) ((component, data) =>
    {
      TagChangedEventData changedEventData = (TagChangedEventData) data;
      if (Tag.op_Equality(changedEventData.tag, Tag.Invalid))
      {
        KPrefabID component1 = ((Component) (object) component).GetComponent<KPrefabID>();
        // ISSUE: explicit constructor call
        ((TagChangedEventData) ref changedEventData).\u002Ector(tag, component1.HasTag(tag));
      }
      if (!Tag.op_Equality(changedEventData.tag, tag) || !changedEventData.added)
        return;
      callback(component, data);
    }));
  }

  static GameUtil()
  {
    string[] strArray = new string[10]
    {
      "BasicPlantFood",
      "MushBar",
      "ColdWheatSeed",
      "ColdWheatSeed",
      "SpiceNut",
      "PrickleFruit",
      "Meat",
      "Mushroom",
      "ColdWheat",
      null
    };
    Tag compostable = GameTags.Compostable;
    strArray[9] = ((Tag) ref compostable).Name;
    GameUtil.foodTags = new TagSet(strArray);
    GameUtil.solidTags = new TagSet(new string[5]
    {
      "Filter",
      "Coal",
      "BasicFabric",
      "SwampLilyFlower",
      "RefinedMetal"
    });
  }

  public enum UnitClass
  {
    SimpleFloat,
    SimpleInteger,
    Temperature,
    Mass,
    Calories,
    Percent,
    Distance,
    Disease,
    Radiation,
    Energy,
    Power,
    Lux,
    Time,
    Seconds,
    Cycles,
  }

  public enum TemperatureUnit
  {
    Celsius,
    Fahrenheit,
    Kelvin,
  }

  public enum MassUnit
  {
    Kilograms,
    Pounds,
  }

  public enum MetricMassFormat
  {
    UseThreshold,
    Kilogram,
    Gram,
    Tonne,
  }

  public enum TemperatureInterpretation
  {
    Absolute,
    Relative,
  }

  public enum TimeSlice
  {
    None,
    ModifyOnly,
    PerSecond,
    PerCycle,
  }

  public enum MeasureUnit
  {
    mass,
    kcal,
    quantity,
  }

  public enum IdentityDescriptorTense
  {
    Normal,
    Possessive,
    Plural,
  }

  public enum WattageFormatterUnit
  {
    Watts,
    Kilowatts,
    Automatic,
  }

  public enum HeatEnergyFormatterUnit
  {
    DTU_S,
    KDTU_S,
    Automatic,
  }

  public struct FloodFillInfo
  {
    public int cell;
    public int depth;
  }

  public static class Hardness
  {
    public const int VERY_SOFT = 0;
    public const int SOFT = 10;
    public const int FIRM = 25;
    public const int VERY_FIRM = 50;
    public const int NEARLY_IMPENETRABLE = 150;
    public const int SUPER_DUPER_HARD = 200;
    public const int RADIOACTIVE_MATERIALS = 251;
    public const int IMPENETRABLE = 255;
    public static Color ImpenetrableColor = new Color(0.831372559f, 0.286274523f, 0.282352954f);
    public static Color nearlyImpenetrableColor = new Color(0.7411765f, 0.349019617f, 0.498039216f);
    public static Color veryFirmColor = new Color(0.6392157f, 0.392156869f, 0.6039216f);
    public static Color firmColor = new Color(0.5254902f, 0.419607848f, 0.647058845f);
    public static Color softColor = new Color(0.427450985f, 0.482352942f, 0.75686276f);
    public static Color verySoftColor = new Color(0.443137258f, 0.670588255f, 0.8117647f);
  }

  public static class GermResistanceValues
  {
    public const float MEDIUM = 2f;
    public const float LARGE = 5f;
    public static Color NegativeLargeColor = new Color(0.831372559f, 0.286274523f, 0.282352954f);
    public static Color NegativeMediumColor = new Color(0.7411765f, 0.349019617f, 0.498039216f);
    public static Color NegativeSmallColor = new Color(0.6392157f, 0.392156869f, 0.6039216f);
    public static Color PositiveSmallColor = new Color(0.5254902f, 0.419607848f, 0.647058845f);
    public static Color PositiveMediumColor = new Color(0.427450985f, 0.482352942f, 0.75686276f);
    public static Color PositiveLargeColor = new Color(0.443137258f, 0.670588255f, 0.8117647f);
  }

  public static class ThermalConductivityValues
  {
    public const float VERY_HIGH = 50f;
    public const float HIGH = 10f;
    public const float MEDIUM = 2f;
    public const float LOW = 1f;
    public static Color veryLowConductivityColor = new Color(0.831372559f, 0.286274523f, 0.282352954f);
    public static Color lowConductivityColor = new Color(0.7411765f, 0.349019617f, 0.498039216f);
    public static Color mediumConductivityColor = new Color(0.6392157f, 0.392156869f, 0.6039216f);
    public static Color highConductivityColor = new Color(0.5254902f, 0.419607848f, 0.647058845f);
    public static Color veryHighConductivityColor = new Color(0.427450985f, 0.482352942f, 0.75686276f);
  }

  public static class BreathableValues
  {
    public static Color positiveColor = new Color(0.443137258f, 0.670588255f, 0.8117647f);
    public static Color warningColor = new Color(0.6392157f, 0.392156869f, 0.6039216f);
    public static Color negativeColor = new Color(0.831372559f, 0.286274523f, 0.282352954f);
  }

  public static class WireLoadValues
  {
    public static Color warningColor = new Color(0.9843137f, 0.6901961f, 0.23137255f);
    public static Color negativeColor = new Color(1f, 0.192156866f, 0.192156866f);
  }
}
