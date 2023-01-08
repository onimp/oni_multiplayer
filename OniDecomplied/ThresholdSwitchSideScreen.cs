// Decompiled with JetBrains decompiler
// Type: ThresholdSwitchSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThresholdSwitchSideScreen : SideScreenContent, IRender200ms
{
  private GameObject target;
  private IThresholdSwitch thresholdSwitch;
  [SerializeField]
  private LocText currentValue;
  [SerializeField]
  private LocText tresholdValue;
  [SerializeField]
  private KToggle aboveToggle;
  [SerializeField]
  private KToggle belowToggle;
  [Header("Slider")]
  [SerializeField]
  private NonLinearSlider thresholdSlider;
  [Header("Number Input")]
  [SerializeField]
  private KNumberInputField numberInput;
  [SerializeField]
  private LocText unitsLabel;
  [Header("Increment Buttons")]
  [SerializeField]
  private GameObject incrementMinor;
  [SerializeField]
  private GameObject incrementMajor;
  [SerializeField]
  private GameObject decrementMinor;
  [SerializeField]
  private GameObject decrementMajor;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.aboveToggle.onClick += (System.Action) (() => this.OnConditionButtonClicked(true));
    this.belowToggle.onClick += (System.Action) (() => this.OnConditionButtonClicked(false));
    LocText component1 = ((Component) ((Component) this.aboveToggle).transform.GetChild(0)).GetComponent<LocText>();
    LocText component2 = ((Component) ((Component) this.belowToggle).transform.GetChild(0)).GetComponent<LocText>();
    ((TMP_Text) component1).SetText((string) STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.ABOVE_BUTTON);
    string belowButton = (string) STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.BELOW_BUTTON;
    ((TMP_Text) component2).SetText(belowButton);
    this.thresholdSlider.onDrag += (System.Action) (() => this.ReceiveValueFromSlider(this.thresholdSlider.GetValueForPercentage(GameUtil.GetRoundedTemperatureInKelvin(((Slider) this.thresholdSlider).value))));
    this.thresholdSlider.onPointerDown += (System.Action) (() => this.ReceiveValueFromSlider(this.thresholdSlider.GetValueForPercentage(GameUtil.GetRoundedTemperatureInKelvin(((Slider) this.thresholdSlider).value))));
    this.thresholdSlider.onMove += (System.Action) (() => this.ReceiveValueFromSlider(this.thresholdSlider.GetValueForPercentage(GameUtil.GetRoundedTemperatureInKelvin(((Slider) this.thresholdSlider).value))));
    ((KInputField) this.numberInput).onEndEdit += (System.Action) (() => this.ReceiveValueFromInput(this.numberInput.currentValue));
    this.numberInput.decimalPlaces = 1;
  }

  public void Render200ms(float dt)
  {
    if (Object.op_Equality((Object) this.target, (Object) null))
      this.target = (GameObject) null;
    else
      this.UpdateLabels();
  }

  public override bool IsValidForTarget(GameObject target) => target.GetComponent<IThresholdSwitch>() != null;

  public override void SetTarget(GameObject new_target)
  {
    this.target = (GameObject) null;
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.target = new_target;
      this.thresholdSwitch = this.target.GetComponent<IThresholdSwitch>();
      if (this.thresholdSwitch == null)
      {
        this.target = (GameObject) null;
        Debug.LogError((object) "The gameObject received does not contain a IThresholdSwitch component");
      }
      else
      {
        this.UpdateLabels();
        if (this.target.GetComponent<IThresholdSwitch>().LayoutType == ThresholdScreenLayoutType.SliderBar)
        {
          ((Component) this.thresholdSlider).gameObject.SetActive(true);
          ((Slider) this.thresholdSlider).minValue = 0.0f;
          ((Slider) this.thresholdSlider).maxValue = 100f;
          this.thresholdSlider.SetRanges(this.thresholdSwitch.GetRanges);
          ((Slider) this.thresholdSlider).value = this.thresholdSlider.GetPercentageFromValue(this.thresholdSwitch.Threshold);
          ((Component) this.thresholdSlider).GetComponentInChildren<ToolTip>();
        }
        else
          ((Component) this.thresholdSlider).gameObject.SetActive(false);
        MultiToggle incrementMinorToggle = this.incrementMinor.GetComponent<MultiToggle>();
        incrementMinorToggle.onClick = (System.Action) (() =>
        {
          this.UpdateThresholdValue(this.thresholdSwitch.Threshold + (float) this.thresholdSwitch.IncrementScale);
          incrementMinorToggle.ChangeState(1);
        });
        incrementMinorToggle.onStopHold = (System.Action) (() => incrementMinorToggle.ChangeState(0));
        MultiToggle incrementMajorToggle = this.incrementMajor.GetComponent<MultiToggle>();
        incrementMajorToggle.onClick = (System.Action) (() =>
        {
          this.UpdateThresholdValue(this.thresholdSwitch.Threshold + 10f * (float) this.thresholdSwitch.IncrementScale);
          incrementMajorToggle.ChangeState(1);
        });
        incrementMajorToggle.onStopHold = (System.Action) (() => incrementMajorToggle.ChangeState(0));
        MultiToggle decrementMinorToggle = this.decrementMinor.GetComponent<MultiToggle>();
        decrementMinorToggle.onClick = (System.Action) (() =>
        {
          this.UpdateThresholdValue(this.thresholdSwitch.Threshold - (float) this.thresholdSwitch.IncrementScale);
          decrementMinorToggle.ChangeState(1);
        });
        decrementMinorToggle.onStopHold = (System.Action) (() => decrementMinorToggle.ChangeState(0));
        MultiToggle decrementMajorToggle = this.decrementMajor.GetComponent<MultiToggle>();
        decrementMajorToggle.onClick = (System.Action) (() =>
        {
          this.UpdateThresholdValue(this.thresholdSwitch.Threshold - 10f * (float) this.thresholdSwitch.IncrementScale);
          decrementMajorToggle.ChangeState(1);
        });
        decrementMajorToggle.onStopHold = (System.Action) (() => decrementMajorToggle.ChangeState(0));
        ((TMP_Text) this.unitsLabel).text = (string) this.thresholdSwitch.ThresholdValueUnits();
        this.numberInput.minValue = this.thresholdSwitch.GetRangeMinInputField();
        this.numberInput.maxValue = this.thresholdSwitch.GetRangeMaxInputField();
        ((KScreen) this.numberInput).Activate();
        this.UpdateTargetThresholdLabel();
        this.OnConditionButtonClicked(this.thresholdSwitch.ActivateAboveThreshold);
      }
    }
  }

  private void OnThresholdValueChanged(float new_value)
  {
    this.thresholdSwitch.Threshold = new_value;
    this.UpdateTargetThresholdLabel();
  }

  private void OnConditionButtonClicked(bool activate_above_threshold)
  {
    this.thresholdSwitch.ActivateAboveThreshold = activate_above_threshold;
    if (activate_above_threshold)
    {
      this.belowToggle.isOn = true;
      this.aboveToggle.isOn = false;
      ((Component) this.belowToggle).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 1);
      ((Component) this.aboveToggle).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 2);
    }
    else
    {
      this.belowToggle.isOn = false;
      this.aboveToggle.isOn = true;
      ((Component) this.belowToggle).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 2);
      ((Component) this.aboveToggle).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 1);
    }
    this.UpdateTargetThresholdLabel();
  }

  private void UpdateTargetThresholdLabel()
  {
    ((KInputField) this.numberInput).SetDisplayValue(this.thresholdSwitch.Format(this.thresholdSwitch.Threshold, false) + (string) this.thresholdSwitch.ThresholdValueUnits());
    if (this.thresholdSwitch.ActivateAboveThreshold)
    {
      ((Component) this.thresholdSlider).GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(this.thresholdSwitch.AboveToolTip, (object) this.thresholdSwitch.Format(this.thresholdSwitch.Threshold, true)));
      ((Component) this.thresholdSlider).GetComponentInChildren<ToolTip>().tooltipPositionOffset = new Vector2(0.0f, 25f);
    }
    else
    {
      ((Component) this.thresholdSlider).GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(this.thresholdSwitch.BelowToolTip, (object) this.thresholdSwitch.Format(this.thresholdSwitch.Threshold, true)));
      ((Component) this.thresholdSlider).GetComponentInChildren<ToolTip>().tooltipPositionOffset = new Vector2(0.0f, 25f);
    }
  }

  private void ReceiveValueFromSlider(float newValue) => this.UpdateThresholdValue(this.thresholdSwitch.ProcessedSliderValue(newValue));

  private void ReceiveValueFromInput(float newValue) => this.UpdateThresholdValue(this.thresholdSwitch.ProcessedInputValue(newValue));

  private void UpdateThresholdValue(float newValue)
  {
    if ((double) newValue < (double) this.thresholdSwitch.RangeMin)
      newValue = this.thresholdSwitch.RangeMin;
    if ((double) newValue > (double) this.thresholdSwitch.RangeMax)
      newValue = this.thresholdSwitch.RangeMax;
    this.thresholdSwitch.Threshold = newValue;
    NonLinearSlider thresholdSlider = this.thresholdSlider;
    if (Object.op_Inequality((Object) thresholdSlider, (Object) null))
      ((Slider) this.thresholdSlider).value = thresholdSlider.GetPercentageFromValue(newValue);
    else
      ((Slider) this.thresholdSlider).value = newValue;
    this.UpdateTargetThresholdLabel();
  }

  private void UpdateLabels() => ((TMP_Text) this.currentValue).text = string.Format((string) STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.CURRENT_VALUE, (object) this.thresholdSwitch.ThresholdValueName, (object) this.thresholdSwitch.Format(this.thresholdSwitch.CurrentValue, true));

  public override string GetTitle() => Object.op_Inequality((Object) this.target, (Object) null) ? (string) this.thresholdSwitch.Title : (string) STRINGS.UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;
}
