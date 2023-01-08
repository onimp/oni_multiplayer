// Decompiled with JetBrains decompiler
// Type: StructureTemperaturePayload
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public struct StructureTemperaturePayload
{
  public int simHandleCopy;
  public bool enabled;
  public bool bypass;
  public bool isActiveStatusItemSet;
  public bool overrideExtents;
  private PrimaryElement primaryElementBacking;
  public Overheatable overheatable;
  public Building building;
  public Operational operational;
  public List<StructureTemperaturePayload.EnergySource> energySourcesKW;
  public float pendingEnergyModifications;
  public float maxTemperature;
  public Extents overriddenExtents;

  public PrimaryElement primaryElement
  {
    get => this.primaryElementBacking;
    set
    {
      if (!Object.op_Inequality((Object) this.primaryElementBacking, (Object) value))
        return;
      this.primaryElementBacking = value;
      this.overheatable = ((Component) this.primaryElementBacking).GetComponent<Overheatable>();
    }
  }

  public StructureTemperaturePayload(GameObject go)
  {
    this.simHandleCopy = -1;
    this.enabled = true;
    this.bypass = false;
    this.overrideExtents = false;
    this.overriddenExtents = new Extents();
    this.primaryElementBacking = go.GetComponent<PrimaryElement>();
    this.overheatable = Object.op_Inequality((Object) this.primaryElementBacking, (Object) null) ? ((Component) this.primaryElementBacking).GetComponent<Overheatable>() : (Overheatable) null;
    this.building = go.GetComponent<Building>();
    this.operational = go.GetComponent<Operational>();
    this.pendingEnergyModifications = 0.0f;
    this.maxTemperature = 10000f;
    this.energySourcesKW = (List<StructureTemperaturePayload.EnergySource>) null;
    this.isActiveStatusItemSet = false;
  }

  public float TotalEnergyProducedKW
  {
    get
    {
      if (this.energySourcesKW == null || this.energySourcesKW.Count == 0)
        return 0.0f;
      float energyProducedKw = 0.0f;
      for (int index = 0; index < this.energySourcesKW.Count; ++index)
        energyProducedKw += this.energySourcesKW[index].value;
      return energyProducedKw;
    }
  }

  public void OverrideExtents(Extents newExtents)
  {
    this.overrideExtents = true;
    this.overriddenExtents = newExtents;
  }

  public Extents GetExtents() => !this.overrideExtents ? this.building.GetExtents() : this.overriddenExtents;

  public float Temperature => this.primaryElement.Temperature;

  public float ExhaustKilowatts => this.building.Def.ExhaustKilowattsWhenActive;

  public float OperatingKilowatts => !Object.op_Inequality((Object) this.operational, (Object) null) || !this.operational.IsActive ? 0.0f : this.building.Def.SelfHeatKilowattsWhenActive;

  public class EnergySource
  {
    public string source;
    public RunningAverage kw_accumulator;

    public EnergySource(float kj, string source)
    {
      this.source = source;
      this.kw_accumulator = new RunningAverage(sampleCount: Mathf.RoundToInt(186f));
    }

    public float value => this.kw_accumulator.AverageValue;

    public void Accumulate(float value) => this.kw_accumulator.AddSample(value);
  }
}
