// Decompiled with JetBrains decompiler
// Type: CustomGameSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.CustomSettings;
using KSerialization;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/CustomGameSettings")]
public class CustomGameSettings : KMonoBehaviour
{
  private static CustomGameSettings instance;
  private const int NUM_STORY_LEVELS = 3;
  public const string STORY_DISABLED_LEVEL = "Disabled";
  public const string STORY_GUARANTEED_LEVEL = "Guaranteed";
  [Serialize]
  public bool is_custom_game;
  [Serialize]
  public CustomGameSettings.CustomGameMode customGameMode;
  [Serialize]
  private Dictionary<string, string> CurrentQualityLevelsBySetting = new Dictionary<string, string>();
  private Dictionary<string, string> currentStoryLevelsBySetting = new Dictionary<string, string>();
  public Dictionary<string, SettingConfig> QualitySettings = new Dictionary<string, SettingConfig>();
  public Dictionary<string, SettingConfig> StorySettings = new Dictionary<string, SettingConfig>();
  private const string storyCoordinatePattern = "(.*)-(\\d*)-(.*)-(.*)";
  private const string noStoryCoordinatePattern = "(.*)-(\\d*)-(.*)";
  private string hexChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

  public static CustomGameSettings Instance => CustomGameSettings.instance;

  public IReadOnlyDictionary<string, string> CurrentStoryLevelsBySetting => (IReadOnlyDictionary<string, string>) this.currentStoryLevelsBySetting;

  public event Action<SettingConfig, SettingLevel> OnQualitySettingChanged;

