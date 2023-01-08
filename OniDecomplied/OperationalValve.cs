// Decompiled with JetBrains decompiler
// Type: OperationalValve
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;

[SerializationConfig]
public class OperationalValve : ValveBase
{
  [MyCmpReq]
  private Operational operational;
  private bool isDispensing;
  private static readonly EventSystem.IntraObjectHandler<OperationalValve> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<OperationalValve>((Action<OperationalValve, object>) ((component, data) => component.OnOperationalChanged(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<OperationalValve>(-592767678, OperationalValve.OnOperationalChangedDelegate);
  }

  protected override void OnSpawn()
  {
    this.OnOperationalChanged((object) this.operational.IsOperational);
    base.OnSpawn();
  }

  protected override void OnCleanUp()
  {
    this.Unsubscribe<OperationalValve>(-592767678, OperationalValve.OnOperationalChangedDelegate, false);
    base.OnCleanUp();
  }

  private void OnOperationalChanged(object data)
  {
    bool flag = (bool) data;
    if (flag)
      this.CurrentFlow = this.MaxFlow;
    else
      this.CurrentFlow = 0.0f;
    this.operational.SetActive(flag);
  }

  protected override void OnMassTransfer(float amount) => this.isDispensing = (double) amount > 0.0;

  public override void UpdateAnim()
  {
    if (this.operational.IsOperational)
    {
      if (this.isDispensing)
        this.controller.Queue(HashedString.op_Implicit("on_flow"), (KAnim.PlayMode) 0);
      else
        this.controller.Queue(HashedString.op_Implicit("on"));
    }
    else
      this.controller.Queue(HashedString.op_Implicit("off"));
  }
}
