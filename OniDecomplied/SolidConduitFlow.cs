// Decompiled with JetBrains decompiler
// Type: SolidConduitFlow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[SerializationConfig]
public class SolidConduitFlow : IConduitFlow
{
  public const float MAX_SOLID_MASS = 20f;
  public const float TickRate = 1f;
  public const float WaitTime = 1f;
  private float elapsedTime;
  private float lastUpdateTime = float.NegativeInfinity;
  private KCompactedVector<SolidConduitFlow.StoredInfo> conveyorPickupables = new KCompactedVector<SolidConduitFlow.StoredInfo>(0);
  private List<HandleVector<int>.Handle> freedHandles = new List<HandleVector<int>.Handle>();
  private SolidConduitFlow.SOAInfo soaInfo = new SolidConduitFlow.SOAInfo();
  private bool dirtyConduitUpdaters;
  private List<SolidConduitFlow.ConduitUpdater> conduitUpdaters = new List<SolidConduitFlow.ConduitUpdater>();
  private SolidConduitFlow.GridNode[] grid;
  public IUtilityNetworkMgr networkMgr;
  private HashSet<int> visited = new HashSet<int>();
  private HashSet<int> replacements = new HashSet<int>();
  private List<SolidConduitFlow.Conduit> path = new List<SolidConduitFlow.Conduit>();
  private List<List<SolidConduitFlow.Conduit>> pathList = new List<List<SolidConduitFlow.Conduit>>();
  public static readonly SolidConduitFlow.ConduitContents emptyContents = new SolidConduitFlow.ConduitContents()
  {
    pickupableHandle = HandleVector<int>.InvalidHandle
  };
  private int maskedOverlayLayer;
  private bool viewingConduits;
  private static readonly Color32 NormalColour = Color32.op_Implicit(Color.white);
  private static readonly Color32 OverlayColour = Color32.op_Implicit(new Color(0.25f, 0.25f, 0.25f, 0.0f));

  public SolidConduitFlow.SOAInfo GetSOAInfo() => this.soaInfo;

  public event System.Action onConduitsRebuilt;

  public void AddConduitUpdater(Action<float> callback, ConduitFlowPriority priority = ConduitFlowPriority.Default)
  {
    this.conduitUpdaters.Add(new SolidConduitFlow.ConduitUpdater()
    {
      priority = priority,
      callback = callback
    });
    this.dirtyConduitUpdaters = true;
  }

  public void RemoveConduitUpdater(Action<float> callback)
  {
    for (int index = 0; index < this.conduitUpdaters.Count; ++index)
    {
      if (this.conduitUpdaters[index].callback == callback)
      {
        this.conduitUpdaters.RemoveAt(index);
        this.dirtyConduitUpdaters = true;
        break;
      }
    }
  }

  public static int FlowBit(SolidConduitFlow.FlowDirection direction) => 1 << (int) (direction - 1 & (SolidConduitFlow.FlowDirection) 31);

  public SolidConduitFlow(
    int num_cells,
    IUtilityNetworkMgr network_mgr,
    float initial_elapsed_time)
  {
    this.elapsedTime = initial_elapsed_time;
    this.networkMgr = network_mgr;
    this.maskedOverlayLayer = LayerMask.NameToLayer("MaskedOverlay");
    this.Initialize(num_cells);
    network_mgr.AddNetworksRebuiltListener(new Action<IList<UtilityNetwork>, ICollection<int>>(this.OnUtilityNetworksRebuilt));
  }

  public void Initialize(int num_cells)
  {
    this.grid = new SolidConduitFlow.GridNode[num_cells];
    for (int index = 0; index < num_cells; ++index)
    {
      this.grid[index].conduitIdx = -1;
      this.grid[index].contents.pickupableHandle = HandleVector<int>.InvalidHandle;
    }
  }

  private void OnUtilityNetworksRebuilt(IList<UtilityNetwork> networks, ICollection<int> root_nodes)
  {
    this.RebuildConnections((IEnumerable<int>) root_nodes);
    foreach (FlowUtilityNetwork network in (IEnumerable<UtilityNetwork>) networks)
      this.ScanNetworkSources(network);
    this.RefreshPaths();
  }

  private void RebuildConnections(IEnumerable<int> root_nodes)
  {
    this.soaInfo.Clear(this);
    this.pathList.Clear();
    ObjectLayer layer = ObjectLayer.SolidConduit;
    foreach (int rootNode in root_nodes)
    {
      if (this.replacements.Contains(rootNode))
        this.replacements.Remove(rootNode);
      GameObject conduit_go = Grid.Objects[rootNode, (int) layer];
      if (!Object.op_Equality((Object) conduit_go, (Object) null))
      {
        int num = this.soaInfo.AddConduit(this, conduit_go, rootNode);
        this.grid[rootNode].conduitIdx = num;
      }
    }
    Game.Instance.conduitTemperatureManager.Sim200ms(0.0f);
    foreach (int rootNode in root_nodes)
    {
      UtilityConnections connections = this.networkMgr.GetConnections(rootNode, true);
      if (connections != (UtilityConnections) 0 && this.grid[rootNode].conduitIdx != -1)
      {
        int conduitIdx = this.grid[rootNode].conduitIdx;
        SolidConduitFlow.ConduitConnections conduitConnections = this.soaInfo.GetConduitConnections(conduitIdx);
        int cell1 = rootNode - 1;
        if (Grid.IsValidCell(cell1) && (connections & UtilityConnections.Left) != (UtilityConnections) 0)
          conduitConnections.left = this.grid[cell1].conduitIdx;
        int cell2 = rootNode + 1;
        if (Grid.IsValidCell(cell2) && (connections & UtilityConnections.Right) != (UtilityConnections) 0)
          conduitConnections.right = this.grid[cell2].conduitIdx;
        int cell3 = rootNode - Grid.WidthInCells;
        if (Grid.IsValidCell(cell3) && (connections & UtilityConnections.Down) != (UtilityConnections) 0)
          conduitConnections.down = this.grid[cell3].conduitIdx;
        int cell4 = rootNode + Grid.WidthInCells;
        if (Grid.IsValidCell(cell4) && (connections & UtilityConnections.Up) != (UtilityConnections) 0)
          conduitConnections.up = this.grid[cell4].conduitIdx;
        this.soaInfo.SetConduitConnections(conduitIdx, conduitConnections);
      }
    }
    if (this.onConduitsRebuilt == null)
      return;
    this.onConduitsRebuilt();
  }

