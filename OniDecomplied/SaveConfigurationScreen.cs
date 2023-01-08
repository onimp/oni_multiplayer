// Decompiled with JetBrains decompiler
// Type: SaveConfigurationScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class SaveConfigurationScreen
{
  [SerializeField]
  private KSlider autosaveFrequencySlider;
  [SerializeField]
  private LocText timelapseDescriptionLabel;
  [SerializeField]
  private KSlider timelapseResolutionSlider;
  [SerializeField]
  private LocText autosaveDescriptionLabel;
  private int[] sliderValueToCycleCount = new int[7]
  {
    -1,
    50,
    20,
    10,
    5,
    2,
    1
  };
  private Vector2I[] sliderValueToResolution = new Vector2I[7]
  {
    new Vector2I(-1, -1),
    new Vector2I(256, 384),
    new Vector2I(512, 768),
    new Vector2I(1024, 1536),
    new Vector2I(2048, 3072),
    new Vector2I(4096, 6144),
    new Vector2I(8192, 12288)
  };
  [SerializeField]
  private GameObject disabledContentPanel;
  [SerializeField]
  private GameObject disabledContentWarning;
  [SerializeField]
  private GameObject perSaveWarning;

  public void ToggleDisabledContent(bool enable)
  {
    if (enable)
    {
      this.disabledContentPanel.SetActive(true);
      this.disabledContentWarning.SetActive(false);
      this.perSaveWarning.SetActive(true);
    }
    else
    {
      this.disabledContentPanel.SetActive(false);
      this.disabledContentWarning.SetActive(true);
      this.perSaveWarning.SetActive(false);
    }
  }

  public void Init()
  {
    ((Slider) this.autosaveFrequencySlider).minValue = 0.0f;
    ((Slider) this.autosaveFrequencySlider).maxValue = (float) (this.sliderValueToCycleCount.Length - 1);
    // ISSUE: method pointer
    ((UnityEvent<float>) ((Slider) this.autosaveFrequencySlider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(\u003CInit\u003Eb__10_0)));
    ((Slider) this.autosaveFrequencySlider).value = (float) this.CycleCountToSlider(SaveGame.Instance.AutoSaveCycleInterval);
    ((Slider) this.timelapseResolutionSlider).minValue = 0.0f;
    ((Slider) this.timelapseResolutionSlider).maxValue = (float) (this.sliderValueToResolution.Length - 1);
    // ISSUE: method pointer
    ((UnityEvent<float>) ((Slider) this.timelapseResolutionSlider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(\u003CInit\u003Eb__10_1)));
    ((Slider) this.timelapseResolutionSlider).value = (float) this.ResolutionToSliderValue(SaveGame.Instance.TimelapseResolution);
    this.OnTimelapseValueChanged(Mathf.FloorToInt(((Slider) this.timelapseResolutionSlider).value));
  }

  public void Show(bool show)
  {
    if (!show)
      return;
    ((Slider) this.autosaveFrequencySlider).value = (float) this.CycleCountToSlider(SaveGame.Instance.AutoSaveCycleInterval);
    ((Slider) this.timelapseResolutionSlider).value = (float) this.ResolutionToSliderValue(SaveGame.Instance.TimelapseResolution);
    this.OnAutosaveValueChanged(Mathf.FloorToInt(((Slider) this.autosaveFrequencySlider).value));
    this.OnTimelapseValueChanged(Mathf.FloorToInt(((Slider) this.timelapseResolutionSlider).value));
  }

  private void OnTimelapseValueChanged(int sliderValue)
  {
    Vector2I resolution = this.SliderValueToResolution(sliderValue);
    if (resolution.x <= 0)
      ((TMP_Text) this.timelapseDescriptionLabel).SetText((string) STRINGS.UI.FRONTEND.COLONY_SAVE_OPTIONS_SCREEN.TIMELAPSE_DISABLED_DESCRIPTION);
    else
      ((TMP_Text) this.timelapseDescriptionLabel).SetText(string.Format((string) STRINGS.UI.FRONTEND.COLONY_SAVE_OPTIONS_SCREEN.TIMELAPSE_RESOLUTION_DESCRIPTION, (object) resolution.x, (object) resolution.y));
    SaveGame.Instance.TimelapseResolution = resolution;
    Game.Instance.Trigger(75424175, (object) null);
  }

  private void OnAutosaveValueChanged(int sliderValue)
  {
    int cycleCount = this.SliderValueToCycleCount(sliderValue);
    if (sliderValue == 0)
      ((TMP_Text) this.autosaveDescriptionLabel).SetText((string) STRINGS.UI.FRONTEND.COLONY_SAVE_OPTIONS_SCREEN.AUTOSAVE_NEVER);
    else
      ((TMP_Text) this.autosaveDescriptionLabel).SetText(string.Format((string) STRINGS.UI.FRONTEND.COLONY_SAVE_OPTIONS_SCREEN.AUTOSAVE_FREQUENCY_DESCRIPTION, (object) cycleCount));
    SaveGame.Instance.AutoSaveCycleInterval = cycleCount;
  }

  private int SliderValueToCycleCount(int sliderValue) => this.sliderValueToCycleCount[sliderValue];

  private int CycleCountToSlider(int count)
  {
    for (int slider = 0; slider < this.sliderValueToCycleCount.Length; ++slider)
    {
      if (this.sliderValueToCycleCount[slider] == count)
        return slider;
    }
    return 0;
  }

  private Vector2I SliderValueToResolution(int sliderValue) => this.sliderValueToResolution[sliderValue];

  private int ResolutionToSliderValue(Vector2I resolution)
  {
    for (int sliderValue = 0; sliderValue < this.sliderValueToResolution.Length; ++sliderValue)
    {
      if (Vector2I.op_Equality(this.sliderValueToResolution[sliderValue], resolution))
        return sliderValue;
    }
    return 0;
  }
}
