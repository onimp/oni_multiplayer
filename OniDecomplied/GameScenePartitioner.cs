// Decompiled with JetBrains decompiler
// Type: GameScenePartitioner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/GameScenePartitioner")]
public class GameScenePartitioner : KMonoBehaviour
{
  public ScenePartitionerLayer solidChangedLayer;
  public ScenePartitionerLayer liquidChangedLayer;
  public ScenePartitionerLayer digDestroyedLayer;
  public ScenePartitionerLayer fogOfWarChangedLayer;
  public ScenePartitionerLayer decorProviderLayer;
  public ScenePartitionerLayer attackableEntitiesLayer;
  public ScenePartitionerLayer fetchChoreLayer;
  public ScenePartitionerLayer pickupablesLayer;
  public ScenePartitionerLayer pickupablesChangedLayer;
  public ScenePartitionerLayer gasConduitsLayer;
  public ScenePartitionerLayer liquidConduitsLayer;
  public ScenePartitionerLayer solidConduitsLayer;
  public ScenePartitionerLayer wiresLayer;
  public ScenePartitionerLayer[] objectLayers;
  public ScenePartitionerLayer noisePolluterLayer;
  public ScenePartitionerLayer validNavCellChangedLayer;
  public ScenePartitionerLayer dirtyNavCellUpdateLayer;
  public ScenePartitionerLayer trapsLayer;
  public ScenePartitionerLayer floorSwitchActivatorLayer;
  public ScenePartitionerLayer floorSwitchActivatorChangedLayer;
  public ScenePartitionerLayer collisionLayer;
  public ScenePartitionerLayer lure;
  public ScenePartitionerLayer plants;
  public ScenePartitionerLayer industrialBuildings;
  public ScenePartitionerLayer completeBuildings;
  public ScenePartitionerLayer prioritizableObjects;
  public ScenePartitionerLayer contactConductiveLayer;
  private ScenePartitioner partitioner;
  private static GameScenePartitioner instance;
  private KCompactedVector<ScenePartitionerEntry> scenePartitionerEntries = new KCompactedVector<ScenePartitionerEntry>(0);
  private List<int> changedCells = new List<int>();

  public static GameScenePartitioner Instance
  {
    get
    {
      Debug.Assert(Object.op_Inequality((Object) GameScenePartitioner.instance, (Object) null));
      return GameScenePartitioner.instance;
    }
  }

  protected virtual void OnPrefabInit()
  {
    Debug.Assert(Object.op_Equality((Object) GameScenePartitioner.instance, (Object) null));
    GameScenePartitioner.instance = this;
    this.partitioner = new ScenePartitioner(16, 64, Grid.WidthInCells, Grid.HeightInCells);
    this.solidChangedLayer = this.partitioner.CreateMask("SolidChanged");
    this.liquidChangedLayer = this.partitioner.CreateMask("LiquidChanged");
    this.digDestroyedLayer = this.partitioner.CreateMask("DigDestroyed");
    this.fogOfWarChangedLayer = this.partitioner.CreateMask("FogOfWarChanged");
    this.decorProviderLayer = this.partitioner.CreateMask("DecorProviders");
    this.attackableEntitiesLayer = this.partitioner.CreateMask("FactionedEntities");
    this.fetchChoreLayer = this.partitioner.CreateMask("FetchChores");
    this.pickupablesLayer = this.partitioner.CreateMask("Pickupables");
    this.pickupablesChangedLayer = this.partitioner.CreateMask("PickupablesChanged");
    this.gasConduitsLayer = this.partitioner.CreateMask("GasConduit");
    this.liquidConduitsLayer = this.partitioner.CreateMask("LiquidConduit");
    this.solidConduitsLayer = this.partitioner.CreateMask("SolidConduit");
    this.noisePolluterLayer = this.partitioner.CreateMask("NoisePolluters");
    this.validNavCellChangedLayer = this.partitioner.CreateMask("validNavCellChangedLayer");
    this.dirtyNavCellUpdateLayer = this.partitioner.CreateMask("dirtyNavCellUpdateLayer");
    this.trapsLayer = this.partitioner.CreateMask("trapsLayer");
    this.floorSwitchActivatorLayer = this.partitioner.CreateMask("FloorSwitchActivatorLayer");
    this.floorSwitchActivatorChangedLayer = this.partitioner.CreateMask("FloorSwitchActivatorChangedLayer");
    this.collisionLayer = this.partitioner.CreateMask("Collision");
    this.lure = this.partitioner.CreateMask("Lure");
    this.plants = this.partitioner.CreateMask("Plants");
    this.industrialBuildings = this.partitioner.CreateMask("IndustrialBuildings");
    this.completeBuildings = this.partitioner.CreateMask("CompleteBuildings");
    this.prioritizableObjects = this.partitioner.CreateMask("PrioritizableObjects");
    this.contactConductiveLayer = this.partitioner.CreateMask("ContactConductiveLayer");
    this.objectLayers = new ScenePartitionerLayer[44];
    for (int index = 0; index < 44; ++index)
    {
      ObjectLayer objectLayer = (ObjectLayer) index;
      this.objectLayers[index] = this.partitioner.CreateMask(objectLayer.ToString());
    }
  }