  public event Action<SettingConfig, SettingLevel> OnStorySettingChanged;

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 6))
      this.customGameMode = this.is_custom_game ? CustomGameSettings.CustomGameMode.Custom : CustomGameSettings.CustomGameMode.Survival;
    if (this.CurrentQualityLevelsBySetting.ContainsKey("CarePackages "))
    {
      if (!this.CurrentQualityLevelsBySetting.ContainsKey(CustomGameSettingConfigs.CarePackages.id))
        this.CurrentQualityLevelsBySetting.Add(CustomGameSettingConfigs.CarePackages.id, this.CurrentQualityLevelsBySetting["CarePackages "]);
      this.CurrentQualityLevelsBySetting.Remove("CarePackages ");
    }
    this.CurrentQualityLevelsBySetting.Remove("Expansion1Active");
    if (!DlcManager.IsExpansion1Active())
    {
      foreach (KeyValuePair<string, SettingConfig> qualitySetting in this.QualitySettings)
      {
        SettingConfig config = qualitySetting.Value;
        if (!DlcManager.IsVanillaId(config.required_content))
        {
          Debug.Assert(config.required_content == "EXPANSION1_ID", (object) "A new expansion setting has been added, but its deserialization has not been implemented.");
          if (this.CurrentQualityLevelsBySetting.ContainsKey(config.id))
            Debug.Assert(this.CurrentQualityLevelsBySetting[config.id] == config.missing_content_default, (object) string.Format("This save has Expansion1 content disabled, but its expansion1-dependent setting {0} is set to {1}", (object) config.id, (object) this.CurrentQualityLevelsBySetting[config.id]));
          else
            this.SetQualitySetting(config, config.missing_content_default);
        }
      }
    }
    string clusterDefaultName;
    this.CurrentQualityLevelsBySetting.TryGetValue(CustomGameSettingConfigs.ClusterLayout.id, out clusterDefaultName);
    if (Util.IsNullOrWhiteSpace(clusterDefaultName))
    {
      DebugUtil.DevAssert(!DlcManager.IsExpansion1Active(), "Deserializing CustomGameSettings.ClusterLayout: ClusterLayout is blank, using default cluster instead", (Object) null);
      clusterDefaultName = WorldGenSettings.ClusterDefaultName;
      this.SetQualitySetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout, clusterDefaultName);
    }
    if (!SettingsCache.clusterLayouts.clusterCache.ContainsKey(clusterDefaultName))
    {
      Debug.Log((object) ("Deserializing CustomGameSettings.ClusterLayout: '" + clusterDefaultName + "' doesn't exist in the clusterCache, trying to rewrite path to scoped path."));
      string key = SettingsCache.GetScope("EXPANSION1_ID") + clusterDefaultName;
      if (SettingsCache.clusterLayouts.clusterCache.ContainsKey(key))
      {
        Debug.Log((object) ("Deserializing CustomGameSettings.ClusterLayout: Success in rewriting ClusterLayout '" + clusterDefaultName + "' to '" + key + "'"));
        this.SetQualitySetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout, key);
      }
      else
      {
        Debug.LogWarning((object) ("Deserializing CustomGameSettings.ClusterLayout: Failed to find cluster '" + clusterDefaultName + "' including the scoped path, setting to default cluster name."));
        Debug.Log((object) ("ClusterCache: " + string.Join(",", (IEnumerable<string>) SettingsCache.clusterLayouts.clusterCache.Keys)));
        this.SetQualitySetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout, WorldGenSettings.ClusterDefaultName);
      }
    }
    this.CheckCustomGameMode();
  }

  protected virtual void OnPrefabInit()
  {
    int num1 = DlcManager.IsExpansion1Active() ? 1 : 0;
    CustomGameSettings.instance = this;
    this.AddQualitySettingConfig((SettingConfig) CustomGameSettingConfigs.ClusterLayout);
    this.AddQualitySettingConfig((SettingConfig) CustomGameSettingConfigs.WorldgenSeed);
    this.AddQualitySettingConfig(CustomGameSettingConfigs.ImmuneSystem);
    this.AddQualitySettingConfig(CustomGameSettingConfigs.CalorieBurn);
    this.AddQualitySettingConfig(CustomGameSettingConfigs.Morale);
    this.AddQualitySettingConfig(CustomGameSettingConfigs.Durability);
    if (num1 != 0)
      this.AddQualitySettingConfig(CustomGameSettingConfigs.Radiation);
    this.AddQualitySettingConfig(CustomGameSettingConfigs.Stress);
    this.AddQualitySettingConfig(CustomGameSettingConfigs.StressBreaks);
    this.AddQualitySettingConfig(CustomGameSettingConfigs.CarePackages);
    this.AddQualitySettingConfig(CustomGameSettingConfigs.SandboxMode);
    this.AddQualitySettingConfig(CustomGameSettingConfigs.FastWorkersMode);
    if (SaveLoader.GetCloudSavesAvailable())
      this.AddQualitySettingConfig(CustomGameSettingConfigs.SaveToCloud);
    if (num1 != 0)
      this.AddQualitySettingConfig(CustomGameSettingConfigs.Teleporters);
    foreach (Story resource in Db.Get().Stories.resources)
    {
      long num2 = (long) Util.IntPow(3, resource.kleiUseOnlyCoordinateOffset);
      string id = resource.Id;
      List<SettingLevel> levels = new List<SettingLevel>();
      levels.Add(new SettingLevel("Disabled", "", ""));
      levels.Add(new SettingLevel("Guaranteed", "", "", 1L));
      long coordinate_dimension = num2;
      this.AddStorySettingConfig((SettingConfig) new ListSettingConfig(id, "", "", levels, "Disabled", "Disabled", coordinate_dimension, 3L, triggers_custom_game: false));
    }
    this.VerifySettingCoordinates();
  }

  public void SetSurvivalDefaults()
  {
    this.customGameMode = CustomGameSettings.CustomGameMode.Survival;
    foreach (KeyValuePair<string, SettingConfig> qualitySetting in this.QualitySettings)
      this.SetQualitySetting(qualitySetting.Value, qualitySetting.Value.GetDefaultLevelId());
  }

  public void SetNosweatDefaults()
  {
    this.customGameMode = CustomGameSettings.CustomGameMode.Nosweat;
    foreach (KeyValuePair<string, SettingConfig> qualitySetting in this.QualitySettings)
      this.SetQualitySetting(qualitySetting.Value, qualitySetting.Value.GetNoSweatDefaultLevelId());
  }

  public SettingLevel CycleSettingLevel(ListSettingConfig config, int direction)
  {
    this.SetQualitySetting((SettingConfig) config, config.CycleSettingLevelID(this.CurrentQualityLevelsBySetting[config.id], direction));
    return config.GetLevel(this.CurrentQualityLevelsBySetting[config.id]);
  }

  public SettingLevel ToggleSettingLevel(ToggleSettingConfig config)
  {
    this.SetQualitySetting((SettingConfig) config, config.ToggleSettingLevelID(this.CurrentQualityLevelsBySetting[config.id]));
    return config.GetLevel(this.CurrentQualityLevelsBySetting[config.id]);
  }

  public void SetQualitySetting(SettingConfig config, string value)
  {
    this.CurrentQualityLevelsBySetting[config.id] = value;
    this.CheckCustomGameMode();
    if (this.OnQualitySettingChanged == null)
      return;
    this.OnQualitySettingChanged(config, this.GetCurrentQualitySetting(config));
  }

  private void CheckCustomGameMode()
  {
    bool flag1 = true;
    bool flag2 = true;
    foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
    {
      if (!this.QualitySettings.ContainsKey(keyValuePair.Key))
        DebugUtil.LogWarningArgs(new object[1]
        {
          (object) ("Quality settings missing " + keyValuePair.Key)
        });
      else if (this.QualitySettings[keyValuePair.Key].triggers_custom_game)
      {
        if (keyValuePair.Value != this.QualitySettings[keyValuePair.Key].GetDefaultLevelId())
          flag1 = false;
        if (keyValuePair.Value != this.QualitySettings[keyValuePair.Key].GetNoSweatDefaultLevelId())
          flag2 = false;
        if (!flag1)
        {
          if (!flag2)
            break;
        }
      }
    }
    CustomGameSettings.CustomGameMode customGameMode = !flag1 ? (!flag2 ? CustomGameSettings.CustomGameMode.Custom : CustomGameSettings.CustomGameMode.Nosweat) : CustomGameSettings.CustomGameMode.Survival;
    if (customGameMode == this.customGameMode)
      return;
    DebugUtil.LogArgs(new object[4]
    {
      (object) "Game mode changed from",
      (object) this.customGameMode,
      (object) "to",
      (object) customGameMode
    });
    this.customGameMode = customGameMode;
  }

  public SettingLevel GetCurrentQualitySetting(SettingConfig setting) => this.GetCurrentQualitySetting(setting.id);

  public SettingLevel GetCurrentQualitySetting(string setting_id)
  {
    SettingConfig qualitySetting = this.QualitySettings[setting_id];
    if (this.customGameMode == CustomGameSettings.CustomGameMode.Survival && qualitySetting.triggers_custom_game)
      return qualitySetting.GetLevel(qualitySetting.GetDefaultLevelId());
    if (this.customGameMode == CustomGameSettings.CustomGameMode.Nosweat && qualitySetting.triggers_custom_game)
      return qualitySetting.GetLevel(qualitySetting.GetNoSweatDefaultLevelId());
    if (!this.CurrentQualityLevelsBySetting.ContainsKey(setting_id))
      this.CurrentQualityLevelsBySetting[setting_id] = this.QualitySettings[setting_id].GetDefaultLevelId();
    string level_id = DlcManager.IsContentActive(qualitySetting.required_content) ? this.CurrentQualityLevelsBySetting[setting_id] : qualitySetting.GetDefaultLevelId();
    return this.QualitySettings[setting_id].GetLevel(level_id);
  }

  public string GetCurrentQualitySettingLevelId(SettingConfig config) => this.CurrentQualityLevelsBySetting[config.id];

  public string GetSettingLevelLabel(string setting_id, string level_id)
  {
    SettingConfig qualitySetting = this.QualitySettings[setting_id];
    if (qualitySetting != null)
    {
      SettingLevel level = qualitySetting.GetLevel(level_id);
      if (level != null)
        return level.label;
    }
    Debug.LogWarning((object) ("No label string for setting: " + setting_id + " level: " + level_id));
    return "";
  }

  public string GetQualitySettingLevelTooltip(string setting_id, string level_id)
  {
    SettingConfig qualitySetting = this.QualitySettings[setting_id];
    if (qualitySetting != null)
    {
      SettingLevel level = qualitySetting.GetLevel(level_id);
      if (level != null)
        return level.tooltip;
    }
    Debug.LogWarning((object) ("No tooltip string for setting: " + setting_id + " level: " + level_id));
    return "";
  }

  public void AddQualitySettingConfig(SettingConfig config)
  {
    this.QualitySettings.Add(config.id, config);
    if (this.CurrentQualityLevelsBySetting.ContainsKey(config.id) && !string.IsNullOrEmpty(this.CurrentQualityLevelsBySetting[config.id]))
      return;
    this.CurrentQualityLevelsBySetting[config.id] = config.GetDefaultLevelId();
  }

  public void LoadClusters()
  {
    Dictionary<string, ClusterLayout> clusterCache = SettingsCache.clusterLayouts.clusterCache;
    List<SettingLevel> levels = new List<SettingLevel>(clusterCache.Count);
    foreach (KeyValuePair<string, ClusterLayout> keyValuePair in clusterCache)
    {
      StringEntry stringEntry;
      string label = Strings.TryGet(new StringKey(keyValuePair.Value.name), ref stringEntry) ? ((object) stringEntry).ToString() : keyValuePair.Value.name;
      string tooltip = Strings.TryGet(new StringKey(keyValuePair.Value.description), ref stringEntry) ? ((object) stringEntry).ToString() : keyValuePair.Value.description;
      levels.Add(new SettingLevel(keyValuePair.Key, label, tooltip));
    }
    CustomGameSettingConfigs.ClusterLayout.StompLevels(levels, WorldGenSettings.ClusterDefaultName, WorldGenSettings.ClusterDefaultName);
  }

  public void Print()
  {
    string str1 = "Custom Settings: ";
    foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
      str1 = str1 + keyValuePair.Key + "=" + keyValuePair.Value + ",";
    Debug.Log((object) str1);
    string str2 = "Story Settings: ";
    foreach (KeyValuePair<string, string> keyValuePair in this.currentStoryLevelsBySetting)
      str2 = str2 + keyValuePair.Key + "=" + keyValuePair.Value + ",";
    Debug.Log((object) str2);
  }

  private bool AllValuesMatch(
    Dictionary<string, string> data,
    CustomGameSettings.CustomGameMode mode)
  {
    bool flag = true;
    foreach (KeyValuePair<string, SettingConfig> qualitySetting in this.QualitySettings)
    {
      if (!(qualitySetting.Key == CustomGameSettingConfigs.WorldgenSeed.id))
      {
        string str = (string) null;
        switch (mode)
        {
          case CustomGameSettings.CustomGameMode.Survival:
            str = qualitySetting.Value.GetDefaultLevelId();
            break;
          case CustomGameSettings.CustomGameMode.Nosweat:
            str = qualitySetting.Value.GetNoSweatDefaultLevelId();
            break;
        }
        if (data.ContainsKey(qualitySetting.Key) && data[qualitySetting.Key] != str)
        {
          flag = false;
          break;
        }
      }
    }
    return flag;
  }

  public List<CustomGameSettings.MetricSettingsData> GetSettingsForMetrics()
  {
    List<CustomGameSettings.MetricSettingsData> settingsForMetrics = new List<CustomGameSettings.MetricSettingsData>();
    settingsForMetrics.Add(new CustomGameSettings.MetricSettingsData()
    {
      Name = "CustomGameMode",
      Value = this.customGameMode.ToString()
    });
    foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
      settingsForMetrics.Add(new CustomGameSettings.MetricSettingsData()
      {
        Name = keyValuePair.Key,
        Value = keyValuePair.Value
      });
    CustomGameSettings.MetricSettingsData metricSettingsData = new CustomGameSettings.MetricSettingsData()
    {
      Name = "CustomGameModeActual",
      Value = CustomGameSettings.CustomGameMode.Custom.ToString()
    };
    foreach (CustomGameSettings.CustomGameMode mode in Enum.GetValues(typeof (CustomGameSettings.CustomGameMode)))
    {
      if (mode != CustomGameSettings.CustomGameMode.Custom && this.AllValuesMatch(this.CurrentQualityLevelsBySetting, mode))
      {
        metricSettingsData.Value = mode.ToString();
        break;
      }
    }
    settingsForMetrics.Add(metricSettingsData);
    return settingsForMetrics;
  }

  public bool VerifySettingCoordinates() => this.VerifySettingsDictionary(this.QualitySettings) | this.VerifySettingsDictionary(this.StorySettings);

  private bool VerifySettingsDictionary(Dictionary<string, SettingConfig> configs)
  {
    Dictionary<long, string> dictionary = new Dictionary<long, string>();
    bool flag1 = false;
    foreach (KeyValuePair<string, SettingConfig> config in configs)
    {
      if (config.Value.coordinate_dimension < 0L || config.Value.coordinate_dimension_width < 0L)
      {
        if (config.Value.coordinate_dimension >= 0L || config.Value.coordinate_dimension_width >= 0L)
        {
          flag1 = true;
          Debug.Assert(false, (object) (config.Value.id + ": Both coordinate dimension props must be unset (-1) if either is unset."));
        }
      }
      else
      {
        List<SettingLevel> levels = config.Value.GetLevels();
        if (config.Value.coordinate_dimension_width < (long) levels.Count)
        {
          flag1 = true;
          Debug.Assert(false, (object) (config.Value.id + ": Range between coordinate min and max insufficient for all levels (" + config.Value.coordinate_dimension_width.ToString() + "<" + levels.Count.ToString() + ")"));
        }
        foreach (SettingLevel settingLevel in levels)
        {
          long key = config.Value.coordinate_dimension * settingLevel.coordinate_offset;
          string str1 = config.Value.id + " > " + settingLevel.id;
          if (settingLevel.coordinate_offset < 0L)
          {
            flag1 = true;
            Debug.Assert(false, (object) (str1 + ": Level coordinate offset must be >= 0"));
          }
          else if (settingLevel.coordinate_offset == 0L)
          {
            if (settingLevel.id != config.Value.GetDefaultLevelId())
            {
              flag1 = true;
              Debug.Assert(false, (object) (str1 + ": Only the default level should have a coordinate offset of 0"));
            }
          }
          else if (settingLevel.coordinate_offset > config.Value.coordinate_dimension_width)
          {
            flag1 = true;
            Debug.Assert(false, (object) (str1 + ": level coordinate must be <= dimension width"));
          }
          else
          {
            string str2;
            bool flag2 = !dictionary.TryGetValue(key, out str2);
            dictionary[key] = str1;
            if (settingLevel.id == config.Value.GetDefaultLevelId())
            {
              flag1 = true;
              Debug.Assert(false, (object) (str1 + ": Default level must be coordinate 0"));
            }
            if (!flag2)
            {
              flag1 = true;
              Debug.Assert(false, (object) (str1 + ": Combined coordinate conflicts with another coordinate (" + str2 + "). Ensure this SettingConfig's min and max don't overlap with another SettingConfig's"));
            }
          }
        }
      }
    }
    return flag1;
  }

  public static string[] ParseSettingCoordinate(string coord)
  {
    string[] coordinate = CustomGameSettings.ParseCoordinate(coord, "(.*)-(\\d*)-(.*)-(.*)");
    if (coordinate.Length == 1)
      coordinate = CustomGameSettings.ParseCoordinate(coord, "(.*)-(\\d*)-(.*)");
    return coordinate;
  }

  private static string[] ParseCoordinate(string coord, string pattern)
  {
    Match match = new Regex(pattern).Match(coord);
    string[] coordinate = new string[match.Groups.Count];
    for (int groupnum = 0; groupnum < match.Groups.Count; ++groupnum)
      coordinate[groupnum] = match.Groups[groupnum].Value;
    return coordinate;
  }

  public string GetSettingsCoordinate()
  {
    SettingLevel currentQualitySetting1 = CustomGameSettings.Instance.GetCurrentQualitySetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout);
    if (currentQualitySetting1 == null)
    {
      DebugUtil.DevLogError("GetSettingsCoordinate: clusterLayoutSetting is null, returning '0' coordinate");
      CustomGameSettings.Instance.Print();
      Debug.Log((object) ("ClusterCache: " + string.Join(",", (IEnumerable<string>) SettingsCache.clusterLayouts.clusterCache.Keys)));
      return "0-0-0-0";
    }
    ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(currentQualitySetting1.id);
    SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting((SettingConfig) CustomGameSettingConfigs.WorldgenSeed);
    string otherSettingsCode = this.GetOtherSettingsCode();
    string traitSettingsCode = this.GetStoryTraitSettingsCode();
    return string.Format("{0}-{1}-{2}-{3}", (object) clusterData.GetCoordinatePrefix(), (object) currentQualitySetting2.id, (object) otherSettingsCode, (object) traitSettingsCode);
  }

  public void ParseAndApplySettingsCode(string code)
  {
    long num1 = this.Base36toBase10(code);
    Dictionary<SettingConfig, string> dictionary = new Dictionary<SettingConfig, string>();
    foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
    {
      SettingConfig qualitySetting = this.QualitySettings[keyValuePair.Key];
      if (qualitySetting.coordinate_dimension >= 0L && qualitySetting.coordinate_dimension_width >= 0L)
      {
        long num2 = 0;
        long num3 = qualitySetting.coordinate_dimension * qualitySetting.coordinate_dimension_width;
        long num4 = num1;
        if (num4 >= num3)
        {
          long num5 = num4 / num3 * num3;
          num4 -= num5;
        }
        if (num4 >= qualitySetting.coordinate_dimension)
          num2 = num4 / qualitySetting.coordinate_dimension;
        foreach (SettingLevel level in qualitySetting.GetLevels())
        {
          if (level.coordinate_offset == num2)
          {
            dictionary[qualitySetting] = level.id;
            break;
          }
        }
      }
    }
    foreach (KeyValuePair<SettingConfig, string> keyValuePair in dictionary)
      this.SetQualitySetting(keyValuePair.Key, keyValuePair.Value);
  }

  private string GetOtherSettingsCode()
  {
    long input = 0;
    foreach (KeyValuePair<string, string> keyValuePair in this.CurrentQualityLevelsBySetting)
    {
      SettingConfig settingConfig;
      this.QualitySettings.TryGetValue(keyValuePair.Key, out settingConfig);
      if (settingConfig != null && settingConfig.coordinate_dimension >= 0L && settingConfig.coordinate_dimension_width >= 0L)
      {
        SettingLevel level = settingConfig.GetLevel(keyValuePair.Value);
        long num = settingConfig.coordinate_dimension * level.coordinate_offset;
        input += num;
      }
    }
    return this.Base10toBase36(input);
  }

  private long Base36toBase10(string input)
  {
    if (input == "0")
      return 0;
    long input1 = 0;
    for (int index = input.Length - 1; index >= 0; --index)
      input1 = input1 * 36L + (long) this.hexChars.IndexOf(input[index]);
    DebugUtil.LogArgs(new object[6]
    {
      (object) "tried converting",
      (object) input,
      (object) ", got",
      (object) input1,
      (object) "and returns to",
      (object) this.Base10toBase36(input1)
    });
    return input1;
  }

  private string Base10toBase36(long input)
  {
    if (input == 0L)
      return "0";
    long num = input;
    string str = "";
    for (; num > 0L; num /= 36L)
      str += this.hexChars[(int) (num % 36L)].ToString();
    return str;
  }

  public void AddStorySettingConfig(SettingConfig config)
  {
    this.StorySettings.Add(config.id, config);
    if (this.currentStoryLevelsBySetting.ContainsKey(config.id) && !string.IsNullOrEmpty(this.currentStoryLevelsBySetting[config.id]))
      return;
    this.currentStoryLevelsBySetting[config.id] = config.GetDefaultLevelId();
  }

  public void SetStorySetting(SettingConfig config, string value) => this.SetStorySetting(config, value == "Guaranteed");

  public void SetStorySetting(SettingConfig config, bool value)
  {
    this.currentStoryLevelsBySetting[config.id] = value ? "Guaranteed" : "Disabled";
    if (this.OnStorySettingChanged == null)
      return;
    this.OnStorySettingChanged(config, this.GetCurrentStoryTraitSetting(config));
  }

  public void ParseAndApplyStoryTraitSettingsCode(string code)
  {
    long num1 = this.Base36toBase10(code);
    Dictionary<SettingConfig, string> dictionary = new Dictionary<SettingConfig, string>();
    foreach (KeyValuePair<string, string> keyValuePair in this.currentStoryLevelsBySetting)
    {
      SettingConfig storySetting = this.StorySettings[keyValuePair.Key];
      if (storySetting.coordinate_dimension >= 0L && storySetting.coordinate_dimension_width >= 0L)
      {
        long num2 = 0;
        long num3 = storySetting.coordinate_dimension * storySetting.coordinate_dimension_width;
        long num4 = num1;
        if (num4 >= num3)
        {
          long num5 = num4 / num3 * num3;
          num4 -= num5;
        }
        if (num4 >= storySetting.coordinate_dimension)
          num2 = num4 / storySetting.coordinate_dimension;
        foreach (SettingLevel level in storySetting.GetLevels())
        {
          if (level.coordinate_offset == num2)
          {
            dictionary[storySetting] = level.id;
            break;
          }
        }
      }
    }
    foreach (KeyValuePair<SettingConfig, string> keyValuePair in dictionary)
      this.SetStorySetting(keyValuePair.Key, keyValuePair.Value);
  }

  private string GetStoryTraitSettingsCode()
  {
    long input = 0;
    foreach (KeyValuePair<string, string> keyValuePair in this.currentStoryLevelsBySetting)
    {
      SettingConfig settingConfig;
      this.StorySettings.TryGetValue(keyValuePair.Key, out settingConfig);
      if (settingConfig != null && settingConfig.coordinate_dimension >= 0L && settingConfig.coordinate_dimension_width >= 0L)
      {
        SettingLevel level = settingConfig.GetLevel(keyValuePair.Value);
        long num = settingConfig.coordinate_dimension * level.coordinate_offset;
        input += num;
      }
    }
    return this.Base10toBase36(input);
  }

  public SettingLevel GetCurrentStoryTraitSetting(SettingConfig setting) => this.GetCurrentStoryTraitSetting(setting.id);

  public SettingLevel GetCurrentStoryTraitSetting(string settingId)
  {
    SettingConfig storySetting = this.StorySettings[settingId];
    if (this.customGameMode == CustomGameSettings.CustomGameMode.Survival && storySetting.triggers_custom_game)
      return storySetting.GetLevel(storySetting.GetDefaultLevelId());
    if (this.customGameMode == CustomGameSettings.CustomGameMode.Nosweat && storySetting.triggers_custom_game)
      return storySetting.GetLevel(storySetting.GetNoSweatDefaultLevelId());
    if (!this.currentStoryLevelsBySetting.ContainsKey(settingId))
      this.currentStoryLevelsBySetting[settingId] = this.StorySettings[settingId].GetDefaultLevelId();
    string level_id = DlcManager.IsContentActive(storySetting.required_content) ? this.currentStoryLevelsBySetting[settingId] : storySetting.GetDefaultLevelId();
    return this.StorySettings[settingId].GetLevel(level_id);
  }

  public List<string> GetCurrentStories()
  {
    List<string> currentStories = new List<string>();
    foreach (KeyValuePair<string, string> keyValuePair in this.currentStoryLevelsBySetting)
    {
      if (this.IsStoryActive(keyValuePair.Key, keyValuePair.Value))
        currentStories.Add(keyValuePair.Key);
    }
    return currentStories;
  }

  public bool IsStoryActive(string id, string level)
  {
    SettingConfig settingConfig;
    return this.StorySettings.TryGetValue(id, out settingConfig) && settingConfig != null && settingConfig.coordinate_dimension >= 0L && settingConfig.coordinate_dimension_width >= 0L && level == "Guaranteed";
  }

  public enum CustomGameMode
  {
    Survival = 0,
    Nosweat = 1,
    Custom = 255, // 0x000000FF
  }

  public struct MetricSettingsData
  {
    public string Name;
    public string Value;
  }
}
