// Decompiled with JetBrains decompiler
// Type: LogicWire
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/LogicWire")]
public class LogicWire : 
  KMonoBehaviour,
  IFirstFrameCallback,
  IHaveUtilityNetworkMgr,
  IBridgedNetworkItem,
  IBitRating,
  IDisconnectable
{
  [SerializeField]
  public LogicWire.BitDepth MaxBitDepth;
  [SerializeField]
  private bool disconnected = true;
  public static readonly KAnimHashedString OutlineSymbol = new KAnimHashedString("outline");
  private static readonly EventSystem.IntraObjectHandler<LogicWire> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<LogicWire>((Action<LogicWire, object>) ((component, data) => component.OnBuildingBroken(data)));
  private static readonly EventSystem.IntraObjectHandler<LogicWire> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<LogicWire>((Action<LogicWire, object>) ((component, data) => component.OnBuildingFullyRepaired(data)));
  private System.Action firstFrameCallback;

  public static int GetBitDepthAsInt(LogicWire.BitDepth rating)
  {
    if (rating == LogicWire.BitDepth.OneBit)
      return 1;
    return rating == LogicWire.BitDepth.FourBit ? 4 : 0;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Game.Instance.logicCircuitSystem.AddToNetworks(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), (object) this, false);
    this.Subscribe<LogicWire>(774203113, LogicWire.OnBuildingBrokenDelegate);
    this.Subscribe<LogicWire>(-1735440190, LogicWire.OnBuildingFullyRepairedDelegate);
    this.Connect();
    ((Component) this).GetComponent<KBatchedAnimController>().SetSymbolVisiblity(LogicWire.OutlineSymbol, false);
  }

  protected virtual void OnCleanUp()
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    BuildingComplete component = ((Component) this).GetComponent<BuildingComplete>();
    if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Object.op_Equality((Object) Grid.Objects[cell, (int) component.Def.ReplacementLayer], (Object) null))
      Game.Instance.logicCircuitSystem.RemoveFromNetworks(cell, (object) this, false);
    this.Unsubscribe<LogicWire>(774203113, LogicWire.OnBuildingBrokenDelegate, false);
    this.Unsubscribe<LogicWire>(-1735440190, LogicWire.OnBuildingFullyRepairedDelegate, false);
    base.OnCleanUp();
  }

  public bool IsConnected => Game.Instance.logicCircuitSystem.GetNetworkForCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform))) is LogicCircuitNetwork;

  public bool IsDisconnected() => this.disconnected;

  public bool Connect()
  {
    BuildingHP component = ((Component) this).GetComponent<BuildingHP>();
    if (Object.op_Equality((Object) component, (Object) null) || component.HitPoints > 0)
    {
      this.disconnected = false;
      Game.Instance.logicCircuitSystem.ForceRebuildNetworks();
    }
    return !this.disconnected;
  }

  public void Disconnect()
  {
    this.disconnected = true;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireDisconnected);
    Game.Instance.logicCircuitSystem.ForceRebuildNetworks();
  }

  public UtilityConnections GetWireConnections() => Game.Instance.logicCircuitSystem.GetConnections(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), true);

  public string GetWireConnectionsString() => Game.Instance.logicCircuitSystem.GetVisualizerString(this.GetWireConnections());

  private void OnBuildingBroken(object data) => this.Disconnect();

  private void OnBuildingFullyRepaired(object data) => this.Connect();

  public void SetFirstFrameCallback(System.Action ffCb)
  {
    this.firstFrameCallback = ffCb;
    ((MonoBehaviour) this).StartCoroutine(this.RunCallback());
  }

  private IEnumerator RunCallback()
  {
    yield return (object) null;
    if (this.firstFrameCallback != null)
    {
      this.firstFrameCallback();
      this.firstFrameCallback = (System.Action) null;
    }
    yield return (object) null;
  }

  public LogicWire.BitDepth GetMaxBitRating() => this.MaxBitDepth;

  public IUtilityNetworkMgr GetNetworkManager() => (IUtilityNetworkMgr) Game.Instance.logicCircuitSystem;

  public void AddNetworks(ICollection<UtilityNetwork> networks)
  {
    UtilityNetwork networkForCell = Game.Instance.logicCircuitSystem.GetNetworkForCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
    if (networkForCell == null)
      return;
    networks.Add(networkForCell);
  }

  public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
  {
    UtilityNetwork networkForCell = Game.Instance.logicCircuitSystem.GetNetworkForCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
    return networks.Contains(networkForCell);
  }

  public int GetNetworkCell() => Grid.PosToCell((KMonoBehaviour) this);

  public enum BitDepth
  {
    OneBit,
    FourBit,
    NumRatings,
  }
}
