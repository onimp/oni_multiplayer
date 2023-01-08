// Decompiled with JetBrains decompiler
// Type: IUtilityItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public interface IUtilityItem
{
  UtilityConnections Connections { get; set; }

  void UpdateConnections(UtilityConnections Connections);

  int GetNetworkID();

  UtilityNetwork GetNetworkForDirection(Direction d);
}
