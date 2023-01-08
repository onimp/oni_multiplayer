// Decompiled with JetBrains decompiler
// Type: PathProberSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class PathProberSensor : Sensor
{
  private Navigator navigator;

  public PathProberSensor(Sensors sensors)
    : base(sensors)
  {
    this.navigator = ((Component) sensors).GetComponent<Navigator>();
  }

  public override void Update() => this.navigator.UpdateProbe();
}
