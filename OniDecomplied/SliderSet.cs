// Decompiled with JetBrains decompiler
// Type: SliderSet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SliderSet
{
  public KSlider valueSlider;
  public KNumberInputField numberInput;
  public LocText unitsLabel;
  public LocText minLabel;
  public LocText maxLabel;
  [NonSerialized]
  public int index;
  private ISliderControl target;

  public void SetupSlider(int index)
  {
    this.index = index;
    this.valueSlider.onReleaseHandle += (System.Action) (() =>
    {
      ((Slider) this.valueSlider).value = Mathf.Round(((Slider) this.valueSlider).value * 10f) / 10f;
      this.ReceiveValueFromSlider();
    });
    this.valueSlider.onDrag += (System.Action) (() => this.ReceiveValueFromSlider());
    this.valueSlider.onMove += (System.Action) (() => this.ReceiveValueFromSlider());
    this.valueSlider.onPointerDown += (System.Action) (() => this.ReceiveValueFromSlider());
    ((KInputField) this.numberInput).onEndEdit += (System.Action) (() => this.ReceiveValueFromInput());
  }

  public void SetTarget(ISliderControl target)
  {
    this.target = target;
    ToolTip component = ((Component) ((Slider) this.valueSlider).handleRect).GetComponent<ToolTip>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.SetSimpleTooltip(target.GetSliderTooltip());
    ((TMP_Text) this.unitsLabel).text = target.SliderUnits;
    ((TMP_Text) this.minLabel).text = target.GetSliderMin(this.index).ToString() + target.SliderUnits;
    ((TMP_Text) this.maxLabel).text = target.GetSliderMax(this.index).ToString() + target.SliderUnits;
    this.numberInput.minValue = target.GetSliderMin(this.index);
    this.numberInput.maxValue = target.GetSliderMax(this.index);
    this.numberInput.decimalPlaces = target.SliderDecimalPlaces(this.index);
    ((TMP_InputField) ((KInputField) this.numberInput).field).characterLimit = Mathf.FloorToInt(1f + Mathf.Log10(this.numberInput.maxValue + (float) this.numberInput.decimalPlaces));
    Vector2 sizeDelta = ((Component) this.numberInput).GetComponent<RectTransform>().sizeDelta;
    sizeDelta.x = (float) ((((TMP_InputField) ((KInputField) this.numberInput).field).characterLimit + 1) * 10);
    ((Component) this.numberInput).GetComponent<RectTransform>().sizeDelta = sizeDelta;
    ((Slider) this.valueSlider).minValue = target.GetSliderMin(this.index);
    ((Slider) this.valueSlider).maxValue = target.GetSliderMax(this.index);
    ((Slider) this.valueSlider).value = target.GetSliderValue(this.index);
    this.SetValue(target.GetSliderValue(this.index));
    if (this.index != 0)
      return;
    ((KScreen) this.numberInput).Activate();
  }

  private void ReceiveValueFromSlider()
  {
    float num1 = ((Slider) this.valueSlider).value;
    if (this.numberInput.decimalPlaces != -1)
    {
      float num2 = Mathf.Pow(10f, (float) this.numberInput.decimalPlaces);
      num1 = Mathf.Round(num1 * num2) / num2;
    }
    this.SetValue(num1);
  }

  private void ReceiveValueFromInput()
  {
    float num1 = this.numberInput.currentValue;
    if (this.numberInput.decimalPlaces != -1)
    {
      float num2 = Mathf.Pow(10f, (float) this.numberInput.decimalPlaces);
      num1 = Mathf.Round(num1 * num2) / num2;
    }
    ((Slider) this.valueSlider).value = num1;
    this.SetValue(num1);
  }

  private void SetValue(float value)
  {
    float percent = value;
    if ((double) percent > (double) this.target.GetSliderMax(this.index))
      percent = this.target.GetSliderMax(this.index);
    else if ((double) percent < (double) this.target.GetSliderMin(this.index))
      percent = this.target.GetSliderMin(this.index);
    this.UpdateLabel(percent);
    this.target.SetSliderValue(percent, this.index);
    ToolTip component = ((Component) ((Slider) this.valueSlider).handleRect).GetComponent<ToolTip>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetSimpleTooltip(this.target.GetSliderTooltip());
  }

  private void UpdateLabel(float value) => ((KInputField) this.numberInput).SetDisplayValue((Mathf.Round(value * 10f) / 10f).ToString());
}
