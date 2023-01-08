// Decompiled with JetBrains decompiler
// Type: NewGameSettingList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.CustomSettings;
using TMPro;
using UnityEngine;

public class NewGameSettingList : NewGameSettingWidget
{
  [SerializeField]
  private LocText Label;
  [SerializeField]
  private ToolTip ToolTip;
  [SerializeField]
  private LocText ValueLabel;
  [SerializeField]
  private ToolTip ValueToolTip;
  [SerializeField]
  private KButton CycleLeft;
  [SerializeField]
  private KButton CycleRight;
  private ListSettingConfig config;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.CycleLeft.onClick += new System.Action(this.DoCycleLeft);
    this.CycleRight.onClick += new System.Action(this.DoCycleRight);
  }

  public void Initialize(
    ListSettingConfig config,
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
    ((TMP_Text) this.ValueLabel).text = currentQualitySetting.label;
    this.ValueToolTip.toolTip = currentQualitySetting.tooltip;
    this.CycleLeft.isInteractable = !this.config.IsFirstLevel(currentQualitySetting.id);
    this.CycleRight.isInteractable = !this.config.IsLastLevel(currentQualitySetting.id);
  }

  private void DoCycleLeft()
  {
    if (!this.IsEnabled())
      return;
    CustomGameSettings.Instance.CycleSettingLevel(this.config, -1);
    this.RefreshAll();
  }

  private void DoCycleRight()
  {
    if (!this.IsEnabled())
      return;
    CustomGameSettings.Instance.CycleSettingLevel(this.config, 1);
    this.RefreshAll();
  }
}
