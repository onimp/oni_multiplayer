// Decompiled with JetBrains decompiler
// Type: NonEssentialEnergyConsumer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class NonEssentialEnergyConsumer : EnergyConsumer
{
  public Action<bool> PoweredStateChanged;
  private bool isPowered;

  public override bool IsPowered
  {
    get => this.isPowered;
    protected set
    {
      if (value == this.isPowered)
        return;
      this.isPowered = value;
      Action<bool> poweredStateChanged = this.PoweredStateChanged;
      if (poweredStateChanged == null)
        return;
      poweredStateChanged(this.isPowered);
    }
  }
}