  public void ScanNetworkSources(FlowUtilityNetwork network)
  {
    if (network == null)
      return;
    for (int index = 0; index < network.sources.Count; ++index)
    {
      FlowUtilityNetwork.IItem source = network.sources[index];
      this.path.Clear();
      this.visited.Clear();
      this.FindSinks(index, source.Cell);
    }
  }

  public void RefreshPaths()
  {
    foreach (List<SolidConduitFlow.Conduit> path in this.pathList)
    {
      for (int index = 0; index < path.Count - 1; ++index)
      {
        SolidConduitFlow.Conduit conduit = path[index];
        SolidConduitFlow.Conduit target_conduit = path[index + 1];
        if (conduit.GetTargetFlowDirection(this) == SolidConduitFlow.FlowDirection.None)
        {
          SolidConduitFlow.FlowDirection direction = this.GetDirection(conduit, target_conduit);
          conduit.SetTargetFlowDirection(direction, this);
        }
      }
    }
  }

  private void FindSinks(int source_idx, int cell)
  {
    SolidConduitFlow.GridNode gridNode = this.grid[cell];
    if (gridNode.conduitIdx == -1)
      return;
    this.FindSinksInternal(source_idx, gridNode.conduitIdx);
  }

  private void FindSinksInternal(int source_idx, int conduit_idx)
  {
    if (this.visited.Contains(conduit_idx))
      return;
    this.visited.Add(conduit_idx);
    SolidConduitFlow.Conduit conduit = this.soaInfo.GetConduit(conduit_idx);
    if (conduit.GetPermittedFlowDirections(this) == -1)
      return;
    this.path.Add(conduit);
    FlowUtilityNetwork.IItem endpoint = (FlowUtilityNetwork.IItem) this.networkMgr.GetEndpoint(this.soaInfo.GetCell(conduit_idx));
    if (endpoint != null && endpoint.EndpointType == Endpoint.Sink)
      this.FoundSink(source_idx);
    SolidConduitFlow.ConduitConnections conduitConnections = this.soaInfo.GetConduitConnections(conduit_idx);
    if (conduitConnections.down != -1)
      this.FindSinksInternal(source_idx, conduitConnections.down);
    if (conduitConnections.left != -1)
      this.FindSinksInternal(source_idx, conduitConnections.left);
    if (conduitConnections.right != -1)
      this.FindSinksInternal(source_idx, conduitConnections.right);
    if (conduitConnections.up != -1)
      this.FindSinksInternal(source_idx, conduitConnections.up);
    if (this.path.Count <= 0)
      return;
    this.path.RemoveAt(this.path.Count - 1);
  }

  private SolidConduitFlow.FlowDirection GetDirection(
    SolidConduitFlow.Conduit conduit,
    SolidConduitFlow.Conduit target_conduit)
  {
    SolidConduitFlow.ConduitConnections conduitConnections = this.soaInfo.GetConduitConnections(conduit.idx);
    if (conduitConnections.up == target_conduit.idx)
      return SolidConduitFlow.FlowDirection.Up;
    if (conduitConnections.down == target_conduit.idx)
      return SolidConduitFlow.FlowDirection.Down;
    if (conduitConnections.left == target_conduit.idx)
      return SolidConduitFlow.FlowDirection.Left;
    return conduitConnections.right == target_conduit.idx ? SolidConduitFlow.FlowDirection.Right : SolidConduitFlow.FlowDirection.None;
  }

  private void FoundSink(int source_idx)
  {
    for (int index = 0; index < this.path.Count - 1; ++index)
    {
      SolidConduitFlow.FlowDirection direction1 = this.GetDirection(this.path[index], this.path[index + 1]);
      SolidConduitFlow.FlowDirection direction2 = SolidConduitFlow.InverseFlow(direction1);
      int cellFromDirection = SolidConduitFlow.GetCellFromDirection(this.soaInfo.GetCell(this.path[index].idx), direction2);
      SolidConduitFlow.Conduit conduitFromDirection = this.soaInfo.GetConduitFromDirection(this.path[index].idx, direction2);
      SolidConduitFlow.Conduit conduit;
      if (index != 0)
      {
        conduit = this.path[index];
        if ((conduit.GetPermittedFlowDirections(this) & SolidConduitFlow.FlowBit(direction2)) != 0 && (cellFromDirection == this.soaInfo.GetCell(this.path[index - 1].idx) || this.soaInfo.GetSrcFlowIdx(this.path[index].idx) != source_idx && (conduitFromDirection.GetPermittedFlowDirections(this) & SolidConduitFlow.FlowBit(direction2)) != 0))
          continue;
      }
      conduit = this.path[index];
      int permittedFlowDirections = conduit.GetPermittedFlowDirections(this);
      this.soaInfo.SetSrcFlowIdx(this.path[index].idx, source_idx);
      conduit = this.path[index];
      conduit.SetPermittedFlowDirections(permittedFlowDirections | SolidConduitFlow.FlowBit(direction1), this);
      conduit = this.path[index];
      conduit.SetTargetFlowDirection(direction1, this);
    }
    for (int index = 1; index < this.path.Count; ++index)
    {
      SolidConduitFlow.FlowDirection direction = this.GetDirection(this.path[index], this.path[index - 1]);
      this.soaInfo.SetSrcFlowDirection(this.path[index].idx, direction);
    }
    List<SolidConduitFlow.Conduit> new_path = new List<SolidConduitFlow.Conduit>((IEnumerable<SolidConduitFlow.Conduit>) this.path);
    new_path.Reverse();
    this.TryAdd(new_path);
  }

