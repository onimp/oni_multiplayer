// Decompiled with JetBrains decompiler
// Type: PercentAttributeFormatter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

public class PercentAttributeFormatter : StandardAttributeFormatter
{
  public PercentAttributeFormatter()
    : base(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None)
  {
  }

  public override string GetFormattedAttribute(AttributeInstance instance) => this.GetFormattedValue(instance.GetTotalDisplayValue(), this.DeltaTimeSlice);

  public override string GetFormattedModifier(AttributeModifier modifier) => this.GetFormattedValue(modifier.Value, this.DeltaTimeSlice);

  public override string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice) => GameUtil.GetFormattedPercent(value * 100f, timeSlice);
}
