// Decompiled with JetBrains decompiler
// Type: ValveSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValveSideScreen : SideScreenContent
{
  private Valve targetValve;
  [Header("Slider")]
  [SerializeField]
  private KSlider flowSlider;
  [SerializeField]
  private LocText minFlowLabel;
  [SerializeField]
  private LocText maxFlowLabel;
  [Header("Input Field")]
  [SerializeField]
  private KNumberInputField numberInput;
  [SerializeField]
  private LocText unitsLabel;
  private float targetFlow;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((TMP_Text) this.unitsLabel).text = GameUtil.AddTimeSliceText((string) STRINGS.UI.UNITSUFFIXES.MASS.GRAM, GameUtil.TimeSlice.PerSecond);
    this.flowSlider.onReleaseHandle += new System.Action(this.OnReleaseHandle);
    this.flowSlider.onDrag += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.flowSlider).value));
    this.flowSlider.onPointerDown += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.flowSlider).value));
    this.flowSlider.onMove += (System.Action) (() =>
    {
      this.ReceiveValueFromSlider(((Slider) this.flowSlider).value);
      this.OnReleaseHandle();
    });
    ((KInputField) this.numberInput).onEndEdit += (System.Action) (() => this.ReceiveValueFromInput(this.numberInput.currentValue));
    this.numberInput.decimalPlaces = 1;
  }

  public void OnReleaseHandle() => this.targetValve.ChangeFlow(this.targetFlow);

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<Valve>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    this.targetValve = target.GetComponent<Valve>();
    if (Object.op_Equality((Object) this.targetValve, (Object) null))
    {
      Debug.LogError((object) "The target object does not have a Valve component.");
    }
    else
    {
      ((Slider) this.flowSlider).minValue = 0.0f;
      ((Slider) this.flowSlider).maxValue = this.targetValve.MaxFlow;
      ((Slider) this.flowSlider).value = this.targetValve.DesiredFlow;
      ((TMP_Text) this.minFlowLabel).text = GameUtil.GetFormattedMass(0.0f, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram);
      ((TMP_Text) this.maxFlowLabel).text = GameUtil.GetFormattedMass(this.targetValve.MaxFlow, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram);
      this.numberInput.minValue = 0.0f;
      this.numberInput.maxValue = this.targetValve.MaxFlow * 1000f;
      ((KInputField) this.numberInput).SetDisplayValue(GameUtil.GetFormattedMass(Mathf.Max(0.0f, this.targetValve.DesiredFlow), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, false, "{0:0.#####}"));
      ((KScreen) this.numberInput).Activate();
    }
  }

  private void ReceiveValueFromSlider(float newValue)
  {
    newValue = Mathf.Round(newValue * 1000f) / 1000f;
    this.UpdateFlowValue(newValue);
  }

  private void ReceiveValueFromInput(float input)
  {
    this.UpdateFlowValue(input / 1000f);
    this.targetValve.ChangeFlow(this.targetFlow);
  }

  private void UpdateFlowValue(float newValue)
  {
    this.targetFlow = newValue;
    ((Slider) this.flowSlider).value = newValue;
    ((KInputField) this.numberInput).SetDisplayValue(GameUtil.GetFormattedMass(newValue, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.Gram, false, "{0:0.#####}"));
  }

  private IEnumerator SettingDelay(float delay)
  {
    float startTime = Time.realtimeSinceStartup;
    float currentTime = startTime;
    while ((double) currentTime < (double) startTime + (double) delay)
    {
      currentTime += Time.unscaledDeltaTime;
      yield return (object) SequenceUtil.WaitForEndOfFrame;
    }
    this.OnReleaseHandle();
  }
}