  private void TryAdd(List<SolidConduitFlow.Conduit> new_path)
  {
    foreach (List<SolidConduitFlow.Conduit> path in this.pathList)
    {
      if (path.Count >= new_path.Count)
      {
        bool flag = false;
        int index1 = path.FindIndex((Predicate<SolidConduitFlow.Conduit>) (t => t.idx == new_path[0].idx));
        int index2 = path.FindIndex((Predicate<SolidConduitFlow.Conduit>) (t => t.idx == new_path[new_path.Count - 1].idx));
        if (index1 != -1 && index2 != -1)
        {
          flag = true;
          int index3 = index1;
          int index4 = 0;
          while (index3 < index2)
          {
            if (path[index3].idx != new_path[index4].idx)
            {
              flag = false;
              break;
            }
            ++index3;
            ++index4;
          }
        }
        if (flag)
          return;
      }
    }
    for (int index = this.pathList.Count - 1; index >= 0; --index)
    {
      if (this.pathList[index].Count <= 0)
        this.pathList.RemoveAt(index);
    }
    for (int index5 = this.pathList.Count - 1; index5 >= 0; --index5)
    {
      List<SolidConduitFlow.Conduit> old_path = this.pathList[index5];
      if (new_path.Count >= old_path.Count)
      {
        bool flag = false;
        int index6 = new_path.FindIndex((Predicate<SolidConduitFlow.Conduit>) (t => t.idx == old_path[0].idx));
        int index7 = new_path.FindIndex((Predicate<SolidConduitFlow.Conduit>) (t => t.idx == old_path[old_path.Count - 1].idx));
        if (index6 != -1 && index7 != -1)
        {
          flag = true;
          int index8 = index6;
          int index9 = 0;
          while (index8 < index7)
          {
            if (new_path[index8].idx != old_path[index9].idx)
            {
              flag = false;
              break;
            }
            ++index8;
            ++index9;
          }
        }
        if (flag)
          this.pathList.RemoveAt(index5);
      }
    }
    foreach (List<SolidConduitFlow.Conduit> path in this.pathList)
    {
      for (int index = new_path.Count - 1; index >= 0; --index)
      {
        SolidConduitFlow.Conduit new_conduit = new_path[index];
        if (path.FindIndex((Predicate<SolidConduitFlow.Conduit>) (t => t.idx == new_conduit.idx)) != -1 && Mathf.IsPowerOfTwo(this.soaInfo.GetPermittedFlowDirections(new_conduit.idx)))
          new_path.RemoveAt(index);
      }
    }
    this.pathList.Add(new_path);
  }

  public SolidConduitFlow.ConduitContents GetContents(int cell)
  {
    SolidConduitFlow.ConduitContents contents = this.grid[cell].contents;
    SolidConduitFlow.GridNode gridNode = this.grid[cell];
    if (gridNode.conduitIdx != -1)
      contents = this.soaInfo.GetConduit(gridNode.conduitIdx).GetContents(this);
    return contents;
  }

  private void SetContents(int cell, SolidConduitFlow.ConduitContents contents)
  {
    SolidConduitFlow.GridNode gridNode = this.grid[cell];
    if (gridNode.conduitIdx != -1)
      this.soaInfo.GetConduit(gridNode.conduitIdx).SetContents(this, contents);
    else
      this.grid[cell].contents = contents;
  }

  public void SetContents(int cell, Pickupable pickupable)
  {
    SolidConduitFlow.ConduitContents contents = new SolidConduitFlow.ConduitContents()
    {
      pickupableHandle = HandleVector<int>.InvalidHandle
    };
    if (Object.op_Inequality((Object) pickupable, (Object) null))
    {
      KBatchedAnimController component1 = ((Component) pickupable).GetComponent<KBatchedAnimController>();
      SolidConduitFlow.StoredInfo storedInfo = new SolidConduitFlow.StoredInfo()
      {
        kbac = component1,
        pickupable = pickupable
      };
      contents.pickupableHandle = this.conveyorPickupables.Allocate(storedInfo);
      KBatchedAnimController component2 = ((Component) pickupable).GetComponent<KBatchedAnimController>();
      component2.enabled = false;
      component2.enabled = true;
      pickupable.Trigger(856640610, (object) true);
    }
    this.SetContents(cell, contents);
  }

  public static int GetCellFromDirection(int cell, SolidConduitFlow.FlowDirection direction)
  {
    switch (direction)
    {
      case SolidConduitFlow.FlowDirection.Left:
        return Grid.CellLeft(cell);
      case SolidConduitFlow.FlowDirection.Right:
        return Grid.CellRight(cell);
      case SolidConduitFlow.FlowDirection.Up:
        return Grid.CellAbove(cell);
      case SolidConduitFlow.FlowDirection.Down:
        return Grid.CellBelow(cell);
      default:
        return -1;
    }
  }

  public static SolidConduitFlow.FlowDirection InverseFlow(SolidConduitFlow.FlowDirection direction)
  {
    switch (direction)
    {
      case SolidConduitFlow.FlowDirection.Left:
        return SolidConduitFlow.FlowDirection.Right;
      case SolidConduitFlow.FlowDirection.Right:
        return SolidConduitFlow.FlowDirection.Left;
      case SolidConduitFlow.FlowDirection.Up:
        return SolidConduitFlow.FlowDirection.Down;
      case SolidConduitFlow.FlowDirection.Down:
        return SolidConduitFlow.FlowDirection.Up;
      default:
        return SolidConduitFlow.FlowDirection.None;
    }
  }

