// Decompiled with JetBrains decompiler
// Type: GoodnessSlider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/GoodnessSlider")]
public class GoodnessSlider : KMonoBehaviour
{
  public Image icon;
  public Text text;
  public Slider slider;
  public Image fill;
  public Gradient gradient;
  public string[] names;

  protected virtual void OnSpawn()
  {
    this.Spawn();
    this.UpdateValues();
  }

  public void UpdateValues()
  {
    ((Graphic) this.text).color = ((Graphic) this.fill).color = this.gradient.Evaluate(this.slider.value);
    for (int index = 0; index < this.gradient.colorKeys.Length; ++index)
    {
      if ((double) this.gradient.colorKeys[index].time < (double) this.slider.value)
        this.text.text = this.names[index];
      if (index == this.gradient.colorKeys.Length - 1 && (double) this.gradient.colorKeys[index - 1].time < (double) this.slider.value)
        this.text.text = this.names[index];
    }
  }
}
