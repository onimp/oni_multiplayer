// Decompiled with JetBrains decompiler
// Type: NonLinearSlider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NonLinearSlider : KSlider
{
  public NonLinearSlider.Range[] ranges;

  public static NonLinearSlider.Range[] GetDefaultRange(float maxValue) => new NonLinearSlider.Range[1]
  {
    new NonLinearSlider.Range(100f, maxValue)
  };

  protected virtual void Start()
  {
    ((UIBehaviour) this).Start();
    ((Slider) this).minValue = 0.0f;
    ((Slider) this).maxValue = 100f;
  }

  public void SetRanges(NonLinearSlider.Range[] ranges) => this.ranges = ranges;

  public float GetPercentageFromValue(float value)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int index = 0; index < this.ranges.Length; ++index)
    {
      if ((double) value >= (double) num2 && (double) value <= (double) this.ranges[index].peakValue)
      {
        float num3 = (float) (((double) value - (double) num2) / ((double) this.ranges[index].peakValue - (double) num2));
        return Mathf.Lerp(num1, num1 + this.ranges[index].width, num3);
      }
      num1 += this.ranges[index].width;
      num2 = this.ranges[index].peakValue;
    }
    return 100f;
  }

  public float GetValueForPercentage(float percentage)
  {
    float num1 = 0.0f;
    float valueForPercentage = 0.0f;
    for (int index = 0; index < this.ranges.Length; ++index)
    {
      if ((double) percentage >= (double) num1 && (double) num1 + (double) this.ranges[index].width >= (double) percentage)
      {
        float num2 = (percentage - num1) / this.ranges[index].width;
        return Mathf.Lerp(valueForPercentage, this.ranges[index].peakValue, num2);
      }
      num1 += this.ranges[index].width;
      valueForPercentage = this.ranges[index].peakValue;
    }
    return valueForPercentage;
  }

  protected virtual void Set(float input, bool sendCallback) => ((Slider) this).Set(input, sendCallback);

  [Serializable]
  public struct Range
  {
    public float width;
    public float peakValue;

    public Range(float width, float peakValue)
    {
      this.width = width;
      this.peakValue = peakValue;
    }
  }
}
