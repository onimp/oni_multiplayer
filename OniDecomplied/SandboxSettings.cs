// Decompiled with JetBrains decompiler
// Type: SandboxSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public class SandboxSettings
{
  private List<SandboxSettings.Setting<int>> intSettings = new List<SandboxSettings.Setting<int>>();
  private List<SandboxSettings.Setting<float>> floatSettings = new List<SandboxSettings.Setting<float>>();
  private List<SandboxSettings.Setting<string>> stringSettings = new List<SandboxSettings.Setting<string>>();
  public bool InstantBuild = true;
  private bool hasRestoredElement;
  public Action<bool> OnChangeElement;
  public System.Action OnChangeMass;
  public System.Action OnChangeDisease;
  public System.Action OnChangeDiseaseCount;
  public System.Action OnChangeEntity;
  public System.Action OnChangeBrushSize;
  public System.Action OnChangeNoiseScale;
  public System.Action OnChangeNoiseDensity;
  public System.Action OnChangeTemperature;
  public System.Action OnChangeAdditiveTemperature;
  public System.Action OnChangeAdditiveStress;
  public System.Action OnChangeMoraleAdjustment;
  public const string KEY_SELECTED_ENTITY = "SandboxTools.SelectedEntity";
  public const string KEY_SELECTED_ELEMENT = "SandboxTools.SelectedElement";
  public const string KEY_SELECTED_DISEASE = "SandboxTools.SelectedDisease";
  public const string KEY_DISEASE_COUNT = "SandboxTools.DiseaseCount";
  public const string KEY_BRUSH_SIZE = "SandboxTools.BrushSize";
  public const string KEY_NOISE_SCALE = "SandboxTools.NoiseScale";
  public const string KEY_NOISE_DENSITY = "SandboxTools.NoiseDensity";
  public const string KEY_MASS = "SandboxTools.Mass";
  public const string KEY_TEMPERATURE = "SandbosTools.Temperature";
  public const string KEY_TEMPERATURE_ADDITIVE = "SandbosTools.TemperatureAdditive";
  public const string KEY_STRESS_ADDITIVE = "SandbosTools.StressAdditive";
  public const string KEY_MORALE_ADJUSTMENT = "SandbosTools.MoraleAdjustment";

  public void AddIntSetting(string prefsKey, Action<int> setAction, int defaultValue) => this.intSettings.Add(new SandboxSettings.Setting<int>(prefsKey, setAction, defaultValue));

  public int GetIntSetting(string prefsKey) => KPlayerPrefs.GetInt(prefsKey);

  public void SetIntSetting(string prefsKey, int value)
  {
    SandboxSettings.Setting<int> setting = this.intSettings.Find((Predicate<SandboxSettings.Setting<int>>) (match => match.PrefsKey == prefsKey));
    if (setting == null)
      Debug.LogError((object) ("No intSetting named: " + prefsKey + " could be found amongst " + this.intSettings.Count.ToString() + " int settings."));
    setting.Value = value;
  }

  public void RestoreIntSetting(string prefsKey)
  {
    if (KPlayerPrefs.HasKey(prefsKey))
      this.SetIntSetting(prefsKey, this.GetIntSetting(prefsKey));
    else
      this.ForceDefaultIntSetting(prefsKey);
  }

  public void ForceDefaultIntSetting(string prefsKey) => this.SetIntSetting(prefsKey, this.intSettings.Find((Predicate<SandboxSettings.Setting<int>>) (match => match.PrefsKey == prefsKey)).defaultValue);

  public void AddFloatSetting(string prefsKey, Action<float> setAction, float defaultValue) => this.floatSettings.Add(new SandboxSettings.Setting<float>(prefsKey, setAction, defaultValue));

  public float GetFloatSetting(string prefsKey) => KPlayerPrefs.GetFloat(prefsKey);

  public void SetFloatSetting(string prefsKey, float value)
  {
    SandboxSettings.Setting<float> setting = this.floatSettings.Find((Predicate<SandboxSettings.Setting<float>>) (match => match.PrefsKey == prefsKey));
    if (setting == null)
      Debug.LogError((object) ("No KPlayerPrefs float setting named: " + prefsKey + " could be found amongst " + this.floatSettings.Count.ToString() + " float settings."));
    setting.Value = value;
  }

  public void RestoreFloatSetting(string prefsKey)
  {
    if (KPlayerPrefs.HasKey(prefsKey))
      this.SetFloatSetting(prefsKey, this.GetFloatSetting(prefsKey));
    else
      this.ForceDefaultFloatSetting(prefsKey);
  }

  public void ForceDefaultFloatSetting(string prefsKey) => this.SetFloatSetting(prefsKey, this.floatSettings.Find((Predicate<SandboxSettings.Setting<float>>) (match => match.PrefsKey == prefsKey)).defaultValue);

  public void AddStringSetting(string prefsKey, Action<string> setAction, string defaultValue) => this.stringSettings.Add(new SandboxSettings.Setting<string>(prefsKey, setAction, defaultValue));

  public string GetStringSetting(string prefsKey) => KPlayerPrefs.GetString(prefsKey);

  public void SetStringSetting(string prefsKey, string value)
  {
    SandboxSettings.Setting<string> setting = this.stringSettings.Find((Predicate<SandboxSettings.Setting<string>>) (match => match.PrefsKey == prefsKey));
    if (setting == null)
      Debug.LogError((object) ("No KPlayerPrefs string setting named: " + prefsKey + " could be found amongst " + this.stringSettings.Count.ToString() + " settings."));
    setting.Value = value;
  }

  public void RestoreStringSetting(string prefsKey)
  {
    if (KPlayerPrefs.HasKey(prefsKey))
      this.SetStringSetting(prefsKey, this.GetStringSetting(prefsKey));
    else
      this.ForceDefaultStringSetting(prefsKey);
  }

  public void ForceDefaultStringSetting(string prefsKey) => this.SetStringSetting(prefsKey, this.stringSettings.Find((Predicate<SandboxSettings.Setting<string>>) (match => match.PrefsKey == prefsKey)).defaultValue);

  public SandboxSettings()
  {
    this.AddStringSetting("SandboxTools.SelectedEntity", (Action<string>) (data =>
    {
      KPlayerPrefs.SetString("SandboxTools.SelectedEntity", data);
      this.OnChangeEntity();
    }), "MushBar");
    this.AddIntSetting("SandboxTools.SelectedElement", (Action<int>) (data =>
    {
      KPlayerPrefs.SetInt("SandboxTools.SelectedElement", data);
      this.OnChangeElement(this.hasRestoredElement);
      this.hasRestoredElement = true;
    }), (int) ElementLoader.GetElementIndex(SimHashes.Oxygen));
    this.AddStringSetting("SandboxTools.SelectedDisease", (Action<string>) (data =>
    {
      KPlayerPrefs.SetString("SandboxTools.SelectedDisease", data);
      this.OnChangeDisease();
    }), Db.Get().Diseases.FoodGerms.Id);
    this.AddIntSetting("SandboxTools.DiseaseCount", (Action<int>) (val =>
    {
      KPlayerPrefs.SetInt("SandboxTools.DiseaseCount", val);
      this.OnChangeDiseaseCount();
    }), 0);
    this.AddIntSetting("SandboxTools.BrushSize", (Action<int>) (val =>
    {
      KPlayerPrefs.SetInt("SandboxTools.BrushSize", val);
      this.OnChangeBrushSize();
    }), 1);
    this.AddFloatSetting("SandboxTools.NoiseScale", (Action<float>) (val =>
    {
      KPlayerPrefs.SetFloat("SandboxTools.NoiseScale", val);
      this.OnChangeNoiseScale();
    }), 1f);
    this.AddFloatSetting("SandboxTools.NoiseDensity", (Action<float>) (val =>
    {
      KPlayerPrefs.SetFloat("SandboxTools.NoiseDensity", val);
      this.OnChangeNoiseDensity();
    }), 1f);
    this.AddFloatSetting("SandboxTools.Mass", (Action<float>) (val =>
    {
      KPlayerPrefs.SetFloat("SandboxTools.Mass", val);
      this.OnChangeMass();
    }), 1f);
    this.AddFloatSetting("SandbosTools.Temperature", (Action<float>) (val =>
    {
      KPlayerPrefs.SetFloat("SandbosTools.Temperature", val);
      this.OnChangeTemperature();
    }), 300f);
    this.AddFloatSetting("SandbosTools.TemperatureAdditive", (Action<float>) (val =>
    {
      KPlayerPrefs.SetFloat("SandbosTools.TemperatureAdditive", val);
      this.OnChangeAdditiveTemperature();
    }), 5f);
    this.AddFloatSetting("SandbosTools.StressAdditive", (Action<float>) (val =>
    {
      KPlayerPrefs.SetFloat("SandbosTools.StressAdditive", val);
      this.OnChangeAdditiveStress();
    }), 50f);
    this.AddIntSetting("SandbosTools.MoraleAdjustment", (Action<int>) (val =>
    {
      KPlayerPrefs.SetInt("SandbosTools.MoraleAdjustment", val);
      this.OnChangeMoraleAdjustment();
    }), 50);
  }

  public void RestorePrefs()
  {
    foreach (SandboxSettings.Setting<int> intSetting in this.intSettings)
      this.RestoreIntSetting(intSetting.PrefsKey);
    foreach (SandboxSettings.Setting<float> floatSetting in this.floatSettings)
      this.RestoreFloatSetting(floatSetting.PrefsKey);
    foreach (SandboxSettings.Setting<string> stringSetting in this.stringSettings)
      this.RestoreStringSetting(stringSetting.PrefsKey);
  }

  public class Setting<T>
  {
    private string prefsKey;
    private Action<T> SetAction;
    public T defaultValue;

    public Setting(string prefsKey, Action<T> setAction, T defaultValue)
    {
      this.prefsKey = prefsKey;
      this.SetAction = setAction;
      this.defaultValue = defaultValue;
    }

    public string PrefsKey => this.prefsKey;

    public T Value
    {
      set => this.SetAction(value);
    }
  }
}
