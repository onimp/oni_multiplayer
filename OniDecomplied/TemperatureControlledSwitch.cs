// Decompiled with JetBrains decompiler
// Type: TemperatureControlledSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig]
public class TemperatureControlledSwitch : CircuitSwitch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
  private HandleVector<int>.Handle structureTemperature;
  private int simUpdateCounter;
  [Serialize]
  public float thresholdTemperature = 280f;
  [Serialize]
  public bool activateOnWarmerThan;
  public float minTemp;
  public float maxTemp = 373.15f;
  private const int NumFrameDelay = 8;
  private float[] temperatures = new float[8];
  private float averageTemp;

  public float StructureTemperature => ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetPayload(this.structureTemperature).Temperature;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.structureTemperature = GameComps.StructureTemperatures.GetHandle(((Component) this).gameObject);
  }

  public void Sim200ms(float dt)
  {
    if (this.simUpdateCounter < 8)
    {
      this.temperatures[this.simUpdateCounter] = Grid.Temperature[Grid.PosToCell((KMonoBehaviour) this)];
      ++this.simUpdateCounter;
    }
    else
    {
      this.simUpdateCounter = 0;
      this.averageTemp = 0.0f;
      for (int index = 0; index < 8; ++index)
        this.averageTemp += this.temperatures[index];
      this.averageTemp /= 8f;
      if (this.activateOnWarmerThan)
      {
        if (((double) this.averageTemp <= (double) this.thresholdTemperature || this.IsSwitchedOn) && ((double) this.averageTemp >= (double) this.thresholdTemperature || !this.IsSwitchedOn))
          return;
        this.Toggle();
      }
      else
      {
        if (((double) this.averageTemp <= (double) this.thresholdTemperature || !this.IsSwitchedOn) && ((double) this.averageTemp >= (double) this.thresholdTemperature || this.IsSwitchedOn))
          return;
        this.Toggle();
      }
    }
  }

  public float GetTemperature() => this.averageTemp;

  public float Threshold
  {
    get => this.thresholdTemperature;
    set => this.thresholdTemperature = value;
  }

  public bool ActivateAboveThreshold
  {
    get => this.activateOnWarmerThan;
    set => this.activateOnWarmerThan = value;
  }

  public float CurrentValue => this.GetTemperature();

  public float RangeMin => this.minTemp;

  public float RangeMax => this.maxTemp;

  public float GetRangeMinInputField() => GameUtil.GetConvertedTemperature(this.RangeMin);

  public float GetRangeMaxInputField() => GameUtil.GetConvertedTemperature(this.RangeMax);

  public LocString Title => UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.TITLE;

  public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;

  public string AboveToolTip => (string) UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_ABOVE;

  public string BelowToolTip => (string) UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_BELOW;

  public string Format(float value, bool units) => GameUtil.GetFormattedTemperature(value, displayUnits: units);

  public float ProcessedSliderValue(float input) => Mathf.Round(input);

  public float ProcessedInputValue(float input) => GameUtil.GetTemperatureConvertedToKelvin(input);

  public LocString ThresholdValueUnits()
  {
    LocString locString = (LocString) null;
    switch (GameUtil.temperatureUnit)
    {
      case GameUtil.TemperatureUnit.Celsius:
        locString = UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
        break;
      case GameUtil.TemperatureUnit.Fahrenheit:
        locString = UI.UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
        break;
      case GameUtil.TemperatureUnit.Kelvin:
        locString = UI.UNITSUFFIXES.TEMPERATURE.KELVIN;
        break;
    }
    return locString;
  }

  public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.InputField;

  public int IncrementScale => 1;

  public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(this.RangeMax);
}
