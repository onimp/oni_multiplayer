// Decompiled with JetBrains decompiler
// Type: Reservoir
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Reservoir")]
public class Reservoir : KMonoBehaviour
{
  private MeterController meter;
  [MyCmpGet]
  private Storage storage;
  private static readonly EventSystem.IntraObjectHandler<Reservoir> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<Reservoir>((Action<Reservoir, object>) ((component, data) => component.OnStorageChange(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[2]
    {
      "meter_fill",
      "meter_OL"
    });
    this.Subscribe<Reservoir>(-1697596308, Reservoir.OnStorageChangeDelegate);
    this.OnStorageChange((object) null);
  }

  private void OnStorageChange(object data) => this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg));
}
