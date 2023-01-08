// Decompiled with JetBrains decompiler
// Type: DisinfectThresholdDiagram
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisinfectThresholdDiagram : MonoBehaviour
{
  [SerializeField]
  private KNumberInputField inputField;
  [SerializeField]
  private KSlider slider;
  [SerializeField]
  private LocText minLabel;
  [SerializeField]
  private LocText maxLabel;
  [SerializeField]
  private LocText unitsLabel;
  [SerializeField]
  private LocText thresholdPrefix;
  [SerializeField]
  private ToolTip toolTip;
  [SerializeField]
  private KToggle toggle;
  [SerializeField]
  private Image disabledImage;
  private static int MAX_VALUE = 1000000;
  private static int SLIDER_CONVERSION = 1000;

  private void Start()
  {
    this.inputField.minValue = 0.0f;
    this.inputField.maxValue = (float) DisinfectThresholdDiagram.MAX_VALUE;
    this.inputField.currentValue = (float) SaveGame.Instance.minGermCountForDisinfect;
    ((KInputField) this.inputField).SetDisplayValue(SaveGame.Instance.minGermCountForDisinfect.ToString());
    ((KInputField) this.inputField).onEndEdit += (System.Action) (() => this.ReceiveValueFromInput(this.inputField.currentValue));
    this.inputField.decimalPlaces = 1;
    ((KScreen) this.inputField).Activate();
    ((Slider) this.slider).minValue = 0.0f;
    ((Slider) this.slider).maxValue = (float) (DisinfectThresholdDiagram.MAX_VALUE / DisinfectThresholdDiagram.SLIDER_CONVERSION);
    ((Slider) this.slider).wholeNumbers = true;
    ((Slider) this.slider).value = (float) (SaveGame.Instance.minGermCountForDisinfect / DisinfectThresholdDiagram.SLIDER_CONVERSION);
    this.slider.onReleaseHandle += new System.Action(this.OnReleaseHandle);
    this.slider.onDrag += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.slider).value));
    this.slider.onPointerDown += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.slider).value));
    this.slider.onMove += (System.Action) (() =>
    {
      this.ReceiveValueFromSlider(((Slider) this.slider).value);
      this.OnReleaseHandle();
    });
    ((TMP_Text) this.unitsLabel).SetText((string) STRINGS.UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.UNITS);
    ((TMP_Text) this.minLabel).SetText((string) STRINGS.UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.MIN_LABEL);
    ((TMP_Text) this.maxLabel).SetText((string) STRINGS.UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.MAX_LABEL);
    ((TMP_Text) this.thresholdPrefix).SetText((string) STRINGS.UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.THRESHOLD_PREFIX);
    this.toolTip.OnToolTip = (Func<string>) (() =>
    {
      this.toolTip.ClearMultiStringTooltip();
      if (SaveGame.Instance.enableAutoDisinfect)
        this.toolTip.AddMultiStringTooltip(STRINGS.UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.TOOLTIP.ToString().Replace("{NumberOfGerms}", SaveGame.Instance.minGermCountForDisinfect.ToString()), (TextStyleSetting) null);
      else
        this.toolTip.AddMultiStringTooltip(STRINGS.UI.OVERLAYS.DISEASE.DISINFECT_THRESHOLD_DIAGRAM.TOOLTIP_DISABLED.ToString(), (TextStyleSetting) null);
      return "";
    });
    ((Component) this.disabledImage).gameObject.SetActive(!SaveGame.Instance.enableAutoDisinfect);
    this.toggle.isOn = SaveGame.Instance.enableAutoDisinfect;
    this.toggle.onValueChanged += new Action<bool>(this.OnClickToggle);
  }

  private void OnReleaseHandle()
  {
    float num = (float) ((int) ((Slider) this.slider).value * DisinfectThresholdDiagram.SLIDER_CONVERSION);
    SaveGame.Instance.minGermCountForDisinfect = (int) num;
    ((KInputField) this.inputField).SetDisplayValue(num.ToString());
  }

  private void ReceiveValueFromSlider(float new_value)
  {
    SaveGame.Instance.minGermCountForDisinfect = (int) new_value * DisinfectThresholdDiagram.SLIDER_CONVERSION;
    ((KInputField) this.inputField).SetDisplayValue((new_value * (float) DisinfectThresholdDiagram.SLIDER_CONVERSION).ToString());
  }

  private void ReceiveValueFromInput(float new_value)
  {
    ((Slider) this.slider).value = new_value / (float) DisinfectThresholdDiagram.SLIDER_CONVERSION;
    SaveGame.Instance.minGermCountForDisinfect = (int) new_value;
  }

  private void OnClickToggle(bool new_value)
  {
    SaveGame.Instance.enableAutoDisinfect = new_value;
    ((Component) this.disabledImage).gameObject.SetActive(!SaveGame.Instance.enableAutoDisinfect);
  }
}
