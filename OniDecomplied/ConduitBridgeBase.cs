// Decompiled with JetBrains decompiler
// Type: ConduitBridgeBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class ConduitBridgeBase : KMonoBehaviour
{
  public ConduitBridgeBase.DesiredMassTransfer desiredMassTransfer;
  public ConduitBridgeBase.ConduitBridgeEvent OnMassTransfer;

  protected void SendEmptyOnMassTransfer()
  {
    if (this.OnMassTransfer == null)
      return;
    this.OnMassTransfer(SimHashes.Void, 0.0f, 0.0f, (byte) 0, 0, (Pickupable) null);
  }

  public delegate float DesiredMassTransfer(
    float dt,
    SimHashes element,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count,
    Pickupable pickupable);

  public delegate void ConduitBridgeEvent(
    SimHashes element,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count,
    Pickupable pickupable);
}
