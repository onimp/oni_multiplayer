// Decompiled with JetBrains decompiler
// Type: DataPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public struct DataPoint
{
  public float periodStart;
  public float periodEnd;
  public float periodValue;

  public DataPoint(float start, float end, float value)
  {
    this.periodStart = start;
    this.periodEnd = end;
    this.periodValue = value;
  }
}
