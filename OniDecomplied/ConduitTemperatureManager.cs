// Decompiled with JetBrains decompiler
// Type: ConduitTemperatureManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

public class ConduitTemperatureManager
{
  private float[] temperatures = new float[0];
  private ConduitTemperatureManager.ConduitInfo[] conduitInfo = new ConduitTemperatureManager.ConduitInfo[0];

  public ConduitTemperatureManager() => ConduitTemperatureManager.ConduitTemperatureManager_Initialize();

  public void Shutdown() => ConduitTemperatureManager.ConduitTemperatureManager_Shutdown();

  public HandleVector<int>.Handle Allocate(
    ConduitType conduit_type,
    int conduit_idx,
    HandleVector<int>.Handle conduit_structure_temperature_handle,
    ref ConduitFlow.ConduitContents contents)
  {
    StructureTemperaturePayload payload = ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetPayload(conduit_structure_temperature_handle);
    Element element = payload.primaryElement.Element;
    BuildingDef def = payload.building.Def;
    float conduit_heat_capacity = def.MassForTemperatureModification * element.specificHeatCapacity;
    float conduit_thermal_conductivity = element.thermalConductivity * def.ThermalConductivity;
    int h = ConduitTemperatureManager.ConduitTemperatureManager_Add(contents.temperature, contents.mass, (int) contents.element, payload.simHandleCopy, conduit_heat_capacity, conduit_thermal_conductivity, (double) def.ThermalConductivity < 1.0);
    HandleVector<int>.Handle handle = new HandleVector<int>.Handle();
    handle.index = h;
    int handleIndex = Sim.GetHandleIndex(h);
    if (handleIndex + 1 > this.temperatures.Length)
    {
      Array.Resize<float>(ref this.temperatures, (handleIndex + 1) * 2);
      Array.Resize<ConduitTemperatureManager.ConduitInfo>(ref this.conduitInfo, (handleIndex + 1) * 2);
    }
    this.temperatures[handleIndex] = contents.temperature;
    this.conduitInfo[handleIndex] = new ConduitTemperatureManager.ConduitInfo()
    {
      type = conduit_type,
      idx = conduit_idx
    };
    return handle;
  }

  public void SetData(HandleVector<int>.Handle handle, ref ConduitFlow.ConduitContents contents)
  {
    if (!handle.IsValid())
      return;
    this.temperatures[Sim.GetHandleIndex(handle.index)] = contents.temperature;
    ConduitTemperatureManager.ConduitTemperatureManager_Set(handle.index, contents.temperature, contents.mass, (int) contents.element);
  }

  public void Free(HandleVector<int>.Handle handle)
  {
    if (!handle.IsValid())
      return;
    int handleIndex = Sim.GetHandleIndex(handle.index);
    this.temperatures[handleIndex] = -1f;
    this.conduitInfo[handleIndex] = new ConduitTemperatureManager.ConduitInfo()
    {
      type = ConduitType.None,
      idx = -1
    };
    ConduitTemperatureManager.ConduitTemperatureManager_Remove(handle.index);
  }

  public void Clear() => ConduitTemperatureManager.ConduitTemperatureManager_Clear();

  public unsafe void Sim200ms(float dt)
  {
    ConduitTemperatureManager.ConduitTemperatureUpdateData* temperatureUpdateDataPtr = (ConduitTemperatureManager.ConduitTemperatureUpdateData*) (void*) ConduitTemperatureManager.ConduitTemperatureManager_Update(dt, (IntPtr) (void*) Game.Instance.simData.buildingTemperatures);
    int numEntries = temperatureUpdateDataPtr->numEntries;
    if (numEntries > 0)
      Marshal.Copy((IntPtr) (void*) temperatureUpdateDataPtr->temperatures, this.temperatures, 0, numEntries);
    for (int index = 0; index < temperatureUpdateDataPtr->numFrozenHandles; ++index)
    {
      ConduitTemperatureManager.ConduitInfo conduitInfo = this.conduitInfo[Sim.GetHandleIndex(temperatureUpdateDataPtr->frozenHandles[index])];
      Conduit.GetFlowManager(conduitInfo.type).FreezeConduitContents(conduitInfo.idx);
    }
    for (int index = 0; index < temperatureUpdateDataPtr->numMeltedHandles; ++index)
    {
      ConduitTemperatureManager.ConduitInfo conduitInfo = this.conduitInfo[Sim.GetHandleIndex(temperatureUpdateDataPtr->meltedHandles[index])];
      Conduit.GetFlowManager(conduitInfo.type).MeltConduitContents(conduitInfo.idx);
    }
  }

  public float GetTemperature(HandleVector<int>.Handle handle) => this.temperatures[Sim.GetHandleIndex(handle.index)];

  [DllImport("SimDLL")]
  private static extern void ConduitTemperatureManager_Initialize();

  [DllImport("SimDLL")]
  private static extern void ConduitTemperatureManager_Shutdown();

  [DllImport("SimDLL")]
  private static extern int ConduitTemperatureManager_Add(
    float contents_temperature,
    float contents_mass,
    int contents_element_hash,
    int conduit_structure_temperature_handle,
    float conduit_heat_capacity,
    float conduit_thermal_conductivity,
    bool conduit_insulated);

  [DllImport("SimDLL")]
  private static extern int ConduitTemperatureManager_Set(
    int handle,
    float contents_temperature,
    float contents_mass,
    int contents_element_hash);

  [DllImport("SimDLL")]
  private static extern void ConduitTemperatureManager_Remove(int handle);

  [DllImport("SimDLL")]
  private static extern IntPtr ConduitTemperatureManager_Update(
    float dt,
    IntPtr building_conductivity_data);

  [DllImport("SimDLL")]
  private static extern void ConduitTemperatureManager_Clear();

  private struct ConduitInfo
  {
    public ConduitType type;
    public int idx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct ConduitTemperatureUpdateData
  {
    public int numEntries;
    public unsafe float* temperatures;
    public int numFrozenHandles;
    public unsafe int* frozenHandles;
    public int numMeltedHandles;
    public unsafe int* meltedHandles;
  }
}
