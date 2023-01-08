// Decompiled with JetBrains decompiler
// Type: StructureToStructureTemperature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class StructureToStructureTemperature : KMonoBehaviour
{
  [MyCmpGet]
  private Building building;
  private List<int> conductiveCells;
  private HashSet<int> inContactBuildings = new HashSet<int>();
  private bool hasBeenRegister;
  private bool buildingDestroyed;
  private int selfHandle;
  protected static readonly EventSystem.IntraObjectHandler<StructureToStructureTemperature> OnStructureTemperatureRegisteredDelegate = new EventSystem.IntraObjectHandler<StructureToStructureTemperature>((Action<StructureToStructureTemperature, object>) ((component, data) => component.OnStructureTemperatureRegistered(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<StructureToStructureTemperature>(-1555603773, StructureToStructureTemperature.OnStructureTemperatureRegisteredDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.DefineConductiveCells();
    GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.contactConductiveLayer, new Action<int, object>(this.OnAnyBuildingChanged));
  }

  protected virtual void OnCleanUp()
  {
    GameScenePartitioner.Instance.RemoveGlobalLayerListener(GameScenePartitioner.Instance.contactConductiveLayer, new Action<int, object>(this.OnAnyBuildingChanged));
    this.UnregisterToSIM();
    base.OnCleanUp();
  }

  private void OnStructureTemperatureRegistered(object _sim_handle) => this.RegisterToSIM((int) _sim_handle);

  private void RegisterToSIM(int sim_handle1)
  {
    string name = this.building.Def.Name;
    HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle = Game.Instance.simComponentCallbackManager.Add((Action<int, object>) ((sim_handle2, callback_data) => this.OnSimRegistered(sim_handle2)), (object) null, "StructureToStructureTemperature.SimRegister");
    SimMessages.RegisterBuildingToBuildingHeatExchange(sim_handle1, handle.index);
  }

  private void OnSimRegistered(int sim_handle)
  {
    if (sim_handle == -1)
      return;
    this.selfHandle = sim_handle;
    this.hasBeenRegister = true;
    if (this.buildingDestroyed)
      this.UnregisterToSIM();
    else
      this.Refresh_InContactBuildings();
  }

  private void UnregisterToSIM()
  {
    if (this.hasBeenRegister)
      SimMessages.RemoveBuildingToBuildingHeatExchange(this.selfHandle);
    this.buildingDestroyed = true;
  }

  private void DefineConductiveCells()
  {
    this.conductiveCells = new List<int>((IEnumerable<int>) this.building.PlacementCells);
    this.conductiveCells.Remove(this.building.GetUtilityInputCell());
    this.conductiveCells.Remove(this.building.GetUtilityOutputCell());
  }

  private void Add(
    StructureToStructureTemperature.InContactBuildingData buildingData)
  {
    if (!this.inContactBuildings.Add(buildingData.buildingInContact))
      return;
    SimMessages.AddBuildingToBuildingHeatExchange(this.selfHandle, buildingData.buildingInContact, buildingData.cellsInContact);
  }

  private void Remove(int building)
  {
    if (!this.inContactBuildings.Contains(building))
      return;
    this.inContactBuildings.Remove(building);
    SimMessages.RemoveBuildingInContactFromBuildingToBuildingHeatExchange(this.selfHandle, building);
  }

  private void OnAnyBuildingChanged(int _cell, object _data)
  {
    if (!this.hasBeenRegister)
      return;
    StructureToStructureTemperature.BuildingChangedObj buildingChangedObj = (StructureToStructureTemperature.BuildingChangedObj) _data;
    bool flag = false;
    int num = 0;
    for (int index = 0; index < buildingChangedObj.building.PlacementCells.Length; ++index)
    {
      if (this.conductiveCells.Contains(buildingChangedObj.building.PlacementCells[index]))
      {
        flag = true;
        ++num;
      }
    }
    if (!flag)
      return;
    int simHandler = buildingChangedObj.simHandler;
    switch (buildingChangedObj.changeType)
    {
      case StructureToStructureTemperature.BuildingChangeType.Created:
        this.Add(new StructureToStructureTemperature.InContactBuildingData()
        {
          buildingInContact = simHandler,
          cellsInContact = num
        });
        break;
      case StructureToStructureTemperature.BuildingChangeType.Destroyed:
        this.Remove(simHandler);
        break;
    }
  }

  private void Refresh_InContactBuildings()
  {
    foreach (StructureToStructureTemperature.InContactBuildingData inContactBuilding in this.GetAll_InContact_Buildings())
      this.Add(inContactBuilding);
  }

  private List<StructureToStructureTemperature.InContactBuildingData> GetAll_InContact_Buildings()
  {
    Dictionary<Building, int> dictionary = new Dictionary<Building, int>();
    List<StructureToStructureTemperature.InContactBuildingData> contactBuildings = new List<StructureToStructureTemperature.InContactBuildingData>();
    List<GameObject> buildingsInCell = new List<GameObject>();
    foreach (int conductiveCell in this.conductiveCells)
    {
      int cell = conductiveCell;
      buildingsInCell.Clear();
      Action<int> action = (Action<int>) (layer =>
      {
        GameObject gameObject = Grid.Objects[cell, layer];
        if (!Object.op_Inequality((Object) gameObject, (Object) null) || buildingsInCell.Contains(gameObject))
          return;
        buildingsInCell.Add(gameObject);
      });
      action(1);
      action(26);
      action(27);
      action(31);
      action(32);
      action(30);
      action(12);
      action(13);
      action(16);
      action(17);
      action(24);
      action(2);
      for (int index = 0; index < buildingsInCell.Count; ++index)
      {
        Building key = Object.op_Equality((Object) buildingsInCell[index], (Object) null) ? (Building) null : buildingsInCell[index].GetComponent<Building>();
        if ((!Object.op_Inequality((Object) key, (Object) null) ? 0 : (key.Def.UseStructureTemperature ? 1 : 0)) != 0)
        {
          if (!dictionary.ContainsKey(key))
            dictionary.Add(key, 0);
          dictionary[key]++;
        }
      }
    }
    foreach (Building key in dictionary.Keys)
    {
      HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle((MonoBehaviour) key);
      if (HandleVector<int>.Handle.op_Inequality(handle, HandleVector<int>.InvalidHandle))
      {
        int simHandleCopy = ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetPayload(handle).simHandleCopy;
        StructureToStructureTemperature.InContactBuildingData contactBuildingData = new StructureToStructureTemperature.InContactBuildingData()
        {
          buildingInContact = simHandleCopy,
          cellsInContact = dictionary[key]
        };
        contactBuildings.Add(contactBuildingData);
      }
    }
    return contactBuildings;
  }

  public enum BuildingChangeType
  {
    Created,
    Destroyed,
    Moved,
  }

  public struct InContactBuildingData
  {
    public int buildingInContact;
    public int cellsInContact;
  }

  public struct BuildingChangedObj
  {
    public StructureToStructureTemperature.BuildingChangeType changeType;
    public int simHandler;
    public Building building;

    public BuildingChangedObj(
      StructureToStructureTemperature.BuildingChangeType _changeType,
      Building _building,
      int sim_handler)
    {
      this.changeType = _changeType;
      this.building = _building;
      this.simHandler = sim_handler;
    }
  }
}
