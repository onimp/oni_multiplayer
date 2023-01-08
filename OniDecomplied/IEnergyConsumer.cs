// Decompiled with JetBrains decompiler
// Type: IEnergyConsumer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public interface IEnergyConsumer : ICircuitConnected
{
  float WattsUsed { get; }

  float WattsNeededWhenActive { get; }

  int PowerSortOrder { get; }

  void SetConnectionStatus(CircuitManager.ConnectionStatus status);

  string Name { get; }

  bool IsConnected { get; }

  bool IsPowered { get; }
}
