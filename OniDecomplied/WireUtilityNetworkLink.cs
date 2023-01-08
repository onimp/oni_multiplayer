// Decompiled with JetBrains decompiler
// Type: WireUtilityNetworkLink
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class WireUtilityNetworkLink : 
  UtilityNetworkLink,
  IWattageRating,
  IHaveUtilityNetworkMgr,
  IBridgedNetworkItem,
  ICircuitConnected
{
  [SerializeField]
  public Wire.WattageRating maxWattageRating;

  public Wire.WattageRating GetMaxWattageRating() => this.maxWattageRating;

  protected override void OnSpawn() => base.OnSpawn();

  protected override void OnDisconnect(int cell1, int cell2)
  {
    Game.Instance.electricalConduitSystem.RemoveLink(cell1, cell2);
    Game.Instance.circuitManager.Disconnect(this);
  }

  protected override void OnConnect(int cell1, int cell2)
  {
    Game.Instance.electricalConduitSystem.AddLink(cell1, cell2);
    Game.Instance.circuitManager.Connect(this);
  }

  public IUtilityNetworkMgr GetNetworkManager() => (IUtilityNetworkMgr) Game.Instance.electricalConduitSystem;

  public bool IsVirtual { get; private set; }

  public int PowerCell => this.GetNetworkCell();

  public object VirtualCircuitKey { get; private set; }

  public void AddNetworks(ICollection<UtilityNetwork> networks)
  {
    int networkCell = this.GetNetworkCell();
    UtilityNetwork networkForCell = this.GetNetworkManager().GetNetworkForCell(networkCell);
    if (networkForCell == null)
      return;
    networks.Add(networkForCell);
  }

  public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
  {
    int networkCell = this.GetNetworkCell();
    UtilityNetwork networkForCell = this.GetNetworkManager().GetNetworkForCell(networkCell);
    return networks.Contains(networkForCell);
  }
}