  public void Sim200ms(float dt)
  {
    if ((double) dt <= 0.0)
      return;
    this.elapsedTime += dt;
    if ((double) this.elapsedTime < 1.0)
      return;
    float num = 1f;
    --this.elapsedTime;
    this.lastUpdateTime = Time.time;
    this.soaInfo.BeginFrame(this);
    foreach (List<SolidConduitFlow.Conduit> path in this.pathList)
    {
      foreach (SolidConduitFlow.Conduit conduit in path)
        this.UpdateConduit(conduit);
    }
    this.soaInfo.UpdateFlowDirection(this);
    if (this.dirtyConduitUpdaters)
      this.conduitUpdaters.Sort((Comparison<SolidConduitFlow.ConduitUpdater>) ((a, b) => a.priority - b.priority));
    this.soaInfo.EndFrame(this);
    for (int index = 0; index < this.conduitUpdaters.Count; ++index)
      this.conduitUpdaters[index].callback(num);
  }

  public void RenderEveryTick(float dt)
  {
    for (int idx = 0; idx < this.GetSOAInfo().NumEntries; ++idx)
    {
      SolidConduitFlow.Conduit conduit = this.GetSOAInfo().GetConduit(idx);
      SolidConduitFlow.ConduitFlowInfo lastFlowInfo = conduit.GetLastFlowInfo(this);
      if (lastFlowInfo.direction != SolidConduitFlow.FlowDirection.None)
      {
        int cell = conduit.GetCell(this);
        int cellFromDirection = SolidConduitFlow.GetCellFromDirection(cell, lastFlowInfo.direction);
        SolidConduitFlow.ConduitContents contents = this.GetContents(cellFromDirection);
        if (contents.pickupableHandle.IsValid())
        {
          Vector3 vector3 = Vector3.Lerp(Grid.CellToPosCCC(cell, Grid.SceneLayer.SolidConduitContents), Grid.CellToPosCCC(cellFromDirection, Grid.SceneLayer.SolidConduitContents), this.ContinuousLerpPercent);
          Pickupable pickupable = this.GetPickupable(contents.pickupableHandle);
          if (Object.op_Inequality((Object) pickupable, (Object) null))
            TransformExtensions.SetPosition(pickupable.transform, vector3);
        }
      }
    }
  }

  private void UpdateConduit(SolidConduitFlow.Conduit conduit)
  {
    if (this.soaInfo.GetUpdated(conduit.idx))
      return;
    if (this.soaInfo.GetSrcFlowDirection(conduit.idx) == SolidConduitFlow.FlowDirection.None)
      this.soaInfo.SetSrcFlowDirection(conduit.idx, conduit.GetNextFlowSource(this));
    if (!this.grid[this.soaInfo.GetCell(conduit.idx)].contents.pickupableHandle.IsValid())
      return;
    SolidConduitFlow.FlowDirection targetFlowDirection = this.soaInfo.GetTargetFlowDirection(conduit.idx);
    SolidConduitFlow.Conduit conduitFromDirection1 = this.soaInfo.GetConduitFromDirection(conduit.idx, targetFlowDirection);
    if (conduitFromDirection1.idx == -1)
    {
      this.soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
    }
    else
    {
      int cell = this.soaInfo.GetCell(conduitFromDirection1.idx);
      SolidConduitFlow.ConduitContents contents1 = this.grid[cell].contents;
      if (contents1.pickupableHandle.IsValid())
      {
        this.soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
      }
      else
      {
        if ((this.soaInfo.GetPermittedFlowDirections(conduit.idx) & SolidConduitFlow.FlowBit(targetFlowDirection)) != 0)
        {
          bool flag = false;
          for (int index = 0; index < 5; ++index)
          {
            SolidConduitFlow.Conduit conduitFromDirection2 = this.soaInfo.GetConduitFromDirection(conduitFromDirection1.idx, this.soaInfo.GetSrcFlowDirection(conduitFromDirection1.idx));
            if (conduitFromDirection2.idx == conduit.idx)
            {
              flag = true;
              break;
            }
            if (conduitFromDirection2.idx == -1 || !this.grid[this.soaInfo.GetCell(conduitFromDirection2.idx)].contents.pickupableHandle.IsValid())
              this.soaInfo.SetSrcFlowDirection(conduitFromDirection1.idx, conduitFromDirection1.GetNextFlowSource(this));
            else
              break;
          }
          if (flag && !contents1.pickupableHandle.IsValid())
          {
            SolidConduitFlow.ConduitContents contents2 = this.RemoveFromGrid(conduit);
            this.AddToGrid(cell, contents2);
            this.soaInfo.SetLastFlowInfo(conduit.idx, this.soaInfo.GetTargetFlowDirection(conduit.idx));
            this.soaInfo.SetUpdated(conduitFromDirection1.idx, true);
            this.soaInfo.SetSrcFlowDirection(conduitFromDirection1.idx, conduitFromDirection1.GetNextFlowSource(this));
          }
        }
        this.soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
      }
    }
  }

  public float ContinuousLerpPercent => Mathf.Clamp01((float) (((double) Time.time - (double) this.lastUpdateTime) / 1.0));

  public float DiscreteLerpPercent => Mathf.Clamp01(this.elapsedTime / 1f);

  private void AddToGrid(int cell_idx, SolidConduitFlow.ConduitContents contents) => this.grid[cell_idx].contents = contents;

