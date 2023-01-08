// Decompiled with JetBrains decompiler
// Type: BreathableAreaSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class BreathableAreaSensor : Sensor
{
  private bool isBreathable;
  private OxygenBreather breather;

  public BreathableAreaSensor(Sensors sensors)
    : base(sensors)
  {
  }

  public override void Update()
  {
    if (Object.op_Equality((Object) this.breather, (Object) null))
      this.breather = this.GetComponent<OxygenBreather>();
    bool isBreathable = this.isBreathable;
    this.isBreathable = this.breather.IsBreathableElement || ((Component) this.breather).HasTag(GameTags.InTransitTube);
    if (this.isBreathable == isBreathable)
      return;
    if (this.isBreathable)
      this.Trigger(99949694);
    else
      this.Trigger(-1189351068);
  }

  public bool IsBreathable() => this.isBreathable;

  public bool IsUnderwater() => this.breather.IsUnderLiquid;
}
