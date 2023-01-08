// Decompiled with JetBrains decompiler
// Type: NewGameSettingWidget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.CustomSettings;
using UnityEngine;
using UnityEngine.UI;

public abstract class NewGameSettingWidget : KMonoBehaviour
{
  [SerializeField]
  private Image BG;
  [SerializeField]
  private Color enabledColor;
  [SerializeField]
  private Color disabledColor;
  private SettingConfig config;
  private NewGameSettingsPanel panel;
  private string disabledDefault;
  private bool widget_enabled = true;

  protected virtual void Initialize(
    SettingConfig config,
    NewGameSettingsPanel panel,
    string disabledDefault)
  {
    this.config = config;
    this.panel = panel;
    this.disabledDefault = disabledDefault;
  }

  public virtual void Refresh()
  {
    bool flag = this.ShouldBeEnabled();
    if (flag == this.widget_enabled)
      return;
    this.widget_enabled = flag;
    if (this.IsEnabled())
    {
      ((Graphic) this.BG).color = this.enabledColor;
      CustomGameSettings.Instance.SetQualitySetting(this.config, this.config.GetDefaultLevelId());
    }
    else
    {
      CustomGameSettings.Instance.SetQualitySetting(this.config, this.disabledDefault);
      ((Graphic) this.BG).color = this.disabledColor;
    }
  }

  protected void RefreshAll() => this.panel.Refresh();

  protected bool IsEnabled() => this.widget_enabled;

  private bool ShouldBeEnabled() => true;
}
