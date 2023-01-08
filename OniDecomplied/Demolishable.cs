// Decompiled with JetBrains decompiler
// Type: Demolishable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using TUNING;
using UnityEngine;

[RequireComponent(typeof (Prioritizable))]
public class Demolishable : Workable
{
  public Chore chore;
  public bool allowDemolition = true;
  [Serialize]
  private bool isMarkedForDemolition;
  private bool destroyed;
  private static readonly EventSystem.IntraObjectHandler<Demolishable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Demolishable>((Action<Demolishable, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  private static readonly EventSystem.IntraObjectHandler<Demolishable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Demolishable>((Action<Demolishable, object>) ((component, data) => component.OnCancel(data)));
  private static readonly EventSystem.IntraObjectHandler<Demolishable> OnDeconstructDelegate = new EventSystem.IntraObjectHandler<Demolishable>((Action<Demolishable, object>) ((component, data) => component.OnDemolish(data)));

  public bool HasBeenDestroyed => this.destroyed;

  private CellOffset[] placementOffsets
  {
    get
    {
      Building component1 = ((Component) this).GetComponent<Building>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        return component1.Def.PlacementOffsets;
      OccupyArea component2 = ((Component) this).GetComponent<OccupyArea>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        return component2.OccupiedCellsOffsets;
      Debug.Assert(false, (object) "Ack! We put a Demolishable on something that's neither a Building nor OccupyArea!", (Object) this);
      return (CellOffset[]) null;
    }
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.requiredSkillPerk = Db.Get().SkillPerks.CanDemolish.Id;
    this.faceTargetWhenWorking = true;
    this.synchronizeAnims = false;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Deconstructing;
    this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
    this.minimumAttributeMultiplier = 0.75f;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
    this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
    this.multitoolContext = HashedString.op_Implicit("demolish");
    this.multitoolHitEffectTag = Tag.op_Implicit(EffectConfigs.DemolishSplashId);
    this.workingPstComplete = (HashedString[]) null;
    this.workingPstFailed = (HashedString[]) null;
    CellOffset[][] table = OffsetGroups.InvertedStandardTable;
    CellOffset[] filter = (CellOffset[]) null;
    Building component = ((Component) this).GetComponent<Building>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.Def.IsTilePiece)
    {
      table = OffsetGroups.InvertedStandardTableWithCorners;
      filter = component.Def.ConstructionOffsetFilter;
      this.SetWorkTime(component.Def.ConstructionTime * 0.5f);
    }
    else
      this.SetWorkTime(30f);
    this.SetOffsetTable(OffsetGroups.BuildReachabilityTable(this.placementOffsets, table, filter));
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<Demolishable>(493375141, Demolishable.OnRefreshUserMenuDelegate);
    this.Subscribe<Demolishable>(-111137758, Demolishable.OnRefreshUserMenuDelegate);
    this.Subscribe<Demolishable>(2127324410, Demolishable.OnCancelDelegate);
    this.Subscribe<Demolishable>(-790448070, Demolishable.OnDeconstructDelegate);
    if (!this.isMarkedForDemolition)
      return;
    this.QueueDemolition();
  }

  protected override void OnStartWork(Worker worker)
  {
    this.progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("DeconstructBar");
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDemolition);
  }

  protected override void OnCompleteWork(Worker worker) => this.TriggerDestroy();

  private void TriggerDestroy()
  {
    if (Object.op_Equality((Object) this, (Object) null) || this.destroyed)
      return;
    this.destroyed = true;
    this.isMarkedForDemolition = false;
    TracesExtesions.DeleteObject(((Component) this).gameObject);
  }

  private void QueueDemolition()
  {
    if (DebugHandler.InstantBuildMode)
    {
      this.OnCompleteWork((Worker) null);
    }
    else
    {
      if (this.chore == null)
      {
        Prioritizable.AddRef(((Component) this).gameObject);
        this.requiredSkillPerk = Db.Get().SkillPerks.CanDemolish.Id;
        this.chore = (Chore) new WorkChore<Demolishable>(Db.Get().ChoreTypes.Demolish, (IStateMachineTarget) this, only_when_operational: false, override_anims: Assets.GetAnim(HashedString.op_Implicit("anim_interacts_clothingfactory_kanim")), is_preemptable: true, ignore_building_assignment: true);
        ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingDemolition, (object) this);
        this.isMarkedForDemolition = true;
        this.Trigger(2108245096, (object) "Demolish");
      }
      this.UpdateStatusItem((object) null);
    }
  }

  private void OnRefreshUserMenu(object data)
  {
    if (!this.allowDemolition)
      return;
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, this.chore == null ? new KIconButtonMenu.ButtonInfo("action_deconstruct", (string) UI.USERMENUACTIONS.DEMOLISH.NAME, new System.Action(this.OnDemolish), tooltipText: ((string) UI.USERMENUACTIONS.DEMOLISH.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_deconstruct", (string) UI.USERMENUACTIONS.DEMOLISH.NAME_OFF, new System.Action(this.OnDemolish), tooltipText: ((string) UI.USERMENUACTIONS.DEMOLISH.TOOLTIP_OFF)), 0.0f);
  }

  public void CancelDemolition()
  {
    if (this.chore != null)
    {
      this.chore.Cancel("Cancelled demolition");
      this.chore = (Chore) null;
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDemolition);
      this.ShowProgressBar(false);
      this.isMarkedForDemolition = false;
      Prioritizable.RemoveRef(((Component) this).gameObject);
    }
    this.UpdateStatusItem((object) null);
  }

  private void OnCancel(object data) => this.CancelDemolition();

  private void OnDemolish(object data)
  {
    if (!this.allowDemolition && !DebugHandler.InstantBuildMode)
      return;
    this.QueueDemolition();
  }

  private void OnDemolish()
  {
    if (this.chore == null)
      this.QueueDemolition();
    else
      this.CancelDemolition();
  }

  protected override void UpdateStatusItem(object data = null)
  {
    this.shouldShowSkillPerkStatusItem = this.isMarkedForDemolition;
    base.UpdateStatusItem(data);
  }
}