  private SolidConduitFlow.ConduitContents RemoveFromGrid(SolidConduitFlow.Conduit conduit)
  {
    int cell = this.soaInfo.GetCell(conduit.idx);
    SolidConduitFlow.ConduitContents contents = this.grid[cell].contents;
    SolidConduitFlow.ConduitContents conduitContents = SolidConduitFlow.ConduitContents.EmptyContents();
    this.grid[cell].contents = conduitContents;
    return contents;
  }

  public void AddPickupable(int cell_idx, Pickupable pickupable)
  {
    if (this.grid[cell_idx].conduitIdx == -1)
    {
      Debug.LogWarning((object) ("No conduit in cell: " + cell_idx.ToString()));
      this.DumpPickupable(pickupable);
    }
    else
    {
      SolidConduitFlow.ConduitContents contents = this.GetConduit(cell_idx).GetContents(this);
      if (contents.pickupableHandle.IsValid())
      {
        Debug.LogWarning((object) ("Conduit already full: " + cell_idx.ToString()));
        this.DumpPickupable(pickupable);
      }
      else
      {
        KBatchedAnimController component = ((Component) pickupable).GetComponent<KBatchedAnimController>();
        SolidConduitFlow.StoredInfo storedInfo = new SolidConduitFlow.StoredInfo()
        {
          kbac = component,
          pickupable = pickupable
        };
        contents.pickupableHandle = this.conveyorPickupables.Allocate(storedInfo);
        if (this.viewingConduits)
          this.ApplyOverlayVisualization(component);
        if (Object.op_Implicit((Object) pickupable.storage))
          pickupable.storage.Remove(((Component) pickupable).gameObject);
        pickupable.Trigger(856640610, (object) true);
        this.SetContents(cell_idx, contents);
      }
    }
  }

  public Pickupable RemovePickupable(int cell_idx)
  {
    Pickupable pickupable = (Pickupable) null;
    SolidConduitFlow.Conduit conduit = this.GetConduit(cell_idx);
    if (conduit.idx != -1)
    {
      SolidConduitFlow.ConduitContents conduitContents = this.RemoveFromGrid(conduit);
      if (conduitContents.pickupableHandle.IsValid())
      {
        SolidConduitFlow.StoredInfo data = this.conveyorPickupables.GetData(conduitContents.pickupableHandle);
        this.ClearOverlayVisualization(data.kbac);
        pickupable = data.pickupable;
        if (Object.op_Implicit((Object) pickupable))
          pickupable.Trigger(856640610, (object) false);
        this.freedHandles.Add(conduitContents.pickupableHandle);
      }
    }
    return pickupable;
  }

  public int GetPermittedFlow(int cell)
  {
    SolidConduitFlow.Conduit conduit = this.GetConduit(cell);
    return conduit.idx == -1 ? 0 : this.soaInfo.GetPermittedFlowDirections(conduit.idx);
  }

  public bool HasConduit(int cell) => this.grid[cell].conduitIdx != -1;

  public SolidConduitFlow.Conduit GetConduit(int cell)
  {
    int conduitIdx = this.grid[cell].conduitIdx;
    return conduitIdx == -1 ? SolidConduitFlow.Conduit.Invalid() : this.soaInfo.GetConduit(conduitIdx);
  }

  private void DumpPipeContents(int cell)
  {
    Pickupable pickupable = this.RemovePickupable(cell);
    if (!Object.op_Implicit((Object) pickupable))
      return;
    pickupable.transform.parent = (Transform) null;
  }

  private void DumpPickupable(Pickupable pickupable)
  {
    if (!Object.op_Implicit((Object) pickupable))
      return;
    pickupable.transform.parent = (Transform) null;
  }

  public void EmptyConduit(int cell)
  {
    if (this.replacements.Contains(cell))
      return;
    this.DumpPipeContents(cell);
  }

  public void MarkForReplacement(int cell) => this.replacements.Add(cell);

  public void DeactivateCell(int cell)
  {
    this.grid[cell].conduitIdx = -1;
    SolidConduitFlow.ConduitContents contents = SolidConduitFlow.ConduitContents.EmptyContents();
    this.SetContents(cell, contents);
  }

  public UtilityNetwork GetNetwork(SolidConduitFlow.Conduit conduit) => this.networkMgr.GetNetworkForCell(this.soaInfo.GetCell(conduit.idx));

  public void ForceRebuildNetworks() => this.networkMgr.ForceRebuildNetworks();

  public bool IsConduitFull(int cell_idx) => this.grid[cell_idx].contents.pickupableHandle.IsValid();

  public bool IsConduitEmpty(int cell_idx) => !this.grid[cell_idx].contents.pickupableHandle.IsValid();

  public void Initialize()
  {
    if (!Object.op_Inequality((Object) OverlayScreen.Instance, (Object) null))
      return;
    OverlayScreen.Instance.OnOverlayChanged -= new Action<HashedString>(this.OnOverlayChanged);
    OverlayScreen.Instance.OnOverlayChanged += new Action<HashedString>(this.OnOverlayChanged);
  }

  private void OnOverlayChanged(HashedString mode)
  {
    bool flag = HashedString.op_Equality(mode, OverlayModes.SolidConveyor.ID);
    if (flag == this.viewingConduits)
      return;
    this.viewingConduits = flag;
    int layer = this.viewingConduits ? this.maskedOverlayLayer : Game.PickupableLayer;
    Color32 color32 = this.viewingConduits ? SolidConduitFlow.OverlayColour : SolidConduitFlow.NormalColour;
    List<SolidConduitFlow.StoredInfo> dataList = this.conveyorPickupables.GetDataList();
    for (int index = 0; index < dataList.Count; ++index)
    {
      SolidConduitFlow.StoredInfo storedInfo = dataList[index];
      if (Object.op_Inequality((Object) storedInfo.kbac, (Object) null))
      {
        storedInfo.kbac.SetLayer(layer);
        storedInfo.kbac.TintColour = color32;
      }
    }
  }

