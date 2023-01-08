// Decompiled with JetBrains decompiler
// Type: EnergyConsumerSelfSustaining
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Diagnostics;
using UnityEngine;

[SerializationConfig]
[DebuggerDisplay("{name} {WattsUsed}W")]
public class EnergyConsumerSelfSustaining : EnergyConsumer
{
  private bool isSustained;
  private CircuitManager.ConnectionStatus connectionStatus;

  public event System.Action OnConnectionChanged;

  public override bool IsPowered => this.isSustained || this.connectionStatus == CircuitManager.ConnectionStatus.Powered;

  public bool IsExternallyPowered => this.connectionStatus == CircuitManager.ConnectionStatus.Powered;

  public void SetSustained(bool isSustained) => this.isSustained = isSustained;

  public override void SetConnectionStatus(CircuitManager.ConnectionStatus connection_status)
  {
    CircuitManager.ConnectionStatus connectionStatus = this.connectionStatus;
    switch (connection_status)
    {
      case CircuitManager.ConnectionStatus.NotConnected:
        this.connectionStatus = CircuitManager.ConnectionStatus.NotConnected;
        break;
      case CircuitManager.ConnectionStatus.Unpowered:
        if (this.connectionStatus == CircuitManager.ConnectionStatus.Powered && Object.op_Equality((Object) ((Component) this).GetComponent<Battery>(), (Object) null))
        {
          this.connectionStatus = CircuitManager.ConnectionStatus.Unpowered;
          break;
        }
        break;
      case CircuitManager.ConnectionStatus.Powered:
        if (this.connectionStatus != CircuitManager.ConnectionStatus.Powered)
        {
          this.connectionStatus = CircuitManager.ConnectionStatus.Powered;
          break;
        }
        break;
    }
    this.UpdatePoweredStatus();
    if (connectionStatus == this.connectionStatus || this.OnConnectionChanged == null)
      return;
    this.OnConnectionChanged();
  }

  public void UpdatePoweredStatus() => this.operational.SetFlag(EnergyConsumer.PoweredFlag, this.IsPowered);
}
