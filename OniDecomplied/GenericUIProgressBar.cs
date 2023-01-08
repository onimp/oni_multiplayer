// Decompiled with JetBrains decompiler
// Type: GenericUIProgressBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/GenericUIProgressBar")]
public class GenericUIProgressBar : KMonoBehaviour
{
  public Image fill;
  public LocText label;
  private float maxValue;

  public void SetMaxValue(float max) => this.maxValue = max;

  public void SetFillPercentage(float value)
  {
    this.fill.fillAmount = value;
    ((TMP_Text) this.label).text = Util.FormatWholeNumber(Mathf.Min(this.maxValue, this.maxValue * value)) + "/" + this.maxValue.ToString();
  }
}
