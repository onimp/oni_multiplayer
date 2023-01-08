// Decompiled with JetBrains decompiler
// Type: ArcadeMachineWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ArcadeMachineWorkable")]
public class ArcadeMachineWorkable : Workable, IWorkerPrioritizable
{
  public ArcadeMachine owner;
  public int basePriority = RELAXATION.PRIORITY.TIER3;
  private static string specificEffect = "PlayedArcade";
  private static string trackingEffect = "RecentlyPlayedArcade";

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetReportType(ReportManager.ReportType.PersonalTime);
    this.synchronizeAnims = false;
    this.showProgressBar = true;
    this.resetProgressOnStop = true;
    this.SetWorkTime(15f);
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    ((Component) worker).GetComponent<Effects>().Add("ArcadePlaying", false);
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    ((Component) worker).GetComponent<Effects>().Remove("ArcadePlaying");
  }

  protected override void OnCompleteWork(Worker worker)
  {
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (!string.IsNullOrEmpty(ArcadeMachineWorkable.trackingEffect))
      component.Add(ArcadeMachineWorkable.trackingEffect, true);
    if (string.IsNullOrEmpty(ArcadeMachineWorkable.specificEffect))
      return;
    component.Add(ArcadeMachineWorkable.specificEffect, true);
  }

  public bool GetWorkerPriority(Worker worker, out int priority)
  {
    priority = this.basePriority;
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (!string.IsNullOrEmpty(ArcadeMachineWorkable.trackingEffect) && component.HasEffect(ArcadeMachineWorkable.trackingEffect))
    {
      priority = 0;
      return false;
    }
    if (!string.IsNullOrEmpty(ArcadeMachineWorkable.specificEffect) && component.HasEffect(ArcadeMachineWorkable.specificEffect))
      priority = RELAXATION.PRIORITY.RECENTLY_USED;
    return true;
  }
}
