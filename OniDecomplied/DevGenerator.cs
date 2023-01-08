// Decompiled with JetBrains decompiler
// Type: DevGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class DevGenerator : Generator
{
  public float wattageRating = 100000f;

  public override void EnergySim200ms(float dt)
  {
    base.EnergySim200ms(dt);
    ushort circuitId = this.CircuitID;
    this.operational.SetFlag(Generator.wireConnectedFlag, circuitId != ushort.MaxValue);
    if (!this.operational.IsOperational)
      return;
    float wattageRating = this.wattageRating;
    if ((double) wattageRating <= 0.0)
      return;
    this.GenerateJoules(Mathf.Max(wattageRating * dt, 1f * dt));
  }
}
