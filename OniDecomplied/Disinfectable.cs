// Decompiled with JetBrains decompiler
// Type: Disinfectable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Disinfectable")]
public class Disinfectable : Workable
{
  private Chore chore;
  [Serialize]
  private bool isMarkedForDisinfect;
  private const float MAX_WORK_TIME = 10f;
  private float diseasePerSecond;
  private static readonly EventSystem.IntraObjectHandler<Disinfectable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Disinfectable>((Action<Disinfectable, object>) ((component, data) => component.OnCancel(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
    this.faceTargetWhenWorking = true;
    this.synchronizeAnims = false;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Disinfecting;
    this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.multitoolContext = HashedString.op_Implicit("disinfect");
    this.multitoolHitEffectTag = Tag.op_Implicit("fx_disinfect_splash");
    this.Subscribe<Disinfectable>(2127324410, Disinfectable.OnCancelDelegate);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    if (this.isMarkedForDisinfect)
      this.MarkForDisinfect(true);
    this.SetWorkTime(10f);
    this.shouldTransferDiseaseWithWorker = false;
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    this.diseasePerSecond = (float) ((Component) this).GetComponent<PrimaryElement>().DiseaseCount / 10f;
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    base.OnWorkTick(worker, dt);
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    component.AddDisease(component.DiseaseIdx, -(int) ((double) this.diseasePerSecond * (double) dt + 0.5), "Disinfectable.OnWorkTick");
    return false;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    component.AddDisease(component.DiseaseIdx, -component.DiseaseCount, "Disinfectable.OnCompleteWork");
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForDisinfection, Object.op_Implicit((Object) this));
    this.isMarkedForDisinfect = false;
    this.chore = (Chore) null;
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
    Prioritizable.RemoveRef(((Component) this).gameObject);
  }

  private void ToggleMarkForDisinfect()
  {
    if (this.isMarkedForDisinfect)
    {
      this.CancelDisinfection();
    }
    else
    {
      this.SetWorkTime(10f);
      this.MarkForDisinfect();
    }
  }

  private void CancelDisinfection()
  {
    if (!this.isMarkedForDisinfect)
      return;
    Prioritizable.RemoveRef(((Component) this).gameObject);
    this.ShowProgressBar(false);
    this.isMarkedForDisinfect = false;
    this.chore.Cancel("disinfection cancelled");
    this.chore = (Chore) null;
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForDisinfection, Object.op_Implicit((Object) this));
  }

  public void MarkForDisinfect(bool force = false)
  {
    if (!(!this.isMarkedForDisinfect | force))
      return;
    this.isMarkedForDisinfect = true;
    Prioritizable.AddRef(((Component) this).gameObject);
    this.chore = (Chore) new WorkChore<Disinfectable>(Db.Get().ChoreTypes.Disinfect, (IStateMachineTarget) this, only_when_operational: false, ignore_building_assignment: true);
    ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.MarkedForDisinfection, (object) this);
  }

  private void OnCancel(object data) => this.CancelDisinfection();
}