  protected virtual void OnForcedCleanUp()
  {
    GameScenePartitioner.instance = (GameScenePartitioner) null;
    this.partitioner.FreeResources();
    this.partitioner = (ScenePartitioner) null;
    this.solidChangedLayer = (ScenePartitionerLayer) null;
    this.liquidChangedLayer = (ScenePartitionerLayer) null;
    this.digDestroyedLayer = (ScenePartitionerLayer) null;
    this.fogOfWarChangedLayer = (ScenePartitionerLayer) null;
    this.decorProviderLayer = (ScenePartitionerLayer) null;
    this.attackableEntitiesLayer = (ScenePartitionerLayer) null;
    this.fetchChoreLayer = (ScenePartitionerLayer) null;
    this.pickupablesLayer = (ScenePartitionerLayer) null;
    this.pickupablesChangedLayer = (ScenePartitionerLayer) null;
    this.gasConduitsLayer = (ScenePartitionerLayer) null;
    this.liquidConduitsLayer = (ScenePartitionerLayer) null;
    this.solidConduitsLayer = (ScenePartitionerLayer) null;
    this.noisePolluterLayer = (ScenePartitionerLayer) null;
    this.validNavCellChangedLayer = (ScenePartitionerLayer) null;
    this.dirtyNavCellUpdateLayer = (ScenePartitionerLayer) null;
    this.trapsLayer = (ScenePartitionerLayer) null;
    this.floorSwitchActivatorLayer = (ScenePartitionerLayer) null;
    this.floorSwitchActivatorChangedLayer = (ScenePartitionerLayer) null;
    this.contactConductiveLayer = (ScenePartitionerLayer) null;
    this.objectLayers = (ScenePartitionerLayer[]) null;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    NavGrid navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
    navGrid.OnNavGridUpdateComplete += new Action<HashSet<int>>(this.OnNavGridUpdateComplete);
    navGrid.NavTable.OnValidCellChanged += new Action<int, NavType>(this.OnValidNavCellChanged);
  }

  public HandleVector<int>.Handle Add(
    string name,
    object obj,
    int x,
    int y,
    int width,
    int height,
    ScenePartitionerLayer layer,
    Action<object> event_callback)
  {
    ScenePartitionerEntry entry = new ScenePartitionerEntry(name, obj, x, y, width, height, layer, this.partitioner, event_callback);
    this.partitioner.Add(entry);
    return this.scenePartitionerEntries.Allocate(entry);
  }

  public HandleVector<int>.Handle Add(
    string name,
    object obj,
    Extents extents,
    ScenePartitionerLayer layer,
    Action<object> event_callback)
  {
    return this.Add(name, obj, extents.x, extents.y, extents.width, extents.height, layer, event_callback);
  }

  public HandleVector<int>.Handle Add(
    string name,
    object obj,
    int cell,
    ScenePartitionerLayer layer,
    Action<object> event_callback)
  {
    int x = 0;
    int y = 0;
    Grid.CellToXY(cell, out x, out y);
    return this.Add(name, obj, x, y, 1, 1, layer, event_callback);
  }

  public void AddGlobalLayerListener(ScenePartitionerLayer layer, Action<int, object> action) => layer.OnEvent += action;

  public void RemoveGlobalLayerListener(ScenePartitionerLayer layer, Action<int, object> action) => layer.OnEvent -= action;

  public void TriggerEvent(List<int> cells, ScenePartitionerLayer layer, object event_data) => this.partitioner.TriggerEvent(cells, layer, event_data);

  public void TriggerEvent(HashSet<int> cells, ScenePartitionerLayer layer, object event_data) => this.partitioner.TriggerEvent(cells, layer, event_data);

