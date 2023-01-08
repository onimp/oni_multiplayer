// Decompiled with JetBrains decompiler
// Type: StressTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class StressTracker : WorldTracker
{
  public StressTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData()
  {
    float num = 0.0f;
    for (int idx = 0; idx < Components.LiveMinionIdentities.Count; ++idx)
    {
      if (Components.LiveMinionIdentities[idx].GetMyWorldId() == this.WorldID)
        num = Mathf.Max(num, ((Component) Components.LiveMinionIdentities[idx]).gameObject.GetAmounts().GetValue(Db.Get().Amounts.Stress.Id));
    }
    this.AddPoint(Mathf.Round(num));
  }

  public override string FormatValueString(float value) => value.ToString() + "%";
}
