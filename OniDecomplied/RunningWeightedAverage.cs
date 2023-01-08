// Decompiled with JetBrains decompiler
// Type: RunningWeightedAverage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class RunningWeightedAverage
{
  private float[] samples;
  private float min;
  private float max;
  private bool ignoreZero;
  private int validValues;

  public RunningWeightedAverage(float minValue = -3.40282347E+38f, float maxValue = 3.40282347E+38f, int sampleCount = 15, bool allowZero = true)
  {
    this.min = minValue;
    this.max = maxValue;
    this.ignoreZero = !allowZero;
    this.samples = new float[sampleCount];
  }

  public float GetWeightedAverage => this.WeightedAverage();

  public float GetUnweightedAverage => this.WeightedAverage();

  public void AddSample(float value)
  {
    if (this.ignoreZero && (double) value == 0.0)
      return;
    if ((double) value > (double) this.max)
      value = this.max;
    if ((double) value < (double) this.min)
      value = this.min;
    if (this.validValues < this.samples.Length)
      ++this.validValues;
    for (int index = 0; index < this.samples.Length - 1; ++index)
      this.samples[index] = this.samples[index + 1];
    this.samples[this.samples.Length - 1] = value;
  }

  private float WeightedAverage()
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int index = this.samples.Length - 1; index > this.samples.Length - 1 - this.validValues; --index)
    {
      float num3 = (float) (index + 1) / ((float) this.validValues + 1f);
      num1 += this.samples[index] * num3;
      num2 += num3;
    }
    float f = num1 / num2;
    return float.IsNaN(f) ? 0.0f : f;
  }

  private float UnweightedAverage()
  {
    float num = 0.0f;
    for (int index = this.samples.Length - 1; index > this.samples.Length - 1 - this.validValues; --index)
      num += this.samples[index];
    float f = num / (float) this.samples.Length;
    return float.IsNaN(f) ? 0.0f : f;
  }
}
