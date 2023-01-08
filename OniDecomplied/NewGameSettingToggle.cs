// Decompiled with JetBrains decompiler
// Type: NewGameSettingToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.CustomSettings;
using TMPro;
using UnityEngine;

public class NewGameSettingToggle : NewGameSettingWidget
{
  [SerializeField]
  private LocText Label;
  [SerializeField]
  private ToolTip ToolTip;
  [SerializeField]
  private MultiToggle Toggle;
  [SerializeField]
  private ToolTip ToggleToolTip;
  private ToggleSettingConfig config;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Toggle.onClick += new System.Action(this.ToggleSetting);
  }

  public void Initialize(
    ToggleSettingConfig config,
    NewGameSettingsPanel panel,
    string disabledDefault)
  {
    this.Initialize((SettingConfig) config, panel, disabledDefault);
    this.config = config;
    ((TMP_Text) this.Label).text = config.label;
    this.ToolTip.toolTip = config.tooltip;
  }

  public override void Refresh()
  {
    base.Refresh();
    SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting((SettingConfig) this.config);
    this.Toggle.ChangeState(this.config.IsOnLevel(currentQualitySetting.id) ? 1 : 0);
    this.ToggleToolTip.toolTip = currentQualitySetting.tooltip;
  }

  public void ToggleSetting()
  {
    if (!this.IsEnabled())
      return;
    CustomGameSettings.Instance.ToggleSettingLevel(this.config);
    this.RefreshAll();
  }
}
