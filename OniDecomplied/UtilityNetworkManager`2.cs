// Decompiled with JetBrains decompiler
// Type: UtilityNetworkManager`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilityNetworkManager<NetworkType, ItemType> : IUtilityNetworkMgr
  where NetworkType : UtilityNetwork, new()
  where ItemType : MonoBehaviour
{
  private Dictionary<int, object> items = new Dictionary<int, object>();
  private Dictionary<int, object> endpoints = new Dictionary<int, object>();
  private Dictionary<object, List<object>> virtualItems = new Dictionary<object, List<object>>();
  private Dictionary<object, List<object>> virtualEndpoints = new Dictionary<object, List<object>>();
  private Dictionary<int, int> links = new Dictionary<int, int>();
  private Dictionary<int, object> semiVirtualLinks = new Dictionary<int, object>();
  private List<UtilityNetwork> networks;
  private Dictionary<object, int> virtualKeyToNetworkIdx = new Dictionary<object, int>();
  private HashSet<int> visitedCells;
  private HashSet<object> visitedVirtualKeys;
  private HashSet<object> queuedVirtualKeys;
  private Action<IList<UtilityNetwork>, ICollection<int>> onNetworksRebuilt;
  private Queue<int> queued = new Queue<int>();
  protected UtilityNetworkGridNode[] visualGrid;
  private UtilityNetworkGridNode[] stashedVisualGrid;
  protected UtilityNetworkGridNode[] physicalGrid;
  protected HashSet<int> physicalNodes;
  protected HashSet<int> visualNodes;
  private bool dirty;
  private int tileLayer = -1;

  public bool IsDirty => this.dirty;

  public UtilityNetworkManager(int game_width, int game_height, int tile_layer)
  {
    this.tileLayer = tile_layer;
    this.networks = new List<UtilityNetwork>();
    this.Initialize(game_width, game_height);
  }

  public void Initialize(int game_width, int game_height)
  {
    this.networks.Clear();
    this.physicalGrid = new UtilityNetworkGridNode[game_width * game_height];
    this.visualGrid = new UtilityNetworkGridNode[game_width * game_height];
    this.stashedVisualGrid = new UtilityNetworkGridNode[game_width * game_height];
    this.physicalNodes = new HashSet<int>();
    this.visualNodes = new HashSet<int>();
    this.visitedCells = new HashSet<int>();
    this.visitedVirtualKeys = new HashSet<object>();
    this.queuedVirtualKeys = new HashSet<object>();
    for (int index1 = 0; index1 < this.visualGrid.Length; ++index1)
    {
      UtilityNetworkGridNode[] visualGrid = this.visualGrid;
      int index2 = index1;
      UtilityNetworkGridNode utilityNetworkGridNode1 = new UtilityNetworkGridNode();
      utilityNetworkGridNode1.networkIdx = -1;
      utilityNetworkGridNode1.connections = (UtilityConnections) 0;
      UtilityNetworkGridNode utilityNetworkGridNode2 = utilityNetworkGridNode1;
      visualGrid[index2] = utilityNetworkGridNode2;
      UtilityNetworkGridNode[] physicalGrid = this.physicalGrid;
      int index3 = index1;
      utilityNetworkGridNode1 = new UtilityNetworkGridNode();
      utilityNetworkGridNode1.networkIdx = -1;
      utilityNetworkGridNode1.connections = (UtilityConnections) 0;
      UtilityNetworkGridNode utilityNetworkGridNode3 = utilityNetworkGridNode1;
      physicalGrid[index3] = utilityNetworkGridNode3;
    }
  }

  public void Update()
  {
    if (!this.dirty)
      return;
    this.dirty = false;
    for (int index = 0; index < this.networks.Count; ++index)
      this.networks[index].Reset(this.physicalGrid);
    this.networks.Clear();
    this.virtualKeyToNetworkIdx.Clear();
    this.RebuildNetworks(this.tileLayer, false);
    this.RebuildNetworks(this.tileLayer, true);
    if (this.onNetworksRebuilt == null)
      return;
    this.onNetworksRebuilt((IList<UtilityNetwork>) this.networks, (ICollection<int>) this.GetNodes(true));
  }

  protected UtilityNetworkGridNode[] GetGrid(bool is_physical_building) => !is_physical_building ? this.visualGrid : this.physicalGrid;

  private HashSet<int> GetNodes(bool is_physical_building) => !is_physical_building ? this.visualNodes : this.physicalNodes;

  public void ClearCell(int cell, bool is_physical_building)
  {
    if (Game.IsQuitting())
      return;
    UtilityNetworkGridNode[] grid = this.GetGrid(is_physical_building);
    HashSet<int> nodes = this.GetNodes(is_physical_building);
    UtilityConnections connections = grid[cell].connections;
    grid[cell].connections = (UtilityConnections) 0;
    Vector2I xy = Grid.CellToXY(cell);
    if (xy.x > 1 && (connections & UtilityConnections.Left) != (UtilityConnections) 0)
      grid[Grid.CellLeft(cell)].connections &= ~UtilityConnections.Right;
    if (xy.x < Grid.WidthInCells - 1 && (connections & UtilityConnections.Right) != (UtilityConnections) 0)
      grid[Grid.CellRight(cell)].connections &= ~UtilityConnections.Left;
    if (xy.y > 1 && (connections & UtilityConnections.Down) != (UtilityConnections) 0)
      grid[Grid.CellBelow(cell)].connections &= ~UtilityConnections.Up;
    if (xy.y < Grid.HeightInCells - 1 && (connections & UtilityConnections.Up) != (UtilityConnections) 0)
      grid[Grid.CellAbove(cell)].connections &= ~UtilityConnections.Down;
    nodes.Remove(cell);
    if (!is_physical_building)
      return;
    this.dirty = true;
    this.ClearCell(cell, false);
  }

  private void QueueCellForVisit(
    UtilityNetworkGridNode[] grid,
    int dest_cell,
    UtilityConnections direction)
  {
    if (!Grid.IsValidCell(dest_cell) || this.visitedCells.Contains(dest_cell) || direction != (UtilityConnections) 0 && (grid[dest_cell].connections & direction.InverseDirection()) == (UtilityConnections) 0 || !Object.op_Inequality((Object) Grid.Objects[dest_cell, this.tileLayer], (Object) null))
      return;
    this.visitedCells.Add(dest_cell);
    this.queued.Enqueue(dest_cell);
  }

  public void ForceRebuildNetworks() => this.dirty = true;

  public void AddToNetworks(int cell, object item, bool is_endpoint)
  {
    if (item != null)
    {
      if (is_endpoint)
      {
        if (this.endpoints.ContainsKey(cell))
        {
          Debug.LogWarning((object) string.Format("Cell {0} already has a utility network endpoint assigned. Adding {1} will stomp previous endpoint, destroying the object that's already there.", (object) cell, (object) item.ToString()));
          KMonoBehaviour endpoint = this.endpoints[cell] as KMonoBehaviour;
          if (Object.op_Inequality((Object) endpoint, (Object) null))
            Util.KDestroyGameObject((Component) endpoint);
        }
        this.endpoints[cell] = item;
      }
      else
      {
        if (this.items.ContainsKey(cell))
        {
          Debug.LogWarning((object) string.Format("Cell {0} already has a utility network connector assigned. Adding {1} will stomp previous item, destroying the object that's already there.", (object) cell, (object) item.ToString()));
          KMonoBehaviour kmonoBehaviour = this.items[cell] as KMonoBehaviour;
          if (Object.op_Inequality((Object) kmonoBehaviour, (Object) null))
            Util.KDestroyGameObject((Component) kmonoBehaviour);
        }
        this.items[cell] = item;
      }
    }
    this.dirty = true;
  }

  public void AddToVirtualNetworks(object key, object item, bool is_endpoint)
  {
    if (item != null)
    {
      if (is_endpoint)
      {
        if (!this.virtualEndpoints.ContainsKey(key))
          this.virtualEndpoints[key] = new List<object>();
        this.virtualEndpoints[key].Add(item);
      }
      else
      {
        if (!this.virtualItems.ContainsKey(key))
          this.virtualItems[key] = new List<object>();
        this.virtualItems[key].Add(item);
      }
    }
    this.dirty = true;
  }

  private unsafe void Reconnect(int cell)
  {
    Vector2I xy = Grid.CellToXY(cell);
    int* numPtr1 = stackalloc int[4];
    int* numPtr2 = stackalloc int[4];
    int* numPtr3 = stackalloc int[4];
    int index1 = 0;
    if (xy.y < Grid.HeightInCells - 1)
    {
      numPtr1[index1] = Grid.CellAbove(cell);
      numPtr2[index1] = 4;
      numPtr3[index1] = 8;
      ++index1;
    }
    if (xy.y > 1)
    {
      numPtr1[index1] = Grid.CellBelow(cell);
      numPtr2[index1] = 8;
      numPtr3[index1] = 4;
      ++index1;
    }
    if (xy.x > 1)
    {
      numPtr1[index1] = Grid.CellLeft(cell);
      numPtr2[index1] = 1;
      numPtr3[index1] = 2;
      ++index1;
    }
    if (xy.x < Grid.WidthInCells - 1)
    {
      numPtr1[index1] = Grid.CellRight(cell);
      numPtr2[index1] = 2;
      numPtr3[index1] = 1;
      ++index1;
    }
    UtilityConnections connections1 = this.physicalGrid[cell].connections;
    UtilityConnections connections2 = this.visualGrid[cell].connections;
    for (int index2 = 0; index2 < index1; ++index2)
    {
      int index3 = numPtr1[index2];
      UtilityConnections utilityConnections1 = (UtilityConnections) numPtr2[index2];
      UtilityConnections utilityConnections2 = (UtilityConnections) numPtr3[index2];
      if ((connections1 & utilityConnections1) != (UtilityConnections) 0)
      {
        if (this.physicalNodes.Contains(index3))
          this.physicalGrid[index3].connections |= utilityConnections2;
        if (this.visualNodes.Contains(index3))
          this.visualGrid[index3].connections |= utilityConnections2;
      }
      else if ((connections2 & utilityConnections1) != (UtilityConnections) 0 && (this.physicalNodes.Contains(index3) || this.visualNodes.Contains(index3)))
        this.visualGrid[index3].connections |= utilityConnections2;
    }
  }

  public void RemoveFromVirtualNetworks(object key, object item, bool is_endpoint)
  {
    if (Game.IsQuitting())
      return;
    this.dirty = true;
    if (item == null)
      return;
    if (is_endpoint)
    {
      this.virtualEndpoints[key].Remove(item);
      if (this.virtualEndpoints[key].Count == 0)
        this.virtualEndpoints.Remove(key);
    }
    else
    {
      this.virtualItems[key].Remove(item);
      if (this.virtualItems[key].Count == 0)
        this.virtualItems.Remove(key);
    }
    this.GetNetworkForVirtualKey(key)?.RemoveItem(item);
  }

  public void RemoveFromNetworks(int cell, object item, bool is_endpoint)
  {
    if (Game.IsQuitting())
      return;
    this.dirty = true;
    if (item == null)
      return;
    if (is_endpoint)
    {
      this.endpoints.Remove(cell);
      int networkIdx = this.physicalGrid[cell].networkIdx;
      if (networkIdx == -1)
        return;
      this.networks[networkIdx].RemoveItem(item);
    }
    else
    {
      int networkIdx = this.physicalGrid[cell].networkIdx;
      this.physicalGrid[cell].connections = (UtilityConnections) 0;
      this.physicalGrid[cell].networkIdx = -1;
      this.items.Remove(cell);
      this.Disconnect(cell);
      object obj;
      if (!this.endpoints.TryGetValue(cell, out obj) || networkIdx == -1)
        return;
      this.networks[networkIdx].DisconnectItem(obj);
    }
  }

  private unsafe void Disconnect(int cell)
  {
    Vector2I xy = Grid.CellToXY(cell);
    int index1 = 0;
    int* numPtr1 = stackalloc int[4];
    int* numPtr2 = stackalloc int[4];
    if (xy.y < Grid.HeightInCells - 1)
    {
      numPtr1[index1] = Grid.CellAbove(cell);
      numPtr2[index1] = -9;
      ++index1;
    }
    if (xy.y > 1)
    {
      numPtr1[index1] = Grid.CellBelow(cell);
      numPtr2[index1] = -5;
      ++index1;
    }
    if (xy.x > 1)
    {
      numPtr1[index1] = Grid.CellLeft(cell);
      numPtr2[index1] = -3;
      ++index1;
    }
    if (xy.x < Grid.WidthInCells - 1)
    {
      numPtr1[index1] = Grid.CellRight(cell);
      numPtr2[index1] = -2;
      ++index1;
    }
    for (int index2 = 0; index2 < index1; ++index2)
    {
      int index3 = numPtr1[index2];
      int num1 = numPtr2[index2];
      int num2 = (int) (this.physicalGrid[index3].connections & (UtilityConnections) num1);
      this.physicalGrid[index3].connections = (UtilityConnections) num2;
    }
  }

  private unsafe void RebuildNetworks(int layer, bool is_physical)
  {
    UtilityNetworkGridNode[] grid = this.GetGrid(is_physical);
    HashSet<int> nodes = this.GetNodes(is_physical);
    this.visitedCells.Clear();
    this.visitedVirtualKeys.Clear();
    this.queuedVirtualKeys.Clear();
    this.queued.Clear();
    int* numPtr1 = stackalloc int[4];
    int* numPtr2 = stackalloc int[4];
    foreach (int index1 in nodes)
    {
      UtilityNetworkGridNode utilityNetworkGridNode1 = grid[index1];
      if (!this.visitedCells.Contains(index1))
      {
        this.queued.Enqueue(index1);
        this.visitedCells.Add(index1);
        NetworkType networkType = new NetworkType();
        networkType.id = this.networks.Count;
        this.networks.Add((UtilityNetwork) networkType);
        while (this.queued.Count > 0)
        {
          int index2 = this.queued.Dequeue();
          UtilityNetworkGridNode utilityNetworkGridNode2 = grid[index2];
          object obj1 = (object) null;
          object obj2 = (object) null;
          if (is_physical)
          {
            if (this.items.TryGetValue(index2, out obj1))
            {
              if (!(obj1 is IDisconnectable) || !(obj1 as IDisconnectable).IsDisconnected())
              {
                if (obj1 != null)
                  networkType.AddItem(obj1);
              }
              else
                continue;
            }
            if (this.endpoints.TryGetValue(index2, out obj2) && obj2 != null)
              networkType.AddItem(obj2);
          }
          grid[index2].networkIdx = networkType.id;
          if (obj1 != null && obj2 != null)
            networkType.ConnectItem(obj2);
          Vector2I xy = Grid.CellToXY(index2);
          int index3 = 0;
          if (xy.x >= 0)
          {
            numPtr1[index3] = Grid.CellLeft(index2);
            numPtr2[index3] = 1;
            ++index3;
          }
          if (xy.x < Grid.WidthInCells)
          {
            numPtr1[index3] = Grid.CellRight(index2);
            numPtr2[index3] = 2;
            ++index3;
          }
          if (xy.y >= 0)
          {
            numPtr1[index3] = Grid.CellBelow(index2);
            numPtr2[index3] = 8;
            ++index3;
          }
          if (xy.y < Grid.HeightInCells)
          {
            numPtr1[index3] = Grid.CellAbove(index2);
            numPtr2[index3] = 4;
            ++index3;
          }
          for (int index4 = 0; index4 < index3; ++index4)
          {
            int direction = numPtr2[index4];
            if ((utilityNetworkGridNode2.connections & (UtilityConnections) direction) != (UtilityConnections) 0)
            {
              int dest_cell = numPtr1[index4];
              this.QueueCellForVisit(grid, dest_cell, (UtilityConnections) direction);
            }
          }
          int dest_cell1;
          if (this.links.TryGetValue(index2, out dest_cell1))
            this.QueueCellForVisit(grid, dest_cell1, (UtilityConnections) 0);
          object key;
          if (this.semiVirtualLinks.TryGetValue(index2, out key) && !this.visitedVirtualKeys.Contains(key))
          {
            this.visitedVirtualKeys.Add(key);
            this.virtualKeyToNetworkIdx[key] = networkType.id;
            if (this.virtualItems.ContainsKey(key))
            {
              foreach (object obj3 in this.virtualItems[key])
              {
                networkType.AddItem(obj3);
                networkType.ConnectItem(obj3);
              }
            }
            if (this.virtualEndpoints.ContainsKey(key))
            {
              foreach (object obj4 in this.virtualEndpoints[key])
              {
                networkType.AddItem(obj4);
                networkType.ConnectItem(obj4);
              }
            }
            foreach (KeyValuePair<int, object> semiVirtualLink in this.semiVirtualLinks)
            {
              if (semiVirtualLink.Value == key)
                this.QueueCellForVisit(grid, semiVirtualLink.Key, (UtilityConnections) 0);
            }
          }
        }
      }
    }
    foreach (KeyValuePair<object, List<object>> virtualItem in this.virtualItems)
    {
      if (!this.visitedVirtualKeys.Contains(virtualItem.Key))
      {
        NetworkType networkType = new NetworkType();
        networkType.id = this.networks.Count;
        this.visitedVirtualKeys.Add(virtualItem.Key);
        this.virtualKeyToNetworkIdx[virtualItem.Key] = networkType.id;
        foreach (object obj in virtualItem.Value)
        {
          networkType.AddItem(obj);
          networkType.ConnectItem(obj);
        }
        foreach (object obj in this.virtualEndpoints[virtualItem.Key])
        {
          networkType.AddItem(obj);
          networkType.ConnectItem(obj);
        }
        this.networks.Add((UtilityNetwork) networkType);
      }
    }
    foreach (KeyValuePair<object, List<object>> virtualEndpoint in this.virtualEndpoints)
    {
      if (!this.visitedVirtualKeys.Contains(virtualEndpoint.Key))
      {
        NetworkType networkType = new NetworkType();
        networkType.id = this.networks.Count;
        this.visitedVirtualKeys.Add(virtualEndpoint.Key);
        this.virtualKeyToNetworkIdx[virtualEndpoint.Key] = networkType.id;
        foreach (object obj in this.virtualEndpoints[virtualEndpoint.Key])
        {
          networkType.AddItem(obj);
          networkType.ConnectItem(obj);
        }
        this.networks.Add((UtilityNetwork) networkType);
      }
    }
  }

  public UtilityNetwork GetNetworkForVirtualKey(object key)
  {
    int index;
    return this.virtualKeyToNetworkIdx.TryGetValue(key, out index) ? this.networks[index] : (UtilityNetwork) null;
  }

  public UtilityNetwork GetNetworkByID(int id)
  {
    UtilityNetwork networkById = (UtilityNetwork) null;
    if (0 <= id && id < this.networks.Count)
      networkById = this.networks[id];
    return networkById;
  }

  public UtilityNetwork GetNetworkForCell(int cell)
  {
    UtilityNetwork networkForCell = (UtilityNetwork) null;
    if (Grid.IsValidCell(cell) && 0 <= this.physicalGrid[cell].networkIdx && this.physicalGrid[cell].networkIdx < this.networks.Count)
      networkForCell = this.networks[this.physicalGrid[cell].networkIdx];
    return networkForCell;
  }

  public UtilityNetwork GetNetworkForDirection(int cell, Direction direction)
  {
    cell = Grid.GetCellInDirection(cell, direction);
    if (!Grid.IsValidCell(cell))
      return (UtilityNetwork) null;
    UtilityNetworkGridNode utilityNetworkGridNode = this.GetGrid(true)[cell];
    UtilityNetwork networkForDirection = (UtilityNetwork) null;
    if (utilityNetworkGridNode.networkIdx != -1 && utilityNetworkGridNode.networkIdx < this.networks.Count)
      networkForDirection = this.networks[utilityNetworkGridNode.networkIdx];
    return networkForDirection;
  }

  private UtilityConnections GetNeighboursAsConnections(int cell, HashSet<int> nodes)
  {
    UtilityConnections neighboursAsConnections = (UtilityConnections) 0;
    Vector2I xy = Grid.CellToXY(cell);
    if (xy.x > 1 && nodes.Contains(Grid.CellLeft(cell)))
      neighboursAsConnections |= UtilityConnections.Left;
    if (xy.x < Grid.WidthInCells - 1 && nodes.Contains(Grid.CellRight(cell)))
      neighboursAsConnections |= UtilityConnections.Right;
    if (xy.y > 1 && nodes.Contains(Grid.CellBelow(cell)))
      neighboursAsConnections |= UtilityConnections.Down;
    if (xy.y < Grid.HeightInCells - 1 && nodes.Contains(Grid.CellAbove(cell)))
      neighboursAsConnections |= UtilityConnections.Up;
    return neighboursAsConnections;
  }

  public virtual void SetConnections(
    UtilityConnections connections,
    int cell,
    bool is_physical_building)
  {
    HashSet<int> nodes = this.GetNodes(is_physical_building);
    nodes.Add(cell);
    this.visualGrid[cell].connections = connections;
    if (is_physical_building)
    {
      this.dirty = true;
      UtilityConnections utilityConnections = is_physical_building ? connections & this.GetNeighboursAsConnections(cell, nodes) : connections;
      this.physicalGrid[cell].connections = utilityConnections;
    }
    this.Reconnect(cell);
  }

  public UtilityConnections GetConnections(int cell, bool is_physical_building)
  {
    UtilityConnections connections = this.GetGrid(is_physical_building)[cell].connections;
    if (!is_physical_building)
    {
      UtilityNetworkGridNode[] grid = this.GetGrid(true);
      connections |= grid[cell].connections;
    }
    return connections;
  }

  public UtilityConnections GetDisplayConnections(int cell) => (UtilityConnections) 0 | this.GetGrid(false)[cell].connections | this.GetGrid(true)[cell].connections;

  public virtual bool CanAddConnection(
    UtilityConnections new_connection,
    int cell,
    bool is_physical_building,
    out string fail_reason)
  {
    fail_reason = (string) null;
    return true;
  }

  public void AddConnection(UtilityConnections new_connection, int cell, bool is_physical_building)
  {
    if (!this.CanAddConnection(new_connection, cell, is_physical_building, out string _))
      return;
    if (is_physical_building)
      this.dirty = true;
    UtilityNetworkGridNode[] grid = this.GetGrid(is_physical_building);
    UtilityConnections connections = grid[cell].connections;
    grid[cell].connections = connections | new_connection;
  }

  public void StashVisualGrids() => Array.Copy((Array) this.visualGrid, (Array) this.stashedVisualGrid, this.visualGrid.Length);

  public void UnstashVisualGrids() => Array.Copy((Array) this.stashedVisualGrid, (Array) this.visualGrid, this.visualGrid.Length);

  public string GetVisualizerString(int cell) => this.GetVisualizerString(this.GetDisplayConnections(cell));

  public string GetVisualizerString(UtilityConnections connections)
  {
    string visualizerString = "";
    if ((connections & UtilityConnections.Left) != (UtilityConnections) 0)
      visualizerString += "L";
    if ((connections & UtilityConnections.Right) != (UtilityConnections) 0)
      visualizerString += "R";
    if ((connections & UtilityConnections.Up) != (UtilityConnections) 0)
      visualizerString += "U";
    if ((connections & UtilityConnections.Down) != (UtilityConnections) 0)
      visualizerString += "D";
    if (visualizerString == "")
      visualizerString = "None";
    return visualizerString;
  }

  public object GetEndpoint(int cell)
  {
    object endpoint = (object) null;
    this.endpoints.TryGetValue(cell, out endpoint);
    return endpoint;
  }

  public void AddSemiVirtualLink(int cell1, object virtualKey)
  {
    Debug.Assert(virtualKey != null, (object) "Can not use a null key for a virtual network");
    this.semiVirtualLinks[cell1] = virtualKey;
    this.dirty = true;
  }

  public void RemoveSemiVirtualLink(int cell1, object virtualKey)
  {
    Debug.Assert(virtualKey != null, (object) "Can not use a null key for a virtual network");
    this.semiVirtualLinks.Remove(cell1);
    this.dirty = true;
  }

  public void AddLink(int cell1, int cell2)
  {
    this.links[cell1] = cell2;
    this.links[cell2] = cell1;
    this.dirty = true;
  }

  public void RemoveLink(int cell1, int cell2)
  {
    this.links.Remove(cell1);
    this.links.Remove(cell2);
    this.dirty = true;
  }

  public void AddNetworksRebuiltListener(
    Action<IList<UtilityNetwork>, ICollection<int>> listener)
  {
    this.onNetworksRebuilt += listener;
  }

  public void RemoveNetworksRebuiltListener(
    Action<IList<UtilityNetwork>, ICollection<int>> listener)
  {
    this.onNetworksRebuilt -= listener;
  }

  public IList<UtilityNetwork> GetNetworks() => (IList<UtilityNetwork>) this.networks;
}
