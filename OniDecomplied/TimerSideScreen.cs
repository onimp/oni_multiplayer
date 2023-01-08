// Decompiled with JetBrains decompiler
// Type: TimerSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimerSideScreen : SideScreenContent, IRenderEveryTick
{
  public Image greenActiveZone;
  public Image redActiveZone;
  private LogicTimerSensor targetTimedSwitch;
  public KToggle modeButton;
  public KButton resetButton;
  public KSlider onDurationSlider;
  [SerializeField]
  private KNumberInputField onDurationNumberInput;
  public KSlider offDurationSlider;
  [SerializeField]
  private KNumberInputField offDurationNumberInput;
  public RectTransform endIndicator;
  public RectTransform currentTimeMarker;
  public LocText labelHeaderOnDuration;
  public LocText labelHeaderOffDuration;
  public LocText labelValueOnDuration;
  public LocText labelValueOffDuration;
  public LocText timeLeft;
  public float phaseLength;
  private bool cyclesMode;
  [SerializeField]
  private float minSeconds;
  [SerializeField]
  private float maxSeconds = 600f;
  [SerializeField]
  private float minCycles;
  [SerializeField]
  private float maxCycles = 10f;
  private const int CYCLEMODE_DECIMALS = 2;
  private const int SECONDSMODE_DECIMALS = 1;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((TMP_Text) this.labelHeaderOnDuration).text = (string) STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.ON;
    ((TMP_Text) this.labelHeaderOffDuration).text = (string) STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.OFF;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.modeButton.onClick += (System.Action) (() => this.ToggleMode());
    this.resetButton.onClick += new System.Action(this.ResetTimer);
    ((KInputField) this.onDurationNumberInput).onEndEdit += (System.Action) (() => this.UpdateDurationValueFromTextInput(this.onDurationNumberInput.currentValue, this.onDurationSlider));
    ((KInputField) this.offDurationNumberInput).onEndEdit += (System.Action) (() => this.UpdateDurationValueFromTextInput(this.offDurationNumberInput.currentValue, this.offDurationSlider));
    ((Slider) this.onDurationSlider).wholeNumbers = false;
    ((Slider) this.offDurationSlider).wholeNumbers = false;
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<LogicTimerSensor>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    ((Graphic) this.greenActiveZone).color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.logicOnSidescreen);
    ((Graphic) this.redActiveZone).color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.logicOffSidescreen);
    base.SetTarget(target);
    this.targetTimedSwitch = target.GetComponent<LogicTimerSensor>();
    ((UnityEventBase) ((Slider) this.onDurationSlider).onValueChanged).RemoveAllListeners();
    ((UnityEventBase) ((Slider) this.offDurationSlider).onValueChanged).RemoveAllListeners();
    this.cyclesMode = this.targetTimedSwitch.displayCyclesMode;
    this.UpdateVisualsForNewTarget();
    this.ReconfigureRingVisuals();
    // ISSUE: method pointer
    ((UnityEvent<float>) ((Slider) this.onDurationSlider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(\u003CSetTarget\u003Eb__27_0)));
    // ISSUE: method pointer
    ((UnityEvent<float>) ((Slider) this.offDurationSlider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(\u003CSetTarget\u003Eb__27_1)));
  }

  private void UpdateVisualsForNewTarget()
  {
    float onDuration = this.targetTimedSwitch.onDuration;
    float offDuration = this.targetTimedSwitch.offDuration;
    bool displayCyclesMode = this.targetTimedSwitch.displayCyclesMode;
    if (displayCyclesMode)
    {
      ((Slider) this.onDurationSlider).minValue = this.minCycles;
      this.onDurationNumberInput.minValue = ((Slider) this.onDurationSlider).minValue;
      ((Slider) this.onDurationSlider).maxValue = this.maxCycles;
      this.onDurationNumberInput.maxValue = ((Slider) this.onDurationSlider).maxValue;
      this.onDurationNumberInput.decimalPlaces = 2;
      ((Slider) this.offDurationSlider).minValue = this.minCycles;
      this.offDurationNumberInput.minValue = ((Slider) this.offDurationSlider).minValue;
      ((Slider) this.offDurationSlider).maxValue = this.maxCycles;
      this.offDurationNumberInput.maxValue = ((Slider) this.offDurationSlider).maxValue;
      this.offDurationNumberInput.decimalPlaces = 2;
      ((Slider) this.onDurationSlider).value = onDuration / 600f;
      ((Slider) this.offDurationSlider).value = offDuration / 600f;
      this.onDurationNumberInput.SetAmount(onDuration / 600f);
      this.offDurationNumberInput.SetAmount(offDuration / 600f);
    }
    else
    {
      ((Slider) this.onDurationSlider).minValue = this.minSeconds;
      this.onDurationNumberInput.minValue = ((Slider) this.onDurationSlider).minValue;
      ((Slider) this.onDurationSlider).maxValue = this.maxSeconds;
      this.onDurationNumberInput.maxValue = ((Slider) this.onDurationSlider).maxValue;
      this.onDurationNumberInput.decimalPlaces = 1;
      ((Slider) this.offDurationSlider).minValue = this.minSeconds;
      this.offDurationNumberInput.minValue = ((Slider) this.offDurationSlider).minValue;
      ((Slider) this.offDurationSlider).maxValue = this.maxSeconds;
      this.offDurationNumberInput.maxValue = ((Slider) this.offDurationSlider).maxValue;
      this.offDurationNumberInput.decimalPlaces = 1;
      ((Slider) this.onDurationSlider).value = onDuration;
      ((Slider) this.offDurationSlider).value = offDuration;
      this.onDurationNumberInput.SetAmount(onDuration);
      this.offDurationNumberInput.SetAmount(offDuration);
    }
    ((TMP_Text) ((Component) this.modeButton).GetComponentInChildren<LocText>()).text = (string) (displayCyclesMode ? STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.MODE_LABEL_CYCLES : STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.MODE_LABEL_SECONDS);
  }

  private void ToggleMode()
  {
    this.cyclesMode = !this.cyclesMode;
    this.targetTimedSwitch.displayCyclesMode = this.cyclesMode;
    float num1 = ((Slider) this.onDurationSlider).value;
    float num2 = ((Slider) this.offDurationSlider).value;
    float num3;
    float num4;
    if (this.cyclesMode)
    {
      num3 = ((Slider) this.onDurationSlider).value / 600f;
      num4 = ((Slider) this.offDurationSlider).value / 600f;
    }
    else
    {
      num3 = ((Slider) this.onDurationSlider).value * 600f;
      num4 = ((Slider) this.offDurationSlider).value * 600f;
    }
    ((Slider) this.onDurationSlider).minValue = this.cyclesMode ? this.minCycles : this.minSeconds;
    this.onDurationNumberInput.minValue = ((Slider) this.onDurationSlider).minValue;
    ((Slider) this.onDurationSlider).maxValue = this.cyclesMode ? this.maxCycles : this.maxSeconds;
    this.onDurationNumberInput.maxValue = ((Slider) this.onDurationSlider).maxValue;
    this.onDurationNumberInput.decimalPlaces = this.cyclesMode ? 2 : 1;
    ((Slider) this.offDurationSlider).minValue = this.cyclesMode ? this.minCycles : this.minSeconds;
    this.offDurationNumberInput.minValue = ((Slider) this.offDurationSlider).minValue;
    ((Slider) this.offDurationSlider).maxValue = this.cyclesMode ? this.maxCycles : this.maxSeconds;
    this.offDurationNumberInput.maxValue = ((Slider) this.offDurationSlider).maxValue;
    this.offDurationNumberInput.decimalPlaces = this.cyclesMode ? 2 : 1;
    ((Slider) this.onDurationSlider).value = num3;
    ((Slider) this.offDurationSlider).value = num4;
    this.onDurationNumberInput.SetAmount(num3);
    this.offDurationNumberInput.SetAmount(num4);
    ((TMP_Text) ((Component) this.modeButton).GetComponentInChildren<LocText>()).text = (string) (this.cyclesMode ? STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.MODE_LABEL_CYCLES : STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.MODE_LABEL_SECONDS);
  }

  private void ChangeSetting()
  {
    this.targetTimedSwitch.onDuration = this.cyclesMode ? ((Slider) this.onDurationSlider).value * 600f : ((Slider) this.onDurationSlider).value;
    this.targetTimedSwitch.offDuration = this.cyclesMode ? ((Slider) this.offDurationSlider).value * 600f : ((Slider) this.offDurationSlider).value;
    this.ReconfigureRingVisuals();
    KNumberInputField durationNumberInput1 = this.onDurationNumberInput;
    float num;
    string str1;
    if (!this.cyclesMode)
    {
      str1 = this.targetTimedSwitch.onDuration.ToString();
    }
    else
    {
      num = this.targetTimedSwitch.onDuration / 600f;
      str1 = num.ToString("F2");
    }
    ((KInputField) durationNumberInput1).SetDisplayValue(str1);
    KNumberInputField durationNumberInput2 = this.offDurationNumberInput;
    string str2;
    if (!this.cyclesMode)
    {
      str2 = this.targetTimedSwitch.offDuration.ToString();
    }
    else
    {
      num = this.targetTimedSwitch.offDuration / 600f;
      str2 = num.ToString("F2");
    }
    ((KInputField) durationNumberInput2).SetDisplayValue(str2);
    this.onDurationSlider.SetTooltipText(string.Format((string) STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.GREEN_DURATION_TOOLTIP, this.cyclesMode ? (object) GameUtil.GetFormattedCycles(this.targetTimedSwitch.onDuration, "F2") : (object) GameUtil.GetFormattedTime(this.targetTimedSwitch.onDuration)));
    this.offDurationSlider.SetTooltipText(string.Format((string) STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.RED_DURATION_TOOLTIP, this.cyclesMode ? (object) GameUtil.GetFormattedCycles(this.targetTimedSwitch.offDuration, "F2") : (object) GameUtil.GetFormattedTime(this.targetTimedSwitch.offDuration)));
    if ((double) this.phaseLength != 0.0)
      return;
    ((TMP_Text) this.timeLeft).text = (string) STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.DISABLED;
    if (this.targetTimedSwitch.IsSwitchedOn)
    {
      this.greenActiveZone.fillAmount = 1f;
      this.redActiveZone.fillAmount = 0.0f;
    }
    else
    {
      this.greenActiveZone.fillAmount = 0.0f;
      this.redActiveZone.fillAmount = 1f;
    }
    this.targetTimedSwitch.timeElapsedInCurrentState = 0.0f;
    ((Transform) this.currentTimeMarker).rotation = Quaternion.identity;
    ((Transform) this.currentTimeMarker).Rotate(0.0f, 0.0f, 0.0f);
  }

  private void ReconfigureRingVisuals()
  {
    this.phaseLength = this.targetTimedSwitch.onDuration + this.targetTimedSwitch.offDuration;
    this.greenActiveZone.fillAmount = this.targetTimedSwitch.onDuration / this.phaseLength;
    this.redActiveZone.fillAmount = this.targetTimedSwitch.offDuration / this.phaseLength;
  }

  public void RenderEveryTick(float dt)
  {
    if ((double) this.phaseLength == 0.0)
      return;
    float elapsedInCurrentState = this.targetTimedSwitch.timeElapsedInCurrentState;
    if (this.cyclesMode)
      ((TMP_Text) this.timeLeft).text = string.Format((string) STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.CURRENT_TIME, (object) GameUtil.GetFormattedCycles(elapsedInCurrentState, "F2"), (object) GameUtil.GetFormattedCycles(this.targetTimedSwitch.IsSwitchedOn ? this.targetTimedSwitch.onDuration : this.targetTimedSwitch.offDuration, "F2"));
    else
      ((TMP_Text) this.timeLeft).text = string.Format((string) STRINGS.UI.UISIDESCREENS.TIMER_SIDE_SCREEN.CURRENT_TIME, (object) GameUtil.GetFormattedTime(elapsedInCurrentState, "F1"), (object) GameUtil.GetFormattedTime(this.targetTimedSwitch.IsSwitchedOn ? this.targetTimedSwitch.onDuration : this.targetTimedSwitch.offDuration, "F1"));
    ((Transform) this.currentTimeMarker).rotation = Quaternion.identity;
    if (this.targetTimedSwitch.IsSwitchedOn)
      ((Transform) this.currentTimeMarker).Rotate(0.0f, 0.0f, (float) ((double) this.targetTimedSwitch.timeElapsedInCurrentState / (double) this.phaseLength * -360.0));
    else
      ((Transform) this.currentTimeMarker).Rotate(0.0f, 0.0f, (float) (((double) this.targetTimedSwitch.onDuration + (double) this.targetTimedSwitch.timeElapsedInCurrentState) / (double) this.phaseLength * -360.0));
  }

  private void UpdateDurationValueFromTextInput(float newValue, KSlider slider)
  {
    if ((double) newValue < (double) ((Slider) slider).minValue)
      newValue = ((Slider) slider).minValue;
    if ((double) newValue > (double) ((Slider) slider).maxValue)
      newValue = ((Slider) slider).maxValue;
    ((Slider) slider).value = newValue;
    NonLinearSlider nonLinearSlider = slider as NonLinearSlider;
    if (Object.op_Inequality((Object) nonLinearSlider, (Object) null))
      ((Slider) slider).value = nonLinearSlider.GetPercentageFromValue(newValue);
    else
      ((Slider) slider).value = newValue;
  }

  private void ResetTimer() => this.targetTimedSwitch.ResetTimer();
}
