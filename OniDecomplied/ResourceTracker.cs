// Decompiled with JetBrains decompiler
// Type: ResourceTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ResourceTracker : WorldTracker
{
  public Tag tag { get; private set; }

  public ResourceTracker(int worldID, Tag materialCategoryTag)
    : base(worldID)
  {
    this.tag = materialCategoryTag;
  }

  public override void UpdateData()
  {
    if (Object.op_Equality((Object) ClusterManager.Instance.GetWorld(this.WorldID).worldInventory, (Object) null))
      return;
    this.AddPoint(ClusterManager.Instance.GetWorld(this.WorldID).worldInventory.GetAmount(this.tag, false));
  }

  public override string FormatValueString(float value) => GameUtil.GetFormattedMass(value);
}
