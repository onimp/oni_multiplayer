// Decompiled with JetBrains decompiler
// Type: CrewRationsEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using TMPro;
using UnityEngine;

public class CrewRationsEntry : CrewListEntry
{
  public KButton incRationPerDayButton;
  public KButton decRationPerDayButton;
  public LocText rationPerDayText;
  public LocText rationsEatenToday;
  public LocText currentCaloriesText;
  public LocText currentStressText;
  public LocText currentHealthText;
  public ValueTrendImageToggle stressTrendImage;
  private RationMonitor.Instance rationMonitor;

  public override void Populate(MinionIdentity _identity)
  {
    base.Populate(_identity);
    this.rationMonitor = ((Component) _identity).GetSMI<RationMonitor.Instance>();
    this.Refresh();
  }

  public override void Refresh()
  {
    base.Refresh();
    ((TMP_Text) this.rationsEatenToday).text = GameUtil.GetFormattedCalories(this.rationMonitor.GetRationsAteToday());
    if (Object.op_Equality((Object) this.identity, (Object) null))
      return;
    foreach (AmountInstance amount in (Modifications<Amount, AmountInstance>) this.identity.GetAmounts())
    {
      float min = amount.GetMin();
      float max = amount.GetMax();
      float num = max - min;
      string str = Mathf.RoundToInt((float) (((double) num - ((double) max - (double) amount.value)) / (double) num * 100.0)).ToString();
      if (amount.amount == Db.Get().Amounts.Stress)
      {
        ((TMP_Text) this.currentStressText).text = amount.GetValueString();
        ((Component) this.currentStressText).GetComponent<ToolTip>().toolTip = amount.GetTooltip();
        this.stressTrendImage.SetValue(amount);
      }
      else if (amount.amount == Db.Get().Amounts.Calories)
      {
        ((TMP_Text) this.currentCaloriesText).text = str + "%";
        ((Component) this.currentCaloriesText).GetComponent<ToolTip>().toolTip = amount.GetTooltip();
      }
      else if (amount.amount == Db.Get().Amounts.HitPoints)
      {
        ((TMP_Text) this.currentHealthText).text = str + "%";
        ((Component) this.currentHealthText).GetComponent<ToolTip>().toolTip = amount.GetTooltip();
      }
    }
  }
}
