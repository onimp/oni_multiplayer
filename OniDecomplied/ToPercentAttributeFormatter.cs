// Decompiled with JetBrains decompiler
// Type: ToPercentAttributeFormatter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

public class ToPercentAttributeFormatter : StandardAttributeFormatter
{
  public float max = 1f;

  public ToPercentAttributeFormatter(float max, GameUtil.TimeSlice deltaTimeSlice = GameUtil.TimeSlice.None)
    : base(GameUtil.UnitClass.Percent, deltaTimeSlice)
  {
    this.max = max;
  }

  public override string GetFormattedAttribute(AttributeInstance instance) => this.GetFormattedValue(instance.GetTotalDisplayValue(), this.DeltaTimeSlice);

  public override string GetFormattedModifier(AttributeModifier modifier) => this.GetFormattedValue(modifier.Value, this.DeltaTimeSlice);

  public override string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice) => GameUtil.GetFormattedPercent((float) ((double) value / (double) this.max * 100.0), timeSlice);
}