  public void TriggerEvent(Extents extents, ScenePartitionerLayer layer, object event_data) => this.partitioner.TriggerEvent(extents.x, extents.y, extents.width, extents.height, layer, event_data);

  public void TriggerEvent(
    int x,
    int y,
    int width,
    int height,
    ScenePartitionerLayer layer,
    object event_data)
  {
    this.partitioner.TriggerEvent(x, y, width, height, layer, event_data);
  }

  public void TriggerEvent(int cell, ScenePartitionerLayer layer, object event_data)
  {
    int x = 0;
    int y = 0;
    Grid.CellToXY(cell, out x, out y);
    this.TriggerEvent(x, y, 1, 1, layer, event_data);
  }

  public void GatherEntries(
    Extents extents,
    ScenePartitionerLayer layer,
    List<ScenePartitionerEntry> gathered_entries)
  {
    this.GatherEntries(extents.x, extents.y, extents.width, extents.height, layer, gathered_entries);
  }

  public void GatherEntries(
    int x_bottomLeft,
    int y_bottomLeft,
    int width,
    int height,
    ScenePartitionerLayer layer,
    List<ScenePartitionerEntry> gathered_entries)
  {
    this.partitioner.GatherEntries(x_bottomLeft, y_bottomLeft, width, height, layer, (object) null, gathered_entries);
  }

  public void Iterate<IteratorType>(
    int x,
    int y,
    int width,
    int height,
    ScenePartitionerLayer layer,
    ref IteratorType iterator)
    where IteratorType : GameScenePartitioner.Iterator
  {
    ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
    GameScenePartitioner.Instance.GatherEntries(x, y, width, height, layer, (List<ScenePartitionerEntry>) gathered_entries);
    for (int index = 0; index < ((List<ScenePartitionerEntry>) gathered_entries).Count; ++index)
    {
      ScenePartitionerEntry partitionerEntry = ((List<ScenePartitionerEntry>) gathered_entries)[index];
      iterator.Iterate(partitionerEntry.obj);
    }
    gathered_entries.Recycle();
  }

  public void Iterate<IteratorType>(
    int cell,
    int radius,
    ScenePartitionerLayer layer,
    ref IteratorType iterator)
    where IteratorType : GameScenePartitioner.Iterator
  {
    int x = 0;
    int y = 0;
    Grid.CellToXY(cell, out x, out y);
    this.Iterate<IteratorType>(x - radius, y - radius, radius * 2, radius * 2, layer, ref iterator);
  }

  private void OnValidNavCellChanged(int cell, NavType nav_type) => this.changedCells.Add(cell);

  private void OnNavGridUpdateComplete(HashSet<int> dirty_nav_cells)
  {
    if (dirty_nav_cells.Count > 0)
      GameScenePartitioner.Instance.TriggerEvent(dirty_nav_cells, GameScenePartitioner.Instance.dirtyNavCellUpdateLayer, (object) null);
    if (this.changedCells.Count <= 0)
      return;
    GameScenePartitioner.Instance.TriggerEvent(this.changedCells, GameScenePartitioner.Instance.validNavCellChangedLayer, (object) null);
    this.changedCells.Clear();
  }

  public void UpdatePosition(HandleVector<int>.Handle handle, int cell)
  {
    Vector2I xy = Grid.CellToXY(cell);
    this.UpdatePosition(handle, xy.x, xy.y);
  }

  public void UpdatePosition(HandleVector<int>.Handle handle, int x, int y)
  {
    if (!handle.IsValid())
      return;
    this.scenePartitionerEntries.GetData(handle).UpdatePosition(x, y);
  }

  public void UpdatePosition(HandleVector<int>.Handle handle, Extents ext)
  {
    if (!handle.IsValid())
      return;
    this.scenePartitionerEntries.GetData(handle).UpdatePosition(ext);
  }

  public void Free(ref HandleVector<int>.Handle handle)
  {
    if (!handle.IsValid())
      return;
    this.scenePartitionerEntries.GetData(handle).Release();
    this.scenePartitionerEntries.Free(handle);
    handle.Clear();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    this.partitioner.Cleanup();
  }

  public bool DoDebugLayersContainItemsOnCell(int cell) => this.partitioner.DoDebugLayersContainItemsOnCell(cell);

  public List<ScenePartitionerLayer> GetLayers() => this.partitioner.layers;

  public void SetToggledLayers(HashSet<ScenePartitionerLayer> toggled_layers) => this.partitioner.toggledLayers = toggled_layers;

  public interface Iterator
  {
    void Iterate(object obj);

    void Cleanup();
  }
}
