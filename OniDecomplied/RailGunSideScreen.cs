// Decompiled with JetBrains decompiler
// Type: RailGunSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RailGunSideScreen : SideScreenContent
{
  public GameObject content;
  private RailGun selectedGun;
  public LocText DescriptionText;
  [Header("Slider")]
  [SerializeField]
  private KSlider slider;
  [Header("Number Input")]
  [SerializeField]
  private KNumberInputField numberInput;
  [SerializeField]
  private LocText unitsLabel;
  [SerializeField]
  private LocText hepStorageInfo;
  private int targetRailgunHEPStorageSubHandle = -1;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((TMP_Text) this.unitsLabel).text = (string) GameUtil.GetCurrentMassUnit();
    this.slider.onDrag += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.slider).value));
    this.slider.onPointerDown += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.slider).value));
    this.slider.onMove += (System.Action) (() => this.ReceiveValueFromSlider(((Slider) this.slider).value));
    ((KInputField) this.numberInput).onEndEdit += (System.Action) (() => this.ReceiveValueFromInput(this.numberInput.currentValue));
    this.numberInput.decimalPlaces = 1;
  }

  protected virtual void OnCmpDisable()
  {
    ((KMonoBehaviour) this).OnCmpDisable();
    if (!Object.op_Implicit((Object) this.selectedGun))
      return;
    this.selectedGun = (RailGun) null;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (!Object.op_Implicit((Object) this.selectedGun))
      return;
    this.selectedGun = (RailGun) null;
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<RailGun>(), (Object) null);

  public override void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.selectedGun = new_target.GetComponent<RailGun>();
      if (Object.op_Equality((Object) this.selectedGun, (Object) null))
      {
        Debug.LogError((object) "The gameObject received does not contain a RailGun component");
      }
      else
      {
        this.targetRailgunHEPStorageSubHandle = this.selectedGun.Subscribe(-1837862626, new Action<object>(this.UpdateHEPLabels));
        ((Slider) this.slider).minValue = this.selectedGun.MinLaunchMass;
        ((Slider) this.slider).maxValue = this.selectedGun.MaxLaunchMass;
        ((Slider) this.slider).value = this.selectedGun.launchMass;
        ((TMP_Text) this.unitsLabel).text = (string) GameUtil.GetCurrentMassUnit();
        this.numberInput.minValue = this.selectedGun.MinLaunchMass;
        this.numberInput.maxValue = this.selectedGun.MaxLaunchMass;
        this.numberInput.currentValue = Mathf.Max(this.selectedGun.MinLaunchMass, Mathf.Min(this.selectedGun.MaxLaunchMass, this.selectedGun.launchMass));
        this.UpdateMaxCapacityLabel();
        ((KScreen) this.numberInput).Activate();
        this.UpdateHEPLabels();
      }
    }
  }

  public override void ClearTarget()
  {
    if (this.targetRailgunHEPStorageSubHandle != -1 && Object.op_Inequality((Object) this.selectedGun, (Object) null))
    {
      this.selectedGun.Unsubscribe(this.targetRailgunHEPStorageSubHandle);
      this.targetRailgunHEPStorageSubHandle = -1;
    }
    this.selectedGun = (RailGun) null;
  }

  public void UpdateHEPLabels(object data = null)
  {
    if (Object.op_Equality((Object) this.selectedGun, (Object) null))
      return;
    string sidescreenHepRequired = (string) BUILDINGS.PREFABS.RAILGUN.SIDESCREEN_HEP_REQUIRED;
    float num = this.selectedGun.CurrentEnergy;
    string newValue1 = num.ToString();
    string str = sidescreenHepRequired.Replace("{current}", newValue1);
    num = this.selectedGun.EnergyCost;
    string newValue2 = num.ToString();
    ((TMP_Text) this.hepStorageInfo).text = str.Replace("{required}", newValue2);
  }

  private void ReceiveValueFromSlider(float newValue) => this.UpdateMaxCapacity(newValue);

  private void ReceiveValueFromInput(float newValue) => this.UpdateMaxCapacity(newValue);

  private void UpdateMaxCapacity(float newValue)
  {
    this.selectedGun.launchMass = newValue;
    ((Slider) this.slider).value = newValue;
    this.UpdateMaxCapacityLabel();
  }

  private void UpdateMaxCapacityLabel() => ((KInputField) this.numberInput).SetDisplayValue(this.selectedGun.launchMass.ToString());
}
