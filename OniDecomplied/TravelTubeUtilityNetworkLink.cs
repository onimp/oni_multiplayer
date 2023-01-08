// Decompiled with JetBrains decompiler
// Type: TravelTubeUtilityNetworkLink
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class TravelTubeUtilityNetworkLink : UtilityNetworkLink, IHaveUtilityNetworkMgr
{
  protected override void OnSpawn() => base.OnSpawn();

  protected override void OnConnect(int cell1, int cell2) => Game.Instance.travelTubeSystem.AddLink(cell1, cell2);

  protected override void OnDisconnect(int cell1, int cell2) => Game.Instance.travelTubeSystem.RemoveLink(cell1, cell2);

  public IUtilityNetworkMgr GetNetworkManager() => (IUtilityNetworkMgr) Game.Instance.travelTubeSystem;
}