  private void ApplyOverlayVisualization(KBatchedAnimController kbac)
  {
    if (Object.op_Equality((Object) kbac, (Object) null))
      return;
    kbac.SetLayer(this.maskedOverlayLayer);
    kbac.TintColour = SolidConduitFlow.OverlayColour;
  }

  private void ClearOverlayVisualization(KBatchedAnimController kbac)
  {
    if (Object.op_Equality((Object) kbac, (Object) null))
      return;
    kbac.SetLayer(Game.PickupableLayer);
    kbac.TintColour = SolidConduitFlow.NormalColour;
  }

  public Pickupable GetPickupable(HandleVector<int>.Handle h)
  {
    Pickupable pickupable = (Pickupable) null;
    if (h.IsValid())
      pickupable = this.conveyorPickupables.GetData(h).pickupable;
    return pickupable;
  }

  private struct StoredInfo
  {
    public KBatchedAnimController kbac;
    public Pickupable pickupable;
  }

  public class SOAInfo
  {
    private List<SolidConduitFlow.Conduit> conduits = new List<SolidConduitFlow.Conduit>();
    private List<SolidConduitFlow.ConduitConnections> conduitConnections = new List<SolidConduitFlow.ConduitConnections>();
    private List<SolidConduitFlow.ConduitFlowInfo> lastFlowInfo = new List<SolidConduitFlow.ConduitFlowInfo>();
    private List<SolidConduitFlow.ConduitContents> initialContents = new List<SolidConduitFlow.ConduitContents>();
    private List<GameObject> conduitGOs = new List<GameObject>();
    private List<bool> diseaseContentsVisible = new List<bool>();
    private List<bool> updated = new List<bool>();
    private List<int> cells = new List<int>();
    private List<int> permittedFlowDirections = new List<int>();
    private List<int> srcFlowIdx = new List<int>();
    private List<SolidConduitFlow.FlowDirection> srcFlowDirections = new List<SolidConduitFlow.FlowDirection>();
    private List<SolidConduitFlow.FlowDirection> targetFlowDirections = new List<SolidConduitFlow.FlowDirection>();

    public int NumEntries => this.conduits.Count;

    public List<int> Cells => this.cells;

    public int AddConduit(SolidConduitFlow manager, GameObject conduit_go, int cell)
    {
      int count = this.conduitConnections.Count;
      this.conduits.Add(new SolidConduitFlow.Conduit(count));
      this.conduitConnections.Add(new SolidConduitFlow.ConduitConnections()
      {
        left = -1,
        right = -1,
        up = -1,
        down = -1
      });
      this.initialContents.Add(manager.grid[cell].contents);
      this.lastFlowInfo.Add(new SolidConduitFlow.ConduitFlowInfo()
      {
        direction = SolidConduitFlow.FlowDirection.None
      });
      this.cells.Add(cell);
      this.updated.Add(false);
      this.diseaseContentsVisible.Add(false);
      this.conduitGOs.Add(conduit_go);
      this.srcFlowIdx.Add(-1);
      this.permittedFlowDirections.Add(0);
      this.srcFlowDirections.Add(SolidConduitFlow.FlowDirection.None);
      this.targetFlowDirections.Add(SolidConduitFlow.FlowDirection.None);
      return count;
    }

    public void Clear(SolidConduitFlow manager)
    {
      for (int index = 0; index < this.conduits.Count; ++index)
      {
        this.ForcePermanentDiseaseContainer(index, false);
        int cell = this.cells[index];
        SolidConduitFlow.ConduitContents contents = manager.grid[cell].contents;
        manager.grid[cell].contents = contents;
        manager.grid[cell].conduitIdx = -1;
      }
      this.cells.Clear();
      this.updated.Clear();
      this.diseaseContentsVisible.Clear();
      this.srcFlowIdx.Clear();
      this.permittedFlowDirections.Clear();
      this.srcFlowDirections.Clear();
      this.targetFlowDirections.Clear();
      this.conduitGOs.Clear();
      this.initialContents.Clear();
      this.lastFlowInfo.Clear();
      this.conduitConnections.Clear();
      this.conduits.Clear();
    }

    public SolidConduitFlow.Conduit GetConduit(int idx) => this.conduits[idx];

    public GameObject GetConduitGO(int idx) => this.conduitGOs[idx];

    public SolidConduitFlow.ConduitConnections GetConduitConnections(int idx) => this.conduitConnections[idx];

    public void SetConduitConnections(int idx, SolidConduitFlow.ConduitConnections data) => this.conduitConnections[idx] = data;

    public void ForcePermanentDiseaseContainer(int idx, bool force_on)
    {
      if (this.diseaseContentsVisible[idx] == force_on)
        return;
      this.diseaseContentsVisible[idx] = force_on;
      GameObject conduitGo = this.conduitGOs[idx];
      if (Object.op_Equality((Object) conduitGo, (Object) null))
        return;
      conduitGo.GetComponent<PrimaryElement>().ForcePermanentDiseaseContainer(force_on);
    }

