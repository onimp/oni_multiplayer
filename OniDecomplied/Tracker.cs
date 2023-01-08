// Decompiled with JetBrains decompiler
// Type: Tracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tracker
{
  private const int standardSampleRate = 4;
  private const int defaultCyclesTracked = 5;
  protected List<DataPoint> dataPoints = new List<DataPoint>();
  private int maxPoints = Mathf.CeilToInt(750f);

  public Tuple<float, float>[] ChartableData(float periodLength)
  {
    float time = GameClock.Instance.GetTime();
    List<Tuple<float, float>> tupleList = new List<Tuple<float, float>>();
    for (int index = this.dataPoints.Count - 1; index >= 0 && (double) this.dataPoints[index].periodStart >= (double) time - (double) periodLength; --index)
      tupleList.Add(new Tuple<float, float>(this.dataPoints[index].periodStart, this.dataPoints[index].periodValue));
    if (tupleList.Count == 0)
    {
      if (this.dataPoints.Count > 0)
        tupleList.Add(new Tuple<float, float>(this.dataPoints[this.dataPoints.Count - 1].periodStart, this.dataPoints[this.dataPoints.Count - 1].periodValue));
      else
        tupleList.Add(new Tuple<float, float>(0.0f, 0.0f));
    }
    tupleList.Reverse();
    return tupleList.ToArray();
  }

  public float GetDataTimeLength()
  {
    float dataTimeLength = 0.0f;
    for (int index = this.dataPoints.Count - 1; index >= 0; --index)
      dataTimeLength += this.dataPoints[index].periodEnd - this.dataPoints[index].periodStart;
    return dataTimeLength;
  }

  public abstract void UpdateData();

  public abstract string FormatValueString(float value);

  public float GetCurrentValue() => this.dataPoints.Count == 0 ? 0.0f : this.dataPoints[this.dataPoints.Count - 1].periodValue;

  public float GetMinValue(float sampleHistoryLengthSeconds)
  {
    float time = GameClock.Instance.GetTime();
    Tuple<float, float>[] tupleArray = this.ChartableData(sampleHistoryLengthSeconds);
    if (tupleArray.Length == 0)
      return 0.0f;
    if (tupleArray.Length == 1)
      return tupleArray[0].second;
    float minValue = tupleArray[tupleArray.Length - 1].second;
    for (int index = tupleArray.Length - 1; index >= 0 && (double) time - (double) tupleArray[index].first <= (double) sampleHistoryLengthSeconds; --index)
      minValue = Mathf.Min(minValue, tupleArray[index].second);
    return minValue;
  }

  public float GetMaxValue(int sampleHistoryLengthSeconds)
  {
    float time = GameClock.Instance.GetTime();
    Tuple<float, float>[] tupleArray = this.ChartableData((float) sampleHistoryLengthSeconds);
    if (tupleArray.Length == 0)
      return 0.0f;
    if (tupleArray.Length == 1)
      return tupleArray[0].second;
    float maxValue = tupleArray[tupleArray.Length - 1].second;
    for (int index = tupleArray.Length - 1; index >= 0 && (double) time - (double) tupleArray[index].first <= (double) sampleHistoryLengthSeconds; --index)
      maxValue = Mathf.Max(maxValue, tupleArray[index].second);
    return maxValue;
  }

  public float GetAverageValue(float sampleHistoryLengthSeconds)
  {
    float time = GameClock.Instance.GetTime();
    Tuple<float, float>[] tupleArray = this.ChartableData(sampleHistoryLengthSeconds);
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int index = tupleArray.Length - 1; index >= 0; --index)
    {
      if ((double) tupleArray[index].first >= (double) time - (double) sampleHistoryLengthSeconds)
      {
        float num3 = index == tupleArray.Length - 1 ? time - tupleArray[index].first : tupleArray[index + 1].first - tupleArray[index].first;
        num2 += num3;
        if (!float.IsNaN(tupleArray[index].second))
          num1 += num3 * tupleArray[index].second;
      }
    }
    return (double) num2 != 0.0 ? num1 / num2 : (tupleArray.Length != 0 ? tupleArray[tupleArray.Length - 1].second : 0.0f);
  }

  public float GetDelta(float secondsAgo)
  {
    float time = GameClock.Instance.GetTime();
    Tuple<float, float>[] tupleArray = this.ChartableData(secondsAgo);
    if (tupleArray.Length < 2)
      return 0.0f;
    float num = -1f;
    float second = tupleArray[tupleArray.Length - 1].second;
    for (int index = tupleArray.Length - 1; index >= 0; --index)
    {
      if ((double) time - (double) tupleArray[index].first >= (double) secondsAgo)
        num = tupleArray[index].second;
    }
    return second - num;
  }

  protected void AddPoint(float value)
  {
    if (float.IsNaN(value))
      value = 0.0f;
    this.dataPoints.Add(new DataPoint(this.dataPoints.Count == 0 ? GameClock.Instance.GetTime() : this.dataPoints[this.dataPoints.Count - 1].periodEnd, GameClock.Instance.GetTime(), value));
    this.dataPoints.RemoveRange(0, Math.Max(0, this.dataPoints.Count - this.maxPoints));
  }

  public List<DataPoint> GetCompressedData()
  {
    int num1 = 10;
    List<DataPoint> compressedData = new List<DataPoint>();
    float num2 = (this.dataPoints[this.dataPoints.Count - 1].periodEnd - this.dataPoints[0].periodStart) / (float) num1;
    for (int index1 = 0; index1 < num1; ++index1)
    {
      float start = num2 * (float) index1;
      float end = start + num2;
      float num3 = 0.0f;
      for (int index2 = 0; index2 < this.dataPoints.Count; ++index2)
      {
        DataPoint dataPoint = this.dataPoints[index2];
        num3 += dataPoint.periodValue * Mathf.Max(0.0f, Mathf.Min(end, dataPoint.periodEnd) - Mathf.Max(dataPoint.periodStart, start));
      }
      compressedData.Add(new DataPoint(start, end, num3 / (end - start)));
    }
    return compressedData;
  }

  public void OverwriteData(List<DataPoint> newData) => this.dataPoints = newData;
}
