// Decompiled with JetBrains decompiler
// Type: DecorDisplayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;

public class DecorDisplayer : StandardAmountDisplayer
{
  public DecorDisplayer()
    : base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle)
  {
    this.formatter = (StandardAttributeFormatter) new DecorDisplayer.DecorAttributeFormatter();
  }

  public override string GetTooltip(Amount master, AmountInstance instance)
  {
    string str = string.Format(LocText.ParseText(master.description), (object) this.formatter.GetFormattedValue(instance.value));
    int cell = Grid.PosToCell(instance.gameObject);
    if (Grid.IsValidCell(cell))
      str += string.Format((string) DUPLICANTS.STATS.DECOR.TOOLTIP_CURRENT, (object) GameUtil.GetDecorAtCell(cell));
    string tooltip = str + "\n";
    DecorMonitor.Instance smi = instance.gameObject.GetSMI<DecorMonitor.Instance>();
    if (smi != null)
      tooltip = tooltip + string.Format((string) DUPLICANTS.STATS.DECOR.TOOLTIP_AVERAGE_TODAY, (object) this.formatter.GetFormattedValue(smi.GetTodaysAverageDecor())) + string.Format((string) DUPLICANTS.STATS.DECOR.TOOLTIP_AVERAGE_YESTERDAY, (object) this.formatter.GetFormattedValue(smi.GetYesterdaysAverageDecor()));
    return tooltip;
  }

  public class DecorAttributeFormatter : StandardAttributeFormatter
  {
    public DecorAttributeFormatter()
      : base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle)
    {
    }
  }
}
