// Decompiled with JetBrains decompiler
// Type: RunningAverage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class RunningAverage
{
  private float[] samples;
  private float min;
  private float max;
  private bool ignoreZero;
  private int validValues;

  public RunningAverage(float minValue = -3.40282347E+38f, float maxValue = 3.40282347E+38f, int sampleCount = 15, bool allowZero = true)
  {
    this.min = minValue;
    this.max = maxValue;
    this.ignoreZero = !allowZero;
    this.samples = new float[sampleCount];
  }

  public float AverageValue => this.GetAverage();

  public void AddSample(float value)
  {
    if ((double) value < (double) this.min || (double) value > (double) this.max || this.ignoreZero && (double) value == 0.0)
      return;
    if (this.validValues < this.samples.Length)
      ++this.validValues;
    for (int index = 0; index < this.samples.Length - 1; ++index)
      this.samples[index] = this.samples[index + 1];
    this.samples[this.samples.Length - 1] = value;
  }

  private float GetAverage()
  {
    float num = 0.0f;
    for (int index = this.samples.Length - 1; index > this.samples.Length - 1 - this.validValues; --index)
      num += this.samples[index];
    return num / (float) this.validValues;
  }
}
