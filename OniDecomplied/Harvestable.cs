// Decompiled with JetBrains decompiler
// Type: Harvestable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using TUNING;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/Workable/Harvestable")]
public class Harvestable : Workable
{
  public HarvestDesignatable harvestDesignatable;
  [Serialize]
  protected bool canBeHarvested;
  protected Chore chore;
  private static readonly EventSystem.IntraObjectHandler<Harvestable> ForceCancelHarvestDelegate = new EventSystem.IntraObjectHandler<Harvestable>((Action<Harvestable, object>) ((component, data) => component.ForceCancelHarvest(data)));
  private static readonly EventSystem.IntraObjectHandler<Harvestable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Harvestable>((Action<Harvestable, object>) ((component, data) => component.OnCancel(data)));

  public Worker completed_by { get; protected set; }

  public bool CanBeHarvested => this.canBeHarvested;

  protected Harvestable() => this.SetOffsetTable(OffsetGroups.InvertedStandardTable);

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Harvesting;
    this.multitoolContext = HashedString.op_Implicit("harvest");
    this.multitoolHitEffectTag = Tag.op_Implicit("fx_harvest_splash");
  }

  protected override void OnSpawn()
  {
    this.harvestDesignatable = ((Component) this).GetComponent<HarvestDesignatable>();
    this.Subscribe<Harvestable>(2127324410, Harvestable.ForceCancelHarvestDelegate);
    this.SetWorkTime(10f);
    this.Subscribe<Harvestable>(2127324410, Harvestable.OnCancelDelegate);
    this.faceTargetWhenWorking = true;
    Components.Harvestables.Add(this);
    this.attributeConverter = Db.Get().AttributeConverters.HarvestSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Farming.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
  }

  public void OnUprooted(object data)
  {
    if (!this.canBeHarvested)
      return;
    this.Harvest();
  }

  public void Harvest()
  {
    this.harvestDesignatable.MarkedForHarvest = false;
    this.chore = (Chore) null;
    this.Trigger(1272413801, (object) this);
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    component.RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
    component.RemoveStatusItem(Db.Get().MiscStatusItems.Operating);
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
  }

  public void OnMarkedForHarvest()
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (this.chore == null)
    {
      this.chore = (Chore) new WorkChore<Harvestable>(Db.Get().ChoreTypes.Harvest, (IStateMachineTarget) this, is_preemptable: true);
      component.AddStatusItem(Db.Get().MiscStatusItems.PendingHarvest, (object) this);
    }
    component.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest);
  }

  public void SetCanBeHarvested(bool state)
  {
    this.canBeHarvested = state;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (this.canBeHarvested)
    {
      component.AddStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest);
      if (this.harvestDesignatable.HarvestWhenReady)
        this.harvestDesignatable.MarkForHarvest();
      else if (this.harvestDesignatable.InPlanterBox)
        component.AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, (object) this);
    }
    else
    {
      component.RemoveStatusItem(Db.Get().CreatureStatusItems.ReadyForHarvest);
      component.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest);
    }
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
  }

  protected override void OnCompleteWork(Worker worker)
  {
    this.completed_by = worker;
    this.Harvest();
  }

  protected virtual void OnCancel(object data)
  {
    if (this.chore != null)
    {
      this.chore.Cancel("Cancel harvest");
      this.chore = (Chore) null;
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
      this.harvestDesignatable.SetHarvestWhenReady(false);
    }
    this.harvestDesignatable.MarkedForHarvest = false;
  }

  public bool HasChore() => this.chore != null;

  public virtual void ForceCancelHarvest(object data = null)
  {
    this.OnCancel((object) null);
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Components.Harvestables.Remove(this);
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest);
  }
}
