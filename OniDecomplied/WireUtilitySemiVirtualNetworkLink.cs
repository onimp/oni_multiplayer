// Decompiled with JetBrains decompiler
// Type: WireUtilitySemiVirtualNetworkLink
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class WireUtilitySemiVirtualNetworkLink : 
  UtilityNetworkLink,
  IHaveUtilityNetworkMgr,
  ICircuitConnected
{
  [SerializeField]
  public Wire.WattageRating maxWattageRating;

  public Wire.WattageRating GetMaxWattageRating() => this.maxWattageRating;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected override void OnSpawn()
  {
    RocketModuleCluster component1 = ((Component) this).GetComponent<RocketModuleCluster>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      this.VirtualCircuitKey = (object) component1.CraftInterface;
    }
    else
    {
      CraftModuleInterface component2 = ((Component) this.GetMyWorld()).GetComponent<CraftModuleInterface>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        this.VirtualCircuitKey = (object) component2;
    }
    Game.Instance.electricalConduitSystem.AddToVirtualNetworks(this.VirtualCircuitKey, (object) this, true);
    base.OnSpawn();
  }

  public void SetLinkConnected(bool connect)
  {
    if (connect && this.visualizeOnly)
    {
      this.visualizeOnly = false;
      if (!this.isSpawned)
        return;
      this.Connect();
    }
    else
    {
      if (connect || this.visualizeOnly)
        return;
      if (this.isSpawned)
        this.Disconnect();
      this.visualizeOnly = true;
    }
  }

  protected override void OnDisconnect(int cell1, int cell2) => Game.Instance.electricalConduitSystem.RemoveSemiVirtualLink(cell1, this.VirtualCircuitKey);

  protected override void OnConnect(int cell1, int cell2) => Game.Instance.electricalConduitSystem.AddSemiVirtualLink(cell1, this.VirtualCircuitKey);

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
