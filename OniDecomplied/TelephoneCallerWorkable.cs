// Decompiled with JetBrains decompiler
// Type: TelephoneCallerWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/TelephoneWorkable")]
public class TelephoneCallerWorkable : Workable, IWorkerPrioritizable
{
  [MyCmpReq]
  private Operational operational;
  public int basePriority;
  private Telephone telephone;

  private TelephoneCallerWorkable()
  {
    this.SetReportType(ReportManager.ReportType.PersonalTime);
    this.workingPstComplete = new HashedString[1]
    {
      HashedString.op_Implicit("on_pst")
    };
    this.workAnims = new HashedString[6]
    {
      HashedString.op_Implicit("on_pre"),
      HashedString.op_Implicit("on"),
      HashedString.op_Implicit("on_receiving"),
      HashedString.op_Implicit("on_pre_loop_receiving"),
      HashedString.op_Implicit("on_loop"),
      HashedString.op_Implicit("on_loop_pre")
    };
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_telephone_kanim"))
    };
    this.showProgressBar = true;
    this.resetProgressOnStop = true;
    this.synchronizeAnims = true;
    this.SetWorkTime(40f);
    this.telephone = ((Component) this).GetComponent<Telephone>();
  }

  protected override void OnStartWork(Worker worker)
  {
    this.operational.SetActive(true);
    this.telephone.isInUse = true;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (((Component) this.telephone).HasTag(GameTags.LongDistanceCall))
    {
      if (!string.IsNullOrEmpty(this.telephone.longDistanceEffect))
        component.Add(this.telephone.longDistanceEffect, true);
    }
    else if (this.telephone.wasAnswered)
    {
      if (!string.IsNullOrEmpty(this.telephone.chatEffect))
        component.Add(this.telephone.chatEffect, true);
    }
    else if (!string.IsNullOrEmpty(this.telephone.babbleEffect))
      component.Add(this.telephone.babbleEffect, true);
    if (string.IsNullOrEmpty(this.telephone.trackingEffect))
      return;
    component.Add(this.telephone.trackingEffect, true);
  }

  protected override void OnStopWork(Worker worker)
  {
    this.operational.SetActive(false);
    this.telephone.HangUp();
  }

  public bool GetWorkerPriority(Worker worker, out int priority)
  {
    priority = this.basePriority;
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (!string.IsNullOrEmpty(this.telephone.trackingEffect) && component.HasEffect(this.telephone.trackingEffect))
    {
      priority = 0;
      return false;
    }
    if (!string.IsNullOrEmpty(this.telephone.chatEffect) && component.HasEffect(this.telephone.chatEffect))
      priority = RELAXATION.PRIORITY.RECENTLY_USED;
    if (!string.IsNullOrEmpty(this.telephone.babbleEffect) && component.HasEffect(this.telephone.babbleEffect))
      priority = RELAXATION.PRIORITY.RECENTLY_USED;
    return true;
  }
}
