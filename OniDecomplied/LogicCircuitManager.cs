// Decompiled with JetBrains decompiler
// Type: LogicCircuitManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LogicCircuitManager
{
  public static float ClockTickInterval = 0.1f;
  private float elapsedTime;
  private UtilityNetworkManager<LogicCircuitNetwork, LogicWire> conduitSystem;
  private List<ILogicUIElement> uiVisElements = new List<ILogicUIElement>();
  public static float BridgeRefreshInterval = 1f;
  private List<LogicUtilityNetworkLink>[] bridgeGroups = new List<LogicUtilityNetworkLink>[2];
  private bool updateEvenBridgeGroups;
  private float timeSinceBridgeRefresh;
  public System.Action onLogicTick;
  public Action<ILogicUIElement> onElemAdded;
  public Action<ILogicUIElement> onElemRemoved;

  public LogicCircuitManager(
    UtilityNetworkManager<LogicCircuitNetwork, LogicWire> conduit_system)
  {
    this.conduitSystem = conduit_system;
    this.timeSinceBridgeRefresh = 0.0f;
    this.elapsedTime = 0.0f;
    for (int index = 0; index < 2; ++index)
      this.bridgeGroups[index] = new List<LogicUtilityNetworkLink>();
  }

  public void RenderEveryTick(float dt) => this.Refresh(dt);

  private void Refresh(float dt)
  {
    if (this.conduitSystem.IsDirty)
    {
      this.conduitSystem.Update();
      LogicCircuitNetwork.logicSoundRegister.Clear();
      this.PropagateSignals(true);
      this.elapsedTime = 0.0f;
    }
    else if (Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null) && !SpeedControlScreen.Instance.IsPaused)
    {
      this.elapsedTime += dt;
      this.timeSinceBridgeRefresh += dt;
      while ((double) this.elapsedTime > (double) LogicCircuitManager.ClockTickInterval)
      {
        this.elapsedTime -= LogicCircuitManager.ClockTickInterval;
        this.PropagateSignals(false);
        if (this.onLogicTick != null)
          this.onLogicTick();
      }
      if ((double) this.timeSinceBridgeRefresh > (double) LogicCircuitManager.BridgeRefreshInterval)
      {
        this.UpdateCircuitBridgeLists();
        this.timeSinceBridgeRefresh = 0.0f;
      }
    }
    foreach (LogicCircuitNetwork network in (IEnumerable<UtilityNetwork>) Game.Instance.logicCircuitSystem.GetNetworks())
      this.CheckCircuitOverloaded(dt, network.id, network.GetBitsUsed());
  }

  private void PropagateSignals(bool force_send_events)
  {
    IList<UtilityNetwork> networks = Game.Instance.logicCircuitSystem.GetNetworks();
    foreach (LogicCircuitNetwork logicCircuitNetwork in (IEnumerable<UtilityNetwork>) networks)
      logicCircuitNetwork.UpdateLogicValue();
    foreach (LogicCircuitNetwork logicCircuitNetwork in (IEnumerable<UtilityNetwork>) networks)
      logicCircuitNetwork.SendLogicEvents(force_send_events, logicCircuitNetwork.id);
  }

  public LogicCircuitNetwork GetNetworkForCell(int cell) => this.conduitSystem.GetNetworkForCell(cell) as LogicCircuitNetwork;

  public void AddVisElem(ILogicUIElement elem)
  {
    this.uiVisElements.Add(elem);
    if (this.onElemAdded == null)
      return;
    this.onElemAdded(elem);
  }

  public void RemoveVisElem(ILogicUIElement elem)
  {
    if (this.onElemRemoved != null)
      this.onElemRemoved(elem);
    this.uiVisElements.Remove(elem);
  }

  public ReadOnlyCollection<ILogicUIElement> GetVisElements() => this.uiVisElements.AsReadOnly();

  public static void ToggleNoWireConnected(bool show_missing_wire, GameObject go) => go.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoLogicWireConnected, show_missing_wire);

  private void CheckCircuitOverloaded(float dt, int id, int bits_used)
  {
    UtilityNetwork networkById = Game.Instance.logicCircuitSystem.GetNetworkByID(id);
    if (networkById == null)
      return;
    ((LogicCircuitNetwork) networkById)?.UpdateOverloadTime(dt, bits_used);
  }

  public void Connect(LogicUtilityNetworkLink bridge) => this.bridgeGroups[(int) bridge.bitDepth].Add(bridge);

  public void Disconnect(LogicUtilityNetworkLink bridge) => this.bridgeGroups[(int) bridge.bitDepth].Remove(bridge);

  private void UpdateCircuitBridgeLists()
  {
    foreach (LogicCircuitNetwork network in (IEnumerable<UtilityNetwork>) Game.Instance.logicCircuitSystem.GetNetworks())
    {
      if (this.updateEvenBridgeGroups)
      {
        if (network.id % 2 == 0)
          network.UpdateRelevantBridges(this.bridgeGroups);
      }
      else if (network.id % 2 == 1)
        network.UpdateRelevantBridges(this.bridgeGroups);
    }
    this.updateEvenBridgeGroups = !this.updateEvenBridgeGroups;
  }

  private struct Signal
  {
    public int cell;
    public int value;

    public Signal(int cell, int value)
    {
      this.cell = cell;
      this.value = value;
    }
  }
}
