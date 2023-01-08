// Decompiled with JetBrains decompiler
// Type: SliderContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/SliderContainer")]
public class SliderContainer : KMonoBehaviour
{
  public bool isPercentValue = true;
  public KSlider slider;
  public LocText nameLabel;
  public LocText valueLabel;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    // ISSUE: method pointer
    ((UnityEvent<float>) ((Slider) this.slider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(UpdateSliderLabel)));
  }

  public void UpdateSliderLabel(float newValue)
  {
    if (this.isPercentValue)
      ((TMP_Text) this.valueLabel).text = (newValue * 100f).ToString("F0") + "%";
    else
      ((TMP_Text) this.valueLabel).text = newValue.ToString();
  }
}
