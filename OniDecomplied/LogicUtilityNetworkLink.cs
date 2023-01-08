// Decompiled with JetBrains decompiler
// Type: LogicUtilityNetworkLink
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class LogicUtilityNetworkLink : 
  UtilityNetworkLink,
  IHaveUtilityNetworkMgr,
  IBridgedNetworkItem
{
  public LogicWire.BitDepth bitDepth;
  public int cell_one;
  public int cell_two;

  protected override void OnSpawn() => base.OnSpawn();

  protected override void OnConnect(int cell1, int cell2)
  {
    this.cell_one = cell1;
    this.cell_two = cell2;
    Game.Instance.logicCircuitSystem.AddLink(cell1, cell2);
    Game.Instance.logicCircuitManager.Connect(this);
  }

  protected override void OnDisconnect(int cell1, int cell2)
  {
    Game.Instance.logicCircuitSystem.RemoveLink(cell1, cell2);
    Game.Instance.logicCircuitManager.Disconnect(this);
  }

  public IUtilityNetworkMgr GetNetworkManager() => (IUtilityNetworkMgr) Game.Instance.logicCircuitSystem;

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
