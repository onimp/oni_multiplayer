// Decompiled with JetBrains decompiler
// Type: LimitValveSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LimitValveSideScreen : SideScreenContent
{
  public static readonly string FLOAT_FORMAT = "{0:0.#####}";
  private LimitValve targetLimitValve;
  [Header("State")]
  [SerializeField]
  private LocText amountLabel;
  [SerializeField]
  private KButton resetButton;
  [Header("Slider")]
  [SerializeField]
  private NonLinearSlider limitSlider;
  [SerializeField]
  private LocText minLimitLabel;
  [SerializeField]
  private LocText maxLimitLabel;
  [SerializeField]
  private ToolTip toolTip;
  [Header("Input Field")]
  [SerializeField]
  private KNumberInputField numberInput;
  [SerializeField]
  private LocText unitsLabel;
  private float targetLimit;
  private int targetLimitValveSubHandle = -1;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.resetButton.onClick += new System.Action(this.ResetCounter);
    this.limitSlider.onReleaseHandle += new System.Action(this.OnReleaseHandle);
    this.limitSlider.onDrag += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.limitSlider).value));
    this.limitSlider.onPointerDown += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.limitSlider).value));
    this.limitSlider.onMove += (System.Action) (() =>
    {
      this.ReceiveValueFromSlider(((Slider) this.limitSlider).value);
      this.OnReleaseHandle();
    });
    ((KInputField) this.numberInput).onEndEdit += (System.Action) (() => this.ReceiveValueFromInput(this.numberInput.currentValue));
    this.numberInput.decimalPlaces = 3;
  }

  public void OnReleaseHandle() => this.targetLimitValve.Limit = this.targetLimit;

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<LimitValve>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    this.targetLimitValve = target.GetComponent<LimitValve>();
    if (Object.op_Equality((Object) this.targetLimitValve, (Object) null))
    {
      Debug.LogError((object) "The target object does not have a LimitValve component.");
    }
    else
    {
      if (this.targetLimitValveSubHandle != -1)
        ((KMonoBehaviour) this).Unsubscribe(this.targetLimitValveSubHandle);
      this.targetLimitValveSubHandle = this.targetLimitValve.Subscribe(-1722241721, new Action<object>(this.UpdateAmountLabel));
      ((Slider) this.limitSlider).minValue = 0.0f;
      ((Slider) this.limitSlider).maxValue = 100f;
      this.limitSlider.SetRanges(this.targetLimitValve.GetRanges());
      ((Slider) this.limitSlider).value = this.limitSlider.GetPercentageFromValue(this.targetLimitValve.Limit);
      this.numberInput.minValue = 0.0f;
      this.numberInput.maxValue = this.targetLimitValve.maxLimitKg;
      ((KScreen) this.numberInput).Activate();
      if (this.targetLimitValve.displayUnitsInsteadOfMass)
      {
        ((TMP_Text) this.minLimitLabel).text = GameUtil.GetFormattedUnits(0.0f);
        ((TMP_Text) this.maxLimitLabel).text = GameUtil.GetFormattedUnits(this.targetLimitValve.maxLimitKg);
        ((KInputField) this.numberInput).SetDisplayValue(GameUtil.GetFormattedUnits(Mathf.Max(0.0f, this.targetLimitValve.Limit), displaySuffix: false, floatFormatOverride: LimitValveSideScreen.FLOAT_FORMAT));
        ((TMP_Text) this.unitsLabel).text = (string) STRINGS.UI.UNITSUFFIXES.UNITS;
        ((Behaviour) this.toolTip).enabled = true;
        this.toolTip.SetSimpleTooltip((string) STRINGS.UI.UISIDESCREENS.LIMIT_VALVE_SIDE_SCREEN.SLIDER_TOOLTIP_UNITS);
      }
      else
      {
        ((TMP_Text) this.minLimitLabel).text = GameUtil.GetFormattedMass(0.0f, massFormat: GameUtil.MetricMassFormat.Kilogram);
        ((TMP_Text) this.maxLimitLabel).text = GameUtil.GetFormattedMass(this.targetLimitValve.maxLimitKg, massFormat: GameUtil.MetricMassFormat.Kilogram);
        ((KInputField) this.numberInput).SetDisplayValue(GameUtil.GetFormattedMass(Mathf.Max(0.0f, this.targetLimitValve.Limit), massFormat: GameUtil.MetricMassFormat.Kilogram, includeSuffix: false, floatFormat: LimitValveSideScreen.FLOAT_FORMAT));
        ((TMP_Text) this.unitsLabel).text = (string) GameUtil.GetCurrentMassUnit();
        ((Behaviour) this.toolTip).enabled = false;
      }
      this.UpdateAmountLabel();
    }
  }

  private void UpdateAmountLabel(object obj = null)
  {
    if (this.targetLimitValve.displayUnitsInsteadOfMass)
    {
      string formattedUnits = GameUtil.GetFormattedUnits(this.targetLimitValve.Amount, floatFormatOverride: LimitValveSideScreen.FLOAT_FORMAT);
      ((TMP_Text) this.amountLabel).text = string.Format((string) STRINGS.UI.UISIDESCREENS.LIMIT_VALVE_SIDE_SCREEN.AMOUNT, (object) formattedUnits);
    }
    else
    {
      string formattedMass = GameUtil.GetFormattedMass(this.targetLimitValve.Amount, massFormat: GameUtil.MetricMassFormat.Kilogram, floatFormat: LimitValveSideScreen.FLOAT_FORMAT);
      ((TMP_Text) this.amountLabel).text = string.Format((string) STRINGS.UI.UISIDESCREENS.LIMIT_VALVE_SIDE_SCREEN.AMOUNT, (object) formattedMass);
    }
  }

  private void ResetCounter() => this.targetLimitValve.ResetAmount();

  private void ReceiveValueFromSlider(float sliderPercentage) => this.UpdateLimitValue((float) Mathf.RoundToInt(this.limitSlider.GetValueForPercentage(sliderPercentage)));

  private void ReceiveValueFromInput(float input)
  {
    this.UpdateLimitValue(input);
    this.targetLimitValve.Limit = this.targetLimit;
  }

  private void UpdateLimitValue(float newValue)
  {
    this.targetLimit = newValue;
    ((Slider) this.limitSlider).value = this.limitSlider.GetPercentageFromValue(newValue);
    if (this.targetLimitValve.displayUnitsInsteadOfMass)
      ((KInputField) this.numberInput).SetDisplayValue(GameUtil.GetFormattedUnits(newValue, displaySuffix: false, floatFormatOverride: LimitValveSideScreen.FLOAT_FORMAT));
    else
      ((KInputField) this.numberInput).SetDisplayValue(GameUtil.GetFormattedMass(newValue, massFormat: GameUtil.MetricMassFormat.Kilogram, includeSuffix: false, floatFormat: LimitValveSideScreen.FLOAT_FORMAT));
  }
}
