// Decompiled with JetBrains decompiler
// Type: HotTubWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/HotTubWorkable")]
public class HotTubWorkable : Workable, IWorkerPrioritizable
{
  public HotTub hotTub;
  private bool faceLeft;

  private HotTubWorkable() => this.SetReportType(ReportManager.ReportType.PersonalTime);

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.synchronizeAnims = false;
    this.showProgressBar = true;
    this.resetProgressOnStop = true;
    this.faceTargetWhenWorking = true;
    this.SetWorkTime(90f);
  }

  public override Workable.AnimInfo GetAnim(Worker worker) => base.GetAnim(worker) with
  {
    smi = (StateMachine.Instance) new HotTubWorkerStateMachine.StatesInstance(worker)
  };

  protected override void OnStartWork(Worker worker)
  {
    this.faceLeft = (double) Random.value > 0.5;
    ((Component) worker).GetComponent<Effects>().Add("HotTubRelaxing", false);
  }

  protected override void OnStopWork(Worker worker) => ((Component) worker).GetComponent<Effects>().Remove("HotTubRelaxing");

  public override Vector3 GetFacingTarget() => Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), this.faceLeft ? Vector3.left : Vector3.right);

  protected override void OnCompleteWork(Worker worker)
  {
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (!string.IsNullOrEmpty(this.hotTub.trackingEffect))
      component.Add(this.hotTub.trackingEffect, true);
    if (string.IsNullOrEmpty(this.hotTub.specificEffect))
      return;
    component.Add(this.hotTub.specificEffect, true);
  }

  public bool GetWorkerPriority(Worker worker, out int priority)
  {
    priority = this.hotTub.basePriority;
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (!string.IsNullOrEmpty(this.hotTub.trackingEffect) && component.HasEffect(this.hotTub.trackingEffect))
    {
      priority = 0;
      return false;
    }
    if (!string.IsNullOrEmpty(this.hotTub.specificEffect) && component.HasEffect(this.hotTub.specificEffect))
      priority = RELAXATION.PRIORITY.RECENTLY_USED;
    return true;
  }
}
