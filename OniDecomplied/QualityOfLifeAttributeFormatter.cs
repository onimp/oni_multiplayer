// Decompiled with JetBrains decompiler
// Type: QualityOfLifeAttributeFormatter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;

public class QualityOfLifeAttributeFormatter : StandardAttributeFormatter
{
  public QualityOfLifeAttributeFormatter()
    : base(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None)
  {
  }

  public override string GetFormattedAttribute(AttributeInstance instance)
  {
    AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(instance.gameObject);
    return string.Format((string) DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.DESC_FORMAT, (object) this.GetFormattedValue(instance.GetTotalDisplayValue()), (object) this.GetFormattedValue(attributeInstance.GetTotalDisplayValue()));
  }

  public override string GetTooltip(Attribute master, AttributeInstance instance)
  {
    string tooltip = base.GetTooltip(master, instance);
    AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(instance.gameObject);
    string str = tooltip + "\n\n" + string.Format((string) DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION, (object) this.GetFormattedValue(attributeInstance.GetTotalDisplayValue()));
    return (double) instance.GetTotalDisplayValue() - (double) attributeInstance.GetTotalDisplayValue() < 0.0 ? str + "\n\n" + (string) DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION_UNDER : str + "\n\n" + (string) DUPLICANTS.ATTRIBUTES.QUALITYOFLIFE.TOOLTIP_EXPECTATION_OVER;
  }
}
