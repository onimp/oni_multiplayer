// Decompiled with JetBrains decompiler
// Type: PickupableSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class PickupableSensor : Sensor
{
  private PathProber pathProber;
  private Worker worker;

  public PickupableSensor(Sensors sensors)
    : base(sensors)
  {
    this.worker = this.GetComponent<Worker>();
    this.pathProber = this.GetComponent<PathProber>();
  }

  public override void Update()
  {
    GlobalChoreProvider.Instance.UpdateFetches(this.pathProber);
    Game.Instance.fetchManager.UpdatePickups(this.pathProber, this.worker);
  }
}