    public SolidConduitFlow.Conduit GetConduitFromDirection(
      int idx,
      SolidConduitFlow.FlowDirection direction)
    {
      SolidConduitFlow.Conduit conduitFromDirection = SolidConduitFlow.Conduit.Invalid();
      SolidConduitFlow.ConduitConnections conduitConnection = this.conduitConnections[idx];
      switch (direction)
      {
        case SolidConduitFlow.FlowDirection.Left:
          conduitFromDirection = conduitConnection.left != -1 ? this.conduits[conduitConnection.left] : SolidConduitFlow.Conduit.Invalid();
          break;
        case SolidConduitFlow.FlowDirection.Right:
          conduitFromDirection = conduitConnection.right != -1 ? this.conduits[conduitConnection.right] : SolidConduitFlow.Conduit.Invalid();
          break;
        case SolidConduitFlow.FlowDirection.Up:
          conduitFromDirection = conduitConnection.up != -1 ? this.conduits[conduitConnection.up] : SolidConduitFlow.Conduit.Invalid();
          break;
        case SolidConduitFlow.FlowDirection.Down:
          conduitFromDirection = conduitConnection.down != -1 ? this.conduits[conduitConnection.down] : SolidConduitFlow.Conduit.Invalid();
          break;
      }
      return conduitFromDirection;
    }

    public void BeginFrame(SolidConduitFlow manager)
    {
      for (int index = 0; index < this.conduits.Count; ++index)
      {
        this.updated[index] = false;
        SolidConduitFlow.ConduitContents contents = this.conduits[index].GetContents(manager);
        this.initialContents[index] = contents;
        this.lastFlowInfo[index] = new SolidConduitFlow.ConduitFlowInfo()
        {
          direction = SolidConduitFlow.FlowDirection.None
        };
        int cell = this.cells[index];
        manager.grid[cell].contents = contents;
      }
      for (int index = 0; index < manager.freedHandles.Count; ++index)
      {
        HandleVector<int>.Handle freedHandle = manager.freedHandles[index];
        manager.conveyorPickupables.Free(freedHandle);
      }
      manager.freedHandles.Clear();
    }

    public void EndFrame(SolidConduitFlow manager)
    {
    }

    public void UpdateFlowDirection(SolidConduitFlow manager)
    {
      for (int index = 0; index < this.conduits.Count; ++index)
      {
        SolidConduitFlow.Conduit conduit = this.conduits[index];
        if (!this.updated[index])
        {
          int cell = conduit.GetCell(manager);
          if (!manager.grid[cell].contents.pickupableHandle.IsValid())
            this.srcFlowDirections[conduit.idx] = conduit.GetNextFlowSource(manager);
        }
      }
    }

    public void MarkConduitEmpty(int idx, SolidConduitFlow manager)
    {
      if (this.lastFlowInfo[idx].direction == SolidConduitFlow.FlowDirection.None)
        return;
      this.lastFlowInfo[idx] = new SolidConduitFlow.ConduitFlowInfo()
      {
        direction = SolidConduitFlow.FlowDirection.None
      };
      SolidConduitFlow.Conduit conduit = this.conduits[idx];
      this.targetFlowDirections[idx] = conduit.GetNextFlowTarget(manager);
      int cell = this.cells[idx];
      manager.grid[cell].contents = SolidConduitFlow.ConduitContents.EmptyContents();
    }

    public void SetLastFlowInfo(int idx, SolidConduitFlow.FlowDirection direction) => this.lastFlowInfo[idx] = new SolidConduitFlow.ConduitFlowInfo()
    {
      direction = direction
    };

    public SolidConduitFlow.ConduitContents GetInitialContents(int idx) => this.initialContents[idx];

    public SolidConduitFlow.ConduitFlowInfo GetLastFlowInfo(int idx) => this.lastFlowInfo[idx];

    public int GetPermittedFlowDirections(int idx) => this.permittedFlowDirections[idx];

    public void SetPermittedFlowDirections(int idx, int permitted) => this.permittedFlowDirections[idx] = permitted;

    public SolidConduitFlow.FlowDirection GetTargetFlowDirection(int idx) => this.targetFlowDirections[idx];

    public void SetTargetFlowDirection(int idx, SolidConduitFlow.FlowDirection directions) => this.targetFlowDirections[idx] = directions;

    public int GetSrcFlowIdx(int idx) => this.srcFlowIdx[idx];

    public void SetSrcFlowIdx(int idx, int new_src_idx) => this.srcFlowIdx[idx] = new_src_idx;

    public SolidConduitFlow.FlowDirection GetSrcFlowDirection(int idx) => this.srcFlowDirections[idx];

    public void SetSrcFlowDirection(int idx, SolidConduitFlow.FlowDirection directions) => this.srcFlowDirections[idx] = directions;

    public int GetCell(int idx) => this.cells[idx];

    public void SetCell(int idx, int cell) => this.cells[idx] = cell;

    public bool GetUpdated(int idx) => this.updated[idx];

    public void SetUpdated(int idx, bool is_updated) => this.updated[idx] = is_updated;
  }

  [DebuggerDisplay("{priority} {callback.Target.name} {callback.Target} {callback.Method}")]
  public struct ConduitUpdater
  {
    public ConduitFlowPriority priority;
    public Action<float> callback;
  }

  public struct GridNode
  {
    public int conduitIdx;
    public SolidConduitFlow.ConduitContents contents;
  }

  public enum FlowDirection
  {
    Blocked = -1, // 0xFFFFFFFF
    None = 0,
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4,
    Num = 5,
  }

  public struct ConduitConnections
  {
    public int left;
    public int right;
    public int up;
    public int down;
  }

  public struct ConduitFlowInfo
  {
    public SolidConduitFlow.FlowDirection direction;
  }

  [Serializable]
  public struct Conduit : IEquatable<SolidConduitFlow.Conduit>
  {
    public int idx;

    public static SolidConduitFlow.Conduit Invalid() => new SolidConduitFlow.Conduit(-1);

    public Conduit(int idx) => this.idx = idx;

    public int GetPermittedFlowDirections(SolidConduitFlow manager) => manager.soaInfo.GetPermittedFlowDirections(this.idx);

    public void SetPermittedFlowDirections(int permitted, SolidConduitFlow manager) => manager.soaInfo.SetPermittedFlowDirections(this.idx, permitted);

