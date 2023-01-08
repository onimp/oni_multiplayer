// Decompiled with JetBrains decompiler
// Type: Tinkerable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Tinkerable")]
public class Tinkerable : Workable
{
  private Chore chore;
  [MyCmpGet]
  private Storage storage;
  [MyCmpGet]
  private Effects effects;
  [MyCmpGet]
  private RoomTracker roomTracker;
  public Tag tinkerMaterialTag;
  public float tinkerMaterialAmount;
  public string addedEffect;
  public string effectAttributeId;
  public float effectMultiplier;
  public HashedString choreTypeTinker = Db.Get().ChoreTypes.PowerTinker.IdHash;
  public HashedString choreTypeFetch = Db.Get().ChoreTypes.PowerFetch.IdHash;
  private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnEffectRemovedDelegate = new EventSystem.IntraObjectHandler<Tinkerable>((Action<Tinkerable, object>) ((component, data) => component.OnEffectRemoved(data)));
  private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<Tinkerable>((Action<Tinkerable, object>) ((component, data) => component.OnStorageChange(data)));
  private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnUpdateRoomDelegate = new EventSystem.IntraObjectHandler<Tinkerable>((Action<Tinkerable, object>) ((component, data) => component.OnUpdateRoom(data)));
  private static readonly EventSystem.IntraObjectHandler<Tinkerable> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Tinkerable>((Action<Tinkerable, object>) ((component, data) => component.OnOperationalChanged(data)));
  private SchedulerHandle updateHandle;
  private bool hasReservedMaterial;

