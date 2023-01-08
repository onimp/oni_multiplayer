// Decompiled with JetBrains decompiler
// Type: KCalTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class KCalTracker : WorldTracker
{
  public KCalTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData() => this.AddPoint(RationTracker.Get().CountRations((Dictionary<string, float>) null, ClusterManager.Instance.GetWorld(this.WorldID).worldInventory));

  public override string FormatValueString(float value) => GameUtil.GetFormattedCalories(value);
}
