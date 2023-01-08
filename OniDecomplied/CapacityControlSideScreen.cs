// Decompiled with JetBrains decompiler
// Type: CapacityControlSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CapacityControlSideScreen : SideScreenContent
{
  private IUserControlledCapacity target;
  [Header("Slider")]
  [SerializeField]
  private KSlider slider;
  [Header("Number Input")]
  [SerializeField]
  private KNumberInputField numberInput;
  [SerializeField]
  private LocText unitsLabel;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((TMP_Text) this.unitsLabel).text = (string) this.target.CapacityUnits;
    this.slider.onDrag += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.slider).value));
    this.slider.onPointerDown += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.slider).value));
    this.slider.onMove += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.slider).value));
    ((KInputField) this.numberInput).onEndEdit += (System.Action) (() => this.ReceiveValueFromInput(this.numberInput.currentValue));
    this.numberInput.decimalPlaces = 1;
  }

  public override bool IsValidForTarget(GameObject target) => !Util.IsNullOrDestroyed((object) target.GetComponent<IUserControlledCapacity>());

  public override void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.target = new_target.GetComponent<IUserControlledCapacity>();
      if (this.target == null)
      {
        Debug.LogError((object) "The gameObject received does not contain a IThresholdSwitch component");
      }
      else
      {
        ((Slider) this.slider).minValue = this.target.MinCapacity;
        ((Slider) this.slider).maxValue = this.target.MaxCapacity;
        ((Slider) this.slider).value = this.target.UserMaxCapacity;
        ((Component) this.slider).GetComponentInChildren<ToolTip>();
        ((TMP_Text) this.unitsLabel).text = (string) this.target.CapacityUnits;
        this.numberInput.minValue = this.target.MinCapacity;
        this.numberInput.maxValue = this.target.MaxCapacity;
        this.numberInput.currentValue = Mathf.Max(this.target.MinCapacity, Mathf.Min(this.target.MaxCapacity, this.target.UserMaxCapacity));
        ((KScreen) this.numberInput).Activate();
        this.UpdateMaxCapacityLabel();
      }
    }
  }

  private void ReceiveValueFromSlider(float newValue) => this.UpdateMaxCapacity(newValue);

  private void ReceiveValueFromInput(float newValue) => this.UpdateMaxCapacity(newValue);

  private void UpdateMaxCapacity(float newValue)
  {
    this.target.UserMaxCapacity = newValue;
    ((Slider) this.slider).value = newValue;
    this.UpdateMaxCapacityLabel();
  }

  private void UpdateMaxCapacityLabel() => ((KInputField) this.numberInput).SetDisplayValue(this.target.UserMaxCapacity.ToString());
}
