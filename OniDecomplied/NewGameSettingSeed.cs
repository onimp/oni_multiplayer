// Decompiled with JetBrains decompiler
// Type: NewGameSettingSeed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.CustomSettings;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NewGameSettingSeed : NewGameSettingWidget
{
  [SerializeField]
  private LocText Label;
  [SerializeField]
  private ToolTip ToolTip;
  [SerializeField]
  private KInputTextField Input;
  [SerializeField]
  private KButton RandomizeButton;
  private const int MAX_VALID_SEED = 2147483647;
  private SeedSettingConfig config;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.Input).onEndEdit).AddListener(new UnityAction<string>((object) this, __methodptr(OnEndEdit)));
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.Input).onValueChanged).AddListener(new UnityAction<string>((object) this, __methodptr(OnValueChanged)));
    this.RandomizeButton.onClick += new System.Action(this.GetNewRandomSeed);
  }

  public void Initialize(SeedSettingConfig config)
  {
    this.config = config;
    ((TMP_Text) this.Label).text = config.label;
    this.ToolTip.toolTip = config.tooltip;
    this.GetNewRandomSeed();
  }

  public override void Refresh() => ((TMP_InputField) this.Input).text = CustomGameSettings.Instance.GetCurrentQualitySettingLevelId((SettingConfig) this.config);

  private char ValidateInput(string text, int charIndex, char addedChar) => '0' > addedChar || addedChar > '9' ? char.MinValue : addedChar;

  private void OnEndEdit(string text)
  {
    int seed;
    try
    {
      seed = Convert.ToInt32(text);
    }
    catch
    {
      seed = 0;
    }
    this.SetSeed(seed);
  }

  public void SetSeed(int seed)
  {
    seed = Mathf.Min(seed, int.MaxValue);
    CustomGameSettings.Instance.SetQualitySetting((SettingConfig) this.config, seed.ToString());
    this.Refresh();
  }

  private void OnValueChanged(string text)
  {
    int num = 0;
    try
    {
      num = Convert.ToInt32(text);
    }
    catch
    {
      if (text.Length > 0)
        ((TMP_InputField) this.Input).text = text.Substring(0, text.Length - 1);
      else
        ((TMP_InputField) this.Input).text = "";
    }
    if (num <= int.MaxValue)
      return;
    ((TMP_InputField) this.Input).text = text.Substring(0, text.Length - 1);
  }

  private void GetNewRandomSeed() => this.SetSeed(Random.Range(0, int.MaxValue));
}
