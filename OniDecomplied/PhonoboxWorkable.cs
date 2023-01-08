// Decompiled with JetBrains decompiler
// Type: PhonoboxWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/PhonoboxWorkable")]
public class PhonoboxWorkable : Workable, IWorkerPrioritizable
{
  public Phonobox owner;
  public int basePriority = RELAXATION.PRIORITY.TIER3;
  public string specificEffect = "Danced";
  public string trackingEffect = "RecentlyDanced";
  public KAnimFile[][] workerOverrideAnims = new KAnimFile[3][]
  {
    new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_phonobox_danceone_kanim"))
    },
    new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_phonobox_dancetwo_kanim"))
    },
    new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_phonobox_dancethree_kanim"))
    }
  };

  private PhonoboxWorkable() => this.SetReportType(ReportManager.ReportType.PersonalTime);

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.synchronizeAnims = false;
    this.showProgressBar = true;
    this.resetProgressOnStop = true;
    this.SetWorkTime(15f);
  }

  protected override void OnCompleteWork(Worker worker)
  {
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (!string.IsNullOrEmpty(this.trackingEffect))
      component.Add(this.trackingEffect, true);
    if (string.IsNullOrEmpty(this.specificEffect))
      return;
    component.Add(this.specificEffect, true);
  }

  public bool GetWorkerPriority(Worker worker, out int priority)
  {
    priority = this.basePriority;
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (!string.IsNullOrEmpty(this.trackingEffect) && component.HasEffect(this.trackingEffect))
    {
      priority = 0;
      return false;
    }
    if (!string.IsNullOrEmpty(this.specificEffect) && component.HasEffect(this.specificEffect))
      priority = RELAXATION.PRIORITY.RECENTLY_USED;
    return true;
  }

  protected override void OnStartWork(Worker worker)
  {
    this.owner.AddWorker(worker);
    ((Component) worker).GetComponent<Effects>().Add("Dancing", false);
  }

  protected override void OnStopWork(Worker worker)
  {
    this.owner.RemoveWorker(worker);
    ((Component) worker).GetComponent<Effects>().Remove("Dancing");
  }

  public override Workable.AnimInfo GetAnim(Worker worker)
  {
    this.overrideAnims = this.workerOverrideAnims[Random.Range(0, this.workerOverrideAnims.Length)];
    return base.GetAnim(worker);
  }
}
