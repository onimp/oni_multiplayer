// Decompiled with JetBrains decompiler
// Type: ConduitSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public abstract class ConduitSensor : Switch
{
  public ConduitType conduitType;
  protected bool wasOn;
  protected KBatchedAnimController animController;
  protected static readonly HashedString[] ON_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("on_pre"),
    HashedString.op_Implicit("on")
  };
  protected static readonly HashedString[] OFF_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("on_pst"),
    HashedString.op_Implicit("off")
  };

  protected abstract void ConduitUpdate(float dt);

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.animController = ((Component) this).GetComponent<KBatchedAnimController>();
    this.OnToggle += new Action<bool>(this.OnSwitchToggled);
    this.UpdateLogicCircuit();
    this.UpdateVisualState(true);
    this.wasOn = this.switchedOn;
    if (this.conduitType == ConduitType.Liquid || this.conduitType == ConduitType.Gas)
      Conduit.GetFlowManager(this.conduitType).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
    else
      SolidConduit.GetFlowManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
  }

  protected virtual void OnCleanUp()
  {
    if (this.conduitType == ConduitType.Liquid || this.conduitType == ConduitType.Gas)
      Conduit.GetFlowManager(this.conduitType).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
    else
      SolidConduit.GetFlowManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
    base.OnCleanUp();
  }

  private void OnSwitchToggled(bool toggled_on)
  {
    this.UpdateLogicCircuit();
    this.UpdateVisualState();
  }

  private void UpdateLogicCircuit() => ((Component) this).GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);

  protected virtual void UpdateVisualState(bool force = false)
  {
    if (!(this.wasOn != this.switchedOn | force))
      return;
    this.wasOn = this.switchedOn;
    if (this.switchedOn)
      this.animController.Play(ConduitSensor.ON_ANIMS, (KAnim.PlayMode) 0);
    else
      this.animController.Play(ConduitSensor.OFF_ANIMS);
  }
}
