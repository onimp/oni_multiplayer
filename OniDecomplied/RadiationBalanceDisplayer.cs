// Decompiled with JetBrains decompiler
// Type: RadiationBalanceDisplayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using UnityEngine;

public class RadiationBalanceDisplayer : StandardAmountDisplayer
{
  public RadiationBalanceDisplayer()
    : base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle)
  {
    this.formatter = (StandardAttributeFormatter) new RadiationBalanceDisplayer.RadiationAttributeFormatter();
  }

  public override string GetValueString(Amount master, AmountInstance instance) => base.GetValueString(master, instance) + (string) UI.UNITSUFFIXES.RADIATION.RADS;

  public override string GetTooltip(Amount master, AmountInstance instance)
  {
    string tooltip = "";
    if (instance.gameObject.GetSMI<RadiationMonitor.Instance>() != null)
    {
      int cell = Grid.PosToCell(instance.gameObject);
      if (Grid.IsValidCell(cell))
        tooltip += (string) DUPLICANTS.STATS.RADIATIONBALANCE.TOOLTIP_CURRENT_BALANCE;
      string str = tooltip + "\n\n";
      float num = Mathf.Clamp01(1f - Db.Get().Attributes.RadiationResistance.Lookup(instance.gameObject).GetTotalValue());
      tooltip = str + string.Format((string) DUPLICANTS.STATS.RADIATIONBALANCE.CURRENT_EXPOSURE, (object) Mathf.RoundToInt(Grid.Radiation[cell] * num)) + "\n" + string.Format((string) DUPLICANTS.STATS.RADIATIONBALANCE.CURRENT_REJUVENATION, (object) Mathf.RoundToInt(Db.Get().Attributes.RadiationRecovery.Lookup(instance.gameObject).GetTotalValue() * 600f));
    }
    return tooltip;
  }

  public class RadiationAttributeFormatter : StandardAttributeFormatter
  {
    public RadiationAttributeFormatter()
      : base(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.PerCycle)
    {
    }
  }
}
