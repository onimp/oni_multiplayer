// Decompiled with JetBrains decompiler
// Type: StandardAmountDisplayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;

public class StandardAmountDisplayer : IAmountDisplayer
{
  protected StandardAttributeFormatter formatter;
  public GameUtil.IdentityDescriptorTense tense;

  public IAttributeFormatter Formatter => (IAttributeFormatter) this.formatter;

  public GameUtil.TimeSlice DeltaTimeSlice
  {
    get => this.formatter.DeltaTimeSlice;
    set => this.formatter.DeltaTimeSlice = value;
  }

  public StandardAmountDisplayer(
    GameUtil.UnitClass unitClass,
    GameUtil.TimeSlice deltaTimeSlice,
    StandardAttributeFormatter formatter = null,
    GameUtil.IdentityDescriptorTense tense = GameUtil.IdentityDescriptorTense.Normal)
  {
    this.tense = tense;
    if (formatter != null)
      this.formatter = formatter;
    else
      this.formatter = new StandardAttributeFormatter(unitClass, deltaTimeSlice);
  }

  public virtual string GetValueString(Amount master, AmountInstance instance) => !master.showMax ? this.formatter.GetFormattedValue(instance.value) : string.Format("{0} / {1}", (object) this.formatter.GetFormattedValue(instance.value), (object) this.formatter.GetFormattedValue(instance.GetMax()));

  public virtual string GetDescription(Amount master, AmountInstance instance) => string.Format("{0}: {1}", (object) master.Name, (object) this.GetValueString(master, instance));

  public virtual string GetTooltip(Amount master, AmountInstance instance)
  {
    string str = "";
    string tooltip = (master.description.IndexOf("{1}") <= -1 ? str + string.Format(master.description, (object) this.formatter.GetFormattedValue(instance.value)) : str + string.Format(master.description, (object) this.formatter.GetFormattedValue(instance.value), (object) GameUtil.GetIdentityDescriptor(instance.gameObject, this.tense))) + "\n\n";
    if (this.formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerCycle)
      tooltip += string.Format((string) UI.CHANGEPERCYCLE, (object) this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerCycle));
    else if (this.formatter.DeltaTimeSlice == GameUtil.TimeSlice.PerSecond)
      tooltip += string.Format((string) UI.CHANGEPERSECOND, (object) this.formatter.GetFormattedValue(instance.deltaAttribute.GetTotalDisplayValue(), GameUtil.TimeSlice.PerSecond));
    for (int index = 0; index != instance.deltaAttribute.Modifiers.Count; ++index)
    {
      AttributeModifier modifier = instance.deltaAttribute.Modifiers[index];
      tooltip = tooltip + "\n" + string.Format((string) UI.MODIFIER_ITEM_TEMPLATE, (object) modifier.GetDescription(), (object) this.formatter.GetFormattedModifier(modifier));
    }
    return tooltip;
  }

  public string GetFormattedAttribute(AttributeInstance instance) => this.formatter.GetFormattedAttribute(instance);

  public string GetFormattedModifier(AttributeModifier modifier) => this.formatter.GetFormattedModifier(modifier);

  public string GetFormattedValue(float value, GameUtil.TimeSlice time_slice) => this.formatter.GetFormattedValue(value, time_slice);
}