  public static Tinkerable MakePowerTinkerable(GameObject prefab)
  {
    RoomTracker roomTracker = prefab.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.PowerPlant.Id;
    roomTracker.requirement = RoomTracker.Requirement.TrackingOnly;
    Tinkerable tinkerable = prefab.AddOrGet<Tinkerable>();
    tinkerable.tinkerMaterialTag = PowerControlStationConfig.TINKER_TOOLS;
    tinkerable.tinkerMaterialAmount = 1f;
    tinkerable.requiredSkillPerk = PowerControlStationConfig.ROLE_PERK;
    tinkerable.SetWorkTime(180f);
    tinkerable.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
    tinkerable.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
    tinkerable.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    tinkerable.choreTypeTinker = Db.Get().ChoreTypes.PowerTinker.IdHash;
    tinkerable.choreTypeFetch = Db.Get().ChoreTypes.PowerFetch.IdHash;
    tinkerable.addedEffect = "PowerTinker";
    tinkerable.effectAttributeId = Db.Get().Attributes.Machinery.Id;
    tinkerable.effectMultiplier = 0.025f;
    tinkerable.multitoolContext = HashedString.op_Implicit("powertinker");
    tinkerable.multitoolHitEffectTag = Tag.op_Implicit("fx_powertinker_splash");
    tinkerable.shouldShowSkillPerkStatusItem = false;
    prefab.AddOrGet<Storage>();
    prefab.AddOrGet<Effects>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    prefab.GetComponent<KPrefabID>().prefabInitFn += Tinkerable.\u003C\u003Ec.\u003C\u003E9__0_0 ?? (Tinkerable.\u003C\u003Ec.\u003C\u003E9__0_0 = new KPrefabID.PrefabFn((object) Tinkerable.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CMakePowerTinkerable\u003Eb__0_0)));
    return tinkerable;
  }

  public static Tinkerable MakeFarmTinkerable(GameObject prefab)
  {
    RoomTracker roomTracker = prefab.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.Farm.Id;
    roomTracker.requirement = RoomTracker.Requirement.TrackingOnly;
    Tinkerable tinkerable = prefab.AddOrGet<Tinkerable>();
    tinkerable.tinkerMaterialTag = FarmStationConfig.TINKER_TOOLS;
    tinkerable.tinkerMaterialAmount = 1f;
    tinkerable.requiredSkillPerk = Db.Get().SkillPerks.CanFarmTinker.Id;
    tinkerable.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
    tinkerable.addedEffect = "FarmTinker";
    tinkerable.effectAttributeId = Db.Get().Attributes.Botanist.Id;
    tinkerable.effectMultiplier = 0.1f;
    tinkerable.SetWorkTime(15f);
    tinkerable.attributeConverter = Db.Get().AttributeConverters.PlantTendSpeed;
    tinkerable.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    tinkerable.choreTypeTinker = Db.Get().ChoreTypes.CropTend.IdHash;
    tinkerable.choreTypeFetch = Db.Get().ChoreTypes.FarmFetch.IdHash;
    tinkerable.multitoolContext = HashedString.op_Implicit("tend");
    tinkerable.multitoolHitEffectTag = Tag.op_Implicit("fx_tend_splash");
    tinkerable.shouldShowSkillPerkStatusItem = false;
    prefab.AddOrGet<Storage>();
    prefab.AddOrGet<Effects>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    prefab.GetComponent<KPrefabID>().prefabInitFn += Tinkerable.\u003C\u003Ec.\u003C\u003E9__1_0 ?? (Tinkerable.\u003C\u003Ec.\u003C\u003E9__1_0 = new KPrefabID.PrefabFn((object) Tinkerable.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CMakeFarmTinkerable\u003Eb__1_0)));
    return tinkerable;
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_use_machine_kanim"))
    };
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Tinkering;
    this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
    this.faceTargetWhenWorking = true;
    this.synchronizeAnims = false;
    this.Subscribe<Tinkerable>(-1157678353, Tinkerable.OnEffectRemovedDelegate);
    this.Subscribe<Tinkerable>(-1697596308, Tinkerable.OnStorageChangeDelegate);
    this.Subscribe<Tinkerable>(144050788, Tinkerable.OnUpdateRoomDelegate);
    this.Subscribe<Tinkerable>(-592767678, Tinkerable.OnOperationalChangedDelegate);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    Prioritizable.AddRef(((Component) this).gameObject);
  }

  protected override void OnCleanUp()
  {
    this.UpdateMaterialReservation(false);
    if (this.updateHandle.IsValid)
      this.updateHandle.ClearScheduler();
    Prioritizable.RemoveRef(((Component) this).gameObject);
    base.OnCleanUp();
  }

  private void OnOperationalChanged(object data) => this.QueueUpdateChore();

  private void OnEffectRemoved(object data) => this.QueueUpdateChore();

  private void OnUpdateRoom(object data) => this.QueueUpdateChore();

  private void OnStorageChange(object data)
  {
    if (!((GameObject) data).IsPrefabID(this.tinkerMaterialTag))
      return;
    this.QueueUpdateChore();
  }

  private void QueueUpdateChore()
  {
    if (this.updateHandle.IsValid)
      this.updateHandle.ClearScheduler();
    this.updateHandle = GameScheduler.Instance.Schedule("UpdateTinkerChore", 1.2f, new Action<object>(this.UpdateChoreCallback), (object) null, (SchedulerGroup) null);
  }

  private void UpdateChoreCallback(object obj) => this.UpdateChore();

  private void UpdateChore()
  {
    Operational component = ((Component) this).GetComponent<Operational>();
    bool flag1 = Object.op_Equality((Object) component, (Object) null) || component.IsFunctional;
    int num = this.HasEffect() ? 1 : 0;
    bool flag2 = this.RoomHasActiveTinkerstation();
    bool flag3 = num == 0 & flag2 & flag1;
    bool flag4 = num != 0 || !flag2;
    if (this.chore == null & flag3)
    {
      this.UpdateMaterialReservation(true);
      if (this.HasMaterial())
      {
        this.chore = (Chore) new WorkChore<Tinkerable>(Db.Get().ChoreTypes.GetByHash(this.choreTypeTinker), (IStateMachineTarget) this, only_when_operational: false);
        if (Object.op_Inequality((Object) component, (Object) null))
          this.chore.AddPrecondition(ChorePreconditions.instance.IsFunctional, (object) component);
      }
      else
      {
        ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.choreTypeFetch);
        Storage storage = this.storage;
        double tinkerMaterialAmount = (double) this.tinkerMaterialAmount;
        HashSet<Tag> tags = new HashSet<Tag>();
        tags.Add(this.tinkerMaterialTag);
        Tag invalid = Tag.Invalid;
        Action<Chore> on_complete = new Action<Chore>(this.OnFetchComplete);
        this.chore = (Chore) new FetchChore(byHash, storage, (float) tinkerMaterialAmount, tags, FetchChore.MatchCriteria.MatchID, invalid, on_complete: on_complete, operational_requirement: Operational.State.Functional);
      }
      this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) this.requiredSkillPerk);
      if (string.IsNullOrEmpty(((Component) this).GetComponent<RoomTracker>().requiredRoomType))
        return;
      this.chore.AddPrecondition(ChorePreconditions.instance.IsInMyRoom, (object) Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
    }
    else
    {
      if (!(this.chore != null & flag4))
        return;
      this.UpdateMaterialReservation(false);
      this.chore.Cancel("No longer needed");
      this.chore = (Chore) null;
    }
  }

  private bool RoomHasActiveTinkerstation()
  {
    if (!this.roomTracker.IsInCorrectRoom() || this.roomTracker.room == null)
      return false;
    foreach (KPrefabID building in this.roomTracker.room.buildings)
    {
      if (!Object.op_Equality((Object) building, (Object) null))
      {
        TinkerStation component = ((Component) building).GetComponent<TinkerStation>();
        if (Object.op_Inequality((Object) component, (Object) null) && Tag.op_Equality(component.outputPrefab, this.tinkerMaterialTag) && ((Component) building).GetComponent<Operational>().IsOperational)
          return true;
      }
    }
    return false;
  }

  private void UpdateMaterialReservation(bool shouldReserve)
  {
    if (shouldReserve && !this.hasReservedMaterial)
    {
      MaterialNeeds.UpdateNeed(this.tinkerMaterialTag, this.tinkerMaterialAmount, ((Component) this).gameObject.GetMyWorldId());
      this.hasReservedMaterial = shouldReserve;
    }
    else
    {
      if (shouldReserve || !this.hasReservedMaterial)
        return;
      MaterialNeeds.UpdateNeed(this.tinkerMaterialTag, -this.tinkerMaterialAmount, ((Component) this).gameObject.GetMyWorldId());
      this.hasReservedMaterial = shouldReserve;
    }
  }

  private void OnFetchComplete(Chore data)
  {
    this.UpdateMaterialReservation(false);
    this.chore = (Chore) null;
    this.UpdateChore();
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    this.storage.ConsumeIgnoringDisease(this.tinkerMaterialTag, this.tinkerMaterialAmount);
    float totalValue = worker.GetAttributes().Get(Db.Get().Attributes.Get(this.effectAttributeId)).GetTotalValue();
    this.effects.Add(this.addedEffect, true).timeRemaining *= (float) (1.0 + (double) totalValue * (double) this.effectMultiplier);
    this.UpdateMaterialReservation(false);
    this.chore = (Chore) null;
    this.UpdateChore();
  }

  private bool HasMaterial() => (double) this.storage.GetAmountAvailable(this.tinkerMaterialTag) >= (double) this.tinkerMaterialAmount;

  private bool HasEffect() => this.effects.HasEffect(this.addedEffect);
}
