// Decompiled with JetBrains decompiler
// Type: VerticalWindTunnelWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/VerticalWindTunnelWorkable")]
public class VerticalWindTunnelWorkable : Workable, IWorkerPrioritizable
{
  public VerticalWindTunnel windTunnel;
  public HashedString overrideAnim;
  public string[] preAnims;
  public string loopAnim;
  public string[] pstAnims;

  private VerticalWindTunnelWorkable() => this.SetReportType(ReportManager.ReportType.PersonalTime);

  public override Workable.AnimInfo GetAnim(Worker worker) => base.GetAnim(worker) with
  {
    smi = (StateMachine.Instance) new WindTunnelWorkerStateMachine.StatesInstance(worker, this)
  };

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.synchronizeAnims = false;
    this.showProgressBar = true;
    this.resetProgressOnStop = true;
    this.SetWorkTime(90f);
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    ((Component) worker).GetComponent<Effects>().Add("VerticalWindTunnelFlying", false);
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    ((Component) worker).GetComponent<Effects>().Remove("VerticalWindTunnelFlying");
  }

  protected override void OnCompleteWork(Worker worker)
  {
    Effects component = ((Component) worker).GetComponent<Effects>();
    component.Add(this.windTunnel.trackingEffect, true);
    component.Add(this.windTunnel.specificEffect, true);
  }

  public bool GetWorkerPriority(Worker worker, out int priority)
  {
    priority = this.windTunnel.basePriority;
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (component.HasEffect(this.windTunnel.trackingEffect))
    {
      priority = 0;
      return false;
    }
    if (component.HasEffect(this.windTunnel.specificEffect))
      priority = RELAXATION.PRIORITY.RECENTLY_USED;
    return true;
  }
}
