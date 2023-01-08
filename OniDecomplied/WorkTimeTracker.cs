// Decompiled with JetBrains decompiler
// Type: WorkTimeTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkTimeTracker : WorldTracker
{
  public ChoreGroup choreGroup;

  public WorkTimeTracker(int worldID, ChoreGroup group)
    : base(worldID)
  {
    this.choreGroup = group;
  }

  public override void UpdateData()
  {
    float num = 0.0f;
    List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(this.WorldID);
    foreach (MinionIdentity minionIdentity in worldItems)
    {
      Chore chore = ((Component) minionIdentity).GetComponent<ChoreConsumer>().choreDriver.GetCurrentChore();
      if (chore != null && this.choreGroup.choreTypes.Find((Predicate<ChoreType>) (match => match == chore.choreType)) != null)
        ++num;
    }
    this.AddPoint((float) ((double) num / (double) worldItems.Count * 100.0));
  }

  public override string FormatValueString(float value) => GameUtil.GetFormattedPercent(Mathf.Round(value)).ToString();
}
