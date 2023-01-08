// Decompiled with JetBrains decompiler
// Type: ConduitFlow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

[SerializationConfig]
[DebuggerDisplay("{conduitType}")]
public class ConduitFlow : IConduitFlow
{
  public const float MAX_LIQUID_MASS = 10f;
  public const float MAX_GAS_MASS = 1f;
  public ConduitType conduitType;
  private float MaxMass = 10f;
  private const float PERCENT_MAX_MASS_FOR_STATE_CHANGE_DAMAGE = 0.1f;
  public const float TickRate = 1f;
  public const float WaitTime = 1f;
  private float elapsedTime;
  private float lastUpdateTime = float.NegativeInfinity;
  public ConduitFlow.SOAInfo soaInfo = new ConduitFlow.SOAInfo();
  private bool dirtyConduitUpdaters;
  private List<ConduitFlow.ConduitUpdater> conduitUpdaters = new List<ConduitFlow.ConduitUpdater>();
  private ConduitFlow.GridNode[] grid;
  [Serialize]
  public int[] serializedIdx;
  [Serialize]
  public ConduitFlow.ConduitContents[] serializedContents;
  [Serialize]
  public ConduitFlow.SerializedContents[] versionedSerializedContents;
  private IUtilityNetworkMgr networkMgr;
  private HashSet<int> replacements = new HashSet<int>();
  private const int FLOW_DIRECTION_COUNT = 4;
  private List<ConduitFlow.Network> networks = new List<ConduitFlow.Network>();
  private WorkItemCollection<ConduitFlow.BuildNetworkTask, ConduitFlow> build_network_job = new WorkItemCollection<ConduitFlow.BuildNetworkTask, ConduitFlow>();
  private WorkItemCollection<ConduitFlow.ConnectTask, ConduitFlow.ConnectContext> connect_job = new WorkItemCollection<ConduitFlow.ConnectTask, ConduitFlow.ConnectContext>();
  private WorkItemCollection<ConduitFlow.UpdateNetworkTask, ConduitFlow> update_networks_job = new WorkItemCollection<ConduitFlow.UpdateNetworkTask, ConduitFlow>();

  public event System.Action onConduitsRebuilt;

