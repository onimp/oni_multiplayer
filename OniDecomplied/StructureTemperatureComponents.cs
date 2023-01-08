// Decompiled with JetBrains decompiler
// Type: StructureTemperatureComponents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StructureTemperatureComponents : 
  KGameObjectSplitComponentManager<StructureTemperatureHeader, StructureTemperaturePayload>
{
  private const float MAX_PRESSURE = 1.5f;
  private static Dictionary<int, HandleVector<int>.Handle> handleInstanceMap = new Dictionary<int, HandleVector<int>.Handle>();
  private StatusItem operatingEnergyStatusItem;

  public HandleVector<int>.Handle Add(GameObject go)
  {
    StructureTemperaturePayload temperaturePayload = new StructureTemperaturePayload(go);
    return this.Add(go, new StructureTemperatureHeader()
    {
      dirty = false,
      simHandle = -1,
      isActiveBuilding = false
    }, ref temperaturePayload);
  }

  public static void ClearInstanceMap() => StructureTemperatureComponents.handleInstanceMap.Clear();

  protected virtual void OnPrefabInit(HandleVector<int>.Handle handle)
  {
    this.InitializeStatusItem();
    ((KSplitComponentManager<StructureTemperatureHeader, StructureTemperaturePayload>) this).OnPrefabInit(handle);
    StructureTemperatureHeader temperatureHeader;
    StructureTemperaturePayload temperaturePayload;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).GetData(handle, ref temperatureHeader, ref temperaturePayload);
    temperaturePayload.primaryElement.getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(StructureTemperatureComponents.OnGetTemperature);
    temperaturePayload.primaryElement.setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(StructureTemperatureComponents.OnSetTemperature);
    temperatureHeader.isActiveBuilding = (double) temperaturePayload.building.Def.SelfHeatKilowattsWhenActive != 0.0 || (double) temperaturePayload.ExhaustKilowatts != 0.0;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).SetHeader(handle, temperatureHeader);
  }

  private void InitializeStatusItem()
  {
    if (this.operatingEnergyStatusItem != null)
      return;
    this.operatingEnergyStatusItem = new StatusItem("OperatingEnergy", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
    this.operatingEnergyStatusItem.resolveStringCallback = (Func<string, object, string>) ((str, ev_data) =>
    {
      int key = (int) ev_data;
      StructureTemperaturePayload payload = ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).GetPayload(StructureTemperatureComponents.handleInstanceMap[key]);
      if (str != (string) BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP)
      {
        try
        {
          str = string.Format(str, (object) GameUtil.GetFormattedHeatEnergy(payload.TotalEnergyProducedKW * 1000f));
        }
        catch (Exception ex)
        {
          Debug.LogWarning((object) ex);
          Debug.LogWarning((object) BUILDING.STATUSITEMS.OPERATINGENERGY.TOOLTIP);
          Debug.LogWarning((object) str);
        }
      }
      else
      {
        string str1 = "";
        foreach (StructureTemperaturePayload.EnergySource energySource in payload.energySourcesKW)
          str1 += string.Format((string) BUILDING.STATUSITEMS.OPERATINGENERGY.LINEITEM, (object) energySource.source, (object) GameUtil.GetFormattedHeatEnergy(energySource.value * 1000f, GameUtil.HeatEnergyFormatterUnit.DTU_S));
        str = string.Format(str, (object) GameUtil.GetFormattedHeatEnergy(payload.TotalEnergyProducedKW * 1000f, GameUtil.HeatEnergyFormatterUnit.DTU_S), (object) str1);
      }
      return str;
    });
  }

  protected virtual void OnSpawn(HandleVector<int>.Handle handle)
  {
    StructureTemperatureHeader header;
    StructureTemperaturePayload payload;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).GetData(handle, ref header, ref payload);
    if (Object.op_Inequality((Object) payload.operational, (Object) null) && header.isActiveBuilding)
      payload.primaryElement.Subscribe(824508782, (Action<object>) (ev_data => StructureTemperatureComponents.OnActiveChanged(handle)));
    payload.maxTemperature = Object.op_Inequality((Object) payload.overheatable, (Object) null) ? payload.overheatable.OverheatTemperature : 10000f;
    if ((double) payload.maxTemperature <= 0.0)
      Debug.LogError((object) "invalid max temperature");
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).SetPayload(handle, ref payload);
    this.SimRegister(handle, ref header, ref payload);
  }

  private static void OnActiveChanged(HandleVector<int>.Handle handle)
  {
    StructureTemperatureHeader temperatureHeader;
    StructureTemperaturePayload temperaturePayload;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetData(handle, ref temperatureHeader, ref temperaturePayload);
    temperaturePayload.primaryElement.InternalTemperature = temperaturePayload.Temperature;
    temperatureHeader.dirty = true;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).SetHeader(handle, temperatureHeader);
  }

  protected virtual void OnCleanUp(HandleVector<int>.Handle handle)
  {
    this.SimUnregister(handle);
    ((KSplitComponentManager<StructureTemperatureHeader, StructureTemperaturePayload>) this).OnCleanUp(handle);
  }

  public virtual void Sim200ms(float dt)
  {
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    List<StructureTemperatureHeader> temperatureHeaderList;
    List<StructureTemperaturePayload> temperaturePayloadList;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).GetDataLists(ref temperatureHeaderList, ref temperaturePayloadList);
    ListPool<int, StructureTemperatureComponents>.PooledList pooledList1 = ListPool<int, StructureTemperatureComponents>.Allocate();
    ((List<int>) pooledList1).Capacity = Math.Max(((List<int>) pooledList1).Capacity, temperatureHeaderList.Count);
    ListPool<int, StructureTemperatureComponents>.PooledList pooledList2 = ListPool<int, StructureTemperatureComponents>.Allocate();
    ((List<int>) pooledList2).Capacity = Math.Max(((List<int>) pooledList2).Capacity, temperatureHeaderList.Count);
    ListPool<int, StructureTemperatureComponents>.PooledList pooledList3 = ListPool<int, StructureTemperatureComponents>.Allocate();
    ((List<int>) pooledList3).Capacity = Math.Max(((List<int>) pooledList3).Capacity, temperatureHeaderList.Count);
    for (int index = 0; index != temperatureHeaderList.Count; ++index)
    {
      StructureTemperatureHeader temperatureHeader = temperatureHeaderList[index];
      if (Sim.IsValidHandle(temperatureHeader.simHandle))
      {
        ((List<int>) pooledList1).Add(index);
        if (temperatureHeader.dirty)
        {
          ((List<int>) pooledList2).Add(index);
          temperatureHeader.dirty = false;
          temperatureHeaderList[index] = temperatureHeader;
        }
        if (temperatureHeader.isActiveBuilding)
          ((List<int>) pooledList3).Add(index);
      }
    }
    foreach (int index in (List<int>) pooledList2)
    {
      StructureTemperaturePayload payload = temperaturePayloadList[index];
      StructureTemperatureComponents.UpdateSimState(ref payload);
    }
    foreach (int index in (List<int>) pooledList2)
    {
      if ((double) temperaturePayloadList[index].pendingEnergyModifications != 0.0)
      {
        StructureTemperaturePayload temperaturePayload = temperaturePayloadList[index];
        SimMessages.ModifyBuildingEnergy(temperaturePayload.simHandleCopy, temperaturePayload.pendingEnergyModifications, 0.0f, 10000f);
        temperaturePayload.pendingEnergyModifications = 0.0f;
        temperaturePayloadList[index] = temperaturePayload;
      }
    }
    foreach (int index1 in (List<int>) pooledList3)
    {
      StructureTemperaturePayload temperaturePayload = temperaturePayloadList[index1];
      if (Object.op_Equality((Object) temperaturePayload.operational, (Object) null) || temperaturePayload.operational.IsActive)
      {
        ++num1;
        if (!temperaturePayload.isActiveStatusItemSet)
        {
          ++num3;
          ((Component) temperaturePayload.primaryElement).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, this.operatingEnergyStatusItem, (object) temperaturePayload.simHandleCopy);
          temperaturePayload.isActiveStatusItemSet = true;
        }
        temperaturePayload.energySourcesKW = this.AccumulateProducedEnergyKW(temperaturePayload.energySourcesKW, temperaturePayload.OperatingKilowatts, (string) BUILDING.STATUSITEMS.OPERATINGENERGY.OPERATING);
        if ((double) temperaturePayload.ExhaustKilowatts != 0.0)
        {
          ++num2;
          Extents extents = temperaturePayload.GetExtents();
          int num4 = extents.width * extents.height;
          float num5 = temperaturePayload.ExhaustKilowatts * dt / (float) num4;
          for (int index2 = 0; index2 < extents.height; ++index2)
          {
            int num6 = extents.y + index2;
            for (int index3 = 0; index3 < extents.width; ++index3)
            {
              int num7 = extents.x + index3;
              int num8 = num6 * Grid.WidthInCells + num7;
              float num9 = Mathf.Min(Grid.Mass[num8], 1.5f) / 1.5f;
              float kilojoules = num5 * num9;
              SimMessages.ModifyEnergy(num8, kilojoules, temperaturePayload.maxTemperature, SimMessages.EnergySourceID.StructureTemperature);
            }
          }
          temperaturePayload.energySourcesKW = this.AccumulateProducedEnergyKW(temperaturePayload.energySourcesKW, temperaturePayload.ExhaustKilowatts, (string) BUILDING.STATUSITEMS.OPERATINGENERGY.EXHAUSTING);
        }
      }
      else if (temperaturePayload.isActiveStatusItemSet)
      {
        ++num3;
        ((Component) temperaturePayload.primaryElement).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.OperatingEnergy, (StatusItem) null);
        temperaturePayload.isActiveStatusItemSet = false;
      }
      temperaturePayloadList[index1] = temperaturePayload;
    }
    pooledList3.Recycle();
    pooledList2.Recycle();
    pooledList1.Recycle();
  }

  private static void UpdateSimState(ref StructureTemperaturePayload payload)
  {
    DebugUtil.Assert(Sim.IsValidHandle(payload.simHandleCopy));
    float internalTemperature = payload.primaryElement.InternalTemperature;
    BuildingDef def = payload.building.Def;
    float mass = def.MassForTemperatureModification;
    float operatingKilowatts = payload.OperatingKilowatts;
    float overheat_temperature = Object.op_Inequality((Object) payload.overheatable, (Object) null) ? payload.overheatable.OverheatTemperature : 10000f;
    if (!payload.enabled || payload.bypass)
      mass = 0.0f;
    Extents extents = payload.GetExtents();
    ushort idx = payload.primaryElement.Element.idx;
    SimMessages.ModifyBuildingHeatExchange(payload.simHandleCopy, extents, mass, internalTemperature, def.ThermalConductivity, overheat_temperature, operatingKilowatts, idx);
  }

  private static unsafe float OnGetTemperature(PrimaryElement primary_element)
  {
    HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(((Component) primary_element).gameObject);
    StructureTemperaturePayload payload = ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetPayload(handle);
    float temperature;
    if (Sim.IsValidHandle(payload.simHandleCopy) && payload.enabled)
    {
      if (!payload.bypass)
      {
        temperature = Game.Instance.simData.buildingTemperatures[Sim.GetHandleIndex(payload.simHandleCopy)].temperature;
      }
      else
      {
        int cell = Grid.PosToCell(TransformExtensions.GetPosition(payload.primaryElement.transform));
        temperature = Grid.Temperature[cell];
      }
    }
    else
      temperature = payload.primaryElement.InternalTemperature;
    return temperature;
  }

  private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
  {
    HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(((Component) primary_element).gameObject);
    StructureTemperatureHeader temperatureHeader;
    StructureTemperaturePayload payload;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetData(handle, ref temperatureHeader, ref payload);
    payload.primaryElement.InternalTemperature = temperature;
    temperatureHeader.dirty = true;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).SetHeader(handle, temperatureHeader);
    if (temperatureHeader.isActiveBuilding || !Sim.IsValidHandle(payload.simHandleCopy))
      return;
    StructureTemperatureComponents.UpdateSimState(ref payload);
    if ((double) payload.pendingEnergyModifications == 0.0)
      return;
    SimMessages.ModifyBuildingEnergy(payload.simHandleCopy, payload.pendingEnergyModifications, 0.0f, 10000f);
    payload.pendingEnergyModifications = 0.0f;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).SetPayload(handle, ref payload);
  }

  public void ProduceEnergy(
    HandleVector<int>.Handle handle,
    float delta_kilojoules,
    string source,
    float display_dt)
  {
    StructureTemperaturePayload payload = ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).GetPayload(handle);
    if (Sim.IsValidHandle(payload.simHandleCopy))
    {
      SimMessages.ModifyBuildingEnergy(payload.simHandleCopy, delta_kilojoules, 0.0f, 10000f);
    }
    else
    {
      payload.pendingEnergyModifications += delta_kilojoules;
      StructureTemperatureHeader header = ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).GetHeader(handle) with
      {
        dirty = true
      };
      ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).SetHeader(handle, header);
    }
    payload.energySourcesKW = this.AccumulateProducedEnergyKW(payload.energySourcesKW, delta_kilojoules / display_dt, source);
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).SetPayload(handle, ref payload);
  }

  private List<StructureTemperaturePayload.EnergySource> AccumulateProducedEnergyKW(
    List<StructureTemperaturePayload.EnergySource> sources,
    float kw,
    string source)
  {
    if (sources == null)
      sources = new List<StructureTemperaturePayload.EnergySource>();
    bool flag = false;
    for (int index = 0; index < sources.Count; ++index)
    {
      if (sources[index].source == source)
      {
        sources[index].Accumulate(kw);
        flag = true;
        break;
      }
    }
    if (!flag)
      sources.Add(new StructureTemperaturePayload.EnergySource(kw, source));
    return sources;
  }

  public static void DoStateTransition(int sim_handle)
  {
    HandleVector<int>.Handle invalidHandle = HandleVector<int>.InvalidHandle;
    if (!StructureTemperatureComponents.handleInstanceMap.TryGetValue(sim_handle, out invalidHandle))
      return;
    StructureTemperatureComponents.DoMelt(((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetPayload(invalidHandle).primaryElement);
  }

  public static void DoMelt(PrimaryElement primary_element)
  {
    Element element = primary_element.Element;
    if (element.highTempTransitionTarget == SimHashes.Unobtanium)
      return;
    SimMessages.AddRemoveSubstance(Grid.PosToCell(TransformExtensions.GetPosition(primary_element.transform)), element.highTempTransitionTarget, CellEventLogger.Instance.OreMelted, primary_element.Mass, primary_element.Element.highTemp, primary_element.DiseaseIdx, primary_element.DiseaseCount);
    Util.KDestroyGameObject(((Component) primary_element).gameObject);
  }

  public static void DoOverheat(int sim_handle)
  {
    HandleVector<int>.Handle invalidHandle = HandleVector<int>.InvalidHandle;
    if (!StructureTemperatureComponents.handleInstanceMap.TryGetValue(sim_handle, out invalidHandle))
      return;
    EventExtensions.Trigger(((Component) ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetPayload(invalidHandle).primaryElement).gameObject, 1832602615, (object) null);
  }

  public static void DoNoLongerOverheated(int sim_handle)
  {
    HandleVector<int>.Handle invalidHandle = HandleVector<int>.InvalidHandle;
    if (!StructureTemperatureComponents.handleInstanceMap.TryGetValue(sim_handle, out invalidHandle))
      return;
    EventExtensions.Trigger(((Component) ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetPayload(invalidHandle).primaryElement).gameObject, 171119937, (object) null);
  }

  public bool IsEnabled(HandleVector<int>.Handle handle) => ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).GetPayload(handle).enabled;

  private void Enable(HandleVector<int>.Handle handle, bool isEnabled)
  {
    StructureTemperatureHeader temperatureHeader;
    StructureTemperaturePayload temperaturePayload;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).GetData(handle, ref temperatureHeader, ref temperaturePayload);
    temperatureHeader.dirty = true;
    temperaturePayload.enabled = isEnabled;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).SetData(handle, temperatureHeader, ref temperaturePayload);
  }

  public void Enable(HandleVector<int>.Handle handle) => this.Enable(handle, true);

  public void Disable(HandleVector<int>.Handle handle) => this.Enable(handle, false);

  public bool IsBypassed(HandleVector<int>.Handle handle) => ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).GetPayload(handle).bypass;

  private void Bypass(HandleVector<int>.Handle handle, bool bypass)
  {
    StructureTemperatureHeader temperatureHeader;
    StructureTemperaturePayload temperaturePayload;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).GetData(handle, ref temperatureHeader, ref temperaturePayload);
    temperatureHeader.dirty = true;
    temperaturePayload.bypass = bypass;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).SetData(handle, temperatureHeader, ref temperaturePayload);
  }

  public void Bypass(HandleVector<int>.Handle handle) => this.Bypass(handle, true);

  public void UnBypass(HandleVector<int>.Handle handle) => this.Bypass(handle, false);

  protected void SimRegister(
    HandleVector<int>.Handle handle,
    ref StructureTemperatureHeader header,
    ref StructureTemperaturePayload payload)
  {
    if (payload.simHandleCopy != -1)
      return;
    PrimaryElement primaryElement = payload.primaryElement;
    if ((double) primaryElement.Mass <= 0.0 || primaryElement.Element.IsTemperatureInsulated)
      return;
    payload.simHandleCopy = -2;
    string dbg_name = ((Object) primaryElement).name;
    HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle1 = Game.Instance.simComponentCallbackManager.Add((Action<int, object>) ((sim_handle, callback_data) => StructureTemperatureComponents.OnSimRegistered(handle, sim_handle, dbg_name)), (object) null, "StructureTemperature.SimRegister");
    BuildingDef def = ((Component) primaryElement).GetComponent<Building>().Def;
    float internalTemperature = primaryElement.InternalTemperature;
    float temperatureModification = def.MassForTemperatureModification;
    float operatingKilowatts = payload.OperatingKilowatts;
    Extents extents = payload.GetExtents();
    ushort idx = primaryElement.Element.idx;
    double mass = (double) temperatureModification;
    double temperature = (double) internalTemperature;
    double thermalConductivity = (double) def.ThermalConductivity;
    double operating_kw = (double) operatingKilowatts;
    int elem_idx = (int) idx;
    int index = handle1.index;
    SimMessages.AddBuildingHeatExchange(extents, (float) mass, (float) temperature, (float) thermalConductivity, (float) operating_kw, (ushort) elem_idx, index);
    header.simHandle = payload.simHandleCopy;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).SetData(handle, header, ref payload);
  }

  private static void OnSimRegistered(
    HandleVector<int>.Handle handle,
    int sim_handle,
    string dbg_name)
  {
    if (!((KCompactedVectorBase) GameComps.StructureTemperatures).IsValid(handle) || !((KCompactedVectorBase) GameComps.StructureTemperatures).IsVersionValid(handle))
      return;
    StructureTemperatureHeader temperatureHeader;
    StructureTemperaturePayload temperaturePayload;
    ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetData(handle, ref temperatureHeader, ref temperaturePayload);
    if (temperaturePayload.simHandleCopy == -2)
    {
      StructureTemperatureComponents.handleInstanceMap[sim_handle] = handle;
      temperatureHeader.simHandle = sim_handle;
      temperaturePayload.simHandleCopy = sim_handle;
      ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).SetData(handle, temperatureHeader, ref temperaturePayload);
      temperaturePayload.primaryElement.Trigger(-1555603773, (object) sim_handle);
      GameScenePartitioner.Instance.TriggerEvent(Grid.PosToCell(TransformExtensions.GetPosition(temperaturePayload.building.transform)), GameScenePartitioner.Instance.contactConductiveLayer, (object) new StructureToStructureTemperature.BuildingChangedObj(StructureToStructureTemperature.BuildingChangeType.Created, temperaturePayload.building, sim_handle));
    }
    else
      SimMessages.RemoveBuildingHeatExchange(sim_handle);
  }

  protected unsafe void SimUnregister(HandleVector<int>.Handle handle)
  {
    if (!((KCompactedVectorBase) GameComps.StructureTemperatures).IsVersionValid(handle))
    {
      KCrashReporter.Assert(false, "Handle version mismatch in StructureTemperature.SimUnregister");
    }
    else
    {
      if (KMonoBehaviour.isLoadingScene)
        return;
      StructureTemperatureHeader temperatureHeader;
      StructureTemperaturePayload temperaturePayload;
      ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetData(handle, ref temperatureHeader, ref temperaturePayload);
      if (temperaturePayload.simHandleCopy == -1)
        return;
      GameScenePartitioner.Instance.TriggerEvent(Grid.PosToCell((KMonoBehaviour) temperaturePayload.building), GameScenePartitioner.Instance.contactConductiveLayer, (object) new StructureToStructureTemperature.BuildingChangedObj(StructureToStructureTemperature.BuildingChangeType.Destroyed, temperaturePayload.building, temperaturePayload.simHandleCopy));
      if (Sim.IsValidHandle(temperaturePayload.simHandleCopy))
      {
        int handleIndex = Sim.GetHandleIndex(temperaturePayload.simHandleCopy);
        temperaturePayload.primaryElement.InternalTemperature = Game.Instance.simData.buildingTemperatures[handleIndex].temperature;
        SimMessages.RemoveBuildingHeatExchange(temperaturePayload.simHandleCopy);
        StructureTemperatureComponents.handleInstanceMap.Remove(temperaturePayload.simHandleCopy);
      }
      temperaturePayload.simHandleCopy = -1;
      temperatureHeader.simHandle = -1;
      ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) this).SetData(handle, temperatureHeader, ref temperaturePayload);
    }
  }
}
