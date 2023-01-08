// Decompiled with JetBrains decompiler
// Type: AsPercentAmountDisplayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;

public class AsPercentAmountDisplayer : IAmountDisplayer
{
  protected StandardAttributeFormatter formatter;

  public IAttributeFormatter Formatter => (IAttributeFormatter) this.formatter;

  public GameUtil.TimeSlice DeltaTimeSlice
  {
    get => this.formatter.DeltaTimeSlice;
    set => this.formatter.DeltaTimeSlice = value;
  }

  public AsPercentAmountDisplayer(GameUtil.TimeSlice deltaTimeSlice) => this.formatter = new StandardAttributeFormatter(GameUtil.UnitClass.Percent, deltaTimeSlice);

  public string GetValueString(Amount master, AmountInstance instance) => this.formatter.GetFormattedValue(this.ToPercent(instance.value, instance));

  public virtual string GetDescription(Amount master, AmountInstance instance) => string.Format("{0}: {1}", (object) master.Name, (object) this.formatter.GetFormattedValue(this.ToPercent(instance.value, instance)));

  public virtual string GetTooltipDescription(Amount master, AmountInstance instance) => string.Format(master.description, (object) this.formatter.GetFormattedValue(instance.value));

  public virtual string GetTooltip(Amount master, AmountInstance instance)
  {
    string str = string.Format(master.description, (object) this.formatter.GetFormattedValue(instance.value)) + "\n\n";
    string tooltip = this.formatter.DeltaTimeSlice != GameUtil.TimeSlice.PerCycle ? str + string.Format((string) UI.CHANGEPERSECOND, (object) this.formatter.GetFormattedValue(this.ToPercent(instance.deltaAttribute.GetTotalDisplayValue(), instance), GameUtil.TimeSlice.PerSecond)) : str + string.Format((string) UI.CHANGEPERCYCLE, (object) this.formatter.GetFormattedValue(this.ToPercent(instance.deltaAttribute.GetTotalDisplayValue(), instance), GameUtil.TimeSlice.PerCycle));
    for (int index = 0; index != instance.deltaAttribute.Modifiers.Count; ++index)
    {
      AttributeModifier modifier = instance.deltaAttribute.Modifiers[index];
      float modifierContribution = instance.deltaAttribute.GetModifierContribution(modifier);
      tooltip = tooltip + "\n" + string.Format((string) UI.MODIFIER_ITEM_TEMPLATE, (object) modifier.GetDescription(), (object) this.formatter.GetFormattedValue(this.ToPercent(modifierContribution, instance), this.formatter.DeltaTimeSlice));
    }
    return tooltip;
  }

  public string GetFormattedAttribute(AttributeInstance instance) => this.formatter.GetFormattedAttribute(instance);

  public string GetFormattedModifier(AttributeModifier modifier) => modifier.IsMultiplier ? GameUtil.GetFormattedPercent(modifier.Value * 100f) : this.formatter.GetFormattedModifier(modifier);

  public string GetFormattedValue(float value, GameUtil.TimeSlice timeSlice) => this.formatter.GetFormattedValue(value, timeSlice);

  protected float ToPercent(float value, AmountInstance instance) => 100f * value / instance.GetMax();
}
