// Decompiled with JetBrains decompiler
// Type: IAttributeFormatter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;

public interface IAttributeFormatter
{
  GameUtil.TimeSlice DeltaTimeSlice { get; set; }

  string GetFormattedAttribute(AttributeInstance instance);

  string GetFormattedModifier(AttributeModifier modifier);

  string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice);

  string GetTooltip(Attribute master, AttributeInstance instance);

  string GetTooltip(
    Attribute master,
    List<AttributeModifier> modifiers,
    AttributeConverters converters);
}
