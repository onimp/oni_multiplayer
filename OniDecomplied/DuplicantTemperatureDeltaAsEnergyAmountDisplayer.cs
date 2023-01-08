// Decompiled with JetBrains decompiler
// Type: DuplicantTemperatureDeltaAsEnergyAmountDisplayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;

public class DuplicantTemperatureDeltaAsEnergyAmountDisplayer : StandardAmountDisplayer
{
  public DuplicantTemperatureDeltaAsEnergyAmountDisplayer(
    GameUtil.UnitClass unitClass,
    GameUtil.TimeSlice timeSlice)
    : base(unitClass, timeSlice)
  {
  }

  public override string GetTooltip(Amount master, AmountInstance instance)
  {
    string str1 = string.Format(master.description, (object) this.formatter.GetFormattedValue(instance.value), (object) this.formatter.GetFormattedValue(310.15f));
    float num = (float) ((double) ElementLoader.FindElementByHash(SimHashes.Creature).specificHeatCapacity * 30.0 * 1000.0);
    string str2 = str1 + "\n\n";
    string tooltip = this.formatter.DeltaTimeSlice != GameUtil.TimeSlice.PerCycle ? str2 + string.Format((string) UI.CHANGEPERSECOND, (object) this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond)) + "\n" + string.Format((string) UI.CHANGEPERSECOND, (object) GameUtil.GetFormattedJoules(instance.deltaAttribute.GetTotalDisplayValue() * num)) : str2 + string.Format((string) UI.CHANGEPERCYCLE, (object) this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle));
    for (int index = 0; index != instance.deltaAttribute.Modifiers.Count; ++index)
    {
      AttributeModifier modifier = instance.deltaAttribute.Modifiers[index];
      tooltip = tooltip + "\n" + string.Format((string) UI.MODIFIER_ITEM_TEMPLATE, (object) modifier.GetDescription(), (object) GameUtil.GetFormattedHeatEnergyRate((float) ((double) modifier.Value * (double) num * 1.0)));
    }
    return tooltip;
  }
}
