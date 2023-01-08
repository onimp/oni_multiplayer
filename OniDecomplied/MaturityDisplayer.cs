// Decompiled with JetBrains decompiler
// Type: MaturityDisplayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using UnityEngine;

public class MaturityDisplayer : AsPercentAmountDisplayer
{
  public MaturityDisplayer()
    : base(GameUtil.TimeSlice.PerCycle)
  {
    this.formatter = (StandardAttributeFormatter) new MaturityDisplayer.MaturityAttributeFormatter();
  }

  public override string GetTooltipDescription(Amount master, AmountInstance instance)
  {
    string tooltipDescription1 = base.GetTooltipDescription(master, instance);
    Growing component = instance.gameObject.GetComponent<Growing>();
    string tooltipDescription2;
    if (component.IsGrowing())
    {
      float seconds = (instance.GetMax() - instance.value) / instance.GetDelta();
      tooltipDescription2 = !Object.op_Inequality((Object) component, (Object) null) || !component.IsGrowing() ? tooltipDescription1 + string.Format((string) CREATURES.STATS.MATURITY.TOOLTIP_GROWING, (object) GameUtil.GetFormattedCycles(seconds)) : tooltipDescription1 + string.Format((string) CREATURES.STATS.MATURITY.TOOLTIP_GROWING_CROP, (object) GameUtil.GetFormattedCycles(seconds), (object) GameUtil.GetFormattedCycles(component.TimeUntilNextHarvest()));
    }
    else
      tooltipDescription2 = !component.ReachedNextHarvest() ? tooltipDescription1 + (string) CREATURES.STATS.MATURITY.TOOLTIP_STALLED : tooltipDescription1 + (string) CREATURES.STATS.MATURITY.TOOLTIP_GROWN;
    return tooltipDescription2;
  }

  public override string GetDescription(Amount master, AmountInstance instance)
  {
    Growing component = instance.gameObject.GetComponent<Growing>();
    return Object.op_Inequality((Object) component, (Object) null) && component.IsGrowing() ? string.Format((string) CREATURES.STATS.MATURITY.AMOUNT_DESC_FMT, (object) master.Name, (object) this.formatter.GetFormattedValue(this.ToPercent(instance.value, instance)), (object) GameUtil.GetFormattedCycles(component.TimeUntilNextHarvest())) : base.GetDescription(master, instance);
  }

  public class MaturityAttributeFormatter : StandardAttributeFormatter
  {
    public MaturityAttributeFormatter()
      : base(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None)
    {
    }

    public override string GetFormattedModifier(AttributeModifier modifier)
    {
      float num = modifier.Value;
      GameUtil.TimeSlice timeSlice = this.DeltaTimeSlice;
      if (modifier.IsMultiplier)
      {
        num *= 100f;
        timeSlice = GameUtil.TimeSlice.None;
      }
      return this.GetFormattedValue(num, timeSlice);
    }
  }
}
