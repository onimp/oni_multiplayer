// Decompiled with JetBrains decompiler
// Type: GermResistanceAttributeFormatter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

public class GermResistanceAttributeFormatter : StandardAttributeFormatter
{
  public GermResistanceAttributeFormatter()
    : base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None)
  {
  }

  public override string GetFormattedModifier(AttributeModifier modifier) => GameUtil.GetGermResistanceModifierString(modifier.Value, false);
}