    public SolidConduitFlow.FlowDirection GetTargetFlowDirection(SolidConduitFlow manager) => manager.soaInfo.GetTargetFlowDirection(this.idx);

    public void SetTargetFlowDirection(
      SolidConduitFlow.FlowDirection directions,
      SolidConduitFlow manager)
    {
      manager.soaInfo.SetTargetFlowDirection(this.idx, directions);
    }

    public SolidConduitFlow.ConduitContents GetContents(SolidConduitFlow manager)
    {
      int cell = manager.soaInfo.GetCell(this.idx);
      return manager.grid[cell].contents;
    }

    public void SetContents(SolidConduitFlow manager, SolidConduitFlow.ConduitContents contents)
    {
      int cell = manager.soaInfo.GetCell(this.idx);
      manager.grid[cell].contents = contents;
      if (!contents.pickupableHandle.IsValid())
        return;
      Pickupable pickupable = manager.GetPickupable(contents.pickupableHandle);
      if (!Object.op_Inequality((Object) pickupable, (Object) null))
        return;
      pickupable.transform.parent = (Transform) null;
      Vector3 posCcc = Grid.CellToPosCCC(cell, Grid.SceneLayer.SolidConduitContents);
      TransformExtensions.SetPosition(pickupable.transform, posCcc);
      ((Component) pickupable).GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.SolidConduitContents);
    }

    public SolidConduitFlow.FlowDirection GetNextFlowSource(SolidConduitFlow manager)
    {
      if (manager.soaInfo.GetPermittedFlowDirections(this.idx) == -1)
        return SolidConduitFlow.FlowDirection.Blocked;
      SolidConduitFlow.FlowDirection flowDirection = manager.soaInfo.GetSrcFlowDirection(this.idx);
      if (flowDirection == SolidConduitFlow.FlowDirection.None)
        flowDirection = SolidConduitFlow.FlowDirection.Down;
      for (int index = 0; index < 5; ++index)
      {
        SolidConduitFlow.FlowDirection direction1 = (SolidConduitFlow.FlowDirection) ((int) (flowDirection + index - 1 + 1) % 5 + 1);
        SolidConduitFlow.Conduit conduitFromDirection = manager.soaInfo.GetConduitFromDirection(this.idx, direction1);
        if (conduitFromDirection.idx != -1 && manager.grid[conduitFromDirection.GetCell(manager)].contents.pickupableHandle.IsValid())
        {
          int permittedFlowDirections = manager.soaInfo.GetPermittedFlowDirections(conduitFromDirection.idx);
          if (permittedFlowDirections != -1)
          {
            SolidConduitFlow.FlowDirection direction2 = SolidConduitFlow.InverseFlow(direction1);
            if (manager.soaInfo.GetConduitFromDirection(conduitFromDirection.idx, direction2).idx != -1 && (permittedFlowDirections & SolidConduitFlow.FlowBit(direction2)) != 0)
              return direction1;
          }
        }
      }
      for (int index = 0; index < 5; ++index)
      {
        SolidConduitFlow.FlowDirection direction3 = (SolidConduitFlow.FlowDirection) ((int) (manager.soaInfo.GetTargetFlowDirection(this.idx) + index - 1 + 1) % 5 + 1);
        SolidConduitFlow.FlowDirection direction4 = SolidConduitFlow.InverseFlow(direction3);
        SolidConduitFlow.Conduit conduitFromDirection = manager.soaInfo.GetConduitFromDirection(this.idx, direction3);
        if (conduitFromDirection.idx != -1)
        {
          int permittedFlowDirections = manager.soaInfo.GetPermittedFlowDirections(conduitFromDirection.idx);
          if (permittedFlowDirections != -1 && (permittedFlowDirections & SolidConduitFlow.FlowBit(direction4)) != 0)
            return direction3;
        }
      }
      return SolidConduitFlow.FlowDirection.None;
    }

    public SolidConduitFlow.FlowDirection GetNextFlowTarget(SolidConduitFlow manager)
    {
      int permittedFlowDirections = manager.soaInfo.GetPermittedFlowDirections(this.idx);
      if (permittedFlowDirections == -1)
        return SolidConduitFlow.FlowDirection.Blocked;
      for (int index = 0; index < 5; ++index)
      {
        int direction = (int) (manager.soaInfo.GetTargetFlowDirection(this.idx) + index - 1 + 1) % 5 + 1;
        if (manager.soaInfo.GetConduitFromDirection(this.idx, (SolidConduitFlow.FlowDirection) direction).idx != -1 && (permittedFlowDirections & SolidConduitFlow.FlowBit((SolidConduitFlow.FlowDirection) direction)) != 0)
          return (SolidConduitFlow.FlowDirection) direction;
      }
      return SolidConduitFlow.FlowDirection.Blocked;
    }

    public SolidConduitFlow.ConduitFlowInfo GetLastFlowInfo(SolidConduitFlow manager) => manager.soaInfo.GetLastFlowInfo(this.idx);

    public SolidConduitFlow.ConduitContents GetInitialContents(SolidConduitFlow manager) => manager.soaInfo.GetInitialContents(this.idx);

    public int GetCell(SolidConduitFlow manager) => manager.soaInfo.GetCell(this.idx);

    public bool Equals(SolidConduitFlow.Conduit other) => this.idx == other.idx;
  }

  [DebuggerDisplay("{pickupable}")]
  public struct ConduitContents
  {
    public HandleVector<int>.Handle pickupableHandle;

    public ConduitContents(HandleVector<int>.Handle pickupable_handle) => this.pickupableHandle = pickupable_handle;

    public static SolidConduitFlow.ConduitContents EmptyContents() => new SolidConduitFlow.ConduitContents()
    {
      pickupableHandle = HandleVector<int>.InvalidHandle
    };
  }
}
