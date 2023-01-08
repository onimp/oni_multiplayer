// Decompiled with JetBrains decompiler
// Type: SimulatedTemperatureAdjuster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedTemperatureAdjuster
{
  private float temperature;
  private float heatCapacity;
  private float thermalConductivity;
  private bool active;
  private Storage storage;

  public SimulatedTemperatureAdjuster(
    float simulated_temperature,
    float heat_capacity,
    float thermal_conductivity,
    Storage storage)
  {
    this.temperature = simulated_temperature;
    this.heatCapacity = heat_capacity;
    this.thermalConductivity = thermal_conductivity;
    this.storage = storage;
    KMonoBehaviourExtensions.Subscribe(((Component) storage).gameObject, 824508782, new Action<object>(this.OnActivechanged));
    KMonoBehaviourExtensions.Subscribe(((Component) storage).gameObject, -1697596308, new Action<object>(this.OnStorageChanged));
    this.OnActivechanged((object) ((Component) storage).gameObject.GetComponent<Operational>());
  }

  public List<Descriptor> GetDescriptors() => SimulatedTemperatureAdjuster.GetDescriptors(this.temperature);

  public static List<Descriptor> GetDescriptors(float temperature)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    string formattedTemperature = GameUtil.GetFormattedTemperature(temperature);
    Descriptor descriptor;
    // ISSUE: explicit constructor call
    ((Descriptor) ref descriptor).\u002Ector(string.Format((string) UI.BUILDINGEFFECTS.ITEM_TEMPERATURE_ADJUST, (object) formattedTemperature), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ITEM_TEMPERATURE_ADJUST, (object) formattedTemperature), (Descriptor.DescriptorType) 1, false);
    descriptors.Add(descriptor);
    return descriptors;
  }

  private void Register(SimTemperatureTransfer stt)
  {
    stt.onSimRegistered -= new Action<SimTemperatureTransfer>(this.OnItemSimRegistered);
    stt.onSimRegistered += new Action<SimTemperatureTransfer>(this.OnItemSimRegistered);
    if (!Sim.IsValidHandle(stt.SimHandle))
      return;
    this.OnItemSimRegistered(stt);
  }

  private void Unregister(SimTemperatureTransfer stt)
  {
    stt.onSimRegistered -= new Action<SimTemperatureTransfer>(this.OnItemSimRegistered);
    if (!Sim.IsValidHandle(stt.SimHandle))
      return;
    SimMessages.ModifyElementChunkTemperatureAdjuster(stt.SimHandle, 0.0f, 0.0f, 0.0f);
  }

  private void OnItemSimRegistered(SimTemperatureTransfer stt)
  {
    if (Object.op_Equality((Object) stt, (Object) null) || !Sim.IsValidHandle(stt.SimHandle))
      return;
    float temperature = this.temperature;
    float heat_capacity = this.heatCapacity;
    float thermal_conductivity = this.thermalConductivity;
    if (!this.active)
    {
      temperature = 0.0f;
      heat_capacity = 0.0f;
      thermal_conductivity = 0.0f;
    }
    SimMessages.ModifyElementChunkTemperatureAdjuster(stt.SimHandle, temperature, heat_capacity, thermal_conductivity);
  }

  private void OnActivechanged(object data)
  {
    this.active = ((Operational) data).IsActive;
    if (this.active)
    {
      foreach (GameObject gameObject in this.storage.items)
      {
        if (Object.op_Inequality((Object) gameObject, (Object) null))
          this.OnItemSimRegistered(gameObject.GetComponent<SimTemperatureTransfer>());
      }
    }
    else
    {
      foreach (GameObject gameObject in this.storage.items)
      {
        if (Object.op_Inequality((Object) gameObject, (Object) null))
          this.Unregister(gameObject.GetComponent<SimTemperatureTransfer>());
      }
    }
  }

  public void CleanUp()
  {
    KMonoBehaviourExtensions.Unsubscribe(((Component) this.storage).gameObject, -1697596308, new Action<object>(this.OnStorageChanged));
    foreach (GameObject gameObject in this.storage.items)
    {
      if (Object.op_Inequality((Object) gameObject, (Object) null))
        this.Unregister(gameObject.GetComponent<SimTemperatureTransfer>());
    }
  }

  private void OnStorageChanged(object data)
  {
    GameObject gameObject = (GameObject) data;
    SimTemperatureTransfer component1 = gameObject.GetComponent<SimTemperatureTransfer>();
    if (Object.op_Equality((Object) component1, (Object) null))
      return;
    Pickupable component2 = gameObject.GetComponent<Pickupable>();
    if (Object.op_Equality((Object) component2, (Object) null))
      return;
    if ((!this.active ? 0 : (Object.op_Equality((Object) component2.storage, (Object) this.storage) ? 1 : 0)) != 0)
      this.Register(component1);
    else
      this.Unregister(component1);
  }
}
