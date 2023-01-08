// Decompiled with JetBrains decompiler
// Type: RocketModulePerformance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

[Serializable]
public class RocketModulePerformance
{
  public float burden;
  public float fuelKilogramPerDistance;
  public float enginePower;

  public RocketModulePerformance(float burden, float fuelKilogramPerDistance, float enginePower)
  {
    this.burden = burden;
    this.fuelKilogramPerDistance = fuelKilogramPerDistance;
    this.enginePower = enginePower;
  }

  public float Burden => this.burden;

  public float FuelKilogramPerDistance => this.fuelKilogramPerDistance;

  public float EnginePower => this.enginePower;
}
