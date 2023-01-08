// Decompiled with JetBrains decompiler
// Type: JuicerWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/JuicerWorkable")]
public class JuicerWorkable : Workable, IWorkerPrioritizable
{
  [MyCmpReq]
  private Operational operational;
  public int basePriority;
  private Juicer juicer;

  private JuicerWorkable() => this.SetReportType(ReportManager.ReportType.PersonalTime);

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_juicer_kanim"))
    };
    this.showProgressBar = true;
    this.resetProgressOnStop = true;
    this.synchronizeAnims = false;
    this.SetWorkTime(30f);
    this.juicer = ((Component) this).GetComponent<Juicer>();
  }

  protected override void OnStartWork(Worker worker) => this.operational.SetActive(true);

  protected override void OnCompleteWork(Worker worker)
  {
    Storage component1 = ((Component) this).GetComponent<Storage>();
    float amount_consumed;
    SimUtil.DiseaseInfo disease_info1;
    float aggregate_temperature;
    component1.ConsumeAndGetDisease(GameTags.Water, this.juicer.waterMassPerUse, out amount_consumed, out disease_info1, out aggregate_temperature);
    GermExposureMonitor.Instance smi = ((Component) worker).GetSMI<GermExposureMonitor.Instance>();
    for (int index = 0; index < this.juicer.ingredientTags.Length; ++index)
    {
      SimUtil.DiseaseInfo disease_info2;
      component1.ConsumeAndGetDisease(this.juicer.ingredientTags[index], this.juicer.ingredientMassesPerUse[index], out amount_consumed, out disease_info2, out aggregate_temperature);
      smi?.TryInjectDisease(disease_info2.idx, disease_info2.count, this.juicer.ingredientTags[index], Sickness.InfectionVector.Digestion);
    }
    smi?.TryInjectDisease(disease_info1.idx, disease_info1.count, GameTags.Water, Sickness.InfectionVector.Digestion);
    Effects component2 = ((Component) worker).GetComponent<Effects>();
    if (!string.IsNullOrEmpty(this.juicer.specificEffect))
      component2.Add(this.juicer.specificEffect, true);
    if (string.IsNullOrEmpty(this.juicer.trackingEffect))
      return;
    component2.Add(this.juicer.trackingEffect, true);
  }

  protected override void OnStopWork(Worker worker) => this.operational.SetActive(false);

  public bool GetWorkerPriority(Worker worker, out int priority)
  {
    priority = this.basePriority;
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (!string.IsNullOrEmpty(this.juicer.trackingEffect) && component.HasEffect(this.juicer.trackingEffect))
    {
      priority = 0;
      return false;
    }
    if (!string.IsNullOrEmpty(this.juicer.specificEffect) && component.HasEffect(this.juicer.specificEffect))
      priority = RELAXATION.PRIORITY.RECENTLY_USED;
    return true;
  }
}