  public void AddConduitUpdater(Action<float> callback, ConduitFlowPriority priority = ConduitFlowPriority.Default)
  {
    this.conduitUpdaters.Add(new ConduitFlow.ConduitUpdater()
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

  private static ConduitFlow.FlowDirections ComputeFlowDirection(int index) => (ConduitFlow.FlowDirections) (1 << index);

  private static int ComputeIndex(ConduitFlow.FlowDirections flow)
  {
    switch (flow)
    {
      case ConduitFlow.FlowDirections.Down:
        return 0;
      case ConduitFlow.FlowDirections.Left:
        return 1;
      case ConduitFlow.FlowDirections.Right:
        return 2;
      case ConduitFlow.FlowDirections.Up:
        return 3;
      default:
        Debug.Assert(false, (object) "multiple bits are set in 'flow'...can't compute refuted index");
        return -1;
    }
  }

  private static ConduitFlow.FlowDirections ComputeNextFlowDirection(
    ConduitFlow.FlowDirections current)
  {
    return current != ConduitFlow.FlowDirections.None ? ConduitFlow.ComputeFlowDirection((ConduitFlow.ComputeIndex(current) + 1) % 4) : ConduitFlow.FlowDirections.Down;
  }

  public static ConduitFlow.FlowDirections Invert(ConduitFlow.FlowDirections directions) => ConduitFlow.FlowDirections.All & ~directions;

  public static ConduitFlow.FlowDirections Opposite(ConduitFlow.FlowDirections directions)
  {
    ConduitFlow.FlowDirections flowDirections = ConduitFlow.FlowDirections.None;
    if ((directions & ConduitFlow.FlowDirections.Left) != ConduitFlow.FlowDirections.None)
      flowDirections = ConduitFlow.FlowDirections.Right;
    else if ((directions & ConduitFlow.FlowDirections.Right) != ConduitFlow.FlowDirections.None)
      flowDirections = ConduitFlow.FlowDirections.Left;
    else if ((directions & ConduitFlow.FlowDirections.Up) != ConduitFlow.FlowDirections.None)
      flowDirections = ConduitFlow.FlowDirections.Down;
    else if ((directions & ConduitFlow.FlowDirections.Down) != ConduitFlow.FlowDirections.None)
      flowDirections = ConduitFlow.FlowDirections.Up;
    return flowDirections;
  }

  public ConduitFlow(
    ConduitType conduit_type,
    int num_cells,
    IUtilityNetworkMgr network_mgr,
    float max_conduit_mass,
    float initial_elapsed_time)
  {
    this.elapsedTime = initial_elapsed_time;
    this.conduitType = conduit_type;
    this.networkMgr = network_mgr;
    this.MaxMass = max_conduit_mass;
    this.Initialize(num_cells);
    network_mgr.AddNetworksRebuiltListener(new Action<IList<UtilityNetwork>, ICollection<int>>(this.OnUtilityNetworksRebuilt));
  }

  public void Initialize(int num_cells)
  {
    this.grid = new ConduitFlow.GridNode[num_cells];
    for (int index = 0; index < num_cells; ++index)
    {
      this.grid[index].conduitIdx = -1;
      this.grid[index].contents.element = SimHashes.Vacuum;
      this.grid[index].contents.diseaseIdx = byte.MaxValue;
    }
  }

  private void OnUtilityNetworksRebuilt(IList<UtilityNetwork> networks, ICollection<int> root_nodes)
  {
    this.RebuildConnections((IEnumerable<int>) root_nodes);
    int count = this.networks.Count - networks.Count;
    if (0 < this.networks.Count - networks.Count)
      this.networks.RemoveRange(networks.Count, count);
    Debug.Assert(this.networks.Count <= networks.Count);
    ConduitFlow.Network network1;
    for (int index1 = 0; index1 != networks.Count; ++index1)
    {
      if (index1 < this.networks.Count)
      {
        List<ConduitFlow.Network> networks1 = this.networks;
        int index2 = index1;
        network1 = new ConduitFlow.Network();
        network1.network = (FlowUtilityNetwork) networks[index1];
        network1.cells = this.networks[index1].cells;
        ConduitFlow.Network network2 = network1;
        networks1[index2] = network2;
        this.networks[index1].cells.Clear();
      }
      else
      {
        List<ConduitFlow.Network> networks2 = this.networks;
        network1 = new ConduitFlow.Network();
        network1.network = (FlowUtilityNetwork) networks[index1];
        network1.cells = new List<int>();
        ConduitFlow.Network network3 = network1;
        networks2.Add(network3);
      }
    }
    this.build_network_job.Reset(this);
    foreach (ConduitFlow.Network network4 in this.networks)
      this.build_network_job.Add(new ConduitFlow.BuildNetworkTask(network4, this.soaInfo.NumEntries));
    GlobalJobManager.Run((IWorkItemCollection) this.build_network_job);
    for (int index = 0; index != this.build_network_job.Count; ++index)
      this.build_network_job.GetWorkItem(index).Finish();
  }

  private void RebuildConnections(IEnumerable<int> root_nodes)
  {
    ConduitFlow.ConnectContext connectContext = new ConduitFlow.ConnectContext(this);
    this.soaInfo.Clear(this);
    this.replacements.ExceptWith(root_nodes);
    ObjectLayer layer = this.conduitType == ConduitType.Gas ? ObjectLayer.GasConduit : ObjectLayer.LiquidConduit;
    foreach (int rootNode in root_nodes)
    {
      GameObject conduit_go = Grid.Objects[rootNode, (int) layer];
      if (!Object.op_Equality((Object) conduit_go, (Object) null))
      {
        global::Conduit component = conduit_go.GetComponent<global::Conduit>();
        if (!Object.op_Inequality((Object) component, (Object) null) || !component.IsDisconnected())
        {
          int num = this.soaInfo.AddConduit(this, conduit_go, rootNode);
          this.grid[rootNode].conduitIdx = num;
          ((List<int>) connectContext.cells).Add(rootNode);
        }
      }
    }
    Game.Instance.conduitTemperatureManager.Sim200ms(0.0f);
    this.connect_job.Reset(connectContext);
    int num1 = 256;
    for (int start = 0; start < ((List<int>) connectContext.cells).Count; start += num1)
      this.connect_job.Add(new ConduitFlow.ConnectTask(start, Mathf.Min(start + num1, ((List<int>) connectContext.cells).Count)));
    GlobalJobManager.Run((IWorkItemCollection) this.connect_job);
    connectContext.Finish();
    if (this.onConduitsRebuilt == null)
      return;
    this.onConduitsRebuilt();
  }

  private ConduitFlow.FlowDirections GetDirection(
    ConduitFlow.Conduit conduit,
    ConduitFlow.Conduit target_conduit)
  {
    Debug.Assert(conduit.idx != -1);
    Debug.Assert(target_conduit.idx != -1);
    ConduitFlow.ConduitConnections conduitConnections = this.soaInfo.GetConduitConnections(conduit.idx);
    if (conduitConnections.up == target_conduit.idx)
      return ConduitFlow.FlowDirections.Up;
    if (conduitConnections.down == target_conduit.idx)
      return ConduitFlow.FlowDirections.Down;
    if (conduitConnections.left == target_conduit.idx)
      return ConduitFlow.FlowDirections.Left;
    return conduitConnections.right == target_conduit.idx ? ConduitFlow.FlowDirections.Right : ConduitFlow.FlowDirections.None;
  }

  public int ComputeUpdateOrder(int cell)
  {
    foreach (ConduitFlow.Network network in this.networks)
    {
      int updateOrder = network.cells.IndexOf(cell);
      if (updateOrder != -1)
        return updateOrder;
    }
    return -1;
  }

  public ConduitFlow.ConduitContents GetContents(int cell)
  {
    ConduitFlow.ConduitContents contents = this.grid[cell].contents;
    ConduitFlow.GridNode gridNode = this.grid[cell];
    if (gridNode.conduitIdx != -1)
      contents = this.soaInfo.GetConduit(gridNode.conduitIdx).GetContents(this);
    if ((double) contents.mass > 0.0 && (double) contents.temperature <= 0.0)
      Debug.LogError((object) string.Format("unexpected temperature {0}", (object) contents.temperature));
    return contents;
  }

  public void SetContents(int cell, ConduitFlow.ConduitContents contents)
  {
    ConduitFlow.GridNode gridNode = this.grid[cell];
    if (gridNode.conduitIdx != -1)
      this.soaInfo.GetConduit(gridNode.conduitIdx).SetContents(this, contents);
    else
      this.grid[cell].contents = contents;
  }

  public static int GetCellFromDirection(int cell, ConduitFlow.FlowDirections direction)
  {
    switch (direction)
    {
      case ConduitFlow.FlowDirections.Down:
        return Grid.CellBelow(cell);
      case ConduitFlow.FlowDirections.Left:
        return Grid.CellLeft(cell);
      case ConduitFlow.FlowDirections.Right:
        return Grid.CellRight(cell);
      case ConduitFlow.FlowDirections.Up:
        return Grid.CellAbove(cell);
      default:
        return -1;
    }
  }

  public void Sim200ms(float dt)
  {
    if ((double) dt <= 0.0)
      return;
    this.elapsedTime += dt;
    if ((double) this.elapsedTime < 1.0)
      return;
    --this.elapsedTime;
    float num = 1f;
    this.lastUpdateTime = Time.time;
    this.soaInfo.BeginFrame(this);
    ListPool<ConduitFlow.UpdateNetworkTask, ConduitFlow>.PooledList pooledList = ListPool<ConduitFlow.UpdateNetworkTask, ConduitFlow>.Allocate();
    ((List<ConduitFlow.UpdateNetworkTask>) pooledList).Capacity = Mathf.Max(((List<ConduitFlow.UpdateNetworkTask>) pooledList).Capacity, this.networks.Count);
    foreach (ConduitFlow.Network network in this.networks)
      ((List<ConduitFlow.UpdateNetworkTask>) pooledList).Add(new ConduitFlow.UpdateNetworkTask(network));
    for (int index1 = 0; index1 != 4 && ((List<ConduitFlow.UpdateNetworkTask>) pooledList).Count != 0; ++index1)
    {
      this.update_networks_job.Reset(this);
      foreach (ConduitFlow.UpdateNetworkTask updateNetworkTask in (List<ConduitFlow.UpdateNetworkTask>) pooledList)
        this.update_networks_job.Add(updateNetworkTask);
      GlobalJobManager.Run((IWorkItemCollection) this.update_networks_job);
      ((List<ConduitFlow.UpdateNetworkTask>) pooledList).Clear();
      for (int index2 = 0; index2 != this.update_networks_job.Count; ++index2)
      {
        ConduitFlow.UpdateNetworkTask workItem = this.update_networks_job.GetWorkItem(index2);
        if (workItem.continue_updating && index1 != 3)
          ((List<ConduitFlow.UpdateNetworkTask>) pooledList).Add(workItem);
        else
          workItem.Finish(this);
      }
    }
    pooledList.Recycle();
    if (this.dirtyConduitUpdaters)
      this.conduitUpdaters.Sort((Comparison<ConduitFlow.ConduitUpdater>) ((a, b) => a.priority - b.priority));
    this.soaInfo.EndFrame(this);
    for (int index = 0; index < this.conduitUpdaters.Count; ++index)
      this.conduitUpdaters[index].callback(num);
  }

  private float ComputeMovableMass(
    ConduitFlow.GridNode grid_node,
    Dictionary<int, ConduitFlow.Sink> sinks)
  {
    ConduitFlow.ConduitContents contents = grid_node.contents;
    if (contents.element == SimHashes.Vacuum)
      return 0.0f;
    ConduitFlow.Sink sink;
    return !sinks.TryGetValue(grid_node.conduitIdx, out sink) || !Object.op_Inequality((Object) sink.consumer, (Object) null) ? contents.movable_mass : Mathf.Max(0.0f, contents.movable_mass - sink.space_remaining);
  }

  private bool UpdateConduit(ConduitFlow.Conduit conduit, Dictionary<int, ConduitFlow.Sink> sinks)
  {
    bool flag1 = false;
    int cell1 = this.soaInfo.GetCell(conduit.idx);
    ConduitFlow.GridNode grid_node = this.grid[cell1];
    float movableMass = this.ComputeMovableMass(grid_node, sinks);
    ConduitFlow.FlowDirections permittedFlowDirections = this.soaInfo.GetPermittedFlowDirections(conduit.idx);
    ConduitFlow.FlowDirections flowDirections1 = this.soaInfo.GetTargetFlowDirection(conduit.idx);
    if ((double) movableMass <= 0.0)
    {
      for (int index = 0; index != 4; ++index)
      {
        flowDirections1 = ConduitFlow.ComputeNextFlowDirection(flowDirections1);
        if ((permittedFlowDirections & flowDirections1) != ConduitFlow.FlowDirections.None)
        {
          ConduitFlow.Conduit conduitFromDirection = this.soaInfo.GetConduitFromDirection(conduit.idx, flowDirections1);
          Debug.Assert(conduitFromDirection.idx != -1);
          if ((this.soaInfo.GetSrcFlowDirection(conduitFromDirection.idx) & ConduitFlow.Opposite(flowDirections1)) != 0)
            this.soaInfo.SetPullDirection(conduitFromDirection.idx, flowDirections1);
        }
      }
    }
    else
    {
      for (int index = 0; index != 4; ++index)
      {
        flowDirections1 = ConduitFlow.ComputeNextFlowDirection(flowDirections1);
        if ((permittedFlowDirections & flowDirections1) != ConduitFlow.FlowDirections.None)
        {
          ConduitFlow.Conduit conduitFromDirection = this.soaInfo.GetConduitFromDirection(conduit.idx, flowDirections1);
          Debug.Assert(conduitFromDirection.idx != -1);
          int srcFlowDirection = (int) this.soaInfo.GetSrcFlowDirection(conduitFromDirection.idx);
          bool flag2 = ((ConduitFlow.FlowDirections) srcFlowDirection & ConduitFlow.Opposite(flowDirections1)) != 0;
          if (srcFlowDirection != 0 && !flag2)
          {
            flag1 = true;
          }
          else
          {
            int cell2 = this.soaInfo.GetCell(conduitFromDirection.idx);
            Debug.Assert(cell2 != -1);
            ConduitFlow.ConduitContents contents1 = this.grid[cell2].contents;
            int num1 = contents1.element == SimHashes.Vacuum ? 1 : (contents1.element == grid_node.contents.element ? 1 : 0);
            float effectiveCapacity = contents1.GetEffectiveCapacity(this.MaxMass);
            bool flag3 = num1 != 0 && (double) effectiveCapacity > 0.0;
            float num2 = Mathf.Min(movableMass, effectiveCapacity);
            if (flag2 & flag3)
              this.soaInfo.SetPullDirection(conduitFromDirection.idx, flowDirections1);
            if ((double) num2 > 0.0 && flag3)
            {
              this.soaInfo.SetTargetFlowDirection(conduit.idx, flowDirections1);
              Debug.Assert((double) grid_node.contents.temperature > 0.0);
              contents1.temperature = GameUtil.GetFinalTemperature(grid_node.contents.temperature, num2, contents1.temperature, contents1.mass);
              contents1.AddMass(num2);
              contents1.element = grid_node.contents.element;
              int src1_count = (int) ((double) num2 / (double) grid_node.contents.mass * (double) grid_node.contents.diseaseCount);
              if (src1_count != 0)
              {
                SimUtil.DiseaseInfo finalDiseaseInfo = SimUtil.CalculateFinalDiseaseInfo(grid_node.contents.diseaseIdx, src1_count, contents1.diseaseIdx, contents1.diseaseCount);
                contents1.diseaseIdx = finalDiseaseInfo.idx;
                contents1.diseaseCount = finalDiseaseInfo.count;
              }
              this.grid[cell2].contents = contents1;
              Debug.Assert((double) num2 <= (double) grid_node.contents.mass);
              float amount = grid_node.contents.mass - num2;
              float num3 = movableMass - num2;
              if ((double) amount <= 0.0)
              {
                Debug.Assert((double) num3 <= 0.0);
                this.soaInfo.SetLastFlowInfo(conduit.idx, flowDirections1, ref grid_node.contents);
                grid_node.contents = ConduitFlow.ConduitContents.Empty;
              }
              else
              {
                int num4 = (int) ((double) amount / (double) grid_node.contents.mass * (double) grid_node.contents.diseaseCount);
                Debug.Assert(num4 >= 0);
                ConduitFlow.ConduitContents contents2 = grid_node.contents;
                double num5 = (double) contents2.RemoveMass(amount);
                contents2.diseaseCount -= num4;
                double num6 = (double) grid_node.contents.RemoveMass(num2);
                grid_node.contents.diseaseCount = num4;
                if (num4 == 0)
                  grid_node.contents.diseaseIdx = byte.MaxValue;
                this.soaInfo.SetLastFlowInfo(conduit.idx, flowDirections1, ref contents2);
              }
              this.grid[cell1].contents = grid_node.contents;
              flag1 = 0.0 < (double) this.ComputeMovableMass(grid_node, sinks);
              break;
            }
          }
        }
      }
    }
    ConduitFlow.FlowDirections srcFlowDirection1 = this.soaInfo.GetSrcFlowDirection(conduit.idx);
    ConduitFlow.FlowDirections pullDirection = this.soaInfo.GetPullDirection(conduit.idx);
    if (srcFlowDirection1 == ConduitFlow.FlowDirections.None || (ConduitFlow.Opposite(srcFlowDirection1) & pullDirection) != ConduitFlow.FlowDirections.None)
    {
      this.soaInfo.SetPullDirection(conduit.idx, ConduitFlow.FlowDirections.None);
      this.soaInfo.SetSrcFlowDirection(conduit.idx, ConduitFlow.FlowDirections.None);
      for (int index1 = 0; index1 != 2; ++index1)
      {
        ConduitFlow.FlowDirections flowDirections2 = srcFlowDirection1;
        for (int index2 = 0; index2 != 4; ++index2)
        {
          flowDirections2 = ConduitFlow.ComputeNextFlowDirection(flowDirections2);
          ConduitFlow.Conduit conduitFromDirection = this.soaInfo.GetConduitFromDirection(conduit.idx, flowDirections2);
          if (conduitFromDirection.idx != -1 && (this.soaInfo.GetPermittedFlowDirections(conduitFromDirection.idx) & ConduitFlow.Opposite(flowDirections2)) != ConduitFlow.FlowDirections.None)
          {
            ConduitFlow.ConduitContents contents = this.grid[this.soaInfo.GetCell(conduitFromDirection.idx)].contents;
            if (0.0 < (index1 == 0 ? (double) contents.movable_mass : (double) contents.mass))
            {
              this.soaInfo.SetSrcFlowDirection(conduit.idx, flowDirections2);
              break;
            }
          }
        }
        if (this.soaInfo.GetSrcFlowDirection(conduit.idx) != ConduitFlow.FlowDirections.None)
          break;
      }
    }
    return flag1;
  }

  public float ContinuousLerpPercent => Mathf.Clamp01((float) (((double) Time.time - (double) this.lastUpdateTime) / 1.0));

  public float DiscreteLerpPercent => Mathf.Clamp01(this.elapsedTime / 1f);

  public float AddElement(
    int cell_idx,
    SimHashes element,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count)
  {
    if (this.grid[cell_idx].conduitIdx == -1)
      return 0.0f;
    ConduitFlow.ConduitContents contents = this.GetConduit(cell_idx).GetContents(this);
    if (contents.element != element && contents.element != SimHashes.Vacuum && (double) mass > 0.0)
      return 0.0f;
    float num1 = Mathf.Min(mass, this.MaxMass - contents.mass);
    float num2 = num1 / mass;
    if ((double) num1 <= 0.0)
      return 0.0f;
    contents.temperature = GameUtil.GetFinalTemperature(temperature, num1, contents.temperature, contents.mass);
    contents.AddMass(num1);
    contents.element = element;
    contents.ConsolidateMass();
    int src1_count = (int) ((double) num2 * (double) disease_count);
    if (src1_count > 0)
    {
      SimUtil.DiseaseInfo finalDiseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, src1_count, contents.diseaseIdx, contents.diseaseCount);
      contents.diseaseIdx = finalDiseaseInfo.idx;
      contents.diseaseCount = finalDiseaseInfo.count;
    }
    this.SetContents(cell_idx, contents);
    return num1;
  }

  public ConduitFlow.ConduitContents RemoveElement(int cell, float delta)
  {
    ConduitFlow.Conduit conduit = this.GetConduit(cell);
    return conduit.idx == -1 ? ConduitFlow.ConduitContents.Empty : this.RemoveElement(conduit, delta);
  }

  public ConduitFlow.ConduitContents RemoveElement(ConduitFlow.Conduit conduit, float delta)
  {
    ConduitFlow.ConduitContents contents1 = conduit.GetContents(this);
    float amount1 = Mathf.Min(contents1.mass, delta);
    float amount2 = contents1.mass - amount1;
    if ((double) amount2 <= 0.0)
    {
      conduit.SetContents(this, ConduitFlow.ConduitContents.Empty);
      return contents1;
    }
    ConduitFlow.ConduitContents conduitContents = contents1;
    double num1 = (double) conduitContents.RemoveMass(amount2);
    int num2 = (int) ((double) amount2 / (double) contents1.mass * (double) contents1.diseaseCount);
    conduitContents.diseaseCount = contents1.diseaseCount - num2;
    ConduitFlow.ConduitContents contents2 = contents1;
    double num3 = (double) contents2.RemoveMass(amount1);
    contents2.diseaseCount = num2;
    if (num2 <= 0)
    {
      contents2.diseaseIdx = byte.MaxValue;
      contents2.diseaseCount = 0;
    }
    conduit.SetContents(this, contents2);
    return conduitContents;
  }

  public ConduitFlow.FlowDirections GetPermittedFlow(int cell)
  {
    ConduitFlow.Conduit conduit = this.GetConduit(cell);
    return conduit.idx == -1 ? ConduitFlow.FlowDirections.None : this.soaInfo.GetPermittedFlowDirections(conduit.idx);
  }

  public bool HasConduit(int cell) => this.grid[cell].conduitIdx != -1;

  public ConduitFlow.Conduit GetConduit(int cell)
  {
    int conduitIdx = this.grid[cell].conduitIdx;
    return conduitIdx == -1 ? ConduitFlow.Conduit.Invalid : this.soaInfo.GetConduit(conduitIdx);
  }

  private void DumpPipeContents(int cell, ConduitFlow.ConduitContents contents)
  {
    if (contents.element == SimHashes.Vacuum || (double) contents.mass <= 0.0)
      return;
    SimMessages.AddRemoveSubstance(cell, contents.element, CellEventLogger.Instance.ConduitFlowEmptyConduit, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount);
    this.SetContents(cell, ConduitFlow.ConduitContents.Empty);
  }

  public void EmptyConduit(int cell)
  {
    if (this.replacements.Contains(cell))
      return;
    this.DumpPipeContents(cell, this.grid[cell].contents);
  }

  public void MarkForReplacement(int cell) => this.replacements.Add(cell);

  public void DeactivateCell(int cell)
  {
    this.grid[cell].conduitIdx = -1;
    this.SetContents(cell, ConduitFlow.ConduitContents.Empty);
  }

  [Conditional("CHECK_NAN")]
  private void Validate(ConduitFlow.ConduitContents contents)
  {
    if ((double) contents.mass <= 0.0 || (double) contents.temperature > 0.0)
      return;
    Debug.LogError((object) "zero degree pipe contents");
  }

  [System.Runtime.Serialization.OnSerializing]
  private void OnSerializing()
  {
    int numEntries = this.soaInfo.NumEntries;
    if (numEntries > 0)
    {
      this.versionedSerializedContents = new ConduitFlow.SerializedContents[numEntries];
      this.serializedIdx = new int[numEntries];
      for (int idx = 0; idx < numEntries; ++idx)
      {
        ConduitFlow.Conduit conduit = this.soaInfo.GetConduit(idx);
        ConduitFlow.ConduitContents contents = conduit.GetContents(this);
        this.serializedIdx[idx] = this.soaInfo.GetCell(conduit.idx);
        this.versionedSerializedContents[idx] = new ConduitFlow.SerializedContents(contents);
      }
    }
    else
    {
      this.serializedContents = (ConduitFlow.ConduitContents[]) null;
      this.versionedSerializedContents = (ConduitFlow.SerializedContents[]) null;
      this.serializedIdx = (int[]) null;
    }
  }

  [System.Runtime.Serialization.OnSerialized]
  private void OnSerialized()
  {
    this.versionedSerializedContents = (ConduitFlow.SerializedContents[]) null;
    this.serializedContents = (ConduitFlow.ConduitContents[]) null;
    this.serializedIdx = (int[]) null;
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (this.serializedContents != null)
    {
      this.versionedSerializedContents = new ConduitFlow.SerializedContents[this.serializedContents.Length];
      for (int index = 0; index < this.serializedContents.Length; ++index)
        this.versionedSerializedContents[index] = new ConduitFlow.SerializedContents(this.serializedContents[index]);
      this.serializedContents = (ConduitFlow.ConduitContents[]) null;
    }
    if (this.versionedSerializedContents == null)
      return;
    for (int index = 0; index < this.versionedSerializedContents.Length; ++index)
    {
      int cell = this.serializedIdx[index];
      ConduitFlow.SerializedContents serializedContent = this.versionedSerializedContents[index];
      ConduitFlow.ConduitContents contents = (double) serializedContent.mass <= 0.0 ? ConduitFlow.ConduitContents.Empty : new ConduitFlow.ConduitContents(serializedContent.element, Math.Min(this.MaxMass, serializedContent.mass), serializedContent.temperature, byte.MaxValue, 0);
      if (0 < serializedContent.diseaseCount || serializedContent.diseaseHash != 0)
      {
        contents.diseaseIdx = Db.Get().Diseases.GetIndex(serializedContent.diseaseHash);
        contents.diseaseCount = contents.diseaseIdx == byte.MaxValue ? 0 : serializedContent.diseaseCount;
      }
      if (float.IsNaN(contents.temperature) || (double) contents.temperature <= 0.0 && contents.element != SimHashes.Vacuum || 10000.0 < (double) contents.temperature)
      {
        Vector2I xy = Grid.CellToXY(cell);
        DeserializeWarnings.Instance.PipeContentsTemperatureIsNan.Warn(string.Format("Invalid pipe content temperature of {0} detected. Resetting temperature. (x={1}, y={2}, cell={3})", (object) contents.temperature, (object) xy.x, (object) xy.y, (object) cell));
        contents.temperature = ElementLoader.FindElementByHash(contents.element).defaultValues.temperature;
      }
      this.SetContents(cell, contents);
    }
    this.versionedSerializedContents = (ConduitFlow.SerializedContents[]) null;
    this.serializedContents = (ConduitFlow.ConduitContents[]) null;
    this.serializedIdx = (int[]) null;
  }

  public UtilityNetwork GetNetwork(ConduitFlow.Conduit conduit) => this.networkMgr.GetNetworkForCell(this.soaInfo.GetCell(conduit.idx));

  public void ForceRebuildNetworks() => this.networkMgr.ForceRebuildNetworks();

  public bool IsConduitFull(int cell_idx) => (double) this.MaxMass - (double) this.grid[cell_idx].contents.mass <= 0.0;

  public bool IsConduitEmpty(int cell_idx) => (double) this.grid[cell_idx].contents.mass <= 0.0;

  public void FreezeConduitContents(int conduit_idx)
  {
    GameObject conduitGo = this.soaInfo.GetConduitGO(conduit_idx);
    if (!Object.op_Inequality((Object) conduitGo, (Object) null) || (double) this.soaInfo.GetConduit(conduit_idx).GetContents(this).mass <= (double) this.MaxMass * 0.10000000149011612)
      return;
    EventExtensions.Trigger(conduitGo, -700727624, (object) null);
  }

  public void MeltConduitContents(int conduit_idx)
  {
    GameObject conduitGo = this.soaInfo.GetConduitGO(conduit_idx);
    if (!Object.op_Inequality((Object) conduitGo, (Object) null) || (double) this.soaInfo.GetConduit(conduit_idx).GetContents(this).mass <= (double) this.MaxMass * 0.10000000149011612)
      return;
    EventExtensions.Trigger(conduitGo, -1152799878, (object) null);
  }

  [DebuggerDisplay("{NumEntries}")]
  public class SOAInfo
  {
    private List<ConduitFlow.Conduit> conduits = new List<ConduitFlow.Conduit>();
    private List<ConduitFlow.ConduitConnections> conduitConnections = new List<ConduitFlow.ConduitConnections>();
    private List<ConduitFlow.ConduitFlowInfo> lastFlowInfo = new List<ConduitFlow.ConduitFlowInfo>();
    private List<ConduitFlow.ConduitContents> initialContents = new List<ConduitFlow.ConduitContents>();
    private List<GameObject> conduitGOs = new List<GameObject>();
    private List<bool> diseaseContentsVisible = new List<bool>();
    private List<int> cells = new List<int>();
    private List<ConduitFlow.FlowDirections> permittedFlowDirections = new List<ConduitFlow.FlowDirections>();
    private List<ConduitFlow.FlowDirections> srcFlowDirections = new List<ConduitFlow.FlowDirections>();
    private List<ConduitFlow.FlowDirections> pullDirections = new List<ConduitFlow.FlowDirections>();
    private List<ConduitFlow.FlowDirections> targetFlowDirections = new List<ConduitFlow.FlowDirections>();
    private List<HandleVector<int>.Handle> structureTemperatureHandles = new List<HandleVector<int>.Handle>();
    private List<HandleVector<int>.Handle> temperatureHandles = new List<HandleVector<int>.Handle>();
    private List<HandleVector<int>.Handle> diseaseHandles = new List<HandleVector<int>.Handle>();
    private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.ClearPermanentDiseaseContainer> clearPermanentDiseaseContainer = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.ClearPermanentDiseaseContainer>();
    private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishTemperatureToSim> publishTemperatureToSim = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishTemperatureToSim>();
    private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishDiseaseToSim> publishDiseaseToSim = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishDiseaseToSim>();
    private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.ResetConduit> resetConduit = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.ResetConduit>();
    private ConduitFlow.SOAInfo.ConduitJob clearJob = new ConduitFlow.SOAInfo.ConduitJob();
    private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.InitializeContentsTask> initializeContents = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.InitializeContentsTask>();
    private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.InvalidateLastFlow> invalidateLastFlow = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.InvalidateLastFlow>();
    private ConduitFlow.SOAInfo.ConduitJob beginFrameJob = new ConduitFlow.SOAInfo.ConduitJob();
    private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishTemperatureToGame> publishTemperatureToGame = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishTemperatureToGame>();
    private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishDiseaseToGame> publishDiseaseToGame = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.PublishDiseaseToGame>();
    private ConduitFlow.SOAInfo.ConduitJob endFrameJob = new ConduitFlow.SOAInfo.ConduitJob();
    private ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.FlowThroughVacuum> flowThroughVacuum = new ConduitFlow.SOAInfo.ConduitTaskDivision<ConduitFlow.SOAInfo.FlowThroughVacuum>();
    private ConduitFlow.SOAInfo.ConduitJob updateFlowDirectionJob = new ConduitFlow.SOAInfo.ConduitJob();

    public int NumEntries => this.conduits.Count;

    public int AddConduit(ConduitFlow manager, GameObject conduit_go, int cell)
    {
      int count = this.conduitConnections.Count;
      this.conduits.Add(new ConduitFlow.Conduit(count));
      this.conduitConnections.Add(new ConduitFlow.ConduitConnections()
      {
        left = -1,
        right = -1,
        up = -1,
        down = -1
      });
      ConduitFlow.ConduitContents contents = manager.grid[cell].contents;
      this.initialContents.Add(contents);
      this.lastFlowInfo.Add(ConduitFlow.ConduitFlowInfo.DEFAULT);
      HandleVector<int>.Handle handle1 = GameComps.StructureTemperatures.GetHandle(conduit_go);
      HandleVector<int>.Handle temperature_handle = Game.Instance.conduitTemperatureManager.Allocate(manager.conduitType, count, handle1, ref contents);
      HandleVector<int>.Handle handle2 = Game.Instance.conduitDiseaseManager.Allocate(temperature_handle, ref contents);
      this.cells.Add(cell);
      this.diseaseContentsVisible.Add(false);
      this.structureTemperatureHandles.Add(handle1);
      this.temperatureHandles.Add(temperature_handle);
      this.diseaseHandles.Add(handle2);
      this.conduitGOs.Add(conduit_go);
      this.permittedFlowDirections.Add(ConduitFlow.FlowDirections.None);
      this.srcFlowDirections.Add(ConduitFlow.FlowDirections.None);
      this.pullDirections.Add(ConduitFlow.FlowDirections.None);
      this.targetFlowDirections.Add(ConduitFlow.FlowDirections.None);
      return count;
    }

    public void Clear(ConduitFlow manager)
    {
      if (this.clearJob.Count == 0)
      {
        this.clearJob.Reset(this);
        this.clearJob.Add<ConduitFlow.SOAInfo.PublishTemperatureToSim>(this.publishTemperatureToSim);
        this.clearJob.Add<ConduitFlow.SOAInfo.PublishDiseaseToSim>(this.publishDiseaseToSim);
        this.clearJob.Add<ConduitFlow.SOAInfo.ResetConduit>(this.resetConduit);
      }
      this.clearPermanentDiseaseContainer.Initialize(this.conduits.Count, manager);
      this.publishTemperatureToSim.Initialize(this.conduits.Count, manager);
      this.publishDiseaseToSim.Initialize(this.conduits.Count, manager);
      this.resetConduit.Initialize(this.conduits.Count, manager);
      this.clearPermanentDiseaseContainer.Run(this);
      GlobalJobManager.Run((IWorkItemCollection) this.clearJob);
      for (int index = 0; index != this.conduits.Count; ++index)
        Game.Instance.conduitDiseaseManager.Free(this.diseaseHandles[index]);
      for (int index = 0; index != this.conduits.Count; ++index)
        Game.Instance.conduitTemperatureManager.Free(this.temperatureHandles[index]);
      this.cells.Clear();
      this.diseaseContentsVisible.Clear();
      this.permittedFlowDirections.Clear();
      this.srcFlowDirections.Clear();
      this.pullDirections.Clear();
      this.targetFlowDirections.Clear();
      this.conduitGOs.Clear();
      this.diseaseHandles.Clear();
      this.temperatureHandles.Clear();
      this.structureTemperatureHandles.Clear();
      this.initialContents.Clear();
      this.lastFlowInfo.Clear();
      this.conduitConnections.Clear();
      this.conduits.Clear();
    }

    public ConduitFlow.Conduit GetConduit(int idx) => this.conduits[idx];

    public ConduitFlow.ConduitConnections GetConduitConnections(int idx) => this.conduitConnections[idx];

    public void SetConduitConnections(int idx, ConduitFlow.ConduitConnections data) => this.conduitConnections[idx] = data;

    public float GetConduitTemperature(int idx)
    {
      double temperature = (double) Game.Instance.conduitTemperatureManager.GetTemperature(this.temperatureHandles[idx]);
      Debug.Assert(!float.IsNaN((float) temperature));
      return (float) temperature;
    }

    public void SetConduitTemperatureData(int idx, ref ConduitFlow.ConduitContents contents) => Game.Instance.conduitTemperatureManager.SetData(this.temperatureHandles[idx], ref contents);

    public ConduitDiseaseManager.Data GetDiseaseData(int idx) => Game.Instance.conduitDiseaseManager.GetData(this.diseaseHandles[idx]);

    public void SetDiseaseData(int idx, ref ConduitFlow.ConduitContents contents) => Game.Instance.conduitDiseaseManager.SetData(this.diseaseHandles[idx], ref contents);

    public GameObject GetConduitGO(int idx) => this.conduitGOs[idx];

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

    public ConduitFlow.Conduit GetConduitFromDirection(
      int idx,
      ConduitFlow.FlowDirections direction)
    {
      ConduitFlow.ConduitConnections conduitConnection = this.conduitConnections[idx];
      switch (direction)
      {
        case ConduitFlow.FlowDirections.Down:
          return conduitConnection.down == -1 ? ConduitFlow.Conduit.Invalid : this.conduits[conduitConnection.down];
        case ConduitFlow.FlowDirections.Left:
          return conduitConnection.left == -1 ? ConduitFlow.Conduit.Invalid : this.conduits[conduitConnection.left];
        case ConduitFlow.FlowDirections.Right:
          return conduitConnection.right == -1 ? ConduitFlow.Conduit.Invalid : this.conduits[conduitConnection.right];
        case ConduitFlow.FlowDirections.Up:
          return conduitConnection.up == -1 ? ConduitFlow.Conduit.Invalid : this.conduits[conduitConnection.up];
        default:
          return ConduitFlow.Conduit.Invalid;
      }
    }

    public void BeginFrame(ConduitFlow manager)
    {
      if (this.beginFrameJob.Count == 0)
      {
        this.beginFrameJob.Reset(this);
        this.beginFrameJob.Add<ConduitFlow.SOAInfo.InitializeContentsTask>(this.initializeContents);
        this.beginFrameJob.Add<ConduitFlow.SOAInfo.InvalidateLastFlow>(this.invalidateLastFlow);
      }
      this.initializeContents.Initialize(this.conduits.Count, manager);
      this.invalidateLastFlow.Initialize(this.conduits.Count, manager);
      GlobalJobManager.Run((IWorkItemCollection) this.beginFrameJob);
    }

    public void EndFrame(ConduitFlow manager)
    {
      if (this.endFrameJob.Count == 0)
      {
        this.endFrameJob.Reset(this);
        this.endFrameJob.Add<ConduitFlow.SOAInfo.PublishDiseaseToGame>(this.publishDiseaseToGame);
      }
      this.publishTemperatureToGame.Initialize(this.conduits.Count, manager);
      this.publishDiseaseToGame.Initialize(this.conduits.Count, manager);
      this.publishTemperatureToGame.Run(this);
      GlobalJobManager.Run((IWorkItemCollection) this.endFrameJob);
    }

    public void UpdateFlowDirection(ConduitFlow manager)
    {
      if (this.updateFlowDirectionJob.Count == 0)
      {
        this.updateFlowDirectionJob.Reset(this);
        this.updateFlowDirectionJob.Add<ConduitFlow.SOAInfo.FlowThroughVacuum>(this.flowThroughVacuum);
      }
      this.flowThroughVacuum.Initialize(this.conduits.Count, manager);
      GlobalJobManager.Run((IWorkItemCollection) this.updateFlowDirectionJob);
    }

    public void ResetLastFlowInfo(int idx) => this.lastFlowInfo[idx] = ConduitFlow.ConduitFlowInfo.DEFAULT;

    public void SetLastFlowInfo(
      int idx,
      ConduitFlow.FlowDirections direction,
      ref ConduitFlow.ConduitContents contents)
    {
      if (this.lastFlowInfo[idx].direction != ConduitFlow.FlowDirections.None)
        return;
      this.lastFlowInfo[idx] = new ConduitFlow.ConduitFlowInfo()
      {
        direction = direction,
        contents = contents
      };
    }

    public ConduitFlow.ConduitContents GetInitialContents(int idx) => this.initialContents[idx];

    public ConduitFlow.ConduitFlowInfo GetLastFlowInfo(int idx) => this.lastFlowInfo[idx];

    public ConduitFlow.FlowDirections GetPermittedFlowDirections(int idx) => this.permittedFlowDirections[idx];

    public void SetPermittedFlowDirections(int idx, ConduitFlow.FlowDirections permitted) => this.permittedFlowDirections[idx] = permitted;

    public ConduitFlow.FlowDirections AddPermittedFlowDirections(
      int idx,
      ConduitFlow.FlowDirections delta)
    {
      return this.permittedFlowDirections[idx] |= delta;
    }

    public ConduitFlow.FlowDirections RemovePermittedFlowDirections(
      int idx,
      ConduitFlow.FlowDirections delta)
    {
      return this.permittedFlowDirections[idx] &= ~delta;
    }

    public ConduitFlow.FlowDirections GetTargetFlowDirection(int idx) => this.targetFlowDirections[idx];

    public void SetTargetFlowDirection(int idx, ConduitFlow.FlowDirections directions) => this.targetFlowDirections[idx] = directions;

    public ConduitFlow.FlowDirections GetSrcFlowDirection(int idx) => this.srcFlowDirections[idx];

    public void SetSrcFlowDirection(int idx, ConduitFlow.FlowDirections directions) => this.srcFlowDirections[idx] = directions;

    public ConduitFlow.FlowDirections GetPullDirection(int idx) => this.pullDirections[idx];

    public void SetPullDirection(int idx, ConduitFlow.FlowDirections directions) => this.pullDirections[idx] = directions;

    public int GetCell(int idx) => this.cells[idx];

    public void SetCell(int idx, int cell) => this.cells[idx] = cell;

    private abstract class ConduitTask : DivisibleTask<ConduitFlow.SOAInfo>
    {
      public ConduitFlow manager;

      public ConduitTask(string name)
        : base(name)
      {
      }
    }

    private class ConduitTaskDivision<Task> : TaskDivision<Task, ConduitFlow.SOAInfo> where Task : ConduitFlow.SOAInfo.ConduitTask, new()
    {
      public void Initialize(int conduitCount, ConduitFlow manager)
      {
        this.Initialize(conduitCount);
        foreach (Task task in this.tasks)
          task.manager = manager;
      }
    }

    private class ConduitJob : 
      WorkItemCollection<ConduitFlow.SOAInfo.ConduitTask, ConduitFlow.SOAInfo>
    {
      public void Add<Task>(
        ConduitFlow.SOAInfo.ConduitTaskDivision<Task> taskDivision)
        where Task : ConduitFlow.SOAInfo.ConduitTask, new()
      {
        foreach (Task task in taskDivision.tasks)
          this.Add((ConduitFlow.SOAInfo.ConduitTask) task);
      }
    }

    private class ClearPermanentDiseaseContainer : ConduitFlow.SOAInfo.ConduitTask
    {
      public ClearPermanentDiseaseContainer()
        : base(nameof (ClearPermanentDiseaseContainer))
      {
      }

      protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
      {
        for (int start = this.start; start != this.end; ++start)
          soaInfo.ForcePermanentDiseaseContainer(start, false);
      }
    }

    private class PublishTemperatureToSim : ConduitFlow.SOAInfo.ConduitTask
    {
      public PublishTemperatureToSim()
        : base(nameof (PublishTemperatureToSim))
      {
      }

      protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
      {
        for (int start = this.start; start != this.end; ++start)
        {
          HandleVector<int>.Handle temperatureHandle = soaInfo.temperatureHandles[start];
          if (temperatureHandle.IsValid())
          {
            float temperature = Game.Instance.conduitTemperatureManager.GetTemperature(temperatureHandle);
            this.manager.grid[soaInfo.cells[start]].contents.temperature = temperature;
          }
        }
      }
    }

    private class PublishDiseaseToSim : ConduitFlow.SOAInfo.ConduitTask
    {
      public PublishDiseaseToSim()
        : base(nameof (PublishDiseaseToSim))
      {
      }

      protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
      {
        for (int start = this.start; start != this.end; ++start)
        {
          HandleVector<int>.Handle diseaseHandle = soaInfo.diseaseHandles[start];
          if (diseaseHandle.IsValid())
          {
            ConduitDiseaseManager.Data data = Game.Instance.conduitDiseaseManager.GetData(diseaseHandle);
            int cell = soaInfo.cells[start];
            this.manager.grid[cell].contents.diseaseIdx = data.diseaseIdx;
            this.manager.grid[cell].contents.diseaseCount = data.diseaseCount;
          }
        }
      }
    }

    private class ResetConduit : ConduitFlow.SOAInfo.ConduitTask
    {
      public ResetConduit()
        : base("ResetConduitTask")
      {
      }

      protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
      {
        for (int start = this.start; start != this.end; ++start)
          this.manager.grid[soaInfo.cells[start]].conduitIdx = -1;
      }
    }

    private class InitializeContentsTask : ConduitFlow.SOAInfo.ConduitTask
    {
      public InitializeContentsTask()
        : base("SetInitialContents")
      {
      }

      protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
      {
        for (int start = this.start; start != this.end; ++start)
        {
          int cell = soaInfo.cells[start];
          ConduitFlow.ConduitContents conduitContents = soaInfo.conduits[start].GetContents(this.manager);
          if ((double) conduitContents.mass <= 0.0)
            conduitContents = ConduitFlow.ConduitContents.Empty;
          soaInfo.initialContents[start] = conduitContents;
          this.manager.grid[cell].contents = conduitContents;
        }
      }
    }

    private class InvalidateLastFlow : ConduitFlow.SOAInfo.ConduitTask
    {
      public InvalidateLastFlow()
        : base(nameof (InvalidateLastFlow))
      {
      }

      protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
      {
        for (int start = this.start; start != this.end; ++start)
          soaInfo.lastFlowInfo[start] = ConduitFlow.ConduitFlowInfo.DEFAULT;
      }
    }

    private class PublishTemperatureToGame : ConduitFlow.SOAInfo.ConduitTask
    {
      public PublishTemperatureToGame()
        : base(nameof (PublishTemperatureToGame))
      {
      }

      protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
      {
        for (int start = this.start; start != this.end; ++start)
          Game.Instance.conduitTemperatureManager.SetData(soaInfo.temperatureHandles[start], ref this.manager.grid[soaInfo.cells[start]].contents);
      }
    }

    private class PublishDiseaseToGame : ConduitFlow.SOAInfo.ConduitTask
    {
      public PublishDiseaseToGame()
        : base(nameof (PublishDiseaseToGame))
      {
      }

      protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
      {
        for (int start = this.start; start != this.end; ++start)
          Game.Instance.conduitDiseaseManager.SetData(soaInfo.diseaseHandles[start], ref this.manager.grid[soaInfo.cells[start]].contents);
      }
    }

    private class FlowThroughVacuum : ConduitFlow.SOAInfo.ConduitTask
    {
      public FlowThroughVacuum()
        : base(nameof (FlowThroughVacuum))
      {
      }

      protected override void RunDivision(ConduitFlow.SOAInfo soaInfo)
      {
        for (int start = this.start; start != this.end; ++start)
        {
          ConduitFlow.Conduit conduit = soaInfo.conduits[start];
          if (this.manager.grid[conduit.GetCell(this.manager)].contents.element == SimHashes.Vacuum)
            soaInfo.srcFlowDirections[conduit.idx] = ConduitFlow.FlowDirections.None;
        }
      }
    }
  }

  [DebuggerDisplay("{priority} {callback.Target.name} {callback.Target} {callback.Method}")]
  public struct ConduitUpdater
  {
    public ConduitFlowPriority priority;
    public Action<float> callback;
  }

  [DebuggerDisplay("conduit {conduitIdx}:{contents.element}")]
  public struct GridNode
  {
    public int conduitIdx;
    public ConduitFlow.ConduitContents contents;
  }

  public struct SerializedContents
  {
    public SimHashes element;
    public float mass;
    public float temperature;
    public int diseaseHash;
    public int diseaseCount;

    public SerializedContents(
      SimHashes element,
      float mass,
      float temperature,
      byte disease_idx,
      int disease_count)
    {
      this.element = element;
      this.mass = mass;
      this.temperature = temperature;
      this.diseaseHash = disease_idx != byte.MaxValue ? Db.Get().Diseases[(int) disease_idx].id.GetHashCode() : 0;
      this.diseaseCount = disease_count;
      if (this.diseaseCount > 0)
        return;
      this.diseaseHash = 0;
    }

    public SerializedContents(ConduitFlow.ConduitContents src)
      : this(src.element, src.mass, src.temperature, src.diseaseIdx, src.diseaseCount)
    {
    }
  }

  [Flags]
  public enum FlowDirections : byte
  {
    None = 0,
    Down = 1,
    Left = 2,
    Right = 4,
    Up = 8,
    All = Up | Right | Left | Down, // 0x0F
  }

  [DebuggerDisplay("conduits l:{left}, r:{right}, u:{up}, d:{down}")]
  public struct ConduitConnections
  {
    public int left;
    public int right;
    public int up;
    public int down;
    public static readonly ConduitFlow.ConduitConnections DEFAULT = new ConduitFlow.ConduitConnections()
    {
      left = -1,
      right = -1,
      up = -1,
      down = -1
    };
  }

  [DebuggerDisplay("{direction}:{contents.element}")]
  public struct ConduitFlowInfo
  {
    public ConduitFlow.FlowDirections direction;
    public ConduitFlow.ConduitContents contents;
    public static readonly ConduitFlow.ConduitFlowInfo DEFAULT = new ConduitFlow.ConduitFlowInfo()
    {
      direction = ConduitFlow.FlowDirections.None,
      contents = ConduitFlow.ConduitContents.Empty
    };
  }

  [DebuggerDisplay("conduit {idx}")]
  [Serializable]
  public struct Conduit : IEquatable<ConduitFlow.Conduit>
  {
    public static readonly ConduitFlow.Conduit Invalid = new ConduitFlow.Conduit(-1);
    public readonly int idx;

    public Conduit(int idx) => this.idx = idx;

    public ConduitFlow.FlowDirections GetPermittedFlowDirections(ConduitFlow manager) => manager.soaInfo.GetPermittedFlowDirections(this.idx);

    public void SetPermittedFlowDirections(
      ConduitFlow.FlowDirections permitted,
      ConduitFlow manager)
    {
      manager.soaInfo.SetPermittedFlowDirections(this.idx, permitted);
    }

    public ConduitFlow.FlowDirections GetTargetFlowDirection(ConduitFlow manager) => manager.soaInfo.GetTargetFlowDirection(this.idx);

    public void SetTargetFlowDirection(ConduitFlow.FlowDirections directions, ConduitFlow manager) => manager.soaInfo.SetTargetFlowDirection(this.idx, directions);

    public ConduitFlow.ConduitContents GetContents(ConduitFlow manager)
    {
      int cell = manager.soaInfo.GetCell(this.idx);
      ConduitFlow.ConduitContents contents = manager.grid[cell].contents;
      ConduitFlow.SOAInfo soaInfo = manager.soaInfo;
      contents.temperature = soaInfo.GetConduitTemperature(this.idx);
      ConduitDiseaseManager.Data diseaseData = soaInfo.GetDiseaseData(this.idx);
      contents.diseaseIdx = diseaseData.diseaseIdx;
      contents.diseaseCount = diseaseData.diseaseCount;
      return contents;
    }

    public void SetContents(ConduitFlow manager, ConduitFlow.ConduitContents contents)
    {
      int cell = manager.soaInfo.GetCell(this.idx);
      manager.grid[cell].contents = contents;
      ConduitFlow.SOAInfo soaInfo = manager.soaInfo;
      soaInfo.SetConduitTemperatureData(this.idx, ref contents);
      soaInfo.ForcePermanentDiseaseContainer(this.idx, contents.diseaseIdx != byte.MaxValue);
      soaInfo.SetDiseaseData(this.idx, ref contents);
    }

    public ConduitFlow.ConduitFlowInfo GetLastFlowInfo(ConduitFlow manager) => manager.soaInfo.GetLastFlowInfo(this.idx);

    public ConduitFlow.ConduitContents GetInitialContents(ConduitFlow manager) => manager.soaInfo.GetInitialContents(this.idx);

    public int GetCell(ConduitFlow manager) => manager.soaInfo.GetCell(this.idx);

    public bool Equals(ConduitFlow.Conduit other) => this.idx == other.idx;
  }

  [DebuggerDisplay("{element} M:{mass} T:{temperature}")]
  public struct ConduitContents
  {
    public SimHashes element;
    private float initial_mass;
    private float added_mass;
    private float removed_mass;
    public float temperature;
    public byte diseaseIdx;
    public int diseaseCount;
    public static readonly ConduitFlow.ConduitContents Empty = new ConduitFlow.ConduitContents()
    {
      element = SimHashes.Vacuum,
      initial_mass = 0.0f,
      added_mass = 0.0f,
      removed_mass = 0.0f,
      temperature = 0.0f,
      diseaseIdx = byte.MaxValue,
      diseaseCount = 0
    };

    public float mass => this.initial_mass + this.added_mass - this.removed_mass;

    public float movable_mass => this.initial_mass - this.removed_mass;

    public ConduitContents(
      SimHashes element,
      float mass,
      float temperature,
      byte disease_idx,
      int disease_count)
    {
      Debug.Assert(!float.IsNaN(temperature));
      this.element = element;
      this.initial_mass = mass;
      this.added_mass = 0.0f;
      this.removed_mass = 0.0f;
      this.temperature = temperature;
      this.diseaseIdx = disease_idx;
      this.diseaseCount = disease_count;
    }

    public void ConsolidateMass()
    {
      this.initial_mass += this.added_mass;
      this.added_mass = 0.0f;
      this.initial_mass -= this.removed_mass;
      this.removed_mass = 0.0f;
    }

    public float GetEffectiveCapacity(float maximum_capacity)
    {
      float mass = this.mass;
      return Mathf.Max(0.0f, maximum_capacity - mass);
    }

    public void AddMass(float amount)
    {
      Debug.Assert(0.0 <= (double) amount);
      this.added_mass += amount;
    }

    public float RemoveMass(float amount)
    {
      Debug.Assert(0.0 <= (double) amount);
      float num1 = 0.0f;
      float num2 = this.mass - amount;
      if ((double) num2 < 0.0)
      {
        amount += num2;
        num1 = -num2;
        Debug.Assert(false);
      }
      this.removed_mass += amount;
      return num1;
    }
  }

  [DebuggerDisplay("{network.ConduitType}:{cells.Count}")]
  private struct Network
  {
    public List<int> cells;
    public FlowUtilityNetwork network;
  }

  private struct BuildNetworkTask : IWorkItem<ConduitFlow>
  {
    private ConduitFlow.Network network;
    private QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distance_nodes;
    private DictionaryPool<int, int, ConduitFlow>.PooledDictionary distances_via_sources;
    private ListPool<KeyValuePair<int, int>, ConduitFlow>.PooledList from_sources;
    private DictionaryPool<int, int, ConduitFlow>.PooledDictionary distances_via_sinks;
    private ListPool<KeyValuePair<int, int>, ConduitFlow>.PooledList from_sinks;
    private ConduitFlow.BuildNetworkTask.Graph from_sources_graph;
    private ConduitFlow.BuildNetworkTask.Graph from_sinks_graph;

    public BuildNetworkTask(ConduitFlow.Network network, int conduit_count)
    {
      this.network = network;
      this.distance_nodes = QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.Allocate();
      this.distances_via_sources = DictionaryPool<int, int, ConduitFlow>.Allocate();
      this.from_sources = ListPool<KeyValuePair<int, int>, ConduitFlow>.Allocate();
      this.distances_via_sinks = DictionaryPool<int, int, ConduitFlow>.Allocate();
      this.from_sinks = ListPool<KeyValuePair<int, int>, ConduitFlow>.Allocate();
      this.from_sources_graph = new ConduitFlow.BuildNetworkTask.Graph(network.network);
      this.from_sinks_graph = new ConduitFlow.BuildNetworkTask.Graph(network.network);
    }

    public void Finish()
    {
      this.distances_via_sinks.Recycle();
      this.distances_via_sources.Recycle();
      this.distance_nodes.Recycle();
      this.from_sources.Recycle();
      this.from_sinks.Recycle();
      this.from_sources_graph.Recycle();
      this.from_sinks_graph.Recycle();
    }

    private void ComputeFlow(ConduitFlow outer)
    {
      this.from_sources_graph.Build(outer, this.network.network.sources, this.network.network.sinks, true);
      this.from_sinks_graph.Build(outer, this.network.network.sinks, this.network.network.sources, false);
      this.from_sources_graph.Merge(this.from_sinks_graph);
      this.from_sources_graph.BreakCycles();
      this.from_sources_graph.WriteFlow();
      this.from_sinks_graph.WriteFlow(true);
    }

    private void ComputeOrder(ConduitFlow outer)
    {
      foreach (int source in (HashSet<int>) this.from_sources_graph.sources)
        ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) this.distance_nodes).Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode()
        {
          cell = source,
          distance = 0
        });
      foreach (int deadEnd in (HashSet<int>) this.from_sources_graph.dead_ends)
        ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) this.distance_nodes).Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode()
        {
          cell = deadEnd,
          distance = 0
        });
      ConduitFlow.BuildNetworkTask.DistanceNode distanceNode1;
      while (((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) this.distance_nodes).Count != 0)
      {
        ConduitFlow.BuildNetworkTask.DistanceNode distanceNode2 = ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) this.distance_nodes).Dequeue();
        int conduitIdx = outer.grid[distanceNode2.cell].conduitIdx;
        if (conduitIdx != -1)
        {
          ((Dictionary<int, int>) this.distances_via_sources)[distanceNode2.cell] = distanceNode2.distance;
          ConduitFlow.ConduitConnections conduitConnections = outer.soaInfo.GetConduitConnections(conduitIdx);
          int permittedFlowDirections = (int) outer.soaInfo.GetPermittedFlowDirections(conduitIdx);
          if ((permittedFlowDirections & 8) != 0)
          {
            QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distanceNodes = this.distance_nodes;
            distanceNode1 = new ConduitFlow.BuildNetworkTask.DistanceNode();
            distanceNode1.cell = outer.soaInfo.GetCell(conduitConnections.up);
            distanceNode1.distance = distanceNode2.distance + 1;
            ConduitFlow.BuildNetworkTask.DistanceNode distanceNode3 = distanceNode1;
            ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) distanceNodes).Enqueue(distanceNode3);
          }
          if ((permittedFlowDirections & 1) != 0)
          {
            QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distanceNodes = this.distance_nodes;
            distanceNode1 = new ConduitFlow.BuildNetworkTask.DistanceNode();
            distanceNode1.cell = outer.soaInfo.GetCell(conduitConnections.down);
            distanceNode1.distance = distanceNode2.distance + 1;
            ConduitFlow.BuildNetworkTask.DistanceNode distanceNode4 = distanceNode1;
            ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) distanceNodes).Enqueue(distanceNode4);
          }
          if ((permittedFlowDirections & 2) != 0)
          {
            QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distanceNodes = this.distance_nodes;
            distanceNode1 = new ConduitFlow.BuildNetworkTask.DistanceNode();
            distanceNode1.cell = outer.soaInfo.GetCell(conduitConnections.left);
            distanceNode1.distance = distanceNode2.distance + 1;
            ConduitFlow.BuildNetworkTask.DistanceNode distanceNode5 = distanceNode1;
            ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) distanceNodes).Enqueue(distanceNode5);
          }
          if ((permittedFlowDirections & 4) != 0)
          {
            QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distanceNodes = this.distance_nodes;
            distanceNode1 = new ConduitFlow.BuildNetworkTask.DistanceNode();
            distanceNode1.cell = outer.soaInfo.GetCell(conduitConnections.right);
            distanceNode1.distance = distanceNode2.distance + 1;
            ConduitFlow.BuildNetworkTask.DistanceNode distanceNode6 = distanceNode1;
            ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) distanceNodes).Enqueue(distanceNode6);
          }
        }
      }
      ((List<KeyValuePair<int, int>>) this.from_sources).AddRange((IEnumerable<KeyValuePair<int, int>>) this.distances_via_sources);
      ((List<KeyValuePair<int, int>>) this.from_sources).Sort((Comparison<KeyValuePair<int, int>>) ((a, b) => b.Value - a.Value));
      ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) this.distance_nodes).Clear();
      foreach (int source in (HashSet<int>) this.from_sinks_graph.sources)
      {
        QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distanceNodes = this.distance_nodes;
        distanceNode1 = new ConduitFlow.BuildNetworkTask.DistanceNode();
        distanceNode1.cell = source;
        distanceNode1.distance = 0;
        ConduitFlow.BuildNetworkTask.DistanceNode distanceNode7 = distanceNode1;
        ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) distanceNodes).Enqueue(distanceNode7);
      }
      foreach (int deadEnd in (HashSet<int>) this.from_sinks_graph.dead_ends)
        ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) this.distance_nodes).Enqueue(new ConduitFlow.BuildNetworkTask.DistanceNode()
        {
          cell = deadEnd,
          distance = 0
        });
      while (((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) this.distance_nodes).Count != 0)
      {
        ConduitFlow.BuildNetworkTask.DistanceNode distanceNode8 = ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) this.distance_nodes).Dequeue();
        int conduitIdx = outer.grid[distanceNode8.cell].conduitIdx;
        if (conduitIdx != -1)
        {
          if (!((Dictionary<int, int>) this.distances_via_sources).ContainsKey(distanceNode8.cell))
            ((Dictionary<int, int>) this.distances_via_sinks)[distanceNode8.cell] = distanceNode8.distance;
          ConduitFlow.ConduitConnections conduitConnections = outer.soaInfo.GetConduitConnections(conduitIdx);
          ConduitFlow.BuildNetworkTask.DistanceNode distanceNode9;
          if (conduitConnections.up != -1 && (outer.soaInfo.GetPermittedFlowDirections(conduitConnections.up) & ConduitFlow.FlowDirections.Down) != ConduitFlow.FlowDirections.None)
          {
            QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distanceNodes = this.distance_nodes;
            distanceNode9 = new ConduitFlow.BuildNetworkTask.DistanceNode();
            distanceNode9.cell = outer.soaInfo.GetCell(conduitConnections.up);
            distanceNode9.distance = distanceNode8.distance + 1;
            ConduitFlow.BuildNetworkTask.DistanceNode distanceNode10 = distanceNode9;
            ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) distanceNodes).Enqueue(distanceNode10);
          }
          if (conduitConnections.down != -1 && (outer.soaInfo.GetPermittedFlowDirections(conduitConnections.down) & ConduitFlow.FlowDirections.Up) != ConduitFlow.FlowDirections.None)
          {
            QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distanceNodes = this.distance_nodes;
            distanceNode9 = new ConduitFlow.BuildNetworkTask.DistanceNode();
            distanceNode9.cell = outer.soaInfo.GetCell(conduitConnections.down);
            distanceNode9.distance = distanceNode8.distance + 1;
            ConduitFlow.BuildNetworkTask.DistanceNode distanceNode11 = distanceNode9;
            ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) distanceNodes).Enqueue(distanceNode11);
          }
          if (conduitConnections.left != -1 && (outer.soaInfo.GetPermittedFlowDirections(conduitConnections.left) & ConduitFlow.FlowDirections.Right) != ConduitFlow.FlowDirections.None)
          {
            QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distanceNodes = this.distance_nodes;
            distanceNode9 = new ConduitFlow.BuildNetworkTask.DistanceNode();
            distanceNode9.cell = outer.soaInfo.GetCell(conduitConnections.left);
            distanceNode9.distance = distanceNode8.distance + 1;
            ConduitFlow.BuildNetworkTask.DistanceNode distanceNode12 = distanceNode9;
            ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) distanceNodes).Enqueue(distanceNode12);
          }
          if (conduitConnections.right != -1 && (outer.soaInfo.GetPermittedFlowDirections(conduitConnections.right) & ConduitFlow.FlowDirections.Left) != ConduitFlow.FlowDirections.None)
          {
            QueuePool<ConduitFlow.BuildNetworkTask.DistanceNode, ConduitFlow>.PooledQueue distanceNodes = this.distance_nodes;
            distanceNode9 = new ConduitFlow.BuildNetworkTask.DistanceNode();
            distanceNode9.cell = outer.soaInfo.GetCell(conduitConnections.right);
            distanceNode9.distance = distanceNode8.distance + 1;
            ConduitFlow.BuildNetworkTask.DistanceNode distanceNode13 = distanceNode9;
            ((Queue<ConduitFlow.BuildNetworkTask.DistanceNode>) distanceNodes).Enqueue(distanceNode13);
          }
        }
      }
      ((List<KeyValuePair<int, int>>) this.from_sinks).AddRange((IEnumerable<KeyValuePair<int, int>>) this.distances_via_sinks);
      ((List<KeyValuePair<int, int>>) this.from_sinks).Sort((Comparison<KeyValuePair<int, int>>) ((a, b) => a.Value - b.Value));
      this.network.cells.Capacity = Mathf.Max(this.network.cells.Capacity, ((List<KeyValuePair<int, int>>) this.from_sources).Count + ((List<KeyValuePair<int, int>>) this.from_sinks).Count);
      foreach (KeyValuePair<int, int> fromSource in (List<KeyValuePair<int, int>>) this.from_sources)
        this.network.cells.Add(fromSource.Key);
      foreach (KeyValuePair<int, int> fromSink in (List<KeyValuePair<int, int>>) this.from_sinks)
        this.network.cells.Add(fromSink.Key);
    }

    public void Run(ConduitFlow outer)
    {
      this.ComputeFlow(outer);
      this.ComputeOrder(outer);
    }

    [DebuggerDisplay("cell {cell}:{distance}")]
    private struct DistanceNode
    {
      public int cell;
      public int distance;
    }

    [DebuggerDisplay("vertices:{vertex_cells.Count}, edges:{edges.Count}")]
    private struct Graph
    {
      private ConduitFlow conduit_flow;
      private HashSetPool<int, ConduitFlow>.PooledHashSet vertex_cells;
      private ListPool<ConduitFlow.BuildNetworkTask.Graph.Edge, ConduitFlow>.PooledList edges;
      private ListPool<ConduitFlow.BuildNetworkTask.Graph.Edge, ConduitFlow>.PooledList cycles;
      private QueuePool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.PooledQueue bfs_traversal;
      private HashSetPool<int, ConduitFlow>.PooledHashSet visited;
      private ListPool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.PooledList pseudo_sources;
      public HashSetPool<int, ConduitFlow>.PooledHashSet sources;
      private HashSetPool<int, ConduitFlow>.PooledHashSet sinks;
      private HashSetPool<ConduitFlow.BuildNetworkTask.Graph.DFSNode, ConduitFlow>.PooledHashSet dfs_path;
      private ListPool<ConduitFlow.BuildNetworkTask.Graph.DFSNode, ConduitFlow>.PooledList dfs_traversal;
      public HashSetPool<int, ConduitFlow>.PooledHashSet dead_ends;
      private ListPool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.PooledList cycle_vertices;

      public Graph(FlowUtilityNetwork network)
      {
        this.conduit_flow = (ConduitFlow) null;
        this.vertex_cells = HashSetPool<int, ConduitFlow>.Allocate();
        this.edges = ListPool<ConduitFlow.BuildNetworkTask.Graph.Edge, ConduitFlow>.Allocate();
        this.cycles = ListPool<ConduitFlow.BuildNetworkTask.Graph.Edge, ConduitFlow>.Allocate();
        this.bfs_traversal = QueuePool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.Allocate();
        this.visited = HashSetPool<int, ConduitFlow>.Allocate();
        this.pseudo_sources = ListPool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.Allocate();
        this.sources = HashSetPool<int, ConduitFlow>.Allocate();
        this.sinks = HashSetPool<int, ConduitFlow>.Allocate();
        this.dfs_path = HashSetPool<ConduitFlow.BuildNetworkTask.Graph.DFSNode, ConduitFlow>.Allocate();
        this.dfs_traversal = ListPool<ConduitFlow.BuildNetworkTask.Graph.DFSNode, ConduitFlow>.Allocate();
        this.dead_ends = HashSetPool<int, ConduitFlow>.Allocate();
        this.cycle_vertices = ListPool<ConduitFlow.BuildNetworkTask.Graph.Vertex, ConduitFlow>.Allocate();
      }

      public void Recycle()
      {
        this.vertex_cells.Recycle();
        this.edges.Recycle();
        this.cycles.Recycle();
        this.bfs_traversal.Recycle();
        this.visited.Recycle();
        this.pseudo_sources.Recycle();
        this.sources.Recycle();
        this.sinks.Recycle();
        this.dfs_path.Recycle();
        this.dfs_traversal.Recycle();
        this.dead_ends.Recycle();
        this.cycle_vertices.Recycle();
      }

      public void Build(
        ConduitFlow conduit_flow,
        List<FlowUtilityNetwork.IItem> sources,
        List<FlowUtilityNetwork.IItem> sinks,
        bool are_dead_ends_pseudo_sources)
      {
        this.conduit_flow = conduit_flow;
        ((HashSet<int>) this.sources).Clear();
        for (int index = 0; index < sources.Count; ++index)
        {
          int cell = sources[index].Cell;
          if (conduit_flow.grid[cell].conduitIdx != -1)
            ((HashSet<int>) this.sources).Add(cell);
        }
        ((HashSet<int>) this.sinks).Clear();
        for (int index = 0; index < sinks.Count; ++index)
        {
          int cell = sinks[index].Cell;
          if (conduit_flow.grid[cell].conduitIdx != -1)
            ((HashSet<int>) this.sinks).Add(cell);
        }
        Debug.Assert(((Queue<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.bfs_traversal).Count == 0);
        ((HashSet<int>) this.visited).Clear();
        foreach (int source in (HashSet<int>) this.sources)
        {
          ((Queue<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.bfs_traversal).Enqueue(new ConduitFlow.BuildNetworkTask.Graph.Vertex()
          {
            cell = source,
            direction = ConduitFlow.FlowDirections.None
          });
          ((HashSet<int>) this.visited).Add(source);
        }
        ((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.pseudo_sources).Clear();
        ((HashSet<int>) this.dead_ends).Clear();
        ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.cycles).Clear();
        while (((Queue<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.bfs_traversal).Count != 0)
        {
          ConduitFlow.BuildNetworkTask.Graph.Vertex node = ((Queue<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.bfs_traversal).Dequeue();
          ((HashSet<int>) this.vertex_cells).Add(node.cell);
          ConduitFlow.FlowDirections flowDirections = ConduitFlow.FlowDirections.None;
          int num = 4;
          if (node.direction != ConduitFlow.FlowDirections.None)
          {
            flowDirections = ConduitFlow.Opposite(node.direction);
            num = 3;
          }
          int conduitIdx = conduit_flow.grid[node.cell].conduitIdx;
          for (int index = 0; index != num; ++index)
          {
            flowDirections = ConduitFlow.ComputeNextFlowDirection(flowDirections);
            ConduitFlow.Conduit conduitFromDirection = conduit_flow.soaInfo.GetConduitFromDirection(conduitIdx, flowDirections);
            ConduitFlow.BuildNetworkTask.Graph.Vertex new_node = this.WalkPath(conduitIdx, conduitFromDirection.idx, flowDirections, are_dead_ends_pseudo_sources);
            if (new_node.is_valid)
            {
              ConduitFlow.BuildNetworkTask.Graph.Edge edge1 = new ConduitFlow.BuildNetworkTask.Graph.Edge();
              // ISSUE: explicit reference operation
              (^ref edge1).vertices = new ConduitFlow.BuildNetworkTask.Graph.Vertex[2]
              {
                new ConduitFlow.BuildNetworkTask.Graph.Vertex()
                {
                  cell = node.cell,
                  direction = flowDirections
                },
                new_node
              };
              ConduitFlow.BuildNetworkTask.Graph.Edge edge2 = edge1;
              if (new_node.cell == node.cell)
                ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.cycles).Add(edge2);
              else if (!((IEnumerable<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges).Any<ConduitFlow.BuildNetworkTask.Graph.Edge>((Func<ConduitFlow.BuildNetworkTask.Graph.Edge, bool>) (edge => edge.vertices[0].cell == new_node.cell && edge.vertices[1].cell == node.cell)) && !((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges).Contains(edge2))
              {
                ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges).Add(edge2);
                if (((HashSet<int>) this.visited).Add(new_node.cell))
                {
                  if (this.IsSink(new_node.cell))
                    ((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.pseudo_sources).Add(new_node);
                  else
                    ((Queue<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.bfs_traversal).Enqueue(new_node);
                }
              }
            }
          }
          if (((Queue<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.bfs_traversal).Count == 0)
          {
            foreach (ConduitFlow.BuildNetworkTask.Graph.Vertex pseudoSource in (List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.pseudo_sources)
              ((Queue<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.bfs_traversal).Enqueue(pseudoSource);
            ((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.pseudo_sources).Clear();
          }
        }
      }

      private bool IsEndpoint(int cell)
      {
        Debug.Assert(cell != -1);
        return this.conduit_flow.grid[cell].conduitIdx == -1 || ((HashSet<int>) this.sources).Contains(cell) || ((HashSet<int>) this.sinks).Contains(cell) || ((HashSet<int>) this.dead_ends).Contains(cell);
      }

      private bool IsSink(int cell) => ((HashSet<int>) this.sinks).Contains(cell);

      private bool IsJunction(int cell)
      {
        Debug.Assert(cell != -1);
        ConduitFlow.GridNode gridNode = this.conduit_flow.grid[cell];
        Debug.Assert(gridNode.conduitIdx != -1);
        ConduitFlow.ConduitConnections conduitConnections = this.conduit_flow.soaInfo.GetConduitConnections(gridNode.conduitIdx);
        return 2 < this.JunctionValue(conduitConnections.down) + this.JunctionValue(conduitConnections.left) + this.JunctionValue(conduitConnections.up) + this.JunctionValue(conduitConnections.right);
      }

      private int JunctionValue(int conduit) => conduit != -1 ? 1 : 0;

      private ConduitFlow.BuildNetworkTask.Graph.Vertex WalkPath(
        int root_conduit,
        int conduit,
        ConduitFlow.FlowDirections direction,
        bool are_dead_ends_pseudo_sources)
      {
        if (conduit == -1)
          return ConduitFlow.BuildNetworkTask.Graph.Vertex.INVALID;
        int cell;
        bool flag;
        do
        {
          cell = this.conduit_flow.soaInfo.GetCell(conduit);
          if (this.IsEndpoint(cell) || this.IsJunction(cell))
            return new ConduitFlow.BuildNetworkTask.Graph.Vertex()
            {
              cell = cell,
              direction = direction
            };
          direction = ConduitFlow.Opposite(direction);
          flag = true;
          for (int index = 0; index != 3; ++index)
          {
            direction = ConduitFlow.ComputeNextFlowDirection(direction);
            ConduitFlow.Conduit conduitFromDirection = this.conduit_flow.soaInfo.GetConduitFromDirection(conduit, direction);
            if (conduitFromDirection.idx != -1)
            {
              conduit = conduitFromDirection.idx;
              flag = false;
              break;
            }
          }
        }
        while (!flag);
        if (are_dead_ends_pseudo_sources)
        {
          ((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.pseudo_sources).Add(new ConduitFlow.BuildNetworkTask.Graph.Vertex()
          {
            cell = cell,
            direction = ConduitFlow.ComputeNextFlowDirection(direction)
          });
          ((HashSet<int>) this.dead_ends).Add(cell);
          return ConduitFlow.BuildNetworkTask.Graph.Vertex.INVALID;
        }
        return new ConduitFlow.BuildNetworkTask.Graph.Vertex()
        {
          cell = cell,
          direction = direction = ConduitFlow.Opposite(ConduitFlow.ComputeNextFlowDirection(direction))
        };
      }

      public void Merge(ConduitFlow.BuildNetworkTask.Graph inverted_graph)
      {
        foreach (ConduitFlow.BuildNetworkTask.Graph.Edge edge1 in (List<ConduitFlow.BuildNetworkTask.Graph.Edge>) inverted_graph.edges)
        {
          ConduitFlow.BuildNetworkTask.Graph.Edge inverted_edge = edge1;
          ConduitFlow.BuildNetworkTask.Graph.Edge candidate = inverted_edge.Invert();
          if (!((IEnumerable<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges).Any<ConduitFlow.BuildNetworkTask.Graph.Edge>((Func<ConduitFlow.BuildNetworkTask.Graph.Edge, bool>) (edge => edge.Equals(inverted_edge) || edge.Equals(candidate))))
          {
            ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges).Add(candidate);
            ((HashSet<int>) this.vertex_cells).Add(candidate.vertices[0].cell);
            ((HashSet<int>) this.vertex_cells).Add(candidate.vertices[1].cell);
          }
        }
        int num = 1000;
        for (int index1 = 0; index1 != num; ++index1)
        {
          Debug.Assert(index1 != num - 1);
          bool flag = false;
          foreach (int vertexCell in (HashSet<int>) this.vertex_cells)
          {
            int cell = vertexCell;
            if (!this.IsSink(cell) && !((IEnumerable<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges).Any<ConduitFlow.BuildNetworkTask.Graph.Edge>((Func<ConduitFlow.BuildNetworkTask.Graph.Edge, bool>) (edge => edge.vertices[0].cell == cell)))
            {
              int index2 = ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) inverted_graph.edges).FindIndex((Predicate<ConduitFlow.BuildNetworkTask.Graph.Edge>) (inverted_edge => inverted_edge.vertices[1].cell == cell));
              if (index2 != -1)
              {
                ConduitFlow.BuildNetworkTask.Graph.Edge edge2 = ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) inverted_graph.edges)[index2];
                for (int index3 = 0; index3 != ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges).Count; ++index3)
                {
                  ConduitFlow.BuildNetworkTask.Graph.Edge edge3 = ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges)[index3];
                  if (edge3.vertices[0].cell == edge2.vertices[0].cell && edge3.vertices[1].cell == edge2.vertices[1].cell)
                    ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges)[index3] = edge3.Invert();
                }
                flag = true;
                break;
              }
            }
          }
          if (!flag)
            break;
        }
      }

      public void BreakCycles()
      {
        ((HashSet<int>) this.visited).Clear();
        foreach (int vertexCell in (HashSet<int>) this.vertex_cells)
        {
          if (!((HashSet<int>) this.visited).Contains(vertexCell))
          {
            ((HashSet<ConduitFlow.BuildNetworkTask.Graph.DFSNode>) this.dfs_path).Clear();
            ((List<ConduitFlow.BuildNetworkTask.Graph.DFSNode>) this.dfs_traversal).Clear();
            ((List<ConduitFlow.BuildNetworkTask.Graph.DFSNode>) this.dfs_traversal).Add(new ConduitFlow.BuildNetworkTask.Graph.DFSNode()
            {
              cell = vertexCell,
              parent = (ConduitFlow.BuildNetworkTask.Graph.DFSNode) null
            });
            while (((List<ConduitFlow.BuildNetworkTask.Graph.DFSNode>) this.dfs_traversal).Count != 0)
            {
              ConduitFlow.BuildNetworkTask.Graph.DFSNode dfsNode = ((List<ConduitFlow.BuildNetworkTask.Graph.DFSNode>) this.dfs_traversal)[((List<ConduitFlow.BuildNetworkTask.Graph.DFSNode>) this.dfs_traversal).Count - 1];
              ((List<ConduitFlow.BuildNetworkTask.Graph.DFSNode>) this.dfs_traversal).RemoveAt(((List<ConduitFlow.BuildNetworkTask.Graph.DFSNode>) this.dfs_traversal).Count - 1);
              bool flag = false;
              for (ConduitFlow.BuildNetworkTask.Graph.DFSNode parent = dfsNode.parent; parent != null; parent = parent.parent)
              {
                if (parent.cell == dfsNode.cell)
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
              {
                for (int index = ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges).Count - 1; index != -1; --index)
                {
                  ConduitFlow.BuildNetworkTask.Graph.Edge edge = ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges)[index];
                  if (edge.vertices[0].cell == dfsNode.parent.cell && edge.vertices[1].cell == dfsNode.cell)
                  {
                    ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.cycles).Add(edge);
                    ((List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges).RemoveAt(index);
                  }
                }
              }
              else if (((HashSet<int>) this.visited).Add(dfsNode.cell))
              {
                foreach (ConduitFlow.BuildNetworkTask.Graph.Edge edge in (List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges)
                {
                  if (edge.vertices[0].cell == dfsNode.cell)
                    ((List<ConduitFlow.BuildNetworkTask.Graph.DFSNode>) this.dfs_traversal).Add(new ConduitFlow.BuildNetworkTask.Graph.DFSNode()
                    {
                      cell = edge.vertices[1].cell,
                      parent = dfsNode
                    });
                }
              }
            }
          }
        }
      }

      public void WriteFlow(bool cycles_only = false)
      {
        if (!cycles_only)
        {
          foreach (ConduitFlow.BuildNetworkTask.Graph.Edge edge in (List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.edges)
          {
            ConduitFlow.BuildNetworkTask.Graph.Edge.VertexIterator vertexIterator = edge.Iter(this.conduit_flow);
            while (vertexIterator.IsValid())
            {
              int num = (int) this.conduit_flow.soaInfo.AddPermittedFlowDirections(this.conduit_flow.grid[vertexIterator.cell].conduitIdx, vertexIterator.direction);
              vertexIterator.Next();
            }
          }
        }
        foreach (ConduitFlow.BuildNetworkTask.Graph.Edge cycle in (List<ConduitFlow.BuildNetworkTask.Graph.Edge>) this.cycles)
        {
          ((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.cycle_vertices).Clear();
          ConduitFlow.BuildNetworkTask.Graph.Edge.VertexIterator vertexIterator = cycle.Iter(this.conduit_flow);
          vertexIterator.Next();
          while (vertexIterator.IsValid())
          {
            ((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.cycle_vertices).Add(new ConduitFlow.BuildNetworkTask.Graph.Vertex()
            {
              cell = vertexIterator.cell,
              direction = vertexIterator.direction
            });
            vertexIterator.Next();
          }
          if (((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.cycle_vertices).Count > 1)
          {
            int index1 = 0;
            int index2 = ((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.cycle_vertices).Count - 1;
            ConduitFlow.FlowDirections direction = cycle.vertices[0].direction;
            for (; index1 <= index2; --index2)
            {
              ConduitFlow.BuildNetworkTask.Graph.Vertex cycleVertex1 = ((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.cycle_vertices)[index1];
              int num1 = (int) this.conduit_flow.soaInfo.AddPermittedFlowDirections(this.conduit_flow.grid[cycleVertex1.cell].conduitIdx, ConduitFlow.Opposite(direction));
              direction = cycleVertex1.direction;
              ++index1;
              ConduitFlow.BuildNetworkTask.Graph.Vertex cycleVertex2 = ((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.cycle_vertices)[index2];
              int num2 = (int) this.conduit_flow.soaInfo.AddPermittedFlowDirections(this.conduit_flow.grid[cycleVertex2.cell].conduitIdx, cycleVertex2.direction);
            }
            ((HashSet<int>) this.dead_ends).Add(((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.cycle_vertices)[index1].cell);
            ((HashSet<int>) this.dead_ends).Add(((List<ConduitFlow.BuildNetworkTask.Graph.Vertex>) this.cycle_vertices)[index2].cell);
          }
        }
      }

      [DebuggerDisplay("{cell}:{direction}")]
      public struct Vertex : IEquatable<ConduitFlow.BuildNetworkTask.Graph.Vertex>
      {
        public ConduitFlow.FlowDirections direction;
        public int cell;
        public static ConduitFlow.BuildNetworkTask.Graph.Vertex INVALID = new ConduitFlow.BuildNetworkTask.Graph.Vertex()
        {
          direction = ConduitFlow.FlowDirections.None,
          cell = -1
        };

        public bool is_valid => this.cell != -1;

        public bool Equals(ConduitFlow.BuildNetworkTask.Graph.Vertex rhs) => this.direction == rhs.direction && this.cell == rhs.cell;
      }

      [DebuggerDisplay("{vertices[0].cell}:{vertices[0].direction} -> {vertices[1].cell}:{vertices[1].direction}")]
      public struct Edge : IEquatable<ConduitFlow.BuildNetworkTask.Graph.Edge>
      {
        public ConduitFlow.BuildNetworkTask.Graph.Vertex[] vertices;
        public static readonly ConduitFlow.BuildNetworkTask.Graph.Edge INVALID = new ConduitFlow.BuildNetworkTask.Graph.Edge()
        {
          vertices = (ConduitFlow.BuildNetworkTask.Graph.Vertex[]) null
        };

        public bool is_valid => this.vertices != null;

        public bool Equals(ConduitFlow.BuildNetworkTask.Graph.Edge rhs)
        {
          if (this.vertices == null)
            return rhs.vertices == null;
          return rhs.vertices != null && this.vertices.Length == rhs.vertices.Length && this.vertices.Length == 2 && this.vertices[0].Equals(rhs.vertices[0]) && this.vertices[1].Equals(rhs.vertices[1]);
        }

        public ConduitFlow.BuildNetworkTask.Graph.Edge Invert()
        {
          ConduitFlow.BuildNetworkTask.Graph.Edge edge = new ConduitFlow.BuildNetworkTask.Graph.Edge();
          // ISSUE: explicit reference operation
          (^ref edge).vertices = new ConduitFlow.BuildNetworkTask.Graph.Vertex[2]
          {
            new ConduitFlow.BuildNetworkTask.Graph.Vertex()
            {
              cell = this.vertices[1].cell,
              direction = ConduitFlow.Opposite(this.vertices[1].direction)
            },
            new ConduitFlow.BuildNetworkTask.Graph.Vertex()
            {
              cell = this.vertices[0].cell,
              direction = ConduitFlow.Opposite(this.vertices[0].direction)
            }
          };
          return edge;
        }

        public ConduitFlow.BuildNetworkTask.Graph.Edge.VertexIterator Iter(ConduitFlow conduit_flow) => new ConduitFlow.BuildNetworkTask.Graph.Edge.VertexIterator(conduit_flow, this);

        [DebuggerDisplay("{cell}:{direction}")]
        public struct VertexIterator
        {
          public int cell;
          public ConduitFlow.FlowDirections direction;
          private ConduitFlow conduit_flow;
          private ConduitFlow.BuildNetworkTask.Graph.Edge edge;

          public VertexIterator(
            ConduitFlow conduit_flow,
            ConduitFlow.BuildNetworkTask.Graph.Edge edge)
          {
            this.conduit_flow = conduit_flow;
            this.edge = edge;
            this.cell = edge.vertices[0].cell;
            this.direction = edge.vertices[0].direction;
          }

          public void Next()
          {
            ConduitFlow.Conduit conduitFromDirection = this.conduit_flow.soaInfo.GetConduitFromDirection(this.conduit_flow.grid[this.cell].conduitIdx, this.direction);
            Debug.Assert(conduitFromDirection.idx != -1);
            this.cell = conduitFromDirection.GetCell(this.conduit_flow);
            if (this.cell == this.edge.vertices[1].cell)
              return;
            this.direction = ConduitFlow.Opposite(this.direction);
            bool flag = false;
            for (int index = 0; index != 3; ++index)
            {
              this.direction = ConduitFlow.ComputeNextFlowDirection(this.direction);
              if (this.conduit_flow.soaInfo.GetConduitFromDirection(conduitFromDirection.idx, this.direction).idx != -1)
              {
                flag = true;
                break;
              }
            }
            Debug.Assert(flag);
            if (flag)
              return;
            this.cell = this.edge.vertices[1].cell;
          }

          public bool IsValid() => this.cell != this.edge.vertices[1].cell;
        }
      }

      [DebuggerDisplay("cell:{cell}, parent:{parent == null ? -1 : parent.cell}")]
      private class DFSNode
      {
        public int cell;
        public ConduitFlow.BuildNetworkTask.Graph.DFSNode parent;
      }
    }
  }

  private struct ConnectContext
  {
    public ListPool<int, ConduitFlow>.PooledList cells;
    public ConduitFlow outer;

    public ConnectContext(ConduitFlow outer)
    {
      this.outer = outer;
      this.cells = ListPool<int, ConduitFlow>.Allocate();
      ((List<int>) this.cells).Capacity = Mathf.Max(((List<int>) this.cells).Capacity, outer.soaInfo.NumEntries);
    }

    public void Finish() => this.cells.Recycle();
  }

  private struct ConnectTask : IWorkItem<ConduitFlow.ConnectContext>
  {
    private int start;
    private int end;

    public ConnectTask(int start, int end)
    {
      this.start = start;
      this.end = end;
    }

    public void Run(ConduitFlow.ConnectContext context)
    {
      for (int start = this.start; start != this.end; ++start)
      {
        int cell1 = ((List<int>) context.cells)[start];
        int conduitIdx = context.outer.grid[cell1].conduitIdx;
        if (conduitIdx != -1)
        {
          UtilityConnections connections = context.outer.networkMgr.GetConnections(cell1, true);
          if (connections != (UtilityConnections) 0)
          {
            ConduitFlow.ConduitConnections data = ConduitFlow.ConduitConnections.DEFAULT;
            int cell2 = cell1 - 1;
            if (Grid.IsValidCell(cell2) && (connections & UtilityConnections.Left) != (UtilityConnections) 0)
              data.left = context.outer.grid[cell2].conduitIdx;
            int cell3 = cell1 + 1;
            if (Grid.IsValidCell(cell3) && (connections & UtilityConnections.Right) != (UtilityConnections) 0)
              data.right = context.outer.grid[cell3].conduitIdx;
            int cell4 = cell1 - Grid.WidthInCells;
            if (Grid.IsValidCell(cell4) && (connections & UtilityConnections.Down) != (UtilityConnections) 0)
              data.down = context.outer.grid[cell4].conduitIdx;
            int cell5 = cell1 + Grid.WidthInCells;
            if (Grid.IsValidCell(cell5) && (connections & UtilityConnections.Up) != (UtilityConnections) 0)
              data.up = context.outer.grid[cell5].conduitIdx;
            context.outer.soaInfo.SetConduitConnections(conduitIdx, data);
          }
        }
      }
    }
  }

  private struct Sink
  {
    public ConduitConsumer consumer;
    public float space_remaining;

    public Sink(FlowUtilityNetwork.IItem sink)
    {
      this.consumer = Object.op_Inequality((Object) sink.GameObject, (Object) null) ? sink.GameObject.GetComponent<ConduitConsumer>() : (ConduitConsumer) null;
      this.space_remaining = !Object.op_Inequality((Object) this.consumer, (Object) null) || !this.consumer.operational.IsOperational ? 0.0f : this.consumer.space_remaining_kg;
    }
  }

  private class UpdateNetworkTask : IWorkItem<ConduitFlow>
  {
    private ConduitFlow.Network network;
    private DictionaryPool<int, ConduitFlow.Sink, ConduitFlow>.PooledDictionary sinks;

    public bool continue_updating { get; private set; }

    public UpdateNetworkTask(ConduitFlow.Network network)
    {
      this.continue_updating = true;
      this.network = network;
      this.sinks = DictionaryPool<int, ConduitFlow.Sink, ConduitFlow>.Allocate();
      foreach (FlowUtilityNetwork.IItem sink in network.network.sinks)
        ((Dictionary<int, ConduitFlow.Sink>) this.sinks).Add(sink.Cell, new ConduitFlow.Sink(sink));
    }

    public void Run(ConduitFlow conduit_flow)
    {
      Debug.Assert(this.continue_updating);
      this.continue_updating = false;
      foreach (int cell in this.network.cells)
      {
        int conduitIdx = conduit_flow.grid[cell].conduitIdx;
        if (conduit_flow.UpdateConduit(conduit_flow.soaInfo.GetConduit(conduitIdx), (Dictionary<int, ConduitFlow.Sink>) this.sinks))
          this.continue_updating = true;
      }
    }

    public void Finish(ConduitFlow conduit_flow)
    {
      foreach (int cell in this.network.cells)
      {
        ConduitFlow.ConduitContents contents = conduit_flow.grid[cell].contents;
        contents.ConsolidateMass();
        conduit_flow.grid[cell].contents = contents;
      }
      this.sinks.Recycle();
    }
  }
}
