// Decompiled with JetBrains decompiler
// Type: KAnimGraphTileVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/KAnimGraphTileVisualizer")]
public class KAnimGraphTileVisualizer : KMonoBehaviour, ISaveLoadable, IUtilityItem
{
  [Serialize]
  private UtilityConnections _connections;
  public bool isPhysicalBuilding;
  public bool skipCleanup;
  public bool skipRefresh;
  public KAnimGraphTileVisualizer.ConnectionSource connectionSource;
  [NonSerialized]
  public IUtilityNetworkMgr connectionManager;

  public UtilityConnections Connections
  {
    get => this._connections;
    set
    {
      this._connections = value;
      this.Trigger(-1041684577, (object) this._connections);
    }
  }

  public IUtilityNetworkMgr ConnectionManager
  {
    get
    {
      switch (this.connectionSource)
      {
        case KAnimGraphTileVisualizer.ConnectionSource.Gas:
          return (IUtilityNetworkMgr) Game.Instance.gasConduitSystem;
        case KAnimGraphTileVisualizer.ConnectionSource.Liquid:
          return (IUtilityNetworkMgr) Game.Instance.liquidConduitSystem;
        case KAnimGraphTileVisualizer.ConnectionSource.Electrical:
          return (IUtilityNetworkMgr) Game.Instance.electricalConduitSystem;
        case KAnimGraphTileVisualizer.ConnectionSource.Logic:
          return (IUtilityNetworkMgr) Game.Instance.logicCircuitSystem;
        case KAnimGraphTileVisualizer.ConnectionSource.Tube:
          return (IUtilityNetworkMgr) Game.Instance.travelTubeSystem;
        case KAnimGraphTileVisualizer.ConnectionSource.Solid:
          return (IUtilityNetworkMgr) Game.Instance.solidConduitSystem;
        default:
          return (IUtilityNetworkMgr) null;
      }
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.connectionManager = this.ConnectionManager;
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    this.connectionManager.SetConnections(this.Connections, cell, this.isPhysicalBuilding);
    Building component = ((Component) this).GetComponent<Building>();
    TileVisualizer.RefreshCell(cell, component.Def.TileLayer, component.Def.ReplacementLayer);
  }

  protected virtual void OnCleanUp()
  {
    if (this.connectionManager == null || this.skipCleanup)
      return;
    this.skipRefresh = true;
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    this.connectionManager.ClearCell(cell, this.isPhysicalBuilding);
    Building component = ((Component) this).GetComponent<Building>();
    TileVisualizer.RefreshCell(cell, component.Def.TileLayer, component.Def.ReplacementLayer);
  }

  [ContextMenu("Refresh")]
  public void Refresh()
  {
    if (this.connectionManager == null || this.skipRefresh)
      return;
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    this.Connections = this.connectionManager.GetConnections(cell, this.isPhysicalBuilding);
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    string visualizerString = this.connectionManager.GetVisualizerString(cell);
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<BuildingUnderConstruction>(), (Object) null) && component.HasAnimation(HashedString.op_Implicit(visualizerString + "_place")))
      visualizerString += "_place";
    if (visualizerString == null || !(visualizerString != ""))
      return;
    component.Play(HashedString.op_Implicit(visualizerString));
  }

  public int GetNetworkID()
  {
    UtilityNetwork network = this.GetNetwork();
    return network == null ? -1 : network.id;
  }

  private UtilityNetwork GetNetwork() => this.connectionManager.GetNetworkForDirection(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), Direction.None);

  public UtilityNetwork GetNetworkForDirection(Direction d) => this.connectionManager.GetNetworkForDirection(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), d);

  public void UpdateConnections(UtilityConnections new_connections)
  {
    this._connections = new_connections;
    if (this.connectionManager == null)
      return;
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    this.connectionManager.SetConnections(new_connections, cell, this.isPhysicalBuilding);
  }

  public KAnimGraphTileVisualizer GetNeighbour(Direction d)
  {
    KAnimGraphTileVisualizer neighbour = (KAnimGraphTileVisualizer) null;
    Vector2I xy;
    Grid.PosToXY(TransformExtensions.GetPosition(this.transform), out xy);
    int cell = -1;
    switch (d)
    {
      case Direction.Up:
        if (xy.y < Grid.HeightInCells - 1)
        {
          cell = Grid.XYToCell(xy.x, xy.y + 1);
          break;
        }
        break;
      case Direction.Right:
        if (xy.x < Grid.WidthInCells - 1)
        {
          cell = Grid.XYToCell(xy.x + 1, xy.y);
          break;
        }
        break;
      case Direction.Down:
        if (xy.y > 0)
        {
          cell = Grid.XYToCell(xy.x, xy.y - 1);
          break;
        }
        break;
      case Direction.Left:
        if (xy.x > 0)
        {
          cell = Grid.XYToCell(xy.x - 1, xy.y);
          break;
        }
        break;
    }
    if (cell != -1)
    {
      ObjectLayer layer;
      switch (this.connectionSource)
      {
        case KAnimGraphTileVisualizer.ConnectionSource.Gas:
          layer = ObjectLayer.GasConduitTile;
          break;
        case KAnimGraphTileVisualizer.ConnectionSource.Liquid:
          layer = ObjectLayer.LiquidConduitTile;
          break;
        case KAnimGraphTileVisualizer.ConnectionSource.Electrical:
          layer = ObjectLayer.WireTile;
          break;
        case KAnimGraphTileVisualizer.ConnectionSource.Logic:
          layer = ObjectLayer.LogicWireTile;
          break;
        case KAnimGraphTileVisualizer.ConnectionSource.Tube:
          layer = ObjectLayer.TravelTubeTile;
          break;
        case KAnimGraphTileVisualizer.ConnectionSource.Solid:
          layer = ObjectLayer.SolidConduitTile;
          break;
        default:
          throw new ArgumentNullException("wtf");
      }
      GameObject gameObject = Grid.Objects[cell, (int) layer];
      if (Object.op_Inequality((Object) gameObject, (Object) null))
        neighbour = gameObject.GetComponent<KAnimGraphTileVisualizer>();
    }
    return neighbour;
  }

  public enum ConnectionSource
  {
    Gas,
    Liquid,
    Electrical,
    Logic,
    Tube,
    Solid,
  }
}
