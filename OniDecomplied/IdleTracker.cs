// Decompiled with JetBrains decompiler
// Type: IdleTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class IdleTracker : WorldTracker
{
  public IdleTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData()
  {
    int num = 0;
    List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(this.WorldID);
    for (int index = 0; index < worldItems.Count; ++index)
    {
      if (((Component) worldItems[index]).HasTag(GameTags.Idle))
        ++num;
    }
    this.AddPoint((float) num);
  }

  public override string FormatValueString(float value) => value.ToString();
}
