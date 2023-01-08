// Decompiled with JetBrains decompiler
// Type: RadiationTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;

public class RadiationTracker : WorldTracker
{
  public RadiationTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData()
  {
    float num = 0.0f;
    List<MinionIdentity> worldItems = Components.MinionIdentities.GetWorldItems(this.WorldID);
    if (worldItems.Count == 0)
    {
      this.AddPoint(0.0f);
    }
    else
    {
      foreach (MinionIdentity cmp in worldItems)
        num += cmp.GetAmounts().Get(Db.Get().Amounts.RadiationBalance.Id).value;
      this.AddPoint(num / (float) worldItems.Count);
    }
  }

  public override string FormatValueString(float value) => GameUtil.GetFormattedRads(value);
}
