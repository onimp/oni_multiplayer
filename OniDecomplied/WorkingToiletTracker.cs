// Decompiled with JetBrains decompiler
// Type: WorkingToiletTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class WorkingToiletTracker : WorldTracker
{
  public WorkingToiletTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData()
  {
    int num = 0;
    List<IUsable> worldItems = Components.Toilets.GetWorldItems(this.WorldID);
    for (int index = 0; index < worldItems.Count; ++index)
    {
      if (worldItems[index].IsUsable())
        ++num;
    }
    this.AddPoint((float) num);
  }

  public override string FormatValueString(float value) => value.ToString();
}
