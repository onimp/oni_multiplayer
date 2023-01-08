// Decompiled with JetBrains decompiler
// Type: FoodQualityAttributeFormatter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

public class FoodQualityAttributeFormatter : StandardAttributeFormatter
{
  public FoodQualityAttributeFormatter()
    : base(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None)
  {
  }

  public override string GetFormattedAttribute(AttributeInstance instance) => this.GetFormattedValue(instance.GetTotalDisplayValue());

  public override string GetFormattedModifier(AttributeModifier modifier) => GameUtil.GetFormattedInt(modifier.Value);

  public override string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice) => Util.StripTextFormatting(GameUtil.GetFormattedFoodQuality((int) value));
}
