// Decompiled with JetBrains decompiler
// Type: AutoDisinfectable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/AutoDisinfectable")]
public class AutoDisinfectable : Workable
{
  private Chore chore;
  private const float MAX_WORK_TIME = 10f;
  private float diseasePerSecond;
  [MyCmpGet]
  private PrimaryElement primaryElement;
  [Serialize]
  private bool enableAutoDisinfect = true;
  private static readonly EventSystem.IntraObjectHandler<AutoDisinfectable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<AutoDisinfectable>((component, data) => component.OnRefreshUserMenu(data));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
    this.faceTargetWhenWorking = true;
    this.synchronizeAnims = false;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Disinfecting;
    this.resetProgressOnStop = true;
    this.multitoolContext = HashedString.op_Implicit("disinfect");
    this.multitoolHitEffectTag = Tag.op_Implicit("fx_disinfect_splash");
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<AutoDisinfectable>(493375141, AutoDisinfectable.OnRefreshUserMenuDelegate);
    this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.SetWorkTime(10f);
    this.shouldTransferDiseaseWithWorker = false;
  }

  public void CancelChore()
  {
    if (this.chore == null)
      return;
    this.chore.Cancel("AutoDisinfectable.CancelChore");
    this.chore = (Chore) null;
  }

  public void RefreshChore()
  {
    if (KMonoBehaviour.isLoadingScene)
      return;
    if (!this.enableAutoDisinfect || !SaveGame.Instance.enableAutoDisinfect)
    {
      if (this.chore == null)
        return;
      this.chore.Cancel("Autodisinfect Disabled");
      this.chore = (Chore) null;
    }
    else
    {
      if (this.chore != null && Object.op_Inequality((Object) this.chore.driver, (Object) null))
        return;
      int diseaseCount = this.primaryElement.DiseaseCount;
      if (this.chore == null && diseaseCount > SaveGame.Instance.minGermCountForDisinfect)
      {
        this.chore = (Chore) new WorkChore<AutoDisinfectable>(Db.Get().ChoreTypes.Disinfect, (IStateMachineTarget) this, only_when_operational: false, ignore_building_assignment: true);
      }
      else
      {
        if (diseaseCount >= SaveGame.Instance.minGermCountForDisinfect || this.chore == null)
          return;
        this.chore.Cancel("AutoDisinfectable.Update");
        this.chore = (Chore) null;
      }
    }
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
    this.chore = (Chore) null;
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
  }

  private void EnableAutoDisinfect()
  {
    this.enableAutoDisinfect = true;
    this.RefreshChore();
  }

  private void DisableAutoDisinfect()
  {
    this.enableAutoDisinfect = false;
    this.RefreshChore();
  }

  private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(((Component) this).gameObject, this.enableAutoDisinfect ? new KIconButtonMenu.ButtonInfo("action_disinfect", (string) STRINGS.BUILDINGS.AUTODISINFECTABLE.DISABLE_AUTODISINFECT.NAME, new System.Action(this.DisableAutoDisinfect), tooltipText: ((string) STRINGS.BUILDINGS.AUTODISINFECTABLE.DISABLE_AUTODISINFECT.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_disinfect", (string) STRINGS.BUILDINGS.AUTODISINFECTABLE.ENABLE_AUTODISINFECT.NAME, new System.Action(this.EnableAutoDisinfect), tooltipText: ((string) STRINGS.BUILDINGS.AUTODISINFECTABLE.ENABLE_AUTODISINFECT.TOOLTIP)), 10f);
}
