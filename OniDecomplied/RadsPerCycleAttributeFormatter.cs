// Decompiled with JetBrains decompiler
// Type: RadsPerCycleAttributeFormatter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

public class RadsPerCycleAttributeFormatter : StandardAttributeFormatter
{
  public RadsPerCycleAttributeFormatter()
    : base(GameUtil.UnitClass.Radiation, GameUtil.TimeSlice.PerCycle)
  {
  }

  public override string GetFormattedAttribute(AttributeInstance instance) => this.GetFormattedValue(instance.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle);

  public override string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice) => base.GetFormattedValue(value / 600f, timeSlice);
}
