// Decompiled with JetBrains decompiler
// Type: RocketFuelTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class RocketFuelTracker : WorldTracker
{
  public RocketFuelTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData()
  {
    Clustercraft component = ((Component) ClusterManager.Instance.GetWorld(this.WorldID)).GetComponent<Clustercraft>();
    this.AddPoint(Object.op_Inequality((Object) component, (Object) null) ? component.ModuleInterface.FuelRemaining : 0.0f);
  }

  public override string FormatValueString(float value) => GameUtil.GetFormattedMass(value);
}
