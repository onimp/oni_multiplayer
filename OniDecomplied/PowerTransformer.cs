// Decompiled with JetBrains decompiler
// Type: PowerTransformer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{name}")]
public class PowerTransformer : Generator
{
  private Battery battery;
  private bool mLoopDetected;
  private static readonly EventSystem.IntraObjectHandler<PowerTransformer> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<PowerTransformer>((Action<PowerTransformer, object>) ((component, data) => component.OnOperationalChanged(data)));

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.battery = ((Component) this).GetComponent<Battery>();
    this.Subscribe<PowerTransformer>(-592767678, PowerTransformer.OnOperationalChangedDelegate);
    this.UpdateJoulesLostPerSecond();
  }

  public override void ApplyDeltaJoules(float joules_delta, bool can_over_power = false)
  {
    this.battery.ConsumeEnergy(-joules_delta);
    base.ApplyDeltaJoules(joules_delta, can_over_power);
  }

  public override void ConsumeEnergy(float joules)
  {
    this.battery.ConsumeEnergy(joules);
    base.ConsumeEnergy(joules);
  }

  private void OnOperationalChanged(object data) => this.UpdateJoulesLostPerSecond();

  private void UpdateJoulesLostPerSecond()
  {
    if (this.operational.IsOperational)
      this.battery.joulesLostPerSecond = 0.0f;
    else
      this.battery.joulesLostPerSecond = 3.33333325f;
  }

  public override void EnergySim200ms(float dt)
  {
    base.EnergySim200ms(dt);
    this.AssignJoulesAvailable(this.operational.IsOperational ? Math.Min(this.battery.JoulesAvailable, this.WattageRating * dt) : 0.0f);
    ushort circuitId1 = this.battery.CircuitID;
    ushort circuitId2 = this.CircuitID;
    bool flag = (int) circuitId1 == (int) circuitId2 && circuitId1 != ushort.MaxValue;
    if (this.mLoopDetected == flag)
      return;
    this.mLoopDetected = flag;
    this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.PowerLoopDetected, this.mLoopDetected, (object) this);
  }
}
