// Decompiled with JetBrains decompiler
// Type: NewGameSettingsPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.CustomSettings;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NewGameSettingsPanel")]
public class NewGameSettingsPanel : KMonoBehaviour
{
  [SerializeField]
  private Transform content;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KButton background;
  [Header("Prefab UI Refs")]
  [SerializeField]
  private GameObject prefab_cycle_setting;
  [SerializeField]
  private GameObject prefab_slider_setting;
  [SerializeField]
  private GameObject prefab_checkbox_setting;
  [SerializeField]
  private GameObject prefab_seed_input_setting;
  private CustomGameSettings settings;
  private List<NewGameSettingWidget> widgets;
  public System.Action OnRefresh;

  public void SetCloseAction(System.Action onClose)
  {
    if (Object.op_Inequality((Object) this.closeButton, (Object) null))
      this.closeButton.onClick += onClose;
    if (!Object.op_Inequality((Object) this.background, (Object) null))
      return;
    this.background.onClick += onClose;
  }

  public void Init()
  {
    CustomGameSettings.Instance.LoadClusters();
    Global.Instance.modManager.Report(((Component) this).gameObject);
    this.settings = CustomGameSettings.Instance;
    this.widgets = new List<NewGameSettingWidget>();
    foreach (KeyValuePair<string, SettingConfig> qualitySetting in this.settings.QualitySettings)
    {
      if ((!qualitySetting.Value.debug_only || DebugHandler.enabled) && (!qualitySetting.Value.editor_only || Application.isEditor) && DlcManager.IsContentActive(qualitySetting.Value.required_content))
      {
        if (qualitySetting.Value is ListSettingConfig config3)
        {
          NewGameSettingList newGameSettingList = Util.KInstantiateUI<NewGameSettingList>(this.prefab_cycle_setting, ((Component) this.content).gameObject, true);
          newGameSettingList.Initialize(config3, this, qualitySetting.Value.missing_content_default);
          this.widgets.Add((NewGameSettingWidget) newGameSettingList);
        }
        else if (qualitySetting.Value is ToggleSettingConfig config2)
        {
          NewGameSettingToggle gameSettingToggle = Util.KInstantiateUI<NewGameSettingToggle>(this.prefab_checkbox_setting, ((Component) this.content).gameObject, true);
          gameSettingToggle.Initialize(config2, this, qualitySetting.Value.missing_content_default);
          this.widgets.Add((NewGameSettingWidget) gameSettingToggle);
        }
        else if (qualitySetting.Value is SeedSettingConfig config1)
        {
          NewGameSettingSeed newGameSettingSeed = Util.KInstantiateUI<NewGameSettingSeed>(this.prefab_seed_input_setting, ((Component) this.content).gameObject, true);
          newGameSettingSeed.Initialize(config1);
          this.widgets.Add((NewGameSettingWidget) newGameSettingSeed);
        }
      }
    }
    this.Refresh();
  }

  public void Refresh()
  {
    foreach (NewGameSettingWidget widget in this.widgets)
      widget.Refresh();
    if (this.OnRefresh == null)
      return;
    this.OnRefresh();
  }

  public void ConsumeSettingsCode(string code) => this.settings.ParseAndApplySettingsCode(code);

  public void ConsumeStoryTraitsCode(string code) => this.settings.ParseAndApplyStoryTraitSettingsCode(code);

  public void SetSetting(SettingConfig setting, string level) => this.settings.SetQualitySetting(setting, level);

  public string GetSetting(SettingConfig setting) => this.settings.GetCurrentQualitySetting(setting).id;

  public string GetSetting(string setting) => this.settings.GetCurrentQualitySetting(setting).id;

  public void Cancel()
  {
  }
}
