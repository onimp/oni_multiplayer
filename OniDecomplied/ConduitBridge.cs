// Decompiled with JetBrains decompiler
// Type: ConduitBridge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConduitBridge")]
public class ConduitBridge : ConduitBridgeBase, IBridgedNetworkItem
{
  [SerializeField]
  public ConduitType type;
  private int inputCell;
  private int outputCell;
  private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.accumulator = Game.Instance.accumulators.Add("Flow", (KMonoBehaviour) this);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Building component = ((Component) this).GetComponent<Building>();
    this.inputCell = component.GetUtilityInputCell();
    this.outputCell = component.GetUtilityOutputCell();
    Conduit.GetFlowManager(this.type).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
  }

  protected virtual void OnCleanUp()
  {
    Conduit.GetFlowManager(this.type).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
    Game.Instance.accumulators.Remove(this.accumulator);
    base.OnCleanUp();
  }

  private void ConduitUpdate(float dt)
  {
    ConduitFlow flowManager = Conduit.GetFlowManager(this.type);
    if (!flowManager.HasConduit(this.inputCell) || !flowManager.HasConduit(this.outputCell))
    {
      this.SendEmptyOnMassTransfer();
    }
    else
    {
      ConduitFlow.ConduitContents contents = flowManager.GetContents(this.inputCell);
      float mass = contents.mass;
      if (this.desiredMassTransfer != null)
        mass = this.desiredMassTransfer(dt, contents.element, contents.mass, contents.temperature, contents.diseaseIdx, contents.diseaseCount, (Pickupable) null);
      if ((double) mass > 0.0)
      {
        int disease_count = (int) ((double) mass / (double) contents.mass * (double) contents.diseaseCount);
        float num = flowManager.AddElement(this.outputCell, contents.element, mass, contents.temperature, contents.diseaseIdx, disease_count);
        if ((double) num > 0.0)
        {
          flowManager.RemoveElement(this.inputCell, num);
          Game.Instance.accumulators.Accumulate(this.accumulator, contents.mass);
          if (this.OnMassTransfer == null)
            return;
          this.OnMassTransfer(contents.element, num, contents.temperature, contents.diseaseIdx, disease_count, (Pickupable) null);
        }
        else
          this.SendEmptyOnMassTransfer();
      }
      else
        this.SendEmptyOnMassTransfer();
    }
  }

  public void AddNetworks(ICollection<UtilityNetwork> networks)
  {
    IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.type);
    UtilityNetwork networkForCell1 = networkManager.GetNetworkForCell(this.inputCell);
    if (networkForCell1 != null)
      networks.Add(networkForCell1);
    UtilityNetwork networkForCell2 = networkManager.GetNetworkForCell(this.outputCell);
    if (networkForCell2 == null)
      return;
    networks.Add(networkForCell2);
  }

  public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
  {
    IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.type);
    return (false ? 1 : (networks.Contains(networkManager.GetNetworkForCell(this.inputCell)) ? 1 : 0)) != 0 || networks.Contains(networkManager.GetNetworkForCell(this.outputCell));
  }

  public int GetNetworkCell() => this.inputCell;
}
