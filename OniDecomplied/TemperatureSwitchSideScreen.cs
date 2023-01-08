// Decompiled with JetBrains decompiler
// Type: TemperatureSwitchSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TemperatureSwitchSideScreen : SideScreenContent, IRender200ms
{
  private TemperatureControlledSwitch targetTemperatureSwitch;
  [SerializeField]
  private LocText currentTemperature;
  [SerializeField]
  private LocText targetTemperature;
  [SerializeField]
  private KToggle coolerToggle;
  [SerializeField]
  private KToggle warmerToggle;
  [SerializeField]
  private KSlider targetTemperatureSlider;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.coolerToggle.onClick += (System.Action) (() => this.OnConditionButtonClicked(false));
    this.warmerToggle.onClick += (System.Action) (() => this.OnConditionButtonClicked(true));
    LocText component1 = ((Component) ((Component) this.coolerToggle).transform.GetChild(0)).GetComponent<LocText>();
    LocText component2 = ((Component) ((Component) this.warmerToggle).transform.GetChild(0)).GetComponent<LocText>();
    ((TMP_Text) component1).SetText((string) STRINGS.UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.COLDER_BUTTON);
    string warmerButton = (string) STRINGS.UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.WARMER_BUTTON;
    ((TMP_Text) component2).SetText(warmerButton);
    Slider.SliderEvent sliderEvent = new Slider.SliderEvent();
    // ISSUE: method pointer
    ((UnityEvent<float>) sliderEvent).AddListener(new UnityAction<float>((object) this, __methodptr(OnTargetTemperatureChanged)));
    ((Slider) this.targetTemperatureSlider).onValueChanged = sliderEvent;
  }

  public void Render200ms(float dt)
  {
    if (Object.op_Equality((Object) this.targetTemperatureSwitch, (Object) null))
      return;
    this.UpdateLabels();
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<TemperatureControlledSwitch>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    if (Object.op_Equality((Object) target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.targetTemperatureSwitch = target.GetComponent<TemperatureControlledSwitch>();
      if (Object.op_Equality((Object) this.targetTemperatureSwitch, (Object) null))
      {
        Debug.LogError((object) "The gameObject received does not contain a TimedSwitch component");
      }
      else
      {
        this.UpdateLabels();
        this.UpdateTargetTemperatureLabel();
        this.OnConditionButtonClicked(this.targetTemperatureSwitch.activateOnWarmerThan);
      }
    }
  }

  private void OnTargetTemperatureChanged(float new_value)
  {
    this.targetTemperatureSwitch.thresholdTemperature = new_value;
    this.UpdateTargetTemperatureLabel();
  }

  private void OnConditionButtonClicked(bool isWarmer)
  {
    this.targetTemperatureSwitch.activateOnWarmerThan = isWarmer;
    if (isWarmer)
    {
      this.coolerToggle.isOn = false;
      this.warmerToggle.isOn = true;
      ((Component) this.coolerToggle).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 1);
      ((Component) this.warmerToggle).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 2);
    }
    else
    {
      this.coolerToggle.isOn = true;
      this.warmerToggle.isOn = false;
      ((Component) this.coolerToggle).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 2);
      ((Component) this.warmerToggle).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 1);
    }
  }

  private void UpdateTargetTemperatureLabel() => ((TMP_Text) this.targetTemperature).text = GameUtil.GetFormattedTemperature(this.targetTemperatureSwitch.thresholdTemperature);

  private void UpdateLabels() => ((TMP_Text) this.currentTemperature).text = string.Format((string) STRINGS.UI.UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.CURRENT_TEMPERATURE, (object) GameUtil.GetFormattedTemperature(this.targetTemperatureSwitch.GetTemperature()));
}
