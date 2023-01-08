// Decompiled with JetBrains decompiler
// Type: ActiveRangeSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActiveRangeSideScreen : SideScreenContent
{
  private IActivationRangeTarget target;
  [SerializeField]
  private KSlider activateValueSlider;
  [SerializeField]
  private KSlider deactivateValueSlider;
  [SerializeField]
  private LocText activateLabel;
  [SerializeField]
  private LocText deactivateLabel;
  [Header("Number Input")]
  [SerializeField]
  private KNumberInputField activateValueLabel;
  [SerializeField]
  private KNumberInputField deactivateValueLabel;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.activateValueLabel.maxValue = this.target.MaxValue;
    this.activateValueLabel.minValue = this.target.MinValue;
    this.deactivateValueLabel.maxValue = this.target.MaxValue;
    this.deactivateValueLabel.minValue = this.target.MinValue;
    // ISSUE: method pointer
    ((UnityEvent<float>) ((Slider) this.activateValueSlider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(OnActivateValueChanged)));
    // ISSUE: method pointer
    ((UnityEvent<float>) ((Slider) this.deactivateValueSlider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(OnDeactivateValueChanged)));
  }

  private void OnActivateValueChanged(float new_value)
  {
    this.target.ActivateValue = new_value;
    if ((double) this.target.ActivateValue < (double) this.target.DeactivateValue)
    {
      this.target.ActivateValue = this.target.DeactivateValue;
      ((Slider) this.activateValueSlider).value = this.target.ActivateValue;
    }
    ((KInputField) this.activateValueLabel).SetDisplayValue(this.target.ActivateValue.ToString());
    this.RefreshTooltips();
  }

  private void OnDeactivateValueChanged(float new_value)
  {
    this.target.DeactivateValue = new_value;
    if ((double) this.target.DeactivateValue > (double) this.target.ActivateValue)
    {
      this.target.DeactivateValue = ((Slider) this.activateValueSlider).value;
      ((Slider) this.deactivateValueSlider).value = this.target.DeactivateValue;
    }
    ((KInputField) this.deactivateValueLabel).SetDisplayValue(this.target.DeactivateValue.ToString());
    this.RefreshTooltips();
  }

  private void RefreshTooltips()
  {
    ((Component) this.activateValueSlider).GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(this.target.ActivateTooltip, (object) ((Slider) this.activateValueSlider).value, (object) ((Slider) this.deactivateValueSlider).value));
    ((Component) this.deactivateValueSlider).GetComponentInChildren<ToolTip>().SetSimpleTooltip(string.Format(this.target.DeactivateTooltip, (object) ((Slider) this.deactivateValueSlider).value, (object) ((Slider) this.activateValueSlider).value));
  }

  public override bool IsValidForTarget(GameObject target) => target.GetComponent<IActivationRangeTarget>() != null;

  public override void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.target = new_target.GetComponent<IActivationRangeTarget>();
      if (this.target == null)
      {
        Debug.LogError((object) "The gameObject received does not contain a IActivationRangeTarget component");
      }
      else
      {
        ((TMP_Text) this.activateLabel).text = this.target.ActivateSliderLabelText;
        ((TMP_Text) this.deactivateLabel).text = this.target.DeactivateSliderLabelText;
        ((KScreen) this.activateValueLabel).Activate();
        ((KScreen) this.deactivateValueLabel).Activate();
        // ISSUE: method pointer
        ((UnityEvent<float>) ((Slider) this.activateValueSlider).onValueChanged).RemoveListener(new UnityAction<float>((object) this, __methodptr(OnActivateValueChanged)));
        ((Slider) this.activateValueSlider).minValue = this.target.MinValue;
        ((Slider) this.activateValueSlider).maxValue = this.target.MaxValue;
        ((Slider) this.activateValueSlider).value = this.target.ActivateValue;
        ((Slider) this.activateValueSlider).wholeNumbers = this.target.UseWholeNumbers;
        // ISSUE: method pointer
        ((UnityEvent<float>) ((Slider) this.activateValueSlider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(OnActivateValueChanged)));
        ((KInputField) this.activateValueLabel).SetDisplayValue(this.target.ActivateValue.ToString());
        ((KInputField) this.activateValueLabel).onEndEdit += (System.Action) (() =>
        {
          float result = this.target.ActivateValue;
          float.TryParse(((TMP_InputField) ((KInputField) this.activateValueLabel).field).text, out result);
          this.OnActivateValueChanged(result);
          ((Slider) this.activateValueSlider).value = result;
        });
        // ISSUE: method pointer
        ((UnityEvent<float>) ((Slider) this.deactivateValueSlider).onValueChanged).RemoveListener(new UnityAction<float>((object) this, __methodptr(OnDeactivateValueChanged)));
        ((Slider) this.deactivateValueSlider).minValue = this.target.MinValue;
        ((Slider) this.deactivateValueSlider).maxValue = this.target.MaxValue;
        ((Slider) this.deactivateValueSlider).value = this.target.DeactivateValue;
        ((Slider) this.deactivateValueSlider).wholeNumbers = this.target.UseWholeNumbers;
        // ISSUE: method pointer
        ((UnityEvent<float>) ((Slider) this.deactivateValueSlider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(OnDeactivateValueChanged)));
        ((KInputField) this.deactivateValueLabel).SetDisplayValue(this.target.DeactivateValue.ToString());
        ((KInputField) this.deactivateValueLabel).onEndEdit += (System.Action) (() =>
        {
          float result = this.target.DeactivateValue;
          float.TryParse(((TMP_InputField) ((KInputField) this.deactivateValueLabel).field).text, out result);
          this.OnDeactivateValueChanged(result);
          ((Slider) this.deactivateValueSlider).value = result;
        });
        this.RefreshTooltips();
      }
    }
  }

  public override string GetTitle() => this.target != null ? this.target.ActivationRangeTitleText : (string) STRINGS.UI.UISIDESCREENS.ACTIVATION_RANGE_SIDE_SCREEN.NAME;
}
